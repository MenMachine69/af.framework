using DevExpress.Utils;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <inheritdoc/>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
public class AFEditComboLookup : LookUpEdit
{
    /// <summary>
    /// Methode die beim Erzeugen des Controls ausgeführt wird.
    /// </summary>
    /// <param name="e">Parameter</param>
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (UI.DesignMode) return;

        QueryPopUp += (_, _) =>
        {
            if (AutoPopupWidth)
                Properties.PopupFormMinSize = new(Width, Properties.PopupFormMinSize.Height);
        };
    }

    /// <summary>
    /// Bestimmt ob die Breite des Popups automatisch an die 
    /// Breite des Controls angepasst werden soll.
    /// </summary>
    [Browsable(true)]
    [DefaultValue(false)]
    public bool AutoPopupWidth { get; set; }
}