namespace AF.WINFORMS.DX;

/// <summary>
/// Interface für einen Controller für Einstellungsdialoge
/// </summary>
public interface ISettingsController
{
    /// <summary>
    /// Liefert die Einstellungen für den spezifischen Einstellungsdialog
    /// </summary>
    /// <param name="config">Name der benötigten Konfiguration (USER, SYSTEM)</param>
    AFSettings GetSettings(string config);

    /// <summary>
    /// Liefert den passenden Editor für ein Element
    /// </summary>
    /// <param name="element">Element, für das der Editor benötigt wird</param>
    /// <returns>das Control, das als Editor verwendet wird.
    /// NULL, wenn ein Standard-Control (vom ValueType abhängig) verwendet werden soll.</returns>
    Control? GetEditor(AFSettingsElement element);
}

/// <summary>
/// Einstellungselement
/// </summary>
public interface ISettingsElement
{
    /// <summary>
    /// Untergeordnete Elemente
    /// </summary>
    List<AFSettingsElement> Elements { get; }

    /// <summary>
    /// Überschrift
    /// </summary>
    string Caption { get; set; }

    /// <summary>
    /// Beschreibung
    /// </summary>
    string Description { get; set; }
}