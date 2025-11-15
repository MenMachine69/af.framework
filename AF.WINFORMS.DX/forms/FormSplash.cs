using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Splash, which can be displayed at the start of the application and which, by means of plugins
/// various functionalities (client selection, login etc.)
/// 
/// The dialogue should be initialised with an image 660 * 400.
/// </summary>
public partial class FormSplash : XtraForm
{
    /// <summary>
    /// Konstruktor
    /// </summary>
    public FormSplash()
    {
        InitializeComponent();

        AutoScaleMode = AutoScaleMode.Font;
        TopMost = true;
    }

    /// <summary>
    /// Konstruktor mit der Angabe eines Hintergrundbildes, dass im Splash verwendet werden soll.
    /// </summary>
    /// <param name="backgrd"></param>
    public FormSplash(Image backgrd)
    {
        InitializeComponent();

        if (UI.DesignMode) return;

        //LookAndFeel.SkinName = GuiLayer.DarkSkin;
        //LookAndFeel.UseDefaultLookAndFeel = false;
        //Size = new Size(UI.GetScaled(backgrd.Width), UI.GetScaled(backgrd.Height));
        base.BackgroundImage = backgrd;
        base.BackgroundImageLayout = ImageLayout.Stretch;

        UI.Splash = this;

        ShowPlugin(new SplashPluginProgress());

        ShowMessage(WinFormsStrings.LBL_WAIT.ToUpper());
        ShowProgress(WinFormsStrings.LBL_APPLOADING);
    }

    /// <summary>
    /// FlyoutManager of the form
    ///
    /// Can be used to display messages e.g.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFFlyoutManager FlyoutManager => crFlyoutManager1;

    /// <summary>
    /// Progress-Display
    /// </summary>
    /// <param name="progress">message to show in progress (current step e.g.)</param>
    public void ShowProgress(string progress)
    {
        if (CurrentPlugin is not SplashPluginProgress)
            ShowPlugin(new SplashPluginProgress());

        (CurrentPlugin as SplashPluginProgress)?.ShowProgress(progress);
    }


    /// <summary>
    /// show a message in the splash
    /// </summary>
    /// <param name="msg">message to display</param>
    public void ShowMessage(string msg)
    {
        if (lblMessage != null)
            lblMessage.Text = msg;

        Application.DoEvents();
    }


    /// <summary>
    /// show the version of the app in the splash
    /// </summary>
    /// <param name="version">version to display</param>
    public void ShowVersion(string version)
    {
        if (lblVersion != null)
            lblVersion.Text = version;

        Application.DoEvents();
    }

    /// <summary>
    /// Label to display the version
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public LabelControl Version => lblVersion;


    /// <summary>
    /// Displaying a message in the footer
    /// 
    /// Attention! The passed colours are kept until another colour is passed!
    /// </summary>
    /// <param name="msg">Message to be displayed</param>.
    /// <param name="backcolor">Background colour</param>.
    /// <param name="forecolor">foreground colour</param>
    public void ShowMessage(string msg, Color backcolor, Color forecolor)
    {
        lblMessage.Text = msg;
        lblMessage.Appearance.BackColor = backcolor;
        lblMessage.Appearance.ForeColor = forecolor;
        Application.DoEvents();
    }


    /// <summary>
    /// Display a plugin and wait until this plugin returns as result != DialogResult.None.
    /// </summary>
    /// <param name="plugin">Plugin to be displayed</param>.
    /// <returns>result of the plugin</returns>.
    public DialogResult ShowPlugionModal(SplashPluginBase plugin)
    {
        if (panelPlugin.Controls.Count > 0)
            panelPlugin.Controls.Clear(true);

        plugin.Dock = DockStyle.Fill;
        panelPlugin.Controls.Add(plugin);
        panelPlugin.Visible = true;

        plugin.AfterShow();

        BringToFront();

        while (plugin.Result == DialogResult.None)
        {
            Thread.Sleep(30);
            plugin.UpdateGui();
            Application.DoEvents();
        }

        panelPlugin.Visible = false;

        return plugin.Result;
    }

    /// <summary>
    /// Show a plugin (non-modal)
    /// </summary>
    /// <param name="plugin">plugin to display</param>.
    public void ShowPlugin(SplashPluginBase plugin)
    {
        if (panelPlugin.Controls.Count > 0)
            panelPlugin.Controls.Clear(true);

        plugin.Dock = DockStyle.Fill;
        panelPlugin.Controls.Add(plugin);
        panelPlugin.Visible = true;
    }

