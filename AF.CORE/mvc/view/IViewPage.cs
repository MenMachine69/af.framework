namespace AF.MVC;

/// <summary>
/// Fortschrittsanzeige für ein IViewPage
/// </summary>
public interface IViewPageProgress
{
    /// <summary>
    /// Überschrift
    /// </summary>
    string Caption { get; set; }

    /// <summary>
    /// Beschreibung
    /// </summary>
    string Description { get; set; }

    /// <summary>
    /// max. Anzahl Schritte
    /// </summary>
    int Total { get; set; }

    /// <summary>
    /// akt. Schritt
    /// </summary>
    int Current { get; set; }

    /// <summary>
    /// Abgebrochen durch Benutzer
    /// </summary>
    bool Canceled { get; set; }
}

/// <summary>
/// Schnittstelle einer Seite, die betrachtet/angezeigt werden kann
/// </summary>
public interface IViewPage : IUIElement
{
    /// <summary>
    /// ViewContext (z.B. für Befehls/Command-Anzeige) des Masters
    /// </summary>
    string ViewContextMaster { get; set;  }

    /// <summary>
    /// ViewContext (z.B. für Befehls/Command-Anzeige) des Details
    /// </summary>
    string ViewContextDetail { get; set; }

    /// <summary>
    /// Zugriff auf den aktuellen Controller
    /// </summary>
    IControllerUI? CurrentController { get; }

    /// <summary>
    /// Zugriff auf den aktuell im Detailbereich der Seite verwendeten Controller.
    /// </summary>
    IControllerUI? CurrentDetailController { get; }

    /// <summary>
    /// Aktueller MasterEditor (kann NULL sein)
    /// </summary>
    IEditor? ViewEditor { get; }

    /// <summary>
    /// Aktueller DetailView (kann NULL sein)
    /// </summary>
    IViewDetail? ViewDetail { get; }

    /// <summary>
    /// Link für aktuelles Anzeige-Mastermodell
    /// </summary>
    ModelLink? ModelLink { get; }

    /// <summary>
    /// ID des aktuellen Modells (Master)
    /// </summary>
    Guid? ModelID { get; }

    /// <summary>
    /// Seite kann geschlossen werden
    /// </summary>
    bool CanClose { get; }

    /// <summary>
    /// Seite schließen
    /// </summary>
    /// <returns>true wenn erfolgreich</returns>
    bool Close();

    /// <summary>
    /// Lade Ansicht für das Modell, das durch den angegebenen Link beschrieben wird
    /// </summary>
    /// <param name="link">Link, der das Modell beschreibt</param>
    /// <returns>die Seite</returns>
    IViewPage LoadModel(ModelLink link);

    /// <summary>
    /// Ansicht für das Modell laden, das durch die ID des Modells beschrieben wird
    /// </summary>
    /// <param name="modelID">Id des Modells</param>
    /// <param name="modelType">Typ des Modells</param>
    /// <returns>die Seite</returns>
    IViewPage LoadModel(Type modelType, Guid modelID);

    /// <summary>
    /// Ansicht für das Modell laden
    /// </summary>
    /// <param name="model">Modell, für das die Seite geladen werden soll</param>
    /// <returns>die Seite</returns>
    IViewPage LoadModel(IModel model);

    /// <summary>
    /// Richtet die Seite für den Controller ein.
    /// </summary>
    /// <param name="controller">der Controller</param>
    /// <returns>die Seite</returns>
    IViewPage Setup(IControllerUI controller);

    /// <summary>
    /// Dialog für ein PropertyModel laden.
    /// </summary>
    /// <param name="model">Model/Einstellungen</param>
    /// <param name="caption">Überschrift</param>
    /// <param name="page">Seite, die den Dialog anzeigt</param>
    IDialogContainer LoadPropertyDialog(IModel model, string caption, IViewPage? page = null);

