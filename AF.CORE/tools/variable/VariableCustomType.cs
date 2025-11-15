namespace AF.CORE;

/// <summary>
/// Beschreibung einer CustomVariable (anwendungsspezifisch)
/// </summary>
public class VariableCustomType
{
    /// <summary>
    /// Typ der Variablen zur eindeutigen Identifikation.
    ///
    /// Nur Werte > 100 erlaubt! Der Wert muss innerhalb der App eindeutig sein!
    /// </summary>
    public int VariableTypIndex { get; init; }

    /// <summary>
    /// Name des Variablentyps (z.B. 'Team', 'Benutzer' etc.)
    /// </summary>
    public string VariableTypeName { get; init; }

    /// <summary>
    /// Typ der Variablen (Klasse, geerbt von VariableBase, serialisierbar)
    /// </summary>
    public Type VariableType { get; init; }

    /// <summary>
    /// Typ des Controls zur Eingabe der Variablen (z.B. abgeleitete Combobox etc.)
    /// </summary>
    public Type VariableEditorType { get; set; }

    /// <summary>
    /// Typ des Controls zur Konfiguration der Variablen (i.d.R. von AFVariableEditBase).
    /// </summary>
    public Type VariableEditorConfigType { get; set; }

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="variableTypIndex">Typ der Variablen zur eindeutigen Identifikation.</param>
    /// <param name="typeName">Name des Variablentyps (z.B. 'Team', 'Benutzer' etc.)</param>
    /// <param name="variableTyp"> Typ der Variablen (Klasse, geerbt von VariableBase, serialisierbar)</param>
    /// <param name="editorType">Typ des Controls zur Eingabe der Variablen (z.B. abgeleitete Combobox etc.)</param>
    /// <param name="editorConfigType">Typ des Controls zur Konfiguration der Variablen (i.d.R. von AFVariableEditBase).</param>
    public VariableCustomType(int variableTypIndex, string typeName, Type variableTyp, Type editorType, Type editorConfigType)
    {
        if (variableTypIndex < 100) throw new ArgumentOutOfRangeException($"Typindex muss >= 100 sein! (ist {variableTypIndex})");

        if (typeName.IsEmpty()) throw new ArgumentException($"Typname darf nicht leer sein!");

        VariableEditorConfigType = editorConfigType;
        VariableTypIndex = variableTypIndex;
        VariableType = variableTyp;
        VariableEditorType = editorType;
        VariableTypeName = typeName;
    }
}