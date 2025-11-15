using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace AF.DATA;

/// <summary>
/// MVC-Modell für den QueryDesigner
/// </summary>
[Serializable]
[AFContext("Designer", "Designer")]
public class QueryDesignerModel : DataObjectBase
{
    #region IDataObject Implementierung
    [XmlIgnore]
    [JsonIgnore]
    private static readonly Guid _id = new("{41380724-0D4C-4044-9A2A-E1169C93F411}");

    /// <summary>
    /// GUID des Moduls
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    public override Guid PrimaryKey => _id;

    /// <inheritdoc />
    public override string ToString() { return "Query Model"; }
    #endregion

    /// <summary>
    /// Constructor
    /// </summary>
    public QueryDesignerModel() { }

}