using System.Data;

namespace AF.CORE;

/// <summary>
/// Erweiterungsmethoden für DataTable
/// </summary>
public static class DataTableEx
{
    /// <summary>
    /// Umwandlung einer Zeile in der Tabelle in eine Dictionary aus Spaltenname und Wert.
    /// </summary>
    /// <param name="table">Tabelle</param>
    /// <param name="row">Zeile</param>
    /// <param name="praefix">Spaltennamen-Präfix</param>
    /// <param name="suffix">Spaltennamen-Suffix</param>
    /// <param name="upperNames">Name in Großbuchstaben umwandeln</param>
    /// <returns>Dictionary der Wert der Zeile</returns>
    public static Dictionary<string, string> ToDictionary(this DataTable table, DataRow? row, string praefix, string suffix, bool upperNames)
    {
        Dictionary<string, string> ret = [];

        foreach (DataColumn column in table.Columns)
        {
            var value = row?[column];
            if (value == null)
            {
                ret.Add(column.ColumnName, "");
                continue;
            }

            if (value is DateTime dtValue)
                ret.Add(column.ColumnName, dtValue.ToString("g"));
            else if (value is DateOnly doValue)
                ret.Add(column.ColumnName, doValue.ToString("d"));
            else if (value is TimeOnly tiValue)
                ret.Add(column.ColumnName, tiValue.ToString("t"));
            else if (value is bool bValue)
                ret.Add(column.ColumnName, bValue ? "Ja" : "Nein");
            else
                ret.Add(column.ColumnName, value.ToString() ?? "");
        }

        return ret;
    }
}
