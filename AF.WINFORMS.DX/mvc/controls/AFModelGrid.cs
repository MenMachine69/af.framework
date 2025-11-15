using AF.MVC;
using DevExpress.Utils.Layout;
using DevExpress.Utils.Menu;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Tile;
using DevExpress.XtraBars;

namespace AF.WINFORMS.DX;

/// <summary>
/// Universell verwendbares Grid zur Darstellung von IModel-Objekten.
/// </summary>
[DesignerCategory("Code")]
public class AFModelGrid : AFUserControl
{
    private readonly AFLabel lblDescription = null!;
    private readonly AFButtonDropDown pshGridView = null!;
    private readonly AFButtonDropDown pshTileView = null!;
    private readonly AFButtonDropDown pshKanbanView = null!;
    private readonly AFGridExtender gridextender = null!;
    private readonly AFGridControl grid = null!;
    private GridView? gridView;
    private AdvBandedGridView? bandedgridView;
    private TileView? tileView;
    private TileView? kanbanView;

    private readonly DXPopupMenu popupGridViews = null!; // Optionsmenü für GridViews
    private readonly DXPopupMenu popupTileViews = null!; // Optionsmenü für TileViews
    private readonly DXPopupMenu popupKanbanViews = null!; // Optionsmenü für KanbanViews

    private AFGridExtender? extender;

    private WeakEvent<EventHandler<EventArgs>>? _rowSelected;
    private WeakEvent<EventHandler<EventArgs>>? _selectionChanged;
    private WeakEvent<EventHandler<EventArgs>>? _viewChanged;
    private WeakEvent<EventHandler<ModelEventArgs>>? _modelChanged;

    private EventToken? dbEventToken;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="plugin"></param>
    public AFModelGrid(Control? plugin = null)
    {
        if (UI.DesignMode) return;

        // Controls vorbereiten
        // Beschreibung im Titel
        lblDescription = new() { Dock = DockStyle.Fill };
        
        // Schaltfläche GridView
        pshGridView = new() { ActAsDropDown = true, DropDownArrowStyle = DropDownArrowStyle.SplitButton, PaintStyle = PaintStyles.Light, AutoSize = true, AllowFocus = false, ShowFocusRectangle = DefaultBoolean.False };
        pshGridView.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.Default;
        pshGridView.ImageOptions.SvgImageSize = new(16, 16);
        pshGridView.ImageOptions.SvgImage = UI.GetImage(Symbol.ColumnTriple);
        pshGridView.Click += (_, _) =>
        {
            if (gridView != null && grid.MainView != gridView)
            {
                grid.MainView = gridView;
                _viewChanged?.Raise(this, EventArgs.Empty);
                return;
            }

            if (bandedgridView != null && grid.MainView != bandedgridView)
            {
                grid.MainView = bandedgridView;
                _viewChanged?.Raise(this, EventArgs.Empty);
            }
        };

        // Popup-Menu der Schaltfläche GridView
        popupGridViews = new();

        pshGridView.DropDownControl = popupGridViews;

        // Schaltfläche TileView
        pshTileView = new() { ActAsDropDown = true, DropDownArrowStyle = DropDownArrowStyle.SplitButton, PaintStyle = PaintStyles.Light, AutoSize = true, AllowFocus = false, ShowFocusRectangle = DefaultBoolean.False };
        pshTileView.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.Default;
        pshTileView.ImageOptions.SvgImageSize = new(16, 16);
        pshTileView.ImageOptions.SvgImage = UI.GetImage(Symbol.RowTriple);
        pshTileView.Click += (_, _) =>
        {
            if (tileView != null && grid.MainView != tileView)
            {
                string? filter = null;

                if (grid.MainView == gridView)
                    filter = gridView.ActiveFilterString;
                else if (grid.MainView == bandedgridView)
                    filter = bandedgridView.ActiveFilterString;

                grid.MainView = tileView;
                
                if (filter != null)
                    tileView.ActiveFilterString = filter;

                _viewChanged?.Raise(this, EventArgs.Empty);
            }
        };

        // Popup-Menu der Schaltfläche TileView
        popupTileViews = new();

        pshTileView.DropDownControl = popupTileViews;

        pshKanbanView = new() { ActAsDropDown = true, DropDownArrowStyle = DropDownArrowStyle.SplitButton, PaintStyle = PaintStyles.Light, AutoSize = true, AllowFocus = false, ShowFocusRectangle = DefaultBoolean.False };
        pshKanbanView.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.Default;
        pshKanbanView.ImageOptions.SvgImageSize = new(16, 16);
        pshKanbanView.ImageOptions.SvgImage = UI.GetImage(Symbol.AlignTop);
        pshKanbanView.Click += (_, _) =>
        {
            if (kanbanView != null && grid.MainView != kanbanView)
            {
                string? filter = null;

                if (grid.MainView == gridView)
                    filter = gridView.ActiveFilterString;
                else if (grid.MainView == bandedgridView)
                    filter = bandedgridView.ActiveFilterString;
                else if (grid.MainView == tileView)
                    filter = tileView.ActiveFilterString;

                grid.MainView = kanbanView;

                if (filter != null)
                    kanbanView.ActiveFilterString = filter;

                _viewChanged?.Raise(this, EventArgs.Empty);
            }
        };

        // Popup-Menu der Schaltfläche KanbanView
        popupKanbanViews = new();

        pshKanbanView.DropDownControl = popupKanbanViews;


        // GridControl
        grid = new() { Dock = DockStyle.Fill };
        
        AFTablePanel table = new() { UseSkinIndents = false, Dock = DockStyle.Fill };
        
        Controls.Add(table);

        table.BeginLayout();

        table.Add(plugin ?? lblDescription, 1, 1);
        table.Add(pshKanbanView, 1, 2);
        table.Add(pshTileView, 1, 3);
        table.Add(pshGridView, 1, 4);
        table.Add(grid, 2, 1, colspan: 4);

        table.SetColumn(1, TablePanelEntityStyle.Relative, 1.0f);
        table.SetRow(2, TablePanelEntityStyle.Relative, 1.0f);

        table.EndLayout();

        gridextender = new();
        gridextender.Grid = grid;
    }

