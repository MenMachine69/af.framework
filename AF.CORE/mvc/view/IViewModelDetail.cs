namespace AF.MVC;

/// <summary>
/// Interface für ein Control, dass ein Model anzeigen kann (Detailanzeige für ein Model) 
/// und ein Öffnen des angezeigten Models unterstützt (GeheZu).
/// </summary>
public interface IViewModelDetail
{
    /// <summary>
    /// current model
    /// </summary>
    IModel? Model { get; set; }

    /// <summary>
    /// Gehe zu/Öffnen des aktuellen Models
    /// </summary>
    void Goto();

    /// <summary>
    /// Anzeige aktualisieren, weil sich die Auswahl geändert hat.
    /// </summary>
    /// <param name="detail"></param>
    void Update(IViewDetail detail);
}