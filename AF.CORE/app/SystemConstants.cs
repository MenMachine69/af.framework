namespace AF.CORE;

/// <summary>
/// System-Konstanten für AF3-Anwendungen
/// </summary>
public static class SystemConstants
{
    private static Guid _idBookmarks = new Guid("{5032E369-BDC5-4CF6-ADFF-F122000AC4B7}");
    private static Guid _idHistory = new Guid("{8F85D13D-D13F-4582-8B77-3C55F461D56C}");

    /// <summary>
    /// GUID der Bookmarks (für Persistierung)
    /// </summary>
    public static Guid IDBookmarks => _idBookmarks;

    /// <summary>
    /// GUID der History (für Persistierung)
    /// </summary>
    public static Guid IDHistory => _idHistory;

}

