using DevExpress.Utils.Layout;
// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace AF.WINFORMS.DX;

/// <summary>
/// Editor für eine Variable
/// </summary>
[ToolboxItem(false)]
[SupportedOSPlatform("windows")]
[DesignerCategory("Code")]
public class AFVariableEdit : AFEditor
{
    private readonly AFEditSingleline VAR_NAME = null!;
    private readonly AFEditSingleline VAR_CAPTION = null!;
    private readonly AFEditMultiline VAR_DESCRIPTION = null!;
    private readonly AFEditSingleline VAR_SECCAPTION = null!;
    private readonly AFEditMultiline VAR_SECDESCRIPTION = null!;
    private readonly AFEditCombo VAR_TYP = null!;
    private readonly AFEditSpinInt VAR_PRIORITY = null!;
    private readonly AFEditCheck VAR_TABBED = null!;
    private readonly AFEditCheck VAR_TWOCOLUMN = null!;
    private readonly AFEditSpinInt VAR_COLUMN = null!;

    private readonly AFPanel PANEL_DETAIL = null!;



    /// <summary>
    /// Constructor
    /// </summary>
    public AFVariableEdit()
    {
        if (UI.DesignMode) return;

        PANEL_DETAIL = new () { Name = nameof(PANEL_DETAIL), Dock = DockStyle.Fill, Margin = new(0) };
        VAR_NAME = new() { Name = nameof(VAR_NAME) };
        VAR_DESCRIPTION = new() { Name = nameof(VAR_DESCRIPTION) };
        VAR_CAPTION = new() { Name = nameof(VAR_CAPTION) };
        VAR_SECCAPTION = new() { Name = nameof(VAR_SECCAPTION) };
        VAR_SECDESCRIPTION = new() { Name = nameof(VAR_SECDESCRIPTION) };
        VAR_TYP = new() { Name = nameof(VAR_TYP) };
        VAR_PRIORITY = new() { Name = nameof(VAR_PRIORITY) };
        VAR_COLUMN = new() { Name = nameof(VAR_COLUMN) };
        VAR_TABBED = new() { Name = nameof(VAR_TABBED) };
        VAR_TWOCOLUMN = new() { Name = nameof(VAR_TWOCOLUMN) };

        VAR_TYP.SetEnumeration(typeof(eVariableType), valueAsInt: true);

        List<ListItem> customTypes = [];

        foreach (var type in (typeof(VariableBase).GetController() as VariableBaseController)!.CustomTypes)
        {
            customTypes.Add(new ListItem() { Caption = type.Value.VariableTypeName, Value = type.Key});
        }
        if (customTypes.Count > 0)
            VAR_TYP.Fill(customTypes, addValues: true);

        VAR_TWOCOLUMN.Text = "2-spaltig anzeigen";
        VAR_TABBED.Text = "in TAB anzeigen";

        VAR_TYP.SelectedValueChanged += (_, e) =>
        {
            setEditor();
        };

        AFTablePanel table = new AFTablePanel() { Dock = DockStyle.Top, AutoSize = true, UseSkinIndents = true };
        Controls.Add(table);

        table.BeginLayout();

        table.Add<AFLabel>(2, 1).Text = "Typ";
        table.Add(VAR_TYP, 2, 2, colspan: 4);
        
        table.Add<AFLabel>(3, 1).Text = "Name";
        table.Add(VAR_NAME, 3, 2, colspan: 4);

        table.Add<AFLabel>(4, 1).Text = "Anzeigename";
        table.Add(VAR_CAPTION, 4, 2, colspan: 4);

        table.Add<AFLabel>(5, 1).Text = "Beschreibung";
        table.Add(VAR_DESCRIPTION, 5, 2, rowspan: 2, colspan: 4).Height(46);

        table.Add<AFLabelCaptionSmall>(6, 1).Text = "Abschnitt";
        table.Add<AFLabel>(7, 1).Text = "Überschrift";
        table.Add(VAR_SECCAPTION, 7, 2, colspan: 4);
        table.Add<AFLabel>(8, 1).Text = "Beschreibung";
        table.Add(VAR_SECDESCRIPTION, 8, 2, rowspan: 2, colspan: 4).Height(46);

        table.Add<AFLabelBoldText>(1, 6).Text = "Eigenschaften";
        table.Add<AFLabel>(2, 6).Text = "Priorität";
        table.Add(VAR_PRIORITY, 2, 7);
        table.Add<AFLabel>(3, 6).Text = "Spalte";
        table.Add(VAR_COLUMN, 3, 7); table.Add(VAR_TABBED, 2, 6);
        table.Add(VAR_TWOCOLUMN, 4, 6, colspan: 2);
        table.Add(VAR_COLUMN, 5, 6, colspan: 2);

        table.SetColumn(5, TablePanelEntityStyle.Relative, 1.0f);

        table.EndLayout();

        Controls.Add(PANEL_DETAIL);
        PANEL_DETAIL.BringToFront();

        DefaultEditorWidth = 600;
        DefaultEditorHeight = 200;
    }

    private void setEditor()
    {
        if (VAR_TYP.SelectedValue is not int type) return;

        var neededEditor = VariableBaseControllerUI.Instance.GetEditorType(type);
        
        if (PANEL_DETAIL.Controls.Count > 0)
        {
            if (PANEL_DETAIL.Controls[0].GetType() != neededEditor)
                PANEL_DETAIL.Controls.Clear(true);
        }

        if (PANEL_DETAIL.Controls.Count < 1)
        {
            PANEL_DETAIL.Controls.Add(ControlsEx.CreateInstance(neededEditor)!);
            PANEL_DETAIL.Controls[0].Dock = DockStyle.Fill;
        }

        ((AFVariableEditBase)PANEL_DETAIL.Controls[0]).Model = Variable?.VAR_VARIABLE;
    }

    /// <summary>
    /// Zugriff auf die Variable
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Variable? Variable
    {
        get => (Variable?)Model;
        set
        {
            if (value == null) return;

            Model = value;
        }
    }
}

