using AF.MVC;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Tile;

namespace AF.WINFORMS.DX;

/// <summary>
/// Extension Methoden für GridControl
/// </summary>
[SupportedOSPlatform("windows")]
public static class GridControlEx
{
    /// <summary>
    /// Exportieren der Daten des Grids
    /// </summary>
    /// <param name="grid"></param>
    public static void Export(this GridControl grid)
    {
        using (DevExpress.XtraEditors.XtraSaveFileDialog dialog = new())
        {
            dialog.Filter = WinFormsStrings.GRID_EXPORTFILTER;
            dialog.FilterIndex = 0;
            dialog.AddExtension = true;
            dialog.CheckPathExists = true;
            dialog.DefaultExt = @"xlsx";
            dialog.Title = WinFormsStrings.LBL_EXPORTTOFILE;
            dialog.UseParentFormIcon = true;

            if (dialog.ShowDialog(grid.FindForm()) != DialogResult.OK) return;

            try
            {
                if (dialog.FileName.ToLower().EndsWith(@"xlsx"))
                    grid.ExportToXlsx(dialog.FileName);
                else if (dialog.FileName.ToLower().EndsWith(@"csv"))
                    grid.ExportToCsv(dialog.FileName);
                else if (dialog.FileName.ToLower().EndsWith(@"pdf"))
                    grid.ExportToPdf(dialog.FileName);
                else if (dialog.FileName.ToLower().EndsWith(@"xls"))
                    grid.ExportToXls(dialog.FileName);
                else if (dialog.FileName.ToLower().EndsWith(@"txt"))
                    grid.ExportToText(dialog.FileName);
                else if (dialog.FileName.ToLower().EndsWith(@"rtf"))
                    grid.ExportToRtf(dialog.FileName);
                else if (dialog.FileName.ToLower().EndsWith(@"docx"))
                    grid.ExportToDocx(dialog.FileName);
                else if (dialog.FileName.ToLower().EndsWith(@"html") || dialog.FileName.ToLower().EndsWith(@"htm"))
                    grid.ExportToHtml(dialog.FileName);
                else if (dialog.FileName.ToLower().EndsWith(@"mht"))
                    grid.ExportToMht(dialog.FileName);
                else
                    throw new Exception(WinFormsStrings.ERR_UNKNOWNFILEFORMAT);

                MsgBox.ShowInfoOk(WinFormsStrings.MSG_EXPORTSUCCESS);
            }
            catch (Exception ex)
            {
                MsgBox.ShowErrorOk(WinFormsStrings.MSG_EXPORTERROR + ex.Message);
            }

        }
    }


    /// <summary>
    /// Return the row at the given location
    /// </summary>
    /// <param name="location">location</param>
    /// <param name="view">GridView</param>
    /// <returns>the row object</returns>
    [SupportedOSPlatform("windows")]
    public static object? GetRowFromLocation(this GridView view, Point location)
    {
        var info = view.CalcHitInfo(view.GridControl.PointToClient(location));

        if (info.InDataRow)
            return view.GetRow(info.RowHandle);

        return null;
    }


    /// <summary>
    /// Setup für das GridControl
    /// </summary>
    /// <param name="grid">GridControl</param>
    /// <param name="setup">Einstellungen die auf das GridControl angewendet werden sollen</param>
    public static void Setup(this GridControl grid, AFGridSetup setup)
    {
        SvgImageCollection? images = null;

        if (setup.HtmlImages != null && setup.HtmlImages.Length > 0)
        {
            images = new();

            setup.HtmlImages.ForEach(i => images.Add(i.Item1, i.Item2));
        }

        switch (setup.DefaultGridStyle)
        {
            case eGridMode.GridView:
                if (grid.MainView is not GridView)
                {
                    var vw = grid.Views.FirstOrDefault(v => v is GridView) as GridView;
                    if (vw is null)
                    {
                        vw = new GridView();
                        vw.GridControl = grid;
                        grid.ViewCollection.Add(vw);
                    }

                    grid.MainView = vw;
                }
                ((GridView)grid.MainView).Setup(setup);
                break;
            case eGridMode.AdvBandedGridView:
                if (grid.MainView is not AdvBandedGridView)
                {
                    var vw = grid.Views.FirstOrDefault(v => v is AdvBandedGridView) as AdvBandedGridView;
                    if (vw is null)
                    {
                        vw = new AdvBandedGridView();
                        vw.GridControl = grid;
                        grid.ViewCollection.Add(vw);
                    }

                    grid.MainView = vw;
                }
                ((AdvBandedGridView)grid.MainView).Setup(setup);
                break;
            case eGridMode.BandedGridView:
                if (grid.MainView is not BandedGridView)
                {
                    var vw = grid.Views.FirstOrDefault(v => v is BandedGridView) as BandedGridView;
                    if (vw is null)
                    {
                        vw = new BandedGridView();
                        vw.GridControl = grid;
                        grid.ViewCollection.Add(vw);
                    }

                    grid.MainView = vw;
                }
                ((BandedGridView)grid.MainView).Setup(setup);
                break;
            case eGridMode.TileView:
                if (grid.MainView is not TileView)
                {
                    var vw = grid.Views.FirstOrDefault(v => v is TileView) as TileView;
                    if (vw is null)
                    {
                        vw = new TileView();
                        vw.GridControl = grid;
                        grid.ViewCollection.Add(vw);
                    }

                    if (images != null)
                        vw.HtmlImages = images;

                    grid.MainView = vw;
                }
                ((TileView)grid.MainView).Setup(setup);

                break;
        }
    }
}

/// <summary>
/// Status eines GridViews zum Speichern im 'Tag' des GridViews
/// </summary>
public sealed class AFGridViewState
{
    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="identifier">ID des Grids</param>
    public AFGridViewState(Guid identifier)
    {
        Identifier = identifier;
    }

    /// <summary>
    /// Standardlayout des GridViews
    /// </summary>
    public byte[]? DefaultLayout { get; set; }

    /// <summary>
    /// Standardlayout des TileViews
    /// </summary>
    public AFHtmlTemplate? DefaultTemplate { get; set; }

    /// <summary>
    /// ID des GridViews
    /// </summary>
    public Guid Identifier { get; set; }

    /// <summary>
    /// KeyName des Grids
    /// </summary>
    public string? KeyName { get; set; }

    /// <summary>
    /// Methode die zur Anpassung der Darstellung einer Zelle verwendet werden soll
    /// </summary>
    public Action<object, object>? RowCellStyler { get; set; }

    /// <summary>
    /// Methode die zur Anpassung der Darstellung einer Zeile verwendet werden soll
    /// </summary>
    public Action<object, object, object>? RowStyler { get; set; }

    /// <summary>
    /// Methode die zum Zeichnen einer Zeile verwendet werden soll
    /// </summary>
    public Action<object, object>? CellPainter { get; set; }

    /// <summary>
    /// Methode die zum ermitteln des Anzeigetextes verwendet werden soll
    /// </summary>
    public Action<object, object>? DisplayTextStyler { get; set; }
}

/// <summary>
/// Extension methods für GridView
/// </summary>
[SupportedOSPlatform("windows")]
public static class GridViewEx
{
    /// <summary>
    /// Setup the gridview to display objects of the given.
    ///
    /// GridStyle is set to Full
    /// </summary>
    /// <param name="view">gridview</param>
    /// <param name="type">Type of objects</param>
    public static void Setup(this GridView view, Type type)
    {
        var controller = type.GetController();

        Setup(view, controller is IControllerUI uiController
            ? uiController.GetGridSetup(eGridStyle.Full)
            : type.GetTypeDescription().GetGridSetup(eGridStyle.Full));
    }

