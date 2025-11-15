using AF.MVC;
using DevExpress.Utils;
using DevExpress.Utils.Layout;
using DevExpress.XtraEditors;
// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace AF.WINFORMS.DX;

/// <summary>
/// Editor für eine Model Variable
/// </summary>
[ToolboxItem(false)]
[DesignerCategory("Code")]
public class AFVariableEditModel : AFVariableEditBase
{
    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFVariableEditModel()
    {
        AFComboboxModelType ModelType = new() { Name = nameof(ModelType), OnlyAllowSelect = true };
        AFEditFilterExpression Filter = new(typeof(Nullable)) { Name = nameof(Filter), Dock = DockStyle.Fill };
        AFLabel FilterHint = new() { Name = nameof(FilterHint) };

        Margin = new(0);
        Padding = new(0);

        AFTablePanel table = new AFTablePanel() { Dock = DockStyle.Fill, AutoSize = true, UseSkinIndents = false };
        Controls.Add(table);

        ModelType.LoadTypes(false, false);
        ModelType.SelectedIndexChanged += (_, _) =>
        {
            Filter.FilterString = "";
            Filter.ModelType = ModelType.SelectedModelType?.GetController()?.GetSelectionModelType() ?? ModelType.SelectedModelType;
            Filter.Enabled = ModelType.SelectedModelType?.GetController()?.SupportSelectionFilter ?? true;

            if (Filter.Enabled == false)
            {
                FilterHint.Text = $"Der Typ <b>{Filter.ModelType!.FullName}</b> unterstützt/benötigt keine Filter für die Auswahl.";
                table.Rows[3].Visible = true;
            }
            else
                table.Rows[3].Visible = false;
        };

        table.BeginLayout();
        var titel = table.Add<AFLabelCaptionSmall>(1, 1, colspan: 2);
        titel.Text = "Eigenschaften: <b>Model</b>";
        titel.Margin = new(0, 3, 0, 3);
        var desc = table.Add<AFLabel>(2, 1, colspan: 2);
        desc.Text = "Auswahl aus Datenobjekten der internen Datenbank. Als Wert wird immer der primäre Schlüssel des ausgewählten Datenobjektes gespeichert.";
        desc.Appearance.TextOptions.WordWrap = WordWrap.Wrap;
        desc.Appearance.Options.UseTextOptions = true;
        desc.AutoSizeMode = LabelAutoSizeMode.Vertical;
        desc.Padding = new(6);

        table.Add<AFLabel>(3, 1).Indent(6).Text = "Daten-/Model-Typ";
        table.Add(ModelType, 3, 2);
        table.Add(FilterHint, 4, 2).Indent(6);
        table.Add<AFLabel>(5, 1).Indent(6).Text = "Filterbedingung";
        table.Add(Filter, 5, 2, rowspan: 2);

        table.SetColumn(2, TablePanelEntityStyle.Relative, 1.0f);
        table.SetRow(6, TablePanelEntityStyle.Relative, 1.0f);

        table.EndLayout();

        table.Rows[3].Visible = false;

        DefaultEditorWidth = 500;
        DefaultEditorHeight = 150;
    }

    /// <summary>
    /// Variable
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public VariableModel? Variable { get; set; }
}

