using DevExpress.Utils.Menu;

namespace AF.WINFORMS.DX;

/// <summary>
/// Canvas für den WYSIWYG SQL Designer.
/// 
/// Auf diesem Canvas werden die Tables und Views platziert
/// </summary>
[ToolboxItem(false)]
public sealed class AFQueryDesignerCanvas : AFDesignerCanvas
{
    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFQueryDesignerCanvas()
    { }

    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (UI.DesignMode) return;

        AllowDrop = true;
        AutoScroll = true;

        DragEnter += dragEnter;
        DragDrop += dragDrop;

        OnJoinSelected = (_, join) =>
        {
            Designer?.JoinSelected(join);
        };
    }
     

    private void dragEnter(object? sender, DragEventArgs e)
    {
        e.Effect = DragDropEffects.None;

        if (e.Data != null && e.Data.GetData(typeof(DatabaseSchemeTable)) != null)
            e.Effect = DragDropEffects.Copy;
    }

    private void dragDrop(object? sender, DragEventArgs e)
    {
        if (e.Data != null && e.Data.GetData(typeof(DatabaseSchemeTable)) != null)
        {
            DatabaseSchemeTable? table = e.Data.GetData(typeof(DatabaseSchemeTable)) as DatabaseSchemeTable;

            if (table == null)
                return;

            var baseName = table.TABLE_NAME.ToLower();

            if (baseName.StartsWith(@"tbl_"))
                baseName = baseName.Substring(4);
            else if (baseName.StartsWith(@"vw_"))
                baseName = baseName.Substring(3);

            var uniqueCount = 0;
            var uniqueName = baseName;

            while (true)
            {
                Control? found = Controls.OfType<Control>().FirstOrDefault(predicate: ctrl => ctrl is AFQueryDesignerTable tdes && tdes.Table!.TABLE_ALIAS == uniqueName);
                if (found != null)
                {
                    ++uniqueCount;
                    uniqueName = baseName + uniqueCount;
                }
                else
                    break;
            }

            ((AFQueryDesigner?)Designer)?.AddTable(table, PointToClient(new Point(e.X, e.Y)), uniqueName);
        }
    }


    /// <summary>
    /// Eine Tabelle zum Designer hinzufügen
    /// </summary>
    /// <param name="table">Tabelle/View</param>
    /// <param name="point">Punkt, an dem das Element im Designer eingefügt werden soll</param>
    /// <param name="alias">Alias-Name der Tabelle/des Views</param>
    public DatabaseSchemeTable AddTable(DatabaseSchemeTable table, Point point, string alias)
    {
        AFQueryDesignerTable ctrl = new(table, point, alias);
        ctrl.Location = point;
        // ctrl.BackColor = Color.Blue;
        // ctrl.Id = Guid.NewGuid();
        AddElement(ctrl, point: point);

        ctrl.Table?.RaisePropertyChangedEvent(ctrl.Table!.TABLE_NAME);

        return ctrl.Table!;
    }

    /// <summary>
    /// Popup-Menu für Join
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DXPopupMenu PopupJoin { get; } = new();

    /// <summary>
    /// Popup-Menu für Tabelle
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DXPopupMenu PopupTable { get; } = new();

    /// <summary>
    /// Popup-Menu für Feld
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DXPopupMenu PopupField { get; } = new();

    /// <summary>
    /// Alle Elemente und Joins entfernen.
    /// </summary>
    public void Clear()
    {
        Controls.Clear(true);

        Joins.Clear();
        Elements.Clear();
        
        Refresh();
    }
}
