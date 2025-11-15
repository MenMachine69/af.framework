using DevExpress.Utils.Layout;
using AF.MVC;
using DevExpress.XtraEditors;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace AF.WINFORMS.DX;

/// <summary>
/// Editor für AF.CORE.Variable
/// </summary>
[ToolboxItem(false)]
[SupportedOSPlatform("windows")]
[DesignerCategory("Code")]
public class VariableEditor : AFEditor, IUIElement
{
    private AFEditSpinInt VAR_PRIORITY;
    private AFEditCheck VAR_READONLY;
    private AFEditSingleline VAR_NAME;
    private AFEditSingleline VAR_CAPTION;
    private AFEditMultiline VAR_DESCRIPTION;
    private AFEditSingleline VAR_SECCAPTION;
    private AFEditMultiline VAR_SECDESCRIPTION;
    private AFEditCombo VAR_TYP;
    private AFEditCheck VAR_TABBED;
    private AFEditCheck VAR_TWOCOLUMN;
    private AFEditSpinInt VAR_COLUMN;

    private AFPanel PANEL_DETAIL;

    /// <summary>
    /// Constructor
    /// </summary>
    public VariableEditor() : this(false)
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public VariableEditor(bool fragebogenModus)
    {
        PANEL_DETAIL = new() { Name = nameof(PANEL_DETAIL), Dock = DockStyle.Fill, Margin = new(0), Padding = new(8,0,8,0) };
        VAR_NAME = new() { Name = nameof(VAR_NAME) };
        VAR_DESCRIPTION = new() { Name = nameof(VAR_DESCRIPTION) };
        VAR_CAPTION = new() { Name = nameof(VAR_CAPTION) };
        VAR_SECCAPTION = new() { Name = nameof(VAR_SECCAPTION) };
        VAR_SECDESCRIPTION = new() { Name = nameof(VAR_SECDESCRIPTION) };
        VAR_TYP = new() { Name = nameof(VAR_TYP) };
        VAR_PRIORITY = new() { Name = nameof(VAR_PRIORITY) };
        VAR_COLUMN = new() { Name = nameof(VAR_COLUMN) };
        VAR_TABBED = new() { Name = nameof(VAR_TABBED), Text = "in TAB anzeigen" };
        VAR_TWOCOLUMN = new() { Name = nameof(VAR_TWOCOLUMN), Text = "2-spaltig anzeigen" };
        VAR_READONLY = new() { Name = nameof(VAR_READONLY), Text = "nicht bearbeitbar" };


        VAR_TYP.SetEnumeration(typeof(eVariableType), valueAsInt: true);

        List<ListItem> customTypes = [];

        foreach (var type in (typeof(VariableBase).GetController() as VariableBaseController)!.CustomTypes)
        {
            customTypes.Add(new ListItem() { Caption = type.Value.VariableTypeName, Value = type.Key });
        }
        if (customTypes.Count > 0)
            VAR_TYP.Fill(customTypes, addValues: true);


        VAR_TYP.SelectedValueChanged += (_, e) =>
        {
            setEditor();
        };

        AFSplitContainer splitContainer = new AFSplitContainer() { Dock = DockStyle.Fill };
        splitContainer.IsSplitterFixed = true;
        splitContainer.FixedPanel = SplitFixedPanel.Panel1;
        splitContainer.Horizontal = false;
        splitContainer.SplitterPosition = 250;
        Controls.Add(splitContainer);

        AFTablePanel table = new AFTablePanel() { Dock = DockStyle.Fill, AutoSize = true, UseSkinIndents = true };
        table.BeginLayout();

        table.Add<AFLabel>(1, 1).Text = "Typ";
        table.Add(VAR_TYP, 1, 2);

        table.Add<AFLabel>(2, 1).Text = "Name";
        table.Add(VAR_NAME, 2, 2);

        table.Add<AFLabel>(3, 1).Text = "Anzeigename";
        table.Add(VAR_CAPTION, 3, 2);

        table.Add<AFLabel>(4, 1).Text = "Beschreibung";
        table.Add(VAR_DESCRIPTION, 4, 2, rowspan: 2).Height(46);

        table.Add<AFLabelCaptionSmall>(6, 1, colspan: 2).Text = "Formularabschnitt";
        
        table.Add<AFLabel>(7, 1).Indent(8).Text = "Überschrift";
        table.Add(VAR_SECCAPTION, 7, 2);

        table.Add<AFLabel>(8, 1).Indent(8).Text = "Beschreibung";
        table.Add(VAR_SECDESCRIPTION, 8, 2, rowspan: 2).Height(46);

        table.Add<AFLabelBoldText>(1, 3, colspan: 2).Text = "Eigenschaften";

        table.Add<AFLabel>(2, 3).Indent(8).Text = "Reihenfolge";
        table.Add(VAR_PRIORITY, 2, 4);

        table.Add<AFLabel>(3, 3).Indent(8).Text = "Spalte";
        table.Add(VAR_COLUMN, 3, 4); 
        
        table.Add(VAR_TABBED, 4, 3, colspan: 2).Indent(8);

        table.Add(VAR_TWOCOLUMN, 5, 3, colspan: 2).Indent(8);

        table.Add(VAR_READONLY, 6, 3, colspan: 2).Indent(8);

        if (fragebogenModus)
        {
            table.Add<AFEditCheck>(7, 3, colspan: 2).Name("FBE_SET_FIRMA").Indent(8).Text("zu Firma");
            table.Add<AFEditCheck>(8, 3, colspan: 2).Name("FBE_SET_FILIALE").Indent(8).Text("zu Filiale");
            table.Add<AFEditCheck>(9, 3, colspan: 2).Name("FBE_SET_ANSP").Indent(8).Text("zu Ansprechpartner");
        }

        table.SetColumn(2, TablePanelEntityStyle.Relative, 1.0f);

        table.EndLayout();

        splitContainer.Panel1.Controls.Add(table);
        splitContainer.Panel2.Controls.Add(PANEL_DETAIL);

        DefaultEditorWidth = 750;
        DefaultEditorHeight = 450;
    }

