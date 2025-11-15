using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.Utils.Svg;
using DevExpress.XtraBars;

namespace AF.MVC;

/// <summary>
/// Seite, die ein IModel-Objekt im ViewManager darstellt.
/// 
/// Die Seite kann u.a. auch Detailansichten für das IModel anzeigen (1:n Beziehungen usw.).
/// </summary>
[ToolboxItem(false)]
public partial class AFPage : AFUserControl, IViewPage
{
    private IControllerUI? _currentController;
    private IEditor? _viewEditor;
    private IViewDetail? _detailView;
    private IControllerUI? _currentDetailController;
    private ModelLink? _modelLink;
    private AFFKeyBar? _keyBar;
    private UIPersistModelPageShortcuts? _shortcuts;
    private readonly Guid persistID = new ("{6A80B75D-5F58-4126-940D-6B699A1E6315}");
    private EventToken? _dataevent;
    private string _viewContextMaster = string.Empty;
    private string _viewContextDetail = string.Empty;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFPage()
    {
        InitializeComponent();

        if (UI.DesignMode) return;

        BarManager.Images = Glyphs.GetImages();
        BarManager.ShowScreenTipsInMenus = true;

        menMasterPopup.AllowGlyphSkinning = DefaultBoolean.False;
        menMasterPopup.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.None;
        menMasterPopup.DrawMenuSideStrip = DefaultBoolean.False;

        menDetailViews.AllowGlyphSkinning = DefaultBoolean.False;
        menDetailViews.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.None;
        menDetailViews.DrawMenuSideStrip = DefaultBoolean.True;
        menDetailViews.ImageOptions.SvgImageSize = new(24, 24);

        shortcut1.AllowGlyphSkinning = DefaultBoolean.False;
        shortcut1.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.None;
        shortcut1.ImageOptions.SvgImageSize = new(24, 24);
        shortcut1.ItemClick += shortcutClick;

        shortcut2.AllowGlyphSkinning = DefaultBoolean.False;
        shortcut2.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.None;
        shortcut2.ImageOptions.SvgImageSize = new(24, 24);
        shortcut2.ItemClick += shortcutClick;

        shortcut3.AllowGlyphSkinning = DefaultBoolean.False;
        shortcut3.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.None;
        shortcut3.ImageOptions.SvgImageSize = new(24, 24);
        shortcut3.ItemClick += shortcutClick;

        shortcut4.AllowGlyphSkinning = DefaultBoolean.False;
        shortcut4.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.None;
        shortcut4.ImageOptions.SvgImageSize = new(24, 24);
        shortcut4.ItemClick += shortcutClick;

        shortcut5.AllowGlyphSkinning = DefaultBoolean.False;
        shortcut5.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.None;
        shortcut5.ImageOptions.SvgImageSize = new(24, 24);
        shortcut5.ItemClick += shortcutClick;

        setupDetailShortcuts.ImageOptions.SvgImage = UI.GetImage(Symbol.Wrench);
        setupDetailShortcuts.ImageOptions.SvgImageSize = new(12, 12);

        menAdd.ImageOptions.SvgImage = UI.GetImage(Symbol.AddCircle);
        menAdd.DrawMenuSideStrip = DefaultBoolean.False;
        
        menSave.ImageOptions.SvgImage = UI.GetImage(Symbol.CheckmarkCircle);
        menDelete.ImageOptions.SvgImage = UI.GetImage(Symbol.DismissCircle);

        menMasterContext.ImageOptions.SvgImage = UI.GetImage(Symbol.MoreVertical);
        menMasterContext.DrawMenuSideStrip = DefaultBoolean.False;

        menAddDetail.ImageOptions.SvgImage = UI.GetImage(Symbol.AddCircle);
        menAddDetail.DrawMenuSideStrip = DefaultBoolean.False;

        menDetailContext.ImageOptions.SvgImage = UI.GetImage(Symbol.MoreVertical);
        menDetailContext.DrawMenuSideStrip = DefaultBoolean.False;

        menDetailMaxView.ImageOptions.SvgImage = UI.GetImage(Symbol.FullScreenMaximize);

        cmbPageSelect.ImageOptions.SvgImageSize = new(24, 24);

        split.ShowSplitGlyph = DefaultBoolean.True;

        menAddCopy.ItemClick += (_, args) => { invokeMasterCommand(_currentController?.GetCommand(eCommand.NewCopy), args); };
        menAddDefault.ItemClick += (_, args) => { invokeMasterCommand(_currentController?.GetCommand(eCommand.New), args); };
        menSave.ItemClick += (_, args) => { invokeMasterCommand(_currentController?.GetCommand(eCommand.Save), args); };
        menDelete.ItemClick += (_, args) => { invokeMasterCommand(_currentController?.GetCommand(eCommand.Delete), args); };

        menAddDetailCopy.ItemClick += (_, args) => { invokeDetailCommand(_currentDetailController?.GetCommand(eCommand.NewCopy), args); };
        menAddDetailDefault.ItemClick += (_, args) => { invokeDetailCommand(_currentDetailController?.GetCommand(eCommand.New), args); };
    }

