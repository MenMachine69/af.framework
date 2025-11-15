namespace AF.CORE;

/// <summary>
/// Art der Benachrichtigung
/// </summary>
public enum eHubEventType
{
    /// <summary>
    /// Neues Modell hinzugefügt
    /// </summary>
    ObjectAdded = 0,

    /// <summary>
    /// Modell geändert
    /// </summary>
    ObjectChanged = 1,

    /// <summary>
    /// Modell gelöscht
    /// </summary>
    ObjectDeleted = 2,

    /// <summary>
    /// Benutzerdefinierte, unspezifische Nachricht
    /// </summary>
    Custom = 3
}

