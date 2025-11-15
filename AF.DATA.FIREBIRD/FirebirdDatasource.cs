using System.Data.Common;

namespace AF.DATA;

/// <summary>
/// einfache FirebirdSQL Datenquelle (ohne ORM)
/// 
/// <see cref="IDatabaseConnection"/>
/// </summary>
public class FirebirdDatasource : DatabaseConnection
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="connectString">ConnectString für die Datenbank</param>
    public FirebirdDatasource(string connectString) : base(connectString)
    {
        FbConnectionStringBuilder builder = new(connectString);
        DatabaseName = builder.Database;
    }

    /// <summary>
    /// Typ der Datenbank.
    /// </summary>
    public override eDatabaseType DatabaseType => eDatabaseType.FirebirdSql;

    
    /// <summary>
    /// Erstellt eine Datenbankverbindung unter Verwendung des ConnectStrings und der
    /// angegebenen Credentials.
    /// </summary>
    /// <returns>die Verbindung</returns>
    public override DbConnection Connect()
    {
        FbConnectionStringBuilder builder = new(ConnectString);

        if (Credentials == null) 
            return new FbConnection(builder.ConnectionString);
        
        builder.UserID = Credentials.Username;
        builder.Password = Credentials.Password;

        return new FbConnection(builder.ConnectionString);
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
        FbCommand cmd = new();
        DataTools.ParseQuery<FbParameter, FbCommand>(cmd, new FirebirdTranslator(DatabaseName), query, variablen: variablen, parameters);
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
        FbCommand cmd = new();
        DataTools.ParseQuery<FbParameter, FbCommand>(cmd, translator, query, variablen: variablen, parameters);
        return cmd;
    }

    /// <summary>
    /// Liefert das Schema der Datenbank
    /// </summary>
    /// <returns>Schema der Datenbank</returns>
    public override DatabaseScheme GetScheme()
    {
        using FbConnection conn = (FbConnection)Connect()!;

        var schema = conn.GetScheme();

        return schema;
    }
}
