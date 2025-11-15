using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Basic control for all controls that can be used as overlay.
/// </summary>
[ToolboxItem(false)]
[SupportedOSPlatform("windows")]
public class OverlayControlDXBase : XtraUserControl, IOverlayControl
{
    /// <summary>
    /// OverlayDisplayManager that uses/manages the overlay.
    /// 
    /// Das Control kann die Methode CloseOverlay des OverlayDisplayManagers 
    /// aufrufen, um sich selbst aktiv zu schliessen.
    /// </summary>
    [Browsable(true)]
    [DefaultValue(null)]
    public AFOverlayDisplayManager? OverlayManager { get; set; }

    /// <summary>
    /// Raises the OverlayClosed event
    /// 
    /// Implement all needed things to do before the control was closed.
    /// </summary>
    public virtual void OverlayClosed() { }

    /// <summary>
    /// Raises the OverlayClosing event
    /// </summary>
    /// <param name="args">event arguments, set args.Cancel to true to avoid closing</param>
    public virtual void OverlayClosing(CancelEventArgs args) { }
}