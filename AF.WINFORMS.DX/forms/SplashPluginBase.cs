using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Basisklasse der Plugins für den Splash-Dialog
/// </summary>
[ToolboxItem(false)]
[SupportedOSPlatform("windows")]
public class SplashPluginBase : XtraUserControl
{
    /// <summary>
    /// Ergebnis des Plugins 
    /// 
    /// Solange dieses Ergebnis DialogResult.None ist, wird das Plugin angezeigt.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DialogResult Result { get; set; } = DialogResult.None;

    /// <summary>
    /// Methode, die vom SplashDialog aller 10ms aufgerufen wird
    /// </summary>
    public virtual void UpdateGui()
    {
    }

    /// <summary>
    /// Method, die nach dem Anzeigen des Plugins ausgeführt wird.
    /// </summary>
    public virtual void AfterShow()
    {
    }

    /// <summary>
    /// Delegate, über den das Plugin den SplashScreen benachrichtigen kann...
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Action<string>? Feedback { get; set; }
}

