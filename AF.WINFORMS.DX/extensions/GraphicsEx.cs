using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Text;

namespace AF.WINFORMS.DX;

/// <summary>
/// Erweiterungsmethoden für die Klasse System.Drawing.Graphics
/// </summary>
[SupportedOSPlatform("windows")]
public static class GraphicsEx
{

    /// <summary>
    /// Zeichnet einen Weg (FindWay) zwischen zei Rechtecken
    /// </summary>
    /// <param name="graphics">Zeichenfläche</param>
    /// <param name="points">zu verbindende Punkte</param>
    /// <param name="pen">Zeichenstift</param>
    /// <param name="backColor">Hintergrundfarbe</param>
    /// <param name="label">auf der Linie zu zeichnendes Label (optional)</param>
    /// <param name="font">Font für das Label (optional)</param>
    public static void DrawWay(this Graphics graphics, Point[] points, Pen pen, Color backColor, Font? font = null, string? label = null)
    {
        if (points.Length < 2) return;

        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
        graphics.CompositingQuality = CompositingQuality.HighQuality;

        Point start = points[0];
        pen.StartCap = LineCap.RoundAnchor;
        for (int i = 1; i < points.Length; ++i)
        {
            if (i >= points.Length - 1)
                pen.EndCap = LineCap.ArrowAnchor;

            graphics.DrawLine(pen, start, points[i]);
            start = points[i];

            pen.StartCap = LineCap.NoAnchor;
        }

        if (string.IsNullOrWhiteSpace(label)) return;

        // Mittelpunkt des Labels berechnen
        Point txtPoint = new((points[1].X + points[2].X) / 2, (points[1].Y + points[2].Y) / 2);
        Size txtSize = TextRenderer.MeasureText(label, font ?? SystemFonts.DefaultFont);

        Rectangle txtRect = new(
            txtPoint.X - (txtSize.Width / 2) - 3, 
            txtPoint.Y - (txtSize.Height / 2) - 3,
            txtSize.Width + 6, txtSize.Height + 6);


        using SolidBrush brush = new SolidBrush(backColor); 
        graphics.FillRectangle(brush, txtRect);

        TextRenderer.DrawText(graphics, label, font ?? SystemFonts.DefaultFont, txtRect, pen.Color, 
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
    }


    /// <summary>
    /// Ermittelt die Wegpunkte um zwei Rechtecke miteinander zu verbinden
    /// </summary>
    /// <param name="ptStart">Ausgangspunkt</param>
    /// <param name="ptEnd">Endpunkt</param>
    /// <param name="rectStart">Ausgangsrechteck</param>
    /// <param name="rectEnd">Endrechteck</param>
    /// <param name="offset">Offset für Start und Endpunkt (optional, default = 20)</param>
    /// <returns>Liste der Punkte die verbunden werden müssen</returns>
    public static Point[] FindWay(Point ptStart, Point ptEnd, Rectangle rectStart, Rectangle rectEnd, int offset = 20)
    {
        return [
            ptStart,
            getPos(ptStart, rectStart, offset),
            getPos(ptEnd, rectEnd, offset),
            ptEnd
        ];
    }

    /// <summary>
    /// Offset für einen Punkt bezogen auf das Rechteck erzeugen.
    /// </summary>
    /// <param name="pt">Punkt</param>
    /// <param name="rect">Rechteck</param>
    /// <param name="offset">Offset für den Punkt</param>
    /// <returns>erzeugter Offset-Punkt</returns>
    private static Point getPos(Point pt, Rectangle rect, int offset)
    {
        Point pos = new(pt.X, pt.Y);

        if (pt.X > rect.Left)
        {
            if (pt.X < rect.Right)
                pos.Y += pt.Y > rect.Top ? offset : 0- offset;
            else
                pos.X += offset;
        }
        else
            pos.X -= offset;

        return pos;
    }

    /// <summary>
    /// Gibt die Höhe eines Textes bei vorgegebener Breite zurück
    /// </summary>
    /// <param name="graphics">Zeichenfläche</param>
    /// <param name="text">der Text dessen Höhe ermittelt werden soll</param>
    /// <param name="font">der Font mit dem der Text gezeichnet werden soll</param>
    /// <param name="width">die Breite des Textes</param>
    /// <returns></returns>
    public static int GetStringHeight(this Graphics graphics, string text, Font font, int width)
    {
        return (int)graphics.MeasureString(text, font, width).Height;
    }

    /// <summary>
    /// Gibt die Höhe eines Textes zurück
    /// </summary>
    /// <param name="graphics">Zeichenfläche</param>
    /// <param name="text">der Text dessen Höhe ermittelt werden soll</param>
    /// <param name="font">der Font mit dem der Text gezeichnet werden soll</param>
    /// <returns></returns>
    public static int GetStringHeight(this Graphics graphics, string text, Font font)
    {
        return (int)graphics.MeasureString(text, font).Height;
    }

    /// <summary>
    /// Gibt die Breite eines Textes an (einzeilig)
    /// </summary>
    /// <param name="graphics">Zeichenfläche</param>
    /// <param name="text">der Text dessen Höhe ermittelt werden soll</param>
    /// <param name="font">der Font mit dem der Text gezeichnet werden soll</param>
    /// <returns></returns>
    public static int GetStringWidth(this Graphics graphics, string text, Font font)
    {
        return (int)graphics.MeasureString(text, font).Width;
    }

    /// <summary>
    /// Gibt die Größe des Textes zurück
    /// </summary>
    /// <param name="graphics">Zeichenfläche</param>
    /// <param name="text">der Text dessen Höhe ermittelt werden soll</param>
    /// <param name="font">der Font mit dem der Text gezeichnet werden soll</param>
    /// <returns></returns>
    public static SizeF GetStringSize(this Graphics graphics, string text, Font font)
    {
        return graphics.MeasureString(text, font);
    }

    /// <summary>
    /// Draw a rectangle with rounded borders
    /// </summary>
    /// <param name="graphics">canvas</param>
    /// <param name="rect">rectangle to fill</param>
    /// <param name="color">fill color if no brush defined</param>
    /// <param name="borderColor">border color</param>
    /// <param name="borderWidth">border with in pixel (0 = no border)</param>
    /// <param name="borderRadius">radius for the corners ( 8 = default)</param>
    /// <param name="brush">brush to use for filling (null = fill with solid color)</param>
    public static void DrawRoundedRect(this Graphics graphics, Rectangle rect, Color color, Color? borderColor, int borderWidth = 0, int borderRadius = 8, Brush? brush = null)
    {
        
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Create a rounded rectangle path based on the specified properties
        using GraphicsPath path = GetPath(rect, borderRadius, eCornerMode.All);

        fillRectanglePath(graphics, path, color, borderColor, borderWidth, brush);
    }

    /// <summary>
    /// Draw a rectangle with rounded borders at the left side
    /// </summary>
    /// <param name="graphics">canvas</param>
    /// <param name="rect">rectangle to fill</param>
    /// <param name="color">fill color if no brush defined</param>
    /// <param name="borderColor">border color</param>
    /// <param name="borderWidth">border with in pixel (0 = no border)</param>
    /// <param name="borderRadius">radius for the corners ( 8 = default)</param>
    /// <param name="brush">brush to use for filling (null = fill with solid color)</param>
    public static void DrawLeftRoundedRect(this Graphics graphics, Rectangle rect, Color color, Color? borderColor, int borderWidth = 0, int borderRadius = 8, Brush? brush = null)
    {

        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Create a rounded rectangle path based on the specified properties
        using GraphicsPath path = GetPath(rect, borderRadius, eCornerMode.Left);

        fillRectanglePath(graphics, path, color, borderColor, borderWidth, brush);
        
    }

    /// <summary>
    /// Draw a rectangle with rounded borders at the left side
    /// </summary>
    /// <param name="graphics">canvas</param>
    /// <param name="rect">rectangle to fill</param>
    /// <param name="color">fill color if no brush defined</param>
    /// <param name="borderColor">border color</param>
    /// <param name="borderWidth">border with in pixel (0 = no border)</param>
    /// <param name="borderRadius">radius for the corners ( 8 = default)</param>
    /// <param name="brush">brush to use for filling (null = fill with solid color)</param>
    public static void DrawRightRoundedRect(this Graphics graphics, Rectangle rect, Color color, Color? borderColor, int borderWidth = 0, int borderRadius = 8, Brush? brush = null)
    {

        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Create a rounded rectangle path based on the specified properties
        using GraphicsPath path = GetPath(rect, borderRadius, eCornerMode.Right);

        fillRectanglePath(graphics, path, color, borderColor, borderWidth, brush);

    }

    /// <summary>
    /// Draw a rectangle with rounded borders at the top side
    /// </summary>
    /// <param name="graphics">canvas</param>
    /// <param name="rect">rectangle to fill</param>
    /// <param name="color">fill color if no brush defined</param>
    /// <param name="borderColor">border color</param>
    /// <param name="borderWidth">border with in pixel (0 = no border)</param>
    /// <param name="borderRadius">radius for the corners ( 8 = default)</param>
    /// <param name="brush">brush to use for filling (null = fill with solid color)</param>
    public static void DrawTopRoundedRect(this Graphics graphics, Rectangle rect, Color color, Color? borderColor, int borderWidth = 0, int borderRadius = 8, Brush? brush = null)
    {

        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Create a rounded rectangle path based on the specified properties
        using GraphicsPath path = GetPath(rect, borderRadius, eCornerMode.Top);

        fillRectanglePath(graphics, path, color, borderColor, borderWidth, brush);
    }

    /// <summary>
    /// Draw a rectangle with rounded borders at the top side
    /// </summary>
    /// <param name="graphics">canvas</param>
    /// <param name="rect">rectangle to fill</param>
    /// <param name="color">fill color if no brush defined</param>
    /// <param name="borderColor">border color</param>
    /// <param name="borderWidth">border with in pixel (0 = no border)</param>
    /// <param name="borderRadius">radius for the corners (8 = default)</param>
    /// <param name="brush">brush to use for filling (null = fill with solid color)</param>
    public static void DrawBottomRoundedRect(this Graphics graphics, Rectangle rect, Color color, Color? borderColor, int borderWidth = 0, int borderRadius = 8, Brush? brush = null)
    {

        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Create a rounded rectangle path based on the specified properties
        using GraphicsPath path = GetPath(rect, borderRadius, eCornerMode.Bottom);

        fillRectanglePath(graphics, path, color, borderColor, borderWidth, brush);

    }

    /// <summary>
    /// Draw a rectangle with borders
    /// </summary>
    /// <param name="graphics">canvas</param>
    /// <param name="rect">rectangle to fill</param>
    /// <param name="color">fill color if no brush defined</param>
    /// <param name="borderColor">border color</param>
    /// <param name="borderWidth">border with in pixel (0 = no border)</param>
    /// <param name="brush">brush to use for filling (null = fill with solid color)</param>
    public static void DrawRect(this Graphics graphics, Rectangle rect, Color color, Color? borderColor, int borderWidth = 0, Brush? brush = null)
    {

        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Create a rounded rectangle path based on the specified properties
        using GraphicsPath path = GetPath(rect, 0, eCornerMode.None);

        fillRectanglePath(graphics, path, color, borderColor, borderWidth, brush);

    }

    /// <summary>
    /// Draw a rectangle with rounded borders
    /// </summary>
    /// <param name="graphics">canvas</param>
    /// <param name="rect">rectangle to fill</param>
    /// <param name="color">fill color if no brush defined</param>
    /// <param name="borderColor">border color</param>
    /// <param name="borderWidth">border with in pixel (0 = no border)</param>
    /// <param name="borderRadius">radius for the corners ( 8 = default)</param>
    /// <param name="brush">brush to use for filling (null = fill with solid color)</param>
    public static void DrawRoundedRect(this GraphicsCache graphics, Rectangle rect, Color color, Color? borderColor, int borderWidth = 0, int borderRadius = 8, Brush? brush = null)
    {
        
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Create a rounded rectangle path based on the specified properties
        using GraphicsPath path = GetPath(rect, borderRadius, eCornerMode.All);

        fillRectanglePath(graphics, path, color, borderColor, borderWidth, brush);
    }

    /// <summary>
    /// Draw a rectangle with rounded borders at the left side
    /// </summary>
    /// <param name="graphics">canvas</param>
    /// <param name="rect">rectangle to fill</param>
    /// <param name="color">fill color if no brush defined</param>
    /// <param name="borderColor">border color</param>
    /// <param name="borderWidth">border with in pixel (0 = no border)</param>
    /// <param name="borderRadius">radius for the corners ( 8 = default)</param>
    /// <param name="brush">brush to use for filling (null = fill with solid color)</param>
    public static void DrawLeftRoundedRect(this GraphicsCache graphics, Rectangle rect, Color color, Color? borderColor, int borderWidth = 0, int borderRadius = 8, Brush? brush = null)
    {
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Create a rounded rectangle path based on the specified properties
        using GraphicsPath path = GetPath(rect, borderRadius, eCornerMode.Left);

        fillRectanglePath(graphics, path, color, borderColor, borderWidth, brush);
        
    }

    /// <summary>
    /// Draw a rectangle with rounded borders at the left side
    /// </summary>
    /// <param name="graphics">canvas</param>
    /// <param name="rect">rectangle to fill</param>
    /// <param name="color">fill color if no brush defined</param>
    /// <param name="borderColor">border color</param>
    /// <param name="borderWidth">border with in pixel (0 = no border)</param>
    /// <param name="borderRadius">radius for the corners ( 8 = default)</param>
    /// <param name="brush">brush to use for filling (null = fill with solid color)</param>
    public static void DrawRightRoundedRect(this GraphicsCache graphics, Rectangle rect, Color color, Color? borderColor, int borderWidth = 0, int borderRadius = 8, Brush? brush = null)
    {

        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Create a rounded rectangle path based on the specified properties
        using GraphicsPath path = GetPath(rect, borderRadius, eCornerMode.Right);

        fillRectanglePath(graphics, path, color, borderColor, borderWidth, brush);

    }

    /// <summary>
    /// Draw a rectangle with rounded borders at the top side
    /// </summary>
    /// <param name="graphics">canvas</param>
    /// <param name="rect">rectangle to fill</param>
    /// <param name="color">fill color if no brush defined</param>
    /// <param name="borderColor">border color</param>
    /// <param name="borderWidth">border with in pixel (0 = no border)</param>
    /// <param name="borderRadius">radius for the corners ( 8 = default)</param>
    /// <param name="brush">brush to use for filling (null = fill with solid color)</param>
    public static void DrawTopRoundedRect(this GraphicsCache graphics, Rectangle rect, Color color, Color? borderColor, int borderWidth = 0, int borderRadius = 8, Brush? brush = null)
    {

        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Create a rounded rectangle path based on the specified properties
        using GraphicsPath path = GetPath(rect, borderRadius, eCornerMode.Top);

        fillRectanglePath(graphics, path, color, borderColor, borderWidth, brush);
    }

    /// <summary>
    /// Draw a rectangle with rounded borders at the top side
    /// </summary>
    /// <param name="graphics">canvas</param>
    /// <param name="rect">rectangle to fill</param>
    /// <param name="color">fill color if no brush defined</param>
    /// <param name="borderColor">border color</param>
    /// <param name="borderWidth">border with in pixel (0 = no border)</param>
    /// <param name="borderRadius">radius for the corners (8 = default)</param>
    /// <param name="brush">brush to use for filling (null = fill with solid color)</param>
    public static void DrawBottomRoundedRect(this GraphicsCache graphics, Rectangle rect, Color color, Color? borderColor, int borderWidth = 0, int borderRadius = 8, Brush? brush = null)
    {

        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Create a rounded rectangle path based on the specified properties
        using GraphicsPath path = GetPath(rect, borderRadius, eCornerMode.Bottom);

        fillRectanglePath(graphics, path, color, borderColor, borderWidth, brush);

    }

    /// <summary>
    /// Draw a rectangle with borders
    /// </summary>
    /// <param name="graphics">canvas</param>
    /// <param name="rect">rectangle to fill</param>
    /// <param name="color">fill color if no brush defined</param>
    /// <param name="borderColor">border color</param>
    /// <param name="borderWidth">border with in pixel (0 = no border)</param>
    /// <param name="brush">brush to use for filling (null = fill with solid color)</param>
    public static void DrawRect(this GraphicsCache graphics, Rectangle rect, Color color, Color? borderColor, int borderWidth = 0, Brush? brush = null)
    {

        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Create a rounded rectangle path based on the specified properties
        using GraphicsPath path = GetPath(rect, 0, eCornerMode.None);

        fillRectanglePath(graphics, path, color, borderColor, borderWidth, brush);

    }

    /// <summary>
    /// Draw a button
    /// </summary>
    /// <param name="graphics">canvas</param>
    /// <param name="rect">rectangle to draw</param>
    /// <param name="style">button style</param>
    /// <param name="mode">current button mode</param>
    /// <param name="useGradients">draw with gradients</param>
    public static void DrawButton(this Graphics graphics, Rectangle rect, eButtonStyle style, eHoverMode mode, bool useGradients = true)
    {
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        eCornerMode cornerMode = style switch
        {
            eButtonStyle.First => eCornerMode.Left,
            eButtonStyle.Middle => eCornerMode.None,
            eButtonStyle.Last => eCornerMode.Right,
            eButtonStyle.Standalone => eCornerMode.All,
            _ => eCornerMode.All
        };

        using GraphicsPath path = GetPath(rect, 5, cornerMode);

        // fill with basic color
        fillRectanglePath(graphics, path, UI.TranslateSystemToSkinColor(SystemColors.Window), null);


        using LinearGradientBrush brush = useGradients 
            ? new(rect, Color.FromArgb(38,getButtonStartColor(mode)), Color.FromArgb(18, getButtonEndColor(mode)), LinearGradientMode.Vertical)
            : new(rect, Color.FromArgb(38, getButtonStartColor(mode)), Color.FromArgb(38, getButtonStartColor(mode)), LinearGradientMode.Vertical);

        using Pen pen = new(Color.FromArgb(75, UI.TranslateSystemToSkinColor(SystemColors.ControlText)));

        if (style == eButtonStyle.Standalone)
        {
            fillRectanglePath(graphics, path, UI.TranslateSystemToSkinColor(SystemColors.Window), UI.TranslateSystemToSkinColor(SystemColors.ControlDark), 1, brush);
            graphics.DrawPath(pen, path);
        }
        else if (style == eButtonStyle.Middle)
        { 
            fillRectanglePath(graphics, path, UI.TranslateSystemToSkinColor(SystemColors.Window), null, 0, brush);
            graphics.DrawLine(pen, rect.X, rect.Y, rect.X + rect.Width, rect.Y);
            graphics.DrawLine(pen, rect.X, rect.Y + rect.Height, rect.X + rect.Width, rect.Y + rect.Height);
        }
        else
        {
            using GraphicsPath buttonPath = GetButtonBorderPath(rect, 5, style);

            fillRectanglePath(graphics, path, UI.TranslateSystemToSkinColor(SystemColors.Window), null, 0, brush);
            graphics.DrawPath(pen, buttonPath);
        }
    }

    /// <summary>
    /// Draw a button
    /// </summary>
    /// <param name="graphics">canvas</param>
    /// <param name="rect">rectangle to draw</param>
    /// <param name="mode">current button mode</param>
    /// <param name="isOn">eingeschaltet ja/nein</param>
    public static void DrawToggleButton(this Graphics graphics, Rectangle rect, eHoverMode mode, bool isOn)
    {
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        eCornerMode cornerMode = eCornerMode.All;

        using GraphicsPath path = GetPath(rect, (rect.Height / 2) - 1, cornerMode);

        Color color = Color.FromArgb(isOn 
            ? 255 
            : mode == eHoverMode.Hover 
                ? 50 
                : mode == eHoverMode.Pressed 
                    ? 255 
                    : 0 , UI.TranslateSystemToSkinColor(SystemColors.Highlight));

        using SolidBrush brush = new(color);

        fillRectanglePath(graphics, path, color, color, 0, brush);
    }

    private static Color getButtonStartColor(eHoverMode mode)
    {
        return mode switch
        {
            eHoverMode.None => UI.TranslateSystemToSkinColor(SystemColors.Window),
            eHoverMode.Hover => UI.TranslateSystemToSkinColor(SystemColors.Highlight),
            eHoverMode.Pressed => UI.TranslateSystemToSkinColor(SystemColors.ControlText),
            _ => UI.TranslateSystemToSkinColor(SystemColors.Window)
        };
    }

    private static Color getButtonEndColor(eHoverMode mode)
    {
        return mode switch
        {
            eHoverMode.None => UI.TranslateSystemToSkinColor(SystemColors.WindowText),
            eHoverMode.Hover => UI.TranslateSystemToSkinColor(SystemColors.HighlightText),
            eHoverMode.Pressed => UI.TranslateSystemToSkinColor(SystemColors.Control),
            _ => UI.TranslateSystemToSkinColor(SystemColors.WindowText)
        };
    }

    private static void fillRectanglePath(Graphics graphics, GraphicsPath path, Color color, Color? borderColor, int borderWidth = 0, Brush? brush = null)
    {
        if (brush == null)
        {
            // Fill the rounded rectangle with color
            using SolidBrush solidbrush = new(color);
            graphics.FillPath(solidbrush, path);
        }
        else
            graphics.FillPath(brush, path);

        if (borderWidth <= 0 || borderColor == null) return;


        using Pen pen = new((Color)borderColor, borderWidth);
        graphics.DrawPath(pen, path);
    }

    private static void fillRectanglePath(GraphicsCache graphics, GraphicsPath path, Color color, Color? borderColor, int borderWidth = 0, Brush? brush = null)
    {
        if (brush == null)
        {
            // Fill the rounded rectangle with color
            using SolidBrush solidbrush = new(color);
            graphics.FillPath(solidbrush, path);
        }
        else
            graphics.FillPath(brush, path);

        if (borderWidth <= 0 || borderColor == null) return;


        using Pen pen = new((Color)borderColor, borderWidth);
        graphics.DrawPath(pen, path);
    }

    private static GraphicsPath GetPath(Rectangle rect, int borderRadius, eCornerMode mode)
    {
        if (borderRadius < 1)
            mode = eCornerMode.None;

        GraphicsPath path = new ();
        if (mode == eCornerMode.All)
        {
            path.AddArc(rect.X, rect.Y, 2 * borderRadius, 2 * borderRadius, 180, 90);
            path.AddLine(rect.X + borderRadius, rect.Y, rect.X + rect.Width - borderRadius, rect.Y);
            path.AddArc(rect.X + rect.Width - 2 * borderRadius, rect.Y, 2 * borderRadius, 2 * borderRadius, 270, 90);
            path.AddLine(rect.X + rect.Width, rect.Y + borderRadius, rect.X + rect.Width, rect.Y + rect.Height - borderRadius);
            path.AddArc(rect.X + rect.Width - 2 * borderRadius, rect.Y + rect.Height - 2 * borderRadius, 2 * borderRadius, 2 * borderRadius, 0, 90);
            path.AddLine(rect.X + rect.Width - borderRadius, rect.Y + rect.Height, rect.X + borderRadius, rect.Y + rect.Height);
            path.AddArc(rect.X, rect.Y + rect.Height - 2 * borderRadius, 2 * borderRadius, 2 * borderRadius, 90, 90);
            path.CloseFigure();
        }
        else if (mode == eCornerMode.Left)
        {
            path.AddArc(rect.X, rect.Y, 2 * borderRadius, 2 * borderRadius, 180, 90);
            path.AddLine(rect.X + borderRadius, rect.Y, rect.X + rect.Width, rect.Y);
            path.AddLine(rect.X + rect.Width, rect.Y, rect.X + rect.Width, rect.Y + rect.Height);
            path.AddLine(rect.X + rect.Width, rect.Y + rect.Height, rect.X + borderRadius, rect.Y + rect.Height);
            path.AddArc(rect.X, rect.Y + rect.Height - 2 * borderRadius, 2 * borderRadius, 2 * borderRadius, 90, 90);
            path.CloseFigure();
        }
        else if (mode == eCornerMode.Right)
        {
            path.AddLine(rect.X, rect.Y, rect.X + rect.Width - borderRadius, rect.Y);
            path.AddArc(rect.X + rect.Width - 2 * borderRadius, rect.Y, 2 * borderRadius, 2 * borderRadius, 270, 90);
            path.AddLine(rect.X + rect.Width, rect.Y + borderRadius, rect.X + rect.Width, rect.Y + rect.Height - borderRadius);
            path.AddArc(rect.X + rect.Width - 2 * borderRadius, rect.Y + rect.Height - 2 * borderRadius, 2 * borderRadius, 2 * borderRadius, 0, 90);
            path.AddLine(rect.X + rect.Width - borderRadius, rect.Y + rect.Height, rect.X, rect.Y + rect.Height);
            path.CloseFigure();
        }
        else if (mode == eCornerMode.Top)
        {
            path.AddArc(rect.X, rect.Y, 2 * borderRadius, 2 * borderRadius, 180, 90);
            path.AddLine(rect.X + borderRadius, rect.Y, rect.X + rect.Width - borderRadius, rect.Y);
            path.AddArc(rect.X + rect.Width - 2 * borderRadius, rect.Y, 2 * borderRadius, 2 * borderRadius, 270, 90);
            path.AddLine(rect.X + rect.Width, rect.Y + borderRadius, rect.X + rect.Width, rect.Y + rect.Height - borderRadius);
            path.AddLine(rect.X + rect.Width, rect.Y + rect.Height, rect.X, rect.Y + rect.Height);
            path.CloseFigure();
        }
        else if (mode == eCornerMode.Bottom)
        {
            path.AddLine(rect.X, rect.Y, rect.X + rect.Width, rect.Y);
            path.AddLine(rect.X + rect.Width, rect.Y, rect.X + rect.Width, rect.Y + rect.Height - borderRadius);
            path.AddArc(rect.X + rect.Width - 2 * borderRadius, rect.Y + rect.Height - 2 * borderRadius, 2 * borderRadius, 2 * borderRadius, 0, 90);
            path.AddLine(rect.X + rect.Width - borderRadius, rect.Y + rect.Height, rect.X + borderRadius, rect.Y + rect.Height);
            path.AddArc(rect.X, rect.Y + rect.Height - 2 * borderRadius, 2 * borderRadius, 2 * borderRadius, 90, 90);
            path.CloseFigure();
        }

        else if (mode == eCornerMode.None)
        {
            path.AddLine(rect.X, rect.Y, rect.X + rect.Width, rect.Y);
            path.AddLine(rect.X + rect.Width, rect.Y, rect.X + rect.Width, rect.Y + rect.Height);
            path.AddLine(rect.X + rect.Width, rect.Y + rect.Height, rect.X, rect.Y + rect.Height);
            path.CloseFigure();
        }

        return path;
    }

    private static GraphicsPath GetButtonBorderPath(Rectangle rect, int borderRadius, eButtonStyle style)
    {
        GraphicsPath path = new();
        if (style == eButtonStyle.First)
        {
            path.AddLine(rect.X + rect.Width, rect.Y + rect.Height, rect.X + borderRadius, rect.Y + rect.Height);
            path.AddArc(rect.X, rect.Y + rect.Height - 2 * borderRadius, 2 * borderRadius, 2 * borderRadius, 90, 90);
            path.AddLine(rect.X, rect.Y + rect.Height - borderRadius, rect.X, rect.Y + borderRadius);
            path.AddArc(rect.X, rect.Y, 2 * borderRadius, 2 * borderRadius, 180, 90);
            path.AddLine(rect.X + borderRadius, rect.Y, rect.X + rect.Width, rect.Y);
            path.AddLine(rect.X + rect.Width, rect.Y, rect.X + rect.Width, rect.Y + rect.Height);
        }
        else if (style == eButtonStyle.Middle)
        {
            path.AddLine(rect.X, rect.Y, rect.X + rect.Width, rect.Y);
            path.AddLine(rect.X + rect.Width, rect.Y + rect.Height, rect.X, rect.Y + rect.Height);
        }
        else if (style == eButtonStyle.Last)
        {
            path.AddLine(rect.X, rect.Y, rect.X + rect.Width - borderRadius, rect.Y);
            path.AddArc(rect.X + rect.Width - 2 * borderRadius, rect.Y, 2 * borderRadius, 2 * borderRadius, 270, 90);
            path.AddLine(rect.X + rect.Width, rect.Y + borderRadius, rect.X + rect.Width, rect.Y + rect.Height - borderRadius);
            path.AddArc(rect.X + rect.Width - 2 * borderRadius, rect.Y + rect.Height - 2 * borderRadius, 2 * borderRadius, 2 * borderRadius, 0, 90);
            path.AddLine(rect.X + rect.Width - borderRadius, rect.Y + rect.Height, rect.X, rect.Y + rect.Height);
            path.CloseFigure();
        }

        return path;
    }


    /// <summary>
    /// HTML-formatierten String zeichnen.
    /// </summary>
    /// <param name="cache">Cache in den gezeichnet wird.</param>
    /// <param name="text">zu zeichnender Text</param>
    /// <param name="bounds">Rectangle in dem der Text angezeigt wird.</param>
    /// <param name="forecolor">Textfarbe</param>
    /// <param name="appearance">Darstellung/Appearance</param>
    public static void DrawString(this GraphicsCache cache, string text, Rectangle bounds, Color? forecolor = null, AppearanceObject? appearance = null)
    {
        StringPainter.Default.DrawString(cache, appearance ?? UI.DefaultAppearance, text, bounds);
    }

    /// <summary>
    /// Für das Zeichnen eines HTML-formatierten Strings benötigtes Rectangle berechnen.
    /// </summary>
    /// <param name="cache"></param>
    /// <param name="text"></param>
    /// <param name="maxWidth"></param>
    /// <param name="appearance"></param>
    /// <returns></returns>
    public static Rectangle CalculateStringRect(this GraphicsCache cache, string text, int maxWidth, AppearanceObject? appearance = null)
    {
        appearance ??= UI.DefaultAppearance;

        var ifo = StringPainter.Default.Calculate(cache, appearance, text, maxWidth);

        return ifo.Bounds;
    }


    /// <summary>
    /// Corner mode (e.g. for rounding corners)
    /// </summary>
    public enum eCornerMode
    {
        /// <summary>
        /// all corners
        /// </summary>
        None,
        /// <summary>
        /// all corners
        /// </summary>
        All,
        /// <summary>
        /// left side corners
        /// </summary>
        Left,
        /// <summary>
        /// right side corners
        /// </summary>
        Right,
        /// <summary>
        /// top corners
        /// </summary>
        Top,
        /// <summary>
        /// bottom corners
        /// </summary>
        Bottom
    }

    /// <summary>
    /// Hover mode
    /// </summary>
    public enum eHoverMode
    {
        /// <summary>
        /// not hovered
        /// </summary>
        None,
        /// <summary>
        /// hover, no mouse button pressed
        /// </summary>
        Hover,
        /// <summary>
        /// mouse button is pressed
        /// </summary>
        Pressed
    }

    /// <summary>
    /// button style
    /// </summary>
    public enum eButtonStyle
    {
        /// <summary>
        /// first in a row (left corners rounded)
        /// </summary>
        First,
        /// <summary>
        /// middle in a row (no corners rounded)
        /// </summary>
        Middle,
        /// <summary>
        /// last in a row (right corners rounded)
        /// </summary>
        Last,
        /// <summary>
        /// standalone (all corners rounded)
        /// </summary>
        Standalone
    }
}

/// <summary>
/// image blur type
/// </summary>
public enum BlurType
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    Mean3x3,
    Mean5x5,
    Mean7x7,
    Mean9x9,
    Mean18x18,
    GaussianBlur3x3,
    GaussianBlur5x5,
    MotionBlur5x5,
    MotionBlur5x5At45Degrees,
    MotionBlur5x5At135Degrees,
    MotionBlur7x7,
    MotionBlur7x7At45Degrees,
    MotionBlur7x7At135Degrees,
    MotionBlur9x9,
    MotionBlur9x9At45Degrees,
    MotionBlur9x9At135Degrees,
    Median3x3,
    Median5x5,
    Median7x7,
    Median9x9,
    Median11x11
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}

