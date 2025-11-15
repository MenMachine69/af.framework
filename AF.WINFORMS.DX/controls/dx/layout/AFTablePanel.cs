using AF.MVC;
using DevExpress.Utils;
using DevExpress.Utils.Extensions;
using DevExpress.Utils.Layout;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Von DevExpress.Utils.Layout.TablePanel abgeleitetes Control zur
/// tabellarischen Anordnung von Controls.
///
/// Dieses Control verfügt über einen Layoutmodus, der das dynamische Erstellen
/// von Eingabemasken via Code unterstützt. Diese müssen dann nicht mehr im Designer
/// entworfen werden. 
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Layout")]
public class AFTablePanel : TablePanel
{
    private bool _paintBackground;
    private List<layoutItem>? _layoutItems;
    private Dictionary<int, Tuple<TablePanelEntityStyle, float?>>? _layoutStyleRows;
    private Dictionary<int, Tuple<TablePanelEntityStyle, float?>>? _layoutStyleColumns;
    private int _layoutRows;
    private int _layoutColumns;
    private readonly List<Rectangle> highlightAreas = [];
    private ToolTip toolTip = new() { AutoPopDelay = 5000, InitialDelay = 1000, ReshowDelay = 500, ShowAlways = true };

    /// <summary>
    /// Custom draw Background using the BackgroundAppearances
    /// </summary>
    [Category("Custom background")]
    [Browsable(true)]
    [DefaultValue(false)]
    public bool CustomPaintBackground 
    {
        get => _paintBackground;
        set
        {
            _paintBackground = value;
            
            if (value)
                BackgroundAppearance ??= new();
            else
                BackgroundAppearance = null;

            Invalidate();
        }
    }

    /// <summary>
    /// Appearance for custom drawing background
    /// </summary>
    [DefaultValue(null)]
    [Category("Auto Padding für AFLabel und AFLabelBold")]
    public Padding? LabelPadding { get; set; } = null;

    /// <summary>
    /// Appearance for custom drawing background
    /// </summary>
    [DefaultValue(null)]
    [Category("Custom background")]
    public AFBackgroundAppearance? BackgroundAppearance { get; set; }

    /// <summary>
    /// Appearance for Hervorhebung bestimmter Bereiche
    /// </summary>
    [DefaultValue(null)]
    [Category("Highlighting background")]
    public AFBackgroundAppearance? HighlightAppearance { get; set; }

    /// <summary>
    /// Hervorgehobene Bereiche löschen und Panel neu zeichnen.
    /// </summary>
    public void ClearHighlightedAreas()
    {
        highlightAreas.Clear();
        Refresh();
    }

    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (UI.DesignMode) return;

        if (UI.Dpi > 100 && UI.AutoScalePanels)
        {
            foreach (var row in Rows.Where(r => r.Style == TablePanelEntityStyle.Absolute))
            {
                row.Height = UI.GetScaled(row.Height);
            }

            foreach (var col in Columns.Where(r => r.Style == TablePanelEntityStyle.Absolute))
            {
                col.Width = UI.GetScaled(col.Width);
            }
        }

