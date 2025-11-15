using DevExpress.Utils;
using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <inheritdoc/>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Layout")]
public class AFSplitContainer : SplitContainerControl
{
    private bool _paintBackgroundPanel1;
    private bool _paintBackgroundPanel2;

    /// <summary>
    /// Custom draw Background using the BackgroundAppearances
    /// </summary>
    [Category("Custom background")]
    [DefaultValue(false)]
    public bool CustomPaintBackgroundPanel1 
    {
        get => _paintBackgroundPanel1;
        set
        {
            _paintBackgroundPanel1 = value;
            
            if (value)
                BackgroundAppearancePanel1 ??= new();
            else
                BackgroundAppearancePanel1 = null;

            Invalidate();
        }
    }

    /// <summary>
    /// Custom draw Background using the BackgroundAppearances
    /// </summary>
    [Category("Custom background")]
    [DefaultValue(false)]
    public bool CustomPaintBackgroundPanel2 
    {
        get => _paintBackgroundPanel2;
        set
        {
            _paintBackgroundPanel2 = value;
            
            if (value)
                BackgroundAppearancePanel2 ??= new();
            else
                BackgroundAppearancePanel2 = null;

            Invalidate();
        }
    }

    /// <summary>
    /// Appearance for custom drawing background
    /// </summary>
    [DefaultValue(null)]
    [Category("Custom background")]
    public AFBackgroundAppearance? BackgroundAppearancePanel1 { get; set; }

    /// <summary>
    /// Appearance for custom drawing background
    /// </summary>
    [DefaultValue(null)]
    [Category("Custom background")]
    public AFBackgroundAppearance? BackgroundAppearancePanel2 { get; set; }


    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        SetStyle(ControlStyles.ResizeRedraw, true);
        DoubleBuffered = true;

        Panel1.Paint += paintPanel1;
        Panel2.Paint += paintPanel2;
    }

    private void paintPanel1(object? sender, PaintEventArgs e)
    {
        if (!CustomPaintBackgroundPanel1) return;

        if (BackgroundAppearancePanel1 == null) return;

        var rect = Panel1.ClientRectangle.WithDeflate(new Padding(0, 0, 1, 1));

        BackgroundAppearancePanel1.Draw(e.Graphics, rect, Panel1.Margin, Panel1.Padding);
    }

    private void paintPanel2(object? sender, PaintEventArgs e)
    {
        if (!CustomPaintBackgroundPanel2) return;

        if (BackgroundAppearancePanel2 == null) return;

        var rect = Panel2.ClientRectangle.WithDeflate(new Padding(0, 0, 1, 1));

        BackgroundAppearancePanel2.Draw(e.Graphics, rect, Panel2.Margin, Panel2.Padding);
    }

    /// <inheritdoc />
    protected override void OnHandleDestroyed(EventArgs e)
    {
        Panel1.Paint -= paintPanel1;
        Panel2.Paint -= paintPanel2;

        base.OnHandleDestroyed(e);
    }
}