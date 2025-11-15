using DevExpress.Utils.Extensions;
using DevExpress.Utils.Drawing;

namespace AF.WINFORMS.DX;

/// <summary>
/// Appearance für den Hintergrund
/// </summary>
[SupportedOSPlatform("windows")]
[TypeConverter(typeof(ExpandableObjectConverter))]
public class AFBackgroundAppearance
{
    /// <summary>
    /// Which corners should be rounded
    /// </summary>
    public GraphicsEx.eCornerMode Corners { get; set; } = GraphicsEx.eCornerMode.All;


    /// <summary>
    /// Farbe des Panels
    /// </summary>
    [Category("Panel")]
    public Color PanelColor { get; set; } = Color.White;

    /// <summary>
    /// Farbe des Rahmens
    /// </summary>
    [Category("Panel")]
    public Color BorderColor { get; set; } = Color.Black;

    /// <summary>
    /// Breite des Rahmens (0=kein Rahmen (Standard))
    /// </summary>
    [Category("Panel")]
    public int BorderWidth { get; set; }

    /// <summary>
    /// Radius der Ecken
    /// </summary>
    [Category("Panel")]
    public int CornerRadius { get; set; } = 8;

    /// <summary>
    /// automatische Farben (vom Skin abhängig) verwenden.
    /// Wenn true, wird Color und BorderColor ignoriert.
    /// </summary>
    [Category("Panel")]
    public bool AutoColors { get; set; } = true;

    /// <summary>
    /// Schatten zeichnen
    /// </summary>
    [Category("Panel")]
    public bool Shadow { get; set; }

    /// <summary>
    /// einen farbigen Rand an der linken Seite zeichnen 
    /// </summary>
    [Category("Bar")]
    [DefaultValue(false)]
    public bool LeftBar { get; set; }

    /// <summary>
    /// Farbe des linken Randes
    /// </summary>
    [Category("Bar")]
    [DefaultValue(null)]
    public Color? LeftBarColor { get; set; }

    /// <summary>
    /// Breite des Randes in Pixel (wird automatisch skaliert!)
    /// </summary>
    [Category("Bar")]
    [DefaultValue(0)]
    public int LeftBarWidth { get; set; }

    /// <summary>
    /// Verwende invertierte Farben bei AutoColors
    /// </summary>
    [Category("Panel")]
    public bool InvertAutoColors { get; set; }

    /// <summary>
    /// Verwende Highlight Farben bei AutoColors
    /// </summary>
    [Category("Panel")]
    public bool HighlightAutoColors { get; set; }


    /// <summary>
    /// Abgedimmte Darstellung verwenden
    /// </summary>
    [Category("Panel")]
    public bool Dimmed { get; set; }


    /// <inheritdoc />
    public override string ToString()
    {
        return @"PanelAppearance";
    }

