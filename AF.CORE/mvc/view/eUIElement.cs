namespace AF.MVC;

/// <summary>
/// verfügbare UI-Elemente
/// </summary>
public enum eUIElement
{
    /// <summary>
    /// IViewPage Container
    /// </summary>
    ViewPage,
    /// <summary>
    /// Kopfzeile zur Anzeige in der Detailansicht (z.B. ein Filterdialog)
    /// </summary>
    HeaderDetail,
    /// <summary>
    /// Fußzeile zur Anzeige in der Detailansicht (z.B. ein Zusammenfassungsdialog)
    /// </summary>
    FooterDetail,
    /// <summary>
    /// Seitenleiste zur Anzeige in der Detailansicht (z.B. ein Dialog mit weiteren Details zum aktuell ausgewählten Detail)
    /// </summary>
    PluginDetail,
    /// <summary>
    /// Detailanzeige im Browser (z.B. ein Dialog mit weiteren Details zum aktuell im Browser ausgewählten Modell)
    /// </summary>
    BrowserDetail,
    /// <summary>
    /// Filteranzeige im Browser (z.B. ein Dialog mit Optionen wie 'archiviert anzeigen')
    /// </summary>
    BrowserFilter,
    /// <summary>
    /// Combobox zur Auswahl eines Modells in einem Editor/Dialog
    /// </summary>
    Combobox,
    /// <summary>
    /// Kombinationsfeld zur Auswahl eines Modells innerhalb eines Grids (Spalteneditor)
    /// </summary>
    ComboboxGrid,
    /// <summary>
    /// Steuerelement zur Anzeige als Overlay im Viewmanager
    /// </summary>
    Overlay,
    /// <summary>
    /// Editor zur Bearbeitung des Modells
    /// </summary>
    Editor,
    /// <summary>
    /// Ansicht der Details (z.B. ein Grid)
    /// </summary>
    Detail,
    /// <summary>
    /// Control zur Anzeige als 'Property'-Dialog
    /// </summary>
    PropertyDialog,
    /// <summary>
    /// Ansicht von Informationen zu einem Model.
    /// 
    /// Kann z.B. als TAB-Page in DetailPlugins verwendet werden.
    /// </summary>
    Info
}