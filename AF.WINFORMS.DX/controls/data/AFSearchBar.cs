using AF.MVC;
using crm.ui;
using DevExpress.Utils;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace AF.WINFORMS.DX;

/// <summary>
/// Control für die Eingabe von Suchen in der Suchmaschine
/// </summary>
[DesignerCategory("Code")]
public class AFSearchBar : AFUserControl, ISearchEngineConsumer
{
    private readonly AFComboPopupSearch sleSearch = null!;
    private readonly AFEditSingleline sleAssist = null!;
    private readonly AFPanel pluginRight = null!;
    private readonly AFSkinnedPanel panelSearch = null!;
    private readonly AFNavSplitter splitterAssist = null!;
    private readonly AFEditComboChecked cmbTargets = null!;
    private readonly AFLabelGrayText lbl = null!;
    private readonly SearchEngine engine = null!;
    private readonly AFSearchBarPopup _searchPopup = null!;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFSearchBar()
    {
        if (UI.DesignMode) return;

        _searchPopup = new(this);

        engine = new(this);

        engine.OnFound = (section, data) =>
        {
            HitDisplay?.ShowHits(section, data);
        };

        UI.StyleChanged += (_, _) => { lbl.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Window); };

        Size = new(500, 48);

        pluginRight = new() { Dock = DockStyle.Right, Visible = false };
        Controls.Add(pluginRight);

        sleAssist = new()
        {
            Dock = DockStyle.Right,
            Size = new(300, 26),
            Properties =
            {
                AutoHeight = false,
                NullValuePrompt = "Was möchten Sie tun?",
                NullValuePromptShowForEmptyValue = true,
                Appearance =
                {
                    FontSizeDelta = 2,
                    Options =
                    {
                        UseFont = true
                    }
                }
            }
        };
        Controls.Add(sleAssist);
        sleAssist.BringToFront();

        splitterAssist = new() { Dock = DockStyle.Right };
        Controls.Add(splitterAssist);
        splitterAssist.BringToFront();


        sleSearch = new()
        {
            SearchBar = this,
            BorderStyle = BorderStyles.NoBorder,
            Dock = DockStyle.Fill,
            Properties =
            {
                AutoHeight = false,
                NullValuePrompt = "Suche nach...",
                NullValuePromptShowForEmptyValue = true,
                TextEditStyle = TextEditStyles.Standard,
                Appearance =
                {
                    FontSizeDelta = 2,
                    Options =
                    {
                        UseFont = true
                    }
                }
            }
        };

