namespace AF.WINFORMS.DX;

/// <summary>
/// Control that can be displayed as an overlay in a IOverlayHost control/form
/// </summary>
public interface IOverlayControl
{
    /// <summary>
    /// OverlayDisplayManager that uses/manages the overlay.
    /// 
    /// Das Control kann die Methode CloseOverlay des OverlayDisplayManagers 
    /// aufrufen, um sich selbst aktiv zu schliessen.
    /// </summary>
    AFOverlayDisplayManager? OverlayManager { get; set; }

    /// <summary>
    /// Raises the OverlayClosing event
    /// </summary>
    /// <param name="args">event arguments, set args.Cancel to true to avoid closing</param>
    void OverlayClosing(CancelEventArgs args);

    /// <summary>
    /// Raises the OverlayClosed event
    /// 
    /// Implement all needed things to do before the control was closed.
    /// </summary>
    void OverlayClosed();
}

/// <summary>
/// Interface for a control/form that can display an overlay control
/// </summary>
public interface IOverlayHost
{
    /// <summary>
    /// OverlayDisplayManager that uses/manages the overlay.
    /// 
    /// The control can call the CloseOverlay method of the OverlayDisplayManager 
    /// to actively close itself.
    /// </summary>
    Control Container { get; }
}

/// <summary>
/// Manager component for displaying IOverlayControl overlays in forms and controls
/// </summary>
public class AFOverlayDisplayManager : AFVisualComponentBase
{
    private readonly List<Tuple<Control, bool>> _states = [];
    private bool _initalized;
    private IOverlayHost? _overlayHost;
    private WeakEvent<EventHandler<CancelEventArgs>>? _overlayClosing;
    private WeakEvent<EventHandler<EventArgs>>? _overlayClosed;
    private WeakEvent<EventHandler<EventArgs>>? _overlayShown;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="container"></param>
    public AFOverlayDisplayManager(IContainer? container)
    {
        container?.Add(this);
    }

    /// <summary>
    /// Current used overlay mode
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public eOverlayDisplayMode CurrentMode { get; private set; }

    /// <summary>
    /// Current displayed IOverlayControl 
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IOverlayControl? CurrentOverlay { get; private set; }

    /// <summary>
    /// Current displayed IOverlayControl 
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IOverlayHost Host
    {
        get
        {
            _overlayHost ??= ((AFWinFormsDXApp)AFCore.App).Shell;

            return _overlayHost ??
                   throw new NullReferenceException(@"No host control assigned to allow overlay display.");
        }
        set => _overlayHost = value;
    }

    /// <summary>
    /// Shows an IOverlayControl
    /// </summary>
    /// <param name="overlay">Control to be displayed (must be implement IOverlayControl)</param>
    /// <param name="mode">Overlay display mode</param>
    public void ShowOverlay(IOverlayControl overlay, eOverlayDisplayMode mode)
    {
            if (CurrentOverlay != null)
                throw new Exception(@"Can't display overlay because ContainerControl displays currently another overlay.");

            if (_initalized == false)
                initalize();

            _states.Clear();

            foreach (Control control in Host.Container.Controls)
                saveSate(control);

            CurrentMode = mode;

            Control ctrl = overlay as Control ?? throw new NullReferenceException(@"IOverlayControl must be a System.Windows.Forms.Control.");

            if (mode == eOverlayDisplayMode.Fullscreen)
                ctrl.Dock = DockStyle.Fill;
            else if (mode == eOverlayDisplayMode.BannerDialog)
            {
                ctrl.Dock = DockStyle.None;
                ctrl.Location = new Point(0, (Host.Container.ClientRectangle.Height - ctrl.Height) / 2);
                ctrl.Size = new Size(Host.Container.ClientRectangle.Width, ctrl.Height);
            }
            else if (mode == eOverlayDisplayMode.CenterDialog)
            {
                if (ctrl.Height >= Host.Container.ClientRectangle.Height ||
                    ctrl.Width >= Host.Container.ClientRectangle.Width)
                {
                    // if width or height greater then clientarea use fullscreen mode
                    ctrl.Dock = DockStyle.Fill;
                    CurrentMode = eOverlayDisplayMode.Fullscreen;
                }
                else
                {
                    ctrl.Dock = DockStyle.None;
                    ctrl.Location = new Point((Host.Container.ClientRectangle.Width - ctrl.Width) / 2,
                        (Host.Container.ClientRectangle.Height - ctrl.Height) / 2);
                }
            }


            CurrentOverlay = overlay;
            CurrentOverlay.OverlayManager = this;

            Host.Container.Controls.Add(ctrl);
            ctrl.BringToFront();
            Application.DoEvents();
            ctrl.Focus();

            _overlayShown?.Raise(this, EventArgs.Empty);
    }

