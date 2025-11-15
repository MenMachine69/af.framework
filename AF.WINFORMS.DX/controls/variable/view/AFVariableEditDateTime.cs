using DevExpress.Utils.Layout;
// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace AF.WINFORMS.DX;

/// <summary>
/// Editor für eine DateTime Variable
/// </summary>
[ToolboxItem(false)]
[DesignerCategory("Code")]
public class AFVariableEditDateTime : AFVariableEditBase
{
    private AFEditCombo VariableType;
    private AFEditDate Minimum;
    private AFEditDate Maximum;
    private AFEditDate Default;
    private AFEditSingleline DisplayMask;
    private AFEditCombo RelativeMinimum;
    private AFEditCombo RelativeMaximum;
    private AFEditCombo RelativeDefault;

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFVariableEditDateTime()
    {
        Maximum = new() { Name = nameof(Maximum) };
        Minimum = new() { Name = nameof(Minimum) };
        DisplayMask = new() { Name = nameof(DisplayMask) };
        Default = new() { Name = nameof(Default) };
        VariableType = new() { Name = nameof(VariableType) };
        RelativeMinimum = new() { Name = nameof(RelativeMinimum) };
        RelativeMaximum = new() { Name = nameof(RelativeMaximum) };
        RelativeDefault = new() { Name = nameof(RelativeDefault) };

        RelativeMinimum.SetEnumeration(typeof(eRelativeDateTime));
        RelativeMaximum.SetEnumeration(typeof(eRelativeDateTime));
        RelativeDefault.SetEnumeration(typeof(eRelativeDateTime));
        VariableType.SetEnumeration(typeof(eDateTimeVariableType));

        Maximum.Size = new(140, 26);
        Minimum.Size = new(140, 26);
        Default.Size = new(140, 26);

        RelativeMinimum.SelectedValueChanged += (_, _) =>
        {
            Minimum.Enabled = (RelativeMinimum.SelectedValue is eRelativeDateTime type && type == eRelativeDateTime.Fixed);
        };

        RelativeMaximum.SelectedValueChanged += (_, _) =>
        {
            Maximum.Enabled = (RelativeMaximum.SelectedValue is eRelativeDateTime type && type == eRelativeDateTime.Fixed);
        };

        RelativeDefault.SelectedValueChanged += (_, _) =>
        {
            Default.Enabled = (RelativeDefault.SelectedValue is eRelativeDateTime type && type == eRelativeDateTime.Fixed);
        };

        VariableType.SelectedValueChanged += (_, _) =>
        {
            if (VariableType.SelectedValue is not eDateTimeVariableType type) return;

            switch (type)
            {
                case eDateTimeVariableType.Date:
                    Maximum.Properties.DisplayFormat.FormatString = "d";
                    Minimum.Properties.DisplayFormat.FormatString = "d";
                    Default.Properties.DisplayFormat.FormatString = "d";
                    Maximum.Properties.EditFormat.FormatString = "d";
                    Minimum.Properties.EditFormat.FormatString = "d";
                    Default.Properties.EditFormat.FormatString = "d";

                    Maximum.Properties.MaskSettings.MaskExpression = "d";
                    Minimum.Properties.MaskSettings.MaskExpression = "d";
                    Default.Properties.MaskSettings.MaskExpression = "d";
                    Maximum.Properties.CalendarTimeEditing = DevExpress.Utils.DefaultBoolean.False;
                    Minimum.Properties.CalendarTimeEditing = DevExpress.Utils.DefaultBoolean.False;
                    Default.Properties.CalendarTimeEditing = DevExpress.Utils.DefaultBoolean.False;
                    Maximum.Properties.CalendarDateEditing = true;
                    Minimum.Properties.CalendarDateEditing = true;
                    Default.Properties.CalendarDateEditing = true;
                    break;
                case eDateTimeVariableType.DateAndTime:
                    Maximum.Properties.DisplayFormat.FormatString = "g";
                    Minimum.Properties.DisplayFormat.FormatString = "g";
                    Default.Properties.DisplayFormat.FormatString = "g";
                    Maximum.Properties.EditFormat.FormatString = "g";
                    Minimum.Properties.EditFormat.FormatString = "g";
                    Default.Properties.EditFormat.FormatString = "g";
                    Maximum.Properties.MaskSettings.MaskExpression = "g";
                    Minimum.Properties.MaskSettings.MaskExpression = "g";
                    Default.Properties.MaskSettings.MaskExpression = "g";
                    Maximum.Properties.CalendarTimeEditing = DevExpress.Utils.DefaultBoolean.True;
                    Minimum.Properties.CalendarTimeEditing = DevExpress.Utils.DefaultBoolean.True;
                    Default.Properties.CalendarTimeEditing = DevExpress.Utils.DefaultBoolean.True;
                    Maximum.Properties.CalendarDateEditing = true;
                    Minimum.Properties.CalendarDateEditing = true;
                    Default.Properties.CalendarDateEditing = true;
                    break;
                case eDateTimeVariableType.Time:
                    Maximum.Properties.DisplayFormat.FormatString = "t";
                    Minimum.Properties.DisplayFormat.FormatString = "t";
                    Default.Properties.DisplayFormat.FormatString = "t";
                    Maximum.Properties.EditFormat.FormatString = "t";
                    Minimum.Properties.EditFormat.FormatString = "t";
                    Default.Properties.EditFormat.FormatString = "t";
                    Maximum.Properties.MaskSettings.MaskExpression = "t";
                    Minimum.Properties.MaskSettings.MaskExpression = "t";
                    Default.Properties.MaskSettings.MaskExpression = "t";
                    Maximum.Properties.CalendarTimeEditing = DevExpress.Utils.DefaultBoolean.True;
                    Minimum.Properties.CalendarTimeEditing = DevExpress.Utils.DefaultBoolean.True;
                    Default.Properties.CalendarTimeEditing = DevExpress.Utils.DefaultBoolean.True;
                    Maximum.Properties.CalendarDateEditing = false;
                    Minimum.Properties.CalendarDateEditing = false;
                    Default.Properties.CalendarDateEditing = false;
                    break;
            }
        };

        Maximum.Properties.MinDate = DateTime.MinValue;
        Minimum.Properties.MinDate = DateTime.MinValue;
        Default.Properties.MinDate = DateTime.MinValue;
        Maximum.Properties.MaxDate = DateTime.MaxValue;
        Minimum.Properties.MaxDate = DateTime.MaxValue;
        Default.Properties.MaxDate = DateTime.MaxValue;

        Margin = new(0);
        Padding = new(0);

        AFTablePanel table = new AFTablePanel() { Dock = DockStyle.Top, AutoSize = true, UseSkinIndents = false };
        Controls.Add(table);

        table.BeginLayout();
        var titel = table.Add<AFLabelCaptionSmall>(1, 1, colspan: 4);
        titel.Text = "Eigenschaften: <b>Datum/Zeit</b>";
        titel.Margin = new(0, 3, 0, 3);

        table.Add<AFLabel>(2, 1).Indent(6).Text = "Typ";
        table.Add(VariableType, 2, 2, colspan: 2);
        table.Add<AFLabel>(3, 1).Indent(6).Text = "Vorgabe";
        table.Add(RelativeDefault, 3, 2);
        table.Add(Default, 3, 3);
        table.Add<AFLabel>(4, 1).Indent(6).Text = "Minimum";
        table.Add(RelativeMinimum, 4, 2);
        table.Add(Minimum, 4, 3);
        table.Add<AFLabel>(5, 1).Indent(6).Text = "Maximum";
        table.Add(RelativeMaximum, 5, 2);
        table.Add(Maximum, 5, 3);

        table.SetColumn(2, TablePanelEntityStyle.Relative, 1.0f);

        table.EndLayout();

        DefaultEditorWidth = 500;
        DefaultEditorHeight = 170;
    }

    /// <summary>
    /// Variable
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public VariableDateTime? Variable { get; set; }
}