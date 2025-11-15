using DevExpress.Utils;
using DevExpress.Utils.Svg;

namespace AF.WINFORMS.DX;

/// <summary>
/// Glyphs für Anwendung (Symbole/Bilder)
/// </summary>
[ToolboxItem(false)]
public partial class Glyphs : Component
{
    private static Glyphs _instance = new();

    private Glyphs()
    {
        InitializeComponent();
    }

    private Glyphs(IContainer container)
    {
        container.Add(this);

        InitializeComponent();
    }

    /// <summary>
    /// Liefert die komplette Sammlung der Symbol-Images
    /// </summary>
    /// <returns>Sammlung als SvgImageCollection</returns>
    public static SvgImageCollection GetImages() => _instance.images;

    /// <summary>
    /// Liefert ein SvgImage aus der images Sammlung (GetImages())
    /// </summary>
    /// <param name="imgIndex">Index des Images (siehe enum Symbol)</param>
    /// <returns>das SvgImage der Sammlung</returns>
    public static SvgImage GetImage(Symbol imgIndex) => _instance.images[(int)imgIndex];

    /// <summary>
    /// Liefert die komplette Sammlung der Object-Images
    /// </summary>
    /// <returns>Sammlung als SvgImageCollection</returns>
    public static SvgImageCollection GetObjectImages() => _instance.objects;

    /// <summary>
    /// Liefert ein SvgImage aus der objects Sammlung (GetObjectImages())
    /// </summary>
    /// <param name="imgIndex">Index des Images (siehe enum ObjectImages)</param>
    /// <returns>das SvgImage der Sammlung</returns>
    public static SvgImage GetObjectImage(ObjectImages imgIndex) => _instance.objects[(int)imgIndex];

    /// <summary>
    /// Liefert die komplette Sammlung der SW Object-Images
    /// </summary>
    /// <returns>Sammlung als SvgImageCollection</returns>
    public static SvgImageCollection GetObjectImagesSw() => _instance.objects_sw;

    /// <summary>
    /// Liefert ein SvgImage aus der objects_sw Sammlung (GetObjectImagesSw())
    /// </summary>
    /// <param name="imgIndex">Index des Images (siehe enum ObjectImages)</param>
    /// <returns>das SvgImage der Sammlung</returns>
    public static SvgImage GetObjectImageSw(ObjectImages imgIndex) => _instance.objects_sw[(int)imgIndex];
}
