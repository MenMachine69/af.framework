using AF.MVC;

namespace AF.WINFORMS.DX;

/// <summary>
/// Designer/Editor für Export-Dokumentvorlagen
/// </summary>
public partial class AFDocumentDesignerExport : AFUserControl, IVariableConsumer
{
    private readonly AFVariableBrowser variableBrowser = null!;
    private readonly AFDatasourceBrowser datasourceBrowser = null!;
    private object? dataSource;
    private readonly AFGridExtender extenderVariablen = null!;
    private readonly AFGridExtender extenderDatasource = null!;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFDocumentDesignerExport()
    {
        InitializeComponent();

        if (UI.DesignMode) return;

        menPreview.ImageOptions.SvgImage = UI.GetImage(Symbol.DocumentSearch);
        menPreview.ItemClick += (_, _) =>
            {
                // TODO: Preview für Export-Dokument ergänzen.
                MsgBox.ShowInfoOk("PREVIEW\r\nKommt noch hier hin...");
            };

        variableBrowser = new() { Dock = DockStyle.Fill };
        dockVariablen.Controls.Add(variableBrowser);

        datasourceBrowser = new() { Dock = DockStyle.Fill };
        dockDatasource.Controls.Add(datasourceBrowser);

        AFDragDropHelper dragHelper1 = new();
        dragHelper1.Register(mleHeader, acceptDropData, dropDataHeader);
        dragHelper1.Register(mleData, acceptDropData, dropDataData);
        dragHelper1.Register(mleFooter, acceptDropData, dropDataFooter);

        extenderVariablen = new();
        extenderVariablen.Grid = variableBrowser.GridVariablen;
        extenderVariablen.SupportDragDrop = true;
        extenderVariablen.RequestDragData += (_, e) =>
        {
            if (e.DraggedRow is IVariable snippet)
                e.DragData = "{"+snippet.VAR_NAME +"}";
        };

        extenderDatasource = new();
        extenderDatasource.Grid = datasourceBrowser.GridFields;
        extenderDatasource.SupportDragDrop = true;
        extenderDatasource.RequestDragData += (_, e) =>
        {
            if (e.DraggedRow is DatasourceField snippet)
                e.DragData = "#" + snippet.FieldName + "#";
        };

        lblHeaderMaster.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Window);
        lblHeaderDetail.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Window);
        lblFooter.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Window);

        UI.StyleChanged += (_, _) =>
        {
            lblHeaderMaster.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Window);
            lblHeaderDetail.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Window);
            lblFooter.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Window);
        };
    }

    private void dropDataHeader(System.Windows.Forms.IDataObject obj, Point pos)
    {
        var data = obj.GetData(typeof(string)) as string;

        if (data == null) return;

        if (string.IsNullOrEmpty(data)) return;

        mleHeader.InnerEditorPaste(data);
    }

    private void dropDataData(System.Windows.Forms.IDataObject obj, Point pos)
    {
        var data = obj.GetData(typeof(string)) as string;

        if (data == null) return;

        if (string.IsNullOrEmpty(data)) return;

        mleData.InnerEditorPaste(data);
    }

    private void dropDataFooter(System.Windows.Forms.IDataObject obj, Point pos)
    {
        var data = obj.GetData(typeof(string)) as string;

        if (data == null) return;

        if (string.IsNullOrEmpty(data)) return;

        mleFooter.InnerEditorPaste(data);
    }

    private bool acceptDropData(System.Windows.Forms.IDataObject obj)
    {
        var data = obj.GetData(typeof(string)) as string;

        if (data == null) return false;

        if (string.IsNullOrEmpty(data)) return false;

        return true;
    }

    /// <summary>
    /// Header/Kopf der Ausgabe
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Header { get => mleHeader.Text; set => mleHeader.Text = value; }

    /// <summary>
    /// Footer/Fuss der Ausgabe
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Footer { get => mleFooter.Text; set => mleFooter.Text = value; }

    /// <summary>
    /// Detail/Data
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Detail { get => mleData.Text; set => mleData.Text = value; }

    /// <summary>
    /// Zugriff auf die Liste der Variablen
    /// </summary>
    public BindingList<Variable> Variables => variableBrowser.Variables;

    /// <summary>
    /// Datenquelle des Dokuments
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public object? DataSource { 
        get => dataSource;
        set
        {
            dataSource = value;

            if (value is IModel)
            {
                var dict = value.GetType().GetTypeDescription().GetAsDictionary(null, false, 1, 2, "", "");
                datasourceBrowser.Fields = dict.Values.ToList();
            }
        }
    }

}

