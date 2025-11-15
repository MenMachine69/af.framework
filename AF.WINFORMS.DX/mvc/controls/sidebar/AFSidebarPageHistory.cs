namespace AF.MVC;

/// <summary>
/// Sidebar to display a list of history entrys
/// </summary>
public class AFSidebarPageHistory : AFSidebarPageBookmarks
{
    /// <inheritdoc />
    public AFSidebarPageHistory()
    {
        HistoryMode = true;
    }
}
