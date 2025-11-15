using AF.MVC;
using DevExpress.XtraGrid.Views.Grid;

namespace AF.WINFORMS.DX;

/// <summary>
/// Erweiterung der Funktionen von XtraGrid- und XtraPivot-Controls (Basisklasse)
/// </summary>
[ToolboxItem(false)]
[SupportedOSPlatform("windows")]
public class AFGridPivotExtenderBase : AFVisualComponentBase
{
    /// <summary>
    /// Zugriff auf die Menüeinträge
    /// </summary>
    protected List<AFPopupMenuEntry> MenuEntries { get; } = [];

    /// <summary>
    /// Zugriff auf die Default-Menüeinträge
    /// </summary>
    protected List<AFPopupMenuEntry> DefaultMenuEntries { get; } = [];

    /// <summary>
    /// Menüpunkte für ein PopupMenu aus einem IController erzeugen
    /// </summary>
    /// <param name="controller">Controller, der die Commands zur Verfügung stellt</param>
    /// <param name="context">Context der Commands, die hinzugefügt werden sollen (default = eCommandContext.DetailContext)</param>
    /// <param name="menuType">Typ des Menüs, zu dem die Einträge hinzugefügt werden (default = GridMenuType.Row)</param>
    public void AddMenuEntrys(IController controller, eCommandContext context = eCommandContext.DetailContext, GridMenuType menuType = GridMenuType.Row)
    {
        var cmds = controller.GetCommands(context, string.Empty);

        if (cmds.Length < 1) return;

        foreach (var cmd in cmds)
        {
            if (cmd.Command == null) continue;

            AddMenuEntry(new(cmd.Caption, menuType, cmd.Command));
        }
    }

    /// <summary>
    /// Einen Menüpunkt für ein PopupMenu hinzufügen
    /// </summary>
    /// <param name="entry">Definition des Menüpunkts</param>
    public void AddMenuEntry(AFPopupMenuEntry entry)
    {
        MenuEntries.Add(entry);
    }

    /// <summary>
    /// Alle Menüpunkte entfernen
    /// </summary>
    public void ResetMenuEntrys()
    {
        MenuEntries.Clear();
    }
}