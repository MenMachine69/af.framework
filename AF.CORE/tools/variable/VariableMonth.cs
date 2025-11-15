namespace AF.CORE;

/// <summary>
/// Variable, die eine Monatsangabe darstellt
/// 
/// Verwenden Sie GetDefault() anstelle von Default, um den Standardmonat zu erhalten 
/// (insbesondere wenn der Standardmonat relativ zum aktuellen Monat ist) 
/// </summary>
[Serializable]
public class VariableMonth : VariableBase
{
    /// <summary>
    /// minimal möglicher Wert
    /// </summary>
    [AFBinding]
    [AFContext("Minimum", Description = "Minmial möglicher Monat.")]
    public int Minimum { get; set; } = 1;

    /// <summary>
    /// maximal möglicher Wert
    /// </summary>
    [AFBinding]
    [AFContext("Maximum", Description = "Maximal möglicher Monat.")]
    public int Maximum { get; set; } = 12;

    /// <summary>
    /// Standard/Vorgabewert
    /// </summary>
    [AFBinding]
    [AFContext("Vorgabewert", Description = "Vorgegebener Monat.")]
    public int Default { get; set; } = DateTime.Now.Month;

    /// <summary>
    /// minimal möglicher Wert relativ zum aktuellen Monat
    /// </summary>
    [AFBinding]
    [AFContext("Minimum relativ", Description = "Mininmal möglicher Monatswert relativ zum aktuellen Monat (fester Monat = absolutes Minimum verwenden).")]
    public eRelativeMonth RelativeMinimum { get; set; } = eRelativeMonth.None;

    /// <summary>
    /// maximal möglicher Wert relativ zum aktuellen Monat
    /// </summary>
    [AFBinding]
    [AFContext("Maximum relativ", Description = "Maximal möglicher Monatswert relativ zum aktuellen Monat (fester Monat = absolutes Maximum verwenden).")]
    public eRelativeMonth RelativeMaximum { get; set; } = eRelativeMonth.None;

    /// <summary>
    /// Standardwert relativ zum aktuellen Monat
    /// </summary>
    [AFBinding]
    [AFContext("Vorgabewert relativ", Description = "Vorgegebener möglicher Monatswert relativ zum aktuellen Monat (fester Monat = absoluten Vorgabwert verwenden).")]
    public eRelativeMonth RelativeDefault { get; set; } = eRelativeMonth.None;

    /// <summary>
    /// gibt den minimal möglichen Monat zurück, basierend auf RelativeMinimum und Minimum
    /// </summary>
    /// <returns>kleinstmöglicher Monat</returns>
    public int GetMinimum()
    {
        if (RelativeMinimum == eRelativeMonth.None || RelativeMinimum == eRelativeMonth.Fixed)
            return Minimum;

        return DateTime.Now.Month.GetRelativeMonth(RelativeMinimum);
    }

    /// <summary>
    /// gibt den maximal möglichen Monat basierend auf RelativeMaximum und Maximum zurück.
    /// </summary>
    /// <returns>größtmöglicher Monat</returns>
    public int GetMaximum()
    {
        if (RelativeMaximum == eRelativeMonth.None || RelativeMaximum == eRelativeMonth.Fixed)
            return Maximum;

        return DateTime.Now.Month.GetRelativeMonth(RelativeMaximum);
    }

    /// <summary>
    /// gibt den Standardmonat basierend auf RelativeDefault und Default zurück
    /// </summary>
    /// <returns>Standarddatum</returns>
    public int GetDefault()
    {
        if (RelativeDefault == eRelativeMonth.None || RelativeMaximum == eRelativeMonth.Fixed)
            return Default;

        return DateTime.Now.Month.GetRelativeMonth(RelativeDefault);
    }
}