    /// <summary>
    /// Draw panel (DirectX)
    /// </summary>
    /// <param name="g"></param>
    /// <param name="rect"></param>
    /// <param name="margin"></param>
    /// <param name="padding"></param>
    public void DrawDX(GraphicsCache g, Rectangle rect, Padding margin, Padding padding)
    {
        Color background = AutoColors 
            ? InvertAutoColors 
                ? UI.TranslateSystemToSkinColor(SystemColors.Control) 
                : UI.TranslateSystemToSkinColor(SystemColors.Window) 
            : PanelColor;

        Color border = AutoColors 
            ? InvertAutoColors
                ? Color.FromArgb(50, UI.TranslateSystemToSkinColor(SystemColors.ControlText))
                : Color.FromArgb(50, UI.TranslateSystemToSkinColor(SystemColors.WindowText))
            : BorderColor;
        
        if (AutoColors && HighlightAutoColors)
        {
            border = UI.TranslateSystemToSkinColor(SystemColors.Highlight);
            background= UI.TranslateSystemToSkinColor(SystemColors.Highlight);

            if (Dimmed)
            {
                border = Color.FromArgb(50, border);
                background = Color.FromArgb(50, border);
            }
        }

        if (AutoColors && Dimmed && !HighlightAutoColors)
        {
            background = UI.IsDarkSkin 
                ? background 
                : Color.FromArgb(10, Color.Black);
        }

        if (BorderWidth > 1)
            rect = rect.WithDeflate(new Padding(BorderWidth - 1));

        if (margin.Vertical > 0 || margin.Horizontal > 0)
            rect = rect.WithDeflate(margin);

        Rectangle shadowrect = rect with { X = rect.X+1, Y = rect.Y + 1 };
        Color shadowcolor = Color.FromArgb(30, UI.TranslateSystemToSkinColor(SystemColors.ControlText));


        switch (Corners)
        {
            case GraphicsEx.eCornerMode.All:
                if (Shadow) g.DrawRoundedRect(shadowrect, shadowcolor, border, 0, CornerRadius);
                g.DrawRoundedRect(rect, background, border, BorderWidth, CornerRadius);
                break;
            case GraphicsEx.eCornerMode.Left:
                if (Shadow) g.DrawLeftRoundedRect(shadowrect, shadowcolor, border, 0, CornerRadius);
                g.DrawLeftRoundedRect(rect, background, border, BorderWidth, CornerRadius);
                break;
            case GraphicsEx.eCornerMode.Right:
                if (Shadow) g.DrawRightRoundedRect(shadowrect, shadowcolor, border, 0, CornerRadius);
                g.DrawRightRoundedRect(rect, background, border, BorderWidth, CornerRadius);
                break;
            case GraphicsEx.eCornerMode.Top:
                if (Shadow) g.DrawTopRoundedRect(shadowrect, shadowcolor, border, 0, CornerRadius);
                g.DrawTopRoundedRect(rect, background, border, BorderWidth, CornerRadius);
                break;
            case GraphicsEx.eCornerMode.Bottom:
                if (Shadow) g.DrawBottomRoundedRect(shadowrect, shadowcolor, border, 0, CornerRadius);
                g.DrawBottomRoundedRect(rect, background, border, BorderWidth, CornerRadius);
                break;
            case GraphicsEx.eCornerMode.None:
                if (Shadow) g.DrawRect(shadowrect, shadowcolor, border);
                g.DrawRect(rect, background, border, BorderWidth);
                break;
            default:
                if (Shadow) g.DrawRoundedRect(shadowrect, shadowcolor, border, 0, CornerRadius);
                g.DrawRoundedRect(rect, background, border, BorderWidth, CornerRadius);
                break;
        }

        if (!LeftBar || LeftBarColor == null || LeftBarWidth <= 0) return;

        rect = new(rect.Left, rect.Top, UI.GetScaled(LeftBarWidth), rect.Height);
        
        if (Corners == GraphicsEx.eCornerMode.Left || Corners == GraphicsEx.eCornerMode.All)
            g.DrawLeftRoundedRect(rect, LeftBarColor ?? Color.Empty, border, 0, CornerRadius);
        else if (Corners == GraphicsEx.eCornerMode.None || Corners == GraphicsEx.eCornerMode.Right)
            g.DrawRect(rect, background, border, BorderWidth);
    }

