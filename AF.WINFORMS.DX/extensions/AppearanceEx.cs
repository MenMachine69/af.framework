
using DevExpress.Utils;

namespace AF.WINFORMS.DX;

/// <summary>
/// Erweiterungsmethoden für alle AppearanceObject-Objekte
/// </summary>
[SupportedOSPlatform("windows")]
public static class AppearanceObjectEx
{
    /// <summary>
    /// Kopiert die AppearanceObject Einstellungen.
    /// </summary>
    /// <param name="target">Ziel</param>
    /// <param name="source">Quelle</param>
    public static void From(this AppearanceObject target, AppearanceObject source)
    {
        target.Options.UseBackColor = source.Options.UseBackColor;
        target.Options.UseForeColor = source.Options.UseForeColor;
        target.Options.UseBorderColor = source.Options.UseBorderColor;
        target.Options.UseTextOptions = source.Options.UseTextOptions;
        target.Options.UseFont = source.Options.UseFont;

        if (source.Options.UseForeColor)
            target.ForeColor = source.ForeColor;
        if (source.Options.UseBackColor)
            target.BackColor = source.BackColor;
        if (source.Options.UseBorderColor)
            target.BorderColor = source.BorderColor;

        if (source.Options.UseTextOptions)
        {
            target.TextOptions.HAlignment = source.TextOptions.HAlignment;
            target.TextOptions.VAlignment = source.TextOptions.VAlignment;
            target.TextOptions.WordWrap = source.TextOptions.WordWrap;
            target.TextOptions.Trimming = source.TextOptions.Trimming;
            target.TextOptions.HotkeyPrefix = source.TextOptions.HotkeyPrefix;
            target.TextOptions.RightToLeft = source.TextOptions.RightToLeft;
        }

        if (source.Options.UseFont)
        {
            target.Font = source.Font;
            target.FontSizeDelta = source.FontSizeDelta;
            target.FontStyleDelta = source.FontStyleDelta;
        }
    }
}
