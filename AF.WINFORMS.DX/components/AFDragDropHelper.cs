namespace AF.WINFORMS.DX;

/// <summary>
/// Helper class for simplified support of DragDrop operations
/// </summary>
public class AFDragDropHelper
{
    private readonly Dictionary<Control, methodInformation> _registeredControls = new();

    /// <summary>
    /// Registers a control in the Helper as target for drag/drop support
    /// </summary>
    /// <param name="control">the control that accepts drag/drop data</param>
    /// <param name="acceptMethod">method to call if dragged data is accepted by the control</param>
    /// <param name="dropMethod">method to call if accepted data are dropped into the control</param>
    public void Register(Control control, Func<System.Windows.Forms.IDataObject, bool> acceptMethod,
        Action<System.Windows.Forms.IDataObject, Point> dropMethod)
    {
        if (_registeredControls.ContainsKey(control)) return;

        control.DragEnter += _onEnter;
        control.DragDrop += _onDrop;
        control.AllowDrop = true;
        _registeredControls.Add(control, new methodInformation(acceptMethod, dropMethod));
    }

    /// <summary>
    /// Log off a specific control from the Helper
    /// </summary>
    /// <param name="control"></param>
    public void UnRegister(Control control)
    {
        if (!_registeredControls.ContainsKey(control)) return;

        control.DragEnter -= _onEnter;
        control.DragDrop -= _onDrop;

        _registeredControls.Remove(control);
    }

    /// <summary>
    /// Log out all controls from the Helper
    /// </summary>
    public void UnRegisterAll()
    {
        foreach (KeyValuePair<Control, methodInformation> pair in _registeredControls)
        {
            pair.Key.DragEnter -= _onEnter;
            pair.Key.DragDrop -= _onDrop;
        }

        _registeredControls.Clear();
    }

    private void _onEnter(object? sender, DragEventArgs e)
    {
        e.Effect = DragDropEffects.None;

        if (sender is not Control control) return;

        if (e.Data == null) return;

        if (!_registeredControls.ContainsKey(control)) return;

        if (!_registeredControls[control].acceptMethod(e.Data)) return;

        e.Effect = DragDropEffects.Copy;
    }

    private void _onDrop(object? sender, DragEventArgs e)
    {
        if (sender is not Control control) return;

        if (e.Data == null) return;

        if (!_registeredControls.ContainsKey(control)) return;

        if (!_registeredControls[control].acceptMethod(e.Data)) return;

        _registeredControls[control].dropMethod(e.Data, new(e.X, e.Y));
    }

    private class methodInformation
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="accept"></param>
        /// <param name="drop"></param>
        public methodInformation(Func<System.Windows.Forms.IDataObject, bool> accept, Action<System.Windows.Forms.IDataObject, Point> drop)
        {
            acceptMethod = accept;
            dropMethod = drop;
        }

        /// <summary>
        /// Delegate des Controls zur Überprüfung, ob das Control die Daten akzeptiert...
        /// 
        /// Die Methode des Controls bekommt die zu akzeptierenden Daten übergeben und 
        /// gibt true zurück, wenn die Daten akzeptiert werden.
        /// </summary>
        internal Func<System.Windows.Forms.IDataObject, bool> acceptMethod { get; }

        /// <summary>
        /// Delegate der ausgeführt wird, wenn die Daten auf dem Control abgelegt werden.
        /// </summary>
        internal Action<System.Windows.Forms.IDataObject, Point> dropMethod { get; }
    }
}