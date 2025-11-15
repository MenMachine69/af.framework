using DevExpress.Utils.Drawing;
using DevExpress.Utils;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Label, dass außer dem Text (Value) noch eine zusätzliche Überschrift enthält (z.B. für Dashboards)
/// </summary>
[DesignerCategory("Code")]
public class AFLabelValueCaption : LabelControl
{
    /// <summary>
    /// Caption, die zusätzlich zum Text angezeigt wird.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Caption { get; set; } = "";

    /// <summary>
    /// Rand/Linie zeichnen (abhängig vom Docking)
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DrawBorder { get; set; } = false;

    /// <summary>
    /// Aussehen der Caption.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AppearanceObject AppearanceCaption { get; init; }

    /// <summary>
    /// Constructor
    /// </summary>
    public AFLabelValueCaption()
    {
        AppearanceCaption = new(Appearance);
        AppearanceCaption.FontSizeDelta = -1;
        AppearanceCaption.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.GrayText);
        AppearanceCaption.TextOptions.HAlignment = HorzAlignment.Near;
        AppearanceCaption.TextOptions.VAlignment = VertAlignment.Top;
        AppearanceCaption.Options.UseTextOptions = true;
    }

    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (UI.DesignMode) return;

        Paint += customPaint;
        UI.StyleChanged += stylChanged;
    }

    private void stylChanged(object? sender, EventArgs e)
    {
        AppearanceCaption.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.GrayText);
        Appearance.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.Highlight);
    }

    /// <inheritdoc />
    protected override void OnHandleDestroyed(EventArgs e)
    {
        base.OnHandleDestroyed(e);

        if (UI.DesignMode) return;

        Paint -= customPaint;
        UI.StyleChanged -= stylChanged;
    }

    private void customPaint(object? sender, PaintEventArgs e)
    {
        using GraphicsCache cache = new(e.Graphics);

        if (!string.IsNullOrEmpty(Caption))
            cache.DrawString(Caption, new(0 + Padding.Left, 0 + Padding.Top, ClientRectangle.Width - Padding.Horizontal, ClientRectangle.Height - Padding.Vertical ), appearance: AppearanceCaption);

        if (DrawBorder)
        {
            using Pen pen = new(UI.TranslateSystemToSkinColor(SystemColors.GrayText));

            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

            switch(Dock)
            {
                case DockStyle.None:
                    break;
                case DockStyle.Bottom:
                    cache.DrawLine(pen, new(0, 0), new(Width, 0));
                    break;
                case DockStyle.Top:
                    cache.DrawLine(pen, new(0, Height), new(Width, Height));
                    break;
                case DockStyle.Right:
                    cache.DrawLine(pen, new(0, 0), new(0, Height));
                    break;
                case DockStyle.Left:
                    cache.DrawLine(pen, new(Width - 1, 0), new(Width - 1, Height));
                    break;
                case DockStyle.Fill:
                    break;
            }
        }
    }
}

