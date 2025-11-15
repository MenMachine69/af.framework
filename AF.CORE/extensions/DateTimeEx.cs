namespace AF.CORE;

/// <summary>
/// Erweiterungsmethoden für die DateTime-Klasse
/// </summary>
public static class DateTimeEx
{
    /// <summary>
    /// Ermittelt, ob ein DateTime zwischen zwei angegebenen Grenzen liegt.
    /// <example>
    /// DateTime test=DateTime.Now.AddDays(1);
    /// bool check=test.Between(DateTime.Now,DateTime.Now.AddDays(2));
    /// </example>
    /// </summary>
    /// <param name="source">Zu prüfende Datumszeit</param>
    /// <param name="low">Untere Grenze</param>
    /// <param name="high">Obergrenze</param>
    /// <returns>true wenn der Wert größer oder gleich dem unteren und kleiner oder gleich dem oberen Grenzwert ist</returns>
    public static bool Between(this DateTime source, DateTime low, DateTime high)
    {
        return source.IsBetween(low, high);
    }

    /// <summary>
    /// Subtrahiert eine Anzahl von Tagen von einem DateTime
    /// </summary>
    /// <param name="source">DateTime, von der subtrahiert wird</param>
    /// <param name="days">Anzahl der Tage</param>
    /// <returns>neue DateTime</returns>
    public static DateTime SubtractDays(this DateTime source, int days)
    {
        return source.Subtract(new TimeSpan(days, 0, 0, 0));
    }

    /// <summary>
    /// Gibt das Viertel des Datums zurück
    /// </summary>
    /// <param name="source">Datumszeitpunkt, dessen Quartal ermittelt wird</param>
    /// <returns>Quartal, in dem das Datum liegt (1 - 4)</returns>
    public static int GetQuarter(this DateTime source)
    {
        return (source.Month / 4) + 1;
    }

    /// <summary>
    /// Gibt den Monat des Beginns des Quartals des Datums zurück.
    /// </summary>
    /// <param name="source">Datumszeitpunkt, dessen Quartalsbeginn ermittelt wird</param>
    /// <returns>Nummer des ersten Monats des Quartals (1, 4, 7 oder 10)</returns>
    public static int GetFirstMonthOfQuarter(this DateTime source)
    {
        return ((GetQuarter(source) - 1) * 3) + 1;
    }

    /// <summary>
    /// Gibt den Monat des Endes des Quartals des Datums zurück.
    /// </summary>
    /// <param name="source">DatumZeitpunkt, dessen Quartalsende ermittelt wird</param>
    /// <returns>Nummer des ersten Monats des Quartals (3, 6, 9 oder 12)</returns>
    public static int GetLastMonthOfQuarter(this DateTime source)
    {
        return GetQuarter(source) * 3;
    }

    /// <summary>
    /// Gibt das Halbjahr zurück, in dem das Datum liegt.
    /// </summary>
    /// <param name="source">DatumZeitpunkt, dessen Halbjahr ermittelt wird</param>
    /// <returns>Halbjahr, in dem das Datum liegt (1 oder 2)</returns>
    public static int GetHalfYear(this DateTime source)
    {
        return ((source.Month - 1) / 6) + 1;
    }

    /// <summary>
    /// Anzahl der vollen Monate zwischen zwei Datumswerten
    /// </summary>
    /// <param name="Date1">Startdatum</param>
    /// <param name="Date2">Enddatum</param>
    /// <returns>Anzahl der Monate</returns>
    public static int MonthsBetween(this DateTime Date1, DateTime Date2)
    {
        // Beide Daten in einer Liste speichern und sortieren 
        List<DateTime> period = [Date1, Date2];
        period.Sort(DateTime.Compare);

        // Monate zählen
        int months;
        for (months = 0; period[0].AddMonths(months + 1).CompareTo(period[1]) <= 0; months++)
        {
        }

        return months;
    }

    /// <summary>
    /// Ermittelt den letzten Tag im aktuellen Monat einer Datetime
    /// </summary>
    /// <param name="date">Quelldatum</param>
    /// <returns>letzter Tag im Monat als datetime</returns>
    public static DateTime LastDayOfMonth(this DateTime date)
    {
        return new DateTime((date.Month < 12 ? date.Year : date.Year + 1), (date.Month < 12 ? date.Month + 1 : 1), 1)
            .SubtractDays(1);
    }

    /// <summary>
    /// Ermittelt den ersten Tag des aktuellen Quartals
    /// </summary>
    /// <param name="date">Quelldatum</param>
    /// <returns>Erster Tag des Quartals</returns>
    public static DateTime FirstDayOfQuarter(this DateTime date)
    {
        return date.Month switch
        {
            <= 3 => new DateTime(date.Year, 1, 1),
            <= 6 => new DateTime(date.Year, 4, 1),
            <= 9 => new DateTime(date.Year, 7, 1),
            _ => new DateTime(date.Year, 10, 1)
        };
    }

    /// <summary>
    /// Ermittelt den letzten Tag des aktuellen Quartals
    /// </summary>
    /// <param name="date">Quelldatum</param>
    /// <returns>Letzter Tag des Quartals</returns>
    public static DateTime LastDayOfQuarter(this DateTime date)
    {
        return date.Month switch
        {
            <= 3 => new DateTime(date.Year, 3, 31),
            <= 6 => new DateTime(date.Year, 6, 30),
            <= 9 => new DateTime(date.Year, 9, 30),
            _ => new DateTime(date.Year, 12, 31)
        };
    }

    /// <summary>
    /// Ermittelt den ersten Tag des aktuellen Halbjahres
    /// </summary>
    /// <param name="date">Quelldatum</param>
    /// <returns>erster Tag des Halbjahres</returns>
    public static DateTime FirstDayOfHalfYear(this DateTime date)
    {
        return (date.Month <= 6 ? new DateTime(date.Year, 1, 1) : new DateTime(date.Year, 7, 1));
    }

    /// <summary>
    /// Ermittelt den letzten Tag des aktuellen Halbjahres
    /// </summary>
    /// <param name="date">Quelldatum</param>
    /// <returns>letzter Tag des Halbjahres</returns>
    public static DateTime LastDayOfHalfYear(this DateTime date)
    {
        return (date.Month <= 6 ? new DateTime(date.Year, 6, 30) : new DateTime(date.Year, 12, 31));
    }


    /// <summary>
    /// Ermittelt den letzten Tag der aktuellen Woche
    /// </summary>
    /// <param name="date">Quelldatum</param>
    /// <returns>letzter Tag der aktuellen Woche</returns>
    public static DateTime LastDayOfWeek(this DateTime date)
    {
        return date.AddDays(7 - Convert.ToDouble(date.DayOfWeekNumber()));
    }

    /// <summary>
    /// Ermittelt den ersten Tag der aktuellen Woche
    /// </summary>
    /// <param name="date">Quelldatum</param>
    /// <returns>erster Tag der aktuellen Woche</returns>
    public static DateTime FirstDayOfWeek(this DateTime date)
    {
        return date.LastDayOfWeek().SubtractDays(6);
    }