    /// <summary>
    /// Liefert das Setup eines gridviews, damit dieses im Anschluss modifiziert werden kann.
    /// </summary>
    /// <param name="view">gridview</param>
    /// <param name="type">Type of objects</param>
    public static AFGridSetup GetSetup(this GridView view, Type type)
    {
        var controller = type.GetController();

        return controller is IControllerUI uiController
            ? uiController.GetGridSetup(eGridStyle.Full)
            : type.GetTypeDescription().GetGridSetup(eGridStyle.Full);
    }

    /// <summary>
    /// Setup the gridview to display objects of the given
    /// </summary>
    /// <param name="view">gridview</param>
    /// <param name="type">Type of objects</param>
    /// <param name="style">style/content of the grid</param>
    public static void Setup(this GridView view, Type type, eGridStyle style)
    {
        IController controller = type.GetController();

        Setup(view, controller is IControllerUI uiController
            ? uiController.GetGridSetup(style)
            : type.GetTypeDescription().GetGridSetup(style));
    }

    /// <summary>
    /// Identifier für das GridView setzen
    /// </summary>
    /// <param name="view">GridView</param>
    /// <param name="identifier">ID des GridViews</param>
    public static void SetGridIdentifier(this GridView view, Guid identifier)
    {
        if (view.Tag is not AFGridViewState state)
            view.Tag = new AFGridViewState(identifier) { KeyName = view is AdvBandedGridView || view is BandedGridView ? "bvw" : "gvw" };
        else
            state.Identifier = identifier;
    }

    /// <summary>
    /// Identifier für das GridView holen
    /// </summary>
    /// <param name="view">GridView</param>
    /// <returns>ID (Guid) des GridViews (gesetzt via SetGridIdentifier oder Setup) oder NULL</returns>
    public static Guid? GetGridIdentifier(this GridView view)
    {
        if (view.Tag is not AFGridViewState state)
            return null;

        return state.Identifier;
    }


