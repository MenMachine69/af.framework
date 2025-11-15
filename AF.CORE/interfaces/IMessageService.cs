namespace AF.CORE;

/// <summary>
/// Schnittstelle für einen Dienst zur Anzeige von Nachrichten innerhalb der Benutzeroberfläche.
/// 
/// Ein solcher Dienst kann verwendet werden, wenn ein MVCContext erstellt wird. 
/// Nach der Erstellung kann dieser Dienst zur Anzeige von Meldungen (z.B. Messageboxen, Flyouts) verwendet werden 
/// über MVCManager.Instance.MessageService...
/// </summary>
public interface IMessageService
{
    /// <summary>
    /// Speicher für alle gespeicherten Messagebox-Antworten
    /// </summary>
    IMessageAnswerStorage AnswerStorage { get; set; }

    /// <summary>
    /// Zeigt eine nicht modale Meldung (z.B. Flyout-Meldung)
    /// </summary>
    /// <param name="args">Argumente für die Meldung</param>
    void ShowMessage(MessageArguments args);

    /// <summary>
    /// Zeigt eine nicht modale Fehlermeldung an (z.B. eine Flyout-Meldung)
    /// </summary>
    /// <param name="message">Text</param>
    /// <param name="timeout">Timeout in Sekunden</param>
    void ShowMessageError(string message, int timeout = 5);

    /// <summary>
    /// Zeigt eine nicht modale Info-Meldung (z.B. Flyout-Meldung)
    /// </summary>
    /// <param name="message">Text</param>
    /// <param name="timeout">Timeout in Sekunden</param>
    void ShowMessageInfo(string message, int timeout = 5);

    /// <summary>
    /// Zeigt eine nicht modale Warnmeldung (z.B. Flyout-Meldung)
    /// </summary>
    /// <param name="message">Text</param>
    /// <param name="timeout">Timeout in Sekunden</param>
    void ShowMessageWarning(string message, int timeout = 5);

    /// <summary>
    /// Behandelt ein CommandResult (z.B. Anzeige einer Nachricht).
    /// </summary>
    /// <param name="result">Anzuzeigendes Ergebnis</param>
    void HandleResult(CommandResult result);

    /// <summary>
    /// Nachricht als Info mit Ok-Taste anzeigen
    /// </summary>
    /// <param name="message">Text der Nachricht</param>.
    /// <param name="moreinfo">zusätzliche Informationen</param>
    /// <returns>eMessageBoxResult</returns>
    eMessageBoxResult ShowInfoOk(string message, string moreinfo);

    /// <summary>
    /// Nachricht als Info mit Ok-Taste anzeigen
    /// </summary>
    /// <param name="message">Nachrichtentext</param>.
    /// <param name="moreinfo">zusätzliche Informationen</param>.
    /// <param name="messageid">ID der Nachricht</param>.
    /// <returns>eMessageBoxResult</returns>
    eMessageBoxResult ShowInfoOk(string message, string moreinfo, int messageid);

    /// <summary>
    /// Nachricht als Info mit Ok-Taste anzeigen
    /// </summary>
    /// <param name="message">Text der Meldung</param>
    /// <returns>eMessageBoxResult</returns>
    eMessageBoxResult ShowInfoOk(string message);

    /// <summary>
    /// Show message as info with Ok button
    /// </summary>
    /// <param name="message">Text of the message</param>
    /// <param name="messageid">ID of the message</param>
    /// <returns>eMessageBoxResult</returns>
    eMessageBoxResult ShowInfoOk(string message, int messageid);

    /// <summary>
    /// Meldung als Fehler mit Ok-Schaltfläche anzeigen
    /// </summary>
    /// <param name="message">Text der Meldung</param>.
    /// <param name="moreinfo">Mehr Informationen</param>
    /// <returns>eMessageBoxResult</returns>
    eMessageBoxResult ShowErrorOk(string message, string moreinfo);

