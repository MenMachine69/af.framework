using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.Utils.Svg;
using DevExpress.XtraBars.Navigation;

namespace AF.MVC;

/// <summary>
/// sidebar control for pages like favorites, history, etc.
/// </summary>
[SupportedOSPlatform("windows")]
public partial class AFSidebarControl : XtraUserControl
{
    /// <summary>
    /// Constructor
    /// </summary>
    public AFSidebarControl()
    {
        InitializeComponent();

        if (UI.DesignMode) return;

        tabs.DynamicPageCreated += pageCreated;
        tabs.AllowTransitionAnimation = DefaultBoolean.False;
    }

    private void pageCreated(object sender, AFTabPaneEventArgs args)
    {
        if (args.Control != null && args.Control is ISidebarPage page)
            page.AfterShow();
    }

    /// <summary>
    /// show a control on top of the page selector
    /// </summary>    
    public void SetPlugin(Control ctrl)
    {
        panelPlugin.Size = new Size(panelPlugin.Width, ctrl.Height + panelPlugin.Padding.Vertical);
        ctrl.Dock = DockStyle.Fill;
        panelPlugin.Controls.Add(ctrl);
        panelPlugin.Visible = true;
    }

    /// <summary>
    /// create a page in the sidebar
    /// </summary>
    /// <param name="caption">caption, null if no text should be displayed</param>
    /// <param name="symbol">symbol, null  if no symbol should be displayed</param>
    /// <param name="allowColorSkin">allow coloring the symbol according to skin</param>
    /// <param name="neededRight">needed right to display the page, -1 = no rights required</param>
    public void RegisterPage<TPage>(string? caption, SvgImage? symbol, bool allowColorSkin, int neededRight = -1) where TPage : class, ISidebarPage, new()
    {
        if (neededRight > -1 && (AFCore.SecurityService?.HasRight(neededRight) ?? true)) return;

        TabNavigationPage page = new()
        {
            Caption = caption ?? ""
        };

        if (symbol != null)
        {
            page.ImageOptions.SvgImage = symbol;
            page.ImageOptions.SvgImageSize = new Size(30, 30);
            page.ImageOptions.SvgImageColorizationMode = allowColorSkin ? SvgImageColorizationMode.Full : SvgImageColorizationMode.None;
        }


        tabs.Controls.Add(page);
        tabs.Pages.Add(page);


        tabs.RegisterContent(page, typeof(TPage), true);

    }
}

