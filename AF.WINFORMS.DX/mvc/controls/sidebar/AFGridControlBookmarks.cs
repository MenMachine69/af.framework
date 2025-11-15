using DevExpress.XtraGrid.Views.Grid;

namespace AF.MVC;

/// <summary>
/// Grid zur Anzeige von Bookmarks
/// </summary>
[DesignerCategory("Code")]
public class AFGridControlBookmarks : AFGridControl
{
    /// <summary>
    /// Constructor
    /// </summary>
    public AFGridControlBookmarks(bool allowGoto)
    {
        AFGridSetup setup = new();
        setup.Columns.Add(new() { ColumnFieldname = nameof(ModelBookmark.TypeName), Caption = "Typ" });
        setup.Columns.Add(new() { ColumnFieldname = nameof(ModelBookmark.Caption), Bold = true, AutoFill = true, Caption = "Bezeichnung", AllowSort = false });
        setup.Columns.Add(new() { ColumnFieldname = nameof(ModelBookmark.LastUsedText), FixedWidth = true, Width = 60, Caption = " ", AllowSort = false });
        setup.Columns.Add(new() { ColumnFieldname = nameof(ModelBookmark.Created), Visible = false });
        setup.GroupBy = [nameof(ModelBookmark.TypeName)];
        setup.SortOrder = eOrderMode.Descending;
        setup.SortOn = nameof(ModelBookmark.Created);

        if (allowGoto)
        {
            setup.OnGotoAction = (view) =>
            {
                if (view is not GridView gridview) return;

                if (gridview.GetFocusedRow() is not ModelBookmark bookmark) return;

                GotoAction?.Invoke(bookmark);
            };
        }

        this.Setup(setup);

        if (this.FocusedView is GridView view)
        {
            view.OptionsView.ShowGroupPanel = false;
            view.OptionsMenu.EnableColumnMenu = false;
        }
    }

    /// <summary>
    /// Aktion, die ausgeführt wird, wenn der GOTO-Button betätigt wird.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Action<ModelBookmark>? GotoAction { get; set; }
}