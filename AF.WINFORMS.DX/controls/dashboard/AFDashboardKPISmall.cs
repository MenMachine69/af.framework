using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// KPI-Anzeige klein.
/// 
/// Stellt zusätzlich zum Text (Value) eine Caption dar. 
/// Value wird IMMER rechtsbündig angezeigt!
/// </summary>
[DesignerCategory("Code")]
public class AFDashboardKPISmall : AFLabel
{
    /// <summary>
    /// Caption/Bezeichnung des Wertes
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Caption { get; set; } = "";

    /// <summary>
    /// Farbe, in der der linke Rand 5px breit dargestellt wird.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color HighlightColor { get; set; } = Color.Transparent;
    

    /// <inheritdoc />
    public AFDashboardKPISmall()
    {
        //BackgroundAppearance = new() { AutoColors = true, CornerRadius = 3, Shadow = true };
        //BackgroundAppearanceHover = new() { AutoColors = true, CornerRadius = 3, Shadow = true };

        Paint += (_, e) =>
        {
            if(UI.DesignMode) return;

            var options1 = new LabelControlAppearanceObject();
            options1.Assign(Appearance);
            options1.Options.UseTextOptions = true;
            options1.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            options1.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            options1.TextOptions.WordWrap = DevExpress.Utils.WordWrap.NoWrap;
            options1.ForeColor = UI.TranslateToSkinColor(Color.Gray);
            options1.Options.UseForeColor = true;

            int barwidth = HighlightColor == Color.Transparent ? 0 : 5;

            Rectangle rect = new(barwidth + Padding.Left, Padding.Top, Width - barwidth - Padding.Horizontal, Height - Padding.Vertical);

            using (GraphicsCache cache = new(e.Graphics))
            {
                cache.FillRectangle(UI.TranslateSystemToSkinColor(SystemColors.Control), ClientRectangle);
                //cache.FillRoundedRectangle(UI.TranslateSystemToSkinColor(SystemColors.Window), ClientRectangle, new(4));

                if (barwidth > 0)
                    cache.FillRectangle(HighlightColor, new Rectangle(0, 0, barwidth, Height));

                if (!string.IsNullOrEmpty(Caption))
                    cache.DrawString($"<size=-1>{Caption}</size>", rect, appearance: options1);

                var options2 = new LabelControlAppearanceObject();
                options2.Assign(Appearance);
                options1.Options.UseTextOptions = true;
                options2.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
                options2.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;

                cache.DrawString(Text, rect, appearance: options2);

                //using (Pen pen = new(UI.TranslateSystemToSkinColor(SystemColors.Control)))
                //    cache.DrawLine(pen, new(ClientRectangle.Right - 1, 0), new(ClientRectangle.Right - 1, ClientRectangle.Bottom));
            }
        };
    }
}

