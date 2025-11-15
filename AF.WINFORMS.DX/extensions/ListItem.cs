namespace AF.WINFORMS.DX;

/// <summary>
/// Universal class for entries in combo and list boxes
/// </summary>
[Serializable]
public class ListItem
{
    /// <summary>
    /// Text to display
    /// </summary>
    public string Caption { get; set; }
    
    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// Group to which the item belongs
    /// </summary>
    public string Group { get; set; }
    
    /// <summary>
    /// Selection value
    /// </summary>
    public object? Value { get; set; }

    /// <summary>
    /// Index of the image to be displayed
    /// </summary>
    public int ImageIndex { get; set; }

    /// <summary>
    /// Status selected
    /// </summary>
    public bool Checked { get; set; }

    /// <summary>
    /// Additional information
    /// </summary>
    public object? Tag { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public ListItem()
    {
        Caption = "";
        Description = "";
        Group = "";
        Value = null;
        Tag = null;
        ImageIndex = -1;
        Checked = false;
    }

    /// <summary>
    /// overridden ToString method to display the caption.
    /// </summary>
    /// <returns>Caption of the item</returns>
    public override string ToString()
    { return Caption; }
}