/// <summary>
/// extension methods for bitmaps
/// </summary>
[SupportedOSPlatform("windows")]
public static class BitmapEx
{
    /// <summary>
    /// assign default blur effect to the image (BlurType.Mean9x9)
    /// </summary>
    /// <param name="bmp"></param>
    /// <returns></returns>
    public static Bitmap Blur(this Bitmap bmp)
    {
        Bitmap result = new Bitmap(bmp.Width, bmp.Height);
        using Graphics g = Graphics.FromImage(result);
        g.DrawImageUnscaled(bmp.ImageBlurFilter(BlurType.GaussianBlur5x5), 0, 0);
        using SolidBrush brush = new(Color.FromArgb(160, UI.TranslateSystemToSkinColor(SystemColors.Window))); 
        g.FillRectangle(brush, new Rectangle(0,0,bmp.Width, bmp.Height));   
        

        return result; 
    }

    /// <summary>
    /// assign advanced blur effect to the image
    /// </summary>
    /// <param name="sourceBitmap">Bitmap</param>
    /// <param name="blurType">Blur effect type</param>
    /// <returns></returns>
    public static Bitmap ImageBlurFilter(this Bitmap sourceBitmap, BlurType blurType)
    {
        Bitmap resultBitmap = blurType switch
        {
            BlurType.Mean3x3 => sourceBitmap.ConvolutionFilter(Matrix.Mean3x3, 1.0 / 9.0),
            BlurType.Mean5x5 => sourceBitmap.ConvolutionFilter(Matrix.Mean5x5, 1.0 / 25.0),
            BlurType.Mean7x7 => sourceBitmap.ConvolutionFilter(Matrix.Mean7x7, 1.0 / 49.0),
            BlurType.Mean9x9 => sourceBitmap.ConvolutionFilter(Matrix.Mean9x9, 1.0 / 81.0),
            BlurType.Mean18x18 => sourceBitmap.ConvolutionFilter(Matrix.Mean9x9, 1.0 / 81.0),
            BlurType.GaussianBlur3x3 => sourceBitmap.ConvolutionFilter(Matrix.GaussianBlur3x3, 1.0 / 16.0),
            BlurType.GaussianBlur5x5 => sourceBitmap.ConvolutionFilter(Matrix.GaussianBlur5x5, 1.0 / 159.0),
            BlurType.MotionBlur5x5 => sourceBitmap.ConvolutionFilter(Matrix.MotionBlur5x5, 1.0 / 10.0),
            BlurType.MotionBlur5x5At45Degrees => sourceBitmap.ConvolutionFilter(Matrix.MotionBlur5x5At45Degrees, 1.0 / 5.0),
            BlurType.MotionBlur5x5At135Degrees => sourceBitmap.ConvolutionFilter(Matrix.MotionBlur5x5At135Degrees, 1.0 / 5.0),
            BlurType.MotionBlur7x7 => sourceBitmap.ConvolutionFilter(Matrix.MotionBlur7x7, 1.0 / 14.0),
            BlurType.MotionBlur7x7At45Degrees => sourceBitmap.ConvolutionFilter(Matrix.MotionBlur7x7At45Degrees, 1.0 / 7.0),
            BlurType.MotionBlur7x7At135Degrees => sourceBitmap.ConvolutionFilter(Matrix.MotionBlur7x7At135Degrees, 1.0 / 7.0),
            BlurType.MotionBlur9x9 => sourceBitmap.ConvolutionFilter(Matrix.MotionBlur9x9, 1.0 / 18.0),
            BlurType.MotionBlur9x9At45Degrees => sourceBitmap.ConvolutionFilter(Matrix.MotionBlur9x9At45Degrees, 1.0 / 9.0),
            BlurType.MotionBlur9x9At135Degrees => sourceBitmap.ConvolutionFilter(Matrix.MotionBlur9x9At135Degrees, 1.0 / 9.0),
            BlurType.Median3x3 => sourceBitmap.MedianFilter(3),
            BlurType.Median5x5 => sourceBitmap.MedianFilter(5),
            BlurType.Median7x7 => sourceBitmap.MedianFilter(7),
            BlurType.Median9x9 => sourceBitmap.MedianFilter(9),
            BlurType.Median11x11 => sourceBitmap.MedianFilter(11),
            _ => sourceBitmap
        };

        return resultBitmap;
    }



