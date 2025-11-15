namespace AF.WINFORMS.DX;

/// <summary>
/// Display messages
/// </summary>
public static class MsgBox
{
    /// <summary>
    /// Show message as info with Ok button
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="moreinfo">additional information</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowInfoOk(string message, string moreinfo)
    {
        return ShowInfoOk(null, message, moreinfo, 0);
    }

    /// <summary>
    /// Show message as info with Ok button
    /// </summary>
    /// <param name="message">message text</param>.
    /// <param name="moreinfo">additional information</param>.
    /// <param name="plugin">plugin that should be displayed in the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowInfoOk(Control? plugin, string message, string moreinfo)
    {
        return ShowInfoOk(plugin, message, moreinfo, 0);
    }

    /// <summary>
    /// Show message as info with Ok button
    /// </summary>
    /// <param name="message">message text</param>.
    /// <param name="moreinfo">additional information</param>.
    /// <param name="messageid">ID of the message</param>
    /// <param name="owner">window that is considered the owner of the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowInfoOk(string message, string moreinfo, int messageid, IWin32Window? owner)
    {
        return ShowInfoOk(null, message, moreinfo, messageid, owner);
    }

    /// <summary>
    /// Show message as info with Ok button
    /// </summary>
    /// <param name="message">message text</param>.
    /// <param name="owner">window that is considered the owner of the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowInfoOk(string message, IWin32Window? owner)
    {
        return ShowInfoOk(null, message, string.Empty, 0, owner);
    }

    /// <summary>
    /// Show message as info with Ok button
    /// </summary>
    /// <param name="message">message text</param>.
    /// <param name="moreinfo">additional information</param>.
    /// <param name="owner">window that is considered the owner of the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowInfoOk(string message, string moreinfo, IWin32Window? owner)
    {
        return ShowInfoOk(null, message, moreinfo, 0, owner);
    }

