namespace AF.CORE;

/// <summary>
/// Erweiterungen für IDictionary
/// </summary>
public static class IDictionaryEx
{
    /// <summary>
    /// fügt einen Wert zu einem Wörterbuch hinzu oder - falls bereits vorhanden - ändert den Wert
    /// </summary>
    /// <param name="dict">Wörterbuch</param>
    /// <param name="key">Schlüssel</param>
    /// <param name="value">Value</param>
    public static void AddOrAppend(this IDictionary<string, string> dict, string key, string value)
    {
        if (dict.ContainsKey(key))
            dict[key] += Environment.NewLine + value;
        else
            dict.Add(key, value);
    }

    /// <summary>
    /// fügt einen Wert zu einem Wörterbuch hinzu oder - falls bereits vorhanden - ersetzt den aktuellen Wert
    /// </summary>
    /// <param name="dict">Wörterbuch</param>
    /// <param name="key">Schlüssel</param>
    /// <param name="value">Wert</param>
    public static void AddOrReplace<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
    {
        dict[key] = value;
    }

    /// <summary>
    /// fügt einen Wert zu einem Wörterbuch hinzu oder - falls bereits vorhanden - ersetzt den aktuellen Wert
    /// </summary>
    /// <param name="dict">Wörterbuch</param>
    /// <param name="key">Schlüssel</param>
    /// <param name="value">Wert</param>
    public static void TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
    {
        dict[key] = value;
    }
}

