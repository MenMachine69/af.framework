using DevExpress.Utils.Layout;
using DevExpress.XtraBars;

namespace AF.MVC;

/// <summary>
/// Sidebar to display a list of bookmarks
/// </summary>
[DesignerCategory("Code")]
public class AFSidebarPageBookmarks : AFSidebarPage, ISidebarPage
{
    private readonly AFGridControlBookmarks grid = null!;
    private readonly AFBarManager manager = null!;
    private readonly AFBarController controller = null!;
    private readonly BarStaticItem lblCaption = null!;
    private bool _historyMode;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFSidebarPageBookmarks()
    {
        if (DesignMode) return;

        grid = new(true) { Dock = DockStyle.Fill, Margin = new(0, 3, 0, 0) };

        grid.GotoAction = (bookmark) =>
        {
            UI.ViewManager.OpenPage(bookmark.Link);
        };
        

        controller = new();
        //controller.AutoBackColorInBars = true;

        manager = new();
        manager.Form = this;
        manager.Controller = controller;
        manager.BeginInit();

        AFTablePanel table = new() { Dock = DockStyle.Fill, UseSkinIndents = false };
        Controls.Add(table);

        table.BeginLayout();
        var bar = table.AddBar(manager, 1, 1);
        var btn = bar.AddButton("btnSettings", UI.GetImage(Symbol.Wrench), rightalign: true);
        btn.ItemClick += (_, _) =>
        {
            editSettings();
        };
        lblCaption = bar.AddLabel("lblCaption", "Lesezeichen");

        table.Add(grid, 2, 1);

        table.SetRow(1, TablePanelEntityStyle.Absolute, 30.0f);
        table.SetRow(2, TablePanelEntityStyle.Relative, 1.0f);
        table.SetColumn(1, TablePanelEntityStyle.Relative, 1.0f);

        table.EndLayout();
        table.BringToFront();

        manager.EndInit();
    }

    private void editSettings()
    {
        // TODO: Settings bearbeiten
    }

    /// <summary>
    /// Gibt an, ob statt der Bookmarks die History gesetzt werden soll.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool HistoryMode
    {
        get => _historyMode;
        set
        {
            _historyMode = value;
            lblCaption.Caption = _historyMode ? "Verlauf" : "Lesezeichen";
        }
    }

    /// <summary>
    /// Called after the page is shown
    ///
    /// Load data here.
    /// </summary>
    public void AfterShow()
    {
        grid.DataSource = HistoryMode ? UI.ViewManager.History : UI.ViewManager.Bookmarks;
    }

    /// <inheritdoc />
    protected override void OnHandleDestroyed(EventArgs e)
    {
        base.OnHandleDestroyed(e);

        manager.Dispose();
        controller.Dispose();
    }
}
