using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AF.CORE;

/// <summary>
/// Einfach Json-Funktionen zum Lesen von Json-Daten
/// </summary>
public class AFJsonReader
{
    private readonly byte[] _jsonbytes;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFJsonReader(byte[] jsonbytes)
    {
        _jsonbytes = jsonbytes;
    }

    /// <summary>
    /// Liest die Json-Daten in das übergebene Objekt ein (wenn möglich)
    /// </summary>
    /// <param name="toobject"></param>
    public void ReadTo(object toobject)
    {
        Utf8JsonReader reader = new(_jsonbytes, new JsonReaderOptions
        {
            AllowTrailingCommas = true,
            CommentHandling = JsonCommentHandling.Skip
        });

        string propName = "";

        while (reader.Read())
        {
            var type = reader.TokenType;
            if (type == JsonTokenType.PropertyName)
                propName = reader.GetString() ?? "";
            else
            {
                if (propName.IsEmpty()) continue;

                var prop = toobject.GetType().GetProperty(propName);

                if (prop == null || prop.CanWrite == false) continue;

                if (prop.GetCustomAttribute(typeof(JsonIgnoreAttribute)) != null) 
                    continue;

                try
                {
                    if (type == JsonTokenType.String)
                    {
                        if (prop.PropertyType == typeof(Guid))
                            prop.SetValue(toobject, reader.GetGuid());
                        else
                            prop.SetValue(toobject, reader.GetString() ?? "");
                    }
                    else if (type == JsonTokenType.True)
                        prop.SetValue(toobject, true);
                    else if (type == JsonTokenType.False)
                        prop.SetValue(toobject, false);
                    else if (type == JsonTokenType.Number)
                        prop.SetValue(toobject, reader.GetInt32());
                }
                catch
                {
                    // ignoriere Lesefehler...
                }
            }
        }
    }
}