    /// <summary>
    /// master editor
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IEditor? ViewEditor => _viewEditor;

    /// <summary>
    /// detail view
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IViewDetail? ViewDetail => panelDetail.Controls.Count > 0 ? panelDetail.Controls[0] as IViewDetail : null;

    /// <summary>
    /// Zugriff auf den BarManager
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public BarManager BarManager => barManager;

    /// <summary>
    /// Zugriff auf den Context-Menu des Details
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public BarSubItem ContextMenuDetail => menDetailContext;


    /// <summary>
    /// Zugriff auf den Context-Menu des Masters
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public BarSubItem ContextMenuMaster => menMasterContext;

    /// <summary>
    /// link for current display master model
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ModelLink? ModelLink => _modelLink;

    /// <summary>
    /// ID of the current model (master)
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Guid? ModelID => _modelLink?.ModelID;


    /// <summary>
    /// Aktuell im Editor angezeigter Master.
    /// </summary>
    public IModel? CurrentMaster => _viewEditor?.Model;


    /// <inheritdoc />
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool CanClose { get; set; } = true;


    /// <inheritdoc />
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string ViewContextMaster
    {
        get => _viewContextMaster;
        set
        {
            _viewContextMaster = value;
            UpdateMasterMenu();
        }
    }

    /// <inheritdoc />
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string ViewContextDetail
    {
        get => _viewContextDetail;
        set
        {
            _viewContextDetail = value;
            UpdateDetailMenu();
        }
    }

    /// <summary>
    /// Zugriff auf den aktuell in der Seite verwendeten Controller.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IControllerUI? CurrentController => _currentController;

    /// <summary>
    /// Zugriff auf den aktuell im Detailbereich der Seite verwendeten Controller.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IControllerUI? CurrentDetailController => _currentDetailController;

    /// <summary>
    /// close view
    /// </summary>
    /// <returns></returns>
    public bool Close()
    {
        return true;
    }
        
    /// <summary>
    /// Funktionstastenleiste anzeigen
    /// </summary>
    public void ShowFKeyBar()
    {
        if (_keyBar == null)
        {
            _keyBar = new(parentpage: this) { Size = new(100, ScaleDPI.ScaleVertical(80)), Visible = false, Dock = DockStyle.Bottom };
            Controls.Add(_keyBar);
        }
        _keyBar.Show();
        _keyBar.BringToFront();
    }

    /// <summary>
    /// Funktionstastenleiste verbergen
    /// </summary>
    public void HideFKeyBar()
    {
        _keyBar?.Hide();
    }


    /// <summary>
    /// Menu Master aktualisieren (z.B. nach Anpassung des ViewContextMaster)
    /// </summary>
    public void UpdateMasterMenu()
    {
        barManager.ClearCommandsInMenu(menMasterContext);
        barManager.ClearCommandsInMenu(menMasterPopup);

        if (_currentController == null) return;

        menMasterPopup.Caption = _currentController.TypeName.ToUpper();

        if (_currentController.TypeImage == null)
        {
            menMasterPopup.ImageOptions.SvgImage = null;
            menMasterPopup.ImageOptions.Image = null;
            menMasterPopup.ImageOptions.ImageIndex = -1;
        }
        else
        {
            if (_currentController.TypeImage is SvgImage svg)
                menMasterPopup.ImageOptions.SvgImage = svg;
            else if (_currentController.TypeImage is Image img)
                menMasterPopup.ImageOptions.Image = img;
            else if (_currentController.TypeImage is int idx)
                menMasterPopup.ImageOptions.ImageIndex = idx;
        }

        // ReSharper disable once CoVariantArrayConversion
        IMenuEntry[] commands = _currentController.GetCommands(eCommandContext.MasterContext, ViewContextMaster);

        bool hasImages = commands.FirstOrDefault(c => c.ImageIndex >= 0 || c.Image != null) != null;

        if (commands.Length > 0)
            barManager.BoundCommandsToMenu(menMasterContext, commands.Where(c => c.CommandType == eCommand.Other).ToArray(), invokeMasterCommand, showimages: hasImages, imgsize: 16);

        menMasterContext.Visibility = menMasterContext.ItemLinks?.Count < 1 ? BarItemVisibility.Never : BarItemVisibility.OnlyInRuntime;

        // ReSharper disable once CoVariantArrayConversion
        commands = _currentController.GetCommands(eCommandContext.MasterPopup, ViewContextMaster);

        hasImages = commands.FirstOrDefault(c => c.ImageIndex >= 0 || c.Image != null) != null;

        // MasterPopup-Menu erzeugen
        if (menMasterPopup.Visibility != BarItemVisibility.Never)
        {
            if (commands.Length > 0)
                barManager.BoundCommandsToMenu(menMasterPopup, commands.Where(c => c.CommandType == eCommand.Other).ToArray(), invokeMasterCommand, showimages: hasImages, imgsize: 16);
            else
                menMasterPopup.AllowDrawArrow = DefaultBoolean.False;
        }

        // default commands (new, save, delete)
        configCommand(menAddDefault, _currentController.GetCommand(eCommand.New, eCommandContext.MasterContext, true));
        configCommand(menAddCopy, _currentController.GetCommand(eCommand.NewCopy, eCommandContext.MasterContext, true));
        configCommand(menSave, _currentController.GetCommand(eCommand.Save, eCommandContext.MasterContext, true));
        configCommand(menDelete, _currentController.GetCommand(eCommand.Delete, eCommandContext.MasterContext, true));

        menAdd.Visibility = (menAddCopy.Enabled || menAddDefault.Enabled ? BarItemVisibility.OnlyInRuntime : BarItemVisibility.Never);
    }