    /// <summary>
    /// Setup the gridview using the given setup.
    /// </summary>
    /// <param name="view">gridview</param>
    /// <param name="setup">setup for the grid</param>
    public static void Setup(this GridView view, AFGridSetup setup)
    {
        // wenn der aktuelle Identifier des GridViews mit dem des Setup übereinstimmt, kann das Setup übersprungen werden
        if (setup.GridIdentifier.IsNotEmpty() && view.GetGridIdentifier().Equals(setup.GridIdentifier))
            return;

        if (setup.GridIdentifier.IsNotEmpty())
            view.SetGridIdentifier(setup.GridIdentifier);

        checkDefaultEditors(view.GridControl);

        view.GridControl.DataSource = null;
        view.GridControl.RefreshDataSource();

        view.BeginInit();

        // alte Events zurücksetzen
        view.RowCellStyle -= customStyleCell;
        view.RowStyle -= customStyleRow;
        view.CustomDrawCell -= customDrawCell;

        // Reset the grid
        if (view is BandedGridView bgrid)
            bgrid.Bands.Clear();
        else
            view.Columns.Clear();

        view.FocusRectStyle = setup.AllowEdit ? DrawFocusRectStyle.CellFocus : DrawFocusRectStyle.None;
        view.RowHeight = UI.GetScaled(26);

        view.OptionsDetail.EnableMasterViewMode = false;

        view.OptionsView.GroupDrawMode = GroupDrawMode.Office;
        view.OptionsView.ColumnAutoWidth = setup.ColumnAutoWidth;
        view.OptionsView.ShowFooter = false;
        view.OptionsView.AllowGlyphSkinning = true;
        view.OptionsView.EnableAppearanceEvenRow = true;
        view.OptionsView.EnableAppearanceOddRow = true;
        view.OptionsView.ShowIndicator = setup.AllowEdit;

        view.OptionsMenu.EnableColumnMenu = true;
        view.OptionsMenu.EnableFooterMenu = true;
        view.OptionsMenu.EnableGroupPanelMenu = true;
        view.OptionsMenu.EnableGroupRowMenu = true;
        view.OptionsMenu.ShowAutoFilterRowItem = true;
        view.OptionsMenu.ShowConditionalFormattingItem = true;
        view.OptionsMenu.ShowDateTimeGroupIntervalItems = true;
        view.OptionsMenu.ShowGroupSortSummaryItems = true;
        view.OptionsMenu.ShowGroupSummaryEditorItem = true;

        view.OptionsBehavior.Editable = setup.AllowEdit;
        view.OptionsBehavior.AllowFixedGroups = DefaultBoolean.Default;
        view.OptionsSelection.EnableAppearanceHotTrackedRow = DefaultBoolean.True;
        view.OptionsSelection.EnableAppearanceFocusedCell = setup.AllowEdit;

        view.OptionsLayout.StoreAllOptions = false;
        view.OptionsLayout.StoreDataSettings = true;
        view.OptionsLayout.StoreFormatRules = true;
        view.OptionsLayout.StoreAppearance = true;
        view.OptionsLayout.StoreVisualOptions = true;

        view.OptionsBehavior.AllowFixedGroups = DefaultBoolean.True;


        if (!string.IsNullOrEmpty(setup.DefaultFilter))
            view.ActiveFilterString = setup.DefaultFilter;

        // check if buttons needed
        if (setup.CmdDelete != null || setup.CmdEdit != null || setup.CmdGoto != null || setup.CmdShowDetail != null ||
            setup.OnDeleteAction != null || setup.OnEditAction != null || setup.OnGotoAction != null || setup.OnGotoAction != null)
        {
            RepositoryItemButtonEdit? buttons = view.GridControl.RepositoryItems.OfType<RepositoryItemButtonEdit>().FirstOrDefault(ctrl => ctrl.Name == @"_editorButtons");

            if (buttons == null)
                throw new NullReferenceException(@"EditorButtons editor not found in grid repository.");

            var button = buttons.Buttons.FirstOrDefault(btn => btn.Caption == @"Goto");

            if (button != null)
            {
                if (setup.CmdGoto != null)
                    button.Tag = new Tuple<AFCommand?, GridView>(setup.CmdGoto, view);
                else
                    button.Tag = new Tuple<Action<object>?, GridView>(setup.OnGotoAction, view);

                button.Visible = setup.CmdGoto != null || setup.OnGotoAction != null;
            }

            button = buttons.Buttons.FirstOrDefault(btn => btn.Caption == @"Edit");

            if (button != null)
            {
                if (setup.CmdEdit != null)
                    button.Tag = new Tuple<AFCommand?, GridView>(setup.CmdEdit, view);
                else
                    button.Tag = new Tuple<Action<object>?, GridView>(setup.OnEditAction, view);

                button.Visible = setup.CmdEdit != null || setup.OnEditAction != null;
            }

            button = buttons.Buttons.FirstOrDefault(btn => btn.Caption == @"Delete");

            if (button != null)
            {
                if (setup.CmdDelete != null)
                    button.Tag = new Tuple<AFCommand?, GridView>(setup.CmdDelete, view);
                else
                    button.Tag = new Tuple<Action<object>?, GridView>(setup.OnDeleteAction, view);

                button.Visible = setup.CmdDelete != null || setup.OnDeleteAction != null;
            }

            button = buttons.Buttons.FirstOrDefault(btn => btn.Caption == @"Details");

            if (button != null)
            {

                if (setup.CmdShowDetail != null)
                    button.Tag = new Tuple<AFCommand?, GridView>(setup.CmdShowDetail, view);
                else
                    button.Tag = new Tuple<Action<object>?, GridView>(setup.OnShowDetailAction, view);
                button.Visible = setup.CmdShowDetail != null || setup.OnShowDetailAction != null;
            }


            GridColumn column;
            GridBand? band = null;

            if (view is BandedGridView bandview)
            {
                band = bandview.CreateBand();
                column = new BandedGridColumn();
                column.VisibleIndex = band.Columns.Count;
                band.Columns.Add((BandedGridColumn)column);
            }
            else
            {
                column = new GridColumn();
                column.VisibleIndex = view.Columns.Count;
                view.Columns.Add(column);
            }

            column.ColumnEdit = buttons;
            column.MinWidth = buttons.Buttons.VisibleCount * UI.GetScaled(20);
            column.MaxWidth = column.MinWidth;

            if (band != null)
            {
                band.MinWidth = column.MinWidth;
                band.Width = column.Width;
                band.OptionsBand.AllowMove = false;
                band.OptionsBand.FixedWidth = true;
                band.OptionsBand.AllowSize = false;
                band.OptionsBand.ShowInCustomizationForm = false;
                band.OptionsBand.ShowCaption = false;
            }

            column.OptionsColumn.FixedWidth = true;
            column.OptionsColumn.AllowMove = false;
            column.OptionsColumn.AllowEdit = true;
            column.OptionsColumn.AllowSize = false;
            column.OptionsColumn.AllowSort = DefaultBoolean.False;
            column.OptionsColumn.ShowInExpressionEditor = false;
            column.OptionsColumn.AllowShowHide = false;
            column.OptionsColumn.AllowGroup = DefaultBoolean.False;
            column.AllowSummaryMenu = false;
            column.Name = @"_syscolButtons";
            column.OptionsFilter.AllowFilter = false;
            column.OptionsFilter.AllowAutoFilter = false;

            view.OptionsBehavior.Editable = true;
            view.PreviewIndent = column.MinWidth + 5;
        }

        if (view is BandedGridView bgridd)
        {
            foreach (GridBandSettings band in setup.Bands)
                createBand(bgridd, band, setup.AllowEdit);
        }
        else
        {
            foreach (AFGridColumn column in setup.Columns.OrderBy(col => col.ColumnIndex))
                CreateColumn(view, column, setup.AllowEdit);
        }

        if (setup.AllowAddNew)
        {
            view.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom;
            view.OptionsBehavior.AllowAddRows = DefaultBoolean.True;
        }

        if (setup.PreviewField.IsNotEmpty())
        {
            view.OptionsView.ShowPreview = true;
            view.OptionsView.AutoCalcPreviewLineCount = true;
            view.PreviewFieldName = setup.PreviewField;
        }

        view.OptionsSelection.MultiSelect = (setup.SelectionMode != eSelectionMode.Single);
        view.OptionsSelection.MultiSelectMode =
            (setup.UseCheckBoxSelection && setup.SelectionMode == eSelectionMode.Multi
                ? GridMultiSelectMode.CheckBoxRowSelect
                : GridMultiSelectMode.RowSelect);


        if (setup.GroupBy.Length > 0)
        {
            foreach (string colname in setup.GroupBy)
            {
                GridColumn? column = view.Columns.FirstOrDefault(col => col.FieldName == colname);

                if (column != null && column.Visible)
                {
                    view.GroupCount += 1;
                    GridColumnSortInfo sortinfo = new GridColumnSortInfo(column, ColumnSortOrder.Ascending);
                    view.SortInfo.Add(sortinfo);
                }
            }
        }

        if (setup.SortOn.IsEmpty() == false)
        {
            GridColumn? column = view.Columns.FirstOrDefault(col => col.FieldName == setup.SortOn);

            if (column != null)
            {
                ColumnSortOrder order = setup.SortOrder == eOrderMode.Ascending ? ColumnSortOrder.Ascending : ColumnSortOrder.Descending;
                column.SortOrder = order;
                view.SortInfo.Add(new GridColumnSortInfo(column, order));
            }
        }

        if (setup.GridCellStyler != null)
        {
            view.Tag ??= new AFGridViewState(Guid.Empty) { KeyName = view is AdvBandedGridView || view is BandedGridView ? "bvw" : "gvw" };
            
            if (view.Tag is not AFGridViewState) throwNotStateException();

            ((AFGridViewState)view.Tag).RowCellStyler = setup.GridCellStyler;

            view.RowCellStyle += customStyleCell;
        }

        if (setup.GridRowStyler != null)
        {
            view.Tag ??= new AFGridViewState(Guid.Empty) { KeyName = view is AdvBandedGridView || view is BandedGridView ? "bvw" : "gvw" };
            
            if (view.Tag is not AFGridViewState) throwNotStateException();

            ((AFGridViewState)view.Tag).RowStyler = setup.GridRowStyler;

            view.RowStyle += customStyleRow;
        }

        if (setup.DisplayTextStyler != null)
        {
            view.Tag ??= new AFGridViewState(Guid.Empty) { KeyName = view is AdvBandedGridView || view is BandedGridView ? "bvw" : "gvw" };
            
            if (view.Tag is not AFGridViewState) throwNotStateException();

            ((AFGridViewState)view.Tag).DisplayTextStyler = setup.DisplayTextStyler;

            view.CustomColumnDisplayText += customDisplayText;
        }

        if (setup.CellPainter != null)
        {
            view.Tag ??= new AFGridViewState(Guid.Empty) { KeyName = view is AdvBandedGridView || view is BandedGridView ? "bvw" : "gvw" };
            
            if (view.Tag is not AFGridViewState) throwNotStateException();

            ((AFGridViewState)view.Tag).CellPainter = setup.CellPainter;

            view.CustomDrawCell += customDrawCell;
        }

        view.EndInit();

        if (view.Tag is not AFGridViewState state) return;

        state.DefaultLayout = view.GetLayout();
    }

    private static void customDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
    {
        if (sender is not ColumnView) throwNotColumnViewException(sender);

        if (((ColumnView)sender).Tag is not AFGridViewState) throwNotStateException();

        if (((AFGridViewState)((ColumnView)sender).Tag).DisplayTextStyler == null) throw new ArgumentException($"Dem State-Objekt wurde kein DisplayTextStyler zugewiesen.");

        (((AFGridViewState)((ColumnView)sender).Tag).DisplayTextStyler)?.Invoke(e, ((ColumnView)sender).GetRow(((ColumnView)sender).GetRowHandle(e.ListSourceRowIndex)));

    }

    private static void throwNotStateException()
    {
        throw new ArgumentException($"Die Eigenschaft Tag des GridView muss ein AFGridViewState-Objekt sein.");
    }

    private static void throwNotColumnViewException(object sender)
    {
        throw new ArgumentException($"Ereignisquelle (sender) muss ein DevExpress.XtraGrid.Views.Base.ColumnView sein, ist aber  {sender.GetType().FullName}.");
    }

