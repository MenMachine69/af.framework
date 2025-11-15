using System.Drawing.Drawing2D;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Ein Label, einen Wert in Form eines Kreises präsentieren kann
/// </summary>
[DesignerCategory("Code")]
public class AFLabelDonut : LabelControl
{
    /// <summary>
    /// Prozentual zu füllender Wert.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public decimal Percent { get; set; } = 0.5m;

    /// <summary>
    /// Farbe
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color Color { get; set; } = UI.TranslateToSkinColor(Color.Green);

    /// <summary>
    /// Hintergrundfarbe des Zirkels
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color BackColorCircle { get; set; } = UI.TranslateToSkinColor(Color.LightGray);


    /// <summary>
    /// Farbe für Zentrum
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color ColorCenter { get; set; } = UI.TranslateSystemToSkinColor(SystemColors.Window);

    /// <summary>
    /// Farbe für Zentrum
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int CircleWidth { get; set; } = 10;

    /// <summary>
    /// Transparenz des Kreises
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public byte Transparency { get; set; } = 255;

    /// <summary>
    /// Startwinkel in Grad (0 - 360). 0 Grad = links waagerecht.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public decimal StartAngel { get; set; } = 270.0m;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFLabelDonut()
    {

    }

    /// <summary>
    /// Hintergrund zeichnen...
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPaintBackground(PaintEventArgs e)
    {
        base.OnPaintBackground(e);

        e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
        e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

        e.Graphics.Clear(UI.TranslateSystemToSkinColor(SystemColors.Window));

        int size = Math.Min(Width - Padding.Horizontal - 2, Height - Padding.Vertical - 2);
        int xoffset = (Width - Padding.Horizontal) > (Height - Padding.Vertical) ? (Width - Padding.Horizontal - 2 - size) / 2 : 0;
        int yoffset = (Width - Padding.Horizontal) < (Height - Padding.Vertical) ? (Height - Padding.Horizontal - 2 - size) / 2 : 0;


        Rectangle rect = new(0 + xoffset, 0 + yoffset, Width - 2 - (xoffset * 2), Height - 2 - (yoffset * 2));

        if (Percent >= 1) // alles füllen
        {
            // using (HatchBrush brush = new(HatchStyle.WideUpwardDiagonal, Color.FromArgb(50, Color.Black), Color.FromArgb(Transparency, Color)))
            using (SolidBrush brush = new(Color.FromArgb(Transparency, Color)))
            { e.Graphics.FillEllipse(brush, rect); }
        }
        else if (Percent is > 0 and < 1)
        {
            // using (HatchBrush brush = new(HatchStyle.WideUpwardDiagonal, Color.FromArgb(50, Color.Black), Color.FromArgb(Transparency, BackColorCircle)))
            using (SolidBrush brush = new(Color.FromArgb(50, BackColorCircle)))
            { e.Graphics.FillEllipse(brush, rect); }

            decimal fDegValue = 360.0m * Percent;
            using (SolidBrush brush = new(Color.FromArgb(Transparency, Color)))
            { e.Graphics.FillPie(brush, rect, Convert.ToSingle(StartAngel), Convert.ToSingle(fDegValue)); }
        }

        int shrink = CircleWidth;
        rect = new(rect.X + shrink, rect.Y + shrink, rect.Width - (shrink * 2), rect.Height - (shrink * 2));

        using (SolidBrush brush = new(UI.TranslateSystemToSkinColor(SystemColors.Window)))
        { e.Graphics.FillEllipse(brush, rect); }

    }
}