    /// <inheritdoc />
    protected override void OnHandleDestroyed(EventArgs e)
    {
        if (dbEventToken != null) AFCore.App.EventHub.Unsubscribe(dbEventToken);

        ProcessModelChange = null;

        base.OnHandleDestroyed(e);
    }

    private void onclick(AFPopupMenuEntry entry)
    {
        var result = entry.ClickAction.Invoke(new() { CommandSource = grid.MainView, Tag = entry });

        if (result.Result != eNotificationType.None)
            (Grid.FindForm() as ICommandResultDisplay)?.HandleResult(result);
    }

    /// <summary>
    /// Func, die ausgeführt wird, wenn das Grid vom EventHub über Änderungen benachrichtigt wurde.
    ///
    /// Übergeben werden an die Func das Grid, das Model, dass das Ereignis auslöste und der Typ des Ereignisses. Als Rückgabe wird das Model erwartet,
    /// mit dem das Grid weiterarbeiten soll (hinzufügen, aktualisieren oder löschen) oder NULL, wenn das Grid die Änderung
    /// ignorieren soll.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Func<AFModelGrid, IModel, eHubEventType, IModel?>? ProcessModelChange { get; set; }

    /// <summary>
    /// Ereignis, das ausgelöst wird, wenn auf eine andere Ansicht umgeschaltet wurde.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public event EventHandler<EventArgs> ViewChanged
    {
        add
        {
            _viewChanged ??= new();
            _viewChanged.Add(value);
        }
        remove => _viewChanged?.Remove(value);
    }

    /// <summary>
    /// Ereignis, das ausgelöst wird, der EventHub eine Datenänderung gemeldet hat und ProcessModelChanged == null ist.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public event EventHandler<ModelEventArgs> ModelChanged
    {
        add
        {
            _modelChanged ??= new();
            _modelChanged.Add(value);
        }
        remove => _modelChanged?.Remove(value);
    }

    /// <summary>
    /// Ereignis, das ausgelöst wird, wenn eine die Auswahl der Zeilen verändert wurde.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public event EventHandler<EventArgs> RowSelected
    {
        add
        {
            _selectionChanged ??= new();
            _selectionChanged.Add(value);
        }
        remove => _selectionChanged?.Remove(value);
    }

    /// <summary>
    /// Ereignis, das ausgelöst wird, wenn eine Zeile fokussiert wurde.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public event EventHandler<EventArgs> SelectionChanged
    {
        add
        {
            _rowSelected ??= new();
            _rowSelected.Add(value);
        }
        remove => _rowSelected?.Remove(value);
    }

