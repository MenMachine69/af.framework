namespace AF.CORE;

/// <summary>
/// Attribut, das ein Eiegenschaft für KPI-Dashboards verfügbar macht.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class AFKPIElement : Attribute
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="id">ID des Elements</param>
    public AFKPIElement(int id)
    {
        ID = id;
    }

    /// <summary>
    /// eindeutige ID des Elements
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Beschriftung des Elements
    /// </summary>
    public string? Caption { get; set; }
}
