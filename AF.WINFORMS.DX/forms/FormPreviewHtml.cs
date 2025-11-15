using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit;

namespace AF.WINFORMS.DX;

/// <summary>
/// Dialog zur Vorschau eines HTML-formatierten Textes
/// </summary>
public sealed class FormPreviewHtml : FormBase
{
    private RichEditControl edit;

    /// <summary>
    /// Constructor
    /// </summary>
    public FormPreviewHtml()
    {
        Text = "VORSCHAU";
        Size = new(600, 400);
        StartPosition = FormStartPosition.CenterParent;

        edit = new() { Dock = DockStyle.Fill, ReadOnly = true };
        edit.ActiveViewType = RichEditViewType.Simple;
        edit.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        
        AFLabel lbl = new() { Dock = DockStyle.Top, Padding = new(5), AutoSizeMode = LabelAutoSizeMode.Vertical, Text = "Bitte beachten Sie, dass Schriftart und -größe bei der späteren Verwendung des Textes abweichen können." };
        Controls.Add(lbl);
        Controls.Add(edit);
        edit.BringToFront();
    }

    /// <summary>
    /// Anzuzeigender HTML-Text
    /// </summary>
    /// <param name="htmlText"></param>
    public void SetHtmlText(string htmlText)
    {
        edit.HtmlText = htmlText;
    }

    /// <summary>
    /// Zugriff auf das RichEdit-Control
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public RichEditControl RichEdit => edit;
}