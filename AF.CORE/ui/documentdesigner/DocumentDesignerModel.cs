using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace AF.DATA;

/// <summary>
/// Modell zum Speichern eines Document Designers 
/// </summary>
[Serializable]
[AFContext("Designer", "Designer")]
public class DocumentDesignerModel : DataObjectBase
{
    #region IDataObject Implementierung
    [XmlIgnore]
    [JsonIgnore]
    private static readonly Guid _id = new("{05864EE5-3719-483E-AD70-D08028CB2B79}");

    /// <summary>
    /// GUID des Moduls
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    public override Guid PrimaryKey => _id;

    /// <inheritdoc />
    public override string ToString() { return "Document Model"; }
    #endregion

    /// <summary>
    /// Constructor
    /// </summary>
    public DocumentDesignerModel() { }

}