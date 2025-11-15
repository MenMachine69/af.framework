using DevExpress.Utils.Drawing;
using System.Drawing.Drawing2D;

namespace AF.WINFORMS.DX;

/// <summary>
/// Basisklasse eines Painters für Hintergründe
/// </summary>

[DesignerCategory("Code")]
public abstract class AFBasePainter : Component
{
    /// <summary>
    /// Default 
    /// </summary>
    [Browsable(true)]
    [DefaultValue(typeof(DevExpress.Utils.AppearanceObject))]
    public DevExpress.Utils.AppearanceObject AppearanceDefault { get; set; } = new();

    /// <summary>
    /// Disabled
    /// </summary>
    [Browsable(true)]
    [DefaultValue(typeof(DevExpress.Utils.AppearanceObject))]
    public DevExpress.Utils.AppearanceObject AppearanceDisabled { get; set; } = new();

    /// <summary>
    /// Hover
    /// </summary>
    [Browsable(true)]
    [DefaultValue(typeof(DevExpress.Utils.AppearanceObject))]
    public DevExpress.Utils.AppearanceObject AppearanceHover { get; set; } = new();

    /// <summary>
    /// Selected
    /// </summary>
    [Browsable(true)]
    [DefaultValue(typeof(DevExpress.Utils.AppearanceObject))]
    public DevExpress.Utils.AppearanceObject AppearanceSelected { get; set; } = new();

    /// <summary>
    /// Selected und Hoover
    /// </summary>
    [Browsable(true)]
    [DefaultValue(typeof(DevExpress.Utils.AppearanceObject))]
    public DevExpress.Utils.AppearanceObject AppearanceSelectedHover { get; set; } = new();

    /// <summary>
    /// Hintergrund zeichnen
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="clipRectangle"></param>
    /// <param name="state"></param>
    public virtual void Paint(Graphics graphics, Rectangle clipRectangle, eControlPaintState state)
    {
    }

    /// <summary>
    /// Hintergrund zeichnen
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="clipRectangle"></param>
    /// <param name="state"></param>
    public virtual void Paint(GraphicsCache graphics, Rectangle clipRectangle, eControlPaintState state)
    {
    }
}