        SetStyle(ControlStyles.ResizeRedraw, true);
        DoubleBuffered = true;
    }


    /// <summary>
    /// Markiert einen Bereich (1 basiert) von Zeilen und Spalten, die 
    /// </summary>
    /// <param name="fromZeile"></param>
    /// <param name="toZeile"></param>
    /// <param name="fromSpalte"></param>
    /// <param name="toSpalte"></param>
    public void HighlightBackground(int fromZeile, int toZeile, int fromSpalte, int toSpalte)
    {
        highlightAreas.Add(new Rectangle(fromSpalte - 1, fromZeile - 1, toSpalte - 1, toZeile - 1));
    }

    /// <summary>
    ///     Draw background if needed
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        paintBackground(e);

        paintHighlights(e);

        if (ShowOverflow)
        {
            var parent = FindForm();

            if (parent != null && parent.WindowState == FormWindowState.Normal && parent.FormBorderStyle == FormBorderStyle.Sizable)
                paintOverflow(e);
        }
    }

    private void paintOverflow(PaintEventArgs e)
    {
        // feststellen ob Controls außerhalb des sichtbaren Bereiches liegen...
        int maxx = Width;
        int maxy = Height;
        bool overflowx = false;
        bool overflowy = false;

        foreach (Control ctrl in Controls)
        {
            if (ctrl.Left > maxx || ctrl.Right > maxx || (ctrl.Visible && ctrl.Right < 1))
                overflowx = true;

            if (ctrl.Top > maxy || ctrl.Bottom > maxy || (ctrl.Visible && ctrl.Height < 1))
                overflowy = true;
        }

        if (overflowx)
        {
            using (Pen pen = new(UI.TranslateToSkinColor(Color.Red), 2f))
                e.Graphics.DrawLine(pen, Width - 2, 0, Width - 2, Height - 1);
        }

        if (overflowy)
        {
            using (Pen pen = new(UI.TranslateToSkinColor(Color.Red), 2f))
                e.Graphics.DrawLine(pen, 0, Height - 1, Width - 1, Height - 1);
        }
    }

    /// <summary>
    /// Überlauf von Controls darstellen
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ShowOverflow { get; set; } = true;


    /// <summary>
    /// Abstand der Highlight-Areas zum Rand der Zelle
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int HighlightsAreaDistance { get; set; } = 0;

    private void paintHighlights(PaintEventArgs e)
    {
        if (highlightAreas.Count < 1) return;

        if (HighlightAppearance == null) return;

        var grid = GetViewInfo().Layout.Grid;

        foreach (var area in highlightAreas)
        {
            Rectangle rect = new Rectangle(
                 grid[area.Top, area.Left].Bounds.Location,
                 new(
                     grid[area.Top, area.Width].Bounds.Right - grid[area.Top, area.Left].Bounds.Left,
                     grid[area.Height, area.Left].Bounds.Bottom - grid[area.Top, area.Left].Bounds.Top))
                .WithDeflate(new Padding(0 + HighlightsAreaDistance, 0 + HighlightsAreaDistance, 1 + HighlightsAreaDistance, 1 + HighlightsAreaDistance));

            HighlightAppearance.Draw(e.Graphics, rect, new(0), new(0));
        }
    }

    private void paintBackground(PaintEventArgs e)
    {
        if (!CustomPaintBackground) return;

        if (BackgroundAppearance == null) return;

        var rect = ClientRectangle.WithDeflate(new Padding(0, 0, 1, 1));

        BackgroundAppearance.Draw(e.Graphics, rect, Margin, Padding);
    }

    /// <summary>
    /// Control in the specified cell of the TablePanel...
    /// </summary>
    /// <param name="column">column</param>.
    /// <param name="row">row</param>
    /// <returns>Control in the cell or null</returns>
    public Control? GetControlFromPosition(int column, int row)
    {
        return Controls.Cast<Control>()
            .FirstOrDefault(c => GetColumn(c) == column && GetRow(c) == row);
    }

    /// <summary>
    /// Dem Table ein Control hinzufügen
    /// </summary>
    /// <param name="row">Zeile</param>
    /// <param name="column">Spalte</param>.
    /// <param name="ctrl">hinzuzufügendes Control</param>
    /// <param name="colspan">Anzahl Spalten</param>
    /// <param name="rowspan">Anzahl Zeilen</param>
    public void AddControl(Control ctrl, int column, int row, int? colspan = null, int? rowspan = null)
    {
        Controls.Add(ctrl);
        
        SetCell(ctrl, row, column);

        if (colspan is > 1) SetColumnSpan(ctrl, (int)colspan);
        
        if (rowspan is > 1) SetRowSpan(ctrl, (int)rowspan);
    }

    /// <summary>
    /// Layoutmodus beginnen.
    ///
    /// Nach dem Beginn können via Add neue Controls hinzugefügt werden.
    /// Der Layoutmodus wird via EndLayout abgeschlossen.
    /// ACHTUNG! BeginLayout löscht alle vorhandenen Controls des Table!
    /// </summary>
    public void BeginLayout()
    {
        this.SuspendDrawing();
        SuspendLayout();

        if (Controls.Count > 0) Controls.Clear(true);

        if (Rows.Count > 0 || Columns.Count > 0)
        {
            Rows.Clear();
            Columns.Clear();
        }

        BeginInit();

        _layoutItems = [];
        _layoutStyleColumns = [];
        _layoutStyleRows = [];
        _layoutRows = 0;
        _layoutColumns = 0;

        LayoutMode = true;

    }

    /// <summary>
    /// Gibt an, ob sich das Panel gerade im Layout-Modus befindet (BeginLayout/EndLayout)
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool LayoutMode { get; private set; }

    /// <summary>
    /// Ein neues Control im Layoutmodus hinzufügen.
    /// </summary>
    /// <typeparam name="TControl">Typ des Controls</typeparam>
    /// <param name="row">Zeile (beginnt mit 1!)</param>
    /// <param name="column">Spalte (beginnt mit 1!)</param>
    /// <param name="rowspan">Anzahl der Zeilen (optional, default = 1)</param>
    /// <param name="colspan">Anzahl der Spalten (optional, default = 1)</param>
    /// <returns>das hinzugefügte Control</returns>
    public TControl Add<TControl>(int row, int column, int rowspan = 1, int colspan = 1) where TControl : Control
    {
        var ctrl = Activator.CreateInstance<TControl>();
        return Add(ctrl, row, column, rowspan, colspan);
    }

    /// <summary>
    /// Ein Element/Widget hinzufügen
    /// </summary>
    /// <param name="ctrl">Control das als Widget angezeigt werden soll widget</param>
    /// <param name="row">Zeile (1-basiert)</param>
    /// <param name="col">Spalte (1-basiert)</param>
    /// <param name="rowspan">Zeilen</param>
    /// <param name="colspan">Spalten</param>
    public T AddWidget<T>(T ctrl, int row, int col, int rowspan, int colspan) where T : Control
    {
        HighlightBackground(row, row + rowspan - 1, col, col + colspan - 1);
        return Add(ctrl, row, col, rowspan: rowspan, colspan: colspan);
    }

    /// <summary>
    /// Ein neues Control im Layoutmodus hinzufügen.
    /// </summary>
    /// <param name="manager">Barmanager für die Toolbar</param>
    /// <param name="row">Zeile (beginnt mit 1!)</param>
    /// <param name="column">Spalte (beginnt mit 1!)</param>
    /// <param name="rowspan">Anzahl der Zeilen (optional, default = 1)</param>
    /// <param name="colspan">Anzahl der Spalten (optional, default = 1)</param>
    /// <param name="style">Dock-Stil</param>
    /// <returns>das hinzugefügte Control</returns>
    public Bar AddBar(BarManager manager, int row, int column, int rowspan = 1, int colspan = 1, DockStyle style = DockStyle.Top)
    {
        StandaloneBarDockControl dock = new() { Dock = style, AutoSize = true, BackColor = Color.BlueViolet, Margin = new(0) };
        
        if (style == DockStyle.Left || style == DockStyle.Right)
            dock.IsVertical = true;

        manager.DockControls.Add(dock);
        Bar bar = new();
        bar.CanDockStyle = BarCanDockStyle.Standalone;
        bar.StandaloneBarDockControl = dock;
        bar.OptionsBar.UseWholeRow = true;
        bar.OptionsBar.DisableClose = true;
        bar.OptionsBar.AllowQuickCustomization = false;
        bar.OptionsBar.DrawDragBorder = false;
        bar.OptionsBar.DrawSizeGrip = false;
        bar.OptionsBar.DrawBorder = false;
        bar.OptionsBar.DisableCustomization = true;
        bar.BarAppearance.Normal.BackColor = Color.Aqua;
        manager.Bars.Add(bar);

        Add(dock, row, column, rowspan, colspan);

        return bar;
    }


    /// <typeparam name="TControl">Typ des Controls</typeparam>
    /// <param name="ctrl">das hinzuzufügenden Control</param>
    /// <param name="row">Zeile (beginnt mit 1!)</param>
    /// <param name="column">Spalte (beginnt mit 1!)</param>
    /// <param name="rowspan">Anzahl der Zeilen (optional, default = 1)</param>
    /// <param name="colspan">Anzahl der Spalten (optional, default = 1)</param>
    /// <returns>das hinzugefügte Control</returns>
    public TControl Add<TControl>(TControl ctrl, int row, int column, int rowspan = 1, int colspan = 1) where TControl : Control
    {
        if (_layoutItems == null) throw new NullReferenceException("Table is currently not in layout mode.");

        if (ctrl is AFLabel label && LabelPadding is not null)
            label.Padding = (Padding)LabelPadding;

        if (ctrl is BaseEdit bedit)
            bedit.Properties.AutoHeight = true;

        if (ctrl is BaseCheckEdit bcheck)
            bcheck.Properties.AutoWidth = true;

        if (ctrl is AFEditToggle toggle)
            toggle.Dock = DockStyle.Right;
        else
            ctrl.Dock = DockStyle.Fill;

        _layoutRows = Math.Max(row + rowspan - 1, _layoutRows);
        _layoutColumns = Math.Max(column + colspan - 1, _layoutColumns);

        _layoutItems.Add(new(ctrl, row, column, rowspan, colspan));
        
        return ctrl;
    }

    /// <summary>
    /// Aktuelle Zeilenanzahl im Layout
    /// </summary>
    public int LayoutRows => _layoutRows;

    /// <summary>
    /// Aktuelle Spaltenanzahl im Layout
    /// </summary>
    public int LayoutColumns => _layoutColumns;

    /// <summary>
    /// Im Layoutmodus den Stil einer Zeile bestimmen (TablePanelEntityStyle), wenn
    /// diese NICHT TablePanelEntityStyle.AutoSize sein soll.
    /// </summary>
    /// <param name="row">zu definierende Zeile (beginnt mit 1!)</param>
    /// <param name="style">neuer Stil</param>
    /// <param name="size">Größe (% oder absolut) </param>
    public void SetRow(int row, TablePanelEntityStyle style, float? size = 0)
    {
        if (_layoutStyleRows == null) throw new NullReferenceException("Table is currently not in layout mode.");

        if (_layoutStyleRows.ContainsKey(row))
            _layoutStyleRows[row] = new(style, size);
        else
            _layoutStyleRows.Add(row, new(style, size));

        _layoutRows = Math.Max(row, _layoutRows);
    }

    /// <summary>
    /// Im Layoutmodus den Stil einer Spalte bestimmen (TablePanelEntityStyle), wenn
    /// diese NICHT TablePanelEntityStyle.AutoSize sein soll.
    /// </summary>
    /// <param name="column">zu definierende Spalte (beginnt mit 1!)</param>
    /// <param name="style">neuer Stil</param>
    /// <param name="size">Größe (% oder absolut) </param>
    public void SetColumn(int column, TablePanelEntityStyle style, float? size = 0)
    {
        if (_layoutStyleColumns == null) throw new NullReferenceException("Table is currently not in layout mode.");

        if (_layoutStyleColumns.ContainsKey(column))
            _layoutStyleColumns[column] = new(style, size);
        else
            _layoutStyleColumns.Add(column, new(style, size));

        _layoutColumns = Math.Max(column, _layoutColumns);
    }

    /// <summary>
    /// Schließt den mit BeginLayout gestarteten Layoutmodus ab.
    ///
    /// Erzeugt die via Add hinzugefügten Controls und baut die Tabelle entsprechend auf.
    /// </summary>
    public void EndLayout()
    {
        if (_layoutItems == null) throw new NullReferenceException("Table is currently not in layout mode.");

        for (var i = 1; i <= _layoutRows; ++i)
        {
            if (_layoutStyleRows?.ContainsKey(i) ?? false)
                Rows.Add(_layoutStyleRows[i].Item1, _layoutStyleRows[i].Item2 ?? 1, true);
            else
                Rows.Add(TablePanelEntityStyle.AutoSize, 1, true);
        }

        // Fake-Zeile hinzufügen, damit das Layout funktioniert... ;-)
        Rows.Add(TablePanelEntityStyle.Absolute, 1, true);

        for (var i = 1; i <= _layoutColumns; ++i)
        {
            if (_layoutStyleColumns?.ContainsKey(i) ?? false)
                Columns.Add(_layoutStyleColumns[i].Item1, _layoutStyleColumns[i].Item2 ?? 1, true);
            else
                Columns.Add(TablePanelEntityStyle.AutoSize, 1, true);
        }

        int tabstops = 0;

        foreach (var item in _layoutItems)
        {
            if (item.ctrl is AFLabel label)
            {
                label.AllowHtmlString = true;
                
                if (item.col > 1 && label.Padding.Left == 0)
                    label.Padding = new(AFTablePanelLayout.LabelPadding, label.Padding.Top, label.Padding.Right, label.Padding.Bottom);
            }

            Controls.Add(item.ctrl);

            SetCell(item.ctrl, item.row - 1, item.col - 1);
            
            if (item.colspan > 1)
                SetColumnSpan(item.ctrl, item.colspan);

            if (item.rowspan > 1)
                SetRowSpan(item.ctrl, item.rowspan);

            item.ctrl.TabIndex = tabstops;
            ++tabstops;
        }

        EndInit();
        ResumeLayout(false);
        PerformLayout();
        this.ResumeDrawing();

        if (Parent is IEditor editor)
            editor.AfterLayout(this);
       
        
        _layoutColumns = 0;
        _layoutRows = 0;
        _layoutStyleRows?.Clear();
        _layoutStyleColumns?.Clear();
        _layoutItems.Clear();

        LayoutMode = false;

    }

    /// <summary>
    /// Item in einem Layout, dass per Layoutmodus erzeugt wird.
    /// </summary>
    /// <param name="ctrl">darzustellendes Control</param>
    /// <param name="row">Zeile</param>
    /// <param name="col">Spalte</param>
    /// <param name="rowspan">Anzahl Zeilen</param>
    /// <param name="colspan">Anzahl Spalten</param>
    internal record layoutItem(Control ctrl, int row, int col, int rowspan, int colspan) { }

    /// <summary>
    /// Eingabemaske für einen ModelTyp erstellen
    /// </summary>
    /// <param name="modeltype">Typ des Models</param>
    /// <param name="controller">Controller, der ggf. benötigte CustomEditoren zur Verfügung stellt</param>
    public void FromModel(Type modeltype, IControllerUI? controller = null)
    {
        List<IVariable> properties = [];
        controller ??= modeltype.GetControllerOrNull() as IControllerUI;

        foreach (var item in modeltype.GetTypeDescription().GetProperties().Where(p => p.Property != null))
        {
            item.Property!.VAR_CONTROL = (item.Property!.UseCustomEditor ? controller?.GetCustomEditor(item, eUIElement.PropertyDialog) : ControlsEx.GetDefaultControl((PropertyInfo)item)) as Control;
            properties.Add(item.Property!);
        }

        LoadForm(properties);
    }
    
    #region Eingabeformulare

    /// <summary>
    /// Füllt ein AFTablePanel mit den Eingabe-Controls für die angegebenen Variablen.
    /// </summary>
    /// <param name="variables">Liste der variablen</param>
    public void LoadForm(IEnumerable<IVariable> variables)
    {
        this.SuspendDrawing();
        this.SuspendLayout();

        if (Controls.Count > 0)
        {
            Controls.Clear(true);
            Rows.Clear();
            Columns.Clear();
        }

        var enumerable = variables as IVariable[] ?? variables.ToArray();

        if (!enumerable.Any()) return;

        bool twoColumns = enumerable.FirstOrDefault(v => v.VAR_COLUMN == 2) != null;

        bool newRow = true;
        AFNavTabControl? currentTabCtrl = null;

        BeginLayout();

        foreach (var variable in enumerable.OrderBy(v => v.VAR_PRIORITY))
        {
            if (twoColumns && (variable.VAR_TWOCOLUMN || variable.VAR_COLUMN == 1))
                newRow = true;
            else if (twoColumns == false)
                newRow = true;

            AddSectionCaption(variable, twoColumns);
            AddSectionDescription(variable, twoColumns);

            if (!variable.VAR_NOEDITOR)
                AddEditor(variable, ref currentTabCtrl, twoColumns, newRow);
            else
            {
                AddCaption(variable, twoColumns);
                AddDescription(variable, twoColumns);
            }
        }

        if (twoColumns)
        {
            SetColumn(2, TablePanelEntityStyle.Relative, .5f);
            SetColumn(4, TablePanelEntityStyle.Relative, .5f);
        }
        else
            SetColumn(2, TablePanelEntityStyle.Relative, 1.0f);

        EndLayout();

        this.ResumeLayout();
        this.ResumeDrawing();

    }

    /// <summary>
    /// Standardhöhe der TAB-Controls in Eingabeformularen
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int DefaultTabHeightInForm { get; set; } = 250;

    /// <summary>
    /// Den passenden Editor für eine Variable zu einem TablePanel hinzufügen.
    /// </summary>
    /// <param name="variable">Variable, für die die Eingabe hinzugefügt werden soll</param>
    /// <param name="currentTabCtrl">aktuelles TAB-Control für Tabbed-Anzeige</param>
    /// <param name="twoColumns">Formular ist zweispaltig (Standard ist false)</param>
    /// <param name="newRow">in neuer Teile erzeugen (Standard ist true)</param>
    public void AddEditor(IVariable variable, ref AFNavTabControl? currentTabCtrl, bool twoColumns = false, bool newRow = true)
    {
        if (LayoutMode == false) throw new ArgumentException("Das TablePanel muss sich im Layoutmode befinden (BeginLayout) um Variablen hinzufügen zu können.");

        var ctrl = variable.VAR_CONTROL as Control ?? VariableControllerUI.Instance.GetEditor(variable);

        if (ctrl == null)
        {
            if (variable.VAR_TYP == (int)eVariableType.Section)
                return;

            throw new Exception($"Für die Variable {variable.VAR_NAME} steht kein passender Eingabeeditor zur Verfügung (Typ {variable.VAR_TYP}).");
        }
               
        if (variable.VAR_DESCRIPTION.IsNotEmpty() && ctrl is BaseEdit bedit)
            bedit.SuperTip = UI.GetSuperTip(variable.VAR_CAPTION, variable.VAR_DESCRIPTION);
        else if (variable.VAR_DESCRIPTION.IsNotEmpty())
            toolTip.SetToolTip(ctrl, variable.VAR_DESCRIPTION);

        if (variable.VAR_READONLY)
        {
            if (variable is AFProperty)
            {
                if (ctrl.HasSet("ReadOnly"))
                    ctrl.InvokeSet("ReadOnly", true);
                else
                    ctrl.Enabled = false;
            }
            else
                return;
        }

        if (variable.VAR_TABBED)
        {
            if (currentTabCtrl == null)
            {
                currentTabCtrl = new() { Dock = DockStyle.Top, Size = new(300, DefaultTabHeightInForm) };
                Add(currentTabCtrl, LayoutRows + 1, 1, colspan: (twoColumns ? 4 : 2));
            }

            currentTabCtrl.TabPages.Add(new DevExpress.XtraTab.XtraTabPage() { Text = variable.VAR_CAPTION });
            ctrl.Dock = DockStyle.Fill;
            currentTabCtrl.TabPages.Last().Controls.Add(ctrl);
            ctrl.Tag = new Tuple<IVariable, AFLabel?>(variable, null);
            return;
        }

        currentTabCtrl = null;

        if (variable is AFProperty prop)
        {
            if (prop.LineBreak)
            {
                ctrl.Tag = new Tuple<IVariable, AFLabel?>(variable, addCaption(variable, true));
                Add(ctrl, LayoutRows + 1, 1, colspan: variable.VAR_TWOCOLUMN ? 4 : 2);
                return;
            }

            ctrl.Tag = new Tuple<IVariable, AFLabel?>(variable, addCaption(variable));
            Add(ctrl, LayoutRows, variable.VAR_COLUMN == 1 ? 2 : 4, colspan: (variable.VAR_TWOCOLUMN ? 3 : 1));
            return;
        }

        switch (variable.VAR_TYP)
        {
            case (int)eVariableType.Memo:
                ctrl.Tag = new Tuple<IVariable, AFLabel?>(variable, addCaption(variable, true));
                Add(ctrl, LayoutRows + 1, 1, colspan: variable.VAR_TWOCOLUMN ? 4 : 2);
                return;
            case (int)eVariableType.RichText:
                ctrl.Tag = new Tuple<IVariable, AFLabel?>(variable, addCaption(variable, true));
                Add(ctrl, LayoutRows + 1, 1, colspan: variable.VAR_TWOCOLUMN ? 4 : 2);
                return;
            default:
                ctrl.Tag = new Tuple<IVariable, AFLabel?>(variable, addCaption(variable));
                Add(ctrl, LayoutRows, variable.VAR_COLUMN == 1 ? 2 : 4, colspan: (variable.VAR_TWOCOLUMN ? 3 : 1));
                return;
        }
    }

    /// <summary>
    /// Ein Label zu einem TablePanel hinzufügen
    /// </summary>
    /// <param name="variable">Variable, die das Label enthält</param>
    /// <param name="twoColumn">Label über zwei Spalten anzeigen</param>
    private AFLabel addCaption(IVariable variable, bool twoColumn = false)
    {
        return Add<AFLabel>(variable.VAR_COLUMN == 1 ? LayoutRows + 1 : LayoutRows, variable.VAR_COLUMN == 1 ? 1 : 3, colspan: (twoColumn ? (variable.VAR_TWOCOLUMN ? 4 : 2) : 1)).Text(variable.VAR_CAPTION).Indent(8);
    }

    /// <summary>
    /// Einem Eingabeformular eine Abschnittsüberschrift hinzufügen.
    /// </summary>
    /// <param name="variable">Variable, in der die Überschrift definiert ist</param>
    /// <param name="twoColumns">Formular ist zweispaltig</param>
    /// <exception cref="ArgumentException">Ausnahme wird ausgelöst, wenn sich das Panel nicht im LayoutModus befindet.</exception>
    public void AddSectionCaption(IVariable variable, bool twoColumns = false)
    {
        if (variable.VAR_SECCAPTION.IsEmpty()) return;

        if (LayoutMode == false) throw new ArgumentException("Das TablePanel muss sich im Layoutmode befinden (BeginLayout) um Variablen hinzufügen zu können.");

        var label = Add<AFLabelCaptionSmall>(LayoutRows + 1, 1, colspan: (twoColumns ? 4 : 2));
        label.Text = variable.VAR_SECCAPTION;
        label.Margin = new(0, 16, 0, 0);
    }

    /// <summary>
    /// Einem Eingabeformular eine Abschnittsüberschrift hinzufügen.
    /// </summary>
    /// <param name="variable">Variable, in der die Überschrift definiert ist</param>
    /// <param name="twoColumns">Formular ist zweispaltig</param>
    /// <exception cref="ArgumentException">Ausnahme wird ausgelöst, wenn sich das Panel nicht im LayoutModus befindet.</exception>
    public void AddCaption(IVariable variable, bool twoColumns = false)
    {
        if (variable.VAR_CAPTION.IsEmpty()) return;

        if (LayoutMode == false) throw new ArgumentException("Das TablePanel muss sich im Layoutmode befinden (BeginLayout) um Variablen hinzufügen zu können.");

        var label = Add<AFLabelCaptionSmall>(LayoutRows + 1, 1, colspan: (twoColumns ? 4 : 2));
        label.Text = variable.VAR_CAPTION;
        label.Margin = new(0, 16, 0, 0);
    }

    /// <summary>
    /// Einem Eingabeformular eine Abschnittsüberschrift hinzufügen.
    /// </summary>
    /// <param name="variable">Variable, in der die Überschrift definiert ist</param>
    /// <param name="twoColumns">Formular ist zweispaltig</param>
    /// <exception cref="ArgumentException">Ausnahme wird ausgelöst, wenn sich das Panel nicht im LayoutModus befindet.</exception>
    public void AddSectionDescription(IVariable variable, bool twoColumns = false)
    {
        if (variable.VAR_SECDESCRIPTION.IsEmpty()) return;

        if (LayoutMode == false) throw new ArgumentException("Das TablePanel muss sich im Layoutmode befinden (BeginLayout) um Variablen hinzufügen zu können.");

        var label = Add<AFLabelGrayText>(LayoutRows + 1, 1, colspan: (twoColumns ? 4 : 2));
        label.Text = variable.VAR_SECDESCRIPTION;
        label.AutoSizeMode = LabelAutoSizeMode.Vertical;
        label.Appearance.TextOptions.WordWrap = WordWrap.Wrap;
    }

    /// <summary>
    /// Einem Eingabeformular eine Abschnittsüberschrift hinzufügen.
    /// </summary>
    /// <param name="variable">Variable, in der die Überschrift definiert ist</param>
    /// <param name="twoColumns">Formular ist zweispaltig</param>
    /// <exception cref="ArgumentException">Ausnahme wird ausgelöst, wenn sich das Panel nicht im LayoutModus befindet.</exception>
    public void AddDescription(IVariable variable, bool twoColumns = false)
    {
        if (variable.VAR_DESCRIPTION.IsEmpty()) return;

        if (LayoutMode == false) throw new ArgumentException("Das TablePanel muss sich im Layoutmode befinden (BeginLayout) um Variablen hinzufügen zu können.");

        var label = Add<AFLabelGrayText>(LayoutRows + 1, 1, colspan: (twoColumns ? 4 : 2));
        label.Text = variable.VAR_DESCRIPTION;
        label.AutoSizeMode = LabelAutoSizeMode.Vertical;
        label.Appearance.TextOptions.WordWrap = WordWrap.Wrap;
    }



    #endregion

}