    /// <summary>
    /// Ermittelt den ersten Tag des Monats
    /// </summary>
    /// <param name="date">Quelldatum</param>
    /// <returns>erster Tag des Monats</returns>
    public static DateTime FirstDayOfMonth(this DateTime date)
    {
        return new DateTime(date.Year, date.Month, 1).Date;
    }


    /// <summary>
    /// Bestimmt den Wochentag als numerischen Wert (1=Montag, 7 = Sonntag)
    /// </summary>
    /// <param name="date">Quelldatum</param>
    /// <returns>Wochentag als numerischer Wert</returns>
    public static int DayOfWeekNumber(this DateTime date)
    {
        return Array.IndexOf(
        [
            DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday,
                DayOfWeek.Saturday, DayOfWeek.Sunday
        ], date.DayOfWeek) + 1;
    }

    /// <summary>
    /// Setzt die Zeit der DateTime auf den angegebenen Wert.
    /// </summary>
    /// <param name="date">DateTime, deren Wert gesetzt werden soll</param>
    /// <param name="hour">Stunde</param>
    /// <param name="minute">Minute</param>
    /// <param name="second">Sekunde</param>
    /// <returns>Neues Datum mit Uhrzeit</returns>
    public static DateTime SetTime(this DateTime date, int hour, int minute, int second)
    {
        return new DateTime(date.Year, date.Month, date.Day, hour, minute, second);
    }

    /// <summary>
    /// Gibt den Monatsnamen für den angegebenen Monat zurück.
    /// </summary>
    /// <param name="month">Monat</param>
    /// <returns>Name des Monats</returns>
    public static string GetMonthName(int month)
    {
        checked
        {
            if (month.IsBetween(1, 12))
            {
                return new[]
                {
                    CoreStrings.JANUARY, CoreStrings.FEBRUARY, CoreStrings.MARCH, CoreStrings.APRIL, CoreStrings.MAY, CoreStrings.JUNE,
                    CoreStrings.JULI, CoreStrings.AUGUST, CoreStrings.SEPTEMBER, CoreStrings.OCTOBER, CoreStrings.NOVEMBER, CoreStrings.DECEMBER
                }[month - 1];
            }
        }

        return CoreStrings.UNKNOWN;
    }

    /// <summary>
    /// Gibt den kurzen, dreistelligen Monatsnamen für den angegebenen Monat zurück.
    /// </summary>
    /// <param name="month">Monat</param>
    /// <returns>3-stelliger Name des Monats</returns>
    public static string GetMonthNameShort(int month)
    {
        checked
        {
            if (month.IsBetween(1, 12))
            {
                return new[]
                    { @"Jan", @"Feb", @"Mrz", @"Apr", @"Mai", @"Jun", @"Jul", @"Aug", @"Sept", @"Okt", @"Nov", @"Dez" }[month - 1];
            }
        }

        return @"xxx";
    }

    /// <summary>
    /// Gibt das relative Jahr zum aktuellen Jahr zurück...
    /// </summary>
    /// <param name="year">aktuelles Jahr</param>
    /// <param name="relation">Relation</param>
    /// <returns>relatives Jahr</returns>
    public static int GetRelativeYear(this int year, eRelativeYear relation)
    {
        return relation switch
        {
            eRelativeYear.Following => year + 1,
            eRelativeYear.Previous => year - 1,
            _ => year
        };
    }

    /// <summary>
    /// Gibt den relativen Monat zum aktuellen Monat zurück...
    /// </summary>
    /// <param name="month">aktueller Monat</param>
    /// <param name="relation">Relation</param>
    /// <returns>relativer Monat</returns>
    public static int GetRelativeMonth(this int month, eRelativeMonth relation)
    {
        return relation switch
        {
            eRelativeMonth.Following => month + 1 > 12 ? 1 : month + 1,
            eRelativeMonth.Previous => month - 1 < 1 ? 12 : month - 1,
            eRelativeMonth.FirstOfHalfYear => month <= 6 ? 1 : 7,
            eRelativeMonth.LastOfHalfYear => month <= 6 ? 6 : 12,
            eRelativeMonth.FirstOfNextHalfYear => month <= 6 ? 7 : 1,
            eRelativeMonth.LastOfNextHalfYear => month <= 6 ? 12 : 6,
            eRelativeMonth.FirstOfPreviousHalfYear => month <= 6 ? 7 : 1,
            eRelativeMonth.LastOfPreviousHalfYear => month <= 6 ? 12 : 6,
            eRelativeMonth.FirstOfQuarter => month.IsBetween(1,3) ? 1 : month.IsBetween(4,6) ? 2 : month.IsBetween(7,9) ? 3 : 4,
            eRelativeMonth.LastOfQuarter => month.IsBetween(1, 3) ? 3 : month.IsBetween(4, 6) ? 6 : month.IsBetween(7, 9) ? 9 : 12,
            eRelativeMonth.FirstOfNextQuarter => month.IsBetween(1, 3) ? 4 : month.IsBetween(4, 6) ? 7 : month.IsBetween(7, 9) ? 10 : 1,
            eRelativeMonth.LastOfNextQuarter => month.IsBetween(1, 3) ? 6 : month.IsBetween(4, 6) ? 9 : month.IsBetween(7, 9) ? 12 : 3,
            eRelativeMonth.FirstOfPreviousQuarter => month.IsBetween(1, 3) ? 10 : month.IsBetween(4, 6) ? 1 : month.IsBetween(7, 9) ? 4 : 7,
            eRelativeMonth.LastOfPreviousQuarter => month.IsBetween(1, 3) ? 12 : month.IsBetween(4, 6) ? 3 : month.IsBetween(7, 9) ? 6 : 9,
            _ => month
        };
    }

