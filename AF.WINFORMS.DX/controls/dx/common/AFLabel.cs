using DevExpress.Utils;
using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Stil eines Labels
/// </summary>
[Flags]
public enum eLableStyle
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    Default = 1 << 0,
    Bold = 1 << 1,
    Big = 1 << 2,
    Gray = 1 << 3,
    Red = 1 << 4,
    Green = 1 << 5,
    Blue = 1 << 6,
    Yellow = 1 << 7,
    White = 1 << 8,
    Inverted = 1 << 9,
    Underline = 1 << 10
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}

/// <inheritdoc/>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
[DefaultBindingProperty("Text")]
[DesignerCategory("Code")]
public class AFLabel : LabelControl
{
    private bool _paintBackground;
    private bool _paintLeftBar;
    private bool _isHover;
    private bool _isClick;

    /// <inheritdoc/>
    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);

        if (UI.DesignMode) return;

        _isHover = false;
        _isClick = false;
        Invalidate();
    }

    /// <inheritdoc/>
    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);

        if (UI.DesignMode) return;

        _isHover = Enabled;
        Invalidate();
    }

    /// <inheritdoc/>
    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        if (UI.DesignMode) return;

        _isHover = Enabled;
        _isClick = Enabled;
        Invalidate();
    }

    /// <inheritdoc/>
    protected override void OnClick(EventArgs e)
    {
        if (UI.DesignMode)
        {
            base.OnClick(e);
            return;
        }

        _isHover = Enabled;
        _isClick = false;
        Invalidate();

        base.OnClick(e);
    }

    /// <inheritdoc/>
    protected override void OnEnabledChanged(EventArgs e)
    {
        base.OnEnabledChanged(e);

        if (UI.DesignMode) return;

        Invalidate();
    }

    /// <summary>
    /// True if mouse pointer is currently over the label
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    protected bool IsHover => _isHover;

    /// <summary>
    /// True if mouse pointer is currently clicked
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    protected bool IsClicked => _isClick;

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
            Invalidate();
        }
    }

    /// <summary>
    /// Appearance for custom drawing background
    /// </summary>
    [DefaultValue(null)]
    [Category("Custom background")]
    public AFBackgroundAppearance? BackgroundAppearance { get; set; }

    /// <summary>
    /// Appearance for custom drawing background on mopuse hover
    /// </summary>
    [DefaultValue(null)]
    [Category("Custom background")]
    public AFBackgroundAppearance? BackgroundAppearanceHover { get; set; }

    /// <summary>
    ///     Painting a colored bar at the left side of the label background.
    ///     This option is only used if PaintBackground is enabled.
    /// </summary>
    [DefaultValue(false)]
    [Category("Left bar")]
    public bool PaintBar
    {
        get => _paintLeftBar;
        set
        {
            _paintLeftBar = value;
            Invalidate();
        }
    }

    /// <summary>
    ///     Style to use for drawing
    /// </summary>
    [Browsable(true)]
    [DefaultValue(UIStyle.STANDARD)]
    public string Style { get; set; } = UIStyle.STANDARD;

    /// <summary>
    ///     Draw rounded rectangle
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

    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
         base.OnHandleCreated(e);

        if (UI.DesignMode) return;

        ApplyStyle();

        UI.StyleChanged += styleChanged;
    }

    /// <inheritdoc />
    protected override void OnHandleDestroyed(EventArgs e)
    {
        base.OnHandleDestroyed(e);

        if (UI.DesignMode) return;

        UI.StyleChanged -= styleChanged;
    }

    private void styleChanged(object? sender, EventArgs e)
    {
        if (UI.DesignMode) return;

        ApplyStyleChanges();
    }

    /// <summary>
    ///     apply style to label
    /// </summary>
    public void ApplyStyle()
    {
        var style = UI.GetStyle(Style);

        if (style == null) return;

        Appearance.ForeColor = style.ForeColor;
        Appearance.Font = style.Font;
        Appearance.Options.UseForeColor = true;
        Appearance.Options.UseFont = true;

        if (style.UseBackgroundColor)
        {
            Appearance.BackColor = style.BackgroundColor;
            Appearance.Options.UseBackColor = true;
        }
    }

    /// <summary>
    ///     apply changes in style after style changed event
    /// </summary>
    protected virtual void ApplyStyleChanges()
    {
        var style = UI.GetStyle(Style);

        if (style == null) return;

        Appearance.ForeColor = style.ForeColor;
        
        if (style.UseBackgroundColor)
        {
            Appearance.BackColor = style.BackgroundColor;
            Appearance.Options.UseBackColor = true;
        }
    }
}