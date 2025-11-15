namespace AF.MVC;

/// <summary>
/// Schnittstelle für Controller, die UI-Funktionen unterstützen
/// </summary>
public interface IControllerUI : IController
{
    /// <summary>
    /// Gibt an, ob mehrere Pages für das gleiche Model geöffnet werden können.
    /// </summary>
    bool AllowMultiplePages { get; }

    /// <summary>
    /// Gibt den Typ des erforderlichen UI-Elements für den angegebenen Mastertyp zurück.
    /// </summary>
    /// <param name="mastertype">Mastertyp</param>
    /// <param name="detailtype">Typ der Details</param>
    /// <param name="type">Typ des erforderlichen UI-Elements</param>
    /// <param name="page">Page, die das Element benötigt. 
    /// Bei einigen UI-Elementen muss hier das Page-Objekt übergeben werden! (Detail, PluginDetail, FooterDetail, HeaderDetail, Editor)(</param>
    /// <returns>der Typ des UI-Elements oder NULL, wenn kein Element für den Typ verfügbar ist</returns>
    Type? GetUIElementType(eUIElement type, Type? mastertype = null, Type? detailtype = null, IViewPage? page = null);

    /// <summary>
    /// Erstellt ein UI-Element eines bestimmten Typs für den angegebenen MasterTyp.
    /// </summary>
    /// <param name="mastertype">MasterTyp</param>
    /// <param name="detailtype">Typ der Details</param>
    /// <param name="type">Typ des gewünschten UI-Elements</param>
    /// <param name="page">Page, die das Element benötigt. 
    /// Bei einigen UI-Elementen muss hier das Page-Objekt übergeben werden! (Detail, PluginDetail, FooterDetail, HeaderDetail, Editor)(</param>
    /// <returns>das UI-Element oder NULL, wenn kein Element für den Typ verfügbar ist</returns>
    IUIElement? GetUIElement(eUIElement type, Type? mastertype = null, Type? detailtype = null, IViewPage? page = null);

    /// <summary>
    /// Setup des Elements.
    ///
    /// Diese Methode wird aufgerufen, nachdem ein UI-Element erstellt wurde. Hier können Sie Anpassungen am UIElement vornehmen (z.B. Gittereinstellungen etc.) 
    /// </summary>
    /// <param name="element">Element, dass konfiguriert wird</param>
    /// <param name="page">Page in der das Element dargestellt wird</param>
    /// <param name="mastertype">MasterType</param>
    /// <param name="detailtype">Typ der Details</param>
    void SetupUIElement(IUIElement element, IViewPage page, Type? mastertype = null, Type? detailtype = null);

    /// <summary>
    /// Liefert eine eindeutige ID für sas DetailView eines bestimmten Masters.
    ///
    /// Diese ID wird verwendet, um Einstellungen des DetailViews zu persistieren.
    /// </summary>
    /// <param name="detailtype">Typ der Details</param>
    /// <param name="master">Typ des Masters, für den das DetailView angezeigt wird.</param>
    /// <returns>die GUID/ID des DetailViews</returns>
    Guid GetDetailIdentifier(Type master, Type? detailtype);

    /// <summary>
    /// Liefert den Typ des benötigten CustomEditors für das angegebene UI-Element
    /// </summary>
    /// <param name="property">Eigenschaft/Spalte</param>
    /// <param name="element">Element, dass den Editor benötigt (Grid, Einstellungsdialog etc.)</param>
    /// <returns>Typ des Editors oder NULL</returns>
    Type? GetCustomEditorType(PropertyDescription property, eUIElement element);

    /// <summary>
    /// Liefert den benötigten CustomEditors für das angegebene UI-Element
    /// </summary>
    /// <param name="property">Eigenschaft/Spalte</param>
    /// <param name="element">Element, dass den Editor benötigt (Grid, Einstellungsdialog etc.)</param>
    /// <returns>Typ des Editors oder NULL</returns>
    object? GetCustomEditor(PropertyDescription property, eUIElement element);

    /// <summary>
    /// Template für die Darstellung eines Suchtreffers in den Suchergebnissen.
    /// </summary>
    AFHtmlTemplate SearchHitHtmlTemplate { get; }

    /// <summary>
    /// Template für die Darstellung der Schaltfläche 'Mehr' in den Suchergebnissen.
    /// </summary>
    AFHtmlTemplate SearchMoreHtmlTemplate { get; }
}

