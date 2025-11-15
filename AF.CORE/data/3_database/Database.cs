using System.Data;
using System.Data.Common;

namespace AF.DATA;

/// <summary>
/// abstrakte Basisklasse für alle Datenbanken
/// </summary>
public abstract class Database : IDatabase
{
    private Action<TraceInfo>? _traceStart;
    private Action<TraceInfo>? _traceEnd;
    private Action<TraceInfo>? _beforeExecute;
    private Action<TraceInfo>? _afterExecute;
    private Action<IDataObject>? _afterSave;
    private Action<IDataObject>? _afterDelete;
    private readonly Dictionary<Guid, string> _queryBuffer = new();
    private readonly Dictionary<Guid, IDataObject> _cache = new();
    private Exception? _lastException;
    
    /// <summary>
    /// Delegate, der ausgeführt wird, wenn eine leere Collection zur
    /// Rückgabe der selektierten Datensätze benötigt wird.
    ///
    /// Hier kann veranlasst werden, dass statt der BindingList eine andere
    /// Collection zurückgegeben wird.
    ///
    /// Liefert der Delegate NULL oder ist er nicht zugewiesen, wird eine
    /// BindingList erzeugt (Standardverhalten). 
    /// </summary>
    public Func<IBindingList?>? EmptyCollectionRequested { get; set; }


    /// <summary>
    /// Zuletzt ausgeführte Datenbankabfrage
    /// </summary>
    public string? LastQuery { get; set; }

    /// <summary>
    /// nächste auszuführende Datenbankabfrage
    /// </summary>
    public string? NextQuery { get; set; }

    /// <summary>
    /// letzte interne Ausnahme
    /// </summary>
    public Exception? LastException { get => _lastException; set => _lastException = value; }

    /// <summary>
    /// Maximale Anzahl von Abfragen im Abfragepuffer
    ///
    /// Standardwert ist 100.
    /// </summary>
    public int MaxQueryBufferSize { get; set; } = 100;

    /// <summary>
    /// last query buffer id
    /// </summary>
    public Guid? LastBufferId => _queryBuffer.Count > 0 ? _queryBuffer.ElementAt(_queryBuffer.Count - 1).Key : null;

    /// <summary>
    /// liest eine Abfrage aus dem Abfragepuffer
    /// </summary>
    /// <param name="queryID"></param>
    /// <returns>Abfrage oder NULL</returns>
    internal string? getQuery(Guid queryID)
    {
        if (_queryBuffer.TryGetValue(queryID, out var query))
        {
            if (_queryBuffer.ElementAt(_queryBuffer.Count - 1).Key == queryID)
                return query;

            // if the query is not the last one, remove it and add it again
            _queryBuffer.Remove(queryID);
            _queryBuffer.Add(queryID, query);
        }


        return null;
    }

    /// <summary>
    /// schreibt eine Abfrage in den Abfragepuffer
    /// </summary>
    /// <param name="queryID">ID</param>
    /// <param name="query">Abfrage</param>
    internal void setQuery(Guid queryID, string query)
    {
        // if the last query is the same as the current one, just update the query
        if (_queryBuffer.Count > 0 && _queryBuffer.ElementAt(_queryBuffer.Count - 1).Key == queryID)
        {
            _queryBuffer[_queryBuffer.ElementAt(_queryBuffer.Count - 1).Key] = query;
            return;
        }

        // if the query is already in the buffer, remove it
        if (_queryBuffer.ContainsKey(queryID))
            _queryBuffer.Remove(queryID);

        // if the buffer is full, remove the oldest query
        if (_queryBuffer.Count > MaxQueryBufferSize)
            _queryBuffer.Remove(_queryBuffer.First().Key);
        
        // add the query to the buffer
        _queryBuffer.Add(queryID, query);
    }

    /// <summary>
    /// Erstellen einer Datenbank
    /// </summary>
    /// <param name="config">Datenbankkonfiguration</param>
    /// <param name="noCheck">true = Existenz der Datenbank NICHT prüfen!</param>
    protected Database(IConfiguration config, bool noCheck = false)
    {
        Configuration = config;
    }

