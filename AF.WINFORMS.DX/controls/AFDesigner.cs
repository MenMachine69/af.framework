using DevExpress.Skins;
using System.Drawing.Drawing2D;

namespace AF.WINFORMS.DX;

/// <summary>
/// Basisklasse der Designer
/// </summary>
[ToolboxItem(false)]
public class AFDesigner : AFEditorBase
{
    /// <summary>
    /// Neuaufbau erzwingen...
    /// </summary>
    public virtual void RaiseNeedRecreate() { }

    /// <summary>
    /// ein Join wurde ausgewählt
    /// </summary>
    /// <param name="join">ausgewählter Join</param>
    public virtual void JoinSelected(IJoin? join) { }

    /// <summary>
    /// Detail eines Elements wurde ausgewählt
    /// </summary>
    /// <param name="detail"></param>
    public virtual void DetailSelected(object? detail) { }

    /// <summary>
    /// eine Element wurde ausgewählt
    /// </summary>
    /// <param name="element">ausgewähltes Element</param>
    public virtual void ElementSelected(AFDesignerCanvasElement? element) { }
}

/// <summary>
/// Control, dass als Designer-Oberfläche für Elemente verwendet werden kann,
/// die auf der Oberfläche verschoben und durch Linien verbunden werden können.
///
/// Typische Anwendungsfälle sind z.B. Query- oder WorkflowDesigner auf denen
/// Tabellen oder WorkflowBausteine dargestellt werden sollen.
///
/// Über ein AFDesignerCanvasModel-Objekt können u.a. Größe und Position der
/// Elemente persistiert werden.
/// </summary>
[ToolboxItem(false)]
public class AFDesignerCanvas : AFScrollablePanel
{
    private readonly Dictionary<Guid, AFDesignerCanvasElement> _elements = [];
    private AFDesignerCanvasElement? _activeElement;
    private readonly BindingList<IJoin> _joins = [];
    private IJoin? _currentJoin;

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFDesignerCanvas()
    {
        if (UI.DesignMode) return;

        SetStyle(ControlStyles.UserPaint, true);
        SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        SetStyle(ControlStyles.DoubleBuffer, true);
        SetStyle(ControlStyles.ResizeRedraw, true);
        SetStyle(ControlStyles.SupportsTransparentBackColor, true);

        MouseClick += (_, e) =>
        {
            _JoinClicked(e.Location);
        };

    }

    /// <summary>
    /// Eine Verknüpfung hinzufügen
    /// </summary>
    /// <param name="join">Verbindung, die hinzugefügt werden soll</param>
    public void AddJoin(IJoin join)
    {
        if (!Elements.ContainsKey(join.ElementSource)) throw new ArgumentException($@"No element with ElementSource Id {join.ElementSource}.");

        if (!Elements.ContainsKey(join.ElementTarget)) throw new ArgumentException($@"No element with ElementTarget Id {join.ElementTarget}.");

        if (join.Exist(_joins)) throw new Exception(@"Join always exist.");

        _joins.Add(join);

        Designer?.RaiseNeedRecreate();

        if (join is BaseBuffered bufferedJoin)
        {
            bufferedJoin.PropertyChanged += (_, _) =>
            {
                Designer?.RaiseNeedRecreate();
                Designer?.Refresh();
            };
        }

        _currentJoin = join;

        Refresh();

        OnJoinSelected?.Invoke(this, join);
    }

    /// <summary>
    /// Ereignis: ein Join wurde ausgewählt
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Action<AFDesignerCanvas, IJoin>? OnJoinSelected { get; set; }

    /// <summary>
    /// Designer, der den Canvas darstellt.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFDesigner? Designer { get; set; }

    /// <summary>
    /// Ein Element hinzufügen
    /// </summary>
    /// <param name="element">einzufügendes Element</param>
    /// <param name="point">Position, an der eingefügt werden soll</param>
    /// <exception cref="ArgumentException"></exception>
    public void AddElement(AFDesignerCanvasElement element, Point? point = null)
    {
        if (element.Id.Equals(Guid.Empty)) throw new ArgumentException($@"Element Id cannot be empty.");

        if (Elements.ContainsKey(element.Id))
            return;

        element.Canvas = this;

        _elements.Add(element.Id, element);

        Controls.Add(element);

        element.Activate();
    }

    /// <summary>
    /// Zeichnen der Oberfläche
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
        e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
        e.Graphics.CompositingQuality = CompositingQuality.HighQuality;

