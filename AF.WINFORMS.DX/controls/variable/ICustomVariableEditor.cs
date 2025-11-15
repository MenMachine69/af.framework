namespace AF.MVC;

/// <summary>
/// Interface eines Editors für eine CustomVariable
/// </summary>
public interface ICustomVariableEditor
{
    /// <summary>
    /// Editor aus Variable bestücken.
    /// </summary>
    /// <param name="variable">Variable</param>
    void SetVariable(VariableBase variable);
}