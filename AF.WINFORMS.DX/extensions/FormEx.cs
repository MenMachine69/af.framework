using AF.MVC;

namespace AF.WINFORMS.DX;

/// <summary>
/// Extenmsuion methods for forms
/// </summary>
public static class FormEx
{
    /// <summary>
    /// Handle a command result
    /// </summary>
    /// <param name="result">result</param>
    /// <param name="form">form</param>
    public static void HandleResult(this Form form, CommandResult? result)
    {
        if (result == null) return;

        if (form is not ICommandResultDisplay resultform) return;
            resultform.HandleResult(result);
    }
}

