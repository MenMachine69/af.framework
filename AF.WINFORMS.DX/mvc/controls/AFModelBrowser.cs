using DevExpress.Utils;
using DevExpress.Utils.Layout;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace AF.MVC;

/// <summary>
/// Browser for Models (View or Table)
/// </summary>
[ToolboxItem(true)]
[SupportedOSPlatform("windows")]
[DesignerCategory("Code")]
public class AFModelBrowser : AFEditor, IModelBrowser
{
    private AFCommand? _importCommand;
    private AFCommand? _addNewCommand;
    private AFComboboxModelType cmbModelType = null!;
    private AFTreeGrid treeModelBrowser = null!;
    private AFGridControl gridModelBrowser = null!;
    private GridView viewModelBrowser = null!;
    private AFLabel lblDescription = null!;
    private AFSkinnedPanel panelOptions = null!;
    private readonly AFGridExtender crGridExtender1 = null!;
    private readonly EventToken? dbChangedToken = null;
    private TypeDescription? _currentType;
    private readonly AFEditFind sleFind = null!;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFModelBrowser()
    {
        if (UI.DesignMode) return;

        Padding = new(2, 0, 0, 0);

        cmbModelType = new() { Name = nameof(cmbModelType) };
        sleFind = new() { Name = nameof(sleFind) };
        sleFind.Margin = new(3, 3, 3, 0);

        treeModelBrowser = new() { Name = nameof(treeModelBrowser), Dock = DockStyle.Fill, Visible = false };
        gridModelBrowser = new() { Name = nameof(gridModelBrowser), Dock = DockStyle.Fill };
        viewModelBrowser = new() { Name = nameof(viewModelBrowser) };
        lblDescription = new() { Name = nameof(lblDescription), Dock = DockStyle.Top , AutoSizeMode = LabelAutoSizeMode.Vertical, Padding = new(5)};
        panelOptions = new() { Name = nameof(panelOptions) };
        crGridExtender1 = new();

        var table = new AFTablePanel() { Dock = DockStyle.Fill, AutoSize = true, UseSkinIndents = false };
        Controls.Add(table);

        table.BeginLayout();
        table.Add(cmbModelType, 1, 1);
        table.Add(sleFind, 2, 1);
        var panel = table.Add<AFSkinnedPanel>(3, 1);
        panel.Controls.Add(treeModelBrowser);
        panel.Controls.Add(gridModelBrowser);

        panel = table.Add<AFSkinnedPanel>(4, 1);
        panel.Controls.Add(lblDescription);

        table.Add(panelOptions, 4, 1);

        table.SetRow(3, TablePanelEntityStyle.Relative, 1.0f);
        table.SetColumn(1, TablePanelEntityStyle.Relative, 1.0f);

        table.EndLayout();

        sleFind.Grid = gridModelBrowser;
        sleFind.Tree = treeModelBrowser;

        cmbModelType.AddButton(caption: "NEU", name: "pshNew", tooltip: UI.GetSuperTip("NEU", "Ein neues Objekt anlegen."));
        cmbModelType.AddButton(caption: "IMPORT", name: "pshNew", tooltip: UI.GetSuperTip("IMPORTIEREN", "Ein Objekt aus einer Datei importieren."));

        cmbModelType.Properties.Buttons[1].Visible = false;
        cmbModelType.Properties.Buttons[2].Visible = false;

        cmbModelType.ButtonPressed += (_, e) =>
        {
            switch (e.Button.Tag)
            {
                case string tag when tag == @"pshNew":
                    invokeCommandAddNew();
                    break;
                case string tag when tag == @"pshImport":
                    invokeCommandImport();
                    break;
            }
        };
        cmbModelType.SelectedValueChanged += modelTypeSelected;
        cmbModelType.BeforeAddType += beforeAddModelType;

        gridModelBrowser.MainView = viewModelBrowser;
        gridModelBrowser.ViewCollection.Add(viewModelBrowser);

        viewModelBrowser.BorderStyle = BorderStyles.NoBorder;
        viewModelBrowser.FocusRectStyle = DrawFocusRectStyle.None;
        viewModelBrowser.GridControl = gridModelBrowser;
        viewModelBrowser.OptionsView.ShowGroupPanel = false;
        viewModelBrowser.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
        viewModelBrowser.FocusedRowObjectChanged += modelSelected;

        treeModelBrowser.BorderStyle = BorderStyles.NoBorder;
        treeModelBrowser.OptionsView.ShowColumns = false;

        // !ACHTUNG! Den GridExtender immer erst konfigurieren,
        // wenn das Grid alle seine Views kennt!
        crGridExtender1.AddCustomColumnsMenu = true;
        crGridExtender1.AddDefaultMenu = true;
        crGridExtender1.ContainerControl = this;
        crGridExtender1.Grid = gridModelBrowser;
    }

