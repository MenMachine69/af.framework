using AF.MVC;
using DevExpress.XtraBars.Ribbon;

namespace AF.WINFORMS.DX;

/// <summary>
/// Base class for a WinFormsDX main form based on AF.
///
/// A form based on this class can be used as the main application window. The form based on DevExpress.XtraBars.ToolbarForm.ToolbarForm.
/// </summary>
public class FormRibbonbarShell : RibbonForm, IShell, ICommandResultDisplay
{
    private AFOverlayDisplayManager? _overlaydisplayManager;
    private AFFlyoutManager? _flyoutManager;
    private readonly IContainer componentsContainer = new Container();

    /// <summary>
    /// <see cref="IShell.OverlayDisplayManager"/>
    /// </summary>
    public AFOverlayDisplayManager OverlayDisplayManager => _overlaydisplayManager ??= new(componentsContainer)
    {
        Host = this
    };
    
    /// <summary>
    /// <see cref="IShell.FlyoutManager"/>
    /// </summary>
    public AFFlyoutManager FlyoutManager => _flyoutManager ??= new(componentsContainer)
    {
        Host = this,
        VerticalMessageOffset = Ribbon.Height
    };
    
    /// <summary>
    /// <see cref="IOverlayHost.Container"/>
    /// </summary>
    Control IOverlayHost.Container => this;

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
            componentsContainer?.Dispose();

        base.Dispose(disposing);
    }

    /// <summary>
    /// Eine Systemnachricht an die Shell senden, damit diese von der Shell verarbeitet werden kann.
    /// </summary>
    /// <param name="message"></param>
    public virtual void PushMessage(ISystemMessage message)
    {

    }

    #region MessageService
    /// <inheritdoc />
    [Browsable(true)]
    [DefaultValue(true)]
    public bool SupportHandleResult { get; set; } = true; 

    /// <summary>
    /// Handle a command result.
    /// <seealso cref="AFApp.HandleResult(CommandResult)"/>
    /// </summary>
    /// <param name="result"></param>
    public void HandleResult(CommandResult result)
    {
        AFCore.App.HandleResult(result);
    }

    /// <summary>
    /// Show a MsgBox
    /// <seealso cref="AFApp.ShowMsgBox(MessageBoxArguments)"/>
    /// </summary>
    /// <param name="args"></param>
    public eMessageBoxResult ShowMsgBox(MessageBoxArguments args)
    {
        return AFCore.App.ShowMsgBox(this, args);
    }

    /// <summary>
    /// Show a MsgBox
    /// <seealso cref="AFApp.ShowMsgBox(object, MessageBoxArguments)"/>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="owner">form thats used as parent for the message</param>
    public eMessageBoxResult ShowMsgBox(object? owner, MessageBoxArguments args)
    {
        return AFCore.App.ShowMsgBox(owner ?? this, args);
    }


    /// <summary>
    /// Show a Message (Flyout)
    /// <seealso cref="AFApp.ShowMessage(MessageArguments)"/>
    /// </summary>
    /// <param name="args">message to display</param>
    public void ShowMessage(MessageArguments args)
    {
        AFCore.App.ShowMessage(args);
    }
    #endregion

    #region Implementation of IShell

    /// <inheritdoc />
    public virtual void ShowAbout()
    {
        throw new NotImplementedException();
    }

    #endregion
}