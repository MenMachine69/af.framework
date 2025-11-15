using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace AF.DATA;

/// <summary>
/// Modell zum Speichern eines Snippet Designers 
/// </summary>
[Serializable]
[AFContext("Designer", "Designer")]
public class CodeSnippetDesignerModel : DataObjectBase
{
    #region IDataObject Implementierung
    [XmlIgnore]
    [JsonIgnore]
    private static readonly Guid _id = new("{E91A1A2D-AFC7-4457-94C4-799BDB6385CE}");

    /// <summary>
    /// GUID des Moduls
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    public override Guid PrimaryKey => _id;

    /// <inheritdoc />
    public override string ToString() { return "Snippet Model"; }
    #endregion

    /// <summary>
    /// Constructor
    /// </summary>
    public CodeSnippetDesignerModel() { }

}