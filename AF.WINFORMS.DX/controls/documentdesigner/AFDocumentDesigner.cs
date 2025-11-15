using AF.BUSINESS;

namespace AF.WINFORMS.DX;

/// <summary>
/// Designer für Dokumente (RichText, Report, HTML/EMail, Text)
/// </summary>
public class AFDocumentDesigner : AFEditorBase, IVariableConsumer
{
    private AFDocumentDesignerRichtext? editorRichText;
    private AFDocumentDesignerReport? editorReport;
    private AFDocumentDesignerEmpty? editorEmpty;
    private AFDocumentDesignerExport? editorExport;
    private IDokumentvorlage? template;
    private eDocumentType currentType = eDocumentType.None;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFDocumentDesigner()
    {
        if (UI.DesignMode) return;

        SetEditor(eDocumentType.None);
    }

    /// <summary>
    /// Zu bearbeitende Vorlage
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IDokumentvorlage? Template 
    { 
        get => template;
        set 
        {
            template = value;

            if (value == null)
            {
                SetEditor(eDocumentType.None);
                return;
            }

            SetEditor(template!.DocumentType);
            SetData(template!.DocumentData);
        } 
    }

    /// <summary>
    /// Daten des Dokuments aus ByteArray setzen (Text, Report etc.).
    /// </summary>
    /// <param name="documentData"></param>
    public void SetData(byte[] documentData)
    {
        if (currentType == eDocumentType.RichText)
        {
            editorRichText!.DataSource = Template!.DocumentSample;
            editorRichText!.DocxMaster = documentData;
            editorRichText!.Variables.Clear();
            editorRichText!.Variables.AddRange(Template!.Variablen.ToArray());
        }
        else if (currentType == eDocumentType.RichTextOverlay)
        {
            editorRichText!.DataSource = Template!.DocumentSample;
            editorRichText!.DocxMaster = documentData;
            editorRichText!.Variables.Clear();
            editorRichText!.Variables.AddRange(Template!.Variablen.ToArray());
        }
        else if (currentType == eDocumentType.Email)
        {
            editorRichText!.DataSource = Template!.DocumentSample;
            editorRichText!.HtmlMaster = documentData.FromByteArray();
            editorRichText!.Variables.Clear();
            editorRichText!.Variables.AddRange(Template!.Variablen.ToArray());
        }
        else if (currentType == eDocumentType.HTMLTemplate)
        {
            IDokumentvorlageHtml text = Template!.TemplateHtml;

            editorRichText!.DataSource = Template!.DocumentSample;
            editorRichText!.HtmlMaster = text.Master;
            editorRichText!.HtmlDetail = text.Detail;
            editorRichText!.Variables.Clear();
            editorRichText!.Variables.AddRange(Template!.Variablen.ToArray());
        }
        else if (currentType == eDocumentType.Report)
        {
            editorReport!.DataSource = Template!.DocumentSample;
            editorReport!.Report = documentData;
        } 
        else if (currentType == eDocumentType.TextOnly)
        {
            IDokumentvorlageText text = Template!.TemplateText;

            editorExport!.DataSource = Template!.DocumentSample;
            editorExport!.Header = text.Header;
            editorExport!.Footer = text.Footer;
            editorExport!.Detail = text.Detail;
        }

    }

    /// <summary>
    /// Daten des Dokuments in ByteArray übertragen.
    /// </summary>
    public byte[] GetData()
    {
        if (currentType == eDocumentType.RichText)
            return editorRichText!.DocxMaster;
        
        if (currentType == eDocumentType.RichTextOverlay)
            return editorRichText!.DocxMaster;
        
        if (currentType == eDocumentType.Email)
            return editorRichText!.HtmlMaster.ToByteArray();
        
        if (currentType == eDocumentType.Report)
            return editorReport!.Report;

        if (currentType == eDocumentType.HTMLTemplate)
        {
            var text = Template!.TemplateHtml;
            text.Master = editorRichText!.HtmlMaster;
            text.Detail = editorRichText!.HtmlDetail;
            return text.ToJsonBytes();
        }

        if (currentType == eDocumentType.TextOnly)
        {
            var text = Template!.TemplateText;
            text.Header = editorExport!.Header;
            text.Detail = editorExport!.Detail;
            text.Footer = editorExport!.Footer;
            return text.ToJsonBytes();
        }

        return [];
    }

