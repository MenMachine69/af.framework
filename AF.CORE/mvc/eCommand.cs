namespace AF.MVC;

/// <summary>
/// Typ/Art des Commands
/// </summary>
public enum eCommand
{
    /// <summary>
    /// Speichern
    /// </summary>
    Save = 0,
    /// <summary>
    /// Neu
    /// </summary>
    New = 1,
    /// <summary>
    /// Neu aus Kopie
    /// </summary>
    NewCopy = 2,
    /// <summary>
    /// Löschen
    /// </summary>
    Delete = 3,
    /// <summary>
    /// Bearbeiten
    /// </summary>
    Edit = 4,
    /// <summary>
    /// Gehe zu
    /// </summary>
    Goto = 5,
    /// <summary>
    /// Details anzeigen
    /// </summary>
    ShowDetails = 6,
    /// <summary>
    /// Importieren
    /// </summary>
    Import = 7,
    /// <summary>
    /// Importieren
    /// </summary>
    Export = 8,
    /// <summary>
    /// Andere
    /// </summary>
    Other = 9
}