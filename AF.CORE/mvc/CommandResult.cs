namespace AF.MVC;

/// <summary>
/// Ergebnis der Ausführung eines Befehls
/// </summary>
public class CommandResult
{
    /// <summary>
    /// Erstellen eines neuen Befehls result (Result = None)
    /// </summary>
    public static CommandResult None => new() { Result = eNotificationType.None };

    /// <summary>
    /// Typ des Ergebnisses
    /// </summary>
    public eNotificationType Result { get; set; } = eNotificationType.None;

    /// <summary>
    /// Liste der ermittelten Fehler
    /// </summary>
    public ValidationErrorCollection? Errors { get; set; }

    /// <summary>
    /// eine Nachricht, die das Ergebnis beschreibt
    /// </summary>
    public string ResultMessage { get; set; } = string.Empty;

    /// <summary>
    /// eine Ausnahme, die bei der Ausführung des Befehls erzeugt wurde
    /// </summary>
    public Exception? Exception { get; set; }

    /// <summary>
    /// ein Ergebnisobjekt (z.B. ein während der Ausführung erstelltes Datenobjekt)
    /// </summary>
    public object? ResultObject { get; set; }

    /// <summary>
    /// Erzeugt ein neues Befehlsergebnis mit einem Ergebnisobjekt vom Typ error
    /// </summary>
    /// <param name="message">Fehlermeldung</param>
    /// <param name="errors">Liste der ermittelten Fehler</param>
    /// <returns>das Befehlsergebnisobjekt</returns>
    public static CommandResult Error(string message, ValidationErrorCollection? errors = null)
    {
        return new CommandResult
        {
            Result = eNotificationType.Error,
            ResultMessage = message,
            Errors = errors
        };
    }

    /// <summary>
    /// Erzeugt ein neues Befehlsergebnis mit einem Ergebnisobjekt vom Typ error
    /// </summary>
    /// <param name="message">Fehlermeldung</param>
    /// <param name="exception">die Ausnahme</param>
    /// <returns>das Befehlsergebnisobjekt</returns>
    public static CommandResult Error(string message, Exception exception)
    {
        return new CommandResult
        {
            Result = eNotificationType.Error,
            ResultMessage = message,
            Exception = exception
        };
    }

    /// <summary>
    /// Erzeugt ein neues Befehlsergebnis mit einem Ergebnisobjekt vom Typ warning
    /// </summary>
    /// <param name="message">Warnungsmeldung</param>
    /// <returns>das Befehlsergebnisobjekt</returns>
    public static CommandResult Warning(string message)
    {
        return new CommandResult
        {
            Result = eNotificationType.Warning,
            ResultMessage = message
        };
    }

    /// <summary>
    /// Erzeugt ein neues Befehlsergebnis mit einem Ergebnisobjekt vom Typ warning
    /// </summary>
    /// <param name="message">Warnungsmeldung</param>
    /// <param name="exception">die Ausnahme</param>
    /// <returns>das Befehlsergebnisobjekt</returns>
    public static CommandResult Warning(string message, Exception exception)
    {
        return new CommandResult
        {
            Result = eNotificationType.Warning,
            ResultMessage = message,
            Exception = exception
        };
    }

    /// <summary>
    /// Erzeugt ein neues Befehlsergebnis mit einem Ergebnisobjekt vom Typ info
    /// </summary>
    /// <param name="message">info message</param>
    /// <returns>das Befehlsergebnisobjekt</returns>
    public static CommandResult Info(string message)
    {
        return new CommandResult
        {
            Result = eNotificationType.Information,
            ResultMessage = message
        };
    }

    /// <summary>
    /// Erzeugt ein neues Befehlsergebnis mit einem Ergebnisobjekt vom Typ Success.
    ///
    /// Zusätzlich zu Info kann Success das Objekt enthalten, auf das die die Meldung bezieht.
    /// </summary>
    /// <param name="message">info message</param>
    /// <param name="result">Objekt, auf das sich die Meldung bezieht (z.B. neu angelegtes Model etc.)</param>
    /// <returns>das Befehlsergebnisobjekt</returns>
    public static CommandResult Success(string message, object? result = null)
    {
        return new CommandResult
        {
            Result = eNotificationType.Success,
            ResultMessage = message,
            ResultObject = result
        };
    }
}
