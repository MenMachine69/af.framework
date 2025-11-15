using DevExpress.Utils;
using DevExpress.Utils.Behaviors;
using DevExpress.Utils.DragDrop;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Tile;
using DrawFocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle;

namespace AF.WINFORMS.DX;

/// <summary>
/// TreeList for ITreeNode objects
///
/// Supports DragDrop and ContextMenu for TreeNodes
/// </summary>
[ToolboxItem(true)]
public class AFTreeListFolder : AFTreeGrid
{
    private TreeListColumn? treeListColumn1;
    private TreeListColumn? treeListColumn2;
    private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit? btndisplay;
    private BehaviorManager? behaviorManager1;
    private DragDropEvents? dragDropEventsTree;
    private DXPopupMenu? treeNodeMenu;
    private TreeListNode? dropNode;
    private TileView? boundTileView;
    private GridView? boundGridView;

    /// <summary>
    /// image index for the closed folder
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int ImageFolderClosed { get; set; } = 0;

    /// <summary>
    /// image index for the open folder
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int ImageFolderOpen { get; set; } = 1;

    /// <summary>
    /// image index for the closed favorits folder
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int ImageFolderFav { get; set; } = 2;

    /// <summary>
    /// image index for the closed last used folder
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int ImageFolderLastUsed { get; set; } = 3;

    /// <summary>
    /// image index for the open last used folder
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int ImageFolderLastUsedOpen { get; set; } = 4;

    /// <summary>
    /// image index for the open favorits folder
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int ImageFolderFavOpen { get; set; } = 5;

    /// <summary>
    /// image index for the open all entrys folder
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int ImageAllEntrysOpen { get; set; } = 6;

    /// <summary>
    /// image index for the all entrys folder
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int ImageAllEntrys { get; set; } = 7;

    /// <summary>
    /// ID of the favorits folder
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Guid IDFavorits { get; set; } = Guid.Empty;

    /// <summary>
    /// ID of the all entrys folder
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Guid IDAllEntrys { get; set; } = Guid.Empty;

    /// <summary>
    /// ID of the last used folder
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Guid IDLastUsed { get; set; } = Guid.Empty;
    
    /// <summary>
    /// method to fill the context menu for a tree node
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Action<DXPopupMenu, ITreeNode, ITreeNode?>? FillTreeNodeMenu { get; set; } = null;

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
    public Action<object, DragDropEventArgs>? OnDragDropAction { get; set; } = null;


    /// <summary>
    /// method to drag a node
    ///
    /// The method receives the node and the action (copy, move, etc.) and must
    /// return true if the node can be dropped on the node.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Action<object, DragOverEventArgs>? DragNode { get; set; } = null;

    /// <summary>
    /// method to get the image index for a node
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Func<ITreeNode, bool, int>? GetCustomImageIndex { get; set; } = null;

    /// <summary>
    /// Load data from a method
    /// </summary>
    /// <param name="getFolderMethod">method to load folders</param>
    public new void Load(Func<IBindingList> getFolderMethod)
    {
        DataSource = getFolderMethod.Invoke();
    }

