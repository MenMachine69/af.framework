namespace AF.WINFORMS.DX;

/// <summary>
/// Navigator for setting like dialogs with pages...
/// </summary>
[ToolboxItem(true)]
[SupportedOSPlatform("windows")]
public partial class AFSettingsNavigation : AFUserControl
{
    private bool smallMode;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFSettingsNavigation()
    {
        InitializeComponent();

        if (UI.DesignMode) return;

        content.BringToFront();
        content.Dock = DockStyle.Fill;
    }

    /// <inheritdoc />
    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        if (UI.DesignMode) return;

        if (Width < 600 && smallMode) return;

        if (Width >= 600 && !smallMode) return;

        SuspendLayout();

        smallMode = !smallMode;

        // sitch to small mode...
        if (smallMode)
        {
            flyout.Controls.Add(navigation.Controls[0]);
            navigation.Controls.Clear();
        }
        else
        {
            navigation.Controls.Add(flyout.Controls[0]);    
            flyout.Controls.Clear();
        }

        navigation.Visible = !smallMode;
        
        ResumeLayout();
    }

    /// <summary>
    /// Control, dass als Navigator verwendet wird.
    /// </summary>
    /// <param name="ctrl">Control</param>
    public void SetNavigator(Control ctrl)
    {
        ctrl.Dock = DockStyle.Fill;
        navigation.Padding = new(5);
        navigation.Controls.Add(ctrl);
    }
}

