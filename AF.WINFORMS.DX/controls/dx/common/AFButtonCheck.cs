using DevExpress.Utils;

namespace AF.WINFORMS.DX;

/// <inheritdoc cref="DevExpress.XtraEditors.CheckButton" />
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
[DesignerCategory("Code")]
public class AFButtonCheck : DevExpress.XtraEditors.CheckButton, IAFControl
{
}