using System.Data.Common;

namespace AF.DATA;

/// <summary>
/// einfache PostgreSQL Datenquelle (ohne ORM)
/// 
/// <see cref="IDatabaseConnection"/>
/// </summary>
public class PostgreDatasource : DatabaseConnection
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="connectString">ConnectString für die Datenbank</param>
    public PostgreDatasource(string connectString) : base(connectString)
    {
        NpgsqlConnectionStringBuilder builder = new(connectString);
        DatabaseName = builder.Database!;
    }

    /// <summary>
    /// Typ der Datenbank.
    /// </summary>
    public override eDatabaseType DatabaseType => eDatabaseType.PostgreSql;
    
    /// <summary>
    /// VARCHAR für String-Felder verwenden
    /// </summary>
    public bool UseVarChar { get; set; } = true;

    /// <summary>
    /// Erstellt eine Datenbankverbindung unter Verwendung des ConnectStrings und der
    /// angegebenen Credentials.
    /// </summary>
    /// <returns>die Verbindung</returns>
    public override DbConnection Connect()
    {
        NpgsqlConnectionStringBuilder builder = new(ConnectString);

        if (Credentials == null) 
            return new NpgsqlConnection(builder.ConnectionString);
        
        builder.Username = Credentials.Username;
        builder.Password = Credentials.Password;

        return new NpgsqlConnection(builder.ConnectionString);
    }

    /// <summary>
    /// Erstellt aus einer SQL-Abfrage eine DbCommand.
    /// </summary>
    /// <param name="query">auszuführende Abfrage</param>
    /// <param name="parameters">Parameter für die Abfrage</param>
    /// <param name="variablen">Variablenwerte für die Abfrage</param>
    /// <returns>DataTable</returns>
    public override DbCommand GetCommand(string query, IList<VariableUserValue>? variablen = null, params object[] parameters)
    {
        NpgsqlCommand cmd = new();
        DataTools.ParseQuery<NpgsqlParameter, NpgsqlCommand>(cmd, new PostgreTranslator(DatabaseName), query, variablen: variablen, parameters);
        return cmd;
    }

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
    public override DbCommand GetCommand(ITranslator translator, string query, IList<VariableUserValue>? variablen = null, params object[] parameters)
    {
        NpgsqlCommand cmd = new();
        DataTools.ParseQuery<NpgsqlParameter, NpgsqlCommand>(cmd, translator, query, variablen: variablen, parameters);
        return cmd;
    }

    /// <summary>
    /// Liefert das Schema der Datenbank
    /// </summary>
    /// <returns>Schema der Datenbank</returns>
    public override DatabaseScheme GetScheme()
    {
        using NpgsqlConnection conn = (NpgsqlConnection)Connect()!;

        var schema = conn.GetScheme();

        return schema;
    }
}
