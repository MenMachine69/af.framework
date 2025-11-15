using AF.MVC;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;

namespace AF.WINFORMS.DX;

/// <summary>
/// Node in einem TreeView-Control
/// </summary>
public class AFTreeListNode
{
    private List<AFTreeListNode>? childNodes;

    /// <summary>
    /// Text/ANzeige
    /// </summary>
    public string Caption { get; set; } = "";
    /// <summary>
    /// Index des Bildes
    /// </summary>
    public int ImageIndex { get; set; } = -1;
    /// <summary>
    /// Index des Bildes, wenn der Knoten aufgeklappt ist
    /// </summary>
    public int ImageIndexSelected { get; set; } = -1;
    /// <summary>
    /// Tag: beleibiges Objekt
    /// </summary>
    public object? Tag { get; set; }
        
    /// <summary>
    /// Kindknoten...
    /// </summary>
    public List<AFTreeListNode> ChildNodes => childNodes ??= [];

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return Caption;
    }

}

/// <summary>
/// Erweiterungsmethoden für TreeList
/// </summary>
public static class TreeListEx
{
    /// <summary>
    /// Erzeugt die Knoten des Trees
    /// </summary>
    /// <param name="nodes">Liste der Nodes</param>
    /// <param name="view">TreeList</param>
    public static void Fill(this TreeList view, List<AFTreeListNode> nodes)
    {
        view.Nodes.Clear();
        view.BeginUnboundLoad();
        view.loadNodes(null, nodes);
        view.EndUnboundLoad();
    }

    private static void loadNodes(this TreeList view, DevExpress.XtraTreeList.Nodes.TreeListNode? parentnode, List<AFTreeListNode> nodes)
    {
        if (view.Columns.Count == 0)
        {
            view.OptionsView.ShowColumns = false;
            view.OptionsView.ShowIndicator = false;
            view.OptionsView.ShowHorzLines = false;
            view.OptionsView.ShowVertLines = false;
            view.OptionsSelection.EnableAppearanceFocusedCell = false;
            view.OptionsSelection.EnableAppearanceFocusedRow = true;
            view.OptionsView.FocusRectStyle = DrawFocusRectStyle.None;
            view.OptionsView.ShowIndentAsRowStyle = true;

            TreeListColumn colNode = new()
            {
                Caption = "Selection",
                FieldName = "Caption",
                Name = "node",
                Visible = true,
                MinWidth = 100,
                VisibleIndex = 0
            };

            colNode.OptionsColumn.AllowEdit = false;

            view.Columns.AddRange([colNode]);
        }

        foreach (AFTreeListNode node in nodes)
        {
            DevExpress.XtraTreeList.Nodes.TreeListNode n = view.AppendNode(new object[] { node.Caption }, parentnode, node.Tag);
            n.ImageIndex = node.ImageIndex;
            n.SelectImageIndex = node.ImageIndexSelected;
            n.Tag = node.Tag;
            view.loadNodes(n, node.ChildNodes);
        }
    }

    /// <summary>
    /// Setup the treeview to display objects of the given
    ///
    /// GridStyle is set to Full
    /// </summary>
    /// <param name="view">gridview</param>
    /// <param name="type">Type of objects</param>
    public static void Setup(this TreeList view, Type type)
    {
        IController controller = type.GetController();

        Setup(view, controller is IControllerUI uiController
            ? uiController.GetGridSetup(eGridStyle.Treeview)
            : type.GetTypeDescription().GetGridSetup(eGridStyle.Treeview));
    }

    /// <summary>
    /// Setup the treeview to display objects of the given
    /// </summary>
    /// <param name="view">gridview</param>
    /// <param name="type">Type of objects</param>
    /// <param name="style">style/content of the grid</param>
    public static void Setup(this TreeList view, Type type, eGridStyle style)
    {
        IController controller = type.GetController();

        Setup(view, controller is IControllerUI uiController
            ? uiController.GetGridSetup(style)
            : type.GetTypeDescription().GetGridSetup(style) );
    }


