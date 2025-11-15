using AF.MVC;
using DevExpress.Utils.Layout;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraVerticalGrid;

namespace AF.WINFORMS.DX;

/// <summary>
/// Dialog der als Inspektor ein Model darstellen kann.
/// </summary>
public sealed class FormModelInspector : FormBase, IModelInspektor
{
    private readonly PropertyGridControl propertyModel = null!;
    private readonly PropertyGridControl propertyField = null!;
    private readonly AFBarManager manager = null!;
    private readonly AFBarController barController = null!;

    /// <summary>
    /// Constructor
    /// </summary>
    public FormModelInspector()
    {
        if (UI.DesignMode) return;

        barController = new();
        barController.AutoBackColorInBars = true;

        manager = new();
        manager.Form = this;
        manager.Controller = barController;
        manager.BeginInit();

        Text = "OBJEKT INSPEKTOR";
        Size = new(400, 600);
        StartPosition = FormStartPosition.CenterParent;
        //TopMost = true;

        propertyModel = new()
        {
            Dock = DockStyle.Fill, BorderStyle = BorderStyles.NoBorder
        };

        propertyModel.OptionsFind.Visibility = FindPanelVisibility.Always;
        propertyModel.OptionsView.ShowRootCategories = false;

        propertyField = new()
        {
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyles.NoBorder
        };

        propertyField.OptionsFind.Visibility = FindPanelVisibility.Always;
        propertyField.OptionsView.ShowRootCategories = false;


        AFTablePanel table = new AFTablePanel() { Dock = DockStyle.Fill, AutoSize = true, UseSkinIndents = false };
        Controls.Add(table);

        table.BeginLayout();
        table.Add<AFLabel>(1, 1, colspan: 2).Text = "Wenn Sie an Ihrem PC eine Kamera angeschlossen haben, können Sie hier ein Bild mit dieser Kamera aufnehmen.";
        var tbar = table.AddBar(manager, 2, 1, colspan: 2);
        var btn = tbar.AddButton("btnSave", img: UI.GetImage(Symbol.Settings)); 
        btn.SuperTip = UI.GetSuperTip("Speichern", "Einstellungen für die Kamera anzeigen.");
        btn.ItemClick += (_, e) =>
        {
            if (e.Item.Name == "btnSave")
                save();
        };

        table.Add(propertyModel, 3, 1);
        table.Add(propertyField, 3, 2);

        table.SetColumn(1, TablePanelEntityStyle.Relative, 0.5f);
        table.SetColumn(2, TablePanelEntityStyle.Relative, 0.5f);
        table.SetRow(2, TablePanelEntityStyle.Absolute, 32.0f);
        table.SetRow(3, TablePanelEntityStyle.Relative, 1.0f);

        table.EndLayout();

        manager.EndInit();
    }

    /// <summary>
    /// Das im Inspektor anzuzeigende/angezeigte Model
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IModel? CurrentModel { get => propertyModel.SelectedObject as IModel; set => propertyModel.SelectedObject = value; }

    /// <summary>
    /// den Inspektor anzeigen
    /// </summary>
    /// <param name="owner"></param>
    public void ShowInspektor(object? owner)
    {
        if (owner is IWin32Window window)
            ShowDialog(window);
        else if (owner is Control ctrl)
            ShowDialog(ctrl.FindForm());
        else
            ShowDialog();
    }

    /// <inheritdoc />
    protected override void OnHandleDestroyed(EventArgs e)
    {
        base.OnHandleDestroyed(e);

        manager.Dispose();
        barController.Dispose();
    }

    private void save()
    {
        ValidationErrorCollection errors = [];

        propertyModel.ClearRowErrors();

        if (CurrentModel!.IsValid(errors) == false)
        {
            foreach (var error in errors)
            {
                var row = propertyModel.GetRowByFieldName(error.PropertyName);
                if (row != null)
                    propertyModel.SetRowError(row.Properties, error.Message, ErrorType.Critical);
            }
        }
    }
}