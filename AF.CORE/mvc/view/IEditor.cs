namespace AF.MVC;

/// <summary>
/// Schnittstelle eines Editors zur Verwendung in der Benutzeroberfläche zur Anzeige/Bearbeitung eines 
/// Modells innerhalb eines IViewMaster oder eines eigenständigen IViewDialog.
/// </summary>
public interface IEditor
{
    /// <summary>
    /// current model
    /// </summary>
    IModel? Model { get; set; }

    /// <summary>
    /// Vom Editor benötigte Standardbreite (in Pixel).
    ///
    /// Der Wert wird bei High-DPI Bildschirmen automatisch skaliert.
    /// </summary>
    int DefaultEditorWidth { get; set; }

    /// <summary>
    /// Vom Editor benötigte Standardhöhe (in Pixel).
    ///
    /// Der Wert wird bei High-DPI Bildschirmen automatisch skaliert.
    /// In der Regel wird die Höhe automatisch anhand des Inhalts ermittelt.
    /// </summary>
    int DefaultEditorHeight { get; set; }
    
    /// <summary>
    /// Methode, die von nach EndLayout aufgerufen wird.
    /// </summary>
    /// <param name="sender">Control, dass den Aufruf verursacht hat</param>
    void AfterLayout(object sender);

    /// <summary>
    /// Validate the current Model and return a list of errors if invalid.
    /// </summary>
    /// <param name="errors"></param>
    /// <returns></returns>
    bool IsValid(ValidationErrorCollection errors);

    /// <summary>
    /// Registriert ein Control im Editor (z.B. zum Setzen der Appearance-Eigenschaften)
    /// </summary>
    /// <param name="control"></param>
    void RegisterControl(object control);

    /// <summary>
    /// Registriertes Control abmelden
    /// </summary>
    /// <param name="control">abzumeldendes Control, dass vorher mit RegisterControl registriert wurde</param>
    void UnRegisterControl(object control);

    /// <summary>
    /// Der Editor unterstützt 'Pages' (einzelne Seiten, auf die die Informationen verteilt werden)
    /// </summary>
    bool SupportPages { get; set; }

    /// <summary>
    /// Titelleiste unterdrücken, wenn der Editor in einer IViewPage dargestellt wird.
    /// </summary>
    bool HideCaption { get; set; }

    /// <summary>
    /// Stellt die Seiten zur verfügung, wenn SupportPages = true
    /// </summary>
    /// <returns></returns>
    EditorPageInfo[] GetPages();

    /// <summary>
    /// Seite aktivieren (wenn SupportPages = true)
    /// </summary>
    /// <param name="pageindex">Index der Seite (aus EditorPageInfo PageIdentifier)</param>
    void SelectPage(int pageindex);

    /// <summary>
    /// Seite, in der der Editor gerade als Master-Editor angezeigt wird.
    ///
    /// Kann NULL sein.
    /// </summary>
    IViewPage? ParentPage { get; set; }

    /// <summary>
    /// Aktueller ViewContext des Editors.
    /// </summary>
    string ViewContext { get; set; }

    /// <summary>
    /// Datenquelle aktualisieren (z.B. nach Reload)
    /// </summary>
    void RefreshDatasource();

    /// <summary>
    /// Schließen der Seite durch Nutzer bestätigen
    /// </summary>
    /// <returns>Yes wenn Seite geschlossen werden soll. No wenn Seite offen bleiben soll.</returns>
    eMessageBoxResult ConfirmClosing();
}

/// <summary>
/// Informationen zu einer Page, die der Editor zur Verfügung stellt.
/// </summary>
public struct EditorPageInfo
{
    /// <summary>
    /// Konstruktor
    /// </summary>
    public EditorPageInfo() { }

    /// <summary>
    /// Überschrift
    /// </summary>
    public string Caption { get; set; } = "";

    /// <summary>
    /// Bild/Symbol-Index
    /// </summary>
    public int? Image { get; set; }

    /// <summary>
    /// Ident für die Page
    /// </summary>
    public object? PageIdentifier { get; set; }
}