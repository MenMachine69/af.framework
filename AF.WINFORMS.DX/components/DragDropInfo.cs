namespace AF.WINFORMS.DX;

/// <summary>
/// Universal storage of DragDro data...
/// </summary>
[Serializable]
public class DragDropInfo
{
    /// <summary>
    /// Type of data to be transmitted...
    /// </summary>
    public Type? DataType { get; set; }

    /// <summary>
    /// ID identifying the data to be transmitted
    /// </summary>
    public Guid DataIdentifier { get; set; }

    /// <summary>
    /// Text representation of the data
    /// </summary>
    public string DataAsString { get; set; } = string.Empty;

    /// <summary>
    /// Binary data (serialised foreign object)
    /// </summary>
    public byte[]? BinaryData { get; set; }

    /// <summary>
    /// String representation of the data
    /// </summary>
    /// <returns></returns>
    public new string ToString()
    {
        return DataAsString;
    }
}


