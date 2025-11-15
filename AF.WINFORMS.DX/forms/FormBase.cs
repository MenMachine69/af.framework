using AF.MVC;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Base class for all forms based on XtraForm.
/// </summary>
public class FormBase : XtraForm, ICommandResultDisplay
{
    private AFFlyoutManager? _flyoutManager;
    private readonly IContainer componentsContainer = new Container();

    /// <summary>
    /// Constructor
    /// </summary>
    public FormBase()
    {
        if (UI.DesignMode) return;

        if (((AFWinFormsDXApp)AFCore.App).Shell != null)
            IconOptions.Icon = ((XtraForm)((AFWinFormsDXApp)AFCore.App).Shell!).IconOptions.Icon;
    }

    /// <summary>
    /// <see cref="IShell.FlyoutManager"/>
    /// </summary>
    public AFFlyoutManager FlyoutManager => _flyoutManager ??= new(componentsContainer)
    {
        Host = this,
    };

    /// <summary>
    /// Container für Components...
    /// </summary>
    public IContainer ComponentsContainer => componentsContainer;

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
            componentsContainer.Dispose();

        base.Dispose(disposing);
    }

    /// <summary>
    /// Das Ergebnis der Ausführung eines Commands bearbeiten.
    /// 
    /// Je nach Ergebnis wird eine Meldung als Fylout oder MessageBox angezeigt.
    /// </summary>
    /// <param name="result">zu behandelndes Ergebnis</param>
    public void HandleResult(CommandResult result)
    {
        if (result.Exception != null)
        {
            MsgBox.ShowErrorOk(string.Format(WinFormsStrings.ERR_HANDLECMDRESULTERROR, result.ResultMessage, result.Exception.Message), result.Exception.StackTrace ?? "");
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
            case eNotificationType.Success:
                ShowMessageInfo(result.ResultMessage);
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
    /// Eine Meldung als Flyout anzeigen.
    /// </summary>
    /// <param name="args">Beschreibung der anzuzeigenden Meldung</param>
    public virtual void ShowMessage(MessageArguments args)
    {
        FlyoutManager.ShowMessage(args);
    }

    /// <summary>
    /// Eine Fehler-Meldung als Flyout anzeigen.
    /// </summary>
    /// <param name="message">Fehlermeldung</param>
    /// <param name="timeout">Anzeigedauer in Sekunden (Optional, Standard = 5s)</param>
    public void ShowMessageError(string message, int timeout = 5)
    {
        ShowMessage(new MessageArguments(message)
        {
            TimeOut = timeout,
            Type = eNotificationType.Error
        });
    }

    /// <summary>
    /// Eine Info-Meldung als Flyout anzeigen.
    /// </summary>
    /// <param name="message">Infomeldung</param>
    /// <param name="timeout">Anzeigedauer in Sekunden (Optional, Standard = 5s)</param>
    public virtual void ShowMessageInfo(string message, int timeout = 5)
    {
        ShowMessage(new MessageArguments(message)
        {
            TimeOut = timeout,
            Type = eNotificationType.Information
        });
    }

    /// <summary>
    /// Eine Warnung-Meldung als Flyout anzeigen.
    /// </summary>
    /// <param name="message">Warnung-Meldung</param>
    /// <param name="timeout">Anzeigedauer in Sekunden (Optional, Standard = 5s)</param>
    public virtual void ShowMessageWarning(string message, int timeout = 5)
    {
        ShowMessage(new MessageArguments(message)
        {
            TimeOut = timeout,
            Type = eNotificationType.Warning
        });
    }

    /// <inheritdoc />
    [Browsable(true)]
    [DefaultValue(true)]
    public bool SupportHandleResult { get; set; } = true;

    /// <summary>
    /// ID die das Fenster identifiziert, wenn eine Persitierung der Größe und Position gewünscht sind.
    /// </summary>
    [Browsable(true)]
    [DefaultValue(true)]
    public Guid FormIdentifier { get; set; } = Guid.Empty;


    /// <inheritdoc />
    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);

        if (Controls.Count < 1)
        {
            resetFormState();
            return;
        }

        Application.DoEvents();

        AFSettingsEditor? settingsEditor = null;

        for (int pos = 0; pos < Controls.Count; ++pos)
        {
            if (Controls[pos] is AFSettingsEditor editor)
            {
                settingsEditor = editor;
                break;
            }
        }

        if (settingsEditor == null)
        {
            resetFormState();
            return;
        }

        Size = new Size(Width + 1, Height + 2);
        Application.DoEvents();

        resetFormState();
    }

    private void resetFormState()
    {
        if (FormIdentifier == Guid.Empty || AFCore.App.Persistance == null) return;

        try
        {
            var state = Functions.DeserializeJsonBytes<FormState>(AFCore.App.Persistance.Get(FormIdentifier), true);

            if (state == null) return;

            if (state.CurrentState == FormWindowState.Minimized) return;

            if (state.CurrentSize == null || state.CurrentLocation == null || state.CurrentState == null) return;

            if (state.Screens.Length != Screen.AllScreens.Length) return;

            bool foundScreen = true;

            foreach (var screen in Screen.AllScreens)
            {
                var found = state.Screens.FirstOrDefault(s =>
                    s.Bounds != null &&
                    s.Bounds.Value.X == screen.Bounds.X &&
                    s.Bounds.Value.Y == screen.Bounds.Y &&
                    s.Bounds.Value.Width == screen.Bounds.Width &&
                    s.Bounds.Value.Height == screen.Bounds.Height);

                if (found == null) foundScreen = false;
            }

            if (!foundScreen) return;

            Size = new Size(state.CurrentSize.Value.Width, state.CurrentSize.Value.Height);
            Location = new Point(state.CurrentLocation.Value.X, state.CurrentLocation.Value.Y);
            WindowState = state.CurrentState.Value;
        }
#if (DEBUG)
        catch (Exception ex)
        {
            MsgBox.ShowErrorOk("FORM PERSISTANCE\r\nFehler beim wiederherstellen.\r\n" + ex.Message);
        }
#else
        catch
        {
            // ignored
        }
#endif
    }


    /// <inheritdoc />
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (FormIdentifier == Guid.Empty || AFCore.App.Persistance == null)
        {
            base.OnFormClosing(e);
            return;
        }

        if (WindowState == FormWindowState.Minimized) return;

        var state = new FormState()
        {
            CurrentState = WindowState,
            CurrentLocation = new(Location.X, Location.Y),
            CurrentSize = new(Size.Width, Size.Height),
        };

        List<ScreenInfo> info = [];
       
        foreach (var screen in Screen.AllScreens)
            info.Add(new ScreenInfo { Bounds = screen.Bounds with { X = screen.Bounds.Location.X, Y = screen.Bounds.Location.Y } });

        state.Screens = info.ToArray();

        AFCore.App.Persistance.Set(FormIdentifier, state.ToJsonBytes());
    }
}

/// <summary>
/// Status eines Forms (für Restore und Save)
/// </summary>
[Serializable]
public class FormState
{
    /// <summary>
    /// Constructor
    /// </summary>
    public FormState()
    {
        CurrentSize = null;
        CurrentLocation = null;
        CurrentState = null;
        Screens = [];
    }

    /// <summary>
    /// aktuelle Größe
    /// </summary>
    public Size? CurrentSize { get; set; }
    /// <summary>
    /// aktuelle Position
    /// </summary>
    public Point? CurrentLocation { get; set; }
    /// <summary>
    /// Aktueller Status
    /// </summary>
    public FormWindowState? CurrentState { get; set; }

    /// <summary>
    /// Informationen zu den Bildschirmen zum Zeitpunkt der Speicherung
    /// </summary>
    public ScreenInfo[] Screens { get; set; }

    
}

/// <summary>
/// Informationen zum Bildschirm
/// </summary>
[Serializable]
public class ScreenInfo
{
    /// <summary>
    /// Constructor
    /// </summary>
    public ScreenInfo()
    {
        Bounds = null;
    }

    /// <summary>
    /// Größe des Bildschirms
    /// </summary>
    public Rectangle? Bounds { get; set; }
}