using AF.MVC;
using DevExpress.XtraGrid.Views.Grid;

namespace AF.WINFORMS.DX;

/// <summary>
/// Zusätzlicher Eintrag in einem Grid-PopupMenu
/// </summary>
public class AFPopupMenuEntry : AFMenuEntry
{
    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="caption">Caption des Menüs. Verwenden Sie \ um Untermenüs zu erzeugen.</param>
    /// <param name="onclick">Aktion die beim Klick auf das Menü aufgerufen wird.</param>
    public AFPopupMenuEntry(string caption, Func<CommandArgs, CommandResult> onclick) : base(caption)
    {
        ClickAction = onclick;
    }

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="caption">Caption des Menüs. Verwenden Sie \ um Untermenüs zu erzeugen.</param>
    /// <param name="onclick">Aktion die beim Klick auf das Menü aufgerufen wird.</param>
    /// <param name="menuType">Menutyp für den der Menüpunkt verwendet werden soll</param>
    public AFPopupMenuEntry(string caption, GridMenuType menuType, Func<CommandArgs, CommandResult> onclick) : base(caption)
    {
        ClickAction = onclick;
        MenuType = menuType;
    }

   
    /// <summary>
    /// Menü, in dem der Eintrag angezeigt werden soll
    /// </summary>
    [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Category("UI")]
    public GridMenuType MenuType { get; set; } = GridMenuType.Column;

    /// <summary>
    /// Name der View (leer = alle Views des Grids)
    /// </summary>
    [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Category("UI")]
    public string? ViewName { get; set; }

    /// <summary>
    /// Aktion die beim Klick auf das Menü aufgerufen wird.
    ///
    /// Das sendende GridView wird in CommandData.CommandSource übergeben.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Func<CommandArgs, CommandResult> ClickAction { get; } 
}