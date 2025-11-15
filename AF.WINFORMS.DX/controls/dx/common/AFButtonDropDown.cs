using DevExpress.Utils;

namespace AF.WINFORMS.DX;

/// <inheritdoc cref="DevExpress.XtraEditors.DropDownButton" />
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
[DesignerCategory("Code")]
public class AFButtonDropDown : DevExpress.XtraEditors.DropDownButton, IAFControl
{
}

