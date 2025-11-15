namespace AF.MVC;

/// <summary>
/// Beschreibung der Klasse, für deren Verwendung bestimmte Rechte erforderlich sind (z. B. UI-Elemente)
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AFPermission : Attribute
{
    /// <summary>
    /// benötigt administrative Rechte
    /// </summary>
    public bool NeedAdminRights { get; set; }

    /// <summary>
    /// Erfordert eine bestimmte Berechtigung
    ///
    /// Beachten Sie, dass Werte kleiner oder gleich 100 für das System reserviert sind!
    /// Verwenden Sie sie nicht in Ihren eigenen Controllern!
    /// </summary>
    public int NeededRight { get; set; } = -1;
}