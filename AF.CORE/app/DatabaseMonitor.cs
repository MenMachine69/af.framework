namespace AF.CORE;

/// <summary>
/// Überwachung von Datenbankoperationen.
/// 
/// Kann verwendet werden, um jede verwendete Abfrage in einer Anwendung zu überwachen, 
/// wie oft sie verwendet wird und wie lange sie zur Ausführung benötigt.
/// 
/// Dieser Monitor verwendet TraceInfo! 
/// </summary>
public sealed class DatabaseMonitor
{
    private Dictionary<string, DatabaseMonitorEntry> results = new();

    /// <summary>
    /// Registriere dies als BeforeExecute-Aktion in der Datenbank.
    /// </summary>
    /// <param name="info"></param>
    public void BeforeExecute(TraceInfo info)
    {
        saveTrace(info);
    }

    /// <summary>
    /// Liste der ausgeführten Abfragen
    /// </summary>
    public IEnumerable<DatabaseMonitorEntry> Entrys => results.Values;

    private void saveTrace(TraceInfo info)
    { 
        string hash = info.CommandText.GetSHA256Hash();

        if (!results.ContainsKey(hash))
            results.Add(hash, new() { Command = info.CommandText });

        var tuple = results[hash];
        tuple.Count += 1;
        tuple.Total.Add(info.TimeSpan);
        tuple.Maximum = (tuple.Maximum > info.TimeSpan ? tuple.Maximum : info.TimeSpan);
        tuple.Minimum = (tuple.Minimum > TimeSpan.Zero && tuple.Minimum < info.TimeSpan ? tuple.Minimum : info.TimeSpan);
        tuple.Average = new TimeSpan(tuple.Total.Ticks / tuple.Count);
    }


    /// <summary>
    /// Registriert dies als AfterExecute-Aktion in der Datenbank.
    /// </summary>
    /// <param name="info"></param>
    public void AfterExecute(TraceInfo info)
    {
        saveTrace(info);
    }
}

/// <summary>
/// Datenbankmonitoring: Informationen zu einer Abfrage
/// </summary>
public class DatabaseMonitorEntry
{
    /// <summary>
    /// ausgeführte Abfrage
    /// </summary>
    public string Command { get; set; } = "";

    /// <summary>
    /// Anzahl der Ausführungen
    /// </summary>
    public int Count { get; set; } = 0;

    /// <summary>
    /// Zeit gesamt für alle Ausführungen
    /// </summary>
    public TimeSpan Total { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// Minimale Ausführungszeit
    /// </summary>
    public TimeSpan Minimum { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// Maximale Ausführungszeit
    /// </summary>
    public TimeSpan Maximum { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// Durchschnittliche Ausführungszeit
    /// </summary>
    public TimeSpan Average { get; set; } = TimeSpan.Zero;
}