    /// <summary>
    /// Gibt das relative Datum zum aktuellen Datum zurück...
    /// </summary>
    /// <param name="date">aktuelles Datum</param>
    /// <param name="relation">Relation</param>
    /// <returns>relatives Datum</returns>
    public static DateTime GetRelativeDate(this DateTime date, eRelativeDateTime relation)
    {
        DateTime ret;

        switch (relation)
        {
            case eRelativeDateTime.Today:
                ret = date;
                break;
            case eRelativeDateTime.Tomorrow:
                ret = date.AddDays(1);
                break;
            case eRelativeDateTime.Yesterday:
                ret = date.SubtractDays(1);
                break;
            case eRelativeDateTime.FirstOfWorkDayWeek:
                ret = date.SubtractDays(date.DayOfWeekNumber() - 1);
                break;
            case eRelativeDateTime.LastOfWorkDayWeek:
                ret = date.AddDays(5 - date.DayOfWeekNumber());
                break;
            case eRelativeDateTime.FirstOfPreviousWorkDayWeek:
                ret = date.SubtractDays(7).SubtractDays(date.DayOfWeekNumber() - 1);
                break;
            case eRelativeDateTime.LastOfPreviousWorkDayWeek:
                ret = date.SubtractDays(7).AddDays(5 - date.DayOfWeekNumber());
                break;
            case eRelativeDateTime.FirstOfNextWorkDayWeek:
                ret = date.AddDays(7).SubtractDays(date.DayOfWeekNumber() - 1);
                break;
            case eRelativeDateTime.LastOfNextWorkDayWeek:
                ret = date.AddDays(7).AddDays(5 - date.DayOfWeekNumber());
                break;
            case eRelativeDateTime.FirstOfWeek:
                ret = date.SubtractDays(date.DayOfWeekNumber() - 1);
                break;
            case eRelativeDateTime.LastOfWeek:
                ret = date.AddDays(7 - date.DayOfWeekNumber());
                break;
            case eRelativeDateTime.FirstOfPreviousWeek:
                ret = date.SubtractDays(7).SubtractDays(date.DayOfWeekNumber() - 1);
                break;
            case eRelativeDateTime.LastOfPreviousWeek:
                ret = date.SubtractDays(7).AddDays(7 - date.DayOfWeekNumber());
                break;
            case eRelativeDateTime.FirstOfNextWeek:
                ret = date.AddDays(7).SubtractDays(date.DayOfWeekNumber() - 1);
                break;
            case eRelativeDateTime.LastOfNextWeek:
                ret = date.AddDays(7).AddDays(7 - date.DayOfWeekNumber());
                break;
            case eRelativeDateTime.FirstOfMonth:
                ret = new DateTime(date.Year, date.Month, 1);
                break;
            case eRelativeDateTime.LastOfMonth:
                ret = new DateTime(date.Year + (date.Month == 12 ? 1 : 0), (date.Month == 12 ? 1 : date.Month + 1), 1);
                ret = ret.SubtractDays(1);
                break;
            case eRelativeDateTime.FirstOfPreviousMonth:
                ret = new DateTime(date.Year - (date.Month == 1 ? 1 : 0), (date.Month == 1 ? 12 : date.Month - 1), 1);
                break;
            case eRelativeDateTime.LastOfPreviousMonth:
                ret = new DateTime(date.Year, date.Month, 1);
                ret = ret.SubtractDays(1);
                break;
            case eRelativeDateTime.FirstOfNextMonth:
                ret = new DateTime(date.Year + (date.Month == 12 ? 1 : 0), (date.Month == 12 ? 1 : date.Month + 1), 1);
                break;
            case eRelativeDateTime.LastOfNextMonth:
                ret = new DateTime(date.Year + (date.Month == 12 ? 1 : 0), (date.Month == 12 ? 2 : date.Month + 2), 1);
                ret = ret.SubtractDays(1);
                break;
            case eRelativeDateTime.FirstOfQuarter:
                ret = new DateTime(date.Year, date.GetFirstMonthOfQuarter(), 1);
                break;
            case eRelativeDateTime.LastOfQuarter:
                ret = new DateTime(date.Year, date.GetLastMonthOfQuarter(), 1);
                ret = ret.LastDayOfMonth();
                break;
            case eRelativeDateTime.FirstOfNextQuarter:
                ret = new DateTime(date.Year + (date.GetQuarter() == 4 ? 1 : 0),
                    (date.GetQuarter() == 4 ? 1 : date.GetLastMonthOfQuarter() + 1), 1);
                break;
            case eRelativeDateTime.LastOfNextQuarter:
                ret = new DateTime(date.Year + (date.GetQuarter() == 4 ? 1 : 0),
                    (date.GetQuarter() == 4 ? 1 : date.GetLastMonthOfQuarter() + 1), 1);
                ret = new DateTime(ret.Year, ret.GetLastMonthOfQuarter(), 1);
                ret = ret.LastDayOfMonth();
                break;
            case eRelativeDateTime.FirstOfPreviousQuarter:
                ret = new DateTime(date.Year - (date.GetQuarter() == 1 ? 1 : 0),
                    (date.GetQuarter() == 1 ? 10 : date.GetFirstMonthOfQuarter() - 1), 1);
                break;
            case eRelativeDateTime.LastOfPreviousQuarter:
                ret = new DateTime(date.Year - (date.GetQuarter() == 1 ? 1 : 0),
                    (date.GetQuarter() == 1 ? 12 : date.GetFirstMonthOfQuarter() - 1), 1);
                ret = new DateTime(ret.Year, ret.GetLastMonthOfQuarter(), 1);
                ret = ret.LastDayOfMonth();
                break;
            case eRelativeDateTime.FirstOfHalfYear:
                ret = new DateTime(date.Year, (date.GetHalfYear() == 1 ? 1 : 7), 1);
                break;
            case eRelativeDateTime.LastOfHalfYear:
                ret = new DateTime(date.Year, (date.GetHalfYear() == 1 ? 6 : 12), 1);
                ret = ret.LastDayOfMonth();
                break;
            case eRelativeDateTime.FirstOfNextHalfYear:
                ret = new DateTime(date.Year + (date.GetHalfYear() == 2 ? 1 : 0), (date.GetHalfYear() == 1 ? 7 : 1), 1);
                break;
            case eRelativeDateTime.LastOfNextHalfYear:
                ret = new DateTime(date.Year + (date.GetHalfYear() == 2 ? 1 : 0), (date.GetHalfYear() == 1 ? 12 : 6),
                    1);
                ret = ret.LastDayOfMonth();
                break;
            case eRelativeDateTime.FirstOfPreviousHalfYear:
                ret = new DateTime(date.Year - (date.GetHalfYear() == 1 ? 1 : 0), (date.GetHalfYear() == 1 ? 7 : 1), 1);
                break;
            case eRelativeDateTime.LastOfPreviousHalfYear:
                ret = new DateTime(date.Year - (date.GetHalfYear() == 1 ? 1 : 0), (date.GetHalfYear() == 1 ? 12 : 6),
                    1);
                ret = ret.LastDayOfMonth();
                break;
            case eRelativeDateTime.FirstOfYear:
                ret = new DateTime(date.Year, 1, 1);
                break;
            case eRelativeDateTime.LastOfYear:
                ret = new DateTime(date.Year, 12, 31);
                break;
            case eRelativeDateTime.FirstOfNextYear:
                ret = new DateTime(date.Year + 1, 1, 1);
                break;
            case eRelativeDateTime.LastOfNextYear:
                ret = new DateTime(date.Year + 1, 12, 31);
                break;
            case eRelativeDateTime.FirstOfPreviousYear:
                ret = new DateTime(date.Year - 1, 1, 1);
                break;
            case eRelativeDateTime.LastOfPreviousYear:
                ret = new DateTime(date.Year - 1, 12, 31);
                break;
            case eRelativeDateTime.None:
                ret = date;
                break;
            case eRelativeDateTime.Fixed:
                ret = date;
                break;
            default:
                ret = DateTime.MinValue;
                break;
        }

        return ret;
    }

