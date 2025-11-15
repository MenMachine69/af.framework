namespace AF.CORE;

/// <summary>
/// mögliche/unterstützte Grid-Styles
/// </summary>
[Flags]
public enum eGridMode
{
    /// <summary>
    /// einfaches GridView 
    /// </summary>
    GridView = 1 << 0,

    /// <summary>
    /// GridView mit Bands
    /// </summary>
    BandedGridView = 1 << 1,

    /// <summary>
    /// GridView mit AdvancedBands
    /// </summary>
    AdvBandedGridView = 1 << 2,

    /// <summary>
    /// Tile view
    /// </summary>
    TileView = 1 << 3,

    /// <summary>
    /// Tree view
    /// </summary>
    TreeView = 1 << 4,

    /// <summary>
    /// Kanban view
    /// </summary>
    KanbanView = 1 << 5
}