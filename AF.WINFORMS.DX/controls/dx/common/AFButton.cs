using DevExpress.Utils;

namespace AF.WINFORMS.DX;

/// <inheritdoc cref="DevExpress.XtraEditors.SimpleButton" />
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
[DesignerCategory("Code")]
public class AFButton : DevExpress.XtraEditors.SimpleButton, IAFControl
{

}