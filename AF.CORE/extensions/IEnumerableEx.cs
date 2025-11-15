using System.Runtime.InteropServices;

namespace AF.CORE;

/// <summary>
/// Erweiterungen von IEnumerable
/// </summary>
public static class IEnumerableEx
{
    /// <summary>
    /// Ermittelt die Position eines Strings in einer Liste von Strings.
    ///
    /// Der Vergleich ist UNABHÄNGIG von der Schreibweise (Gross/Klein - StringComparison.OrdinalIgnoreCase)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="seek"></param>
    /// <returns></returns>
    public static int IndexOfString<T>(this IEnumerable<T> list, string seek)
    {
        int idx = -1;

        foreach (var value in list)
        {
            idx += 1;
            if (value is string strval && seek.Equals(strval, StringComparison.OrdinalIgnoreCase))
                return idx;
        }

        return -1;
    }

    /// <summary>
    /// Prüft, ob eine Liste von strings einen bestimmten string enthält.
    ///
    /// Der Vergleich ist UNABHÄNGIG von der Schreibweise (Gross/Klein - StringComparison.OrdinalIgnoreCase)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">Liste</param>
    /// <param name="seek">gesuchter Wert</param>
    /// <returns></returns>
    public static bool ContainsString<T>(this IEnumerable<T> list, string seek)
    {
        return list.IndexOfString(seek) >= 0;
    }

    /// <summary>
    /// Führt einen Aktions-Delegaten für jedes Element einer IEnumerable-Auflistung aus
    /// </summary>
    /// <typeparam name="T">Typ von Objekten</typeparam>
    /// <param name="enumerable">Liste von Objekten</param>
    /// <param name="action">Aktion, die ausgeführt werden soll</param>
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var item in enumerable)
            action(item);
    }

    /// <summary>
    /// ForEach für schnelle Listenverarbeitung
    /// </summary>
    /// <typeparam name="T">Typ</typeparam>
    /// <param name="list">Liste von Objekten</param>
    /// <param name="action">Aktion, die ausgeführt werden soll</param>
    public static void ForEachFast<T>(this List<T> list, Action<T> action)
    {
#if NET48_OR_GREATER
        foreach (T item in list) action(item);
#else
        foreach (T item in CollectionsMarshal.AsSpan(list)) action(item);
#endif
    }

    /// <summary>
    /// ForEach für schnelle verbindliche Listenverarbeitung
    /// </summary>
    /// <typeparam name="T">Typ</typeparam>
    /// <param name="list">Liste von Objekten</param>
    /// <param name="action">Aktion, die ausgeführt werden soll</param>
    public static void ForEachFast<T>(this BindingList<T> list, Action<T> action)
    {
#if NET48_OR_GREATER
        foreach (T item in list.ToList()) action(item);
#else
        foreach (T item in CollectionsMarshal.AsSpan(list.ToList())) action(item);
#endif
    }


}

