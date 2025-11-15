namespace AF.DATA;

/// <summary>
/// Ausnahme, die von einem IConnection-Objekt stammt.
/// </summary>
public class AFDataException : Exception
{
    /// <summary>
    /// Verbindung, bei der dieser Fehler aufgetreten ist.
    /// </summary>
    public IConnection Connection { get; init; }

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="exception">Meldung</param>
    /// <param name="connection">Datenbankverbindung, bei der dieser Fehler aufgetreten ist</param>
    public AFDataException(string exception, IConnection connection) : base(exception)
    {
        Connection = connection;
    }

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="exception">Meldung</param>
    /// <param name="connection">Datenbankverbindung, von der aus der Fehler aufgetreten ist.</param>
    /// <param name="innerexception">eingebettete Ausnahme</param>
    public AFDataException(string exception, Exception innerexception, IConnection connection) : base(exception, innerexception)
    {
        Connection = connection;
    }
}