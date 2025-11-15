namespace AF.WINFORMS.DX;

/// <summary>
/// Control zur Dateivorschau.
/// 
/// Unterstützt:
/// PDF, Word, Excel, JPEG, PNG, ZIP
/// </summary>
public class AFFilePreview : AFUserControl
{
    private AFEditRichText? previewWord;
    private AFPictureBox? previewJpegPng;
    private AFPdfViewer? previewPdf;
    private AFGridControl? previewZip;
    private AFLabelBoldText? previewUnknown;
    private AFEditMultiline? previewText;
    private AFEditMultilineSyntax? previewSyntax;
    private AFSpreadSheet? previewSpreadSheet;
    private ILibraryDocument? libraryDocument;
    private FileInfo? currentFile;
    private readonly AFLabel lblInfo = null!;
    private readonly AFPanel panel = null!;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFFilePreview()
    {
        if (UI.DesignMode) return;

        lblInfo = new() { Dock = DockStyle.Bottom, Padding = new(8), AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical, Text = "... MB" };
        panel = new() { Dock = DockStyle.Fill, BorderStyle = BorderStyle.None };
        Controls.Add(lblInfo);
        Controls.Add(panel);
        panel.BringToFront();
    }

    /// <summary>
    /// In der Preview anzuzeigendes Dokument.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ILibraryDocument? LibraryDocument 
    { 
        get => libraryDocument;
        set
        {
            libraryDocument = value;

            if (libraryDocument == null)
            { 
                SetEditor(eLibraryDocumentType.None);
                return;
            }

            Show(libraryDocument.DocumentData, libraryDocument.DocumentType);
        } 
    }

    /// <summary>
    /// den passenden Editor für die Vorschau setzen.
    /// </summary>
    /// <param name="type"></param>
    public void SetEditor(eLibraryDocumentType type)
    {
        Type neededitor;

        switch (type)
        {
            case eLibraryDocumentType.None:
                neededitor = typeof(AFLabelBoldText);
                break;
            case eLibraryDocumentType.PDF:
                neededitor = typeof(AFPdfViewer);
                break;
            case eLibraryDocumentType.Word:
                neededitor = typeof(AFEditRichText);
                break;
            case eLibraryDocumentType.Excel:
                neededitor = typeof(AFSpreadSheet);
                break;
            case eLibraryDocumentType.Jpeg:
                neededitor = typeof(AFPictureBox);
                break;
            case eLibraryDocumentType.Png:
                neededitor = typeof(AFPictureBox);
                break;
            case eLibraryDocumentType.Zip:
                neededitor = typeof(AFGridControl);
                break;
            case eLibraryDocumentType.Text:
                neededitor = typeof(AFEditMultiline);
                break;
            case eLibraryDocumentType.XML:
                neededitor = typeof(AFEditMultilineSyntax);
                break;
            case eLibraryDocumentType.Json:
                neededitor = typeof(AFEditMultilineSyntax);
                break;
            case eLibraryDocumentType.SQL:
                neededitor = typeof(AFEditMultilineSyntax);
                break;
            case eLibraryDocumentType.CSS:
                neededitor = typeof(AFEditMultilineSyntax);
                break;
            case eLibraryDocumentType.HTML:
                neededitor = typeof(AFEditMultilineSyntax);
                break;
            case eLibraryDocumentType.CSharp:
                neededitor = typeof(AFEditMultilineSyntax);
                break;
            case eLibraryDocumentType.Other:
                neededitor = typeof(AFPdfViewer);
                break;
            default:
                neededitor = typeof(AFLabelBoldText);
                return;
        }

        if (panel.Controls.Count > 0 && panel.Controls[0].GetType() == neededitor)
            return;


        Control? ctrl = null;

        if (neededitor == typeof(AFPdfViewer))
            ctrl = previewPdf ??= new AFPdfViewer() { Dock = DockStyle.Fill };
        else if (neededitor == typeof(AFLabelBoldText))
        {
            ctrl = previewUnknown ??= new AFLabelBoldText() { Dock = DockStyle.Fill };
            previewUnknown.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            previewUnknown.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            previewUnknown.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            previewUnknown.Appearance.Options.UseTextOptions = true;
            previewUnknown.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            previewUnknown.Text = "Vorschau für ausgewählten Typen/ausgewählte Datei nicht verfügbar.";
        }
        else if (neededitor == typeof(AFSpreadSheet))
        {
            ctrl = previewSpreadSheet ??= new AFSpreadSheet() { Dock = DockStyle.Fill, ReadOnly = true };
            previewSpreadSheet.Options.Behavior.UseSkinColors = false;
        }
        else if (neededitor == typeof(AFEditMultiline))
            ctrl = previewText ??= new AFEditMultiline() { Dock = DockStyle.Fill, ReadOnly = true };
        else if (neededitor == typeof(AFEditMultilineSyntax))
        {
            ctrl = previewSyntax ??= new AFEditMultilineSyntax() { Dock = DockStyle.Fill };
            if (type == eLibraryDocumentType.XML)
                previewSyntax.SetMode(eSyntaxMode.XML);
            else if (type == eLibraryDocumentType.SQL)
                previewSyntax.SetMode(eSyntaxMode.SQL);
            else if (type == eLibraryDocumentType.CSharp)
                previewSyntax.SetMode(eSyntaxMode.CSharp);
            else if (type == eLibraryDocumentType.Json)
                previewSyntax.SetMode(eSyntaxMode.Json);
            else if (type == eLibraryDocumentType.CSS)
                previewSyntax.SetMode(eSyntaxMode.CSS);
            else if (type == eLibraryDocumentType.HTML)
                previewSyntax.SetMode(eSyntaxMode.HTML);
        }
        else if (neededitor == typeof(AFEditRichText))
        {
            ctrl = previewWord ??= new AFEditRichText() { Dock = DockStyle.Fill, ReadOnly = true };
            previewWord.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.PrintLayout;
        }
        else if (neededitor == typeof(AFPictureBox))
        {
            ctrl = previewJpegPng ??= new AFPictureBox() { Dock = DockStyle.Fill, BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder };
            previewJpegPng.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
            previewJpegPng.PopupMenuShowing += (_, a) =>
            {
                a.Cancel = true;
            };
        }
        else if (neededitor == typeof(AFGridControl))
        {
            ctrl = previewZip ??= new AFGridControl() { Dock = DockStyle.Fill };
            
            AFGridSetup setup = new()
            {
                AllowEdit = false,
                AllowAddNew = false,
                AllowMultiSelect = false
            };
            setup.Columns.Add(new() { ColumnFieldname = nameof(ZipFileInfo.Name), Caption = "Name", Bold = true });
            setup.Columns.Add(new() { ColumnFieldname = nameof(ZipFileInfo.FullName), Caption = "vollst. Name" });
            setup.Columns.Add(new() { ColumnFieldname = nameof(ZipFileInfo.Length), Caption = "Größe" });
            setup.Columns.Add(new() { ColumnFieldname = nameof(ZipFileInfo.CompressedLength), Caption = "Komprimiert" });
            setup.Columns.Add(new() { ColumnFieldname = nameof(ZipFileInfo.CompressionFactor), Caption = "Rate", DisplayFormat = "p1" });

            previewZip.Setup(setup);
        }

        if (panel.Controls.Count > 0)
            panel.Controls.Clear();


        if (ctrl != null) panel.Controls.Add(ctrl);
    }