    /// <summary>
    /// Gibt den größeren der beiden übergebenen Datumswerte zurück (wie Math.Max für numerische Werte).
    /// </summary>
    /// <param name="datetime1"></param>
    /// <param name="datetime2"></param>
    /// <returns>DateTime</returns>
    public static DateTime Max(DateTime datetime1, DateTime datetime2)
    {
        return (datetime2 > datetime1 ? datetime2 : datetime1);
    }

    /// <summary>
    /// Gibt den kleineren der beiden übergebenen Datumswerte zurück (wie Math.Min für numerische Werte)
    /// </summary>
    /// <param name="datetime1"></param>
    /// <param name="datetime2"></param>
    /// <returns>DateTime</returns>
    public static DateTime Min(DateTime datetime1, DateTime datetime2)
    {
        return (datetime2 < datetime1 ? datetime2 : datetime1);
    }

    /// <summary>
    /// Gibt einen Text zurück, der eine relative Uhrzeit enthält (vor 3 Tagen, vor 2 Stunden usw.).
    /// </summary>
    /// <param name="datetime">Datumsstempel umgewandelt in relative Textangabe</param>
    /// <returns>relative Textangabe</returns>
    public static string GetDateTimeText(this DateTime datetime)
    {
        if (datetime > DateTime.Now) // in der Zukunft
            return datetime.ToShortDateString() + @" " + datetime.ToShortTimeString();

        if ((DateTime.Now - datetime).TotalMinutes < 120) // in den letzten 2 Stunden
            return CoreStrings.DATE_BEFOREMINUTES.DisplayWith(Convert.ToInt32((DateTime.Now - datetime).TotalMinutes));

        if ((DateTime.Now - datetime).TotalHours < 48) // in den letzten 2 Tagen
            return CoreStrings.DATE_BEFOREHOURS.DisplayWith(Convert.ToInt32((DateTime.Now - datetime).TotalHours));

        if ((DateTime.Now - datetime).TotalDays < 90) // in den letzten 90 Tagen
            return CoreStrings.DATE_BEFOREDAYS.DisplayWith(Convert.ToInt32((DateTime.Now - datetime).TotalDays));

        if ((DateTime.Now - datetime).TotalDays < 730) // in den letzten 2 Jahren
            return CoreStrings.DATE_BEFOREMONTHS.DisplayWith(Convert.ToInt32((DateTime.Now - datetime).TotalDays / 30));

        return CoreStrings.DATE_BEVORYEARS.DisplayWith(Convert.ToInt32((DateTime.Now - datetime).TotalDays / 365));
    }

    /// <summary>
    /// Wandelt ein DateTime in ein DateOnly
    /// </summary>
    /// <param name="dateTime">Das umzuwandelnde DateTime</param>
    /// <returns>Das DateTime als DateOnly</returns>
    public static DateOnly ToDateOnly(this DateTime dateTime) => DateOnly.FromDateTime(dateTime);
}

/// <summary>
/// Erweiterungsmethoden für die DateOnly-Klasse
/// </summary>
public static class DateOnlyEx
{
    /// <summary>
    /// Ermittelt, ob ein DateOnly zwischen zwei angegebenen Grenzen liegt.
    /// <example>
    /// DateOnly test=DateOnly.Now.AddDays(1);
    /// bool check=test.Between(DateOnly.Now,DateOnly.Now.AddDays(2));
    /// </example>
    /// </summary>
    /// <param name="source">Zu prüfende Datumszeit</param>
    /// <param name="low">Untere Grenze</param>
    /// <param name="high">Obergrenze</param>
    /// <returns>true wenn der Wert größer oder gleich dem unteren und kleiner oder gleich dem oberen Grenzwert ist</returns>
    public static bool Between(this DateOnly source, DateOnly low, DateOnly high)
    {
        return source.IsBetween(low, high);
    }

    /// <summary>
    /// Subtrahiert eine Anzahl von Tagen von einem DateOnly
    /// </summary>
    /// <param name="source">DateOnly, von der subtrahiert wird</param>
    /// <param name="days">Anzahl der Tage</param>
    /// <returns>neue DateOnly</returns>
    public static DateOnly SubtractDays(this DateOnly source, int days)
    {
        return source.AddDays(-days);
    }

    /// <summary>
    /// Gibt das Viertel des Datums zurück
    /// </summary>
    /// <param name="source">Datumszeitpunkt, dessen Quartal ermittelt wird</param>
    /// <returns>Quartal, in dem das Datum liegt (1 - 4)</returns>
    public static int GetQuarter(this DateOnly source)
    {
        return (source.Month / 4) + 1;
    }

    /// <summary>
    /// Gibt den Monat des Beginns des Quartals des Datums zurück.
    /// </summary>
    /// <param name="source">Datumszeitpunkt, dessen Quartalsbeginn ermittelt wird</param>
    /// <returns>Nummer des ersten Monats des Quartals (1, 4, 7 oder 10)</returns>
    public static int GetFirstMonthOfQuarter(this DateOnly source)
    {
        return ((GetQuarter(source) - 1) * 3) + 1;
    }

    /// <summary>
    /// Gibt den Monat des Endes des Quartals des Datums zurück.
    /// </summary>
    /// <param name="source">DatumZeitpunkt, dessen Quartalsende ermittelt wird</param>
    /// <returns>Nummer des ersten Monats des Quartals (3, 6, 9 oder 12)</returns>
    public static int GetLastMonthOfQuarter(this DateOnly source)
    {
        return GetQuarter(source) * 3;
    }

    /// <summary>
    /// Gibt das Halbjahr zurück, in dem das Datum liegt.
    /// </summary>
    /// <param name="source">DatumZeitpunkt, dessen Halbjahr ermittelt wird</param>
    /// <returns>Halbjahr, in dem das Datum liegt (1 oder 2)</returns>
    public static int GetHalfYear(this DateOnly source)
    {
        return ((source.Month - 1) / 6) + 1;
    }

    /// <summary>
    /// Anzahl der vollen Monate zwischen zwei Datumswerten
    /// </summary>
    /// <param name="Date1">Startdatum</param>
    /// <param name="Date2">Enddatum</param>
    /// <returns>Anzahl der Monate</returns>
    public static int MonthsBetween(this DateOnly Date1, DateOnly Date2)
    {
        // Beide Daten in einer Liste speichern und sortieren 
        List<DateOnly> period;
        if (Date1 < Date2)
            period = [Date1, Date2];
        else
            period = [Date2, Date1];

        // Monate zählen
        int months;
        for (months = 0; period[0].AddMonths(months + 1).CompareTo(period[1]) <= 0; months++)
            ;

        return months;
    }

    /// <summary>
    /// Ermittelt den letzten Tag im aktuellen Monat einer Datetime
    /// </summary>
    /// <param name="date">Quelldatum</param>
    /// <returns>letzter Tag im Monat als datetime</returns>
    public static DateOnly LastDayOfMonth(this DateOnly date)
    {
        return new DateOnly((date.Month < 12 ? date.Year : date.Year + 1), (date.Month < 12 ? date.Month + 1 : 1), 1)
            .SubtractDays(1);
    }

