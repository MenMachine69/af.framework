using DevExpress.Utils;
using DevExpress.XtraBars.Navigation;

namespace AF.WINFORMS.DX;

/// <inheritdoc/>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Navigation")]
public class AFNavTabPane : TabPane
{
    readonly Dictionary<TabNavigationPage, Tuple<Type, bool>> _dynamicPages = new();

    /// <summary>
    /// Control handle created...
    ///
    /// Load first page.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (UI.DesignMode) return;   

        loadPage(SelectedPage);
    }


    /// <summary>
    /// Register a page as dynamic page.
    /// </summary>
    /// <param name="page">TabPage that should show the content if selected</param>
    /// <param name="contentControlType">type of the content conrtrol to show</param>
    /// <param name="clearPage">if true the content control will be removed if another page is selected</param>
    public void RegisterContent(TabNavigationPage page, Type contentControlType, bool clearPage)
    {
        if (Pages.Count == 1)
            SelectedPage = page;

        _dynamicPages.Add(page, new Tuple<Type, bool>(contentControlType, clearPage));
    }

    /// <summary>
    /// Event that raises if a TabPage is selected.
    ///
    /// Remove the content control from previoues page (if it is a dynamic page) and create the content control for the new page (if it is a dynamic page).
    /// </summary>
    /// <param name="oldPage">previous page</param>
    /// <param name="newPage">selected page</param>
    protected override void OnSelectedPageChanging(INavigationPageBase? oldPage, INavigationPageBase newPage)
    {
        base.OnSelectedPageChanging(oldPage, newPage);

        if (UI.DesignMode) return;   

        // ggf. alte Seite bereinigen
        if (oldPage != null && ((TabNavigationPage)oldPage).Controls.Count > 0)
        {
            TabNavigationPage page = (TabNavigationPage)oldPage;

            if (_dynamicPages.ContainsKey(page) && _dynamicPages[page].Item2) // prüfen ob die Seite dynamisch ist...
            {
                Control ctrl = page.Controls[0];
                
                DynamicPageRemoving?.Invoke(this, new AFTabPaneEventArgs { Page = page, Control = ctrl });

                page.Controls.Remove(ctrl);
                ctrl.Dispose();
            }
        }

        if (((TabNavigationPage)newPage).Controls.Count < 1)
        {
            // TAB ausgewählt, dass noch keine Controls enthält...
            loadPage((TabNavigationPage)newPage);
        }
    }

    /// <summary>
    /// load the content control for the page
    /// </summary>
    /// <param name="page"></param>
    private void loadPage(TabNavigationPage page)
    {
        if (_dynamicPages.TryGetValue(page, out var dynamicPage)) // prüfen ob die Seite dynamisch ist...
        {
            page.SuspendLayout();
            page.SuspendDrawing();

            Control ctrl = ControlsEx.CreateInstance(dynamicPage.Item1)!;

            ctrl.Dock = DockStyle.Fill;
            page.Controls.Add(ctrl);
            ctrl.Show();

            DynamicPageCreated?.Invoke(this, new AFTabPaneEventArgs { Page = page, Control = ctrl });

            page.ResumeDrawing();
            page.ResumeLayout(true);
        }
    }

    /// <summary>
    /// Handler für die DynamicPage-Events
    /// </summary>
    public delegate void DynamicPageEventHandler(object sender, AFTabPaneEventArgs args);

    /// <summary>
    /// Page-Control will be removed
    /// Use this event to save something from the Page-Control that will be removed after this event.
    /// </summary>
    public event DynamicPageEventHandler? DynamicPageRemoving;

    /// <summary>
    /// new Page-Control created
    /// Use this event to init something in the new created Page-Control before this Control will be displayed.
    /// </summary>
    public event DynamicPageEventHandler? DynamicPageCreated;
}

/// <summary>
/// Event arguments for the DynamicPage events
/// </summary>
public class AFTabPaneEventArgs : EventArgs
{
    /// <summary>
    /// Page
    /// </summary>
    public TabNavigationPage? Page { get; set; }

    /// <summary>
    /// Control
    /// </summary>
    public Control? Control { get; set; }
}
