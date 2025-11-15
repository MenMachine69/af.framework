namespace AF.CORE;

/// <summary>
/// Variable, die eine Datetime darstellt
/// 
/// Verwenden Sie GetDefault() anstelle von Default, um das Standarddatum zu erhalten 
/// (insbesondere wenn das Standarddatum relativ zum aktuellen Datum ist) 
/// </summary>
[Serializable]
public class VariableDateTime : VariableBase
{
    /// <summary>
    /// Variablentyp
    /// 
    /// Current und Default sind immer DateTime, aber Eingabesteuerungen können je nach Typ variieren.
    /// </summary>
    [AFBinding]
    [AFContext("Datumstyp", Description = "Typ der Datums-/Zeit-Variablen.")]
    public eDateTimeVariableType VariableType { get; set; } = eDateTimeVariableType.Date;

    /// <summary>
    /// minimal möglicher Wert
    /// </summary>
    [AFBinding]
    [AFContext("Minimum", Description = "Minmial möglicher Datumswert.")]
    public DateTime Minimum { get; set; } = DateTime.MinValue;

    /// <summary>
    /// maximal möglicher Wert
    /// </summary>
    [AFBinding]
    [AFContext("Maximum", Description = "Maximal möglicher Datumswert.")]
    public DateTime Maximum { get; set; } = DateTime.MaxValue;

    /// <summary>
    /// Standard/Vorgabewert
    /// </summary>
    [AFBinding]
    [AFContext("Vorgabewert", Description = "Vorgegebener Datumswert.")]
    public DateTime Default { get; set; } = DateTime.Now;

    /// <summary>
    /// minimal möglicher Wert relativ zum aktuellen Datum
    /// </summary>
    [AFBinding]
    [AFContext("Minimum relativ", Description = "Minimal möglicher Datumswert relativ zum aktuellen Datum (festes Datum = absolutes Minimum verwenden).")]
    public eRelativeDateTime RelativeMinimum { get; set; } = eRelativeDateTime.None;

    /// <summary>
    /// maximal möglicher Wert relativ zum aktuellen Datum
    /// </summary>
    [AFBinding]
    [AFContext("Maximum relativ", Description = "Maximal möglicher Datumswert relativ zum aktuellen Datum (festes Datum = absolutes Maximum verwenden).")]
    public eRelativeDateTime RelativeMaximum { get; set; } = eRelativeDateTime.None;

    /// <summary>
    /// Standardwert relativ zum aktuellen Datum
    /// </summary>
    [AFBinding]
    [AFContext("Vorgabewert relativ", Description = "Vorgegebener Datumswert relativ zum aktuellen Datum (festes Datum = absoluten Vorgabwert verwenden).")]
    public eRelativeDateTime RelativeDefault { get; set; } = eRelativeDateTime.None;

    /// <summary>
    /// gibt das minimal mögliche Datum zurück, basierend auf RelativeMinimum und Minimum
    /// </summary>
    /// <returns>kleinstmögliches Datum</returns>
    public DateTime GetMinimum()
    {
        if (RelativeMinimum == eRelativeDateTime.None || RelativeMinimum == eRelativeDateTime.Fixed)
            return Minimum;

        return DateTime.Now.GetRelativeDate(RelativeMinimum);
    }

    /// <summary>
    /// gibt das maximal mögliche Datum basierend auf RelativeMaximum und Maximum zurück.
    /// </summary>
    /// <returns>größtmögliches Datum</returns>
    public DateTime GetMaximum()
    {
        if (RelativeMaximum == eRelativeDateTime.None || RelativeMaximum == eRelativeDateTime.Fixed)
            return Maximum;

        return DateTime.Now.GetRelativeDate(RelativeMaximum);
    }

    /// <summary>
    /// gibt das Standarddatum basierend auf RelativeDefault und Default zurück
    /// </summary>
    /// <returns>Standarddatum</returns>
    public DateTime GetDefault()
    {
        if (RelativeDefault == eRelativeDateTime.None || RelativeMaximum == eRelativeDateTime.Fixed)
            return Default;

        return DateTime.Now.GetRelativeDate(RelativeDefault);
    }
}

/// <summary>
/// Typ der DateTime Variable
/// </summary>
public enum eDateTimeVariableType
{
    /// <summary>
    /// Nur datum
    /// </summary>
    [Description("Nur Datum")]
    Date,
    /// <summary>
    /// Nur Zeit
    /// </summary>
    [Description("Nur Zeit")]
    Time,
    /// <summary>
    /// Datum und Zeit
    /// </summary>
    [Description("Datum und Zeit")]
    DateAndTime
}