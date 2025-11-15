using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Eingabecontrol für Suchen via SearchEngine
/// </summary>
public class AFComboPopupSearch : AFEditComboPopup
{
    private AFComboPopupSearchPopup? popup;
    private PopupContainerControl? container;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFComboPopupSearch()
    {
        if (UI.DesignMode) return;

        Properties.NullValuePrompt = "Suche nach...";
        Properties.NullValuePromptShowForEmptyValue = true;
        AutoPopupWidth = true;

    }

    /// <summary>
    /// Suchleiste, in der das Control angezeigt und verwendet wird.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFSearchBar? SearchBar { get; set; }


    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (UI.DesignMode) return;

        this.AddButton(UI.GetImage(Symbol.Search), new Size(16, 16), "pshSearch", tooltip: UI.GetSuperTip("Suche", "Ausführen der Suche nach den eingegebenen Suchbegriffen."));

        QueryPopUp += (_, _) =>
        {
            if (popup == null)
            {
                popup = new() { Dock = DockStyle.Fill };
                container = new() { Size = new(550, 550) };
                container.Controls.Add(popup);
                Properties.PopupControl = container;
            }
        };

        ButtonClick += (_, args) =>
        {
            if (args.Button.Tag != null && args.Button.Tag.ToString() == "pshSearch")
                doSearch();
        };

        KeyDown += (_, args) =>
        {
            if (args.KeyCode == Keys.Enter) doSearch();
            else if (args.KeyCode == Keys.Escape) Text = "";
        };

        //if (SearchBar != null)
        //{
        //    SearchBar.Size = new(SearchBar.Width, Math.Max(Height, SearchBar.Plugin?.Height ?? 0) + SearchBar.Padding.Vertical);
        //}
    }

    /// <inheritdoc />
    public override void ClosePopup()
    {
        if (IsPopupOpen)
            DoClosePopup(PopupCloseMode.Normal);
    }

    private void doSearch() 
    {
        ClosePopup(PopupCloseMode.Normal);

        if (Text.IsEmpty())
            return;

        //popup.addPhrase(Text);

        // ExecuteSearch?.Invoke(this, ea);
    }
}

/// <summary>
/// Popup für AFComboboxPopupSearch
/// </summary>
public class AFComboPopupSearchPopup : AFUserControl
{
    private readonly AFNavTabControl tab;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFComboPopupSearchPopup()
    {
        Padding = new(3);

        tab = new() { Dock = DockStyle.Fill };
        tab.BeginInit();
        tab.TabPages.Add(new DevExpress.XtraTab.XtraTabPage() { Text = "SUCHEN IN", Name = "tabSearchIn" });
        tab.TabPages.Add(new DevExpress.XtraTab.XtraTabPage() { Text = "ZULETZT GESUCHT", Name = "tabSearchHistory" });

        tab.TabPages[0].Controls.Add(new AFLabel()
        {
            Padding = new(5), Dock = DockStyle.Top, AutoSizeMode = LabelAutoSizeMode.Vertical, AllowHtmlString = true, Text =
                "Wählen Sie ein Feld aus (Doppleklick) um die Suche eines Begriffes auf dieses Feld zu begrenzen. " +
                "Beispiel: Sucheingabe <b>Dresden</b> sucht den Begriff <b>Dresden</b> in <b>allen</b> Feldern. " +
                "Sucheingabe <b>ORT:Dresden</b> beschränkt die Suche das Begriffs <b>Dresden</b> auf das Feld <b>Ort</b>."
        });
        tab.TabPages[0].Controls.Add(new AFTreeGrid { Dock = DockStyle.Fill, BorderStyle = BorderStyles.NoBorder, Name = "treeSearchIn" });

        tab.TabPages[1].Controls.Add(new AFLabel()
        {
            Padding = new(5), Dock = DockStyle.Top, AutoSizeMode = LabelAutoSizeMode.Vertical, AllowHtmlString = true, Text =
                "Die zuletzt verwendeten Suchabfragen. Klicken Sie eine Abfrage an, um diese zu übernehmen und damit erneut ausführen zu können."
        });
        tab.TabPages[1].Controls.Add(new AFListbox() { Dock = DockStyle.Fill, BorderStyle = BorderStyles.NoBorder });

        AFTreeGrid tree = (AFTreeGrid)tab.TabPages[0].Controls["treeSearchIn"]!;

        tree.OptionsBehavior.AutoNodeHeight = false;
        tree.RowHeight = UI.GetScaled(26);
        tree.Appearance.Row.FontSizeDelta = 1;
        tree.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.None;
        tree.OptionsView.ShowColumns = false;
        tree.OptionsView.ShowHorzLines = false;
        tree.OptionsView.ShowIndicator = false;
        tree.OptionsView.ShowVertLines = false;

        tab.PaintStyleName = "AFFlat";
        tab.EndInit();

        tab.SelectedTabPageIndex = 0;
        Controls.Add(tab);
    }
}


