namespace AF.BUSINESS;

/// <summary>
/// BusinessSecurityService für Anwendungen.
/// 
/// Der BusinessSecurityService stellt Funktionen zur Verwaltung von Benutzer, 
/// deren Berechtigungen und weiteren Business-relevanten Dingen zur Verfügung.
/// </summary>
public class BusinessSecurityService : ISecurityService
{
    private readonly IDatabase _database;
    private readonly Type _userObjectType;
    private readonly Type _roleObjectType;

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="database">Datenbank für den BusinessService</param>
    /// <param name="roleObjectType">Typ/Klasse der Rollen</param>
    /// <param name="userObjectType">Typ/Klasse der Benutzer</param>
    public BusinessSecurityService(IDatabase database, Type userObjectType, Type roleObjectType)
    {
        _database = database;
        _userObjectType = userObjectType;
        _roleObjectType = roleObjectType;
    }

    /// <summary>
    /// Liefert einen Benutzer anhand seines Anmelde-/Benutzernamens 
    /// oder NULL, wenn kein Benutzer mit dem angegebenen Namen existiert.
    /// </summary>
    /// <param name="loginName">Anmeldename des Benutzers</param>
    /// <returns>der Benutzer oder NULL</returns>
    public IUser? GetUser(string loginName)
    {
        return ((IUserController)_userObjectType.GetController()!).GetUser(loginName);
    }

    /// <summary>
    /// Liefert eine Rolle anhand des Namens.
    /// NULL, wenn keine Rolle mit dem angegebenen Namen existiert.
    /// </summary>
    /// <param name="roleName">Name der Rolle</param>
    /// <returns>die Rolle oder NULL</returns>
    public IRole? GetRole(string roleName)
    {
        return ((IRoleController)_roleObjectType.GetController()!).GetRole(roleName);
    }

    /// <summary>
    /// Legt eine Rolle an, die ALLE Berechtigungen hat.
    /// 
    /// Die Rolle wird nur angelegt, wenn noch keine Rolle 
    /// mit dem angegebenen Namen existiert.
    /// </summary>
    /// <returns>die neu angelegte oder bereits vorhandene Rolle</returns>
    public IRole CreateAllRightsRole()
    {
        return ((IRoleController)_roleObjectType.GetController()!).CreateAllRightsRole();
    }

    /// <summary>
    /// Legt eine Rolle an, die KEINE Berechtigungen hat.
    /// 
    /// Die Rolle wird nur angelegt, wenn noch keine Rolle 
    /// mit dem angegebenen Namen existiert.
    /// </summary>
    /// <returns>die neu angelegte oder bereits vorhandene Rolle</returns>
    public IRole CreateNoRightsRole()
    {
        return ((IRoleController)_roleObjectType.GetController()!).CreateNoRightsRole();
    }


    /// <summary>
    /// Eine Verbindung zur Datenbank aufbauen
    /// </summary>
    /// <returns></returns>
    public IConnection GetConnection() { return _database.GetConnection(); }

    /// <summary>
    /// Eine Rolle einem Benutzer zuweisen.
    /// 
    /// Jedem Benutzer können beliebig viele Rollen zugewiesen werden, 
    /// deren Berechtigungen kombiniert werden (UND Verknüpfung)
    /// </summary>
    /// <param name="user">Benutzer dem die Rolle zugewiesen wird</param>
    /// <param name="role">Zuzuweisende Rolle</param>
    public void AssignRoleToUser(IUser user, IRole role)
    {
        ((IUserController)_userObjectType.GetController()!).AssignRole(user, role);
    }

    /// <summary>
    /// Unattended Login (Anmeldung ohne Kennwort).
    /// 
    /// Es wird versucht einen Benutzer ohne Kennwort anzumelden.
    /// </summary>
    /// <param name="loginName">Name des Benutzers</param>
    /// <param name="feedback">Variable, die das Feedback für den Anmeldeversuch aufnimmt. 
    /// Das können z.B. Informationen sein, warum eine Anmeldung nicht möglich war.</param>
    /// <returns>true, wenn der Benutzer angemeldet wurde (CurrentUser = angemeldeter Benutzer) oder false, wenn das nicht möglich war</returns>
    public bool TryLoginUnattend(string loginName, out string feedback)
    {
        if (loginName.Equals(@"system", StringComparison.OrdinalIgnoreCase))
        {
            feedback = CoreStrings.ERR_USERSYSTEMNOTALLOWED;
            return false;
        }

        CurrentUser = ((IUserController)_userObjectType.GetController()!).TryLoginUnattend(loginName, out feedback);

        return CurrentUser != null;
    }

    /// <summary>
    /// Login mit Benutzername und Kennwort
    /// </summary>
    /// <param name="loginName">Name des Benutzers</param>
    /// <param name="password">Kennwort des Benutzers</param>
    /// <param name="feedback">Variable, die das Feedback für den Anmeldeversuch aufnimmt. 
    /// Das können z.B. Informationen sein, warum eine Anmeldung nicht möglich war.</param>
    /// <returns>true, wenn der Benutzer angemeldet wurde (CurrentUser = angemeldeter Benutzer) oder false, wenn das nicht möglich war</returns>
    public bool TryLogin(string loginName, string password, out string feedback)
    {
        if (loginName.Equals(@"system", StringComparison.OrdinalIgnoreCase))
        {
            feedback = CoreStrings.ERR_USERSYSTEMNOTALLOWED;
            return false;
        }

        if (password.IsEmpty())
        {
            feedback = CoreStrings.ERR_EMPTYPASSWORDNOTALLOWED;
            return false;
        }

        CurrentUser = ((IUserController)_userObjectType.GetController()!).TryLogin(loginName, password, out feedback);

        return CurrentUser != null;
    }

