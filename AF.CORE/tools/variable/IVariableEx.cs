namespace AF.CORE;

/// <summary>
/// Erweiterungsmethoden für IVariable
/// </summary>
public static class IVariableEx
{
    /// <summary>
    /// Setzt die Variablendetails in einer IVariable.
    /// </summary>
    /// <param name="variable"></param>
    /// <param name="value"></param>
    public static void SetVariable(this IVariable variable, VariableBase value)
    {
        variable.VAR_STORAGE ??= new();
        variable.VAR_STORAGE.Store(value);
    }

    /// <summary>
    /// Variablendetails 
    /// </summary>
    /// <param name="variable"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static VariableBase GetVariable(this IVariable variable)
    {
        if (variable.VAR_STORAGE.VariableType != variable.VAR_TYP) 
            variable.VAR_STORAGE.VariableType = variable.VAR_TYP;

        return variable.VAR_STORAGE.Load();
    }
}