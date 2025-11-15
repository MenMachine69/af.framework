namespace AF.MVC;

/// <summary>
/// Filter-Plugin für Filter in ModelBrowser, DetailViews etc.
/// </summary>
public interface IViewFilterPlugin : IViewDetailHeader
{
    /// <summary>
    /// gibt den Filterstring und die Parameter für den Filter als Abfrage zurück
    /// </summary>
    /// <param name="parameters">Parameter</param>
    /// <returns>die Filterzeichenkette</returns>
    string GetFilterString(out object[] parameters);

    /// <summary>
    /// Ereignis, das ausgelöst wird, wenn der Filter geändert wird
    /// </summary>
    event EventHandler FilterChanged;
}