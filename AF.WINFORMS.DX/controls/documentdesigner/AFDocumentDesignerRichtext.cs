using AF.MVC;

namespace AF.WINFORMS.DX;

/// <summary>
/// Designer/Editor für RichTexte
/// </summary>
public partial class AFDocumentDesignerRichtext : AFUserControl, IVariableConsumer
{
    private readonly AFVariableBrowser variableBrowser = null!;
    private readonly AFDatasourceBrowser datasourceBrowser = null!;
    private eEditorMode _mode = eEditorMode.Undefined;
    private object? dataSource;
    private readonly AFGridExtender extenderVariablen = null!;
    private readonly AFGridExtender extenderDatasource = null!;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFDocumentDesignerRichtext()
    {
        InitializeComponent();

        if (UI.DesignMode) return;
        
        replaceGlyphs();

        richEditControl1.Options.HorizontalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Visible;
        richEditControl1.Options.VerticalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Visible;
        richEditControl1.AllowDrop = true;
        richEditControl1.FormatCopyFinished += (_, _) =>
        {
            checkFormatCopy.Checked = false;
        };

        richEditControl2.Options.HorizontalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Visible;
        richEditControl2.Options.VerticalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Visible;
        richEditControl2.AllowDrop = true;
        richEditControl2.FormatCopyFinished += (_, _) =>
        {
            checkFormatCopy.Checked = false;
        };

        variableBrowser = new() { Dock = DockStyle.Fill };
        dockVariablen.Controls.Add(variableBrowser);

        datasourceBrowser = new() { Dock = DockStyle.Fill };
        dockDatasource.Controls.Add(datasourceBrowser);

        AFDragDropHelper dragHelper = new();
        dragHelper.Register(richEditControl1, acceptDropData, dropData);
        dragHelper.Register(richEditControl2, acceptDropData, dropData2);

        extenderVariablen = new();
        extenderVariablen.Grid = variableBrowser.GridVariablen;
        extenderVariablen.SupportDragDrop = true;
        extenderVariablen.RequestDragData += (_, e) =>
        {
            if (e.DraggedRow is IVariable snippet)
                e.DragData = "{"+snippet.VAR_NAME +"}";
        };

        extenderDatasource = new();
        extenderDatasource.Grid = datasourceBrowser.GridFields;
        extenderDatasource.SupportDragDrop = true;
        extenderDatasource.RequestDragData += (_, e) =>
        {
            if (e.DraggedRow is DatasourceField snippet)
                e.DragData = "#" + snippet.FieldName + "#";
        };


        lblHeaderMaster.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Window);
        lblHeaderDetail.BackColor = UI.TranslateSystemToSkinColor(SystemColors.GrayText); 