    private static class Matrix 
    {  
         public static double[,] Mean3x3 
         {  
             get 
             {  
                 return new double[,]   
                { {  1, 1, 1, },  
                  {  1, 1, 1, },  
                  {  1, 1, 1, }, }; 
             }  
         }  

  
         public static double[,] Mean5x5 
         {  
             get 
             {  
                 return new double[,]   
                { {  1, 1, 1, 1, 1 },  
                  {  1, 1, 1, 1, 1 }, 
                  {  1, 1, 1, 1, 1 }, 
                  {  1, 1, 1, 1, 1 }, 
                  {  1, 1, 1, 1, 1 }, }; 
            }  
        }  

    
        public static double[,] Mean7x7 
        {  
             get 
             {  
                return new double[,]   
                { {  1, 1, 1, 1, 1, 1, 1 },  
                  {  1, 1, 1, 1, 1, 1, 1 }, 
                  {  1, 1, 1, 1, 1, 1, 1 }, 
                  {  1, 1, 1, 1, 1, 1, 1 }, 
                  {  1, 1, 1, 1, 1, 1, 1 }, 
                  {  1, 1, 1, 1, 1, 1, 1 }, 
                  {  1, 1, 1, 1, 1, 1, 1 }, }; 
             }  
        }  

    
         public static double[,] Mean9x9 
         {  
             get 
             {  
                return new double[,]   
                { 
                    {  1, 1, 1, 1, 1, 1, 1, 1, 1 },  
                    {  1, 1, 1, 1, 1, 1, 1, 1, 1 }, 
                    {  1, 1, 1, 1, 1, 1, 1, 1, 1 }, 
                    {  1, 1, 1, 1, 1, 1, 1, 1, 1 }, 
                    {  1, 1, 1, 1, 1, 1, 1, 1, 1 }, 
                    {  1, 1, 1, 1, 1, 1, 1, 1, 1 }, 
                    {  1, 1, 1, 1, 1, 1, 1, 1, 1 }, 
                    {  1, 1, 1, 1, 1, 1, 1, 1, 1 }, 
                    {  1, 1, 1, 1, 1, 1, 1, 1, 1 },
                }; 
            }  
         }  

