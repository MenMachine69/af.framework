using DevExpress.Utils.Layout;
using DevExpress.XtraGrid.Views.Grid;

namespace AF.MVC;

/// <summary>
/// Popup-Control für AFEditComboPopupModel.
/// </summary>
/// <typeparam name="T">Typ des auszuwählenden Models</typeparam>
public class AFEditComboPopupPopupModel<T> : AFEditComboPopupPopup, ISearchEngineConsumer where T : class, ITable
{
    private readonly AFGridControl? gridSearch;
    private readonly AFGridControl? gridBrowser;
    private readonly AFGridControlBookmarks? gridHistory;
    private readonly AFGridControlBookmarks? gridBookmarks;
    private readonly SearchEngine _engine = null!;
    private readonly AFEditComboPopupModel<T> _combobox = null!;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFEditComboPopupPopupModel(AFEditComboPopupModel<T> combobox, AFEditComboPopupModelOptions options)
    {
        if (UI.DesignMode) return;

        _combobox = combobox;

        var tdesc = typeof(T).GetTypeDescription();
        var controller = typeof(T).GetController();

        _engine = new(this);
        _engine.RegisterTable<T>();

        Padding = new(5);

        AFTablePanel table = new() { Dock = DockStyle.Fill, UseSkinIndents = false };
        Controls.Add(table);

        table.BeginLayout();

        var tabctrl = table.Add<AFNavTabControl>(1, 1);
        tabctrl.Dock = DockStyle.Fill;
        tabctrl.PaintStyleName = "AFFlat";

        tabctrl.BeginInit();
        tabctrl.BeginUpdate();

        if (options.ShowBrowser)
        {
            var page = tabctrl.TabPages.Add(tdesc.Context?.NamePlural.ToUpper() ?? typeof(T).Name.ToUpper());
            tabctrl.SelectedTabPage = page;
            AFTablePanel tableBrowser = new AFTablePanel() { Dock = DockStyle.Fill, AutoSize = true, UseSkinIndents = false };
            page.Controls.Add(tableBrowser);
            tableBrowser.BeginLayout();
            tableBrowser.Add<AFLabelGrayText>(1, 1).Padding(3).Text = "Geben Sie die Begriffe, nach denen Sie suchen möchten ein. Verwenden Sie dabei die " +
                                                                      "gleiche Syntax wie bei der primären Suche des Programms. ";
            gridBrowser = tableBrowser.Add<AFGridControl>(2, 1);
            gridBrowser.Dock = DockStyle.Fill;
            gridBrowser.Setup(controller.GetGridSetup(eGridStyle.SearchHits));

            ((GridView)gridBrowser.MainView).RowClick += (s, e) =>
            {
                object? data = (s as GridView)?.GetRow(e.RowHandle) ?? null;
                if (data is IModel model)
                {
                    _combobox.SelectedValue = model.PrimaryKey;
                    _combobox.ClosePopup();
                }
            };

            tableBrowser.SetRow(2, TablePanelEntityStyle.Relative, 1.0f);
            tableBrowser.SetColumn(1, TablePanelEntityStyle.Relative, 1.0f);
            tableBrowser.EndLayout();
        }

        if (controller.AllowSearch && options.ShowSearch)
        {
            var page = tabctrl.TabPages.Add("SUCHE");
            if (!options.ShowBrowser) tabctrl.SelectedTabPage = page;
            AFTablePanel tableSearch = new AFTablePanel() { Dock = DockStyle.Fill, AutoSize = true, UseSkinIndents = false };
            page.Controls.Add(tableSearch);
            tableSearch.BeginLayout();
            tableSearch.Add<AFLabelGrayText>(1, 1).Padding(3).Text = "Geben Sie die Begriffe, nach denen Sie suchen möchten ein. Verwenden Sie dabei die " +
                                                                     "gleiche Syntax wie bei der primären Suche des Programms. ";

            var sleSearch = tableSearch.Add<AFEditSingleline>(2, 1);
            sleSearch.Dock = DockStyle.Fill;
            sleSearch.Properties.NullValuePrompt = "Suche nach...";

            sleSearch.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Return)
                {
                    _engine.OnFound = (sec, list) =>
                    {
                        gridSearch!.DataSource = list;
                    };

                    _engine.Search(sleSearch.Text, inType: typeof(T), all: true);
                }
            };

            gridSearch = tableSearch.Add<AFGridControl>(3, 1);
            gridSearch.Dock = DockStyle.Fill;
            gridSearch.Setup(controller.GetGridSetup(eGridStyle.SearchHits));

            ((GridView)gridSearch.MainView).RowClick += (s, e) =>
            {
                object? data = (s as GridView)?.GetRow(e.RowHandle) ?? null;
                if (data is IModel model)
                {
                    _combobox.SelectedValue = model.PrimaryKey;
                    _combobox.ClosePopup();
                }
            };

