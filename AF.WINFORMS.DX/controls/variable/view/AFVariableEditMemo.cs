using DevExpress.Utils.Layout;
// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace AF.WINFORMS.DX;

/// <summary>
/// Editor für eine Memo Variable
/// </summary>
[ToolboxItem(false)]
[DesignerCategory("Code")]
public class AFVariableEditMemo : AFVariableEditBase
{
    private AFEditSpinInt MaxLength;
    private AFEditSpinInt MinLength;
    private AFEditSingleline CharsAllowed;
    private AFEditSingleline CharsNotAllowed;
    private AFEditMultiline Default;

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFVariableEditMemo()
    {
        MaxLength = new() { Name = nameof(MaxLength) };
        MinLength = new() { Name = nameof(MinLength) };
        CharsAllowed = new() { Name = nameof(CharsAllowed) };
        CharsNotAllowed = new() { Name = nameof(CharsNotAllowed) };
        Default = new() { Name = nameof(Default) };

        MaxLength.Properties.MinValue = 0;
        MinLength.Properties.MinValue = 0;
        MaxLength.Properties.MaxValue = 9999;
        MinLength.Properties.MaxValue = 9999;

        Margin = new(0);
        Padding = new(0);

        AFTablePanel table = new AFTablePanel() { Dock = DockStyle.Top, AutoSize = true, UseSkinIndents = false };
        Controls.Add(table);

        table.BeginLayout();
        var titel = table.Add<AFLabelCaptionSmall>(1, 1, colspan: 4);
        titel.Text = "Eigenschaften: <b>mehrzeiliger Text</b>";
        titel.Margin = new(0, 3, 0, 3);

        table.Add<AFLabel>(2, 1).Indent(6).Text = "Vorgabe";
        table.Add(Default, 2, 2, colspan: 3, rowspan: 2).Height(76);
        table.Add<AFLabel>(4, 1).Indent(6).Text = "Regeln";
        table.Add<AFLabel>(4, 2).Text = "nur folgende Zeichen <b>erlauben</b>";
        table.Add(CharsAllowed, 4, 3, colspan: 2);
        table.Add<AFLabel>(5, 2).Text = "folgende Zeichen <b>nicht erlauben</b>";
        table.Add(CharsNotAllowed, 5, 3, colspan: 2);
        table.Add<AFLabel>(6, 2).Text = "minimale Länge";
        table.Add(MinLength, 6, 3);
        table.Add<AFLabel>(7, 2).Text = "maximale Länge";
        table.Add(MaxLength, 7, 3);

        table.SetColumn(4, TablePanelEntityStyle.Relative, 1.0f);
        table.SetRow(3, TablePanelEntityStyle.Absolute, 50.0f);
        table.SetRow(8, TablePanelEntityStyle.Relative, 1.0f);

        table.EndLayout();

        DefaultEditorWidth = 500;
        DefaultEditorHeight = 250;
    }

    /// <summary>
    /// Variable
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public VariableMemo? Variable { get; set; }
}

