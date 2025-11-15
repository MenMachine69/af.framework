namespace AF.DATA;

/// <summary>
/// Attribut, das eine Tabelle in der Datenbank beschreibt, die genau wie eine vorhandene aufgebaut ist und von dieser erbt.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class AFSiblingTable : Attribute
{
    /// <summary>
    /// Typ des der Tabelle, von der geerbt wird.
    /// </summary>
    public Type SiblingType { get; set; } = typeof(Nullable);

    /// <summary>
    /// Interne ID der Tabelle
    /// IDs von 0 - 100 sind für AF3-interne Dinge reserviert. Für eigene Tabellen nur IDs &gt; 100 verwenden!
    /// </summary>
    public int TableId { get; set; }
}