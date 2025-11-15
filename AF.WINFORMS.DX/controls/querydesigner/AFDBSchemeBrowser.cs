using AF.BUSINESS;
using AF.WINFORMS.DX.Properties;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;

namespace AF.WINFORMS.DX;

/// <summary>
/// Browser zur Anzeige der Struktur einer Datenbank (AF.DATA.IDatasource).
///
/// Datenbanken/Datenquellen werden via RegisterDatasource registriert und stehen dann zur Auswahl in einer Combobox zur Verfügung. 
/// </summary>
public partial class AFDBSchemeBrowser : AFUserControl
{
    private readonly BindingList<IDatabaseConnection> datasources = [];
    private IDatabaseConnection? currentDatasource;
    private DatabaseScheme? currentScheme;
    private DatabaseSchemeTable? currentTable;
    private bool treeLoaded;
    private bool gridLoaded;
    private bool firstGridView = true;
    private readonly Dictionary<Guid, DatabaseScheme> schemeBuffer = [];


    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFDBSchemeBrowser()
    {
        InitializeComponent();

        if (UI.DesignMode) return;

        grid.Dock = DockStyle.Fill;
        treeClassStructure.Dock = DockStyle.Fill;

        cmbDatasource.Properties.ValueMember = nameof(IDatabaseConnection.DatabaseName);
        cmbDatasource.Properties.DisplayMember = nameof(IDatabaseConnection.DatabaseName);
        cmbDatasource.Properties.ExportMode = ExportMode.Value;

        cmbDatasource.Properties.View.OptionsBehavior.AutoPopulateColumns = false;
        cmbDatasource.Properties.View.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
        
        // cmbDatasource.Properties.AllowNullInput = DefaultBoolean.True;
        cmbDatasource.Properties.NullValuePromptShowForEmptyValue = true;
        cmbDatasource.Properties.NullText = Resources.LBL_SELECTDATASOURCE;

        cmbDatasource.AddButton(UI.GetImage(Symbol.Database), 
            imagesize: new Size(16, 16), 
            enabled: false,
            showleft: true);

        AFGridSetup setup = new()
        {
            SortOn = nameof(IDatabaseConnection.DatabaseName),
            DefaultGridStyle = eGridMode.GridView
        };
        setup.Columns.Add(new() { ColumnType = typeof(string), Caption = Resources.LBL_DATABASE, ColumnFieldname = nameof(IDatabaseConnection.DatabaseName) });

        cmbDatasource.Setup(setup);

        menShowGrid.ImageOptions.SvgImage = UI.GetImage(Symbol.Table);
        menShowTree.ImageOptions.SvgImage = UI.GetImage(Symbol.Organization);
        menRefresh.ImageOptions.SvgImage= UI.GetImage(Symbol.ArrowSync);

        menShowGrid.CheckedChanged += (_, _) =>
        {
            if (!menShowGrid.Checked) return;

            grid.Visible = true;

            if (firstGridView)
            {
                grid.SplitterPosition = grid.Height / 2;
                firstGridView = false;
            }

            treeClassStructure.Visible = false;
            menShowTree.Checked = false;

            if (gridLoaded) return;

            loadGrid();
        };

        menShowTree.CheckedChanged += (_, _) =>
        {
            if (!menShowTree.Checked) return;

            grid.Visible = false;
            treeClassStructure.Visible = true;
            menShowGrid.Checked = false;

            if (treeLoaded) return;

            loadTree();
        };

        menRefresh.ItemClick += (_, _) =>
        { loadScheme(forceLoad: true); };

        cmbDatasource.EditValueChanged += (_, _) =>
        {
            if (cmbDatasource.GetSelectedObject() is not IDatabaseConnection ds) return;

            currentDatasource = ds;

            loadScheme();
        };

        setup = new()
        {
            GroupBy = [nameof(DatabaseSchemeTable.SchemeType)],
            SortOn = nameof(DatabaseSchemeTable.TABLE_NAME),
            DefaultGridStyle = eGridMode.GridView
        };
        setup.Columns.Add(new() { ColumnType = typeof(string), Caption = Resources.LBL_TYPE, ColumnFieldname = nameof(DatabaseSchemeTable.SchemeType) });
        setup.Columns.Add(new() { ColumnType = typeof(string), Caption = Resources.LBL_TABLEVIEW, ColumnFieldname = nameof(DatabaseSchemeTable.TABLE_NAME) });

        viewDatasourceBrowser.Setup(setup);
        viewDatasourceBrowser.OptionsView.EnableAppearanceEvenRow = false;
        viewDatasourceBrowser.OptionsView.EnableAppearanceOddRow = false;

        viewDatasourceBrowser.CustomColumnDisplayText += (s, e) =>
        {
            if (e.Column.FieldName != nameof(DatabaseSchemeTable.TABLE_NAME)) return;

            e.DisplayText = "<image=Table;size=16,16>  " + e.Value;
        };


        viewDatasourceBrowser.FocusedRowChanged += (_,_) =>
        {
            if (!ShowInfo) return;

            if (viewDatasourceBrowser.GetFocusedRow() is not DatabaseSchemeTable table) return;

            mleInfo.Text = RequestDescription?.Invoke((IDatabaseConnection)cmbDatasource.GetSelectedObject()!, table) ?? table.TABLE_DESCRIPTION;

            Table = table;
        };

        setup = new()
        {
            SortOn = nameof(DatabaseSchemeField.FIELD_NAME),
            DefaultGridStyle = eGridMode.GridView
        };
        setup.Columns.Add(new() { ColumnType = typeof(string), Caption = @"Name", ColumnFieldname = nameof(DatabaseSchemeField.FIELD_NAME) });
        setup.Columns.Add(new() { ColumnType = typeof(string), Caption = Resources.LBL_TYPE, ColumnFieldname = nameof(DatabaseSchemeField.FIELD_TYPE) });
        // setup.Columns.Add(new() { ColumnType = typeof(string), Caption = Resources.LBL_TYPE, ColumnFieldname = nameof(DatabaseSchemeField.FieldSystemType) });
        setup.Columns.Add(new() { ColumnType = typeof(string), Caption = Resources.LBL_MAXLENGTH, ColumnFieldname = nameof(DatabaseSchemeField.FIELD_LENGTH) });

        viewDatasourceBrowserFields.Setup(setup);

        viewDatasourceBrowserFields.OptionsView.EnableAppearanceEvenRow = false;
        viewDatasourceBrowserFields.OptionsView.EnableAppearanceOddRow = false;

        viewDatasourceBrowserFields.FocusedRowChanged += (_, _) =>
        {
            if (!ShowInfo) return;

            if (viewDatasourceBrowserFields.GetFocusedRow() is not DatabaseSchemeField field) return;

            mleInfo.Text = field.FIELD_DESCRIPTION;
        };

        viewDatasourceBrowserFields.CustomColumnDisplayText += (s, e) =>
        {
            if (e.Column.FieldName != nameof(DatabaseSchemeField.FIELD_NAME)) return;

            e.DisplayText = "<image=TextField;size=16,16>  " + e.Value;
        };

        viewDatasourceBrowser.HtmlImages = Glyphs.GetImages();
        viewDatasourceBrowserFields.HtmlImages = Glyphs.GetImages();

        treeClassStructure.SelectImageList = Glyphs.GetImages();
    }

