using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Plugin zur Anmeldung eines Benutzers
/// </summary>
[ToolboxItem(false)]
[SupportedOSPlatform("windows")]
public partial class SplashPluginCreateAdmin : SplashPluginBase
{
    /// <summary>
    /// Constructor
    /// </summary>
    public SplashPluginCreateAdmin()
    {
        InitializeComponent();

        if (UI.DesignMode == false)
        {
            sleLoginName.Text = Environment.UserName;

            pshCreate.Appearance.BackColor = DevExpress.LookAndFeel.DXSkinColors.FillColors.Success;
            pshCancel.Appearance.BackColor = DevExpress.LookAndFeel.DXSkinColors.FillColors.Danger;

            slePassword.Properties.Buttons[0].Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph;
            slePassword.Properties.Buttons[0].ImageOptions.SvgImage = Glyphs.GetImage(Symbol.EyeShow);
            slePassword.Properties.Buttons[0].ImageOptions.SvgImageSize = new(12,12);

            slePasswordRepeat.Properties.Buttons[0].Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph;
            slePasswordRepeat.Properties.Buttons[0].ImageOptions.SvgImage = Glyphs.GetImage(Symbol.EyeShow);
            slePasswordRepeat.Properties.Buttons[0].ImageOptions.SvgImageSize = new(12,12);
        }
    }

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

    /// <summary>
    /// Custom-Function zur Prüfung der eingegebenen Credentials.
    ///
    /// Funktion muss true liefern, damit die Credentials gültig sind.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Func<string, string, bool>? CustomCheckCredentials { get; set; }

    private void pshCreate_Click(object sender, EventArgs e)
    {
        errprovider.ClearErrors();

        if (sleLoginName.Text.Trim().Length < 5)
            errprovider.SetError(sleLoginName, WinFormsStrings.ERR_LOGINNAMETOSHORT);

        if (slePassword.Text.Trim().Length < 8)
            errprovider.SetError(slePassword, WinFormsStrings.ERR_PASSWORDTOSHORT);

        if (slePassword.Text != slePasswordRepeat.Text)
            errprovider.SetError(slePasswordRepeat, WinFormsStrings.ERR_WRONGPASSWORDREPEAT);

        if (errprovider.HasErrors)
            return;

        if (CustomCheckCredentials != null)
        {
            if (CustomCheckCredentials(sleLoginName.Text, slePassword.Text) == false)
                return;
        }


        Model = new()
        {
            LoginName = sleLoginName.Text,
            Password = slePassword.Text,
            Success = true
        };

        Result = DialogResult.OK;

    }

    /// <summary>
    /// Optionen für den anzulegenden Admin
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public UserCreateOptions? Model { get; private set; }

    private void pshCancel_Click(object sender, EventArgs e)
    {
        Result = DialogResult.Cancel;
    }
}

