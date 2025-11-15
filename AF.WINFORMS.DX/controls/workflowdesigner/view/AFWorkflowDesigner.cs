using DevExpress.Utils.Layout;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Designer für Workflows
/// </summary>
public partial class AFWorkflowDesigner : AFDesigner
{
    private readonly AFWorkflowDesignerCanvas _canvas = new() { Dock = DockStyle.Fill };

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFWorkflowDesigner()
    {
        InitializeComponent();

        if (UI.DesignMode) return;

        components ??= new Container();
        
        SidePanel panel = new() { Dock = DockStyle.Right, Size = new(400, Height) };
        crPanel1.Controls.Add(panel);
        AFTablePanel table = new() { Dock = DockStyle.Fill, UseSkinIndents = true };
        table.BeginLayout();
        table.Add<AFLabelCaption>(1, 1, colspan: 2).Text = "EIGENSCHAFTEN";
        
        table.SetRow(2, TablePanelEntityStyle.Relative, 1.0f );
        table.EndLayout();
        panel.Controls.Add(table);


        _canvas.WorkflowDesigner = this;
        crPanel1.Controls.Add(_canvas);
        _canvas.BringToFront();
    }

    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (UI.DesignMode) return;

        crDockManager1.ForceInitialize();

        crDockManager1.Panels.ForEach(p =>
        {
            p.Options.AllowFloating = false;
            p.Options.ShowCloseButton = false;
        });

    }

    /// <summary>
    /// eine Tabelle wurde ausgewählt
    /// </summary>
    /// <param name="table"></param>
    public void ElementSelected(AFWorkflowDesignerElement? table)
    {
        CurrentElement = table;
    }

    /// <summary>
    /// Momentan aktive Tabelle
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFWorkflowDesignerElement? CurrentElement { get; set; }

    /// <summary>
    /// Zugriff auf die eigentliche Zeichenfläche.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFWorkflowDesignerCanvas Canvas => _canvas;

    private void simpleButton1_Click(object sender, EventArgs e)
    {
        _canvas.AddFilter(new WorkflowDesignerModelFilter() { NAME = "Filter", DESCRIPTION = "Ein einfacher Filter." }, new Point(100, 100));
    }

    private void simpleButton2_Click(object sender, EventArgs e)
    {
        _canvas.AddAction(new WorkflowDesignerModelAction() { NAME = "Action", DESCRIPTION = "Eine einfache Action." }, new Point(100, 100));
    }
}