    /// <summary>
    /// Draw panel (GDI)
    /// </summary>
    /// <param name="g"></param>
    /// <param name="rect"></param>
    /// <param name="margin"></param>
    /// <param name="padding"></param>
    public void Draw(GraphicsCache g, Rectangle rect, Padding margin, Padding padding)
    {
        Color background = AutoColors
            ? InvertAutoColors
                ? UI.TranslateSystemToSkinColor(SystemColors.Control)
                : UI.TranslateSystemToSkinColor(SystemColors.Window)
            : PanelColor;

        Color border = AutoColors
            ? InvertAutoColors
                ? Color.FromArgb(50, UI.TranslateSystemToSkinColor(SystemColors.ControlText))
                : Color.FromArgb(50, UI.TranslateSystemToSkinColor(SystemColors.WindowText))
            : BorderColor;

        if (AutoColors && HighlightAutoColors)
        {
            border = UI.TranslateSystemToSkinColor(SystemColors.Highlight);
            background = UI.TranslateSystemToSkinColor(SystemColors.Highlight);

            if (Dimmed)
            {
                border = Color.FromArgb(50, border);
                background = Color.FromArgb(50, border);
            }
        }


        if (AutoColors && Dimmed && !HighlightAutoColors)
        {
            background = UI.IsDarkSkin
                ? background
                : Color.FromArgb(10, Color.Black);
        }

        if (BorderWidth > 1)
            rect = rect.WithDeflate(new Padding(BorderWidth - 1));

        if (margin.Vertical > 0 || margin.Horizontal > 0)
            rect = rect.WithDeflate(margin);

        Rectangle shadowrect = rect with { X = rect.X + 1, Y = rect.Y + 1 };
        Color shadowcolor = Color.FromArgb(30, UI.TranslateSystemToSkinColor(SystemColors.ControlText));


        switch (Corners)
        {
            case GraphicsEx.eCornerMode.All:
                if (Shadow) g.DrawRoundedRect(shadowrect, shadowcolor, border, 0, CornerRadius);
                g.DrawRoundedRect(rect, background, border, BorderWidth, CornerRadius);
                break;
            case GraphicsEx.eCornerMode.Left:
                if (Shadow) g.DrawLeftRoundedRect(shadowrect, shadowcolor, border, 0, CornerRadius);
                g.DrawLeftRoundedRect(rect, background, border, BorderWidth, CornerRadius);
                break;
            case GraphicsEx.eCornerMode.Right:
                if (Shadow) g.DrawRightRoundedRect(shadowrect, shadowcolor, border, 0, CornerRadius);
                g.DrawRightRoundedRect(rect, background, border, BorderWidth, CornerRadius);
                break;
            case GraphicsEx.eCornerMode.Top:
                if (Shadow) g.DrawTopRoundedRect(shadowrect, shadowcolor, border, 0, CornerRadius);
                g.DrawTopRoundedRect(rect, background, border, BorderWidth, CornerRadius);
                break;
            case GraphicsEx.eCornerMode.Bottom:
                if (Shadow) g.DrawBottomRoundedRect(shadowrect, shadowcolor, border, 0, CornerRadius);
                g.DrawBottomRoundedRect(rect, background, border, BorderWidth, CornerRadius);
                break;
            case GraphicsEx.eCornerMode.None:
                if (Shadow) g.DrawRect(shadowrect, shadowcolor, border);
                g.DrawRect(rect, background, border, BorderWidth);
                break;
            default:
                if (Shadow) g.DrawRoundedRect(shadowrect, shadowcolor, border, 0, CornerRadius);
                g.DrawRoundedRect(rect, background, border, BorderWidth, CornerRadius);
                break;
        }

        if (!LeftBar || LeftBarColor == null || LeftBarWidth <= 0) return;

        rect = new(rect.Left, rect.Top, UI.GetScaled(LeftBarWidth), rect.Height);

        if (Corners == GraphicsEx.eCornerMode.Left || Corners == GraphicsEx.eCornerMode.All)
            g.DrawLeftRoundedRect(rect, LeftBarColor ?? Color.Empty, border, 0, CornerRadius);
        else if (Corners == GraphicsEx.eCornerMode.None || Corners == GraphicsEx.eCornerMode.Right)
            g.DrawRect(rect, background, border, BorderWidth);
    }