    /// <summary>
    /// Meldung als Fehler mit Ok-Schaltfläche anzeigen
    /// </summary>
    /// <param name="message">Text der Meldung</param>.
    /// <param name="moreinfo">zusätzliche Informationen</param>.
    /// <param name="messageid">ID der Nachricht</param>.
    /// <returns>eMessageBoxResult</returns>
    eMessageBoxResult ShowErrorOk(string message, string moreinfo, int messageid);


    /// <summary>
    /// Meldung als Fehler mit Ok-Schaltfläche anzeigen
    /// </summary>
    /// <param name="message">Text der Meldung</param>
    /// <returns>eMessageBoxResult</returns>
    eMessageBoxResult ShowErrorOk(string message);

    /// <summary>
    /// Meldung als Fehler mit Ok-Schaltfläche anzeigen
    /// </summary>
    /// <param name="message">Text der Meldung</param>.
    /// <param name="messageid">Text der Meldung</param>
    /// <returns>eMessageBoxResult</returns>
    eMessageBoxResult ShowErrorOk(string message, int messageid);

    /// <summary>
    /// Zeigt eine Fehlermeldung mit den Schaltflächen Ja/Nein an, wobei Nein vorausgewählt ist.
    /// </summary>
    /// <param name="message">Text der Meldung</param>
    /// <returns>eMessageBoxResult</returns>
    eMessageBoxResult ShowErrorYesNo(string message);

    /// <summary>
    /// Zeigt eine Fehlermeldung mit den Schaltflächen Ja/Nein an, wobei Nein vorausgewählt ist.
    /// </summary>
    /// <param name="message">Text der Meldung</param>.
    /// <param name="messageid">Text der Meldung</param>
    /// <returns>eMessageBoxResult</returns>
    eMessageBoxResult ShowErrorYesNo(string message, int messageid);

    /// <summary>
    /// Zeigt eine Fehlermeldung mit den Schaltflächen Ja/Nein an, wobei Nein vorausgewählt ist.
    /// </summary>
    /// <param name="message">Text der Meldung</param>.
    /// <param name="moreinfo">zusätzliche Informationen</param>
    /// <returns>eMessageBoxResult</returns>
    eMessageBoxResult ShowErrorYesNo(string message, string moreinfo);

    /// <summary>
    /// Zeigt eine Fehlermeldung mit den Schaltflächen Ja/Nein an, wobei Nein vorausgewählt ist.
    /// </summary>
    /// <param name="message">Text der Meldung</param>.
    /// <param name="moreinfo">zusätzliche Informationen</param>.
    /// <param name="messageid">Text der Nachricht</param>.
    /// <returns>eMessageBoxResult</returns>
    eMessageBoxResult ShowErrorYesNo(string message, string moreinfo, int messageid);

    /// <summary>
    /// Nachricht als Frage mit Ja/Nein-Schaltflächen anzeigen, Nein ist vorausgewählt.
    /// </summary>
    /// <param name="message">Text der Meldung</param>
    /// <returns>eMessageBoxResult</returns>
    eMessageBoxResult ShowQuestionYesNo(string message);

    /// <summary>
    /// Nachricht als Frage mit Ja/Nein-Schaltflächen anzeigen, Nein ist vorausgewählt.
    /// </summary>
    /// <param name="message">Text der Meldung</param>.
    /// <param name="moreinfo">Text der Meldung</param>
    /// <returns>eMessageBoxResult</returns>
    eMessageBoxResult ShowQuestionYesNo(string message, string moreinfo);

    /// <summary>
    /// Nachricht als Frage mit Ja/Nein-Schaltflächen anzeigen, Nein ist vorausgewählt.
    /// </summary>
    /// <param name="message">Text der Nachricht</param>.
    /// <param name="messageid">Text der Nachricht</param>.
    /// <returns>eMessageBoxResult</returns>
    eMessageBoxResult ShowQuestionYesNo(string message, int messageid);

