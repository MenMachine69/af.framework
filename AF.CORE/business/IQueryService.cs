using System.Data;

namespace AF.CORE;

/// <summary>
/// Unterstützung für Datenbankabfragen/Datenquellen
/// </summary>
public interface IQueryService
{
    /// <summary>
    /// Liefert die Abfrage mit der angegebenen ID
    /// </summary>
    /// <param name="queryId">ID der Abfrage</param>
    /// <returns>Die Abfrage oder NULL wenn keine existiert.</returns>
    IQuery? GetQuery(Guid queryId);

    /// <summary>
    /// Registriert den Typ des Controls über das eine
    /// Query ausgewählt werden kann.
    ///
    /// Das Control/der Typ muss die Schnittstelle IQuerySelectControl implementieren.
    /// </summary>
    void RegisterQuerySelectControlType(Type controlType);

    /// <summary>
    /// Liefert den vorher via RegisterQuerySelectControlType registrierten Typ des Controls für die Query-Auswahl.
    /// </summary>
    Type GetQuerySelectControlType();

    /// <summary>
    /// Einen Query ausführen und Ergebnis als DataTable zurückliefern.
    /// </summary>
    /// <param name="source">Datenquelle (Connectstring, Datenbank etc.), kann/muss null sein, wenn query ein Script o.ä. ist.</param>
    /// <param name="query">Query</param>
    /// <param name="variableValues">Variablen-Werte</param>
    /// <param name="result">DataTable oder NULL</param>
    /// <param name="useCache">Cache verwenden - Daten werden nur einmal aus der Datenquelle gelesen und dann aus dem Cache zur verfügung gestellt.</param>
    /// <param name="cacheID">ID für den Cache (Identifikation der Daten)</param>
    /// <returns>Ergebnis der Ausführung</returns>
    CommandResult ExecuteQuery(IDatabaseConnection? source, IQuery query, IList<VariableUserValue> variableValues, out DataTable? result, bool useCache = false, Guid? cacheID = null);

    /// <summary>
    /// Platzhalter, die die Anwendung unterstützt...
    /// </summary>
    Dictionary<string, IPlaceholder> Placeholders { get; }
}

/// <summary>
/// Interface, dass ein Control implementieren muss, damit es für die Query-Auswahl verwendet werden kann.
/// </summary>
public interface IQuerySelectControl
{
    /// <summary>
    /// Filterbedingung für die anzuzeigenden Querys
    /// </summary>
    Type? QueryTypeFilter { get; set; }
}