namespace AF.MVC;

/// <summary>
/// Basisklasse für exportierbare Models
/// </summary>
[Serializable]
public abstract class ModelExportBase
{
    /// <summary>
    /// ID des Models
    /// </summary>
    public Guid PrimaryKey { get; set; } = Guid.Empty;
}