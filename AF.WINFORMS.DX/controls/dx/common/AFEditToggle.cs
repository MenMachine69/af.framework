using DevExpress.Utils;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <inheritdoc/>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
public class AFEditToggle : ToggleSwitch
{
    /// <inheritdoc />
    public AFEditToggle()
    {
        if (UI.DesignMode) return;

        Properties.AutoWidth = true;
        Properties.OnText = "Ja";
        Properties.OffText = "Nein";
        Properties.FullFocusRect = true;
        Properties.GlyphAlignment = HorzAlignment.Near;
    }
}