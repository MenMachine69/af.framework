using DevExpress.Utils;
using DevExpress.Utils.Svg;

namespace AF.WINFORMS.DX;

/// <summary>
/// Ein Element in den Einstellungen
/// </summary>
public class AFSettingsElement : ISettingsElement
{
    /// <summary>
    /// Constructor
    /// </summary>
    public AFSettingsElement() { }

    /// <summary>
    /// Constructor.
    ///
    /// Erzeugt aus einem PropertyDescription Objekt das SettingsElement.
    /// </summary>
    /// <param name="property">PropertyDescription der Eigenschaft</param>
    public AFSettingsElement(PropertyDescription property)
    {
        ValueName = property.Name;
        Caption = property.Context?.TalkingName ?? "";
        Description = property.Context?.Description ?? "";
        ValueType = ((PropertyInfo)property).PropertyType;
    }

    /// <summary>
    /// Überschrift
    /// </summary>
    public string Caption { get; set; } = "";

    /// <summary>
    /// Beschreibung
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    /// Liste der untergeordneten Elemente
    /// </summary>
    public List<AFSettingsElement> Elements { get; } = [];
    
    /// <summary>
    /// Typ der zu bearbeitenden Einstellung.
    /// 
    /// NULL, wenn keine Einstellung existiert und dies nur eine Gruppe ist.
    /// </summary>
    public Type? ValueType { get; set; }

    /// <summary>
    /// Editor als Popup im Einstellungselement anzeigen
    /// </summary>
    public bool ShowEditorInPopup { get; set; } = false;

    /// <summary>
    /// Popup-Editor immer geöffnet (nur relevant wenn ShowEditorInPopup == true ist)
    /// </summary>
    public bool PopupAllwaysOpen { get; set; } = false;

    /// <summary>
    /// Name des Wertes (wird zu Namen des Editors, damit DataBinding funktioniert).
    /// </summary>
    public string ValueName { get; set; } = "";

    /// <summary>
    /// Ist ein Unterelement, dass direkt im Popup dargestellt wird.
    ///
    /// SubElemente dürfen KEINE weiteren Elemente in Elements enthalten. 
    /// </summary>
    public bool IsSubElement { get; set; } = false;

    /// <summary>
    /// Für den Eintrag zu verwendendes Symbol
    /// </summary>
    public SvgImage? Symbol { get; set; }

    /// <summary>
    /// Farbanpassungsmodus für das Symbol
    /// </summary>
    public SvgImageColorizationMode SymbolColorization { get; set; } = SvgImageColorizationMode.Full;
}