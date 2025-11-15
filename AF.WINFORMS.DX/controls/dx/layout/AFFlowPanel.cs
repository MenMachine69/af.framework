using DevExpress.Utils;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace AF.WINFORMS.DX;

/// <summary>
/// Panel, das Controls in einem Flow anzeigen kann (idealerweise sollten die Controls gleich groß sein!)
/// </summary>
public class AFFlowPanel : LayoutControl
{
    private readonly LayoutControlGroup root = null!;
    private readonly LayoutControlGroup flow = null!;

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFFlowPanel()
    {
        if (UI.DesignMode) return;

        AllowCustomization = false;


        flow = new()
        {
            GroupBordersVisible = false,
            LayoutMode = LayoutMode.Flow,
            TextVisible = false
        };

        root = new()
        {
            EnableIndentsWithoutBorders = DefaultBoolean.True,
            GroupBordersVisible = false,
            TextVisible = false,
        };
        root.Add(flow);

        Root = root;
    }


    /// <summary>
    /// Eine Control hinzufügen...
    /// </summary>
    /// <param name="control"></param>
    public void AddItem(Control control)
    {
        control.MinimumSize = new(control.Width, control.Height);
        control.MaximumSize = new(control.Width, control.Height);

        flow.AddItem(string.Empty, control).TextVisible = false;
    }

    /// <summary>
    /// Löscht die vorhandenen Controls
    /// </summary>
    public void ClearItems()
    {
        flow.Clear(true);
    }

}
