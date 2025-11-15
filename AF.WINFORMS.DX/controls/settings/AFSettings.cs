namespace AF.WINFORMS.DX;

/// <summary>
/// Klasse zur strukturierten Verwaltung von Einstellungen in einem
/// AFSettingsDialog (wie Einstellungen bei Windows 11).
/// </summary>
public class AFSettings : ISettingsElement
{
    /// <summary>
    /// Liste der Elemente
    /// </summary>
    public List<AFSettingsElement> Elements { get; } = [];
    
    /// <summary>
    /// Überschrift
    /// </summary>
    public string Caption { get; set; } = "Einstellungen";

    /// <summary>
    /// Beschreibung
    /// </summary>
    public string Description { get; set; } = "Einstellungen der Anwendung. " +
                                              "<b>Bitte beachten Sie, dass einige der Einstellungen erst mit einem Neustart der Anwendung wirksam werden.</b> " +
                                              "Es empfiehlt sich daher die Anwendung nach Änderungen an den Einstellungen einmal neu zu starten.";
}