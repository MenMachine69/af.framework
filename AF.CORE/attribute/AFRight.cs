namespace AF.DATA;

/// <summary>
/// Beschreibung einer Berechtigung
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class AFRight : Attribute
{
    /// <summary>
    /// Kategrie / Gruppe
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Name/Titel der Berechtigung
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Beschreibung
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// ID des übergeordneten Rechts
    /// </summary>
    public int ParentRight { get; set; } 
}