    /// <summary>
    /// Nachricht als Frage mit Ja/Nein-Schaltflächen anzeigen, Nein ist vorausgewählt.
    /// </summary>
    /// <param name="message">Text der Meldung</param>.
    /// <param name="moreinfo">Text der Meldung</param>.
    /// <param name="messageid">Text der Nachricht</param>.
    /// <returns>eMessageBoxResult</returns>
    eMessageBoxResult ShowQuestionYesNo(string message, string moreinfo, int messageid);

    /// <summary>
    /// Meldung als Warnung mit Schaltflächen Ok/Abbrechen anzeigen, Abbruch ist vorgewählt.
    /// </summary>
    /// <param name="message">Text der Meldung</param>
    /// <returns>eMessageBoxResult</returns>
    eMessageBoxResult ShowWarningOkCancel(string message);

    /// <summary>
    /// Meldung als Warnung mit Schaltflächen Ok/Abbrechen anzeigen, Abbruch ist vorgewählt.
    /// </summary>
    /// <param name="message">Text der Meldung</param>.
    /// <param name="moreinfo">Text der Meldung</param>.
    /// <returns>eMessageBoxResult</returns>
    eMessageBoxResult ShowWarningOkCancel(string message, string moreinfo);

    /// <summary>
    /// Meldung als Warnung mit Schaltflächen Ok/Abbrechen anzeigen, Abbruch ist vorgewählt.
    /// </summary>
    /// <param name="message">Text der Meldung</param>.
    /// <param name="messageid">Text der Meldung</param>.
    /// <returns>eMessageBoxResult</returns>
    eMessageBoxResult ShowWarningOkCancel(string message, int messageid);

    /// <summary>
    /// Meldung als Warnung mit Schaltflächen Ok/Abbrechen anzeigen, Abbruch ist vorgewählt.
    /// </summary>
    /// <param name="message">Text der Meldung</param>.
    /// <param name="moreinfo">Zusätzliche Informationsmeldung</param>.
    /// <param name="messageid">Text der Nachricht</param>.
    /// <returns>eMessageBoxResult</returns>
    eMessageBoxResult ShowWarningOkCancel(string message, string moreinfo, int messageid);

    /// <summary>
    /// Zeigt eine modale Nachrichtenbox
    /// </summary>
    /// <param name="args">Argumente für die Box</param>
    /// <returns>Ergebnis, das der Benutzer gewählt hat</returns>
    eMessageBoxResult ShowMsgBox(MessageBoxArguments args);

    /// <summary>
    /// Zeigt ein modales Nachrichtenfeld
    /// </summary>
    /// <param name="owner">Elternteil für die Anzeige dieser Nachrichtenbox (z.B. ein Fenster)</param>
    /// <param name="args">Argumente für die Box</param>
    /// <returns>Ergebnis, das der Benutzer ausgewählt hat</returns>
    eMessageBoxResult ShowMsgBox(object? owner, MessageBoxArguments args);

    /// <summary>
    /// Anzeige einer Messagebox
    /// </summary>
    /// <param name="message">Nachricht (erste Zeile = Titel der Nachricht)</param>
    /// <param name="caption">Titelzeile des Fensters</param>.
    /// <param name="buttons">eMessageBoxButton</param>
    /// <param name="icon">Icon/Symbol</param>
    /// <param name="defaultButton">Standard-Schaltfläche</param>
    /// <returns>eMessageBoxResult</returns>
    eMessageBoxResult ShowMsgBox(string message, string caption, eMessageBoxButton buttons, eMessageBoxIcon icon,
        eMessageBoxDefaultButton defaultButton);

    /// <summary>
    /// Anzeige einer Messagebox
    /// </summary>
    /// <param name="message">Nachricht (erste Zeile = Titel der Nachricht)</param>
    /// <param name="caption">Titelzeile des Fensters</param>.
    /// <param name="buttons">eMessageBoxButton</param>
    /// <param name="icon">Icon/Symbol</param>
    /// <returns>eMessageBoxResult</returns>
    eMessageBoxResult ShowMsgBox(string message, string caption, eMessageBoxButton buttons,
        eMessageBoxIcon icon);

