using DevExpress.XtraBars;
using DevExpress.XtraEditors.Repository;

namespace AF.WINFORMS.DX;

/// <summary>
/// Anzeige eines Dashboards
/// </summary>
[DesignerCategory("Code")]
public class AFDashboardViewer : AFUserControl
{
    private readonly AFBarManager manager = null!;
    private readonly AFBarController barController = null!;
    private AFDashboard? dashboard;
    private RepositoryItemImageComboBox? cmbPages;
    private BarEditItem? selectPages;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFDashboardViewer(string caption, ObjectImages image, AFDashboardViewerOptions? options = null)
    {
        if (UI.DesignMode) return;

        bool hideHeader = options?.HideHeader ?? false;

        if (hideHeader) return;

        barController = new();
        //barController.AutoBackColorInBars = true;
        barController.PropertiesBar.AllowLinkLighting = false;

        manager = new();
        manager.Form = this;
        manager.Images = Glyphs.GetImages();
        manager.Controller = barController;
        manager.BeginInit();


        StandaloneBarDockControl bardock = new() { Dock = DockStyle.Top, AutoSize = true };
        var bar = manager.AddBar(bardock);
        if (!(options?.HideCaption ?? false))
        {
            var label = bar.AddLabel("lblCaption", caption, UI.GetObjectImage(image));
            label.ImageOptions.SvgImageSize = new(24, 24);
            label.ImageOptions.SvgImageColorizationMode = DevExpress.Utils.SvgImageColorizationMode.None;
            label.ImageToTextAlignment = BarItemImageToTextAlignment.BeforeText;

            var spacer = bar.AddLabel("spaceLeft", " ".PadLeft(20, ' '));
            spacer.Enabled = false;
            spacer.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        }
        

        if (!(options?.HidePages ?? false))
        {
            selectPages = bar.AddImageCombobox("cmbSelectPage", out var combo);
            combo.SmallImages = Glyphs.GetObjectImages();
            selectPages.AutoFillWidth = true;
            selectPages.ImageOptions.SvgImageColorizationMode = DevExpress.Utils.SvgImageColorizationMode.None;

            if (options?.HideCaption ?? false)
            {
                barController.PropertiesBar.BarItemVertIndent = 8;
                combo.Appearance.FontSizeDelta = 0;
                selectPages.ImageOptions.SvgImageSize = new(12, 12);
            }
            else
            { 
                combo.Appearance.FontSizeDelta = 2;
                combo.Appearance.Options.UseFont = true;
                selectPages.ImageOptions.SvgImageSize = new(24, 24);
            }


            cmbPages = combo;
        }

        if (!(options?.HideMenu ?? false))
        {
            var spacerRight = bar.AddLabel("spaceRight", " ".PadLeft(20, ' '));
            spacerRight.Enabled = false;
            spacerRight.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            var menu = bar.AddMenu("menDashboard", img: UI.GetImage(Symbol.MoreVertical));
            menu.Alignment = BarItemLinkAlignment.Right;
            menu.ImageOptions.SvgImageColorizationMode = DevExpress.Utils.SvgImageColorizationMode.Full;

            IMenuEntry[] entrys = [
                new AFMenuEntry("ALLES AKTUALISIEREN") {  },
                new AFMenuEntry("LAYOUT SPEICHERN") {  },
                new AFMenuEntry("LAYOUT ZURÜCKSETZEN") {  },
                new AFMenuEntry("SEITE ANPASSEN") {  }
                ];

            manager.BoundCommandsToMenu(menu, entrys, onClick, showimages: true, imgsize: 16);
        }

        Controls.Add(bardock);

        manager.EndInit();

    }

    private void onClick(object sender, ItemClickEventArgs e)
    {
        
    }

    /// <summary>
    /// Setup des Dashboards...
    /// </summary>
    /// <param name="model">Beschreibung des Dashboards</param>
    public void Setup(AFDashboardModel model)
    {
        if (dashboard == null)
        {
            dashboard = AFDashboard.Create(model, 0);
            dashboard.Dock = DockStyle.Fill;
            Controls.Add(dashboard);
            dashboard.BringToFront();
        }
        else
            dashboard.Setup(model, 0);

        if (cmbPages == null) return;

        List<ListItem> pages = [];

        foreach (var page in model.Pages)
            pages.Add(new() { Caption = page.Caption, ImageIndex = page.PageObjectImageIndex ?? (int)ObjectImages.piece, Value = page.PagePosition });

        cmbPages.Fill(pages);
        selectPages!.EditValue = pages[0].Value;
    }

}

/// <summary>
/// Optionen für die Anzeige
/// </summary>
public sealed class AFDashboardViewerOptions
{
    /// <summary>
    /// Kopf komplett ausblenden
    /// </summary>
    public bool HideHeader { get; set; } = false;

    /// <summary>
    /// Beschriftung ausblenden
    /// </summary>
    public bool HideCaption { get; set; } = false;

    /// <summary>
    /// Seitenauswahl ausblenden
    /// </summary>
    public bool HidePages { get; set; } = false;

    /// <summary>
    /// Menü ausblenden
    /// </summary>
    public bool HideMenu { get; set; } = false;

    /// <summary>
    /// Anpassen der Seiten erlauben
    /// </summary>
    public bool AllowCustomize { get; set; } = false;
}