    /// <inheritdoc />
    protected override void OnHandleDestroyed(EventArgs e)
    {
        if (dbChangedToken != null)
            AFCore.App.EventHub.Unsubscribe(dbChangedToken);

        base.OnHandleDestroyed(e);
    }

    /// <summary>
    /// Load all registered Model-Types.
    ///
    /// Before a type will be added, the event BeforeAddType will be fired.
    /// If the event is canceled, the type will not be added.
    /// </summary>
    /// <param name="onlyBrowsable">load only types marked as </param>
    public void LoadTypes(bool onlyBrowsable)
    {
        cmbModelType.LoadTypes(onlyBrowsable);
    }

    /// <summary>
    /// Liefert true, wenn mindestens ein ModelTyp auswählbar ist.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool HasModels => cmbModelType.HasModels;

    /// <summary>
    /// Momentan ausgewählter Typ
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TypeDescription? CurrentType => _currentType;

    /// <summary>
    /// Event that is fired before a type will be added to the model type combobox.
    /// Set Cancel to true to prevent the type from being added.
    /// </summary>
    public event CancelEventHandler? BeforeAddType;

    /// <summary>
    /// Ereignis, das ausgelöst wird, wenn ein Typ ausgewählt wurde.
    /// </summary>
    public event EventHandler? TypeSelected;

    private void invokeCommandImport()
    {
        if (_importCommand != null)
            UI.App.HandleResult(_importCommand.Execute(new CommandArgs() { CommandContext = eCommandContext.Browser }));
    }

    private void invokeCommandAddNew()
    {
        if (_addNewCommand != null)
            UI.App.HandleResult(_addNewCommand.Execute(new CommandArgs() { CommandContext = eCommandContext.Browser }));
    }

    /// <summary>
    /// react on BeforeAddType from the combobox
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void beforeAddModelType(object? sender, CancelEventArgs e)
    {
        BeforeAddType?.Invoke(sender, e);
    }


