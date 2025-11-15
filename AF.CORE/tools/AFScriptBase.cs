namespace AF.CORE;

/// <summary>
/// Basisklasse für ausführbare Scripts
/// </summary>
public abstract class AFScriptBase
{
    /// <summary>
    /// Variablen des Scripts für die Scriptausführung
    /// </summary>
    public Dictionary<string, object?> Variablen { get; } = new();

    /// <summary>
    /// Log der Scriptausführung
    /// </summary>
    public Log Log { get; } = new();

    /// <summary>
    /// Lädt eine der übergebenen Variablen
    /// </summary>
    /// <typeparam name="T">Zurückzugebener Typ</typeparam>
    /// <param name="key">Name der Variable</param>
    /// <returns></returns>
    public T GetVariable<T>(string key)
    {
        object? variable = GetVariable(key);
        if (variable is T t)
            return t;
        return default!;
    }

    /// <summary>
    /// Lädt eine der übergebenen Variablen
    /// </summary>
    /// <param name="key">Name der Variable</param>
    /// <returns></returns>
    public object? GetVariable(string key) => Variablen.TryGetValue(key, out object? value) ? value : null;

    /// <summary>
    /// Fügt dem Log eine Nachricht hinzu
    /// </summary>
    /// <param name="message">Nachricht</param>
    /// <param name="description">Beschreibung (optional)</param>
    public virtual void AddLogMessage(string message, string description = "") => Log.AddMessage(message, description);

    /// <summary>
    /// Fügt dem Log eine Warnung hinzu
    /// </summary>
    /// <param name="message">Nachricht</param>
    /// <param name="description">Beschreibung (optional)</param>
    public virtual void AddLogWarning(string message, string description = "") => Log.AddWarning(message, description);

    /// <summary>
    /// Fügt dem Log eine Fehlermeldung hinzu
    /// </summary>
    /// <param name="message">Nachricht</param>
    /// <param name="description">Beschreibung (optional)</param>
    public virtual void AddLogError(string message, string description = "") => Log.AddError(message, description);

    /// <summary>
    /// Wird vor der Scriptausführung ausgeführt
    /// </summary>
    public virtual void BeforeExecute() { }

    /// <summary>
    /// Wird nach der Scriptausführung ausgeführt
    /// </summary>
    public virtual void AfterExecute() { }
}
