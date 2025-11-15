using DevExpress.Utils;
using DevExpress.Utils.Layout;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Dialog zur Anzeige eines Controls, dass Daten vom Benutzer erfragt.
///
/// Es kann ein beliebiges Control angezeigt werden.
/// </summary>
public sealed class FormAsk : FormBase
{
    /// <summary>
    /// Dialog initialisieren
    /// </summary>
    /// <param name="ctrl">Control, dass zur Eingabe angezeigt werden soll</param>
    /// <param name="label">Label zur Anzeige vor dem Control</param>
    /// <param name="question">Beschreibung/Frage</param>
    /// <param name="caption">Fenstertitel</param>
    /// <param name="newsize">Größe des Dialogs (unskaliert) - Standard ist 500 x 200 px</param>
    public FormAsk(Control ctrl, string label, string question, string caption, Size? newsize = null)
    {
        if (UI.DesignMode) return;

        Text = caption;
        StartPosition = FormStartPosition.CenterParent;

        AFButtonPanel buttons = new() { Dock = DockStyle.Bottom };
        buttons.ButtonOk.Text = "OK";
        buttons.ButtonOk.Click += (_, _) =>
        {
            if (Validate() && (ValidateInput?.Invoke(ctrl) ?? true))
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        };
        buttons.ButtonCancel.Text = "ABBRECHEN";
        buttons.ButtonCancel.Click += (_, _) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };
        Controls.Add(buttons);

        AFTablePanel table = new()
        {
            Dock = DockStyle.Fill,
            UseSkinIndents = true
        };
        table.BeginLayout();

        var lbl = table.Add<AFLabel>(1, 1, colspan: 2);
        lbl.Text = question;
        lbl.AutoSizeMode = LabelAutoSizeMode.Vertical;
        lbl.Dock = DockStyle.Fill;
        lbl.Appearance.TextOptions.WordWrap = WordWrap.Wrap;
        lbl.Appearance.TextOptions.VAlignment = VertAlignment.Top;
        lbl.Appearance.Options.UseTextOptions = true;

        lbl = table.Add<AFLabel>(2, 1);
        lbl.Text = label;

        table.Add(ctrl, 2, 2);

        table.SetRow(1, TablePanelEntityStyle.Relative, 1.0f);
        table.SetColumn(2, TablePanelEntityStyle.Relative, 1.0f);

        table.EndLayout();
        Controls.Add(table);
        table.BringToFront();

        Size = newsize ?? new(500, 200);
    }

    /// <summary>
    /// Funktion, die zur Validierung der Eingabe aufgerufen werden soll (optional).
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Func<Control, bool>? ValidateInput { get; set; }
}

