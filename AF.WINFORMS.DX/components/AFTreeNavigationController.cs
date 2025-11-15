using DevExpress.Utils.Svg;
using DevExpress.Utils.Behaviors;
using DevExpress.Utils.DragDrop;
using DevExpress.Utils.Menu;
using DevExpress.XtraGrid;
using DevExpress.XtraTreeList;
using DevExpress.XtraGrid.Views.Tile;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.TableLayout;
using DevExpress.XtraGrid.Columns;

namespace AF.WINFORMS.DX;

/// <summary>
/// Controller-Komponente für ein Navigationselement bestehend aus eienr Baumansicht (Tree) und eine Listenansicht (Grid).
/// 
/// In der Baumansicht werde Elemente dargestellt, die die Schnittstelle ITreeNode implementieren und in der 
/// Listenansicht Element, die die Schnittstelle IListElement implementieren.
/// 
/// Tree- und List-Elemente sind immer IDataObject-Objekte, die via Controller verfügbar sein müssen.
/// </summary>

[DesignerCategory("Code")]
public sealed class AFTreeNavigationController : AFVisualComponentBase
{
    private TreeListColumn? treeListColumn1;
    private TreeListColumn? treeListColumn2;
    private RepositoryItemButtonEdit? btnMenu;
    private BehaviorManager? behaviorManager1;
    private DragDropEvents? dragDropEventsTree;
    private DragDropEvents? dragDropEventsList;
    private DXPopupMenu? treeNodeMenu;
    private TreeListNode? dropNode;
    private TileView? tileview;
    private readonly Dictionary<string, ITreeNode> customFolders = [];