    /// <summary>
    /// SidebarDialog anzeigen
    /// </summary>
    /// <param name="editor">Dialog</param>
    /// <param name="caption">Überschrift</param>
    /// <param name="page">Seite</param>
    /// <returns>der Dialog</returns>
    IDialogContainer ShowSidebarDialog(IDialogContainer editor, string caption, IViewPage? page = null);

    /// <summary>
    /// Dialog für ein PropertyModel laden, dass die Einstellungen repräsentiert.
    /// </summary>
    /// <param name="model">Model/Einstellungen</param>
    /// <param name="caption">Überschrift</param>
    /// <param name="page">Seite, die den Dialog anzeigt</param>
    /// <param name="folded">geschlossen darstellen</param>
    /// <param name="width">Breite des Dialogs (Standard: 300)</param>
    /// <param name="widthfolded">Breite wenn geschlossen (Standard: 30)</param>
    IDialogContainer LoadSettingsDialog(IModel model, string caption, IViewPage? page = null, bool folded = true, int width = 300, int widthfolded = 30);

    /// <summary>
    /// Den Einstellungsdialog 'zusammenfalten' (geschlossene Darstellung)
    /// </summary>
    void FoldSettingsDialog();

    /// <summary>
    /// Den Einstellungsdialog 'auffalten' (geöffnete Darstellung)
    /// </summary>
    void UnfoldSettingsDialog();

    /// <summary>
    /// Einen PropertyDialog schließen.
    /// </summary>
    void ClosePropertyDialog();

    /// <summary>
    /// Verarbeiten eines CommandResult-Objektes (Meldung anzeigen etc.)
    /// </summary>
    /// <param name="result"></param>
    void HandleResult(CommandResult  result);

    /// <summary>
    /// Funktionstastenleiste verbergen
    /// </summary>
    void HideFKeyBar();

    /// <summary>
    /// Funktionstastenleiste anzeigen
    /// </summary>
    void ShowFKeyBar();

    /// <summary>
    /// Command für die aktuelle Seite ausführen
    /// </summary>
    /// <param name="command"></param>
    void InvokeCommand(AFCommand command);

    /// <summary>
    /// Command für das aktuelle Detail ausführen
    /// </summary>
    /// <param name="command"></param>
    void InvokeCommandDetail(AFCommand command);

    /// <summary>
    /// Progress-Overlay in Seite anzeigen
    /// </summary>
    /// <param name="caption">Überschrift</param>
    /// <param name="description">Beschreibung</param>
    /// <param name="total">max. Anzahl Schritte</param>
    /// <param name="current">akt. Schritt</param>
    /// <param name="allowcancel">abbrechen erlauben</param>
    /// <returns></returns>
    IViewPageProgress ShowProgress(string caption, string description, int total, int current, bool allowcancel);

    /// <summary>
    /// Progress-Overlay schließen
    /// </summary>
    /// <param name="progress">zu schließende Overlay</param>
    void CloseProgress(IViewPageProgress progress);

    /// <summary>
    /// Validate the current Page and return a list of errors if invalid.
    /// </summary>
    /// <param name="errors"></param>
    /// <returns></returns>
    bool IsValid(ValidationErrorCollection errors);

    /// <summary>
    /// Pages des Editors neu laden...
    /// </summary>
    /// <param name="selectpage">nach dem Laden auszuwählende Seite. Wenn 0, wird Seite 1 ausgewählt.</param>
    void UpdatePages(object? selectpage);
}

/// <summary>
/// Erweiterungsmethoden für IViewPage
/// </summary>
public static class IViewPageEx
{
    /// <summary>
    /// Schließen der Seite soll verhindert werden.
    /// </summary>
    /// <param name="page"></param>
    /// <returns>True, wenn Seite offen bleiben soll.</returns>
    public static bool CancelClose(this IViewPage page)
    {
        if (page.CanClose)
            return false;
        if (page.ViewEditor == null)
            return false;
        return page.ViewEditor.ConfirmClosing() == eMessageBoxResult.No;
    }
}