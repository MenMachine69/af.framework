using DevExpress.LookAndFeel;

namespace AF.WINFORMS.DX;

/// <summary>
/// UI Style for various elements
/// </summary>
[SupportedOSPlatform("windows")]
public class UIStyle
{
    /// <summary>
    /// default style
    /// </summary>
    public const string STANDARD = "Standard";
    /// <summary>
    /// small text style
    /// </summary>
    public const string SMALL = "Small";
    /// <summary>
    /// default style
    /// </summary>
    public const string LINK = "Link";
    /// <summary>
    /// gray text style
    /// </summary>
    public const string GRAY = "Gray";
    /// <summary>
    /// gray text style
    /// </summary>
    public const string GRAYSMALL = "GraySmall";
    /// <summary>
    /// bold text style
    /// </summary>
    public const string BOLD = "Bold";
    /// <summary>
    /// bold text style
    /// </summary>
    public const string BOLDLINK = "BoldLink";
    /// <summary>
    /// caption style
    /// </summary>
    public const string CAPTION = "Caption";
    /// <summary>
    /// caption style small
    /// </summary>
    public const string CAPTIONSMALL = "CaptionSmall";
    /// <summary>
    /// caption H1 style
    /// </summary>
    public const string CAPTIONH1 = "CaptionH1";
    /// <summary>
    /// caption H2 style
    /// </summary>
    public const string CAPTIONH2 = "CaptionH2";
    /// <summary>
    /// caption H3 style
    /// </summary>
    public const string CAPTIONH3 = "CaptionH3";
    /// <summary>
    /// Invertierte Überschrift
    /// </summary>
    public const string CAPTIONINVERTED = "CaptionInverted";

    private Font? font;

    /// <summary>
    /// Cosntructor
    /// </summary>
    public UIStyle()
    {
        UserLookAndFeel.Default.StyleChanged += styleChanged;
    }

    private void styleChanged(object? sender, EventArgs e)
    {
        if (IgnoreStyleChanges) return;

        (BackgroundColor, ForeColor) = (ForeColor, BackgroundColor);
    }

    /// <summary>
    /// Ignore style changes - do NOT swap colors...
    ///
    /// If true the application itself should be modify this style if needed.
    /// </summary>
    public bool IgnoreStyleChanges { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="fontFace">font face/name</param>
    /// <param name="fontSize">font size</param>
    /// <param name="bold">bold style</param>
    /// <param name="italic">italic style</param>
    /// <param name="underline">underline</param>
    /// <param name="backgroundColor">background color</param>
    /// <param name="foreColor">fore/text color</param>
    public UIStyle(string fontFace, float fontSize, bool bold, bool italic, bool underline, Color backgroundColor, Color foreColor) : this()
    {
        FontFace = fontFace;
        FontSize = fontSize;
        Bold = bold;
        Italic = italic;
        Underline = underline;
        BackgroundColor = backgroundColor;
        ForeColor = foreColor;
        
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="basestyle">create new based on this</param>
    /// <param name="fontFace">font face/name</param>
    /// <param name="fontSize">font size</param>
    /// <param name="bold">bold style</param>
    /// <param name="italic">italic style</param>
    /// <param name="underline">underline</param>
    /// <param name="backgroundColor">background color</param>
    /// <param name="foreColor">fore/text color</param>
    /// <param name="ignoreStyleChanges">Ignore StyleChanged event (switching skins)</param>
    public UIStyle(UIStyle basestyle, string? fontFace = null, float? fontSize = null, bool? bold = null, bool? italic = null, bool? underline = null, Color? backgroundColor = null, Color? foreColor = null, bool? ignoreStyleChanges = null) : this()
    {
        FontFace = fontFace ?? basestyle.FontFace;
        FontSize = fontSize ?? basestyle.FontSize;
        Bold = bold ?? basestyle.Bold;
        Italic = italic ?? basestyle.Italic;
        Underline = underline ?? basestyle.Underline;
        BackgroundColor = backgroundColor ?? basestyle.BackgroundColor;
        ForeColor = foreColor ?? basestyle.ForeColor;
        IgnoreStyleChanges = ignoreStyleChanges ?? basestyle.IgnoreStyleChanges;

        UseBackgroundColor = backgroundColor != null || basestyle.UseBackgroundColor;
    }

    /// <summary>
    /// FontFace/Name to use
    /// </summary>
    [DefaultValue("Segoe UI")]
    public string FontFace { get; init; } = @"Segoe UI";

    /// <summary>
    /// FontSize
    /// </summary>
    [DefaultValue(11.0f)]
    public float FontSize { get; init; } = 11.0f;

    /// <summary>
    /// Background color (will atomatic swapped if style changed)
    /// </summary>
    public Color BackgroundColor { get; set; } = UI.TranslateSystemToSkinColor(SystemColors.Window);

    /// <summary>
    /// Fore/Text color (will atomatic swapped if style changed)
    /// </summary>
    public Color ForeColor { get; set; } = UI.TranslateSystemToSkinColor(SystemColors.WindowText);

    /// <summary>
    /// Use bold style text
    /// </summary>
    public bool Bold { get; init; }

    /// <summary>
    /// use underline text
    /// </summary>
    public bool Underline { get; init; }

    /// <summary>
    /// use italic text
    /// </summary>
    public bool Italic { get; init; }

    /// <summary>
    /// access the font based on the parameters
    /// </summary>
    public Font Font => font ?? createFont();

    /// <summary>
    /// Hintergrundfarbe benutzen
    /// </summary>
    public bool UseBackgroundColor { get; set; } = false;

    private Font createFont()
    {
        if (font != null) return font;

        FontStyle style = FontStyle.Regular;
        if (Bold) style |= FontStyle.Bold;
        if (Italic) style |= FontStyle.Italic;
        if (Underline) style |= FontStyle.Underline;

        font = new Font(new FontFamily(FontFace), FontSize, style);

        return font;
    }
}

/// <summary>
/// Standard-Template für eine Grid-Ansicht
/// </summary>
public class UIHtmlTemplate
{
    /// <summary>
    /// HTML-Code des Templates
    /// </summary>
    public string HtmlTemplate { get; set; } = "";

    /// <summary>
    /// CSS-Code des Templates
    /// </summary>
    public string CssTemplate { get; set; } = "";
}