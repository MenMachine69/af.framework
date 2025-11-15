using DevExpress.Utils.Layout;
// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace AF.WINFORMS.DX;

/// <summary>
/// Editor für eine boolsche Variable
/// </summary>
[ToolboxItem(false)]
[DesignerCategory("Code")]
public class AFVariableEditBool : AFVariableEditBase
{
    private AFEditSingleline DisplayStringOn;
    private AFEditSingleline DisplayStringOff;
    private AFEditSingleline OutputStringOn;
    private AFEditSingleline OutputStringOff;
    private AFEditToggle Default;

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFVariableEditBool()
    {
        DisplayStringOn = new() { Name = nameof(DisplayStringOn) };
        DisplayStringOff = new() { Name = nameof(DisplayStringOff) };
        OutputStringOn = new() { Name = nameof(OutputStringOn) };
        OutputStringOff = new() { Name = nameof(OutputStringOff) };
        Default = new() { Name = nameof(Default) };

        Margin = new(0);
        Padding = new(0);

        AFTablePanel table = new AFTablePanel() { Dock = DockStyle.Top, AutoSize = true, UseSkinIndents = false };
        Controls.Add(table);

        table.BeginLayout();
        var titel = table.Add<AFLabelCaptionSmall>(1, 1, colspan: 4);
        titel.Text = "Eigenschaften des <b>Ja/Nein</b> Wertes";
        titel.Margin = new(0, 3, 0, 3);

        table.Add<AFLabel>(2, 1).Indent(6).Text = "Vorgabe";
        table.Add(Default, 2, 2);
        table.Add<AFLabel>(3, 2).Text = "anzuzeigender Wert";
        table.Add<AFLabel>(3, 3).Text = "auszugebender Wert";
        table.Add<AFLabel>(4, 1).Indent(6).Text = "ausgewählt/ein";
        table.Add(DisplayStringOn, 4, 2);
        table.Add(OutputStringOn, 4, 3);
        table.Add<AFLabel>(5, 1).Indent(6).Text = "nicht ausgewählt/aus";
        table.Add(DisplayStringOff, 5, 2);
        table.Add(OutputStringOff, 5, 3);

        table.SetColumn(2, TablePanelEntityStyle.Relative, 0.50f);
        table.SetColumn(3, TablePanelEntityStyle.Relative, 0.50f);

        table.EndLayout();

        DefaultEditorWidth = 500;
        DefaultEditorHeight = 150;
    }

    /// <summary>
    /// Editor für den Default-Wert
    /// </summary>
    public AFEditToggle DefaultEditor => Default;

    /// <summary>
    /// Variable
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public VariableBool? Variable { get; set; } 
}

