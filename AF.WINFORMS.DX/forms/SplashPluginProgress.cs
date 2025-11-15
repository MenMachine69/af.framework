namespace AF.WINFORMS.DX;

/// <summary>
/// Plugin zur Anmeldung eines Benutzers
/// </summary>
[ToolboxItem(false)]
[SupportedOSPlatform("windows")]
public partial class SplashPluginProgress : SplashPluginBase
{
    /// <summary>
    /// Constructor
    /// </summary>
    public SplashPluginProgress()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Schrittbeschreibung im Fortschritt anzeigen
    /// </summary>
    /// <param name="progress"></param>
    public void ShowProgress(string progress)
    {
        lblMessage.Text = progress;
        Application.DoEvents();
    }

}

