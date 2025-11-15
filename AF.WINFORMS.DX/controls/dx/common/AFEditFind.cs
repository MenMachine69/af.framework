using DevExpress.Utils;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Tile;
using DevExpress.XtraTreeList;

namespace AF.WINFORMS.DX;

/// <summary>
/// Einzeilige Eingabe für einen Suchbegriff, der dann im
/// angeschlossenen Grid/Tree gesucht wird (ersetzt FindPanel).
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
[DesignerCategory("Code")]
public class AFEditFind : AFEditButtons
{
    /// <summary>
    /// Constructor
    /// </summary>
    public AFEditFind()
    {
        Properties.Buttons.Clear();

        this.AddButton(Symbol.Search, showleft: true, enabled: false);
        this.AddButton(Symbol.DismissCircle, name: "btnClear");
        Properties.Buttons[1].Visible = false;

        Properties.NullValuePrompt = "Suche nach...";

        EditValueChanged += (_, _) =>
        {
            var search = Text.Trim();
            Properties.Buttons[1].Visible = search.Length > 0;

            if (Grid != null)
            {
                if (Grid.FocusedView is GridView grid)
                    grid.FindFilterText = search;
                else if (Grid.FocusedView is AdvBandedGridView advbanded)
                    advbanded.FindFilterText = search;
                else if (Grid.FocusedView is BandedGridView banded)
                    banded.FindFilterText = search;
                else if (Grid.FocusedView is TileView tiles)
                    tiles.FindFilterText = search;
            }

            if (Tree != null)
                Tree.FindFilterText = search;
        };


        ButtonClick += (_, e) => { Text = ""; };
    }

    /// <summary>
    /// Grid, auf das sich die Suche bezieht.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DevExpress.XtraGrid.GridControl? Grid { get; set; }

    /// <summary>
    /// Tree, auf das sich die Suche bezieht.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TreeList? Tree { get; set; }
}