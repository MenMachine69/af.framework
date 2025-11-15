namespace AF.WINFORMS.DX;

/// <summary>
/// Model für den Designer
/// </summary>
public class WorkflowDesignerModel : Base
{

}

/// <summary>
/// Basis-Model für ein DesignerElement
/// </summary>
public class WorkflowDesignerModelElement : Base
{
    /// <summary>
    /// Name des Elements
    /// </summary>
    public string NAME { get; set; } = "<name>";

    /// <summary>
    /// Beschreibung des Elements
    /// </summary>
    public string DESCRIPTION { get; set; } = "<beschreibung>";

}

/// <summary>
/// Model für ein Action-Element
/// </summary>
public class WorkflowDesignerModelAction : WorkflowDesignerModelElement
{

}

/// <summary>
/// Model für ein Filter-Element
/// </summary>
public class WorkflowDesignerModelFilter : WorkflowDesignerModelElement
{

}
