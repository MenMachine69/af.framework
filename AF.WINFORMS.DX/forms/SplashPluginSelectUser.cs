using AF.BUSINESS;

namespace AF.WINFORMS.DX;

/// <summary>
/// Plugin zur Auswahl eines alternativen Benutzeraccounts.
///
/// Administratoren können so eine Anmeldung mit einem bestimmten Account erzwingen.
/// </summary>
[ToolboxItem(false)]
[SupportedOSPlatform("windows")]
public partial class SplashPluginSelectUser : SplashPluginBase
{
    /// <summary>
    /// Constructor
    /// </summary>
    public SplashPluginSelectUser()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public SplashPluginSelectUser(IEnumerable<IUser> users)
    {
        InitializeComponent();

        if (UI.DesignMode) return;

        lstUsers.Appearance.BackColor = base.BackColor;

        List<ListItem> items = [];

        foreach (var user in users.OrderBy(u => u.UserLoginName.ToUpper()))
            items.Add(new ListItem { Caption = @" " + user.UserLoginName, Value = user });

        lstUsers.Fill(items);
        lstUsers.SelectedIndex = 0;
        lstUsers.ItemHeight = UI.GetScaled(30);

        pshSelect.Appearance.BackColor = DevExpress.LookAndFeel.DXSkinColors.FillColors.Success;
        pshSelect.Click += (_, _) => 
        {
            if (lstUsers.SelectedIndex >= 0)
            {
                Result = DialogResult.OK;
                return;
            }

            ((FormSplash)FindForm()!).FlyoutManager.ShowMessage(WinFormsStrings.ERR_NOCLIENTSELECTED,
                eNotificationType.Error, 5);
        };
        pshCancel.Appearance.BackColor = DevExpress.LookAndFeel.DXSkinColors.FillColors.Danger;
        pshCancel.Click += (_, _) => { Result = DialogResult.Cancel; };
    }

    /// <summary>
    /// Der ausgewählte Benutzer...
    /// </summary>
    public IUser? SelectedUser => ((ListItem)lstUsers.SelectedItem).Value as IUser;
}

