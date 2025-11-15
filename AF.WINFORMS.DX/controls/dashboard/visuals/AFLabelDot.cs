using System.Drawing.Drawing2D;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Ein Label, mit einem farbigen Punkt vor oder nach dem Text.
/// 
/// Der Punkt wird in der Padding-Area des Labels gezeichnet. 
/// Daher sollte der Padding-Bereich rechts/links groß genug sein den Punkt aufzunehmen (i.d.R mindestens die Höhe des Labels).
/// </summary>
[DesignerCategory("Code")]
public class AFLabelDot : LabelControl
{
    /// <summary>
    /// Farbe
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color Color { get; set; } = UI.TranslateToSkinColor(Color.Green);

    /// <summary>
    /// Position
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public HorizontalAlignment Alignment { get; set; } = HorizontalAlignment.Left;

    /// <summary>
    /// Transparenz des Kreises
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public byte Transparency { get; set; } = 255;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFLabelDot() { }

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

        e.Graphics.Clear(BackColor);


        int size = ClientRectangle.Height;

        Rectangle rect = new((Padding.Left - size) / 2, ClientRectangle.Top, size, size);

        using (SolidBrush brush = new(Color.FromArgb(Transparency, Color)))
            e.Graphics.FillEllipse(brush, rect); 
    }
}