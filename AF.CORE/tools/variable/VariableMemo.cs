namespace AF.CORE;

/// <summary>
/// Variable, die eine Zeichenkette mit unbegrenzter Länge darstellt (memo)
/// </summary>
[Serializable]
public class VariableMemo : VariableString
{
    /// <summary>
    /// maximale Wertlänge
    /// </summary>
    [AFBinding]
    [AFContext("Max. Länge", Description = "Maximale Anzahl der Zeichen, die der Text lang sein darf (-1 = Länge unbegrenzt).")]
    public new int MaxLength { get; set; } = -1;

    /// <inheritdoc />
    public override bool ValidateValue(ValidationErrorCollection errors, string name, object? value)
    {
        if (value is not string strvalue) return false;

        if (MaxLength > 0 && strvalue.Length > MaxLength)
        {
            errors.Add(name, $"Der Text ist zu lang. Maximal {MaxLength} Zeichen erlaubt.");
            return false;
        }

        return true;
    }
}