using DevExpress.Utils;
using DevExpress.Utils.Layout;
using DevExpress.XtraEditors;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace AF.WINFORMS.DX;

/// <summary>
/// Editor für eine Query Variable
/// </summary>
[ToolboxItem(false)]
[DesignerCategory("Code")]
public class AFVariableEditQuery : AFVariableEditBase
{
    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFVariableEditQuery()
    {
        Control QueryId = ControlsEx.CreateInstance(AFCore.App.QueryService!.GetQuerySelectControlType());
        QueryId.Name = nameof(VariableQuery.QueryId);

        if (QueryId is SearchLookUpEdit dsselect)
        {
            dsselect.EditValueChanged += (_, _) =>
            {
                var source = ((AFComboboxLookup)QueryId).EditValue;
            };
        }

        Margin = new(0);
        Padding = new(0);

        AFTablePanel table = new AFTablePanel() { Dock = DockStyle.Fill, AutoSize = true, UseSkinIndents = false };
        Controls.Add(table);

        table.BeginLayout();
        var titel = table.Add<AFLabelCaptionSmall>(1, 1, colspan: 3);
        titel.Text = "Eigenschaften: <b>Datenquelle</b>";
        titel.Margin = new(0, 3, 0, 3);
        var desc = table.Add<AFLabel>(2, 1, colspan: 3);
        desc.Text = "Variablen/Eigenschaften die sich auf eine Datenquelle beziehen, liefern den vom Benutzer ausgewählten Wert für die Variable/Eigenschaft. Die Anzeige bei der Auswahl besteht aus einem Namen und (optional) der Anzeige des Wertes.";
        desc.Appearance.TextOptions.WordWrap = WordWrap.Wrap;
        desc.Appearance.Options.UseTextOptions = true;
        desc.AutoSizeMode = LabelAutoSizeMode.Vertical;
        desc.Padding = new(6);

        table.Add<AFLabel>(3, 1).Indent(6).Text("Datenquelle");
        table.Add(QueryId, 3, 2, colspan: 2);

        table.Add<AFLabel>(4, 1).Indent(6).Text("Spalte Anzeige");
        table.Add<AFEditCombo>(4, 2, colspan: 2).Name(nameof(VariableQuery.DisplayValueColumn));

        table.Add<AFLabel>(5, 1).Indent(6).Text("Spalte Wert");
        table.Add<AFEditCombo>(5, 2).Name(nameof(VariableQuery.ValueColumn));
        table.Add<AFEditCheck>(5, 3).Name(nameof(VariableQuery.DisplayValueColumn)).Text("Spalte anzeigen");

        AFGridControl grid = new() { Dock = DockStyle.Fill };
        table.Add(grid, 6, 1, colspan: 3);

        table.SetColumn(2, TablePanelEntityStyle.Relative, 1.0f);
        table.SetRow(6, TablePanelEntityStyle.Relative, 1.0f);

        table.EndLayout();

        DefaultEditorWidth = 500;
        DefaultEditorHeight = 150;
    }

    /// <summary>
    /// Variable
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public VariableQuery? Variable { get; set; }
}

