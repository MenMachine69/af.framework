namespace AF.CORE;

/// <summary>
/// Schnittstelle für eine Variable
/// </summary>
public interface IVariable
{
    /// <summary>
    /// Eindeutiger Name dieser Variablen
    /// </summary>
    string VAR_NAME { get; set; }

    /// <summary>
    /// Anzeigename dieser Variablen
    /// </summary>
    string VAR_CAPTION { get; set; }

    /// <summary>
    /// Beschreibung dieser Variablen
    /// </summary>
    string VAR_DESCRIPTION { get; set; }

    /// <summary>
    /// Der Typ der Variablen. Je nach Typ werden weitere Einstellungen zur Verfügung gestellt.
    /// </summary>
    int VAR_TYP { get; set; }

    /// <summary>
    /// Priorität der Variablen (bestimmt z.B. die Reihenfolge in Eingabedialogen)
    /// </summary>
    int VAR_PRIORITY { get; set; }

    /// <summary>
    /// Spezifische Daten der Eigenschaft (abhängig vom Typ).
    /// </summary>
    VariableStorageObject VAR_STORAGE { get; set; }

    /// <summary>
    /// Liefert die Details zur Variablen abhängig vom Typ der Variablen.
    /// </summary>
    VariableBase VAR_VARIABLE { get; set; }

    /// <summary>
    /// Variablenwert kann vom Benutzer NICHT interaktiv bearbeitet werden (nur Anzeige)
    /// </summary>
    public bool VAR_READONLY { get; set; }

    /// <summary>
    /// Zweispaltige Anzeige des Wertes in Eingabeformularen.
    /// 
    /// Sobald dieser Wert bei einer Variablen auf true gesetzt wird, wird ein zeipaltiges Forumlar erzeugt.
    /// </summary>
    bool VAR_TWOCOLUMN { get; set; }

    /// <summary>
    /// Den Wert in Formularen in dieser Spalte anzeigen (1 oder 2).
    /// 
    /// Sobald dieser Wert bei einer Variablen auf den Wert 2 gesetzt wird, wird ein zeispaltiges Forumlar erzeugt.
    /// </summary>
    int VAR_COLUMN { get; set; }

    /// <summary>
    /// Den Wert in Formularen in einem separaten TAB anzeigen (Tabcontrol).
    /// 
    /// Sobald dieser Wert bei einer Variablen auf true gesetzt wird, wird das Control zur Eingabe in einem TAB angezeigt. 
    /// Folgende Variablen, die ebenfalls in Tabs angezeigt werden sollen, erzeugen jeweils einen neuen TAB.
    /// </summary>
    bool VAR_TABBED { get; set; }

    /// <summary>
    /// Eingabe mehrere Werte wird unterstützt (nur Variablentyp eVariableTyp.List).
    ///
    /// Wenn true wird eine Eingabe zur verfügung gestellt, die mehrere Werte unterstützt.
    /// </summary>
    bool VAR_MULTIPLE { get; set; }

    /// <summary>
    /// Überschrift eines Abschnitts. 
    /// 
    /// Wird hier ein Text eingetragen, erscheint im Formular automatisch ein neuer Abschnitt mit dieser Überschrift vor der eigentlichen Eingabe.
    /// </summary>
    string VAR_SECCAPTION { get; set; }

    /// <summary>
    /// Beschreibung eines Abschnitts. 
    /// 
    /// Wird hier ein Text eingetragen, erscheint im Formular automatisch ein neuer Abschnitt mit dieser Überschrift vor der eigentlichen Eingabe.
    /// </summary>
    string VAR_SECDESCRIPTION { get; set; }

    /// <summary>
    /// Control, dass dür die Eingabe verwendet werden soll.
    /// 
    /// Wenn NULL, wird das passende Control anhand VAR_TYP ermittelt.
    /// </summary>
    object? VAR_CONTROL { get; set; }

    /// <summary>
    /// Variable bietet keinen Editor (nur Caption und Description)
    /// </summary>
    bool VAR_NOEDITOR { get; }
}