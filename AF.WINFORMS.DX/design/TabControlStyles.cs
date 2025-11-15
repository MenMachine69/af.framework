using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.XtraTab.Drawing;
using DevExpress.XtraTab.Registrator;
using DevExpress.XtraTab.ViewInfo;
using DevExpress.XtraTab;
using System.Drawing.Drawing2D;

namespace AF.WINFORMS.DX;

/// <summary>
/// Registriert den PaintStyleName 'AFFlat' als neuen Stil für TabControls und stellt den passenden Painter zur Verfügung...
/// </summary>
[SupportedOSPlatform("windows")]
public class AFFlatTabRegistrator : SkinViewInfoRegistrator
{

    /// <summary>
    /// Constructor
    /// </summary>
    public AFFlatTabRegistrator()
    {

    }

    /// <summary>
    /// Name des Stylse
    /// </summary>
    public override string ViewName => @"AFFlat"; 

    /// <summary>
    /// Stellt einen passenden Painter für das TabControl zur Verfügung...
    /// </summary>
    /// <param name="tabControl"></param>
    /// <returns></returns>
    public override BaseTabPainter CreatePainter(IXtraTab tabControl)
    {
        if (tabControl is XtraTabControl tabctrl)
        {
            tabctrl.AppearancePage.PageClient.Options.UseBackColor = true;

            if (tabctrl.BackColor.Equals(Color.Empty) == false)
                tabctrl.AppearancePage.PageClient.BackColor = tabctrl.BackColor;
            else
            {
                tabctrl.AppearancePage.PageClient.BackColor = CommonSkins.GetSkin(UserLookAndFeel.Default)
                    .TranslateColor(SystemColors.Control);
            }
        }

        return new AFFlatTabPainter(tabControl);
    }
}

