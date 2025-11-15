using Alternet.Editor;

namespace AF.WINFORMS.DX;

/// <summary>
/// Erweiterungsmethoden für das SyntaxEdit von Alternate
/// </summary>
public static class SyntaxEditEx
{
    /// <summary>
    /// Setzt den Syntax-Modus für den SyntaxEdit
    /// </summary>
    /// <param name="edit">SyntaxEdit</param>
    /// <param name="mode">Modus</param>
    public static void SetMode(this SyntaxEdit edit, eSyntaxMode mode)
    {
        edit.Font = UI.ConsoleFont;
        edit.AllowDrop = true;
        edit.Scrolling.Options =
            ScrollingOptions.AllowSplitHorz |
            ScrollingOptions.SmoothScroll |
            ScrollingOptions.ShowScrollHint;
        edit.Outlining.AllowOutlining = true;
        edit.Outlining.ImageSize = 8;
        edit.SearchGlobal = false;
        edit.EditMargin.ShowHints = true;
        edit.Gutter.Options =
            GutterOptions.PaintLineNumbers |
            GutterOptions.PaintBookMarks |
            GutterOptions.PaintLineModificators |
            GutterOptions.SelectLineOnClick;
    }

    /// <summary>
    /// Passt die Darstellung des SyntaxEdit an den aktuellen Skin an    
    /// </summary>
    /// <param name="edit"></param>
    public static void SetSkin(this SyntaxEdit edit)
    {
        edit.VisualThemeType = UI.IsDarkSkin ? VisualThemeType.Dark : VisualThemeType.Light;
        edit.BackColor = UI.GetSkinPaletteColor("Paint High");
        edit.Gutter.BrushColor = UI.GetSkinPaletteColor("Paint Shadow");
        edit.Gutter.LineNumbersBackColor = UI.GetSkinPaletteColor("Paint");
    }
}

/// <summary>
/// Syntax-Modus für SyntaxEdit
/// </summary>
public enum eSyntaxMode
{
    /// <summary>
    /// C#
    /// </summary>
    CSharp,
    /// <summary>
    /// SQL
    /// </summary>
    SQL,
    /// <summary>
    /// MS SQL
    /// </summary>
    MSSQL,
    /// <summary>
    /// XML
    /// </summary>
    XML,
    /// <summary>
    /// HTML
    /// </summary>
    HTML,
    /// <summary>
    /// JavaScript
    /// </summary>
    JavaScript,
    /// <summary>
    /// Visual Basic
    /// </summary>
    VB,
    /// <summary>
    /// CSS
    /// </summary>
    CSS,
    /// <summary>
    /// JSON
    /// </summary>
    Json,
    /// <summary>
    /// PowerShell
    /// </summary>
    PowerShell
}