    /// <summary>
    /// Übersetzt einen Feld-, Tabellen- oder View-Namen in das 
    /// richtige Format für diese Datenbank unter Verwendung von Datenbank-Namenskonventionen
    /// </summary>
    /// <param name="original"></param>
    /// <returns>übersetzter Name</returns>
    public string GetName(string original)
    {
        switch (NamingConventions)
        {
            case eDatabaseNamingScheme.original:
                return original;
            case eDatabaseNamingScheme.lowercase:
                return original.ToLowerInvariant();
            case eDatabaseNamingScheme.uppercase:
                return original.ToUpper();
            default:
                return original;
        }
    }

    /// <summary>
    /// Übersetzt eine Konstante in eine datenbankspezifische Zeichenkette
    /// </summary>
    /// <param name="constant">Konstante</param>
    /// <returns>die Zeichenkette</returns>
    public virtual string GetConstant(eDatabaseConstant constant)
    {
        if (eDatabaseConstant.asc == constant)
            return @"ASC";
        if (eDatabaseConstant.desc == constant)
            return @"DESC";
        if (eDatabaseConstant.unique == constant)
            return @"UNIQUE";
        if (eDatabaseConstant.notunique == constant)
            return @"";

        return @"";
    }

    /// <summary>
    /// Namenskonventionen für Tabellen und Felder
    /// 
    /// Standard ist eDatabaseNamingScheme.original
    /// </summary>
    public eDatabaseNamingScheme NamingConventions { get; set; } = eDatabaseNamingScheme.original;

    /// <summary>
    /// Konfiguration dieser Datenbank
    /// </summary>
    public IConfiguration Configuration { get; set; }

    /// <summary>
    /// Wenn diese Eigenschaft true ist, werden alle Ereignisse (TraceStart, TraceEnd, AfterSave und AfterDelete) unterdrückt.
    /// </summary>
    public bool Silent { get; set; }

    /// <summary>
    /// Ereignisaktion für TraceStart.
    /// 
    /// Dieser Aktions-Delegate wird ausgeführt, wenn ein TraceStart-Ereignis in der Datenbank auftritt.
    /// </summary>
    public Action<TraceInfo>? TraceStart { get => Silent ? null : _traceStart; set => _traceStart = value; }

    /// <summary>
    /// Ereignisaktion für TraceEnd.
    /// 
    /// Dieser Aktions-Delegate wird ausgeführt, wenn ein TraceEnd-Ereignis in der Datenbank auftritt.
    /// </summary>
    public Action<TraceInfo>? TraceEnd { get => Silent ? null : _traceEnd; set => _traceEnd = value; }

    /// <summary>
    /// Ereignisaktion für AfterSave.
    /// 
    /// Dieser Aktions-Delegate wird im Falle eines AfterSave-Ereignisses in der Datenbank ausgeführt.
    /// </summary>
    public Action<IDataObject>? AfterSave { get => Silent ? null : _afterSave; set => _afterSave = value; }

    /// <summary>
    /// Ereignisaktion für AfterDelete.
    /// 
    /// Dieser Aktions-Delegate wird im Falle eines AfterSave-Ereignisses in der Datenbank ausgeführt.
    /// </summary>
    public Action<IDataObject>? AfterDelete { get => Silent ? null : _afterDelete; set => _afterDelete = value; }

    /// <summary>
    /// Aktion zum Protokollieren von Änderungen. 
    /// 
    /// Diese Aktion wird von Save aufgerufen und ein Array von 
    /// Tupeln mit geänderten Werten übergeben. Jedes Tupel enthält die ID des Eintrags, den alten Wert und den neuen Wert.
    /// </summary>
    public ILogger? Logger { get; set; }

    /// <summary>
    /// Übersetzer für die Datenbank.
    /// 
    /// Diese Methode muss in der konkreten Datenbankklasse überschrieben werden.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public virtual ITranslator Translator => throw new NotImplementedException();

    /// <summary>
    /// Überprüft Datenbanktypen (Tabellen und Ansichten)
    /// </summary>
    /// <param name="forceCheck">Prüfung aller Tabellen und Ansichten erzwingen (einschließlich vier Wiederherstellungen)</param>
    /// <param name="feedback">Aktion zur Rückmeldung über den Status der Prüfung</param>
    public bool Check(bool forceCheck, Action<string>? feedback = null)
    {
        return Check(Configuration.BaseTableTypes, Configuration.BaseViewTypes, feedback, forceCheck);
    }

