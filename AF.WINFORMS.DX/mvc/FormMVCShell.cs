namespace AF.MVC;

/// <summary>
/// Basisfenster einer MVC Anwendung.
///
/// Leiten Sie das Hauptfenster der Anwendung von dieser Klasse ab.
/// </summary>
[SupportedOSPlatform("windows")]
public class FormMVCShell : FormToolbarShell, IShellMVC
{
    private IntPtr _clipboardMonitor;
    private readonly AFToastNotificator notificator = null!;

    /// <summary>
    /// Constructor
    /// </summary>
    public FormMVCShell() 
    {
        if (UI.DesignMode) return;

        notificator = new();
    }

    /// <summary>
    /// Sidebar-control der Anwendung.
    /// </summary>
    private AFSidebarControl? _sidebar;

    /// <summary>
    /// ViewManager for the application.
    /// </summary>
    public virtual AFViewManager ViewManager =>
        throw new NullReferenceException(@"Access to ViewManager is not implemented.");

    /// <summary>
    /// Zugriff auf die Sidebar
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFSidebarControl Sidebar
    {
        get { return _sidebar ??= new() { Dock = DockStyle.Fill }; }
    } 

    /// <summary>
    /// Sidebar anzeigen
    /// </summary>
    /// <exception cref="InvalidOperationException">throws if no viewmanager is assigned</exception>
    public void ShowSidebar()
    {
        if (ViewManager == null)
            throw new InvalidOperationException(@"ViewManager is not set.");

        ViewManager.ShowSidebar();
    }

    /// <summary>
    /// Sidebar ausblenden
    /// </summary>
    /// <exception cref="InvalidOperationException">throws if no viewmanager is assigned</exception>
    public void CloseSidebar()
    {
        if (ViewManager == null)
            throw new InvalidOperationException(@"ViewManager is not set.");

        ViewManager.CloseSidebar();
    }

    /// <summary>
    /// Sichtbarkeit der Sidebar umschalten
    /// </summary>
    /// <exception cref="InvalidOperationException">throws if no viewmanager is assigned</exception>
    public void ToggleSidebar()
    {
        if (ViewManager == null)
            throw new InvalidOperationException(@"ViewManager is not set.");

        ViewManager.ToggleSidebar();
    }

    /// <inheritdoc />
    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);

        // close the current splash screen
        UI.CloseSplash();

        // Clipboard-Monitor einrichten...
        _clipboardMonitor = SetClipboardViewer(Handle);
    }


    /// <inheritdoc />
    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        // Clipboard-Monitor entfernen
        ChangeClipboardChain(Handle, _clipboardMonitor);
        base.OnFormClosed(e);
    }

    /// <summary>
    /// Einen neuen Toast anzeigen...
    /// </summary>
    /// <param name="type">Typ der Benachrichtigung</param>
    /// <param name="caption">Überschrift</param>
    /// <param name="description">Mitteilung (kann HTML enthalten)</param>
    /// <param name="linkaction">auszuführende Aktion</param>
    /// <param name="link">ModelLink auf den hingewiesen wird</param>
    /// <param name="timeoutSeconds">Timeout, standard = 15</param>
    public void ShowToast(eNotificationType type, string caption, string description, Action<ModelLink>? linkaction = null, ModelLink? link = null, int timeoutSeconds = 15)
    {
        notificator.ShowToast(type, caption, description, linkaction, link, timeoutSeconds);
    }

    /// <summary>
    /// Aktion/Handler für Clipboard-Ereignisse (Text wurde in der Zwischenablage gefunden).
    ///
    /// Der Handler wird aufgerufen und der Text in der Zwischenablage übergeben.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Action<string>? ClipboardHandler { get; set; }

    #region ClipBoard Monitor

    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    internal static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    internal static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

    /// <summary>
    /// Standardbehandlung von Windows-Nachrichten...
    /// </summary>
    /// <param name="m"></param>
    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);

        if (m.Msg == 0x0308) // WM_DRAWCLIPBOARD
        {

            try
            {
                if (ClipboardHandler == null) return;

                System.Windows.Forms.IDataObject? iData = Clipboard.GetDataObject(); // Daten in der Zwischenablage lesen...
                
                if (iData == null) return;

                if (!iData.GetDataPresent(DataFormats.Text)) return;

                if (iData.GetData(DataFormats.Text) is not string text) return;
                    
                if (text.IsNotEmpty())
                    ClipboardHandler?.Invoke(text);
            }
            catch //(Exception ex)
            {
                // ignorieren, wenn ein Fehler auftritt...
            }
        }
    }
    #endregion
}