    /// <summary>
    /// Setup des Grids 
    /// </summary>
    /// <param name="setup">Einstellungen für das Grid</param>
    public void Setup(AFGridSetup setup)
    {
        if (dbEventToken != null) AFCore.App.EventHub.Unsubscribe(dbEventToken);

        dbEventToken = null;

        MultiSelect = setup.AllowMultiSelect;

        grid.BeginInit();

        grid.ViewCollection.Clear();

        if (setup.SupportedGridStyle.HasFlag(eGridMode.KanbanView))
        {
            if (kanbanView == null)
            {
                kanbanView = new();
                kanbanView.GridControl = grid;
                kanbanView.SetHtmlMode();
                kanbanView.FocusedRowObjectChanged += (_, _) =>
                {
                    _rowSelected?.Raise(this, EventArgs.Empty);
                };
                kanbanView.SelectionChanged += (_, _) =>
                {
                    _selectionChanged?.Raise(this, EventArgs.Empty);
                };


                grid.ViewCollection.Add(kanbanView);
            }

            kanbanView.Setup(setup, true);

            kanbanView.OptionsSelection.MultiSelect = MultiSelect;

            if (kanbanView.GetGridIdentifier() is not { } id || id == Guid.Empty) return;

            var data = AFCore.App.Persistance?.Get(id, extName: "kvw");

            if (data != null)
                kanbanView.RestoreLayout(data);
        }
        else
        {
            if (kanbanView != null)
            {
                grid.ViewCollection.Remove(kanbanView);
                kanbanView.Dispose();
                kanbanView = null;
            }
        }

        if (setup.SupportedGridStyle.HasFlag(eGridMode.TileView))
        {
            if (tileView == null)
            {
                tileView = new();
                tileView.GridControl = grid;
                tileView.SetHtmlMode();
                tileView.FocusedRowObjectChanged += (_, _) =>
                {
                    _rowSelected?.Raise(this, EventArgs.Empty);
                };
                tileView.SelectionChanged += (_, _) =>
                {
                    _selectionChanged?.Raise(this, EventArgs.Empty);
                };


                grid.ViewCollection.Add(tileView);


            }

            tileView.Setup(setup);

            tileView.OptionsSelection.MultiSelect = MultiSelect;
            
            if (tileView.GetGridIdentifier() is not { } id || id == Guid.Empty) return;

            var data = AFCore.App.Persistance?.Get(id, extName: "tvw");
            
            if (data != null)
                tileView.RestoreLayout(data);
        }
        else
        {
            if (tileView != null)
            {
                grid.ViewCollection.Remove(tileView);
                tileView.Dispose();
                tileView = null;
            }
        }

        if (setup.SupportedGridStyle.HasFlag(eGridMode.GridView))
        {
            if (gridView == null)
            {
                gridView = new();
                gridView.GridControl = grid;
                gridView.FocusedRowObjectChanged += (_, _) =>
                {
                    _rowSelected?.Raise(this, EventArgs.Empty);
                };
                gridView.SelectionChanged += (_, _) =>
                {
                    _selectionChanged?.Raise(this, EventArgs.Empty);
                };

                if (PopupMenuRows != null)
                {
                    gridView.PopupMenuShowing += (s, e) =>
                    {
                        if (e.HitInfo.InRow)
                            ((GridView)s).FocusedRowHandle = e.HitInfo.RowHandle;

                        PopupMenuRows.ShowPopup(PointToScreen(e.Point));
                    };
                }

                grid.ViewCollection.Add(gridView);
            }

            gridView.Setup(setup);

            gridView.OptionsSelection.MultiSelect = MultiSelect;
            gridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;

            gridView.LoadLayout(extName: "gvw");


        }
        else
        {
            if (gridView != null)
            {
                grid.ViewCollection.Remove(gridView);
                gridView.Dispose();
                gridView = null;
            }
        }

        if (setup.SupportedGridStyle.HasFlag(eGridMode.AdvBandedGridView) || setup.SupportedGridStyle.HasFlag(eGridMode.BandedGridView))
        {
            if (bandedgridView == null)
            {
                bandedgridView = new();
                bandedgridView.GridControl = grid;
                bandedgridView.FocusedRowObjectChanged += (_, _) =>
                {
                    _rowSelected?.Raise(this, EventArgs.Empty);
                };
                bandedgridView.SelectionChanged += (_, _) =>
                {
                    _selectionChanged?.Raise(this, EventArgs.Empty);
                };
                bandedgridView.EndGrouping += (s, _) =>
                {
                    if (s is not BandedGridView view) return;

                    foreach (BandedGridColumn Column in view.GroupedColumns)
                    {
                        Column.OwnerBand = null;
                    }
                };
                if (PopupMenuRows != null)
                {
                    bandedgridView.PopupMenuShowing += (s, e) =>
                    {
                        if (e.HitInfo.InRow)
                            ((GridView)s).FocusedRowHandle = e.HitInfo.RowHandle;

                        PopupMenuRows.ShowPopup(PointToScreen(e.Point));
                    };
                }

                grid.ViewCollection.Add(bandedgridView);
            }

            bandedgridView.Setup(setup);

            bandedgridView.OptionsSelection.MultiSelect = MultiSelect;
            bandedgridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;


            bandedgridView.LoadLayout(extName: "bvw");
        }
        else
        {
            if (bandedgridView != null)
            {
                grid.ViewCollection.Remove(bandedgridView);
                bandedgridView.Dispose();
                bandedgridView = null;
            }
        }

        if (setup.DefaultGridStyle == eGridMode.KanbanView)
            grid.MainView = kanbanView;
        else if (setup.DefaultGridStyle == eGridMode.TileView)
            grid.MainView = tileView;
        else if (setup.DefaultGridStyle == eGridMode.GridView)
            grid.MainView = gridView;
        else if (setup.DefaultGridStyle == eGridMode.AdvBandedGridView || setup.DefaultGridStyle == eGridMode.BandedGridView)
            grid.MainView = bandedgridView;


        lblDescription.Text = setup.GridDescription;

        pshKanbanView.Visible = setup.SupportedGridStyle.HasFlag(eGridMode.TileView);
        pshTileView.Visible = setup.SupportedGridStyle.HasFlag(eGridMode.TileView);
        pshGridView.Visible = setup.SupportedGridStyle.HasFlag(eGridMode.GridView) || setup.SupportedGridStyle.HasFlag(eGridMode.AdvBandedGridView) || setup.SupportedGridStyle.HasFlag(eGridMode.BandedGridView);

       

        if (pshKanbanView.Visible && kanbanView != null)
        {
            popupKanbanViews.Items.Clear();

            List<AFPopupMenuEntry> entries = [];

            var dict = kanbanView.GetAvailableLayouts(extName: "kvw");

            entries.Add(new("Standardlayout", loadKvLayout) { Tag = "" });

            foreach (var entry in dict)
                entries.Add(new("Layout: " + entry.Value, loadKvLayout) { Tag = entry.Value });

            entries.Add(new("LAYOUT\\LAYOUT BEARBEITEN", editKanbanHtmlTemplates)
            {
                BeginGroup = true,
                Image = UI.GetImage(Symbol.Edit),
                Description = "HTML- und CSS- Vorlage zur Darstellung der Zeilen in der Liste bearbeiten."
            });
            entries.Add(new("LAYOUT\\LAYOUT SPEICHERN", saveKvLayout) { Image = UI.GetImage(Symbol.Save) });
            entries.Add(new("LAYOUT\\LAYOUT SPEICHERN ALS", saveKvLayoutAs) { Image = UI.GetImage(Symbol.SaveEdit) });
            entries.Add(new("LAYOUT\\LAYOUTS VERWALTEN", manageKvLayouts) { Image = UI.GetImage(Symbol.Wrench) });
            entries.Add(new("LAYOUT\\LAYOUT ZURÜCKSETZEN", resetKvLayout) { BeginGroup = true, Image = UI.GetImage(Symbol.ArrowReset) });

            if (AFCore.App.SecurityService != null && (AFCore.App.SecurityService.CurrentUser?.IsAdmin ?? false))
            {
                entries.Add(new("ADMIN\\STANDARDLAYOUT SPEICHERN", saveKvDefaultLayout) { BeginGroup = true });
                entries.Add(new("ADMIN\\STANDARDLAYOUT ZURÜCKSETZEN", resetKvDefaultLayout));
            }

            foreach (var entry in entries)
            {
                var item = popupKanbanViews.AddMenuEntry(entry);
                if (item != null) item.Click += (_, _) => { onclick(entry); };
            }
        }

        if (pshTileView.Visible && tileView != null)
        {
            popupTileViews.Items.Clear();

            List<AFPopupMenuEntry> entries = [];

            var dict = tileView.GetAvailableLayouts(extName: "tvw");

            entries.Add(new("Standardlayout", loadTvLayout) { Tag = "" });
            
            foreach (var entry in dict)
                entries.Add(new("Layout: " + entry.Value, loadTvLayout) { Tag = entry.Value });
            
            entries.Add(new("LAYOUT\\LAYOUT BEARBEITEN", editHtmlTemplates)
            {
                BeginGroup = true, Image = UI.GetImage(Symbol.Edit),
                Description = "HTML- und CSS- Vorlage zur Darstellung der Zeilen in der Liste bearbeiten."
            });
            entries.Add(new("LAYOUT\\LAYOUT SPEICHERN", saveTvLayout) { Image = UI.GetImage(Symbol.Save) });
            entries.Add(new("LAYOUT\\LAYOUT SPEICHERN ALS", saveTvLayoutAs) { Image = UI.GetImage(Symbol.SaveEdit) });
            entries.Add(new("LAYOUT\\LAYOUTS VERWALTEN", manageTvLayouts) { Image = UI.GetImage(Symbol.Wrench) });
            entries.Add(new("LAYOUT\\LAYOUT ZURÜCKSETZEN", resetTvLayout) { BeginGroup = true, Image = UI.GetImage(Symbol.ArrowReset) });

            if (AFCore.App.SecurityService != null && (AFCore.App.SecurityService.CurrentUser?.IsAdmin ?? false))
            {
                entries.Add(new("ADMIN\\STANDARDLAYOUT SPEICHERN", saveTvDefaultLayout) { BeginGroup = true });
                entries.Add(new("ADMIN\\STANDARDLAYOUT ZURÜCKSETZEN", resetTvDefaultLayout));
            }
            
            foreach (var entry in entries)
            {
                var item = popupTileViews.AddMenuEntry(entry);
                if (item != null) item.Click += (_, _) => { onclick(entry); };
            }
        }

        if (pshGridView.Visible)
        {
            popupGridViews.Items.Clear();

            List<AFPopupMenuEntry> entries = [];
            Dictionary<Guid, string> dict = [];

            if (gridView != null)
                dict = gridView.GetAvailableLayouts(extName: "gvw");
            else if (bandedgridView != null)
                dict = bandedgridView.GetAvailableLayouts(extName: "bvw");

            entries.Add(new("Standardlayout", loadGvLayout) { Tag = "" });

            foreach (var entry in dict)
                entries.Add(new("Layout: "+entry.Value, loadGvLayout) { Tag = entry.Value });
        
            entries.Add(new("LAYOUT\\LAYOUT SPEICHERN", saveGvLayout) { BeginGroup = true, Image = UI.GetImage(Symbol.Save) });
            entries.Add(new("LAYOUT\\LAYOUT SPEICHERN ALS", saveGvLayoutAs) { Image = UI.GetImage(Symbol.SaveEdit) });
            entries.Add(new("LAYOUT\\LAYOUTS VERWALTEN", manageGvLayouts) { Image = UI.GetImage(Symbol.Wrench) });
            entries.Add(new("LAYOUT\\LAYOUT ZURÜCKSETZEN", resetGvLayout) { BeginGroup = true, Image = UI.GetImage(Symbol.ArrowReset) });


            if (AFCore.App.SecurityService != null && (AFCore.App.SecurityService.CurrentUser?.IsAdmin ?? false))
            {
                entries.Add(new("ADMIN\\STANDARDLAYOUT SPEICHERN", saveGvDefaultLayout) { BeginGroup = true });
                entries.Add(new("ADMIN\\STANDARDLAYOUT ZURÜCKSETZEN", resetGvDefaultLayout));
            }

            foreach (var entry in entries)
            {
                var item = popupGridViews.AddMenuEntry(entry);
                if (item != null) item.Click += (_, _) => { onclick(entry); };
            }
        }

        grid.EndInit();

        extender ??= new();
        extender.Grid = grid;

        if (setup.ModelType != null)
            dbEventToken = AFCore.App.EventHub.Subscribe(setup.ModelType, this, onModelChanged);
    }

