namespace AF.CORE;

/// <summary>
/// Stil des Gridviews
/// </summary>
[Flags]
public enum eGridStyle
{
    /// <summary>
    /// Undefiniert
    /// </summary>
    None = 1 << 0,
    /// <summary>
    /// Liste aller Spalten (alle Eigenschaften mit GridStyle-Attribut)
    /// </summary>
    All = 1 << 1,
    /// <summary>
    /// Liste aller Spalten (alle Eigenschaften mit GridStyle-Attribut)
    /// </summary>
    Full = 1 << 2,
    /// <summary>
    /// kleine Liste von Spalten (einige Eigenschaften mit GridStyle-Attribut)
    /// </summary>
    Small = 1 << 3,
    /// <summary>
    /// Liste von Spalten zur Darstellung der Modelle im Browser (einige Eigenschaften mit GridStyle-Attribut)
    /// </summary>
    Browser = 1 << 4,
    /// <summary>
    /// Spalten für die Combobox, die im Popup angezeigt werden sollen (einige Eigenschaften mit GridStyle-Attribut)
    /// </summary>
    ComboboxEntrys = 1 << 5,
    /// <summary>
    /// Spalten für eine Darstellung als Baumansicht (einige Eigenschaften mit GridStyle-Attribut)
    /// </summary>
    Treeview = 1 << 6,
    /// <summary>
    /// Custom-Style, der vom Controller bestimmt wird.
    /// Dazu bekommt der Controller einen Namen für das gewünschte Layout.
    /// </summary>
    Custom = 1 << 7,
    /// <summary>
    /// Trefferliste in der Suche
    /// </summary>
    SearchHits = 1 << 8
}