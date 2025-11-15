using System.Drawing;

namespace AF.CORE;

/// <summary>
/// Weg zwischen zwei durch eine Linie zu verbindenden Rechtecken finden
/// </summary>
public static class WayFinder
{
    /// <summary>
    ///     Generiert ein Array von Punkten, die eine Verbindung zwischen zwei gegebenen Punkten
    ///     darstellen, wobei umgebende Rechtecke und eine optionale Marge berücksichtigt werden.
    /// </summary>
    /// <param name="startPoint">Der Startpunkt der Linie.</param>
    /// <param name="endPoint">Der Endpunkt der Linie.</param>
    /// <param name="startRect">Ein Rechteck, das den Startpunkt umgibt.</param>
    /// <param name="endRect">Ein Rechteck, das den Endpunkt umgibt.</param>
    /// <param name="margin">
    ///     Ein optionaler Mindestabstand, der zu den Rechtecken eingehalten werden muss. (Standard ist 10).
    ///     Wenn kleiner als 1, wird sie auf 1 gesetzt.
    /// </param>
    /// <returns>
    ///     Ein Array von PointF, das die berechneten Verbindungspunkte darstellt.
    ///     Gibt ein leeres Array zurück, wenn eine der angegebenen Bedingungen die Verbindung ungültig macht.
    /// </returns>
    public static PointF[] GetPoints(PointF startPoint, PointF endPoint, RectangleF startRect, RectangleF endRect, float margin = 10)
    {
        // Sicherstellen, dass der Randabstand mindestens 1 ist
        if (margin < 1)
            margin = 1;

        // Erzeuge erweiterte Rechtecke, die den Randabstand berücksichtigen
        float margin2 = margin * 2;
        RectangleF marginStartRect = new(startRect.X - margin, startRect.Y - margin, startRect.Width + margin2, startRect.Height + margin2);
        RectangleF marginEndRect = new(endRect.X - margin, endRect.Y - margin, endRect.Width + margin2, endRect.Height + margin2);

        // Bestimme den Randpunkt beider Rechtecke
        PointF edgeStartPoint = GetEdgePoint(startPoint, startRect, margin);
        PointF edgeEndPoint = GetEdgePoint(endPoint, endRect, margin);

        // Überprüfe, ob einer der Punkte in den Rechtecken oder den erweiterten Rechtecken liegt
        if (startRect.Contains(endPoint.X, endPoint.Y) ||
            endRect.Contains(startPoint.X, startPoint.Y) ||
            marginStartRect.Contains(endPoint.X, endPoint.Y) ||
            marginEndRect.Contains(startPoint.X, startPoint.Y) ||
            startRect.Contains(edgeEndPoint.X, edgeEndPoint.Y) ||
            endRect.Contains(edgeStartPoint.X, edgeStartPoint.Y))
            return []; // Gebe ein leeres Array zurück

        List<PointF> points = new(8) // Initialisiere die Liste der Punkte mit den Start- und Randpunkten
        {
            startPoint,
            edgeStartPoint
        };
        RectangleF rect1 = marginStartRect; // Erstes Rechteck
        RectangleF rect2 = marginEndRect;   // Zweites Rechteck
        PointF point1 = edgeStartPoint;     // Erster Randpunkt
        PointF point2 = edgeEndPoint;       // Zweiter Randpunkt

        // Bestimme die maximalen Grenzen für die X- und Y-Koordinaten
        float bottom = Math.Max(rect1.Bottom, rect2.Bottom) * 2F;
        float right = Math.Max(rect1.Right, rect2.Right) * 2F;

        // Flags zur Steuerung von Transformationen
        bool reverse = false; // Umkehrung der Punkte
        bool rotate = false;  // Drehung der Punkte
        bool flipX = false;   // Spiegelung entlang der X-Achse
        bool flipY = false;   // Spiegelung entlang der Y-Achse

        // Überprüfe die Position der Start- und Endpunkte in Bezug auf die Rechtecke
        if (startPoint.X.Equals(startRect.Left))
        {
            if (endPoint.Y.Equals(endRect.Top))
                reverse = flipY = true;
            else if (endPoint.Y.Equals(endRect.Bottom))
                reverse = true;
            else
                rotate = true;
        }
        else if (startPoint.Y.Equals(startRect.Top))
        {
            if (endPoint.X.Equals(endRect.Left))
                flipY = true;
            else if (endPoint.X.Equals(endRect.Right))
                flipX = flipY = true;
        }
        else if (startPoint.X.Equals(startRect.Right))
        {
            if (endPoint.Y.Equals(endRect.Top))
                reverse = flipX = flipY = true;
            else if (endPoint.X.Equals(endRect.Left))
                reverse = rotate = true;
            else if (endPoint.Y.Equals(endRect.Bottom))
                reverse = flipX = true;
            else
                rotate = flipX = flipY = true;
        }
        else if (startPoint.Y.Equals(startRect.Bottom))
        {
            if (endPoint.Y.Equals(endRect.Top))
                reverse = true;
            else if (endPoint.Y.Equals(endRect.Bottom))
                flipX = flipY = true;
            else if (endPoint.X.Equals(endRect.Right))
                flipX = true;
        }

        if (reverse) // Wenn die Punkte umgekehrt werden müssen, vertausche die Rechtecke und Punkte
        {
            (rect1, rect2) = (rect2, rect1);
            (point1, point2) = (point2, point1);
        }

        if (rotate) // Wenn die Punkte rotiert werden müssen, rotiere die Punkte und Rechtecke
        {
            (point1.X, point1.Y) = (point1.Y, point1.X);
            (point2.X, point2.Y) = (point2.Y, point2.X);
            (rect1.X, rect1.Y, rect1.Width, rect1.Height) = (rect1.Y, rect1.X, rect1.Height, rect1.Width);
            (rect2.X, rect2.Y, rect2.Width, rect2.Height) = (rect2.Y, rect2.X, rect2.Height, rect2.Width);
        }

        if (flipX) // Wenn die Punkte entlang der X-Achse gespiegelt werden müssen
        {
            rect1.X = right - rect1.Right;
            rect2.X = right - rect2.Right;
            point1.X = right - point1.X;
            point2.X = right - point2.X;
        }

        if (flipY) // Wenn die Punkte entlang der Y-Achse gespiegelt werden müssen
        {
            rect1.Y = bottom - rect1.Bottom;
            rect2.Y = bottom - rect2.Bottom;
            point1.Y = bottom - point1.Y;
            point2.Y = bottom - point2.Y;
        }

        if (!point1.Y.Equals(rect1.Top)) // Punkte um die Ecke, Basis: Unten nach Links
        {
            if (point1.Y <= point2.Y)
            {
                if (point1.X <= point2.X)
                    points.Add(point1.X, point2.Y);
                else if (point1.Y > rect2.Top && point1.X < rect2.Right)
                    points.Add(point2.X, point1.Y);
                else
                {
                    float y = point1.Y > rect2.Top ? rect2.Bottom : rect2.Top;
                    points.Add(point1.X, y);
                    points.Add(point2.X, y);
                }
            }
            else if (point1.X < point2.X)
            {
                if (rect1.Right <= point2.X)
                {
                    points.Add(rect1.Right, point1.Y);
                    points.Add(rect1.Right, point2.Y);
                }
                else
                {
                    points.Add(rect1.Left, point1.Y);
                    points.Add(rect1.Left, rect1.Top);
                    points.Add(point2.X, rect1.Top);
                }
            }
            else if (rect1.Left <= point2.X && point2.X < rect1.Right)
            {
                float y = Math.Max(point1.Y, rect2.Bottom);
                points.Add(point1.X, y);
                points.Add(rect1.Left, y);
                points.Add(rect1.Left, point2.Y);
            }
            else
            {
                float x = point1.Y > rect2.Bottom ? Math.Min(rect1.Left, rect2.Right) : Math.Max(rect1.Left, rect2.Right);
                float y = Math.Max(rect1.Top, rect2.Bottom);
                points.Add(x, point1.Y);
                points.Add(x, y);
                points.Add(point2.X, y);
            }
        }
        else if (point2.Y.Equals(rect2.Top)) // Punkte auf derselben Seite, Basis: Oben zu Oben
        {
            if (point2.Y >= rect1.Bottom)
            {
                float x = point1.X < point2.X ? rect1.Right : rect1.Left;
                points.Add(x, point1.Y);
                points.Add(x, rect1.Bottom);
                points.Add(point2.X, rect1.Bottom);
            }
            else if (rect2.Bottom <= point1.Y)
            {
                float x = point1.X < point2.X ? rect2.Left : rect2.Right;
                points.Add(point1.X, rect2.Bottom);
                points.Add(x, rect2.Bottom);
                points.Add(x, point2.Y);
            }
            else if (point1.Y <= point2.Y)
            {
                if (point1.X >= rect2.Left || (point2.X <= rect1.Right && point2.X > point1.X))
                {
                    points.Add(rect1.Right, point1.Y);
                    points.Add(rect1.Right, point2.Y);
                }
                else
                {
                    points.Add(rect2.Right, point1.Y);
                    points.Add(point2.X, point1.Y);
                }
            }
            else if (point1.X > rect2.Left)
            {
                points.Add(rect2.Left, point1.Y);
                points.Add(rect2.Left, point2.Y);
            }
            else
                points.Add(point1.X, point2.Y);
        }
        else // Punkte auf gegenüberliegenden Seiten, Basis: Oben zu Unten
        {
            if (point1.Y >= point2.Y)
            {
                if (rect1.Right >= rect2.Left && ((point1.X <= point2.X && rect1.Right < point2.X) || (point1.X > point2.X && rect1.Right > point2.X)))
                    points.Add(rect1.Right, point1.Y);
                points.Add(point2.X, point1.Y);
            }
            else if (point1.X < point2.X && rect1.Right >= rect2.Left)
            {
                float y = Math.Max(rect1.Bottom, point2.Y);
                float x1 = rect1.Left > rect2.Left && rect1.Top > rect2.Top ? Math.Max(rect1.Left, rect2.Left) : Math.Min(rect1.Left, rect2.Left);
                float x2 = rect1.Bottom > point2.Y ? rect1.Right : rect2.Left;
                points.Add(x1, point1.Y);
                points.Add(x1, y);
                points.Add(x2, y);
                points.Add(x2, point2.Y);
            }
            else if (point1.X >= point2.X && rect1.Left < rect2.Right)
            {
                float y = Math.Max(rect1.Bottom, point2.Y);
                float x1 = rect1.Right > rect2.Right || rect1.Top < rect2.Top ? Math.Max(rect1.Right, rect2.Right) : Math.Min(rect1.Right, rect2.Right);
                float x2 = rect1.Bottom > point2.Y ? rect1.Left : rect2.Right;
                points.Add(x1, point1.Y);
                points.Add(x1, y);
                points.Add(x2, y);
                points.Add(x2, point2.Y);
            }
            else if (!point1.X.Equals(point2.X) || rect1.Top <= rect2.Top || rect1.Bottom <= rect2.Bottom || (rect1.Bottom > rect2.Top && rect1.Top > rect2.Bottom))
            {
                float x = point1.X < point2.X ? rect1.Right : rect1.Left;
                float y = Math.Min(rect1.Bottom, point2.Y);
                points.Add(x, point1.Y);
                points.Add(x, y);
                if (point2.Y > y)
                    points.Add(x, point2.Y);
            }
        }

        if (reverse && points.Count > 3) // Wenn die Punkte umgekehrt wurden, kehre die Reihenfolge der Punkte ab dem dritten Punkt um
            for (int i1 = 2, i2 = points.Count - 1; i1 < i2; ++i1, --i2)
                (points[i1], points[i2]) = (points[i2], points[i1]);

        if (rotate) // Wenn die Punkte rotiert wurden, tausche die Werte von right und bottom
        {
            (right, bottom) = (bottom, right);
            for (int i = 2; i < points.Count; i++)
                points[i] = new PointF(points[i].Y, points[i].X);
        }

        if (flipX || flipY) // Wenn die Punkte gespiegelt wurden, passe die Position an
            for (int i = 2; i < points.Count; i++)
            {
                PointF p = points[i];
                if (flipX)
                    p.X = right - p.X; // Spiegelung entlang der X-Achse
                if (flipY)
                    p.Y = bottom - p.Y; // Spiegelung entlang der Y-Achse
                points[i] = p;
            }

        points.Add(edgeEndPoint); // Füge den Randendpunkt zur Liste der Punkte hinzu
        points.Add(endPoint);     // Füge den Endpunkt zur Liste der Punkte hinzu

        if (!points.CutCorners(marginStartRect, marginEndRect)) // Überprüfe, ob es unnötige Ecken gibt und entferne diese
        {
            points.RemoveRedundant();                          // Entferne redundante Punkte
            points.CutCorners(marginStartRect, marginEndRect); // Entferne unnötige Ecken
        }

        points.RemoveRedundant(); // Stelle sicher, dass alle redundanten Punkte entfernt werden
        return points.ToArray();  // Gebe das Array der Punkte zurück
    }

