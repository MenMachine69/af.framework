using AF.MVC;
using DevExpress.Utils;

namespace AF.WINFORMS.DX;

/// <summary>
/// Eintrag in einem PopupMenu
/// </summary>
public class AFMenuEntry : IMenuEntry
{
    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFMenuEntry() { Caption = ""; }

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="caption">Caption des Menüs. Verwenden Sie \ um Untermenüs zu erzeugen.</param>
    public AFMenuEntry(string caption)
    {
        Caption = caption;
    }

    /// <summary>
    /// Name des Menüpunktes
    /// </summary>
    [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Category("UI")]
    public string Name { get; set; } = "";

    /// <summary>
    /// Trennlinie vor dem Menüeintrag darstellen
    /// </summary>
    [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Category("UI")]
    public bool BeginGroup { get; set; }

    /// <summary>
    /// Index des SVG Bildes in der ImageList der Komponente
    /// </summary>
    [Browsable(true), DefaultValue(-1), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
     Category("UI")]
    public int ImageIndex { get; set; } = -1;

    /// <summary>
    /// Symbol, dass vor dem Menüeintrag angezeigt werden soll
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public object? Image { get; set; }

    /// <summary>
    /// Symbol, dass vor dem Menüeintrag angezeigt werden soll, wenn es sich um ein SubMenu handelt.
    /// 
    /// Das Symbol muss nur dem ersten Menüeintrag des SubMenus zugewiesen werden (Position).
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public object? GroupImage { get; set; }

    /// <summary>
    /// Colorization mode für das Symbol (SVG-Colorization)
    /// </summary>
    public SvgImageColorizationMode SymbolColorizationMode { get; set; } = SvgImageColorizationMode.Default;

    /// <summary>
    /// Position im Menü (0 = ganz oben)
    /// </summary>
    [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Category("UI")]
    public int Position { get; set; }

    /// <summary>
    /// Caption des Menüs
    ///
    /// Verwenden Sie \ um Untermenüs zu erzeugen
    /// </summary>
    [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Category("UI")]
    public string Caption { get; }

    /// <summary>
    /// Beschreibung im Tooltip
    /// </summary>
    [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Category("UI")]
    public string? Description { get; set; }

    /// <summary>
    /// Tipp im Tooltip
    /// </summary>
    [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Category("UI")]
    public string? Hint { get; set; }

    /// <summary>
    /// Shortcut für den Menüpunkt
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public eKeys HotKey => eKeys.None;

    /// <inheritdoc />
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Toggle { get; set; }

    /// <summary>
    /// Speicherplatz für zusätzliche Informationen
    /// </summary>
    public object? Tag { get; set; }

    /// <inheritdoc />
    public eCommandContext CommandContext => eCommandContext.Other;

    /// <inheritdoc />
    public eCommand CommandType => eCommand.Other;
}