    /// <summary>
    /// Preview für den übergebenen Inhalt anzeigen
    /// </summary>
    /// <param name="data"></param>
    /// <param name="type"></param>
    public void Show(byte[] data, eLibraryDocumentType type)
    {
        SetEditor(type);

        lblInfo.Text = $"{(data.Length / 1024.0 / 1024.0).ToString("f3")} MB";

        if (panel.Controls.Count < 1) return;

        if (panel.Controls[0] is AFLabelBoldText) return;

        if (panel.Controls[0] is AFPdfViewer pdfViewer)
            pdfViewer.LoadDocument(data);

        if (panel.Controls[0] is AFEditMultiline mle)
            mle.Text = data.FromByteArray();

        if (panel.Controls[0] is AFEditMultilineSyntax mleSyntax)
            mleSyntax.SourceCode = data.FromByteArray();

        if (panel.Controls[0] is AFEditRichText rtf)
            rtf.LoadDocument(data);

        if (panel.Controls[0] is AFPictureBox pict)
            pict.Image = ImageEx.FromByteArray(data);

        if (panel.Controls[0] is AFSpreadSheet spreadsheet)
            spreadsheet.LoadDocument(data);

        if (panel.Controls[0] is AFGridControl grid)
            grid.DataSource = data.GetZipEntries();
    }


    /// <summary>
    /// Vorschau aus Datei anzeigen
    /// </summary>
    /// <param name="file"></param>
    public void Show(FileInfo file)
    {
        if (!file.Exists) return;

        currentFile = file;
        lblInfo.Text = $"{(file.Length / 1024.0 / 1024.0).ToString("f3")} MB";

        var type = file.GetLibraryDocumentType();

        SetEditor(type);

        if (type == eLibraryDocumentType.Jpeg)
            previewJpegPng!.Image = ImageEx.FromFile(file.FullName);
        else if (type == eLibraryDocumentType.Png)
            previewJpegPng!.Image = ImageEx.FromFile(file.FullName);
        else if (type == eLibraryDocumentType.Word)
            previewWord!.LoadDocument(file.FullName);
        else if (type == eLibraryDocumentType.Excel)
            previewSpreadSheet!.LoadDocument(file.FullName);
        else if (type == eLibraryDocumentType.PDF)
            previewPdf!.LoadDocument(file.FullName);
        else if (type == eLibraryDocumentType.CSS)
            previewSyntax!.SourceCode = File.ReadAllText(file.FullName);
        else if (type == eLibraryDocumentType.HTML)
            previewSyntax!.SourceCode = File.ReadAllText(file.FullName);
        else if (type == eLibraryDocumentType.SQL)
            previewSyntax!.SourceCode = File.ReadAllText(file.FullName);
        else if (type == eLibraryDocumentType.CSharp)
            previewSyntax!.SourceCode = File.ReadAllText(file.FullName);
        else if (type == eLibraryDocumentType.Json)
            previewSyntax!.SourceCode = File.ReadAllText(file.FullName);
        else if (type == eLibraryDocumentType.Text)
            previewText!.Text = File.ReadAllText(file.FullName);
        else if (type == eLibraryDocumentType.XML)
            previewSyntax!.SourceCode = File.ReadAllText(file.FullName);
        else if (type == eLibraryDocumentType.Zip)
            previewZip!.DataSource = file.GetZipEntries();
    }

    /// <summary>
    /// Aktuell in der Preview angezeigte Informationen als ByteArray
    /// </summary>
    /// <returns></returns>
    public byte[]? GetData()
    {
        if (panel.Controls.Count < 1) return null;

        return (currentFile != null && currentFile.Exists ? File.ReadAllBytes(currentFile.FullName) : null);
    }
}