         public static double[,] Mean18x18 
         {  
             get 
             {  
                 return new double[,]   
                 { 
                     {  1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },  
                     {  1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, 
                     {  1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, 
                     {  1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, 
                     {  1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, 
                     {  1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, 
                     {  1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, 
                     {  1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, 
                     {  1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, 
                     {  1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, 
                     {  1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, 
                     {  1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, 
                     {  1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, 
                     {  1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, 
                     {  1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, 
                     {  1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, 
                     {  1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, 
                     {  1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, 
                 }; 
             }  
         }  

    
        public static double[,] GaussianBlur3x3 
        {  
            get 
            {  
                return new double[,]   
                { {  1, 2, 1, },  
                  {  2, 4, 2, },  
                  {  1, 2, 1, }, }; 
            }  
        }  

    
        public static double[,] GaussianBlur5x5 
        {  
            get 
            {  
                return new double[,]   
                { {  2, 04, 05, 04, 2 },  
                  {  4, 09, 12, 09, 4 },  
                  {  5, 12, 15, 12, 5 }, 
                  {  4, 09, 12, 09, 4 }, 
                  {  2, 04, 05, 04, 2 }, }; 
            }  
        }  

    
        public static double[,] MotionBlur5x5 
        {  
             get 
            {  
                return new double[,]   
                { {  1, 0, 0, 0, 1 },  
                  {  0, 1, 0, 1, 0 },  
                  {  0, 0, 1, 0, 0 }, 
                  {  0, 1, 0, 1, 0 }, 
                  {  1, 0, 0, 0, 1 }, }; 
            }  
        }  

    
        public static double[,] MotionBlur5x5At45Degrees 
        {  
             get 
            {  
                return new double[,]   
                { {  0, 0, 0, 0, 1 },  
                  {  0, 0, 0, 1, 0 },  
                  {  0, 0, 1, 0, 0 }, 
                  {  0, 1, 0, 0, 0 }, 
                  {  1, 0, 0, 0, 0 }, }; 
            }  
        }  

    
        public static double[,] MotionBlur5x5At135Degrees 
        {  
             get 
            {  
                return new double[,]   
                { {  1, 0, 0, 0, 0 },  
                  {  0, 1, 0, 0, 0 },  
                  {  0, 0, 1, 0, 0 }, 
                  {  0, 0, 0, 1, 0 }, 
                  {  0, 0, 0, 0, 1 }, }; 
            }  
        }  

    
        public static double[,] MotionBlur7x7 
        {  
            get 
            {  
                return new double[,]   
                { {  1, 0, 0, 0, 0, 0, 1 },  
                  {  0, 1, 0, 0, 0, 1, 0 },  
                  {  0, 0, 1, 0, 1, 0, 0 }, 
                  {  0, 0, 0, 1, 0, 0, 0 }, 
                  {  0, 0, 1, 0, 1, 0, 0 }, 
                  {  0, 1, 0, 0, 0, 1, 0 }, 
                  {  1, 0, 0, 0, 0, 0, 1 }, }; 
            }  
        }  

    
        public static double[,] MotionBlur7x7At45Degrees 
        {  
            get 
            {  
                return new double[,]   
                { {  0, 0, 0, 0, 0, 0, 1 },  
                  {  0, 0, 0, 0, 0, 1, 0 },  
                  {  0, 0, 0, 0, 1, 0, 0 }, 
                  {  0, 0, 0, 1, 0, 0, 0 }, 
                  {  0, 0, 1, 0, 0, 0, 0 }, 
                  {  0, 1, 0, 0, 0, 0, 0 }, 
                  {  1, 0, 0, 0, 0, 0, 0 }, }; 
            }  
        }  

    
        public static double[,] MotionBlur7x7At135Degrees 
        {  
            get 
            {  
                return new double[,]   
                { {  1, 0, 0, 0, 0, 0, 0 },  
                  {  0, 1, 0, 0, 0, 0, 0 },  
                  {  0, 0, 1, 0, 0, 0, 0 }, 
                  {  0, 0, 0, 1, 0, 0, 0 }, 
                  {  0, 0, 0, 0, 1, 0, 0 }, 
                  {  0, 0, 0, 0, 0, 1, 0 }, 
                  {  0, 0, 0, 0, 0, 0, 1 }, }; 
            }  
        }  

    
        public static double[,] MotionBlur9x9 
        {  
            get 
            {  
                return new double[,]   
                { { 1, 0, 0, 0, 0, 0, 0, 0, 1, }, 
                  { 0, 1, 0, 0, 0, 0, 0, 1, 0, }, 
                  { 0, 0, 1, 0, 0, 0, 1, 0, 0, }, 
                  { 0, 0, 0, 1, 0, 1, 0, 0, 0, }, 
                  { 0, 0, 0, 0, 1, 0, 0, 0, 0, }, 
                  { 0, 0, 0, 1, 0, 1, 0, 0, 0, }, 
                  { 0, 0, 1, 0, 0, 0, 1, 0, 0, }, 
                  { 0, 1, 0, 0, 0, 0, 0, 1, 0, }, 
                  { 1, 0, 0, 0, 0, 0, 0, 0, 1, }, }; 
            }  
        }  

    
        public static double[,] MotionBlur9x9At45Degrees 
        {  
             get 
            {  
                return new double[,]   
                { { 0, 0, 0, 0, 0, 0, 0, 0, 1, }, 
                  { 0, 0, 0, 0, 0, 0, 0, 1, 0, }, 
                  { 0, 0, 0, 0, 0, 0, 1, 0, 0, }, 
                  { 0, 0, 0, 0, 0, 1, 0, 0, 0, }, 
                  { 0, 0, 0, 0, 1, 0, 0, 0, 0, }, 
                  { 0, 0, 0, 1, 0, 0, 0, 0, 0, }, 
                  { 0, 0, 1, 0, 0, 0, 0, 0, 0, }, 
                  { 0, 1, 0, 0, 0, 0, 0, 0, 0, }, 
                  { 1, 0, 0, 0, 0, 0, 0, 0, 0, }, }; 
            }  
        }  

    
        public static double[,] MotionBlur9x9At135Degrees 
        {  
             get 
            {  
                return new double[,]   
                { { 1, 0, 0, 0, 0, 0, 0, 0, 0, }, 
                  { 0, 1, 0, 0, 0, 0, 0, 0, 0, }, 
                  { 0, 0, 1, 0, 0, 0, 0, 0, 0, }, 
                  { 0, 0, 0, 1, 0, 0, 0, 0, 0, }, 
                  { 0, 0, 0, 0, 1, 0, 0, 0, 0, }, 
                  { 0, 0, 0, 0, 0, 1, 0, 0, 0, }, 
                  { 0, 0, 0, 0, 0, 0, 1, 0, 0, }, 
                  { 0, 0, 0, 0, 0, 0, 0, 1, 0, }, 
                  { 0, 0, 0, 0, 0, 0, 0, 0, 1, }, }; 
            }  
        }  
    }

    private static Bitmap ConvolutionFilter(this Bitmap sourceBitmap, double[,] filterMatrix, double factor = 1, int bias = 0)
    {
        BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0,
                sourceBitmap.Width, sourceBitmap.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb);


        byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
        byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height];


        Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
        sourceBitmap.UnlockBits(sourceData);


        int filterWidth = filterMatrix.GetLength(1);
        int filterOffset = (filterWidth - 1) / 2;


        for (int offsetY = filterOffset;
             offsetY <
             sourceBitmap.Height - filterOffset;
             offsetY++)
        {
            for (int offsetX = filterOffset;
                 offsetX <
                 sourceBitmap.Width - filterOffset;
                 offsetX++)
            {
                double blue = 0;
                double green = 0;
                double red = 0;


                var byteOffset = offsetY *
                                 sourceData.Stride +
                                 offsetX * 4;


                for (int filterY = -filterOffset;
                     filterY <= filterOffset;
                     filterY++)
                {
                    for (int filterX = -filterOffset;
                         filterX <= filterOffset;
                         filterX++)
                    {


                        var calcOffset = byteOffset +
                                         (filterX * 4) +
                                         (filterY * sourceData.Stride);


                        blue += pixelBuffer[calcOffset] *
                                filterMatrix[filterY + filterOffset,
                                    filterX + filterOffset];


                        green += pixelBuffer[calcOffset + 1] *
                                 filterMatrix[filterY + filterOffset,
                                     filterX + filterOffset];


                        red += pixelBuffer[calcOffset + 2] *
                               filterMatrix[filterY + filterOffset,
                                   filterX + filterOffset];
                    }
                }


                blue = factor * blue + bias;
                green = factor * green + bias;
                red = factor * red + bias;


                blue = (blue > 255 ? 255 : (blue < 0 ? 0 : blue));


                green = (green > 255 ? 255 : (green < 0 ? 0 : green));


                red = (red > 255 ? 255 : (red < 0 ? 0 : red));


                resultBuffer[byteOffset] = (byte)(blue);
                resultBuffer[byteOffset + 1] = (byte)(green);
                resultBuffer[byteOffset + 2] = (byte)(red);
                resultBuffer[byteOffset + 3] = 255;
            }
        }


        Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);


        BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                resultBitmap.Width, resultBitmap.Height),
            ImageLockMode.WriteOnly,
            PixelFormat.Format32bppArgb);


        Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
        resultBitmap.UnlockBits(resultData);


        return resultBitmap;
    }

    private static Bitmap MedianFilter(this Bitmap sourceBitmap, int matrixSize)
    {
        BitmapData sourceData =
            sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), 
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);


        byte[] pixelBuffer = new byte[sourceData.Stride *
                                      sourceData.Height];


        byte[] resultBuffer = new byte[sourceData.Stride *
                                       sourceData.Height];


        Marshal.Copy(sourceData.Scan0, pixelBuffer, 0,
            pixelBuffer.Length);


        sourceBitmap.UnlockBits(sourceData);


        int filterOffset = (matrixSize - 1) / 2;


        List<int> neighbourPixels = [];


        for (int offsetY = filterOffset;
             offsetY <
             sourceBitmap.Height - filterOffset;
             offsetY++)
        {
            for (int offsetX = filterOffset;
                 offsetX <
                 sourceBitmap.Width - filterOffset;
                 offsetX++)
            {
                var byteOffset = offsetY *
                                 sourceData.Stride +
                                 offsetX * 4;


                neighbourPixels.Clear();


                for (int filterY = -filterOffset;
                     filterY <= filterOffset;
                     filterY++)
                {
                    for (int filterX = -filterOffset;
                         filterX <= filterOffset;
                         filterX++)
                    {
                        var calcOffset = byteOffset +
                                         (filterX * 4) +
                                         (filterY * sourceData.Stride);


                        neighbourPixels.Add(BitConverter.ToInt32(
                            pixelBuffer, calcOffset));
                    }
                }


                neighbourPixels.Sort();

                var middlePixel = BitConverter.GetBytes(
                    neighbourPixels[filterOffset]);


                resultBuffer[byteOffset] = middlePixel[0];
                resultBuffer[byteOffset + 1] = middlePixel[1];
                resultBuffer[byteOffset + 2] = middlePixel[2];
                resultBuffer[byteOffset + 3] = middlePixel[3];
            }
        }


        Bitmap resultBitmap = new Bitmap(sourceBitmap.Width,
            sourceBitmap.Height);


        BitmapData resultData =
            resultBitmap.LockBits(new Rectangle(0, 0,
                    resultBitmap.Width, resultBitmap.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);


        Marshal.Copy(resultBuffer, 0, resultData.Scan0,
            resultBuffer.Length);


        resultBitmap.UnlockBits(resultData);


        return resultBitmap;
    }
}

