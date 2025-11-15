using DevExpress.Utils;
using DevExpress.Utils.Layout;
using DevExpress.Utils.Svg;
using DevExpress.XtraBars;

namespace AF.WINFORMS.DX;

/// <summary>
/// Editor für HTML-Quellcodes mit einer Toolbar und rudimentären Bearbeitungsfunktionen.
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[DefaultBindingProperty("CurrentText")]
[ToolboxTabName("AF Common")]
public class AFEditMultilineHtml : AFUserControl
{
    private readonly DevExpress.XtraEditors.Internal.HtmlCodeViewer textBox = null!;
    private readonly AFBarManager manager = null!;
    private readonly AFBarController barController = null!;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFEditMultilineHtml()
    {
        if (UI.DesignMode) return;

        barController = new();
        barController.AutoBackColorInBars = true;

        manager = new();
        manager.Form = this;
        manager.Controller = barController;
        manager.BeginInit();


        textBox = new() {
            ReadOnly = false,
            ShowLineNumbers = DefaultBoolean.False,
            ShowIndentGuides = DefaultBoolean.False,
            AllowCodeFolding = DefaultBoolean.False,
            Dock = DockStyle.Fill
        };

        AFTablePanel table = new AFTablePanel() { Dock = DockStyle.Fill, AutoSize = true, UseSkinIndents = false };
        Controls.Add(table);

        table.BeginLayout();
        var tbar = table.AddBar(manager, 1, 1);
        addButton(tbar, "btnCut", UI.GetImage(Symbol.Cut), "Ausschneiden", "Den markierten Text ausschneiden.");
        addButton(tbar, "btnCopy", UI.GetImage(Symbol.Copy), "Kopieren", "Den markierten Text in die Zwischenablage kopieren.");
        addButton(tbar, "btnPaste", UI.GetImage(Symbol.ClipboardPaste), "Einfügen", "Text aus der Zwischenablage einfügen.");
        addButton(tbar, "btnUndo", UI.GetImage(Symbol.ArrowUndo), "Rückgängig", "Das Bild zuschneiden.", true);
        addButton(tbar, "btnRedo", UI.GetImage(Symbol.ArrowRedo), "Wiederholen", "Das Bild zuschneiden.");
        addButton(tbar, "btnBold", UI.GetImage(Symbol.TextBold), "Fett", "Markierten Text fett darstellen.", true);
        addButton(tbar, "btnItalic", UI.GetImage(Symbol.TextItalic), "Kursiv", "Markierten Text kursiv darstellen.");
        addButton(tbar, "btnUnderline", UI.GetImage(Symbol.TextUnderline), "Unterstrichen", "Markierten Text unterstreichen.");
        addButton(tbar, "btnTextColor", UI.GetImage(Symbol.TextColor), "Textfarbe anpassen", "Farbe des markierten Textes anpassen (<font color...>).", true);
        addButton(tbar, "btnLink", UI.GetImage(Symbol.Link), "Link einfügen", "Link (URL) einfügen.");
        addButton(tbar, "btnImage", UI.GetImage(Symbol.Image), "Bild einfügen", "Verweis auf ein Bild einfügen (Verknüpfung zum Bild).");

        addButton(tbar, "btnPreview", UI.GetImage(Symbol.Search), "Vorschau", "Eine Vorschau des Textes anzeigen.", true);

        table.Add(textBox, 2, 1);

        table.SetColumn(1, TablePanelEntityStyle.Relative, 1.0f);
        table.SetRow(1, TablePanelEntityStyle.Absolute, 32.0f);
        table.SetRow(2, TablePanelEntityStyle.Relative, 1.0f);

        table.EndLayout();
        manager.EndInit();
    }

    private void addButton(Bar tbar, string name, SvgImage img, string caption, string description, bool group = false)
    {
        var btn = tbar.AddButton(name, img: img, begingroup: group);
        btn.ItemClick += btnclick;
        btn.SuperTip = UI.GetSuperTip(caption, description);
    }


    /// <summary>
    /// Zugriff auf das Bild.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
    public string CurrentText { get => textBox.Template; set => textBox.Template = value; }

    /// <summary>
    /// Zugriff auf das eigentliche DevExpress.XtraEditors.Internal.HtmlCodeViewer
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
    public DevExpress.XtraEditors.Internal.HtmlCodeViewer TextBox { get => textBox; }

    private void btnclick(object sender, ItemClickEventArgs e)
    {
        var edit = textBox.Controls.OfType<DevExpress.XtraEditors.SyntaxEditor.SyntaxEdit>().Single();


        switch (e.Item.Name)
        {
            case "btnCut":
                edit.Cut();
                break;
            case "btnCopy":
                edit.Copy();
                break;
            case "btnPaste":
                edit.Paste();
                break;
            case "btnUndo":
                edit.Undo();
                break;
            case "btnRedo":
                edit.Redo();
                break;
            case "btnBold":
                string txtBold = $"<b>{edit.SelectedText}</b>";
                pasteText(edit, txtBold);
                break;
            case "btnItalic":
                string txtItalic = $"<i>{edit.SelectedText}</i>";
                pasteText(edit, txtItalic);
                break;
            case "btnUnderline":
                string txtUnderline = $"<u>{edit.SelectedText}</u>";
                pasteText(edit, txtUnderline);
                break;
            case "btnTextColor":
                string txtColor = $"<font color=#000000>{edit.SelectedText}</font>";
                pasteText(edit, txtColor);
                break;
            case "btnLink":
                string txtlink = $"<a href=\"https://\">{edit.SelectedText}</a>";
                pasteText(edit, txtlink);
                break;
            case "btnImage":
                string txtimage = $"<img src=\"https://\" border=\"0\" />{edit.SelectedText}";
                pasteText(edit, txtimage);
                break;
            case "btnPreview":
                using (var dlg = new FormPreviewHtml())
                {
                    dlg.SetHtmlText(edit.Text.Replace("\r\n", "<br />\r\n"));
                    dlg.ShowDialog(FindForm());
                    dlg.RichEdit.Document.DefaultCharacterProperties.FontName = "Arial";
                    dlg.RichEdit.Document.DefaultCharacterProperties.FontSize = 12.0f;
                }
                break;

        }
    }

    private void pasteText(DevExpress.XtraEditors.SyntaxEditor.SyntaxEdit edit, string text)
    {
        string saved = Clipboard.GetText();
        Clipboard.SetText(text);
        edit.Paste();
        Clipboard.SetText(saved);
    }

    /// <inheritdoc />
    protected override void OnHandleDestroyed(EventArgs e)
    {
        base.OnHandleDestroyed(e);
        manager?.Dispose();
        barController?.Dispose();
    }
}