    private void onModelChanged(object data, eHubEventType eventType, int messageCode)
    {
        if (data is not IModel model) return;

        if (ProcessModelChange == null)
        {
            _modelChanged?.Raise(this, new ModelEventArgs() { Data = model });
            return;
        }

        var outModel = ProcessModelChange(this, model, eventType);

        switch (eventType)
        {
            case eHubEventType.ObjectAdded:
                //onModelAdded(model);
                if (outModel == null) return;
                Models?.Add(outModel);
                break;
            case eHubEventType.ObjectChanged:
                if (outModel == null) return;

                var vorhanden = Models?.OfType<IModel>().FirstOrDefault(m => m.PrimaryKey == outModel.PrimaryKey);
                if (vorhanden == null)
                    Models?.Add(outModel);
                else
                    vorhanden.CopyFrom(outModel, true);
                break;
            case eHubEventType.ObjectDeleted:
                var loeschen = Models?.OfType<IModel>().FirstOrDefault(m => m.PrimaryKey == model.PrimaryKey);
                if (loeschen != null)
                    Models?.Remove(loeschen);
                break;
        }
    }

    /// <summary>
    /// Aktuell im Grid dargestellte Models (Datasource des Grids)
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IBindingList? Models
    {
        get => grid.DataSource as IBindingList;
        set => grid.DataSource = value;
    }

