using DevExpress.XtraGrid.Views.Base;

namespace AF.WINFORMS.DX;

/// <summary>
/// Erweiterungen der Basisklasse ColumnView
/// </summary>
public static class ColumnViewEx
{
    /// <summary>
    /// Alle ausgewählten Zeilen/Objekte
    /// </summary>
    /// <param name="view">view</param>
    /// <returns>Array der Objekte, die die ausgewählten Zeilen repräsentieren</returns>
    public static object[]? GetAllSelectedRows(this ColumnView view)
    {
        if (view.SelectedRowsCount == 0)
            return null;

        var ret = new object[view.SelectedRowsCount];
        var pos = 0;

        foreach (var row in view.GetSelectedRows())
        {
            ret[pos] = view.GetRow(row);
            ++pos;
        }

        return ret;
    }
}

