using DevExpress.Utils;
using DevExpress.Utils.Layout;
using DevExpress.XtraEditors;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace AF.WINFORMS.DX;

/// <summary>
/// Editor für eine Script Variable
/// </summary>
[ToolboxItem(false)]
[DesignerCategory("Code")]
public class AFVariableEditScript : AFVariableEditBase
{
    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFVariableEditScript()
    {
        Control Script = ControlsEx.CreateInstance(AFCore.ScriptingService!.GetScriptSelectControlType());
        ((IScriptSelectControl)Script).ScriptTypeFilter = eScriptType.Variable;

        Script.Name = nameof(Script);
        
        Margin = new(0);
        Padding = new(0);

        AFTablePanel table = new AFTablePanel() { Dock = DockStyle.Top, AutoSize = true, UseSkinIndents = false };
        Controls.Add(table);

        table.BeginLayout();
        var titel = table.Add<AFLabelCaptionSmall>(1, 1, colspan: 2);
        titel.Text = "Eigenschaften: <b>Script</b>";
        titel.Margin = new(0, 3, 0, 3);
        var desc = table.Add<AFLabel>(2, 1, colspan: 2); 
        desc.Text = "Variablen/Eigenschaften, die durch einen Script berechnet werden, stehen niemals zur interaktiven Bearbeitung durch den Benutzer zur Verfügung. " +
                    "Es werden auch keine Werte tatsächlich gespeichert und/oder archiviert. Stattdessen wird der Wert durch den Script basierend auf der Datenquelle " +
                    "(z.B. Firma, Beteiligung oder Person) immer neu berechnet. Der benutzer kann also z.B. die Eigenschaft zuweisen, den Wert der Eigenschaft aber " +
                    "NICHT bearbeiten.";
        desc.Appearance.TextOptions.WordWrap = WordWrap.Wrap;
        desc.Appearance.Options.UseTextOptions = true;
        desc.AutoSizeMode = LabelAutoSizeMode.Vertical;
        desc.Padding = new(6);

        table.Add<AFLabel>(3, 1).Indent(6).Text = "auszuführender Script";
        table.Add(Script, 3, 2);

        table.SetColumn(2, TablePanelEntityStyle.Relative, 1.0f);

        table.EndLayout();

        DefaultEditorWidth = 500;
        DefaultEditorHeight = 150;
    }

    /// <summary>
    /// Variable
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public VariableScript? Variable { get; set; }
}

