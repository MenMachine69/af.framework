namespace AF.CORE;

/// <summary>
/// Variable, die eine GUID repräsentiert
/// </summary>
[Serializable]
public class VariableGuid : VariableBase
{
    /// <summary>
    /// Standard/Vorgabewert
    /// </summary>
    [AFBinding]
    [AFContext("Vorgabewert", Description = "GUID, die als Vorgabe angegeben ist.")]
    public string Default { get; set; } = Guid.Empty.ToString();
}