    private static void customDrawCell(object sender, RowCellCustomDrawEventArgs e)
    {
        if (sender is not ColumnView) throwNotColumnViewException(sender);

        if (((ColumnView)sender).Tag is not AFGridViewState) throwNotStateException();

        if (((AFGridViewState)((ColumnView)sender).Tag).CellPainter == null) throw new ArgumentException($"Dem State-Objekt wurde kein CellPainter zugewiesen.");

        (((AFGridViewState)((ColumnView)sender).Tag).CellPainter)?.Invoke(e, ((ColumnView)sender).GetRow(e.RowHandle));
    }

    private static void customStyleRow(object sender, RowStyleEventArgs e)
    {
        if (e.RowHandle < 0) return;

        if (sender is not ColumnView) throwNotColumnViewException(sender);

        if (((ColumnView)sender).Tag is not AFGridViewState) throwNotStateException();

        if (((AFGridViewState)((ColumnView)sender).Tag).RowStyler == null) throw new ArgumentException($"Dem State-Objekt wurde kein RowStyler zugewiesen.");

        (((AFGridViewState)((ColumnView)sender).Tag).RowStyler)?.Invoke(e, ((ColumnView)sender).GetRow(e.RowHandle), (ColumnView)sender);
    }



    private static void customStyleCell(object sender, RowCellStyleEventArgs e)
    {
        if (sender is not ColumnView) throwNotColumnViewException(sender);

        if (((ColumnView)sender).Tag is not AFGridViewState) throwNotStateException();

        if (((AFGridViewState)((ColumnView)sender).Tag).RowCellStyler == null) throw new ArgumentException($"Dem State-Objekt wurde kein RowCellStyler zugewiesen.");

        (((AFGridViewState)((ColumnView)sender).Tag).RowCellStyler)?.Invoke(e, ((ColumnView)sender).GetRow(e.RowHandle));
    }

    /// <summary>
    /// Liefert das aktuelle Layout des GridViews
    /// </summary>
    /// <param name="view">GridView</param>
    /// <returns>byte[] mit dem gesicherten Layout</returns>
    public static byte[] GetLayout(this GridView view)
    {
        using MemoryStream stream = new();
        view.OptionsLayout.StoreAllOptions = false;
        view.OptionsLayout.StoreDataSettings = true;
        view.OptionsLayout.StoreFormatRules = true;
        view.OptionsLayout.StoreAppearance = true;
        view.OptionsLayout.StoreVisualOptions = true;
        view.SaveLayoutToStream(stream);
        stream.Seek(0, SeekOrigin.Begin);
        return stream.ToArray();
    }

    private static void invokeCommand(object? sender, ButtonPressedEventArgs e)
    {
        if (e.Button.Tag is Tuple<AFCommand?, GridView> cmd)
        {
            CommandArgs args = new CommandArgs
            {
                Page = cmd.Item2.GridControl.GetParentControl<IViewPage>(),
                ParentControl = cmd.Item2.GridControl.Parent,
                CommandSource = cmd.Item2,
                CommandContext = eCommandContext.GridContext,
                Model = cmd.Item2.GetFocusedRow() as IModel,
                SelectedModels = cmd.Item2.GetAllSelectedRows() as IModel[]
            };

            var editor = cmd.Item2.GridControl.GetParentControl<IEditor>();

            if (editor != null)
                args.Editor = editor;

            var result = cmd.Item1?.Execute(args);

            if (result != null)
                cmd.Item2.GridControl.FindForm()?.HandleResult(result);
        }
        else if (e.Button.Tag is Tuple<Action<object>?, GridView> action)
            action.Item1?.Invoke(action.Item2);
    }

