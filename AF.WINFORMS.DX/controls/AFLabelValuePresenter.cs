namespace AF.WINFORMS.DX;


/// <summary>
/// a label that presents a second value and a color bar
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
public class AFLabelValuePresenter : AFLabel
{
    /// <summary>
    /// Displayed on top as Caption
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Caption { get; set; } = "";

    /// <summary>
    /// Value description - displayed on bottom as small description text
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Description { get; set; } = "";


    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (UI.DesignMode) return;

        SetStyle(ControlStyles.ResizeRedraw, true);
        SetStyle(ControlStyles.AllPaintingInWmPaint, true);
    }

    /// <inheritdoc />
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        float left = Margin.Left + 5; //BarWidth + 5;
        float right = Margin.Right + 5;


        if (UI.DesignMode)
        {
            if (!string.IsNullOrEmpty(Caption)) 
                e.Graphics.DrawString(Caption, SystemFonts.DefaultFont, SystemBrushes.ControlText, left, Padding.Top + Margin.Top);

            if (!string.IsNullOrEmpty(Description)) 
            {
                int txtwidth = e.Graphics.GetStringWidth(Description, SystemFonts.DefaultFont);
                int txtheight = e.Graphics.GetStringHeight(Description, SystemFonts.DefaultFont);

                e.Graphics.DrawString(Description, SystemFonts.DefaultFont, SystemBrushes.ControlDark, Width - right - txtwidth, Height - Margin.Bottom - Padding.Bottom - txtheight);
            } 

            return;
        }

        if (!string.IsNullOrEmpty(Caption)) 
        {
            using (SolidBrush brush = new(UI.GetStyle(UIStyle.STANDARD)!.ForeColor))
                e.Graphics.DrawString(Caption, UI.GetStyle(UIStyle.STANDARD)!.Font, brush, left, Padding.Top + Margin.Top);
        } 

        if (!string.IsNullOrEmpty(Description)) 
        {
            int txtwidth = e.Graphics.GetStringWidth(Description, UI.GetStyle(UIStyle.STANDARD)!.Font);
            int txtheight = e.Graphics.GetStringHeight(Description, UI.GetStyle(UIStyle.STANDARD)!.Font);

            using (SolidBrush brush = new(UI.GetStyle(UIStyle.GRAY)!.ForeColor))
                e.Graphics.DrawString(Description, UI.GetStyle(UIStyle.GRAY)!.Font, brush, Width - right - txtwidth, Height - Margin.Bottom - Padding.Bottom - txtheight);
        } 
    }
}

