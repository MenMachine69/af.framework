namespace AF.WINFORMS.DX;

/// <summary>
/// interface for the main application window in a WinFormsDX application
/// </summary>
public interface IShell : IOverlayHost
{
    /// <summary>
    /// Close the shell/main application form
    /// </summary>
    void Close();

    /// <summary>
    /// Show about dialog 
    /// </summary>
    void ShowAbout();

    /// <summary>
    /// The OverlayDisplayManager of the shell/main application form
    ///
    /// This can be used to show overlays in the main application form.
    /// for example: App.Shell.OverlayDisplayManager.ShowOverlay(...)
    /// </summary>
    AFOverlayDisplayManager OverlayDisplayManager { get; }

    /// <summary>
    /// The FlyoutManager of the shell/main application form
    ///
    /// This can be used to show flyouts in the main application form.
    /// for example: App.Shell.FlyoutManager.ShowMessage(...)
    ///
    /// You can also use shortcuts like App.Shell.ShowErrorMessage(...)
    /// </summary>
    AFFlyoutManager FlyoutManager { get; }

    /// <summary>
    /// Eine Systemnachricht an die Shell senden, damit diese von der Shell verarbeitet werden kann.
    /// </summary>
    /// <param name="message"></param>
    void PushMessage(ISystemMessage message);

}