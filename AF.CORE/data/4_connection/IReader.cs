namespace AF.DATA;

/// <summary>
/// Interface for a forward reader of a database
/// </summary>
public interface IReader<out T> : IDisposable where T : IDataObject, new()
{
    /// <summary>
    /// Lesen eines Objekts/Datensatzes vom Leser
    /// </summary>
    /// <returns>das gelesene Objekt/Datensatz</returns>
    T? Read();

    /// <summary>
    /// schließt den Leser
    /// </summary>
    void Close();

    /// <summary>
    /// Liefert true, wenn der Leser das Ende erreicht hat....
    /// </summary>
    bool Eof { get; }
}