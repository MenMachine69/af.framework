using System.Data.Common;

namespace AF.DATA;

/// <summary>
/// Zugriff auf eine PostgreSql-Datenbank
/// </summary>
public class PostgreDatabase : Database
{
    private readonly PostgreTranslator _translator;

    /// <summary>
    /// PostgreSql-Datenbank erstellen
    /// </summary>
    /// <param name="config">Datenban-Konfiguration</param>
    /// <param name="noCheck">true = Existenz der Datenbank NICHT prüfen!</param>
    public PostgreDatabase(PostgreConfiguration config, bool noCheck = false) : base(config, noCheck)
    {
        _translator = new PostgreTranslator(this);

        NamingConventions = config.NamingConventions;

        if (noCheck) return;

        if (Exist == false)
            Create();
    }

    /// <summary>
    /// Übersetzer für die Datenbank
    /// </summary>
    public override ITranslator Translator => _translator;
    

    /// <summary>
    /// Erstellt eine Verbindung zur Datenbank.
    /// Alle Datenbankoperationen benötigen diese Verbindung.
    /// </summary>
    /// <returns>eine Verbindung zur Datenbank</returns>
    public override IConnection GetConnection()
    {
        return new PostgreConnection(this, false);
    }

    /// <summary>
    /// Erstellt eine Verbindung zur Datenbank.
    /// Alle Datenbankoperationen benötigen diese Verbindung.
    /// </summary>
    /// <returns>eine Verbindung zur Datenbank</returns>
    public override IConnection GetAdminConnection()
    {
        return new PostgreConnection(this, true);
    }

    /// <summary>
    /// Übersetzt eine Konstante in eine datenbankspezifische Zeichenkette
    /// </summary>
    /// <param name="constant">Konstante</param>
    /// <returns>die Zeichenkette</returns>
    public override string GetConstant(eDatabaseConstant constant)
    {
        return eDatabaseConstant.asc == constant ? "" : base.GetConstant(constant);
    }

    /// <summary>
    /// Prüfen, ob Datenbank existiert
    /// </summary>
    public sealed override bool Exist
    {
        get
        {
            bool exist;

            try
            {
                NpgsqlConnectionStringBuilder builder = new(Configuration.GetConnectString(true));

                // zuerst versuchen eine NpgsqlConnection zur Datenbank herzustellen
                using (NpgsqlConnection connect = new(builder.ConnectionString))
                {
                    connect.Open();
                    exist = true;
                }
            }
            catch (Exception ex)
            {
                LastException = ex;

                if (ex.Message.StartsWith(@"3D000"))
                    exist = false;
                else 
                    throw;
            }

            if (!exist) return false;

            // Check required System tables
            using IConnection conn = GetAdminConnection();
            conn.Check<SystemDatabaseInformation>();

            return exist;
        }
    }

    /// <summary>
    /// Prüft, ob die Datenbank existiert und erstellt sie ggf....
    /// </summary>
    public sealed override void Create()
    {

        NpgsqlConnectionStringBuilder builder =
            new NpgsqlConnectionStringBuilder(Configuration.GetConnectString(true));
        
        if (builder.Database == null)
            throw new Exception($@"Configuration does not contain a Server/Hostname. ({Configuration.GetConnectString(true)})");

        string qry = $@"CREATE DATABASE {builder.Database.ToLower()} " +
                     $@"WITH OWNER = {builder.Username} " +
                     $@"ENCODING = 'UTF8'" +
                     $@"LC_COLLATE = 'German_Germany.1252'" +
                     $@"LC_CTYPE = 'German_Germany.1252'" +
                     $@"CONNECTION LIMIT = -1";


        using (NpgsqlConnection tmpConn = new NpgsqlConnection(
                   $@"Server={builder.Host};Port={builder.Port};" +
                   $@"Database=postgres;" +
                   $@"Userid={builder.Username};Password={builder.Password};" +
                   $@"Pooling=true;MinPoolSize=0;MaxPoolSize=100;" +
                   $@"ConnectionLifeTime=0;CommandTimeout=30;"))
        {
            using (NpgsqlCommand sqlCmd = new NpgsqlCommand(qry, tmpConn))
            {
                tmpConn.Open();
                sqlCmd.ExecuteNonQuery();
            }
        }


        // Create custom IIF function because PostgreSql does not have one
        qry = $@"CREATE FUNCTION IIF(  "+
              $@"condition boolean, true_result TEXT, false_result TEXT  "+
              $@") RETURNS TEXT LANGUAGE plpgsql AS $$  "+
              $@"BEGIN  "+
              $@"    IF condition THEN  "+
              $@"         RETURN true_result;  "+
              $@"    ELSE  "+
              $@"         RETURN false_result;  "+
              $@"    END IF;  "+
              $@"END  "+
              $@"$$;";

        using IConnection connection = GetAdminConnection();
        connection.ExecuteCommand(qry);
        connection.Check<SystemDatabaseInformation>();
    }

    /// <summary>
    /// Typ der Datenbank
    /// </summary>
    public override eDatabaseType DatabaseType => eDatabaseType.PostgreSql;


    /// <summary>
    /// Liefert das Schema der Datenbank
    /// </summary>
    /// <returns>Schema der Datenbank</returns>
    public override DatabaseScheme GetScheme()
    {
        string connectsring = Configuration.GetConnectString(true);
        using NpgsqlConnection conn = new(connectsring);

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
        NpgsqlCommand cmd = new();
        DataTools.ParseQuery<NpgsqlParameter, NpgsqlCommand>(cmd, _translator, query, variablen, parameters);
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
        DataTools.ParseQuery<NpgsqlParameter, NpgsqlCommand>(cmd, translator, query, variablen, parameters);
        return cmd;
    }

    #endregion
}


