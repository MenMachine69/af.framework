using DevExpress.Utils;
using DevExpress.XtraBars;

namespace AF.WINFORMS.DX;

/// <summary>
/// Contral, dass eine Toolbar enthalten kann
/// <seealso cref="DevExpress.XtraBars.StandaloneBarDockControl"/>
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Components")]
public class AFBarDockControl : StandaloneBarDockControl
{
}