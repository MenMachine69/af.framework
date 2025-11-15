using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace AF.CORE;

/// <summary>
/// Erweiterungen für die Klasse Image
/// </summary>
public static class ImageEx
{
    private const float rwgt = 0.3086f;
    private const float gwgt = 0.6094f;
    private const float bwgt = 0.0820f;

    /// <summary>
    /// Bild in Graustufen wandeln
    /// </summary>
    /// <param name="img">Bild</param>
    /// <returns>umgewandeltes Bild</returns>
    public static Image GreyScale(this Image img)
    {
        Bitmap grayBitmap = new(img.Width, img.Height);

        ImageAttributes imgAttributes = new();

        ColorMatrix gray = new(
            new[]
            {
                new[] { 0.299f, 0.299f, 0.299f, 0, 0 },
                new[] { 0.588f, 0.588f, 0.588f, 0, 0},
                new[] { 0.111f, 0.111f, 0.111f, 0, 0 },
                new float[] { 0, 0, 0, 1, 0 },
                new float[] { 0, 0, 0, 0, 1},
            });

        imgAttributes.SetColorMatrix(gray);

        using (Graphics g = Graphics.FromImage(grayBitmap))
        {
            g.DrawImage(img, new(0, 0, img.Width, img.Height),
                0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttributes);
        }

        return grayBitmap;
    }

    /// <summary>
    /// Sätttigung eine Bildes ändern
    /// </summary>
    /// <param name="img">Bild </param>
    /// <param name="saturation">Anpassung der Süttigung (0.0 bis 2.0, 1.0 keine Änderung)</param>
    /// <returns>angepasstes Bild</returns>
    public static Image Saturation(this Image img, float saturation)
    {
        Bitmap bmp = new(img.Width, img.Height);

        ImageAttributes imgAttributes = new();

        float newval = 1f - saturation;
        float baseSat = 1.0f - newval;

        ColorMatrix matrix = new();

        matrix[0, 0] = baseSat * rwgt + newval;
        matrix[0, 1] = baseSat * rwgt;
        matrix[0, 2] = baseSat * rwgt;
        matrix[1, 0] = baseSat * gwgt;
        matrix[1, 1] = baseSat * gwgt + newval;
        matrix[1, 2] = baseSat * gwgt;
        matrix[2, 0] = baseSat * bwgt;
        matrix[2, 1] = baseSat * bwgt;
        matrix[2, 2] = baseSat * bwgt + newval;

        imgAttributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

        using (Graphics g = Graphics.FromImage(bmp))
        {

            g.DrawImage(img, new(0, 0, img.Width, img.Height),
                0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttributes);
        }

        return bmp;
    }

    /// <summary>
    /// Brightness eine Bildes ändern
    /// </summary>
    /// <param name="img">Bild </param>
    /// <param name="brightness">Anpassung der Brightness (0.0 bis 2.0, 1.0 keine Änderung)</param>
    /// <returns>angepasstes Bild</returns>
    public static Image Brightness(this Image img, float brightness)
    {
        Bitmap bmp = new(img.Width, img.Height);

        ImageAttributes imgAttributes = new();

        ColorMatrix matrix = new();

        float newval = 1f + brightness;
        float adjustedBrightness = newval - 1f;

        matrix[4, 0] = adjustedBrightness;
        matrix[4, 1] = adjustedBrightness;
        matrix[4, 2] = adjustedBrightness;

        imgAttributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

        using (Graphics g = Graphics.FromImage(bmp))
        {

            g.DrawImage(img, new(0, 0, img.Width, img.Height),
                0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttributes);
        }

        return bmp;
    }

    /// <summary>
    /// Anpassen von Contrast, Brightness und Gamma eines Bildes
    /// </summary>
    /// <param name="img">Bild</param>
    /// <param name="brightness">Wert zwischen 0.0 und 2.0, 1.0 = keine Änderung</param>
    /// <param name="contrast">Wert größer 0.0, 1.0 = keine Änderung</param>
    /// <param name="gamma">Wert größer 0.0, 1.0 = keine Änderung</param>
    /// <returns>angepasstest Image</returns>
    public static Image BrightnessContrastGamma(this Image img, float brightness, float contrast, float gamma)
    {
        Bitmap bmp = new(img.Width, img.Height);

        ImageAttributes imgAttributes = new();

        float adjustedBrightness = brightness - 1.0f;

        ColorMatrix matrix = new(
            new[]
            {
                new[] { contrast, 0, 0, 0, 0 },
                new[] { 0, contrast, 0, 0, 0 },
                new[] { 0, 0, contrast, 0, 0 },
                new[] { 0, 0, 0, 1.0f, 0 },
                new[] { adjustedBrightness, adjustedBrightness, adjustedBrightness, 0, 1 },
            });

        imgAttributes.ClearColorMatrix();
        imgAttributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
        imgAttributes.SetGamma(gamma, ColorAdjustType.Bitmap);

        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.DrawImage(img, new(0, 0, img.Width, img.Height),
                0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttributes);
        }

