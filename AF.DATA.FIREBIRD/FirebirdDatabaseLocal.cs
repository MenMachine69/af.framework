using System.Reflection;

namespace AF.DATA;

/// <summary>
/// Access to a local FirebirdSQL database (aka embedded database)
/// Attention! Using this needs to deploy the dll files for the 
/// embedded FirebirdSQL within your application!
/// </summary>
public class FirebirdDatabaseLocal : FirebirdDatabase
{
    /// <summary>
    /// Create access to a database
    /// </summary>
    /// <param name="dbfile">DB File (fullname includig Path, UNC is allowed)</param>
    /// <param name="databaseName">unique database name</param>
    /// <param name="noCheck">true = Existenz der Datenbank NICHT prüfen!</param>
    public FirebirdDatabaseLocal(string databaseName, string dbfile, bool noCheck = false) : base(_createConfig(databaseName, dbfile), noCheck)
    {
        Configuration.AllowDropColumns = true;

        if (noCheck) return;

        if (Exist == false)
            Create();
    }


    private static FirebirdConfiguration _createConfig(string databaseName, string dbfile,
        eDatabaseNamingScheme namingScheme = eDatabaseNamingScheme.original)
    {
        string? path = Assembly.GetEntryAssembly()?.Location;

        if (path == null)
            throw new FileNotFoundException(@"Can't find the application directory.");

        path = Path.GetDirectoryName(path);

        if (path == null)
            throw new FileNotFoundException(@"Can't find the application directory.");

        if (!File.Exists(Path.Combine(path, @"fbclient.dll")))
            path = Path.Combine(path, @"fb_embedded");

        if (!File.Exists(Path.Combine(path, @"fbclient.dll")))
            throw new Exception(@$"Can't find file ({Path.Combine(path, @"fbclient.dll")}).");

        string connstring =
            $@"User=SYSDBA;Database={dbfile};ClientLibrary={Path.Combine(path, @"fbclient.dll")};DataSource=localhost;Port=3050;Dialect=3;Charset=UTF8;Collation=UTF8;Role=;Connectionlifetime=15;Pooling=true;MinPoolSize=0;MaxPoolSize=50;PacketSize=8192;ServerType = 1;";

        return new FirebirdConfiguration(databaseName, connstring) { NamingConventions = namingScheme };
    }
}
