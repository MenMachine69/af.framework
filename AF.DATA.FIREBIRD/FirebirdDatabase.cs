using System.Data.Common;

namespace AF.DATA;

/// <summary>
/// Access to a FirebirdSQL-Database
/// </summary>
public class FirebirdDatabase : Database
{
    private readonly FirebirdTranslator _translator;

    /// <summary>
    /// Bound to a Firebird-Database
    /// </summary>
    /// <param name="config">Datenbank-Konfiguration</param>
    /// <param name="noCheck">true = Existenz der Datenbank NICHT prüfen!</param>
    public FirebirdDatabase(FirebirdConfiguration config, bool noCheck = false) : base(config, noCheck)
    {
        _translator = new FirebirdTranslator(this);

        NamingConventions = config.NamingConventions;

        if (noCheck) return;

        if (Exist == false)
            Create();
    }

    /// <summary>
    /// Translator for the Database
    /// </summary>
    public override ITranslator Translator => _translator;

    /// <summary>
    /// Create a connection to the database.
    /// All database operations need this conenction.
    /// </summary>
    /// <returns>a connection to the database</returns>
    public override IConnection GetConnection()
    {
        return new FirebirdConnection(this, false);
    }

    /// <summary>
    /// Create a connection to the database.
    /// All database operations need this conenction.
    /// </summary>
    /// <returns>a connection to the database</returns>
    public override IConnection GetAdminConnection()
    {
        return new FirebirdConnection(this, true);
    }

    /// <summary>
    /// Check if database exist
    /// </summary>
    public sealed override bool Exist
    {
        get
        {
            bool exist;

            // try to connect
            using (FbConnection connect = new(Configuration.GetConnectString(true)))
            {
                try
                {
                    connect.Open();
                    exist = true;
                }
                catch (Exception ex)
                {
                    LastException = ex;

                    if (ex is FbException exfb && exfb.ErrorCode == 335544379) // unsupported on-disk structure
                        throw new(@"FirebirdSQL-Server version is to old.");

                    exist = false;
                }
            }

            if (!exist) return false;

            using IConnection connection = GetAdminConnection();
            connection.Check<SystemDatabaseInformation>();

            return exist;
        }
    }

    /// <summary>
    /// Checks whether the database exists and creates it if necessary...
    /// </summary>
    public sealed override void Create()
    {
        FbConnectionStringBuilder builder = new(Configuration.GetConnectString(true));
        FbConnection.CreateDatabase(Configuration.GetConnectString(true), builder.PacketSize);
        
        using IConnection connection = GetAdminConnection();
        connection.Check<SystemDatabaseInformation>();
    }

    /// <summary>
    /// Typ der Datenbank
    /// </summary>
    public override eDatabaseType DatabaseType => eDatabaseType.FirebirdSql;


    /// <summary>
    /// Liefert das Schema der Datenbank
    /// </summary>
    /// <returns>Schema der Datenbank</returns>
    public override DatabaseScheme GetScheme()
    {
        string connectsring = Configuration.GetConnectString(true);
        using FbConnection conn = new(connectsring);

        var schema = conn.GetScheme();

        return schema;
    }

    #region IDBConnection

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
        DataTools.ParseQuery<FbParameter, FbCommand>(cmd, _translator, query, variablen, parameters);
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
        DataTools.ParseQuery<FbParameter, FbCommand>(cmd, translator, query, variablen, parameters);
        return cmd;
    }

    #endregion
}


