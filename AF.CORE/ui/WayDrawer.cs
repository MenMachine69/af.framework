using System.Drawing;
using System.Drawing.Drawing2D;

namespace AF.CORE;

/// <summary>
/// Zeichnen eines Pfades
/// </summary>
public static class WayDrawer
{
    /// <summary>
    ///     Erstellt einen GraphicsPath basierend auf einer Sammlung von Linien, wobei abgerundete Ecken und Sprunglinien an
    ///     Überschneidungen unterstützt werden.
    /// </summary>
    /// <param name="lines">Eine Auflistung von Punkt-Arrays, die die Linien darstellen.</param>
    /// <param name="cornerradius">Der Radius, um Ecken abzurunden (Standardwert: 10).</param>
    /// <param name="lineHopSize">Die Größe von Sprunglinien (Standardwert: 10).</param>
    /// <returns>Ein GraphicsPath-Objekt, das den resultierenden Pfad der Linien enthält.</returns>
    public static GraphicsPath GetPath(IEnumerable<PointF[]> lines, float cornerradius = 10, float lineHopSize = 10)
    {
        if (lines == null)
            throw new ArgumentNullException(nameof(lines), "Das Linien-Array darf nicht null sein.");

        PointF[][] linesArray = lines as PointF[][] ?? lines.ToArray(); // Konvertiere die Linien in ein Array von PointF-Arrays, falls es noch keine solche Struktur ist.

        if (linesArray.Length == 0)
            throw new ArgumentException("Das Linien-Array muss mindestens eine Linie enthalten.", nameof(lines));

        if (cornerradius < 1)
            cornerradius = 1;
        if (lineHopSize < 0)
            lineHopSize = 0;

        GraphicsPath fullPath = new();                                              // Erstelle ein neues GraphicsPath-Objekt für den gesamten Pfad.
        PastLineCollection[] pastLines = new PastLineCollection[linesArray.Length]; // Erstelle ein Array, um Informationen über bereits gezeichnete Linien zu speichern.
        using GraphicsPath path = new();
        for (int i = 0, k = 0; i < linesArray.Length; i++) // Iteriere durch jede Linie (jede Linie besteht aus einem Array von Punkten).
        {
            PointF[] points = linesArray[i];
            if (points.Length < 2) // Falls die Linie weniger als zwei Punkte enthält, überspringe sie.
                continue;

            path.Reset();
            pastLines[k] = DrawLine(path, points, cornerradius, lineHopSize, pastLines, k); // Speichere die erstellten Eckpunkte und Linieninformationen in pastPoints[k].
            fullPath.AddPath(path, false);                                                  // Füge den temporären Pfad der aktuellen Linie dem Gesamtpfad hinzu.
            k++;                                                                            // Erhöhe den Index k, um die nächste Linie zu verarbeiten.
        }

        return fullPath; // Gib den vollständigen Pfad aller Linien zurück.
    }

    /// <summary>
    ///     Erstellt einen GraphicsPath basierend auf einem Array von Punkten, wobei abgerundete Ecken und Sprunglinien an
    ///     Überschneidungen unterstützt werden.
    /// </summary>
    /// <param name="points">Ein Array von Punkten (PointF), das die Linie darstellt.</param>
    /// <param name="cornerradius">Der Radius, um Ecken abzurunden (Standardwert: 10).</param>
    /// <param name="lineHopSize">Die Größe von Sprunglinien (Standardwert: 10).</param>
    /// <returns>Ein GraphicsPath-Objekt, das den resultierenden Pfad der Linie enthält.</returns>
    public static GraphicsPath GetPath(PointF[] points, float cornerradius = 10, float lineHopSize = 10)
    {
        if (points == null)
            throw new ArgumentNullException(nameof(points), "Das Punkt-Array darf nicht null sein.");
        if (points.Length < 2)
            throw new ArgumentException("Das Punkt-Array muss mindestens zwei Punkte enthalten, um eine Linie zu bilden.", nameof(points));
        if (cornerradius < 1)
            cornerradius = 1;
        if (lineHopSize < 0)
            lineHopSize = 0;

        GraphicsPath path = new();                                // Erstelle ein neues GraphicsPath-Objekt für den Pfad.
        DrawLine(path, points, cornerradius, lineHopSize, [], 0); // Zeichne die Linie mit abgerundeten Ecken und eventuell Sprunglinien.
        return path;                                              // Gib den Pfad der gezeichneten Linie zurück.
    }

