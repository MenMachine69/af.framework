using DevExpress.Utils;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <inheritdoc/>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
public class AFEditToken : TokenEdit
{
    /// <inheritdoc />
    public AFEditToken()
    {
        if (UI.DesignMode) return;

        Properties.DropDownShowMode = TokenEditDropDownShowMode.Outlook;
        Properties.EditMode = TokenEditMode.TokenList;
        Properties.EditValueType = TokenEditValueType.List;
        Properties.ExportMode = DevExpress.XtraEditors.Repository.ExportMode.Value;
        Properties.NullText = "[Kein Eintrag]";
    }
}