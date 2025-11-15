using System.Data;

namespace AF.DATA;

/// <summary>
/// Connection for a FirebirdDatabase
/// </summary>
public class FirebirdConnection : Connection<FbConnection, FbCommand, FbParameter, FbTransaction>
{
    #region constructors

    /// <summary>
    /// Constructor
    /// 
    /// DO NOT CALL THIS CONSTRUCTOR FROM YOUR OWN CODE!
    /// </summary>
    /// <param name="database">Database to which the connection is created</param>
    /// <param name="admin">a connection with admin credentials</param>
    public FirebirdConnection(FirebirdDatabase database, bool admin) : base(database)
    {
        if (database.Configuration == null || string.IsNullOrEmpty(database.Configuration.ConnectionString))
            throw new(@"Missing configuration for the database (ConnectString is empty or Configuration is not assigned).");

        CurrentConnection = new FbConnection(database.Configuration.GetConnectString(admin));
        CurrentConnection.Open();
    }

    #endregion

    /// <summary>
    /// Creates a new Transaction...
    /// </summary>
    public override void BeginTransaction()
    {
        BeginTransaction(FbTransactionBehavior.LockWrite & FbTransactionBehavior.Shared);
    }

    /// <summary>
    /// Creates a new Transaction with the given behavior
    /// </summary>
    /// <param name="behavior">parameter that controls the transaction (default is 'FbTransactionBehavior.LockWrite &amp; FbTransactionBehavior.Shared')</param>
    public void BeginTransaction(FbTransactionBehavior behavior)
    {
        if (CurrentConnection == null)
            throw new AFDataException(@"There is no active connection to begin a transaction.", this);

        if (Transaction != null)
            throw new AFDataException(@"There is always a active transaction in the current connection.", this);

        Database.Logger?.BeginTransaction();

        FbTransactionOptions options = new() { TransactionBehavior = behavior };
        Transaction = ((FbConnection)CurrentConnection).BeginTransaction(options);
    }

    /// <summary>
    /// Create a forward reader for a query.
    /// </summary>
    /// <param name="cmd">Query command to execute</param>
    /// <returns>the reader</returns>
    public override IDataReader GetReader(FbCommand cmd)
    {
        return cmd.ExecuteReader(CommandBehavior.Default);
    }

    /// <summary>
    /// Create a forward reader for a query.
    /// </summary>
    /// <param name="cmd">Query command to execute</param>
    /// <param name="recordCount">affected recors</param>
    /// <returns>the reader</returns>
    public override IDataReader GetReader(FbCommand cmd, out long recordCount)
    {
        recordCount = 0;

        return cmd.ExecuteReader(CommandBehavior.Default);
    }
}