    /// <summary>
    ///     Zeichnet eine Sammlung von Linien auf ein Graphics-Objekt, wobei abgerundete Ecken und Sprunglinien an
    ///     Überschneidungen unterstützt werden.
    /// </summary>
    /// <param name="graphics">Das Graphics-Objekt, auf dem die Linien gezeichnet werden.</param>
    /// <param name="pen">Der Stift, der zum Zeichnen der Linien verwendet wird.</param>
    /// <param name="lines">Eine Auflistung von Punkt-Arrays, die die Linien darstellen.</param>
    /// <param name="cornerradius">Der Radius, um Ecken abzurunden (Standardwert: 10).</param>
    /// <param name="lineHopSize">Die Größe von Sprunglinien (Standardwert: 10).</param>
    public static void DrawPath(Graphics graphics, Pen pen, IEnumerable<PointF[]> lines, float cornerradius = 10, float lineHopSize = 10)
    {
        if (graphics == null)
            throw new ArgumentNullException(nameof(graphics), "Das Graphics-Objekt darf nicht null sein.");
        if (pen == null)
            throw new ArgumentNullException(nameof(pen), "Der Stift darf nicht null sein.");
        if (lines == null)
            throw new ArgumentNullException(nameof(lines), "Das Linien-Array darf nicht null sein.");

        PointF[][] linesArray = lines as PointF[][] ?? lines.ToArray(); // Konvertiere die Linien in ein Array von PointF-Arrays, falls es noch keine solche Struktur ist.

        if (linesArray.Length == 0)
            throw new ArgumentException("Das Linien-Array muss mindestens eine Linie enthalten.", nameof(lines));

        if (cornerradius < 1)
            cornerradius = 1;
        if (lineHopSize < 0)
            lineHopSize = 0;

        PastLineCollection[] pastLines = new PastLineCollection[linesArray.Length]; // Erstelle ein Array, um Informationen über bereits gezeichnete Linien zu speichern.
        using GraphicsPath path = new();
        for (int i = 0, k = 0; i < linesArray.Length; i++) // Iteriere durch jede Linie (jede Linie besteht aus einem Array von Punkten).
        {
            PointF[] points = linesArray[i];
            if (points.Length < 2) // Falls die Linie weniger als zwei Punkte enthält, überspringe sie.
                continue;
            path.Reset();
            pastLines[k] = DrawLine(path, points, cornerradius, lineHopSize, pastLines, k); // Speichere die erstellten Eckpunkte und Linieninformationen in pastPoints[k].
            graphics.DrawPath(pen, path);
            k++; // Erhöhe den Index k, um die nächste Linie zu verarbeiten.
        }
    }

    /// <summary>
    ///     Zeichnet ein Array von Punkten auf ein Graphics-Objekt, wobei abgerundete Ecken und Sprunglinien an
    ///     Überschneidungen unterstützt werden.
    /// </summary>
    /// <param name="graphics">Das Graphics-Objekt, auf dem die Linien gezeichnet werden.</param>
    /// <param name="pen">Der Stift, der zum Zeichnen der Linien verwendet wird.</param>
    /// <param name="points">Ein Array von Punkten (PointF), das die Linie darstellt.</param>
    /// <param name="cornerradius">Der Radius, um Ecken abzurunden (Standardwert: 10).</param>
    /// <param name="lineHopSize">Die Größe von Sprunglinien (Standardwert: 10).</param>
    public static void DrawPath(Graphics graphics, Pen pen, PointF[] points, float cornerradius = 10, float lineHopSize = 10)
    {
        if (graphics == null)
            throw new ArgumentNullException(nameof(graphics), "Das Graphics-Objekt darf nicht null sein.");
        if (pen == null)
            throw new ArgumentNullException(nameof(pen), "Der Stift darf nicht null sein.");
        if (points == null)
            throw new ArgumentNullException(nameof(points), "Das Punkt-Array darf nicht null sein.");
        if (points.Length < 2)
            throw new ArgumentException("Das Punkt-Array muss mindestens zwei Punkte enthalten, um eine Linie zu bilden.", nameof(points));
        if (cornerradius < 1)
            cornerradius = 1;
        if (lineHopSize < 0)
            lineHopSize = 0;
        using GraphicsPath path = new();                          // Erstelle ein temporäres GraphicsPath-Objekt für die aktuelle Linie.
        DrawLine(path, points, cornerradius, lineHopSize, [], 0); // Zeichne die Linie mit abgerundeten Ecken und eventuell Sprunglinien.
        graphics.DrawPath(pen, path);
    }

    /// <summary>
    ///     Zeichnet ein Array von Punkten in einen GraphicsPath, wobei abgerundete Ecken und Sprunglinien an
    ///     Überschneidungen unterstützt werden.
    /// </summary>
    /// <param name="path">Der GraphicsPath, in das die Linien gezeichnet werden.</param>
    /// <param name="points">Ein Array von Punkten (PointF), das die Linie darstellt.</param>
    /// <param name="pastLines">Sammlung von vorherig gezeichneten Linien.</param>
    /// <param name="cornerradius">Der Radius, um Ecken abzurunden (Standardwert: 10).</param>
    /// <param name="lineHopSize">Die Größe von Sprunglinien (Standardwert: 10).</param>
    internal static PastLineCollection DrawLine(GraphicsPath path, PointF[] points, IEnumerable<PastLineCollection> pastLines, float cornerradius = 10, float lineHopSize = 10)
    {
        if (pastLines == null)
            throw new ArgumentNullException(nameof(pastLines), "Die Sammlung der vergangenen Linien darf nicht null sein.");
        return DrawLine(path, points, pastLines as PastLineCollection[] ?? pastLines.ToArray(), cornerradius, lineHopSize);
    }

    /// <summary>
    ///     Zeichnet ein Array von Punkten in einen GraphicsPath, wobei abgerundete Ecken und Sprunglinien an
    ///     Überschneidungen unterstützt werden.
    /// </summary>
    /// <param name="path">Der GraphicsPath, in das die Linien gezeichnet werden.</param>
    /// <param name="points">Ein Array von Punkten (PointF), das die Linie darstellt.</param>
    /// <param name="pastLines">Sammlung von vorherig gezeichneten Linien.</param>
    /// <param name="cornerradius">Der Radius, um Ecken abzurunden (Standardwert: 10).</param>
    /// <param name="lineHopSize">Die Größe von Sprunglinien (Standardwert: 10).</param>
    internal static PastLineCollection DrawLine(GraphicsPath path, PointF[] points, PastLineCollection[] pastLines, float cornerradius = 10, float lineHopSize = 10)
    {
        if (path == null)
            throw new ArgumentNullException(nameof(path), "Das GraphicsPath-Objekt darf nicht null sein.");
        if (points == null)
            throw new ArgumentNullException(nameof(points), "Das Punkt-Array darf nicht null sein.");
        if (points.Length < 2)
            throw new ArgumentException("Das Punkt-Array muss mindestens zwei Punkte enthalten, um eine Linie zu bilden.", nameof(points));
        if (pastLines == null)
            throw new ArgumentNullException(nameof(pastLines), "Die Sammlung der vergangenen Linien darf nicht null sein.");
        if (cornerradius < 1)
            cornerradius = 1;
        if (lineHopSize < 0)
            lineHopSize = 0;
        return DrawLine(path, points, cornerradius, lineHopSize, pastLines, pastLines.Length);
    }