/// <summary>
/// Schnittstelle für Controller, die UI-Funktionen unterstützen
/// </summary>
public interface IControllerUI<TModel> : IControllerUI, IController<TModel> where TModel : class, IDataObject, new()
{
    
}

/// <summary>
/// Erweiterungen für IControllerUI
/// </summary>
public static class IControllerUIEx
{
    /// <summary>
    /// Grid-Setup modifizieren (Commands des Controllers für Standardaktionen (Add, Delete, Edit, Goto und ShowDetails) in das Setup integrieren)
    /// </summary>
    /// <param name="controller">Controller, der durch diese Methode erweitert wird</param>
    /// <param name="setup">setup to modify</param>
    public static void ModifyGridSetup(this IControllerUI controller, AFGridSetup setup)
    {
        setup.CmdAdd = controller.GetCommand(eCommand.New, eCommandContext.GridButton, true);
        setup.CmdDelete = controller.GetCommand(eCommand.Delete, eCommandContext.GridButton, true);
        setup.CmdEdit = controller.GetCommand(eCommand.Edit, eCommandContext.GridButton, true);
        setup.CmdGoto = controller.GetCommand(eCommand.Goto, eCommandContext.GridButton, true);
        setup.CmdShowDetail = controller.GetCommand(eCommand.ShowDetails, eCommandContext.GridButton, true);
    }

    /// <summary>
    /// Diese Methode wird aufgerufen, nachdem ein UI-Element erstellt wurde. Hier können Sie Anpassungen am UIElement vornehmen (z.B. Gittereinstellungen etc.) 
    /// </summary>
    /// <param name="element">das UIElement</param>
    /// <param name="controller">Controller, der durch diese Methode erweitert wird</param>
    /// <param name="page">die Ansicht, die das Element enthält</param>
    public static void SetupUIElement<TModel>(this IControllerUI<TModel> controller, IUIElement element, IViewPage page) where TModel : class, IDataObject, new()
    {

    }

    /// <summary>
    /// Gibt den Typ des gewünschten UI-Elements zurück
    /// </summary>
    /// <param name="type">Typ des benötigten UI-Elements</param>
    /// <param name="controller">Controller, der durch diese Methode erweitert wird</param>
    /// <returns>der Typ des UI-Elements oder NULL, wenn kein Element für den Typ verfügbar ist</returns>
    public static Type? GetUIElementType<TModel>(this IControllerUI<TModel> controller, eUIElement type) where TModel : class, IDataObject, new()
    {
        return null;
    }

    /// <summary>
    /// Gibt den Typ des erforderlichen UI-Elements für den angegebenen Mastertyp zurück.
    /// </summary>
    /// <param name="controller">Controller, der durch diese Methode erweitert wird</param>
    /// <param name="mastertype">Mastertyp</param>
    /// <param name="type">Typ des erforderlichen UI-Elements</param>
    /// <returns>der Typ des UI-Elements oder NULL, wenn kein Element für den Typ verfügbar ist</returns>
    public static Type? GetUIElementType<TModel>(this IControllerUI<TModel> controller, eUIElement type, Type mastertype) where TModel : class, IDataObject, new()
    {
        return null;
    }

    /// <summary>
    /// Erzeugt ein UI-Element eines bestimmten Typs.
    /// </summary>
    /// <param name="controller">Controller, der durch diese Methode erweitert wird</param>
    /// <param name="type">Typ des gewünschten UI-Elements</param>
    public static IUIElement? GetUIElement<TModel>(this IControllerUI<TModel> controller, eUIElement type) where TModel : class, IDataObject, new()
    {
        return null;
    }

    /// <summary>
    /// Erstellt ein UI-Element eines bestimmten Typs für den angegebenen Mastertyp.
    /// </summary>
    /// <param name="controller">Controller, der durch diese Methode erweitert wird</param>
    /// <param name="mastertype">Mastertyp</param>
    /// <param name="type">Typ des gewünschten UI-Elements</param>
    /// <returns>das UI-Element oder NULL, wenn kein Element für den Typ verfügbar ist</returns>
    public static IUIElement? GetUIElement<TModel>(this IControllerUI<TModel> controller, eUIElement type, Type mastertype) where TModel : class, IDataObject, new()
    {
        return null;
    }
}