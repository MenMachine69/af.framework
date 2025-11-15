using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Docking2010.Views.Tabbed;

namespace AF.MVC;

/// <summary>
/// Container, der alle Seiten enthält.
///
/// Dieser Container enthält einen DocumentManager mit einer Registerkartenansicht zur Verwaltung aller geöffneten IViewPages.
/// Verwenden Sie dieses Steuerelement nicht direkt, sondern verwenden Sie stattdessen den ViewManager.
/// </summary>
[ToolboxItem(false)]
public class AFPageContainer : AFUserControl
{
    private readonly DocumentManager documentManager1 = null!;
    private readonly TabbedView tabbedView1 = null!;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFPageContainer()
    {
        if (UI.DesignMode) return;

        documentManager1 = new (Container);
        tabbedView1 = new (Container);
        ((ISupportInitialize)documentManager1).BeginInit();
        ((ISupportInitialize)tabbedView1).BeginInit();
        SuspendLayout();

        documentManager1.ContainerControl = this;
        documentManager1.View = tabbedView1;
        documentManager1.ViewCollection.Add(tabbedView1);

        ((ISupportInitialize)documentManager1).EndInit();
        ((ISupportInitialize)tabbedView1).EndInit();
        ResumeLayout(false);

        updateColors();

        UI.StyleChanged += (_, _) =>
        {
            updateColors();
        };
    }

    private void updateColors()
    {
        tabbedView1.AppearancePage.HeaderActive.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Highlight);
        tabbedView1.AppearancePage.HeaderActive.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.HighlightText);
        tabbedView1.AppearancePage.HeaderActive.Options.UseBackColor = true;
        tabbedView1.AppearancePage.HeaderActive.Options.UseForeColor = true;
    }

    /// <summary>
    /// Zugriff auf den Document-Manager
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DocumentManager DocumentManager => documentManager1;

    /// <summary>
    /// Zugriff auf den View der die Seiten anzeigt
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TabbedView PageView => tabbedView1;
}

