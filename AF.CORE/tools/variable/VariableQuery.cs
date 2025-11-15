using System.Data;

namespace AF.CORE;

/// <summary>
/// Variable, aus der ein Wert ausgewählt werden kann
/// aus einer Liste/Tabelle einer Datenbank
/// </summary>
[Serializable]
public class VariableQuery : VariableBase
{
    /// <summary>
    /// ID der auszuführenden Abfrage
    /// </summary>
    [AFBinding]
    [AFContext("Datenquelle", Description = "Datenquelle die genutzt werden soll.")]
    public Guid QueryId { get; set; } = Guid.Empty;

    /// <summary>
    /// Name der Spalte, die den Wert enthält
    /// </summary>
    [AFBinding]
    [AFContext("Spalte Wert", Description = "Spalte, deren Wert übernommen wird.")]
    public string ValueColumn { get; set; } = string.Empty;

    /// <summary>
    /// Name der Spalte, die den Anzeigewert enthält
    /// </summary>
    [AFBinding]
    [AFContext("Spalte Anzeige", Description = "Spalte, deren Wert bei der Auswahl angezeigt wird.")]
    public string DisplayColumn { get; set; } = string.Empty;

    /// <summary>
    /// Wertespalte im Raster anzeigen
    /// </summary>
    [AFBinding]
    [AFContext("Spalte Wert anzeigen", Description = "Gibt an, ob die Spalte mit den Werten bei der Auswahl angezeigt werden soll.")]
    public bool DisplayValueColumn { get; set; } = false;

    /// <summary>
    /// DatenTabelle laden
    /// </summary>
    /// <returns>Datentabelle</returns>
    public DataTable? GetData(IList<VariableUserValue> values) 
    {
        if (AFCore.App.QueryService == null) throw new NullReferenceException(@"No query service provided (AFCore.App.QueryService).");

        if (QueryId.Equals(Guid.Empty)) throw new InvalidOperationException($@"No QueryID assigned to variable {Parent?.VAR_NAME}.");

        IQuery? query = AFCore.App.QueryService.GetQuery(QueryId);

        if (query == null) throw new NullReferenceException($@"Query not available {QueryId} (Variable {Parent?.VAR_NAME})");

        return query.GetData(values);
    }
}