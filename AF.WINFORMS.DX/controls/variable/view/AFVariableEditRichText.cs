using DevExpress.Utils.Layout;
// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace AF.WINFORMS.DX;

/// <summary>
/// Editor für eine RichText Variable
/// </summary>
[ToolboxItem(false)]
[DesignerCategory("Code")]
public class AFVariableEditRichText : AFVariableEditBase
{
    private AFEditSpinInt MaxLength;
    private AFEditSpinInt MinLength;
    private AFRichEditSimple Default;

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFVariableEditRichText()
    {
        MaxLength = new() { Name = nameof(MaxLength) };
        MinLength = new() { Name = nameof(MinLength) };
        Default = new() { Name = nameof(Default) };

        MaxLength.Properties.MinValue = 0;
        MinLength.Properties.MinValue = 0;
        MaxLength.Properties.MaxValue = 9999;
        MinLength.Properties.MaxValue = 9999;

        Margin = new(0);
        Padding = new(0);

        AFTablePanel table = new AFTablePanel() { Dock = DockStyle.Fill, AutoSize = true, UseSkinIndents = false };
        Controls.Add(table);

        table.BeginLayout();
        var titel = table.Add<AFLabelCaptionSmall>(1, 1, colspan: 4);
        titel.Text = "Eigenschaften: <b>formatierter Text</b>";
        titel.Margin = new(0, 3, 0, 3);

        table.Add<AFLabel>(2, 1).Indent(6).Text = "Vorgabe";
        table.Add(Default, 3, 1, colspan: 4, rowspan: 2);
        table.Add<AFLabel>(5, 1).Indent(6).Text = "Regeln";
        table.Add<AFLabel>(5, 2).Text = "minimale Länge";
        table.Add(MinLength, 5, 3);
        table.Add<AFLabel>(6, 2).Text = "maximale Länge";
        table.Add(MaxLength, 6, 3);

        table.SetColumn(4, TablePanelEntityStyle.Relative, 1.0f);
        table.SetRow(4, TablePanelEntityStyle.Relative, 1.0f);

        table.EndLayout();

        DefaultEditorWidth = 500;
        DefaultEditorHeight = 350;
    }

    /// <summary>
    /// Variable
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public VariableMemo? Variable { get; set; }
}