    /// <summary>
    /// Aktuell im Grid dargestellte ausgewählte Models
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IEnumerable<IModel>? SelectedModels 
    {
        get
        {
            if (tileView != null && grid.MainView == tileView)
                return tileView.GetAllSelectedRows()?.OfType<IModel>();

            if (kanbanView != null && grid.MainView == kanbanView)
                return kanbanView.GetAllSelectedRows()?.OfType<IModel>();

            if (gridView != null && grid.MainView == gridView)
                return gridView.GetAllSelectedRows()?.OfType<IModel>();

            if (bandedgridView != null && grid.MainView == bandedgridView)
                return bandedgridView.GetAllSelectedRows()?.OfType<IModel>();

            return null;
        }
    }

    /// <summary>
    /// Aktuell im Grid fokusiertes/ausgewähltes Model
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IModel? CurrentModel
    {
        get
        {
            if (tileView != null && grid.MainView == tileView)
                return (IModel)tileView.GetFocusedRow();

            if (kanbanView != null && grid.MainView == kanbanView)
                return (IModel)kanbanView.GetFocusedRow();

            if (gridView != null && grid.MainView == gridView)
                return (IModel)gridView.GetFocusedRow();

            if (bandedgridView != null && grid.MainView == bandedgridView)
                return (IModel)bandedgridView.GetFocusedRow();

            return null;
        }
    }

