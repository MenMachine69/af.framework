using DevExpress.Skins;
using DevExpress.Utils.Layout;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using System.Drawing.Drawing2D;
using DevExpress.Utils.Svg;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.DPI;

namespace AF.WINFORMS.DX;

/// <summary>
/// Darstellung einer Tabelle/eines Views im Designer.
/// 
/// Die Darstellung ist ein Grid mit je einer Zeile pro Feld in der Tabelle/im View.
/// </summary>
[ToolboxItem(false)]
[DesignerCategory("Code")]
public class AFQueryDesignerTable : AFDesignerCanvasElement
{
    private DatabaseSchemeTable? _table;
    private readonly AFGridControl _grid;
    private readonly GridView _view;
    private readonly TablePanel _toppanel;
    private int _dropTarget = GridControl.InvalidRowHandle;
    private GridHitInfo? _dragData;
    private System.Windows.Forms.Timer? _scrollTimer;
    private DatabaseSchemeField? _currentfield;

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="table">Tabelle/View</param>
    /// <param name="pt">Punkt für Anzeige</param>
    /// <param name="alias">Alias-Name der Tabelle</param>
    public AFQueryDesignerTable(DatabaseSchemeTable table, Point pt, string alias) : this()
    {
        _table = new(table);
        _table.TABLE_ALIAS = alias;
        Location = pt;
        Size = new(300, 450);
        Id = _table.Id != Guid.Empty ? _table.Id : Guid.NewGuid();
        _table.Id = Id;

        _grid.DataSource = _table.Fields;
        _grid.RefreshDataSource();
    }


    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFQueryDesignerTable()
    {
        Padding = new(6, ScaleHelper.DefaultDpiScale.ScaleVertical(30), 6, 6);
        CustomPaintBackground = true;
        BackgroundAppearance = new()
        {
            AutoColors = true,
            BorderWidth = 1,
            CornerRadius = 8
        };

        _toppanel = new();
        _toppanel.Size = new(100, ScaleHelper.DefaultDpiScale.ScaleVertical(30));
        _toppanel.Dock = DockStyle.Top;
        _toppanel.Rows.Add(new(TablePanelEntityStyle.AutoSize, 30));
        _toppanel.Rows.Add(new(TablePanelEntityStyle.Relative, 100));
        _toppanel.Columns.Add(new(TablePanelEntityStyle.Relative, 100));
        _toppanel.Columns.Add(new(TablePanelEntityStyle.AutoSize, 60));

        Controls.Add(_toppanel);

        var pshAddField = new SimpleButton { AutoSize = true, PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light, Text = CoreStrings.LBL_ADDFIELD };
        pshAddField.TabStop = false;
        pshAddField.AllowFocus = false;
        pshAddField.ImageOptions.SvgImage = UI.GetImage(Symbol.AddCircle);
        pshAddField.ImageOptions.SvgImageColorizationMode = DevExpress.Utils.SvgImageColorizationMode.Default;
        pshAddField.ImageOptions.SvgImageSize = new(16, 16);

        _toppanel.Controls.Add(pshAddField);
        _toppanel.SetCell(pshAddField, 0, 1);

        pshAddField.Click += _addField;

        _grid = new();
        _grid.Dock = DockStyle.Fill;
        _grid.AllowDrop = true;
        _view = new(_grid);
        //_grid.RegisterView(_view);
        _grid.MainView = _view;
        _grid.FocusedView = _view;
        _grid.ViewCollection.Add(_view);
        _view.BeginInit();
        _view.Columns.Add(new() { Name = @"colFieldName", Caption = @"Name", FieldName = nameof(DatabaseSchemeField.FIELD_NAME), VisibleIndex = 0 });
        _view.Columns.Add(new() { Name = @"colFieldTypeName", Caption = CoreStrings.TYPE, FieldName = nameof(DatabaseSchemeField.FIELD_TYPE), Width = 80, VisibleIndex = 1 });
        _view.Columns[nameof(DatabaseSchemeField.FIELD_TYPE)].OptionsColumn.FixedWidth = true;
        _view.EndInit();

        _view.RowHeight = 28;
        _view.FocusRectStyle = DrawFocusRectStyle.None;
        _view.OptionsView.ColumnAutoWidth = true;
        _view.OptionsView.ShowIndicator = false;
        _view.OptionsView.ShowGroupPanel = false;
        _view.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
        _view.OptionsSelection.MultiSelect = true;
        _view.OptionsSelection.CheckBoxSelectorField = nameof(DatabaseSchemeField.IsSelected);
        _view.OptionsBehavior.Editable = false;
        _view.OptionsCustomization.AllowColumnMoving = false;
        _view.OptionsCustomization.AllowColumnResizing = false;
        _view.OptionsCustomization.AllowFilter = false;
        _view.OptionsCustomization.AllowGroup = false;
        _view.OptionsCustomization.AllowSort = false;
        _view.OptionsCustomization.AllowQuickHideColumns = false;
        _view.OptionsMenu.EnableColumnMenu = false;
        _view.OptionsSelection.EnableAppearanceFocusedCell = false;
        _view.OptionsSelection.EnableAppearanceFocusedRow = false;
        _view.OptionsSelection.EnableAppearanceHideSelection = false;
        _view.OptionsSelection.CheckBoxSelectorColumnWidth = 40;
        _view.OptionsDetail.EnableMasterViewMode = false;


        _grid.GotFocus += (_, _) => { Activate(); };
        _grid.DragOver += _grid_DragOver;
        _grid.DragDrop += _grid_DragDrop;

        _view.SelectionChanged += (_, _) =>
        {
            _table?.RaisePropertyChangedEvent(_table.TABLE_NAME);
        };

        _view.TopRowChanged += _scrolled;
        _view.RowCellStyle += _view_RowCellStyle;
        //_view.RowStyle += _view_RowStyle;
        _view.MouseDown += _view_MouseDown;
        _view.MouseMove += _view_MouseMove;
        _view.FocusedRowObjectChanged += _view_RowSelected;
        _view.PopupMenuShowing += _beforePopupShowing;
        _view.CustomDrawCell += (_, e) =>
        {
            e.DefaultDraw();

            if (e.Column.FieldName != nameof(DatabaseSchemeField.FIELD_NAME))
            {
                e.Handled = true;
                return;
            }

            var row = _view.GetRow(e.RowHandle) as DatabaseSchemeField;

            if (row == null)
            {
                e.Handled = true;
                return;
            }

            Rectangle rect = new(e.Bounds.X + e.Bounds.Width - 20, e.Bounds.Y + ((e.Bounds.Height - 16) / 2), 16, 16);

            if (row.FIELD_GROUPBY)
            {
                e.Cache.DrawSvgImage(UI.GetImage(Symbol.GroupList), rect, SvgPaletteHelper.GetSvgPalette(UserLookAndFeel.Default, ObjectState.Normal));
                rect = new(rect.Left - 20, rect.Top, rect.Width, rect.Height);
            }

            if (row.FIELD_WHERE.IsNotEmpty())
            {
                e.Cache.DrawSvgImage(UI.GetImage(Symbol.Filter), rect,
                    SvgPaletteHelper.GetSvgPalette(UserLookAndFeel.Default, ObjectState.Normal));
            }

            e.Handled = true;
        };

        Controls.Add(_grid);
        _grid.BringToFront();

        adjustGridColors(this, EventArgs.Empty);
    }

