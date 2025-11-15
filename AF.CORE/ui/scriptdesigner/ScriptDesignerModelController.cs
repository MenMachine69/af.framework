namespace AF.DATA;

/// <summary>
/// Controller für 'ScriptDesignerModel'
/// </summary>
public class ScriptDesignerModelController : Controller<ScriptDesignerModel>
{
    #region Singleton
    private static ScriptDesignerModelController? instance;

    /// <summary>
    /// Zugriff auf die Instanz des Controllers
    /// </summary>
    public static ScriptDesignerModelController Instance => instance ??= new();

    /// <summary>
    /// Constructor
    /// </summary>
    public ScriptDesignerModelController() { }
    #endregion
}