    /// <summary>
    /// Zugriff auf das aktuelle View des Grids
    /// </summary>
    public BaseView CurrentView => grid.MainView;

    /// <summary>
    /// Zugriff auf das GridView
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GridView? GridView => gridView ?? bandedgridView;

    /// <summary>
    /// Zugriff auf das BandedGridView
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AdvBandedGridView? BandedGridView => bandedgridView;

    /// <summary>
    /// Zugriff auf das Grid
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFGridControl Grid => grid;

    /// <summary>
    /// Zugriff auf das TileView (alternative Darstellung via HTML/CSS)
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TileView? TileView => tileView;

    /// <summary>
    /// Zugriff auf das KanbanView (alternative Darstellung via HTML/CSS als Kanban-Board)
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TileView? KanbanView => kanbanView;

    /// <summary>
    /// Auswahl mehrerer Zeilen erlauben (muss vor Setup gesetzt werden), Standard = true
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool MultiSelect { get; set; } = true;

    /// <summary>
    /// Popup-Menu, dass als Zeilenmenu in GridView und AdvBandedGridView angezeigt werden soll.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public PopupMenu? PopupMenuRows { get; set; }

    private CommandResult editHtmlTemplates(CommandArgs args)
    {
        if (tileView == null) return CommandResult.None;

        using var dlg = new FormEditHtmlTemplate();

        dlg.HtmlTemplate = tileView.TileHtmlTemplate.Template;
        dlg.CssTemplate = tileView.TileHtmlTemplate.Styles;


        if (dlg.ShowDialog(FindForm()!) == DialogResult.OK)
        {
            if (tileView.TileHtmlTemplate.Template == dlg.HtmlTemplate && tileView.TileHtmlTemplate.Styles == dlg.CssTemplate) return CommandResult.None;

            tileView.TileHtmlTemplate.Template = dlg.HtmlTemplate;
            tileView.TileHtmlTemplate.Styles = dlg.CssTemplate;

            if (tileView.GetGridIdentifier() is not { } id || id == Guid.Empty) return CommandResult.None;

            AFCore.App.Persistance?.Set(id, tileView.GetTemplate().ToJsonBytes(), extName: "tvw");

            return CommandResult.Info("Schablone gespeichert.");
        }

        return CommandResult.None;
    }

    private CommandResult editKanbanHtmlTemplates(CommandArgs args)
    {
        if (kanbanView == null) return CommandResult.None;

        using var dlg = new FormEditHtmlTemplate();

        dlg.HtmlTemplate = kanbanView.TileHtmlTemplate.Template;
        dlg.CssTemplate = kanbanView.TileHtmlTemplate.Styles;


        if (dlg.ShowDialog(FindForm()!) == DialogResult.OK)
        {
            if (kanbanView.TileHtmlTemplate.Template == dlg.HtmlTemplate && kanbanView.TileHtmlTemplate.Styles == dlg.CssTemplate) return CommandResult.None;

            kanbanView.TileHtmlTemplate.Template = dlg.HtmlTemplate;
            kanbanView.TileHtmlTemplate.Styles = dlg.CssTemplate;

            if (kanbanView.GetGridIdentifier() is not { } id || id == Guid.Empty) return CommandResult.None;

            AFCore.App.Persistance?.Set(id, kanbanView.GetTemplate().ToJsonBytes(), extName: "kvw");

            return CommandResult.Info("Schablone gespeichert.");
        }

        return CommandResult.None;
    }

    private CommandResult saveTvLayout(CommandArgs args)
    {
        if (grid.MainView != tileView) return CommandResult.Error("In dieser Ansicht nicht verfügbar.");

        return tileView.SaveLayout(extName: "tvw");
    }

