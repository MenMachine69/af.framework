using AF.MVC;
using DevExpress.XtraGrid.Views.Grid;

namespace AF.WINFORMS.DX;

/// <summary>
/// UI-Controller der Klasse AF.CORE.VariableBase
/// </summary>
public class VariableBaseControllerUI : VariableBaseController, IControllerUI<VariableBase>
{
    private static VariableBaseControllerUI? instance;

    /// <summary>
    /// Zugriff auf die Instanz des Controllers (Singleton).
    /// </summary>
    public new static VariableBaseControllerUI Instance => instance ??= new();

    private VariableBaseControllerUI() { }

    /// <summary>
    /// Gibt an, ob mehrere Pages für das gleiche Model geöffnet werden können.
    /// </summary>
    public bool AllowMultiplePages => false;

    /// <summary>
    /// Bild (SVG, Bitmap usw.), das den Typ in einer Benutzeroberfläche darstellt.
    /// </summary>
    public override object TypeImage => UI.GetObjectImage(ObjectImages.tag);


    /// <summary>
    /// Eintrag in der VariableList-Liste löschen
    /// </summary>
    /// <param name="data">Parameter, der 'Model', das zu löschende Objekt enthält</param>
    /// <returns>CommandResult-Objekt</returns>
    [AFCommand("LISTENEINTRAG LÖSCHEN", CommandContext = eCommandContext.Nowhere, CommandType = eCommand.Other)]
    public CommandResult CmdDeleteListEntry(CommandArgs data)
    {
        if (data.CommandSource is not GridView grid) return CommandResult.None;

        if (data.Model is not VariableListEntry entry) return CommandResult.None;

        if (grid.GridControl.DataSource is not BindingList<VariableListEntry> list) return CommandResult.None;

        list.Remove(entry);

        return CommandResult.None;
    }

    /// <summary>
    /// Liefert den Type des Editors passend zum Variablentypen.
    /// </summary>
    /// <param name="type">Variablentyp</param>
    /// <returns>Typ des Editor</returns>
    /// <exception cref="ArgumentOutOfRangeException">unbekannter Typ</exception>
    public Type GetEditorType(int type)
    {
        switch (type)
        {
            case (int)eVariableType.Bool:
                return typeof(AFVariableEditBool);
            case (int)eVariableType.String:
                return typeof(AFVariableEditString);
            case (int)eVariableType.DateTime:
                return typeof(AFVariableEditDateTime);
            case (int)eVariableType.Decimal:
                return typeof(AFVariableEditDecimal);
            case (int)eVariableType.Int:
                return typeof(AFVariableEditInteger);
            case (int)eVariableType.List:
                return typeof(AFVariableEditList);
            case (int)eVariableType.Model:
                return typeof(AFVariableEditModel);
            case (int)eVariableType.Query:
                return typeof(AFVariableEditQuery);
            case (int)eVariableType.Script:
                return typeof(AFVariableEditScript);
            case (int)eVariableType.Memo:
                return typeof(AFVariableEditMemo);
            case (int)eVariableType.RichText:
                return typeof(AFVariableEditRichText);
            case (int)eVariableType.Guid:
                return typeof(AFVariableEditGuid);
            case (int)eVariableType.Formula:
                return typeof(AFVariableEditFormula);
            case (int)eVariableType.Year:
                return typeof(AFVariableEditYear);
            case (int)eVariableType.Month:
                return typeof(AFVariableEditMonth);
            case (int)eVariableType.Section:
                return typeof(AFVariableEditSection);
            default:
            {
                if (!CustomTypes.TryGetValue(type, out var customtype)) throw new Exception($"Unbekannter Variablentyp {type}.");

                return customtype.VariableEditorConfigType;
            }
        }
    }
}
