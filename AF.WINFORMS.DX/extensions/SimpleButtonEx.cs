using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace AF.WINFORMS.DX;

/// <summary>
/// Erweiterungsmethoden für SimpleButton-Klasse
/// </summary>
public static class SimpleButtonEx
{
    /// <summary>
    /// Button in einem vordefinierten Stil konfigurieren.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="button">der Button</param>
    /// <param name="type"></param>
    /// <returns>angepasster Button</returns>
    public static T SetButton<T>(this T button, eButtonType type) where T : SimpleButton
    {
        button.ImageOptions.ImageToTextAlignment = ImageAlignToText.LeftCenter;
        button.ImageOptions.ImageToTextIndent = 10;
        button.ImageOptions.SvgImageColorizationMode = DevExpress.Utils.SvgImageColorizationMode.Full;
        button.ImageOptions.SvgImageSize = new(16, 16);

        switch (type)
        {
            case eButtonType.Menu:
                button.ImageOptions.SvgImage = UI.GetImage(Symbol.MoreVertical);
                button.PaintStyle = PaintStyles.Light;
                button.AllowFocus = false;
                button.ShowFocusRectangle = DefaultBoolean.False;
                break;
            case eButtonType.Flat:
                button.PaintStyle = PaintStyles.Light;
                button.AllowFocus = false;
                button.ShowFocusRectangle = DefaultBoolean.False;
                break;
            case eButtonType.Goto:
                button.ImageOptions.SvgImage = UI.GetImage(Symbol.GoTo);
                button.Text = "GEHE ZU";
                break;
            case eButtonType.Edit:
                button.ImageOptions.SvgImage = UI.GetImage(Symbol.Edit);
                button.PaintStyle = PaintStyles.Light;
                button.AllowFocus = false;
                button.ShowFocusRectangle = DefaultBoolean.False;
                break;
            case eButtonType.Customize:
                button.ImageOptions.SvgImage = UI.GetImage(Symbol.Wrench);
                button.PaintStyle = PaintStyles.Light;
                button.AllowFocus = false;
                button.ShowFocusRectangle = DefaultBoolean.False;
                break;
            case eButtonType.Save:
                button.ImageOptions.SvgImage = UI.GetImage(Symbol.Save);
                button.Text = "SPEICHERN";
                button.Appearance.BackColor = DXSkinColors.FillColors.Primary;
                button.Appearance.Options.UseBackColor = true;
                break;
            case eButtonType.Cancel:
                button.ImageOptions.SvgImage = UI.GetImage(Symbol.DismissCircle);
                button.Text = "ABBRECHEN";
                button.Appearance.BackColor = DXSkinColors.FillColors.Danger;
                button.Appearance.Options.UseBackColor = true;
                break;
            case eButtonType.Ok:
                button.ImageOptions.SvgImage = UI.GetImage(Symbol.CheckmarkCircle);
                button.Text = "OK";
                button.Appearance.BackColor = DXSkinColors.FillColors.Success;
                button.Appearance.Options.UseBackColor = true;
                break;
            case eButtonType.Delete:
                button.ImageOptions.SvgImage = UI.GetImage(Symbol.Delete);
                button.Text = "LÖSCHEN";
                button.Appearance.BackColor = DXSkinColors.FillColors.Danger;
                button.Appearance.Options.UseBackColor = true;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        return button;
    }
}

/// <summary>
/// Gestaltungsvarianten für einen SimpleButton 
/// </summary>
public enum eButtonType
{
    /// <summary>
    /// GeheZu
    /// </summary>
    Goto,
    /// <summary>
    /// Speichern
    /// </summary>
    Save,
    /// <summary>
    /// Abbrechen
    /// </summary>
    Cancel,
    /// <summary>
    /// Ok
    /// </summary>
    Ok,
    /// <summary>
    /// Löschen
    /// </summary>
    Delete,
    /// <summary>
    /// Editieren (kleiner Stift, Flat)
    /// </summary>
    Edit,
    /// <summary>
    /// Anpassen (kleiner Schraubenschlüssel, Flat)
    /// </summary>
    Customize,
    /// <summary>
    /// FlatStyle, kein Focus
    /// </summary>
    Flat,
    /// <summary>
    /// Zur Anzeige eines PopupMenus
    /// </summary>
    Menu
}
