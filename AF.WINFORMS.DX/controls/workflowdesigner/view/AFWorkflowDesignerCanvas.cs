namespace AF.WINFORMS.DX;

/// <summary>
/// Zeichenfläche für den WorkflowDesigner
/// </summary>
public class AFWorkflowDesignerCanvas : AFDesignerCanvas
{
    private AFWorkflowDesignerElement? dragSource;
    private eWorkflowDragOutboundOrigin dragOrigin;

    /// <summary>
    /// Designer, der den Canvas enthält.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFWorkflowDesigner? WorkflowDesigner { get; set; }

    /// <summary>
    /// Ein Action-Element zum Designer hinzufügen
    /// </summary>
    /// <param name="action">Action-Element</param>
    /// <param name="point">Punkt, an dem das Element im Designer eingefügt werden soll</param>
    public void AddAction(WorkflowDesignerModelAction action, Point point)
    {
        AFWorkflowDesignerAction ctrl = new(action, point);
        ctrl.Location = point;
        ctrl.Size = new(ScaleDPI.ScaleHorizontal(300), ScaleDPI.ScaleVertical(60));
        ctrl.Id = Guid.NewGuid();

        // ctrl.BackColor = Color.Blue;
        // ctrl.Id = Guid.NewGuid();
        AddElement(ctrl, point: point);
    }

    /// <summary>
    /// Ein Filter-Element zum Designer hinzufügen
    /// </summary>
    /// <param name="filter">Filter-Element</param>
    /// <param name="point">Punkt, an dem das Element im Designer eingefügt werden soll</param>
    public void AddFilter(WorkflowDesignerModelFilter filter, Point point)
    {
        AFWorkflowDesignerFilter ctrl = new(filter, point);
        ctrl.Location = point;
        ctrl.Size = new(ScaleDPI.ScaleHorizontal(300), ScaleDPI.ScaleVertical(60));
        ctrl.Id = Guid.NewGuid();

        // ctrl.BackColor = Color.Blue;
        // ctrl.Id = Guid.NewGuid();
        AddElement(ctrl, point: point);
    }

    /// <summary>
    /// Begin eines DragDrop im Designer
    /// </summary>
    /// <param name="source">Quellelement</param>
    /// <param name="origin">Ausgangspunkt</param>
    public void StartDragDrop(AFWorkflowDesignerElement source, eWorkflowDragOutboundOrigin origin)
    {
        dragSource = source;
        dragOrigin = origin;
        dragSource.DoDragDrop(dragSource, DragDropEffects.Move);
    }

    /// <summary>
    /// Begin eines DragDrop im Designer
    /// </summary>
    /// <param name="target">Zielelement</param>
    public void FinishDragDrop(AFWorkflowDesignerElement target)
    {
        MsgBox.ShowInfoOk("DRAGDROP\r\nFinshed...");
        dragSource = null;
        dragOrigin = eWorkflowDragOutboundOrigin.Undefined;
    }

    /// <summary>
    /// Begin eines DragDrop im Designer
    /// </summary>
    public void CancelDragDrop()
    {
        if (dragSource != null)
        {
            dragSource = null;
            dragOrigin = eWorkflowDragOutboundOrigin.Undefined;
        }
    }
}

/// <summary>
/// Element zur Darstellung eines Filter-Bausteins im WorkflowDesigner
/// </summary>
[ToolboxItem(false)]
public class AFWorkflowDesignerFilter : AFWorkflowDesignerElement
{
    private WorkflowDesignerModelFilter _filter;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="filter">Filter-Model</param>
    /// <param name="point">Position im Canvas</param>
    public AFWorkflowDesignerFilter(WorkflowDesignerModelFilter filter, Point point) : base(filter)
    {
        _filter = filter;
    }
}

/// <summary>
/// Element zur Darstellung eines Action-Bausteins im WorkflowDesigner
/// </summary>
[ToolboxItem(false)]
public class AFWorkflowDesignerAction : AFWorkflowDesignerElement
{
    private WorkflowDesignerModelAction _action;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="action">Action-Model</param>
    /// <param name="point">Position im Canvas</param>
    public AFWorkflowDesignerAction(WorkflowDesignerModelAction action, Point point) : base(action)
    {
        _action = action;
    }
}
