namespace AF.CORE;

/// <summary>
/// ein Formular/Steuerelement, das das Ergebnis eines Befehls anzeigen kann
/// </summary>
public interface ICommandResultDisplay
{
    /// <summary>
    /// kann dieses Formular/Steuerelement das Ergebnis eines Befehls anzeigen
    /// </summary>
    bool SupportHandleResult { get; }

    /// <summary>
    /// zeigt das Ergebnis eines Befehls an
    /// </summary>
    /// <param name="result">result</param>
    void HandleResult(CommandResult result);
}