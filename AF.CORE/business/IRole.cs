namespace AF.BUSINESS;

/// <summary>
/// Definition einer Rolle, die benutzern zur Steuerung der 
/// Berchtigungen in einer Anwendung zugewiesen werden können.  
/// </summary>
public interface IRole
{
    /// <summary>
    /// Name zur Anzeige der Rolle in der UI
    /// </summary>
    string RoleName { get; set; }

    /// <summary>
    /// Eindeutige ID der Rolle zur Identifikation der Rolle (z.B. in Datenbanken)
    /// </summary>
    Guid RoleId { get; set; }

    /// <summary>
    /// Die Berechtigungen der Rolle.
    ///
    /// Gesetztes Bit = berechtigt, nicht gesetztes Bit = nicht berechtigt
    /// </summary>
    AFBitArray Rights { get; set; }
}


/// <summary>
/// Schnittstelle für ein RoleController-Objekt (Controller für Rollenobjekte)
/// </summary>
public interface IRoleController
{
    /// <summary>
    /// liefert eine Rolle (oder null, falls nicht vorhanden) anhand ihres Namens
    /// </summary>
    /// <param name="roleName">Name der Rolle</param>
    /// <returns>die Rolle oder null</returns>
    IRole? GetRole(string roleName);

    /// <summary>
    /// Erstellt eine Rolle mit allen Rechten, falls nicht vorhanden.
    /// </summary>
    /// <returns>das Rollenobjekt</returns>
    IRole CreateAllRightsRole();

    /// <summary>
    /// Erstellen einer Rolle ohne Rechte, falls nicht vorhanden
    /// </summary>
    /// <returns>das Rollenobjekt</returns>
    IRole CreateNoRightsRole();

    /// <summary>
    /// Weist einen Benutzer einer Rolle zu
    /// </summary>
    /// <param name="user">Benutzerobjekt</param>
    /// <param name="role">Rollenobjekt</param>
    void AssignUser(IRole role, IUser user);
}


/// <summary>
/// Zentrale Erweiterungsmethoden für IRole und IRoleController, 
/// die dann im konkreten Implementierungen immer automatisch zur verfügung stehen.
/// </summary>
public static class IRoleEx
{

}