using DevExpress.Utils;
using DevExpress.Utils.Layout;
// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace AF.WINFORMS.DX;

/// <summary>
/// Editor für eine Decimal Variable
/// </summary>
[ToolboxItem(false)]
[DesignerCategory("Code")]
public class AFVariableEditDecimal : AFVariableEditBase
{
    private AFEditSpin Minimum;
    private AFEditSpin Maximum;
    private AFEditSpin Default;
    private AFEditSingleline DisplayMask;

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFVariableEditDecimal()
    {
        Maximum = new() { Name = nameof(Maximum) };
        Minimum = new() { Name = nameof(Minimum) };
        DisplayMask = new() { Name = nameof(DisplayMask) };
        Default = new() { Name = nameof(Default) };

        Minimum.Properties.IsFloatValue = true;
        Maximum.Properties.IsFloatValue = true;
        Default.Properties.IsFloatValue = true;

        DisplayMask.EditValueChanged += (_, _) =>
        {
            Minimum.Properties.DisplayFormat.FormatType = FormatType.Numeric;
            Minimum.Properties.DisplayFormat.FormatString = DisplayMask.Text;
            Minimum.Properties.EditFormat.FormatType = FormatType.Numeric;
            Minimum.Properties.EditFormat.FormatString = DisplayMask.Text;

            Maximum.Properties.DisplayFormat.FormatType = FormatType.Numeric;
            Maximum.Properties.DisplayFormat.FormatString = DisplayMask.Text;
            Maximum.Properties.EditFormat.FormatType = FormatType.Numeric;
            Maximum.Properties.EditFormat.FormatString = DisplayMask.Text;

            Default.Properties.DisplayFormat.FormatType = FormatType.Numeric;
            Default.Properties.DisplayFormat.FormatString = DisplayMask.Text;
            Default.Properties.EditFormat.FormatType = FormatType.Numeric;
            Default.Properties.EditFormat.FormatString = DisplayMask.Text;
        };

        Maximum.Properties.MinValue = -9999999;
        Minimum.Properties.MinValue = -9999999;
        Maximum.Properties.MaxValue = 9999999;
        Minimum.Properties.MaxValue = 9999999;

        Default.Properties.MinValue = -9999999;
        Default.Properties.MaxValue = 9999999;


        Margin = new(0);
        Padding = new(0);

        AFTablePanel table = new AFTablePanel() { Dock = DockStyle.Top, AutoSize = true, UseSkinIndents = false };
        Controls.Add(table);

        table.BeginLayout();
        var titel = table.Add<AFLabelCaptionSmall>(1, 1, colspan: 4);
        titel.Text = "Eigenschaften: <b>Dezimalwert</b>";
        titel.Margin = new(0, 3, 0, 3);

        table.Add<AFLabel>(2, 1).Indent(6).Text = "Vorgabe";
        table.Add(Default, 2, 2);
        table.Add<AFLabel>(3, 1).Indent(6).Text = "Regeln";
        table.Add<AFLabel>(3, 2).Text = "Minimum";
        table.Add(Minimum, 3, 3);
        table.Add<AFLabel>(4, 2).Text = "Maximum";
        table.Add(Maximum, 4, 3);
        table.Add<AFLabel>(5, 2).Text = "Maske";
        table.Add(DisplayMask, 5, 3);

        table.SetColumn(4, TablePanelEntityStyle.Relative, 1.0f);

        table.EndLayout();

        DefaultEditorWidth = 500;
        DefaultEditorHeight = 170;
    }

    /// <summary>
    /// Variable
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public VariableDecimal? Variable { get; set; }
}