    /// <summary>
    /// Close current IOverlayControl
    /// </summary>
    public void CloseOverlay()
    {
        if (CurrentOverlay == null) return;

        CancelEventArgs args = new();
        CurrentOverlay.OverlayClosing(args);

        if (args.Cancel) return;

        _overlayClosing?.Raise(CurrentOverlay, args);

        if (args.Cancel) return;

        _overlayClosed?.Raise(CurrentOverlay, args);
        CurrentOverlay.OverlayClosed();

        Host.Container.Controls.Remove((Control)CurrentOverlay);

        CurrentOverlay = null;

        foreach (var state in _states)
            state.Item1.Enabled = state.Item2;

        _states.Clear();
    }

    /// <summary>
    /// Event that is triggered before the overlay is closed.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public event EventHandler<CancelEventArgs> OverlayClosing
    {
        add
        {
            _overlayClosing ??= new();
            _overlayClosing.Add(value);
        }
        remove => _overlayClosing?.Remove(value);
    }

    /// <summary>
    /// Event that is triggered after the overlay has been closed.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public event EventHandler<EventArgs> OverlayClosed
    {
        add
        {
            _overlayClosed ??= new();
            _overlayClosed.Add(value);
        }
        remove => _overlayClosed?.Remove(value);
    }
    /// <summary>
    /// Event that is triggered after the overlay has been displayed.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public event EventHandler<EventArgs> OverlayShown
    {
        add
        {
            _overlayShown ??= new();
            _overlayShown.Add(value);
        }
        remove => _overlayShown?.Remove(value);
    }

    private void initalize()
    {
        if (ContainerControl != null)
            ContainerControl.ClientSizeChanged += containerSizeChanged;

        _initalized = true;
    }

    private void containerSizeChanged(object? sender, EventArgs e)
    {
        if (CurrentOverlay == null) return;

        if (CurrentMode == eOverlayDisplayMode.CenterDialog)
        {
            int xpos = ((Host.Container.ClientRectangle.Width - ((Control)CurrentOverlay).Width) / 2);
            int ypos = ((Host.Container.ClientRectangle.Height - ((Control)CurrentOverlay).Height) / 2);

            xpos = (xpos < 0 ? 0 :
                ((xpos + ((Control)CurrentOverlay).Height) > Host.Container.ClientRectangle.Bottom) ? 0 : xpos);
            ypos = (ypos < 0 ? 0 :
                ((ypos + ((Control)CurrentOverlay).Width) > Host.Container.ClientRectangle.Right) ? 0 : ypos);

            ((Control)CurrentOverlay).Location = new Point(xpos, ypos);
        }
        else if (CurrentMode == eOverlayDisplayMode.BannerDialog)
        {
            ((Control)CurrentOverlay).Location = new Point(0,
                (Host.Container.ClientRectangle.Height - ((Control)CurrentOverlay).Height) / 2);
            ((Control)CurrentOverlay).Size = new Size(Host.Container.ClientRectangle.Width,
                ((Control)CurrentOverlay).Height);
        }

    }


    private void saveSate(Control ctl)
    {
        _states.Add(new Tuple<Control, bool>(ctl, ctl.Enabled));
        ctl.Enabled = false;
    }
}

/// <summary>
/// Modes for displaying an IOverlayControl
/// </summary>
public enum eOverlayDisplayMode
{
    /// <summary>
    /// Control fills the complete area.
    /// </summary>
    Fullscreen,

    /// <summary>
    /// The control is displayed as a banner that fills the entire width and is vertically centered.
    /// </summary>
    BannerDialog,

    /// <summary>
    /// The control is vertically and horizontally centered on parent
    /// </summary>
    CenterDialog
}
