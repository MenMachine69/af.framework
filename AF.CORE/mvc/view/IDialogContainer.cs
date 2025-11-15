namespace AF.MVC;

/// <summary>
/// Schnittstelle eines Containers, der Dialoge (PropertyDialog, SettingsDialog) darstellen kann.
/// </summary>
public interface IDialogContainer
{
    /// <summary>
    /// in geöffnete/aufgefaltete Darstellung wechseln
    /// </summary>
    int Expand();

    /// <summary>
    /// in geschlossene/zusammengefaltete Darstellung wechseln
    /// </summary>
    int Collapse();

    /// <summary>
    /// Command, das ausgeführt wird, wenn der Dialog bestätigt wird.
    /// </summary>
    Func<CommandArgs, CommandResult>? CommandExecute { get; set; }

    /// <summary>
    /// Command, das ausgeführt wird, wenn der Dialog abgebrochen wird.
    /// </summary>
    Func<CommandArgs, CommandResult>? CommandCancel { get; set; }

    /// <summary>
    /// Zugriff auf das Model, dass im Dialog bearbeitet wird
    /// </summary>
    IModel? Model { get; }
}