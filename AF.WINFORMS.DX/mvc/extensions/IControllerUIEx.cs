using AF.MVC;
using DevExpress.XtraBars;

namespace AF.WINFORMS.DX;

/// <summary>
/// Erweiterungsmethoden für IControllerUI-Controller
/// </summary>
public static class IControllerUIEx
{
    /// <summary>
    /// Liefert, wenn möglich ein Popup-Menu mit Commands zur Bearbeitung von Elementen, die der Controller verwaltet.
    /// </summary>
    /// <param name="controller">erweiterter Controller</param>
    /// <param name="barmanager">Barmanger, der für das Menü verwendet wird (z.B. IViewPage.BaraManager)</param>
    /// <param name="onclick">wird ausgeführt, wenn der Benutzer einen Menüpunkt anklickt - sender ist das AFCommand-Objekt</param>
    /// <param name="commandcontext">Kontext, dessen Commands benötigt werden (Standard ist DetailContext)</param>
    /// <param name="viewcontext">Kontext des Views/der Anzeige um nur bestimmte, zur aktuellen Anzeige passende Commands anzuzeigen (Standard ist leer = alle anzeige)</param>
    /// <param name="model">Datenobjekt, für das das PopupMenu gelten soll</param>
    /// <returns>das passende PopupMenu</returns>
    public static PopupMenu? GetDetailsPopupMenu(this IControllerUI controller,  BarManager barmanager, ItemClickEventHandler onclick, eCommandContext commandcontext = eCommandContext.DetailContext, string viewcontext = "", IModel? model = null)
    {
        // ReSharper disable once CoVariantArrayConversion
        IMenuEntry[] commands = controller.GetCommands(commandcontext, viewcontext, model);

        if (commands.Length < 1) return null;

        PopupMenu menu = new();
        if (commandcontext == eCommandContext.DetailContext)
            barmanager.BoundCommandsToMenu(menu, commands.Where(c => c.CommandType == eCommand.Other || c.CommandType == eCommand.Goto).ToArray(), onclick);
        else
            barmanager.BoundCommandsToMenu(menu, commands.Where(c => c.CommandType == eCommand.Other).ToArray(), onclick);

        menu.Manager = barmanager;

        return menu;
    }
}