    /// <summary>
    /// The currently displayed plugin
    /// </summary>
    public SplashPluginBase? CurrentPlugin => panelPlugin.Controls.Count > 0
        ? (SplashPluginBase)panelPlugin.Controls[0]
        : null;

    /// <summary>
    /// Show the dialog
    /// </summary>
    public new void Show()
    {
        TopMost = true;
        ShowInTaskbar = false;
        ShowIcon = false;
        FormBorderStyle = FormBorderStyle.None;
        MinimizeBox = false;
        MaximizeBox = false;
        ControlBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Opacity = 0;

        if (UI.DesignMode)
            base.Show();
        else
        {
            base.Show();
            _fadeIn();
        }
    }

    /// <summary>
    /// Close the dialog
    /// </summary>
    public new void Close()
    {
        if (UI.DesignMode)
            base.Close();
        else
        {
            _fadeOut();
            base.Close();
        }
    }

    private void _fadeIn()
    {
        Opacity = 0;

        while (Opacity < 1)
        {
            Application.DoEvents();
            Thread.Sleep(25);
            Opacity += 0.1;
        }
    }

    private void _fadeOut()
    {
        Opacity = 1;

        while (Opacity > 0)
        {
            Application.DoEvents();
            Thread.Sleep(25);
            Opacity -= 0.1;
        }
    }


    /// <summary>
    /// Interception of standard messages and processing of these messages...
    ///
    /// Using this method, the form can be moved by clicking on the form
    /// </summary>
    /// <param name="m"></param>
    protected override void WndProc(ref Message m)
    {
        var handled = false;

        if (m.Msg == 0x84)
        {
            m.Result = (IntPtr)2; // HTCAPTION
            handled = true;
        }

        if (handled)
            return;

        base.WndProc(ref m);
    }

    /// <summary>
    /// Set the form to the topmost position (true) or remove the topmost position (false)
    /// </summary>
    /// <param name="set">true to set topmost, false to remove topmost</param>
    public void ChangeTopMost(bool set)
    {
        Win32Invokes.SetWindowPos(
            Handle,
            new IntPtr(set ? -1 : -2),
            Location.X,
            Location.Y,
            Width,
            Height,
            (uint)(Win32Enums.SetWindowPosOptions.SWP_NOMOVE & Win32Enums.SetWindowPosOptions.SWP_NOSIZE)
        );
    }

    /// <summary>
    /// Display mandant selection and return selected mandant
    /// </summary>
    /// <param name="files">available mandant configurations</param>.
    /// <returns>selected mandant (file) or null if cancelled</returns>.
    public FileInfo? SelectMandant(FileInfo[] files)
    {
        FileInfo? ret;

        ShowMessage(WinFormsStrings.LBL_SELECTMANDANT);

        SplashPluginMandant plugin = new(files);
        if (ShowPlugionModal(plugin) == DialogResult.OK)
        {
            ShowMessage(WinFormsStrings.LBL_CLIENTLOADING);

            ret = plugin.SelectedMandant;
        }
        else
            ret = null;

        return ret;
    }

    /// <summary>
    /// User login
    /// </summary>
    /// <param name="action">method to execute when user clicks LOGIN</param>.
    /// <param name="options">options for login</param>
    /// <returns>true if successful, false otherwise</returns>
    public bool Login(Func<LoginOptions, bool> action, LoginOptions options)
    {
        // wenn Business enabled und anonyme Anmeldung erlaubt ist,
        // wird die Anmeldemethode einmal aufgerufen, um dem Programm die Chance
        // zu geben das unattended Login durchzuführen  
        if (options.TryUnattendedLogin)
        {
            options.LoginName = Environment.UserName;
            return action.Invoke(options);
        }

        ShowMessage(WinFormsStrings.LBL_LOGIN.ToUpper());

        SplashPluginLogin plugin = new(action, options);
        return ShowPlugionModal(plugin) == DialogResult.OK;
    }

    /// <summary>
    /// Ask user for new administrator 
    /// </summary>
    /// <returns>true if successful, false otherwise</returns>
    public UserCreateOptions? CreateAdmin(Func<string, string, bool>? checkCredentials)
    {
        ShowMessage(WinFormsStrings.LBL_CREATEADMIN.ToUpper());

        SplashPluginCreateAdmin plugin = new();
        plugin.CustomCheckCredentials = checkCredentials;

        return ShowPlugionModal(plugin) == DialogResult.OK ? plugin.Model : null;
    }
}