    /// <summary>
    /// Menu Detail aktualisieren (z.B. nach Anpassung des ViewContextDetail)
    /// </summary>
    public void UpdateDetailMenu()
    {
        barManager.ClearCommandsInMenu(menDetailContext);
        barManager.ClearCommandsInMenu(menDetailViews);
    }

    /// <inheritdoc />
    public void UpdatePages(object? selectPage)
    {
        if (_viewEditor == null) return;

        List<ListItem> pages = [];

        foreach (var page in _viewEditor.GetPages())
            pages.Add(new() { Caption = page.Caption, ImageIndex = page.Image ?? (int)ObjectImages.piece, Value = page.PageIdentifier });

        repositoryItemPageSelect.Fill(pages);

        if (selectPage != null)
            cmbPageSelect.EditValue = selectPage;
        else
            cmbPageSelect.EditValue = pages[0].Value;

    }

    /// <summary>
    /// setup the page for the given controller
    /// </summary>
    /// <param name="controller">controller</param>
    /// <returns>the page</returns>
    public IViewPage Setup(IControllerUI controller)
    {
        if (_currentController == controller) return this;

        SuspendLayout();

        _currentDetailController = null;
        _currentController = controller;

        barManager.ClearCommandsInMenu(menDetailContext);
        barManager.ClearCommandsInMenu(menDetailViews);

        // reset current layout...
        detailViewPluginContainer.Controls.Clear(true);
        detailViewPluginContainer.Visible = false;

        Type? neededMaster = null;

        var typeDescriptions = _currentController.GetChildTypes();
        var hasChilds = typeDescriptions?.Any() ?? false;
        
        var detailViewMode = hasChilds
            ? _currentController.DetailViewMode
            : ePageDetailMode.NoDetails;

        switch (detailViewMode)
        {
            case ePageDetailMode.Default:
                showMasterAndDetail();
                neededMaster = _currentController.GetUIElementType(eUIElement.Editor);
                break;
            case ePageDetailMode.DetailsOnly:
                hideMaster();
                break;
            case ePageDetailMode.DetailsAsTab:
                hideDetails();
                break;
            case ePageDetailMode.NoDetails:
                hideDetails();
                neededMaster = _currentController.GetUIElementType(eUIElement.Editor);
                break;
        }

        _viewContextMaster = loadEditor(neededMaster)?.ViewContext ?? string.Empty;

        UpdateMasterMenu();

        if (hasChilds)
        {
            var details = typeDescriptions!.OfType<IMenuEntry>().ToArray();

            if (menDetailViews.Visibility == BarItemVisibility.Always)
                barManager.BoundCommandsToMenu(menDetailViews, details, invokeSelectChild, showimages: true, colormode: SvgImageColorizationMode.None);
        }

        // TODO: Setzen der Shortcuts für die Detailviews...
        _shortcuts = Functions.DeserializeJsonBytes<UIPersistModelPageShortcuts>(AFCore.App.Persistance?.Get(persistID, name: _currentController?.ControlledType.FullName!.Right(20) ?? "") ?? null, true);

        if (_shortcuts == null)
            _shortcuts = new();

        ResumeLayout();

        showShortcuts();

        return this;
    }


    void configCommand(BarButtonItem menItem, AFCommand? command)
    {
        menItem.Tag = command;

        if (command != null)
        {
            menItem.Enabled = true;
            menItem.Visibility = BarItemVisibility.OnlyInRuntime;
        }
        else
        {
            menItem.Enabled = false;
            menItem.Visibility = BarItemVisibility.Never;
        }
    }
    
    /// <summary>
    /// Ein Detail wurde ausgewählt
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void invokeSelectChild(object? sender, ItemClickEventArgs? e)
    {
        if (sender is not IMenuEntry menu) return;

        menDetailViews.Caption = menu.Caption.ToUpper();
        if (menu.Image is SvgImage svg)
            menDetailViews.ImageOptions.SvgImage = svg;
        else if (menu.Image is Image img)
            menDetailViews.ImageOptions.Image = img;

        if (sender is not TypeDescription tdesc) return;

        setupDetail(tdesc);

        // Detail-Daten laden
        if (_detailView != null && _currentController!.LoadDetails(_viewEditor!.Model, _detailView, tdesc)) return;

        _detailView!.Models =  (IBindingList)tdesc.Type.GetController().InvokeGeneric(nameof(IController.ReadDetailModels), [tdesc.Type], _currentController!.ControlledType, _viewEditor!.Model!.PrimaryKey, null, null)!;
    }

