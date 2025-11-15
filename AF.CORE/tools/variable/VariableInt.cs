namespace AF.CORE;

/// <summary>
/// Variable, die einen Integer-Wert repräsentiert
/// </summary>
[Serializable]
public class VariableInt : VariableBase
{
    /// <summary>
    /// minimal möglicher Wert
    /// </summary>
    [AFBinding]
    [AFContext("Minimum", Description = "Minimal möglicher Wert.")]
    public int Minimum { get; set; } = int.MinValue;

    /// <summary>
    /// maximal möglicher Wert
    /// </summary>
    [AFBinding]
    [AFContext("Maximum", Description = "Maximal möglicher Wert.")]
    public int Maximum { get; set; } = int.MaxValue;

    /// <summary>
    /// Standard/Vorgabewert
    /// </summary>
    [AFBinding]
    [AFContext("Vorgabewert", Description = "Vorgegebener Wert.")]
    public int Default { get; set; } = 0;

    /// <summary>
    /// Maske für Anzeige/Bearbeitung
    /// </summary>
    [AFBinding]
    [AFContext("Eingabemaske", Description = "Eingabemaske für den Wert (z.B. f0 = Zahl, c0 = Währung)")]
    public virtual string DisplayMask { get; set; } = @"f0";
}