using System.Data.Common;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;

namespace AF.DATA;

/// <summary>
/// Basisklasse für Verbindungsklassen, die die 
/// gemeinsamen Eigenschaften und Methoden für alle diese Klassen kapseln.
/// </summary>
// ReSharper disable once UnusedTypeParameter
public class Connection<TConnection, TCommand, TParameter, TTransaction> : IConnection 
    where TConnection : DbConnection, new() 
    where TCommand : DbCommand, new() 
    where TParameter : DbParameter, new() 
    where TTransaction : DbTransaction
{
    private readonly List<Tuple<IDataObject, eHubEventType>> _msgBuffer = [];
    private readonly object msgBufferLock = new();

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="database">Datenbank für diese Verbindung</param>
    public Connection(IDatabase database)
    {
        Database = database;
    }

    /// <summary>
    /// Datenbank für diese Verbindung
    /// </summary>
    public IDatabase Database { get; init; }

    /// <summary>
    /// Zugriff auf die aktuelle innere Verbindung
    /// </summary>
    public DbConnection? CurrentConnection { get; set; }

    /// <summary>
    /// Ereignisse via Core.MessageHub unterdrücken
    /// </summary>
    public bool Silent { get; set; } = false;

    #region Transactions
    /// <summary>
    /// Zugriff auf die aktuelle Transaktion
    /// </summary>
    public TTransaction? Transaction { get; set; }

    /// <summary>
    /// Erzeugt eine neue Transaktion...
    /// </summary>
    public virtual void BeginTransaction()
    {
        if (CurrentConnection == null)
            throw new AFDataException(CoreStrings.ERROR_NOACTIVECONNECTION, this);

        if (Transaction != null)
            throw new AFDataException(CoreStrings.ERROR_DBTRANSACTIONEXISTS, this);

        Database.Logger?.BeginTransaction();

        Transaction = (TTransaction)CurrentConnection.BeginTransaction();
    }

    /// <summary>
    /// Änderungen für eine bestehende Transaktion festschreiben
    /// </summary>
    public void CommitTransaction()
    {
        if (CurrentConnection == null)
            throw new AFDataException(CoreStrings.ERROR_NOACTIVECONNECTION, this);

        if (Transaction == null)
            throw new AFDataException(CoreStrings.ERROR_NOACTIVETRANSACTION, this);

        try
        {
            Transaction.Commit();
            Transaction.Dispose();

            lock (msgBufferLock)
            {
                foreach (var msg in _msgBuffer) AFCore.App.EventHub.Deliver(msg.Item1, msg.Item2);
                _msgBuffer.Clear();
            } 

            Transaction = null;

            Database.Logger?.CommitTransaction();
        }
        catch (Exception ex)
        {
            throw new AFDataException(CoreStrings.ERROR_COMMITTRANSACTION, ex, this);
        }
    }

    /// <summary>
    /// Rollback von Änderungen für eine bestehende Transaktion
    /// </summary>
    public void RollbackTransaction()
    {
        if (CurrentConnection == null)
            throw new AFDataException(CoreStrings.ERROR_NOACTIVECONNECTION, this);

        if (Transaction == null)
            throw new AFDataException(CoreStrings.ERROR_NOACTIVETRANSACTION, this);

        try
        {
            Transaction.Rollback();
            Transaction.Dispose();

            lock (msgBufferLock)
            {
                _msgBuffer.Clear();
            }

            Transaction = null;

            Database.Logger?.RollbackTransaction();
        }
        catch (Exception ex)
        {
            throw new AFDataException(CoreStrings.ERROR_COMMITTRANSACTION, ex, this);
        }
    }
    #endregion

    #region execute commands or procedures
    /// <summary>
    /// Führt eine gespeicherte Prozedur in der Datenbank aus, die genau einen Wert zurückgibt.
    /// </summary>
    /// <typeparam name="T">Rückgabetyp</typeparam>
    /// <param name="procedureName">Name der Prozedur</param>
    /// <param name="args">Argumente für die Prozedur</param>
    /// <returns>Wert</returns>
    public T? ExecuteProcedureScalar<T>(string procedureName, params object[]? args)
    {
        T? ret = default;

        var qry = _getCommand(eCommandString.ExecProcedure).Replace(@"#PROCEDURE#", Database.GetName(procedureName)).ToString();
        qry += args != null && args.Length > 0 
            ? (@"(" + @"#".PadRight(args.Length, '#').Replace(@"#", @"?,") + @")").Replace(@"?,)", @"?)") 
            : "";

        using var cmd = _parseCommand(null, qry, args);
        
        cmd.CommandType = CommandType.StoredProcedure;
        Database.TraceBeforeExecute?.Invoke(new TraceInfo(cmd.CommandText));
        var data = Database.Translator.FromDatabase(cmd.ExecuteScalar(), typeof(T));

        if (data is T d)
            ret = d;

        Database.TraceAfterExecute?.Invoke(new TraceInfo(cmd.CommandText));

        Database.LastQuery = cmd.CommandText;

        return ret;
    }

    /// <summary>
    /// Führt eine gespeicherte Prozedur in der Datenbank aus.
    /// </summary>
    /// <param name="procedureName">Name der Prozedur</param>
    /// <param name="args">Argumente für die Prozedur</param>
    /// <returns>Anzahl der betroffenen Zeilen</returns>
    public int ExecuteProcedure(string procedureName, params object[]? args)
    {
        var qry = _getCommand(eCommandString.ExecProcedure).Replace(@"#PROCEDURE#", Database.GetName(procedureName)).ToString();
        qry += args != null && args.Length > 0 
            ? (@"(" + @"#".PadRight(args.Length, '#').Replace(@"#", @"?,") + @")").Replace(@"?,)", @"?)") 
            : "";

        using var cmd = _parseCommand(null, qry, args);
        cmd.CommandType = CommandType.StoredProcedure;
        Database.TraceBeforeExecute?.Invoke(new TraceInfo(cmd.CommandText));
        var ret = cmd.ExecuteNonQuery();
        Database.TraceAfterExecute?.Invoke(new TraceInfo(cmd.CommandText));

        Database.LastQuery = cmd.CommandText;

        return ret;
    }

    /// <summary>
    /// Führt einen Sql-Befehl aus, der genau einen Wert zurückgibt.
    /// </summary>
    /// <typeparam name="T">Rückgabetyp</typeparam>
    /// <param name="commandQuery">Abfrage/Befehl</param>
    /// <param name="args">Argumente für den Befehl</param>
    /// <returns>Wert</returns>
    public T? ExecuteCommandScalar<T>(string commandQuery, params object[]? args)
    {
        T? ret = default;

        using (var cmd = _parseCommand(null, commandQuery, args))
        {
            cmd.CommandType = CommandType.Text;
            Database.TraceBeforeExecute?.Invoke(new TraceInfo(cmd.CommandText));

            var data = Database.Translator.FromDatabase(cmd.ExecuteScalar(), typeof(T));

            if (data is T d)
                ret = d;

            Database.TraceAfterExecute?.Invoke(new TraceInfo(cmd.CommandText));

            Database.LastQuery = cmd.CommandText;
        }

        return ret;
    }

    /// <summary>
    /// Führt einen Befehl/eine Abfrage in der Datenbank aus.
    /// </summary>
    /// <param name="commandQuery">Abfrage/Befehl</param>
    /// <param name="args">Argumente für den Befehl</param>
    /// <returns>Anzahl der betroffenen Zeilen</returns>
    public int ExecuteCommand(string commandQuery, params object[]? args)
    {
        var ret = 0;

        if (commandQuery.IsEmpty())
            return ret;

        using var cmd = _parseCommand(null, commandQuery, args);

        cmd.CommandType = CommandType.Text;
        Database.TraceBeforeExecute?.Invoke(new TraceInfo(cmd.CommandText));
        ret = cmd.ExecuteNonQuery();
        Database.TraceAfterExecute?.Invoke(new TraceInfo(cmd.CommandText));

        Database.LastQuery = cmd.CommandText;

        return ret;
    }
    #endregion

    #region create/modify/delete data
    /// <summary>
    /// Speichern eines Datenobjekts vom Typ T
    /// </summary>
    /// <typeparam name="T">Typ des zu speichernden Objekts</typeparam>
    /// <param name="data">Objekt</param>
    /// <param name="options">Abfrageoptionen</param>
    /// <param name="forcecreate">Datensatzerstellung erzwingen (immer einen neuen Datensatz erstellen)</param>
    /// <param name="writeallfields">Schreibe alle Felder, standardmäßig werden nur geänderte Felder geschrieben, wenn der Datensatz nicht neu ist (= bei Update).</param>
    public bool Save<T>(ReadOptions options, T data, bool forcecreate = false, bool writeallfields = false) where T : class, ITable
    {
        data.BeforeSave();

        var isNew = false;
        List<TParameter> para = [];

        var tdesc = data.GetType().GetTypeDescription();

        if (tdesc.Table == null)
            throw new Exception(string.Format(CoreStrings.ERR_TYPEISNOTTABLE, tdesc.Name));

        if (tdesc.FieldKey == null)
            throw new Exception(string.Format(CoreStrings.ERR_TYPE_DOES_NOT_CONTAIN_KEY, tdesc.Name));

        var keyvalue = (Guid)tdesc.Accessor[data, ((PropertyInfo)tdesc.FieldKey).Name];

        StringBuilder? query;

        if (forcecreate || keyvalue.IsEmpty() || Exist<T>(keyvalue) == false)
        {
            query = _getCommand(eCommandString.Insert);
            isNew = true;
        }
        else
        {
             query = _getCommand(eCommandString.Update);

            if (Database.Configuration.ConflictMode == eConflictMode.FirstWins && tdesc.FieldChanged != null)
            {
                var lastchanged = LoadValue<DateTime>(typeof(T), keyvalue, tdesc.FieldChanged.Name);

                if (lastchanged > (DateTime)tdesc.Accessor[data, ((PropertyInfo)tdesc.FieldChanged).Name])
                    throw new AFDataException(CoreStrings.ERR_RECORDCHANGED, this);
            }

            para.Add(new TParameter { ParameterName = @"@v0", Value = Database.Translator.ToDatabase(keyvalue, typeof(Guid)) });
        }

        // save only changed fields if record is not new and writing all is not forced... 
        if (options.Fields.Length < 1 && !writeallfields && !isNew)
        {
            options.Fields = data.ChangedProperties.Keys.ToArray();

            var allways = tdesc.Properties.Values.Where(p => options.Fields.Contains(p.Name) == false && p.Field != null && p.Field?.SaveAllways == DevExpress.Utils.DefaultBoolean.True).Select(p => p.Name).ToArray();

            if (allways.Any())
            {
                string[] newfields = new string[options.Fields.Length + allways.Length];
                Array.Copy(options.Fields, newfields, options.Fields.Length);
                Array.Copy(allways, 0, newfields, options.Fields.Length, allways.Length);

                options.Fields = newfields;
            }
                        
            if (options.Fields.Length < 1)
                return true; // nothing to save, directly return true
        }

        var cnt = 1;

        StringBuilder sbFields = StringBuilderPool.GetStringBuilder();
        StringBuilder sbValues = StringBuilderPool.GetStringBuilder();
        StringBuilder sbPairs = StringBuilderPool.GetStringBuilder();


        foreach (var desc in tdesc.Fields.Values)
        {
            if (desc.Field == null)
                continue;

            if ((desc.Field.SystemFieldFlag == eSystemFieldFlag.TimestampChanged || desc.Field.SystemFieldFlag == eSystemFieldFlag.TimestampCreated) && forcecreate == false)
                continue;

            if (desc.Field.Delayed != data.IsDelayedLoaded(((PropertyInfo)desc).Name))
                continue;

            if (desc.Field.SystemFieldFlag != eSystemFieldFlag.PrimaryKey && options.Fields.Length > 0 && options.Fields.Contains(((PropertyInfo)desc).Name) == false)
                continue;

            if (isNew == false && desc.Field.SystemFieldFlag == eSystemFieldFlag.PrimaryKey)
                continue;

            var varname = @"@v" + cnt.ToString().Trim();

            if (cnt > 1)
            {
                sbFields.Append(@", ");
                sbValues.Append(@", ");
                sbPairs.Append(@", ");
            }

            sbFields.Append(((PropertyInfo)desc).Name);
            sbValues.Append(varname);
            sbPairs.Append(((PropertyInfo)desc).Name);
            sbPairs.Append(@" = ");
            sbPairs.Append(varname);

            TParameter paramater = new()
            {
                ParameterName = varname
            };


            if (desc.Field.SystemFieldFlag == eSystemFieldFlag.PrimaryKey && keyvalue.IsEmpty())
            {
                var newkey = Guid.NewGuid();
                tdesc.Accessor[data, ((PropertyInfo)desc).Name] = newkey;
                paramater.Value = Database.Translator.ToDatabase(newkey, typeof(Guid));
            }
            else
                paramater.Value = Database.Translator.ToDatabase(tdesc.Accessor[data, ((PropertyInfo)desc).Name], ((PropertyInfo)desc).PropertyType);

#if (NET481_OR_GREATER)
            Database.Translator.UpdateParameter(paramater, ((PropertyInfo)desc).PropertyType);
#endif

            para.Add(paramater);

            ++cnt;
        }

        if (sbFields.Length < 1) // es muss nichts gespeichert werden...
            return true;

        query.Replace(@"#TABLENAME#", tdesc.Table.TableName)
            .Replace(@"#FIELDNAMEKEY#", ((PropertyInfo)tdesc.FieldKey).Name)
            .Replace(@"#FIELDS#", sbFields.ToString())
            .Replace(@"#VALUES#", sbValues.ToString())
            .Replace(@"#PAIRS#", sbPairs.ToString());

        StringBuilderPool.ReturnStringBuilder(sbFields);
        StringBuilderPool.ReturnStringBuilder(sbValues);
        StringBuilderPool.ReturnStringBuilder(sbPairs);

        var cmd = _parseCommand(options, query.ToString(), null);

        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddRange(para.ToArray());

        Database.TraceBeforeExecute?.Invoke(new TraceInfo(cmd.CommandText));

        var ret = (cmd.ExecuteNonQuery() > 0);

        if (Silent != true && tdesc.Table.LogChanges && data.ChangedProperties.Count > 0 && Database.Logger != null)
        {
            ChangeInformation info = new(isNew ? eLoggerOperation.Create : eLoggerOperation.Update, data.PrimaryKey);

            foreach (var prop in data.ChangedProperties)
            {
                var property = tdesc.Properties[prop.Key];

                if (property.Field == null || !property.Field.LogChanges) continue;

                info.Fields.Add(
                    new(((PropertyInfo)property).Name,
                    prop.Value.OldValue,
                    prop.Value.NewValue));
            }

            Database.Logger.Log(info);
        }

        // commmit changes of the object to reset the changed fields (after all fields are 'unchanged')
        data.CommitChanges();

        Database.TraceAfterExecute?.Invoke(new TraceInfo(cmd.CommandText));
        Database.AfterSave?.Invoke(data);

        Database.LastQuery = cmd.CommandText;


        if (Silent) return ret;

        if (Transaction == null)
            AFCore.App.EventHub.Deliver(data, isNew ? eHubEventType.ObjectAdded : eHubEventType.ObjectChanged);
        else
        {
            lock (msgBufferLock)
            {
                _msgBuffer.Add(new Tuple<IDataObject, eHubEventType>(data, isNew ? eHubEventType.ObjectAdded : eHubEventType.ObjectChanged));
            }
        }

        return ret;
    }

    /// <summary>
    /// Speichern einer Liste von Datenobjekten des Typs T
    /// </summary>
    /// <typeparam name="T">Typ des zu speichernden Objekts</typeparam>
    /// <param name="data">Liste der Objekte</param>
    /// <param name="fields">nur diese Felder speichern</param>
    public int Save<T>(IEnumerable<T> data, string[]? fields = null) where T : class, ITable
    {
        ReadOptions options = fields != null ? new() { Fields = fields } : new();

        return Save(options, data);
    }

    /// <summary>
    /// Speichern einer Liste von Datenobjekten des Typs T
    /// </summary>
    /// <typeparam name="T">Typ des zu speichernden Objekts</typeparam>
    /// <param name="data">Liste der Objekte</param>
    /// <param name="options">Abfrageoptionen</param>
    public int Save<T>(ReadOptions options, IEnumerable<T> data) where T : class, ITable
    {
        var ret = 0;

        foreach (var entry in data)
        {
            if (Save(options, entry))
                ++ret;
        }

        return ret;
    }

    /// <summary>
    /// Speichern einer Liste von Datenobjekten des Typs T
    /// </summary>
    /// <typeparam name="T">Typ des zu speichernden Objekts</typeparam>
    /// <param name="data">Objekt</param>
    /// <param name="fields">nur diese Felder speichern</param>
    public bool Save<T>(T data, string[]? fields = null) where T : class, ITable
    {

        ReadOptions options = fields != null ? new() { Fields = fields } : new();
        return Save(options, data);
    }

    /// <summary>
    /// löscht ein Datenobjekt vom Typ T
    /// </summary>
    /// <typeparam name="T">Typ des zu speichernden Objekts</typeparam>
    /// <param name="data">zu löschendes Objekt</param>
    public bool Delete<T>(T data) where T : class, ITable
    {
        var ret = false;
        var tdesc = data.GetType().GetTypeDescription();

        if (tdesc.Table == null)
            throw new Exception(string.Format(CoreStrings.ERR_TYPEISNOTTABLE, tdesc.Name));

        if (tdesc.FieldKey == null)
            throw new Exception(string.Format(CoreStrings.ERR_TYPE_DOES_NOT_CONTAIN_KEY, tdesc.Name));

        var keyvalue = (Guid)tdesc.Accessor[data, ((PropertyInfo)tdesc.FieldKey).Name];

        if (ExecuteCommand(_getCommand(eCommandString.Delete)
            .Replace(@"#TABLENAME#", tdesc.Table.TableName)
            .Replace(@"#FIELDNAMEKEY#", tdesc.FieldKey.Name).ToString(), keyvalue) == 1)

            ret = true;

        Database.AfterDelete?.Invoke(data);

        if (Silent) return ret;

        if (tdesc.Table.LogChanges && Database.Logger != null)
        {
            ChangeInformation info = new(eLoggerOperation.Delete, data.PrimaryKey);
            Database.Logger.Log(info);
        }

        if (Transaction == null)
            AFCore.App.EventHub.Deliver(data, eHubEventType.ObjectDeleted);
        else
        {
            lock (msgBufferLock)
            {
                _msgBuffer.Add(new Tuple<IDataObject, eHubEventType>(data, eHubEventType.ObjectDeleted));
            }
        }

        return ret;
    }

    /// <summary>
    /// löscht ein Datenobjekt vom Typ T mit der angegebenen id
    /// </summary>
    /// <typeparam name="T">Typ des zu speichernden Objekts</typeparam>
    /// <param name="id">id des zu löschenden Objekts</param>
    public bool Delete<T>(Guid id) where T : class, ITable, new()
    {
        var data = Load<T>(id);

        return data == null
            ? throw new ArgumentException(string.Format(CoreStrings.ERR_NORECORDWITHID, id), nameof(id))
            : Delete(data);
    }

    /// <summary>
    /// löscht eine Liste von Datenobjekten des Typs T
    /// </summary>
    /// <typeparam name="T">Typ des zu speichernden Objekts</typeparam>
    /// <param name="data">Liste der zu löschenden Objekte</param>
    public int Delete<T>(IEnumerable<T> data) where T : class, ITable
    {
        var ret = 0;

        foreach (var entry in data)
        {
            if (Delete(entry))
                ++ret;
        }

        return ret;
    }

    /// <summary>
    /// Mehrere Objekte aus der Tabelle löschen
    /// </summary>
    /// <typeparam name="T">Typ der Objekte</typeparam>
    /// <param name="query">WHERE-Klausel zum Löschen. Verwenden Sie ? als Platzhalter für einen Parameter/Argument.
    /// Die Klausel kann nur die WHERE-Klausel der Abfrage enthalten</param>
    /// <param name="args">Argumente/Parameter für die WHERE-Klausel</param>
    /// <returns>Anzahl der gelöschten Datensätze</returns>
    public int Delete<T>(string query, params object[]? args) where T : ITable
    {
        using var cmd = _createQuery(typeof(T).GetTypeDescription(), new(), query, eQueryType.Delete, args);

        Database.TraceBeforeExecute?.Invoke(new TraceInfo(cmd.CommandText));
        cmd.CommandType = CommandType.Text;
        var records = cmd.ExecuteScalar();

        Database.TraceAfterExecute?.Invoke(new TraceInfo(cmd.CommandText));

        Database.LastQuery = cmd.CommandText;

        if (records is int ret)
            return ret;

        return 0;
    }
    #endregion

    #region read/select

    /// <summary>
    /// Wählt ein einzelnes/das erste Objekt aus einer Tabelle oder Ansicht aus und gibt den IModelLink des Objekts zurück.
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>
    /// <param name="query">WHERE-Klausel zu lesen. ? als Platzhalter für einen Parameter/Argument verwenden.
    /// Die Klausel kann nur die WHERE-Klausel der Abfrage oder eine komplette SQL-Abfrage enthalten
    /// (einschließlich SELECT und mehr)</param>
    /// <param name="args">Argumente/Parameter für die WHERE-Klausel</param>
    /// <returns>das gelesene Objekt oder NULL, wenn nichts gefunden wurde</returns>
    public ModelInfo? SelectSingleInfo<T>(string query, params object[]? args) where T : class, IDataObject, new()
    {
        return SelectSingleInfo<T>(new ReadOptions { MaximumRecordCount = 1 }, query, args);
    }

    /// <summary>
    /// wählt ein einzelnes (das erste tah entspricht der Abfrage) Datenobjekt vom Typ T
    /// </summary>
    /// <typeparam name="T">Typ des Datenobjekts</typeparam>
    /// <param name="options">Abfrageoptionen</param>
    /// <param name="query">Abfragezeichenfolge</param>
    /// <param name="args">Argumente für die Abfrage (Parameter)</param>
    /// <returns>Datenobjekt oder default(T)</returns>
    public T? SelectSingle<T>(ReadOptions options, string query, params object[]? args) where T : class, IDataObject, new()
    {
        return SelectSingle<T>(null, options, query, args);
    }

    /// <summary>
    /// Wählt ein einzelnes/das erste Objekt aus einer Tabelle oder Ansicht aus und gibt den IModelLink des Objekts zurück.
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>
    /// <param name="options">Optionen zum Lesen</param>
    /// <param name="where">WHERE-Klausel zum Lesen. ? als Platzhalter für einen Parameter/Argument verwenden.
    /// Die Klausel kann nur die WHERE-Klausel der Abfrage oder eine komplette SQL-Abfrage enthalten
    /// (einschließlich SELECT und mehr)</param>
    /// <param name="args">Argumente/Parameter für die WHERE-Klausel</param>
    /// <returns>das gelesene Objekt oder NULL, wenn nichts gefunden wurde</returns>
    public ModelInfo? SelectSingleInfo<T>(ReadOptions options, string where, params object[]? args) where T : class, IDataObject, new()
    {
        ModelInfo? ret = null;

        // PostgreSQL Anomalie bei Order DESC und Index = ASC
        options.MaximumRecordCount = options.OrderMode == eOrderMode.Descending && Database.DatabaseType == eDatabaseType.PostgreSql ? 10 : 1;

        var tdesc = typeof(T).GetTypeDescription();

        if (tdesc.ModelInfoFiels.Count < 1 || tdesc.ModelInfoCaptionField == null || tdesc.ModelKeyField == null)
            throw new ArgumentException($"Der Type {typeof(T).FullName} unterstützt ModelInfo nicht (ModelInfoFields, ModelInfoCaption und ModelKeyField müssen vorhanden sein).");

        options.Fields = new string[tdesc.ModelInfoFiels.Count + 2];
        int pos = 0;
        foreach (var f in tdesc.ModelInfoFiels)
        {
            options.Fields[pos] = f.Key;
            ++pos;
        }
        options.Fields[pos] = tdesc.ModelInfoCaptionField!.Name;
        options.Fields[pos + 1] = tdesc.ModelKeyField!.Name;


        using var cmd = _createQuery(tdesc, options, where, eQueryType.Select, args);

        Database.TraceBeforeExecute?.Invoke(new TraceInfo(cmd.CommandText));

        using var reader = GetReader(cmd);

        if (reader == null)
            throw new Exception(string.Format(CoreStrings.ERR_GETREADER_NOT_AVAILABLE));

        var cols = reader.FieldCount;
        var dict = new PropertyDescription[cols];

        for (var i = 0; i < cols; i++)
        {
            var fieldname = reader.GetName(i).ToLowerInvariant();
            var prop = tdesc.Fields.Values.FirstOrDefault(f => f.Name.Equals(fieldname, StringComparison.OrdinalIgnoreCase));

            if (prop != null)
                dict[i] = prop;
        }

        if (reader.Read())
            ret =  ReadModelInfoFromReader<T>(reader, dict, tdesc);

        Database.TraceAfterExecute?.Invoke(new TraceInfo(cmd.CommandText));

        Database.LastQuery = cmd.CommandText;

        return ret;
    }

    /// <summary>
    /// wählt ein einzelnes (das erste tah entspricht der Abfrage) Datenobjekt vom Typ T
    /// </summary>
    /// <typeparam name="T">Typ des Datenobjekts</typeparam>
    /// <param name="into">Einlesen in dieses Objekt, statt neues zu erzeugen</param>
    /// <param name="options">Abfrageoptionen</param>
    /// <param name="query">Abfragezeichenfolge</param>
    /// <param name="args">Argumente für die Abfrage (Parameter)</param>
    /// <returns>Datenobjekt oder default(T)</returns>
    public T? SelectSingle<T>(T? into, ReadOptions options, string query, params object[]? args) where T : class, IDataObject, new()
    {
        T? ret = null;

        // PostgreSQL Anomalie bei Order DESC und Index = ASC
        options.MaximumRecordCount = options.OrderMode == eOrderMode.Descending && Database.DatabaseType == eDatabaseType.PostgreSql ? 10 : 1;

        var tdesc = typeof(T).GetTypeDescription();

        using var cmd = _createQuery(tdesc, options, query, eQueryType.Select, args);
        
        Database.TraceBeforeExecute?.Invoke(new TraceInfo(cmd.CommandText));

        using var reader = GetReader(cmd);

        if (reader == null)
            throw new Exception(string.Format(CoreStrings.ERR_GETREADER_NOT_AVAILABLE));

        var cols = reader.FieldCount;
        var dict = new PropertyDescription[cols];

        for (var i = 0; i < cols; i++)
        {
            var fieldname = reader.GetName(i).ToLowerInvariant();
            var prop = tdesc.Fields.Values.FirstOrDefault(f => f.Name.Equals(fieldname, StringComparison.OrdinalIgnoreCase));

            if (prop != null)
                dict[i] = prop;
        }

        if (reader.Read())
            ret = ReadFromReader(into, reader, dict, tdesc);

        Database.TraceAfterExecute?.Invoke(new TraceInfo(cmd.CommandText));

        Database.LastQuery = cmd.CommandText;

        return ret;
    }

    /// <summary>
    /// wählt ein einzelnes (das erste tah entspricht der Abfrage) Datenobjekt vom Typ T
    /// </summary>
    /// <typeparam name="T">Typ des Datenobjekts</typeparam>
    /// <param name="query">Abfrage-String</param>
    /// <param name="args">Argumente für die Abfrage (Parameter)</param>
    /// <returns>Datenobjekt oder Standard(T)</returns>
    public T? SelectSingle<T>(string query, params object[]? args) where T : class, IDataObject, new()
    {
        return SelectSingle<T>(new ReadOptions { MaximumRecordCount = 1 }, query, args);
    }

    /// <summary>
    /// lädt ein einzelnes Datenobjekt vom Typ T anhand seines Primärschlüssels.
    /// 
    /// Diese Methode unterstützt die Verwendung eines Caches, wenn das Tabellendefinitionsattribut AFTable.UseCache auf true gesetzt ist!
    /// </summary>
    /// <typeparam name="T">Typ des Datenobjekts</typeparam>
    /// <param name="guid">primärer Schlüssel</param>
    /// <returns>Datenobjekt oder default(T)</returns>
    public T? Load<T>(Guid guid) where T : class, IDataObject, new()
    {
        return Load<T>(guid, null, false);
    }

    /// <summary>
    /// lädt ein einzelnes Datenobjekt vom Typ T anhand seines Primärschlüssels.
    /// 
    /// Diese Methode unterstützt die Verwendung eines Caches, wenn das Tabellendefinitionsattribut AFTable.UseCache auf true gesetzt ist!
    /// </summary>
    /// <typeparam name="T">Typ des Datenobjekts</typeparam>
    /// <param name="guid">primärer Schlüssel</param>
    /// <param name="into">In dieses Objekt einlesen, statt neues zu erzeugen</param>
    /// <param name="ignorecache">Cache beim Lesen ignorieren (Default = false, Cache verwenden)</param>
    /// <returns>Datenobjekt oder default(T)</returns>
    public T? Load<T>(Guid guid, T? into, bool ignorecache) where T : class, IDataObject, new()
    {
        var tdesc = typeof(T).GetTypeDescription();
        var useCache = false;

        if (tdesc.IsTable)
            useCache = tdesc.Table!.UseCache;
        else if (tdesc.IsView)
            useCache = tdesc.View!.UseCache;

        if (useCache && !ignorecache)
        {
            var ret = Database.FromCache<T>(guid);
            
            if (ret != null)
                return ret;
        }
        
        var fldKey = tdesc.FieldKey;

        if (fldKey == null)
            throw new Exception(string.Format(CoreStrings.ERR_TYPE_DOES_NOT_CONTAIN_KEY, typeof(T).FullName));

        var query = fldKey.Name + @" = ?";

        var model = SelectSingle(into, new ReadOptions(), query, guid); 
        
        if (useCache && model != null)
            Database.ToCache(guid, model);

        return model;
    }

    /// <summary>
    /// Lädt ein einzelnes Objekt aus einer Tabelle oder Ansicht auf der Basis seines PrimaryKey und gibt dem IModelLink des Objekts zurück.
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>
    /// <param name="guid">PrimaryKey des Objekts</param>
    /// <param name="ignorecache">Objekt nicht aus Cache lesen, sondern immer aus DB</param>
    /// <returns>das gelesene Objekt oder NULL, wenn nichts gefunden wurde</returns>
    public ModelInfo? LoadInfo<T>(Guid guid, bool ignorecache = false) where T : class, IDataObject, new()
    {
        var tdesc = typeof(T).GetTypeDescription();
        var useCache = false;

        if (tdesc.IsTable)
            useCache = tdesc.Table!.UseCache;
        else if (tdesc.IsView)
            useCache = tdesc.View!.UseCache;

        if (useCache && !ignorecache)
        {
            var ret = Database.FromCache<T>(guid);

            if (ret != null)
                return ret.ModelInfo;
        }

        var fldKey = tdesc.FieldKey;

        if (fldKey == null)
            throw new Exception(string.Format(CoreStrings.ERR_TYPE_DOES_NOT_CONTAIN_KEY, typeof(T).FullName));

        var query = fldKey.Name + @" = ?";

        var model = SelectSingleInfo<T>(new ReadOptions(), query, guid);

        //if (useCache && model != null)
        //    Database.ToCache(guid, model);

        return model;
    }


    /// <inheritdoc />
    public bool ReLoad<T>(T model) where T : class, IDataObject, new()
    {
        if (model.PrimaryKey.IsEmpty()) return false;

        var tdesc = typeof(T).GetTypeDescription();
        
        // feststellen, ob ein neu laden notwendig ist (TS letzte Änderung)
        var field = tdesc.Fields.Values.FirstOrDefault(f => f.Field?.SystemFieldFlag == eSystemFieldFlag.TimestampChanged);
        
        if (field != null)
        {
            var changedat = LoadValue<DateTime, T>(model.PrimaryKey, field.Name);

            if (changedat == (DateTime)tdesc.Accessor[model, field.Name])
                return false;
        }

        Load(model.PrimaryKey, model, true);

        return true;
    }


    /// <summary>
    /// Mehrere Objekte aus Tabelle oder Ansicht auswählen und als IModelLinks zurückgeben.
    /// </summary>
    /// <typeparam name="T">Typ der Objekte</typeparam>
    /// <param name="options">Optionen zum Lesen</param>
    /// <param name="where">WHERE-Klausel zum Lesen. ? als Platzhalter für einen Parameter/Argument verwenden.
    /// Die Klausel kann nur die WHERE-Klausel der Abfrage oder eine komplette SQL-Abfrage enthalten
    /// (einschließlich SELECT und mehr)</param>
    /// <param name="args">Argumente/Parameter für die WHERE-Klausel</param>
    /// <returns>BindingList mit gelesenen Objekten oder eine leere BindingList</returns>
    public BindingList<ModelInfo> SelectInfos<T>(ReadOptions options, string where, params object[]? args) where T : class, IDataObject, new()
    {
        BindingList<ModelInfo> ret = [];

        var restore = ret.RaiseListChangedEvents;
        ret.RaiseListChangedEvents = false;

        var tdesc = typeof(T).GetTypeDescription();

        if (tdesc.ModelInfoFiels.Count < 1 || tdesc.ModelInfoCaptionField == null || tdesc.ModelKeyField == null)
            throw new ArgumentException($"Der Type {typeof(T).FullName} unterstützt ModelInfo nicht (ModelInfoFields, ModelInfoCaption und ModelKeyField müssen vorhanden sein).");

        options.Fields = new string[tdesc.ModelInfoFiels.Count + 2];
        int pos = 0;
        foreach (var f in tdesc.ModelInfoFiels)
        {
            options.Fields[pos] = f.Key;
            ++pos;
        }
        options.Fields[pos] = tdesc.ModelInfoCaptionField!.Name;
        options.Fields[pos + 1] = tdesc.ModelKeyField!.Name;

        using var cmd = _createQuery(tdesc, options, where, eQueryType.Select, args);

        Database.TraceBeforeExecute?.Invoke(new TraceInfo(cmd.CommandText));

        using var reader = GetReader(cmd);

        if (reader == null)
            throw new Exception(string.Format(CoreStrings.ERR_GETREADER_NOT_AVAILABLE));

        var cols = reader.FieldCount;
        var dict = new PropertyDescription?[cols];

        var fieldLookup = tdesc.Fields.Values.ToDictionary(f => f.Name.ToLowerInvariant(), f => f);

        for (var i = 0; i < cols; i++)
        {
            var fieldname = reader.GetName(i).ToLowerInvariant();
            fieldLookup.TryGetValue(fieldname, out dict[i]);
        }

        while (reader.Read())
        {
            var modelInfo = ReadModelInfoFromReader<T>(reader, dict, tdesc);

            if (options.Filter != null && options.Filter(modelInfo))
                ret.Add(modelInfo);
            else if (options.Filter == null)
                ret.Add(modelInfo);
        }

        Database.TraceAfterExecute?.Invoke(new TraceInfo(cmd.CommandText));

        Database.LastQuery = cmd.CommandText;

        ret.RaiseListChangedEvents = restore;
        if (restore) ret.ResetBindings();

        return ret;
    }

    private ModelInfo ReadModelInfoFromReader<T>(IDataReader reader, PropertyDescription?[] dict, TypeDescription tdesc) where T : IModel, new()
    {
        ModelInfo ret = new() { ModelType = typeof(T) };

        var sb = StringBuilderPool.GetStringBuilder();

        var fields = reader.FieldCount;
        for (var i = 0; i < fields; i++)
        {
            if (dict[i] is null) continue;

            if (dict[i]!.Field?.ModelInfoCaption ?? false)
            {
                ret.Caption = Database.Translator.FromDatabase(reader.GetValue(i), ((PropertyInfo)dict[i]!).PropertyType)?.ToString() ?? "";
                continue;
            }

            if (dict[i]!.Field?.SystemFieldFlag == eSystemFieldFlag.PrimaryKey)
            {
                ret.Id = (Guid)(Database.Translator.FromDatabase(reader.GetValue(i), ((PropertyInfo)dict[i]!).PropertyType) ?? Guid.Empty);
                continue;
            }

            if (dict[i]!.Field?.ModelInfoData ?? false)
            {
                ret.Data.Add(dict[i]!.Name, Database.Translator.FromDatabase(reader.GetValue(i), ((PropertyInfo)dict[i]!).PropertyType));
                sb.AppendLine($"{dict[i]!.Name} : {Database.Translator.FromDatabase(reader.GetValue(i), ((PropertyInfo)dict[i]!).PropertyType)}");
            }
        }

        try
        {
            typeof(T).GetController().CustomizeModelInfo(ret);
        }
        catch
        {
            // wenn es keinen Controller gibt, wird die ModelInfo eben nicht angepasst.
        }

        return ret;
    }

    /// <summary>
    /// lädt eine Liste von Datensätzen aus der Verbindung
    /// </summary>
    /// <typeparam name="T">Typ zu lesen</typeparam>
    /// <param name="options">Abfrageoptionen</param>
    /// <returns>Liste</returns>
    public BindingList<T> Select<T>(ReadOptions options) where T : class, IDataObject, new()
    {
        return Select<T>(options, string.Empty, []);
    }

    /// <summary>
    /// Mehrere Objekte aus Tabelle oder Ansicht auswählen und als IModelLinks zurückgeben.
    /// </summary>
    /// <typeparam name="T">Typ der Objekte</typeparam>
    /// <param name="options">Optionen zum Lesen</param>
    /// <returns>BindingList mit gelesenen Objekten oder eine leere BindingList</returns>
    public BindingList<ModelInfo> SelectInfos<T>(ReadOptions options) where T : class, IDataObject, new()
    {
        return SelectInfos<T>(options, string.Empty, []);
    }

    /// <summary>
    /// Selektieren eines Dictionary (ein Feld als Key, ein Feld als Value). 
    /// </summary>
    /// <typeparam name="T">Typ der Objekte</typeparam>
    /// <typeparam name="TKey">Typ des Key-Feldes</typeparam>
    /// <typeparam name="TValue">Typ des Value-Feldes</typeparam>
    /// <param name="nameKeyField">Name des Key-Feldes (z.B. "SYS_ID")</param>
    /// <param name="nameValueField">Name des Value-Feldes (z.B. "NAME")</param>
    /// <param name="query">WHERE-Klausel zu lesen. Verwenden Sie ? als Platzhalter für einen Parameter/Argument.
    /// Die Klausel kann nur die WHERE-Klausel der Abfrage oder eine komplette SQL-Abfrage enthalten
    /// (einschließlich SELECT und mehr)</param>
    /// <param name="args">Argumente/Parameter für die WHERE-Klausel</param>
    /// <returns>Dictionary mit gelesenen Daten oder eine leeres Dictionary</returns>
    public Dictionary<TKey, TValue> SelectDictionary<T, TKey, TValue>(string nameKeyField, string nameValueField, string query, params object[]? args) 
        where T : class, IDataObject, new() 
        where TKey : notnull 
        where TValue : notnull
    {
        Dictionary<TKey, TValue> ret = [];

        ReadOptions options = new() { Fields = [nameKeyField, nameValueField], OrderBy = nameValueField };

        using var cmd = _createQuery(typeof(T).GetTypeDescription(), options, query, eQueryType.Select, args);

        Database.TraceBeforeExecute?.Invoke(new TraceInfo(cmd.CommandText));

        using var reader = GetReader(cmd);

        if (reader == null)
            throw new Exception(string.Format(CoreStrings.ERR_GETREADER_NOT_AVAILABLE));

        int idxKey = reader.GetOrdinal(nameKeyField);
        int idxValue = reader.GetOrdinal(nameValueField);

        while (reader.Read())
        {
            if (reader.GetValue(idxKey) is not TKey key) break;
            if (reader.GetValue(idxValue) is not TValue value) break;

            ret.Add(key, value);
        }

        Database.TraceAfterExecute?.Invoke(new TraceInfo(cmd.CommandText));

        Database.LastQuery = cmd.CommandText;

        return ret;
    }


    /// <summary>
    /// lädt eine Liste von Datensätzen aus der Verbindung
    /// </summary>
    /// <typeparam name="T">Typ zu lesen</typeparam>
    /// <param name="options">Abfrageoptionen</param>
    /// <param name="query">Abfrage (where-Klausel oder vollständige select-Abfrage)</param>
    /// <param name="args">Argumente in der Abfrage</param>
    /// <returns>Liste</returns>
    public BindingList<T> Select<T>(ReadOptions options, string query, params object[]? args) where T : class, IDataObject, new()
    {
        BindingList<T> ret = [];

        var restore = ret.RaiseListChangedEvents;
        ret.RaiseListChangedEvents = false;

        using var cmd = _createQuery(typeof(T).GetTypeDescription(), options, query, eQueryType.Select, args);

        Database.TraceBeforeExecute?.Invoke(new TraceInfo(cmd.CommandText));

        using var reader = GetReader(cmd);

        if (reader == null)
            throw new Exception(string.Format(CoreStrings.ERR_GETREADER_NOT_AVAILABLE));

        var cols = reader.FieldCount;
        var dict = new PropertyDescription?[cols];

        var tdesc = typeof(T).GetTypeDescription();
        var fieldLookup = tdesc.Fields.Values.ToDictionary(f => f.Name.ToLowerInvariant(), f => f);

        for (var i = 0; i < cols; i++)
        {
            var fieldname = reader.GetName(i).ToLowerInvariant();
            fieldLookup.TryGetValue(fieldname, out dict[i]);
        }

        var allwaysIncluded = false;

        while (reader.Read())
        {
            var record = ReadFromReader<T>(reader, dict, tdesc);

            if (options.Filter != null && options.Filter(record))
                ret.Add(record);
            else if (options.Filter == null)
                ret.Add(record);

            if (options.AllwaysInclude != null && options.AllwaysInclude != Guid.Empty)
            {
                if (record.PrimaryKey.Equals(options.AllwaysInclude))
                    allwaysIncluded = true;
            }
        }

        if (allwaysIncluded == false && options.AllwaysInclude != null && options.AllwaysInclude != Guid.Empty)
        {
            var record = Load<T>((Guid)options.AllwaysInclude);
            if (record != null)
                ret.Add(record);
        }

        Database.TraceAfterExecute?.Invoke(new TraceInfo(cmd.CommandText));

        Database.LastQuery = cmd.CommandText;

        ret.RaiseListChangedEvents = restore;
        if (restore) ret.ResetBindings();

        return ret;
    }

    /// <summary>
    /// Erstellt einen Vorwärtsleser für eine Abfrage.
    ///
    /// Diese Methode muss von der abgeleiteten Klasse implementiert werden!
    /// </summary>
    /// <param name="command">Abfragebefehl zur Ausführung</param>
    /// <param name="recordCount">betroffene Aufzeichnungen</param>
    /// <returns>der Leser oder null</returns>
    public virtual IDataReader GetReader(TCommand command, out long recordCount)
    {
        throw new NotImplementedException(CoreStrings.ERROR_NOTIMPLEMENTEDINBASE);
    }

    /// <summary>
    /// Erstellt einen Vorwärtsleser für eine Abfrage.
    ///
    /// Diese Methode muss von der abgeleiteten Klasse implementiert werden!
    /// </summary>
    /// <param name="command">Abfragebefehl zur Ausführung</param>
    /// <returns>der Leser oder null</returns>
    public virtual IDataReader GetReader(TCommand command)
    {
        throw new NotImplementedException(CoreStrings.ERROR_NOTIMPLEMENTEDINBASE);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public BindingList<T> Select<T>(string query, params object[]? args) where T : class, IDataObject, new()
    {
        return Select<T>(new ReadOptions(), query, args);
    }

    /// <summary>
    /// Mehrere Objekte aus Tabelle oder Ansicht auswählen und als IModelLinks zurückgeben.
    /// </summary>
    /// <typeparam name="T">Typ der Objekte</typeparam>
    /// <param name="where">WHERE-Klausel zu lesen. Verwenden Sie ? als Platzhalter für einen Parameter/Argument.
    /// Die Klausel kann nur die WHERE-Klausel der Abfrage oder eine komplette SQL-Abfrage enthalten
    /// (einschließlich SELECT und mehr)</param>
    /// <param name="args">Argumente/Parameter für die WHERE-Klausel</param>
    /// <returns>BindingList mit gelesenen Objekten oder eine leere BindingList</returns>
    public BindingList<ModelInfo> SelectInfos<T>(string where, params object[]? args) where T : class, IDataObject, new()
    {
        return SelectInfos<T>(new ReadOptions(), where, args);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public BindingList<T> Select<T>() where T : class, IDataObject, new()
    {
        return Select<T>(new ReadOptions(), "", null);
    }

    /// <summary>
    /// Alle Objekte aus Tabelle oder Ansicht auswählen
    /// </summary>
    /// <typeparam name="T">Typ der Objekte</typeparam>
    /// <returns>BindingList mit allen gelesenen Objekten oder eine leere BindingList, wenn Tabelle oder View leer ist</returns>
    public BindingList<ModelInfo> SelectInfos<T>() where T : class, IDataObject, new()
    {
        return SelectInfos<T>(new ReadOptions(), "", null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IReader<T> SelectReader<T>() where T : class, IDataObject, new()
    {
        return SelectReader<T>(new ReadOptions(), "", null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public IReader<T> SelectReader<T>(string query, params object[]? args) where T : class, IDataObject, new()
    {
        return SelectReader<T>(new ReadOptions(), query, args);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="options"></param>
    /// <param name="query"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public IReader<T> SelectReader<T>(ReadOptions options, string query, params object[]? args) where T : class, IDataObject, new()
    {
        var cmd = _createQuery(typeof(T).GetTypeDescription(), options, query, eQueryType.Select, args);

        Database.TraceBeforeExecute?.Invoke(new TraceInfo(cmd.CommandText));

        var reader = GetReader(cmd);
        
        if (reader == null)
            throw new Exception(string.Format(CoreStrings.ERR_GETREADER_NOT_AVAILABLE));

        DataReader<T> ret = new(reader, this);
        Database.TraceAfterExecute?.Invoke(new TraceInfo(cmd.CommandText));

        Database.LastQuery = cmd.CommandText;

        return ret;
    }

    /// <summary>
    /// Komplette Tabelle als DataTable zurückgeben
    /// </summary>
    /// <typeparam name="T">Tabellentyp</typeparam>
    /// <returns>Datentabelle</returns>
    public DataTable SelectDataTable<T>() where T : class, IDataObject, new()
    {
        return SelectDataTable<T>(new ReadOptions(), "", null);
    }

    /// <summary>
    /// Wählt Zeilen in einer Datentabelle aus
    /// </summary>
    /// <typeparam name="T">Tabellentyp</typeparam>
    /// <param name="query">Abfrage</param>
    /// <param name="args">Argumente</param>
    /// <returns>datierbar</returns>
    public DataTable SelectDataTable<T>(string query, params object[] args) where T : class, IDataObject, new()
    {
        return SelectDataTable<T>(new ReadOptions(), query, args);
    }

    /// <summary>
    /// execute a query and return a datatable with the affected rows
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="options"></param>
    /// <param name="query"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public DataTable SelectDataTable<T>(ReadOptions options, string query, params object[]? args) where T : class, IDataObject, new()
    {
        DataTable ret = new();

        using var cmd = _createQuery(typeof(T).GetTypeDescription(), options, query, eQueryType.Select, args);

        Database.TraceBeforeExecute?.Invoke(new TraceInfo(cmd.CommandText));
        
        using var reader = GetReader(cmd);

        if (reader == null)
            throw new Exception(string.Format(CoreStrings.ERR_GETREADER_NOT_AVAILABLE));

        ret.Load(reader);

        Database.TraceAfterExecute?.Invoke(new TraceInfo(cmd.CommandText));

        Database.LastQuery = cmd.CommandText;

        return ret;
    }

    /// <summary>
    /// execute a query and return a datatable with the affected rows
    /// </summary>
    /// <param name="query"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public DataTable SelectDataTable(string query, params object[]? args)
    {
        DataTable ret = new();

        using var cmd = _createQuery(null, new(), query, eQueryType.Select, args);

        Database.TraceBeforeExecute?.Invoke(new TraceInfo(cmd.CommandText));
        
        using var reader = GetReader(cmd);
        
        if (reader == null)
            throw new Exception(string.Format(CoreStrings.ERR_GETREADER_NOT_AVAILABLE));

        ret.Load(reader);
        Database.TraceAfterExecute?.Invoke(new TraceInfo(cmd.CommandText));

        Database.LastQuery = cmd.CommandText;

        return ret;
    }

    /// <summary>
    /// Loads data for the property with a specific name from a record with the given id
    /// </summary>
    /// <typeparam name="T">Type of the column/property</typeparam>
    /// <typeparam name="TFrom">Typ, von dem der Wert gelesen werden soll</typeparam>
    /// <param name="id">ID of the record</param>
    /// <param name="name">name of column</param>
    /// <returns>value for the field</returns>
    public T? LoadValue<T, TFrom>(Guid id, string name) where TFrom : IDataObject
    {
        return LoadValue<T>(typeof(TFrom), id, name);
    }

    /// <summary>
    /// Loads data for the property with a specific name from a record with the given id
    /// </summary>
    /// <typeparam name="T">Type of the column/property</typeparam>
    /// <param name="type">Type which defines the table or view</param>
    /// <param name="id">ID of the record</param>
    /// <param name="name">name of column</param>
    /// <returns>value for the field</returns>
    public T? LoadValue<T>(Type type, Guid id, string name)
    {
        var tdesc = type.GetTypeDescription();

        var fldKey = tdesc.FieldKey;

        if (fldKey == null)
            throw new Exception(string.Format(CoreStrings.ERR_TYPE_DOES_NOT_CONTAIN_KEY, typeof(T).FullName));

        var sbQuery = StringBuilderPool.GetStringBuilder(content: Database.Translator.GetCommandString(eCommandString.LoadValue));

        var query = sbQuery
            .Replace(@"#FIELDNAME#", tdesc.Fields[name].Name)
            .Replace(@"#TABLENAME#", tdesc.Table != null ? tdesc.Table?.TableName : tdesc.View?.ViewName)
            .Replace(@"#FIELDNAMEKEY#", fldKey.Name).ToString();

        StringBuilderPool.ReturnStringBuilder(sbQuery);

        return ExecuteCommandScalar<T>(query, id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool Exist<T>(Guid id) where T : ITable
    {
        var ret = false;

        var tdesc = typeof(T).GetTypeDescription();

        var fldKey = tdesc.FieldKey;

        if (fldKey == null)
            throw new(string.Format(CoreStrings.ERR_TYPE_DOES_NOT_CONTAIN_KEY, typeof(T).FullName));

        var field = fldKey.Name;
        var table = tdesc.Table != null 
            ? tdesc.Table.TableName 
            : tdesc.View != null 
                ? tdesc.View.ViewName 
                : throw new(string.Format(CoreStrings.ERR_TYPEISNOTTABLEORVIEW, typeof(T).FullName));

        var qry = _getCommand(eCommandString.Exist).Replace(@"#TABLENAME#", table).Replace(@"#FIELDNAMEKEY#", field).ToString();

        using var cmd = _parseCommand(null, qry, [id]);

        cmd.CommandType = CommandType.Text;
        Database.TraceBeforeExecute?.Invoke(new TraceInfo(cmd.CommandText));
        var found = cmd.ExecuteScalar();

        if (found == null) return ret;

        var guid = Database.Translator.FromDatabase(found, typeof(Guid));
                
        if (guid is Guid g)
        {
            Database.TraceAfterExecute?.Invoke(new TraceInfo(cmd.CommandText));

            Database.LastQuery = cmd.CommandText;

            ret = g.Equals(id);
        }

        return ret;
    }

    /// <summary>
    /// count rows in database
    /// </summary>
    /// <typeparam name="T">object type (table/view)</typeparam>
    /// <returns>row count</returns>
    public int Count<T>() where T : IDataObject
    {
        return Count<T>(new ReadOptions(), @"", null);
    }

    /// <summary>
    /// count rows in database
    /// </summary>
    /// <typeparam name="T">object type (table/view)</typeparam>
    /// <param name="query">sql query to filter specific rows</param>
    /// <param name="args">arguments for the query</param>
    /// <returns>row count</returns>
    public int Count<T>(string query, params object[]? args) where T : IDataObject
    {
        return Count<T>(new ReadOptions(), query, args);
    }

    /// <summary>
    /// count rows in database
    /// </summary>
    /// <typeparam name="T">object type (table/view)</typeparam>
    /// <param name="options">query options</param>
    /// <param name="query">sql query to filter specific rows</param>
    /// <param name="args">arguments for the query</param>
    /// <returns>row count</returns>
    public int Count<T>(ReadOptions options, string query, params object[]? args) where T : IDataObject
    {
        int ret;

        var fldKey = typeof(T).GetTypeDescription().FieldKey;

        if (fldKey == null)
            throw new Exception(string.Format(CoreStrings.ERR_TYPE_DOES_NOT_CONTAIN_KEY, typeof(T).FullName));

        options.Fields = [fldKey.Name];

        using var cmd = _createQuery(typeof(T).GetTypeDescription(), options, query, eQueryType.Count, args);

        Database.TraceBeforeExecute?.Invoke(new TraceInfo(cmd.CommandText));
        ret = Convert.ToInt32(cmd.ExecuteScalar());
        Database.TraceAfterExecute?.Invoke(new TraceInfo(cmd.CommandText));

        Database.LastQuery = cmd.CommandText;

        return ret;
    }

    /// <summary>
    /// select summary for a sepecific column in database which holds numeric values
    /// </summary>
    /// <typeparam name="T">object type (table/view)</typeparam>
    /// <param name="field">field to summarize</param>
    /// <returns>sum of column values in all selected rows</returns>
    public decimal Sum<T>(string field) where T : IDataObject
    {
        return Sum<T>(new ReadOptions(), field, "", null);
    }

    /// <summary>
    /// select summary for a sepecific column in database which holds numeric values
    /// </summary>
    /// <typeparam name="T">object type (table/view)</typeparam>
    /// <param name="field">field to summarize</param>
    /// <param name="query">sql query to filter specific rows</param>
    /// <param name="args">arguments for the query</param>
    /// <returns>sum of column values in all selected rows</returns>
    public decimal Sum<T>(string field, string query, params object[]? args) where T : IDataObject
    {
        return Sum<T>(new ReadOptions(), field, query, args);
    }

    /// <summary>
    /// select summary for a sepecific column in database which holds numeric values
    /// </summary>
    /// <typeparam name="T">object type (table/view)</typeparam>
    /// <param name="options">query options</param>
    /// <param name="field">field to summarize</param>
    /// <param name="query">sql query to filter specific rows</param>
    /// <param name="args">arguments for the query</param>
    /// <returns>sum of column values in all selected rows</returns>
    public decimal Sum<T>(ReadOptions options, string field, string query, params object[]? args) where T : IDataObject
    {
        options.Fields =  [ field ];

        using var cmd = _createQuery(typeof(T).GetTypeDescription(), options, query, eQueryType.Sum, args);

        Database.TraceBeforeExecute?.Invoke(new TraceInfo(cmd.CommandText));
        var ret = Convert.ToDecimal(cmd.ExecuteScalar());
        Database.TraceAfterExecute?.Invoke(new TraceInfo(cmd.CommandText));

        Database.LastQuery = cmd.CommandText;

        return ret;
    }
    
    /// <summary>
    /// read a single row/object from a reader
    /// </summary>
    /// <typeparam name="T">type of object</typeparam>
    /// <param name="reader">reader</param>
    /// <param name="dict">dictionary with all properties to read</param>
    /// <param name="into">Datenobjekt, in das die Daten eingelesen werden sollen</param>
    /// <param name="tdesc">TypeDescription des Models (optional, default = null), Zuweisung verbessert die Performance!</param>
    /// <returns>row object</returns>
    public T ReadFromReader<T>(T? into, IDataReader reader, PropertyDescription?[] dict, TypeDescription? tdesc = null) where T : IDataObject, new()
    {
        T ret = into ?? new() { Database = Database };

        return (T)readFromReader(typeof(T), ret, reader, dict, tdesc);
    }

    /// <summary>
    /// read a single row/object from a reader
    /// </summary>
    /// <typeparam name="T">type of object</typeparam>
    /// <param name="reader">reader</param>
    /// <param name="dict">dictionary with all properties to read</param>
    /// <param name="tdesc">TypeDescription des Models (optional, default = null), Zuweisung verbessert die Performance!</param>
    /// <returns>row object</returns>
    public T ReadFromReader<T>(IDataReader reader, PropertyDescription?[] dict, TypeDescription? tdesc = null) where T : IDataObject, new()
    {
        T ret = new() { Database = Database };

        return (T)readFromReader(typeof(T), ret, reader, dict, tdesc);
    }

    /// <summary>
    /// read a single row/object from a reader
    /// </summary>
    /// <param name="modelType">type of object</param>
    /// <param name="reader">reader</param>
    /// <param name="dict">dictionary with all properties to read</param>
    /// <param name="tdesc">TypeDescription des Models (optional, default = null), Zuweisung verbessert die Performance!</param>
    /// <returns>row object</returns>
    public IDataObject ReadFromReader(Type modelType, IDataReader reader, PropertyDescription?[] dict, TypeDescription? tdesc = null)
    {
        return readFromReader(modelType, (Activator.CreateInstance(modelType) as IDataObject)!, reader, dict, tdesc);
    }

    /// <summary>
    /// read a single row/object from a reader
    /// </summary>
    /// <param name="modelType">type of object</param>
    /// <param name="reader">reader</param>
    /// <param name="record">the record which will be filled</param>
    /// <param name="dict">dictionary with all properties to read</param>
    /// <param name="tdesc">TypeDescription des Models (optional, default = null), Zuweisung verbessert die Performance!</param>
    /// <returns>row object</returns>
    private IDataObject readFromReader(Type modelType, IDataObject record, IDataReader reader, PropertyDescription?[] dict, TypeDescription? tdesc = null)
    {
        record.Database = Database;

        if (record is Base bret)  // disable property change events while reading
            bret.RaiseChangeEvents = false;

        tdesc ??= modelType.GetTypeDescription();

        var fields = reader.FieldCount;
        for (var i = 0; i < fields; i++)
        {
            if (dict[i] is null) continue;

            var pinfo = (PropertyInfo)dict[i]!;

            if (pinfo.CanWrite == false)
                continue;

            if (pinfo.PropertyType == typeof(short))
                tdesc.Accessor[record, pinfo.Name] = Convert.ToInt16(Database.Translator.FromDatabase(reader.GetValue(i), pinfo.PropertyType));
            else if (pinfo.PropertyType == typeof(byte))
                tdesc.Accessor[record, pinfo.Name] = Convert.ToByte(Database.Translator.FromDatabase(reader.GetValue(i), pinfo.PropertyType));
            else
                tdesc.Accessor[record, pinfo.Name] = Database.Translator.FromDatabase(reader.GetValue(i), pinfo.PropertyType);
        }

        // commit changes of the object to reset the change tracker (only if tracking is not disabled)
        if (record is not Base brett)
            record.CommitChanges();
        else
            brett.RaiseChangeEvents = true;

        record.AfterLoad();

        return record;

    }
    #endregion

    #region private methods
    private void _checkView(TypeDescription tdesc)
    {
        if (tdesc.View == null)
            throw new Exception(string.Format(CoreStrings.ERR_TYPEISNOTVIEW, tdesc.Name));

        var viewname = tdesc.View.ViewName;

        if (ExistView(viewname))
            DropView(viewname);

        StringBuilder sbFields = StringBuilderPool.GetStringBuilder();
        StringBuilder sbFieldsonly = StringBuilderPool.GetStringBuilder();
        StringBuilder sbSourcefields = StringBuilderPool.GetStringBuilder();

        foreach (var field in tdesc.Fields.Values)
        {
            if (field.Field == null)
                continue;

            var fieldname = field.Name;

            if (field.Field.SourceField.IsEmpty())
                sbFieldsonly.AppendEach(fieldname, @", ");
            else
                sbFields.AppendEach(fieldname, @", ");

            if (field.Name.IsEmpty())
                continue;

            if (field.Field.SourceField.IsEmpty())
                continue;

            sbSourcefields.AppendEach(field.Field.SourceField, @" as ", fieldname, @", ");
        }

        sbFields.Append(sbFieldsonly);

        var fields = sbFields.ToString();
        var sourcefields = sbSourcefields.ToString();

        StringBuilderPool.ReturnStringBuilder(sbFields);
        StringBuilderPool.ReturnStringBuilder(sbSourcefields);
        StringBuilderPool.ReturnStringBuilder(sbFieldsonly);

        if (fields.EndsWith(@", "))
            fields = fields[..^2];

        if (sourcefields.EndsWith(@", "))
            sourcefields = sourcefields[..^2];

        CreateView(viewname, fields, tdesc.View.Query.Replace(@"#FIELDS#", sourcefields));
    }

    private void _checkTable(TypeDescription tdesc)
    {
        if (tdesc.Table == null)
            throw new Exception(string.Format(CoreStrings.ERR_TYPEISNOTTABLE, tdesc.Name));

        if (tdesc.FieldKey == null)
            throw new Exception(string.Format(CoreStrings.ERR_TYPE_DOES_NOT_CONTAIN_KEY, tdesc.Name));

        if (tdesc.FieldCreated == null)
            throw new Exception(string.Format(CoreStrings.ERR_TYPE_NOT_CONTAIN_AFEATED, tdesc.Name));

        if (tdesc.FieldChanged == null)
            throw new Exception(string.Format(CoreStrings.ERR_TYPE_NOT_CONTAIN_CHANGED, tdesc.Name));


        var tablename = Database.GetName(tdesc.Table.TableName);

        if (ExistTable(tablename) == false)
        {
            BeginTransaction();
            try
            {
                CreateTable(
                    tablename,
                    tdesc.FieldKey.Name,
                    tdesc.FieldCreated.Name,
                    tdesc.FieldChanged.Name
                );
                CommitTransaction();
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                throw new Exception(string.Format(CoreStrings.ERR_TABLEAFEATING, tablename), ex);
            }
        }

        var scheme = GetScheme(tablename)?.AsEnumerable();
        List<DataRow> processedRows = [];

        foreach (var prop in tdesc.Fields.Values)
        {
            if (prop.Field == null)
                continue;

            if (scheme == null) continue;

            var row = scheme.FirstOrDefault(r => string.Equals(r.Field<string>(@"ColumnName")?.ToLowerInvariant(), prop.Name, StringComparison.OrdinalIgnoreCase));

            if (row == null)
            {
                try
                {
                    var query = _getCommand(eCommandString.CreateField)
                        .Replace(@"#FIELDNAME#", prop.Name)
                        .Replace(@"#NAME#", prop.Name);
                    if (((PropertyInfo)prop).PropertyType.IsEnum)
                        query.Replace(@"#FIELDOPTIONS#", _getCommandString(eCommandString.FieldDefInt32));
                    else if (((PropertyInfo)prop).PropertyType == typeof(bool))
                        query.Replace(@"#FIELDOPTIONS#", _getCommandString(eCommandString.FieldDefBool));
                    else if (((PropertyInfo)prop).PropertyType == typeof(byte))
                        query.Replace(@"#FIELDOPTIONS#", _getCommandString(eCommandString.FieldDefByte));
                    else if (((PropertyInfo)prop).PropertyType == typeof(DateTime))
                        query.Replace(@"#FIELDOPTIONS#", _getCommandString(eCommandString.FieldDefDateTime));
                    else if (((PropertyInfo)prop).PropertyType == typeof(DateOnly))
                        query.Replace(@"#FIELDOPTIONS#", _getCommandString(eCommandString.FieldDefDate));
                    else if (((PropertyInfo)prop).PropertyType == typeof(TimeOnly))
                        query.Replace(@"#FIELDOPTIONS#", _getCommandString(eCommandString.FieldDefTime));
                    else if (((PropertyInfo)prop).PropertyType == typeof(decimal))
                        query.Replace(@"#FIELDOPTIONS#", _getCommandString(eCommandString.FieldDefDecimal));
                    else if (((PropertyInfo)prop).PropertyType == typeof(double))
                        query.Replace(@"#FIELDOPTIONS#", _getCommandString(eCommandString.FieldDefDouble));
                    else if (((PropertyInfo)prop).PropertyType == typeof(float))
                        query.Replace(@"#FIELDOPTIONS#", _getCommandString(eCommandString.FieldDefFloat));
                    else if (((PropertyInfo)prop).PropertyType == typeof(Guid))
                        query.Replace(@"#FIELDOPTIONS#", _getCommandString(eCommandString.FieldDefGuid));
                    else if (((PropertyInfo)prop).PropertyType == typeof(Image))
                        query.Replace(@"#FIELDOPTIONS#", _getCommandString(eCommandString.FieldDefImage));
                    else if (((PropertyInfo)prop).PropertyType == typeof(int))
                        query.Replace(@"#FIELDOPTIONS#", _getCommandString(eCommandString.FieldDefInt32));
                    else if (((PropertyInfo)prop).PropertyType == typeof(short))
                        query.Replace(@"#FIELDOPTIONS#", _getCommandString(eCommandString.FieldDefInt16));
                    else if (((PropertyInfo)prop).PropertyType == typeof(long))
                        query.Replace(@"#FIELDOPTIONS#", _getCommandString(eCommandString.FieldDefInt64));
                    else if (((PropertyInfo)prop).PropertyType == typeof(Type))
                        query.Replace(@"#FIELDOPTIONS#", _getCommandString(eCommandString.FieldDefType));
                    else if (((PropertyInfo)prop).PropertyType == typeof(byte[]))
                        query.Replace(@"#FIELDOPTIONS#", _getCommandString(eCommandString.FieldDefBinary));
                    else if (((PropertyInfo)prop).PropertyType == typeof(Color))
                        query.Replace(@"#FIELDOPTIONS#", _getCommandString(eCommandString.FieldDefColor));
                    else if (((PropertyInfo)prop).PropertyType == typeof(AFBitArray))
                        query.Replace(@"#FIELDOPTIONS#", _getCommandString(eCommandString.FieldDefBinary));
                    else if (((PropertyInfo)prop).PropertyType == typeof(ModelLinkCollection))
                        query.Replace(@"#FIELDOPTIONS#", _getCommandString(eCommandString.FieldDefMemo));
                    else if (((PropertyInfo)prop).PropertyType == typeof(string))
                    {
                        query.Replace(@"#FIELDOPTIONS#",
                            prop.Field.MaxLength == -1
                                ? _getCommandString(eCommandString.FieldDefMemo)
                                : _getCommandString(eCommandString.FieldDefString));
                    }
                    else
                        query.Replace(@"#FIELDOPTIONS#", _getCommandString(eCommandString.FieldDefObject));

                    query = query
                        .Replace(@"#TABLENAME#", tablename)
                        .Replace(@"#SIZE#", prop.Field.MaxLength.ToString())
                        .Replace(@"#BLOCKSIZE#", prop.Field.BlobBlockSize.ToString());

                    ExecuteCommand(query.ToString(), null);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error while create/update field {prop.Name}.", ex);
                }

                var desc = prop.Context?.Description ?? "";

                if (desc.IsNotEmpty())
                {
                    string qry = "";

                    try
                    {
                        // Kommentar/Beschreibung der Spalte setzen
                        var query = _getCommand(eCommandString.SetComment)
                            .Replace(@"#FIELDNAME#", prop.Name)
                            .Replace(@"#TARGETFIELD#", prop.Name)
                            .Replace(@"#NAME#", prop.Name)
                            .Replace(@"#TABLENAME#", tablename)
                            .Replace(@"#COMMENT#", prop.Context?.Description.Replace("'", "''") ?? "");

                        qry = query.ToString();

                        if (query.ToString().Length > 0) // nur wenn nicht leer ausführen
                            ExecuteCommand(query.ToString(), null);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error while create/update comment for field {prop.Name} ({qry}).", ex);
                    }
                }
            }
            else
            {
                processedRows.Add(row);
                if (((PropertyInfo)prop).PropertyType == typeof(string) &&
                    (int)row[@"ColumnSize"] < prop.Field.MaxLength)
                {
                    try
                    {
                        var query = _getCommand(eCommandString.AlterFieldLength)
                            .Replace(@"#FIELDNAME#", prop.Name)
                            .Replace(@"#TABLENAME#", tablename)
                            .Replace(@"#SIZE#", prop.Field.MaxLength.ToString().Trim());

                        if (query.Length > 5) // execute only if needed (for example in PostgreSQL databases NOT using varchar)
                            ExecuteCommand(query.ToString(), null);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error while update length for field {prop.Name}.", ex);
                    }
                }
            }

            if (prop.Field.Indexed)
            {
                try
                {
                    var idxName = @"IDX_" + tdesc.Table?.TableId.ToString().Trim() + @"_" + prop.Name;
                    string idxExpr;

                    //if (prop.Field.SystemFieldFlag != eSystemFieldFlag.None)
                    //    idxName += @"_" + tdesc.Table?.TableId.ToString().Trim();

                    if (prop.Field.IndexDefinition != null && !prop.Field.IndexDefinition.IsEmpty())
                        idxExpr = prop.Field.IndexDefinition;
                    else
                        idxExpr = prop.Name;

                    if (ExistIndex(idxName, tablename))
                        DropIndex(idxName, tablename);

                    CreateIndex(idxName, tablename, idxExpr, prop.Field.Unique, false);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error while create/update index for field {prop.Name}.", ex);
                }
            }

            if (prop.Field.ConstraintType == null || prop.Field.ConstraintType == tdesc.Type) continue;

            var tdescRef = prop.Field.ConstraintType.GetTypeDescription();

            if (tdescRef == null || tdescRef.Table == null)
            {
                throw new InvalidOperationException(
                    string.Format(CoreStrings.ERR_MISSTYPEDESCRIPTION,
                        prop.Field.ConstraintType.FullName));
            }

            if (tdescRef.FieldKey == null)
            {
                throw new InvalidOperationException(
                    string.Format(CoreStrings.ERR_TYPE_DOES_NOT_CONTAIN_KEY,
                        prop.Field.ConstraintType.FullName));
            }


            var fkeyName = @"FKEY_" + prop.Name;

            if (ExistConstraint(fkeyName, tablename)) continue;

            // prüfen ob die Zieltabelle existiert und wenn nicht vorher anlegen...
            if (!ExistTable(tdescRef.Table.TableName))
                Check(prop.Field.ConstraintType);

            CreateForeignKeyConstraint(fkeyName, tablename, prop.Name, tdescRef.Table.TableName,
                tdescRef.FieldKey.Name, prop.Field.ConstraintUpdate, prop.Field.ConstraintDelete);
        }

        if (!Database.Configuration.AllowDropColumns || scheme == null) return;

        foreach (DataRow row in scheme)
        {
            if (processedRows.Contains(row))
                continue;

            // remove Index 
            var idxName = @"IDX_" + row.Field<string>(@"ColumnName");

            if (ExistIndex(idxName, tablename))
                DropIndex(idxName, tablename);

            // Drop row...
            var query = _getCommand(eCommandString.DropField)
                .Replace(@"#FIELDNAME#", row.Field<string>(@"ColumnName"))
                .Replace(@"#TABLENAME#", tablename).ToString();

            ExecuteCommand(query, null);
        }
    }

    private StringBuilder _getCommand(eCommandString command) { return new StringBuilder(Database.Translator.GetCommandString(command)); }
    
    private string _getCommandString(eCommandString command) { return Database.Translator.GetCommandString(command); }

    private string _translateEventType(eTriggerEvent code) { return Database.Translator.GetTriggerEvent(code); }

    /// <summary>
    /// Erzeugt aus einem SQL-Query und optionalen Parametern eine parametrisiertes DbCommand-Objekt, dass dann ausgeführt werden kann.
    /// </summary>
    /// <param name="options">QueryOptions</param>
    /// <param name="command">SQL-Query</param>
    /// <param name="args">optionale Parameter</param>
    /// <returns>DBCommand für die Ausführung</returns>
    internal TCommand _parseCommand(ReadOptions? options, string command, object[]? args)
    {
        TCommand cmd = new()
        {
            Connection = CurrentConnection,
            Transaction = Transaction
        };

        DataTools.ParseQuery<TParameter, TCommand>(cmd, options != null && options.SkipTranslator ? null : Database.Translator, command, args);

        Database.NextQuery = cmd.CommandText;

        return cmd;
    }

    internal TCommand _createQuery(TypeDescription? tdesc, ReadOptions options, string query, eQueryType queryType, params object[]? args)
    {
        string qry;

        if (options.BufferedQueryId != null && options.BufferedQueryId != Guid.Empty)
        {
            var bufferedQuery = ((Database)Database).getQuery(options.BufferedQueryId.Value);

            if (bufferedQuery != null)
            {
                qry = bufferedQuery;
                return _parseCommand(options, qry, args);
            }
        }

                
        if (query.IsEmpty() == false && (query.Contains(@"&&") || query.Contains(@"||")))
            query = query.Replace(@"&&", @"and").Replace(@"||", @"or");

        if (tdesc == null || query.StartsWith(@"SELECT", StringComparison.OrdinalIgnoreCase))
            qry = query;
        else
        {
            StringBuilder sbQry = StringBuilderPool.GetStringBuilder();

            if (queryType == eQueryType.Count)
                sbQry.Append(_getCommand(eCommandString.SelectCount).Replace(@"#FIELDNAME#", options.Fields[0]));
            else if (queryType == eQueryType.Sum)
                sbQry.Append(_getCommand(eCommandString.SelectSum).Replace(@"#FIELDNAME#", options.Fields[0]));
            else if (queryType == eQueryType.Select)
                sbQry.Append(options.MaximumRecordCount > 0 ? _getCommandString(eCommandString.SelectTop) : _getCommandString(eCommandString.Select));
            else if (queryType == eQueryType.Delete)
                sbQry.Append(_getCommandString(eCommandString.DeleteQuery));

            if (query.IsNotEmpty())
            {
                sbQry.Append(@" WHERE ");
                sbQry.Append(query);
            }
            if (queryType != eQueryType.Delete && options.OrderBy.IsNotEmpty())
            {
                sbQry.Append(@" ORDER BY ");
                sbQry.Append(options.OrderBy);

                if (options.OrderMode == eOrderMode.Descending)
                    sbQry.Append(@" DESC");
            }

            if (queryType != eQueryType.Delete && options.GroupOn.IsNotEmpty())
            {
                sbQry.Append(@" GROUP ON ");
                sbQry.Append(options.GroupOn);
            }

            if (queryType != eQueryType.Delete && options.MaximumRecordCount > 0)
            {
                if (Database.Configuration.DatabaseType == eDatabaseType.PostgreSql)
                    sbQry.Append(@" LIMIT #COUNT# ");

                sbQry.Replace(@"#COUNT#", options.MaximumRecordCount.ToString().Trim());
            }

            qry = sbQry.ToString();

            StringBuilderPool.ReturnStringBuilder(sbQry);


            if (qry.Contains(@"#FIELDNAMES#"))
            {
                if (tdesc == null)
                    throw new NullReferenceException("Query contains placeholder #FIELDNAMES# but no Type-Description is given.");

                StringBuilder sbFields = StringBuilderPool.GetStringBuilder();

                if (options.Fields.Length < 1 && (options.LoadDelayed || tdesc.Fields.Values.FirstOrDefault(dsc => dsc.Field != null && dsc.Field.Delayed) == null))
                    sbFields.Append('*');
                else
                {
                    foreach (var desc in tdesc.Fields.Values)
                    {
                        if (desc.Field == null)
                            continue;

                        if (options.Fields.Length > 0 && Array.Find(options.Fields, fld => Database.GetName(fld) == Database.GetName(((PropertyInfo)desc).Name)) == null && desc.Field.SystemFieldFlag != eSystemFieldFlag.PrimaryKey)
                            continue;

                        if (desc.Field.Delayed && options.LoadDelayed == false)
                            continue;

                        if (sbFields.Length > 0)
                            sbFields.Append(@", ");

                        sbFields.Append(Database.GetName(((PropertyInfo)desc).Name));
                    }
                }

                qry = qry.Replace(@"#FIELDNAMES#", sbFields.ToString());

                StringBuilderPool.ReturnStringBuilder(sbFields);
            }
        }

        if (qry.Contains(@"#TABLENAME#"))
        {
            if (tdesc == null)
                throw new NullReferenceException("Query contains placeholder #FIELDNAMES# but no Type-Description is given.");

            qry = qry.Replace(@"#TABLENAME#", tdesc.IsTable ? tdesc.Table?.TableName : tdesc.View?.ViewName);
        }

        
        if (queryType != eQueryType.Delete && options.UseQueryBuffer)
        {
            options.BufferedQueryId ??= Guid.NewGuid();

            ((Database)Database).setQuery(options.BufferedQueryId.Value, qry);
        }



        return _parseCommand(options, qry, args);
    }

    #endregion

    #region structure
    /// <summary>
    /// wählt das Schema einer Tabelle/Ansicht aus der Datenbank als Datentabelle mit Informationen über jede Spalte in der Tabelle/Ansicht aus
    /// </summary>
    /// <param name="tableviewName">Name der Tabelle/des Views in der Datenbank</param>
    /// <returns>Schema als Datentabelle oder null, wenn kein Schema vorhanden ist</returns>
    public DataTable? GetScheme(string tableviewName)
    {
        DataTable? scheme;
        using (TCommand cmd = new())
        {
            cmd.CommandText = _getCommand(eCommandString.GetSchema).Replace(@"#NAME#", tableviewName).ToString();
            cmd.CommandType = CommandType.Text;
            cmd.Connection = CurrentConnection;
            cmd.Transaction = Transaction;

            Database.TraceBeforeExecute?.Invoke(new TraceInfo(cmd.CommandText));
            using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly)) scheme = reader.GetSchemaTable();
            Database.TraceBeforeExecute?.Invoke(new TraceInfo(cmd.CommandText));
        }

        return scheme;
    }

    /// <summary>
    /// Prüft, ob Tabelle/Ansicht für einen bestimmten Typ, der einen persistenten Typ darstellt, existiert
    /// </summary>
    /// <typeparam name="T">persistenter Typ (Tabelle oder Ansicht)</typeparam>
    public void Check<T>() where T : IDataObject
    {
        Check(typeof(T), false);
    }

    /// <summary>
    /// Prüft, ob Tabelle/Ansicht für einen bestimmten Typ, der einen persistenten Typ darstellt, existiert
    /// </summary>
    /// <param name="type">persistenter Typ (Tabelle oder Ansicht)</param>
    public void Check(Type type)
    {
        Check(type, false);
    }

    /// <summary>
    /// Prüft, ob Tabelle/Ansicht für einen bestimmten Typ, der einen persistenten Typ darstellt, existiert
    /// </summary>
    /// <typeparam name="T">persistenter Typ (Tabelle oder Ansicht)</typeparam>
    /// <param name="force">erzwingt vollständige Prüfung</param>
    public void Check<T>(bool force) where T : IDataObject
    {
        Check(typeof(T), force);
    }

    /// <summary>
    /// Prüft, ob Tabelle/Ansicht für einen bestimmten Typ, der einen persistenten Typ darstellt, existiert
    /// </summary>
    /// <param name="type">persistenter Typ (Tabelle oder Ansicht)</param>
    /// <param name="force">erzwingt vollständige Prüfung</param>
    public void Check(Type type, bool force)
    {
        var tdesc = type.GetTypeDescription();
        SystemDatabaseInformation? dbinfo = null;

        if (type == typeof(SystemDatabaseInformation))
        {
            _checkTable(tdesc);
            return;
        }

        if (tdesc.IsTable && tdesc.Table != null)
        {
            TypeEx.CheckUniqueId(tdesc.Table.TableId, tdesc.Type);

            dbinfo = SelectSingle<SystemDatabaseInformation>(
                         $@"{nameof(SystemDatabaseInformation.SYSINFO_IDENTIFIER)} = ?", tdesc.Table.TableId) ??
                     new SystemDatabaseInformation
            {
                SYSINFO_DBVERSION = 0,
                SYSINFO_TABLENAME = tdesc.Table.TableName,
                SYSINFO_IDENTIFIER = tdesc.Table.TableId
            };

            // Name des Tables anpassen, wenn dieser sich ggf. geändert hat
            dbinfo.SYSINFO_TABLENAME = tdesc.Table.TableName;

            if (force || dbinfo.SYSINFO_DBVERSION < tdesc.Table.Version)
            {
                try
                {
                    _checkTable(tdesc);

                    if (dbinfo.SYSINFO_DBVERSION != tdesc.Table.Version)
                        dbinfo.SYSINFO_DBVERSION = tdesc.Table.Version;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error while check table for type {tdesc.Type.FullName} ({ex.Message}).", ex);
                }
            }
        }
        else if (tdesc.IsView && tdesc.View != null)
        {
            TypeEx.CheckUniqueId(tdesc.View.ViewId, tdesc.Type);

            dbinfo = SelectSingle<SystemDatabaseInformation>(
                         $@"{nameof(SystemDatabaseInformation.SYSINFO_IDENTIFIER)} = ?", tdesc.View.ViewId) ??
                     new SystemDatabaseInformation
                     {
                         SYSINFO_DBVERSION = 0,
                         SYSINFO_TABLENAME = tdesc.View.ViewName,
                         SYSINFO_IDENTIFIER = tdesc.View.ViewId
                     };

            // Name des Views anpassen, wenn dieser sich ggf. geändert hat
            dbinfo.SYSINFO_TABLENAME = tdesc.View.ViewName;

            if (force || dbinfo.SYSINFO_DBVERSION < tdesc.View.Version)
            {
                try
                {
                    _checkView(tdesc);

                    if (dbinfo.SYSINFO_DBVERSION != tdesc.View.Version)
                        dbinfo.SYSINFO_DBVERSION = tdesc.View.Version;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error while check view for type {tdesc.Type.FullName} ({ex.Message}).", ex);
                }
            }
        }

        if (dbinfo != null && dbinfo.HasChanged)
            Save(dbinfo);
    }

    /// <summary>
    /// prüft, ob eine Tabelle existiert
    /// </summary>
    /// <param name="tableName">Tabellenname</param>
    /// <returns>true Tabelle existiert</returns>
    public bool ExistTable(string tableName)
    {
        return ExecuteCommandScalar<int>(_getCommand(eCommandString.ExistTable).Replace(@"#NAME#", Database.GetName(tableName)).ToString()) > 0;
    }

    /// <summary>
    /// prüft, ob eine Prozedur existiert
    /// </summary>
    /// <param name="procedureName">Prozedurname</param>
    /// <returns>true Prozedur existiert</returns>
    public virtual bool ExistProcedure(string procedureName)
    {
        return ExecuteCommandScalar<int>(_getCommand(eCommandString.ExistProcedure).Replace(@"#NAME#", Database.GetName(procedureName)).ToString()) > 0;
    }

    /// <summary>
    /// prüft, ob ein Trigger existiert
    /// </summary>
    /// <param name="triggerName">Triggername</param>
    /// <returns>true Trigger existiert</returns>
    public bool ExistTrigger(string triggerName)
    {
        return ExecuteCommandScalar<int>(_getCommand(eCommandString.ExistTrigger).Replace(@"#NAME#", Database.GetName(triggerName)).ToString()) > 0;
    }

    /// <summary>
    /// prüft, ob eine Ansicht existiert
    /// </summary>
    /// <param name="viewName">Name der Ansicht</param>
    /// <returns>true Ansicht existiert</returns>
    public bool ExistView(string viewName)
    {
        return ExecuteCommandScalar<int>(_getCommand(eCommandString.ExistView).Replace(@"#NAME#", Database.GetName(viewName)).ToString()) > 0;
    }

    /// <summary>
    /// prüft, ob ein Index existiert
    /// </summary>
    /// <param name="indexName">Indexname</param>
    /// <param name="tablename">Name der Tabelle</param>
    /// <returns>true wenn Index existiert</returns>
    public bool ExistIndex(string indexName, string tablename)
    {
        return ExecuteCommandScalar<int>(_getCommand(eCommandString.ExistIndex)
            .Replace(@"#NAME#", Database.GetName(indexName))
            .Replace(@"#TABLENAME#", tablename).ToString()) > 0;
    }

    /// <summary>
    /// prüft, ob eine Fremdschlüssel-Beschränkung existiert
    /// </summary>
    /// <param name="constraintName">Indexname</param>
    /// <param name="tablename">Name der Tabelle</param>
    /// <returns>true wenn Index vorhanden</returns>
    public bool ExistConstraint(string constraintName, string tablename)
    {
        return ExecuteCommandScalar<int>(_getCommand(eCommandString.ExistConstraint)
            .Replace(@"#NAME#", Database.GetName(constraintName))
            .Replace(@"#TABLENAME#", tablename).ToString()) > 0;
    }

    /// <summary>
    /// Löschen einer Tabelle aus der Datenbank
    /// </summary>
    /// <param name="tableName">Tabellenname</param>
    public void DropTable(string tableName)
    {
        ExecuteCommand(_getCommand(eCommandString.BeforeAlterSchema)
            .Replace(@"#DBNAME#", Database.Configuration.DatabaseName).ToString());
        ExecuteCommand(_getCommand(eCommandString.DropTable)
            .Replace(@"#NAME#", Database.GetName(tableName)).ToString());
        ExecuteCommand(_getCommand(eCommandString.AfterAlterSchema)
            .Replace(@"#DBNAME#", Database.Configuration.DatabaseName).ToString());
    }

    /// <summary>
    /// Löschen einer Prozedur aus der Datenbank
    /// </summary>
    /// <param name="procedureName">Prozedurname</param>
    public virtual void DropProcedure(string procedureName)
    {
        ExecuteCommand(_getCommand(eCommandString.BeforeAlterSchema)
            .Replace(@"#DBNAME#", Database.Configuration.DatabaseName).ToString());
        ExecuteCommand(_getCommand(eCommandString.DropProcedure)
            .Replace(@"#NAME#", Database.GetName(procedureName)).ToString());
        ExecuteCommand(_getCommand(eCommandString.AfterAlterSchema)
            .Replace(@"#DBNAME#", Database.Configuration.DatabaseName).ToString());
    }

    /// <summary>
    /// Löschen eines Triggers aus der Datenbank
    /// </summary>
    /// <param name="triggerName">Triggername</param>
    public void DropTrigger(string triggerName)
    {
        ExecuteCommand(_getCommand(eCommandString.BeforeAlterSchema)
            .Replace(@"#DBNAME#", Database.Configuration.DatabaseName).ToString());
        ExecuteCommand(_getCommand(eCommandString.DropTrigger)
            .Replace(@"#NAME#", Database.GetName(triggerName)).ToString());
        ExecuteCommand(_getCommand(eCommandString.AfterAlterSchema)
            .Replace(@"#DBNAME#", Database.Configuration.DatabaseName).ToString());
    }

    /// <summary>
    /// Löschen einer Ansicht aus der Datenbank 
    /// </summary>
    /// <param name="viewName">Name der Ansicht</param>
    public void DropView(string viewName)
    {
        ExecuteCommand(_getCommand(eCommandString.BeforeAlterSchema)
            .Replace(@"#DBNAME#", Database.Configuration.DatabaseName).ToString());
        ExecuteCommand(_getCommand(eCommandString.DropView)
            .Replace(@"#NAME#", Database.GetName(viewName)).ToString());
        ExecuteCommand(_getCommand(eCommandString.AfterAlterSchema)
            .Replace(@"#DBNAME#", Database.Configuration.DatabaseName).ToString());
    }

    /// <summary>
    /// Löschen eines Index aus der Datenbank
    /// </summary>
    /// <param name="indexName">Indexname</param>
    /// <param name="tableName">Tabellenname</param>
    public void DropIndex(string indexName, string tableName)
    {
        ExecuteCommand(_getCommand(eCommandString.BeforeAlterSchema).Replace(@"#DBNAME#", Database.Configuration.DatabaseName).ToString());
        ExecuteCommand(_getCommand(eCommandString.DropIndex)
            .Replace(@"#NAME#", Database.GetName(indexName))
            .Replace(@"#TABLENAME#", Database.GetName(tableName))
            .Replace(@"#DBNAME#", Database.Configuration.DatabaseName).ToString());
        ExecuteCommand(_getCommand(eCommandString.AfterAlterSchema).Replace(@"#DBNAME#", Database.Configuration.DatabaseName).ToString());
    }


    /// <summary>
    /// löscht eine Fremdschlüssel-Beschränkung aus der Datenbank
    /// </summary>
    /// <param name="constraintName">Beschränkungsname</param>
    /// <param name="tableName">Tabellenname</param>
    public void DropConstraint(string constraintName, string tableName)
    {
        ExecuteCommand(_getCommand(eCommandString.BeforeAlterSchema).Replace(@"#DBNAME#", Database.Configuration.DatabaseName).ToString());
        ExecuteCommand(_getCommand(eCommandString.DropConstraint)
            .Replace(@"#NAME#", Database.GetName(constraintName))
            .Replace(@"#TABLENAME#", Database.GetName(tableName))
            .Replace(@"#DBNAME#", Database.Configuration.DatabaseName).ToString());
        ExecuteCommand(_getCommand(eCommandString.AfterAlterSchema).Replace(@"#DBNAME#", Database.Configuration.DatabaseName).ToString());
    }



    /// <summary>
    /// Erstellen einer Tabelle
    /// </summary>
    /// <param name="tableName">Tabellenname</param>
    /// <param name="keyField">Primärschlüsselfeldname</param>
    /// <param name="createdField">Name des Feldes, das zum Zeitstempel erstellt wurde</param>
    /// <param name="changedField">Name des Feldes für die Änderung zum Zeitpunkt des Zeitstempels</param>
    public void CreateTable(string tableName, string keyField, string createdField, string changedField)
    {
        ExecuteCommand(_getCommand(eCommandString.CreateTable)
            .Replace(@"#NAME#", Database.GetName(tableName))
            .Replace(@"#TABLENAME#", Database.GetName(tableName))
            .Replace(@"#FIELDNAMEKEY#", Database.GetName(keyField))
            .Replace(@"#FIELDNAMEAFEATED#", Database.GetName(createdField))
            .Replace(@"#FIELDNAMECHANGED#", Database.GetName(changedField)).ToString());

        var cmd = _getCommand(eCommandString.CreateKeyField);

        if (cmd.ToString().IsNotEmpty())
        {
            ExecuteCommand(cmd
                .Replace(@"#NAME#", Database.GetName(tableName))
                .Replace(@"#TABLENAME#", Database.GetName(tableName))
                .Replace(@"#FIELDNAMEKEY#", Database.GetName(keyField))
                .Replace(@"#FIELDNAMEAFEATED#", Database.GetName(createdField))
                .Replace(@"#FIELDNAMECHANGED#", Database.GetName(changedField)).ToString());
        }

        ExecuteCommand(_getCommand(eCommandString.TriggerBeforeInsert)
            .Replace(@"#NAME#", Database.GetName(tableName))
            .Replace(@"#TABLENAME#", Database.GetName(tableName))
            .Replace(@"#FIELDNAMEKEY#", Database.GetName(keyField))
            .Replace(@"#FIELDNAMEAFEATED#", Database.GetName(createdField))
            .Replace(@"#FIELDNAMECHANGED#", Database.GetName(changedField)).ToString());

        ExecuteCommand(_getCommand(eCommandString.TriggerBeforeUpdate)
            .Replace(@"#NAME#", Database.GetName(tableName))
            .Replace(@"#TABLENAME#", Database.GetName(tableName))
            .Replace(@"#FIELDNAMEKEY#", Database.GetName(keyField))
            .Replace(@"#FIELDNAMEAFEATED#", Database.GetName(createdField))
            .Replace(@"#FIELDNAMECHANGED#", Database.GetName(changedField)).ToString());

        cmd = _getCommand(eCommandString.GrantRightsToUser);

        if (cmd.ToString().IsNotEmpty() && Database.Configuration.Credentials != null && Database.Configuration.Credentials.Username.IsNotEmpty())
        {
            ExecuteCommand(cmd
                .Replace(@"#USERNAME#", Database.Configuration.Credentials.Username)
                .Replace(@"#NAME#", Database.GetName(tableName))
                .Replace(@"#TABLENAME#", Database.GetName(tableName))
                .Replace(@"#FIELDNAMEKEY#", Database.GetName(keyField))
                .Replace(@"#FIELDNAMEAFEATED#", Database.GetName(createdField))
                .Replace(@"#FIELDNAMECHANGED#", Database.GetName(changedField)).ToString());
        }



    }


    /// <summary>
    /// Erstellen einer Prozedur
    /// </summary>
    /// <param name="procedureName">Prozedurname</param>
    /// <param name="code">Prozedurencode</param>
    public virtual void CreateProcedure(string procedureName, string code)
    {

    }

    /// <summary>
    /// Erstellen eines Triggers
    /// </summary>
    /// <param name="triggerName">Triggername</param>
    /// <param name="tableName">Tabellenname</param>
    /// <param name="eventType">Trigger-Ereignis</param>
    /// <param name="code">Quellcode des Auslösers</param>
    public void CreateTrigger(string triggerName, string tableName, eTriggerEvent eventType, string code)
    {
        var eventCode = "";
        switch (eventType)
        {
            case eTriggerEvent.AfterDelete:
                eventCode = _getCommandString(eCommandString.EventAfterDelete);
                break;
            case eTriggerEvent.BeforeInsert:
                eventCode = _getCommandString(eCommandString.EventBeforeInsert);
                break;
            case eTriggerEvent.BeforeUpdate:
                eventCode = _getCommandString(eCommandString.EventBeforeUpdate);
                break;
            case eTriggerEvent.BeforeDelete:
                eventCode = _getCommandString(eCommandString.EventBeforeDelete);
                break;
            case eTriggerEvent.AfterInsert:
                eventCode = _getCommandString(eCommandString.EventAfterInsert);
                break;
            case eTriggerEvent.AfterUpdate:
                eventCode = _getCommandString(eCommandString.EventAfterUpdate);
                break;
        }

        ExecuteCommand(_getCommand(eCommandString.CreateTrigger)
            .Replace(@"#NAME#", Database.GetName(triggerName))
            .Replace(@"#TABLENAME#", Database.GetName(tableName))
            .Replace(@"#EVENT#", eventType.ToString())
            .Replace(@"#EVENTCODE#", eventCode)
            .Replace(@"#CODE#", code).ToString());
    }

    /// <summary>
    /// Als Ansicht erstellen
    /// </summary>
    /// <param name="viewName">Name der Ansicht</param>
    /// <param name="fields">Ansicht Felder</param>
    /// <param name="query">Ansicht abfrage</param>
    public void CreateView(string viewName, string fields, string query)
    {
        ExecuteCommand(_getCommand(eCommandString.CreateView)
           .Replace(@"#NAME#", Database.GetName(viewName))
           .Replace(@"#FIELDS#", fields)
           .Replace(@"#QUERY#", query).ToString());

        var cmd = _getCommand(eCommandString.GrantRightsToUser);

        if (cmd.ToString().IsNotEmpty() && Database.Configuration.Credentials != null && Database.Configuration.Credentials.Username.IsNotEmpty())
        {
            ExecuteCommand(cmd
                .Replace(@"#USERNAME#", Database.Configuration.Credentials.Username)
                .Replace(@"#NAME#", Database.GetName(viewName))
                .Replace(@"#VIEWNAME#", Database.GetName(viewName))
                .Replace(@"#TABLENAME#", Database.GetName(viewName)).ToString());
        }
    }

    /// <summary>
    /// Erstellen eines Index
    /// </summary>
    /// <param name="indexName">Indexname</param>
    /// <param name="tableName">Tabellenname</param>
    /// <param name="expression">Index-Ausdruck</param>
    /// <param name="unique">Index mit eindeutigen Werten erstellen</param>
    /// <param name="descending">Index mit absteigender Reihenfolge erstellen</param>
    public void CreateIndex(string indexName, string tableName, string expression, bool unique, bool descending)
    {
        ExecuteCommand(_getCommand(eCommandString.CreateIndex)
           .Replace(@"#NAME#", Database.GetName(indexName))
           .Replace(@"#TABLENAME#", Database.GetName(tableName))
           .Replace(@"#FIELDS#", expression)
           .Replace(@"#UNIQUE#", unique ? Database.GetConstant(eDatabaseConstant.unique) : Database.GetConstant(eDatabaseConstant.notunique))
           .Replace(@"#DESC#", descending ? Database.GetConstant(eDatabaseConstant.desc) : Database.GetConstant(eDatabaseConstant.asc)).ToString());
    }


    /// <summary>
    /// Liefert die Beschreibung einer Spalte in einer Tabelle
    /// </summary>
    /// <param name="tablename">Name der Tabelle</param>
    /// <param name="fieldname">Name der Spalte</param>
    /// <returns>Beschreibung (string.empty wenn nicht vorhanden oder nicht unterstützt)</returns>
    public string GetComment(string tablename, string fieldname)
    {
        var comment = string.Empty;

        // Kommentar/Beschreibung der Spalte setzen
        var query = _getCommand(eCommandString.GetComment)
            .Replace(@"#FIELDNAME#", fieldname)
            .Replace(@"#TARGETFIELD#", fieldname)
            .Replace(@"#NAME#", fieldname)
            .Replace(@"#TABLENAME#", tablename);

        if (query.ToString().Length > 0) // nur wenn nicht leer ausführen
            comment = ExecuteCommandScalar<string>(query.ToString(), null) ?? string.Empty;

        return comment;
    }

    /// <summary>
    /// Erstellen einer Fremdschlüssel-Beschränkung
    /// </summary>
    /// <param name="constraintName">Name der Einschränkung</param>
    /// <param name="tableName">Tabellenname</param>
    /// <param name="targetTable">Zieltabelle</param>
    /// <param name="targetField">Feldname in der Zieltabelle (meist das Primärschlüsselfeld)</param>
    /// <param name="fieldName"></param>
    /// <param name="constraintDelete"></param>
    /// <param name="constraintUpdate"></param>
    public void CreateForeignKeyConstraint(string constraintName, string tableName, string fieldName, string targetTable, string targetField, eConstraintOperation constraintUpdate, eConstraintOperation constraintDelete)
    {
        var constraint = "";
        if (constraintUpdate != eConstraintOperation.NoAction)
        {
            constraint += @"ON UPDATE ";
            switch (constraintUpdate)
            {
                case eConstraintOperation.SetDefault:
                    constraint += @"SET DEFAULT";
                    break;
                case eConstraintOperation.SetNull:
                    constraint += @"SET NULL";
                    break;
                case eConstraintOperation.Cascade:
                    constraint += @"CASCADE";
                    break;
            }
        }

        if (constraintDelete != eConstraintOperation.NoAction)
        {
            constraint += @" ON DELETE ";
            switch (constraintDelete)
            {
                case eConstraintOperation.SetDefault:
                    constraint += @"SET DEFAULT";
                    break;
                case eConstraintOperation.SetNull:
                    constraint += @"SET NULL";
                    break;
                case eConstraintOperation.Cascade:
                    constraint += @"CASCADE";
                    break;
            }
        }

        ExecuteCommand(_getCommand(eCommandString.CreateConstraint)
           .Replace(@"#NAME#", Database.GetName(constraintName))
           .Replace(@"#FIELDNAME#", Database.GetName(fieldName))
           .Replace(@"#TABLENAME#", Database.GetName(tableName))
           .Replace(@"#TARGETTABLE#", targetTable)
           .Replace(@"#CONSTRAINT#", constraint)
           .Replace(@"#TARGETFIELD#", targetField).ToString());
    }


    /// <summary>
    /// Erstellt ein Feld in der Datenbank
    /// </summary>
    /// <param name="fieldName">Feldname</param>
    /// <param name="tableName">Tabellenname</param>
    /// <param name="fieldType">Feldtyp</param>
    /// <param name="fieldsize">Feldgröße (nur für String-Felder erforderlich - 0 bedeutet, dass das String-Feld ein Blob-Feld mit unbegrenzter Größe ist)</param>
    public void CreateField(string fieldName, string tableName, Type fieldType, int fieldsize)
    {
        string fielddef;

        if (fieldType.IsEnum)
            fielddef = _getCommandString(eCommandString.FieldDefInt32);
        else if (fieldType == typeof(string))
        {
            if (fieldsize > 0)
            {
                fielddef = _getCommand(eCommandString.FieldDefString)
                    .Replace(@"#SIZE#", fieldsize.ToString().Trim()).ToString();
            }
            else
                fielddef = _getCommandString(eCommandString.FieldDefMemo);
        }
        else if (fieldType == typeof(int))
            fielddef = _getCommandString(eCommandString.FieldDefInt);
        else if (fieldType == typeof(short))
            fielddef = _getCommandString(eCommandString.FieldDefInt16);
        else if (fieldType == typeof(int))
            fielddef = _getCommandString(eCommandString.FieldDefInt32);
        else if (fieldType == typeof(long))
            fielddef = _getCommandString(eCommandString.FieldDefInt64);
        else if (fieldType == typeof(long))
            fielddef = _getCommandString(eCommandString.FieldDefLong);
        else if (fieldType == typeof(short))
            fielddef = _getCommandString(eCommandString.FieldDefShort);
        else if (fieldType == typeof(double))
            fielddef = _getCommandString(eCommandString.FieldDefDouble);
        else if (fieldType == typeof(float))
            fielddef = _getCommandString(eCommandString.FieldDefFloat);
        else if (fieldType == typeof(byte))
            fielddef = _getCommandString(eCommandString.FieldDefByte);
        else if (fieldType == typeof(decimal))
            fielddef = _getCommandString(eCommandString.FieldDefDecimal);
        else if (fieldType == typeof(bool))
        {
            fielddef = _getCommand(eCommandString.FieldDefBool).ToString()
                .Replace(@"#SIZE#", @"1");
        }
        else if (fieldType == typeof(Type))
            fielddef = _getCommandString(eCommandString.FieldDefType);
        else if (fieldType == typeof(ModelLinkCollection))
            fielddef = _getCommandString(eCommandString.FieldDefMemo);
        else if (fieldType == typeof(Image))
            fielddef = _getCommandString(eCommandString.FieldDefImage);
        else if (fieldType == typeof(Guid))
        {
            fielddef = _getCommand(eCommandString.FieldDefGuid).ToString()
                .Replace(@"#SIZE#", @"36");
        }
        else if (fieldType == typeof(DateTime))
            fielddef = _getCommandString(eCommandString.FieldDefDateTime);
        else if (fieldType == typeof(DateOnly))
            fielddef = _getCommandString(eCommandString.FieldDefDate);
        else if (fieldType == typeof(TimeOnly))
            fielddef = _getCommandString(eCommandString.FieldDefTime);
        else if (fieldType == typeof(byte[]))
            fielddef = _getCommandString(eCommandString.FieldDefBinary);
        else
            fielddef = _getCommandString(eCommandString.FieldDefObject);

        ExecuteCommand(_getCommand(eCommandString.CreateField)
            .Replace(@"#NAME#", Database.GetName(fieldName))
            .Replace(@"#TABLENAME#", Database.GetName(tableName))
            .Replace(@"#FIELDOPTIONS#", fielddef).ToString());
    }

    #endregion

    #region tools
    /// <summary>
    /// Aktiviert die Standard-Trigger (BeforeUpdate, BeforeInsert) für die angegebene Tabelle.
    /// </summary>
    /// <param name="tableName">Name der Tabelle</param>
    public void EnableDefaultTriggers(string tableName)
    {
        if (ExistTrigger(tableName + @"_BI"))
        {
            ExecuteCommand(Database.Translator.GetCommandString(eCommandString.EnableTrigger)
                .Replace(@"#NAME#", Database.GetName(tableName + @"_BI")));
        }

        if (ExistTrigger(tableName + @"_BU"))
        {
            ExecuteCommand(Database.Translator.GetCommandString(eCommandString.EnableTrigger)
                .Replace(@"#NAME#", Database.GetName(tableName + @"_BU")));
        }
    }

    /// <summary>
    /// Deaktiviert die Standard-Trigger (BeforeUpdate, BeforeInsert) für die angegebene Tabelle.
    /// </summary>
    /// <param name="tableName">Name der Tabelle</param>
    public void DisableDefaultTriggers(string tableName)
    {
        if (ExistTrigger(tableName + @"_BI"))
        {
            ExecuteCommand(Database.Translator.GetCommandString(eCommandString.DisableTrigger)
                .Replace(@"#NAME#", Database.GetName(tableName + @"_BI")));
        }

        if (ExistTrigger(tableName + @"_BU"))
        {
            ExecuteCommand(Database.Translator.GetCommandString(eCommandString.DisableTrigger)
                .Replace(@"#NAME#", Database.GetName(tableName + @"_BU")));
        }
    }

    /// <summary>
    /// Schließt diese Verbindung zur Datenbank.
    /// 
    /// Führt ein Rollback einer Transaktion durch, wenn diese Transaktion nicht commited wurde.
    /// </summary>
    public void Close()
    {
        if (Transaction != null)
        {
            Transaction.Rollback();
            Transaction.Dispose();
        }

        CurrentConnection?.Dispose();
    }

    /// <summary>
    /// Schließt die Verbindung und gibt alle nicht benötigten Ressourcen für diese Verbindung frei
    /// </summary>
    public void Dispose()
    {
        Close();
    }


    /// <summary>
    /// liest einen einzelnen Datensatz aus einem Lesegerät
    /// </summary>
    /// <typeparam name="T">Typ des Objekts/Tabelle/Ansicht</typeparam>
    /// <param name="reader">Leser, von dem gelesen werden soll</param>
    /// <param name="fields">zu lesende Felder</param>
    /// <returns>das einzelne Datensatzobjekt oder null</returns>
    public T ReadFromReader<T>(IDataReader reader, string[] fields) where T : IDataObject, new()
    {
        return ReadFromReader<T>(reader, typeof(T).GetTypeDescription().Properties.Values
            .Where(p => fields.Contains(p.Name)).ToArray());
    }

    /// <summary>
    /// Prüft, ob ein Wert eindeutig ist
    /// </summary>
    /// <typeparam name="T">Typ des Datensatzes/der Tabelle</typeparam>
    /// <param name="data">Datensatz</param>
    /// <param name="field">Feldname</param>
    /// <param name="value">wert</param>
    /// <returns></returns>
    public virtual bool IsUnique<T>(T data, string field, object value) where T : ITable
    {
        var ret = false;

        var desc = data.GetType().GetTypeDescription();

        if (desc.FieldKey == null)
            throw new(string.Format(CoreStrings.ERR_TYPE_DOES_NOT_CONTAIN_KEY, typeof(T)));

        if (desc.Table == null)
            throw new(string.Format(CoreStrings.ERR_TYPEISNOTTABLE, typeof(T)));

        var pkey = (Guid)desc.Accessor[data, desc.FieldKey.Name];
        Guid? result;

        if (pkey.IsEmpty()) // leere GUID muss gesondert berücksichtigt werden.
        {
            var query =
                $@"select {desc.FieldKey.Name} " +
                $@"from {desc.Table.TableName} " +
                $@"where {field} = ?";

            result = ExecuteCommandScalar<Guid>(query, value);
        }
        else
        {
            var query =
                $@"select {desc.FieldKey.Name} " +
                $@"from {desc.Table.TableName} " +
                $@"where {desc.FieldKey.Name} <> ? and {field} = ?";

            result = ExecuteCommandScalar<Guid>(query, pkey, value);
        }

        if (result == null || result.Equals(Guid.Empty))
            ret = true;

        return ret;
    }
    #endregion
}
