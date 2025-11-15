using System.Drawing;
using System.Drawing.Drawing2D;

namespace AF.CORE;

/// <summary>
/// Erweiterungsmethoden für System.Drawing.Point
/// </summary>
public static class PointX
{
    /// <summary>
    /// Prüft, ob ein Punkt sich auf einer eine Linie zwischen zwei Punkten befindet.
    /// </summary>
    /// <param name="point">Der Punkt, der geprüft wird.</param>
    /// <param name="lineStart">Startpunkt der Linie.</param>
    /// <param name="lineEnd">Endpunkt der Linie.</param>
    /// <param name="margin">Der Margin um die Linie, innerhalb dessen der Punkt noch auf der Linie erkannt wird.</param>
    /// <returns>True, wenn der Punkt auf der Linie bzw. innerhalb der Margins liegt.</returns>
    public static bool IsPointOnLine(this Point point, Point lineStart, Point lineEnd, int margin = 3)
    {
        // Berechne die Distanz des Klickpunkts zur Linie
        float distance = DistanceFromPointToLine(point, lineStart, lineEnd);

        // Prüfe, ob die Distanz innerhalb des Margins liegt
        return distance <= margin;
    }

    /// <summary>
    /// Berechnet die Distanz eines Punktes zu einer Linie.
    /// </summary>
    /// <param name="point">Der Punkt.</param>
    /// <param name="lineStart">Startpunkt der Linie.</param>
    /// <param name="lineEnd">Endpunkt der Linie.</param>
    /// <returns>Die Distanz des Punktes zur Linie.</returns>
    public static float DistanceFromPointToLine(this Point point, Point lineStart, Point lineEnd)
    {
        float a = point.X - lineStart.X;
        float b = point.Y - lineStart.Y;
        float c = lineEnd.X - lineStart.X;
        float d = lineEnd.Y - lineStart.Y;

        float dot = a * c + b * d;
        float len_sq = c * c + d * d;
        float param = dot / len_sq;

        float xx, yy;

        if (param < 0 || (lineStart.X == lineEnd.X && lineStart.Y == lineEnd.Y))
        {
            xx = lineStart.X;
            yy = lineStart.Y;
        }
        else if (param > 1)
        {
            xx = lineEnd.X;
            yy = lineEnd.Y;
        }
        else
        {
            xx = lineStart.X + param * c;
            yy = lineStart.Y + param * d;
        }

        float dx = point.X - xx;
        float dy = point.Y - yy;
        return (float)Math.Sqrt(dx * dx + dy * dy);
    }
}

/// <summary>
/// Erweiterungsmethoden für System.Drawing.Graphics
/// </summary>
public static class GraphicsEx
{
    /// <summary>
    /// Gibt die Höhe eines Textes bei vorgegebener Breite zurück
    /// </summary>
    /// <param name="graphics">Zeichenfläche</param>
    /// <param name="text">der Text dessen Höhe ermittelt werden soll</param>
    /// <param name="font">der Font mit dem der Text gezeichnet werden soll</param>
    /// <param name="width">die Breite des Textes</param>
    /// <returns>Höhe des Textes</returns>
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
    /// Zeichnet ein Rechteck mit abgerundeten Ecken
    /// </summary>
    /// <param name="graphics">Zeichnen an der Oberfläche</param>
    /// <param name="rect">Rechteck zum Füllen</param>
    /// <param name="fillcolor">Füllfarbe</param>
    /// <param name="borderwidth">Randbreite mit (0 = kein Rand)</param>
    /// <param name="bordercolor">Rahmenfarbe</param>
    /// <param name="borderRadius">Rahmenradius (Standard = 8)</param>
    public static void DrawRoundedRect(this Graphics graphics, Rectangle rect, Color fillcolor, float borderwidth, Color bordercolor, int borderRadius = 8) 
    {
        var rectangleOrigin = new Point(rect.X, rect.Y);
        var rectangleSize = new Size(rect.Width - 1, rect.Height - 1);

        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Create a rounded rectangle path based on the specified properties
        GraphicsPath path = new();
        path.AddArc(rectangleOrigin.X, rectangleOrigin.Y, 2 * borderRadius, 2 * borderRadius, 180, 90);
        path.AddLine(rectangleOrigin.X + borderRadius, rectangleOrigin.Y, rectangleOrigin.X + rectangleSize.Width - borderRadius, rectangleOrigin.Y);
        path.AddArc(rectangleOrigin.X + rectangleSize.Width - 2 * borderRadius, rectangleOrigin.Y, 2 * borderRadius, 2 * borderRadius, 270, 90);
        path.AddLine(rectangleOrigin.X + rectangleSize.Width, rectangleOrigin.Y + borderRadius, rectangleOrigin.X + rectangleSize.Width, rectangleOrigin.Y + rectangleSize.Height - borderRadius);
        path.AddArc(rectangleOrigin.X + rectangleSize.Width - 2 * borderRadius, rectangleOrigin.Y + rectangleSize.Height - 2 * borderRadius, 2 * borderRadius, 2 * borderRadius, 0, 90);
        path.AddLine(rectangleOrigin.X + rectangleSize.Width - borderRadius, rectangleOrigin.Y + rectangleSize.Height, rectangleOrigin.X + borderRadius, rectangleOrigin.Y + rectangleSize.Height);
        path.AddArc(rectangleOrigin.X, rectangleOrigin.Y + rectangleSize.Height - 2 * borderRadius, 2 * borderRadius, 2 * borderRadius, 90, 90);
        path.CloseFigure();

        // Fill the rounded rectangle with a white color
        using SolidBrush brush = new(fillcolor);
        graphics.FillPath(brush, path);

        if (borderwidth <= 0) return;

        using var pen = new Pen(bordercolor, borderwidth);
        graphics.DrawPath(pen, path);

    }

}
