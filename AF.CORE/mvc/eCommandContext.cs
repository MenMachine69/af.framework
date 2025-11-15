namespace AF.MVC;

/// <summary>
/// Context, in dem das Command sichtbar ist. 
/// Mehrere Einstellungen können kombiniert werden (Bsp: DetailContext und GridContext)
/// </summary>
[Flags]
public enum eCommandContext
{
    /// <summary>
    /// Nirgendwo sichtbar, kann nur via Code ausgeführt werden
    /// </summary>
    Nowhere = 1 << 0,
    /// <summary>
    /// Master - Contextmenu
    /// </summary>
    MasterContext = 1 << 1,
    /// <summary>
    /// Detail - Contextmenu
    ///
    /// Das DetailContext-menu wird auch in AFDetail verwendet!
    /// </summary>
    DetailContext = 1 << 2,
    /// <summary>
    /// Gridview - Contextmenu
    /// </summary>
    GridContext = 1 << 3,
    /// <summary>
    /// Gridview - Button
    /// </summary>
    GridButton = 1 << 4,
    /// <summary>
    /// Other Context
    /// </summary>
    Other = 1 << 5,
    /// <summary>
    /// in Combobox sichtbar
    /// </summary>
    ComboBox = 1 << 6,
    /// <summary>
    /// Überall sichtbar
    /// </summary>
    EveryWhere = 1 << 7,
    /// <summary>
    /// Popup-Menü des Masters
    /// </summary>
    MasterPopup = 1 << 8,
    /// <summary>
    /// Browser
    /// </summary>
    Browser = 1 << 9,
    /// <summary>
    /// Button in einem HTML-Template für Grids
    /// </summary>
    HTMLGridButton = 1 << 10,
    /// <summary>
    /// Kontextmenü eines Links
    /// </summary>
    LinkContext = 1 << 11
}