    /// <summary>
    /// Ermittelt den ersten Tag des aktuellen Quartals
    /// </summary>
    /// <param name="date">Quelldatum</param>
    /// <returns>Erster Tag des Quartals</returns>
    public static DateOnly FirstDayOfQuarter(this DateOnly date)
    {
        return date.Month switch
        {
            <= 3 => new DateOnly(date.Year, 1, 1),
            <= 6 => new DateOnly(date.Year, 4, 1),
            <= 9 => new DateOnly(date.Year, 7, 1),
            _ => new DateOnly(date.Year, 10, 1)
        };
    }

    /// <summary>
    /// Ermittelt den letzten Tag des aktuellen Quartals
    /// </summary>
    /// <param name="date">Quelldatum</param>
    /// <returns>Letzter Tag des Quartals</returns>
    public static DateOnly LastDayOfQuarter(this DateOnly date)
    {
        return date.Month switch
        {
            <= 3 => new DateOnly(date.Year, 3, 31),
            <= 6 => new DateOnly(date.Year, 6, 30),
            <= 9 => new DateOnly(date.Year, 9, 30),
            _ => new DateOnly(date.Year, 12, 31)
        };
    }

    /// <summary>
    /// Ermittelt den ersten Tag des aktuellen Halbjahres
    /// </summary>
    /// <param name="date">Quelldatum</param>
    /// <returns>erster Tag des Halbjahres</returns>
    public static DateOnly FirstDayOfHalfYear(this DateOnly date)
    {
        return (date.Month <= 6 ? new DateOnly(date.Year, 1, 1) : new DateOnly(date.Year, 7, 1));
    }

    /// <summary>
    /// Ermittelt den letzten Tag des aktuellen Halbjahres
    /// </summary>
    /// <param name="date">Quelldatum</param>
    /// <returns>letzter Tag des Halbjahres</returns>
    public static DateOnly LastDayOfHalfYear(this DateOnly date)
    {
        return (date.Month <= 6 ? new DateOnly(date.Year, 6, 30) : new DateOnly(date.Year, 12, 31));
    }


    /// <summary>
    /// Ermittelt den letzten Tag der aktuellen Woche
    /// </summary>
    /// <param name="date">Quelldatum</param>
    /// <returns>letzter Tag der aktuellen Woche</returns>
    public static DateOnly LastDayOfWeek(this DateOnly date)
    {
        return date.AddDays(7 - Convert.ToInt32(date.DayOfWeekNumber()));
    }

    /// <summary>
    /// Ermittelt den ersten Tag der aktuellen Woche
    /// </summary>
    /// <param name="date">Quelldatum</param>
    /// <returns>erster Tag der aktuellen Woche</returns>
    public static DateOnly FirstDayOfWeek(this DateOnly date)
    {
        return date.LastDayOfWeek().SubtractDays(6);
    }

    /// <summary>
    /// Ermittelt den ersten Tag des Monats
    /// </summary>
    /// <param name="date">Quelldatum</param>
    /// <returns>erster Tag des Monats</returns>
    public static DateOnly FirstDayOfMonth(this DateOnly date)
    {
        return new DateOnly(date.Year, date.Month, 1);
    }


    /// <summary>
    /// Bestimmt den Wochentag als numerischen Wert (1=Montag, 7 = Sonntag)
    /// </summary>
    /// <param name="date">Quelldatum</param>
    /// <returns>Wochentag als numerischer Wert</returns>
    public static int DayOfWeekNumber(this DateOnly date)
    {
        return Array.IndexOf(
        [
            DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday,
                DayOfWeek.Saturday, DayOfWeek.Sunday
        ], date.DayOfWeek) + 1;
    }

    /// <summary>
    /// Setzt die Zeit der DateOnly auf den angegebenen Wert.
    /// </summary>
    /// <param name="date">DateOnly, deren Wert gesetzt werden soll</param>
    /// <param name="hour">Stunde</param>
    /// <param name="minute">Minute</param>
    /// <param name="second">Sekunde</param>
    /// <returns>Neues Datum mit Uhrzeit</returns>
    public static DateTime SetTime(this DateOnly date, int hour, int minute, int second)
    {
        return new DateTime(date.Year, date.Month, date.Day, hour, minute, second);
    }

    /// <summary>
    /// Gibt den Monatsnamen für den angegebenen Monat zurück.
    /// </summary>
    /// <param name="month">Monat</param>
    /// <returns>Name des Monats</returns>
    public static string GetMonthName(int month)
    {
        checked
        {
            if (month.IsBetween(1, 12))
            {
                return new[]
                {
                    CoreStrings.JANUARY, CoreStrings.FEBRUARY, CoreStrings.MARCH, CoreStrings.APRIL, CoreStrings.MAY, CoreStrings.JUNE,
                    CoreStrings.JULI, CoreStrings.AUGUST, CoreStrings.SEPTEMBER, CoreStrings.OCTOBER, CoreStrings.NOVEMBER, CoreStrings.DECEMBER
                }[month - 1];
            }
        }

        return CoreStrings.UNKNOWN;
    }

    /// <summary>
    /// Gibt den kurzen, dreistelligen Monatsnamen für den angegebenen Monat zurück.
    /// </summary>
    /// <param name="month">Monat</param>
    /// <returns>3-stelliger Name des Monats</returns>
    public static string GetMonthNameShort(int month)
    {
        checked
        {
            if (month.IsBetween(1, 12))
            {
                return new[]
                    { @"Jan", @"Feb", @"Mrz", @"Apr", @"Mai", @"Jun", @"Jul", @"Aug", @"Sept", @"Okt", @"Nov", @"Dez" }[month - 1];
            }
        }

        return @"xxx";
    }

