namespace AF.MVC;

/// <summary>
/// Schnittstelle, die eine ModelInfo beschreibt.
///
/// ModelInfos werden z.B. zur Anzeige von Suchtreffern verwendet.
/// </summary>
public class ModelInfo : AF.CORE.Base
{
    /// <summary>
    /// Constructor
    /// </summary>
    public ModelInfo() { }

    /// <summary>
    /// Stellt den zum ModelInfo passenden Link bereit, um z.B. das Model direkt öffnen zu können.
    /// </summary>
    public ModelLink Link => new(Id, Caption, ModelType);

    /// <summary>
    /// ID des Models
    /// </summary>
    public Guid Id { get; set; } = Guid.Empty;

    /// <summary>
    /// Bezeichnung/Titel des Models
    /// </summary>
    [AFBinding]
    public string Caption { get; set; } = string.Empty;

    /// <summary>
    /// Beschreibung des Models
    /// </summary>
    [AFBinding]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Typ des Models
    /// </summary>
    public Type ModelType { get; set; } = typeof(Nullable);

    /// <summary>
    /// Dictionary der Daten für die Modelbeschreibung
    /// </summary>
    public Dictionary<string, object?> Data { get; set; } = [];
}