using AF.MVC;
using DevExpress.Utils.Layout;
using DevExpress.XtraGrid.Views.Grid;

namespace AF.WINFORMS.DX;

/// <summary>
/// Form zur Verwaltung von Layouts
/// </summary>
public sealed class FormLayoutManager : FormBase
{
    private readonly AFGridControl grid = null!;
    private readonly GridView view = null!;


    /// <summary>
    /// Constructor
    /// </summary>
    public FormLayoutManager(string description, Guid idelement, string? extName = null)
    {
        if (UI.DesignMode) return;

        Text = "LAYOUTS VERWALTEN";
        Size = new(300, 300);
        StartPosition = FormStartPosition.CenterParent;

        AFTablePanel table = new AFTablePanel { Dock = DockStyle.Fill, AutoSize = true, UseSkinIndents = true };
        Controls.Add(table);

        table.BeginLayout();
        table.Add<AFLabel>(1, 1, colspan: 2).Text = description;
        grid = table.Add<AFGridControl>(2, 1, rowspan: 4);
        view = new(); 
        grid.ViewCollection.Add(view);
        grid.MainView = view;
        
        var setup = typeof(LayoutModel).GetTypeDescription().GetGridSetup(eGridStyle.Full);
        view.Setup(setup);
        view.OptionsView.ShowGroupPanel = false;
        view.OptionsView.ShowColumnHeaders = false;
        
        var btn = table.Add<AFButton>(2, 2);
        btn.Text = "VERWENDEN";
        btn.Click += (_, _) =>
        {
            if (view.FocusedRowObject != null)
                SelectedLayout = ((LayoutModel)view.FocusedRowObject).PrimaryKey;

            if (SelectedLayout != null)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        };

        btn = table.Add<AFButton>(3, 2);
        btn.Text = "LÖSCHEN";
        btn.Click += (_, _) =>
        {
            var selected = view.GetAllSelectedRows();
            if (selected == null || selected.Length < 1) return;

            if (MsgBox.ShowQuestionYesNo($"LAYOUTS LÖSCHEN\r\nMöchten Sie die {selected.Length} ausgewählten Layouts jetzt löschen?") == eMessageBoxResult.No)
                return;

            foreach (var entry in selected)
            {
                AFCore.App.Persistance?.DeleteById(((LayoutModel)entry).PrimaryKey);
                ((BindingList<LayoutModel>)grid.DataSource).Remove((LayoutModel)entry);
            }
        };

        btn = table.Add<AFButton>(4, 2);
        btn.Text = "KOPIEREN";
        btn.Click += (_, _) =>
        {

        };

        table.SetRow(5, TablePanelEntityStyle.Relative, 1.0f);
        table.SetColumn(1, TablePanelEntityStyle.Relative, 1.0f);

        table.EndLayout();
        Controls.Add(table);

        var dict = AFCore.App.Persistance?.GetNamedValues(idelement, extName: extName) ?? [];

        BindingList<LayoutModel> data = [];
        foreach (var entry in dict)
        {
            data.Add(new()
            {
                LAYOUT_NAME = entry.Value,
                PrimaryKey = entry.Key
            });
        }

        grid.DataSource = data;
        // grid.RefreshDataSource();
    }

    /// <summary>
    /// Ausgewähltes Layout für VERWENDEN
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Guid? SelectedLayout { get; set; }
}

/// <summary>
/// Einfaches Model zur Verwaltung der Layouts
/// </summary>
public class LayoutModel : DataObjectBase
{
    /// <summary>
    /// Name des gespeicherten Layouts
    /// </summary>
    [AFBinding]
    [AFContext("Layout", "Layouts", "Name des Layouts.")]
    [AFGridColumn]
    public string LAYOUT_NAME { get; set; } = "";

    /// <summary>
    /// ID des Layouts
    /// </summary>
    public new Guid PrimaryKey { get; set; } = Guid.Empty;
}

