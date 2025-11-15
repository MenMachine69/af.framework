using DevExpress.Utils.Layout;

namespace AF.WINFORMS.DX;

/// <summary>
/// Editor für eine Month Variable
/// </summary>
[ToolboxItem(false)]
[DesignerCategory("Code")]
public class AFVariableEditSection : AFVariableEditBase
{
    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFVariableEditSection()
    {
        Margin = new(0);
        Padding = new(0);

        AFTablePanel table = new AFTablePanel() { Dock = DockStyle.Top, AutoSize = true, UseSkinIndents = false };
        Controls.Add(table);

        table.BeginLayout();
        var titel = table.Add<AFLabelCaptionSmall>(1, 1, colspan: 4);
        titel.Text = "Eigenschaften: <b>Abschnitt</b>";
        titel.Margin = new(0, 3, 0, 3);

        table.Add<AFLabel>(2, 1, colspan: 4).Indent(6).Text = "Keine weiteren Einstellungen notwendig.";

        table.SetColumn(2, TablePanelEntityStyle.Relative, 1.0f);

        table.EndLayout();

        DefaultEditorWidth = 500;
        DefaultEditorHeight = 60;
    }

    /// <summary>
    /// Variable
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public VariableSection? Variable { get; set; }
}