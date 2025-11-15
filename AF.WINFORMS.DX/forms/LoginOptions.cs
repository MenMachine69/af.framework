namespace AF.WINFORMS.DX;

/// <summary>
/// Anmeldeinformationen und Feedback bei Anmeldung
/// </summary>
public class LoginOptions
{
    /// <summary>
    /// Anmeldename
    /// </summary>
    public string LoginName { get; set; } = "";

    /// <summary>
    /// Kennwort
    /// </summary>
    public string Password { get; set; } = "";

    /// <summary>
    /// Rückgabe/Meldung
    /// </summary>
    public string Feedback { get; set; } = "";

    /// <summary>
    /// Art der Rückgabe/Meldung
    /// </summary>
    public eNotificationType FeedbackType { get; set; } = eNotificationType.None;

    /// <summary>
    /// Erfolgreich ja/Nein
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Unattended login versuchen
    /// </summary>
    public bool TryUnattendedLogin { get; set; }
}

/// <summary>
/// Anmeldeinformationen und Feedback für Anlegen eines neuen Benutzers (Admin beim ersten Start)
/// </summary>
public class UserCreateOptions
{
    /// <summary>
    /// Anmeldename
    /// </summary>
    public string LoginName { get; set; } = "";

    /// <summary>
    /// Kennwort
    /// </summary>
    public string Password { get; set; } = "";

    /// <summary>
    /// Erfolgreich ja/Nein
    /// </summary>
    public bool Success { get; set; }
}

