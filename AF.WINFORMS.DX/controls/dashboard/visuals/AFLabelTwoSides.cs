using System.Drawing.Drawing2D;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Ein Label, dass zwei Anteile in Balkenform darstellen kann.
/// </summary>
[DesignerCategory("Code")]
public class AFLabelTwoSides : LabelControl
{
    /// <summary>
    /// Links darzustellender Wert
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public decimal LeftValue { get; set; } = 0.5m;

    /// <summary>
    /// rechts darzustellender Wert
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public decimal RightValue { get; set; } = 0.5m;

    /// <summary>
    /// Farbe
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color LeftColor { get; set; } = UI.TranslateToSkinColor(Color.Green);

    /// <summary>
    /// Farbe des rechten Wertes
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color RightColor { get; set; } = UI.TranslateToSkinColor(Color.Red);

    /// <summary>
    /// Hintergrundfarbe des Balkens
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color BackColorBar { get; set; } = UI.TranslateToSkinColor(Color.LightGray);

    /// <summary>
    /// Höhe der Bar
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int BarHeight { get; set; } = 20;

    /// <summary>
    /// Transparenz des Kreises
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public byte Transparency { get; set; } = 255;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFLabelTwoSides() { this.AdjustAlignment(ContentAlignment.TopLeft);  }

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

        Rectangle rect = new(0 + Padding.Left, (Height - BarHeight) / 2, Width - Padding.Horizontal, BarHeight);

        e.Graphics.DrawRoundedRect(rect, Color.FromArgb(50, BackColorBar), Color.FromArgb(50, BackColorBar), 1, borderRadius: 3);

        if (Right <= 0 && Left <= 0) // nix füllen
            return;

        if (Right <= 0 && Left > 0) // alles von links füllen
        {
            e.Graphics.DrawRoundedRect(rect, Color.FromArgb(Transparency, LeftColor), Color.FromArgb(Transparency, LeftColor), 1, borderRadius: 3);
            return;
        }

        if (Left <= 0 && Right > 0) // alles von rechts füllen
        {
            e.Graphics.DrawRoundedRect(rect, Color.FromArgb(Transparency, RightColor), Color.FromArgb(Transparency, RightColor), 1, borderRadius: 3);
            return;
        }

        int leftsize = Convert.ToInt32(LeftValue * (Width - Padding.Horizontal) / (LeftValue + RightValue));
        
        rect = new(0 + Padding.Left, (Height - BarHeight) / 2, leftsize, BarHeight);
        e.Graphics.DrawLeftRoundedRect(rect, Color.FromArgb(Transparency, LeftColor), Color.FromArgb(Transparency, LeftColor), 1, borderRadius: 3);

        rect = new(0 + Padding.Left + leftsize, (Height - BarHeight) / 2, Width - Padding.Horizontal - leftsize, BarHeight);
        e.Graphics.DrawRightRoundedRect(rect, Color.FromArgb(Transparency, RightColor), Color.FromArgb(Transparency, RightColor), 1, borderRadius: 3);
    }
}