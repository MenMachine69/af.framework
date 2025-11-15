using DevExpress.CodeParser;

namespace AF.WINFORMS.DX;

/// <summary>
/// Anwendungsfenster, dass in Form einer AppBar angezeigt wird, die auf dem Desktop angedockt 
/// werden kann und den verfügbaren Arbeitsbereich einschränkt (ähnl. Toolbar auf Desktopebene)
/// </summary>
public class FormAppBar : Form
{
    private AppBarPopup? _currentPopup;
    private AFLabel? _currentLabel;

#pragma warning disable 1591
    #region Enums
    /// <summary>
    /// AppBar-Nachrichten
    /// </summary>
    public enum AppBarMessages
    {
        /// <summary>
        /// Registers a new appbar and specifies the message identifier
        /// that the system should use to send notification messages to 
        /// the appbar. 
        /// </summary>
        New = 0x00000000,
        /// <summary>
        /// Unregisters an appbar, removing the bar from the system's 
        /// internal list.
        /// </summary>
        Remove = 0x00000001,
        /// <summary>
        /// Requests a size and screen position for an appbar.
        /// </summary>
        QueryPos = 0x00000002,
        /// <summary>
        /// Sets the size and screen position of an appbar. 
        /// </summary>
        SetPos = 0x00000003,
        /// <summary>
        /// Retrieves the autohide and always-on-top states of the 
        /// Microsoft® Windows® taskbar. 
        /// </summary>
        GetState = 0x00000004,
        /// <summary>
        /// Retrieves the bounding rectangle of the Windows taskbar. 
        /// </summary>
        GetTaskBarPos = 0x00000005,
        /// <summary>
        /// Notifies the system that an appbar has been activated. 
        /// </summary>
        Activate = 0x00000006,
        /// <summary>
        /// Retrieves the handle to the autohide appbar associated with
        /// a particular edge of the screen. 
        /// </summary>
        GetAutoHideBar = 0x00000007,
        /// <summary>
        /// Registers or unregisters an autohide appbar for an edge of 
        /// the screen. 
        /// </summary>
        SetAutoHideBar = 0x00000008,
        /// <summary>
        /// Notifies the system when an appbar's position has changed. 
        /// </summary>
        WindowPosChanged = 0x00000009,
        /// <summary>
        /// Sets the state of the appbar's autohide and always-on-top 
        /// attributes.
        /// </summary>
        SetState = 0x0000000a
    }

    public enum AppBarNotifications
    {
        /// <summary>
        /// Notifies an appbar that the taskbar's autohide or 
        /// always-on-top state has changed—that is, the user has selected 
        /// or cleared the "Always on top" or "Auto hide" check box on the
        /// taskbar's property sheet. 
        /// </summary>
        StateChange = 0x00000000,
        /// <summary>
        /// Notifies an appbar when an event has occurred that may affect 
        /// the appbar's size and position. Events include changes in the
        /// taskbar's size, position, and visibility state, as well as the
        /// addition, removal, or resizing of another appbar on the same 
        /// side of the screen.
        /// </summary>
        PosChanged = 0x00000001,
        /// <summary>
        /// Notifies an appbar when a full-screen application is opening or
        /// closing. This notification is sent in the form of an 
        /// application-defined message that is set by the ABM_NEW message. 
        /// </summary>
        FullScreenApp = 0x00000002,
        /// <summary>
        /// Notifies an appbar that the user has selected the Cascade, 
        /// Tile Horizontally, or Tile Vertically command from the 
        /// taskbar's shortcut menu.
        /// </summary>
        WindowArrange = 0x00000003
    }

    [Flags]
    public enum AppBarStates
    {
        AutoHide = 0x00000001,
        AlwaysOnTop = 0x00000002
    }

    public enum AppBarEdges
    {
        Left = 0,
        Top = 1,
        Right = 2,
        Bottom = 3,
        Float = 4
    }