    /// <summary>
    /// Gibt das relative Datum zum aktuellen Datum zurück...
    /// </summary>
    /// <param name="date">aktuelles Datum</param>
    /// <param name="relation">Relation</param>
    /// <returns>relatives Datum</returns>
    public static DateOnly GetRelativeDate(this DateOnly date, eRelativeDateTime relation)
    {
        DateOnly ret;

        switch (relation)
        {
            case eRelativeDateTime.Today:
                ret = date;
                break;
            case eRelativeDateTime.Tomorrow:
                ret = date.AddDays(1);
                break;
            case eRelativeDateTime.Yesterday:
                ret = date.SubtractDays(1);
                break;
            case eRelativeDateTime.FirstOfWorkDayWeek:
                ret = date.SubtractDays(date.DayOfWeekNumber() - 1);
                break;
            case eRelativeDateTime.LastOfWorkDayWeek:
                ret = date.AddDays(5 - date.DayOfWeekNumber());
                break;
            case eRelativeDateTime.FirstOfPreviousWorkDayWeek:
                ret = date.SubtractDays(7).SubtractDays(date.DayOfWeekNumber() - 1);
                break;
            case eRelativeDateTime.LastOfPreviousWorkDayWeek:
                ret = date.SubtractDays(7).AddDays(5 - date.DayOfWeekNumber());
                break;
            case eRelativeDateTime.FirstOfNextWorkDayWeek:
                ret = date.AddDays(7).SubtractDays(date.DayOfWeekNumber() - 1);
                break;
            case eRelativeDateTime.LastOfNextWorkDayWeek:
                ret = date.AddDays(7).AddDays(5 - date.DayOfWeekNumber());
                break;
            case eRelativeDateTime.FirstOfWeek:
                ret = date.SubtractDays(date.DayOfWeekNumber() - 1);
                break;
            case eRelativeDateTime.LastOfWeek:
                ret = date.AddDays(7 - date.DayOfWeekNumber());
                break;
            case eRelativeDateTime.FirstOfPreviousWeek:
                ret = date.SubtractDays(7).SubtractDays(date.DayOfWeekNumber() - 1);
                break;
            case eRelativeDateTime.LastOfPreviousWeek:
                ret = date.SubtractDays(7).AddDays(7 - date.DayOfWeekNumber());
                break;
            case eRelativeDateTime.FirstOfNextWeek:
                ret = date.AddDays(7).SubtractDays(date.DayOfWeekNumber() - 1);
                break;
            case eRelativeDateTime.LastOfNextWeek:
                ret = date.AddDays(7).AddDays(7 - date.DayOfWeekNumber());
                break;
            case eRelativeDateTime.FirstOfMonth:
                ret = new DateOnly(date.Year, date.Month, 1);
                break;
            case eRelativeDateTime.LastOfMonth:
                ret = new DateOnly(date.Year + (date.Month == 12 ? 1 : 0), (date.Month == 12 ? 1 : date.Month + 1), 1);
                ret = ret.SubtractDays(1);
                break;
            case eRelativeDateTime.FirstOfPreviousMonth:
                ret = new DateOnly(date.Year - (date.Month == 1 ? 1 : 0), (date.Month == 1 ? 12 : date.Month - 1), 1);
                break;
            case eRelativeDateTime.LastOfPreviousMonth:
                ret = new DateOnly(date.Year, date.Month, 1);
                ret = ret.SubtractDays(1);
                break;
            case eRelativeDateTime.FirstOfNextMonth:
                ret = new DateOnly(date.Year + (date.Month == 12 ? 1 : 0), (date.Month == 12 ? 1 : date.Month + 1), 1);
                break;
            case eRelativeDateTime.LastOfNextMonth:
                ret = new DateOnly(date.Year + (date.Month == 12 ? 1 : 0), (date.Month == 12 ? 2 : date.Month + 2), 1);
                ret = ret.SubtractDays(1);
                break;
            case eRelativeDateTime.FirstOfQuarter:
                ret = new DateOnly(date.Year, date.GetFirstMonthOfQuarter(), 1);
                break;
            case eRelativeDateTime.LastOfQuarter:
                ret = new DateOnly(date.Year, date.GetLastMonthOfQuarter(), 1);
                ret = ret.LastDayOfMonth();
                break;
            case eRelativeDateTime.FirstOfNextQuarter:
                ret = new DateOnly(date.Year + (date.GetQuarter() == 4 ? 1 : 0),
                    (date.GetQuarter() == 4 ? 1 : date.GetLastMonthOfQuarter() + 1), 1);
                break;
            case eRelativeDateTime.LastOfNextQuarter:
                ret = new DateOnly(date.Year + (date.GetQuarter() == 4 ? 1 : 0),
                    (date.GetQuarter() == 4 ? 1 : date.GetLastMonthOfQuarter() + 1), 1);
                ret = new DateOnly(ret.Year, ret.GetLastMonthOfQuarter(), 1);
                ret = ret.LastDayOfMonth();
                break;
            case eRelativeDateTime.FirstOfPreviousQuarter:
                ret = new DateOnly(date.Year - (date.GetQuarter() == 1 ? 1 : 0),
                    (date.GetQuarter() == 1 ? 10 : date.GetFirstMonthOfQuarter() - 1), 1);
                break;
            case eRelativeDateTime.LastOfPreviousQuarter:
                ret = new DateOnly(date.Year - (date.GetQuarter() == 1 ? 1 : 0),
                    (date.GetQuarter() == 1 ? 12 : date.GetFirstMonthOfQuarter() - 1), 1);
                ret = new DateOnly(ret.Year, ret.GetLastMonthOfQuarter(), 1);
                ret = ret.LastDayOfMonth();
                break;
            case eRelativeDateTime.FirstOfHalfYear:
                ret = new DateOnly(date.Year, (date.GetHalfYear() == 1 ? 1 : 7), 1);
                break;
            case eRelativeDateTime.LastOfHalfYear:
                ret = new DateOnly(date.Year, (date.GetHalfYear() == 1 ? 6 : 12), 1);
                ret = ret.LastDayOfMonth();
                break;
            case eRelativeDateTime.FirstOfNextHalfYear:
                ret = new DateOnly(date.Year + (date.GetHalfYear() == 2 ? 1 : 0), (date.GetHalfYear() == 1 ? 7 : 1), 1);
                break;
            case eRelativeDateTime.LastOfNextHalfYear:
                ret = new DateOnly(date.Year + (date.GetHalfYear() == 2 ? 1 : 0), (date.GetHalfYear() == 1 ? 12 : 6),
                    1);
                ret = ret.LastDayOfMonth();
                break;
            case eRelativeDateTime.FirstOfPreviousHalfYear:
                ret = new DateOnly(date.Year - (date.GetHalfYear() == 1 ? 1 : 0), (date.GetHalfYear() == 1 ? 7 : 1), 1);
                break;
            case eRelativeDateTime.LastOfPreviousHalfYear:
                ret = new DateOnly(date.Year - (date.GetHalfYear() == 1 ? 1 : 0), (date.GetHalfYear() == 1 ? 12 : 6),
                    1);
                ret = ret.LastDayOfMonth();
                break;
            case eRelativeDateTime.FirstOfYear:
                ret = new DateOnly(date.Year, 1, 1);
                break;
            case eRelativeDateTime.LastOfYear:
                ret = new DateOnly(date.Year, 12, 31);
                break;
            case eRelativeDateTime.FirstOfNextYear:
                ret = new DateOnly(date.Year + 1, 1, 1);
                break;
            case eRelativeDateTime.LastOfNextYear:
                ret = new DateOnly(date.Year + 1, 12, 31);
                break;
            case eRelativeDateTime.FirstOfPreviousYear:
                ret = new DateOnly(date.Year - 1, 1, 1);
                break;
            case eRelativeDateTime.LastOfPreviousYear:
                ret = new DateOnly(date.Year - 1, 12, 31);
                break;
            case eRelativeDateTime.None:
                ret = date;
                break;
            case eRelativeDateTime.Fixed:
                ret = date;
                break;
            default:
                ret = DateOnly.MinValue;
                break;
        }

        return ret;
    }

