using System.Data;

namespace AF.DATA;

/// <summary>
/// Ein Vorwärts-Leser für eine Datenbank, der die Bearbeitung einzelner Datensätze ermöglicht....
/// </summary>
public class DataReader<T> : IReader<T> where T : IDataObject, new()
{
    private readonly IDataReader _reader;
    private readonly IConnection _connection;
    private readonly PropertyDescription[] _dict;


    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="reader">Leser, von dem gelesen werden soll</param>
    /// <param name="connection">Verbindung, die vom Leser verwendet wird</param>
    public DataReader(IDataReader reader, IConnection connection)
    {
        _connection = connection;
        _reader = reader;

        var cols = _reader.FieldCount;
        _dict = new PropertyDescription[cols];

        for (int i = 0; i < cols; i++)
        {
            string fieldname = reader.GetName(i);

            var fld = typeof(T).GetTypeDescription().Fields.Values.FirstOrDefault(f => f.Name.Equals(fieldname, StringComparison.OrdinalIgnoreCase));

            if (fld != null)
                _dict[i] = fld;
        }

        Eof = !reader.Read();
    }

    /// <summary>
    /// Liest ein Objekt aus dem Lesegerät
    /// </summary>
    /// <returns>das gelesene Objekt oder NULL</returns>
    public T? Read()
    {
        if (Eof)
            return default(T);

        T? ret = _connection.ReadFromReader<T>(_reader, _dict);

        Eof = !(_reader.Read());

        return ret;
    }

    /// <summary>
    /// Schließt den Leser und räumt auf.
    /// 
    /// DIE VERBINDUNG WIRD NICHT GESCHLOSSEN! Dies muss im aufrufenden Code geschehen.
    /// </summary>
    public void Close()
    {
        _reader.Close();
        _reader.Dispose();
    }

    /// <summary>
    /// true, wenn der Leser das Ende erreicht hat
    /// </summary>
    public bool Eof { get; private set; }

    /// <summary>
    /// clean-up
    /// </summary>
    public void Dispose()
    {
        Close();
    }
}