    private static PointF GetEdgePoint(PointF p, RectangleF rect, float margin)
    {
        if (p.X.Equals(rect.Left))                     // Überprüfe, ob der Punkt auf der linken Seite des Rechtecks liegt
            return new PointF(p.X - margin, p.Y); // Rückgabe eines neuen Punktes, der um den Margin nach links verschoben ist.

        if (p.Y.Equals(rect.Top))                      // Überprüfe, ob der Punkt auf der oberen Seite des Rechtecks liegt
            return new PointF(p.X, p.Y - margin); // Rückgabe eines neuen Punktes, der um den Margin nach oben verschoben ist.

        if (p.X.Equals(rect.Right))                    // Überprüfe, ob der Punkt auf der rechten Seite des Rechtecks liegt
            return new PointF(p.X + margin, p.Y); // Rückgabe eines neuen Punktes, der um den Margin nach rechts verschoben ist.

        if (p.Y.Equals(rect.Bottom))                   // Überprüfe, ob der Punkt auf der unteren Seite des Rechtecks liegt
            return new PointF(p.X, p.Y + margin); // Rückgabe eines neuen Punktes, der um den Margin nach unten verschoben ist.

        // Berechne die Abstände zu den Kanten des Rechtecks
        float distanceLeft = Math.Abs(p.X - rect.Left);
        float distanceTop = Math.Abs(p.Y - rect.Top);
        float distanceRight = Math.Abs(p.X - rect.Right);
        float distanceBottom = Math.Abs(p.Y - rect.Bottom);

        // Wenn der Punkt am nähesten zu der linken Kante liegt...
        if (distanceLeft <= distanceTop && distanceLeft <= distanceRight && distanceLeft <= distanceBottom)
            return new PointF(rect.Left, rect.Top + rect.Height / 2F);

        // Wenn der Punkt am nähesten zu der oberen Kante liegt...
        if (distanceTop <= distanceLeft && distanceTop <= distanceRight && distanceTop <= distanceBottom)
            return new PointF(rect.Left + rect.Width / 2F, rect.Top);

        // Wenn der Punkt am nähesten zu der rechten Kante liegt...
        if (distanceRight <= distanceLeft && distanceRight <= distanceTop && distanceRight <= distanceBottom)
            return new PointF(rect.Right, rect.Top + rect.Height / 2F);

        // Wenn der Punkt am nähesten zu der unteren Kante liegt...
        if (distanceBottom <= distanceLeft && distanceBottom <= distanceTop && distanceBottom <= distanceRight)
            return new PointF(rect.Left + rect.Width / 2F, rect.Bottom);

        // Wenn der Punkt nicht auf dem Rand des Rechtecks liegt, werfe eine Ausnahme mit einer entsprechenden Fehlermeldung.
        throw new Exception($"Der Punkt liegt nicht auf dem Rand von dem Rechteck.\n - Punkt: {p}\n - Rechteck: {rect}");
    }

