using DevExpress.Utils;

namespace AF.WINFORMS.DX;

/// <inheritdoc cref="DevExpress.XtraEditors.Controls.CalendarControl" />
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
[DesignerCategory("Code")]
public class AFCalendar : DevExpress.XtraEditors.Controls.CalendarControl, IAFControl
{

}