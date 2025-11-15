using DevExpress.XtraEditors.DXErrorProvider;

namespace AF.WINFORMS.DX;

/// <summary>
/// Errorprovider component to use as replacement for DXErrorProvider.
/// This provider supports IErrorProvider and can be used for IModel.Validate().
/// </summary>
[ToolboxItem(true)]
[SupportedOSPlatform("windows")]
[DesignerCategory("Code")]
public class AFErrorProvider : DXErrorProvider, IErrorProvider
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="container">Container control</param>
    public AFErrorProvider(IContainer? container) : base(container)
    {
    }
    
    /// <summary>
    /// True if warnings are present
    /// </summary>
    public bool HasWarnings => HasErrorsOfType(ErrorType.Warning);

    /// <summary>
    /// Remove all errors and warnings
    /// </summary>
    public void Clear()
    {
        ClearErrors();
    }

    /// <summary>
    /// display errrors from a ValidationErrorCollection.
    /// </summary>
    /// <param name="errors">Collection of Errors</param>
    public void FromCollection(ValidationErrorCollection errors)
    {
        Clear();

        foreach (var error in errors)
            SetError(error.PropertyName, error.Message);
    }

    /// <summary>
    /// Set a error for a property
    /// </summary>
    /// <param name="propertyName">property name</param>
    /// <param name="errorMessage">error message</param>
    public void SetError(string propertyName, string errorMessage)
    {
        set(propertyName, errorMessage, ErrorType.Critical);
    }

    /// <summary>
    /// Set a warning for a property
    /// </summary>
    /// <param name="propertyName">property name</param>
    /// <param name="errorMessage">error message</param>
    public void SetWarning(string propertyName, string errorMessage)
    {
        var ctrls = ContainerControl.Controls.Find(propertyName, true);

        if (ctrls.Length > 0)
            base.SetError(ctrls[0], errorMessage, ErrorType.Warning);
    }

    private void set(string propertyName, string errorMessage, ErrorType errorType)
    {
        var ctrls = ContainerControl.Controls.Find(propertyName, true);

        if (ctrls.Length > 0)
            base.SetError(ctrls[0], errorMessage, errorType);

    }
}