            tableSearch.SetRow(3, TablePanelEntityStyle.Relative, 1.0f);
            tableSearch.SetColumn(1, TablePanelEntityStyle.Relative, 1.0f);
            tableSearch.EndLayout();
        }

        if (controller.AllowHistory && options.ShowHistory)
        {
            var page = tabctrl.TabPages.Add("VERLAUF");
            AFTablePanel tableHistory = new AFTablePanel() { Dock = DockStyle.Fill, AutoSize = true, UseSkinIndents = false };
            page.Controls.Add(tableHistory);
            tableHistory.BeginLayout();
            tableHistory.Add<AFLabelGrayText>(1, 1).Padding(3).Text = "Liste der zuletzt verwendeten Objekte des auswählbaren Typs. Wählen Sie ein Objekt " +
                                                                      "in der Liste aus, um diese Objekt zu übernehmen.";

            gridHistory = new AFGridControlBookmarks(false);
            gridHistory.Dock = DockStyle.Fill;
            tableHistory.Add(gridHistory, 2, 1);

            tableHistory.SetRow(2, TablePanelEntityStyle.Relative, 1.0f);
            tableHistory.SetColumn(1, TablePanelEntityStyle.Relative, 1.0f);
            tableHistory.EndLayout();
        }

        if (controller.AllowBookmarks && options.ShowBookmarks)
        {
            var page = tabctrl.TabPages.Add("LESEZEICHEN");
            AFTablePanel tableBookmarks = new AFTablePanel() { Dock = DockStyle.Fill, AutoSize = true, UseSkinIndents = false };
            page.Controls.Add(tableBookmarks);
            tableBookmarks.BeginLayout();
            tableBookmarks.Add<AFLabelGrayText>(1, 1).Padding(3).Text = "Liste der Objekte des auswählbaren Typs, für die Sie ein lesezeichen gesetzt haben " +
                                                                        "Wählen Sie ein Objekt in der Liste aus, um diese Objekt zu übernehmen.";

            gridBookmarks = new AFGridControlBookmarks(false);
            gridBookmarks.Dock = DockStyle.Fill;
            tableBookmarks.Add(gridBookmarks, 2, 1);

            tableBookmarks.SetRow(2, TablePanelEntityStyle.Relative, 1.0f);
            tableBookmarks.SetColumn(1, TablePanelEntityStyle.Relative, 1.0f);
            tableBookmarks.EndLayout();
        }

        tabctrl.EndUpdate();
        tabctrl.EndInit();

        table.SetRow(1, TablePanelEntityStyle.Relative, 1.0f);
        table.SetColumn(1, TablePanelEntityStyle.Relative, 1.0f);

        table.EndLayout();
        Size = new Size(500, 500);
    }

    /// <summary>
    /// Zugriff auf den Browser
    /// </summary>
    public AFGridControl? Browser => gridBrowser;

    /// <summary>
    /// Zugriff auf den Verlauf
    /// </summary>
    public AFGridControlBookmarks? History => gridHistory;

    /// <summary>
    /// Zugriff auf die Bookmarks
    /// </summary>
    public AFGridControlBookmarks? Bookmarks => gridBookmarks;

    /// <summary>
    /// Wird von der Suchmaschine aufgerufen um zu ermitteln, ob der Typ target durchsucht werden soll.
    ///
    /// Für AFEditComboPopupModel ist der Wert immer true.
    /// </summary>
    /// <param name="target">zu durchsuchender Typ (hier: ModelTyp der Combobox)</param>
    /// <returns>true</returns>
    public bool SearchIn(Type target)
    {
        return true;
    }

    /// <summary>
    /// Suche in der Suchmaschine wurde gestartet.
    /// </summary>
    /// <param name="engine">Suchmaschine, die die Suche durchführt</param>
    public void BeginSearch(SearchEngine engine)
    {

    }

    /// <summary>
    /// Suche in der Suchmaschine beginnt mit der Durchsuchung des angegebenen Typen.
    /// </summary>
    /// <param name="engine">Suchmaschine, die die Suche durchführt</param>
    /// <param name="tdesc">durchsuchter Typ</param>
    public void SearchProgress(SearchEngine engine, TypeDescription tdesc)
    {

    }

    /// <summary>
    /// Suche in der Suchmaschine wurde beendet.
    /// </summary>
    /// <param name="engine">Suchmaschine, die die Suche durchführte</param>
    public void EndSearch(SearchEngine engine)
    {

    }

    /// <summary>
    /// Wird von der Suchmaschine abgefragt, ob der User die Suche beenden möchte.
    ///
    /// Wenn true zurückgegeben wird, wird die Suche beendet.
    /// </summary>
    public bool CancelSearch => false;
}