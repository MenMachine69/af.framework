namespace AF.DATA;

/// <summary>
/// Controller für 'LibraryDocumentDesignerModel'
/// </summary>
public class LibraryDocumentDesignerModelController : Controller<LibraryDocumentDesignerModel>
{
    #region Singleton
    private static LibraryDocumentDesignerModelController? instance;

    /// <summary>
    /// Zugriff auf die Instanz des Controllers
    /// </summary>
    public static LibraryDocumentDesignerModelController Instance => instance ??= new();

    /// <summary>
    /// Constructor
    /// </summary>
    public LibraryDocumentDesignerModelController() { }
    #endregion
}