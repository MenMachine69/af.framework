using System.Text.Json;

namespace AF.CORE;

/// <summary>
/// Variable, die eine Liste darstellt, in der ein Wert ausgewählt werden kann
/// </summary>
[Serializable]
public class VariableList : VariableBase
{
    /// <summary>
    /// Listeneinträge
    /// </summary>
    [AFBinding]
    [AFContext("Listeneinträge", Description = "Liste der Wertte, die dem Benutzer bei der Auswahl zur Verfügung gestell werden.")]
    public BindingList<VariableListEntry> Entrys { get; set; } = [];
}

/// <summary>
/// ein Eintrag in einer VariablenListe
/// </summary>
[Serializable]
public class VariableListEntry : ModelBase
{
    /// <summary>
    /// Anzeigename des Eintrags
    /// </summary>
    [AFBinding]
    [AFContext("Anzeigename", Description = "Anzeigename des Eintrags (Auswahl und gewählte Werte).")]
    public string DisplayName { get; set; } = "";

    /// <summary>
    /// Wert des Eintrags
    /// </summary>
    [AFBinding]
    [AFContext("Wert", Description = "Wert des Eintrags.")]
    public object? Value { get; set; }

    /// <summary>
    /// Gibt an, ob dies der Vorgabewert ist
    /// </summary>
    [AFBinding]
    [AFContext("Standardwert", Description = "Gibt an, ob dies der Standard-/Vorgabewert ist.")]
    public bool IsDefault { get; set; }

    /// <summary>
    /// Bestimmt den tatsächlichen Wert.
    /// </summary>
    /// <returns>Tatsächlicher Wert</returns>
    public object? GetValue()
    {
        if (Value is not JsonElement jsonElement)
            return Value;
        return jsonElement.GetString(); 
        // Aktuell scheint es mir, dass bisher nur Strings hiermit gespeichert werden. 
        // Wäre aber ausbaufähig.
    }
}