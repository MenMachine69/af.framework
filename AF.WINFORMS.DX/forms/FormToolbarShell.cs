using AF.MVC;
using DevExpress.XtraBars;

namespace AF.WINFORMS.DX;

/// <summary>
/// Base class for a WinFormsDX main form based on AF.
///
/// A form based on this class can be used as the main application window. The form based on DevExpress.XtraBars.ToolbarForm.ToolbarForm.
/// </summary>
public class FormToolbarShell : DevExpress.XtraBars.ToolbarForm.ToolbarForm, IShell, ICommandResultDisplay
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
        VerticalMessageOffset = ToolbarFormControl.Height
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

    /// <summary>
    /// ID die das Fenster identifiziert, wenn eine Persitierung der Größe und Position gewünscht sind.
    /// </summary>
    [Browsable(true)]
    [DefaultValue(true)]
    public Guid FormIdentifier { get; set; } = Guid.Empty;

    /// <inheritdoc/>
    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);

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
            // Ignored
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

        base.OnFormClosing(e);
    }

    #region Popup Unterstützung

    private ShellPopup? _currentPopup;
    private BarItemLink? _currentLabel;

    /// <summary>
    /// Aktuelles Popup schließen...
    /// </summary>
    public void HidePopup()
    {
        if (_currentPopup == null) return;

        _currentPopup.Hide();
        _currentPopup.Close();
        _currentPopup = null;

        if (_currentLabel == null) return;

        _currentLabel.Item.Tag = false;
        _currentLabel = null;
    }

    /// <summary>
    /// Ein Popup anzeigen...
    /// </summary>
    /// <param name="popup"></param>
    /// <param name="caption"></param>
    public void ShowPopup(ShellPopup popup, BarItemLink caption)
    {
        HidePopup();

        _currentPopup = popup;
        _currentLabel = caption;

        _currentLabel.Item.Tag = true;

        popup.ShowPopup(this, caption);
    }
    #endregion


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

/// <summary>
/// Popup-Fenster der FormToolbarShell
/// </summary>
public class ShellPopup : FormBase
{
    private System.Windows.Forms.Timer? _closeTimer;

    /// <summary>
    /// Shell, zu der das Popup gehört...
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public FormToolbarShell? Shell { get; set; }

    /// <summary>
    /// Breite an Bildschirm anpassen
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AutoWidth { get; set; } = true;

    /// <summary>
    /// Ausrichtung zum übergeordneten Label
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public eAlignmentHorizontal Alignment { get; set; } = eAlignmentHorizontal.Far;



    /// <summary>
    /// Wird nach dem Anzeigen ausgeführt
    /// </summary>
    public virtual void AfterShow() { }

    /// <summary>
    /// Wird vor dem Anzeigen ausgeführt
    /// </summary>
    public virtual void BeforeShow() { }

    /// <summary>
    /// Wird nach dem Erzeugen ausgeführt
    /// </summary>
    public virtual void AfterCreate() { }

    /// <summary>
    /// 
    /// </summary>
    public new void Hide()
    {
        _closeTimer?.Stop();

        base.Hide();
    }

    /// <summary>
    /// Popup anzeigen
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="menuentry">Menüeintrag zu dem das Popup gehört</param>
    public void ShowPopup(FormToolbarShell owner, BarItemLink menuentry)
    {
        Shell = owner;

        // var pt = menuentry.LinkPointToScreen(new(0, 0));

        _closeTimer ??= new() { Interval = 5000 };

        _closeTimer.Tick += (_, _) =>
        {
            _closeTimer?.Stop();

            if (ClientRectangle.Contains(PointToClient(Cursor.Position)))
                _closeTimer?.Start();
            else
                Shell?.HidePopup();
        };

        _closeTimer?.Start();

        BeforeShow();

        TopMost = true;
        StartPosition = FormStartPosition.Manual;
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;

        if (AutoWidth)
        {
            Location = new(owner.Left, owner.Bottom);
            Size = new(owner.Width, Height);
        }
        else
            Location = new(Alignment == eAlignmentHorizontal.Far ? owner.Left + (menuentry.Bounds.Right - Width + 10) : owner.Left + menuentry.Bounds.Left, owner.Top + menuentry.Bounds.Bottom);

        Show(owner);

        AfterShow();
    }
}

