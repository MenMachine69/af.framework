using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace AF.DATA;

/// <summary>
/// Modell zum Speichern eines Script Designers 
/// </summary>
[Serializable]
[AFContext("Designer", "Designer")]
public class ScriptDesignerModel : DataObjectBase
{
    #region IDataObject Implementierung
    [XmlIgnore]
    [JsonIgnore]
    private static readonly Guid _id = new("{65CA6EC5-A21E-4AD7-A706-458BBEAC88F1}");

    /// <summary>
    /// GUID des Moduls
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    public override Guid PrimaryKey => _id;

    /// <inheritdoc />
    public override string ToString() { return "Script Model"; }
    #endregion

    /// <summary>
    /// Constructor
    /// </summary>
    public ScriptDesignerModel() { }

}