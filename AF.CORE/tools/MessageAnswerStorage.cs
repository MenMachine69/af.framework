namespace AF.CORE;

/// <summary>
/// Standardimplementierung von IMessageAnswerStorage. 
/// 
/// Diese Komponente unterstützt das Speichern/Laden gespeicherter Antworten in/aus einer Datei über SaveToFile/LoadFromFile 
/// und den Zugriff auf ein Dictionary, in dem die aktuellen Antworten für einzelne Speicher-/Ladelösungen gespeichert werden.
/// 
/// Um diese Komponente zu verwenden, fügen Sie sie in das Hauptformular ein und weisen sie GuiLayer.MsgAnswerStore zu...
/// </summary>
public class MessageAnswerStorage : IMessageAnswerStorage
{
    /// <summary>
    /// Constructor
    /// </summary>
    public MessageAnswerStorage() { }

    /// <summary>
    /// Wörterbuch mit allen aktuell gespeicherten Ergebnissen. Kann null sein, wenn keine Antworten/Ergebnisse gespeichert sind.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Dictionary<int, eMessageBoxResult> Results { get; set; } = new();

    /// <summary>
    /// Ermittelt ein gespeichertes Ergebnis oder eMessageBoxResult.None, wenn derzeit kein Ergebnis für die Nachricht mit dieser ID gespeichert ist
    /// </summary>
    /// <param name="msgID">ID der Nachricht</param>
    /// <returns>gespeichertes Ergebnis oder DialogResult.None, wenn kein gespeichertes Ergebnis vorhanden ist</returns>
    public eMessageBoxResult GetResult(int msgID)
    {
        return Results.TryGetValue(msgID, out var result) ? result : eMessageBoxResult.None;
    }

    /// <summary>
    /// Speichert eine Antwort auf eine Nachricht in den Speicher.
    /// </summary>
    /// <param name="msgID">ID der Nachricht</param>
    /// <param name="result">Ergebnis/Antwort, die gespeichert werden soll</param>
    public void SetResult(int msgID, eMessageBoxResult result)
    {
        Results[msgID] = result;
    }


    /// <summary>
    /// Speichern aller Antworten/Ergebnisse im Wörterbuch in einer Datei (unter Verwendung binärer Serialisierung)
    /// </summary>
    /// <param name="filename">Name der zu speichernden Datei (inkl. Pfad)</param>
    public void SaveToFile(string filename)
    {
        msgboxAnswers answers = new();
        Results.ForEach(e => answers.Answers.Add(new msgboxAnswer { ID = e.Key, Answer = (int)e.Value }));
        answers.ToJsonFile(new FileInfo(filename));
    }

    /// <summary>
    /// Lädt alle Antworten/Ergebnisse aus einer mit SaveToFile erstellten Datei in das aktuelle Wörterbuch. 
    /// </summary>
    /// <param name="filename">Name der Datei, aus der die Antworten/Ergebnisse geladen werden sollen (inkl. Pfad)</param>
    public void LoadFromFile(string filename)
    {
        Results.Clear();

        msgboxAnswers? answers = Functions.DeserializeJsonFile<msgboxAnswers>(new FileInfo(filename));
        answers?.Answers.ForEach(a => Results.Add(a.ID, (eMessageBoxResult)a.Answer));
    }

    /// <summary>
    /// Entfernt alle gespeicherten Antworten/Ergebnisse aus dem Speicher.
    /// </summary>
    public void Clear()
    {
        Results.Clear();
    }

    #region classes for serialization
    [Serializable]
    internal class msgboxAnswers
    {
        public List<msgboxAnswer> Answers { get; set; } = [];
    }

    [Serializable]
    internal class msgboxAnswer
    {
        public int ID { get; set; }

        public int Answer { get; set; }
    }
    #endregion
}
