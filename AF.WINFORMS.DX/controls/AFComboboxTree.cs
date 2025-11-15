using AF.MVC;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTreeList;

namespace AF.WINFORMS.DX;

/// <summary>
/// Combobox, die ein TreeView zur Auswahl anzeigt.
/// 
/// Unterstützt direkt die Verwendung von ITreeNode-Listen als Datenquelle.
/// Dazu muss lediglich die Methode Setup() aufgerufen werden.
/// </summary>
[SupportedOSPlatform("windows")]
[DefaultBindingProperty("SelectedNode")]
[ToolboxItem(true)]
[DesignerCategory("Code")]
public class AFComboboxTree : TreeListLookUpEdit
{
    BindingList<ITreeNode> datasource = [];

    /// <summary>
    /// Methode um die Baumansicht so zu konfigurieren, dass als Datenquelle eine Liste mit
    /// ITreeNode-Objekten verwendet werden kann.
    /// </summary>
    /// <param name="images">Image collection</param>
    public void Setup(SvgImageCollection? images = null)
    {
        Properties.TreeList ??= new TreeList();

        Properties.TreeList.ForceInitialize();

        Properties.TreeList.Columns.Clear();
        Properties.TreeList.Setup(images);
        Properties.TreeList.GetStateImage += _getStateImage;

        Properties.AllowNullInput = DefaultBoolean.True;
        Properties.DisplayMember = nameof(ITreeNode.NODE_NAME);
        Properties.ExportMode = DevExpress.XtraEditors.Repository.ExportMode.Value;
        Properties.NullValuePrompt = @"<no parent folder>";
        Properties.TreeList = Properties.TreeList;
        Properties.UseAdvancedMode = DefaultBoolean.True;
        Properties.ValueMember = nameof(ITreeNode.NODE_ID);
        ParseEditValue += _parseEditValue;
    }

    /// <summary>
    /// Datenquelle
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public BindingList<ITreeNode> Datasource
    {
        get => datasource;
        set
        {
            datasource = value;
            Properties.TreeList.DataSource = datasource;
            Properties.TreeList.RefreshDataSource();
        }
    }

    /// <summary>
    /// ID der ausgewählten TreeNode
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Guid SelectedNode
    {
        get => base.EditValue != null && base.EditValue is Guid guid ? guid : Guid.Empty;
        set => base.EditValue = value;
    }

    /// <summary>
    /// Index des Symbols für eine geöffnete Node
    /// </summary>
    [Browsable(true)]
    [DefaultValue(0)]
    public int ImageIndexNodeOpen { get; set; }

    /// <summary>
    /// Index des Symbols für eine gesachlossene Node
    /// </summary>
    [Browsable(true)]
    [DefaultValue(0)]
    public int ImageIndexNodeClosed { get; set; }

    /// <summary>
    /// Ereignis, dass zur Anpassung eines Setups (AFGridSetup) ausgelöst wird. Durch dieses Ereignis kann ein Setup, 
    /// dass über den Controller eines IModels ermittelt wurde noch angepasst werden.
    /// 
    /// Das Setup wird als sender übergeben.
    /// </summary>
    public event EventHandler<EventArgs>? CustomizeSetup;

    /// <summary>
    /// Setup der Combobox für ein bestimmtes IModel.
    /// </summary>
    /// <param name="type"></param>
    protected void Setup(Type type)
    {
        Properties.TreeList.ForceInitialize();

        AFGridSetup? setup = null;

        var controller = type.GetController();

        if (controller is IControllerUI uiController)
            setup = uiController.GetGridSetup(eGridStyle.ComboboxEntrys);

        setup ??= type.GetTypeDescription().GetGridSetup(eGridStyle.ComboboxEntrys);

        if (setup == null)
            throw new NullReferenceException($@"There is no GridSetup available for type {type.FullName}.");


        setup.AllowEdit = false;

        // Anpassungen gewünscht?
        CustomizeSetup?.Invoke(setup, EventArgs.Empty);
        
        this.Setup(setup);
    }

    private void _getStateImage(object sender, GetStateImageEventArgs e)
    {
        if (e.Node.Expanded || e.Node.Focused)
            e.NodeImageIndex = ImageIndexNodeOpen;
        else
            e.NodeImageIndex = ImageIndexNodeClosed;
    }

    private void _parseEditValue(object sender, ConvertEditValueEventArgs e)
    {
        if (e.Value == null || e.Value == DBNull.Value)
        {
            e.Value = Guid.Empty;
            e.Handled = true;
        }
    }
}