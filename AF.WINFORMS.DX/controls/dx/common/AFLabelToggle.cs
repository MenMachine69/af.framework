using DevExpress.Utils;
using DevExpress.Utils.Extensions;

namespace AF.WINFORMS.DX;

/// <summary>
/// ein Label, das Hover und Klick unterstützt, und aussieht wie eine
/// Schaltfläche mit abgerundeten Ecken.
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
public class AFLabelToggle : AFLabel
{
    private bool _isOn;

    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (UI.DesignMode) return;

        BackColor = Color.Transparent;
    }

    /// <summary>
    /// Current togggle state
    /// </summary>
    [Browsable(true)]
    [DefaultValue(false)]
    public bool IsOn { get => _isOn; set { _isOn = value; Invalidate(); } }


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

        pevent.Graphics.DrawToggleButton(ClientRectangle.WithDeflate(new Padding(0, 0, 1, 1)), mode, IsOn);
    }
}