        sleSearch.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Return)
            {
                HitDisplay = ((AFWinFormsMVCApp)AFCore.App).Shell.ViewManager.OpenPage(ObjectEx.CreateInstance<IModel>(typeof(ModulSearchResults))).ViewEditor as ModulSearchResultsEditor;
                HitDisplay?.SetEngine(engine);

                Application.DoEvents();

                engine.Search(sleSearch.Text);
            }
        };


        cmbTargets = new()
        {
            Dock = DockStyle.Right,
            BorderStyle = BorderStyles.NoBorder,
            Properties =
            {
                NullText = "<Alle>",
                TextEditStyle = TextEditStyles.DisableTextEditor,
                AutoHeight = false
            }
        };


        cmbTargets.CustomDisplayText += (_, a) =>
        {
            if (cmbTargets.AllItemsChecked)
                a.DisplayText = "<alle>";
            else if (cmbTargets.NoItemsChecked)
                a.DisplayText = "<keine>";

        };

        lbl = new AFLabelGrayText()
        {
            Text = "in",
            AutoSizeMode = LabelAutoSizeMode.None,
            Dock = DockStyle.Right,
            BackColor = UI.TranslateSystemToSkinColor(SystemColors.Window),
            Padding = new(5, 0, 5, 0),
            Size = new(30, 26),
            Appearance =
            {
                TextOptions =
                {
                    VAlignment = VertAlignment.Center
                },
                Options =
                {
                    UseTextOptions = true
                }

            }
        };

        panelSearch = new()
        {
            Dock = DockStyle.Fill,
            Margin = new(0),
            Padding = new(0)
        };

        panelSearch.Controls.Add(cmbTargets);
        cmbTargets.BringToFront();
        panelSearch.Controls.Add(lbl);
        lbl.BringToFront();
        panelSearch.Controls.Add(sleSearch);
        sleSearch.BringToFront();

        Controls.Add(panelSearch);
        panelSearch.BringToFront();
    }

    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (UI.DesignMode) return;

        sleSearch.EditValueChanged += searchPreview;
    }

    private void _hook_MouseDown(object sender, MouseEventArgs e)
    {
        if (_searchPopup.IsMouseOver() == false && sleSearch.IsMouseOver() == false)
            CloseDirectSearchPopup();
    }


    /// <inheritdoc />
    protected override void OnHandleDestroyed(EventArgs e)
    {
        base.OnHandleDestroyed(e);

        if (UI.DesignMode) return;

        sleSearch.EditValueChanged -= searchPreview;
    }

    private void searchPreview(object? sender, EventArgs e)
    {
        SearchTermChanged?.Invoke(sleSearch.Text, EventArgs.Empty);
    }

    /// <summary>
    /// Plugin, dass rechts angezeigt wird
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Control? Plugin => pluginRight.Controls.Count > 0 ? pluginRight.Controls[0] : null;

    /// <summary>
    /// Ein Plugin anzeigen...
    /// </summary>
    /// <param name="ctrl"></param>
    public void ShowPlugin(Control ctrl)
    {
        if (pluginRight.Controls.Count > 0)
        {
            var current = pluginRight.Controls[0];
            pluginRight.Controls.RemoveAt(0);
            current.Dispose();
        }

        pluginRight.Visible = true;
        pluginRight.Size = new(ctrl.Width, sleSearch.Height);
        ctrl.Dock = DockStyle.Top;
        pluginRight.Controls.Add(ctrl);
    }

    /// <summary>
    /// Ereignis, das ausgelöst wird, wenn der Benutzer Suchbegriffe eingibt.
    ///
    /// Hier kann direkt auf die Eingabe reagiert werden um z.B. Treffer in der Vorschau ohne eine echte Suchanfrage anzuzeigen.
    /// Beispiel: Benutzer gibt eine 9stellige Zahl ein -> direkte Suche einer Firma über Mitgliedsnummer und Anzeige der
    /// Firma in der Ergebnisvorschau, damit der Benutzer diese direkt auswählen kann.
    /// 
    /// Sender ist der string/die Eingabe selbst.
    /// </summary>
    [field: NonSerialized]
    public event EventHandler? SearchTermChanged;

    /// <summary>
    /// Zugriff auf die SearchEngine.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SearchEngine Engine => engine;

    /// <summary>
    /// Einen durchsuchbaren Typ registrieren.
    /// </summary>
    /// <typeparam name="TTable">Tabellentyp</typeparam>
    /// <param name="selected">ausgewählt für Suche</param>
    public void RegisterTable<TTable>(bool selected) where TTable : class, ITable
    {
        if (!engine.RegisterTable<TTable>()) return;

        var item = new CheckedListBoxItem(typeof(TTable).GetTypeDescription().Context?.NameSingular ?? typeof(TTable).Name,
            selected ? CheckState.Checked : CheckState.Unchecked);
        
        item.Tag = typeof(TTable);

        cmbTargets.Properties.Items.Add(item);
    }

    /// <summary>
    /// Prüfen, ob der Typ für Suche aktiviert ist.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool SearchIn(Type target)
    {
        return cmbTargets.Properties.Items.FirstOrDefault(i => i.CheckState == CheckState.Checked && i.Tag is Type type && type == target) != null;
    }

    /// <summary>
    /// Suche wurde gestartet
    /// </summary>
    /// <param name="searchengine">Engine, die die Suche gestartet hat</param>
    public void BeginSearch(SearchEngine searchengine)
    {
        HitDisplay?.SetEngine(searchengine);
        HitDisplay?.BeginSearch();
    }

    /// <summary>
    /// Suchefortschritt (Suche in Typ wurde gestartet)
    /// </summary>
    /// <param name="searchengine">Engine, die die Suche gestartet hat</param>
    /// <param name="tdesc">Typ der durchsucht wird</param>
    public void SearchProgress(SearchEngine searchengine, TypeDescription tdesc)
    {
        HitDisplay?.SearchProgress(tdesc);
    }

    /// <summary>
    /// Suche wurde beendet
    /// </summary>
    /// <param name="searchengine">Engine, die die Suche beendet hat</param>
    public void EndSearch(SearchEngine searchengine)
    {
        HitDisplay?.EndSearch();
    }

    /// <summary>
    /// Suche soll abgebrochen werden
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool CancelSearch { get; set;  }


    /// <summary>
    /// Anzeige für die Treffer
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ModulSearchResultsEditor? HitDisplay { get; set; }

    /// <summary>
    /// Popup der Direktsuche schließen
    /// </summary>
    public void CloseDirectSearchPopup()
    {
        if (_searchPopup.Visible) _searchPopup.Hide();
    }

    /// <summary>
    /// Treffer aus Direktsuche im Popup anzeigen
    /// </summary>
    /// <param name="result"></param>
    public void ShowDirectSearchPopup(ModelInfo result)
    {
        if (_searchPopup.Visible == false)
        {
            // Point location = PointToScreen(new(sleSearch.Left + Left + Padding.Left, sleSearch.Top + sleSearch.Height + Padding.Vertical));
            Point location = PointToScreen(new(panelSearch.Left, panelSearch.Top + panelSearch.Height));
            _searchPopup.Location = location;
            _searchPopup.Size = new(sleSearch.Width, 80);
            _searchPopup.TopMost = true;
            _searchPopup.Show();
            sleSearch.Focus();
        }

        _searchPopup.ShowResult(result, openFromDirectSearch);
    }

    private void openFromDirectSearch(ModelInfo ifo)
    {
        if (_searchPopup.Visible) _searchPopup.Hide();

        UI.ViewManager.OpenPage(ifo.Link);
    }
}

