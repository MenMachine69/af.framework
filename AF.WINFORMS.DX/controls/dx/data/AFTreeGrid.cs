using DevExpress.Utils;
using DevExpress.Utils.Behaviors;
using DevExpress.Utils.DragDrop;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;

namespace AF.WINFORMS.DX;

/// <inheritdoc />
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Data")]
public class AFTreeGrid : TreeList
{
    // Drag-and-Drop Support
    private BehaviorManager? behaviorManager1;
    private DragDropEvents? dragDropEventsTree;
    private TreeListNode? dropNode;
    private WeakEvent<EventHandler<EventArgs>>? _DragDropCanceled;
    private WeakEvent<EventHandler<NodeDroppedEventArgs>>? _NodeDropped;
    private WeakEvent<EventHandler<QueryDropEventArgs>>? _QueryDrop;
    private WeakEvent<EventHandler<QueryDragEventArgs>>? _QueryDrag;

    // Menu-Spalte Support
    private TreeListColumn? colMenu;
    private RepositoryItemButtonEdit? btnMenu;
    private DXPopupMenu? nodeMenu;
    private WeakEvent<EventHandler<BevorShowMenuEventArgs>>? _bevorShowMenu;


    /// <summary>
    /// Gibt den BehaviorManager für DragDrop zurück.
    /// </summary>
    /// <returns>Der BehaviorManager für DragDrop. NULL wenn DragDrop nicht enabled wurde.</returns>
    public BehaviorManager? DragDropManager => behaviorManager1;
    
