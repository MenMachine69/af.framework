using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Plugin zur Anmeldung eines Benutzers
/// </summary>
[ToolboxItem(false)]
[SupportedOSPlatform("windows")]
public partial class SplashPluginLogin : SplashPluginBase
{
    LoginOptions _options = new();

    /// <summary>
    /// Constructor
    /// </summary>
    public SplashPluginLogin()
    {
        InitializeComponent();

        if (UI.DesignMode) return;

        pshLogin.Appearance.BackColor = DevExpress.LookAndFeel.DXSkinColors.FillColors.Success;
        pshCancel.Appearance.BackColor = DevExpress.LookAndFeel.DXSkinColors.FillColors.Danger;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="action">Bei Anmeldung auszuführende Aktion (Methode/Delegate)</param>
    /// <param name="options">Optiopnen für die Anmeldung</param>
    public SplashPluginLogin(Func<LoginOptions, bool> action, LoginOptions options)
    {
        InitializeComponent();

        if (UI.DesignMode) return;
        _options = options;
        ActionLogin = action;
        sleLoginName.Text = Environment.UserName;

        pshLogin.Appearance.BackColor = DevExpress.LookAndFeel.DXSkinColors.FillColors.Success;
        pshCancel.Appearance.BackColor = DevExpress.LookAndFeel.DXSkinColors.FillColors.Danger;

        slePassword.Properties.Buttons[0].Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph;
        slePassword.Properties.Buttons[0].ImageOptions.SvgImage = Glyphs.GetImage(Symbol.EyeShow);
        slePassword.Properties.Buttons[0].ImageOptions.SvgImageSize = new(12,12);
    }

    /// <summary>
    /// Bei Anmeldung auszuführende Aktion (Methode/Delegate)
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Func<LoginOptions, bool>? ActionLogin { get; set; }

    private void _tooglePwdDisplay(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        if (sender is ButtonEdit editor)
        {
            editor.Properties.UseSystemPasswordChar =
                !editor.Properties.UseSystemPasswordChar;

            editor.Properties.Buttons[0].ImageOptions.SvgImage = editor.Properties.UseSystemPasswordChar 
                ? Glyphs.GetImage(Symbol.EyeShow)
                : Glyphs.GetImage(Symbol.EyeHide);
        }
    }

    private void pshLogin_Click(object sender, EventArgs e)
    {
        _options.LoginName = sleLoginName.Text;
        _options.Password = slePassword.Text;

        if (ActionLogin != null && ActionLogin.Invoke(_options))
            Result = DialogResult.OK;
        else
            ((FormSplash)FindForm()!).FlyoutManager.ShowMessage(_options.Feedback, _options.FeedbackType, 3);

    }

    private void pshCancel_Click(object sender, EventArgs e)
    {
        Result = DialogResult.Cancel;
    }
}

