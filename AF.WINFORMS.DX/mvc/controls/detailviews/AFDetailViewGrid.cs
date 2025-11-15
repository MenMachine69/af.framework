using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Tile;

namespace AF.MVC;

/// <summary>
/// DetailView, dass Daten in einem Grid anzeigen kann.
/// </summary>
[ToolboxItem(false)]
[SupportedOSPlatform("windows")]
[DesignerCategory("Code")]
public class AFDetailViewGrid : AFDetailView, IViewDetail
{
    private readonly AFModelGrid modelGrid = null!;
    private readonly PopupMenu? gridRowPopup;
    private bool popupEntriesLoaded;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFDetailViewGrid(IViewPage page) : base(page)
    {
        if (UI.DesignMode) return;

        if (page is AFPage crpage)
        {
            gridRowPopup = new() { Manager = crpage.BarManager };
            gridRowPopup.BeforePopup += (_, _) =>
            {
                if (popupEntriesLoaded) return;

                foreach (BarItemLink item in crpage.ContextMenuDetail.ItemLinks)
                    gridRowPopup.AddItem(item.Item);

                popupEntriesLoaded = true;
            };
        }

        modelGrid = new() { Dock = DockStyle.Fill };

        if (gridRowPopup != null)
            modelGrid.PopupMenuRows = gridRowPopup;

        modelGrid.RowSelected += (s, e) =>
        {
            PluginDetail?.Update(this);
        };

        Controls.Add(modelGrid);

        modelGrid.ProcessModelChange = processModelChange;
    }

    /// <summary>
    /// Änderungen eines Models verarbeiten, dessen Typ im Grid dargestellt wird. 
    /// </summary>
    /// <param name="grid">Grid (Absender)</param>
    /// <param name="modelIn">Model, dass das Ereignis auslöste</param>
    /// <param name="modelEvent">Ereignis</param>
    /// <returns>Model, dass im Grid weiterverarbeitet werden soll oder NULL</returns>
    private IModel? processModelChange(AFModelGrid grid, IModel modelIn, eHubEventType modelEvent)
    {
        if (Page.ModelID == null) return null;

        if (Page.CurrentDetailController == null) return null;

        if (Page.CurrentController == null) return null;

        if (Page.CurrentController.IsChildOf(modelIn, (Guid)Page.ModelID))
            return Page.CurrentDetailController.ReadDetailModel(Page.CurrentController.ControlledType, modelIn.PrimaryKey);

        grid.RemoveModel(modelIn);

        return null;
    }

    /// <summary>
    /// Setup des Grids 
    /// </summary>
    /// <param name="setup">Einstellungen für das Grid</param>
    public void Setup(AFGridSetup setup)
    {
        modelGrid.Setup(setup);
    }

    /// <inheritdoc />
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IBindingList? Models 
    { 
        get => modelGrid.Models;
        set => modelGrid.Models = value; 
    }

    /// <inheritdoc />
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] 
    public IEnumerable<IModel>? SelectedModels => modelGrid.SelectedModels;

    /// <inheritdoc />
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IModel? CurrentModel => modelGrid.CurrentModel;

    /// <summary>
    /// Zugriff auf das GridView
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public GridView? GridView => modelGrid.GridView;

    /// <summary>
    /// Zugriff auf das BandedGridView
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AdvBandedGridView? BandedGridView => modelGrid.BandedGridView;

    /// <summary>
    /// Zugriff auf das Grid
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFGridControl Grid => modelGrid.Grid;

    /// <summary>
    /// Zugriff auf das TileView (alternative Darstellung via HTML/CSS)
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TileView? TileView => modelGrid.TileView;
}