    /// <summary>
    /// Show message as info with Ok button
    /// </summary>
    /// <param name="message">message text</param>.
    /// <param name="moreinfo">additional information</param>.
    /// <param name="messageid">ID of the message</param>
    /// <param name="owner">window that is considered the owner of the message</param>.
    /// <param name="plugin">Plugin to be displayed in the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowInfoOk(Control? plugin, string message, string moreinfo, int messageid,
        IWin32Window? owner)
    {
        eMessageBoxResult ret;

        bool found = false;

        ret = _RestoreAnswer(messageid, ref found);

        if (found)
            return ret;

        return _show(plugin, owner, message, moreinfo, WinFormsStrings.INFORMATION, eMessageBoxButton.OK,
            eMessageBoxIcon.Information, eMessageBoxDefaultButton.Button1, messageid);
    }

    /// <summary>
    /// Show message as info with Ok button
    /// </summary>
    /// <param name="message">message text</param>.
    /// <param name="moreinfo">additional information</param>.
    /// <param name="messageid">ID of the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowInfoOk(string message, string moreinfo, int messageid)
    {
        return ShowInfoOk(null, message, moreinfo, messageid, null);
    }

    /// <summary>
    /// Show message as info with Ok button
    /// </summary>
    /// <param name="message">message text</param>.
    /// <param name="moreinfo">additional information</param>.
    /// <param name="messageid">ID of the message</param>
    /// <param name="plugin">Plugin that should be displayed in the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowInfoOk(Control? plugin, string message, string moreinfo, int messageid)
    {
        return ShowInfoOk(plugin, message, moreinfo, messageid, null);
    }

    /// <summary>
    /// Show message as info with Ok button
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="plugin">Plugin to be displayed in the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowInfoOk(Control plugin, string message)
    {
        return ShowInfoOk(plugin, message, "", 0);
    }

    /// <summary>
    /// Show message as info with Ok button
    /// </summary>
    /// <param name="message">Text of the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowInfoOk(string message)
    {
        return ShowInfoOk(null, message, "", 0);
    }

    /// <summary>
    /// Meldung als Info mit Schaltfläche Ok anzeigen
    /// </summary>
    /// <param name="message">Text der Meldung</param>
    /// <param name="messageid">ID der Nachricht</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowInfoOk(string message, int messageid)
    {
        return ShowInfoOk(null, message, "", messageid);
    }

    /// <summary>
    /// Show message as info with Ok button
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="messageid">ID of the message</param>.
    /// <param name="plugin">Plugin to be displayed in the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowInfoOk(Control? plugin, string message, int messageid)
    {
        return ShowInfoOk(plugin, message, "", messageid);
    }

    /// <summary>
    /// Show message as error with Ok button
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="moreinfo">more information</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowErrorOk(string message, string moreinfo)
    {
        return ShowErrorOk(null, message, moreinfo, 0);
    }

    /// <summary>
    /// Show message as error with Ok button
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="moreinfo">additional information</param>.
    /// <param name="plugin">plugin that should be displayed in the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowErrorOk(Control? plugin, string message, string moreinfo)
    {
        return ShowErrorOk(plugin, message, moreinfo, 0);
    }

    /// <summary>
    /// Show message as error with Ok button
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="moreinfo">additional information</param>.
    /// <param name="messageid">text of the message</param>.
    /// <param name="owner">owner of the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowErrorOk(string message, string moreinfo, int messageid, IWin32Window? owner)
    {
        return ShowErrorOk(null, message, moreinfo, messageid, owner);
    }

    /// <summary>
    /// Show message as error with Ok button
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="owner">owner of the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowErrorOk(string message, IWin32Window? owner)
    {
        return ShowErrorOk(null, message, string.Empty, 0, owner);
    }

    /// <summary>
    /// Show message as error with Ok button
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="moreinfo">additional information</param>.
    /// <param name="owner">owner of the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowErrorOk(string message, string moreinfo, IWin32Window? owner)
    {
        return ShowErrorOk(null, message, moreinfo, 0, owner);
    }

    /// <summary>
    /// Show message as error with Ok button
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="moreinfo">additional information</param>.
    /// <param name="messageid">text of the message</param>.
    /// <param name="owner">owner of the message</param>.
    /// <param name="plugin">Plugin that should be displayed in the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowErrorOk(Control? plugin, string message, string moreinfo, int messageid,
        IWin32Window? owner)
    {
        bool found = false;

        eMessageBoxResult ret = _RestoreAnswer(messageid, ref found);

        if (found)
            return ret;

        return _show(plugin, owner, message, moreinfo, WinFormsStrings.ERROR, eMessageBoxButton.OK, eMessageBoxIcon.Error,
            eMessageBoxDefaultButton.Button1, messageid);
    }

    private static eMessageBoxResult _show(Control? plugin, IWin32Window? owner, string message, string moreinfo,
        string caption, eMessageBoxButton buttons, eMessageBoxIcon icon, eMessageBoxDefaultButton def, int messageid)
    {
        eMessageBoxResult ret;
        
        if (owner == null)
        {
            try
            {
                owner = UI.Shell as Form;
            }
            catch
            {
                owner = null;
            }
        }


        using _formMessageBox dlg = new(plugin, message, moreinfo, caption, buttons, icon, def, messageid > 0);

        dlg.MessageID = messageid;
        
        if (owner is Form { WindowState: FormWindowState.Minimized })
            dlg.StartPosition = FormStartPosition.CenterScreen;

        ret = owner != null ? dlg.ShowDialog(owner) : dlg.ShowDialog();

        return ret;
    }

    private static eMessageBoxResult _RestoreAnswer(int messageid, ref bool found)
    {
        eMessageBoxResult answer = eMessageBoxResult.Ignore;

        if (messageid > 0 && ((AFWinFormsDXApp)AFCore.App).DefaultMsgAnswerStorage.Results.ContainsKey(messageid))
        {
            answer =  ((AFWinFormsDXApp)AFCore.App).DefaultMsgAnswerStorage.GetResult(messageid);
            found = true;
        }

        return answer;
    }

    /// <summary>
    /// Show message as error with Ok button
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="moreinfo">additional information</param>.
    /// <param name="messageid">ID of the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowErrorOk(string message, string moreinfo, int messageid)
    {
        return ShowErrorOk(null, message, moreinfo, messageid, null);
    }

    /// <summary>
    /// Show message as error with Ok button
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="moreinfo">additional information</param>.
    /// <param name="messageid">ID of the message</param>
    /// <param name="plugin">Plugin that should be displayed in the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowErrorOk(Control? plugin, string message, string moreinfo, int messageid)
    {
        return ShowErrorOk(plugin, message, moreinfo, messageid, null);
    }

    /// <summary>
    /// Show message as error with Ok button
    /// </summary>
    /// <param name="message">Text of the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowErrorOk(string message)
    {
        return ShowErrorOk(null, message, "", 0);
    }

    /// <summary>
    /// Show message as error with Ok button
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="plugin">Plugin to be displayed in the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowErrorOk(Control plugin, string message)
    {
        return ShowErrorOk(plugin, message, "", 0);
    }

    /// <summary>
    /// Show message as error with Ok button
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="messageid">Text of the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowErrorOk(string message, int messageid)
    {
        return ShowErrorOk(null, message, "", messageid);
    }

    /// <summary>
    /// Show message as error with Ok button
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="messageid">Text of the message</param>
    /// <param name="plugin">Plugin to be displayed in the message</param>.
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowErrorOk(Control plugin, string message, int messageid)
    {
        return ShowErrorOk(plugin, message, "", messageid);
    }

    /// <summary>
    /// Show message as error with Yes/No buttons, No is preselected.
    /// </summary>
    /// <param name="message">Text of the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowErrorYesNo(string message)
    {
        return ShowErrorYesNo(null, message, "", 0);
    }

    /// <summary>
    /// Show message as error with Yes/No buttons, No is preselected.
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="plugin">Plugin to be displayed in the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowErrorYesNo(Control? plugin, string message)
    {
        return ShowErrorYesNo(plugin, message, "", 0);
    }

    /// <summary>
    /// Show message as error with Yes/No buttons, No is preselected.
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="messageid">Text of the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowErrorYesNo(string message, int messageid)
    {
        return ShowErrorYesNo(null, message, "", messageid);
    }

    /// <summary>
    /// Show message as error with Yes/No buttons, No is preselected.
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="messageid">Text of the message</param>.
    /// <param name="plugin">Plugin to be displayed in the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowErrorYesNo(Control? plugin, string message, int messageid)
    {
        return ShowErrorYesNo(plugin, message, "", messageid);
    }

    /// <summary>
    /// Show message as error with Yes/No buttons, No is preselected.
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="moreinfo">additional information</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowErrorYesNo(string message, string moreinfo)
    {
        return ShowErrorYesNo(null, message, moreinfo, 0);
    }

    /// <summary>
    /// Show message as error with Yes/No buttons, No is preselected.
    /// </summary>
    /// <param name="message">text of the message</param>.
    /// <param name="moreinfo">additional information</param>.
    /// <param name="plugin">plugin that should be displayed in the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowErrorYesNo(Control? plugin, string message, string moreinfo)
    {
        return ShowErrorYesNo(plugin, message, moreinfo, 0);
    }

    /// <summary>
    /// Show message as error with Yes/No buttons, No is preselected.
    /// </summary>
    /// <param name="message">text of the message</param>.
    /// <param name="moreinfo">additional information</param>.
    /// <param name="messageid">text of the message</param>.
    /// <param name="owner">owner window of the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowErrorYesNo(string message, string moreinfo, int messageid, IWin32Window? owner)
    {
        return ShowErrorYesNo(null, message, moreinfo, messageid, owner);
    }

    /// <summary>
    /// Show message as error with Yes/No buttons, No is preselected.
    /// </summary>
    /// <param name="message">text of the message</param>.
    /// <param name="moreinfo">additional information</param>.
    /// <param name="owner">owner window of the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowErrorYesNo(string message, string moreinfo, IWin32Window? owner)
    {
        return ShowErrorYesNo(null, message, moreinfo, 0, owner);
    }

    /// <summary>
    /// Show message as error with Yes/No buttons, No is preselected.
    /// </summary>
    /// <param name="message">text of the message</param>.
    /// <param name="owner">owner window of the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowErrorYesNo(string message, IWin32Window? owner)
    {
        return ShowErrorYesNo(null, message, string.Empty, 0, owner);
    }

    /// <summary>
    /// Show message as error with Yes/No buttons, No is preselected.
    /// </summary>
    /// <param name="message">text of the message</param>.
    /// <param name="moreinfo">additional information</param>.
    /// <param name="messageid">text of the message</param>.
    /// <param name="owner">owner window of the message</param>.
    /// <param name="plugin">Plugin to be displayed in the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowErrorYesNo(Control? plugin, string message, string moreinfo, int messageid,
        IWin32Window? owner)
    {
        bool found = false;

        eMessageBoxResult ret = _RestoreAnswer(messageid, ref found);

        if (found)
            return ret;

        return _show(plugin, owner, message, moreinfo, WinFormsStrings.ERROR, eMessageBoxButton.YesNo, eMessageBoxIcon.Error,
            eMessageBoxDefaultButton.Button2, messageid);
    }

    /// <summary>
    /// Show message as error with Yes/No buttons, No is preselected.
    /// </summary>
    /// <param name="message">text of the message</param>.
    /// <param name="moreinfo">additional information</param>.
    /// <param name="messageid">text of the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowErrorYesNo(string message, string moreinfo, int messageid)
    {
        return ShowErrorYesNo(null, message, moreinfo, messageid, null);
    }

    /// <summary>
    /// Show message as error with Yes/No buttons, No is preselected.
    /// </summary>
    /// <param name="message">text of the message</param>.
    /// <param name="moreinfo">additional information</param>.
    /// <param name="messageid">text of the message</param>.
    /// <param name="plugin">Plugin that should be displayed in the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowErrorYesNo(Control? plugin, string message, string moreinfo, int messageid)
    {
        return ShowErrorYesNo(plugin, message, moreinfo, messageid, null);
    }

    /// <summary>
    /// Show message as question with Yes/No buttons, No is preselected.
    /// </summary>
    /// <param name="message">Text of the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowQuestionYesNo(string message)
    {
        return ShowQuestionYesNo(null, message, "", 0);
    }

    /// <summary>
    /// Show message as question with Yes/No buttons, No is preselected.
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="plugin">Plugin to be displayed in the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowQuestionYesNo(Control? plugin, string message)
    {
        return ShowQuestionYesNo(plugin, message, "", 0);
    }

    /// <summary>
    /// Show message as question with Yes/No buttons, No is preselected.
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="moreinfo">Text of the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowQuestionYesNo(string message, string moreinfo)
    {
        return ShowQuestionYesNo(null, message, moreinfo, 0);
    }

    /// <summary>
    /// Show message as question with Yes/No buttons, No is preselected.
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="moreinfo">Text of the message</param>.
    /// <param name="plugin">Plugin to be displayed in the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowQuestionYesNo(Control? plugin, string message, string moreinfo)
    {
        return ShowQuestionYesNo(plugin, message, moreinfo, 0);
    }

    /// <summary>
    /// Show message as question with Yes/No buttons, No is preselected.
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="messageid">Text of the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowQuestionYesNo(string message, int messageid)
    {
        return ShowQuestionYesNo(null, message, "", messageid);
    }

    /// <summary>
    /// Show message as question with Yes/No buttons, No is preselected.
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="messageid">Text of the message</param>.
    /// <param name="plugin">Plugin to be displayed in the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowQuestionYesNo(Control? plugin, string message, int messageid)
    {
        return ShowQuestionYesNo(plugin, message, "", messageid);
    }

    /// <summary>
    /// Show message as question with Yes/No buttons, No is preselected.
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="moreinfo">Text of the message</param>.
    /// <param name="messageid">Text of the message</param>.
    /// <param name="owner">owner window of the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowQuestionYesNo(string message, string moreinfo, int messageid, IWin32Window? owner)
    {
        return ShowQuestionYesNo(null, message, moreinfo, messageid, owner);
    }

    /// <summary>
    /// Show message as question with Yes/No buttons, No is preselected.
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="moreinfo">Text of the message</param>.
    /// <param name="owner">owner window of the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowQuestionYesNo(string message, string moreinfo, IWin32Window? owner)
    {
        return ShowQuestionYesNo(null, message, moreinfo, 0, owner);
    }

    /// <summary>
    /// Show message as question with Yes/No buttons, No is preselected.
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="owner">owner window of the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowQuestionYesNo(string message, IWin32Window? owner)
    {
        return ShowQuestionYesNo(null, message, string.Empty, 0, owner);
    }

    /// <summary>
    /// Show message as question with Yes/No buttons, No is preselected.
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="moreinfo">Text of the message</param>.
    /// <param name="messageid">Text of the message</param>.
    /// <param name="owner">owner window of the message</param>.
    /// <param name="plugin">Plugin to be displayed in the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowQuestionYesNo(Control? plugin, string message, string moreinfo, int messageid,
        IWin32Window? owner)
    {
        bool found = false;

        eMessageBoxResult ret = _RestoreAnswer(messageid, ref found);

        if (found)
            return ret;

        return _show(plugin, owner, message, moreinfo, WinFormsStrings.QUESTION, eMessageBoxButton.YesNo,
            eMessageBoxIcon.Question, eMessageBoxDefaultButton.Button2, messageid);
    }

    /// <summary>
    /// Show message as question with Yes/No buttons, No is preselected.
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="moreinfo">Text of the message</param>.
    /// <param name="messageid">Text of the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowQuestionYesNo(string message, string moreinfo, int messageid)
    {
        return ShowQuestionYesNo(message, moreinfo, messageid, null);
    }

    /// <summary>
    /// Show message as question with Yes/No buttons, No is preselected.
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="moreinfo">Text of the message</param>.
    /// <param name="messageid">Text of the message</param>.
    /// <param name="plugin">Plugin to be displayed in the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowQuestionYesNo(Control? plugin, string message, string moreinfo, int messageid)
    {
        return ShowQuestionYesNo(plugin, message, moreinfo, messageid, null);
    }

    /// <summary>
    /// Show message as warning with Ok/Cancel buttons, Cancel is preselected.
    /// </summary>
    /// <param name="message">Text of the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowWarningOkCancel(string message)
    {
        return ShowWarningOkCancel(null, message, "", 0);
    }

    /// <summary>
    /// Show message as warning with Ok/Cancel buttons, Cancel is preselected.
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="plugin">Plugin to be displayed in the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowWarningOkCancel(Control? plugin, string message)
    {
        return ShowWarningOkCancel(plugin, message, "", 0);
    }

    /// <summary>
    /// Show message as warning with Ok/Cancel buttons, Cancel is preselected.
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="moreinfo">Text of the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowWarningOkCancel(string message, string moreinfo)
    {
        return ShowWarningOkCancel(null, message, moreinfo, 0);
    }

    /// <summary>
    /// Show message as warning with Ok/Cancel buttons, Cancel is preselected.
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="moreinfo">Additional information message</param>.
    /// <param name="plugin">Plugin to be displayed in the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowWarningOkCancel(Control? plugin, string message, string moreinfo)
    {
        return ShowWarningOkCancel(plugin, message, moreinfo, 0);
    }

    /// <summary>
    /// Show message as warning with Ok/Cancel buttons, Cancel is preselected.
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="messageid">Text of the message</param>.
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowWarningOkCancel(string message, int messageid)
    {
        return ShowWarningOkCancel(null, message, "", messageid);
    }

    /// <summary>
    /// Show message as warning with Ok/Cancel buttons, Cancel is preselected.
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="messageid">Text of the message</param>.
    /// <param name="plugin">Plugin to be displayed in the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowWarningOkCancel(Control? plugin, string message, int messageid)
    {
        return ShowWarningOkCancel(plugin, message, "", messageid);
    }

    /// <summary>
    /// Show message as warning with Ok/Cancel buttons, Cancel is preselected.
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="moreinfo">Additional information message</param>.
    /// <param name="messageid">Text of the message</param>.
    /// <param name="owner">Owner window of the message</param>.
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowWarningOkCancel(string message, string moreinfo, int messageid, IWin32Window? owner)
    {
        return ShowWarningOkCancel(null, message, moreinfo, messageid, owner);
    }

    /// <summary>
    /// Show message as warning with Ok/Cancel buttons, Cancel is preselected.
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="moreinfo">Additional information message</param>.
    /// <param name="messageid">Text of the message</param>.
    /// <param name="owner">Owner window of the message</param>.
    /// <param name="plugin">Plugin to be displayed in the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowWarningOkCancel(Control? plugin, string message, string moreinfo, int messageid,
        IWin32Window? owner)
    {
        bool found = false;

        var ret = _RestoreAnswer(messageid, ref found);

        if (found)
            return ret;

        return _show(plugin, owner, message, moreinfo, WinFormsStrings.WARNING, eMessageBoxButton.OKCancel,
            eMessageBoxIcon.Warning, eMessageBoxDefaultButton.Button2, messageid);
    }

    /// <summary>
    /// Show message as warning with Ok/Cancel buttons, Cancel is preselected.
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="moreinfo">Additional information message</param>.
    /// <param name="messageid">Text of the message</param>.
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowWarningOkCancel(string message, string moreinfo, int messageid)
    {
        return ShowWarningOkCancel(null, message, moreinfo, messageid, null);
    }

    /// <summary>
    /// Show message as warning with Ok/Cancel buttons, Cancel is preselected.
    /// </summary>
    /// <param name="message">Text of the message</param>.
    /// <param name="moreinfo">Additional information message</param>.
    /// <param name="messageid">Text of the message</param>.
    /// <param name="plugin">Plugin to be displayed in the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult ShowWarningOkCancel(Control? plugin, string message, string moreinfo, int messageid)
    {
        return ShowWarningOkCancel(plugin, message, moreinfo, messageid, null);
    }

    /// <summary>
    /// Display a messagebox
    /// </summary>
    /// <param name="message">Message (first line = title of the message)</param>
    /// <param name="caption">Title line of the window</param>.
    /// <param name="buttons">eMessageBoxButton</param>
    /// <param name="icon">Icon/symbol</param>
    /// <param name="defaultButton">Default button</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult Show(string message, string caption, eMessageBoxButton buttons, eMessageBoxIcon icon,
        eMessageBoxDefaultButton defaultButton)
    {
        return Show(null, message, "", caption, buttons, icon, defaultButton, 0, null);
    }

    /// <summary>
    /// Display a messagebox
    /// </summary>
    /// <param name="message">Message (first line = title of the message)</param>
    /// <param name="caption">Title line of the window</param>.
    /// <param name="buttons">eMessageBoxButton</param>
    /// <param name="icon">Icon/symbol</param>
    /// <param name="defaultButton">Default button</param>
    /// <param name="plugin">Plugin to be displayed in the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult Show(Control plugin, string message, string caption, eMessageBoxButton buttons,
        eMessageBoxIcon icon, eMessageBoxDefaultButton defaultButton)
    {
        return Show(plugin, message, "", caption, buttons, icon, defaultButton, 0, null);
    }

    /// <summary>
    /// Display a messagebox
    /// </summary>
    /// <param name="message">Message (first line = title of the message)</param>
    /// <param name="moreinfo">Additional information message</param>.
    /// <param name="caption">Title line of the window</param>.
    /// <param name="buttons">eMessageBoxButton</param>
    /// <param name="icon">Icon/symbol</param>
    /// <param name="defaultButton">Default button</param>
    /// <param name="messageid">ID of the message</param>
    /// <param name="owner">Owner window of the message</param>.
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult Show(string message, string moreinfo, string caption, eMessageBoxButton buttons,
        eMessageBoxIcon icon, eMessageBoxDefaultButton defaultButton, int messageid, IWin32Window? owner)
    {
        return Show(null, message, moreinfo, caption, buttons, icon, defaultButton, messageid, owner);
    }

    /// <summary>
    /// Display a messagebox
    /// </summary>
    /// <param name="owner">Message (first line = title of the message)</param>
    /// <param name="args">Title line of the window</param>.
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult Show(object? owner, MessageBoxArguments args)
    {
        return Show(null, args.Message, args.MoreInfo, args.Caption, args.Buttons, args.Icon, args.DefaultButton,
            args.MessageId, owner is not null and IWin32Window ? (IWin32Window)owner : null);
    }

    /// <summary>
    /// Display a messagebox
    /// </summary>
    /// <param name="message">Message (first line = title of the message)</param>
    /// <param name="moreinfo">Additional information message</param>.
    /// <param name="caption">Title line of the window</param>.
    /// <param name="buttons">eMessageBoxButton</param>
    /// <param name="icon">Icon/symbol</param>
    /// <param name="defaultButton">Default button</param>
    /// <param name="owner">Owner window of the message</param>.
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult Show(string message, string moreinfo, string caption, eMessageBoxButton buttons,
        eMessageBoxIcon icon, eMessageBoxDefaultButton defaultButton, IWin32Window? owner)
    {
        return Show(null, message, moreinfo, caption, buttons, icon, defaultButton, 0, owner);
    }


    /// <summary>
    /// Display a messagebox
    /// </summary>
    /// <param name="message">Message (first line = title of the message)</param>
    /// <param name="moreinfo">Additional information message</param>.
    /// <param name="caption">Title line of the window</param>.
    /// <param name="buttons">eMessageBoxButton</param>
    /// <param name="icon">Icon/symbol</param>
    /// <param name="defaultButton">Default button</param>
    /// <param name="messageid">ID of the message</param>
    /// <param name="owner">Owner window of the message</param>.
    /// <param name="plugin">Plugin to be displayed in the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult Show(Control? plugin, string message, string moreinfo, string caption,
        eMessageBoxButton buttons, eMessageBoxIcon icon, eMessageBoxDefaultButton defaultButton, int messageid,
        IWin32Window? owner)
    {
        bool found = false;

        eMessageBoxResult ret = _RestoreAnswer(messageid, ref found);

        if (found)
            return ret;

        return _show(plugin, owner, message, moreinfo, caption, buttons, icon, defaultButton, messageid);
    }

    /// <summary>
    /// Display a messagebox
    /// </summary>
    /// <param name="message">Message (first line = title of the message)</param>
    /// <param name="caption">Title line of the window</param>.
    /// <param name="buttons">eMessageBoxButton</param>
    /// <param name="icon">Icon/symbol</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult Show(string message, string caption, eMessageBoxButton buttons, eMessageBoxIcon icon)
    {
        return Show(null, message, "", caption, buttons, icon, eMessageBoxDefaultButton.Button1, 0, null);
    }

    /// <summary>
    /// Display a messagebox
    /// </summary>
    /// <param name="message">Message (first line = title of the message)</param>
    /// <param name="moreinfo">Additional information message</param>.
    /// <param name="caption">Title line of the window</param>.
    /// <param name="buttons">eMessageBoxButton</param>
    /// <param name="icon">Icon/symbol</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult Show(string message, string moreinfo, string caption, eMessageBoxButton buttons,
        eMessageBoxIcon icon)
    {
        return Show(null, message, moreinfo, caption, buttons, icon, eMessageBoxDefaultButton.Button1, 0, null);
    }

    /// <summary>
    /// Display a messagebox
    /// </summary>
    /// <param name="message">Message (first line = title of the message)</param>
    /// <param name="caption">Title line of the window</param>.
    /// <param name="buttons">eMessageBoxButton</param>
    /// <param name="icon">Icon/symbol</param>
    /// <param name="plugin">Plugin to be displayed in the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult Show(Control plugin, string message, string caption, eMessageBoxButton buttons,
        eMessageBoxIcon icon)
    {
        return Show(plugin, message, "", caption, buttons, icon, eMessageBoxDefaultButton.Button1, 0, null);
    }

    /// <summary>
    /// Display a messagebox
    /// </summary>
    /// <param name="message">Message (first line = title of the message)</param>
    /// <param name="moreinfo">Additional information message</param>.
    /// <param name="caption">Title line of the window</param>.
    /// <param name="buttons">eMessageBoxButton</param>
    /// <param name="icon">Icon/symbol</param>
    /// <param name="plugin">Plugin to be displayed in the message</param>
    /// <returns>eMessageBoxResult</returns>
    public static eMessageBoxResult Show(Control plugin, string message, string moreinfo, string caption,
        eMessageBoxButton buttons, eMessageBoxIcon icon)
    {
        return Show(plugin, message, moreinfo, caption, buttons, icon, eMessageBoxDefaultButton.Button1, 0, null);
    }
}