    private static void checkDefaultEditors(GridControl grid)
    {
        RepositoryItemButtonEdit? buttons = grid.RepositoryItems.OfType<RepositoryItemButtonEdit>()
            .FirstOrDefault(ctrl => ctrl.Name == @"_editorButtons");

        if (buttons == null)
        {
            buttons = new RepositoryItemButtonEdit
            {
                Name = @"_editorButtons",
                TextEditStyle = TextEditStyles.HideTextEditor,
                AllowGlyphSkinning = DefaultBoolean.True
            };

            buttons.Buttons.Clear();
            buttons.AllowFocused = false;
            buttons.ButtonClick += invokeCommand;

            EditorButton btn = new()
                { Caption = @"Goto", IsLeft = false, Kind = ButtonPredefines.Glyph, ToolTip = WinFormsStrings.LBL_GOTO };

            btn.ImageOptions.SvgImage = UI.GetImage(Symbol.GoTo);
            btn.ImageOptions.SvgImageSize = new Size(16, 16);
            btn.ImageOptions.AllowGlyphSkinning = DefaultBoolean.True;
            buttons.Buttons.Add(btn);

            btn = new EditorButton
                { Caption = @"Edit", IsLeft = false, Kind = ButtonPredefines.Glyph, ToolTip = WinFormsStrings.LBL_EDIT };
            btn.ImageOptions.SvgImage = UI.GetImage(Symbol.Edit);
            btn.ImageOptions.SvgImageSize = new Size(16, 16);
            btn.ImageOptions.AllowGlyphSkinning = DefaultBoolean.True;
            buttons.Buttons.Add(btn);

            btn = new EditorButton
                { Caption = @"Delete", IsLeft = false, Kind = ButtonPredefines.Glyph, ToolTip = WinFormsStrings.LBL_DELETE };
            btn.ImageOptions.SvgImage = UI.GetImage(Symbol.Delete);
            btn.ImageOptions.SvgImageSize = new Size(16, 16);
            btn.ImageOptions.AllowGlyphSkinning = DefaultBoolean.True;
            buttons.Buttons.Add(btn);

            btn = new EditorButton
                { Caption = @"Details", IsLeft = false, Kind = ButtonPredefines.Glyph, ToolTip = WinFormsStrings.LBL_SHOWDETAILS };
            btn.ImageOptions.SvgImage = UI.GetImage(Symbol.Search);
            btn.ImageOptions.SvgImageSize = new Size(16, 16);
            btn.ImageOptions.AllowGlyphSkinning = DefaultBoolean.True;
            buttons.Buttons.Add(btn);

            grid.RepositoryItems.Add(buttons);
        }

        RepositoryItemImageComboBox? symboledit = grid.RepositoryItems.OfType<RepositoryItemImageComboBox>()
            .FirstOrDefault(ctrl => ctrl.Name == @"_editorSymbol");

        if (symboledit == null)
        {
            symboledit = new RepositoryItemImageComboBox
            {
                Name = @"_editorSymbol",
                AllowGlyphSkinning = DefaultBoolean.True,
                GlyphAlignment = HorzAlignment.Center
            };

            grid.RepositoryItems.Add(symboledit);
        }

        RepositoryItemTimeEdit? timeedit = grid.RepositoryItems.OfType<RepositoryItemTimeEdit>()
            .FirstOrDefault(ctrl => ctrl.Name == @"_editorTime");

        if (timeedit == null)
        {
            timeedit = new RepositoryItemTimeEdit { Name = @"_editorTime", NullText = "" };
            timeedit.DisplayFormat.FormatType = FormatType.DateTime;
            timeedit.DisplayFormat.FormatString = @"t";
            timeedit.EditFormat.FormatType = timeedit.DisplayFormat.FormatType;
            timeedit.EditFormat.FormatString = timeedit.DisplayFormat.FormatString;
            timeedit.TimeEditStyle = TimeEditStyle.SpinButtons;
            grid.RepositoryItems.Add(timeedit);
        }

        RepositoryItemTimeEdit? timeonlyedit = grid.RepositoryItems.OfType<RepositoryItemTimeEdit>()
            .FirstOrDefault(ctrl => ctrl.Name == @"_editorTimeOnly");

        if (timeonlyedit == null)
        {
            timeonlyedit = new RepositoryItemTimeEdit { Name = @"_editorTimeOnly", NullText = "" };
            timeonlyedit.DisplayFormat.FormatType = FormatType.DateTime;
            timeonlyedit.DisplayFormat.FormatString = @"t";
            timeonlyedit.EditFormat.FormatType = timeedit.DisplayFormat.FormatType;
            timeonlyedit.EditFormat.FormatString = timeedit.DisplayFormat.FormatString;
            timeonlyedit.TimeEditStyle = TimeEditStyle.SpinButtons;
            grid.RepositoryItems.Add(timeonlyedit);
        }

        RepositoryItemDateEdit? dateedit = grid.RepositoryItems.OfType<RepositoryItemDateEdit>()
            .FirstOrDefault(ctrl => ctrl.Name == @"_editorDate");

        if (dateedit == null)
        {
            dateedit = new RepositoryItemDateEdit
                { Name = @"_editorDate", NullDate = DateTime.MinValue, NullText = "" };
            dateedit.DisplayFormat.FormatType = FormatType.DateTime;
            dateedit.DisplayFormat.FormatString = @"d";
            dateedit.EditFormat.FormatType = dateedit.DisplayFormat.FormatType;
            dateedit.EditFormat.FormatString = dateedit.DisplayFormat.FormatString;
            grid.RepositoryItems.Add(dateedit);
        }

        RepositoryItemDateEdit? dateonlyedit = grid.RepositoryItems.OfType<RepositoryItemDateEdit>()
            .FirstOrDefault(ctrl => ctrl.Name == @"_editorDateOnly");

        if (dateonlyedit == null)
        {
            dateonlyedit = new RepositoryItemDateEdit
                { Name = @"_editorDateOnly", NullDate = DateTime.MinValue, NullText = "" };
            dateonlyedit.DisplayFormat.FormatType = FormatType.DateTime;
            dateonlyedit.DisplayFormat.FormatString = @"d";
            dateonlyedit.EditFormat.FormatType = dateedit.DisplayFormat.FormatType;
            dateonlyedit.EditFormat.FormatString = dateedit.DisplayFormat.FormatString;
            grid.RepositoryItems.Add(dateonlyedit);
        }

        RepositoryItemMemoEdit? memoedit = grid.RepositoryItems.OfType<RepositoryItemMemoEdit>()
            .FirstOrDefault(ctrl => ctrl.Name == @"_editorMemo");

        if (memoedit == null)
        {
            memoedit = new RepositoryItemMemoEdit
            {
                Name = @"_editorMemo",
                WordWrap = true,
                AutoHeight = true
            };
            grid.RepositoryItems.Add(memoedit);
        }

        RepositoryItemSpinEdit? calcedit = grid.RepositoryItems.OfType<RepositoryItemSpinEdit>()
            .FirstOrDefault(ctrl => ctrl.Name == @"_editorCalc");

        if (calcedit == null)
        {
            calcedit = new RepositoryItemSpinEdit
            {
                Name = @"_editorCalc",
                AutoHeight = true
            };
            calcedit.DisplayFormat.FormatType = FormatType.Numeric;
            calcedit.DisplayFormat.FormatString = @"c2";
            calcedit.EditFormat.FormatType = FormatType.Numeric;
            calcedit.EditFormat.FormatString = @"c2";
            calcedit.IsFloatValue = true;
            calcedit.Increment = 0.01m;
            grid.RepositoryItems.Add(calcedit);
        }

        RepositoryItemSpinEdit? prozedit = grid.RepositoryItems.OfType<RepositoryItemSpinEdit>()
            .FirstOrDefault(ctrl => ctrl.Name == @"_editorProz");

        if (prozedit == null)
        {
            prozedit = new RepositoryItemSpinEdit
            {
                Name = @"_editorProz",
                AutoHeight = true
            };
            prozedit.DisplayFormat.FormatType = FormatType.Numeric;
            prozedit.DisplayFormat.FormatString = @"p2";
            prozedit.EditFormat.FormatType = FormatType.Numeric;
            prozedit.EditFormat.FormatString = @"p2";
            prozedit.Increment = 0.01m;
            prozedit.IsFloatValue = true;
            grid.RepositoryItems.Add(prozedit);
        }

        RepositoryItemSpinEdit? spinedit = grid.RepositoryItems.OfType<RepositoryItemSpinEdit>()
            .FirstOrDefault(ctrl => ctrl.Name == @"_editorSpin");

        if (spinedit == null)
        {
            spinedit = new RepositoryItemSpinEdit
            {
                Name = @"_editorSpin",
                AutoHeight = true
            };
            spinedit.DisplayFormat.FormatType = FormatType.Numeric;
            spinedit.DisplayFormat.FormatString = @"f0";
            spinedit.EditFormat.FormatType = FormatType.Numeric;
            spinedit.EditFormat.FormatString = @"f0";
            spinedit.Increment = 1.0m;
            spinedit.IsFloatValue = false;
            grid.RepositoryItems.Add(spinedit);
        }
    }

    private static void createBand(BandedGridView view, GridBandSettings band, bool allowedit, GridBand? parent = null)
    {
        GridBand newBand = parent != null ? parent.CreateBand(band.Caption) : view.CreateBand(band.Caption);

        foreach (GridBandSettings subband in band.Bands)
            createBand(view, subband, allowedit, newBand);

        if (band.Columns.Any(c => c.ColumnIndex > 0)) // prüfen ob die Reihenfolge mindestens einer Spalte vorgegeben ist
        {
            foreach (AFGridColumn column in band.Columns.OrderBy(col => col.ColumnIndex))
                CreateColumn(newBand, column, allowedit);
        }
        else
        {
            foreach (AFGridColumn column in band.Columns) // Reihenfolge in der Definition benutzen
                CreateColumn(newBand, column, allowedit);
        }
    }

    /// <summary>
    /// Add a new band to the gridview
    /// </summary>
    /// <param name="view">gridview to which the new band should be added</param>
    /// <param name="caption">caption for the band (default: empty string)</param>
    /// <returns>the new created band</returns>
    public static GridBand CreateBand(this BandedGridView view, string caption = "")
    {
        var band = new GridBand { Caption = caption };
        view.Bands.Add(band);
        return band;
    }

    /// <summary>
    /// Add a new band to the band (as subband)
    /// </summary>
    /// <param name="parentBand">band to which the new band should be added</param>
    /// <param name="caption">caption for the band (default: empty string)</param>
    /// <returns>the new created band</returns>
    public static GridBand CreateBand(this GridBand parentBand, string caption = "")
    {
        var band = new GridBand { Caption = caption };
        parentBand.Children.Add(band);
        return band;
    }

    /// <summary>
    /// Add a new column to the gridview
    /// </summary>
    /// <param name="view">gridview to which the column should be added</param>
    /// <param name="columndefinition">definition of the column</param>
    /// <param name="allowedit">edit mode on/off</param>
    /// <returns>the new created column</returns>
    public static GridColumn CreateColumn(this GridView view, AFGridColumn columndefinition, bool allowedit)
    {
        GridColumn column = new GridColumn();
        setupColumn(view, column, columndefinition, allowedit);

        if (columndefinition.Visible)
            column.VisibleIndex = view.Columns.Count;
        else
            column.VisibleIndex = -1;

        view.Columns.Add(column);

        return column;
    }




