namespace AF.BUSINESS;

/// <summary>
/// Interface für Platzhalter
/// </summary>
public interface IPlaceholder
{
    /// <summary>
    /// Name des Platzhalters
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Beschreibung des Platzhalters
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gibt an, ob der Platzhalter eine Flee-Expression ist.
    /// </summary>
    bool IsExpression { get; }

    /// <summary>
    /// Ausdruck/Inhalt des Platzhalters.
    /// 
    /// Der Platzhalter wird durch diesen Inhalt/das Ergebnis des Ausdrucks ersetzt.
    /// </summary>
    string Expression { get; }

    /// <summary>
    /// Kennzeichnet einen Platzthalter als vordefinierten Standard-Platzhalter.
    /// </summary>
    bool System { get; }
}

/// <summary>
/// Erweiterungen für alle IPlaceholder-Objekte
/// </summary>
public static class IPlaceholderEx
{
    /// <summary>
    /// Wert als Text ausgeben
    /// </summary>
    /// <returns></returns>
    public static string GetValue(this IPlaceholder ph)
    {
        if (ph.IsExpression)
        {
            try
            {
                return AFCore.App.ScriptingService?.EvaluateExpression(ph.Expression)?.ToString() ?? "<n. verfügbar>" ;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fehler in der Definition des Platzhalters {ph.Name}. " + ex.Message);
            }
        }

        return ph.Expression;
    }
}

/// <summary>
/// System-Platzhalter
/// </summary>
public sealed class SystemPlatzhalter : IPlaceholder
{
    /// <summary>
    /// Name des Platzhalters
    /// </summary>
    public string Name { get; set;  } = "";

    /// <summary>
    /// Beschreibung des Platzhalters
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    /// Gibt an, ob der Platzhalter eine Flee-Expression ist.
    /// </summary>
    public bool IsExpression { get; set; } = false;

    /// <summary>
    /// Ausdruck/Inhalt des Platzhalters.
    /// 
    /// Der Platzhalter wird durch diesen Inhalt/das Ergebnis des Ausdrucks ersetzt.
    /// </summary>
    public string Expression { get; set; } = "";

    /// <summary>
    /// Kennzeichnet einen Platzthalter als vordefinierten Standard-Platzhalter.
    /// </summary>
    public bool System => true;
}