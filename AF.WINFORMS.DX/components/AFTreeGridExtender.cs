using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTreeList;

namespace AF.WINFORMS.DX;

/// <summary>
/// Erweiterung der Funktionen eines XtraTree-Controls
/// </summary>
[ToolboxItem(true)]
[SupportedOSPlatform("windows")]
[DesignerCategory("Code")]
public class AFTreeGridExtender : AFGridPivotExtenderBase
{
    private TreeList? _grid;
    private TreeListHitInfo? _hitInfo;

    /// <summary>
    /// das zu erweiternde Grid
    /// </summary>
    [Browsable(true)]
    [DefaultValue(null)]
    public TreeList? Grid
    {
        get => _grid;
        set
        {
            _grid = value;

            if (!UI.DesignMode)
                extendGrid();
        }
    }

    /// <summary>
    /// DragDrop-Unterstützung
    /// </summary>
    [Browsable(true)]
    [DefaultValue(false)]
    public bool SupportDragDrop { get; set; } = false;

    /// <summary>
    /// Handler für RequestDragDropData
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void RequestDragDropDataHandler(object sender, RequestDragDataEventArgs args);

    /// <summary>
    /// RequestDragDropData Ereignis, das ausgelöst wird, wenn Drag-Daten benötigt werden
    /// </summary>
    public event RequestDragDropDataHandler? RequestDragData;

    private void extendGrid()
    {
        if (_grid == null) return;

        _grid.MouseDown += _mouseDown;
        _grid.MouseMove += _mouseMove;

        _grid.PopupMenuShowing += extendMenu;
    }

    private void extendMenu(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
    {
        if (sender is not ColumnView view) return;

    }

    private void _mouseDown(object? sender, MouseEventArgs ev)
    {
        if (_grid == null || SupportDragDrop == false) return;

        TreeListHitInfo hitInfo = _grid.CalcHitInfo(new Point(ev.X, ev.Y));

        if (ev.Button == MouseButtons.Left && hitInfo.InRow)
            _hitInfo = hitInfo;
    }

    private void _mouseMove(object? sender, MouseEventArgs ev)
    {
        if (_grid == null || SupportDragDrop == false || ev.Button != MouseButtons.Left) return;

        Size dragSize = SystemInformation.DragSize;

        if (_hitInfo == null) return;

        Rectangle dragRect = new Rectangle(new Point(_hitInfo.MousePoint.X - dragSize.Width / 2,
            _hitInfo.MousePoint.Y - dragSize.Height / 2), dragSize);

        if (dragRect.Contains(new Point(ev.X, ev.Y))) return;

        var obj = _grid.GetRow(_hitInfo.Node.Id);

        if (RequestDragData != null)
        {
            RequestDragDataEventArgs args = new RequestDragDataEventArgs { DraggedRow = obj, Tree = _grid, DragNode = _hitInfo.Node };
            RequestDragData?.Invoke(this, args);
            if (args.DragData != null)
                obj = args.DragData;
        }

        if (obj != null)
            _grid.DoDragDrop(obj, DragDropEffects.All);

        _hitInfo = null;
    }
}