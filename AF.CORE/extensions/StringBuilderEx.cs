using System.Text;

namespace AF.CORE;

/// <summary>
/// Erweiterungsmethoden für StringBuilder
/// </summary>
public static class StringBuilderEx
{
    /// <summary>
    /// Ersetzt eine Zeichenkette in einem StringBuilder-Objekt, wobei Groß- und Kleinschreibung ignoriert werden.
    /// </summary>
    /// <param name="sb"></param>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    public static void ReplaceIgnoreCase(this StringBuilder sb, string oldValue, string newValue)
    {
        // Abrufen der String-Darstellung des StringBuilder-Objekts
        string str = sb.ToString();

        // Alle Vorkommen von oldValue durch newValue ersetzen, Groß- und Kleinschreibung ignorieren
        str = str.Replace(oldValue, newValue, StringComparison.OrdinalIgnoreCase);

        // Löschen des StringBuilder-Objekts
        sb.Clear();

        // Anhängen der geänderten Zeichenfolge an das StringBuilder-Objekt
        sb.Append(str);
    }

    /// <summary>
    /// Prüft, ob ein StringBuilder-Objekt eine Zeichenkette enthält und ignoriert dabei Groß- und Kleinschreibung.
    /// </summary>
    /// <param name="sb"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool ContainsIgnoreCase(this StringBuilder sb, string value)
    {
        for (int i = 0; i < sb.Length - value.Length + 1; i++)
        {
            bool found = true;
            for (int j = 0; j < value.Length; j++)
            {
                if (char.ToLowerInvariant(sb[i + j]) != char.ToLowerInvariant(value[j]))
                {
                    found = false;
                    break;
                }
            }
            if (found)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Prüft, ob ein StringBuilder-Objekt eine Zeichenkette enthält und ignoriert dabei Groß- und Kleinschreibung.
    /// </summary>
    /// <param name="sb"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool Contains(this StringBuilder sb, string value)
    {
        for (int i = 0; i < sb.Length - value.Length + 1; i++)
        {
            bool found = true;
            for (int j = 0; j < value.Length; j++)
            {
                if (sb[i + j] != value[j])
                {
                    found = false;
                    break;
                }
            }
            if (found)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Mehrere Zeichenketten an StringBuilder anhängen
    /// </summary>
    /// <param name="sb">StringBuilder</param>
    /// <param name="values">Anzuhängende Werte</param>
    public static void AppendEach(this StringBuilder sb, params string[] values)
    {
        values.ForEach(v => sb.Append(v));
    }
}