    private void _addField(object? sender, EventArgs e)
    {
        Table?.Fields.Add(new()
        {
            FIELD_NAME = @"<newfield>",
            FIELD_TYPE = @"(calc)",
            FIELD_CALCULATED = true
        });

        _view.FocusedRowHandle = _view.GetRowHandle(Table!.Fields.Count - 1);
    }

    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (UI.DesignMode) return;

        UI.StyleChanged += adjustGridColors;
    }


    /// <inheritdoc />
    protected override void OnHandleDestroyed(EventArgs e)
    {
        base.OnHandleDestroyed(e);

        if (UI.DesignMode) return;

        UI.StyleChanged -= adjustGridColors;
    }


    private void adjustGridColors(object? sender, EventArgs args)
    {
        Color back = CommonSkins.GetSkin(UserLookAndFeel.Default).TranslateColor(SystemColors.Highlight);
        Color fore = CommonSkins.GetSkin(UserLookAndFeel.Default).TranslateColor(SystemColors.HighlightText);

        _view.Appearance.FocusedRow.BackColor = back;
        _view.Appearance.FocusedRow.ForeColor = fore;

        back = CommonSkins.GetSkin(UserLookAndFeel.Default).TranslateColor(SystemColors.Window);
        fore = CommonSkins.GetSkin(UserLookAndFeel.Default).TranslateColor(SystemColors.WindowText);

        _view.Appearance.Row.BackColor = back;
        _view.Appearance.Row.ForeColor = fore;

        back = CommonSkins.GetSkin(UserLookAndFeel.Default).TranslateColor(SystemColors.Control);
        fore = CommonSkins.GetSkin(UserLookAndFeel.Default).TranslateColor(SystemColors.ControlText);

        _view.Appearance.HideSelectionRow.BackColor = back;
        _view.Appearance.HideSelectionRow.ForeColor = fore;

        _grid.Refresh();
    }

    /// <summary>
    /// Zugriff auf die Tabelle
    /// </summary>
    public DatabaseSchemeTable? Table => _table;

    /// <summary>
    /// Rechteck, dass ein feld in der Tabelle repräsentiert.
    /// </summary>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    public Rectangle GetFieldRect(string fieldName)
    {
        Rectangle ret = new Rectangle(Location.X, Location.Y, Width, 3);

        if (_table == null) return ret;

        var rowIdx = _view.FindRow(_table.Fields.FirstOrDefault(r => r.FIELD_NAME == fieldName));
        var row = _view.GetRowHandle(rowIdx);

        GridRowInfo info = ((GridViewInfo)_view.GetViewInfo()).GetGridRowInfo(row);

        if (info == null)
        {
            ret = _view.GetRowHandle(_view.TopRowIndex) > row 
                ? new(Location.X, Location.Y + Padding.Top + _toppanel.Height, Width, 3) 
                : new Rectangle(Location.X, Location.Y + Height - 3, Width, 3);
        }
        else
            ret = new(Left, Top + Padding.Top + _toppanel.Height + info.Bounds.Top, Width, info.Bounds.Height);

        return ret;
    }

    void _showContextMenu()
    {
        ((AFQueryDesignerCanvas)Canvas!).PopupTable.Tag = this;
        ((AFQueryDesignerCanvas)Canvas!).PopupTable.ShowPopup(Canvas, PointToScreen(new(1, Padding.Top)));
    }


    private void _beforePopupShowing(object? sender, PopupMenuShowingEventArgs e)
    {
        if (sender == null) return;

        if (e.HitInfo.InRow)
            ((GridView)sender).FocusedRowHandle = e.HitInfo.RowHandle;

        DatabaseSchemeField fld = (DatabaseSchemeField)_view.GetFocusedRow();

        if (fld == null || !fld.FIELD_CALCULATED) return;

        ((AFQueryDesignerCanvas)Canvas!).PopupField.Tag = this;
        ((AFQueryDesignerCanvas)Canvas!).PopupField.ShowPopup(Canvas, _grid.PointToScreen(e.Point));
    }

    void _scrolled(object? sender, EventArgs e)
    {
        if (_scrollTimer != null)
            _scrollTimer.Stop();

        Canvas?.Refresh();

        if (_scrollTimer == null)
        {
            _scrollTimer = new();
            _scrollTimer.Interval = 200;
            _scrollTimer.Tick += (_, _) =>
            {
                _scrollTimer?.Stop();

                Canvas?.Refresh();

                _scrollTimer?.Dispose();
                _scrollTimer = null;
            };
        }

        _scrollTimer.Start();
    }

    private void _view_RowCellStyle(object? sender, RowCellStyleEventArgs e)
    {
        e.Appearance.BackColor = _view.Appearance.Row.BackColor;
        e.Appearance.ForeColor = _view.Appearance.Row.ForeColor;
        e.Appearance.Options.UseBackColor = true;
        e.Appearance.Options.UseForeColor = true;

        if (((DatabaseSchemeField)_view.GetRow(e.RowHandle)).FIELD_CALCULATED)
            e.Appearance.FontStyleDelta = FontStyle.Italic;

        if (_view.FocusedRowHandle == e.RowHandle)
        {
            if (_grid.Focused)
            {
                e.Appearance.BackColor = _view.Appearance.FocusedRow.BackColor;
                e.Appearance.ForeColor = _view.Appearance.FocusedRow.ForeColor;
            }
            else
            {
                e.Appearance.BackColor = _view.Appearance.HideSelectionRow.BackColor;
                e.Appearance.ForeColor = _view.Appearance.HideSelectionRow.ForeColor;
            }
        }
    }

    private void _view_MouseDown(object? sender, MouseEventArgs e)
    {
        GridHitInfo info = _view.CalcHitInfo(new(e.X, e.Y)); 

        if (e.Button == MouseButtons.Left && info.InRow)
            _dragData = info;
    }

    private void _view_MouseMove(object? sender, MouseEventArgs e)
    {
        if (_view.RowCount == 0)
            return;

        if (e.Button == MouseButtons.Left && _dragData != null)
        {
            Size dragSize = SystemInformation.DragSize;

            Rectangle dragRect = new Rectangle(new(_dragData.HitPoint.X - dragSize.Width / 2,
                _dragData.HitPoint.Y - dragSize.Height / 2), dragSize);

            if (dragRect.Contains(new Point(e.X, e.Y)) == false)
            {
                object obj = _view.GetRow(_dragData.RowHandle);

                if (obj != null)
                    _view.GridControl.DoDragDrop(obj, DragDropEffects.All);

                _dragData = null;
            }
        }
    }

    private void _view_RowSelected(object? sender, FocusedRowObjectChangedEventArgs e)
    {
        if (_currentfield != null)
            _currentfield.PropertyChanged -= _fieldPropertyChanged;

        _currentfield = (DatabaseSchemeField)_view.GetFocusedRow();

        Canvas!.Designer?.DetailSelected(_currentfield);

        if (_currentfield != null)
            _currentfield.PropertyChanged += _fieldPropertyChanged;
    }


    private void _fieldPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DatabaseSchemeField.FIELD_ALIAS) ||
            e.PropertyName == nameof(DatabaseSchemeField.FIELD_NAME) ||
            e.PropertyName == nameof(DatabaseSchemeField.FIELD_EXPRESSION))

            TableChanged?.Invoke(_table, EventArgs.Empty);
    }

    /// <summary>
    /// Ereignis, das ausgelöst wird, wenn sich eine Tabelle ändert (Felder etc.)
    /// </summary>
    public event EventHandler? TableChanged;

    /// <summary>
    /// Ein Feld wurde per Drag/Drop fallengelassen. Join anlegen...
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _grid_DragDrop(object? sender, DragEventArgs e)
    {
        if (e.Data == null) return;

        DatabaseSchemeField? fld = e.Data.GetData(typeof(DatabaseSchemeField)) as DatabaseSchemeField;
        DatabaseSchemeField? fldTarget = (DatabaseSchemeField)_view.GetRow(_dropTarget);

        if (fld != null && fldTarget != null)
        {
            DatabaseSchemeJoin join = new(fld.Table!, fld, fldTarget.Table!, fldTarget)
            {
                JoinType = (int)eJoinType.LeftJoin,
            };
            Canvas!.AddJoin(join);
        }
    }


    private void _grid_DragOver(object? sender, DragEventArgs e)
    {
        if (_table == null) return;

        if (e.Data == null) return;

        if (e.Data.GetDataPresent(typeof(DatabaseSchemeField)))
        {
            GridHitInfo info = _view.CalcHitInfo(_grid.PointToClient(new(e.X, e.Y)));

            if (info != null && info.InRow) //&& info.InRow) //
            {
                DatabaseSchemeField? fld = e.Data.GetData(typeof(DatabaseSchemeField)) as DatabaseSchemeField;
                if (fld != null && _table.Fields.Contains(fld) == false)
                {
                    DatabaseSchemeField fldTarget = (DatabaseSchemeField)_view.GetRow(info.RowHandle);
                    if (fldTarget != null && fldTarget.FIELD_TYPE == fld.FIELD_TYPE)
                    {
                        e.Effect = DragDropEffects.Copy;
                        _dropTarget = info.RowHandle;
                    }
                    else
                        e.Effect = DragDropEffects.None;
                }
                else
                    e.Effect = DragDropEffects.None;
            }
            else
                e.Effect = DragDropEffects.None;
        }
        else
            e.Effect = DragDropEffects.None;
    }

    /// <summary>
    /// Zeichnen der Oberfläche
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPaint(PaintEventArgs e)
    {

        base.OnPaint(e);
        // Alle Linien zeichnen
        e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
        e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
        e.Graphics.CompositingQuality = CompositingQuality.HighQuality;

        //Color c = CommonSkins.GetSkin(UserLookAndFeel.Default).TranslateColor(SystemColors.ControlText);

        Rectangle rect = new Rectangle(0, 0, Width, Padding.Top);
        TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;

        Color fore = CommonSkins.GetSkin(UserLookAndFeel.Default).TranslateColor(SystemColors.ControlText);
        TextRenderer.DrawText(e.Graphics, $@"{_table!.TABLE_NAME} ({_table!.TABLE_ALIAS})", SystemFonts.DefaultFont, rect, fore, flags);
    }


    /// <inheritdoc />
    public override void Activate()
    {
        base.Activate();

        if (BackgroundAppearance == null) return;

        BackgroundAppearance.Dimmed = true;
        BackgroundAppearance.HighlightAutoColors = true;

        Refresh();

        if (Canvas != null) Canvas.ActiveElement = this;

        if (Canvas is AFQueryDesignerCanvas qcanvas)
            qcanvas.Designer?.ElementSelected(this);
    }

    /// <inheritdoc />
    public override void Deactivate()
    {
        base.Deactivate();

        if (BackgroundAppearance == null) return;

        BackgroundAppearance.Dimmed = false;
        BackgroundAppearance.HighlightAutoColors = false;

        Refresh();
    }

    /// <inheritdoc />
    public override Point GetJoinPoint(IJoin join, bool target)
    {
        if (join is not DatabaseSchemeJoin dbjoin) throw new ArgumentException(@"Join is not a DatabaseSchemeJoin!");

        string fieldname = target
            ? _table!.Fields.FirstOrDefault(f => f.Id.Equals(dbjoin.ToField))?.FIELD_NAME ?? string.Empty
            : _table!.Fields.FirstOrDefault(f => f.Id.Equals(dbjoin.FromField))?.FIELD_NAME ?? string.Empty;

        var rect = GetFieldRect(fieldname);

        return new((target ? rect.Left : rect.Right), rect.Y + (rect.Height / 2));
    }



    /// <summary>
    /// Abfangen von Standardnachrichten und verarbeiten dieser Nachrichten...
    /// </summary>
    /// <param name="m"></param>
    protected override void WndProc(ref Message m)
    {
        bool changed = false;
        bool handled = false;

        Point pos = Cursor.Position;
        pos = PointToClient(pos);

        if (m.Msg == 0x201) // Focus...
            Activate();
        else if (m.Msg == 0x84 && AllowResize)
        {
            //if (pos.Y > Padding.Bottom && pos.Y < Padding.Top && pos.X > Padding.Left && (pos.X < (ClientSize.Width - Padding.Right) || pos.Y > Padding.Top - 2 || pos.Y < 2))
            //{
            //    BringToFront();
            //    Canvas?.SetActiveTable(this);

            //    m.Result = (IntPtr)2;  // HTCAPTION
            //    handled = true;
            //}
            if (pos.X >= ClientSize.Width - Padding.Right && pos.Y >= ClientSize.Height - Padding.Bottom)
            {
                m.Result = (IntPtr)17; // HTBOTTOMRIGHT
                handled = true;
            }
            else if (pos.X <= Padding.Left && pos.Y >= ClientSize.Height - Padding.Bottom)
            {
                m.Result = (IntPtr)16; // HTBOTTOMLEFT
                handled = true;
            }
            else if (pos.X <= Padding.Left && pos.Y <= Padding.Bottom)
            {
                m.Result = (IntPtr)13; // HTTOPLEFT
                handled = true;
            }
            else if (pos.X >= ClientSize.Width - Padding.Right && pos.Y <= Padding.Bottom)
            {
                m.Result = (IntPtr)14; // HTTOPRIGHT
                handled = true;
            }
            else if (pos.X > 0 && pos.X <= Padding.Left && pos.Y >= Padding.Bottom && pos.Y <= ClientSize.Height - Padding.Bottom)
            {
                m.Result = (IntPtr)10; // HTLEFT
                handled = true;
            }
            else if (pos.X > Padding.Left && pos.X <= ClientSize.Width - Padding.Right && pos.Y >= ClientSize.Height - Padding.Bottom)
            {
                m.Result = (IntPtr)15; // HTBOTTOM
                handled = true;
            }
            else if (pos.X > Padding.Left && pos.X <= ClientSize.Width - Padding.Right && pos.Y <= Padding.Bottom)
            {
                m.Result = (IntPtr)12; // HTTOP
                handled = true;
            }
            else if (pos.X >= ClientSize.Width - Padding.Right && pos.Y >= Padding.Bottom && pos.Y <= ClientSize.Height - Padding.Bottom)
            {
                m.Result = (IntPtr)11; // HTRIGHT
                handled = true;
            }
            else
            {
                if (Canvas?.ActiveElement != null && Canvas.ActiveElement == this)
                {
                    BringToFront();
                    if (pos.X > 22)
                    {
                        m.Result = (IntPtr)2;  // HTCAPTION
                        handled = true;
                    }
                }
            }

            if (changed) Refresh();

            if (handled) return;
        }
        else if (m.Msg == 0x84 && !AllowResize)
        {
            if (Canvas?.ActiveElement != null && Canvas.ActiveElement == this)
            {
                BringToFront();
                m.Result = (IntPtr)2;  // HTCAPTION
                handled = true;
            }

            if (changed) Refresh();

            if (handled) return;
        }

        base.WndProc(ref m);
    }
}