    /// <summary>
    /// Anzeige einer Messagebox
    /// </summary>
    /// <param name="message">Nachricht (erste Zeile = Titel der Nachricht)</param>
    /// <param name="moreinfo">Zusätzliche Informationsmeldung</param>.
    /// <param name="caption">Titelzeile des Fensters</param>.
    /// <param name="buttons">eMessageBoxButton</param>.
    /// <param name="icon">Icon/Symbol</param>
    /// <returns>eMessageBoxResult</returns>
    eMessageBoxResult ShowMsgBox(string message, string moreinfo, string caption, eMessageBoxButton buttons,
        eMessageBoxIcon icon);
}

/// <summary>
/// Argumente für eine Nachricht, die als nicht modal angezeigt werden soll (z.B. Flyout)
/// </summary>
public class MessageArguments
{
    /// <summary>
    /// Erzeugt die Argumente für eine nicht modale Meldung (z.B. eine Flyout-Meldung)
    /// </summary>
    /// <param name="message">Anzuzeigende Nachricht</param>
    public MessageArguments(string message)
    {
        Message = message;
    }

    /// <summary>
    /// Nachrichtentext
    /// </summary>
    public string Message { get; init; }

    /// <summary>
    /// Typ der Nachricht
    /// 
    /// Standard ist 'Information'
    /// </summary>
    public eNotificationType Type { get; set; } = eNotificationType.Information;

    /// <summary>
    /// Timeout für die Nachricht
    /// 
    /// Nach dieser Zeit (in Sekunden) wird die Nachricht automatisch gelöscht.
    /// Standard ist 6 Sekunden.
    /// </summary>
    public int TimeOut { get; set; } = 6;

}

/// <summary>
/// Argumente für die Anzeige einer Messagebox
/// </summary>
public class MessageBoxArguments
{
    /// <summary>
    /// Überschrift/Fensterüberschrift
    /// </summary>
    public string Caption { get; set; } = CoreStrings.QUESTION;


    /// <summary>
    /// ID der Nachricht
    ///
    /// Bei > 0 kann die Antwort auf die Nachricht gespeichert werden, danach wird die Nachricht automatisch geschlossen und die gespeicherte Antwort immer zurückgegeben.
    /// </summary>
    public int MessageId { get; set; } = 0;

    /// <summary>
    /// die anzuzeigende Nachricht.
    /// </summary>
    public string Message { get; set; } = "";

    /// <summary>
    /// weitere Informationen, die im Nachrichtenfeld angezeigt werden.
    /// Normalerweise werden diese Informationen nur angezeigt, wenn 
    /// der Benutzer sie anzeigt (z.B. wenn eine Schaltfläche gedrückt wird)
    /// </summary>
    public string MoreInfo { get; set; } = string.Empty;

    /// <summary>
    /// Anzuzeigendes Icon/Symbol
    /// </summary>
    public eMessageBoxIcon Icon { get; set; } = eMessageBoxIcon.Question;

    /// <summary>
    /// Anzuzeigende Schaltflächen. Mit diesen Schaltflächen werden die möglichen Ergebnisse festgelegt.
    /// </summary>
    public eMessageBoxButton Buttons { get; set; } = eMessageBoxButton.YesNo;

    /// <summary>
    /// Welche Taste als Standardtaste verwendet werden soll (z.B. wenn der Benutzer ENTER drückt)
    /// </summary>
    public eMessageBoxDefaultButton DefaultButton { get; set; } = eMessageBoxDefaultButton.Button1;
}

