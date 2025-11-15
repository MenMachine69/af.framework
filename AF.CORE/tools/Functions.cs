using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace AF.CORE;

/// <summary>
/// Beschreibung eines Custom-Serializer für benutzerdefinierte Serialisierung von Objekten.
/// </summary>
public interface ICustomSerializer
{
    /// <summary>
    /// Deserialisieren aus einem Byte-Array
    /// </summary>
    /// <typeparam name="T">Typ der zu deserialisierenden Klasse</typeparam>
    /// <param name="data">Daten</param>
    /// <returns>das deserialisierte Objekt</returns>
    T DeserializeFromBytes<T>(byte[] data) where T : new();

    /// <summary>
    /// Deserialisieren aus einem Stream
    /// </summary>
    /// <typeparam name="T">Typ der zu deserialisierenden Klasse</typeparam>
    /// <param name="stream">Daten-Stream</param>
    /// <returns>das deserialisierte Objekt</returns>
    T DeserializeFromStream<T>(Stream stream) where T : new();

    /// <summary>
    /// Deserialisieren aus einem string
    /// </summary>
    /// <typeparam name="T">Typ der zu deserialisierenden Klasse</typeparam>
    /// <param name="data">Daten</param>
    /// <returns>das deserialisierte Objekt</returns>
    T DeserializeFromString<T>(string data) where T : new();
    /// <summary>
    /// Deserialisieren aus einer Datei
    /// </summary>
    /// <typeparam name="T">Typ der zu deserialisierenden Klasse</typeparam>
    /// <param name="file">Datei, die die serialisierten Daten enthält</param>
    /// <returns>das deserialisierte Objekt</returns>
    T DeserializeFromFile<T>(FileInfo file) where T : new();

    /// <summary>
    /// Ein Objekt in einen string serialisieren
    /// </summary>
    /// <typeparam name="T">Typ der zu serialisierenden Klasse</typeparam>
    /// <param name="obj">das zu serialsierende Objekt</param>
    /// <returns>string, der das serialisierte Objekt enthält</returns>
    string SerializeToString<T>(T obj) where T : new();

    /// <summary>
    /// Ein Objekt in einen string serialisieren
    /// </summary>
    /// <typeparam name="T">Typ der zu serialisierenden Klasse</typeparam>
    /// <param name="obj">das zu serialsierende Objekt</param>
    /// <param name="stream">Stream in den serialsiert wird</param>
    void SerializeToStream<T>(T obj, Stream stream) where T : new();

    /// <summary>
    /// Ein Objekt in eine Datei serialisieren
    /// </summary>
    /// <typeparam name="T">Typ der zu serialisierenden Klasse</typeparam>
    /// <param name="obj">das zu serialsierende Objekt</param>
    /// <param name="file">Datei, in die serialsiert wird</param>
    void SerializeToFile<T>(T obj, FileInfo file) where T : new();

    /// <summary>
    /// Ein Objekt in eine byte[] serialisieren
    /// </summary>
    /// <typeparam name="T">Typ der zu serialisierenden Klasse</typeparam>
    /// <param name="obj">das zu serialsierende Objekt</param>
    /// <returns>byte[], der das serialisierte Objekt enthält</returns>
    byte[] SerializeToBytes<T>(T obj) where T : new();
}

/// <summary>
/// Type-Converter für Json
/// </summary>
public class JsonConverterForType : JsonConverter<Type>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="typeToConvert"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public override Type Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        // Caution: Deserialization of type instances like this 
        // is not recommended and should be avoided
        // since it can lead to potential security issues.

        // If you really want this supported (for instance if the JSON input is trusted):
        // string assemblyQualifiedName = reader.GetString();
        // return Type.GetType(assemblyQualifiedName);
        throw new NotSupportedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    /// <param name="options"></param>
    public override void Write(
        Utf8JsonWriter writer,
        Type value,
        JsonSerializerOptions options
    )
    {
        string assemblyQualifiedName = value.AssemblyQualifiedName!;
        // Use this with caution, since you are disclosing type information.
        writer.WriteStringValue(assemblyQualifiedName);
    }
}

/// <summary>
/// universelle Funktionen
/// </summary>
public static class Functions
{
    private static JsonSerializerOptions? defaultJsonSerializerOptions = null;

    /// <summary>
    /// Custom-Serializer für benutzerdefinierte Serialisierung von Objekten.
    /// </summary>
    public static ICustomSerializer? CustomSerializer { get; set; }