    private void setEditor()
    {
        if (VAR_TYP.SelectedValue is not int type) return;

        Dock = DockStyle.Fill;

        var neededEditor = VariableBaseControllerUI.Instance.GetEditorType(type);

        if (PANEL_DETAIL.Controls.Count > 0)
        {
            if (PANEL_DETAIL.Controls[0].GetType() != neededEditor)
                PANEL_DETAIL.Controls.Clear(true);
        }

        if (PANEL_DETAIL.Controls.Count < 1)
        {
            PANEL_DETAIL.Controls.Add(ControlsEx.CreateInstance(neededEditor));
            PANEL_DETAIL.Controls[0].Dock = DockStyle.Fill;
        }

        //int panelHeight = ((CRVariableEditBase)PANEL_DETAIL.Controls[0]).DefaultEditorHeight + Padding.Vertical;
        //PANEL_DETAIL.Size = new(PANEL_DETAIL.Width, panelHeight);

        //MinimumSize = new(10, 200 + panelHeight);

        VAR_TYP.DataBindings[0].WriteValue();

        ((AFVariableEditBase)PANEL_DETAIL.Controls[0]).Model = (Model as Variable)!.GetVariable();
    }

    /// <inheritdoc />
    public override bool IsValid(ValidationErrorCollection errors)
    {
        var ret = base.IsValid(errors);

        if (PANEL_DETAIL.Controls[0] is not AFVariableEditBase details) return ret;

        if (details.Model is not VariableBase variable) return ret;

        if (details.IsValid(errors) == false)
            ret = false;
        
        if (ret)
            (Model as Variable)!.SetVariable(variable);

        return ret;
    }

    /// <summary>
    /// Beschriftungen der Variablen setzen
    /// </summary>
    /// <param name="name">Name der Variablen</param>
    /// <param name="caption">Beschriftung</param>
    /// <param name="description">Beschreibung</param>
    public void SetNameCaptionDescription(string? name, string? caption, string? description)
    {
        if (caption != null && caption.IsNotEmpty())
        { VAR_CAPTION.Text = caption; VAR_CAPTION.DataBindings[0].WriteValue(); }
        
        if (name != null && name.IsNotEmpty())
        { VAR_NAME.Text = caption; VAR_NAME.DataBindings[0].WriteValue(); }

        if (description != null && description.IsNotEmpty())
        { VAR_DESCRIPTION.Text = caption; VAR_DESCRIPTION.DataBindings[0].WriteValue(); }
    }
}

