using DevExpress.XtraTab;

/// <summary>
/// Presenter, der beliebige anderen Presenter in Tabs kombiniert (Charts etc.)
/// </summary>
[DesignerCategory("Code")]
public class AFPresenterTabs : AFUserControl
{
    private readonly AFNavTabControlDynamic tabs = null!;

    /// <summary>
    /// Zugriff auf das TabControl...
    /// </summary>
    public AFNavTabControlDynamic TabControl => tabs;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFPresenterTabs()
    {
        if (UI.DesignMode) return;

        tabs = new() { Dock = DockStyle.Fill };
        tabs.PaintStyleName = "AFFlat";
        tabs.CustomPaintBackground = true;
        tabs.BackgroundAppearance = new()
        {
            AutoColors = true,
            CornerRadius = 5
        };

        Controls.Add(tabs);
        tabs.BringToFront();
    }

    /// <summary>
    /// Fügt eine statische Seite hinzu
    /// </summary>
    /// <param name="caption">Überschrift</param>
    /// <param name="ctrl">Control, das im tab angezeigt wird</param>
    /// <returns>hinzugefügte Seite</returns>
    public XtraTabPage AddTabPage(string caption, Control ctrl)
    {
        var page = tabs.TabPages.Add();
        page.Text = caption;
        page.Padding(8);
        ctrl.Dock = DockStyle.Fill;
        page.Controls.Add(ctrl);

        return page;
    }

    /// <summary>
    /// Fügt eine dynamische Seite hinzu
    /// </summary>
    /// <param name="caption">Überschrift</param>
    /// <param name="type">Typ des Controls, das im Tab angezeigt wird</param>
    /// <returns>hinzugefügte Seite</returns>
    public XtraTabPage AddTabPage(string caption, Type type)
    {
        var page = tabs.TabPages.Add();
        page.Text = caption;
        page.Padding(8);
        tabs.RegisterContent(page, type, true);

        return page;
    }
}