    public enum MousePositionCodes
    {
        HTERROR = -2,
        HTTRANSPARENT = -1,
        HTNOWHERE = 0,
        HTCLIENT = 1,
        HTCAPTION = 2,
        HTSYSMENU = 3,
        HTGROWBOX = 4,
        HTSIZE = HTGROWBOX,
        HTMENU = 5,
        HTHSCROLL = 6,
        HTVSCROLL = 7,
        HTMINBUTTON = 8,
        HTMAXBUTTON = 9,
        HTLEFT = 10,
        HTRIGHT = 11,
        HTTOP = 12,
        HTTOPLEFT = 13,
        HTTOPRIGHT = 14,
        HTBOTTOM = 15,
        HTBOTTOMLEFT = 16,
        HTBOTTOMRIGHT = 17,
        HTBORDER = 18,
        HTREDUCE = HTMINBUTTON,
        HTZOOM = HTMAXBUTTON,
        HTSIZEFIRST = HTLEFT,
        HTSIZELAST = HTBOTTOMRIGHT,
        HTOBJECT = 19,
        HTCLOSE = 20,
        HTHELP = 21
    }

    #endregion Enums
#pragma warning restore 1591

    #region AppBar Functions

    private void AppbarNew()
    {
        if (CallbackMessageID == 0)
            throw new("CallbackMessageID is 0");

        if (IsAppbarMode) return;

        m_PrevSize = Size;
        m_PrevLocation = Location;

        // prepare data structure of message
        Win32Structs.APPBARDATA msgData = new();
        msgData.cbSize = (uint)Marshal.SizeOf(msgData);
        msgData.hWnd = Handle;
        msgData.uCallbackMessage = CallbackMessageID;

        // install new appbar
        uint retVal = Win32Invokes.SHAppBarMessage((uint)AppBarMessages.New, ref msgData);
        IsAppbarMode = retVal != 0;

        SizeAppBar();
    }

    private void AppbarRemove()
    {
        // prepare data structure of message
        Win32Structs.APPBARDATA msgData = new();
        msgData.cbSize = (uint)Marshal.SizeOf(msgData);
        msgData.hWnd = Handle;

        // remove appbar
        Win32Invokes.SHAppBarMessage((uint)AppBarMessages.Remove, ref msgData);

        IsAppbarMode = false;

        Size = m_PrevSize;
        Location = m_PrevLocation;
    }

    private void AppbarQueryPos(ref Win32Structs.RECT appRect)
    {
        // prepare data structure of message
        Win32Structs.APPBARDATA msgData = new();
        msgData.cbSize = (uint)Marshal.SizeOf(msgData);
        msgData.hWnd = Handle;
        msgData.uEdge = (uint)m_Edge;
        msgData.rc = appRect;

        // query postion for the appbar
        Win32Invokes.SHAppBarMessage((uint)AppBarMessages.QueryPos, ref msgData);
        appRect = msgData.rc;
    }

    private void AppbarSetPos(ref Win32Structs.RECT appRect)
    {
        // prepare data structure of message
        Win32Structs.APPBARDATA msgData = new();
        msgData.cbSize = (uint)Marshal.SizeOf(msgData);
        msgData.hWnd = Handle;
        msgData.uEdge = (uint)m_Edge;
        msgData.rc = appRect;

        // set postion for the appbar
        Win32Invokes.SHAppBarMessage((uint)AppBarMessages.SetPos, ref msgData);
        appRect = msgData.rc;
    }

    private void AppbarActivate()
    {
        // prepare data structure of message
        Win32Structs.APPBARDATA msgData = new();
        msgData.cbSize = (uint)Marshal.SizeOf(msgData);
        msgData.hWnd = Handle;

        // send activate to the system
        Win32Invokes.SHAppBarMessage((uint)AppBarMessages.Activate, ref msgData);
    }

    private void AppbarWindowPosChanged()
    {
        // prepare data structure of message
        Win32Structs.APPBARDATA msgData = new();
        msgData.cbSize = (uint)Marshal.SizeOf(msgData);
        msgData.hWnd = Handle;

        // send windowposchanged to the system 
        Win32Invokes.SHAppBarMessage((uint)AppBarMessages.WindowPosChanged, ref msgData);
    }

    private void AppbarSetAutoHideBar(bool hideValue)
    {
        // prepare data structure of message
        Win32Structs.APPBARDATA msgData = new();
        msgData.cbSize = (uint)Marshal.SizeOf(msgData);
        msgData.hWnd = Handle;
        msgData.uEdge = (uint)m_Edge;
        msgData.lParam = hideValue ? 1 : 0;

        // set auto hide
        Win32Invokes.SHAppBarMessage((uint)AppBarMessages.SetAutoHideBar, ref msgData);
    }

