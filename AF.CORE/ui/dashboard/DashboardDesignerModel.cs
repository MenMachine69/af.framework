using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace AF.CORE;

/// <summary>
/// Model für den DashboardDesigner
/// </summary>
[Serializable]
[AFContext("Dashboard-Designer", "Dashboard-Designer")]
public class DashboardDesignerModel : DataObjectBase
{
    #region IDataObject Implementierung
    [XmlIgnore]
    [JsonIgnore]
    private static readonly Guid _id = new("{31FF1D1C-E6ED-47F5-A643-D01770A32813}");

    /// <summary>
    /// GUID des Moduls
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    public override Guid PrimaryKey => _id;

    /// <inheritdoc />
    public override string ToString() { return "Dashboard-Designer Model"; }
    #endregion

    /// <summary>
    /// Constructor
    /// </summary>
    public DashboardDesignerModel() { }

}
