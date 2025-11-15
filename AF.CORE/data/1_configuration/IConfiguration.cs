namespace AF.DATA;

/// <summary>
/// Schnittstelle zur Implementierung durch Klassen, die Datenbankkonfigurationen darstellen.
/// </summary>
public interface IConfiguration
{
    /// <summary>
    /// Verbindungszeichenfolge für die Datenbank.
    ///
    /// In Fällen, in denen eine Master-Slave-Replikation verwendet wird, ist dies der Verbindungsstring
    /// für die Master-Datenbank (primär zum Schreiben).
    /// </summary>
    /// <returns>Gültiger Verbindungsring für den Zugriff auf die Datenbank basierend auf den Einstellungen</returns>
    string ConnectionString { get; }

    /// <summary>
    /// Name der Datenbank
    /// </summary>
    string DatabaseName { get; }

    /// <summary>
    /// Konfliktbehandlung für die Datenbank
    /// </summary>
    eConflictMode ConflictMode { get; }

    /// <summary>
    /// Datenbanktyp
    /// </summary>
    eDatabaseType DatabaseType { get; }

    /// <summary>
    /// Gibt an, ob Spalten/Felder der Tabellen bei der Aktualisierung gelöscht werden können, 
    /// wenn es in der Klasse des Modells kein Feld mehr für diese Spalte gibt.
    /// </summary>
    bool AllowDropColumns { get; set; }

    /// <summary>
    /// Benutzeranmeldedaten für die Datenbank (Master)
    /// </summary>
    Credentials? Credentials { get; set; }

    /// <summary>
    /// Admin-Anmeldedaten für die Datenbank (Master)
    /// </summary>
    Credentials? AdminCredentials { get; set; }

    /// <summary>
    /// Erstellen eines Datenbankobjekts basierend auf der Konfiguration
    /// </summary>
    /// <returns>Datenbank</returns>
    IDatabase Create();

    /// <summary>
    /// gibt die Verbindungszeichenfolge für die Datenbank zurück
    /// </summary>
    /// <param name="admin">ein Verbindungsstring mit Admin-Zugangsdaten</param>
    string GetConnectString(bool admin);

    /// <summary>
    /// Liste der Basistypen für Tabellen
    /// </summary>
    public List<Type> BaseTableTypes { get; set; }

    /// <summary>
    /// Liste der Basistypen für Views
    /// </summary>
    public List<Type> BaseViewTypes { get; set; }
}