    /// <summary>
    /// Standardoptionen für die Serialisierung via Json
    /// </summary>
    public static JsonSerializerOptions DefaultJsonOptions
    {
        get
        {
            if (defaultJsonSerializerOptions == null)
            {
                defaultJsonSerializerOptions = new()
                {
                    WriteIndented = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    IgnoreReadOnlyFields = true,
                    IgnoreReadOnlyProperties = true
                };
                defaultJsonSerializerOptions.Converters.Add(new JsonConverterForType());
            }

            return defaultJsonSerializerOptions;
        }
    }

    /// <summary>
    /// Standardoptionen für die Serialisierung via Xml
    /// </summary>
    public static XmlWriterSettings DefaultXmlSettings { get; } = new()
    {
        Encoding = Encoding.UTF8
    };

    #region serialization
    /// <summary>
    /// Serialisiert ein Objekt in ein Byte-Array
    /// </summary>
    /// <param name="obj">zu serialisierendes Objekt</param>
    /// <returns>Byte-Array mit serialisiertem Objekt</returns>
    public static byte[] SerializeToJsonBytes(object obj)
    {
        return JsonSerializer.SerializeToUtf8Bytes(obj, DefaultJsonOptions);
    }

    /// <summary>
    /// Serialisieren eines Objekts in eine Datei
    /// </summary>
    /// <param name="obj">zu serialisierendes Objekt</param>
    /// <param name="file">Datei, die serialisiert werden soll</param>
    public static void SerializeToJsonFile(object obj, FileInfo file)
    {
        if (file.Directory == null)
            throw new NullReferenceException(CoreStrings.ERR_FILE_NODIRECTORY);

        if (!file.Directory.Exists)
            file.Directory.Create();

        using FileStream stream = file.Create();
        SerializeToJsonStream(obj, stream);
        stream.Flush();
    }

    /// <summary>
    /// Serialisieren eines Objekts in einen Stream
    /// </summary>
    /// <param name="obj">zu serialisierendes Objekt</param>
    /// <param name="stream">Stream, in den serialisiert werden soll</param>
    public static void SerializeToJsonStream(object obj, Stream stream)
    {
        SerializeToJson(obj, stream, DefaultJsonOptions);
    }

    /// <summary>
    /// Serialisieren eines Objekts in einen String
    /// </summary>
    /// <param name="obj">zu serialisierendes Objekt</param>
    public static string SerializeToJsonString(object obj)
    {
        return JsonSerializer.Serialize(obj, DefaultJsonOptions);
    }

    /// <summary>
    /// Serialisieren eines Objekts in eine Datei
    /// </summary>
    /// <param name="obj">zu serialisierendes Objekt</param>
    /// <param name="stream">Stream, in dem serialisiert werden soll</param>
    /// <param name="options">Optionen</param>
    public static void SerializeToJson(object obj, Stream stream, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(stream, obj, options);
    }

    /// <summary>
    /// Serialisiert ein Objekt in einen XML-String
    /// </summary>
    /// <param name="obj">zu serialisierendes Objekt</param>
    /// <returns>xml string</returns>
    public static string SerializeToXmlString(object obj)
    {
        StringBuilder output = StringBuilderPool.GetStringBuilder();

        using (XmlWriter stream = XmlWriter.Create(output, DefaultXmlSettings))
        {
            XmlSerializer serializer = new(obj.GetType());
            serializer.Serialize(stream, obj);
            stream.Flush();
        }

        var ret = output.ToString();
        StringBuilderPool.ReturnStringBuilder(output);

        return ret;
    }

    /// <summary>
    /// Serialisiert ein Objekt via XML zu einem Byte-Array
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static byte[] SerializeToXmlBytes(object obj)
    {
        using MemoryStream output = new();

        using (XmlWriter stream = XmlWriter.Create(output, DefaultXmlSettings))
        {
            XmlSerializer serializer = new(obj.GetType());
            serializer.Serialize(stream, obj);
            stream.Flush();
        }

        return output.ToArray();
    }

    /// <summary>
    /// Serialisiert ein Objekt via XML in einen Stream
    /// </summary>
    /// <param name="obj">zu serialsierendes Objekt</param>
    /// <param name="output">Stream, in den serialisiert wird</param>
    public static void SerializeToXmlStream(object obj, Stream output)
    {
        using XmlWriter stream = XmlWriter.Create(output, DefaultXmlSettings);
        XmlSerializer serializer = new(obj.GetType());
        serializer.Serialize(stream, obj);
        stream.Flush();
    }