    /// <summary>
    /// Add a new column to the band
    /// </summary>
    /// <param name="parentBand">band to which the column should be added</param>
    /// <param name="columndefinition">definition of the column</param>
    /// <param name="allowedit">edit mode on/off</param>
    /// <returns>the new created column</returns>
    public static GridColumn CreateColumn(this GridBand parentBand, AFGridColumn columndefinition, bool allowedit)
    {
        BandedGridColumn column = new BandedGridColumn();
        setupColumn(parentBand.View, column, columndefinition, allowedit);

        if (columndefinition.Visible)
            column.VisibleIndex = parentBand.Columns.Count;

        parentBand.View.Columns.Add(column);
        parentBand.Columns.Add(column);

        return column;
    }



    private static void setupColumn(GridView view, GridColumn column, AFGridColumn columndefinition, bool allowedit)
    {
        column.Caption = columndefinition.Caption;
        column.FieldName = (columndefinition.ColumnProperty != null
            ? columndefinition.ColumnProperty.Name
            : columndefinition.ColumnFieldname ?? columndefinition.Caption);
        column.Name = "col" + column.FieldName;

        if (columndefinition.Width > 0)
            column.Width = columndefinition.Width;

        if (columndefinition.FixedWidth)
        {
            column.MinWidth = columndefinition.Width;
            column.MaxWidth = columndefinition.Width;
            column.OptionsColumn.FixedWidth = true;
            column.OptionsColumn.AllowSize = false;
        }

        column.OptionsColumn.AllowMove = columndefinition.AllowMove;
        column.OptionsColumn.AllowEdit = columndefinition.AllowEdit && allowedit;
        column.OptionsColumn.AllowSort = (columndefinition.AllowSort ? DefaultBoolean.True : DefaultBoolean.False);

        column.AppearanceCell.FontStyleDelta = (columndefinition.Bold ? FontStyle.Bold : FontStyle.Regular);
        column.AppearanceCell.Options.UseFont = columndefinition.Bold;

        if (!columndefinition.Visible)
            column.Visible = false;

        if (columndefinition.ShowAsSymbol)
        {
            RepositoryItemImageComboBox? symboledit = view.GridControl.RepositoryItems.OfType<RepositoryItemImageComboBox>().FirstOrDefault(ctrl => ctrl.Name == @"_editorSymbol");

            if (symboledit == null)
                throw new NullReferenceException(@"Symbol-Editor nicht gefunden.");

            column.ColumnEdit = symboledit;
            column.ShowButtonMode = ShowButtonModeEnum.ShowOnlyInEditor;
            column.AllowSummaryMenu = false;
            column.ColumnEdit.AutoHeight = false;
            column.MinWidth = UI.GetScaled(Math.Max(28, columndefinition.Width));
            column.MaxWidth = column.MinWidth;

            // column.ImageIndex = colDesc.GridStyle.ImageIndex;

            if (column.ImageIndex >= 0)
            {
                column.Caption = "";
                column.ImageAlignment = StringAlignment.Center;
            }
        }

        if (columndefinition.CustomEditor != null)
        {
            RepositoryItem? editor = (view.GridControl.RepositoryItems.OfType<RepositoryItem>()
                .FirstOrDefault(i => i.GetType() == columndefinition.CustomEditor));

            if (editor == null)
            {
                editor = (RepositoryItem)Activator.CreateInstance(columndefinition.CustomEditor)!;
                view.GridControl.RepositoryItems.Add(editor);
            }

            column.ColumnEdit = editor;
            column.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
        }

        string? dispformat = columndefinition.DisplayFormat;

        Type? columnType = columndefinition.ColumnProperty != null ? ((PropertyInfo)columndefinition.ColumnProperty).PropertyType : columndefinition.ColumnType;

        if (columnType != null)
        {
            if (columnType == typeof(DateTime))
            {
                if (dispformat != null && dispformat.IsNotEmpty() &&
                    (dispformat.ToUpper().TrimStart().StartsWith(@"HH") ||
                     dispformat.ToUpper().Trim() == @"T"))
                {
                    RepositoryItemTimeEdit? timeedit = view.GridControl.RepositoryItems.OfType<RepositoryItemTimeEdit>()
                        .FirstOrDefault(ctrl => ctrl.Name == @"_editorTime");

                    if (timeedit == null)
                        throw new NullReferenceException(@"Time-Editor nicht gefunden.");

                    timeedit.DisplayFormat.FormatString = dispformat;
                    timeedit.DisplayFormat.FormatType = FormatType.DateTime;
                    timeedit.EditFormat.FormatType = timeedit.DisplayFormat.FormatType;
                    timeedit.EditFormat.FormatString = timeedit.DisplayFormat.FormatString;

                    column.ColumnEdit = timeedit;
                }
                else
                {
                    RepositoryItemDateEdit? dateedit = view.GridControl.RepositoryItems.OfType<RepositoryItemDateEdit>()
                        .FirstOrDefault(ctrl => ctrl.Name == @"_editorDate");

                    if (dateedit == null)
                        throw new NullReferenceException(@"Date-Editor nicht gefunden.");

                    dateedit.DisplayFormat.FormatString = (dispformat.IsEmpty() ? @"d" : dispformat);
                    dateedit.DisplayFormat.FormatType = FormatType.DateTime;
                    dateedit.EditFormat.FormatType = dateedit.DisplayFormat.FormatType;
                    dateedit.EditFormat.FormatString = dateedit.DisplayFormat.FormatString;

                    column.ColumnEdit = dateedit;
                }
            }

            if (columnType == typeof(DateOnly))
            {
                RepositoryItemDateEdit? dateedit = view.GridControl.RepositoryItems.OfType<RepositoryItemDateEdit>()
                    .FirstOrDefault(ctrl => ctrl.Name == @"_editorDateOnly");

                if (dateedit == null)
                    throw new NullReferenceException(@"DateOnly-Editor nicht gefunden.");

                dateedit.DisplayFormat.FormatString = (dispformat.IsEmpty() ? @"d" : dispformat);
                dateedit.DisplayFormat.FormatType = FormatType.DateTime;
                dateedit.EditFormat.FormatType = dateedit.DisplayFormat.FormatType;
                dateedit.EditFormat.FormatString = dateedit.DisplayFormat.FormatString;

                column.ColumnEdit = dateedit;
            }

            if (columnType == typeof(TimeOnly))
            {
                RepositoryItemTimeEdit? timeedit = view.GridControl.RepositoryItems.OfType<RepositoryItemTimeEdit>()
                    .FirstOrDefault(ctrl => ctrl.Name == @"_editorTimeOnly");

                if (timeedit == null)
                    throw new NullReferenceException(@"TimeOnly-Editor nicht gefunden.");

                timeedit.DisplayFormat.FormatString = dispformat;
                timeedit.DisplayFormat.FormatType = FormatType.DateTime;
                timeedit.EditFormat.FormatType = timeedit.DisplayFormat.FormatType;
                timeedit.EditFormat.FormatString = timeedit.DisplayFormat.FormatString;

                column.ColumnEdit = timeedit;
            }

            if (columnType == typeof(string) && dispformat != null && dispformat.ToLower() == @"memo")
            {

                RepositoryItemMemoEdit? memoedit = view.GridControl.RepositoryItems.OfType<RepositoryItemMemoEdit>()
                    .FirstOrDefault(ctrl => ctrl.Name == @"_editorMemo");

                if (memoedit == null)
                    throw new NullReferenceException(@"Memo-Editor nicht gefunden.");

                column.ColumnEdit = memoedit;
            }

            if (columnType.IsNumericType() && allowedit && columndefinition.AllowEdit)
            {
                if (columndefinition.DisplayFormat?.ToLower() == @"c2")
                {
                    RepositoryItemSpinEdit? calcedit = view.GridControl.RepositoryItems.OfType<RepositoryItemSpinEdit>()
                        .FirstOrDefault(ctrl => ctrl.Name == @"_editorCalc");

                    if (calcedit == null)
                        throw new NullReferenceException(@"Calc-Editor nicht gefunden.");

                    column.ColumnEdit = calcedit;
                }
                else if (columndefinition.DisplayFormat?.ToLower() == @"p2")
                {
                    RepositoryItemSpinEdit? calcedit = view.GridControl.RepositoryItems.OfType<RepositoryItemSpinEdit>()
                        .FirstOrDefault(ctrl => ctrl.Name == @"_editorProz");

                    if (calcedit == null)
                        throw new NullReferenceException(@"Prozent-Editor nicht gefunden.");

                    column.ColumnEdit = calcedit;
                }
                else
                {
                    RepositoryItemSpinEdit? calcedit = view.GridControl.RepositoryItems.OfType<RepositoryItemSpinEdit>()
                        .FirstOrDefault(ctrl => ctrl.Name == @"_editorSpin");

                    if (calcedit == null)
                        throw new NullReferenceException(@"Spin-Editor nicht gefunden.");

                    calcedit.DisplayFormat.FormatType = FormatType.Numeric;
                    calcedit.DisplayFormat.FormatString = (columndefinition.DisplayFormat.IsEmpty()
                        ? @"f0"
                        : columndefinition.DisplayFormat);
                    calcedit.EditFormat.FormatType = FormatType.Numeric;
                    calcedit.EditFormat.FormatString = (columndefinition.DisplayFormat.IsEmpty()
                        ? @"f0"
                        : columndefinition.DisplayFormat);

                    column.ColumnEdit = calcedit;
                }
            }
        }

        if (dispformat.IsNotEmpty())
        {
            if (dispformat == @"g")
                column.DisplayFormat.FormatType = FormatType.DateTime;
            else
                column.DisplayFormat.FormatType = FormatType.Numeric;

            column.DisplayFormat.FormatString = dispformat;
        }

        if (columndefinition.Aggregate != eAggregate.None)
        {
            view.OptionsView.ShowFooter = true;

            switch (columndefinition.Aggregate)
            {
                case eAggregate.Sum:
                    column.Summary.Add(new GridColumnSummaryItem(SummaryItemType.Sum));
                    break;
                case eAggregate.Avg:
                    column.Summary.Add(new GridColumnSummaryItem(SummaryItemType.Average));
                    break;
                case eAggregate.Max:
                    column.Summary.Add(new GridColumnSummaryItem(SummaryItemType.Max));
                    break;
                case eAggregate.Min:
                    column.Summary.Add(new GridColumnSummaryItem(SummaryItemType.Min));
                    break;
                case eAggregate.Count:
                    column.Summary.Add(new GridColumnSummaryItem(SummaryItemType.Count));
                    break;
            }

            if (column.Summary.Count > 0)
                column.Summary[0].DisplayFormat = @"{0:" + column.DisplayFormat.FormatString + @"}";
        }

        column.OptionsColumn.AllowEdit = columndefinition.AllowEdit && allowedit;
        column.ShowButtonMode = (columndefinition.AllowEdit && allowedit
            ? ShowButtonModeEnum.Default
            : ShowButtonModeEnum.ShowOnlyInEditor);
    }