    /// <summary>
    /// Standardsetup für ITreeNode Listen
    /// </summary>
    public static void Setup(this TreeList view, SvgImageCollection? images = null)
    {
        view.RootValue = Guid.Empty;
        view.KeyFieldName = nameof(ITreeNode.NODE_ID);
        view.ParentFieldName = nameof(ITreeNode.NODE_PARENT_ID);
        view.StateImageList = images ?? Glyphs.GetImages();
        view.Columns.Add(new TreeListColumn()
        {
            Caption = @"Folder",
            FieldName = nameof(ITreeNode.NODE_NAME),
            Visible = true,
            Name = $@"col{nameof(ITreeNode.NODE_NAME)}",
            VisibleIndex = 0,
            OptionsColumn = { AllowEdit = false }
        });
        view.AutoFillColumn = view.Columns[0];
        view.OptionsView.FocusRectStyle = DrawFocusRectStyle.None;
        view.OptionsView.ShowColumns = false;
        view.OptionsView.ShowHorzLines = false;
        view.OptionsView.ShowIndentAsRowStyle = true;
        view.OptionsView.ShowIndicator = false;
        view.OptionsView.ShowVertLines = false;
        view.OptionsView.ShowTreeLines = DefaultBoolean.False;
        view.OptionsSelection.EnableAppearanceFocusedCell = false;
        view.OptionsView.AutoWidth = true;
        view.RowHeight = 24;
    }

    /// <summary>
    /// Identifier für das TreeView setzen
    /// </summary>
    /// <param name="view">TreeView</param>
    /// <param name="identifier">ID des GridViews</param>
    public static void SetGridIdentifier(this TreeList view, Guid identifier)
    {
        if (view.Tag is not AFGridViewState state)
            view.Tag = new AFGridViewState(identifier);
        else
            state.Identifier = identifier;
    }

    /// <summary>
    /// Identifier für das TreeView holen
    /// </summary>
    /// <param name="view">TreeView</param>
    /// <returns>ID (Guid) des GridViews (gesetzt via SetGridIdentifier oder Setup) oder NULL</returns>
    public static Guid? GetGridIdentifier(this TreeList view)
    {
        if (view.Tag is not AFGridViewState state)
            return null;

        return state.Identifier;
    }

