using System.Text.Json;

namespace AF.CORE;

/// <summary>
/// Erweiterungsmethoden für Dictionary
/// </summary>
public static class DictionaryEx
{
    /// <summary>
    /// Wandelt ein Dictionary in einen JSON String um
    /// </summary>
    /// <typeparam name="TValue">Value-Typ des Dictionary</typeparam>
    /// <param name="dict">das Diactionary</param>
    /// <returns>JSON-String des Dictionarys</returns>
    public static string ToJson<TValue>(this Dictionary<string, TValue?> dict)
    {
        return JsonSerializer.Serialize(dict, new JsonSerializerOptions
        {
            WriteIndented = true, 
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });
    }

    /// <summary>
    /// Wandlet einen JSON-String (erstellt via ToJson) wieder in ein Dictionary um.
    /// </summary>
    /// <typeparam name="TValue">Value-Typ des Dictionary</typeparam>
    /// <param name="jsondict">JSON-String, der das Dictionary enthält.</param>
    /// <returns>Dictionary aus dem JSON-String</returns>
    public static Dictionary<string, TValue> DictionaryFromJson<TValue>(this string jsondict)
    {
        return JsonSerializer.Deserialize<Dictionary<string, TValue>>(jsondict) ?? [];
    }
}

