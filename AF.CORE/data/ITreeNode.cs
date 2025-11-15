namespace AF.CORE;

/// <summary>
/// Darstellung eines TreeNode in einer ordnerähnlichen Navigation.
///
/// Verwendung eines ITable-Modells und Zuweisung dieser Schnittstelle zur Verwendung
/// diese Modelle als Datenquelle für eine TreeList zu verwenden.
/// </summary>
public interface ITreeNode
{
    /// <summary>
    /// Übergeordnete Knoten-ID
    /// </summary>
    Guid NODE_PARENT_ID { get; }

    /// <summary>
    /// Ordner-Knoten-ID
    /// </summary>
    Guid NODE_ID { get; }

    /// <summary>
    /// Ordnername (Anzeigename)
    /// </summary>
    string NODE_NAME { get; }

    /// <summary>
    /// Ordnername für die Sortierung (Sortierwert)
    /// </summary>
    string NODE_NAME_SORT { get; }

    /// <summary>
    /// Bildindex (bei Verwendung einer ImageList) das den geöffneten TreeNode darstellt
    /// </summary>
    int? NODE_IMAGEINDX_OPEN { get; set; }

    /// <summary>
    /// Bildindex (bei Verwendung einer ImageList) das den geöffneten TreeNode darstellt
    /// </summary>
    int? NODE_IMAGEINDX_CLOSED { get; set; }

    /// <summary>
    /// Dropen auf das Element ist erlaubt
    /// </summary>
    bool NODE_ALLOWDROP { get; set; }

    /// <summary>
    /// Drag des Elements ist erlaubt
    /// </summary>
    bool NODE_ALLOWDRAG { get; set; }
}
