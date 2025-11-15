namespace AF.BUSINESS;

/// <summary>
/// Schnittstelle für einen SecurityService, der dem AF3 zugeordnet werden kann
/// zugewiesen werden kann und Funktionen für User-Login, Rechteprüfung etc. bereitstellt.  
/// </summary>
public interface ISecurityService
{
    /// <summary>
    /// Wenn ein Benutzer, der Administrator ist auch immer alle Rechte hat, liefert dieser Wert true.
    /// </summary>
    bool AdminHasAllRights { get; set; }

    /// <summary>
    ///Der aktuell angemeldete Benutzer (wenn login erfolgreich war)
    /// </summary>
    IUser? CurrentUser { get; set; }

    /// <summary>
    /// Legt eine Rolle an, die ALLE Berechtigungen hat.
    /// 
    /// Die Rolle wird nur angelegt, wenn noch keine Rolle 
    /// mit dem angegebenen Namen existiert.
    /// </summary>
    /// <returns>die neu angelegte oder bereits vorhandene Rolle</returns>
    IRole CreateAllRightsRole();

    /// <summary>
    /// Legt eine Rolle an, die KEINE Berechtigungen hat.
    /// 
    /// Die Rolle wird nur angelegt, wenn noch keine Rolle 
    /// mit dem angegebenen Namen existiert.
    /// </summary>
    /// <returns>die neu angelegte oder bereits vorhandene Rolle</returns>
    IRole CreateNoRightsRole();

    /// <summary>
    /// Eine Rolle einem Benutzer zuweisen.
    /// 
    /// Jedem Benutzer können belibig viele Rollen zugewiesen werden, 
    /// deren Berechtigungen kombiniert werden (UND Verknüpfung)
    /// </summary>
    /// <param name="user">Benutzer dem die Rolle zugewiesen wird</param>
    /// <param name="role">Zuzuweisende Rolle</param>
    void AssignRoleToUser(IUser user, IRole role);


    /// <summary>
    /// Prüft ob der Benutzer eine bestimmte Berechtigung hat.
    /// </summary>
    /// <param name="right">ID der zu prüfenden Berechtigung</param>
    /// <returns>true, wenn der Benutzer berechtigt ist, sonst false</returns>
    bool HasRight(int right);

    /// <summary>
    /// Prüft ob der Benutzer eine bestimmte Berechtigung hat.
    /// </summary>
    /// <param name="user">Benutzer dessen berechtigung geprüft wird</param>
    /// <param name="right">ID der zu prüfenden Berechtigung</param>
    /// <returns>true, wenn der Benutzer berechtigt ist, sonst false</returns>
    bool HasRight(IUser user, int right);

    /// <summary>
    /// Liefert einen Benutzer anhand seines Anmelde-/Benutzernamens 
    /// oder NULL, wenn kein Benutzer mit dem angegebenen Namen existiert.
    /// </summary>
    /// <param name="loginName">Anmeldename des Benutzers</param>
    /// <returns>der Benutzer oder NULL</returns>
    IUser? GetUser(string loginName);

    /// <summary>
    /// Einen Benutzer anmelden
    /// </summary>
    /// <param name="userName">Anmeldename des Benutzers</param>
    /// <param name="userPassword">Kennwort des Benutzers</param>
    /// <param name="message">Variable, die den Text mit Informationen zur Anmeldung aufnimmt (Fehlermeldungen u.ä.)</param>
    /// <returns>true wenn erfolgreich. wenn false enthält message Informationen zum Grund</returns>
    bool Login(string userName, string userPassword, ref string message);


    /// <summary>
    /// Einen Benutzer anmelden
    /// </summary>
    /// <param name="message">Variable, die den Text mit Informationen zur Anmeldung aufnimmt (Fehlermeldungen u.ä.)</param>
    /// <returns>true wenn erfolgreich. wenn false enthält message Informationen zum Grund</returns>
    bool Login(ref string message);

    /// <summary>
    /// Eine Verbindung zur Datenbank aufbauen
    /// </summary>
    /// <returns></returns>
    IConnection GetConnection();

}