namespace AF.CORE;

/// <summary>
/// Erweiterungsmethoden für Int32
/// </summary>
public static class Int32Ex
{
    /// <summary>
    /// Range erlauben für Int32
    /// 
    /// Beispiel
    /// 
    /// foreach (int value in 1..100)
    /// ....
    /// </summary>
    /// <param name="range">range of Int32 values to enumerate</param>
    /// <returns>the enumerator for that range</returns>
    public static CustomIntEnumerator GetEnumerator(this Range range)
    {
        return new CustomIntEnumerator(range);
    }

    /// <summary>
    /// Range erlauben für Int32
    /// 
    /// Beispiel
    /// 
    /// foreach (int value in ..100)
    /// ....
    /// </summary>
    /// <param name="end">end of Int32 values to enumerate</param>
    /// <returns>the enumerator for that range</returns>
    public static CustomIntEnumerator GetEnumerator(this int end)
    {
        return new CustomIntEnumerator(new Range(0, end));
    }
}

/// <summary>
/// Aufzählungszeichen zur Verwendung von Int32 mit Range
/// </summary>
public ref struct CustomIntEnumerator
{
    private readonly int _max;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="range">Bereich</param>
    public CustomIntEnumerator(Range range)
    {
        if (range.End.IsFromEnd)
            throw new NotSupportedException();

        Current = range.Start.Value - 1;
        _max = range.End.Value;
    }

    /// <summary>
    /// Aktueller Wert
    /// </summary>
    public int Current { get; private set; }

    /// <summary>
    /// Gehe zum nächsten Wert
    /// </summary>
    /// <returns>erfolgreich = true, sonst false</returns>
    public bool MoveNext()
    {
        Current++;
        return Current <= _max;
    }
}

/// <summary>
/// Art der Rundung
/// </summary>
public enum eRundungsArt
{
    /// <summary>
    /// Hundertstel
    /// </summary>
    [Description("Hundertstel (0,01)")]
    Hundertstel = 0,
    /// <summary>
    /// Zehntel
    /// </summary>
    [Description("Zehntel (0,1)")]
    Zehntel = 1,
    /// <summary>
    /// Einer/Ganze
    /// </summary>
    [Description("Einer (1)")]
    Einer = 2,
    /// <summary>
    /// Zehner
    /// </summary>
    [Description("Zehner (10)")]
    Zehner = 3
}