    private void setupDetail(TypeDescription tdesc)
    {
        var controller = tdesc.Type.GetUIController() ?? throw new NullReferenceException($"Für den als Detail anzuzeigenden Typen {tdesc.Type.FullName} existiert kein passender UI-Controller!");

        if (_currentDetailController != null && _currentDetailController.ControlledType != controller.ControlledType && _shortcuts?.Shortcuts.Count < 5)
        {
            // Shortcut anlegen...
            if (_currentDetailController.ControlledType.FullName != null)
            {
                if (_shortcuts.Shortcuts.FirstOrDefault(s => s.DetailType == _currentDetailController.ControlledType.FullName) == null)
                {
                    _shortcuts.Shortcuts.Insert(0, new() { DetailType = _currentDetailController.ControlledType.FullName });

                    AFCore.App.Persistance?.Set(persistID, _shortcuts.ToJsonBytes(), name: _currentController?.ControlledType.FullName!.Right(20) ?? "");

                    showShortcuts();
                }
            }
        }

        _currentDetailController = controller;

        var needdetail = _currentController == null 
            ? controller.GetUIElementType(eUIElement.Detail, detailtype: tdesc.Type ) 
            : controller.GetUIElementType(eUIElement.Detail, _currentController.ControlledType, detailtype: tdesc.Type);

        _detailView = (IViewDetail?)loadUIElement(eUIElement.Detail, needdetail, panelDetail, controller, tdesc.Type);
        
        var needplugin = _currentController == null
            ? controller.GetUIElementType(eUIElement.PluginDetail, detailtype: tdesc.Type)
            : controller.GetUIElementType(eUIElement.PluginDetail, _currentController.ControlledType, detailtype: tdesc.Type);

        var plugin = loadUIElement(eUIElement.PluginDetail, needplugin, detailViewPluginContainer, controller, tdesc.Type);

        if (_detailView != null)
            _detailView.PluginDetail = plugin as IViewModelDetail ?? null;
        
        barManager.ClearCommandsInMenu(menDetailContext);
        
        // ReSharper disable once CoVariantArrayConversion
        IMenuEntry[] commands = controller.GetCommands(eCommandContext.DetailContext, ViewContextDetail);

        if (commands.Length > 0)
            barManager.BoundCommandsToMenu(menDetailContext, commands.Where(c => c.CommandType == eCommand.Other).ToArray(), invokeDetailCommand);

        menDetailContext.Visibility = (menDetailContext.ItemLinks.Count > 0 ? BarItemVisibility.OnlyInRuntime : BarItemVisibility.Never);

        var addCommand = _currentDetailController.GetCommand(eCommand.New, eCommandContext.DetailContext, true);

        configCommand(menAddDetailDefault, addCommand);
        configCommand(menAddDetailCopy, _currentDetailController.GetCommand(eCommand.NewCopy, eCommandContext.DetailContext, true));

        menAddDetail.Caption = addCommand?.Caption.ToUpper() ?? "NEU";
        menAddDetail.Visibility = (menAddDetailCopy.Enabled || menAddDetailDefault.Enabled ? BarItemVisibility.OnlyInRuntime : BarItemVisibility.Never);
    }

    private void showShortcuts()
    {
        if (_shortcuts == null) return;

        if (_shortcuts.Shortcuts.Count < 1) return;

        int pos = 1;
        List<UIPersistModelPageShortcuts.PageShortcut> toremove = [];
        foreach (var shortcut in _shortcuts.Shortcuts)
        {
            var lnk = menDetailViews.ItemLinks.FirstOrDefault(i => i.Item.Tag is Tuple<object?, ItemClickEventHandler?> tuple && tuple.Item1 is TypeDescription tdesc && tdesc.Type.FullName == shortcut.DetailType);

            if (lnk == null)
            {
                toremove.Add(shortcut);
                continue;
            }

            if (pos == 1)
            {
                shortcut1.Visibility = BarItemVisibility.OnlyInRuntime;
                shortcut1.Tag = lnk.Item.Tag;
                shortcut1.ImageOptions.SvgImage = lnk.SvgImage;
                shortcut1.SuperTip = lnk.GetSuperTip();
            }

            if (pos == 2)
            {
                shortcut2.Visibility = BarItemVisibility.OnlyInRuntime;
                shortcut2.Tag = lnk.Item.Tag;
                shortcut2.ImageOptions.SvgImage = lnk.SvgImage;
                shortcut2.SuperTip = lnk.GetSuperTip();
            }

            if (pos == 3)
            {
                shortcut3.Visibility = BarItemVisibility.OnlyInRuntime;
                shortcut3.Tag = lnk.Item.Tag;
                shortcut3.ImageOptions.SvgImage = lnk.SvgImage;
                shortcut3.SuperTip = lnk.GetSuperTip();
            }

            if (pos == 4)
            {
                shortcut4.Visibility = BarItemVisibility.OnlyInRuntime;
                shortcut4.Tag = lnk.Item.Tag;
                shortcut4.ImageOptions.SvgImage = lnk.SvgImage;
                shortcut4.SuperTip = lnk.GetSuperTip();
            }

            if (pos == 5)
            {
                shortcut5.Visibility = BarItemVisibility.OnlyInRuntime;
                shortcut5.Tag = lnk.Item.Tag;
                shortcut5.ImageOptions.SvgImage = lnk.SvgImage;
                shortcut5.SuperTip = lnk.GetSuperTip();
            }

            ++pos;

            if (pos > 5)
                break;
        }

        foreach (var shortcut in toremove)
            _shortcuts.Shortcuts.Remove(shortcut);
    }