    /// <summary>
    /// Datenbankstruktur prüfen
    /// </summary>
    /// <param name="tableTypes">Tabellenklassentypen</param>
    /// <param name="viewTypes">Ansichtsklassentypen</param>
    /// <param name="feedback">Rückmeldung bei Prüfung</param>
    /// <param name="force">vollständige Strukturprüfung erzwingen</param>
    /// <returns>true, wenn Struktur ohne Fehler geprüft</returns>
    public bool Check(List<Type> tableTypes, List<Type> viewTypes, Action<string>? feedback, bool force)
    {
        // remove all views if force is true
        if (force)
        {
            foreach (var basetype in viewTypes)
            {
                foreach (var viewType in basetype.GetChildTypesOf())
                {
                    TypeDescription tdesc = viewType.GetTypeDescription();

                    if (tdesc.IsView == false || tdesc.View == null) continue;

                    feedback?.Invoke(string.Format(CoreStrings.MSG_DELETEVIEW, tdesc.View.ViewName));

                    using var conn = GetAdminConnection();
                    if (conn.ExistView(tdesc.View.ViewName)) conn.DropView(tdesc.View.ViewName);
                }
            }
        }

        foreach (var basetype in tableTypes)
        {
            foreach (var tableType in basetype.GetChildTypesOf())
            {
                TypeDescription tdesc = tableType.GetTypeDescription();

                if (!tdesc.IsTable) continue;

                // geerbte werden nicht geprüft.
                if (tdesc.SiblingTable != null) continue;

                feedback?.Invoke(string.Format(CoreStrings.MSG_CHECKTABLE, tdesc.Table?.TableName));

                using var conn = GetAdminConnection();
                conn.Check(tableType, force);
            }
        }

        foreach (var basetype in viewTypes)
        {
            foreach (var viewType in basetype.GetChildTypesOf())
            {
                TypeDescription tdesc = viewType.GetTypeDescription();

                if (!tdesc.IsView) continue;

                feedback?.Invoke(string.Format(CoreStrings.MSG_CHECKVIEW, tdesc.View?.ViewName));

                using var conn = GetAdminConnection();
                conn.Check(viewType, force);
            }
        }

        return true;
    }

    /// <summary>
    /// Prüfen, ob die Datenbank existiert.
    /// 
    /// Diese Methode muss in der konkreten Datenbankklasse überschrieben werden.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public virtual bool Exist => throw new NotImplementedException();

    /// <summary>
    /// Ereignisaktion für AfterDelete.
    /// 
    /// Dieser Aktionsdelegat wird ausgeführt, wenn ein TraceBeforeExecute-Ereignis in der Datenbank auftritt.
    /// </summary>
    public Action<TraceInfo>? TraceBeforeExecute { get => Silent ? null : _beforeExecute; set => _beforeExecute = value; }

    /// <summary>
    /// Ereignisaktion für AfterDelete.
    /// 
    /// Dieser Aktionsdelegat wird ausgeführt, wenn ein TraceAfterExecute-Ereignis in der Datenbank auftritt.
    /// </summary>
    public Action<TraceInfo>? TraceAfterExecute { get => Silent ? null : _afterExecute; set => _afterExecute = value; }

    /// <summary>
    /// Name der Datenbank zur Darstellung in Auswahlen
    /// </summary>
    public string DatabaseName { get; set; } = @"<unknown>";

    /// <summary>
    /// Typ der Datenbank
    /// </summary>
    public virtual eDatabaseType DatabaseType => eDatabaseType.PostgreSql;


    /// <summary>
    /// Erstellen einer Datenbank.
    /// 
    /// Diese Methode muss in der konkreten Datenbankklasse überschrieben werden.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public virtual void Create() { throw new NotImplementedException(); }

    /// <summary>
    /// Öffnet eine Verbindung zur Datenbank. Gleiches gilt für GetMasterConnection().
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public virtual IConnection GetConnection() { throw new NotImplementedException(); }

