namespace AF.WINFORMS.DX;

/// <summary>
/// 
/// </summary>
public partial class FormSettings : DevExpress.XtraEditors.XtraForm
{
    /// <summary>
    /// 
    /// </summary>
    public FormSettings()
    {
        InitializeComponent();

        if (UI.DesignMode) return;

        Controls.Add(new AFSettingsNavigation { Dock = DockStyle.Fill });
    }
}