    /// <summary>
    /// Setup the TreeView using the given setup.
    /// </summary>
    /// <param name="view">TreeView</param>
    /// <param name="setup">setup for the grid</param>
    public static void Setup(this TreeList view, AFGridSetup setup)
    {
        // wenn der aktuelle Identifier des GridViews mit dem des Setup übereinstimmt, kann das Setup übersprungen werden
        if (setup.GridIdentifier.IsNotEmpty() && view.GetGridIdentifier().Equals(setup.GridIdentifier))
            return;

        if (setup.GridIdentifier.IsNotEmpty())
            view.SetGridIdentifier(setup.GridIdentifier);

        checkDefaultEditors(view);

        view.DataSource = null;
        view.RefreshDataSource();

        view.BeginInit();

        // Reset the grid
        view.Columns.Clear();
        view.Bands.Clear();

        view.OptionsView.FocusRectStyle = setup.AllowEdit ? DrawFocusRectStyle.CellFocus : DrawFocusRectStyle.None ;
        view.RowHeight = UI.GetScaled(26);

        view.OptionsView.AllowGlyphSkinning = true;
        //view.OptionsView.EnableAppearanceEvenRow = true;
        //view.OptionsView.EnableAppearanceOddRow = true;
        view.OptionsView.ShowHorzLines = false;
        view.OptionsView.ShowVertLines = false;
        view.OptionsView.ShowIndicator = setup.AllowEdit;

        view.OptionsMenu.EnableColumnMenu = true;
        view.OptionsMenu.EnableFooterMenu = true;
        view.OptionsMenu.ShowAutoFilterRowItem = true;

        view.OptionsBehavior.Editable = setup.AllowEdit;
        view.OptionsSelection.EnableAppearanceHotTrackedRow = DefaultBoolean.True;
        view.OptionsSelection.EnableAppearanceFocusedCell = setup.AllowEdit;

        view.OptionsLayout.StoreFormatRules = DefaultBoolean.True;
        view.OptionsLayout.StoreAllOptions = true;
        
        if (setup.Bands.Count > 0)
        {
            foreach (GridBandSettings band in setup.Bands)
                createBand(view, band, setup.AllowEdit);
        }
        else
        {
            int idx = 0;
            foreach (AFGridColumn column in setup.Columns)
            {
                var treecol = CreateColumn(view, column, setup.AllowEdit);
                treecol.VisibleIndex = idx;
                view.Columns.Add(treecol);

                if (column.AutoFill)
                    view.AutoFillColumn = treecol;

                ++idx;
            }
        }

        // check if buttons needed
        if (setup.CmdDelete != null || setup.CmdEdit != null || setup.CmdGoto != null || setup.CmdShowDetail != null)
        {
            RepositoryItemButtonEdit? buttons = view.RepositoryItems.OfType<RepositoryItemButtonEdit>().FirstOrDefault(ctrl => ctrl.Name == @"_editorButtons");

            if (buttons == null)
                throw new NullReferenceException(@"EditorButtons editor not found in grid repository.");

            var button = buttons.Buttons.FirstOrDefault(btn => btn.Caption == @"Goto");

            if (button != null)
            {
                button.Tag = new Tuple<AFCommand?, TreeList>(setup.CmdGoto, view);
                button.Visible = setup.CmdGoto != null;
            }

            button = buttons.Buttons.FirstOrDefault(btn => btn.Caption == @"Edit");

            if (button != null)
            {
                button.Tag = new Tuple<AFCommand?, TreeList>(setup.CmdEdit, view);
                button.Visible = setup.CmdEdit != null;
            }

            button = buttons.Buttons.FirstOrDefault(btn => btn.Caption == @"Delete");

            if (button != null)
            {
                button.Tag = new Tuple<AFCommand?, TreeList>(setup.CmdDelete, view);
                button.Visible = setup.CmdDelete != null;
            }

            button = buttons.Buttons.FirstOrDefault(btn => btn.Caption == @"Details");

            if (button != null)
            {
                button.Tag = new Tuple<AFCommand?, TreeList>(setup.CmdShowDetail, view);
                button.Visible = setup.CmdShowDetail != null;
            }

            TreeListColumn column = new();
            column.VisibleIndex = view.Columns.Count;
            column.ColumnEdit = buttons;
            column.MinWidth = buttons.Buttons.VisibleCount * UI.GetScaled(20);
            column.MaxWidth = column.MinWidth;
            column.OptionsColumn.FixedWidth = true;
            column.OptionsColumn.AllowSize = false;
            column.OptionsColumn.AllowMove = false;
            column.OptionsColumn.AllowEdit = true;
            column.OptionsColumn.AllowSort = false;
            column.OptionsColumn.ShowInExpressionEditor = false;
            column.Name = @"_syscolButtons";
            column.OptionsFilter.AllowFilter = false;
            column.OptionsFilter.AllowAutoFilter = false;

            view.OptionsBehavior.Editable = true;
            view.PreviewIndent = column.MinWidth + 5;

            view.Columns.Add(column);

        }


        if (setup.AllowAddNew)
        {
            view.OptionsView.NewItemRowPosition = TreeListNewItemRowPosition.Bottom;
            //view.OptionsBehavior. = DefaultBoolean.True;
        }

        if (setup.PreviewField.IsNotEmpty())
        {
            view.OptionsView.ShowPreview = true;
            view.OptionsView.AutoCalcPreviewLineCount = true;
            view.PreviewFieldName = setup.PreviewField;
        }

        view.OptionsSelection.MultiSelect = (setup.SelectionMode != eSelectionMode.Single);
        view.OptionsSelection.MultiSelectMode = TreeListMultiSelectMode.RowSelect;

        view.EndInit();

        if (view.Tag is not AFGridViewState state) return;

        state.DefaultLayout = view.GetLayout();
    }

    /// <summary>
    /// Eine Spalte erzeugen
    /// </summary>
    /// <param name="view">Tree</param>
    /// <param name="columndefinition">Spaltendefinition</param>
    /// <param name="allowedit">bearbeiten erlaubt</param>
    /// <returns>Spalten-Objekt</returns>
    public static TreeListColumn CreateColumn(this TreeList view, AFGridColumn columndefinition, bool allowedit)
    {
        TreeListColumn column = new();
        setupColumn(view, column, columndefinition, allowedit);
        return column;
    }

