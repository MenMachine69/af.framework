using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList;
using AF.WINFORMS.DX.Properties;

namespace AF.WINFORMS.DX;

/// <summary>
/// Browser für Klassen und Typen z.B. im ScriptDesigner
/// </summary>
public partial class AFDBSchemeTreeBrowser : AFUserControl
{
    private readonly BindingList<IDatabaseConnection> datasource = [];

    /// <summary>
    /// Constructor
    /// </summary>
    public AFDBSchemeTreeBrowser()
    {
        InitializeComponent();

        if (UI.DesignMode) return;

        cmbClass.Properties.ValueMember = nameof(IDatabaseConnection.DatabaseName);
        cmbClass.Properties.DisplayMember = nameof(IDatabaseConnection.DatabaseName);
        cmbClass.Properties.ExportMode = ExportMode.Value;

        cmbClass.Properties.View.OptionsBehavior.AutoPopulateColumns = false;
        cmbClass.Properties.View.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;

        // cmbDatasource.Properties.AllowNullInput = DefaultBoolean.True;
        cmbClass.Properties.NullValuePromptShowForEmptyValue = true;
        cmbClass.Properties.NullText = Resources.LBL_SELECTDATASOURCE;

        cmbClass.AddButton(UI.GetImage(Symbol.Database),
            imagesize: new Size(16, 16),
            enabled: false,
            showleft: true);

        AFGridSetup setup = new()
        {
            SortOn = nameof(IDatabaseConnection.DatabaseName),
            DefaultGridStyle = eGridMode.GridView
        };
        setup.Columns.Add(new() { ColumnType = typeof(string), Caption = @"Database", ColumnFieldname = nameof(IDatabaseConnection.DatabaseName) });

        cmbClass.Setup(setup);

        cmbClass.EditValueChanged += (_, _) =>
        {
            if (cmbClass.GetSelectedObject() is not IDatabaseConnection ds) return;

            var scheme = ds.GetScheme();

            loadTree(scheme, ds.DatabaseName);
        };

        treeClassStructure.SelectImageList = Glyphs.GetImages();
    }

    /// <summary>
    /// Zugriff auf die Baumansicht
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFTreeGrid TreeView => treeClassStructure;

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

    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (UI.DesignMode) return;

        cmbClass.Properties.DataSource = datasource;
        treeClassStructure.FocusedNodeChanged += treeNodeSelected;
    }

    /// <summary>
    /// Registriert eine Datenquelle.
    ///
    /// Die Datenquellen sollten VOR der Anzeige des Elements registriert werden.
    /// </summary>
    /// <param name="ds">zu registrierende Datenquelle</param>
    public void RegisterDatasource(IDatabaseConnection ds)
    {
        datasource.Add(ds);
    }

    /// <summary>
    /// Löscht alle registrierten Datenquellen
    /// </summary>
    public void ClearDatasources()
    {
        datasource.Clear();
    }

    /// <summary>
    /// Delegate zur Anforderung der Beschreibung einer Tabelle/eines Views.
    ///
    /// Wird aufgerufen, wenn eine Tabelle/View ausgewählt wurde und die Beschreibung angezeigt werden soll.
    /// Kann NULL liefern, wenn keine Beschreibung existiert.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Func<IDatabaseConnection, DatabaseSchemeTable, string?>? RequestDescription { get; set; }

    /// <inheritdoc />
    protected override void OnHandleDestroyed(EventArgs e)
    {
        treeClassStructure.FocusedNodeChanged -= treeNodeSelected;

        base.OnHandleDestroyed(e);
    }

    private void loadTree(DatabaseScheme scheme, string dbname)
    {
        treeClassStructure.ClearNodes();

        List<AFTreeListNode> nodes = [];

        AFTreeListNode tables = new() { Caption = "Tables", ImageIndex = (int)Symbol.Folder, Tag = null, ImageIndexSelected = (int)Symbol.Folder };
        AFTreeListNode views = new() { Caption = "Views", ImageIndex = (int)Symbol.Folder, Tag = null, ImageIndexSelected = (int)Symbol.Folder };

        nodes.Add(new() { Caption = dbname, ImageIndex = (int)Symbol.Database, ImageIndexSelected = (int)Symbol.Database });

        foreach (var table in scheme.Tables.Where( p => p.IsView == false).OrderBy(p => p.TABLE_NAME))
        {
            AFTreeListNode tableNode = new()
            {
                Caption = $"{table.TABLE_NAME} ({table.TABLE_SCHEME})", ImageIndex = (int)Symbol.Table, Tag = table,
                ImageIndexSelected = (int)Symbol.Table
            };
            tables.ChildNodes.Add(tableNode);

            foreach (var field in table.Fields.OrderBy(f => f.FIELD_NAME))
            {
                tableNode.ChildNodes.Add(new() { Caption = field.FIELD_FULLNAME, ImageIndex = (int)Symbol.TextField, Tag = field, ImageIndexSelected = (int)Symbol.TextField });
            }
        }

        foreach (var view in scheme.Tables.Where(p => p.IsView).OrderBy(p => p.TABLE_NAME))
        {
            AFTreeListNode tableNode = new()
            {
                Caption = $"{view.TABLE_NAME} ({view.TABLE_SCHEME})", 
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
