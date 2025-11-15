namespace AF.DATA;

/// <summary>
/// Controller für 'DocumentDesignerModel'
/// </summary>
public class DocumentDesignerModelController : Controller<DocumentDesignerModel>
{
    #region Singleton
    private static DocumentDesignerModelController? instance;

    /// <summary>
    /// Zugriff auf die Instanz des Controllers
    /// </summary>
    public static DocumentDesignerModelController Instance => instance ??= new();

    /// <summary>
    /// Constructor
    /// </summary>
    public DocumentDesignerModelController() { }
    #endregion
}