    /// <summary>
    /// Aktuelle Datenquelle
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IDatabaseConnection? CurrentConnection => currentDatasource; 

    /// <summary>
    /// Zugriff auf die Baumansicht
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFTreeGrid TreeView => treeClassStructure;

    /// <summary>
    /// Browser, in dem die Felder der ausgewählten Tabelle/View angezeigt werden.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFDBSchemeBrowserFields? FieldBrowser { get; set; }

    /// <summary>
    /// Zugriff auf das GridView der Tabellen
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GridView GridViewTable => viewDatasourceBrowser;

    /// <summary>
    /// Zugriff auf das GridControl der Tabellen
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFGridControl GridControlTable => gridDatasourceBrowser;

    /// <summary>
    /// Zugriff auf das GridView der Felder
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GridView GridViewFields => viewDatasourceBrowser;

    /// <summary>
    /// Zugriff auf das GridControl der Felder
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFGridControl GridControlFields => gridDatasourceBrowserFields;

    /// <summary>
    /// Informationen zum Feld anzeigen.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ShowInfo
    {
        get => mleInfo.Visible;
        set
        {
            mleInfo.Visible = value;
            //separatorControl2.Visible = value;
        }
    }

    /// <summary>
    /// Tabelle/View, deren Felder angezeigt werden sollen.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DatabaseSchemeTable? Table
    {
        get => currentTable;
        set
        {
            currentTable = value;

            gridDatasourceBrowserFields.DataSource = value?.Fields;
            gridDatasourceBrowserFields.RefreshDataSource();
        }
    }

    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (UI.DesignMode) return;