/// <summary>
/// Painter für abgerundete Ecken
/// </summary>
[DesignerCategory("Code")]
public class AFRoundRectPainter : AFBasePainter
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="clipRectangle"></param>
    /// <param name="state"></param>
    public override void Paint(Graphics graphics, Rectangle clipRectangle, eControlPaintState state)
    {
        Color color = AppearanceDefault.BackColor;

        switch (state)
        {
            case eControlPaintState.Default:
                color = AppearanceDefault.BackColor;
                break;
            case eControlPaintState.Disabled:
                color = AppearanceDisabled.BackColor;
                break;
            case eControlPaintState.Hoover:
                color = AppearanceHover.BackColor;
                break;
            case eControlPaintState.Selected:
                color = AppearanceSelected.BackColor;
                break;
            case eControlPaintState.HooverSelected:
                color = AppearanceSelectedHover.BackColor;
                break;
        }

        graphics.SmoothingMode = SmoothingMode.HighQuality;

        using (SolidBrush brush = new(color))
        {
            using (GraphicsPath path = new(FillMode.Winding))
            {
                path.AddRectangle(new Rectangle(clipRectangle.X + 1 + ((clipRectangle.Height - 2) / 2),
                    clipRectangle.Y + 1, clipRectangle.Width - 4 - clipRectangle.Height, clipRectangle.Height - 1));
                path.AddEllipse(clipRectangle.X + 1, clipRectangle.Y + 1, clipRectangle.Height - 2,
                    clipRectangle.Height - 2);
                path.AddEllipse(clipRectangle.X + clipRectangle.Width - clipRectangle.Height - 3, clipRectangle.Y + 1,
                    clipRectangle.Height - 2, clipRectangle.Height - 2);
                path.CloseFigure();

                graphics.FillPath(brush, path);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="clipRectangle"></param>
    /// <param name="state"></param>
    public override void Paint(GraphicsCache graphics, Rectangle clipRectangle, eControlPaintState state)
    {
        Color color = state switch
        {
            eControlPaintState.Default => AppearanceDefault.BackColor,
            eControlPaintState.Disabled => AppearanceDisabled.BackColor,
            eControlPaintState.Hoover => AppearanceHover.BackColor,
            eControlPaintState.Selected => AppearanceSelected.BackColor,
            eControlPaintState.HooverSelected => AppearanceSelectedHover.BackColor,
            _ => AppearanceDefault.BackColor
        };

        graphics.SmoothingMode = SmoothingMode.HighQuality;

        using (SolidBrush brush = new(color))
        {
            using (GraphicsPath path = new(FillMode.Winding))
            {
                path.AddRectangle(new Rectangle(clipRectangle.X + 1 + ((clipRectangle.Height - 2) / 2),
                    clipRectangle.Y + 1, clipRectangle.Width - 4 - clipRectangle.Height, clipRectangle.Height - 1));
                path.AddEllipse(clipRectangle.X + 1, clipRectangle.Y + 1, clipRectangle.Height - 2,
                    clipRectangle.Height - 2);
                path.AddEllipse(clipRectangle.X + clipRectangle.Width - clipRectangle.Height - 3, clipRectangle.Y + 1,
                    clipRectangle.Height - 2, clipRectangle.Height - 2);
                path.CloseFigure();

                graphics.FillPath(brush, path);
            }
        }
    }
}

/// <summary>
/// zu zeichnender Status des Controls
/// </summary>
public enum eControlPaintState
{
    /// <summary>
    /// Standard
    /// </summary>
    Default,
    /// <summary>
    /// Disabled
    /// </summary>
    Disabled,
    /// <summary>
    /// Hoover
    /// </summary>
    Hoover,
    /// <summary>
    /// ausgewählt
    /// </summary>
    Selected,
    /// <summary>
    /// Hoover und ausgewählt
    /// </summary>
    HooverSelected
}

/// <summary>
/// Lable mit Hoover Effekt
/// </summary>

[DesignerCategory("Code")]
public class AFLabelControlHover : DevExpress.XtraEditors.LabelControl
{
    bool _hover;
    bool _selected;

    /// <summary>
    /// Painter für den Hintergrund
    /// </summary>
    [Browsable(true)]
    [DefaultValue(null)] 
    public AFBasePainter? ControlBackgroundPainter { get; set; }
    
    /// <inheritdoc/>
    protected override void OnPaintBackground(PaintEventArgs pevent)
    {
        base.OnPaintBackground(pevent);

        if (ControlBackgroundPainter != null)
        {
            eControlPaintState state = eControlPaintState.Default;
            Appearance.ForeColor = ControlBackgroundPainter.AppearanceDefault.ForeColor;
            if (Enabled)
            {
                if (_hover)
                {
                    if (_selected)
                    {
                        state = eControlPaintState.HooverSelected;
                        Appearance.ForeColor = ControlBackgroundPainter.AppearanceSelectedHover.ForeColor;
                    }
                    else
                    {
                        state = eControlPaintState.Hoover;
                        Appearance.ForeColor = ControlBackgroundPainter.AppearanceHover.ForeColor;
                    }
                }
            }
            else
            {
                state = eControlPaintState.Disabled;
                Appearance.ForeColor = ControlBackgroundPainter.AppearanceDisabled.ForeColor;
            }

            ControlBackgroundPainter.Paint(pevent.Graphics, pevent.ClipRectangle, state);
        }
    }

    /// <inheritdoc />
    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);

        _hover = true;

        Refresh();
    }

    /// <inheritdoc />
    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);

        _hover = false;

        Refresh();
    }

    /// <inheritdoc />
    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        _selected = true;

        Refresh();
    }

    /// <inheritdoc />
    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        _selected = false;

        Refresh();
    }
}