    private void shortcutClick(object sender, ItemClickEventArgs e)
    {
        if (e.Item.Tag is Tuple<object, ItemClickEventHandler> tuple)
            tuple.Item2.Invoke(tuple.Item1, e);
    }

    private IUIElement? loadUIElement(eUIElement elementType, Type? pluginType, Control container, IControllerUI controller, Type? detailtype)
    {
        if (pluginType == null)
        {
            if (container.Controls.Count > 0)
                container.Controls.Clear(true);

            container.Visible = false;

            return null;
        }

        IUIElement? plugin = null;

        if (container.Controls.Count > 0 && container.Controls[0].GetType() == pluginType)
            plugin = (IUIElement)container.Controls[0];
        else
            container.Controls.Clear(true);

        plugin ??= _currentController == null
                ? controller.GetUIElement(elementType, page: this, detailtype: detailtype)
                : controller.GetUIElement(elementType, mastertype: _currentController!.ControlledType, page: this, detailtype: detailtype);

        if (plugin == null)
        {
            container.Visible = false;
            return null;
        }

        controller.SetupUIElement(plugin, this, _currentController!.ControlledType, detailtype: detailtype);

        ((Control)plugin).Dock = DockStyle.Fill;
        container.Size = new(ScaleDPI.ScaleHorizontal(((Control)plugin).Width) , container.Height);
        container.Controls.Add((Control)plugin);
        container.Visible = true;

        return plugin;
    }

    void invokeMasterCommand(object? sender, ItemClickEventArgs e)
    {
        if (sender is not AFCommand command) return;

        InvokeCommand(command);
    }

    /// <summary>
    /// Command für die aktuelle Seite ausführen
    /// </summary>
    /// <param name="command"></param>
    public void InvokeCommand(AFCommand command)
    {
        CommandArgs data = new()
        {
            Page = this,
            Editor = ViewEditor,
            CommandContext = eCommandContext.MasterContext,
            CommandSource = ViewEditor,
            Model = ViewEditor?.Model
        };

        AFCore.App.HandleResult(command.Execute(data));
    }

    /// <summary>
    /// Command für die aktuelles Detail ausführen
    /// </summary>
    /// <param name="command"></param>
    public void InvokeCommandDetail(AFCommand command)
    {
        CommandArgs data = new()
        {
            Page = this,
            CommandContext = eCommandContext.DetailContext,
            CommandSource = ViewDetail,
            Model = ViewDetail?.CurrentModel,
            SelectedModels = ViewDetail?.SelectedModels?.ToArray()
        };

        AFCore.App.HandleResult(command.Execute(data));
    }


    void invokeDetailCommand(object? sender, ItemClickEventArgs e)
    {
        if (sender is not AFCommand command) return;

        InvokeCommandDetail(command);
    }

    /// <summary>
    /// Dialog für ein PropertyModel laden.
    /// </summary>
    /// <param name="model">Model/Einstellungen</param>
    /// <param name="caption">Überschrift</param>
    /// <param name="page">Seite, die den Dialog anzeigt</param>
    public IDialogContainer LoadPropertyDialog(IModel model, string caption, IViewPage? page = null)
    {
        AFOptionsDialog dlg = new(model, caption: caption, foldable: false, folded: false, parentpage: page);

        sideDialog.Padding = new(0);

        sideDialog.Size = new Size(dlg.Width, sideDialog.Size.Height);
        sideDialog.Controls.Add(dlg);
        sideDialog.Visible = true;
        
        return dlg;
    }

    /// <summary>
    /// SidebarDialog anzeigen
    /// </summary>
    /// <param name="editor">Dialog</param>
    /// <param name="caption">Überschrift</param>
    /// <param name="page">Seite</param>
    /// <returns>der Dialog</returns>
    public IDialogContainer ShowSidebarDialog(IDialogContainer editor, string caption, IViewPage? page = null)
    {
        sideDialog.Padding = new(0);

        sideDialog.Size = new Size(((Control)editor).Width, sideDialog.Size.Height);
        sideDialog.Controls.Add((Control)editor);
        sideDialog.Visible = true;

        return editor;
    }

    /// <summary>
    /// Dialog des PropertyPanels schließen.
    /// </summary>
    public void ClosePropertyDialog()
    {
        if (!sideDialog.Visible) return;

        sideDialog.Visible = false;

        sideDialog.SuspendDrawing();
        sideDialog.SuspendLayout();

        if (sideDialog.Controls.Count > 0)
            sideDialog.Controls.Clear(true);

        sideDialog.ResumeLayout();
        sideDialog.ResumeDrawing();
    }

