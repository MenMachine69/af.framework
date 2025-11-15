using DevExpress.Utils;
using DevExpress.Utils.Extensions;

namespace AF.WINFORMS.DX;

/// <inheritdoc/>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Layout")]
public class AFPanel : Panel
{
    private bool _paintBackground;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFPanel() : base()
    {

    }

    /// <summary>
    /// Custom draw Background using the BackgroundAppearances
    /// </summary>
    [Category("Custom background")]
    [Browsable(true)]
    [DefaultValue(false)]
    public bool CustomPaintBackground 
    {
        get => _paintBackground;
        set
        {
            _paintBackground = value;
            
            if (value)
                BackgroundAppearance ??= new();
            else
                BackgroundAppearance = null;

            Invalidate();
        }
    }

    /// <summary>
    /// Appearance for custom drawing background
    /// </summary>
    [DefaultValue(null)]
    [Category("Custom background")]
    public AFBackgroundAppearance? BackgroundAppearance { get; set; }

    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        SetStyle(ControlStyles.ResizeRedraw, true);
        DoubleBuffered = true;
    }


    /// <summary>
    ///     Draw background if needed
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPaintBackground(PaintEventArgs e)
    {
        base.OnPaintBackground(e);

        if (!CustomPaintBackground) return;

        if (BackgroundAppearance == null) return;

        var rect = ClientRectangle.WithDeflate(new Padding(0, 0, 1, 1));

        BackgroundAppearance.Draw(e.Graphics, rect, Margin, Padding);
    }
}