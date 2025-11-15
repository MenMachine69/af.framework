namespace AF.CORE;

/// <summary>
/// Variable, die eine Jahresangabe darstellt
/// 
/// Verwenden Sie GetDefault() anstelle von Default, um das Standardjahr zu erhalten 
/// (insbesondere wenn das Standardjahr relativ zum aktuellen Jahr ist) 
/// </summary>
[Serializable]
public class VariableYear : VariableBase
{
    /// <summary>
    /// minimal möglicher Wert
    /// </summary>
    [AFBinding]
    [AFContext("Minimum", Description = "Minmial mögliches Jahr.")]
    public int Minimum { get; set; } = 1000;

    /// <summary>
    /// maximal möglicher Wert
    /// </summary>
    [AFBinding]
    [AFContext("Maximum", Description = "Maximal mögliches Jahr.")]
    public int Maximum { get; set; } = 3999;

    /// <summary>
    /// Standard/Vorgabewert
    /// </summary>
    [AFBinding]
    [AFContext("Vorgabewert", Description = "Vorgegebenes Jahr.")]
    public int Default { get; set; } = DateTime.Now.Year;

    /// <summary>
    /// minimal möglicher Wert relativ zum aktuellen Jahr
    /// </summary>
    [AFBinding]
    [AFContext("Minimum relativ", Description = "Mininmal möglicher Jahreswert relativ zum aktuellen Jahr (festes Jahr = absolutes Minimum verwenden).")]
    public eRelativeYear RelativeMinimum { get; set; } = eRelativeYear.None;

    /// <summary>
    /// maximal möglicher Wert relativ zum aktuellen Jahr
    /// </summary>
    [AFBinding]
    [AFContext("Maximum relativ", Description = "Maximal möglicher Jahreswert relativ zum aktuellen Jahr (festes Jahr = absolutes Maximum verwenden).")]
    public eRelativeYear RelativeMaximum { get; set; } = eRelativeYear.None;

    /// <summary>
    /// Standardwert relativ zum aktuellen Jahr
    /// </summary>
    [AFBinding]
    [AFContext("Vorgabewert relativ", Description = "Vorgegebener möglicher Jahreswert relativ zum aktuellen Jahr (festes Jahr = absoluten Vorgabwert verwenden).")]
    public eRelativeYear RelativeDefault { get; set; } = eRelativeYear.None;

    /// <summary>
    /// gibt das minimal mögliche Jahr zurück, basierend auf RelativeMinimum und Minimum
    /// </summary>
    /// <returns>kleinstmögliches Jahr</returns>
    public int GetMinimum()
    {
        if (RelativeMinimum == eRelativeYear.None || RelativeMinimum == eRelativeYear.Fixed)
            return Minimum;

        return DateTime.Now.Year.GetRelativeYear(RelativeMinimum);
    }

    /// <summary>
    /// gibt das maximal mögliche Jahr basierend auf RelativeMaximum und Maximum zurück.
    /// </summary>
    /// <returns>größtmögliches Jahr</returns>
    public int GetMaximum()
    {
        if (RelativeMaximum == eRelativeYear.None || RelativeMaximum == eRelativeYear.Fixed)
            return Maximum;

        return DateTime.Now.Year.GetRelativeYear(RelativeMaximum);
    }

    /// <summary>
    /// gibt das Standardjahr basierend auf RelativeDefault und Default zurück
    /// </summary>
    /// <returns>Standarddatum</returns>
    public int GetDefault()
    {
        if (RelativeDefault == eRelativeYear.None || RelativeMaximum == eRelativeYear.Fixed)
            return Default;

        return DateTime.Now.Year.GetRelativeYear(RelativeDefault);
    }
}