    private AppBarStates AppbarGetTaskbarState()
    {
        // prepare data structure of message
        Win32Structs.APPBARDATA msgData = new();
        msgData.cbSize = (uint)Marshal.SizeOf(msgData);

        // get taskbar state
        uint retVal = Win32Invokes.SHAppBarMessage((uint)AppBarMessages.GetState, ref msgData);
        return (AppBarStates)retVal;
    }

    #endregion AppBar Functions

    #region Private Variables

    // saves the current edge
    private AppBarEdges m_Edge = AppBarEdges.Float;

    // saves the callback message id
    private readonly uint CallbackMessageID;

    // are we in dock mode?
    private bool IsAppbarMode;

    // save the floating size and location
    private Size m_PrevSize;
    private Point m_PrevLocation;

    #endregion Private Variables

    /// <summary>
    /// Constructor
    /// </summary>
    public FormAppBar()
    {
        FormBorderStyle = FormBorderStyle.SizableToolWindow;

        if (UI.DesignMode) return;

        AutoScaleMode = AutoScaleMode.Font;


        // Register a unique message as our callback message
        CallbackMessageID = RegisterCallbackMessage();
        if (CallbackMessageID == 0)
            throw new("RegisterCallbackMessage failed");
    }

    /// <summary>
    /// Automatisches Ausblenden ein/ausschalten
    /// </summary>
    /// <param name="bAutoHide">ein (true) oder aus (false)</param>
    public void SetAutoHide(bool bAutoHide)
    {
        AppbarSetAutoHideBar(bAutoHide);
    }

    private static uint RegisterCallbackMessage()
    {
        string uniqueMessageString = Guid.NewGuid().ToString();
        return Win32Invokes.RegisterWindowMessage(uniqueMessageString);
    }

    private void SizeAppBar()
    {
        Win32Structs.RECT rt = new();

        if ((m_Edge == AppBarEdges.Left) ||
            (m_Edge == AppBarEdges.Right))
        {
            rt.Top = 0;
            rt.Bottom = SystemInformation.PrimaryMonitorSize.Height;
            if (m_Edge == AppBarEdges.Left)
            {
                rt.Right = m_PrevSize.Width;
            }
            else
            {
                rt.Right = SystemInformation.PrimaryMonitorSize.Width;
                rt.Left = rt.Right - m_PrevSize.Width;
            }
        }
        else
        {
            rt.Left = 0;
            rt.Right = SystemInformation.PrimaryMonitorSize.Width;
            if (m_Edge == AppBarEdges.Top)
            {
                rt.Bottom = m_PrevSize.Height;
            }
            else
            {
                rt.Bottom = SystemInformation.PrimaryMonitorSize.Height;
                rt.Top = rt.Bottom - m_PrevSize.Height;
            }
        }

        AppbarQueryPos(ref rt);

        switch (m_Edge)
        {
            case AppBarEdges.Left:
                rt.Right = rt.Left + m_PrevSize.Width;
                break;
            case AppBarEdges.Right:
                rt.Left = rt.Right - m_PrevSize.Width;
                break;
            case AppBarEdges.Top:
                rt.Bottom = rt.Top + m_PrevSize.Height;
                break;
            case AppBarEdges.Bottom:
                rt.Top = rt.Bottom - m_PrevSize.Height;
                break;
            case AppBarEdges.Float:
                break;
            default:
                rt.Bottom = rt.Top + m_PrevSize.Height;
                break;
        }

        AppbarSetPos(ref rt);

        Location = new(rt.Left, rt.Top);
        Size = new(rt.Right - rt.Left, rt.Bottom - rt.Top);
    }

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