/// <summary>
/// Popup zur Anzeige der Treffer der Direktsuche...
/// </summary>
internal class AFSearchBarPopup : FormBase
{
    private readonly AFSearchBar _searchbar = null!;
    private Action<ModelInfo>? _onClick;
    private AFLabel lblSymbol = null!;
    private readonly AFLabelBoldLink lblLink = null!;
    private ModelInfo? model;
    private readonly AFBindingConnector connector = null!;
    private readonly IContainer? components;
    // private AFErrorProvider? errProvider;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }
    
    /// <summary>
    /// Constructor
    /// </summary>
    public AFSearchBarPopup()
    {
        if (UI.DesignMode) return;

        components = new Container();
        connector = new(components) { ContainerControl = this };

        AFTablePanel table = new() { UseSkinIndents = true, Dock = DockStyle.Fill };
        Controls.Add(table);
        table.BeginLayout();

        lblSymbol = table.Add<AFLabel>(1, 1, rowspan: 2);
        lblSymbol.ImageOptions.SvgImageSize = new(32, 32);
        lblSymbol.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.None;
        lblSymbol.ImageOptions.Alignment = ContentAlignment.MiddleCenter;

        lblLink = table.Add<AFLabelBoldLink>(1, 2).Name(nameof(ModelInfo.Caption)).Text("Label");
        lblLink.ClickAction = (label) => { if (model != null) _onClick?.Invoke(model); };

        table.Add<AFLabelBoldText>(2, 2).Name(nameof(ModelInfo.Description)).Text("Description");
        table.EndLayout();

        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public AFSearchBarPopup(AFSearchBar searchbar) : this()
    {
        _searchbar = searchbar;
    }


    /// <summary>
    /// Ergebnis anzeigen.
    /// </summary>
    /// <param name="entry">Ergebnis</param>
    /// <param name="onClick">bei Click ausführen</param>
    public void ShowResult(ModelInfo entry, Action<ModelInfo> onClick)
    {
        model = entry;
        
        lblSymbol.ImageOptions.SvgImage = entry.ModelType.GetUIController()?.TypeImage as SvgImage;
        lblSymbol.Visible = lblSymbol.ImageOptions.SvgImage != null;

        _onClick = onClick;
        connector!.DataSource = entry;
    }
}