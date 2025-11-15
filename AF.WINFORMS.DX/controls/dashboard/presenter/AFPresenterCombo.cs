using DevExpress.XtraTab;

namespace AF.WINFORMS.DX;

/// <summary>
/// Presenter, der einen AFPresenterValues mit einem beliebigen anderen Presenter kombiniert (Charts etc.)
/// </summary>
[DesignerCategory("Code")]
public class AFPresenterCombo : AFPresenterBase
{
    private readonly AFPresenterValues valuePresenter = null!;

    /// <summary>
    /// Presenter der Values
    /// </summary>
    public AFPresenterValues ValuePresenter => valuePresenter;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="subPresenter"></param>
    public AFPresenterCombo(AFUserControl subPresenter) : base(false)
    {
        if (UI.DesignMode) return;

        valuePresenter = new() { Dock = DockStyle.Top, Size = new(ClientRectangle.Width, 50) };
        Controls.Add(valuePresenter);

        AFPanel panel = new() { Dock = DockStyle.Fill, Padding = new(8) };
        panel.CustomPaintBackground = true;
        panel.BackgroundAppearance = new()
        {
            AutoColors = true,
            CornerRadius = 5
        };
        Controls.Add(panel);
        panel.BringToFront();


        subPresenter.Dock = DockStyle.Fill;
        panel.Controls.Add(subPresenter);
    }
}

/// <summary>
/// Presenter, der einen AFPresenterValues mit einem beliebigen anderen Presenter kombiniert (Charts etc.)
/// </summary>
public class AFPresenterComboTabs : AFUserControl
{
    private readonly AFPresenterValues valuePresenter = null!;
    private readonly AFNavTabControlDynamic tabs = null!;

    /// <summary>
    /// Presenter der Values
    /// </summary>
    public AFPresenterValues ValuePresenter => valuePresenter;

    /// <summary>
    /// Zugriff auf das TabControl...
    /// </summary>
    public AFNavTabControlDynamic TabControl => tabs;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFPresenterComboTabs()
    {
        if (UI.DesignMode) return;

        valuePresenter = new() { Dock = DockStyle.Top, Size = new(ClientRectangle.Width, 50) };
        Controls.Add(valuePresenter);

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
        tabs.RegisterContent(page ,type, true);
        
        return page;
    }
}



