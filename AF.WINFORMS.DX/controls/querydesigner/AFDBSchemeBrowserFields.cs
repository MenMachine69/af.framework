using AF.WINFORMS.DX.Properties;
using DevExpress.XtraGrid.Views.Grid;

namespace AF.WINFORMS.DX;

/// <summary>
/// Browser zur Anzeige der Struktur einer Datenbank (AF.DATA.IDatasource).
///
/// Datenbanken/Datenquellen werden via RegisterDatasource registriert und stehen dann zur Auswahl in einer Combobox zur Verfügung. 
/// </summary>
public partial class AFDBSchemeBrowserFields : AFUserControl
{
    private DatabaseSchemeTable? _table;

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFDBSchemeBrowserFields()
    {
        InitializeComponent();

        if (UI.DesignMode) return;

        AFGridSetup setup = new()
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

        viewDatasourceBrowserFields.CustomColumnDisplayText += (s, e) =>
        {
            if (e.Column.FieldName != nameof(DatabaseSchemeField.FIELD_NAME)) return;

            e.DisplayText = @"<image=TextField;size=16,16>  " + e.Value;
        };

        viewDatasourceBrowserFields.FocusedRowChanged += (_,_) =>
        {
            if (!ShowInfo) return;

            if (viewDatasourceBrowserFields.GetFocusedRow() is not DatabaseSchemeField field) return;

            mleInfo.Text = RequestDescription?.Invoke(Table!, field) ?? field.FIELD_DESCRIPTION;
        };
    }

    /// <summary>
    /// Zugriff auf das GridView
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GridView GridView => viewDatasourceBrowserFields;

    /// <summary>
    /// Zugriff auf das GridControl
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFGridControl GridControl => gridDatasourceBrowserFields;


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

    /// <summary>
    /// Tabelle/View, deren Felder angezeigt werden sollen.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DatabaseSchemeTable? Table
    {
        get => _table;
        set
        {
            _table = value;

            gridDatasourceBrowserFields.DataSource = value?.Fields;
            gridDatasourceBrowserFields.RefreshDataSource();
        }
    }

    /// <summary>
    /// Delegate zur Anforderung der Beschreibung einer Tabelle/eines Views.
    ///
    /// Wird aufgerufen, wenn eine Tabelle/View ausgewählt wurde und die Beschreibung angezeigt werden soll.
    /// Kann NULL liefern, wenn keine Beschreibung existiert.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Func<DatabaseSchemeTable, DatabaseSchemeField, string?>? RequestDescription { get; set; }
}
