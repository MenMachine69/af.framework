namespace AF.CORE;

/// <summary>
/// interface for pages that can be displayed in the sidebar
/// </summary>
public interface ISidebarPage
{
    /// <summary>
    /// called after the page is shown (load data etc.
    /// </summary>
    void AfterShow();
}