/// <summary>
/// Painter für TabControls im Metro-Stil
/// </summary>
[SupportedOSPlatform("windows")]
public class AFFlatTabPainter : BaseTabPainter
{

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tabControl">TabControl, auf das der Painter angewendet werden soll</param>
    public AFFlatTabPainter(IXtraTab tabControl)
        : base(tabControl)
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    /// <param name="row"></param>
    /// <param name="pInfo"></param>
    protected override void DrawHeaderPage(TabDrawArgs e, BaseTabRowViewInfo row, BaseTabPageViewInfo pInfo)
    {
        if (pInfo.Page == TabControl.ViewInfo.SelectedTabPage)
        {
            pInfo.PaintAppearance.BackColor =
                CommonSkins.GetSkin(UserLookAndFeel.Default).TranslateColor(SystemColors.Highlight);
            pInfo.PaintAppearance.BorderColor =
                CommonSkins.GetSkin(UserLookAndFeel.Default).TranslateColor(SystemColors.Highlight);
        }
        else
        {
            if (pInfo.IsHotState)
            {
                pInfo.PaintAppearance.BackColor = Color.FromArgb(40,
                    CommonSkins.GetSkin(UserLookAndFeel.Default).TranslateColor(SystemColors.Highlight));
                pInfo.PaintAppearance.BorderColor = Color.FromArgb(40,
                    CommonSkins.GetSkin(UserLookAndFeel.Default).TranslateColor(SystemColors.Highlight));
            }
            else
            {
                pInfo.PaintAppearance.BackColor = ((Control)TabControl).BackColor;
                pInfo.PaintAppearance.BorderColor = ((Control)TabControl).BackColor;
            }
        }

        base.DrawHeaderPage(e, row, pInfo);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    /// <param name="pInfo"></param>
    /// <returns></returns>
    protected override Color CheckHeaderPageForeColor(TabDrawArgs e, BaseTabPageViewInfo pInfo)
    {
        if (pInfo.Page == TabControl.ViewInfo.SelectedTabPage)
            return CommonSkins.GetSkin(UserLookAndFeel.Default).TranslateColor(SystemColors.HighlightText);

        return base.CheckHeaderPageForeColor(e, pInfo);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    /// <param name="pInfo"></param>
    /// <param name="r"></param>
    protected override void DrawHeaderBorder(TabDrawArgs e, BaseTabPageViewInfo pInfo, ref Rectangle r)
    {
        // base.DrawHeaderBorder(e, pInfo, ref r);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected override void DrawTabPage(TabDrawArgs e)
    {
        using SolidBrush brush = new(((Control)TabControl).BackColor);
        e.Graphics.FillRectangle(brush, e.Bounds);
    }
}

/// <summary>
/// Registriert den PaintStyleName 'AFLine' als neuen Stil für TabControls und stellt den passenden Painter zur Verfügung...
/// </summary>
[SupportedOSPlatform("windows")]
public class AFLineTabRegistrator : SkinViewInfoRegistrator
{

    /// <summary>
    /// Constructor
    /// </summary>
    public AFLineTabRegistrator()
    {

    }

    /// <summary>
    /// Name des Stylse
    /// </summary>
    public override string ViewName => @"AFLine";

    /// <summary>
    /// Stellt einen passenden Painter für das TabControl zur Verfügung...
    /// </summary>
    /// <param name="tabControl"></param>
    /// <returns></returns>
    public override BaseTabPainter CreatePainter(IXtraTab tabControl)
    {
        if (tabControl is XtraTabControl tab)
        {
            tab.AppearancePage.PageClient.Options.UseBackColor = true;

            if (tab.BackColor.Equals(Color.Empty) == false)
                tab.AppearancePage.PageClient.BackColor = tab.BackColor;
            else
            {
                tab.AppearancePage.PageClient.BackColor = CommonSkins.GetSkin(UserLookAndFeel.Default)
                    .TranslateColor(SystemColors.Control);
            }
        }

        return new AFLineTabPainter(tabControl);
    }
}

/// <summary>
/// Painter für TabControls mit einer Linie statt der klassischen Darstellung
/// </summary>
[SupportedOSPlatform("windows")]
public class AFLineTabPainter : BaseTabPainter
{

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tabControl">TabControl, auf das der Painter angewendet werden soll</param>
    public AFLineTabPainter(IXtraTab tabControl)
        : base(tabControl)
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    /// <param name="row"></param>
    /// <param name="pInfo"></param>
    protected override void DrawHeaderPage(TabDrawArgs e, BaseTabRowViewInfo row, BaseTabPageViewInfo pInfo)
    {
        pInfo.PaintAppearance.BackColor = ((Control)TabControl).BackColor;
        pInfo.PaintAppearance.BorderColor = ((Control)TabControl).BackColor;

        base.DrawHeaderPage(e, row, pInfo);

        if (pInfo.Page == TabControl.ViewInfo.SelectedTabPage)
        {
            using SolidBrush brush =
                   new(CommonSkins.GetSkin(UserLookAndFeel.Default).TranslateColor(SystemColors.Highlight));
            e.Graphics.FillRectangle(brush,
                new Rectangle(pInfo.Bounds.X, pInfo.Bounds.Y + pInfo.Bounds.Height - 5, pInfo.Bounds.Width, 3));
        }
        else
        {
            if (pInfo.IsHotState)
            {
                using SolidBrush brush = new(Color.FromArgb(40,
                           CommonSkins.GetSkin(UserLookAndFeel.Default).TranslateColor(SystemColors.Highlight)));
                e.Graphics.FillRectangle(brush,
                    new Rectangle(pInfo.Bounds.X, pInfo.Bounds.Y + pInfo.Bounds.Height - 4, pInfo.Bounds.Width, 3));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    /// <param name="pInfo"></param>
    /// <returns></returns>
    protected override Color CheckHeaderPageForeColor(TabDrawArgs e, BaseTabPageViewInfo pInfo)
    {
        return CommonSkins.GetSkin(UserLookAndFeel.Default).TranslateColor(SystemColors.ControlText);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    /// <param name="pInfo"></param>
    /// <param name="r"></param>
    protected override void DrawHeaderBorder(TabDrawArgs e, BaseTabPageViewInfo pInfo, ref Rectangle r)
    {
        // base.DrawHeaderBorder(e, pInfo, ref r);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected override void DrawTabPage(TabDrawArgs e)
    {
        using SolidBrush brush = new(((Control)TabControl).BackColor); e.Graphics.FillRectangle(brush, e.Bounds);
    }
}

/// <summary>
/// Registriert den PaintStyleName 'AFRound' als neuen Stil für TabControls und stellt den passenden Painter zur Verfügung...
/// </summary>
[SupportedOSPlatform("windows")]
public class AFRoundedTabRegistrator : SkinViewInfoRegistrator
{

    /// <summary>
    /// Constructor
    /// </summary>
    public AFRoundedTabRegistrator()
    {

    }

    /// <summary>
    /// Name des Stylse
    /// </summary>
    public override string ViewName => @"AFRound";

    /// <summary>
    /// Stellt einen passenden Painter für das TabControl zur Verfügung...
    /// </summary>
    /// <param name="tabControl"></param>
    /// <returns></returns>
    public override BaseTabPainter CreatePainter(IXtraTab tabControl)
    {
        if (tabControl is XtraTabControl tab)
        {
            tab.AppearancePage.PageClient.Options.UseBackColor = true;

            if (tab.BackColor.Equals(Color.Empty) == false)
                tab.AppearancePage.PageClient.BackColor = tab.BackColor;
            else
            {
                tab.AppearancePage.PageClient.BackColor = CommonSkins.GetSkin(UserLookAndFeel.Default)
                    .TranslateColor(SystemColors.Control);
            }
        }

        return new AFRoundedTabPainter(tabControl);
    }
}

/// <summary>
/// Painter für TabControls mit einer Linie statt der klassischen Darstellung
/// </summary>
[SupportedOSPlatform("windows")]
public class AFRoundedTabPainter : BaseTabPainter
{

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tabControl">TabControl, auf das der Painter angewendet werden soll</param>
    public AFRoundedTabPainter(IXtraTab tabControl)
        : base(tabControl)
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    /// <param name="row"></param>
    /// <param name="pInfo"></param>
    protected override void DrawHeaderPage(TabDrawArgs e, BaseTabRowViewInfo row, BaseTabPageViewInfo pInfo)
    {
        pInfo.PaintAppearance.BackColor = ((Control)TabControl).BackColor;
        pInfo.PaintAppearance.BorderColor = ((Control)TabControl).BackColor;

        bool paint = false;
        Color color = Color.Empty;

        if (pInfo.Page == TabControl.ViewInfo.SelectedTabPage)
        {
            color = CommonSkins.GetSkin(UserLookAndFeel.Default).TranslateColor(SystemColors.Highlight);
            paint = true;
        }
        else
        {

            if (pInfo.IsHotState)
            {
                color = Color.FromArgb(40,
                    CommonSkins.GetSkin(UserLookAndFeel.Default).TranslateColor(SystemColors.Highlight));
                paint = true;
            }
        }

        if (paint)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            using SolidBrush brush = new(color);
            using GraphicsPath path = new(FillMode.Winding);
            
            path.AddRectangle(new Rectangle(pInfo.Bounds.X + 1 + ((pInfo.Bounds.Height - 2) / 2),
                pInfo.Bounds.Y + 1, pInfo.Bounds.Width - 4 - pInfo.Bounds.Height, pInfo.Bounds.Height - 1));
            path.AddEllipse(pInfo.Bounds.X + 1, pInfo.Bounds.Y + 1, pInfo.Bounds.Height - 2,
                pInfo.Bounds.Height - 2);
            path.AddEllipse(pInfo.Bounds.X + pInfo.Bounds.Width - pInfo.Bounds.Height - 3, pInfo.Bounds.Y + 1,
                pInfo.Bounds.Height - 2, pInfo.Bounds.Height - 2);
            path.CloseFigure();

            e.Graphics.FillPath(brush, path);

        }

        base.DrawHeaderPage(e, row, pInfo);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    /// <param name="pInfo"></param>
    /// <returns></returns>
    protected override Color CheckHeaderPageForeColor(TabDrawArgs e, BaseTabPageViewInfo pInfo)
    {
        return CommonSkins.GetSkin(UserLookAndFeel.Default).TranslateColor(pInfo.Page == TabControl.ViewInfo.SelectedTabPage 
            ? SystemColors.HighlightText 
            : SystemColors.ControlText);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    /// <param name="pInfo"></param>
    /// <param name="r"></param>
    protected override void DrawHeaderBorder(TabDrawArgs e, BaseTabPageViewInfo pInfo, ref Rectangle r)
    {
        // base.DrawHeaderBorder(e, pInfo, ref r);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected override void DrawTabPage(TabDrawArgs e)
    {
        using SolidBrush brush = new(((Control)TabControl).BackColor); 
        e.Graphics.FillRectangle(brush, e.Bounds);
    }
}