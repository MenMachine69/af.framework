namespace AF.DATA;

/// <summary>
/// Configuration for an FirebirdSQL database
/// </summary>
public class FirebirdConfiguration : Configuration
{
    /// <summary>
    /// Create a configuration object
    /// </summary>
    /// <param name="databaseName">Unique Name for this database</param>
    /// <param name="connectString">Connectstring for the database</param>
    public FirebirdConfiguration(string databaseName, string connectString) 
    {
        DatabaseName = databaseName;

        ConnectionString = connectString;

        FbConnectionStringBuilder builder = new(ConnectionString);

        if (builder.Database == null)
            throw new($@"There is no database name configured ({ConnectionString})");

        if (builder.DataSource == null)
            throw new($@"There is no server/hostname configured ({ConnectionString})");
    }


    /// <summary>
    /// Create a configuration object
    /// </summary>
    /// <param name="databaseName">Unique Name for this database</param>
    /// <param name="builder">Connectstring for the database</param>
    public FirebirdConfiguration(string databaseName, FbConnectionStringBuilder builder) 
    {
        DatabaseName = databaseName;

        ConnectionString = builder.ConnectionString;
    }

    /// <summary>
    /// Defines how table, view and fieldnames are created 
    /// 
    /// This option is usefull especially for Databases that use a naming scheme like PostgreSQL.
    /// </summary>
    public eDatabaseNamingScheme NamingConventions { get; set; } = eDatabaseNamingScheme.original;

    
    /// <summary>
    /// Typ of Database...
    /// </summary>
    public override eDatabaseType DatabaseType => eDatabaseType.FirebirdSql;

    

    /// <summary>
    /// Create a database object based on the configuration
    /// </summary>
    /// <returns></returns>
    public override IDatabase Create()
    {
        return new FirebirdDatabase(this);
    }

    /// <inheritdoc />
    public override string GetConnectString(bool admin)
    {
        string connString = ConnectionString;

        Credentials? credentials = admin ? AdminCredentials : Credentials;

        FbConnectionStringBuilder builder = new(connString);

        if (credentials == null) return builder.ConnectionString;
        
        builder.UserID = credentials.Username;
        builder.Password = credentials.Password;

        return builder.ConnectionString;
    }
}



