namespace AF.DATA;

/// <summary>
/// Konstanten für datenbankspezifische SQL-Anweisungen
/// </summary>
public enum eDatabaseConstant
{
    /// <summary>
    /// ASC/ASCENDING
    /// </summary>
    asc,
    /// <summary>
    /// DESC/DESCENDING
    /// </summary>
    desc,
    /// <summary>
    /// UNIQUE
    /// </summary>
    unique,
    /// <summary>
    /// NOT UNIQUE
    /// </summary>
    notunique
}

/// <summary>
/// Schnittstelle für eine Datenbank, die Tabellen und Ansichten enthalten kann
/// </summary>
public interface IDatabase : IDatabaseConnection
{
    /// <summary>
    /// letzte interne Ausnahme
    /// </summary>
    public Exception? LastException { get; set; }

    /// <summary>
    /// Zuletzt ausgeführte Datenbankabfrage
    /// </summary>
    string? LastQuery { get; set; }

    /// <summary>
    /// nächste auszuführende Datenbankabfrage
    /// </summary>
    string? NextQuery { get; set; }

    /// <summary>
    /// Maximale Anzahl von Abfragen im Abfragepuffer
    ///
    /// Standardwert ist 100.
    /// </summary>
    int MaxQueryBufferSize {get; set;}

    /// <summary>
    /// Konfiguration der Datenbank
    /// </summary>
    IConfiguration Configuration { get; set; }

    /// <summary>
    /// Öffnet eine Verbindung zu dieser Datenbank.
    /// </summary>
    /// <returns>Datenbankverbindung</returns>
    IConnection GetConnection();

    /// <summary>
    /// Öffnet eine Verbindung zur Datenbank mit Admin-Anmeldedaten.
    /// </summary>
    /// <returns>Datenbankverbindung</returns>
    IConnection GetAdminConnection();

    /// <summary>
    /// Übersetzt den Namen eines Feldes, einer Tabelle oder eines Views in das 
    /// richtige Format für diese Datenbank unter Verwendung von Datenbankbenennungskonventionen
    /// </summary>
    /// <param name="original"></param>
    /// <returns>übersetzter Name</returns>
    string GetName(string original);

    /// <summary>
    /// Übersetzt eine Konstante in eine datenbankspezifische Zeichenkette
    /// </summary>
    /// <param name="constant">Konstante</param>
    /// <returns>die Zeichenkette</returns>
    string GetConstant(eDatabaseConstant constant);

    /// <summary>
    /// Namenskonventionen für Tabellen und Felder
    /// 
    /// Standard ist eDatabaseNamingScheme.original
    /// </summary>
    eDatabaseNamingScheme NamingConventions { get; set; }
    
    /// <summary>
    /// Übersetzer für diese Datenbank.
    /// </summary>
    ITranslator Translator { get; }
    
    /// <summary>
    /// Datenbank erstellen, falls nicht vorhanden.
    /// </summary>
    void Create();

    /// <summary>
    /// Überprüft Datenbanktypen (Tabellen und Ansichten)
    /// </summary>
    /// <param name="tableTypes">Basistypen der Tabellen</param>
    /// <param name="viewTypes">Basistypen der Views</param>
    /// <param name="feedback">Feedback während der Prüfung</param>
    /// <param name="force">Datenbank erzwingen/vollständig prüfen</param>
    /// <returns>true wenn ok, sonst false</returns>
    bool Check(List<Type> tableTypes, List<Type> viewTypes, Action<string>? feedback, bool force);

    /// <summary>
    /// Prüfen, ob die Datenbank existiert.
    /// </summary>
    bool Exist { get; }
    

    /// <summary>
    /// Aktion, die - bei eingeschalteter Ablaufverfolgung - vor der Ausführung einer SQL-Anweisung ausgeführt wird.
    /// </summary>
    Action<TraceInfo>? TraceBeforeExecute { get; set; }

    /// <summary>
    /// Aktion, die - bei eingeschalteter Ablaufverfolgung - nach der Ausführung einer SQL-Anweisung ausgeführt wird.
    /// </summary>
    Action<TraceInfo>? TraceAfterExecute { get; set; }

    /// <summary>
    /// Aktion, die ausgeführt werden soll, nachdem ein Datenobjekt geändert/gespeichert wurde (wenn die Ablaufverfolgung eingeschaltet ist).
    /// </summary>
    Action<IDataObject>? AfterSave { get; set; }

    /// <summary>
    /// Aktion, die nach dem Löschen eines Datenobjekts ausgeführt wird (wenn die Verfolgung eingeschaltet ist).
    /// </summary>
    Action<IDataObject>? AfterDelete { get; set; }

    /// <summary>
    /// eine Fabrik, die ILogger-Objekte erzeugen kann
    /// </summary>
    ILogger? Logger { get; set; }

    /// <summary>
    /// Liest ein Objekt aus dem Cache
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>
    /// <param name="id">eindeutige ID des Objekts</param>
    /// <returns>das Objekt oder NULL</returns>
    T? FromCache<T>(Guid id) where T : class, IDataObject, new();

    /// <summary>
    /// Ein Objekt im Cache speichern
    /// </summary>
    /// <param name="id">Id des Objekts</param>
    /// <param name="value">Wert des Objekts</param>
    public void ToCache(Guid id, IDataObject value);

    /// <summary>
    /// Alle Einträge aus dem Cache entfernen
    /// </summary>
    public void ClearCache();

    /// <summary>
    /// entfernt alle Einträge von Type aus dem Cache
    /// </summary>
    /// <param name="entryType">Typ der zu entfernenden Einträge</param>
    public void ClearCache(Type entryType);

    /// <summary>
    /// entfernt den Eintrag mit der angegebenen ID aus dem Cache
    /// </summary>
    /// <param name="id">Id des Eintrags</param>
    public void ClearCache(Guid id);
}