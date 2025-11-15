namespace AF.CORE;

/// <summary>
/// Erweiterungsmethoden für die Klasse BindingList
/// </summary>
public static class BindingListEx
{
    /// <summary>
    /// Erweiterungsmethoden für die Klasse BindingList
    /// <example>
    /// var list = new List&lt;Int32&gt;();
    /// list.AddRange(5, 4, 8, 4, 2);
    /// </example>
    /// </summary>
    /// <typeparam name="T">Listentyp</typeparam>
    /// <param name="list">Liste, zu der die Werte hinzugefügt werden</param>
    /// <param name="values">Werte, die hinzugefügt werden sollen</param>
    public static void AddRange<T>(this BindingList<T> list, params T[] values)
    {
        list.RaiseListChangedEvents = false;

        Span<T> span = values;

        foreach (T value in span)
            list.Add(value);

        list.RaiseListChangedEvents = true;
    }

    /// <summary>
    /// Element zur BindingList hinzufügen, wenn dieses nicht NULL ist.
    /// </summary>
    /// <param name="list">liste</param>
    /// <param name="item">hinzuzufügendes Element</param>
    public static void AddNotNull(this IBindingList list, object? item)
    {
        if (item != null)
            list.Add(item);
    }

    /// <summary>
    /// Entfernt alle Elemente mit dem angegebenen PrimaryKey.
    /// </summary>
    /// <param name="list"></param>
    /// <param name="id">Der PrimaryKey des zu entfernenden Elements.</param>
    /// <returns>Anzahl der entfernten Elemente</returns>
    public static int Remove<T>(this BindingList<T> list, Guid id) where T : IModel
    {
        int count = 0;
        for (int i = list.Count - 1; i >= 0; i--)
            if (list[i].PrimaryKey == id)
            {
                list.RemoveAt(i);
                count++;
            }
        return count;
    }
}


