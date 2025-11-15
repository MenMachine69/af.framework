using DevExpress.Utils;
using DevExpress.XtraBars;

namespace AF.WINFORMS.DX;

/// <summary>
/// Manager für klassische Toolbars
/// <seealso cref="DevExpress.XtraBars.BarManager"/>
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Components")]
public class AFBarManager : BarManager
{
}