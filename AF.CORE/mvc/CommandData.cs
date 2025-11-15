namespace AF.MVC;

/// <summary>
/// Speicher für die Befehlsausführung
/// </summary>
public class CommandArgs
{
    /// <summary>
    /// Seite, von der aus der Befehl zur Ausführung aufgerufen wird.
    /// </summary>
    public IViewPage? Page { get; set; }

    /// <summary>
    /// Dialog, von dem aus der Befehl zur Ausführung aufgerufen wird.
    /// </summary>
    public IDialogContainer? Dialog { get; set; }

    /// <summary>
    /// Editor, der die Quelle enthält.
    /// </summary>
    public IEditor? Editor { get; set; }

    /// <summary>
    /// Seite, von der aus der Befehl zur Ausführung aufgerufen wird.
    /// </summary>
    public object? ParentControl { get; set; }

    /// <summary>
    /// Kontext, aus dem der Befehl zur Ausführung aufgerufen wird
    /// </summary>
    public eCommandContext CommandContext { get; set; } = eCommandContext.EveryWhere;

    /// <summary>
    /// ein einzelnes Modell, mit dem innerhalb des Befehls gearbeitet wird
    /// </summary>
    public IModel? Model { get; set; }
        
    /// <summary>
    /// ein Array von Modellen, mit denen innerhalb des Befehls gearbeitet werden kann
    /// </summary>
    public IModel[]? SelectedModels { get; set; }

    /// <summary>
    /// Quelle des Befehls
    /// </summary>
    public object? CommandSource { get; set; }

    /// <summary>
    /// Container zur Übergabe beliebiger Daten.
    /// </summary>
    public object? Tag { get; set; }
}