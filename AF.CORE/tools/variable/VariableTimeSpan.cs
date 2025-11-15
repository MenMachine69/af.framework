namespace AF.CORE;

/// <summary>
/// Variable, die eine Zeitspanne repräsentiert
/// </summary>
[Serializable]
public class VariableTimeSpan : VariableBase
{
    /// <summary>
    /// minimal possible value
    /// </summary>
    public TimeSpan Minimum { get; set; } = TimeSpan.MinValue;

    /// <summary>
    /// maximal possible value
    /// </summary>
    public TimeSpan Maximum { get; set; } = TimeSpan.MaxValue;

    /// <summary>
    /// Maske für Anzeige/Bearbeitung
    /// </summary>
    public string DisplayMask { get; set; } = @"[d.]hh:mm";

    /// <inheritdoc />
    public override bool ValidateValue(ValidationErrorCollection errors, string name, object? value)
    {
        if (value is not TimeSpan tsvalue) return false;

        if (Minimum.TotalMinutes > 0 && tsvalue.TotalMinutes < Minimum.TotalMinutes)
        {
            errors.Add(name, $"Der angegebene Zeitraum ist zu klein. Mindestens {Minimum:[d.]hh:mm} erforderlich.");
            return false;
        }

        if (Maximum.TotalMinutes > 0 && tsvalue.TotalMinutes > Maximum.TotalMinutes)
        {
            errors.Add(name, $"Der angegebene Zeitraum ist zu groß. Maximal {Minimum:[d.]hh:mm} möglich.");
            return false;
        }

        return true;
    }
}