    /// <summary>
    /// Login als Benutzer SYSTEM (aus jobs etc.)
    /// </summary>
    /// <returns>true, wenn der Benutzer angemeldet wurde (CurrentUser = angemeldeter Benutzer) oder false, wenn das nicht möglich war</returns>
    public bool LoginAsSystem(out string feedback)
    {
        feedback = "";

        CurrentUser = ((IUserController)_userObjectType.GetController()!).GetUser(@"SYSTEM");

        if (CurrentUser == null)
            feedback = @"User SYSTEM not found.";

        return CurrentUser != null;
    }

    /// <summary>
    /// der aktuell angemeldete Benutzer oder NULL, 
    /// wenn kein Benutzer angemeldet ist.
    /// </summary>
    public IUser? CurrentUser { get; set; }

    /// <summary>
    /// Wenn true, kann der Administrator die Anwendung in vollem Umfang nutzen, 
    /// die Überprüfung der Berechtigungen liefert dann immer true.
    /// 
    /// Wenn false, kann der Administrator nur die Programmfunktionen nutzen, 
    /// die explizit für Administratoren zur Verfügung stehen oder die, 
    /// für die er eine Berechtigung besitzt.
    /// </summary>
    public bool AdminHasAllRights { get; set; } = true;

    /// <summary>
    /// Prüfen, ob der Benutzer die angegebene Berechtigung besitzt (Bit ist gesetzt)
    /// </summary>
    /// <param name="right">ID der Berechtigung (Index im BitArray)</param>
    /// <returns>true, wenn der Benutzer berechtigt ist, sonst false</returns>
    public bool CheckUserRight(int right)
    {
        return CurrentUser != null && CurrentUser.UserRights.GetBit(right);
    }

    /// <summary>
    /// Prüfen, ob der Benutzer die angegebene Berechtigung besitzt (Bit ist gesetzt)
    /// </summary>
    /// <param name="user">Benutzer dessen Berechtigung geprüft wird</param>
    /// <param name="right">ID der Berechtigung (Index im BitArray)</param>
    /// <returns>true, wenn der Benutzer berechtigt ist, sonst false</returns>
    public bool CheckUserRight(IUser user, int right)
    {
        return user.UserRights.GetBit(right);
    }

    /// <summary>
    /// Prüft ob der Benutzer eine bestimmte Berechtigung hat.
    /// </summary>
    /// <param name="user">Benutzer dessen Berechtigung geprüft wird</param>
    /// <param name="right">ID der zu prüfenden Berechtigung</param>
    /// <returns>true, wenn der Benutzer berechtigt ist, sonst false</returns>
    public bool HasRight(IUser user, int right)
    {
        if (user.IsAdmin && AdminHasAllRights)
            return true;

        return CheckUserRight(user, right);
    }

    /// <summary>
    /// Prüft ob der Benutzer eine bestimmte Berechtigung hat.
    /// </summary>
    /// <param name="right">ID der zu prüfenden Berechtigung</param>
    /// <returns>true, wenn der Benutzer berechtigt ist, sonst false</returns>
    public bool HasRight(int right)
    {
        if (CurrentUser == null)
            throw new NullReferenceException(CoreStrings.ERR_NOUSERLOGGEDIN);

        if (CurrentUser?.IsAdmin == true && AdminHasAllRights)
            return true;

        return CheckUserRight(right);
    }

    /// <summary>
    /// Benutzer anmelden
    /// </summary>
    /// <param name="userName">Anmeldename des Benutzers</param>
    /// <param name="userPassword">Kennwort des Benutzers</param>
    /// <param name="feedback">Variable, die das Feedback für den Anmeldeversuch aufnimmt. 
    /// Das können z.B. Informationen sein, warum eine Anmeldung nicht möglich war.</param>
    /// <returns>true, wenn der Benutzer angemeldet wurde (CurrentUser = angemeldeter Benutzer) oder false, wenn das nicht möglich war</returns>
    public bool Login(string userName, string userPassword, ref string feedback)
    {
        return TryLogin(userName, userPassword, out feedback);
    }

    /// <summary>
    /// Benutzer anmelden
    /// </summary>
    /// <param name="feedback">Variable, die das Feedback für den Anmeldeversuch aufnimmt. 
    /// Das können z.B. Informationen sein, warum eine Anmeldung nicht möglich war.</param>
    /// <returns>true, wenn der Benutzer angemeldet wurde (CurrentUser = angemeldeter Benutzer) oder false, wenn das nicht möglich war</returns>
    public bool Login(ref string feedback)
    {
        return TryLoginUnattend(Environment.UserName, out feedback);
    }
}

