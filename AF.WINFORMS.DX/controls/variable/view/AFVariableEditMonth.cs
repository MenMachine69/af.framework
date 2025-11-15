using DevExpress.Utils.Layout;

namespace AF.WINFORMS.DX;

/// <summary>
/// Editor für eine Month Variable
/// </summary>
[ToolboxItem(false)]
[DesignerCategory("Code")]
public class AFVariableEditMonth : AFVariableEditBase
{
    private AFEditSpinInt Minimum;
    private AFEditSpinInt Maximum;
    private AFEditSpinInt Default;
    private AFEditCombo RelativeMinimum;
    private AFEditCombo RelativeMaximum;
    private AFEditCombo RelativeDefault;

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFVariableEditMonth()
    {
        Maximum = new() { Name = nameof(Maximum) };
        Minimum = new() { Name = nameof(Minimum) };
        Default = new() { Name = nameof(Default) };
        RelativeMinimum = new() { Name = nameof(RelativeMinimum) };
        RelativeMaximum = new() { Name = nameof(RelativeMaximum) };
        RelativeDefault = new() { Name = nameof(RelativeDefault) };

        RelativeMinimum.SetEnumeration(typeof(eRelativeMonth));
        RelativeMaximum.SetEnumeration(typeof(eRelativeMonth));
        RelativeDefault.SetEnumeration(typeof(eRelativeMonth));

        Maximum.Size = new(140, 26);
        Minimum.Size = new(140, 26);
        Default.Size = new(140, 26);

        RelativeMinimum.SelectedValueChanged += (_, _) =>
        {
            Minimum.Enabled = (RelativeMinimum.SelectedValue is eRelativeMonth type && type == eRelativeMonth.Fixed);
        };

        RelativeMaximum.SelectedValueChanged += (_, _) =>
        {
            Maximum.Enabled = (RelativeMaximum.SelectedValue is eRelativeMonth type && type == eRelativeMonth.Fixed);
        };

        RelativeDefault.SelectedValueChanged += (_, _) =>
        {
            Default.Enabled = (RelativeDefault.SelectedValue is eRelativeMonth type && type == eRelativeMonth.Fixed);
        };

        Maximum.Properties.MinValue = 1;
        Minimum.Properties.MinValue = 1;
        Default.Properties.MinValue = 1;
        Maximum.Properties.MaxValue = 12;
        Minimum.Properties.MaxValue = 12;
        Default.Properties.MaxValue = 12;

        Margin = new(0);
        Padding = new(0);

        AFTablePanel table = new AFTablePanel() { Dock = DockStyle.Top, AutoSize = true, UseSkinIndents = false };
        Controls.Add(table);

        table.BeginLayout();
        var titel = table.Add<AFLabelCaptionSmall>(1, 1, colspan: 4);
        titel.Text = "Eigenschaften: <b>Monat</b>";
        titel.Margin = new(0, 3, 0, 3);

        table.Add<AFLabel>(2, 1).Indent(6).Text = "Vorgabe";
        table.Add(RelativeDefault, 2, 2);
        table.Add(Default, 2, 3);
        table.Add<AFLabel>(3, 1).Indent(6).Text = "Minimum";
        table.Add(RelativeMinimum, 3, 2);
        table.Add(Minimum, 3, 3);
        table.Add<AFLabel>(4, 1).Indent(6).Text = "Maximum";
        table.Add(RelativeMaximum, 4, 2);
        table.Add(Maximum, 4, 3);

        table.SetColumn(2, TablePanelEntityStyle.Relative, 1.0f);

        table.EndLayout();

        DefaultEditorWidth = 500;
        DefaultEditorHeight = 140;
    }

    /// <summary>
    /// Variable
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public VariableMonth? Variable { get; set; }
}
