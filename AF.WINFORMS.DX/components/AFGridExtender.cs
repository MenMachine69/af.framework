using AF.MVC;
using DevExpress.Utils.Menu;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraGrid.Views.Card.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.XtraGrid.Views.Layout.ViewInfo;

namespace AF.WINFORMS.DX;

/// <summary>
/// Erweiterung der Funktionen eines XtraGrid-Controls
/// </summary>
[ToolboxItem(true)]
[SupportedOSPlatform("windows")]
public class AFGridExtender : AFGridPivotExtenderBase
{
    private GridControl? _grid;
    private bool _defaultEntriesAdded;
    private CardHitInfo? _cardhitInfo;
    private BandedGridHitInfo? _bandhitinfo;
    private LayoutViewHitInfo? _layouthitinfo;
    private GridHitInfo? _gridhitInfo;


    /// <summary>
    /// das zu erweiternde Grid
    /// </summary>
    [Browsable(true)]
    [DefaultValue(null)]
    public GridControl? Grid
    {
        get => _grid;
        set
        {
            if (_grid == value) return;

            _grid = value;

            if (!UI.DesignMode)
                extendGrid();
        }
    }

    /// <summary>
    /// Menüeinträge für AutoColumnWith, Footer und Export hinzufügen
    /// </summary>
    [Browsable(true)]
    [DefaultValue(true)]
    public bool AddDefaultMenu { get; set; } = true;

    /// <summary>
    /// Menüeinträge für die Verwaltung des Layouts hinzufügen
    /// </summary>
    [Browsable(true)]
    [DefaultValue(true)]
    public bool AddLayoutMenu { get; set; } = true;

    /// <summary>
    /// Menüeinträge für berechnete Felder hinzufügen
    /// </summary>
    [Browsable(true)]
    [DefaultValue(true)]
    public bool AddCustomColumnsMenu { get; set; } = true;

    /// <summary>
    /// Menüeinträge für Export hinzufügen
    /// </summary>
    [Browsable(true)]
    [DefaultValue(true)]
    public bool AddExportMenu { get; set; } = true;

    /// <summary>
    /// DragDrop-Unterstützung
    /// </summary>
    [Browsable(true)]
    [DefaultValue(false)]
    public bool SupportDragDrop { get; set; } = false;

    /// <summary>
    /// Handler für RequestDragDropData
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void RequestDragDropDataHandler(object sender, RequestDragDataEventArgs args);

    /// <summary>
    /// RequestDragDropData Ereignis, das ausgelöst wird, wenn Drag-Daten benötigt werden
    /// </summary>
    public event RequestDragDropDataHandler? RequestDragData;

    private void extendGrid()
    {
        if (_grid == null) return;

        _grid.MouseDown += _mouseDown;
        _grid.MouseMove += _mouseMove;

        _defaultEntriesAdded = false;

        foreach (var view in _grid.ViewCollection)
        {
            if (view is GridView gridView)
                gridView.PopupMenuShowing += extendMenu;
            else if (view is BandedGridView bandedView)
                bandedView.PopupMenuShowing += extendMenu;
            else if (view is AdvBandedGridView advbandedView)
                advbandedView.PopupMenuShowing += extendMenu;
        }
    }

