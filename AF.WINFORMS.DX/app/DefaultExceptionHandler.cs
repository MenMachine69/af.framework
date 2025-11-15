namespace AF.WINFORMS.DX;

/// <summary>
/// Ausnahmebehandlung für nicht behandelte Ausnahmen/Exceptions
/// </summary>
public class DefaultExceptionHandler : ExceptionHandler
{
    /// <summary>
    /// Ausnahmen behandeln.
    ///
    /// Neben der standardmässigen Benachrichtigung der Beobachter wird ein Dialog angezeigt.
    /// </summary>
    /// <param name="exception"></param>
    public override void HandleException(Exception exception)
    {
        base.HandleException(exception);

        using FormExceptionMessage dlg = new FormExceptionMessage(new ExceptionInfo(exception));
        dlg.ShowDialog();
    }
}