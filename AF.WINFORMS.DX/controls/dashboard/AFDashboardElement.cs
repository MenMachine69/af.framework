namespace AF.WINFORMS.DX;

/// <summary>
/// Basisklasse von Dashboard Elementen
/// </summary>
[DesignerCategory("Code")]
public class AFDashboardElement : AFUserControl
{
}

/// <summary>
/// Dashboard-Element, dass ein Grid darstellt
/// </summary>
[DesignerCategory("Code")]
public class AFDashboardElementGrid : AFDashboardElement
{
}

/// <summary>
/// Dashboard-Element, dass eine Pivot-Tabelle darstellt
/// </summary>
[DesignerCategory("Code")]
public class AFDashboardElementPivot : AFDashboardElement
{
}


/// <summary>
/// Dashboard-Element, mehrere Dashboard-Elemente enthält
/// </summary>
[DesignerCategory("Code")]
public class AFDashboardElementCompound : AFDashboardElement
{
}