/// <summary>
/// Anzuzeigendes Symbol/Icon
/// </summary>
public enum eMessageBoxIcon
{
    /// <summary>
    /// Kein Symbol
    /// </summary>
    None = 0,
    /// <summary>
    /// Hand/Achtung
    /// </summary>
    Hand = 16,
    /// <summary>
    /// Stop
    /// </summary>
    Stop = 16,
    /// <summary>
    /// Fehler
    /// </summary>
    Error = 16,
    /// <summary>
    /// Frage
    /// </summary>
    Question = 32,
    /// <summary>
    /// Ausfrufungszeichen
    /// </summary>
    Exclamation = 48,
    /// <summary>
    /// Warnung
    /// </summary>
    Warning = 48,
    /// <summary>
    /// Sternchen
    /// </summary>
    Asterisk = 64,
    /// <summary>
    /// Information
    /// </summary>
    Information = 64
}

/// <summary>
/// Schaltfläche
/// </summary>
public enum eMessageBoxButton
{
    /// <summary>
    /// OK-Schaltfläche
    /// </summary>
    OK = 0,
    /// <summary>
    /// OK- und Abbrechen-Schaltflächen
    /// </summary>
    OKCancel = 1,
    /// <summary>
    /// Abbrechen, Wiederholen und Ignorieren
    /// </summary>
    AbortRetryIgnore = 2,
    /// <summary>
    /// Ja, Nein und Abbrechen
    /// </summary>
    YesNoCancel = 3,
    /// <summary>
    /// Ja und Nein
    /// </summary>
    YesNo = 4,
    /// <summary>
    /// Wiederholen und Abbrechen
    /// </summary>
    RetryCancel = 5
}

/// <summary>
/// Resultat/Antwort einer Messagebox
/// </summary>
public enum eMessageBoxResult
{
    /// <summary>
    /// Keine Antwort
    /// </summary>
    None = 0,
    /// <summary>
    /// OK
    /// </summary>
    OK = 1,
    /// <summary>
    /// Abbrechen
    /// </summary>
    Cancel = 2,
    /// <summary>
    /// Abbruch
    /// </summary>
    Abort = 3,
    /// <summary>
    /// Wiederholen
    /// </summary>
    Retry = 4,
    /// <summary>
    /// Ignorieren
    /// </summary>
    Ignore = 5,
    /// <summary>
    /// Ja
    /// </summary>
    Yes = 6,
    /// <summary>
    /// Nein
    /// </summary>
    No = 7,
    /// <summary>
    /// Fortfahren
    /// </summary>
    Continue = 8,
    /// <summary>
    /// Nochmal versuchen
    /// </summary>
    TryAgain = 9
}

/// <summary>
/// Standard-Schaltfläche/vorausgewählte Schaltfläche
/// </summary>
public enum eMessageBoxDefaultButton
{
    /// <summary>
    /// Schaltfläche 1  
    /// </summary>
    Button1 = 0,
    /// <summary>
    /// Schaltfläche 2
    /// </summary>
    Button2 = 256,
    /// <summary>
    /// Schaltfläche 3  
    /// </summary>
    Button3 = 512
}


/// <summary>
/// Schnittstelle für einen Speicher zum Speichern der Antworten einer Messagebox.
/// </summary>
public interface IMessageAnswerStorage
{
    /// <summary>
    /// Wörterbuch mit allen aktuell gespeicherten Ergebnissen. Kann null sein, wenn keine Antworten/Ergebnisse gespeichert sind.
    /// </summary>
    Dictionary<int, eMessageBoxResult> Results { get; set; }

    /// <summary>
    /// Ermittelt ein gespeichertes Ergebnis oder eMessageBoxResult.None, wenn derzeit kein Ergebnis für die Nachricht mit dieser ID gespeichert ist
    /// </summary>
    /// <param name="msgID">ID der Nachricht</param>
    /// <returns>gespeichertes Ergebnis oder DialogResult.None, wenn kein gespeichertes Ergebnis vorhanden ist</returns>
    eMessageBoxResult GetResult(int msgID);

    /// <summary>
    /// Speichert eine Antwort auf eine Nachricht in den Speicher.
    /// </summary>
    /// <param name="msgID">ID der Nachricht</param>
    /// <param name="result">Ergebnis/Antwort, die gespeichert werden soll</param>
    void SetResult(int msgID, eMessageBoxResult result);
}