    private static bool IntersectsWithLine(this RectangleF rect, PointF p1, PointF p2) => rect.Left < Math.Max(p1.X, p2.X) && rect.Top < Math.Max(p1.Y, p2.Y) && Math.Min(p1.X, p2.X) < rect.Right && Math.Min(p1.Y, p2.Y) < rect.Bottom;
    private static void Add(this List<PointF> points, float x, float y) => points.Add(new PointF(x, y));

    private static void RemoveRedundant(this List<PointF> points)
    {
        if (points.Count < 3)
            return;

        // Durchlaufe die Liste von hinten, um Punkte sicher zu entfernen.
        for (int i = points.Count - 2; i > 0; i--)
        {
            PointF p1 = points[i - 1]; // Der Punkt vor dem aktuellen Punkt.
            PointF p2 = points[i];     // Der aktuelle Punkt.
            PointF p3 = points[i + 1]; // Der Punkt nach dem aktuellen Punkt.

            // Überprüfe, ob die drei Punkte kollinear sind.
            // Die Bedingung prüft, ob der Flächeninhalt des Dreiecks, 
            // das durch die Punkte p1, p2 und p3 gebildet wird, gleich null ist.
            if (p1.X * (p2.Y - p3.Y) + p2.X * (p3.Y - p1.Y) + p3.X * (p1.Y - p2.Y) == 0)
                points.RemoveAt(i); // Entferne den überflüssigen Punkt p2, wenn er redundant ist.
        }
    }

