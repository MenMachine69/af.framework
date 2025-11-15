namespace AF.CORE;

/// <summary>
/// Erweiterungen von Decimal/Double
/// </summary>
public static class DecimalDoubleEx
{
    /// <summary>
    /// Wert runden
    /// </summary>
    /// <param name="wert">Wert</param>
    /// <param name="art">Art der Rundung</param>
    /// <returns>gerundeter Wert</returns>
    /// <exception cref="ArgumentOutOfRangeException">Unbekannte Rundungsart</exception>
    public static decimal Runden(this decimal wert, eRundungsArt art)
    {
        return art switch
        {
            eRundungsArt.Hundertstel => Math.Round(wert, 2),
            eRundungsArt.Zehntel => Math.Round(wert, 1),
            eRundungsArt.Einer => Math.Round(wert, 0),
            eRundungsArt.Zehner => Math.Round(wert / 10, 0) * 10,
            _ => throw new ArgumentOutOfRangeException(nameof(art), art, "Unbekannter Rundungstyp.")
        };
    }

    /// <summary>
    /// Wert runden
    /// </summary>
    /// <param name="wert">Wert</param>
    /// <param name="art">Art der Rundung</param>
    /// <returns>gerundeter Wert</returns>
    /// <exception cref="ArgumentOutOfRangeException">Unbekannte Rundungsart</exception>
    public static double Runden(this double wert, eRundungsArt art)
    {
        return art switch
        {
            eRundungsArt.Hundertstel => Math.Round(wert, 2),
            eRundungsArt.Zehntel => Math.Round(wert, 1),
            eRundungsArt.Einer => Math.Round(wert, 0),
            eRundungsArt.Zehner => Math.Round(wert / 10, 0) * 10,
            _ => throw new ArgumentOutOfRangeException(nameof(art), art, "Unbekannter Rundungstyp.")
        };
    }

    /// <summary>
    /// Finanzmathematisch runden auf zwei Kommastellen
    /// </summary>
    /// <param name="value">zu rundender Wert</param>
    /// <returns>gerundeter Wert</returns>
    public static double RoundFinancial(this double value)
    {
        return Math.Round(Math.Truncate(value * 1000) / 1000, 2, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Finanzmathematisch runden auf zwei Kommastellen
    /// </summary>
    /// <param name="value">zu rundender Wert</param>
    /// <returns>gerundeter Wert</returns>
    public static decimal RoundFinancial(this decimal value)
    {
        return Math.Round(Math.Truncate(value * 1000) / 1000, 2, MidpointRounding.AwayFromZero);
    }

}