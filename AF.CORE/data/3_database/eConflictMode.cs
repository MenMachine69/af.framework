namespace AF.DATA;

/// <summary>
/// Behandlung von Konflikten beim Speichern in einer Datenbank
/// </summary>
public enum eConflictMode
{
    /// <summary>
    /// Die erste gewinnt, weitere Änderungen werden abgelehnt
    /// </summary>
    FirstWins,
    /// <summary>
    /// der letzte gewinnt - Änderungen werden nie abgelehnt
    /// </summary>
    LastWins
}