    private CommandResult saveKvLayout(CommandArgs args)
    {
        if (grid.MainView != kanbanView) return CommandResult.Error("In dieser Ansicht nicht verfügbar.");

        return kanbanView.SaveLayout(extName: "kvw");
    }
    
    private CommandResult saveTvLayoutAs(CommandArgs args)
    {
        if (grid.MainView != tileView) return CommandResult.Error("In dieser Ansicht nicht verfügbar.");

        var name = getLayoutName();
        if (name == null) return CommandResult.Warning("Das Speichern wurde abgebrochen.");
        
        return tileView.SaveLayoutAs(name, extName: "tvw");
    }

    private CommandResult saveKvLayoutAs(CommandArgs args)
    {
        if (grid.MainView != kanbanView) return CommandResult.Error("In dieser Ansicht nicht verfügbar.");

        var name = getLayoutName();
        if (name == null) return CommandResult.Warning("Das Speichern wurde abgebrochen.");

        return kanbanView.SaveLayoutAs(name, extName: "kvw");
    }

    private CommandResult manageTvLayouts(CommandArgs args)
    {
        if (grid.MainView != tileView) return CommandResult.Error("In dieser Ansicht nicht verfügbar.");

        return tileView.ManageLayouts(extName: "tvw");
    }

    private CommandResult manageKvLayouts(CommandArgs args)
    {
        if (grid.MainView != kanbanView) return CommandResult.Error("In dieser Ansicht nicht verfügbar.");

        return kanbanView.ManageLayouts(extName: "kvw");
    }

    private CommandResult resetTvLayout(CommandArgs args)
    {
        if (grid.MainView != tileView) return CommandResult.Error("In dieser Ansicht nicht verfügbar.");

        tileView.DeleteUserLayout(extName: "tvw");

        return tileView.LoadDefaultLayout(extName: "tvw");
    }

    private CommandResult resetKvLayout(CommandArgs args)
    {
        if (grid.MainView != kanbanView) return CommandResult.Error("In dieser Ansicht nicht verfügbar.");

        kanbanView.DeleteUserLayout(extName: "kvw");

        return kanbanView.LoadDefaultLayout(extName: "kvw");
    }

    private CommandResult saveTvDefaultLayout(CommandArgs args)
    {
        if (grid.MainView != tileView) return CommandResult.Error("In dieser Ansicht nicht verfügbar.");

        return tileView.SaveDefaultLayout(extName: "tvw");
    }

    private CommandResult saveKvDefaultLayout(CommandArgs args)
    {
        if (grid.MainView != kanbanView) return CommandResult.Error("In dieser Ansicht nicht verfügbar.");

        return kanbanView.SaveDefaultLayout(extName: "kvw");
    }

    private CommandResult resetTvDefaultLayout(CommandArgs args)
    {
        if (grid.MainView != tileView) return CommandResult.Error("In dieser Ansicht nicht verfügbar.");
         
        return tileView.DeleteDefaultLayout(extName: "tvw");
    }

    private CommandResult resetKvDefaultLayout(CommandArgs args)
    {
        if (grid.MainView != kanbanView) return CommandResult.Error("In dieser Ansicht nicht verfügbar.");

        return kanbanView.DeleteDefaultLayout(extName: "kvw");
    }

    private CommandResult loadTvLayout(CommandArgs args)
    {
        if (grid.MainView != tileView) return CommandResult.Error("In dieser Ansicht nicht verfügbar.");

        if (args.Tag is not AFPopupMenuEntry menu) return CommandResult.None;

        if (menu.Tag is not string name) return CommandResult.None;

        if (name == "")
            return tileView.LoadDefaultLayout(extName: "tvw");

        return tileView.LoadLayout(name, extName: "tvw");
    }

    private CommandResult loadKvLayout(CommandArgs args)
    {
        if (grid.MainView != kanbanView) return CommandResult.Error("In dieser Ansicht nicht verfügbar.");

        if (args.Tag is not AFPopupMenuEntry menu) return CommandResult.None;

        if (menu.Tag is not string name) return CommandResult.None;

        if (name == "")
            return kanbanView.LoadDefaultLayout(extName: "kvw");

        return kanbanView.LoadLayout(name, extName: "kvw");
    }

    private CommandResult saveGvLayout(CommandArgs args)
    {
        if (grid.MainView == tileView) return CommandResult.Error("In dieser Ansicht nicht verfügbar.");

        if (grid.MainView == gridView)
            return gridView.SaveLayout(extName: "gvw");
        
        if (grid.MainView == bandedgridView)
            return bandedgridView.SaveLayout(extName: "bvw");

        return CommandResult.None;
    }

