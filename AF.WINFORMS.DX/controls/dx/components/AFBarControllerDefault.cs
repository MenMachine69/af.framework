using DevExpress.Utils;
using DevExpress.XtraBars;

namespace AF.WINFORMS.DX;

/// <summary>
/// Default-Controller für Toolbars und Docking
/// <seealso cref="DevExpress.XtraBars.DefaultBarAndDockingController"/>
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Components")]
public class AFBarControllerDefault : DefaultBarAndDockingController
{
}
