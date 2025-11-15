namespace AF.CORE;

/// <summary>
/// Interface eines Editors/Controls, dass Variablen konsumiert
/// </summary>
public interface IVariableConsumer
{
    /// <summary>
    /// Datenquelle/Liste, die die Variablen enthält
    /// </summary>
    BindingList<Variable> Variables { get; }
}