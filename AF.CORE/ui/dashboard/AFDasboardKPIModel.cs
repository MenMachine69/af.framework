using System.Drawing;

namespace AF.CORE;

/// <summary>
/// Konfiguratiopn eines AFDashboardKPI (ausgewählte Elemente und deren Reihenfolge).
/// </summary>
[Serializable]
public sealed class AFDashboardKPIConfig
{
    /// <summary>
    /// Elemente in der Konfiguration
    /// </summary>
    public List<AFDashboardKPIConfigElement> Elements { get; set; } = [];
}

/// <summary>
/// Konfiguratiopn eines AFDashboardKPI (ausgewählte Elemente und deren Reihenfolge).
/// </summary>
[Serializable]
public sealed class AFDashboardKPIConfigElement
{
    /// <summary>
    /// eindeutige ID des Elements
    /// </summary>
    public int ID { get; set; }
}



/// <summary>
/// Model, dessen Elemente in einem AFDashboardKPI angezeigt werden können.
/// </summary>
public class AFDashboardKPIModel
{
    /// <summary>
    /// Elemente im Panel
    /// </summary>
    public List<AFDashboardKPIElementModel> Elements { get; set; } = [];
}

/// <summary>
/// In AFDasboardKPI darstellbarer Wert
/// </summary>
public class AFDashboardKPIElementModel
{
    /// <summary>
    /// Größe des Elements
    /// </summary>
    public eDashboardKPIElementSize Size { get; set; } = eDashboardKPIElementSize.Small;

    /// <summary>
    /// Beschriftung
    /// </summary>
    public string Caption { get; set; } = "<Beschriftung>";

    /// <summary>
    /// Tooltip Beschreibung
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    /// Tooltip Titel
    /// </summary>
    public string Title { get; set; } = "";

    /// <summary>
    /// Wert
    /// </summary>
    public string Value { get; set; } = "<b>0</b>";

    /// <summary>
    /// Farbbalken (Transparent = kein Balken)
    /// </summary>
    public Color Indicator { get; set; } = Color.Transparent;
}

/// <summary>
/// Größe des Elements
/// </summary>
public enum eDashboardKPIElementSize
{
    /// <summary>
    /// Klein
    /// </summary>
    Small,   // 1 Zeile
    /// <summary>
    /// Mittel
    /// </summary>
    MidSize, // 2 Zeilen
    /// <summary>
    /// Groß
    /// </summary>
    Large    // 3 Zeilen
}