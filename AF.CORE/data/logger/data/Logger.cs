namespace AF.DATA;

/// <summary>
/// Standard-Logger für Änderungen in der Datenbank.
///
/// Nutzt zur Protokollierung zwei Tabellen in der im Konstruktor übergebenen Datenbank.
///
/// Für diese Tabellen werden die Models LogEntry und LogEntryField verwendet.
/// Für diese Models stehen auch entsprechende Controller zur Verfügung, die z.B. zur Anzeige der Logs verwendet werden können.
/// </summary>
public sealed class Logger : BaseLogger<LogEntry, LogEntryField>
{
    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="db">Datenbank in welche die Änderungen geschrieben werden</param>
    public Logger(IDatabase db) : base(db)
    {

    }
}