using System.Drawing;

namespace AF.CORE;

/// <summary>
/// Optionen für den KabanView der Grids
/// </summary>
public class AFKanbanOptions
{
    /// <summary>
    /// Größe der Tiles im Kanban (Default: B:380, H:100)
    ///
    /// Die Höhe spielt nur eine Rolle, wenn AutoHeight nicht verendet wird.
    /// </summary>
    public Size TileSize { get; set; } = new(380, 100);

    /// <summary>
    /// Automatische Höhe der Tiles im HTML-Modus (Default: true)
    /// </summary>
    public bool AutoHeight { get; set; } = true;

    /// <summary>
    /// DragDrop der Tiles erlauben (Default: true)
    /// </summary>
    public bool AllowDrag { get; set; } = true;

    /// <summary>
    /// Name der Spalte, die die Werte für die Gruppierung enthält.
    ///
    /// Muss angegeben werden, damit die Gruppierung funktioniert!
    /// </summary>
    public string GroupColumn { get; set; } = "";
}