    /// <summary>
    /// Gibt den größeren der beiden übergebenen Datumswerte zurück (wie Math.Max für numerische Werte).
    /// </summary>
    /// <param name="datetime1"></param>
    /// <param name="datetime2"></param>
    /// <returns>DateOnly</returns>
    public static DateOnly Max(DateOnly datetime1, DateOnly datetime2)
    {
        return (datetime2 > datetime1 ? datetime2 : datetime1);
    }

    /// <summary>
    /// Gibt den kleineren der beiden übergebenen Datumswerte zurück (wie Math.Min für numerische Werte)
    /// </summary>
    /// <param name="datetime1"></param>
    /// <param name="datetime2"></param>
    /// <returns>DateOnly</returns>
    public static DateOnly Min(DateOnly datetime1, DateOnly datetime2)
    {
        return (datetime2 < datetime1 ? datetime2 : datetime1);
    }

    /// <summary>
    /// Gibt einen Text zurück, der eine relative Uhrzeit enthält (vor 3 Tagen, vor 2 Stunden usw.).
    /// </summary>
    /// <param name="dateonly">Datumsstempel umgewandelt in relative Textangabe</param>
    /// <returns>relative Textangabe</returns>
    public static string GetDateOnlyText(this DateOnly dateonly)
    {
        DateTime datetime = dateonly.ToDateTime(TimeOnly.MinValue); 
        if (datetime > DateTime.Now) // in der Zukunft
            return datetime.ToShortDateString() + @" " + datetime.ToShortTimeString();

        if ((DateTime.Now - datetime).TotalMinutes < 120) // in den letzten 2 Stunden
            return CoreStrings.DATE_BEFOREMINUTES.DisplayWith(Convert.ToInt32((DateTime.Now - datetime).TotalMinutes));

        if ((DateTime.Now - datetime).TotalHours < 48) // in den letzten 2 Tagen
            return CoreStrings.DATE_BEFOREHOURS.DisplayWith(Convert.ToInt32((DateTime.Now - datetime).TotalHours));

        if ((DateTime.Now - datetime).TotalDays < 90) // in den letzten 90 Tagen
            return CoreStrings.DATE_BEFOREDAYS.DisplayWith(Convert.ToInt32((DateTime.Now - datetime).TotalDays));

        if ((DateTime.Now - datetime).TotalDays < 730) // in den letzten 2 Jahren
            return CoreStrings.DATE_BEFOREMONTHS.DisplayWith(Convert.ToInt32((DateTime.Now - datetime).TotalDays / 30));

        return CoreStrings.DATE_BEVORYEARS.DisplayWith(Convert.ToInt32((DateTime.Now - datetime).TotalDays / 365));
    }
}

/// <summary>
/// relative Monatsangabe
/// </summary>
public enum eRelativeMonth
{
    /// <summary>
    /// Aktueller Monat
    /// </summary>
    [Description("Aktueller Monat")]
    Current = 0,
    /// <summary>
    /// Folgender Monat
    /// </summary>
    [Description("Folgemonat")]
    Following = 1,
    /// <summary>
    /// Vormonat
    /// </summary>
    [Description("Vormonat")]
    Previous = 2,
    /// <summary>
    /// Erster Monat des aktuellen Quartals
    /// </summary>
    [Description("Erster des Quartals")]
    FirstOfQuarter = 3,
    /// <summary>
    /// Letzter Monat des aktuellen Quartals
    /// </summary>
    [Description("Letzter des Quartals")]
    LastOfQuarter = 4,
    /// <summary>
    /// Erster Monat des vorangegangenen Quartals
    /// </summary>
    [Description("Erster des Vor-Quartals")]
    FirstOfPreviousQuarter = 5,
    /// <summary>
    /// Letzter Monat des vorangegangenen Quartals
    /// </summary>
    [Description("Letzter des Vor-Quartals")]
    LastOfPreviousQuarter = 6,
    /// <summary>
    /// Erster Monat des nachfolgenden Quartals
    /// </summary>
    [Description("Erster des Folge-Quartals")]
    FirstOfNextQuarter = 7,
    /// <summary>
    /// Letzter Monat des nachfolgenden Quartals
    /// </summary>
    [Description("Letzter des Folge-Quartals")]
    LastOfNextQuarter = 8,
    /// <summary>
    /// Erster des aktuellen Halbjahres
    /// </summary>
    [Description("Erster des aktuellen Halbjahres")]
    FirstOfHalfYear = 9,
    /// <summary>
    /// Letzter des aktuellen Halbjahres
    /// </summary>
    [Description("Letzter des aktuellen Halbjahres")]
    LastOfHalfYear = 10,
    /// <summary>
    /// Erster des nächsten Halbjahres
    /// </summary>
    [Description("Erster des nächsten Halbjahres")]
    FirstOfNextHalfYear = 11,
    /// <summary>
    /// Letzter des nächsten Halbjahres
    /// </summary>
    [Description("Letzter des nächsten Halbjahres")]
    LastOfNextHalfYear = 12,
    /// <summary>
    /// Erster des vorangegangenen Halbjahres
    /// </summary>
    [Description("Erster des vorangegangenen Halbjahres")]
    FirstOfPreviousHalfYear = 13,
    /// <summary>
    /// Letzter des vorangegangenen Halbjahres
    /// </summary>
    [Description("Letzter des vorangegangenen Halbjahres")]
    LastOfPreviousHalfYear = 14,
    /// <summary>
    /// fester Monat
    /// </summary>
    [Description("fester Monat")]
    Fixed = 15,
    /// <summary>
    /// nicht definiert...
    /// </summary>
    [Description("keine Vorgabe")]
    None = 16
}

/// <summary>
/// relative Monatsangabe
/// </summary>
public enum eRelativeYear
{
    /// <summary>
    /// Aktuelles Jahr
    /// </summary>
    [Description("Aktuelles Jahr")]
    Current = 0,
    /// <summary>
    /// Folgendes Jahr
    /// </summary>
    [Description("Folgejahr")]
    Following = 1,
    /// <summary>
    /// Vorjahr
    /// </summary>
    [Description("Vorjahr")]
    Previous = 2,
    /// <summary>
    /// festes Jahr
    /// </summary>
    [Description("festes Jahr")]
    Fixed = 3,
    /// <summary>
    /// nicht definiert...
    /// </summary>
    [Description("keine Vorgabe")]
    None = 4
}