    private void modelTypeSelected(object? sender, EventArgs e)
    {
        if (dbChangedToken != null)
            AFCore.App.EventHub.Unsubscribe(dbChangedToken);

        if (cmbModelType.SelectedItem != null)
        {
            TypeDescription? tdesc = cmbModelType.SelectedType;

            if (tdesc == null) return;

            if (_currentType != null && _currentType == tdesc) return;

            _currentType = tdesc;

            TypeSelected?.Invoke(this, EventArgs.Empty);

            _importCommand = tdesc.GetController()!.GetCommand(eCommand.Import);
            _addNewCommand = tdesc.GetController()!.GetCommand(eCommand.New);

            (cmbModelType.Properties.Buttons[1].SuperTip.Items[1] as ToolTipItem)!.Text = tdesc.Caption + " neu anlegen.";
            (cmbModelType.Properties.Buttons[2].SuperTip.Items[1] as ToolTipItem)!.Text = tdesc.Caption + " aus Datei importieren.";

            cmbModelType.Properties.Buttons[1].Visible = _addNewCommand != null;
            cmbModelType.Properties.Buttons[2].Visible = _importCommand != null;

            AFCore.App.EventHub.Subscribe(tdesc.GetController()!.ControlledType, this, onDataChanged);

            if (tdesc.GetController() != null && tdesc.GetController() is IControllerUI uicontroller)
            {
                var setup = uicontroller.GetGridSetup(eGridStyle.Browser);

                treeModelBrowser.Visible = setup.BrowseAsTree;
                gridModelBrowser.Visible = !setup.BrowseAsTree;

                if (!setup.BrowseAsTree)
                    viewModelBrowser.Setup(setup);
                else
                {
                    treeModelBrowser.Setup(setup);
                    treeModelBrowser.RootValue = Guid.Empty;
                    treeModelBrowser.KeyFieldName = nameof(ITreeNode.NODE_ID);
                    treeModelBrowser.ParentFieldName = nameof(ITreeNode.NODE_PARENT_ID);
                }

                // load a plugin for filtering
                var filtertype = uicontroller.GetUIElementType(eUIElement.BrowserFilter);

                if (filtertype == null)
                {
                    if (panelOptions.Controls.Count > 0 && panelOptions.Controls[0] is IViewFilterPlugin filter)
                        filter.FilterChanged -= filterChanged;

                    panelOptions.Controls.Clear(true);
                    panelOptions.Visible = false;
                    (Controls[0] as AFTablePanel)!.Rows[^1].Visible = false;
                }
                else
                {
                    (Controls[0] as AFTablePanel)!.Rows[^1].Visible = true;
                    panelOptions.Visible = true;

                    if (panelOptions.Controls.Count > 0 && panelOptions.Controls[0].GetType() != filtertype)
                    {
                        if (panelOptions.Controls[0] is IViewFilterPlugin filter)
                            filter.FilterChanged -= filterChanged;
                    }

                    panelOptions.Controls.Clear(true);

                    if (panelOptions.Controls.Count == 0)
                    {
                        panelOptions.Controls.Add((Control)uicontroller.GetUIElement(eUIElement.BrowserFilter)!);
                        panelOptions.Size = new Size(
                            panelOptions.Width,
                            panelOptions.Controls[0].Height +
                            panelOptions.Padding.Vertical);
                        panelOptions.Controls[0].Dock = DockStyle.Fill;

                        if (panelOptions.Controls[0] is IViewFilterPlugin filter)
                            filter.FilterChanged += filterChanged;
                    }
                }

                reloadData();
            }
            else
                throw new Exception($@"UI Controller not found for type {tdesc.Type.Name}.");
        }
        else
        {
            _currentType = null;
            _importCommand = null;
            _addNewCommand = null;

            cmbModelType.Properties.Buttons[1].Visible = false;
            cmbModelType.Properties.Buttons[2].Visible = false;

            treeModelBrowser.Visible = false;
            gridModelBrowser.Visible = true;
        }
    }