    private CommandResult saveGvLayoutAs(CommandArgs args)
    {
        if (grid.MainView == tileView) return CommandResult.Error("In dieser Ansicht nicht verfügbar.");

        var name = getLayoutName();
        
        if (name == null) return CommandResult.Warning("Das Speichern wurde abgebrochen.");

        if (grid.MainView == gridView)
            return gridView.SaveLayoutAs(name, extName: "gvw");
        
        if (grid.MainView == bandedgridView)
            return bandedgridView.SaveLayoutAs(name, extName: "bvw");

        return CommandResult.None;
    }

    private CommandResult manageGvLayouts(CommandArgs args)
    {
        if (grid.MainView == tileView) return CommandResult.Error("In dieser Ansicht nicht verfügbar.");

        if (grid.MainView == gridView)
            return gridView.ManageLayouts(extName: "gvw");

        if (grid.MainView == bandedgridView)
            return bandedgridView.ManageLayouts(extName: "bvw");

        return CommandResult.None;
    }

    private CommandResult resetGvLayout(CommandArgs args)
    {
        if (grid.MainView == tileView) return CommandResult.Error("In dieser Ansicht nicht verfügbar.");

        if (grid.MainView == gridView)
        {
            gridView.DeleteUserLayout(extName: "gvw");
            return gridView.LoadDefaultLayout(extName: "gvw");
        }
        if (grid.MainView == bandedgridView)
        {
            bandedgridView.DeleteUserLayout(extName: "bvw");
            return bandedgridView.LoadDefaultLayout(extName: "bvw");
        }

        return CommandResult.None;
    }

    private CommandResult saveGvDefaultLayout(CommandArgs args)
    {
        if (grid.MainView == tileView) return CommandResult.Error("In dieser Ansicht nicht verfügbar.");

        if (grid.MainView == gridView)
            return gridView.SaveDefaultLayout(extName: "gvw");
        
        if (grid.MainView == bandedgridView)
            return bandedgridView.SaveDefaultLayout(extName: "bvw");

        return CommandResult.None;

    }

    private CommandResult resetGvDefaultLayout(CommandArgs args)
    {
        if (grid.MainView == tileView) return CommandResult.Error("In dieser Ansicht nicht verfügbar.");

        if (grid.MainView == gridView)
            return gridView.DeleteDefaultLayout(extName: "gvw");
        
        if (grid.MainView == bandedgridView)
            return bandedgridView.DeleteDefaultLayout(extName: "bvw");

        return CommandResult.None;
    }

    private CommandResult loadGvLayout(CommandArgs args)
    {
        if (grid.MainView == tileView) return CommandResult.Error("In dieser Ansicht nicht verfügbar.");

        if (args.Tag is not AFPopupMenuEntry menu) return CommandResult.None;

        if (menu.Tag is not string name) return CommandResult.None;

        if (name == "")
        {
            if (grid.MainView == gridView)
                return gridView.LoadDefaultLayout(extName: "gvw");
            if (grid.MainView == bandedgridView)
                return bandedgridView.LoadDefaultLayout(extName: "bvw");
        }

        if (grid.MainView == gridView)
            return gridView.LoadLayout(name, extName: "gvw");
        
        if (grid.MainView == bandedgridView)
            return bandedgridView.LoadLayout(name, extName: "bvw");

        return CommandResult.None;

    }

    private string? getLayoutName()
    {
        AFEditSingleline sle = new() { Dock = DockStyle.Fill };
        sle.Properties.MaxLength = 150;

        using FormAsk dlg = new(sle, "Name des Layouts", "Geben Sie einen Namen für ein, unter dem das Layout gespeichert werden soll. <b>Existiert bereits ein Layout mit dem gleichen Namen, wird es überschrieben.</b>", "LAYOUT SPEICHERN ALS");
        dlg.ValidateInput = checkName;

        if (dlg.ShowDialog(FindForm()) == DialogResult.Cancel) return null;

        return sle.Text;
    }

    private bool checkName(Control ctrl)
    {
        if (ctrl is not AFEditSingleline sle) return true;

        if (sle.Text.Trim().Length < 3)
        {
            (sle.FindForm() as FormAsk)?.ShowMessageError("Der Name muss mindestens 3 Zeichen lang sein.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Löscht ein Model aus der Datenquelle  (wenn enthalten)
    /// </summary>
    /// <param name="model"></param>
    public void RemoveModel(IModel model)
    {
        var loeschen = Models?.OfType<IModel>().FirstOrDefault(m => m.PrimaryKey == model.PrimaryKey);
        
        if (loeschen != null)
            Models?.Remove(loeschen);
    }
}

