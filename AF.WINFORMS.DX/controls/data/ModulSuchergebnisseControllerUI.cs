using AF.MVC;
using crm.ui;

namespace AF.WINFORMS.DX;

/// <summary>
/// UI Controller für die Anzeige von Suchergebnissen
/// </summary>
public class ModulSearchResultsControllerUI : Controller<ModulSearchResults>, IControllerUI<ModulSearchResults>
{
    private static ModulSearchResultsControllerUI? instance;

    /// <summary>
    /// Zugriff auf die Instanz des Controllers
    /// </summary>
    public static ModulSearchResultsControllerUI Instance => instance ??= new();

    /// <summary>
    /// Gibt an, ob mehrere Seiten für das gleiche Model geöffnet werden können.
    /// </summary>
    public bool AllowMultiplePages => false;

    /// <summary>
    /// Detail-Ansicht abschalten
    /// </summary>
    public override ePageDetailMode DetailViewMode => ePageDetailMode.NoDetails;
        
    /// <summary>
    /// Symbol
    /// </summary>
    public override object TypeImage => UI.GetObjectImage(ObjectImages.handshake);

    /// <inheritdoc cref="IControllerUI.GetUIElement(eUIElement, Type?, Type?, IViewPage?)" />
    public override IUIElement? GetUIElement(eUIElement type, Type? mastertype = null, Type? detailtype = null, IViewPage? page = null)
    {
        if (type == eUIElement.Editor) return new ModulSearchResultsEditor();

        return null;
    }

    /// <inheritdoc cref="IControllerUI.GetUIElementType(eUIElement, Type?, Type?, IViewPage?)" />
    public override Type? GetUIElementType(eUIElement type, Type? mastertype = null, Type? detailtype = null, IViewPage? page = null)
    {
        if (type == eUIElement.Editor) return typeof(ModulSearchResultsEditor);

        return null;
    }

    /// <summary>
    /// Diese Methode wird aufgerufen, nachdem ein UI-Element erstellt wurde. Hier können Sie Anpassungen am UIElement vornehmen (z.B. Gittereinstellungen etc.) 
    /// </summary>
    /// <param name="element">das UIElement</param>
    /// <param name="page">die Ansicht, die das Element enthält</param>
    public void SetupUIElement(IUIElement element, IViewPage page)
    {
        if (this is IControllerUI<ModulSearchResults> uictrl)
            uictrl.SetupUIElement(element, page);
    }
}