    /// <summary>
    /// Serialisieren zu XmlFile
    /// </summary>
    /// <param name="obj">Objekt, das serialisiert werden soll</param>
    /// <param name="file">Datei zum Speichern der serialisierten Daten</param>
    public static void SerializeToXmlFile(object obj, FileInfo file)
    {
        if (file.Directory == null)
            throw new NullReferenceException(CoreStrings.ERR_FILE_NODIRECTORY);

        if (!file.Directory.Exists)
            file.Directory.Create();

        using XmlWriter writer = XmlWriter.Create(file.FullName, DefaultXmlSettings);
        XmlSerializer serializer = new(obj.GetType());
        serializer.Serialize(writer, obj);
        writer.Flush();
    }


    /// <summary>
    /// Deserialisiere ein Objekt byte[], das das Objekt als serialisiertes json enthält.
    /// </summary>
    /// <param name="data">serialisiertes Objekt</param>
    /// <param name="allownull">default(T), wenn beim Deserialisieren des Objekts ein Fehler auftritt</param>
    /// <typeparam name="T">Typ des Objekts</typeparam>
    /// <returns>das deserialisierte Objekt oder null</returns>
    public static T? DeserializeJsonBytes<T>(byte[]? data, bool allownull = false) where T : new()
    {
        if (data == null || data.Length < 1)
        {
            if (allownull) return default(T);

            throw new ArgumentNullException($"Keine oder leere Daten zur Deserialisiering nach Typ {typeof(T).FullName} übergeben.", nameof(data));
        }

        try
        {
            return (T?)DeserializeJsonBytes(typeof(T), data, allownull: allownull);
        }
        catch
        {
            if (allownull)
                return default(T);

            throw;
        }
    }

    /// <summary>
    /// Deserialisiere ein Objekt byte[], das das Objekt als serialisiertes json enthält.
    /// </summary>
    /// <param name="array">serialisiertes Objekt</param>.
    /// <param name="type">Typ des Objekts</param>
    /// <param name="allownull">Rückgabe von NULL erlaubt, wen Deserialisierung nicht möglich ist</param>
    /// .
    /// <returns>das deserialisierte Objekt oder null</returns>
    public static object? DeserializeJsonBytes(Type type, byte[]? array, bool allownull = false)
    {
        if (array == null || array.Length < 1)
        {
            if (allownull) return null;

            throw new ArgumentNullException($"Keine oder leere Daten zur Deserialisiering nach Typ {type.FullName} übergeben.", nameof(array));
        }

        try
        {
            return JsonSerializer.Deserialize(array, type, DefaultJsonOptions);
        }
        catch
        {
            if (allownull)
                return null;

            throw;
        }
    }

    /// <summary>
    /// Objekt aus einer json-Datei deserialisieren
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>.
    /// <param name="file">Datei, die das serialisierte Objekt enthält</param>.
    /// <returns>deserialisiertes Objekt</returns>
    public static T? DeserializeJsonFile<T>(FileInfo file) where T : new()
    {
        if (!file.Exists)
            throw new FileNotFoundException(string.Format(CoreStrings.ERR_FILE_NOTFOUND, file.FullName));

        using FileStream stream = file.OpenRead();
        return DeserializeJsonStream<T>(stream);
    }

    /// <summary>
    /// Deserialisiere Objekt aus einem json-Stream
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>.
    /// <param name="stream">Stream, der das serialisierte Objekt enthält</param>.
    /// <returns>das deserialisierte Objekt oder ein neues Objekt von T</returns>
    public static T? DeserializeJsonStream<T>(Stream stream) where T : new()
    {
        T? ret = default;

        var read = JsonSerializer.Deserialize(stream, typeof(T), DefaultJsonOptions);

        if (read is T tval) return tval;

        return ret;
    }

    /// <summary>
    /// Deserialisiere Objekt aus einem JSON String
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>.
    /// <param name="content">Via JSON serialisierte Daten als string</param>.
    /// <returns>das deserialisierte Objekt oder ein neues Objekt von T</returns>
    public static T? DeserializeJsonString<T>(string content) where T : new()
    {
        T? ret = default;

        var read = JsonSerializer.Deserialize(content, typeof(T), DefaultJsonOptions);

        if (read is T tval) return tval;

        return ret;
    }


