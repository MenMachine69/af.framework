using DevExpress.XtraPivotGrid;

namespace AF.WINFORMS.DX;

/// <summary>
/// Erweiterung der Funktionen eines PivotGridControl-Controls
/// </summary>
[ToolboxItem(true)]
[SupportedOSPlatform("windows")]
public class AFPivotExtender : AFGridPivotExtenderBase
{
    private PivotGridControl? _pivot;

    /// <summary>
    /// das zu erweiternde Grid
    /// </summary>
    [Browsable(true)]
    [DefaultValue(null)]
    public PivotGridControl? PivotGrid
    {
        get => _pivot;
        set
        {
            _pivot = value;

            if (!UI.DesignMode)
                extendGrid();
        }
    }

    private void extendGrid()
    {
        if (_pivot == null) return;
        
        _pivot.PopupMenuShowing += extendMenu;
    }

    private void extendMenu(object sender, DevExpress.XtraPivotGrid.PopupMenuShowingEventArgs e)
    {
        if (e.MenuType != PivotGridMenuType.Header) return;


    }
}