    /// <summary>
    /// Speziellen Ordner regisrtrieren, der nicht originär 
    /// aus der Datenquelle stammt (Favoriten, Suchergebnis etc.).
    /// </summary>
    /// <param name="name">eindeutiger Name des Ordners</param>
    /// <param name="customFolder">Definition des Ordners</param>
    public void RegisterFolder(string name, ITreeNode customFolder)
    {
        customFolders.TryAdd(name, customFolder);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TTreeElement"></typeparam>
    /// <typeparam name="TListElement"></typeparam>
    public void Setup<TTreeElement, TListElement>() where TTreeElement : class, ITreeNode, DATA.IDataObject
                                                    where TListElement : class, IListElement, DATA.IDataObject

    {
        if (typeof(TTreeElement).GetController() == null)
            throw new NullReferenceException($"Missing Controller for Type {typeof(TTreeElement)}");
    
        if (typeof(TListElement).GetController() == null)
            throw new NullReferenceException($"Missing Controller for Type {typeof(TListElement)}");

        if (Tree == null)
            throw new NullReferenceException($"No TreeList assigned to property Tree.");

        if (Grid == null)
            throw new NullReferenceException($"No GridControl assigned to property Grid.");

        // setup TreeView
        treeListColumn1 = new();
        
        if (OptionsAllowDragDrop)
        {
            behaviorManager1 = new();
            ((ISupportInitialize)behaviorManager1).BeginInit();
        }

        Tree.OptionsMenu.EnableColumnMenu = false;
        Tree.OptionsMenu.EnableFooterMenu = false;
        Tree.OptionsMenu.ShowExpandCollapseItems = false;
        Tree.OptionsSelection.EnableAppearanceFocusedCell = false;
        Tree.OptionsView.AllowGlyphSkinning = true;
        Tree.OptionsView.FocusRectStyle = DrawFocusRectStyle.None;
        Tree.OptionsView.ShowIndentAsRowStyle = true;
        Tree.OptionsView.ShowTreeLines = DefaultBoolean.False;
        Tree.RowHeight = UI.GetScaled(24);
        Tree.ViewStyle = TreeListViewStyle.TreeView;

        Tree.RootValue = Guid.Empty;
        Tree.KeyFieldName = nameof(ITreeNode.NODE_ID);
        Tree.ParentFieldName = nameof(ITreeNode.NODE_PARENT_ID);
        Tree.StateImageList = TreeImages;

        treeListColumn1.Caption = @"Folder";
        treeListColumn1.FieldName = nameof(ITreeNode.NODE_NAME);
        treeListColumn1.FilterMode = ColumnFilterMode.DisplayText;
        treeListColumn1.Name = @"colCaption";
        treeListColumn1.OptionsColumn.AllowEdit = false;
        treeListColumn1.OptionsFilter.FilterBySortField = DefaultBoolean.True;
        treeListColumn1.SortMode = ColumnSortMode.Custom;
        treeListColumn1.SortOrder = SortOrder.Ascending;
        treeListColumn1.Visible = true;
        treeListColumn1.VisibleIndex = 0;

        // Menu-Button initialisieren
        if (OptionsAllowMenu) setupMenuColumn();

        if (treeListColumn2 != null)
            Tree.Columns.AddRange([treeListColumn1, treeListColumn2]);
        else
            Tree.Columns.AddRange([treeListColumn1]);

        // DragDrop initalisieren
        if (OptionsAllowDragDrop) 
        { 
            setupDragDrop();
            ((ISupportInitialize)behaviorManager1!).EndInit();
        }

        // setup GridControl
        setupGrid();
    }

    private void setupGrid()
    {
        tileview = new TileView();

        var colCaption = new TileViewColumn()
        {
            FieldName = nameof(IListElement.Caption),
            Name = "colCaption"
        };
       
        var colImage = new TileViewColumn()
        {
            FieldName = nameof(IListElement.Image),
            Name = "colImage"
        };

        var colDescription = new TileViewColumn()
        {
            FieldName = nameof(IListElement.Description),
            Name = "colDescription"
        };

        tileview.Columns.AddRange([colCaption, colImage, colDescription]);
        tileview.GridControl = Grid!;
        tileview.Name = "tileView";
        tileview.OptionsBehavior.AutoPopulateColumns = false;
        tileview.OptionsTiles.GroupTextPadding = new Padding(12, 8, 12, 8);
        tileview.OptionsTiles.IndentBetweenGroups = 0;
        tileview.OptionsTiles.IndentBetweenItems = 0;
        tileview.OptionsTiles.ItemSize = new Size(334, 76);
        tileview.OptionsTiles.LayoutMode = TileViewLayoutMode.List;
        tileview.OptionsTiles.Orientation = Orientation.Vertical;
        tileview.OptionsTiles.Padding = new Padding(0);
        tileview.OptionsTiles.RowCount = 0;

        tileview.TileColumns.Add(new()
        {
            Length =
            {
                Type = TableDefinitionLengthType.Pixel,
                Value = 50D
            }
        });
        tileview.TileColumns.Add(new()
        {
            Length =
            {
                Type = TableDefinitionLengthType.Pixel,
                Value = 100D
            },
            PaddingLeft = 8
        });
        tileview.TileRows.Add(new()
        { 
            Length =
            {
                Type = TableDefinitionLengthType.Pixel,
                Value = 26D
            }
        });
        tileview.TileRows.Add(new()
        { 
            Length =
            {
                Type = TableDefinitionLengthType.Pixel,
                Value = 24D
            }
        });
        tileview.TileRows.Add(new()
        { 
            Length =
            {
                Value = 25D
            }
        });
        tileview.TileSpans.Add(new() { RowSpan = 2 });
        tileview.TileSpans.Add(new() { ColumnSpan = 2, ColumnIndex = 1 });
        tileview.TileSpans.Add(new() { RowIndex = 1, RowSpan = 2, ColumnSpan = 2, ColumnIndex = 1 });

        tileview.TileTemplate.Add(new TileViewItemElement()
        {
            Column = colCaption,
            ColumnIndex = 1,
            TextAlignment = TileItemContentAlignment.MiddleLeft,
            ImageOptions = 
            { 
                ImageAlignment = TileItemContentAlignment.MiddleCenter,
                ImageScaleMode = TileItemImageScaleMode.Squeeze,
            },
            Appearance =
            {
                Normal =
                {
                    FontSizeDelta = 2,
                    FontStyleDelta = FontStyle.Bold,
                    Options =
                    {
                        UseFont = true,
                        UseTextOptions = true
                    },
                    TextOptions =
                    {
                        Trimming = Trimming.EllipsisCharacter,
                        WordWrap = WordWrap.NoWrap
                    }
                }
            }

        });
        tileview.TileTemplate.Add(new TileViewItemElement()
        {
            Column = colImage,
            ImageOptions =
            {
                ImageAlignment = TileItemContentAlignment.MiddleCenter,
                ImageScaleMode = TileItemImageScaleMode.Squeeze
            },
            TextAlignment = TileItemContentAlignment.MiddleCenter
        });

        tileview.TileTemplate.Add(new TileViewItemElement()
        {
            Column = colDescription,
            TextAlignment = TileItemContentAlignment.TopLeft,
            RowIndex = 1,
            ColumnIndex = 1,
            Appearance = 
            { 
                Normal =
                {
                    Options = { UseTextOptions = true },
                    TextOptions = 
                    { 
                        Trimming = Trimming.EllipsisCharacter, 
                        WordWrap = WordWrap.Wrap 
                    },
                }
            }
        });


        tileview.ItemCustomize += (_, e) =>
        {
            if (e.Item.RowHandle == tileview.FocusedRowHandle)
            {
                if (!tileview.GridControl.Focused)
                {
                    e.Item.Appearance.BackColor = UI.TranslateSystemToSkinColor(SystemColors.ControlLightLight); // Color.FromArgb(50, UIDX.TranslateSystemToSkinColor(SystemColors.Highlight));
                    e.Item.Appearance.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.ControlText);
                }
                else
                {
                    e.Item.Appearance.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Highlight);
                    e.Item.Appearance.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.HighlightText);
                }
            }
            else
            {
                e.Item.Appearance.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Window);
                e.Item.Appearance.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.ControlText);
            }
        };

        tileview.FocusedRowObjectChanged += (_, _) =>
        {

        };

        if (OptionsAllowDragDrop && OptionsAllowDragDropList)
        {
            dragDropEventsList = new();

            behaviorManager1!.SetBehaviors(tileview, [
                DragDropBehavior.Create(typeof(DevExpress.XtraGrid.Extensions.ColumnViewDragDropSource), true, false, false, true, dragDropEventsList)
            ]);
        }

        Grid!.ViewCollection.AddRange([tileview]);
        Grid!.MainView = tileview;
        Grid!.Enter += (_, _) => 
        {
            tileview.RefreshRow(tileview.FocusedRowHandle);
        };
    }

    private void setupDragDrop()
    {
        dragDropEventsTree = new(); 

        behaviorManager1!.SetBehaviors(Tree!, [DragDropBehavior.Create(typeof(TreeListDragDropSource), true, true, true, true, dragDropEventsTree)]);

        dragDropEventsTree.DragOver += (_, e) =>
        {
            ITreeNode? node = Tree!.GetNodeFromLocation<ITreeNode>(e.Location);

            if (node != null)
            {
                if (node.NODE_ALLOWDROP)
                {
                    e.Action = DragDropActions.None;
                    e.Handled = true;

                    ClearDropNode?.Invoke(this, EventArgs.Empty);

                    return;
                }
                
                DragNode?.Invoke(node, e);

                if (e.Handled)
                    return;
            }
            else 
                ClearDropNode?.Invoke(this, EventArgs.Empty);

            e.Default();
        };

        dragDropEventsTree.DragDrop += (_, e) =>
        {
            dropNode = null;

            var data = e.GetData<IEnumerable<TreeListNode>>();
            
            if (data != null)
                dropNode = data.First();
        };
        
        dragDropEventsTree.BeginDragDrop += (_, e) =>
        {
            dropNode = null;

            if (e.Data == null || e.Data is not IEnumerable<TreeListNode> treelist) return;

            TreeListNode node = treelist.First();
            ITreeNode row = (ITreeNode)Tree!.GetRow(node.Id);

            if (row == null || !row.NODE_ALLOWDRAG)
                e.Cancel = true;
        };

        dragDropEventsTree.EndDragDrop += (_, _) =>
        {
            if (dropNode == null) return;

            TreeListNode parentnode = dropNode.ParentNode;
            var row = Tree!.GetRow(dropNode.Id);
            var parentrow = (parentnode == null ? null : Tree!.GetRow(parentnode.Id));

            DropNode?.Invoke(row, parentrow);

            dropNode = null;
        };

    }

    private void setupMenuColumn()
    {
        treeListColumn2 = new();

        treeListColumn2.Caption = @"";
        treeListColumn2.FieldName = @"none";
        treeListColumn2.Name = @"colMenu";
        treeListColumn2.OptionsColumn.FixedWidth = true;
        treeListColumn2.Visible = true;
        treeListColumn2.VisibleIndex = 1;
        treeListColumn2.Width = UI.GetScaled(30);

        btnMenu = new();
        treeNodeMenu = new();

        Tree!.RepositoryItems.AddRange([btnMenu]);

        btnMenu.AllowFocused = false;
        btnMenu.AllowGlyphSkinning = DefaultBoolean.True;
        btnMenu.AutoHeight = false;
        btnMenu.BorderStyle = BorderStyles.NoBorder;
        btnMenu.Buttons.AddRange([new EditorButton(ButtonPredefines.Glyph, "", -1, true, true, false, new()
        {
            AllowGlyphSkinning = DefaultBoolean.True,
            SvgImage = Glyphs.GetImage(Symbol.MoreVertical),
            SvgImageSize = new(16, 16)
        })]);
        btnMenu.ButtonsStyle = BorderStyles.NoBorder;
        btnMenu.ContextImageOptions.AllowGlyphSkinning = DefaultBoolean.True;
        btnMenu.Name = @"btnMenu";
        btnMenu.Padding = new(1, 0, 2, 0);
        btnMenu.TextEditStyle = TextEditStyles.HideTextEditor;


        btnMenu.ButtonClick += (s, _) =>
        {
            treeNodeMenu.Items.Clear();

            ITreeNode node = (ITreeNode)Tree.GetRow(Tree.FocusedNode.Id);

            if (node == null)
                return;
            
            ITreeNode? parentnode = (Tree.FocusedNode.ParentNode != null
                ? (ITreeNode)Tree.GetRow(Tree.FocusedNode.ParentNode.Id)
                : null);

            FillTreeNodeMenu?.Invoke(treeNodeMenu!, node, parentnode);

            if (s is ButtonEdit btnedit && treeNodeMenu.Items.Count > 0)
                treeNodeMenu.ShowPopup(Tree, btnedit.Location with { Y = btnedit.Location.Y + btnedit.Height });
        };

        Tree!.CustomColumnSort += (_, e) =>
        {
            ITreeNode node1 = (ITreeNode)e.Node1.TreeList.GetRow(e.Node1.Id);
            ITreeNode node2 = (ITreeNode)e.Node2.TreeList.GetRow(e.Node2.Id);

            e.Result = string.Compare(node1.NODE_NAME_SORT, node2.NODE_NAME_SORT, StringComparison.OrdinalIgnoreCase);
        };

        Tree!.ShownEditor += (s, _) =>
        {
            if (s is ButtonEdit btnedit)
                btnedit.SendMouse(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        };

        Tree!.MouseDown += (s, e) =>
        {
            if (s is not TreeList view)
                return;

            
            TreeListHitInfo hi = view.CalcHitInfo(e.Location);
            if (!hi.InRowCell) return;
            
            if (hi.Column != treeListColumn2) return;
            
            view.FocusedColumn = hi.Column;
            view.ShowEditor();
            //force button click  
            ButtonEdit edit = (ButtonEdit)view.ActiveEditor;
            var p = view.PointToScreen(e.Location);
            p = edit.PointToClient(p);

            if (edit.GetViewInfo() == null || edit.GetViewInfo() is not ButtonEditViewInfo info) return;

            EditHitInfo? ehi = info.CalcHitInfo(p);
            
            if (ehi != null && ehi.HitTest == EditHitTest.Button)
            {
                edit.PerformClick(ehi.HitObject as EditorButton);
                ((DXMouseEventArgs)e).Handled = true;
            }
        };
    }


    /// <summary>
    /// No target node available event
    /// </summary>
    public event EventHandler? ClearDropNode;

    /// <summary>
    /// method to drag a node
    ///
    /// The method receives the node and the action (copy, move, etc.) and must
    /// return true if the node can be dropped on the node.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Action<object, DragDropEventArgs>? OnDragDrop { get; set; } = null;


    /// <summary>
    /// method to drop a node
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Action<object, object?>? DropNode { get; set; } = null;

    /// <summary>
    /// method to drag a node
    ///
    /// The method receives the node and the action (copy, move, etc.) and must
    /// return true if the node can be dropped on the node.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Action<object, DragOverEventArgs>? DragNode { get; set; } = null;


    /// <summary>
    /// method to fill the context menu for a tree node
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Action<DXPopupMenu, ITreeNode, ITreeNode?>? FillTreeNodeMenu { get; set; } = null;

    /// <summary>
    /// TreeView, dass die Ordner darstellen soll
    /// </summary>
    [Browsable(true)]
    [DefaultValue(null)]
    public AFTreeGrid? Tree { get; set; }

    /// <summary>
    /// TreeView, dass die Ordner darstellen soll
    /// </summary>
    [Browsable(true)]
    [DefaultValue(null)]
    public AFGridControl? Grid { get; set;}

    /// <summary>
    /// Name des Controllers (zur Identifikation bei Ereignissen)
    /// </summary>
    [Browsable(true)]
    [DefaultValue("")]
    public string Name { get; set;} = "";

    /// <summary>
    /// Liste
    /// </summary>
    public TileView? List => Grid?.Views[0] as TileView;

    /// <summary>
    /// Für das TreeView zu verwendende Bilder
    /// </summary>
    [Browsable(true)]
    [DefaultValue(null)] 
    public SvgImageCollection? TreeImages { get; set; }

    /// <summary>
    /// Menu unterstützen (für jeder Node wird ein Popup-Menu angezeigt)
    /// </summary>
    [Browsable(true)]
    [DefaultValue(false)] 
    public bool OptionsAllowMenu { get; set; }

    /// <summary>
    /// DragDrop unterstützen
    /// </summary>
    [Browsable(true)]
    [DefaultValue(false)]
    public bool OptionsAllowDragDrop { get; set; }

    /// <summary>
    /// DragDrop unterstützen aus der Liste
    /// </summary>
    [Browsable(true)]
    [DefaultValue(false)]
    public bool OptionsAllowDragDropList { get; set; }
}





/// <summary>
/// Ein Element das in der Liste eines TreeNavigation-Controls dargestellt werden kann.
/// </summary>
public interface IListElement 
{
    /// <summary>
    /// Überschrift
    /// </summary>
    string Caption { get; set; }

    /// <summary>
    /// Beschreibung
    /// </summary>
    string Description { get; set; }

    /// <summary>
    /// Bildindex (bei Verwendung einer ImageList)
    /// </summary>
    int? ImageIndex { get; set; }

    /// <summary>
    /// Bild (bei Verwendung individueller Bilder)
    /// </summary>
    SvgImage? Image { get; set; }   
}