        foreach (IJoin join in _joins)
            _DrawLines(e.Graphics, join);
    }

    private void _DrawLines(Graphics graphics, IJoin join)
    {
        Color c = CommonSkins.GetSkin(DevExpress.LookAndFeel.UserLookAndFeel.Default).TranslateColor(SystemColors.ControlText);

        Point[]? points = _getPoints(join);

        if (points == null || points.Length < 2) return;

        using Pen pen = new Pen(c, (_currentJoin != null && _currentJoin == join ? 3.0f : 1.0f));

        graphics.DrawWay(points, pen, BackColor, label: join.Label);
    }

    private Point[]? _getPoints(IJoin join)
    {
        AFDesignerCanvasElement? fromElement = Elements[join.ElementSource];
        AFDesignerCanvasElement? toElement = Elements[join.ElementTarget];

        if (fromElement == null || toElement == null) return null;

        Rectangle rectFromElement = fromElement.GetElementRectangle();
        Rectangle rectToElement = toElement.GetElementRectangle();

        Point ptFromElement = fromElement.GetJoinPoint(join, false);
        Point ptToElement = toElement.GetJoinPoint(join, true);

        return GraphicsEx.FindWay(ptFromElement, ptToElement, rectFromElement, rectToElement);
    }

    private void _JoinClicked(Point clickpoint)
    {
        foreach (var join in _joins)
        {
            var points = _getPoints(join);

            if (points == null || points.Length < 2) return;

            for (var pos = 0; pos < points.Length - 1; ++pos)
            {
                var start = points[pos];
                var end = points[pos + 1];

                if (!clickpoint.IsPointOnLine(start, end)) continue;

                if (_currentJoin != join)
                {
                    _currentJoin = join;
                    Refresh();
                    OnJoinSelected?.Invoke(this, _currentJoin);
                }

                break;
            }
        }
    }

    /// <summary>
    /// Elemente im Designer
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Dictionary<Guid, AFDesignerCanvasElement> Elements => _elements;

    /// <summary>
    /// Elemente im Designer
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public BindingList<IJoin> Joins => _joins;

    /// <summary>
    /// Element löschen
    /// </summary>
    /// <param name="element"></param>
    public void Remove(AFDesignerCanvasElement element)
    {
        SuspendLayout();

        var toRemove = Joins.Where(j => j.ElementSource == element.Id || j.ElementTarget == element.Id).ToArray();

        foreach (var join in toRemove)
            Joins.Remove(join);

        if (ActiveElement == element)
            ActiveElement = null;

        if (Elements.ContainsKey(element.Id))
            Elements.Remove(element.Id);

        Controls.Remove(element);

        ResumeLayout();

        Refresh();
    }

    /// <summary>
    /// Join löschen
    /// </summary>
    /// <param name="join"></param>
    public void Remove(IJoin join)
    {
        if (ActiveJoin == join)
            _currentJoin = null;

        if (Joins.Contains(join))
            Joins.Remove(join);

        Refresh();
    }


    /// <summary>
    /// Ausgewählter Join
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IJoin? ActiveJoin
    {
        get => _currentJoin; 
        set => _currentJoin = value;
    }

    /// <summary>
    /// das aktive Element im Designer (Element mit Focus)
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFDesignerCanvasElement? ActiveElement
    {
        get => _activeElement;
        set
        {
            if (_activeElement != null && _activeElement == value)
                return;

            _activeElement?.Deactivate();

            _activeElement = value;
        }
    }
}

/// <summary>
/// Element, dass auf einem AFDesignerCanvas dargestellt werden kann.
///
/// Das Element kann mit der Maus verschoben werden. Zwischen den Elementen
/// auf einem Canvas können Verbindungslinien angezeigt werden.
/// </summary>
[ToolboxItem(false)]
public class AFDesignerCanvasElement : AFPanel
{
    /// <summary>
    /// Cosntrcutor
    /// </summary>
    public AFDesignerCanvasElement()
    {
        SetStyle(ControlStyles.UserMouse, true);
    }

    /// <summary>
    /// Änderung der Größe erlauben
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AllowResize { get; set; } = true;

    /// <summary>
    /// Element ID
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Guid Id { get; set; } = Guid.Empty;

    /// <summary>
    /// Canvas, auf dem, das Element dargestellt wird.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFDesignerCanvas? Canvas { get; set; }

    /// <summary>
    /// Element aktivieren (Fokus auf Element setzen)
    /// </summary>
    public virtual void Activate()
    {
        BringToFront();

        if (Canvas != null) Canvas.ActiveElement = this;

    }

    /// <summary>
    /// Element deaktivieren (Element hat Fokus verloren)
    /// </summary>
    public virtual void Deactivate()
    {

    }

    /// <summary>
    /// Liefert das Rechteck, dass das Element repräsentiert für die Verbindungslinien.
    ///
    /// Kann ggf. überschrieben werden.
    /// </summary>
    /// <returns></returns>
    public virtual Rectangle GetElementRectangle()
    {
        return new Rectangle(Location.X, Location.Y, Width, Height);
    }

    /// <summary>
    /// Liefert den Punkt, an dem die Linie (Join) anknüpfen soll.
    ///
    /// Kann ggf. überschrieben werden. Standard ist Mitte-Unten
    /// wenn target = false und Mitte-Oben wenn target = true.
    /// </summary>
    /// <param name="join">Join, für den der Punkt benötigt wird</param>
    /// <param name="target">Gesuchter Punt ist Ziel (true) oder Quelle (false)</param>
    /// <returns>Anknüpfungspunkt auf dem Canvas</returns>
    public virtual Point GetJoinPoint(IJoin join, bool target)
    {
        return new Point(Left + (Width / 2), target ? Top : Bottom);
    }

    /// <inheritdoc />
    protected override void OnLocationChanged(EventArgs e)
    {
        base.OnLocationChanged(e);

        Canvas?.Refresh();
    }

    /// <inheritdoc />
    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);

        Canvas?.Refresh();
    }
}