    /// <summary>
    /// Den zum Dokumenttyp passenden Editor laden...
    /// </summary>
    /// <param name="type"></param>
    public void SetEditor(eDocumentType type)
    {
        if (currentType == type && Controls.Count > 0) return;

        currentType = type;

        Type? needEditor = null;
        if (type == eDocumentType.RichText)
            needEditor = typeof(AFDocumentDesignerRichtext);
        else if (type == eDocumentType.RichTextOverlay)
            needEditor = typeof(AFDocumentDesignerRichtext);
        else if (type == eDocumentType.Email)
            needEditor = typeof(AFDocumentDesignerRichtext);
        else if (type == eDocumentType.TextOnly)
            needEditor = typeof(AFDocumentDesignerExport);
        else if (type == eDocumentType.Report)
            needEditor = typeof(AFDocumentDesignerReport);
        else if (type == eDocumentType.HTMLTemplate)
            needEditor = typeof(AFDocumentDesignerRichtext);
        else if (type == eDocumentType.None)
            needEditor = typeof(AFDocumentDesignerEmpty);

        if (Controls.Count > 0 && Controls[0].GetType() == needEditor)
        {
            if (needEditor == typeof(AFDocumentDesignerRichtext))
            {
                switch (type)
                {
                    case eDocumentType.Email:
                        editorRichText?.SetMode(AFDocumentDesignerRichtext.eEditorMode.Email);
                        break;
                    case eDocumentType.HTMLTemplate:
                        editorRichText?.SetMode(AFDocumentDesignerRichtext.eEditorMode.HtmlTemplate);
                        break;
                    default:
                        editorRichText?.SetMode(AFDocumentDesignerRichtext.eEditorMode.RichText);
                        break;
                }
            }
        }

        if (needEditor == typeof(AFDocumentDesignerRichtext))
        {
            editorRichText ??= new() { Dock = DockStyle.Fill };

            switch (type)
            {
                case eDocumentType.Email:
                    editorRichText.SetMode(AFDocumentDesignerRichtext.eEditorMode.Email);
                    break;
                case eDocumentType.HTMLTemplate:
                    editorRichText.SetMode(AFDocumentDesignerRichtext.eEditorMode.HtmlTemplate);
                    break;
                default:
                    editorRichText.SetMode(AFDocumentDesignerRichtext.eEditorMode.RichText);
                    break;
            }

            Controls.Clear();
            Controls.Add(editorRichText);
        }
        else if (needEditor == typeof(AFDocumentDesignerReport))
        {
            editorReport ??= new() { Dock = DockStyle.Fill };

            Controls.Clear();
            Controls.Add(editorReport);
        }
        else if (needEditor == typeof(AFDocumentDesignerExport))
        {
            editorExport ??= new() { Dock = DockStyle.Fill };

            Controls.Clear();
            Controls.Add(editorExport);
        }
        else if (needEditor == typeof(AFDocumentDesignerEmpty))
        {
            editorEmpty ??= new() { Dock = DockStyle.Fill };

            Controls.Clear();
            Controls.Add(editorEmpty);
        }

    }

    /// <inheritdoc />
    public override bool IsValid(ValidationErrorCollection errors)
    {
        bool ret = true;

        if (Template == null) return base.IsValid(errors);

        Template.DocumentData = GetData();
        if (Controls[0] == editorRichText)
            Template.Variablen = editorRichText?.Variables ?? [];
        else if (Controls[0] == editorExport)
            Template.Variablen = editorExport?.Variables ?? [];

        return ret && base.IsValid(errors);
    }

    /// <summary>
    /// Zugriff auf die Liste der Variablen
    /// </summary>
    public BindingList<Variable> Variables => editorRichText?.Variables ?? [];
}