    /// <summary>
    /// Aktuelles Layout des GridViews für den aktuellen Benutzer speichern.
    /// 
    /// Voraussetzung ist die Verwendung der AF-Persistance (AFCore.App.Persistance). 
    /// Es ist nicht zwingend notwendig, dass eine User-ID gesetzt ist (z.B. in Anwendungen ohne Userverwaltung/Single-User-Apps).
    /// </summary>
    /// <param name="gridview">GridView</param>
    /// <param name="extName">Name des Layouts (KeyName)</param>
    public static CommandResult SaveLayout(this GridView gridview, string? extName = null)
    {
        Guid? ident = gridview.GetGridIdentifier();

        if (!supportLayout(ident))
            return CommandResult.None;

        AFCore.App.Persistance?.Set((Guid)ident!, gridview.GetLayout(), extName: extName);

        return CommandResult.Info(Properties.Resources.LBL_LAYOUTSAVED);
    }

    /// <summary>
    /// Standard-Layout des GridViews speichern.
    /// 
    /// Voraussetzung ist die Verwendung der AF-Persistance (AFCore.App.Persistance). 
    /// In Single-User-Apps (keine User-ID) entspricht diese Methode der SaveLayout-Methode.
    /// </summary>
    /// <param name="gridview">GridView</param>
    /// <param name="extName">Name des Layouts (KeyName)</param>
    public static CommandResult SaveDefaultLayout(this GridView gridview, string? extName = null)
    {
        Guid? ident = gridview.GetGridIdentifier();

        if (!supportLayout(ident))
            return CommandResult.None;

        AFCore.App.Persistance?.Set((Guid)ident!, gridview.GetLayout(), userid: Guid.Empty, extName: extName);

        return CommandResult.Info(Properties.Resources.LBL_LAYOUTSAVED);
    }

    /// <summary>
    /// Standard-Layout des GridViews löschen/zurücksetzen.
    /// 
    /// Voraussetzung ist die Verwendung der AF-Persistance (AFCore.App.Persistance). 
    /// In Single-User-Apps (keine User-ID) entspricht diese Methode der SaveLayout-Methode.
    /// </summary>
    /// <param name="gridview">GridView</param>
    /// <param name="extName">Name des Layouts (KeyName)</param>
    public static CommandResult DeleteDefaultLayout(this GridView gridview, string? extName = null)
    {
        Guid? ident = gridview.GetGridIdentifier();

        if (!supportLayout(ident))
            return CommandResult.None;

        AFCore.App.Persistance?.Delete((Guid)ident!, userid: Guid.Empty, extName: extName);

        return CommandResult.Info(Properties.Resources.LBL_LAYOUTRESETED);
    }

    /// <summary>
    /// Standard-Layout des Benutzers löschen/zurücksetzen.
    /// 
    /// Voraussetzung ist die Verwendung der AF-Persistance (AFCore.App.Persistance). 
    /// In Single-User-Apps (keine User-ID) entspricht diese Methode der SaveLayout-Methode.
    /// </summary>
    /// <param name="gridview">GridView</param>
    /// <param name="extName">Name des Layouts (KeyName)</param>
    public static CommandResult DeleteUserLayout(this GridView gridview, string? extName = null)
    {
        Guid? ident = gridview.GetGridIdentifier();

        if (!supportLayout(ident))
            return CommandResult.None;

        AFCore.App.Persistance?.Delete((Guid)ident!, extName: extName);

        return CommandResult.Info(Properties.Resources.LBL_LAYOUTRESETED);
    }

