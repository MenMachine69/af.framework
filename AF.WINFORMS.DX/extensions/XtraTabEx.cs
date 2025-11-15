using DevExpress.Utils;
using DevExpress.XtraTab;

namespace AF.WINFORMS.DX;

/// <summary>
/// Erweiterungsmethoden für XtraTabControl und XtraTabPage
/// </summary>
public static class XtraTabEx
{
    /// <summary>
    /// Symbol zuweisen, dass für die Page angezeigt werden soll
    /// </summary>
    /// <param name="page">Page</param>
    /// <param name="image">ObjectImage</param>
    /// <returns>Page</returns>
    public static XtraTabPage SetSymbol(this XtraTabPage page, ObjectImages image)
    {
        page.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.None;
        page.ImageOptions.SvgImageSize = new(24, 24);
        page.ImageOptions.ImageIndex = (int)image;

        return page;
    }
}