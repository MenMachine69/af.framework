using DevExpress.XtraBars.Docking2010.Views.Widget;

namespace AF.WINFORMS.DX;

/// <summary>
/// Extensions of the WidgetView class (Dashboards)
/// </summary>
public static class WidgetViewEx
{
    /// <summary>
    /// Finds a free area in the WidgetView
    /// 
    /// Searches for an area of the specified size that is not yet occupied by any element.
    /// If such an area is not found, the area to be searched for is reduced step by step. 
    /// (up to a size of 1:1) and tries to find a free area. If there is no 
    /// free area, the method Rectangle.Empty returns.
    /// </summary>
    /// <param name="view">View to be searched</param>.
    /// <param name="rows">rows needed</param>.
    /// <param name="cols">required columns</param>
    /// <returns>found range or Rectangle.Empty if none is free</returns>
    public static Rectangle FindFreeArea(this WidgetView view, int rows, int cols)
    {
        Rectangle ret = Rectangle.Empty;


        while (rows > 1)
        {
            int fcols = cols;

            while (fcols > 1)
            {
                ret = _findFree(view, rows, fcols);

                if (ret != Rectangle.Empty)
                    break;

                --fcols;
            }

            if (ret != Rectangle.Empty)
                break;

            --rows;
        }

        if (ret == Rectangle.Empty)
            ret = _findFree(view, 1, 1);

        return ret;
    }

    private static Rectangle _findFree(WidgetView view, int rows, int cols)
    {
        Rectangle ret = Rectangle.Empty;

        if (view.Documents.Count > 0)
        {
            for (int row = 0; row < view.Rows.Count; row++)
            {
                for (int col = 0; col < view.Columns.Count; col++)
                {
                    // prüfen, ob oben links oben frei ist
                    if (view.Documents.OfType<Document>().FirstOrDefault(doc =>
                            row.IsBetween(doc.RowIndex, doc.RowIndex + doc.RowSpan - 1) &&
                            col.IsBetween(doc.ColumnIndex, doc.ColumnIndex + doc.ColumnSpan - 1)) != null)
                        continue;
                    else
                    {
                        // prüfen ob unten rechts unten frei ist
                        if (view.Documents.OfType<Document>().FirstOrDefault(doc =>
                                (row + rows - 1).IsBetween(doc.RowIndex, doc.RowIndex + doc.RowSpan - 1) &&
                                (col + cols - 1).IsBetween(doc.ColumnIndex, doc.ColumnIndex + doc.ColumnSpan - 1)) !=
                            null)
                            continue;
                        else
                            ret = new Rectangle(col, row, cols, rows);
                    }

                    if (!ret.IsEmpty)
                        break;
                }

                if (!ret.IsEmpty)
                    break;
            }
        }
        else
            ret = new Rectangle(0, 0, cols, rows);

        return ret;
    }
}