    /// <summary>
    /// Dialog für ein PropertyModel laden, dass die Einstellungen repräsentiert.
    /// </summary>
    /// <param name="model">Model/Einstellungen</param>
    /// <param name="caption">Überschrift</param>
    /// <param name="page">Seite, die den Dialog anzeigt</param>
    /// <param name="folded">geschlossen darstellen</param>
    /// <param name="width">Breite des Dialogs (Standard: 300)</param>
    /// <param name="widthfolded">Breite wenn geschlossen (Standard: 30)</param>
    public IDialogContainer LoadSettingsDialog(IModel model, string caption, IViewPage? page = null, bool folded = true, int width = 300, int widthfolded = 30)
    {
        AFOptionsDialog dlg = new(model, caption: caption, foldable: true, folded: folded, parentpage: page, width: width, collapsedWidth: widthfolded);

        sideSettings.Padding = new(0);

        sideSettings.Size = new Size(dlg.Width, sideSettings.Size.Height);
        sideSettings.Controls.Add(dlg);
        sideSettings.Visible = true;

        ShowFKeyBar();

        return dlg;
    }

    /// <summary>
    /// Einstellungsdialog zusammenfalten (schmale Darstellung)
    /// </summary>
    public void FoldSettingsDialog()
    {
        if (!sideSettings.Visible || sideSettings.Controls.Count < 1 || sideSettings.Controls[0] is not AFOptionsDialog dialog) return;

        sideSettings.Size = new(dialog.Collapse(), sideSettings.Size.Height);
    }

    /// <summary>
    /// Einstellungsdialog auffalten (breite/geöffnete Darstellung)
    /// </summary>
    public void UnfoldSettingsDialog()
    {
        if (!sideSettings.Visible || sideSettings.Controls.Count < 1 || sideSettings.Controls[0] is not AFOptionsDialog dialog) return;

        sideSettings.Size = new(dialog.Expand(), sideSettings.Size.Height);
    }


    /// <summary>
    /// load a master view control if needed
    /// </summary>
    /// <param name="neededMaster">needed type of control</param>
    private IEditor? loadEditor(Type? neededMaster)
    {
        if (neededMaster == null) return null;

        if (panelMaster.Controls.Count > 0 && panelMaster.Controls[0].GetType() == neededMaster) return (IEditor)panelMaster.Controls[0];

        if (_dataevent != null)
            AFCore.App.EventHub.Unsubscribe(_dataevent);

        _dataevent = AFCore.App.EventHub.Subscribe(_currentController!.ControlledType, this, onDataEvent);

        if (panelMaster.Controls.Count > 0)
            panelMaster.Controls.Clear(true);

        _viewEditor = (IEditor)_currentController!.GetUIElement(eUIElement.Editor)!;
        _viewEditor.ParentPage = this;

        if (!_viewEditor.SupportPages && _viewEditor.HideCaption)
        {
            barDockMaster.Visible = false;
            panelMaster.Padding = new(0);
            split.Panel1.Padding = new(0);
        }

        if (_viewEditor.SupportPages)
        {
            lblSelectView.Visibility = BarItemVisibility.Always;
            cmbPageSelect.Visibility = BarItemVisibility.Always;

            repositoryItemPageSelect.SmallImages = Glyphs.GetObjectImages();
            ((SvgImageCollection)repositoryItemPageSelect.SmallImages).ImageSize = new(24, 24);

            List<ListItem> pages = [];

            foreach (var page in _viewEditor.GetPages())
                pages.Add(new() { Caption = page.Caption, ImageIndex = page.Image ?? (int)ObjectImages.piece, Value = page.PageIdentifier });

            repositoryItemPageSelect.Fill(pages);

            cmbPageSelect.EditValue = pages[0].Value;

            cmbPageSelect.EditValueChanged += (_, _) =>
            {
                if (cmbPageSelect.EditValue != null && cmbPageSelect.EditValue is int pageindex)
                    _viewEditor.SelectPage(pageindex);
            };
        }

        ((Control)_viewEditor).Dock = DockStyle.Fill; // split.PanelVisibility == SplitPanelVisibility.Panel1 ? DockStyle.Fill : DockStyle.Top;
        panelMaster.Controls.Add((Control)_viewEditor);

        if (split.PanelVisibility == SplitPanelVisibility.Both)
            split.SplitterPosition = _viewEditor.DefaultEditorHeight + barDockMaster.Height + split.Panel1.Padding.Vertical + 5;

        var tdesc = _currentController.ControlledType.GetTypeDescription();

        // set caption and symbol for the master view
        menMasterPopup.Caption = (tdesc.Context != null ? tdesc.Context.NameSingular : tdesc.Name).ToUpper();

        switch (_currentController.TypeImage)
        {
            case SvgImage svg:
                menMasterPopup.ImageOptions.SvgImage = svg;
                break;
            case Image img:
                menMasterPopup.ImageOptions.Image = img;
                break;
            default:
                menMasterPopup.ImageOptions.SvgImage = UI.GetObjectImage(ObjectImages.data);
                break;
        }

        return _viewEditor;
    }