        return bmp;
    }

    /// <summary>
    /// Rot-, Grün- und Blauanteil eines Bildes sowie dessen Gammawert anpassen
    /// 
    /// Beispiele:
    /// Bei Verwendung eines im Original dunkelgrauen Bildes
    ///
    ///    blau (Metro): 0,1,3,0.55
    ///    hellgrau: 1,1,1,0.35
    ///    weiü: 1,1,1,0.01
    ///    grün: 0,2,0,0.75
    ///    rot: 2,0,0,0.75
    /// </summary>
    /// <param name="img">Bild</param>
    /// <param name="red">VerÄnderung des Rotanteils (Multiplikator, 0.00 bis ...) 1.0 = keine Änderung</param>
    /// <param name="green">VerÄnderung des Grünanteils (Multiplikator, 0.00 bis ...) 1.0 = keine Änderung</param>
    /// <param name="blue">VerÄnderung des Blauanteils (Multiplikator, 0.00 bis ...) 1.0 = keine Änderung</param>
    /// <param name="gamma">VerÄnderung des Gammawertes (Multiplikator, > 0.00 ) 1.0 = keine Änderung</param>
    /// <returns>das umgewandelte Bild</returns>
    public static Image AdjustRGBGamma(this Image img, float red, float green, float blue, float gamma)
    {
        Bitmap bmp = new(img.Width, img.Height);

        ImageAttributes imgAttributes = new();

        ColorMatrix matrix = new(
            new[]
            {
                new[] { red, 0, 0, 0, 0 },
                new[] { 0, green, 0, 0, 0 },
                new[] { 0, 0, blue, 0, 0 },
                new[] { 0, 0, 0, 1.0f, 0 },
                new float[] { 0, 0, 0, 0, 1 },
            });

        imgAttributes.ClearColorMatrix();
        imgAttributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
        imgAttributes.SetGamma(gamma, ColorAdjustType.Bitmap);

        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.DrawImage(img, new(0, 0, img.Width, img.Height),
                0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttributes);
        }

        return bmp;
    }

    /// <summary>
    /// Ersetzt in einem Bitmap eine Farbe durch eine andere
    /// </summary>
    /// <param name="bitmap">Bitmap</param>
    /// <param name="source">zu ändernde Farbe</param>
    /// <param name="destination">geünderte Farbe</param>
    /// <returns>das geünderte Bitmap</returns>
    public static Image ChangeColor(this Image bitmap, Color source, Color destination)
    {
        var newBitmap = new Bitmap(bitmap);
        var data = newBitmap.LockBits(new(Point.Empty, newBitmap.Size), ImageLockMode.ReadWrite, newBitmap.PixelFormat);
        var ptr = data.Scan0;

        // Declare an array to hold the bytes of the bitmap.
        var bytes = Math.Abs(data.Stride) * newBitmap.Height;
        var values = new byte[bytes];
        var bpp = data.Stride / data.Width;

        // Copy the RGB values into the array.
        Marshal.Copy(ptr, values, 0, bytes);

        for (var i = 0; i < bytes; i += bpp)
        {
            var color = Color.FromArgb(
                bpp == 4 ? values[i + 3] : 255, //Alpha
                values[i + 2], //Red
                values[i + 1], //Green
                values[i + 0]);//Blue

            if (color == source)
            {
                if (bpp == 4)
                    values[i + 3] = destination.A;

                values[i + 2] = destination.R;
                values[i + 1] = destination.G;
                values[i + 0] = destination.B;
            }
        }

        Marshal.Copy(values, 0, ptr, bytes);
        newBitmap.UnlockBits(data);
        return newBitmap;
    }

    /// <summary>
    /// Liefert ein Bild aus einer Datei, ohne das dabei die Datei blockiert wird
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static Image FromFile(string file)
    {
        Image img;

        using (FileStream fstream = new(file, FileMode.Open, FileAccess.Read))
            img = Image.FromStream(fstream);

        return img;
    }

    /// <summary>
    /// Image in ein Byte-Array umwandeln
    /// </summary>
    /// <param name="image"></param>
    /// <returns></returns>
    public static byte[] AsByteArray(Image image)
    {
        using (MemoryStream ms = new())
        {
            image.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }
    }

    /// <summary>
    /// Image aus einem ByteArray erzeugen
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static Image FromByteArray(byte[] data)
    {
        using (MemoryStream ms = new())
        {
            ms.Write(data, 0, data.Length);
            ms.Flush();
            return Image.FromStream(ms);
        }
    }

    /// <summary>
    /// Erzeugt aus einem Image ein ThumbNail-Image mit einer maximalen Abmessung unter Beibehaltung der Proportionen
    /// </summary>
    /// <param name="image">Image von dem das ThumbNail erzeugt werden soll</param>
    /// <param name="maxwidth">max. Breite des ThumbNail</param>
    /// <param name="maxheight">max. Hühe des ThumbNail</param>
    /// <returns>das ThumbNail-Image</returns>
    public static Image CreateThumbnail(this Image image, int maxwidth, int maxheight)
    {
        if (image.Width > maxwidth || image.Height > maxheight)
        {
            // Berechne die Breite und Hühe des Thumnails
            if (image.Width >= image.Height)
            {
                maxheight = maxwidth * image.Height / image.Width;
            }
            else
            {
                maxwidth = maxheight * image.Width / image.Height;
            }


            return image.GetThumbnailImage(maxwidth, maxheight, delegate { return false; }, IntPtr.Zero);
        }

        return image.GetThumbnailImage(image.Width, image.Height, delegate { return false; }, IntPtr.Zero);
    }

    /// <summary>
    /// Liefert ein ImageCodecInfo-Objekt zu einem Mime-typ (z.B. image/jpeg
    /// </summary>
    /// <param name="mimeType">Mime-Typ (z.B. image/jpeg)</param>
    /// <returns>ImageCodecInfo-Objekt</returns>
    public static ImageCodecInfo? GetEncoderInfo(string mimeType)
    {
        int j;
        ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
        for (j = 0; j < encoders.Length; ++j)
        {
            if (encoders[j].MimeType == mimeType)
                return encoders[j];
        }
        return null;
    }

    /// <summary>
    /// Speichert ein Image als Jpeg ab
    /// </summary>
    /// <param name="image">zu speicherndes Image</param>
    /// <param name="filename">vollst. Dateiname</param>
    public static void SaveAsJpeg(this Image image, string filename)
    {
        ImageCodecInfo? codec = GetEncoderInfo("image/jpeg");
        Encoder enc = Encoder.Quality;
        EncoderParameters para = new(1);
        EncoderParameter quality = new(enc, 100L);
        para.Param[0] = quality;
        image.Save(filename, codec!, para);
    }

    /// <summary>
    /// Speichert ein Image als Png ab
    /// </summary>
    /// <param name="image">zu speicherndes Image</param>
    /// <param name="filename">vollst. Dateiname</param>
    public static void SaveAsPng(this Image image, string filename)
    {
        image.Save(filename, ImageFormat.Png);
    }

    /// <summary>
    /// Speichert ein Image als Gif ab
    /// </summary>
    /// <param name="image">zu speicherndes Image</param>
    /// <param name="filename">vollst. Dateiname</param>
    public static void SaveAsGif(this Image image, string filename)
    {
        image.Save(filename, ImageFormat.Gif);
    }

    /// <summary>
    /// Speichert ein Image als Bmp (Bitmap) ab
    /// </summary>
    /// <param name="image">zu speicherndes Image</param>
    /// <param name="filename">vollst. Dateiname</param>
    public static void SaveAsBmp(this Image image, string filename)
    {
        image.Save(filename, ImageFormat.Bmp);
    }


    /// <summary>
    /// Größe eines Bildes ändern
    /// </summary>
    /// <param name="img">das zu skalierende Bild</param>
    /// <param name="newsize">neue Größe</param>
    /// <returns>das geünderte Bild</returns>
    public static Image Resize(this Image img, Size newsize)
    {
        var destRect = new Rectangle(0, 0, newsize.Width, newsize.Height);
        var destImage = new Bitmap(newsize.Width, newsize.Height);

        destImage.SetResolution(img.HorizontalResolution, img.VerticalResolution);

        using (var graphics = Graphics.FromImage(destImage))
        {
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            using (var wrapMode = new ImageAttributes())
            {
                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                graphics.DrawImage(img, destRect, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, wrapMode);
            }
        }

        return destImage;
    }

    /// <summary>
    /// Bild anhand eines Rechteckes für den Ausschnitt zuschneiden
    /// </summary>
    /// <param name="image">
    /// das Bild
    /// </param>
    /// <param name="selection">
    /// die Auswahl
    /// </param>
    /// <returns>
    /// das zugeschnittene Bild
    /// </returns>
    public static Image Crop(this Image image, Rectangle selection)
    {
        // Check if it is a bitmap:
        if (!(image is Bitmap bmp))
            throw new ArgumentException("Kein gültiges Bitmap.");

        if (selection.X + selection.Width > image.Width)
            selection.Width = image.Width - selection.X;

        if (selection.Y + selection.Height > image.Height)
            selection.Height = image.Height - selection.Y;

        // Crop the image:
        Bitmap cropBmp = bmp.Clone(selection, bmp.PixelFormat);

        // Release the resources:
        bmp.Dispose();

        return cropBmp;
    }
}