    private static PastLineCollection DrawLine(GraphicsPath path, PointF[] points, float cornerradius, float lineHopSize, PastLineCollection[] pastLines, int pastLinesCount)
    {
        if (points.Length < 2) // Wenn weniger als 2 Punkte vorhanden sind...
        {
            if (points.Length == 0)
                return new PastLineCollection(float.MaxValue, float.MinValue, float.MaxValue, float.MinValue, [], []);
            PointF point = points[0];
            return new PastLineCollection(point.X, point.X, point.Y, point.Y, [], []);
        }

        PointF p1 = points[0]; // Erster Punkt aus dem Array
        PointF p2 = points[1]; // Zweiter Punkt aus dem Array
        path.AddLine(p1, p1);  // Fügt p1 als Startpunkt hinzu

        float minX, minY, maxX, maxY;
        if (p1.X < p2.X)
        {
            minX = p1.X;
            maxX = p2.X;
        }
        else
        {
            minX = p2.X;
            maxX = p1.X;
        }

        if (p1.Y < p2.Y)
        {
            minY = p1.Y;
            maxY = p2.Y;
        }
        else
        {
            minY = p2.Y;
            maxY = p1.Y;
        }

        RectifyCornerradius(p1, p2, ref cornerradius); // Überprüft den Eckradius

        PastLineCollection[] pastLinesArray = pastLines; // Kopiere Array von vorherigen Linien
        if (points.Length > 2)
        {
            // Schleife durch jeden zweiten Punkt, um die Min- und Max-Werte für X und Y zu bestimmen
            for (int i = 2; i < points.Length; i += 2)
            {
                PointF point = points[i];
                if (point.X < minX)
                    minX = point.X;
                if (point.X > maxX)
                    maxX = point.X;
                if (point.Y < minY)
                    minY = point.Y;
                if (point.Y > maxY)
                    maxY = point.Y;
            }

            PointF pL = points[points.Length - 1];
            RectifyCornerradius(points.Length == 3 ? p2 : points[points.Length - 2], pL, ref cornerradius); // Überprüft den Eckradius mit den letzten beiden Punkten

            // Überprüfe die Min- und Max-Werte vom letzten Punkt.
            if (points.Length % 2 == 0)
            {
                if (pL.X < minX)
                    minX = pL.X;
                if (pL.X > maxX)
                    maxX = pL.X;
                if (pL.Y < minY)
                    minY = pL.Y;
                if (pL.Y > maxY)
                    maxY = pL.Y;
            }

            if (pastLinesCount > 0)
            {
                MyList<PastLineCollection> possiblePoints = new(pastLinesCount); // Liste für Linien, die die aktuelle Linie überschneiden könnten
                for (int i = 0; i < pastLinesCount; i++)                         // Überprüft, ob sich die aktuelle Linie mit einer vorherigen Linie überschneiden könnte
                {
                    PastLineCollection prevPoints = pastLines[i];
                    if (prevPoints.MinX < maxX && minX < prevPoints.MaxX && prevPoints.MinY < maxY && minY < prevPoints.MaxY)
                        possiblePoints.Add(prevPoints);
                }

                pastLinesArray = possiblePoints.ToArray(); // Wandelt die Liste in ein Array um
                pastLinesCount = pastLinesArray.Length;    // Aktualisiert die Anzahl der vorherigen Linien
            }
        }

        MyList<PastLine> verticalLines = new(points.Length);   // Liste für vertikale Linien
        MyList<PastLine> horizontalLines = new(points.Length); // Liste für horizontale Linien
        MyList<PointF> lineHops = new(4);                      // Liste für Sprunglinien
        float halfCornerradius = cornerradius / 2F;            // Halber Cornerradius
        float halfLineHopSize = lineHopSize / 2F;              // Halbe Sprunglinie
        RectangleF arcRect1 = RectangleF.Empty;                // Rechteck für die erste unvollständige Kurve
        RectangleF arcRect2 = RectangleF.Empty;                // Rechteck für die zweite unvollständige Kurve
        RectangleF arcRect3 = RectangleF.Empty;                // Rechteck für die dritte unvollständige Kurve
        PointF arcPoint1 = PointF.Empty;                       // Punkt für die erste unvollständige Kurve
        PointF arcPoint2 = PointF.Empty;                       // Punkt für die zweite unvollständige Kurve
        int sweepAngle1 = 90;                                  // Sweepwinkel für die erste unvollständige Kurve
        int sweepAngle2 = 90;                                  // Sweepwinkel für die zweite unvollständige Kurve
        int startAngle1 = -1;                                  // Startwinkel für die erste unvollständige Kurve
        int startAngle2 = -1;                                  // Startwinkel für die zweite unvollständige Kurve

        for (int i = 2; i < points.Length; i++) // Schleife, die alle Punkte von Index 2 bis zum Ende des Arrays durchläuft
        {
            PointF p3 = points[i];            // Nächster Punkt im Array
            PointF linePoint1 = p1;           // Startpunkt
            PointF linePoint2 = p2;           // Eckpunkt
            PointF linePoint3 = p2;           // Endpunkt
            float arcX = p2.X;                // X-Koordinate für die Kurve
            float arcY = p2.Y;                // Y-Koordinate für die Kurve
            int startAngle = 0;               // Startwinkel für die Kurve
            int sweepAngle = 0;               // Sweepwinkel für die Kurve
            bool arcIsFull;                   // Bedingung, um einen vollständigen Bogen zu zeichnen
            if (p1.X.Equals(p2.X) && p2.Y.Equals(p3.Y)) // Linie 1: Vertikal & Linie 2: Horizontal
            {
                arcIsFull = Math.Abs(p1.Y - p2.Y) >= cornerradius && Math.Abs(p2.X - p3.X) >= cornerradius;
                if (p1.Y < p2.Y) // Linie 1: Nach unten
                {
                    linePoint1.Y += halfCornerradius; // Punkt entlang der Linie verschieben
                    linePoint2.Y -= halfCornerradius; // Punkt entlang der Linie verschieben
                    arcY -= cornerradius;             // Bogen nach oben verschieben
                    if (p2.X < p3.X)                  // Linie 2: Nach rechts
                    {
                        linePoint3.X += halfCornerradius; // Linie nach rechts verschieben
                        startAngle = 180;                 // Startwinkel setzen
                        sweepAngle = -90;                 // Sweepwinkel setzen
                    }
                    else if (p2.X > p3.X) // Linie 2: Nach links
                    {
                        linePoint3.X -= halfCornerradius; // Linie nach links verschieben
                        arcX -= cornerradius;             // Bogen nach links verschieben
                        startAngle = 0;                   // Startwinkel setzen
                        sweepAngle = 90;                  // Sweepwinkel setzen
                    }
                }
                else if (p1.Y > p2.Y) // Linie 1: Nach oben
                {
                    linePoint1.Y -= halfCornerradius; // Linie nach oben verschieben
                    linePoint2.Y += halfCornerradius; // Linie nach unten verschieben
                    if (p2.X < p3.X)                  // Linie 2: Nach rechts
                    {
                        linePoint3.X += halfCornerradius; // Linie nach rechts verschieben
                        startAngle = 180;                 // Startwinkel setzen
                        sweepAngle = 90;                  // Sweepwinkel setzen
                    }
                    else if (p2.X > p3.X) // Linie 2: Nach links
                    {
                        linePoint3.X -= halfCornerradius; // Linie nach links verschieben
                        arcX -= cornerradius;             // Bogen nach links verschieben
                        startAngle = 0;                   // Startwinkel setzen
                        sweepAngle = -90;                 // Sweepwinkel setzen
                    }
                }
            }
            else if (p1.Y.Equals(p2.Y) && p2.X.Equals(p3.X)) // Linie 1: Horizontal & Linie 2: Vertikal
            {
                arcIsFull = Math.Abs(p1.X - p2.X) >= cornerradius && Math.Abs(p2.Y - p3.Y) >= cornerradius;
                if (p1.X < p2.X) // Linie 1: Nach rechts
                {
                    linePoint1.X += halfCornerradius; // Linie nach rechts verschieben
                    linePoint2.X -= halfCornerradius; // Linie nach links verschieben
                    arcX -= cornerradius;             // Bogen nach links verschieben
                    if (p2.Y < p3.Y)                  // Linie 2: Nach unten
                    {
                        linePoint3.Y += halfCornerradius; // Linie nach unten verschieben
                        startAngle = 270;                 // Startwinkel setzen
                        sweepAngle = 90;                  // Sweepwinkel setzen
                    }
                    else if (p2.Y > p3.Y) // Linie 2: Nach oben
                    {
                        linePoint3.Y -= halfCornerradius; // Linie nach oben verschieben
                        arcY -= cornerradius;             // Bogen nach oben verschieben
                        startAngle = 90;                  // Startwinkel setzen
                        sweepAngle = -90;                 // Sweepwinkel setzen
                    }
                }
                else if (p1.X > p2.X) // Linie 1: Nach links
                {
                    linePoint1.X -= halfCornerradius; // Linie nach links verschieben
                    linePoint2.X += halfCornerradius; // Linie nach rechts verschieben
                    if (p2.Y < p3.Y)                  // Linie 2: Nach unten
                    {
                        linePoint3.Y += halfCornerradius; // Linie nach unten verschieben
                        startAngle = 270;                 // Startwinkel setzen
                        sweepAngle = -90;                 // Sweepwinkel setzen
                    }
                    else if (p2.Y > p3.Y) // Linie 2: Nach oben
                    {
                        linePoint3.Y -= halfCornerradius; // Linie nach oben verschieben
                        arcY -= cornerradius;             // Bogen nach oben verschieben
                        startAngle = 90;                  // Startwinkel setzen
                        sweepAngle = 90;                  // Sweepwinkel setzen
                    }
                }
            }
            else // Die Linie ist diagonal und damit (für dieses Programm) ungültig.
                continue;

            PointF intersectionPoint1 = i == 2 ? p1 : linePoint1; // Punkt, mit dem auf Schnittpunkte geprüft wird. Wenn es die erste Linie ist, lass die Kurveneinrückung weg. 

            if (arcIsFull)
            {
                if (startAngle2 != -1) // Wenn zwei unvollständige Bögen definiert sind...
                {
                    path.AddArc(arcRect1, startAngle1, sweepAngle1); // Füge den ersten Bogen zum Pfad hinzu
                    path.AddArc(arcRect2, startAngle2, sweepAngle2); // Füge den zweiten Bogen zum Pfad hinzu
                    startAngle1 = startAngle2 = -1;                  // Setze die Winkel auf -1 zurück, um anzuzeigen, dass die Bögen hinzugefügt wurden und keine weiteren Bögen übrig sind.
                }

                // Überprüft auf Schnittpunkte mit vorherigen Linien
                CheckIntersections(path, intersectionPoint1, linePoint2, pastLinesArray, pastLinesCount, lineHopSize, halfLineHopSize, lineHops);

                // Fügt die runde Ecke zum Pfad hinzu
                path.AddArc(arcX, arcY, cornerradius, cornerradius, startAngle, sweepAngle);
            }
            else if (startAngle2 != -1)
            {
                arcRect3.X = arcX;
                arcRect3.Y = arcY;
                arcRect3.Width = cornerradius;
                arcRect3.Height = cornerradius;

                // Überprüft und passt die Dimensionen der Kurvenrechtecke an
                if (arcRect2.Width < cornerradius)
                {
                    float offset = (Math.Abs(Math.Max(arcRect2.Bottom, arcRect3.Bottom) - Math.Min(arcRect2.Y, arcRect3.Y)) - Math.Abs(Math.Min(arcRect2.Bottom, arcRect3.Bottom) - Math.Max(arcRect2.Y, arcRect3.Y))) / 2F;

                    if (arcRect2.Y < arcRect3.Y)
                        arcRect2.Y += offset;
                    else
                        arcRect3.Y += offset;

                    arcRect2.Height -= offset;
                    arcRect3.Height -= offset;
                }
                else
                {
                    float offset = (Math.Abs(Math.Max(arcRect2.Right, arcRect3.Right) - Math.Min(arcRect2.X, arcRect3.X)) - Math.Abs(Math.Min(arcRect2.Right, arcRect3.Right) - Math.Max(arcRect2.X, arcRect3.X))) / 2F;

                    if (arcRect2.X < arcRect3.X)
                        arcRect2.X += offset;
                    else
                        arcRect3.X += offset;

                    arcRect2.Width -= offset;
                    arcRect3.Width -= offset;
                }

                path.AddArc(arcRect1, startAngle1, sweepAngle1); // Füge den ersten Bogen zum Pfad hinzu
                path.AddArc(arcRect2, startAngle2, sweepAngle2); // Füge den zweiten Bogen zum Pfad hinzu
                path.AddArc(arcRect3, startAngle, sweepAngle);   // Füge den dritten Bogen zum Pfad hinzu
                startAngle1 = startAngle2 = -1;                  // Setze die Winkel auf -1 zurück, um anzuzeigen, dass die Bögen hinzugefügt wurden und keine weiteren Bögen übrig sind.
            }
            else if (startAngle1 != -1)
            {
                // Berechnung der Dimensionen der Kurvenrechtecke
                float width, height, x1, x2, y1, y2;
                if (arcPoint1.X > linePoint3.X)
                {
                    width = arcPoint1.X - linePoint3.X;
                    x1 = x2 = linePoint3.X;
                }
                else if (arcPoint1.X < linePoint3.X)
                {
                    width = linePoint3.X - arcPoint1.X;
                    x1 = x2 = arcPoint1.X;
                }
                else
                {
                    width = Math.Abs(arcPoint1.X - arcPoint2.X) * 2;
                    x1 = x2 = linePoint3.X;
                }

                if (arcPoint1.Y > linePoint3.Y)
                {
                    height = arcPoint1.Y - linePoint3.Y;
                    y1 = y2 = linePoint3.Y;
                }
                else if (arcPoint1.Y < linePoint3.Y)
                {
                    height = linePoint3.Y - arcPoint1.Y;
                    y1 = y2 = arcPoint1.Y;
                }
                else
                {
                    height = Math.Abs(arcPoint1.Y - arcPoint2.Y) * 2;
                    y1 = y2 = linePoint3.Y;
                }

                if (arcPoint2.X.Equals(linePoint2.X))
                {
                    if (arcPoint1.X < linePoint3.X)
                    {
                        x1 -= halfCornerradius;
                        x2 += halfCornerradius;
                    }
                    else if (arcPoint1.X > linePoint3.X)
                    {
                        x1 += halfCornerradius;
                        x2 -= halfCornerradius;
                    }
                    else
                    {
                        x1 -= halfCornerradius;
                        x2 -= halfCornerradius;
                    }
                }
                else if (arcPoint1.Y > linePoint3.Y)
                {
                    y1 += halfCornerradius;
                    y2 -= halfCornerradius;
                }
                else if (arcPoint1.Y < linePoint3.Y)
                {
                    y1 -= halfCornerradius;
                    y2 += halfCornerradius;
                }
                else
                {
                    y1 -= halfCornerradius;
                    y2 -= halfCornerradius;
                }

                // Setzt die Rechtecke für die Kurven neu
                arcRect1.X = x1;
                arcRect1.Y = y1;
                arcRect1.Width = width;
                arcRect1.Height = height;
                arcRect2.X = x2;
                arcRect2.Y = y2;
                arcRect2.Width = width;
                arcRect2.Height = height;

                startAngle2 = startAngle;
                sweepAngle2 = sweepAngle;
            }
            else
            {
                startAngle1 = startAngle;
                sweepAngle1 = sweepAngle;
                arcPoint1 = linePoint2;
                arcPoint2 = linePoint3;

                // Überprüft auf Schnittpunkte mit vorherigen Linien
                CheckIntersections(path, intersectionPoint1, linePoint2, pastLinesArray, pastLinesCount, lineHopSize, halfLineHopSize, lineHops);
            }

            // Füge die Linie der jeweiligen Liste hinzu
            // Wenn es die erste Linie ist, lass die Kurveneinrückung weg
            if (p1.X.Equals(p2.X))
                if (linePoint1.Y < linePoint2.Y)
                    verticalLines.Add(new PastLine(intersectionPoint1.Y, linePoint2.Y, p1.X, lineHops));
                else
                    verticalLines.Add(new PastLine(linePoint2.Y, intersectionPoint1.Y, p1.X, lineHops));
            else if (linePoint1.X < linePoint2.X)
                horizontalLines.Add(new PastLine(intersectionPoint1.X, linePoint2.X, p1.Y, lineHops));
            else
                horizontalLines.Add(new PastLine(linePoint2.X, intersectionPoint1.X, p1.Y, lineHops));
            lineHops.Clear();

            // Aktualisiert p1 und p2 für den nächsten Schleifendurchlauf
            p1 = p2;
            p2 = p3;
        }

        if (startAngle2 != -1) // Wenn zwei unvollständige Bögen definiert sind...
        {
            path.AddArc(arcRect1, startAngle1, sweepAngle1); // Fügt den ersten Bogen zum Pfad hinzu
            path.AddArc(arcRect2, startAngle2, sweepAngle2); // Fügt den zweiten Bogen zum Pfad hinzu
        }

        // Überprüft die relative Position von p1 und p2, um Anpassungen bei der Bogenzeichnung vorzunehmen
        if (p1.X.Equals(p2.X)) // Vertikale Linie
        {
            if (p1.Y > p2.Y) // Nach oben
            {
                if (points.Length > 2)        // Wenn es nicht die erste Linie ist
                    p1.Y -= halfCornerradius; // p1 nach oben verschieben
                if (p1.Y > p2.Y)
                {
                    CheckIntersections(path, p1, p2, pastLinesArray, pastLinesCount, lineHopSize, halfLineHopSize, lineHops);
                    verticalLines.Add(new PastLine(p2.Y, p1.Y, p1.X, lineHops));
                }
            }
            else if (p1.Y < p2.Y) // Nach unten
            {
                if (points.Length > 2)        // Wenn es nicht die erste Linie ist
                    p1.Y += halfCornerradius; // p1 nach unten verschieben
                if (p1.Y < p2.Y)
                {
                    CheckIntersections(path, p1, p2, pastLinesArray, pastLinesCount, lineHopSize, halfLineHopSize, lineHops);
                    verticalLines.Add(new PastLine(p1.Y, p2.Y, p1.X, lineHops));
                }
            }
        }
        else if (p1.Y.Equals(p2.Y)) // Horizontale Linie
        {
            if (p1.X > p2.X) // Nach links
            {
                if (points.Length > 2)        // Wenn es nicht die erste Linie ist
                    p1.X -= halfCornerradius; // p1 nach links verschieben
                if (p1.X > p2.X)
                {
                    CheckIntersections(path, p1, p2, pastLinesArray, pastLinesCount, lineHopSize, halfLineHopSize, lineHops);
                    horizontalLines.Add(new PastLine(p2.X, p1.X, p1.Y, lineHops));
                }
            }
            else if (p1.X < p2.X) // Nach rechts
            {
                if (points.Length > 2)        // Wenn es nicht die erste Linie ist
                    p1.X += halfCornerradius; // p1 nach rechts verschieben
                if (p1.X < p2.X)
                {
                    CheckIntersections(path, p1, p2, pastLinesArray, pastLinesCount, lineHopSize, halfLineHopSize, lineHops);
                    horizontalLines.Add(new PastLine(p1.X, p2.X, p1.Y, lineHops));
                }
            }
        }

        path.AddLine(p2, p2); // Fügt p2 als Endpunkt hinzu
        return new PastLineCollection(minX, maxX, minY, maxY, verticalLines.ToArray(), horizontalLines.ToArray());
    }