    private void onDataEvent(object? data, eHubEventType eventtype, int messageid)
    {
        if (_currentController == null) return;

        if (data is not IModel model) return;

        if (CurrentMaster == null) return;

        if (data.GetType() != _currentController.ControlledType) return;

        if (model.PrimaryKey != CurrentMaster.PrimaryKey) return;

        if (_modelLink != null && (_modelLink.ModelID != model.PrimaryKey || _modelLink.ModelCaption != model.ModelLink.ModelCaption))
            _modelLink = model.ModelLink;

        if (eventtype == eHubEventType.ObjectDeleted)
        {
            MsgBox.ShowInfoOk("DATENSATZ GELÖSCHT\r\nDer Datensatz wurde gelöscht. Die Seite wird nun geschlossen.");

            UI.ViewManager.ClosePage(this);
        }

        if (_modelLink != null) UI.ViewManager.UpdatePage(this, _modelLink);
    }

    /// <summary>
    /// adjust view to show master and details area
    /// </summary>
    private void showMasterAndDetail()
    {
        if (split.PanelVisibility == SplitPanelVisibility.Both && lblSelectView.Visibility == BarItemVisibility.Never)
            return;

        split.PanelVisibility = SplitPanelVisibility.Both;

        barDockMaster.Visible = true;
        barMainView.Visible = true;

        lblSelectView.Visibility = BarItemVisibility.Never;
        cmbPageSelect.Visibility = BarItemVisibility.Never;
        
        barDockDetail.Visible = true;
        barDetailView.Visible = true;

        menDetailContext.Visibility = BarItemVisibility.Always;
        menDetailViews.Visibility = BarItemVisibility.Always;

        menDetailMaxView.Visibility = BarItemVisibility.Always;
    }


    /// <summary>
    /// adjust view to show only master area
    /// </summary>
    private void hideDetails()
    {
        if (split.PanelVisibility == SplitPanelVisibility.Panel1)
            return;

        split.PanelVisibility = SplitPanelVisibility.Panel1;

        barDockDetail.Visible = false;
        barDetailView.Visible = false;

        menDetailContext.Visibility = BarItemVisibility.Never;
        menDetailViews.Visibility = BarItemVisibility.Never;

        if (_currentController!.DetailViewMode == ePageDetailMode.NoDetails)
        {
            lblSelectView.Visibility = BarItemVisibility.Never;
            cmbPageSelect.Visibility = BarItemVisibility.Never;
        }
        else // DetailsAsTab
        {
            lblSelectView.Visibility = BarItemVisibility.Always;
            cmbPageSelect.Visibility = BarItemVisibility.Always;
        }
    }

    /// <summary>
    /// adjust view to show only details area
    /// </summary>
    private void hideMaster()
    {
        if (split.PanelVisibility == SplitPanelVisibility.Panel2)
            return;

        split.PanelVisibility = SplitPanelVisibility.Panel2;

        lblSelectView.Visibility = BarItemVisibility.Never;
        cmbPageSelect.Visibility = BarItemVisibility.Never;

        barDockMaster.Visible = false;
        barMainView.Visible = false;

        menMasterContext.Visibility = BarItemVisibility.Never;
        menMasterPopup.Visibility = BarItemVisibility.Never;

        menAdd.Visibility = BarItemVisibility.Never;
        menAddCopy.Visibility = BarItemVisibility.Never;
        menAddDefault.Visibility = BarItemVisibility.Never;
        menSave.Visibility = BarItemVisibility.Never;
        menDelete.Visibility = BarItemVisibility.Never;

        barDockDetail.Visible = true;
        barDetailView.Visible = true;

        menDetailViews.Visibility = BarItemVisibility.Always;
        menDetailContext.Visibility = BarItemVisibility.Always;

        menDetailMaxView.Visibility = BarItemVisibility.Never;
    }

    /// <summary>
    /// load view for the model described by the given link
    /// </summary>
    /// <param name="link">link which describe the model</param>
    public IViewPage LoadModel(ModelLink link)
    {
        if (ModelID.Equals(link.ModelID)) // straight return if currently displayed model is the same
            return this;

        if (_currentController == null || _currentController.ControlledType != link.ModelType)
            setupForModel(link.ModelType);

        load(link);

        return this;
    }


    /// <summary>
    /// load view for the model 
    /// </summary>
    /// <param name="model">model to display in the page</param>
    public IViewPage LoadModel(IModel model)
    {
        if (ModelID.Equals(model.PrimaryKey)) // straight return if currently displayed model is the same
            return this;

        // kein Controller vorhanden oder falscher Controller > erfordert eine neues setup der Seite
        if (_currentController == null || _currentController != model.GetType().GetController())
            setupForModel(model.GetType());

        load(model.ModelLink);

        return this;
    }


