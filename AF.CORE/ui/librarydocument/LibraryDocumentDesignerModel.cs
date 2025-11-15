using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace AF.DATA;

/// <summary>
/// Modell zum Speichern eines Library Documents
/// </summary>
[Serializable]
[AFContext("Vorschau", "Vorschau")]
public class LibraryDocumentDesignerModel : DataObjectBase
{
    #region IDataObject Implementierung
    [XmlIgnore]
    [JsonIgnore]
    private static readonly Guid _id = new("{BB5CED4C-0EE0-438A-88A2-577D2E06AADF}");

    /// <summary>
    /// GUID des Moduls
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    public override Guid PrimaryKey => _id;

    /// <inheritdoc />
    public override string ToString() { return "Library Document"; }
    #endregion

    /// <summary>
    /// Constructor
    /// </summary>
    public LibraryDocumentDesignerModel() { }

}