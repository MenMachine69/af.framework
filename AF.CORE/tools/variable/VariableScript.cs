namespace AF.CORE;

/// <summary>
/// eine Variable, deren Wert aus einem ausführbaren Skript stammt
/// </summary>
[Serializable]
public class VariableScript : VariableBase
{
    /// <summary>
    /// ID des Skripts im ScriptManager.
    /// 
    /// Dieses Skript wird ausgeführt, wenn die Variable benötigt wird
    /// </summary>
    [AFBinding]
    [AFContext("Script", Description = "Script, der zur Wertermittlung verwendet wird.")]
    public Guid ScriptId { get; set; }

    
}