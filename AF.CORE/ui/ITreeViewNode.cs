namespace AF.CORE;

/// <summary>
/// Interface für Elemente, die in einem TreeView (z.B. AFTreeView) angezeiugt werden können.
/// </summary>
public interface ITreeViewNode
{
    /// <summary>
    /// Text/ANzeige
    /// </summary>
    string Caption { get; set; }
    /// <summary>
    /// Index des Bildes
    /// </summary>
    int ImageIndex { get; set; }
    /// <summary>
    /// Index des Bildes wenn der Knoten aufgeklappt ist
    /// </summary>
    int ImageIndexSelected { get; set; }
    /// <summary>
    /// Value: beliebiges Objekt, dass den Wert des Eintrags repräsentiert. 
    /// Dieser Wert muss innerhalb eines Tree's eindeutig sein.
    /// </summary>
    object Value { get; set; }

    /// <summary>
    /// Kindknoten...
    /// </summary>
    List<ITreeViewNode> ChildNodes { get; }

    /// <summary>
    /// Anzahl der ChildNodes ermitteln.
    /// </summary>
    int ChildCount { get; }
}

/// <summary>
/// Node in einem TreeView.
/// 
/// Standardimpelemntierung eines ITreeViewNode.
/// </summary>
public sealed class TreeViewNode : ITreeViewNode
{
    private List<ITreeViewNode>? _ChildNodes;

    /// <summary>
    /// Text/ANzeige
    /// </summary>
    public string Caption { get; set; } = "";
    /// <summary>
    /// Index des Bildes
    /// </summary>
    public int ImageIndex { get; set; }
    /// <summary>
    /// Index des Bildes wenn der Knoten aufgeklappt ist
    /// </summary>
    public int ImageIndexSelected { get; set; }
    /// <summary>
    /// Tag: beleibiges Objekt
    /// </summary>
    public object Value { get; set; } = -1;

    /// <summary>
    /// Kindknoten...
    /// </summary>
    public List<ITreeViewNode> ChildNodes => _ChildNodes ??= [];

    /// <summary>
    /// Anzahl der ChildNodes ermitteln.
    /// </summary>
    public int ChildCount => _ChildNodes?.Count ?? 0;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return Caption;
    }
}