namespace AF.WINFORMS.DX;


/// <summary>
/// Konfiguration eines Grids als serialisierbare Information
/// </summary>
[Serializable]
public class AFGridLayout
{
    /// <summary>
    /// serialisiertes Layout eines GridViews
    /// </summary>
    public byte[] GridViewLayout { get; set; } = [];

    /// <summary>
    /// HTML-Template zur Darstellung
    /// </summary>
    public string HtmlTemplate { get; set; } = "";

    /// <summary>
    /// CSS-Template zur Darstellung
    /// </summary>
    public string CssTemplate { get; set; } = "";
}