    /// <summary>
    /// Draw panel (GDI)
    /// </summary>
    /// <param name="g"></param>
    /// <param name="rect"></param>
    /// <param name="margin"></param>
    /// <param name="padding"></param>
    public void Draw(Graphics g, Rectangle rect, Padding margin, Padding padding)
    {
        Color background = AutoColors 
            ? InvertAutoColors 
                ? UI.TranslateSystemToSkinColor(SystemColors.Control) 
                : UI.TranslateSystemToSkinColor(SystemColors.Window) 
            : PanelColor;

        Color border = AutoColors 
            ? InvertAutoColors
                ? Color.FromArgb(50, UI.TranslateSystemToSkinColor(SystemColors.ControlText))
                : Color.FromArgb(50, UI.TranslateSystemToSkinColor(SystemColors.WindowText))
            : BorderColor;
        
        if (AutoColors && HighlightAutoColors)
        {
            border = UI.TranslateSystemToSkinColor(SystemColors.Highlight);
            background= UI.TranslateSystemToSkinColor(SystemColors.Highlight);

            if (Dimmed)
            {
                border = Color.FromArgb(50, border);
                background = Color.FromArgb(50, border);
            }
        }


        if (AutoColors && Dimmed && !HighlightAutoColors)
        {
            background = UI.IsDarkSkin 
                ? background 
                : Color.FromArgb(10, Color.Black);
        }

        if (BorderWidth > 1)
            rect = rect.WithDeflate(new Padding(BorderWidth - 1));

        if (margin.Vertical > 0 || margin.Horizontal > 0)
            rect = rect.WithDeflate(margin);

        Rectangle shadowrect = rect with { X = rect.X+1, Y = rect.Y + 1 };
        Color shadowcolor = Color.FromArgb(30, UI.TranslateSystemToSkinColor(SystemColors.ControlText));


        switch (Corners)
        {
            case GraphicsEx.eCornerMode.All:
                if (Shadow) g.DrawRoundedRect(shadowrect, shadowcolor, border, 0, CornerRadius);
                g.DrawRoundedRect(rect, background, border, BorderWidth, CornerRadius);
                break;
            case GraphicsEx.eCornerMode.Left:
                if (Shadow) g.DrawLeftRoundedRect(shadowrect, shadowcolor, border, 0, CornerRadius);
                g.DrawLeftRoundedRect(rect, background, border, BorderWidth, CornerRadius);
                break;
            case GraphicsEx.eCornerMode.Right:
                if (Shadow) g.DrawRightRoundedRect(shadowrect, shadowcolor, border, 0, CornerRadius);
                g.DrawRightRoundedRect(rect, background, border, BorderWidth, CornerRadius);
                break;
            case GraphicsEx.eCornerMode.Top:
                if (Shadow) g.DrawTopRoundedRect(shadowrect, shadowcolor, border, 0, CornerRadius);
                g.DrawTopRoundedRect(rect, background, border, BorderWidth, CornerRadius);
                break;
            case GraphicsEx.eCornerMode.Bottom:
                if (Shadow) g.DrawBottomRoundedRect(shadowrect, shadowcolor, border, 0, CornerRadius);
                g.DrawBottomRoundedRect(rect, background, border, BorderWidth, CornerRadius);
                break;
            case GraphicsEx.eCornerMode.None:
                if (Shadow) g.DrawRect(shadowrect, shadowcolor, border);
                g.DrawRect(rect, background, border, BorderWidth);
                break;
            default:
                if (Shadow) g.DrawRoundedRect(shadowrect, shadowcolor, border, 0, CornerRadius);
                g.DrawRoundedRect(rect, background, border, BorderWidth, CornerRadius);
                break;
        }

        if (!LeftBar || LeftBarColor == null || LeftBarWidth <= 0) return;

        rect = new(rect.Left, rect.Top, UI.GetScaled(LeftBarWidth), rect.Height);
        
        if (Corners == GraphicsEx.eCornerMode.Left || Corners == GraphicsEx.eCornerMode.All)
            g.DrawLeftRoundedRect(rect, LeftBarColor ?? Color.Empty, border, 0, CornerRadius);
        else if (Corners == GraphicsEx.eCornerMode.None || Corners == GraphicsEx.eCornerMode.Right)
            g.DrawRect(rect, background, border, BorderWidth);
    }
}