    private static void RectifyCornerradius(PointF p1, PointF p2, ref float cornerradius)
    {
        // Wenn die erste oder letzte Strecke kleiner als Cornerradius ist, verwende die Strecke als Cornerradius.
        float distance = Math.Abs(p1.X.Equals(p2.X) ? p1.Y - p2.Y : p1.X - p2.X);
        if (cornerradius > distance)
            cornerradius = distance;
    }

    private static void CheckIntersections(GraphicsPath path, PointF p1, PointF p2, PastLineCollection[] pastLines, int pastLinesCount, float lineHopSize, float halfLineHopSize, MyList<PointF> lineHops)
    {
        if (pastLinesCount < 1 || lineHopSize < 1) // Wenn es keine vergangenen Punkte gibt oder die Sprunglinie gleich 0 ist, beende die Methode.
            return;

        if (p1.X.Equals(p2.X)) // Vertikale Linie
        {
            float y1 = p1.Y, y2 = p2.Y, ppMinY, ppMaxY;
            if (y1 > y2) // Nach oben
            {
                y1 -= halfLineHopSize; // Verschiebe y1 nach oben
                y2 += halfLineHopSize; // Verschiebe y2 nach unten
                if (y1 < y2)           // Wenn y1 über y2 liegt, beende die Methode
                    return;
                ppMinY = y2;
                ppMaxY = y1;
            }
            else if (y1 < y2) // Nach unten
            {
                y1 += halfLineHopSize; // Verschiebe y1 nach unten
                y2 -= halfLineHopSize; // Verschiebe y2 nach oben
                if (y1 > y2)           // Wenn y1 unter y2 liegt, beende die Methode
                    return;
                ppMinY = y1;
                ppMaxY = y2;
            }
            else
                return; // Wenn die Linie keine Länge hat, beende die Methode.

            if (Math.Abs(y1 - y2) <= lineHopSize) // Wenn die Linie kürzer als die Sprunglinie ist, beende die Methode.
                return;

            float x = p1.X;
            IntersectionList intersections = new(); // Liste zur Speicherung von Schnittpunkten.
            for (int i = 0; i < pastLinesCount; i++)
            {
                PastLine[] lines = pastLines[i].HorizontalLines;
                for (int j = 0; j < lines.Length; j++)
                {
                    PastLine pastLine = lines[j];
                    // Überprüfen, ob der Schnittpunkt außerhalb beider Liniensegmente liegt.
                    if (x < pastLine.Min || x > pastLine.Max || pastLine.Other < ppMinY || pastLine.Other > ppMaxY)
                        continue;

                    // Prüfe, ob die Schnittstelle bereits durch eine andere dargestellt wird.
                    for (int k = 0; k < pastLine.LineHops.Length; k++)
                    {
                        PointF lineHop = pastLine.LineHops[k];
                        if (lineHop.X < x && x <= lineHop.Y)
                            goto wayPoint;
                    }

                    intersections.Add(pastLine.Other); // Füge Schnittpunkt zur Liste hinzu.
                wayPoint:;
                }
            }

            if (intersections.Count == 0) // Wenn keine Schnittpunkte gefunden wurden, beende die Methode.
                return;
            float gapArcX = x - halfLineHopSize; // X-Koordinate der Sprunglinie.
            intersections.Sort();                // Sortiere die Schnittpunkte aufsteigend.
            if (y1 > y2)                         // Nach oben
            {
                int index = intersections.NegCount;
                while (index > -1) // Iteriere rückwärts über die Schnittpunkte, um Sprunglinien zu erstellen.
                {
                    float intersection = intersections[index];      // Aktueller Schnittpunkt.
                    float end = intersection + halfLineHopSize;     // Endpunkt für die Sprunglinie.
                    float height = lineHopSize;                     // Höhe der Sprunglinie.
                    float gapArcY = intersection - halfLineHopSize; // Y-Koordinate der Sprunglinie.

                    while (--index > -1) // Falls mehrere Schnittpunkte nahe beieinander liegen, werden sie zu einer Sprunglinie zusammengefasst.
                    {
                        float nextIntersection = intersections[index]; // Nächster Schnittpunkt.
                        if (nextIntersection + lineHopSize >= gapArcY)
                        {
                            gapArcY = nextIntersection - halfLineHopSize; // Aktualisiere die Y-Position des Bogens.
                            height = end - gapArcY;                       // Aktualisiere die Höhe.
                        }
                        else
                            break; // Breche die Schleife ab, wenn nicht aktualisiert wurde.
                    }

                    // Füge die Sprunglinie zum Pfad hinzu.
                    path.AddArc(gapArcX, gapArcY, lineHopSize, height, 90, -180);
                    lineHops.Add(new PointF(gapArcY, gapArcY + height));
                }
            }
            else // Nach unten
            {
                int index = 0;
                while (index < intersections.Count) // Iteriere über die Schnittpunkte, um Sprunglinien zu erstellen.
                {
                    float intersection = intersections[index];      // Aktueller Schnittpunkt.
                    float start = intersection + halfLineHopSize;   // Startpunkt für die Sprunglinie.
                    float height = lineHopSize;                     // Höhe der Sprunglinie.
                    float gapArcY = intersection - halfLineHopSize; // Y-Koordinate der Sprunglinie.

                    while (++index < intersections.Count) // Falls mehrere Schnittpunkte nahe beieinander liegen, werden sie zu einer Sprunglinie zusammengefasst.
                    {
                        float nextIntersection = intersections[index]; // Nächster Schnittpunkt.
                        if (nextIntersection - lineHopSize <= start)
                        {
                            start = nextIntersection + halfLineHopSize;
                            height = start - gapArcY; // Aktualisiere die Höhe.
                        }
                        else
                            break; // Breche die Schleife ab, wenn nicht aktualisiert wurde.
                    }

                    // Füge die Sprunglinie zum Pfad hinzu.
                    path.AddArc(gapArcX, gapArcY, lineHopSize, height, 270, 180);
                    lineHops.Add(new PointF(gapArcY, gapArcY + height));
                }
            }
        }
        else if (p1.Y.Equals(p2.Y)) // Horizontale Linie
        {
            float x1 = p1.X, x2 = p2.X, ppMinX, ppMaxX;
            if (x1 > x2) // Nach links
            {
                x1 -= halfLineHopSize; // Verschiebe x1 nach links
                x2 += halfLineHopSize; // Verschiebe x2 nach rechts
                if (x1 < x2)           // Wenn x1 links von x2 liegt, beende die Methode
                    return;
                ppMinX = x2;
                ppMaxX = x1;
            }
            else if (x1 < x2) // Nach rechts
            {
                x1 += halfLineHopSize; // Verschiebe x1 nach rechts
                x2 -= halfLineHopSize; // Verschiebe x2 nach links
                if (x1 > x2)           // Wenn x1 rechts von x2 liegt, beende die Methode
                    return;
                ppMinX = x1;
                ppMaxX = x2;
            }
            else
                return; // Wenn die Linie keine Länge hat, beende die Methode.

            if (Math.Abs(x1 - x2) <= lineHopSize) // Wenn die Linie kürzer als die Sprunglinie ist, beende die Methode.
                return;

            float y = p1.Y;
            IntersectionList intersections = new(); // Liste zur Speicherung von Schnittpunkten.
            for (int i = 0; i < pastLinesCount; i++)
            {
                PastLine[] lines = pastLines[i].VerticalLines;
                for (int j = 0; j < lines.Length; j++)
                {
                    PastLine pastLine = lines[j];

                    // Überprüfen, ob der Schnittpunkt außerhalb beider Liniensegmente liegt.
                    if (y < pastLine.Min || y > pastLine.Max || pastLine.Other < ppMinX || pastLine.Other > ppMaxX)
                        continue;

                    // Prüfe, ob die Schnittstelle bereits durch eine andere dargestellt wird.
                    for (int k = 0; k < pastLine.LineHops.Length; k++)
                    {
                        PointF lineHop = pastLine.LineHops[k];
                        if (lineHop.X < y && y <= lineHop.Y)
                            goto wayPoint;
                    }

                    intersections.Add(pastLine.Other); // Füge Schnittpunkt zur Liste hinzu.
                wayPoint:;
                }
            }

            if (intersections.Count == 0) // Wenn keine Schnittpunkte gefunden wurden, beende die Methode.
                return;

            float gapArcY = y - halfLineHopSize; // Y-Koordinate der Sprunglinie.
            intersections.Sort();                // Sortiere die Schnittpunkte aufsteigend.
            if (x1 > x2)                         // Nach links
            {
                int index = intersections.NegCount;
                while (index > -1) // Iteriere rückwärts über die Schnittpunkte, um Sprunglinien zu erstellen.
                {
                    float intersection = intersections[index];      // Aktueller Schnittpunkt.
                    float end = intersection + halfLineHopSize;     // Endpunkt für die Sprunglinie.
                    float width = lineHopSize;                      // Breite der Sprunglinie.
                    float gapArcX = intersection - halfLineHopSize; // X-Koordinate der Sprunglinie.

                    while (--index > -1) // Falls mehrere Schnittpunkte nahe beieinander liegen, werden sie zu einer Sprunglinie zusammengefasst.
                    {
                        float nextIntersection = intersections[index]; // Nächster Schnittpunkt.
                        if (nextIntersection + lineHopSize >= gapArcX)
                        {
                            gapArcX = nextIntersection - halfLineHopSize; // Aktualisiere die X-Position des Bogens.
                            width = end - gapArcX;                        // Aktualisiere die Breite.
                        }
                        else
                            break; // Breche die Schleife ab, wenn nicht aktualisiert wurde.
                    }

                    // Füge die Sprunglinie zum Pfad hinzu.
                    path.AddArc(gapArcX, gapArcY, width, lineHopSize, 0, -180);
                    lineHops.Add(new PointF(gapArcX, gapArcX + width));
                }
            }
            else // Nach rechts
            {
                int index = 0;
                while (index < intersections.Count) // Iteriere über die Schnittpunkte, um Sprunglinien zu erstellen.
                {
                    float intersection = intersections[index];      // Aktueller Schnittpunkt.
                    float start = intersection + halfLineHopSize;   // Startpunkt für die Sprunglinie.
                    float width = lineHopSize;                      // Breite der Sprunglinie.
                    float gapArcX = intersection - halfLineHopSize; // X-Koordinate der Sprunglinie.

                    while (++index < intersections.Count) // Falls mehrere Schnittpunkte nahe beieinander liegen, werden sie zu einer Sprunglinie zusammengefasst.
                    {
                        float nextIntersection = intersections[index]; // Nächster Schnittpunkt.
                        if (nextIntersection - lineHopSize <= start)
                        {
                            start = nextIntersection + halfLineHopSize;
                            width = start - gapArcX; // Aktualisiere die Breite.
                        }
                        else
                            break; // Breche die Schleife ab, wenn nicht aktualisiert wurde.
                    }

                    // Füge die Sprunglinie zum Pfad hinzu.
                    path.AddArc(gapArcX, gapArcY, width, lineHopSize, 180, 180);
                    lineHops.Add(new PointF(gapArcX, gapArcX + width));
                }
            }
        }
    }
}

