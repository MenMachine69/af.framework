namespace AF.CORE;

/// <summary>
/// Beschreibt ein Element, das für Anpassungen zur Verfügung steht.
/// 
/// Siehe z.B. KPIDashboard.
/// </summary>
public class CustomizableElement
{
    /// <summary>
    /// Constructor
    /// </summary>
    public CustomizableElement() { }

    /// <summary>
    /// Beschriftung
    /// </summary>
    public string Caption { get; set; } = string.Empty;

    /// <summary>
    /// Beschreibung
    /// </summary>
    public string Description { get; set; } = string.Empty;


    /// <summary>
    /// ID des Elements
    /// </summary>
    public int ID { get;  set; } = -1;
}