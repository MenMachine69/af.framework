namespace AF.MVC;

/// <summary>
/// Detailanzeigemodus
/// </summary>
public enum ePageDetailMode
{
    /// <summary>
    /// Standardmodus, abhängig von der konkreten Benutzeroberfläche
    /// </summary>
    Default,
    /// <summary>
    /// Details nicht anzeigen - für Seiten ohne Details
    /// </summary>
    NoDetails,
    /// <summary>
    /// nur Details anzeigen - für Seiten ohne Master-Editor
    /// </summary>
    DetailsOnly,
    /// <summary>
    /// Anzeige von Details als Registerkarte, abhängig von konkreter UI
    /// </summary>
    DetailsAsTab
}