    /// <summary>
    /// Objekt aus einer Xml-Datei deserialisieren
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>.
    /// <param name="file">Datei, die das serialisierte Objekt enthält</param>.
    /// <returns>deserialisiertes Objekt</returns>
    public static T? DeserializeXmlFile<T>(FileInfo file) where T : new()
    {
        T? ret = default;

        if (!file.Exists)
            throw new FileNotFoundException(string.Format(CoreStrings.ERR_FILE_NOTFOUND, file.FullName));

        using StreamReader stream = new(file.FullName, Encoding.UTF8);
        XmlSerializer bin = new(typeof(T));
        var read = bin.Deserialize(stream);

        if (read is T tval) return tval;

        return ret;
    }

    /// <summary>
    /// Deserialisieren von einer XML-Zeichenkette
    /// </summary>
    /// <typeparam name="T">Typ des deserialisierten Objekts</typeparam>
    /// <param name="data">XML-String</param>
    /// <returns>deserialisiertes Objekt</returns>
    public static T? DeserializeXmlString<T>(string data) where T : new()
    {
        T? result = default;

        using MemoryStream stream = new(Encoding.UTF8.GetBytes(data));
        XmlSerializer bin = new(typeof(T));
        var read = bin.Deserialize(stream);

        if (read is T tval) result = tval;

        return result;
    }

    /// <summary>
    /// Deserialisieren von einer XML-Zeichenkette
    /// </summary>
    /// <typeparam name="T">Typ des deserialisierten Objekts</typeparam>
    /// <param name="data">XML-Bytes</param>
    /// <returns>deserialisiertes Objekt</returns>
    public static T? DeserializeXmlBytes<T>(byte[] data) where T : new()
    {
        T? result = default;

        using MemoryStream stream = new(data);
        XmlSerializer bin = new(typeof(T));
        var read = bin.Deserialize(stream);

        if (read is T tval) result = tval;

        return result;
    }

    /// <summary>
    /// Deserialisieren von einer XML-Zeichenkette
    /// </summary>
    /// <typeparam name="T">Typ des deserialisierten Objekts</typeparam>
    /// <param name="data">XML-Stream</param>
    /// <returns>deserialisiertes Objekt</returns>
    public static T? DeserializeXmlStream<T>(Stream data) where T : new()
    {
        T? result = default;

        XmlSerializer bin = new(typeof(T));
        var read = bin.Deserialize(data);

        if (read is T tval) result = tval;

        return result;
    }
    #endregion

    #region Compression (ZIP)
    /// <summary>
    /// Komprimiert ein byte[] via ZIP
    /// </summary>
    /// <param name="array">zu komprimierende Daten</param>.
    /// <returns>Byte-Array mit den komprimierten Daten</returns>.
    public static byte[] Compress(byte[] array)
    {
        using var memoryStream = new MemoryStream();
        using var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress);
        gZipStream.Write(array, 0, array.Length);

        return memoryStream.ToArray();
    }

    /// <summary>
    /// dekomprimiert über ZIP komprimierte byte[]
    /// </summary>
    /// <param name="array">komprimierte Daten</param>.
    /// <returns> unkomprimierte Daten</returns>
    public static byte[] Decompress(byte[] array)
    {
        using var memoryStream = new MemoryStream(array);
        using var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
        using var memoryStreamOutput = new MemoryStream();
        gZipStream.CopyTo(memoryStreamOutput);
        return memoryStreamOutput.ToArray();
    }
    #endregion


    /// <summary>
    /// gibt aktuell zugewiesenen Speicher zurück
    /// </summary>
    /// <returns></returns>
    public static long GetAllocatedMemory()
    {
        // Abrufen des aktuellen Prozesses
        var process = System.Diagnostics.Process.GetCurrentProcess();

        // Rückgabe des aktuell zugewiesenen Speichers in Bytes
        return process.PrivateMemorySize64;
    }

    /// <summary>
    /// Lädt eine eingebettete Datei als Byte-Array
    /// </summary>
    /// <param name="filename">vollst. Name der Datei</param>
    /// <param name="fromAssembly">Assembly, von der geladen wird</param>
    /// <returns></returns>
    public static byte[] LoadEmbeddedFile(string filename, Assembly fromAssembly)
    {
        byte[] result = [];
        string[] ressourcen = fromAssembly.GetManifestResourceNames();


        using (Stream? stream = fromAssembly.GetManifestResourceStream(filename))
        {
            if (stream != null)
            {
                result = new byte[stream.Length];
                _ = stream.Read(result, 0, result.Length);
            }
        }
        return result;
    }
}
