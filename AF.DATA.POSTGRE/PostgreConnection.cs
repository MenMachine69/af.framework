using System.Data;

namespace AF.DATA;

/// <summary>
/// Verbindung für eine AF.DATA.PostgreDatabase
/// </summary>
public class PostgreConnection : Connection<NpgsqlConnection, NpgsqlCommand, NpgsqlParameter, NpgsqlTransaction>
{
    #region constructors

    /// <summary>
    /// Konstrukteur
    /// 
    /// RUFEN SIE DIESEN KONSTRUKTOR NICHT IN IHREM EIGENEN CODE AUF!
    /// </summary>
    /// <param name="database">Datenbank, zu der die Verbindung erstellt wird</param>
    /// <param name="admin">Verwendung von Admin-Anmeldedaten für die Verbindung</param>
    internal PostgreConnection(PostgreDatabase database, bool admin) : base(database)
    {
        if (database.Configuration == null || string.IsNullOrEmpty(database.Configuration.ConnectionString))
            throw new(@"Missing configuration for the database (ConnectString is empty or Configuration is not assigned).");

        CurrentConnection = new NpgsqlConnection(database.Configuration.GetConnectString(admin));
        CurrentConnection.Open();
    }

#endregion

    /// <summary>
    /// Erzeugt eine neue Transaktion...
    /// </summary>
    public override void BeginTransaction()
    {
        BeginTransaction(IsolationLevel.ReadCommitted); 
    }

    /// <summary>
    /// Erzeugt eine neue Transaktion mit dem angegebenen Verhalten
    /// </summary>
    /// <param name="behavior">Parameter, der die Transaktion steuert (Standard ist 'IsolationLevel.ReadCommitted')</param>
    public void BeginTransaction(IsolationLevel behavior)
    {
        if (CurrentConnection == null)
            throw new AFDataException(@"There is no active connection to the database.", this);

        if (Transaction != null)
            throw new AFDataException(@"There is another active transaction for this connection.", this);

        Database.Logger?.BeginTransaction();

        Transaction = ((NpgsqlConnection)CurrentConnection).BeginTransaction(behavior);
    }

    /// <summary>
    /// Erstellt einen Vorwärtsleser für eine Abfrage.
    /// </summary>
    /// <param name="cmd">Abfragebefehl zur Ausführung</param>
    /// <returns>der Leser</returns>
    public override IDataReader GetReader(NpgsqlCommand cmd)
    {
        return cmd.ExecuteReader();
    }

    /// <summary>
    /// Erstellt einen Vorwärtsleser für eine Abfrage.
    /// </summary>
    /// <param name="cmd">Abfragebefehl zur Ausführung</param>
    /// <param name="recordCount">betroffene Aufzeichnungen</param>
    /// <returns>der Leser</returns>
    public override IDataReader GetReader(NpgsqlCommand cmd, out long recordCount)
    {
        recordCount = 0;

        return cmd.ExecuteReader();
    }
   
}