    /// <summary>
    /// Aktuelles Layout des Benutzers unter einem bestimmten Namen speichern.
    /// 
    /// Voraussetzung ist die Verwendung der AF-Persistance (AFCore.App.Persistance). 
    /// In Single-User-Apps (keine User-ID) entspricht diese Methode der SaveLayout-Methode.
    /// </summary>
    /// <param name="gridview">GridView</param>
    /// <param name="name">Name des zu speichernden Layouts</param>
    /// <param name="extName">Name des Layouts (KeyName)</param>
    public static CommandResult SaveLayoutAs(this GridView gridview, string name, string? extName = null)
    {
        Guid? ident = gridview.GetGridIdentifier();

        if (string.IsNullOrWhiteSpace(name))
            return CommandResult.Error("Es wurde kein Name angegeben.");

        if (!supportLayout(ident))
            return CommandResult.None;

        AFCore.App.Persistance?.Set((Guid)ident!, gridview.GetLayout(), name: name, extName: extName);

        return CommandResult.Info(Properties.Resources.LBL_LAYOUTSAVED);
    }

    /// <summary>
    /// Layouts des GridViews für den aktuellen Benutzer verwalten.
    /// 
    /// Voraussetzung ist die Verwendung der AF-Persistance (AFCore.App.Persistance). 
    /// Es ist nicht zwingend notendig, dass eine User-ID gesetzt ist (z.B. in Anwendungen ohne Userverwaltung/Single-User-Apps).
    /// </summary>
    /// <param name="gridview">GridView</param>
    /// <param name="extName">Name des Layouts (KeyName)</param>
    public static CommandResult ManageLayouts(this GridView gridview, string? extName = null)
    {
        Guid? ident = gridview.GetGridIdentifier();

        if (ident == null || !supportLayout(ident))
            return CommandResult.None;

        using var dlg = new FormLayoutManager("Layouts der Ansicht verwalten", (Guid)ident, extName);

        if (dlg.ShowDialog(gridview.GridControl.FindForm()) == DialogResult.OK && dlg.SelectedLayout != null)
        {
            return gridview.LoadLayout((Guid)dlg.SelectedLayout);
        }

        return CommandResult.None;
    }


    /// <summary>
    /// Layout des GridViews mit einer bestimmten ID (SYS_ID) laden.
    /// 
    /// Voraussetzung ist die Verwendung der AF-Persistance (AFCore.App.Persistance). 
    /// Es ist nicht zwingend notwendig, dass eine User-ID gesetzt ist (z.B. in Anwendungen ohne Userverwaltung/Single-User-Apps).
    /// </summary>
    /// <param name="gridview">GridView</param>
    /// <param name="id">ID des Layouts (SYS_ID)</param>
    public static CommandResult LoadLayout(this GridView gridview, Guid id)
    {
        Guid? ident = gridview.GetGridIdentifier();

        if (!supportLayout(ident))
            return CommandResult.None;

        byte[]? data = AFCore.App.Persistance?.GetByID(id);

        if (data == null)
            return CommandResult.None;

        gridview.RestoreLayout(data);

        return CommandResult.Info(Properties.Resources.LBL_LAYOUTLOADED);
    }

    /// <summary>
    /// Liefert die gespeicherten Layouts eines Benutzers als Dictionary.
    /// </summary>
    /// <param name="gridview">GridView</param>
    /// <param name="extName">Erweiterungsname</param>
    /// <returns>Dictionary mit den Layouts</returns>
    public static Dictionary<Guid, string> GetAvailableLayouts(this GridView gridview, string? extName = null)
    {
        Guid? ident = gridview.GetGridIdentifier();
        if (ident == null) return [];

        return AFCore.App.Persistance?.GetNamedValues((Guid)ident, extName: extName) ?? [];
    }

    /// <summary>
    /// Layout des GridViews für den aktuellen Benutzer laden.
    /// 
    /// Voraussetzung ist die Verwendung der AF-Persistance (AFCore.App.Persistance). 
    /// Es ist nicht zwingend notwendig, dass eine User-ID gesetzt ist (z.B. in Anwendungen ohne Userverwaltung/Single-User-Apps).
    /// </summary>
    /// <param name="gridview">GridView</param>
    /// <param name="name">Name des zu ladenden Layouts</param>
    /// <param name="extName">Name des Layouts (KeyName)</param>
    public static CommandResult LoadLayout(this GridView gridview, string? name = null, string? extName = null)
    {
        Guid? ident = gridview.GetGridIdentifier();

        if (!supportLayout(ident))
            return CommandResult.None;

        byte[]? data = AFCore.App.Persistance?.Get((Guid)ident!, name: name, extName: extName);

        if (data == null)
            return CommandResult.None;

        gridview.RestoreLayout(data);

        return CommandResult.Info(Properties.Resources.LBL_LAYOUTLOADED);
    }

    /// <summary>
    /// Standard-Layout des GridViews für den aktuellen Benutzer laden.
    /// 
    /// Voraussetzung ist die Verwendung der AF-Persistance (AFCore.App.Persistance). 
    /// In Single-User-Apps (keine User-ID) entspricht diese Methode der LoadLayout-Methode.
    /// </summary>
    /// <param name="gridview">GridView</param>
    /// <param name="extName">Name des Layouts (KeyName)</param>
    public static CommandResult LoadDefaultLayout(this GridView gridview, string? extName = null)
    {
        if (gridview.GetGridIdentifier() is not { } id || id == Guid.Empty) return CommandResult.None;

        var data = AFCore.App.Persistance?.Get(id, extName: extName);

        if (data != null)
            gridview.RestoreLayout(data);
        else
        {
            if (gridview.Tag is not AFGridViewState state || state.DefaultLayout == null) return CommandResult.None;

            RestoreLayout(gridview, state.DefaultLayout);
        }

        return CommandResult.Info(Properties.Resources.LBL_LAYOUTLOADED);
    }


    /// <summary>
    /// Standardlayout des Grids laden (ignoriert alle persistierten Layouts)
    /// 
    /// Voraussetzung ist die Verwendung der AF-Persistance (AFCore.App.Persistance). 
    /// Es ist nicht zwingend notwendig, dass eine User-ID gesetzt ist (z.B. in Anwendungen ohne Userverwaltung/Single-User-Apps).
    /// </summary>
    /// <param name="gridview">GridView</param>
    public static CommandResult ResetLayout(this GridView gridview)
    {
        if (gridview.Tag is not AFGridViewState state) return CommandResult.None;

        RestoreLayout(gridview, state.DefaultLayout);

        return CommandResult.Info(Properties.Resources.LBL_LAYOUTRESETED);
    }

    /// <summary>
    /// Layout wiederherstellen
    /// </summary>
    /// <param name="gridview">GridView</param>
    /// <param name="layout">Layout</param>
    public static void RestoreLayout(this GridView gridview, byte[]? layout)
    {
        if (layout == null || layout.Length == 0)
            return;

        using MemoryStream stream = new(layout);

        try
        {
            stream.Seek(0, SeekOrigin.Begin);
            gridview.RestoreLayoutFromStream(stream);
        }
#if (DEBUG)
    // ReSharper disable once RedundantCatchClause
    catch
    {
        throw;
    }
#else
        // ReSharper disable once EmptyGeneralCatchClause
        catch
        {
        }
#endif
    }

    private static bool supportLayout(Guid? ident)
    {
        if (ident == null || ident.Equals(Guid.Empty))
            return false;

        // prüfen ob Speicherung überhaupt möglich ist...
        if (AFCore.App.Persistance == null || AFCore.App.Persistance.IsAvailable == false)
            return false;

        return true;
    }
}