    /// <inheritdoc />
    protected override void OnHandleDestroyed(EventArgs e)
    {
        base.OnHandleDestroyed(e);

        behaviorManager1?.Dispose();
    }

    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        // Custom Sortierung, wenn es sich um ITreeNode Nodes handelt
        CustomColumnSort += (_, args) =>
        {
            ITreeNode? node1 = args.Node1.TreeList.GetRow(args.Node1.Id) as ITreeNode;
            ITreeNode? node2 = args.Node2.TreeList.GetRow(args.Node2.Id) as ITreeNode;

            if (node1 == null || node2 == null)
                return;

            args.Result = string.Compare(node1.NODE_NAME_SORT, node2.NODE_NAME_SORT, StringComparison.OrdinalIgnoreCase);
        };
    }

    /// <summary>
    /// Erzeugt die Knoten des Trees
    /// </summary>
    /// <param name="nodes"></param>
    public void Fill<TNode>(IList<TNode> nodes) where TNode : ITreeViewNode
    {
        if (Columns.Count < 1)
        {
            OptionsView.ShowColumns = false;
            OptionsView.ShowIndicator = false;
            OptionsView.ShowHorzLines = false;
            OptionsView.ShowVertLines = false;
            OptionsSelection.EnableAppearanceFocusedCell = false;
            OptionsSelection.EnableAppearanceFocusedRow = true;
            OptionsView.FocusRectStyle = DrawFocusRectStyle.None;
            OptionsView.ShowIndentAsRowStyle = true;

            // alle Spalten löschen...
            Columns.Clear();

            TreeListColumn colNode = new()
            {
                Caption = "Name/Type",
                FieldName = "Caption",
                Name = "node",
                Visible = true,
                MinWidth = 100,
                VisibleIndex = 0
            };
            colNode.OptionsColumn.AllowEdit = false;
            Columns.AddRange([colNode]);
        }

        Nodes.Clear();
        BeginUnboundLoad();
        _loadNodes(null, nodes);
        EndUnboundLoad();
    }

    private void _loadNodes<TNode>(TreeListNode? parentnode, IList<TNode> nodes) where TNode : ITreeViewNode
    {
        foreach (TNode node in nodes)
        {
            TreeListNode n = AppendNode(new object[] { node.Caption }, parentnode, node.Value);
            n.ImageIndex = node.ImageIndex;
            n.SelectImageIndex = node.ImageIndexSelected;
            n.Tag = node.Value;
            _loadNodes(n, node.ChildNodes);
        }
    }

    /// <summary>
    /// Ereignis, das ausgelöst wird, wenn eine DragDrop-Operation abgebrochen wird.
    /// </summary>
    public event EventHandler<EventArgs> DragDropCanceled
    {
        add
        {
            _DragDropCanceled ??= new();
            _DragDropCanceled.Add(value);
        }
        remove => _DragDropCanceled?.Remove(value);
    }

    /// <summary>
    /// Ereignis, das ausgelöst wird, eine gezogene Node fallen gelassen wird.
    /// 
    /// Übergeben werden die Argumente (NodeDroppedEventArgs).
    /// </summary>
    public event EventHandler<NodeDroppedEventArgs> NodeDropped
    {
        add
        {
            _NodeDropped ??= new();
            _NodeDropped.Add(value);
        }
        remove => _NodeDropped?.Remove(value);
    }

    /// <summary>
    /// Ereignis, das ausgelöst wird, wenn das eine Node über eine andere gezogen wird (Drop verfügbar?).
    /// 
    /// Übergeben wird die Node und die Argumente (BeginDragEventArgs).
    /// </summary>
    public event EventHandler<QueryDropEventArgs> QueryDrop
    {
        add
        {
            _QueryDrop ??= new();
            _QueryDrop.Add(value);
        }
        remove => _QueryDrop?.Remove(value);
    }

    /// <summary>
    /// Ereignis, das ausgelöst wird, wenn das Ziehen einer Node beginnt (Drag).
    /// 
    /// Übergeben wird die Node und die Argumente (BeginDragEventArgs).
    /// </summary>
    public event EventHandler<QueryDragEventArgs> QueryDrag
    {
        add
        {
            _QueryDrag ??= new();
            _QueryDrag.Add(value);
        }
        remove => _QueryDrag?.Remove(value);
    }

    /// <summary>
    /// Schaltet Drag/Drop von Nodes ein.
    /// 
    /// Voraussetzungen: die Nodes müssen das Interface ITreeNode implementieren.
    /// 
    /// Um das Drag/Drop zu verarbeiten, die Ereignisse NodeDropped, DragDropCanceled, 
    /// QueryDrop und QueryDrag abonnieren.
    /// </summary>
    public void EnableDragDrop()
    {
        behaviorManager1 = new();
        ((ISupportInitialize)behaviorManager1).BeginInit();

        dragDropEventsTree = new();

        behaviorManager1!.SetBehaviors(this, [DragDropBehavior.Create(typeof(TreeListDragDropSource), true, true, true, true, dragDropEventsTree)]);

        dragDropEventsTree.DragOver += (_, e) =>
        {
            ITreeNode? node = this.GetNodeFromLocation<ITreeNode>(e.Location);

            if (node != null)
            {
                if (node.NODE_ALLOWDROP)
                {
                    e.Action = DragDropActions.None;
                    e.Handled = true;

                    _DragDropCanceled?.Raise(this, EventArgs.Empty);

                    return;
                }

                var args = new QueryDropEventArgs(node, e);
                _QueryDrop?.Raise(this, args);

                if (args.Cancel)
                {
                    e.Action = DragDropActions.None;
                    e.Handled = true;

                    _DragDropCanceled?.Raise(this, EventArgs.Empty);

                    return;
                }

                if (e.Handled)
                    return;
            }
            else
                _DragDropCanceled?.Raise(this, EventArgs.Empty);

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
            ITreeNode row = (ITreeNode)GetRow(node.Id);

            if (row == null || !row.NODE_ALLOWDRAG)
                e.Cancel = true;
            else
            {
                var args = new QueryDragEventArgs(row, e);
                _QueryDrag?.Raise(this, args);
                e.Cancel = args.Cancel;
            }
        };

        dragDropEventsTree.EndDragDrop += (_, e) =>
        {
            if (dropNode == null) return;

            TreeListNode parentnode = dropNode.ParentNode;
            ITreeNode row = (ITreeNode)GetRow(dropNode.Id);
            ITreeNode? parentrow = (parentnode == null ? null : (ITreeNode)GetRow(parentnode.Id));

            _NodeDropped?.Raise(this, new NodeDroppedEventArgs(row, parentrow, e));

            dropNode = null;
        };

        ((ISupportInitialize)behaviorManager1!).EndInit();
    }

    /// <summary>
    /// Eine zusätzliche Spalte für ein Kontextmenü hinzufügen.
    /// 
    /// Das Kontextmenü kann im Ereignis BeforShowMenu angepasst werden.
    /// </summary>
    public void EnableMenuColumn()
    {
        colMenu = new()
        {
            Caption = @"",
            FieldName = @"none",
            Name = @"colMenu",
            OptionsColumn =
            {
                FixedWidth = true
            },
            Visible = true,
            VisibleIndex = Columns.Count, // an letzter Stelle einfügen
            Width = UI.GetScaled(30)
        };

        Columns.Add(colMenu);

        btnMenu = new();
        nodeMenu = new();

        RepositoryItems.AddRange([btnMenu]);

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
            nodeMenu.Items.Clear();

            if (GetRow(FocusedNode.Id) is not ITreeNode node)
                return;
            
            ITreeNode? parentnode = (FocusedNode.ParentNode != null
                ? GetRow(FocusedNode.ParentNode.Id) as ITreeNode
                : null);

            BevorShowMenuEventArgs args = new(node, parentnode, nodeMenu);
            _bevorShowMenu?.Raise(this, args);

            if (args.Cancel)
                return;
                        
            if (s is ButtonEdit btnedit && nodeMenu.Items.Count > 0)
                nodeMenu.ShowPopup(this, btnedit.Location with { Y = btnedit.Location.Y + btnedit.Height });
        };

        ShownEditor += (s, _) =>
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
            
            if (hi.Column != colMenu) return;
            
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
    /// Ereignis, das ausgelöst wird, wenn eine DragDrop-Operatuion abgebrochen wird.
    /// </summary>
    public event EventHandler<BevorShowMenuEventArgs> BevorShowMenu
    {
        add
        {
            _bevorShowMenu ??= new();
            _bevorShowMenu.Add(value);
        }
        remove => _bevorShowMenu?.Remove(value);
    }
    
    /// <summary>
    /// Stellt Ereignisdaten für das QueryDrop-Ereignis bereit.
    /// </summary>
    public class BevorShowMenuEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Node, für den das menu angezeigt werden soll
        /// </summary>
        public ITreeNode Node { get; }

        /// <summary>
        /// Übergeordneter Node
        /// </summary>
        public ITreeNode? ParentNode { get; }

        /// <summary>
        /// Menu, dass angepasst werden soll
        /// </summary>
        public DXPopupMenu Menu { get; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="node">Node, auf die gezogen wird</param>
        /// <param name="parentnode">übergeordneter Node</param>
        /// <param name="menu">anzupassendes/anzuzeigendes Menu</param>
        public BevorShowMenuEventArgs(ITreeNode node, ITreeNode? parentnode, DXPopupMenu menu)
        {
            Node = node;
            ParentNode = parentnode;
            Menu = menu;
        }
    }

    /// <summary>
    /// Stellt Ereignisdaten für das QueryDrop-Ereignis bereit.
    /// </summary>
    public class QueryDropEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Node, auf die gezogen wird
        /// </summary>
        public ITreeNode Node { get; }
        /// <summary>
        /// Ereignis-Parameter
        /// </summary>
        public DragOverEventArgs Args { get; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="node">Node, auf die gezogen wird</param>
        /// <param name="args">Ereignis-Parameter</param>
        public QueryDropEventArgs(ITreeNode node, DragOverEventArgs args)
        {
            Node = node;
            Args = args;
        }
    }

    /// <summary>
    /// Stellt Ereignisdaten für das QueryDrag-Ereignis bereit.
    /// </summary>
    public class QueryDragEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Zu ziehende Node
        /// </summary>
        public ITreeNode DragNode { get; }
        /// <summary>
        /// Ereignis-Parameter
        /// </summary>
        public BeginDragDropEventArgs Args { get; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dragNode">zu ziehende Node</param>
        /// <param name="args">Ereignis-Parameter</param>
        public QueryDragEventArgs(ITreeNode dragNode, BeginDragDropEventArgs args)
        {
            DragNode = dragNode;
            Args = args;
        }
    }

    /// <summary>
    /// Stellt Ereignisdaten für das NodeDropped-Ereignis bereit.
    /// </summary>
    public class NodeDroppedEventArgs : EventArgs
    {
        /// <summary>
        /// die gezogene Node
        /// </summary>
        public ITreeNode Node { get; }
        /// <summary>
        /// die Node, auf die gezogen wurde (ParentNode). 
        /// Die Node kann null sein, wenn auf den Root gezogen wurde.
        /// </summary>
        public ITreeNode? ParentNode { get; }
        /// <summary>
        /// Ereignis-Parameter
        /// </summary>
        public EndDragDropEventArgs Args { get; }

        /// <summary>
        /// Konstruktor 
        /// </summary>
        /// <param name="node">die gezogene Node</param>
        /// <param name="parentNode">die Node, auf die gezogen wurde (ParentNode)</param>
        /// <param name="args">Ereignis-Parameter</param>
        public NodeDroppedEventArgs(ITreeNode node, ITreeNode? parentNode, EndDragDropEventArgs args)
        {
            Node = node;
            ParentNode = parentNode;
            Args = args;
        }
    }
}

