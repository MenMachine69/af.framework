using DevExpress.Utils;
using DevExpress.Utils.Layout;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Editor für eine Memo Variable
/// </summary>
[ToolboxItem(false)]
[DesignerCategory("Code")]
public class AFVariableEditGuid : AFVariableEditBase
{
    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFVariableEditGuid()
    {
        AFEditSingleline Default = new() { Name = nameof(Default) };
        Default.SetMask(eSinglelineEditMask.Guid);
        Default.Size = new(280, 26);

        AFTablePanel table = new AFTablePanel() { Dock = DockStyle.Top, AutoSize = true, UseSkinIndents = false };
        Controls.Add(table);

        table.BeginLayout();
        var titel = table.Add<AFLabelCaptionSmall>(1, 1, colspan: 3);
        titel.Text = "Eigenschaften: <b>GUID</b>";
        titel.Margin = new(0, 3, 0, 3);

        var desc = table.Add<AFLabel>(2, 1, colspan: 3);
        desc.Text = "Eine GUID-Variable kann verwendet werden um zum Beispiel flexibel auf ein Datenelement zu verweisen (primärer Key des Datenelements). Dazu muss der Benutzer die GUID des Datenelements eingeben.";
        desc.Appearance.TextOptions.WordWrap = WordWrap.Wrap;
        desc.Appearance.Options.UseTextOptions = true;
        desc.AutoSizeMode = LabelAutoSizeMode.Vertical;
        desc.Padding = new(6);

        table.Add<AFLabel>(3, 1).Indent(6).Text = "Vorgabe";
        table.Add(Default, 3, 2);

        table.SetColumn(3, TablePanelEntityStyle.Relative, 1.0f);

        table.EndLayout();

        DefaultEditorWidth = 500;
        DefaultEditorHeight = 170;
    }

    /// <summary>
    /// Variable
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public VariableGuid? Variable { get; set; }
}