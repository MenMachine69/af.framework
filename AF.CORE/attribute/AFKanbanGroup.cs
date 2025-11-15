namespace AF.CORE;

/// <summary>
/// Gruppe/Spalte in einem Kanban-View
/// </summary>
public class AFKanbanGroup
{
    /// <summary>
    /// Überschrift/Beschriftung der Spalte
    /// </summary>
    public string Caption { get; set; } = "";

    /// <summary>
    /// Name der Spalte
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Wert, der in der Datenquelle die Gruppe repräsentiert
    ///
    /// Datensätze, die den Wert enthalten, werden in dieser Gruppe dargestellt.
    /// </summary>
    public object GroupValue { get; set; } = new();

}