    private void extendMenu(object? sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
    {
        if (sender is not ColumnView view) return;
      
        if (!_defaultEntriesAdded)
        {
            if (AddDefaultMenu)
            { 
                DefaultMenuEntries.Add(new(Properties.Resources.MEN_TOGGLEAUTOWITH, toggleAutoColumnWitdth));
                DefaultMenuEntries.Add(new(Properties.Resources.MEN_TOGGLEFOOTER, toggleFooter));
            }

            if (AddCustomColumnsMenu)
                DefaultMenuEntries.Add(new(Properties.Resources.MEN_GRIDCALCEDCOLUMNS, manageCustomColumns));

            if (AddExportMenu)
                DefaultMenuEntries.Add(new(Properties.Resources.MEN_GRIDEXPORT, export) { BeginGroup = true });


            if (AddLayoutMenu && view is GridView gridView && 
                gridView.Tag is AFGridViewState state && 
                state.Identifier.IsNotEmpty() && 
                AFCore.App.Persistance != null && AFCore.App.Persistance.IsAvailable)
            {

                var dict = gridView.GetAvailableLayouts(extName: state.KeyName);

                DefaultMenuEntries.Add(new("Layout\\Standardlayout", loadLayout) { BeginGroup = true });

                foreach (var entry in dict)
                    DefaultMenuEntries.Add(new("Layout\\" + entry.Value, loadLayout) { Tag = entry.Value });

                DefaultMenuEntries.Add(new("Layout\\Layout speichern", saveLayout) { BeginGroup = true, Image = UI.GetImage(Symbol.Save)});
                DefaultMenuEntries.Add(new("Layout\\Layout speichern als", saveLayoutAs) { Image = UI.GetImage(Symbol.SaveEdit)});
                DefaultMenuEntries.Add(new("Layout\\Layouts verwalten", manageLayout) { Image = UI.GetImage(Symbol.Wrench)});
                DefaultMenuEntries.Add(new("Layout\\Layout zurücksetzen", resetLayout) { BeginGroup = true, Image = UI.GetImage(Symbol.ArrowReset)});

                if (AFCore.App.SecurityService != null && (AFCore.App.SecurityService.CurrentUser?.IsAdmin ?? false))
                {
                    DefaultMenuEntries.Add(new("Layout\\Administration\\Standardlayout speichern", saveDefaultLayout) { BeginGroup = true });
                    DefaultMenuEntries.Add(new("Layout\\Administration\\Standardlayout zurücksetzen", resetDefaultLayout));
                }
            }

            _defaultEntriesAdded = true;
        }

        var defaultentries = DefaultMenuEntries.Where(
                entry => entry.MenuType == e.MenuType && 
                         (entry.ViewName == view.Name || 
                          entry.ViewName.IsEmpty()))
            .ToArray();

        foreach (var entry in defaultentries)
        {
            DXMenuItem? item = e.Menu.AddMenuEntry(entry, view);
            if (item != null) item.Click += (_, _) => onclick(entry);
        }

        var entries = MenuEntries.Where(
                entry => entry.MenuType == e.MenuType && 
                         (entry.ViewName == view.Name || 
                          entry.ViewName.IsEmpty()))
            .OrderBy(entry => entry.Position).ToArray();


        foreach (var entry in entries)
        {
            DXMenuItem? item = e.Menu.AddMenuEntry(entry, view);
            if (item != null) item.Click += (_, _) => onclick(entry);
        }
    }

    private void onclick(AFPopupMenuEntry entry)
    {
        var result = entry.ClickAction.Invoke(new() { CommandSource = Grid!.FocusedView });
        
        if (result.Result != eNotificationType.None)
            (Grid!.FindForm() as ICommandResultDisplay)?.HandleResult(result);
    }

    private CommandResult saveLayout(CommandArgs data)
    {
        if (data.CommandSource is not GridView view) return CommandResult.None;

        if (view.Tag is not AFGridViewState state) return CommandResult.None;

        return view.SaveLayout(extName: state.KeyName);
    }

    private string? getLayoutName(GridView view)
    {
        AFEditSingleline sle = new() { Dock = DockStyle.Fill };
        sle.Properties.MaxLength = 150;

        using FormAsk dlg = new(sle, "Name des Layouts", "Geben Sie einen Namen für ein, unter dem das Layout gespeichert werden soll. <b>Existiert bereits ein Layout mit dem gleichen Namen, wird es überschrieben.</b>", "LAYOUT SPEICHERN ALS");
        dlg.ValidateInput = checkName;

        if (dlg.ShowDialog(view.GridControl.FindForm()) == DialogResult.Cancel) return null;

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

    private CommandResult saveLayoutAs(CommandArgs data)
    {
        if (data.CommandSource is not GridView view) return CommandResult.None;

        if (view.Tag is not AFGridViewState state) return CommandResult.None;

        var name = getLayoutName(view);

        if (name == null) return CommandResult.Warning("Das Speichern wurde abgebrochen.");

        return view.SaveLayoutAs(name, extName: state.KeyName);
    }

    private CommandResult loadLayout(CommandArgs args)
    {
        if (args.CommandSource is not GridView view) return CommandResult.None;

        if (args.Tag is not AFPopupMenuEntry menu) return CommandResult.None;

        if (view.Tag is not AFGridViewState state) return CommandResult.None;

        if (menu.Tag is not string name) return CommandResult.None;

        if (name == "")
            return view.LoadDefaultLayout(extName: state.KeyName);

        return view.LoadLayout(name, extName: state.KeyName);
    }

    private CommandResult resetLayout(CommandArgs args)
    {
        if (args.CommandSource is not GridView view) return CommandResult.None;

        if (view.Tag is not AFGridViewState state) return CommandResult.None;

        view.DeleteUserLayout(extName: state.KeyName);
        
        return view.LoadDefaultLayout(extName: state.KeyName);
    }

    private CommandResult saveDefaultLayout(CommandArgs args)
    {
        if (args.CommandSource is not GridView view) return CommandResult.None;

        if (view.Tag is not AFGridViewState state) return CommandResult.None;

        return view.SaveDefaultLayout(extName: state.KeyName);
    }


    private CommandResult resetDefaultLayout(CommandArgs args)
    {
        if (args.CommandSource is not GridView view) return CommandResult.None;

        if (view.Tag is not AFGridViewState state) return CommandResult.None;

        return view.DeleteDefaultLayout(extName: state.KeyName);
    }

    private CommandResult manageLayout(CommandArgs data)
    {
        if (data.CommandSource is not GridView view) return CommandResult.None;

        if (view.Tag is not AFGridViewState state) return CommandResult.None;

        return view.ManageLayouts(extName: state.KeyName);
    }

    private CommandResult manageCustomColumns(CommandArgs data)
    {
        if (data.CommandSource is not GridView view) return CommandResult.None;

        return view.ManageLayouts();
    }

    private CommandResult export(CommandArgs data)
    {
        if (data.CommandSource is not ColumnView view) return CommandResult.None;

        view.GridControl.Export();

        return CommandResult.None;
    }

    /// <summary>
    /// Das Export-Command auslösen
    /// </summary>
    /// <returns></returns>
    public CommandResult InvokeExport()
    {
        if (Grid == null) return CommandResult.None;

        CommandArgs args = new() { CommandSource = Grid.FocusedView };

        return export(args);
    }

    private CommandResult toggleFooter(CommandArgs data)
    {
        if (data.CommandSource is GridView view) 
            view.OptionsView.ShowFooter = !view.OptionsView.ShowFooter;

        return CommandResult.None;
    }

    private CommandResult toggleAutoColumnWitdth(CommandArgs data)
    {
        if (data.CommandSource is GridView view) 
            view.OptionsView.ColumnAutoWidth = !view.OptionsView.ColumnAutoWidth;

        return CommandResult.None;
    }

    private void _mouseDown(object? sender, MouseEventArgs ev)
    {
        if (_grid == null || SupportDragDrop == false) return;

        if (_grid.FocusedView is GridView gridview)
        {
            GridHitInfo hitInfo = gridview.CalcHitInfo(new Point(ev.X, ev.Y));

            if (ev.Button == MouseButtons.Left && hitInfo.InRow)
                _gridhitInfo = hitInfo;
        }
        else if (_grid.FocusedView is CardView cardview)
        {
            CardHitInfo hitInfo = cardview.CalcHitInfo(new Point(ev.X, ev.Y));

            if (ev.Button == MouseButtons.Left && hitInfo.InCard)
                _cardhitInfo = hitInfo;
        }
        else if (_grid.FocusedView is AdvBandedGridView advgridview)
        {
            BandedGridHitInfo hitInfo = advgridview.CalcHitInfo(new Point(ev.X, ev.Y));

            if (ev.Button == MouseButtons.Left && hitInfo.InRow)
                _bandhitinfo = hitInfo;
        }
        else if (_grid.FocusedView is BandedGridView bandedgridview)
        {
            BandedGridHitInfo hitInfo = bandedgridview.CalcHitInfo(new Point(ev.X, ev.Y));

            if (ev.Button == MouseButtons.Left && hitInfo.InRow)
                _bandhitinfo = hitInfo;
        }
        else if (_grid.FocusedView is LayoutView layoutview)
        {
            LayoutViewHitInfo hitInfo = layoutview.CalcHitInfo(new Point(ev.X, ev.Y));

            if (ev.Button == MouseButtons.Left && hitInfo.InCard)
                _layouthitinfo = hitInfo;
        }
    }

    private void _mouseMove(object? sender, MouseEventArgs ev)
    {
        if (_grid == null || SupportDragDrop == false || ev.Button != MouseButtons.Left) return;

        Size dragSize = SystemInformation.DragSize;

        if (_grid.FocusedView is GridView gridview)
        {
            if (gridview.RowCount == 0) return;

            if (_gridhitInfo == null) return;

            Rectangle dragRect = new Rectangle(new Point(_gridhitInfo.HitPoint.X - dragSize.Width / 2,
                _gridhitInfo.HitPoint.Y - dragSize.Height / 2), dragSize);

            if (dragRect.Contains(new Point(ev.X, ev.Y))) return;

            var obj = gridview.GetRow(_gridhitInfo.RowHandle);

            if (RequestDragData != null)
            {
                RequestDragDataEventArgs args = new RequestDragDataEventArgs { DraggedRow = obj, Grid = gridview.GridControl, View = gridview };
                RequestDragData?.Invoke(this, args);
                if (args.DragData != null)
                    obj = args.DragData;
            }

            if (obj != null)
                gridview.GridControl.DoDragDrop(obj, DragDropEffects.All);

            _gridhitInfo = null;
        }
        else if (_grid.MainView is CardView cardview)
        {
            if (cardview.RowCount == 0) return;

            if (_cardhitInfo == null) return;

            Rectangle dragRect = new Rectangle(new Point(_cardhitInfo.HitPoint.X - dragSize.Width / 2,
                _cardhitInfo.HitPoint.Y - dragSize.Height / 2), dragSize);

            if (dragRect.Contains(new Point(ev.X, ev.Y))) return;

            var obj = cardview.GetRow(_cardhitInfo.RowHandle);

            if (RequestDragData != null)
            {
                RequestDragDataEventArgs args = new RequestDragDataEventArgs { DraggedRow = obj, Grid = cardview.GridControl, View = cardview };
                RequestDragData(this, args);
                if (args.DragData != null)
                    obj = args.DragData;
            }

            if (obj != null)
                cardview.GridControl.DoDragDrop(obj, DragDropEffects.All);

            _cardhitInfo = null;
        }
        else if (_grid.MainView is AdvBandedGridView advgridview)
        {
            if (advgridview.RowCount == 0)
                return;

            if (_bandhitinfo == null) return;

            Rectangle dragRect = new Rectangle(new Point(_bandhitinfo.HitPoint.X - dragSize.Width / 2,
                _bandhitinfo.HitPoint.Y - dragSize.Height / 2), dragSize);

            if (dragRect.Contains(new Point(ev.X, ev.Y))) return;

            var obj = advgridview.GetRow(_bandhitinfo.RowHandle);

            if (RequestDragData != null)
            {
                RequestDragDataEventArgs args = new RequestDragDataEventArgs { DraggedRow = obj, Grid = advgridview.GridControl, View = advgridview };
                RequestDragData(this, args);
                if (args.DragData != null)
                    obj = args.DragData;
            }

            if (obj != null)
                advgridview.GridControl.DoDragDrop(obj, DragDropEffects.All);

            _bandhitinfo = null;
        }
        else if (_grid.MainView is BandedGridView bandedgridview)
        {
            if (bandedgridview.RowCount == 0) return;

            if (_bandhitinfo == null) return;

            Rectangle dragRect = new Rectangle(new Point(_bandhitinfo.HitPoint.X - dragSize.Width / 2,
                _bandhitinfo.HitPoint.Y - dragSize.Height / 2), dragSize);

            if (dragRect.Contains(new Point(ev.X, ev.Y))) return;

            var obj = bandedgridview.GetRow(_bandhitinfo.RowHandle);

            if (RequestDragData != null)
            {
                RequestDragDataEventArgs args = new RequestDragDataEventArgs { DraggedRow = obj, Grid = bandedgridview.GridControl, View = bandedgridview };
                RequestDragData(this, args);
                if (args.DragData != null)
                    obj = args.DragData;
            }

            if (obj != null)
                bandedgridview.GridControl.DoDragDrop(obj, DragDropEffects.All);

            _bandhitinfo = null;
        }
        else if (_grid.MainView is LayoutView layoutview)
        {
            if (layoutview.RowCount == 0) return;

            if (_layouthitinfo == null) return;

            Rectangle dragRect = new Rectangle(new Point(_layouthitinfo.HitPoint.X - dragSize.Width / 2,
                _layouthitinfo.HitPoint.Y - dragSize.Height / 2), dragSize);

            if (dragRect.Contains(new Point(ev.X, ev.Y))) return;

            var obj = layoutview.GetRow(_layouthitinfo.RowHandle);

            if (RequestDragData != null)
            {
                RequestDragDataEventArgs args = new RequestDragDataEventArgs { DraggedRow = obj, Grid = layoutview.GridControl, View = layoutview };
                RequestDragData(this, args);
                if (args.DragData != null)
                    obj = args.DragData;
            }

            if (obj != null)
                layoutview.GridControl.DoDragDrop(obj, DragDropEffects.All);

            _layouthitinfo = null;
        }
    }
}