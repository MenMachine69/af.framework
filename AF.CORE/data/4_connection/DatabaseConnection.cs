using System.Data.Common;
using System.Data;

namespace AF.DATA;

/// <summary>
/// Datenquelle für den Zugriff auf eine Datenbank.
/// 
/// Erlaubt den Zugriff auf Datenbank via ConnectString ohne ORM.
/// </summary>
public abstract class DatabaseConnection : IDatabaseConnection
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="connectString"></param>
    protected DatabaseConnection(string connectString)
    {
        ConnectString = connectString;
    }

    /// <summary>
    /// Name der Datenbank.
    /// </summary>
    public string DatabaseName { get; set; } = @"<unknown>";

    /// <summary>
    /// Typ der Datenbank.
    /// </summary>
    public virtual eDatabaseType DatabaseType => eDatabaseType.PostgreSql;

    /// <summary>
    /// Verbindungsstring zur Datenbank.
    /// </summary>
    public string ConnectString { get; init; }

    /// <summary>
    /// Zugangsdaten für die Datenbank (Benutzer, Kennwort).
    /// </summary>
    public Credentials? Credentials { get; set; }

    /// <summary>
    /// Erstellt eine Datenbankverbindung.
    /// </summary>
    /// <returns>DbConnection-Objekt</returns>
    public abstract DbConnection Connect();

    /// <summary>
    /// Erstellt aus einer SQL-Abfrage eine DbCommand.
    /// </summary>
    /// <param name="query">auszuführende Abfrage</param>
    /// <param name="parameters">Parameter für die Abfrage</param>
    /// <param name="variablen">Variablenwerte für die Abfrage</param>
    /// <returns>DataTable</returns>
    public abstract DbCommand GetCommand(string query, IList<VariableUserValue>? variablen = null, params object[] parameters);

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
    public abstract DbCommand GetCommand(ITranslator translator, string query, IList<VariableUserValue>? variablen = null, params object[] parameters);

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

        throw new Exception(
            $@"Invalid return type from ExecuteCommandScalar (await {typeof(T).FullName}, got {obj.GetType().FullName})");
    }

    /// <summary>
    /// Beschreibung einer Spalte in einer Tabelle ermitteln
    /// </summary>
    /// <param name="tablename">Tabelle</param>
    /// <param name="columnname">Spalte</param>
    /// <returns>Beschreibung</returns>
    public virtual string GetComment(string tablename, string columnname)
    {
        return string.Empty;
    }

    /// <summary>
    /// Liefert das Schema der Datenbank
    /// </summary>
    /// <returns>Schema der Datenbank</returns>
    public virtual DatabaseScheme GetScheme()
    {
        return new();
    }


    /// <inheritdoc />
    public virtual BindingList<DatasourceField> GetFieldInformations()
    {
        return [];
    }

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
    {
    }

    /// <inheritdoc />
    public virtual void LoadFrom(DataRow data) { }
}