internal class PastLineCollection
{
    internal PastLine[] VerticalLines { get; }
    internal PastLine[] HorizontalLines { get; }
    internal float MinX { get; }
    internal float MaxX { get; }
    internal float MinY { get; }
    internal float MaxY { get; }

    internal PastLineCollection(float minX, float maxX, float minY, float maxY, PastLine[] verticalLines, PastLine[] horizontalLines)
    {
        VerticalLines = verticalLines;
        HorizontalLines = horizontalLines;
        MinX = minX;
        MaxX = maxX;
        MinY = minY;
        MaxY = maxY;
    }
}

internal class PastLine(float min, float max, float other, MyList<PointF> lineHops)
{
    internal float Min { get; } = min;
    internal float Max { get; } = max;
    internal float Other { get; } = other;
    internal PointF[] LineHops { get; } = lineHops.ToArray();
}

internal class IntersectionList
{
    internal int Count { get; private set; }
    internal int NegCount { get; private set; } = -1;
    internal float this[int index] => Items[index];
    private float[] Items = new float[4];

    internal void Add(float item)
    {
        if (Count > 0)
        {
            for (int i1 = 0, i2 = NegCount; i1 <= i2; ++i1, --i2)
            {
                if (Items[i1].Equals(item) || Items[i2].Equals(item))
                    return;
            }
        }

        if (Count == Items.Length)
        {
            int num = Items.Length * 2;
            if (num > 2146435071)
                num = 2146435071;
            float[] destinationArray = new float[num];
            Array.Copy(Items, 0, destinationArray, 0, Count);
            Items = destinationArray;
        }

        Items[Count++] = item;
        ++NegCount;
    }

    internal void Sort() => Array.Sort(Items, 0, Count, Comparer<float>.Default);
}

internal class MyList<T>(int capacity)
{
    private int Count;
    private T[] Items = new T[capacity];

    internal void Add(T item)
    {
        if (Count == Items.Length)
        {
            int num = Items.Length * 2;
            if (num > 2146435071)
                num = 2146435071;
            T[] destinationArray = new T[num];
            Array.Copy(Items, 0, destinationArray, 0, Count);
            Items = destinationArray;
        }

        Items[Count++] = item;
    }

    internal void Clear() => Count = 0;

    internal T[] ToArray()
    {
        T[] destinationArray = new T[Count];
        for (int i = 0; i < Count; i++)
            destinationArray[i] = Items[i];
        return destinationArray;
    }
}