using System.Drawing;
using System.Drawing.Imaging;

namespace AF.CORE;

/// <summary>
/// Filterfunktionen für Bitmaps
/// 
/// Wendet Weichzeichner und Gaußscher Weichzeichner auf eine Bitmap an (Unschärfeeffekt).
/// </summary>
public class BitmapFilter
{
    private static bool _conv3x3(Bitmap b, _matrix m)
    {
        // Avoid divide by zero errors
        if (0 == m.Factor) return false;

        Bitmap bSrc = (Bitmap)b.Clone();

        // GDI+ still lies to us - the return format is BGR, NOT RGB.
        BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite,
            PixelFormat.Format24bppRgb);
        BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadWrite,
            PixelFormat.Format24bppRgb);

        int stride = bmData.Stride;
        int stride2 = stride * 2;
        IntPtr Scan0 = bmData.Scan0;
        IntPtr SrcScan0 = bmSrc.Scan0;

        unsafe
        {
            byte* p = (byte*)(void*)Scan0;
            byte* pSrc = (byte*)(void*)SrcScan0;

            int nOffset = stride + 6 - b.Width * 3;
            int nWidth = b.Width - 2;
            int nHeight = b.Height - 2;

            for (int y = 0; y < nHeight; ++y)
            {
                for (int x = 0; x < nWidth; ++x)
                {
                    int nPixel = (((pSrc[2] * m.TopLeft) + (pSrc[5] * m.TopMid) + (pSrc[8] * m.TopRight) +
                               (pSrc[2 + stride] * m.MidLeft) + (pSrc[5 + stride] * m.Pixel) +
                               (pSrc[8 + stride] * m.MidRight) +
                               (pSrc[2 + stride2] * m.BottomLeft) + (pSrc[5 + stride2] * m.BottomMid) +
                               (pSrc[8 + stride2] * m.BottomRight)) / m.Factor) + m.Offset;

                    if (nPixel < 0) nPixel = 0;
                    if (nPixel > 255) nPixel = 255;

                    p[5 + stride] = (byte)nPixel;

                    nPixel = (((pSrc[1] * m.TopLeft) + (pSrc[4] * m.TopMid) + (pSrc[7] * m.TopRight) +
                               (pSrc[1 + stride] * m.MidLeft) + (pSrc[4 + stride] * m.Pixel) +
                               (pSrc[7 + stride] * m.MidRight) +
                               (pSrc[1 + stride2] * m.BottomLeft) + (pSrc[4 + stride2] * m.BottomMid) +
                               (pSrc[7 + stride2] * m.BottomRight)) / m.Factor) + m.Offset;

                    if (nPixel < 0) nPixel = 0;
                    if (nPixel > 255) nPixel = 255;

                    p[4 + stride] = (byte)nPixel;

                    nPixel = (((pSrc[0] * m.TopLeft) + (pSrc[3] * m.TopMid) + (pSrc[6] * m.TopRight) +
                               (pSrc[0 + stride] * m.MidLeft) + (pSrc[3 + stride] * m.Pixel) +
                               (pSrc[6 + stride] * m.MidRight) +
                               (pSrc[0 + stride2] * m.BottomLeft) + (pSrc[3 + stride2] * m.BottomMid) +
                               (pSrc[6 + stride2] * m.BottomRight)) / m.Factor) + m.Offset;

                    if (nPixel < 0) nPixel = 0;
                    if (nPixel > 255) nPixel = 255;

                    p[3 + stride] = (byte)nPixel;

                    p += 3;
                    pSrc += 3;
                }

                p += nOffset;
                pSrc += nOffset;
            }
        }

        b.UnlockBits(bmData);
        bSrc.UnlockBits(bmSrc);

        return true;
    }

    /// <summary>
    /// Gaußscher Weichzeichner
    /// </summary>
    /// <param name="b">Bitmap, auf die der Filter angewendet wird</param>.
    /// <param name="nWeight">Stärke des Filters (Standard: 4)</param>
    /// <returns>Resultat der Anwendung des Filters</returns>
    public static bool GaussianBlur(Bitmap b, int nWeight /* default to 4*/)
    {
        checked
        {
            _matrix m = new();
            m.SetAll(1);
            m.Pixel = nWeight;
            m.TopMid = m.MidLeft = m.MidRight = m.BottomMid = 2;
            m.Factor = Math.Min(nWeight + 12, int.MaxValue);

            return _conv3x3(b, m);
        }
    }

    /// <summary>
    /// Unschärfefilter
    /// </summary>
    /// <param name="b">Bitmap, auf die der Filter angewendet wird</param>.
    /// <param name="nWeight">Stärke des Unschärfeeffekts</param>.
    /// <returns>Resultat der Anwendung des Filters</returns>
    public static bool Smooth(Bitmap b, int nWeight /* default to 1 */)
    {
        checked
        {
            _matrix m = new();
            m.SetAll(1);
            m.Pixel = nWeight;
            m.Factor = Math.Min(nWeight + 8, int.MaxValue);

            return _conv3x3(b, m);
        }
    }

    private class _matrix
    {
        public int TopLeft, TopMid, TopRight;
        public int MidLeft, Pixel = 1, MidRight;
        public int BottomLeft, BottomMid, BottomRight;
        public int Factor = 1;
        public int Offset = 0;

        public void SetAll(int nVal)
        {
            TopLeft = TopMid = TopRight = MidLeft = Pixel = MidRight = BottomLeft = BottomMid = BottomRight = nVal;
        }
    }
}

