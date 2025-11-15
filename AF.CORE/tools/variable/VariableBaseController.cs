namespace AF.CORE;

/// <summary>
/// Controller der Klasse VariableBase.
/// </summary>
public class VariableBaseController : Controller<VariableBase>
{
    private static VariableBaseController? instance;
    private readonly Dictionary<int, VariableCustomType> customvariables = [];

    /// <summary>
    /// Zugriff auf die Instanz des Controllers
    /// </summary>
    public static VariableBaseController Instance => instance ??= new();

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <exception cref="Exception">Wenn bereits ein Controller existiert (Instanz != null)</exception>
    public VariableBaseController()
    {
        if (instance != null)
            throw new Exception("Controller VariableBaseController existiert bereits. Statt einen neuen Controller zu erzeugen VariableBaseController.Instance verwenden!");

        instance = this;
    }

    /// <summary>
    /// Custom-Variabletypen
    /// </summary>
    public Dictionary<int, VariableCustomType> CustomTypes => customvariables;

    /// <summary>
    /// Registriert einen neuen, anwendungsspezifischen Variablentypen.
    /// </summary>
    /// <param name="variableCustomType"></param>
    /// <exception cref="ArgumentException"></exception>
    public void RegisterCustomVariable(VariableCustomType variableCustomType)
    {
        if (customvariables.TryGetValue(variableCustomType.VariableTypIndex, out var vorhanden)) 
            throw new ArgumentException($"Der VariablentypIndex {variableCustomType.VariableTypIndex} wird bereist für den Typ {vorhanden.VariableTypeName} verwendet.");

        if (customvariables.Values.FirstOrDefault(v => v.VariableTypeName.Equals(variableCustomType.VariableTypeName, StringComparison.OrdinalIgnoreCase)) != null)
            throw new ArgumentException($"Ein Variablentyp mit dem Namen {variableCustomType.VariableTypeName} ist bereist vorhanden.");


        customvariables.Add(variableCustomType.VariableTypIndex, variableCustomType);
    }
}