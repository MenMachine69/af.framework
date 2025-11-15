namespace AF.MVC;

/// <summary>
/// Schnittstelle für Klassen, die Modelle in einer MVC-Anwendung darstellen.
///
/// Für Modelle, die in der Datenbank als Tabellen oder Views abgebildet werden, verwenden Sie ITableModel oder IViewModel.
/// </summary>
public interface IModel : IBindable, IDatasource
{
    /// <summary>
    /// Eindeutige ID des Modells.
    ///
    /// Jedes Modell muss eine eindeutige ID haben.
    /// Bei Datenbankobjekten ist dies der Primärschlüssel des Datensatzes in der Datenbank.
    /// </summary>
    public Guid PrimaryKey { get; }

    /// <summary>
    /// eine Beschreibung des Modells
    ///
    /// die Verwendung von html-Tags ist erlaubt
    /// </summary>
    public string? ModelDescription { get; }

    /// <summary>
    /// gibt den Link für das aktuelle Modell zurück
    /// </summary>
    public ModelLink ModelLink { get; }

    /// <summary>
    /// gibt den ModelInfo für das aktuelle Modell zurück
    /// </summary>
    public ModelInfo ModelInfo { get; }
}

/// <summary>
/// Eventargumente für ein Model bezogene Ereignisse
/// </summary>
public sealed class ModelEventArgs : EventArgs
{
    /// <summary>
    /// Datenobjekt, dass die Nachricht schickt
    /// </summary>
    public IModel? Data { get; set; }
}

/// <summary>
/// Eventargumente für ein Model bezogene Ereignisse, die abgebrochen werden können
/// </summary>
public sealed class ModelCancelEventArgs : CancelEventArgs
{
    /// <summary>
    /// Datenobjekt, dass die Nachricht schickt
    /// </summary>
    public IModel? Data { get; set; }
}
