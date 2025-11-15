namespace AF.WINFORMS.DX;

/// <summary>
/// Dashboard
/// </summary>
[DesignerCategory("Code")]
public class AFDashboard : AFUserControl
{
    private AFDashboardModel? currentModel;
    private AFDashboardPageModel? currentPage;
    private readonly AFWidgetBoard board;

    /// <summary>
    /// Neues Dashboard für das übergebene Model erzeugen.
    /// </summary>
    /// <param name="model">Beschreibung des Dashboards</param>
    /// <param name="pageindex">anzuzeigende Seite (default = 0, erste Seite)</param>
    /// <returns>das neu erzeugte Dashboard</returns>
    public static AFDashboard Create(AFDashboardModel model, int pageindex = 0)
    {
        AFDashboard dashboard = new(model, pageindex);

        return dashboard;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="model">Beschreibung des Dashboards</param>
    /// <param name="pageindex">darzustellende Seite</param>
    public AFDashboard(AFDashboardModel model, int pageindex = 0) 
    {
        currentModel = model;

        currentPage = model.Pages[pageindex]!;

        board = new(currentPage.Rows, currentPage.Columns, currentPage.SpaceBetween);
        board.Dock = DockStyle.Fill;
        Controls.Add(board);

    }

    /// <summary>
    /// Setup des Dashboards (Layout)
    /// </summary>
    /// <param name="model">Model</param>
    /// <param name="pageindex">aktive Seite</param>
    public void Setup(AFDashboardModel model, int pageindex = 0)
    {
        currentModel = model;

        currentPage = model.Pages[pageindex]!;

        board.Setup(currentPage.Rows, currentPage.Columns, currentPage.SpaceBetween);
    }

    /// <summary>
    /// Aktuell geladenes Model
    /// </summary>
    public AFDashboardModel? CurrentDashboard => currentModel;


    /// <summary>
    /// Aktuelle Seite
    /// </summary>
    public AFDashboardPageModel? CurrentPage => currentPage;

    /// <summary>
    /// Zugriff auf das WidgetBoard
    /// </summary>
    public AFWidgetBoard WidgetBoard => board;
}