    /// <summary>
    /// load view for the model described by the given link
    /// </summary>
    /// <param name="modelID">id of the model</param>
    /// <param name="modelType">type of the model</param>
    public IViewPage LoadModel(Type modelType, Guid modelID)
    {
        if (ModelID.Equals(modelID)) // straight return if currently displayed model is the same
            return this;

        // kein Controller vorhanden oder falscher Controller > erfordert eine neues setup der Seite
        if (_currentController == null || _currentController != modelType.GetController())
            setupForModel(modelType);

        IModel model = _currentController!.InvokeGeneric("Load", [modelType], modelID) as IModel
                       ?? throw new Exception(string.Format(WinFormsStrings.ERR_NOMODELWITHID,
                           modelType.Name, modelID));

        load(model.ModelLink);

        return this;

    }

    private void setupForModel(Type modelType)
    {
        if (modelType.GetController() is IControllerUI uiController)
            Setup(uiController);
        else
            throw new Exception(@$"No UI Controller Available for type {modelType.FullName}.");
    }

    /// <summary>
    /// load a page
    /// </summary>
    /// <param name="link"></param>
    private void load(ModelLink link)
    {
        _modelLink = link;

        if (_viewEditor != null)
            _viewEditor.Model = link.Model;

        Type? detailtype = link.CurrentDetailType ?? _currentController?.GetPrimaryChildType();

        if (detailtype != null)
            invokeSelectChild(detailtype.GetTypeDescription(), null);
    }

    /// <summary>
    /// Validate the current Page (Editor AND DetailView if available) and return a list of errors if invalid.
    /// </summary>
    /// <param name="errors">Liste der Fehler</param>
    /// <returns>true wenn valide, sonst false</returns>
    public bool IsValid( ValidationErrorCollection errors)
    {
        bool ret = _viewEditor?.IsValid(errors) ?? true;

        if (_detailView != null && !_detailView.IsValid(errors))
            ret = false;

        return ret;
    }

    /// <summary>
    /// Create a new page for the given controller
    /// </summary>
    /// <param name="controller">the controller for the new page</param>
    /// <returns>the new page</returns>
    internal static IViewPage Create(IControllerUI controller)
    {
        var page = new AFPage();
        page.Setup(controller);
        return page;
    }

    /// <summary>
    /// Progress-Overlay in Seite anzeigen
    /// </summary>
    /// <param name="caption">Überschrift</param>
    /// <param name="description">Beschreibung</param>
    /// <param name="total">max. Anzahl Schritte</param>
    /// <param name="current">akt. Schritt</param>
    /// <param name="allowcancel">abbrechen erlauben</param>
    /// <returns></returns>
    public IViewPageProgress ShowProgress(string caption, string description, int total, int current, bool allowcancel)
    {
        var progress = new AFPageProgress(this);
        progress.Size = new(Width, progress.Height);
        progress.Location = new(0, (Height - progress.Height) / 2);

        Controls.Add(progress);
        progress.BringToFront();

        return progress;
    }

    
    /// <summary>
    /// Progress-Overlay schließen
    /// </summary>
    /// <param name="progress">zu schließende Overlay</param>
    public void CloseProgress(IViewPageProgress progress)
    {
        if (progress is not Control progressControl) return;

        progressControl.Hide();
        Controls.Remove(progressControl);

        progressControl.Dispose();
    }

    /// <inheritdoc />
    protected override void OnHandleDestroyed(EventArgs e)
    {
        if (_dataevent != null) AFCore.App.EventHub.Unsubscribe(_dataevent);

        base.OnHandleDestroyed(e);
    }

    private void maxminDetail(object sender, ItemClickEventArgs e)
    {
        if (split.PanelVisibility == SplitPanelVisibility.Both)
        {
            split.PanelVisibility = SplitPanelVisibility.Panel2;
            menDetailMaxView.ImageOptions.SvgImage = UI.GetImage(Symbol.FullScreenMinimize);
        }
        else
        {
            split.PanelVisibility = SplitPanelVisibility.Both;
            menDetailMaxView.ImageOptions.SvgImage = UI.GetImage(Symbol.FullScreenMaximize);
        }
    }
}

/// <summary>
/// Model um UI-Einstellungen zu persistieren
/// </summary>
[Serializable]
public abstract class UIPersistModel
{

}

/// <summary>
/// Model um die Shortcuts in AFPage (Detailansichten) zu persistieren.
/// </summary>
[Serializable]
public class UIPersistModelPageShortcuts : UIPersistModel
{


    /// <summary>
    /// Liste der ShortCuts
    /// </summary>
    public List<PageShortcut> Shortcuts { get; set; } = [];

    /// <summary>
    /// Shortcut auf einen Typ in der Detailansicht
    /// </summary>
    public class PageShortcut
    {
        /// <summary>
        /// Name des Typs in der Detailansicht, auf den verwiesen wird.
        /// </summary>
        public string DetailType { get; set; } = string.Empty;
    }
}


/// <summary>
/// Model um Fenstereinstellungen (Größe und Position) zu persistieren.
/// </summary>
[Serializable]
public class UIPersistModelForm : UIPersistModel
{

}