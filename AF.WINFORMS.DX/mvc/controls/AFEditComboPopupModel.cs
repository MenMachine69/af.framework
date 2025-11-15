using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;

namespace AF.MVC;

/// <summary>
/// Eventargumente für ein Event SelectedModelChanged.
/// </summary>
public sealed class ModelSelectEventArgs : EventArgs
{
    /// <summary>
    /// ID des ausgewählten Models
    /// </summary>
    public Guid ModelID { get; set; }
}

/// <summary>
/// Optionen für die Anzeige von AFEditComboPopupModel
/// </summary>
public struct AFEditComboPopupModelOptions
{
    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFEditComboPopupModelOptions() { }

    /// <summary>
    /// Browser/Liste anzeigen
    /// </summary>
    public bool ShowBrowser { get; set; }= false;

    /// <summary>
    /// Verlauf/History anzeigen (wenn verfügbar)
    /// </summary>
    public bool ShowHistory { get; set; } = true;

    /// <summary>
    /// Lesezeichen/Bookmarks anzeigen (wenn verfügbar)
    /// </summary>
    public bool ShowBookmarks { get; set; } = true;
    
    /// <summary>
    /// Suche (SearchEngine) anzeigen (wenn verfügbar)
    /// </summary>
    public bool ShowSearch { get; set; } = true;
}

/// <summary>
/// Combobox mit eigenem Popup zur Auswahl eines Models.
/// 
/// Über den UI-Controller des Models wird das Popup gesteuert.
/// </summary>
/// <typeparam name="T">Typ des auszuwählenden Models</typeparam>
[DefaultBindingProperty("SelectedValue")]
public class AFEditComboPopupModel<T> : AFEditComboPopup, IComboboxModel where T : class, ITable
{
    private readonly List<IComboboxModel> childs = [];
    private ModelLink? _currenModelLink = null;
    private EventToken? token;
    private AFEditComboPopupPopupModel<T>? _popup;
    private WeakEvent<EventHandler<ModelSelectEventArgs>>? _selectedModelChanged;
    private readonly AFEditComboPopupModelOptions _options;

    /// <summary>
    /// Ereignis, das ausgelöst wird, wenn ein Model ausgewählt wurde.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public event EventHandler<ModelSelectEventArgs> SelectedModelChanged
    {
        add
        {
            _selectedModelChanged ??= new();
            _selectedModelChanged.Add(value);
        }
        remove => _selectedModelChanged?.Remove(value);
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public AFEditComboPopupModel(AFEditComboPopupModelOptions options)
    {
        if (UI.DesignMode) return;

        _options = options;

        Properties.AllowMouseWheel = false;
        Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        Properties.AllowNullInput = DefaultBoolean.True;
        Properties.NullValuePrompt = "";
        Properties.NullValuePromptShowForEmptyValue = true;
        QueryDisplayText += (_, e) =>
        {
            e.DisplayText = _currenModelLink?.ModelCaption ?? string.Empty;
        };

        //Properties.View.OptionsView.ShowIndicator = false;
        //Properties.View.FocusRectStyle = DrawFocusRectStyle.RowFullFocus;
        //Properties.View.OptionsSelection.EnableAppearanceFocusedCell = false;
        //Properties.ValueMember = nameof(IModel.PrimaryKey);
    }

    /// <summary>
    /// Zugriff auf das Popup
    /// </summary>
    public AFEditComboPopupPopupModel<T>? PopupControl => _popup;

    /// <summary>
    /// Setup des Popups und der Funktionen (Buttons)
    /// </summary>
    public void Setup()
    {
        _popup = new AFEditComboPopupPopupModel<T>(this, _options);
        Setup(_popup);

        this.Setup(CustomizeSetup(typeof(T).GetController().GetGridSetup(eGridStyle.SearchHits)));
    }

    /// <summary>
    /// Anpassungen des Setups erlauben.
    /// </summary>
    /// <param name="setup">anzupassendes Setup</param>
    /// <returns>das angepasste Setup</returns>
    public virtual AFGridSetup CustomizeSetup(AFGridSetup setup)
    {
        if (!ShowAdd)
            setup.AllowAddNew = false;

        if (!ShowEdit)
            setup.AllowEdit = false;

        if (!ShowGoto)
            setup.CmdGoto = null;


        return setup;
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
                _currenModelLink = null;
                EditValue = null;

                _selectedModelChanged?.Raise(this, new ModelSelectEventArgs { ModelID = Guid.Empty });

                return;
            }

            if (value.Equals(EditValue)) return;

            if (_currenModelLink?.ModelID.Equals(value) ?? false) return;

            _currenModelLink = typeof(T).GetController().GetModelLink(value);
            EditValue = value;

            _selectedModelChanged?.Raise(this, new ModelSelectEventArgs { ModelID = value });
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

        // CustomizeSetup -= customizeSetup;
    }


    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (UI.DesignMode) return;

        //ForceInitialize();

        //CustomizeSetup += customizeSetup;

        //Setup(typeof(T));

        //if (Properties.DataSource is not IBindingList)
        //    Properties.DataSource = typeof(T).GetController().GetGridModelType(eGridStyle.ComboboxEntrys).CreateBindingList();

        QueryPopUp += (_, _) =>
        {
            Properties.PopupFormMinSize = Properties.PopupFormSize with { Width = Width };

            //if (!_loaded)
            //{
            //    LoadModels(SelectedValue);
            //    _loaded = true;

            //    token = AFCore.App.EventHub.Subscribe(typeof(T), this, onDataChanged);
            //}

            token = AFCore.App.EventHub.Subscribe(typeof(T), this, onDataChanged);
        };
    }

    #region DataEvents
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
        if (SelectedValue == model.PrimaryKey)
            SelectedValue = Guid.Empty;

        DataBindings[0]?.WriteValue();
    }

    private void onModelChanged(T model)
    {
        var entry = typeof(T).GetController().ReadSelectionModel(model.PrimaryKey);

        if (entry == null)
            return;

    }

    private void onModelAdded(T model)
    {

    }
    #endregion

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

        //if (entry != null)
        //    (Properties.DataSource as IBindingList)!.Add(entry);
    }

    /// <summary>
    /// Load all selectable models to the selection
    /// </summary>
    /// <param name="currentModelID">ID of the current selected model. This model must be included in the list</param>
    public virtual void LoadModels(Guid currentModelID)
    {
        //Properties.DataSource = typeof(T).GetController().ReadSelectionModels(ShowArchived, filter: Filter, args: FilterParameters);

        //if (currentModelID.IsNotEmpty() && Properties.GetIndexByKeyValue(currentModelID) < 0)
        //    LoadModel(currentModelID);
    }
}