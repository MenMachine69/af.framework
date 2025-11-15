using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Internal;
using DevExpress.Utils;
using DevExpress.Utils.Layout;

namespace AF.WINFORMS.DX;

/// <summary>
/// Editor für HTML-Templates
/// </summary>
[ToolboxItem(false)]
public class AFEditHtmlTemplate : AFUserControl
{
    private readonly HtmlContentControl preview = null!;
    private readonly CssCodeViewer csseditor = null!;
    private readonly HtmlCodeViewer htmleditor = null!;
    private readonly AFSplitContainer container = null!;
    private readonly AFSplitContainer container2 = null!;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFEditHtmlTemplate()
    {
        if (UI.DesignMode) return;

        SuspendLayout();

        container = new()
        {
            Horizontal = true,
            Dock = DockStyle.Fill
        };
        container2 = new()
        {
            Horizontal = false,
            Dock = DockStyle.Fill
        };
        container.Panel1.Controls.Add(container2);

        preview = new()
        {
            IsTemplateEditing = true,
            Dock = DockStyle.Fill
        };
        preview.HtmlImages = Glyphs.GetImages();

        htmleditor = new()
        {
            ReadOnly = false,
            ShowLineNumbers = DefaultBoolean.True,
            ShowIndentGuides = DefaultBoolean.True,
            AllowCodeFolding = DefaultBoolean.True,
            Dock = DockStyle.Fill
        };
        htmleditor.CodeChanged += (_, _) =>
        {
            preview.HtmlTemplate.Set(htmleditor.Template, csseditor.Styles);
        };

        htmleditor.SetAvailableHtmlImages(Glyphs.GetImages());

        csseditor = new()
        {
            ReadOnly = false,
            ShowLineNumbers = DefaultBoolean.True,
            ShowIndentGuides = DefaultBoolean.True,
            AllowCodeFolding = DefaultBoolean.True,
            Dock = DockStyle.Fill
        };
        csseditor.CodeChanged += (_, _) =>
        {
            preview.HtmlTemplate.Set(htmleditor.Template, csseditor.Styles);
            htmleditor.SetAvailableStyles(csseditor.GetClasses().Keys);
        };

        container2.Panel1.Controls.Add(new AFLabel() { Text = "HTML", Padding = new(5), Dock = DockStyle.Top });
        container2.Panel1.Controls.Add(htmleditor);
        htmleditor.BringToFront();
        container2.Panel2.Controls.Add(new AFLabel() { Text = "CSS", Padding = new(5), Dock = DockStyle.Top });
        container2.Panel2.Controls.Add(csseditor);
        csseditor.BringToFront();
        container.Panel2.Controls.Add(new AFLabel() { Text = "Vorschau", Padding = new(5), Dock = DockStyle.Top });
        container.Panel2.Controls.Add(preview);
        preview.BringToFront();

        Controls.Add(container);

        ResumeLayout();
    }


    /// <summary>
    /// CSS-Code des Templates
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string CssTemplate { get => csseditor.Styles;  set => csseditor.Styles = value; }

    /// <summary>
    /// HTML-Code des Templates
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string HtmlTemplate { get => htmleditor.Template; set => htmleditor.Template = value; }


    /// <summary>
    /// HTML-Code Editor
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public HtmlCodeViewer HtmlEditor => htmleditor;

    /// <summary>
    /// CSS-Code Editor
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public CssCodeViewer CssEditor => csseditor;

    /// <summary>
    /// Preview-Control
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public HtmlContentControl Preview => preview;

    /// <summary>
    /// Verfügbare Felder bekannt machen
    /// </summary>
    /// <param name="fields"></param>
    public void SetAvailableFields(IEnumerable<string> fields)
    {
        htmleditor.SetAvailableFields(fields);
    }

    /// <summary>
    /// Laden aus einem AFHtmlTemplate
    /// </summary>
    /// <param name="htmlTemplate">HTML-Code</param>
    /// <param name="cssTemplate">CSS-Code</param>
    public void LoadTemplate(string htmlTemplate, string cssTemplate)
    {
        HtmlTemplate = htmlTemplate;
        CssTemplate = cssTemplate;
    }
    
    /// <summary>
    /// Splitterpositionen ausrichten
    /// </summary>
    public void AdjustSplitterPositions()
    {
        container.SplitterPosition = Width / 2;
        container2.SplitterPosition = Height / 2;
    }
}

/// <summary>
/// Editor-Form für HTML-Templates
/// </summary>
public sealed class FormEditHtmlTemplate : FormBase
{
    private readonly AFEditHtmlTemplate editor = null!;

    /// <summary>
    /// Constructor
    /// </summary>
    public FormEditHtmlTemplate()
    {
        if (UI.DesignMode) return;

        Text = "Schablone bearbeiten";
        StartPosition = FormStartPosition.CenterParent; 

        AFButtonPanel buttons = new();
        buttons.Dock = DockStyle.Bottom;
        buttons.ButtonOk.Text = "ÜBERNEHMEN";
        buttons.ButtonCancel.Text = "ABBRECHEN";
        buttons.ButtonOk.Click += (_, _) =>
        {
           DialogResult = DialogResult.OK;
           Close();
        };
        buttons.ButtonCancel.Click += (_, _) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };
        Controls.Add(buttons);

        AFTablePanel table = new() { Dock = DockStyle.Fill, UseSkinIndents = true };
        table.BeginLayout();

        editor = table.Add<AFEditHtmlTemplate>(1, 1);
        editor.Dock = DockStyle.Fill;

        table.SetRow(1, TablePanelEntityStyle.Relative, 1.0f);
        table.SetColumn(1, TablePanelEntityStyle.Relative, 1.0f);

        table.EndLayout();

        Controls.Add(table);

        Size = new(800, 500);
    }

    /// <inheritdoc />
    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);

        Application.DoEvents();
        editor.AdjustSplitterPositions();
    }

    /// <summary>
    /// HTML-Template
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string HtmlTemplate
    {
        get => editor.HtmlTemplate; 
        set => editor.HtmlTemplate = value;
    }

    /// <summary>
    /// CSS-Template
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string CssTemplate
    {
        get => editor.CssTemplate;
        set => editor.CssTemplate = value;
    }

}
