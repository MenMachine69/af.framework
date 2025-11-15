namespace AF.CORE;

/// <summary>
/// Protokoll für die Protokollierung
/// </summary>
[Serializable]
public class Log : IErrorProvider
{
    private WeakEvent<EventHandler<EventArgs>>? notify;

    /// <summary>
    /// Liste der Protokolleinträge
    /// </summary>
    public List<LogEntry> Entries { get; set; } = [];

    /// <summary>
    /// Gibt True zurück, wenn das Protokoll mindestens eine Fehlermeldung enthält.
    /// </summary>
    public bool HasErrors => Count(eNotificationType.Error) > 0 || Count(eNotificationType.SystemError) > 0;

    /// <summary>
    /// Gibt True zurück, wenn das Protokoll mindestens eine Warnmeldung enthält.
    /// </summary>
    public bool HasWarnings => Count(eNotificationType.Warning) > 0;

    /// <summary>
    /// Ereignis, das ausgelöst wird, wenn ein Log-Eintrag hinzugefügt wurde.
    /// 
    /// Sender ist der LogEintrag selbst.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public event EventHandler<EventArgs> OnNotify
    {
        add
        {
            notify ??= new();
            notify.Add(value);
        }
        remove => notify?.Remove(value);
    }

    /// <summary>
    /// Nachricht hinzufügen
    /// </summary>
    /// <param name="message">Nachricht</param>
    /// <param name="description">Beschreibung</param>
    public void AddMessage(string message, string description = "")
    {
        _add(eNotificationType.Information, message, description);
    }

    /// <summary>
    /// Fehlermeldung hinzufügen
    /// </summary>
    /// <param name="message">Meldung</param>
    /// <param name="description">Beschreibung</param>
    public void AddError(string message, string description = "")
    {
        _add(eNotificationType.Error, message, description);
    }

    /// <summary>
    /// Warnmeldung hinzufügen
    /// </summary>
    /// <param name="message">Meldung</param>
    /// <param name="description">Beschreibung</param>
    public void AddWarning(string message, string description = "")
    {
        _add(eNotificationType.Warning, message, description);
    }

    private void _add(eNotificationType logEntryType, string message, string description)
    {
        Entries.Add(new LogEntry
        {
            Timestamp = DateTime.Now,
            MsgType = logEntryType,
            Message = message,
            Description = description
        });

        notify?.Raise(Entries.Last(), EventArgs.Empty);
    }


    /// <summary>
    /// Ermittelt die Anzahl der Einträge eines Typs
    /// </summary>
    /// <param name="type">Nachrichtentyp</param>
    /// <returns>Anzahl der Einträge im Protokoll</returns>
    public int Count(eNotificationType type)
    {
        return Entries.Count(ent => ent.MsgType == type);
    }

    /// <summary>
    /// Schreibt Fehler für eine Eigenschaft in das Protokoll
    /// </summary>
    /// <param name="propertyName">Name der Eigenschaft</param>.
    /// <param name="errorMessage">Fehlermeldung</param>
    public void SetError(string propertyName, string errorMessage)
    {
        AddError($@"{propertyName}: {errorMessage}");
    }

    /// <summary>
    /// Schreibt eine Warnung für eine Eigenschaft in das Protokoll
    /// </summary>
    /// <param name="propertyName">Name der Eigenschaft</param>.
    /// <param name="errorMessage">Fehlermeldung</param>
    public void SetWarning(string propertyName, string errorMessage)
    {
        AddWarning($@"{propertyName}: {errorMessage}");
    }

    /// <summary>
    /// Alle aktuellen Einträge aus dem Protokoll entfernen
    /// </summary>
    public void Clear()
    {
        Entries.Clear();
    }

    /// <summary>
    /// Hinzufügen von Fehlermeldungen aus einer ValidationErrorCollection zum Protokoll
    /// </summary>
    /// <param name="errors">Fehlermeldungen</param>
    public void FromCollection(ValidationErrorCollection errors)
    {
        errors.ForEach(e => AddError(e.PropertyName, e.Message));
    }
}

/// <summary>
/// Eintrag in ein Protokoll
/// </summary>
[Serializable]
public class LogEntry
{
    /// <summary>
    /// Timestamp
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Nachrichtentyp
    /// </summary>
    public eNotificationType MsgType { get; set; }

    /// <summary>
    /// Nachricht
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Erweiterte Beschreibung der Nachricht
    /// </summary>
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Art der Benachrichtigung
/// 
/// Unter anderem zur Verwendung in Meldungsboxen oder zur Anzeige von Meldungen
/// </summary>
public enum eNotificationType
{

    /// <summary>
    /// Erfolgreich
    /// Zusätzlich zu Info kann Success das Objekt enthalten, auf das die die Meldung bezieht.
    /// </summary>
    Success,

    /// <summary>
    /// Information/Erfolg
    /// </summary>
    Information,

    /// <summary>
    /// Warnung
    /// </summary>
    Warning,

    /// <summary>
    /// Fehler
    /// </summary>
    Error,

    /// <summary>
    /// Systemfehler
    /// </summary>
    SystemError,

    /// <summary>
    /// unspezifisch
    /// </summary>
    None
}