/// <summary>
/// relative Zeiten
/// </summary>
public enum eRelativeDateTime
{
    /// <summary>
    /// Heute
    /// </summary>
    [Description("Heute/Jetzt")]
    Today = 0,
    /// <summary>
    /// Morgen
    /// </summary>
    [Description("Morgen")]
    Tomorrow = 1,
    /// <summary>
    /// Gestern
    /// </summary>
    [Description("Gestern")]
    Yesterday = 2,
    /// <summary>
    /// erster Arbeitstag der Woche
    /// </summary>
    [Description("Erster Arbeitstag der Woche")]
    FirstOfWorkDayWeek = 3,
    /// <summary>
    /// letzer Arbeitstag der Woche
    /// </summary>
    [Description("Letzter Arbeitstag der Woche")]
    LastOfWorkDayWeek = 4,
    /// <summary>
    /// erster Arbeitstag der Vorwoche
    /// </summary>
    [Description("Erster Arbeitstag der Vorwoche")]
    FirstOfPreviousWorkDayWeek = 5,
    /// <summary>
    /// letzter Arbeitstag der Vorwoche
    /// </summary>
    [Description("Letzter Arbeitstag der Vorwoche")]
    LastOfPreviousWorkDayWeek = 6,
    /// <summary>
    /// erster Arbeitstag der nüchsten Woche
    /// </summary>
    [Description("Erster Arbeitstag der nächsten Woche")]
    FirstOfNextWorkDayWeek = 7,
    /// <summary>
    /// erster Arbeitstag der nüchsten Woche
    /// </summary>
    [Description("Letzter Arbeitstag der nächsten Woche")]
    LastOfNextWorkDayWeek = 8,
    /// <summary>
    /// erster Tag der Woche
    /// </summary>
    [Description("Erster Tag der Woche")]
    FirstOfWeek = 9,
    /// <summary>
    /// letzter Tag der Woche
    /// </summary>
    [Description("Letzter Tag der Woche")]
    LastOfWeek = 10,
    /// <summary>
    /// erster Tag der Vorwoche
    /// </summary>
    [Description("Erster Tag der Vorwoche")]
    FirstOfPreviousWeek = 11,
    /// <summary>
    /// letzter Tag der Vorwoche
    /// </summary>
    [Description("Letzter Tag der Vorwoche")]
    LastOfPreviousWeek = 12,
    /// <summary>
    /// erster Tag der nüchsten Woche
    /// </summary>
    [Description("Erster Tag der nächsten Woche")]
    FirstOfNextWeek = 13,
    /// <summary>
    /// letzter Tag der nüchsten Woche
    /// </summary>
    [Description("Letzter Tag der nächsten Woche")]
    LastOfNextWeek = 14,
    /// <summary>
    /// erster Tag der des Monats
    /// </summary>
    [Description("Erster Tag des Monats")]
    FirstOfMonth = 15,
    /// <summary>
    /// letzter Tag der des Monats
    /// </summary>
    [Description("Letzter Tag des Monats")]
    LastOfMonth = 16,
    /// <summary>
    /// erster Tag der des Vormonats
    /// </summary>
    [Description("Erster Tag des Vormonats")]
    FirstOfPreviousMonth = 17,
    /// <summary>
    /// letzter Tag der des Vormonats
    /// </summary>
    [Description("Letzter Tag des Vormonats")]
    LastOfPreviousMonth = 18,
    /// <summary>
    /// erster Tag der des nüchsten Monats
    /// </summary>
    [Description("Erster Tag des nächsten Monats")]
    FirstOfNextMonth = 19,
    /// <summary>
    /// letzter Tag der des nüchsten Monats
    /// </summary>
    [Description("Letzter Tag des nächsten Monats")]
    LastOfNextMonth = 20,
    /// <summary>
    /// erster Tag des Quartals
    /// </summary>
    [Description("Erster Tag des Quartals")]
    FirstOfQuarter = 21,
    /// <summary>
    /// letzter Tag des Quartals
    /// </summary>
    [Description("Letzter Tag des Quartals")]
    LastOfQuarter = 22,
    /// <summary>
    /// erster Tag des vorigen Quartals
    /// </summary>
    [Description("Erster Tag des Vorquartals")]
    FirstOfPreviousQuarter = 23,
    /// <summary>
    /// letzter Tag des vorigen Quartals
    /// </summary>
    [Description("Letzter Tag des Vorquartals")]
    LastOfPreviousQuarter = 24,
    /// <summary>
    /// erster Tag des nüchsten Quartals
    /// </summary>
    [Description("Erster Tag des nächsten Quartals")]
    FirstOfNextQuarter = 25,
    /// <summary>
    /// letzter Tag des nüchsten Quartals
    /// </summary>
    [Description("Letzter Tag des nächsten Quartals")]
    LastOfNextQuarter = 26,
    /// <summary>
    /// erster Tag des Halbjahres
    /// </summary>
    [Description("Erster Tag des Halbjahres")]
    FirstOfHalfYear = 27,
    /// <summary>
    /// letzter Tag des Halbjahres
    /// </summary>
    [Description("Letzter Tag des Halbjahres")]
    LastOfHalfYear = 28,
    /// <summary>
    /// erster Tag des vorigen Halbjahres
    /// </summary>
    [Description("Erster Tag des Vor-Halbjahres")]
    FirstOfPreviousHalfYear = 29,
    /// <summary>
    /// letzter Tag des vorigen Halbjahres
    /// </summary>
    [Description("Letzter Tag des Vor-Halbjahres")]
    LastOfPreviousHalfYear = 30,
    /// <summary>
    /// erster Tag des nüchsten Halbjahres
    /// </summary>
    [Description("Erster Tag des nächsten Halbjahres")]
    FirstOfNextHalfYear = 31,
    /// <summary>
    /// letzter Tag des nüchsten Halbjahres
    /// </summary>
    [Description("Letzter Tag des nächsten Halbjahres")]
    LastOfNextHalfYear = 32,
    /// <summary>
    /// erster Tag des Jahres
    /// </summary>
    [Description("Erster Tag des Jahres")]
    FirstOfYear = 33,
    /// <summary>
    /// letzter Tag des Jahres
    /// </summary>
    [Description("Letzter Tag des Jahres")]
    LastOfYear = 34,
    /// <summary>
    /// erster Tag des Vorjahres
    /// </summary>
    [Description("Erster Tag des Vorjahres")]
    FirstOfPreviousYear = 35,
    /// <summary>
    /// letzter Tag des Vorjahres
    /// </summary>
    [Description("Letzter Tag des Vorjahres")]
    LastOfPreviousYear = 36,
    /// <summary>
    /// erster Tag des nüchsten Vorjahres
    /// </summary>
    [Description("Erster Tag des Folgejahres")]
    FirstOfNextYear = 37,
    /// <summary>
    /// letzter Tag des nüchsten Vorjahres
    /// </summary>
    [Description("Letzter Tag des Folgejahres")]
    LastOfNextYear = 38,
    /// <summary>
    /// nicht definiert...
    /// </summary>
    [Description("keine Vorgabe")]
    None = 39,
    /// <summary>
    /// festes Datum
    /// </summary>
    [Description("festes Datum")]
    Fixed = 40
}

/// <summary>
/// Bereiche für die Definition eines Zeitraums
/// </summary>
public enum eDateRange
{
    /// <summary>
    /// Kalenderwoche
    /// </summary>
    KalenderWeek,
    /// <summary>
    /// Kalendermonat
    /// </summary>
    KalenderMonat,
    /// <summary>
    /// Quartal
    /// </summary>
    KalenderQuartal,
    /// <summary>
    /// Halbjahr
    /// </summary>
    KalenderHalbjahr,
    /// <summary>
    /// Jahr
    /// </summary>
    KalenderJahr
}