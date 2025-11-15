namespace AF.WINFORMS.DX;

/// <summary>
/// Formular zur Eingabe von Variablenwerten (hostet ein AFVariableInputPanel)
/// </summary>
[DesignerCategory("Code")]
public sealed class AFFormVariableFormular : FormBase
{
    private readonly AFVariableInputPanel panel = null!;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="variables">Liste der variablen zur Eingabe.</param>
    /// <param name="caption">Überschrift des Dialogs (Standard: Parameter definieren)</param>
    /// <param name="description">Beschreibung für den Dialog</param>
    /// <param name="captionOk">Caption für OK-Button (Standard: Übernehmen)</param>
    /// <param name="captionCancel">Caption für Cancel-Button (Standard: Abbrechen)</param>
    public AFFormVariableFormular(IEnumerable<IVariable> variables, string caption = "Parameter definieren", string? description = null, string captionOk = "Übernehmen", string captionCancel = "Abbrechen")
    {
        if (UI.DesignMode) return;

        Text = caption;
        StartPosition = FormStartPosition.CenterParent;
        Size = new(600, 800);

        AFErrorProvider errorProvider = new(ComponentsContainer);
        
        AFButtonPanel buttons = new() { Dock = DockStyle.Bottom };
        Controls.Add(buttons);

        if (description != null)
        {
            AFLabel lblDesc = new() { Dock = DockStyle.Top, Text = description, AllowHtmlString = true };
            lblDesc.Padding = new(8);
            lblDesc.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            lblDesc.Appearance.Options.UseTextOptions = true;
            lblDesc.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            Controls.Add(lblDesc);
        }

        panel = new(variables) { Dock = DockStyle.Fill, UseSkinIndents = true };
        Controls.Add(panel);
        panel.BringToFront();

        buttons.ButtonOk.Text = captionOk;
        buttons.ButtonOk.Click += (_, _) =>
        {
            if (Validate() && panel.Validate(errorProvider))
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        };
        
        buttons.ButtonCancel.Text = captionCancel;
        buttons.ButtonCancel.Click += (_, _) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };
    }

    /// <summary>
    /// Ergebnisse der Eingabe (Werte)
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public List<VariableUserValue> Result => panel.Result;
}

/// <summary>
/// Panel zur Eingabe von Variablen-Werten
/// </summary>
public class AFVariableInputPanel : AFTablePanel
{
    /// <summary>
    /// Constructor...
    /// </summary>
    public AFVariableInputPanel()
    {
        if (UI.DesignMode) return;

        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
    }

    /// <summary>
    /// Constructor...
    /// </summary>
    /// <param name="variables"></param>
    public AFVariableInputPanel(IEnumerable<IVariable> variables)
    {
        if (UI.DesignMode) return;

        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;


        LoadForm(variables);
    }

    /// <summary>
    /// Ergebnisse der Eingabe (Werte)
    /// </summary>
    public List<VariableUserValue> Result
    {
        get
        {
            List<VariableUserValue> liste = [];
            getResults(this.Controls, liste);
            return liste;
        }
    }

    private void getResults(ControlCollection controls, List<VariableUserValue> liste)
    {
        liste ??= new();

        foreach (Control control in controls)
        {
            if (control.Tag is not Tuple<IVariable, AFLabel?> info) continue;

            if (info.Item1.VAR_TYP == (int)eVariableType.Section) continue;

            liste.Add(new() { Name = control.Name, Variable = info.Item1, Value = control.InvokeGet(control.GetBindingPropertyName()) });

            if (control.Controls.Count > 0)
                getResults(control.Controls, liste);
        }
    }

    /// <summary>
    /// Validieren der Eingaben.
    /// </summary>
    /// <param name="errorProvider">ErrorProvider, der die Fehler anzeigt.</param>
    /// <returns></returns>
    public bool Validate(AFErrorProvider errorProvider)
    {
        return true;
    }
}
