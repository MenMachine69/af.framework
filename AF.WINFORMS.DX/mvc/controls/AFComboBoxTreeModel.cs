using DevExpress.XtraEditors.Controls;

namespace AF.MVC;

/// <summary>
/// Control zur Auswahl von Models.
/// </summary>
/// <typeparam name="T">Typ der auswählbaren Models</typeparam>
[ToolboxItem(false)]
[SupportedOSPlatform("windows")]
[DefaultBindingProperty("SelectedValue")]
[DesignerCategory("Code")]
public class AFComboBoxTreeModel<T> : AFComboboxTree where T : class, IModel, ITreeNode
{
    private bool _loaded;
    private EventToken? token;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFComboBoxTreeModel()
    {
        if (UI.DesignMode) return;

        Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
        Properties.AllowMouseWheel = false;
        Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        Properties.ValueMember = nameof(IModel.PrimaryKey);
    }

    /// <summary>
    /// Filterbedingung für die anzuzeigende Auswahl
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? Filter { get; set; }

    /// <summary>
    /// Parameter für die Filterbedingung
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public object[]? FilterParameters { get; set; }

    /// <summary>
    /// Archivierte zur Auswahl anbieten
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ShowArchived { get; set; }

    /// <summary>
    /// Command 'Goto' verwenden.
    ///
    /// Standard ist 'true'.
    /// </summary>
    [Browsable(true), DefaultValue(true)]
    public bool ShowGoto { get; set; } = true;

    /// <summary>
    /// Command 'Edit' verwenden.
    ///
    /// Standard ist 'true'.
    /// </summary>
    [Browsable(true), DefaultValue(true)]
    public bool ShowEdit { get; set; } = true;

    /// <summary>
    /// Command 'Add' verwenden.
    ///
    /// Standard ist 'true'.
    /// </summary>
    [Browsable(true), DefaultValue(true)]
    public bool ShowAdd { get; set; } = true;

    /// <summary>
    /// Symbol der auswählbaren Objekte anzeigen.
    ///
    /// Standard ist 'true'.
    /// </summary>
    [Browsable(true), DefaultValue(true)]
    public bool ShowSymbol { get; set; } = true;

    /// <summary>
    /// Command 'Delete' verwenden.
    ///
    /// Standard ist 'false'.
    /// </summary>
    [Browsable(true), DefaultValue(false)]
    public bool ShowDelete { get; set; } = false;

    /// <summary>
    /// Command 'ShowDetail' verwenden.
    ///
    /// Standard ist 'false'.
    /// </summary>
    [Browsable(true), DefaultValue(false)]
    public bool ShowDetails { get; set; } = false;


    /// <summary>
    /// überschriebene EditValue Eigenschaft damit Guid's unterstützt werden.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Guid SelectedValue
    {
        get
        {
            if (EditValue is Guid guid)
                return guid;

            return Guid.Empty;
        }
        set
        {
            if (value.Equals(Guid.Empty))
            {
                EditValue = null;
                return;
            }

            if (Properties.GetIndexByKeyValue(value) < 0) // element not loaded
                LoadModel(value);

            if (Properties.GetIndexByKeyValue(value) >= 0)
                EditValue = value;
            else
                EditValue = null;
        }
    }

    /// <summary>
    /// ID of the selected model as strong typed Guid (for easier access)
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Guid CurrentModelID => (Guid)EditValue;

    /// <inheritdoc />
    protected override void OnHandleDestroyed(EventArgs e)
    {
        base.OnHandleDestroyed(e);

        if (UI.DesignMode) return;

        if (token != null)
            AFCore.App.EventHub.Unsubscribe(token);

        CustomizeSetup -= customizeSetup;
    }


    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (UI.DesignMode) return;

        CustomizeSetup += customizeSetup;

        Setup(typeof(T));

