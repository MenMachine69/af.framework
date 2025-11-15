namespace AF.CORE;

/// <summary>
/// Controller für Variablen
/// </summary>
public class VariableController : Controller<Variable>
{
    #region Singleton
    private static VariableController? instance;

    /// <summary>
    /// Zugriff auf die Instanz des Controllers
    /// </summary>
    public static VariableController Instance => instance ??= new();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <exception cref="Exception">Wenn bereits ein Controller existiert (Instanz != null)</exception>
    public VariableController()
    {
        if (instance != null)
            throw new Exception("Controller VariableController existiert bereits. Statt einen neuen Controller zu erzeugen VariableController.Instance verwenden!");

        instance = this;
    }
    #endregion

    /// <summary>
    /// Eine Variable-Objekt speichern
    /// </summary>
    /// <param name="data">Parameter, der 'Model', das zu speichernde Objekt enthält</param>
    /// <returns>CommandResult-Objekt</returns>
    [AFCommand("VARIABLE SPEICHERN", CommandContext = eCommandContext.EveryWhere, CommandType = eCommand.Save)]
    public CommandResult CmdSave(CommandArgs data)
    {
        if (data.Model is not Variable model) return CommandResult.Error($"Es wurde kein Variable-Objekt übergeben.");

        ValidationErrorCollection errors = [];

        if (data is { CommandSource: IEditor editor })
            if (!editor.IsValid(errors)) return CommandResult.Error("Bitte überprüfen Sie die Eingaben.");
        else
            if (!model.IsValid(errors)) return CommandResult.Error("Bitte überprüfen Sie die Eingaben.");

        return CommandResult.Success("Variable gespeichert.");
    }

    /// <summary>
    /// Eine Variable-Objekt löschen
    /// </summary>
    /// <param name="data">Parameter, der 'Model', das zu löschende Objekt enthält</param>
    /// <returns>CommandResult-Objekt</returns>
    [AFCommand("VARIABLE LÖSCHEN", CommandContext = eCommandContext.EveryWhere, CommandType = eCommand.Delete)]
    public CommandResult CmdDelete(CommandArgs data)
    {
        if (data.Editor is IVariableConsumer consumer && data.Model is Variable variable)
        {
            if (AFCore.App.ShowQuestionYesNo($"VARIABLE LÖSCHEN\r\nMöchten Sie die Variable <b>{variable.VAR_NAME}</b> löschen?") == eMessageBoxResult.No) return CommandResult.None;

            consumer.Variables.Remove(variable);

            return CommandResult.Success("Variable gelöscht.");
        }

        return CommandResult.None;
    }
}