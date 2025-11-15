namespace AF.MVC;

/// <summary>
/// Abstrakte Basisklasse einer MVCAnwendung für WinForms
/// </summary>
public abstract class AFWinFormsMVCApp : AFWinFormsDXApp
{
    /// <summary>
    /// Konstruktor der MVC Anwendung
    /// </summary>
    /// <param name="setup">Setup/Einstellungen für die Anwendung</param>
    protected AFWinFormsMVCApp(WinFormsMVCAppSetup setup) : base(setup)
    {

    }

    /// <summary>
    /// Zugriff auf das Hauptfenster der Anwendung (Shell)
    /// </summary>
    public new IShellMVC Shell 
    {
        get => (IShellMVC)GetShell()!;
        set => SetShell(value); 
    }

    /// <inheritdoc />
    public override IViewManager? ViewManager => Shell.ViewManager;
}

/// <summary>
/// Setup/Einstellungen für eine WinForms MVC Anwendung
/// </summary>
public class WinFormsMVCAppSetup : WinFormsDXAppSetup
{
   
}

/// <summary>
/// Interface für eine Shell mit MVC-Unterstützung
/// </summary>
public interface IShellMVC : IShell
{
    /// <summary>
    /// ViewManager für die MVC Anwendung.
    /// </summary>
    AFViewManager ViewManager { get; }

    /// <summary>
    /// Einen neuen Toast anzeigen.
    /// </summary>
    /// <param name="type">Typ der Benachrichtigung</param>
    /// <param name="caption">Überschrift</param>
    /// <param name="description">Mitteilung (kann HTML enthalten)</param>
    /// <param name="linkaction">auszuführende Aktion</param>
    /// <param name="link">ModelLink auf den hingewiesen wird</param>
    /// <param name="timeoutSeconds">Timeout, standard = 15</param>
    void ShowToast(eNotificationType type, string caption, string description, Action<ModelLink>? linkaction = null, ModelLink? link = null, int timeoutSeconds = 15);
}
