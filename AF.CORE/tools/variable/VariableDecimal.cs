namespace AF.CORE;

/// <summary>
/// Variable, die einen dezimalen Wert darstellt
/// </summary>
[Serializable]
public class VariableDecimal : VariableBase
{
    /// <summary>
    /// minimal möglicher Wert
    /// </summary>
    [AFBinding]
    [AFContext("Minimum", Description = "Minimal möglicher Wert.")]
    public decimal Minimum { get; set; } = decimal.MinValue;

    /// <summary>
    /// maximal möglicher Wert
    /// </summary>
    [AFBinding]
    [AFContext("Maximum", Description = "Maximal möglicher Wert.")]
    public decimal Maximum { get; set; } = decimal.MaxValue;

    /// <summary>
    /// Standard/Vorgabewert
    /// </summary>
    [AFBinding]
    [AFContext("Vorgabewert", Description = "Vorgegebener Wert.")]
    public decimal Default { get; set; } = 0;

    /// <summary>
    /// Maske für die Eingabe
    /// </summary>
    [AFBinding]
    [AFContext("Eingabemaske", Description = "Eingabemaske für den Wert (z.B. f2 = Dezimalzahl mi zwei Kommastellen, p2 = Prozentwert mit zwei Kommastellen, c2 = Währung mit zwei Kommastellen.")]
    public virtual string DisplayMask { get; set; } = @"f2";
}