        richEditControl1.GotFocus += (_, _) => { 
            richEditBarController1.Control = richEditControl1;
            lblHeaderMaster.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Window);
            lblHeaderDetail.BackColor = UI.TranslateSystemToSkinColor(SystemColors.GrayText);
        };
        richEditControl2.GotFocus += (_, _) => {
            richEditBarController1.Control = richEditControl2;
            lblHeaderMaster.BackColor = UI.TranslateSystemToSkinColor(SystemColors.GrayText);
            lblHeaderDetail.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Window);
        };
    }

    private void dropData(System.Windows.Forms.IDataObject obj, Point pos)
    {
        var data = obj.GetData(typeof(string)) as string;

        if (data == null) return;

        if (string.IsNullOrEmpty(data)) return;

        if (richEditControl1.Document != null && richEditControl1.Document.CaretPosition != null)
            richEditControl1.Document.InsertText(richEditControl1.Document.CaretPosition, data);
    }

    private void dropData2(System.Windows.Forms.IDataObject obj, Point pos)
    {
        var data = obj.GetData(typeof(string)) as string;

        if (data == null) return;

        if (string.IsNullOrEmpty(data)) return;

        if (richEditControl2.Document != null && richEditControl2.Document.CaretPosition != null)
            richEditControl2.Document.InsertText(richEditControl2.Document.CaretPosition, data);
    }

    private bool acceptDropData(System.Windows.Forms.IDataObject obj)
    {
        var data = obj.GetData(typeof(string)) as string;

        if (data == null) return false;

        if (string.IsNullOrEmpty(data)) return false;

        return true;
    }

    /// <summary>
    /// RichText (RTF) des Masters
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string RtfMaster { get => richEditControl1.RtfText; set => richEditControl1.RtfText = value; }

    /// <summary>
    /// RichText (RTF) des Details
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string RtfDetail { get => richEditControl2.RtfText; set => richEditControl2.RtfText = value; }

    /// <summary>
    /// HtmlText (HTML) des Masters
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string HtmlMaster { get => richEditControl1.HtmlText; set => richEditControl1.HtmlText = value; }

    /// <summary>
    /// HtmlText (HTML) des Details
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string HtmlDetail { get => richEditControl2.HtmlText; set => richEditControl2.HtmlText = value; }

    /// <summary>
    /// MhtText (MHT) des Masters
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string MhtMaster { get => richEditControl1.MhtText; set => richEditControl1.MhtText = value; }

    /// <summary>
    /// MhtText (MHT) des Details
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string MhtDetail { get => richEditControl2.MhtText; set => richEditControl2.MhtText = value; }

    /// <summary>
    /// Docx (Word) des Masters
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public byte[] DocxMaster { get => richEditControl1.DocxBytes; set => richEditControl1.DocxBytes = value; }

    /// <summary>
    /// Docx (Word) des Details
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public byte[] DocxDetail { get => richEditControl2.DocxBytes; set => richEditControl2.DocxBytes = value; }


    /// <summary>
    /// Aktueller Modus
    /// </summary>
    public eEditorMode CurrentMode => _mode;

    /// <summary>
    /// Modus des Editors
    /// </summary>
    public enum eEditorMode
    {
        /// <summary>
        /// Undefinierter Zustand
        /// </summary>
        Undefined,
        /// <summary>
        /// RichText-Dokument odser Overlay
        /// </summary>
        RichText,
        /// <summary>
        /// Email-Dokument
        /// </summary>
        Email,
        /// <summary>
        /// HTML-Template
        /// </summary>
        HtmlTemplate
    }

    /// <summary>
    /// Modus des Editors setzen
    /// </summary>
    /// <param name="mode"></param>
    public void SetMode(eEditorMode mode)
    {
        if (_mode == mode) return;

        _mode = mode;

        if (_mode == eEditorMode.Undefined) return;

        if (mode == eEditorMode.RichText)
        {
            crSplitContainer1.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel1;
            lblHeaderMaster.Visible = false;
            lblHeaderDetail.Visible = false;
            richEditControl1.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.PrintLayout;
            richEditControl1.Unit = DevExpress.Office.DocumentUnit.Millimeter;
            richEditBarController1.Control = richEditControl1;
            richEditControl1.Focus();
        }
        else if (mode == eEditorMode.Email)
        {
            crSplitContainer1.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel1;
            richEditControl1.LayoutUnit = DevExpress.XtraRichEdit.DocumentLayoutUnit.Pixel;
            richEditControl1.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Simple;
            richEditBarController1.Control = richEditControl1;
            lblHeaderMaster.Visible = false;
            lblHeaderDetail.Visible = false;
            richEditControl1.Focus();
        }
        else if (mode == eEditorMode.HtmlTemplate)
        {
            crSplitContainer1.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Both;
            richEditControl1.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Simple;
            richEditControl2.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Simple;
            richEditControl1.LayoutUnit = DevExpress.XtraRichEdit.DocumentLayoutUnit.Pixel;
            richEditControl2.LayoutUnit = DevExpress.XtraRichEdit.DocumentLayoutUnit.Pixel;
            lblHeaderMaster.Visible = true;
            lblHeaderDetail.Visible = true;
            richEditControl1.Focus();
        }
    }
    
    /// <summary>
    /// Symbole ersetzen
    /// </summary>
    private void replaceGlyphs()
    {
        checkFormatCopy.ImageOptions.SvgImageSize = new(16, 16);
        checkFormatCopy.ImageOptions.SvgImage = UI.GetImage(Symbol.PaintBrush);

        pasteItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.ClipboardPaste);
        copyItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.Copy);
        cutItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.Cut);
        undoItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.ArrowUndo);
        redoItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.ArrowRedo);
        increaseIndentItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.FontIncrease);
        decreaseIndentItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.FontDecrease);
        toggleFontBoldItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextBold);
        toggleFontItalicItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextItalic);
        toggleFontUnderlineItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextUnderline);
        //toggleFontStrikeoutItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextStrikethroughS);
        toggleFontSuperscriptItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextSuperscript);
        toggleFontSubscriptItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextSubscript);

        clearFormattingItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.ClearFormatting);
        toggleBulletedListItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextBulletListLtr);
        toggleNumberingListItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextNumberListLtr);
        decreaseIndentItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextIndentDecreaseLtr);
        increaseIndentItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextIndentIncreaseLtr);
        toggleParagraphAlignmentLeftItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextAlignLeft);
        toggleParagraphAlignmentCenterItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextAlignCenter);
        toggleParagraphAlignmentRightItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextAlignRight);

        insertPageBreakItem21.ImageOptions.SvgImage = UI.GetImage(Symbol.DocumentPageBreak);
        insertTableItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.Table);
        insertTextBoxItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.Textbox);
        insertFloatingPictureItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.Image);
        insertHyperlinkItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.Link);
        insertSymbolItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.Pi);

        setLandscapePageOrientationItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.DocumentLandscape);
        setPortraitPageOrientationItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.Document);
        changeSectionPageMarginsItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.DocumentMargins);

        editPageHeaderItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.DocumentHeader);
        editPageFooterItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.DocumentFooter);
        editPageFooterItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.DocumentFooter);

        setSectionOneColumnItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextColumnOne);
        setSectionTwoColumnsItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextColumnTwo);
        setSectionThreeColumnsItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextColumnThree);

        //showEditStyleFormItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextEditStyle);
        //showFontFormItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextFont);
        toggleMultiLevelListItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextBulletListTree);
        replaceItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.ArrowSwap);
        printPreviewItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.DocumentSearch);

        //changeTableCellsHorizontalTextDirectionItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextDire);

        findItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.Search);
    }

    /// <summary>
    /// Zugriff auf die Liste der Variablen
    /// </summary>
    public BindingList<Variable> Variables => variableBrowser.Variables;

    /// <summary>
    /// Datenquelle des Dokuments
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public object? DataSource { 
        get => dataSource;
        set
        {
            dataSource = value;

            if (value is IModel)
            {
                var dict = value.GetType().GetTypeDescription().GetAsDictionary(null, true, 1, 2, "", ""); ;
                datasourceBrowser.Fields = dict.Values.ToList();

                richEditControl1.Document.Variables.Clear();

                foreach (var fld in dict.Values)
                    richEditControl1.Document.Variables.Add(fld.FieldName, fld.CurrentValue);

                richEditControl1.Options.MailMerge.Reset();
            }
            else
            {
                richEditControl1.Options.MailMerge.Reset();
                richEditControl1.Options.MailMerge.DataSource = new[] { dataSource };
                richEditControl1.Options.MailMerge.ViewMergedData = true;
            } 
        }
    }

    private void checkFormatCopy_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
        (richEditBarController1.Control as AFEditRichText)?.ToggleFormatCopy(checkFormatCopy.Checked);
    }
}

