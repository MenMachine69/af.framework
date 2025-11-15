namespace AF.DATA;

/// <summary>
/// Tracing Informationen
/// </summary>
public class TraceInfo
{
    /// <summary>
    /// Erzeugt Tracing-Informationen für den Befehl
    /// </summary>
    /// <param name="command">ausgeführter Befehl</param>
    public TraceInfo(string command)
    {
        CommandText = command;
        TimeStamp = DateTime.Now;
    }

    /// <summary>
    /// ausgeführter Befehl
    /// </summary>
    public string CommandText { get; init; }

    /// <summary>
    /// Parameter für den Befehl (kann leer sein)
    /// </summary>
    public object[]? CommandParameters { get; set; }

    /// <summary>
    /// Zeitstempel für die Befehlsausführung
    /// </summary>
    public DateTime TimeStamp { get; set; }

    /// <summary>
    /// Zeitspanne für die Ausführung
    /// </summary>
    public TimeSpan TimeSpan { get; set; }
}