        cmbDatasource.Properties.DataSource = datasources;
        treeClassStructure.FocusedNodeChanged += treeNodeSelected;
    }

    /// <inheritdoc />
    protected override void OnHandleDestroyed(EventArgs e)
    {
        treeClassStructure.FocusedNodeChanged -= treeNodeSelected;

        base.OnHandleDestroyed(e);
    }

    /// <summary>
    /// Registriert eine Datenquelle.
    ///
    /// Die Datenquellen sollten VOR der Anzeige des Elements registriert werden.
    /// </summary>
    /// <param name="ds">zu registrierende Datenquelle</param>
    public void RegisterDatasource(IDatabaseConnection ds)
    {
        datasources.Add(ds);
    }

    /// <summary>
    /// Löscht alle registrierten Datenquellen
    /// </summary>
    public void ClearDatasources()
    {
        datasources.Clear();
    }

    /// <summary>
    /// Delegate zur Anforderung der Beschreibung einer Tabelle/eines Views.
    ///
    /// Wird aufgerufen, wenn eine Tabelle/View ausgewählt wurde und die Beschreibung angezeigt werden soll.
    /// Kann NULL liefern, wenn keine Beschreibung existiert.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Func<IDatabaseConnection, DatabaseSchemeTable, string?>? RequestDescription { get; set; }

    private void loadScheme(bool forceLoad = false)
    {
        mleInfo.Text = "";

        if (currentDatasource == null) return;

        UI.ShowWait(Resources.LBL_WAIT, Resources.LBL_LOADDBSCHEME); // "Loading databases scheme...");
        try
        {
            currentScheme = null;
            Guid schemeId = Guid.Empty;

            if (currentDatasource is IConnectstring connstring && connstring.Id != Guid.Empty)
                schemeId = connstring.Id;

            if (forceLoad == false && schemeId.IsNotEmpty())
            {
                if (schemeBuffer.ContainsKey(schemeId))
                    currentScheme = schemeBuffer[schemeId];
            }

            if (currentScheme == null)
            {
                currentScheme = currentDatasource.GetScheme();

                if (currentScheme != null && schemeId.IsNotEmpty())
                    schemeBuffer.Add(schemeId, currentScheme);
            }

            treeLoaded = false;
            gridLoaded = false;

            if (grid.Visible)
                loadGrid();
            else
                loadTree();

            DatabaseSchemeLoaded?.Invoke(this, EventArgs.Empty);
        }
        finally
        {
            UI.HideWait();
        }
    }

    /// <summary>
    /// Ereignis, das ausgelöst wird, nachdem ein Datenbankschema geladen wurde.
    /// </summary>
    public event EventHandler? DatabaseSchemeLoaded;

    private void loadGrid()
    {
        gridDatasourceBrowser.DataSource = currentScheme?.Tables ?? null;
        gridDatasourceBrowser.RefreshDataSource();

        viewDatasourceBrowser.ExpandAllGroups();

        gridLoaded = true;
    }

    private void loadTree()
    {
        treeClassStructure.ClearNodes();

        if (currentDatasource == null || currentScheme == null) return;


        List<AFTreeListNode> nodes = [];

        AFTreeListNode tables = new() { Caption = "Tables", ImageIndex = (int)Symbol.Folder, Tag = null, ImageIndexSelected = (int)Symbol.Folder };
        AFTreeListNode views = new() { Caption = "Views", ImageIndex = (int)Symbol.Folder, Tag = null, ImageIndexSelected = (int)Symbol.Folder };

        nodes.Add(new() { Caption = currentDatasource.DatabaseName, ImageIndex = (int)Symbol.Database, ImageIndexSelected = (int)Symbol.Database });

        foreach (var table in currentScheme.Tables.Where(p => p.IsView == false).OrderBy(p => p.TABLE_NAME))
        {
            AFTreeListNode tableNode = new()
            {
                Caption = table.TABLE_NAME,
                ImageIndex = (int)Symbol.Table,
                Tag = table,
                ImageIndexSelected = (int)Symbol.Table
            };
            tables.ChildNodes.Add(tableNode);

            foreach (var field in table.Fields.OrderBy(f => f.FIELD_NAME))
            {
                tableNode.ChildNodes.Add(new() { Caption = field.FIELD_FULLNAME, ImageIndex = (int)Symbol.TextField, Tag = field, ImageIndexSelected = (int)Symbol.TextField });
            }
        }

        foreach (var view in currentScheme.Tables.Where(p => p.IsView).OrderBy(p => p.TABLE_NAME))
        {
            AFTreeListNode tableNode = new()
            {
                Caption = view.TABLE_NAME,
                ImageIndex = (int)Symbol.Table,
                Tag = view,
                ImageIndexSelected = (int)Symbol.Table
            };

            views.ChildNodes.Add(tableNode);

            foreach (var field in view.Fields.OrderBy(f => f.FIELD_NAME))
            {
                tableNode.ChildNodes.Add(new() { Caption = field.FIELD_FULLNAME, ImageIndex = (int)Symbol.TextField, Tag = field, ImageIndexSelected = (int)Symbol.TextField });
            }
        }

        nodes[0].ChildNodes.Add(tables);
        nodes[0].ChildNodes.Add(views);

        treeClassStructure.Fill(nodes);

        treeClassStructure.ExpandToLevel(1);

        treeLoaded = true;
    }

    private void treeNodeSelected(object sender, FocusedNodeChangedEventArgs e)
    {
        if (e.Node == null)
        {
            mleInfo.Text = "";
            return;
        }

        if (e.Node.Tag is DatabaseSchemeTable info)
            mleInfo.Text = info.TABLE_DESCRIPTION;
        else if (e.Node.Tag is DatabaseSchemeField propertyInfo)
            mleInfo.Text = propertyInfo.FIELD_DESCRIPTION;
        else
            mleInfo.Text = "";
    }
}
