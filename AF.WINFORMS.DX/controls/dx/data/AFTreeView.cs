using DevExpress.Utils;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;

namespace AF.WINFORMS.DX;

/// <summary>
/// TreeView zur vereinfachten Anzeige von Elementen als Baumansicht.
/// 
/// Der TreeVIew hat nur eine Spalte, in der die daten angezeigt werden. 
/// Zur Anzeige können alle Elemente gebracht werden, die ITreeViewNode implementieren.
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Data")]
public class AFTreeView : AFTreeGrid
{
    private WeakEvent<EventHandler<NodeEventArgs>>? _nodeSelected;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFTreeView()
    {
        if (UI.DesignMode) return;

        OptionsView.ShowColumns = false;
        OptionsView.ShowIndicator = false;
        OptionsView.ShowHorzLines = false;
        OptionsView.ShowVertLines = false;
        OptionsSelection.EnableAppearanceFocusedCell = false;
        OptionsSelection.EnableAppearanceFocusedRow = true;
        OptionsView.FocusRectStyle = DrawFocusRectStyle.None;
        OptionsView.ShowIndentAsRowStyle = true;

        TreeListColumn colNode = new()
        {
            Caption = "Auswahl",
            FieldName = "Caption",
            Name = "node",
            Visible = true,
            MinWidth = 100,
            VisibleIndex = 0
        };
        colNode.OptionsColumn.AllowEdit = false;
        Columns.AddRange([ colNode ]);

        AfterFocusNode += (_, args) =>
        {
            _nodeSelected?.Raise(this, args);
        };
    }

    /// <summary>
    /// Ereignis, das ausgelöst wird, wenn ein Element ausgewählt wurde
    /// </summary>
    public event EventHandler<NodeEventArgs> SelectedValueChanged
    {
        add
        {
            _nodeSelected ??= new();
            _nodeSelected.Add(value);
        }
        remove => _nodeSelected?.Remove(value);
    }

    /// <summary>
    /// Erzeugt die Knoten des Trees
    /// </summary>
    /// <param name="nodes">Liste der Knoten</param>
    public void Fill(List<ITreeViewNode> nodes)
    {
        Nodes.Clear();
        BeginUnboundLoad();
        _loadNodes(null, nodes);
        EndUnboundLoad();
    }

    /// <summary>
    /// Fügt einer Node eine neue Node hinzu...
    /// </summary>
    /// <param name="parentnode"></param>
    /// <param name="node"></param>
    public void AppendNode(TreeListNode parentnode, TreeViewNode node)
    {
        TreeListNode n = AppendNode(new object[] { node.Caption }, parentnode, node.Value);

        n.ImageIndex = node.ImageIndex;
        n.SelectImageIndex = node.ImageIndexSelected;
        n.Tag = node.Value;

        if (node.ChildCount > 0)
            _loadNodes(n, node.ChildNodes);
    }

    /// <summary>
    /// Aktualsiert eine Node
    /// </summary>
    /// <param name="node"></param>
    /// <param name="allowappend">wenn die Node noch nicht existiert, darf sie hinzuigefügt werden...</param>
    public void UpdateNode(TreeViewNode node, bool allowappend)
    {
        TreeListNode? found = FindNode(no => no.Tag.Equals(node.Value));

        if (found == null)
        {
            if (allowappend) AppendNode(Nodes[0], node);
            return;
        }

        found.ImageIndex = node.ImageIndex;
        found.SelectImageIndex = node.ImageIndexSelected;
        found.SetValue(Columns[0], node.Caption);
    }

    /// <summary>
    /// Löscht eine Node
    /// </summary>
    /// <param name="node"></param>
    public void RemoveNode(TreeViewNode node)
    {
        TreeListNode? n = FindNode(no => no.Tag.Equals(node.Value));
        n?.ParentNode.Nodes.Remove(n);
    }


    private void _loadNodes(TreeListNode? parentnode, List<ITreeViewNode> nodes)
    {
        foreach (ITreeViewNode node in nodes)
        {
            TreeListNode n = AppendNode(new object[] { node.Caption }, parentnode, node.Value);
            n.ImageIndex = node.ImageIndex;
            n.SelectImageIndex = node.ImageIndexSelected;
            n.Tag = node.Value;
            _loadNodes(n, node.ChildNodes);
        }
    }
}