    /// <summary>
    /// Öffnet eine Verbindung zur Master-/Hauptdatenbank.
    /// 
    /// Diese Methode muss in der konkreten Datenbankklasse überschrieben werden.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public virtual IConnection GetAdminConnection() { throw new NotImplementedException(); }

    /// <summary>
    /// Liest ein Objekt aus dem Cache
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>
    /// <param name="id">eindeutige ID des Objekts</param>
    /// <returns>das Objekt oder NULL</returns>
    public T? FromCache<T>(Guid id) where T : class, IDataObject, new()
    {
        if (_cache.ContainsKey(id) && _cache[id] is T retval)
            return retval;

        return null;
    }

    /// <summary>
    /// Ein Objekt im Cache speichern
    /// </summary>
    /// <param name="id">Id des Objekts</param>
    /// <param name="value">Wert des Objekts</param>
    public void ToCache(Guid id, IDataObject value)
    {
        if (_cache.ContainsKey(id)) 
        {    
            _cache[id] = value;
            return;
        }
        
        _cache.TryAdd(id, value);
    }

    /// <summary>
    /// Alle Einträge aus dem Cache entfernen
    /// </summary>
    public void ClearCache()
    {
        _cache.Clear();
    }

    /// <summary>
    /// entfernt alle Einträge von Type aus dem Cache
    /// </summary>
    /// <param name="entryType">Typ der zu entfernenden Einträge</param>
    public void ClearCache(Type entryType)
    {
        var toremove = _cache.Where(entry => entry.Value.GetType() == entryType); 
        foreach (var entry in toremove)
            _cache.Remove(entry.Key);
    }

    /// <summary>
    /// entfernt den Eintrag mit der angegebenen ID aus dem Cache
    /// </summary>
    /// <param name="id">Id des Eintrags</param>
    public void ClearCache(Guid id)
    {
        if (_cache.ContainsKey(id))
            _cache.Remove(id);
    }

    #region IDatasource

    /// <summary>
    /// Verbindungsstring zur Datenbank.
    /// </summary>
    public string ConnectString { get => Configuration.ConnectionString ; init => throw new Exception(@"ConnectString can not be set in this Database. Use Database.Configration to set the ConnectString."); }

    /// <summary>
    /// Zugangsdaten für die Datenbank (Benutzer, Kennwort).
    /// </summary>
    public Credentials? Credentials { get => Configuration.Credentials; set => Configuration.Credentials = value; }

    /// <summary>
    /// Erstellt eine Datenbankverbindung.
    /// </summary>
    /// <returns>DbConnection-Objekt</returns>
    public DbConnection Connect()
    {
        return GetConnection().CurrentConnection!;
    }

    /// <summary>
    /// Erstellt aus einer SQL-Abfrage eine DbCommand.
    /// </summary>
    /// <param name="query">auszuführende Abfrage</param>
    /// <param name="parameters">Parameter für die Abfrage</param>
    /// <param name="variablen">Variablenwerte für die Abfrage</param>
    /// <returns>DataTable</returns>
    public virtual DbCommand GetCommand(string query, IList<VariableUserValue>? variablen = null, params object[] parameters)
    { throw new NotImplementedException(@"Must be implemented in concrete Database!"); }

    /// <summary>
    /// Erstellt aus einer SQL-Abfrage eine DbCommand.
    ///
    /// Beim Erstellen wird der übergebene, datenbankspezifische Übersetzer verwendet.
    /// </summary>
    /// <param name="translator">datenbankspezifischer Translator</param>
    /// <param name="query">auszuführende Abfrage</param>
    /// <param name="parameters">Parameter für die Abfrage</param>
    /// <param name="variablen">Variablenwerte für die Abfrage</param>
    /// <returns>DataTable</returns>
    public virtual DbCommand GetCommand(ITranslator translator, string query, IList<VariableUserValue>? variablen = null, params object[] parameters)
    { throw new NotImplementedException(@"Must be implemented in concrete Database!"); }

