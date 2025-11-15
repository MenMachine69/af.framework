using System.Management;

namespace AF.CORE;

/// <summary>
/// Benutzer in der Domäne
/// </summary>
/// <param name="LoginName">Nachname</param>
/// <param name="Nachname">Nachname</param>
/// <param name="Vorname">Vorname</param>
/// <param name="Domain">Domäne</param>
public record DomainUser(string LoginName, string Nachname, string Vorname, string Domain)
{
    /// <inheritdoc />
    public override string ToString() => $"{Nachname}, {Vorname} ({LoginName})";
}

/// <summary>
/// Hilfsfunktionen für Netzwerkoperationen
/// </summary>
public static class NetworkTools
{
    /// <summary>
    /// Liste der Benutzer in der Domäne ermitteln
    /// </summary>
    /// <param name="domain">Domäne</param>
    /// <param name="addlocaluser">Lokalen Benutzer des Computers hinzufügen wenn nicht enthalten</param>
    /// <returns>Liste der Benutzernamen</returns>
    public static List<DomainUser> GetAllUsers(string domain, bool addlocaluser = false)
    {
        List<DomainUser> users = [];
        SelectQuery query = new SelectQuery("Win32_UserAccount", $"Domain='{domain}'");

        using ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

        foreach (var user in searcher.Get())
        {
            string? fullName = user["FullName"]?.ToString();
            string? loginName = user["Name"]?.ToString();
            string? nachname = fullName?.Contains(',') ?? false ? fullName?.Split(',')[0] : fullName;
            string? vorname = fullName?.Contains(',') ?? false ? fullName?.Split(',')[1] : "";

            if (loginName != null)
                users.Add(new DomainUser(loginName ?? "", nachname ?? "", vorname ?? "", domain));
        }

        if (users.Any(u => u.LoginName == Environment.UserName) == false)
            users.Add(new DomainUser(Environment.UserName, Environment.UserName, Environment.UserName, ""));

        return users;
    }
}
