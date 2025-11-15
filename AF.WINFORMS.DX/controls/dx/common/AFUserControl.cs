using AF.MVC;
using DevExpress.Utils;
using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace AF.WINFORMS.DX;

/// <inheritdoc/>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
public class AFUserControl : XtraUserControl
{
    private readonly IContainer componentsContainer = new Container();
    private AFFlyoutManager? _flyoutManager;
    private AFProgressPanel? waitPanel;
    // private List<Tuple<Control, bool>>? beforeWaitState = null;

    private bool _paintBackground;

    /// <summary>
    /// Components-Container
    /// </summary>
    public new IContainer Container => componentsContainer;

    /// <summary>
    /// Custom draw Background using the BackgroundAppearances
    /// </summary>
    [Category("Custom background")]
    [Browsable(true)]
    [DefaultValue(false)]
    public bool CustomPaintBackground 
    {
        get => _paintBackground;
        set
        {
            _paintBackground = value;
            
            if (value)
                BackgroundAppearance ??= new();
            else
                BackgroundAppearance = null;

            Invalidate();
        }
    }

    /// <summary>
    /// Appearance for custom drawing background
    /// </summary>
    [DefaultValue(null)]
    [Category("Custom background")]
    public AFBackgroundAppearance? BackgroundAppearance { get; set; }

    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        SetStyle(ControlStyles.ResizeRedraw, true);
        DoubleBuffered = true;
    }


    /// <summary>
    ///     Draw background if needed
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPaintBackground(PaintEventArgs e)
    {
        base.OnPaintBackground(e);

        if (!CustomPaintBackground) return;

        if (BackgroundAppearance == null) return;

        var rect = ClientRectangle.WithDeflate(new Padding(0, 0, 1, 1));

        BackgroundAppearance.Draw(e.Graphics, rect, Margin, Padding);
    }

    /// <summary>
    ///     Use Background drawing (rounded corners etc.)
    /// </summary>
    [Browsable(true)]
    [DefaultValue(false)]
    public bool PaintBackground
    {
        get => _paintBackground;
        set
        {
            _paintBackground = value;
            Invalidate();
        }
    }

    /// <summary>
    ///     <see cref="IShell.FlyoutManager" />
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFFlyoutManager FlyoutManager => _flyoutManager ??= new(componentsContainer)
    {
        Host = this
    };

    /// <summary>
    /// Control can handle CommandResult and do not 
    /// forward this to parent control or form
    /// </summary>
    [Browsable(true)]
    [DefaultValue(false)]
    public bool SupportHandleResult { get; set; }
      

    /// <summary>
    ///     Handle a command result.
    /// </summary>
    /// <param name="result">result</param>
    [Localizable(false)]
    public void HandleResult(CommandResult result)
    {
        if (!SupportHandleResult)
        {
            if (FindForm() is not ICommandResultDisplay disp) return;

            disp.HandleResult(result);

            return;
        }

        if (result.Exception != null)
        {
            MsgBox.ShowErrorOk(
                $"{WinFormsStrings.ERROR}" + Environment.NewLine + result.ResultMessage + Environment.NewLine + "(" +
                result.Exception.Message + ")", result.Exception.StackTrace ?? "");
            return;
        }

        switch (result.Result)
        {
            case eNotificationType.Error:
            case eNotificationType.SystemError:
                ShowMessageError(result.ResultMessage);
                break;
            case eNotificationType.Warning:
                ShowMessageWarning(result.ResultMessage);
                break;
            case eNotificationType.Information:
                ShowMessageInfo(result.ResultMessage);
                break;
            case eNotificationType.None:
                break;
            default:
            {
                if (result.ResultMessage.IsNotEmpty())
                    ShowMessageInfo(result.ResultMessage);
                break;
            }
        }
    }

    /// <summary>
    ///     show a message as flyout
    /// </summary>
    /// <param name="args">message</param>
    public virtual void ShowMessage(MessageArguments args)
    {
        FlyoutManager.ShowMessage(args);
    }

    /// <summary>
    ///     show a error message as flyout
    /// </summary>
    /// <param name="message">message</param>
    /// <param name="timeout">timeout in seconds</param>
    public void ShowMessageError(string message, int timeout = 5)
    {
        ShowMessage(new MessageArguments(message)
        {
            TimeOut = timeout,
            Type = eNotificationType.Error
        });
    }

    /// <summary>
    ///     show a info message as flyout
    /// </summary>
    /// <param name="message">message</param>
    /// <param name="timeout">timeout in seconds</param>
    public virtual void ShowMessageInfo(string message, int timeout = 5)
    {
        ShowMessage(new MessageArguments(message)
        {
            TimeOut = timeout,
            Type = eNotificationType.Information
        });
    }

    /// <summary>
    ///     show a warning message as flyout
    /// </summary>
    /// <param name="message">message</param>
    /// <param name="timeout">timeout in seconds</param>
    public virtual void ShowMessageWarning(string message, int timeout = 5)
    {
        ShowMessage(new MessageArguments(message)
        {
            TimeOut = timeout,
            Type = eNotificationType.Warning
        });
    }


    /// <summary>
    /// Nachrichten 'Bitte warten' anzeigen
    /// </summary>
    /// <param name="caption">Überschrift</param>
    /// <param name="description">Beschreibung</param>
    public virtual void ShowWait(string caption, string description)
    {
        if (waitPanel != null) return;

        waitPanel = new() { Caption = caption, Description = description, AutoHeight = true, AutoWidth = true, BorderStyle = BorderStyles.Simple, Padding = new(10) };
        Controls.Add(waitPanel);
        waitPanel.BringToFront();
        waitPanel.Location = new((Width - waitPanel.Width) / 2, (Height - waitPanel.Height) / 2);
        Application.DoEvents();
    }

    /// <summary>
    /// Nachricht 'Bitte warten' verbergen
    /// </summary>
    public virtual void HideWait()
    {
        if (waitPanel == null) return;

        waitPanel.Hide();
        Controls.Remove(waitPanel);
        Application.DoEvents();
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
            componentsContainer?.Dispose();

        base.Dispose(disposing);
    }
}