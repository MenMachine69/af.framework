namespace AF.CORE;

/// <summary>
/// Methoden zur Erweiterung von IComparable
/// </summary>
public static class IComparableEx
{
    /// <summary>
    /// Prüft, ob ein Wert innerhalb von zwei anderen liegt und gibt true zurück, wenn dies der Fall ist.
    /// Die Maximal- und Minimalwerte werden einbezogen.
    /// </summary>
    /// <param name="high">Oberer Grenzwert</param>
    /// <param name="low">Unterer Grenzwert</param>
    /// <param name="value">zu prüfender Wert</param>
    /// <example>
    /// <code>
    /// bool bRet="F".IsBetween&lt;string&gt;("A","F"); // returns true
    /// bool bRet="A".IsBetween&lt;string&gt;("A","F"); // returns true
    /// bool bRet="C".IsBetween&lt;string&gt;("A","F"); // returns true
    /// bool bRet="H".IsBetween&lt;string&gt;("A","F"); // returns false
    /// </code>
    /// </example>
    public static bool IsBetween<T>(this T value, T low, T high) where T : IComparable
    {
        return Comparer<T>.Default.Compare(low, value) <= 0
             && Comparer<T>.Default.Compare(high, value) >= 0;
    }
}
