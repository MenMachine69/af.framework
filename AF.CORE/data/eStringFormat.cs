namespace AF.CORE;

/// <summary>
/// Format eines Textes
/// </summary>
public enum eStringFormat
{
    /// <summary>
    /// undefiniert
    /// </summary>
    Undefined,
    /// <summary>
    /// nur Text
    /// </summary>
    Text,
    /// <summary>
    /// HTML-formatierter Text
    /// </summary>
    Html,
    /// <summary>
    /// XML-formatierter Text
    /// </summary>
    Xml,
    /// <summary>
    /// JSON-formatierter Text
    /// </summary>
    Json,
    /// <summary>
    /// andere Formate
    /// </summary>
    Other
}