using DevExpress.Utils;
using DevExpress.Utils.Extensions;
using DevExpress.Utils.Layout;

namespace AF.WINFORMS.DX;

/// <inheritdoc/>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Layout")]
public class AFStackPanel : StackPanel
{
    private bool _paintBackground;
    private bool _autoSizeChilds;


    /// <summary>
    /// Adjust width/height of all child controls automatic
    /// </summary>
    [Browsable(true)]
    [DefaultValue(false)]
    public bool AutoSizeChilds
    {
        get => _autoSizeChilds;
        set { _autoSizeChilds = value; Invalidate(); }
    }

    /// <summary>Löst das <see cref="System.Windows.Forms.Control.SizeChanged" />-Ereignis aus.</summary>
    /// <param name="e">Ein <see cref="System.EventArgs" />, das die Ereignisdaten enthält.</param>
    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);

        if (UI.DesignMode) return;

        AlignChilds();
    }

    /// <summary>
    /// Größe der Controls anpassen
    /// </summary>
    public void AlignChilds()
    {
        if (!AutoSizeChilds) return;

        foreach (Control ctrl in Controls)
        {
            switch (LayoutDirection)
            {
                case StackPanelLayoutDirection.LeftToRight:
                case StackPanelLayoutDirection.RightToLeft:
                    ctrl.Size = new Size(ctrl.Width, ClientRectangle.Height - ctrl.Margin.Vertical);
                    break;
                case StackPanelLayoutDirection.TopDown:
                case StackPanelLayoutDirection.BottomUp:
                    ctrl.Size = new Size(ClientRectangle.Width - ctrl.Margin.Horizontal, ctrl.Height);
                    break;
            }
        }
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

        AlignChilds();
    }


    /// <summary>
    ///     Draw background if needed
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (!CustomPaintBackground) return;

        if (BackgroundAppearance == null) return;

        var rect = ClientRectangle.WithDeflate(new Padding(0, 0, 1, 1));

        BackgroundAppearance.Draw(e.Graphics, rect, Margin, Padding);
    }
}