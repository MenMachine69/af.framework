using System.Data;

namespace AF.CORE;

/// <summary>
/// Abstrakte Basisklasse für den QueryService...
/// </summary>
public abstract class QueryServiceBase
{
    private Type? querySelectControlType;
    private Dictionary<Guid, DataTable> cache = [];

    /// <summary>
    /// Registriert den Typ des Controls über das ein
    /// Script ausgewählt werden kann.
    ///
    /// Das Control/der Typ muss die Schnittstelle IScriptSelectControl implementieren.
    /// </summary>
    /// <param name="controlType"></param>
    public void RegisterQuerySelectControlType(Type controlType)
    {
        if (!controlType.HasInterface(typeof(IQuerySelectControl)))
            throw new ArgumentException($"Der Typ {controlType.FullName} muss das Interface IQuerySelectControl implementieren.", nameof(controlType));

        querySelectControlType = controlType;
    }

    /// <summary>
    /// Liefert den vorher via RegisterQuerySelectControlType registrierten Typ des Controls für die Query-Auswahl.
    /// </summary>
    /// <exception cref="NullReferenceException">Wenn kein Typ registriert wurde (via RegisterQuerySelectControlType)</exception>
    public Type GetQuerySelectControlType()
    {
        if (querySelectControlType == null) throw new NullReferenceException("Es wurde kein ControlType zur Auswahl der Datenquelle registriert (via RegisterQuerySelectControlType).");

        return querySelectControlType;
    }

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
    public CommandResult ExecuteQuery(IDatabaseConnection? source, IQuery query, IList<VariableUserValue> variableValues, out DataTable? result, bool useCache = false, Guid? cacheID = null)
    {
        result = null;

        if (useCache && cacheID != null && cache.ContainsKey((Guid)cacheID))
        {
            result = cache[(Guid)cacheID];
            return CommandResult.Success("Erfolgreich.");
        }

        if (source == null)
        {
            result = query.GetData(variableValues);
            
            if (result.Rows.Count > 0 && variableValues.Count < 1 && useCache && cacheID != null && cacheID != Guid.Empty)
                cache.TryAdd((Guid)cacheID, result);

            return CommandResult.Success("Erfolgreich.");
        }


        if (query is IScript script)
        {
            // Datenquelle ist ein Script, der ein DataTable liefert.
        }
        else 
        {
            // Datenquelle ist eine SQL Datenquelle

            using var conn = source.Connect();
            using var command = source.GetCommand(query.Query);

            result = source.ExecuteTable(conn, command);

            if (result.Rows.Count > 0 && variableValues.Count < 1 && useCache && cacheID != null && cacheID != Guid.Empty)
                cache.TryAdd((Guid)cacheID, result);
        }

        result ??= new DataTable();

        return CommandResult.Success("Erfolgreich.");

    }
}