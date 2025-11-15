namespace AF.WINFORMS.DX;

/// <summary>
/// Splash-Plugin zur Mandantenauswahl
/// </summary>
[ToolboxItem(false)]
[SupportedOSPlatform("windows")]
public partial class SplashPluginMandant : SplashPluginBase
{
    /// <summary>
    /// Constructor
    /// </summary>
    public SplashPluginMandant()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="files">Liste der auswählbaren Mandanten</param>
    public SplashPluginMandant(IEnumerable<FileInfo> files)
    {
        InitializeComponent();

        if (UI.DesignMode) return;

        lstMandanten.Appearance.BackColor = base.BackColor;

        List<ListItem> items = [];
        foreach (var file in files)
            items.Add(new ListItem { Caption = @" " + file.Name.Replace(file.Extension, ""), Value = file });

        lstMandanten.Fill(items);
        lstMandanten.SelectedIndex = 0;
        lstMandanten.ItemHeight = UI.GetScaled(30);

        pshSelect.Appearance.BackColor = DevExpress.LookAndFeel.DXSkinColors.FillColors.Success;
        pshSelect.Click += (_, _) => 
        {
            if (lstMandanten.SelectedIndex >= 0)
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
    /// Der ausgewählte Mandant.
    /// </summary>
    public FileInfo? SelectedMandant => ((ListItem)lstMandanten.SelectedItem).Value as FileInfo;
}

