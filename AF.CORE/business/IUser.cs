namespace AF.BUSINESS;

/// <summary>
/// Definition eines Benutzers
/// </summary>
public interface IUser
{
    /// <summary>
    /// Eindeutige ID des Benutzers 
    /// (z.B. zur Identifikation in einer Datenbank)
    /// </summary>
    Guid UserId { get; set; }

    /// <summary>
    /// Anmeldename des Benutzers.
    ///
    /// Wenn SignleSignOn genutzt werden soll muss dieser Anmeldename 
    /// dem Windows-Benutzernamen entsprechen.
    /// </summary>
    string UserLoginName { get; set; }

    /// <summary>
    /// Kennwort des Benutzers-
    ///
    /// Nur während des Anmeldevorgangs verfügbar. Nach dem Login ist dieser Wert immer leer.
    /// </summary>
    string? UserPassword { get; set; }

    /// <summary>
    /// Der name des Benutzers zur Anzeige in der UI
    /// </summary>
    string UserName { get; set; }

    /// <summary>
    /// Benutzer ist Administrator
    /// </summary>
    bool IsAdmin { get; }

    /// <summary>
    /// Benutzer ist gesperrt (kann sich nicht anmelden)
    /// </summary>
    bool IsLocked { get; }

    /// <summary>
    /// IDs der dem Benutzer zugewiesenen Rollen
    /// </summary>
    IEnumerable<Guid> Roles { get; }

    /// <summary>
    /// Die Rechte des Benutzers.
    ///
    /// Dieses BitArray ist die Zusammenfassung aller einzelnen
    /// Rechte/BitArrays in den dem Benutzer zugewiesenen Rollen.
    /// </summary>
    AFBitArray UserRights { get; }

    /// <summary>
    /// Prüft ob der Benutzer eines bestimmte Berechtigung hat
    /// </summary>
    /// <param name="right">Berechtigung (Index des Bits im BitArray)</param>
    /// <returns>true wenn der Benutzer berechtigt ist, sonst false</returns>
    bool HasRight(int right);
}

/// <summary>
/// Schnittstelle für ein UserController-Objekt (Controller für Benutzerobjekte)
/// </summary>
public interface IUserController
{
    /// <summary>
    /// liefert einen Benutzer (oder null, falls nicht vorhanden) anhand seines Anmeldenamens
    /// </summary>
    /// <param name="loginName">Loginname des Benutzers</param>
    /// <returns>der Benutzer oder null</returns>
    IUser? GetUser(string loginName);

    /// <summary>
    /// Weisen Sie einem Benutzer eine Rolle zu
    /// </summary>
    /// <param name="user">Benutzerobjekt</param>
    /// <param name="role">Rollenobjekt</param>
    void AssignRole(IUser user, IRole role);

    /// <summary>
    /// Versuch, sich unbeaufsichtigt anzumelden (ohne Passwort)
    /// </summary>
    /// <param name="loginName">Benutzername für die Anmeldung</param>
    /// <param name="feedback">Variable zur Rückgabe einer Rückmeldung über die Anmeldung</param>
    /// <returns>das Benutzerobjekt oder null, wenn nicht erfolgreich</returns>
    IUser? TryLoginUnattend(string loginName, out string feedback);

    /// <summary>
    /// Versuch der Anmeldung (mit Passwort)
    /// </summary>
    /// <param name="loginName">Benutzername für die Anmeldung</param>
    /// <param name="password">Passwort für die Anmeldung</param>
    /// <param name="feedback">Variable zur Rückgabe einer Rückmeldung über die Anmeldung</param>
    /// <returns>das Benutzerobjekt oder null, wenn nicht erfolgreich</returns>
    IUser? TryLogin(string loginName, string password, out string feedback);
}

/// <summary>
/// Zentrale Erweiterungsmethoden für IUser und IUserController, 
/// die dann im konkreten Implementierungen immer automatisch zur verfügung stehen.
/// </summary>
public static class IUserEx
{

}