namespace AF.WINFORMS.DX;

/// <summary>
/// Basisklasse der typabhängigen Editoren für Variablen
/// </summary>
[DesignerCategory("Code")]
public class AFVariableEditBase : AFEditor
{
    private WeakEvent<EventHandler<EventArgs>>? variablePropertiesChanged;

    /// <summary>
    /// Variablendetails können die Variable über dieses Event benachrichtigen, 
    /// so dass die Variable reagieren kann.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public event EventHandler<EventArgs> OnVariablePropertiesChanged
    {
        add
        {
            variablePropertiesChanged ??= new();
            variablePropertiesChanged.Add(value);
        }
        remove => variablePropertiesChanged?.Remove(value);
    }

    /// <summary>
    /// Variable zuweisen
    /// </summary>
    /// <param name="variable">Variable</param>
    public virtual void SetVariable(VariableBase variable) { }


}

