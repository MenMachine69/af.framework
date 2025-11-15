namespace AF.CORE;

/// <summary>
/// Klasse, die Werte bestehend aus Key (Name) und Value (Wert) beschreibt.
/// </summary>
[Serializable]
public sealed class KeyValue
{
    /// <summary>
    /// Key/Name des Wertes
    /// </summary>
    public string Key { get; set; } = "";

    /// <summary>
    /// Wert
    /// </summary>
    public string Value { get; set; } = "";

    /// <summary>
    /// Constructor
    /// </summary>
    public KeyValue() { }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="key">Key/Name</param>
    /// <param name="value">Wert</param>
    public KeyValue(string key, string value)
    {
        Key = key;
        Value = value;
    }
}

