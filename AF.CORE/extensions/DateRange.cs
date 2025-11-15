namespace AF.CORE;

/// <summary>
/// Definition eines Datumsbereiches mit Start und Enddatum
/// </summary>
public class DateRange
{
    /// <summary>
    /// Beginn des Zeitraums
    /// </summary>
    public DateOnly Start { get; set; } = DateOnly.MinValue;

    /// <summary>
    /// Ende des Zeitraums
    /// </summary>
    public DateOnly End { get; set; } = DateOnly.MaxValue;

    /// <summary>
    /// Setzt den Zeitraum für den angegebenen Bereich basierend auf dem Basisdatum
    /// </summary>
    /// <param name="range">Bereich</param>
    /// <param name="basedate">Basisdatum</param>
    public void Set(eDateRange range, DateOnly basedate)
    {
        switch (range)
        {
            case eDateRange.KalenderWeek:
                Start = basedate.FirstDayOfWeek();
                End = basedate.LastDayOfWeek();
                break;
            case eDateRange.KalenderMonat:
                Start = basedate.FirstDayOfMonth();
                End = basedate.LastDayOfMonth();
                break;
            case eDateRange.KalenderQuartal:
                Start = basedate.FirstDayOfQuarter();
                End = basedate.LastDayOfQuarter();
                break;
            case eDateRange.KalenderHalbjahr:
                Start = basedate.FirstDayOfHalfYear();
                End = basedate.LastDayOfHalfYear();
                break;
            case eDateRange.KalenderJahr:
                Start = new(basedate.Year, 1, 1);
                End = new(basedate.Year, 12, 31);
                break;
        }
    }
}