    /// <summary>
    /// Setup the tree
    /// </summary>
    /// <param name="images">images list</param>
    /// <param name="components"></param>
    [SupportedOSPlatform("windows")]
    public void Setup(SvgImageCollection images, IContainer components)
    {
        treeListColumn1 = new();
        treeListColumn2 = new();
        btndisplay = new();
        behaviorManager1 = new(components);
        dragDropEventsTree = new(components);

        ((ISupportInitialize)behaviorManager1).BeginInit();

        Columns.AddRange([treeListColumn1, treeListColumn2]);
        OptionsMenu.EnableColumnMenu = false;
        OptionsMenu.EnableFooterMenu = false;
        OptionsMenu.ShowExpandCollapseItems = false;
        OptionsSelection.EnableAppearanceFocusedCell = false;
        OptionsView.AllowGlyphSkinning = true;
        OptionsView.FocusRectStyle = DrawFocusRectStyle.None;
        OptionsView.ShowIndentAsRowStyle = true;
        OptionsView.ShowTreeLines = DefaultBoolean.False;
        RepositoryItems.AddRange([btndisplay]);
        RowHeight = 24;
        ViewStyle = TreeListViewStyle.TreeView;

        RootValue = Guid.Empty;
        KeyFieldName = nameof(ITreeNode.NODE_ID);
        ParentFieldName = nameof(ITreeNode.NODE_PARENT_ID);
        StateImageList = images;
        
        treeNodeMenu = new();

        treeListColumn1.Caption = @"Folder";
        treeListColumn1.FieldName = @"NODE_NAME";
        treeListColumn1.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
        treeListColumn1.Name = @"treeListColumn1";
        treeListColumn1.OptionsColumn.AllowEdit = false;
        treeListColumn1.OptionsFilter.FilterBySortField = DefaultBoolean.True;
        treeListColumn1.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
        treeListColumn1.SortOrder = SortOrder.Ascending;
        treeListColumn1.Visible = true;
        treeListColumn1.VisibleIndex = 0;
        treeListColumn1.Width = 327;

        treeListColumn2.Caption = @"treeListColumn2";
        treeListColumn2.FieldName = @"treeListColumn2";
        treeListColumn2.Name = @"treeListColumn2";
        treeListColumn2.OptionsColumn.FixedWidth = true;
        treeListColumn2.Visible = true;
        treeListColumn2.VisibleIndex = 1;
        treeListColumn2.Width = 30;

        btndisplay.AllowFocused = false;
        btndisplay.AllowGlyphSkinning = DefaultBoolean.True;
        btndisplay.AutoHeight = false;
        btndisplay.BorderStyle = BorderStyles.NoBorder;
        SerializableAppearanceObject serializableAppearanceObject1 = new();
        SerializableAppearanceObject serializableAppearanceObject2 = new();
        SerializableAppearanceObject serializableAppearanceObject3 = new();
        SerializableAppearanceObject serializableAppearanceObject4 = new();
        EditorButtonImageOptions editorButtonImageOptions1 = new()
        {
            AllowGlyphSkinning = DefaultBoolean.True,
            SvgImage = Glyphs.GetImage(Symbol.MoreVertical),
            SvgImageSize = new(16, 16)
        };
        btndisplay.Buttons.AddRange([new EditorButton(ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions1, new(Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, ToolTipAnchor.Default)
        ]);
        btndisplay.ButtonsStyle = BorderStyles.NoBorder;
        btndisplay.ContextImageOptions.AllowGlyphSkinning = DefaultBoolean.True;
        btndisplay.Name = @"btndisplay";
        btndisplay.Padding = new(1, 0, 2, 0);
        btndisplay.TextEditStyle = TextEditStyles.HideTextEditor;
        
        btndisplay.ButtonClick += (s, e) =>
        {
            treeNodeMenu.Items.Clear();

            ITreeNode node = (ITreeNode)GetRow(FocusedNode.Id);

            if (node == null)
                return;
            
            ITreeNode? parentnode = (FocusedNode.ParentNode != null
                ? (ITreeNode)GetRow(FocusedNode.ParentNode.Id)
                : null);

            FillTreeNodeMenu?.Invoke(treeNodeMenu, node, parentnode);

            if (s is ButtonEdit btnedit && treeNodeMenu.Items.Count > 0)
                treeNodeMenu.ShowPopup(this, btnedit.Location with { Y = btnedit.Location.Y + btnedit.Height });
        };

        behaviorManager1.SetBehaviors(this, [DragDropBehavior.Create(typeof(TreeListDragDropSource), true, true, true, true, dragDropEventsTree)
        ]);

        dragDropEventsTree.DragOver += (s, e) =>
        {
            ITreeNode? node = GetNodeFromLocation(e.Location);

            if (node != null)
            {
                if (node.NODE_ID.Equals(IDFavorits) ||
                    node.NODE_ID.Equals(IDLastUsed) ||
                    node.NODE_ID.Equals(IDAllEntrys))
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

        dragDropEventsTree.DragDrop += (s, e) =>
        {
            dropNode = null;

            var data = e.GetData<IEnumerable<TreeListNode>>();
            
            if (data != null && data.Any())
                dropNode = data.First();
        };
        
        dragDropEventsTree.BeginDragDrop += (s, e) =>
        {
            dropNode = null;

            if (e.Data == null || e.Data is not IEnumerable<TreeListNode> treelist) return;

            TreeListNode node = treelist.First();
            ITreeNode row = (ITreeNode)GetRow(node.Id);

            if (row != null && (row.NODE_ID.Equals(IDFavorits) ||
                                row.NODE_ID.Equals(IDLastUsed) ||
                                row.NODE_ID.Equals(IDAllEntrys)))
                e.Cancel = true;
        };

        dragDropEventsTree.EndDragDrop += (s, e) =>
        {
            if (dropNode == null) return;

            TreeListNode parentnode = dropNode.ParentNode;
            var row = GetRow(dropNode.Id);
            var parentrow = (parentnode == null ? null : GetRow(parentnode.Id));

            DropNode?.Invoke(row, parentrow);

            dropNode = null;
        };
        
        CustomNodeCellEdit += (s, e) =>
        {
            if (e.Column == treeListColumn2)
            {
                if (e.Node == FocusedNode)
                    e.RepositoryItem = btndisplay;
                else
                    e.RepositoryItem = null;
            }
        };

        GetStateImage += (s, e) =>
        {
            ITreeNode node = (ITreeNode)GetRow(e.Node.Id);

            if (node == null)
                return;

            if (GetCustomImageIndex != null)
            {
                e.NodeImageIndex = GetCustomImageIndex(node, e.Node.Expanded || e.Node.Focused);
                return;
            }

            if (node.NODE_ID.Equals(IDFavorits))
            {
                if (e.Node.Expanded || e.Node.Focused)
                    e.NodeImageIndex = ImageFolderFavOpen;
                else
                    e.NodeImageIndex = ImageFolderFav;
            }
            else if (node.NODE_ID.Equals(IDLastUsed))
            {
                if (e.Node.Expanded || e.Node.Focused)
                    e.NodeImageIndex = ImageFolderLastUsedOpen;
                else
                    e.NodeImageIndex = ImageFolderLastUsed;
            }
            else if (node.NODE_ID.Equals(IDAllEntrys))
            {
                if (e.Node.Expanded || e.Node.Focused)
                    e.NodeImageIndex = ImageAllEntrysOpen;
                else
                    e.NodeImageIndex = ImageAllEntrys;
            }
            else
            {
                if (e.Node.Expanded || e.Node.Focused)
                    e.NodeImageIndex = ImageFolderOpen;
                else
                    e.NodeImageIndex = ImageFolderClosed;
            }
        };

        CustomColumnSort += (s, e) =>
        {
            ITreeNode node1 = (ITreeNode)e.Node1.TreeList.GetRow(e.Node1.Id);
            ITreeNode node2 = (ITreeNode)e.Node2.TreeList.GetRow(e.Node2.Id);

            e.Result = node1.NODE_NAME_SORT.CompareTo(node2.NODE_NAME_SORT);
        };

        ShownEditor += (s, e) =>
        {
            if (s is ButtonEdit btnedit)
                btnedit.SendMouse(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        };

        MouseDown += (s, e) =>
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

        ((ISupportInitialize)behaviorManager1).EndInit();
    }

    /// <summary>
    /// No target node available event
    /// </summary>
    public event EventHandler? ClearDropNode;

    /// <summary>
    /// Return the node at the given location
    /// </summary>
    /// <param name="location">location</param>
    /// <param name="listNode">the TreeListNode object which represents the node</param>
    /// <returns>the node object</returns>
    [SupportedOSPlatform("windows")]
    public ITreeNode? GetNodeFromLocation(Point location, out TreeListNode? listNode)
    {
        ITreeNode? node = null;

        var info = CalcHitInfo(PointToClient(location));

        if (info.Node != null)
        {
            listNode = info.Node;
            node = (ITreeNode)GetRow(info.Node.Id);
        }
        else
            listNode = null;

        return node;
    }

    /// <summary>
    /// Return the node at the given location
    /// </summary>
    /// <param name="location">location</param>
    /// <returns>the node object</returns>
    [SupportedOSPlatform("windows")]
    public ITreeNode? GetNodeFromLocation(Point location)
    {
        ITreeNode? node = null;

        var info = CalcHitInfo(PointToClient(location));

        if (info.Node != null)
            node = (ITreeNode)GetRow(info.Node.Id);

        return node;
    }

    /// <summary>
    /// Bound a TileView to the TreeList for drag an drop of tileview items to the TreeList
    /// </summary>
    /// <param name="view">TileView</param>
    /// <param name="components"></param>
    /// <returns>DragDropEbents object</returns>
    /// <exception cref="Exception"></exception>
    [SupportedOSPlatform("windows")]
    public DragDropEvents BoundTileView(TileView view, IContainer components)
    {
        if (behaviorManager1 == null)
            throw new Exception(@"Call Setup before bound a TileView.");

        DragDropEvents dragDropTilesEvents = new(components);

        behaviorManager1.SetBehaviors(view, [
            ((Behavior)(DragDropBehavior.Create(typeof(DevExpress.XtraGrid.Extensions.ColumnViewDragDropSource), true, false, false, true, dragDropTilesEvents)))
        ]);

        boundTileView = view;

        return dragDropTilesEvents;
    }

    /// <summary>
    /// Bound a GridView to the TreeList for drag an drop of tileview items to the TreeList
    /// </summary>
    /// <param name="view">GridView</param>
    /// <param name="components"></param>
    /// <returns>DragDropEbents object</returns>
    /// <exception cref="Exception"></exception>
    [SupportedOSPlatform("windows")]
    public DragDropEvents BoundGridView(GridView view, IContainer components)
    {
        if (behaviorManager1 == null)
            throw new Exception(@"Call Setup before bound a GridView.");

        DragDropEvents dragDropGridEvents = new(components);

        behaviorManager1.SetBehaviors(view, [
            (Behavior)DragDropBehavior.Create(typeof(DevExpress.XtraGrid.Extensions.ColumnViewDragDropSource), true, false, false, true, dragDropGridEvents)
        ]);

        boundGridView = view;

        return dragDropGridEvents;
    }

    /// <summary>
    /// access to the DragDropManager
    /// </summary>
    public new BehaviorManager? DragDropManager => behaviorManager1;
}