    /// <summary>
    /// Query ausführen und einen Reader zurückgeben, über den die selektierten Daten gelesen werden können.
    /// <seealso cref="System.Data.Common.DbCommand.ExecuteReader()"/>
    /// </summary>
    /// <param name="connection">Datenbankverbindung</param>
    /// <param name="command">Abfrage</param>
    /// <returns>Anzahl der betroffenen Zeilen</returns>
    public DbDataReader ExecuteReader(DbConnection connection, DbCommand command)
    {
        command.Connection = connection;

        return command.ExecuteReader();
    }

    /// <summary>
    /// Query ausführen und ein DataTable zurückgeben, dass die gelesenen Informationen enthält.
    /// </summary>
    /// <param name="connection">Datenbankverbindung</param>
    /// <param name="command">Abfrage</param>
    /// <returns>Anzahl der betroffenen Zeilen</returns>
    public DataTable ExecuteTable(DbConnection connection, DbCommand command)
    {
        command.Connection = connection;

        DataTable ret = new();
        ret.Load(command.ExecuteReader());

        return ret;
    }


    /// <summary>
    /// Query ausführen und die Anzahl der betroffenen Zeilen zurückgeben.
    /// <seealso cref="System.Data.Common.DbCommand.ExecuteNonQuery"/>
    /// </summary>
    /// <param name="connection">Datenbankverbindung</param>
    /// <param name="command">Abfrage</param>
    /// <returns>Anzahl der betroffenen Zeilen</returns>
    public int ExecuteCommand(DbConnection connection, DbCommand command)
    {
        command.Connection = connection;
        return command.ExecuteNonQuery();
    }

    /// <summary>
    /// Query ausführen und einen einzelnen Wert zurückgeben.
    /// <seealso cref="System.Data.Common.DbCommand.ExecuteScalar"/>
    /// </summary>
    /// <param name="connection">Datenbankverbindung</param>
    /// <param name="command">Abfrage</param>
    /// <typeparam name="T">Typ des zu ermittelnden Wertes - stimmt dieser nicht mit der Rückgabe aus der Datenbank überein, 
    /// wird eine Exception ausgelöst.</typeparam>
    /// <returns>der ermittelte Wert oder NULL</returns>
    public T? ExecuteCommandScalar<T>(DbConnection connection, DbCommand command)
    {
        command.Connection = connection;
        var obj = command.ExecuteScalar();

        if (obj == null || obj == DBNull.Value)
            return default;

        if (obj is T retval)
            return retval;

        throw new Exception($@"Invalid return type from ExecuteCommandScalar (await {typeof(T).FullName}, got {obj.GetType().FullName})");
    }

    /// <summary>
    /// Liefert das Schema der Datenbank
    /// </summary>
    /// <returns>Schema der Datenbank</returns>
    public virtual DatabaseScheme GetScheme()
    { throw new NotImplementedException(@"Must be implemented in concrete Database!"); }

    /// <inheritdoc />
    public virtual BindingList<DatasourceField> GetFieldInformations() 
    { throw new NotImplementedException(@"Must be implemented in concrete Database!"); }

    /// <inheritdoc />
    public virtual SortedDictionary<string, DatasourceField> AsDictionary(bool ignoreGuid = false, string praefix = "#", string suffix = "#")
    {
        SortedDictionary<string, DatasourceField> ret = [];

        var fields = GetFieldInformations();

        foreach (var field in fields)
        {
            if (field.FieldType == typeof(Guid) && ignoreGuid) continue;

            ret.Add($"{praefix}{field.FieldName}{suffix}", field);
        }

        return ret;
    }

    /// <inheritdoc />
    public virtual void LoadFrom<TModel>(TModel parent) where TModel : IModel
    { throw new NotImplementedException(@"Must be implemented in concrete Database!"); }

    /// <inheritdoc />
    public virtual void LoadFrom(DataRow row)
    { throw new NotImplementedException(@"Must be implemented in concrete Database!"); }

    /// <summary>
    /// Kommentar/Beschreibung zu einer Spalte in einer Tabelle.
    /// </summary>
    /// <param name="tablename">Tabelle</param>
    /// <param name="columnname">Spalte</param>
    /// <returns>Beschreibung (kann leer sein)</returns>
    public string GetComment(string tablename, string columnname) 
    { throw new NotImplementedException(@"Must be implemented in concrete Database!"); }
    #endregion
}