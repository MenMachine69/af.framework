namespace AF.DATA;

/// <summary>
/// Attribut, das einen View in der Datenbank beschreibt, die genau wie eine vorhandene aufgebaut ist und von dieser erbt.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class AFSiblingView : Attribute
{
    /// <summary>
    /// Typ des der Tabelle, von der geerbt wird.
    /// </summary>
    public Type SiblingType { get; set; } = typeof(Nullable);

    /// <summary>
    /// Interne ID des Views
    /// IDs von 0 - 100 sind für AF3-interne Dinge reserviert. Für eigene Views nur IDs &gt; 100 verwenden!
    /// </summary>
    public int ViewId { get; set; }

    /// <summary>
    /// Typ des Master/Tabelle für diese Ansicht.
    /// 
    /// Master muss der primäre Typ der primären Tabelle für diese Ansicht sein (select ... from 'master' ...). 
    /// Dieser Typ wird benötigt, um den richtigen IController für diese Ansicht zu erkennen.
    /// </summary>
    public Type MasterType { get; set; } = typeof(Nullable);
}