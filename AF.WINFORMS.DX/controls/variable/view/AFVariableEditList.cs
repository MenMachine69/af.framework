// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

using AF.MVC;
using DevExpress.Utils;
using DevExpress.Utils.Layout;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;

namespace AF.WINFORMS.DX;

/// <summary>
/// Editor für eine List Variable
/// </summary>
[ToolboxItem(false)]
[DesignerCategory("Code")]
public class AFVariableEditList : AFVariableEditBase
{
    private AFGridControl grid;

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFVariableEditList()
    {
        AFTablePanel table = new AFTablePanel() { Dock = DockStyle.Fill, AutoSize = true, UseSkinIndents = false };
        Controls.Add(table);

        table.BeginLayout();
        var titel = table.Add<AFLabelCaptionSmall>(1, 1);
        titel.Text = "Eigenschaften: <b>Auswahlliste</b>";
        titel.Margin = new(0, 3, 0, 3);
        var desc = table.Add<AFLabel>(2, 1);
        desc.Text = "Variablen/Eigenschaften die sich auf eine Datenquelle beziehen, liefern den vom Benutzer ausgewählten Wert für die Variable/Eigenschaft. Die Anzeige bei der Auswahl besteht aus einem Namen und (optional) der Anzeige des Wertes.";
        desc.Appearance.TextOptions.WordWrap = WordWrap.Wrap;
        desc.Appearance.Options.UseTextOptions = true;
        desc.AutoSizeMode = LabelAutoSizeMode.Vertical;
        desc.Padding = new(6);
        
        grid = new() { Dock = DockStyle.Fill };
        table.Add(grid, 3, 1);

        table.SetColumn(1, TablePanelEntityStyle.Relative, 1.0f);
        table.SetRow(3, TablePanelEntityStyle.Relative, 1.0f);

        table.EndLayout();


        AFGridSetup setup = new()
        {
            AllowAddNew = true,
            AllowEdit = true,
            CmdDelete = VariableBaseControllerUI.Instance.GetCommand(nameof(VariableBaseControllerUI.CmdDeleteListEntry))
        };
        setup.Columns =
        [
            new AFGridColumn() { Caption = "Anzeigetext", ColumnFieldname = nameof(VariableListEntry.DisplayName) },
            new AFGridColumn() { Caption = "Wert", ColumnFieldname = nameof(VariableListEntry.Value) },
            new AFGridColumn() { Caption = "Ist Vorgabe", ColumnFieldname = nameof(VariableListEntry.IsDefault) },
        ];
        ((GridView)grid.DefaultView).Setup(setup);
        grid.ForceInitialize();
        ((GridView)grid.DefaultView).Columns[nameof(VariableListEntry.IsDefault)].ColumnEdit = new RepositoryItemCheckEdit();
        (((GridView)grid.DefaultView).Columns[nameof(VariableListEntry.IsDefault)].ColumnEdit as RepositoryItemCheckEdit)!.CheckedChanged += (_, _) =>
        {
            if (((GridView)grid.DefaultView).GetFocusedRow() is not VariableListEntry entry) return;

            if (entry.IsDefault) return;

            if (grid.DataSource is not BindingList<VariableListEntry> list) return;

            list.RaiseListChangedEvents = false;
            foreach (VariableListEntry listEntry in list)
            {
                if (listEntry == entry)
                    continue;

                listEntry.IsDefault = false;
            }
            list.RaiseListChangedEvents = true;

            grid.RefreshDataSource();
        };


        DefaultEditorWidth = 500;
        DefaultEditorHeight = 150;
    }

    /// <summary>
    /// Variable
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public VariableList? Variable { get; set; }


    /// <inheritdoc />
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override IModel? Model
    {
        get => base.Model;
        set
        {
            base.Model = value;

            if (value is not VariableList listvar) return;

            grid.DataSource = listvar.Entrys;
        }
    }
}

