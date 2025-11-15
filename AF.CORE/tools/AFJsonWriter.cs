using System.Text;
using System.Text.Json;

namespace AF.CORE;

/// <summary>
/// Einfach Json-Funktionen zum Schreiben von Json-Daten
/// </summary>
public class AFJsonWriter
{
    private readonly MemoryStream stream;
    private readonly Utf8JsonWriter writer;
    private bool endwritten = false;
    private byte[] result = [];

    /// <summary>
    /// Constructor
    /// </summary>
    public AFJsonWriter()
    {
        stream = new();
        writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
        writer.WriteStartObject();
    }

    /// <summary>
    /// Ergebnis als Bytes
    /// </summary>
    /// <returns></returns>
    public byte[] AsBytes()
    {
        checkEnd();

        return result;
    }

    /// <summary>
    /// Ergebnis als String
    /// </summary>
    /// <returns></returns>
    public string AsString()
    {
        checkEnd();

        return Encoding.UTF8.GetString(result);
    }


    private void checkEnd()
    {
        if (endwritten) return;

        writer.WriteEndObject();
        writer.Flush();

        result = stream.ToArray();
        stream.Close();
        writer.Dispose();
        stream.Dispose();

        endwritten = true;
    }

    /// <summary>
    /// Einen Wert schreiben.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="data"></param>
    /// <exception cref="ArgumentException"></exception>
    public void Write(string name, object? data)
    {
        if (data is null) writer.WriteNull(name);
        else if (data is string strval) writer.WriteString(name, strval);
        else if (data is short shortval) writer.WriteNumber(name, shortval);
        else if (data is int intval) writer.WriteNumber(name, intval);
        else if (data is long longval) writer.WriteNumber(name, longval);
        else if (data is decimal decval) writer.WriteNumber(name, decval);
        else if (data is float floatval) writer.WriteNumber(name, floatval);
        else if (data is double doubleval) writer.WriteNumber(name, doubleval);
        else if (data is byte byteval) writer.WriteNumber(name, byteval);
        else if (data is bool boolval) writer.WriteBoolean(name, boolval);
        else if (data is DateTime dtval) writer.WriteString(name, dtval.ToString("O"));
        else if (data is DateOnly dateval) writer.WriteString(name, dateval.ToDateTime(TimeOnly.MinValue).ToString("O"));
        else if (data is TimeOnly timeval) writer.WriteString(name, timeval.ToString("O"));
        else if (data is Guid guidval) writer.WriteString(name, guidval.ToString("D"));
        else throw new ArgumentException($"Typ {data.GetType().FullName} kann nicht geschrieben werden.");
    }
}