    private void onDataChanged(object data, eHubEventType eventType, int messageCode)
    {
        if (data is not IModel model) return;

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

    private void onModelAdded(IModel model)
    {
        var data = (gridModelBrowser.Visible ? gridModelBrowser.DataSource as IBindingList : treeModelBrowser.DataSource as IBindingList);

        if (data == null) return;

        if (data.OfType<IModel>().FirstOrDefault(d => d.PrimaryKey.Equals(model.PrimaryKey)) == null)
        {
            var dispmodel = _currentType?.GetController()?.ReadBrowserModel(model.PrimaryKey) ?? null;
            if (dispmodel != null) data.Add(dispmodel);
        }

        if (treeModelBrowser.Visible)
            treeModelBrowser.RefreshDataSource();
    }

    private void onModelChanged(IModel model)
    {
        var data = (gridModelBrowser.Visible ? gridModelBrowser.DataSource as IBindingList : treeModelBrowser.DataSource as IBindingList);

        if (data == null) return;

        var current = data.OfType<IModel>().FirstOrDefault(d => d.PrimaryKey.Equals(model.PrimaryKey));

        if (current == null) return;

        //if (SelectedObject != null && SelectedObject.PrimaryKey == current.PrimaryKey)
        //    SelectedObject = current;

        var dispmodel = _currentType?.GetController()?.ReadBrowserModel(model.PrimaryKey) ?? null;

        if (dispmodel == null) return;

        var index = data.IndexOf(current);
        if (index >= 0)
            data[index] = dispmodel;


        if (treeModelBrowser.Visible)
            treeModelBrowser.RefreshDataSource();
    }

    private void onModelDeleted(IModel model)
    {
        var data = (gridModelBrowser.Visible ? gridModelBrowser.DataSource as IBindingList : treeModelBrowser.DataSource as IBindingList);

        if (data == null) return;

        var current = data.OfType<IModel>().FirstOrDefault(d => d.PrimaryKey.Equals(model.PrimaryKey));

        if (current == null) return;

        //if (SelectedObject != null && SelectedObject.PrimaryKey == current.PrimaryKey)
        //    SelectedObject = null;

        data.Remove(current);

        if (treeModelBrowser.Visible)
            treeModelBrowser.RefreshDataSource();
    }

    private void filterChanged(object? sender, EventArgs e)
    {
        reloadData();
    }

    private void reloadData()
    {
        if (_currentType == null) return;

        if (gridModelBrowser.Visible)
        {
            gridModelBrowser.DataSource = null;

            if (panelOptions.Controls.Count > 0 && panelOptions.Controls[0] is IViewFilterPlugin filter)
            {
                string filtertext = filter.GetFilterString(out var parameters);

                if (filtertext.IsNotEmpty())
                {
                    gridModelBrowser.DataSource = _currentType.GetController()!.ReadBrowserModels(true, filter: filtertext, args: parameters);
                    gridModelBrowser.RefreshDataSource();
                    return;
                }
            }

            gridModelBrowser.DataSource = _currentType.GetController()!.ReadBrowserModels(true);
            gridModelBrowser.RefreshDataSource();
            
            if (viewModelBrowser.GroupCount > 0)
                viewModelBrowser.ExpandGroupLevel(0);
        }
        else if (treeModelBrowser.Visible)
        {
            treeModelBrowser.DataSource = null;

            if (panelOptions.Controls.Count > 0 && panelOptions.Controls[0] is IViewFilterPlugin filter)
            {
                string filtertext = filter.GetFilterString(out var parameters);

                if (filtertext.IsNotEmpty())
                {
                    treeModelBrowser.DataSource = _currentType.GetController()!.ReadBrowserModels(true, filter: filtertext, args: parameters);
                    treeModelBrowser.RefreshDataSource();
                    return;
                }
            }

            treeModelBrowser.DataSource = _currentType.GetController()!.ReadBrowserModels(true);
            treeModelBrowser.RefreshDataSource();
        }
    }

    /// <summary>
    /// a model is selected in the grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void modelSelected(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventArgs e)
    {
        if (viewModelBrowser.GetFocusedRow() != null)
        {
            if (viewModelBrowser.GetFocusedRow() is IModel model)
                lblDescription.Text = model.ModelDescription ?? "";
        }
    }

    /// <summary>
    /// Ersten in der Combobox verfügbaren Typen auswählen
    /// </summary>
    public void SelectFirst()
    {
        cmbModelType.SelectedIndex = 0;
    }

    /// <summary>
    /// Ersten in der Combobox verfügbaren Typen auswählen
    /// </summary>
    public bool Select(Type modelType)
    {
        var item = cmbModelType.Properties.Items.FirstOrDefault(i => i.Value is TypeDescription model && model.Type == modelType);

        if (item == null) return false;

        var idx = cmbModelType.Properties.Items.IndexOf(item);

        if (idx < 0) return false;

        cmbModelType.SelectedIndex = idx;

        return true;
    }
}

