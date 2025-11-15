using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;

namespace AF.WINFORMS.DX;

/// <summary>
/// Informationen zum DragDrop
/// </summary>
public class RequestDragDataEventArgs : EventArgs
{
    /// <summary>
    /// Gedraggte Zeile
    /// </summary>
    public object? DraggedRow { get; set; }

    /// <summary>
    /// Objekt, dass die gedraggte Zeile repräsentiert
    /// </summary>
    public object? DragData { get; set; }

    /// <summary>
    /// TreeNode, die bewegt wird
    /// </summary>
    public TreeListNode? DragNode { get; set; }

    /// <summary>
    /// Tree (Quelle)
    /// </summary>
    public TreeList? Tree { get; set; }

    /// <summary>
    /// Grid (Quelle)
    /// </summary>
    public GridControl? Grid { get; set; }

    /// <summary>
    /// View (Quelle)
    /// </summary>
    public ColumnView? View { get; set; }
}