    private static bool CutCorners(this List<PointF> points, RectangleF rect1, RectangleF rect2)
    {
        // Wenn die Liste weniger als 5 Punkte enthält, sind keine Ecken zu entfernen.
        if (points.Count < 5)
            return true;

        bool changed = false;  // Flag, um festzustellen, ob Änderungen vorgenommen wurden.
        PointF p1 = points[0]; // Der vorherige Punkt (zuerst der erste Punkt).
        PointF p3 = points[2]; // Der aktuelle Punkt, der möglicherweise verändert wird.
        PointF p4 = points[3]; // Der nachfolgende Punkt.
        PointF p2, p5;         // Weitere Punkte zur Verarbeitung.

        // Durchlaufe die Liste der Punkte, beginnend mit dem dritten Punkt.
        for (int i = 2; i < points.Count - 2; i++, p1 = p2, p3 = p4, p4 = p5)
        {
            p2 = points[i - 1]; // Der Punkt vor dem aktuellen Punkt.
            p5 = points[i + 2]; // Der Punkt nach dem nachfolgenden Punkt.

            // Überprüfe, ob die Punkte eine Ecke bilden, die entfernt werden kann.
            if (p1.Y.Equals(p2.Y) && p5.X.Equals(p4.X) && p3.X.Equals(p2.X) && p3.Y.Equals(p4.Y) && !p3.X.Equals(p4.X) && !p3.Y.Equals(p2.Y))
                p3 = new PointF(p4.X, p2.Y); // Setze p3 auf eine neue Position (horizontal).
            else if (p1.X.Equals(p2.X) && p5.Y.Equals(p4.Y) && p3.X.Equals(p4.X) && p3.Y.Equals(p2.Y) && !p3.X.Equals(p2.X) && !p3.Y.Equals(p4.Y))
                p3 = new PointF(p2.X, p4.Y); // Setze p3 auf eine neue Position (vertikal).
            else
                continue; // Wenn keine der Bedingungen erfüllt ist, fahre mit der nächsten Iteration fort.

            // Überprüfe, ob die Linie von p3 zu p2 oder p4 mit rect1 oder rect2 schneidet.
            if (rect1.IntersectsWithLine(p3, p2) ||
                rect1.IntersectsWithLine(p3, p4) ||
                rect2.IntersectsWithLine(p3, p2) ||
                rect2.IntersectsWithLine(p3, p4))
                continue; // Wenn es eine Kollision gibt, fahre mit der nächsten Iteration fort.

            // Aktualisiere den Punkt in der Liste und setze changed auf true.
            points[i] = p2 = p3;
            changed = true;
        }

        // Gib zurück, ob Änderungen an den Punkten vorgenommen wurden.
        return changed;
    }
}