    private static void setupColumn(TreeList view, TreeListColumn column, AFGridColumn columndefinition, bool allowedit)
    {
        column.Caption = columndefinition.Caption;
        column.FieldName = (columndefinition.ColumnProperty != null
            ? columndefinition.ColumnProperty.Name
            : columndefinition.ColumnFieldname ?? columndefinition.Caption);

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
        column.OptionsColumn.AllowSort = columndefinition.AllowSort;

        column.AppearanceCell.FontStyleDelta = (columndefinition.Bold ? FontStyle.Bold : FontStyle.Regular);
        column.AppearanceCell.Options.UseFont = columndefinition.Bold;

        if (!columndefinition.Visible)
            column.Visible = false;

        if (columndefinition.ShowAsSymbol)
        {
            RepositoryItemImageComboBox? symboledit = view.RepositoryItems.OfType<RepositoryItemImageComboBox>().FirstOrDefault(ctrl => ctrl.Name == @"_editorSymbol");

            if (symboledit == null)
                throw new NullReferenceException(@"Symbol-Editor nicht gefunden.");

            column.ColumnEdit = symboledit;
            column.ShowButtonMode = ShowButtonModeEnum.ShowOnlyInEditor;
            column.ColumnEdit.AutoHeight = false;
            column.MinWidth = UI.GetScaled(Math.Max(28, columndefinition.Width));
            column.MaxWidth = column.MinWidth;

            if (column.ImageIndex >= 0)
            {
                column.Caption = "";
                column.ImageAlignment = StringAlignment.Center;
            }
        }

        if (columndefinition.CustomEditor != null)
        {
            RepositoryItem? editor = (view.RepositoryItems.OfType<RepositoryItem>()
                .FirstOrDefault(i => i.GetType() == columndefinition.CustomEditor));

            if (editor == null)
            {
                editor = (RepositoryItem)Activator.CreateInstance(columndefinition.CustomEditor)!;
                view.RepositoryItems.Add(editor);
            }

            column.ColumnEdit = editor;
            column.ShowButtonMode = ShowButtonModeEnum.ShowOnlyInEditor;
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
                    RepositoryItemTimeEdit? timeedit = view.RepositoryItems.OfType<RepositoryItemTimeEdit>()
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
                    RepositoryItemDateEdit? dateedit = view.RepositoryItems.OfType<RepositoryItemDateEdit>()
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

            if (columnType == typeof(string) && dispformat != null && dispformat.ToLower() == @"memo")
            {

                RepositoryItemMemoEdit? memoedit = view.RepositoryItems.OfType<RepositoryItemMemoEdit>()
                    .FirstOrDefault(ctrl => ctrl.Name == @"_editorMemo");

                if (memoedit == null)
                    throw new NullReferenceException(@"Memo-Editor nicht gefunden.");

                column.ColumnEdit = memoedit;
            }

            if (columnType.IsNumericType() && allowedit && columndefinition.AllowEdit)
            {
                if (columndefinition.DisplayFormat?.ToLower() == @"c2")
                {
                    RepositoryItemSpinEdit? calcedit = view.RepositoryItems.OfType<RepositoryItemSpinEdit>()
                        .FirstOrDefault(ctrl => ctrl.Name == @"_editorCalc");

                    if (calcedit == null)
                        throw new NullReferenceException(@"Calc-Editor nicht gefunden.");

                    column.ColumnEdit = calcedit;
                }
                else if (columndefinition.DisplayFormat?.ToLower() == @"p2")
                {
                    RepositoryItemSpinEdit? calcedit = view.RepositoryItems.OfType<RepositoryItemSpinEdit>()
                        .FirstOrDefault(ctrl => ctrl.Name == @"_editorProz");

                    if (calcedit == null)
                        throw new NullReferenceException(@"Prozent-Editor nicht gefunden.");

                    column.ColumnEdit = calcedit;
                }
                else
                {
                    RepositoryItemSpinEdit? calcedit = view.RepositoryItems.OfType<RepositoryItemSpinEdit>()
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
                column.Format.FormatType = FormatType.DateTime;
            else
                column.Format.FormatType = FormatType.Numeric;

            column.Format.FormatString = dispformat;
        }

        column.OptionsColumn.AllowEdit = columndefinition.AllowEdit && allowedit;
        column.ShowButtonMode = (columndefinition.AllowEdit && allowedit
            ? ShowButtonModeEnum.Default
            : ShowButtonModeEnum.ShowOnlyInEditor);

    }

    private static void createBand(TreeList view, GridBandSettings band, bool allowedit, TreeListBand? parent = null)
    {
        TreeListBand newBand = parent != null ? parent.CreateBand(band.Caption) : view.CreateBand(band.Caption);

        foreach (GridBandSettings subband in band.Bands)
            createBand(view, subband, allowedit, newBand);

        foreach (AFGridColumn column in band.Columns.OrderBy(col => col.ColumnIndex))
            CreateColumn(newBand, column, allowedit);
    }

    /// <summary>
    /// Add a new band to the gridview
    /// </summary>
    /// <param name="view">gridview to which the new band should be added</param>
    /// <param name="caption">caption for the band (default: empty string)</param>
    /// <returns>the new created band</returns>
    public static TreeListBand CreateBand(this TreeList view, string caption = "")
    {
        var band = new TreeListBand { Caption = caption };
        view.Bands.Add(band);
        return band;
    }

    /// <summary>
    /// Add a new band to the band (as subband)
    /// </summary>
    /// <param name="parentBand">band to which the new band should be added</param>
    /// <param name="caption">caption for the band (default: empty string)</param>
    /// <returns>the new created band</returns>
    public static TreeListBand CreateBand(this TreeListBand parentBand, string caption = "")
    {
        var band = new TreeListBand { Caption = caption };
        parentBand.Bands.Add(band);
        return band;
    }

    /// <summary>
    /// Add a new column to the band
    /// </summary>
    /// <param name="parentBand">band to which the column should be added</param>
    /// <param name="columndefinition">definition of the column</param>
    /// <param name="allowedit">edit mode on/off</param>
    /// <returns>the new created column</returns>
    public static TreeListColumn CreateColumn(this TreeListBand parentBand, AFGridColumn columndefinition, bool allowedit)
    {
        TreeListColumn column = new TreeListColumn();
        setupColumn(parentBand.TreeList, column, columndefinition, allowedit);

        if (columndefinition.Visible)
            column.VisibleIndex = parentBand.Columns.Count;

        parentBand.Columns.Add(column);

        return column;
    }

    /// <summary>
    /// Liefert das aktuelle Layout des GridViews
    /// </summary>
    /// <param name="view">GridView</param>
    /// <returns>byte[] mit dem gesicherten Layout</returns>
    public static byte[] GetLayout(this TreeList view)
    {
        using MemoryStream stream = new();

        view.SaveLayoutToStream(stream);
        stream.Seek(0, SeekOrigin.Begin);
        return stream.ToArray();
    }

    private static void invokeCommand(object? sender, ButtonPressedEventArgs e)
    {

        if (e.Button.Tag is not Tuple<AFCommand?, TreeList> cmd) return;

        var result = cmd.Item1?.Execute(new CommandArgs
        {
            Page = cmd.Item2.GetParentControl<IViewPage>(),
            CommandSource = cmd.Item2,
            CommandContext = eCommandContext.GridContext,
            Model = cmd.Item2.GetFocusedRow() as IModel,
            SelectedModels = cmd.Item2.Selection.Select(node => cmd.Item2.GetRow(node.Id))?.ToArray() as IModel[]
        });

        if (result != null)
            cmd.Item2.FindForm()?.HandleResult(result);
    }

    private static void checkDefaultEditors(TreeList grid)
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


    /// <summary>
    /// Return the node at the given location
    /// </summary>
    /// <param name="location">location</param>
    /// <param name="listNode">the TreeListNode object which represents the node</param>
    /// <param name="tree">TreeList</param>
    /// <typeparam name="TNodeType">type of the node</typeparam>
    /// <returns>the node object</returns>
    [SupportedOSPlatform("windows")]
    public static TNodeType? GetNodeFromLocation<TNodeType>(this TreeList tree, Point location, out DevExpress.XtraTreeList.Nodes.TreeListNode? listNode)
    {
        TNodeType? node = default;

        var info = tree.CalcHitInfo(tree.PointToClient(location));

        if (info.Node != null)
        {
            listNode = info.Node;
            node = (TNodeType)tree.GetRow(info.Node.Id);
        }
        else
            listNode = null;

        return node;
    }

    /// <summary>
    /// Return the node at the given location
    /// </summary>
    /// <param name="location">location</param>
    /// <param name="tree">TreeList</param>
    /// <typeparam name="TNodeType">type of the node</typeparam>
    /// <returns>the node object</returns>
    [SupportedOSPlatform("windows")]
    public static TNodeType? GetNodeFromLocation<TNodeType>(this TreeList tree, Point location)
    {
        TNodeType? node = default;

        var info = tree.CalcHitInfo(tree.PointToClient(location));

        if (info.Node != null)
            node = (TNodeType)tree.GetRow(info.Node.Id);

        return node;
    }
}