        Properties.TreeList.OptionsView.ShowColumns = false;
        Properties.TreeList.ParentFieldName = nameof(ITreeNode.NODE_PARENT_ID);
        Properties.TreeList.KeyFieldName = nameof(ITreeNode.NODE_ID);

        if (Properties.DataSource is not IBindingList)
            Properties.DataSource = typeof(T).GetController().GetGridModelType(eGridStyle.ComboboxEntrys).CreateBindingList();

        QueryPopUp += (_, _) =>
        {
            Properties.PopupFormMinSize = Properties.PopupFormSize with { Width = Width };

            if (!_loaded)
            {
                LoadModels(SelectedValue);
                _loaded = true;

                token = AFCore.App.EventHub.Subscribe(typeof(T), this, onDataChanged);
            }
        };
    }

    private void onDataChanged(object data, eHubEventType eventType, int messageCode)
    {
        if (data is not T model) return;

        switch (eventType)
        {
            case eHubEventType.ObjectAdded:
                onModelAdded(model);
                break;
            case eHubEventType.ObjectChanged:
                onModelChanged(model);
                break;
            case eHubEventType.ObjectDeleted:
                onModelDeleted(model);
                break;
        }
    }

    private void onModelDeleted(T model)
    {
        var entry = (Properties.DataSource as IBindingList)!.OfType<IModel>().FirstOrDefault(o => o.PrimaryKey == model.PrimaryKey);

        if (entry != null)
        {
            (Properties.DataSource as IBindingList)!.Remove(entry);

            if (SelectedValue == entry.PrimaryKey)
                SelectedValue = Guid.Empty;

            DataBindings[0].WriteValue();

            Properties.TreeList.RefreshDataSource();
        }
    }

    private void onModelChanged(T model)
    {
        var entry = typeof(T).GetController().ReadSelectionModel(model.PrimaryKey);

        if (entry == null) return;

        var oldentry = (Properties.DataSource as IBindingList)!.OfType<IModel>().FirstOrDefault(o => o.PrimaryKey == model.PrimaryKey);

        if (oldentry != null)
            oldentry.CopyFrom(entry, true);
        else
            (Properties.DataSource as IBindingList)!.Add(entry);

        Properties.TreeList.RefreshDataSource();
    }

    private void onModelAdded(T model)
    {
        var entry = typeof(T).GetController().ReadSelectionModel(model.PrimaryKey);

        if (entry != null)
            (Properties.DataSource as IBindingList)!.Add(entry);

        Properties.TreeList.RefreshDataSource();
    }

    private void customizeSetup(object? sender, EventArgs e)
    {
        if (sender is not AFGridSetup setup) return;

        if (!ShowSymbol)
            setup.Symbol = null;
        else
            setup.Symbol = typeof(T).GetController().TypeImage;


        // remove not allowed commands
        if (!ShowGoto)
            setup.CmdGoto = null;

        if (!ShowEdit)
            setup.CmdEdit = null;

        if (!ShowAdd)
            setup.CmdAdd = null;

        if (!ShowDelete)
            setup.CmdDelete = null;

        if (!ShowDetails)
            setup.CmdShowDetail = null;
    }

    /// <summary>
    /// Load a model with the given ID to the selection
    /// </summary>
    /// <param name="modelID"></param>
    public virtual void LoadModel(Guid modelID)
    {
        var entry = typeof(T).GetController().ReadSelectionModel(modelID);

        if (entry != null)
            (Properties.DataSource as IBindingList)!.Add(entry);
    }

    /// <summary>
    /// Load all selectable models to the selection
    /// </summary>
    /// <param name="currentModelID">ID of the current selected model. This model must be included in the list</param>
    public virtual void LoadModels(Guid currentModelID)
    {
        Properties.DataSource = typeof(T).GetController().ReadSelectionModels(ShowArchived, filter: Filter, args: FilterParameters);

        if (currentModelID.IsNotEmpty() && Properties.GetIndexByKeyValue(currentModelID) < 0)
            LoadModel(currentModelID);
    }
}