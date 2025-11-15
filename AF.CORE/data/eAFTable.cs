namespace AF.CORE;

/// <summary>
/// AF3 Tabellen-IDs.
///
/// Alle IDs liegen zwischen 0 und 100.
/// Verwenden Sie keine IDs zwischen 0 und 100 für andere als die in dieser Aufzählung beschriebenen Tabellentypen.
/// </summary>
public enum eAFTable
{
    /// <summary>
    /// Systeminformationstabelle (SYS_INFO)
    /// </summary>
    SysInfo = 1,
    /// <summary>
    /// Persistierung von Werten/Einstellungen etc.
    /// </summary>
    Persistance = 2,
    /// <summary>
    /// ID der Tabelle 'Ordner'
    /// </summary>
    Folder = 3,
    /// <summary>
    /// ID der Tabelle 'Benutzer'
    /// </summary>
    User = 4,
    /// <summary>
    /// ID der Tabelle 'Modul'
    /// </summary>
    Modul = 5,
    /// <summary>
    /// ID der Tabelle 'Rechte'
    /// </summary>
    Right = 6,
    /// <summary>
    /// ID der Tabelle 'Rollen'
    /// </summary>
    Role = 7,
    /// <summary>
    /// ID der Tabelle 'Benutzer-Rolle' (Zuordnung der Rollen zu den Benutzern)
    /// </summary>
    RoleRecht = 8,
    /// <summary>
    /// ID der Tabelle 'Benutzer-Rolle' (Zuordnung der Rollen zu den Benutzern)
    /// </summary>
    UserRole = 9,
    /// <summary>
    /// ID der Tabelle 'Script'
    /// </summary>
    Script = 10,
    /// <summary>
    /// ID der Tabelle 'Log' (Protokoll der Änderungen)
    /// </summary>
    Log = 11,
    /// <summary>
    /// Log-Entry....
    /// </summary>
    LogEntry = 12,
    /// <summary>
    /// ID der Tabelle 'Log-Feld' (Information zu einem geänderten Feld)
    /// </summary>
    LogField = 13,
    /// <summary>
    /// ID der Tabelle 'Login' (Protokoll der ANneldungen)
    /// </summary>
    Login = 14
}


