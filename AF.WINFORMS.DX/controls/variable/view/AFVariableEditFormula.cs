using DevExpress.Utils;
using DevExpress.Utils.Layout;
using DevExpress.XtraEditors;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace AF.WINFORMS.DX;

/// <summary>
/// Editor für eine Formel Variable
/// </summary>
[ToolboxItem(false)]
[DesignerCategory("Code")]
public class AFVariableEditFormula : AFVariableEditBase
{
    private AFEditCombo ReturnType;
    private AFEditSingleline NullValue;
    private AFEditMultiline Formel;
    private AFButton pshEvaluate;

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFVariableEditFormula()
    {
        ReturnType = new() { Name = nameof(ReturnType) };
        NullValue = new() { Name = nameof(NullValue) };
        Formel = new() { Name = nameof(Formel) };
        pshEvaluate = new() { Text = "Test" };
        pshEvaluate.Click += (_, _) =>
        {
            try
            {
                var result = AFCore.App.ScriptingService?.EvaluateExpression(Formel.Text);
                MsgBox.ShowInfoOk($"FORMEL TESTEN\r\nErgebnis: {result?.ToString() ?? "<null>"}");
            }
            catch (Exception ex)
            {
                MsgBox.ShowErrorOk($"FORMEL TESTEN\r\nBeim Test der Formel trat folgender Fehler auf: {ex.Message}");
            }
        };
        

        ReturnType.SetEnumeration(typeof(eVariableFormulaType));


        Margin = new(0);
        Padding = new(0);

        AFTablePanel table = new AFTablePanel() { Dock = DockStyle.Top, AutoSize = true, UseSkinIndents = false };
        Controls.Add(table);

        table.BeginLayout();
        var titel = table.Add<AFLabelCaptionSmall>(1, 1, colspan: 2);
        titel.Text = "Eigenschaften: <b>Formel</b>";
        titel.Margin = new(0, 3, 0, 3);

        var desc = table.Add<AFLabel>(2, 1, colspan: 2);
        desc.Text = "Eine Formel kann verwendet werden, um berechnete Variablen abzubilden. Dazu kann in der Formel jede andere Variable im " +
                    "gleichen Kontext (z.B. alle Eigenschaften einer Firma oder alle Variablen eines Scripts) zusammen mit diversen Funktionen " +
                    "(z.B. String-Funktionen, mathematische Funktionen etc.) verwendet werden. " +
                    "Der Rückgabetyp bestimmt den Typ der berechneten Variablen. Der Standardwert wird zurückgegeben, wenn die Berechnung der Formel " +
                    "nicht möglich ist (fehlende Variablen etc.).";
        desc.Appearance.TextOptions.WordWrap = WordWrap.Wrap;
        desc.Appearance.Options.UseTextOptions = true;
        desc.AutoSizeMode = LabelAutoSizeMode.Vertical;
        desc.Padding = new(6);

        table.Add<AFLabel>(3, 1).Indent(6).Text = "Rückgabetyp";
        table.Add(ReturnType, 3, 2);
        table.Add<AFLabel>(4, 1).Indent(6).Text = "Formel";
        table.Add(Formel, 4, 2, rowspan: 2).Height(46);
        table.Add<AFLabel>(6, 1).Text = "Standardwert";
        table.Add(NullValue, 6, 2);
        table.Add(pshEvaluate, 7, 1);

        table.SetColumn(2, TablePanelEntityStyle.Relative, 1.0f);
        table.SetRow(5, TablePanelEntityStyle.Absolute, 26.0f);

        table.EndLayout();

        DefaultEditorWidth = 500;
        DefaultEditorHeight = 220;
    }

    /// <summary>
    /// Variable
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public VariableFormula? Variable { get; set; }
}

