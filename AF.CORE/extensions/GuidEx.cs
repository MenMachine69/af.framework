namespace AF.CORE;

/// <summary>
/// Erweiterungsmethoden für System.Guid
/// </summary>
public static class GuidEx
{
    /// <summary>
    /// Prüft, ob ein Guid leer ist (gleich Guid.Empty)
    /// </summary>
    /// <param name="guid">zu prüfende Guid</param>
    /// <returns>true wenn die Guid gleich Guid.Empty ist</returns>
    public static bool IsEmpty(this Guid guid) { return guid.Equals(Guid.Empty); }

    /// <summary>
    /// Prüft, ob ein Guid nicht leer ist (nicht gleich Guid.Empty)
    /// </summary>
    /// <param name="guid">zu prüfende Guid</param>
    /// <returns>true wenn die Guid nicht gleich Guid.Empty ist</returns>
    public static bool IsNotEmpty(this Guid guid) { return !(guid.Equals(Guid.Empty)); }
}
