using DevExpress.XtraGrid.Views.Grid;

namespace AF.WINFORMS.DX;

/// <summary>
/// Browser für IDatasource-Datenquellen (Struktur)
/// </summary>
public partial class AFDatasourceBrowser : AFUserControl
{
    /// <summary>
    /// Constructor
    /// </summary>
    public AFDatasourceBrowser()
    {
        InitializeComponent();

        if (UI.DesignMode) return;

        mleInfo.AllowHtmlString = true;
        mleInfo.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
        mleInfo.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
        mleInfo.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
        mleInfo.Padding = new(5);
        mleInfo.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;

        AFGridSetup setup = new()
        {
            AllowAddNew = false,
            AllowEdit = false,
            AllowMultiSelect = false,
            SortOn = nameof(DatasourceField.FieldName),
            GroupBy = [nameof(DatasourceField.EntityDisplayName)],
            Columns =
            [
                new AFGridColumn() { Caption = "Quelle", ColumnFieldname = nameof(DatasourceField.EntityDisplayName) },
                new AFGridColumn() { Caption = "Name", Bold = true, ColumnFieldname = nameof(DatasourceField.FieldName) },
                new AFGridColumn() { Caption = "Typ", Bold = true, ColumnFieldname = nameof(DatasourceField.FieldType) }
            ]
        };

        viewDatasourceBrowser.Setup(setup);

        Fields = [];

        viewDatasourceBrowser.FocusedRowChanged += (_, _) =>
        {
            var row = viewDatasourceBrowser.GetFocusedRow() as DatasourceField;

            if (row == null) return;

            mleInfo.Text = $"<b>{row.FieldDisplayName}</b> ({row.FieldName})\r\n{row.FieldDescription}";
        };
    }

    /// <summary>
    /// Liste der Variablen im Browser
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IList<DatasourceField> Fields
    {
        get => (List<DatasourceField>)gridDatasourceBrowser.DataSource;
        set { gridDatasourceBrowser.DataSource = value; gridDatasourceBrowser.RefreshDataSource(); }
    }

    /// <summary>
    /// Zugriff auf das GridView der Variablen
    /// </summary>
    public GridView ViewFields => viewDatasourceBrowser;

    /// <summary>
    /// Zugriff auf das Grid der Variablen
    /// </summary>
    public AFGridControl GridFields => gridDatasourceBrowser;
}
