namespace AF.CORE;

/// <summary>
/// Speicherbares Template
/// </summary>
[Serializable]
public class AFHtmlTemplate
{
    /// <summary>
    /// HTML-Code
    /// </summary>
    public string HtmlTemplate { get; set; }

    /// <summary>
    /// CSS-Code
    /// </summary>
    public string CssTemplate { get; set; }

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFHtmlTemplate()
    {
        HtmlTemplate = "";
        CssTemplate = "";
    }

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="html"></param>
    /// <param name="css"></param>
    public AFHtmlTemplate(string html, string css)
    {
        HtmlTemplate = html;
        CssTemplate = css;
    }
}