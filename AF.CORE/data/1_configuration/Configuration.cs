namespace AF.DATA;

/// <summary>
/// Zugangsdaten für Datenbank
/// </summary>
public class Credentials
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="username">Benutzername (verschlüsselt)</param>
    /// <param name="password"></param>
    public Credentials(string username, string password)
    {
        Username = username;
        Password = password;
    }

    /// <summary>
    /// Benutzername
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Kennwort
    /// </summary>
    public string Password { get; }
}

/// <summary>
/// Basisklasse für die Datenbankkonfiguration (IConfiguration)
/// </summary>
public abstract class Configuration : IConfiguration
{
    private Credentials? _adminCredentials;

    /// <inheritdoc/>
    public List<Type> BaseTableTypes { get; set; } = [];

    /// <inheritdoc />
    public List<Type> BaseViewTypes { get; set; } = [];

    /// <inheritdoc />
    public string ConnectionString { get; set; } = "";

    /// <inheritdoc />
    public string DatabaseName { get; set; } = "";

    /// <inheritdoc />
    public eConflictMode ConflictMode { get; set; } = eConflictMode.LastWins;


    /// <inheritdoc />
    public virtual eDatabaseType DatabaseType => eDatabaseType.PostgreSql;

    /// <inheritdoc />
    public bool AllowDropColumns { get; set; }
    
    /// <inheritdoc />
    public Credentials? Credentials { get; set; }

    /// <inheritdoc />
    public Credentials? AdminCredentials
    {
        get => _adminCredentials ?? Credentials;
        set => _adminCredentials = value;
    }

    /// <inheritdoc />
    public virtual IDatabase Create()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    /// <exception cref="NotImplementedException"></exception>
    public virtual string GetConnectString(bool admin)
    {
        throw new NotImplementedException();
    }
}