        _currentLabel.Tag = false;
        _currentLabel.Appearance.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Window);
        _currentLabel.Appearance.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.WindowText);
        _currentLabel = null;
    }

    /// <summary>
    /// Ein Popup anzeigen...
    /// </summary>
    /// <param name="popup"></param>
    /// <param name="caption"></param>
    public void ShowPopup(AppBarPopup popup, AFLabel caption)
    {
        HidePopup();

        _currentPopup = popup;
        _currentLabel = caption;

        _currentLabel.Appearance.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Highlight);
        _currentLabel.Appearance.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.HighlightText);
        _currentLabel.Tag = true;

        popup.ShowPopup(this, caption);
    }

    #region Message Handlers

    private void OnAppbarNotification(ref Message msg)
    {
        AppBarStates state;
        AppBarNotifications msgType = (AppBarNotifications)(int)msg.WParam;

        switch (msgType)
        {
            case AppBarNotifications.PosChanged:
                SizeAppBar();
                break;

            case AppBarNotifications.StateChange:
                state = AppbarGetTaskbarState();
                if ((state & AppBarStates.AlwaysOnTop) != 0)
                {
                    TopMost = true;
                    BringToFront();
                }
                else
                {
                    TopMost = false;
                    SendToBack();
                }
                break;

            case AppBarNotifications.FullScreenApp:
                if ((int)msg.LParam != 0)
                {
                    TopMost = false;
                    SendToBack();
                }
                else
                {
                    state = AppbarGetTaskbarState();
                    if ((state & AppBarStates.AlwaysOnTop) != 0)
                    {
                        TopMost = true;
                        BringToFront();
                    }
                    else
                    {
                        TopMost = false;
                        SendToBack();
                    }
                }

                break;

            case AppBarNotifications.WindowArrange:
                if ((int)msg.LParam != 0)	// before
                    Visible = false;
                else						// after
                    Visible = true;

                break;
        }
    }

    private void OnNcHitTest(ref Message msg)
    {
        DefWndProc(ref msg);
        
        bool go;

        if ((m_Edge == AppBarEdges.Top) && ((int)msg.Result == (int)MousePositionCodes.HTBOTTOM))
            go = true;
        else if ((m_Edge == AppBarEdges.Bottom) && ((int)msg.Result == (int)MousePositionCodes.HTTOP))
            go = true;
        else if ((m_Edge == AppBarEdges.Left) && ((int)msg.Result == (int)MousePositionCodes.HTRIGHT))
            go = true;
        else if ((m_Edge == AppBarEdges.Right) && ((int)msg.Result == (int)MousePositionCodes.HTLEFT))
            go = true;
        else if ((int)msg.Result == (int)MousePositionCodes.HTCLOSE)
            go = true;
        else
        {
            msg.Result = (IntPtr)MousePositionCodes.HTCLIENT;
            return;
        }

        if (go)
            base.WndProc(ref msg);
    }


    #endregion Message Handlers

#pragma warning disable 1591
    #region Window Procedure
    protected override void WndProc(ref Message msg)
    {
        if (IsAppbarMode)
        {
            if (msg.Msg == CallbackMessageID)
            {
                OnAppbarNotification(ref msg);
            }
            else if (msg.Msg == (int)Win32Enums.WM.ACTIVATE)
            {
                AppbarActivate();
            }
            else if (msg.Msg == (int)Win32Enums.WM.WINDOWPOSCHANGED)
            {
                AppbarWindowPosChanged();
            }
            else if (msg.Msg == (int)Win32Enums.WM.NCHITTEST)
            {
                OnNcHitTest(ref msg);
                return;
            }
        }

        base.WndProc(ref msg);
    }

    #endregion Window Procedure

    protected override void OnLoad(EventArgs e)
    {
        m_PrevSize = Size;
        m_PrevLocation = Location;
        base.OnLoad(e);
    }

    /// <inheritdoc />
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        AppbarRemove();
        base.OnFormClosing(e);
    }

    protected override void OnSizeChanged(EventArgs e)
    {
        if (IsAppbarMode)
        {
            if (m_Edge == AppBarEdges.Top || m_Edge == AppBarEdges.Bottom)
                m_PrevSize.Height = m_PrevSize.Height > 0 ? m_PrevSize.Height : Size.Height;
            else
                m_PrevSize.Width = m_PrevSize.Width > 0 ? m_PrevSize.Width : Size.Width;

            SizeAppBar();
        }

        base.OnSizeChanged(e);
    }
    

    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AppBarEdges Edge
    {
        get => m_Edge;
        set
        {
            m_Edge = value;
            if (value == AppBarEdges.Float)
                AppbarRemove();
            else
                AppbarNew();

            if (IsAppbarMode)
                SizeAppBar();
        }
    }
#pragma warning restore 1591


}