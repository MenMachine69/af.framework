namespace AF.DATA;

/// <summary>
/// Controller für 'SnippetDesignerModel'
/// </summary>
public class CodeSnippetDesignerModelController : Controller<CodeSnippetDesignerModel>
{
    #region Singleton
    private static CodeSnippetDesignerModelController? instance;

    /// <summary>
    /// Zugriff auf die Instanz des Controllers
    /// </summary>
    public static CodeSnippetDesignerModelController Instance => instance ??= new();

    /// <summary>
    /// Constructor
    /// </summary>
    public CodeSnippetDesignerModelController() { }
    #endregion
}