using AF.WINFORMS.DX.Properties;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace AF.WINFORMS.DX;

/// <summary>
/// Browser zur Anzeige der Struktur einer Datenbank (AF.DATA.IDatasource).
///
/// Datenbanken/Datenquellen werden via RegisterDatasource registriert und stehen dann zur Auswahl in einer Combobox zur Verfügung. 
/// </summary>
public partial class AFDBSchemeTableBrowser : AFUserControl
{
    private readonly BindingList<IDatabaseConnection> datasource = [];

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFDBSchemeTableBrowser()
    {
        InitializeComponent();

        if (UI.DesignMode) return;

        cmbDatasource.Properties.ValueMember = nameof(IDatabaseConnection.DatabaseName);
        cmbDatasource.Properties.DisplayMember = nameof(IDatabaseConnection.DatabaseName);
        cmbDatasource.Properties.ExportMode = ExportMode.Value;

        cmbDatasource.Properties.View.OptionsBehavior.AutoPopulateColumns = false;
        cmbDatasource.Properties.View.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
        
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

        cmbDatasource.EditValueChanged += (_, _) =>
        {
            if (cmbDatasource.GetSelectedObject() is not IDatabaseConnection ds) return;

            gridDatasourceBrowser.DataSource = ds.GetScheme().Tables;
            gridDatasourceBrowser.RefreshDataSource();
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

        viewDatasourceBrowser.HtmlImages = Glyphs.GetImages();

        viewDatasourceBrowser.CustomColumnDisplayText += (s, e) =>
        {
            if (e.Column.FieldName != nameof(DatabaseSchemeTable.TABLE_NAME)) return;

            e.DisplayText = @"<image=Table;size=16,16>  " + e.Value;
        };

        viewDatasourceBrowser.FocusedRowChanged += (_,_) =>
        {
            if (!ShowInfo) return;

            if (viewDatasourceBrowser.GetFocusedRow() is not DatabaseSchemeTable table) return;

            mleInfo.Text = RequestDescription?.Invoke((IDatabaseConnection)cmbDatasource.GetSelectedObject()!, table) ?? table.TABLE_DESCRIPTION;

            if (FieldBrowser != null) FieldBrowser.Table = table;
        };
    }

    /// <summary>
    /// Browser, in dem die Felder der ausgewählten Tabelle/View angezeigt werden.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFDBSchemeBrowserFields? FieldBrowser { get; set; }

    /// <summary>
    /// Zugriff auf das GridView
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GridView GridView => viewDatasourceBrowser;

    /// <summary>
    /// Zugriff auf das GridControl
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFGridControl GridControl => gridDatasourceBrowser;

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
            separatorControl2.Visible = value;
        }
    }

    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (UI.DesignMode) return;

        cmbDatasource.Properties.DataSource = datasource;
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
}
