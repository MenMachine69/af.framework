using DevExpress.Utils;
using DevExpress.Utils.Extensions;

namespace AF.WINFORMS.DX;

/// <summary>
/// Label um Text als wie eine Schaltfläche anzuzeigen
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
public class AFLabelButton : AFLabel
{
    /// <summary>
    /// style of the button
    /// </summary>
    [Browsable(true)]
    [DefaultValue(GraphicsEx.eButtonStyle.Standalone)]
    public GraphicsEx.eButtonStyle ButtonStyle { get; set; } = GraphicsEx.eButtonStyle.Standalone;

    /// <summary>
    /// draw buttons with gradient style
    /// </summary>
    [Browsable(true)]
    [DefaultValue(true)] 
    public bool EnableGradients { get; set; } = true;
     

    /// <summary>Paints the background of the control.</summary>
    /// <param name="pevent">A <see cref="T:System.Windows.Forms.PaintEventArgs" /> that contains information about the control to paint.</param>
    protected override void OnPaintBackground(PaintEventArgs pevent)
    {
        base.OnPaintBackground(pevent);

        if (UI.DesignMode) return;

        if (CustomPaintBackground) return;

        GraphicsEx.eHoverMode mode = GraphicsEx.eHoverMode.None;
        
        if (IsHover)
            mode = GraphicsEx.eHoverMode.Hover;

        if (IsClicked)
            mode = GraphicsEx.eHoverMode.Pressed;

        pevent.Graphics.DrawButton(
            ButtonStyle == GraphicsEx.eButtonStyle.Middle
                ? ClientRectangle.WithDeflate(new Padding(0, 0, 0, 1))
                : ClientRectangle.WithDeflate(new Padding(0, 0, 1, 1)), 
            ButtonStyle, mode,
            useGradients: EnableGradients);
    }
}