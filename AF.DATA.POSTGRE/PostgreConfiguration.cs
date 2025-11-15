namespace AF.DATA;

/// <summary>
/// Konfiguration für eine PostgreSQL Server Datenbank
/// </summary>
public class PostgreConfiguration : Configuration
{
    /// <summary>
    /// Erstellen eines Konfigurationsobjekts
    /// </summary>
    /// <param name="databaseName">Einziger Name für diese Datenbank</param>
    /// <param name="connectString">Verbindungszeichenfolge für die Datenbank</param>
    public PostgreConfiguration(string databaseName, string connectString) 
    {
        ConnectionString = connectString;
        DatabaseName = databaseName;

        NpgsqlConnectionStringBuilder builder = new(ConnectionString);
        
        if (builder.Database == null)
            throw new(@"Configuration does not contain a database name.");
        
        if (builder.Host == null)
            throw new(@"Configuration does not contain a server/hostname.");
    }

    /// <summary>
    /// Erstellen eines Konfigurationsobjekts
    /// </summary>
    /// <param name="databaseName">Einziger Name für diese Datenbank</param>
    /// <param name="builder">ConnectstringBuilder für die Datenbank</param>
    public PostgreConfiguration(string databaseName, NpgsqlConnectionStringBuilder builder) 
    {
        DatabaseName = databaseName;
        ConnectionString = builder.ConnectionString;
    }

    /// <summary>
    /// Definiert, wie Tabellen-, View- und Feldnamen erstellt werden. 
    /// 
    /// Diese Option ist vor allem für Datenbanken nützlich, die ein Namensschema wie PostgreSQL verwenden.
    /// </summary>
    public eDatabaseNamingScheme NamingConventions => eDatabaseNamingScheme.lowercase;
    
    /// <summary>
    /// Typ der Datenbank...
    /// </summary>
    public override eDatabaseType DatabaseType => eDatabaseType.PostgreSql;

    /// <summary>
    /// Erzwingt die Verwendung von VARCHAR anstelle von TEXT für Stringfelder.
    /// 
    /// Sollte nur in bestehenden Datenbanken aus Kompatibilitätsgründen verwendet werden!
    /// </summary>
    public bool UseVarchar { get; set; }

    /// <summary>
    /// Erstellen eines Datenbankobjekts basierend auf der Konfiguration
    /// </summary>
    /// <returns>Datenbank</returns>
    public override IDatabase Create()
    {
        return new PostgreDatabase(this);
    }


    /// <inheritdoc />
    public override string GetConnectString(bool admin)
    {
        Credentials? credentials = admin ? AdminCredentials: Credentials;

        if (credentials == null) return ConnectionString;

        NpgsqlConnectionStringBuilder builder = new(ConnectionString)
        {
            Username = credentials.Username,
            Password = credentials.Password
        };

        return builder.ConnectionString;
    }
}



