using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;

namespace AF.WINFORMS.DX;

/// <summary>
/// Einfacher RichTextEditor mit den wichtigsten Funktionen (Toolbar)
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[DefaultBindingProperty("RichText")]
[ToolboxTabName("AF Common")]
public partial class AFRichEditSimple : AFUserControl
{
    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFRichEditSimple()
    {
        InitializeComponent();

        if (UI.DesignMode) return;

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
        toggleFontStrikeoutItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextStrikethroughS);
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

        findItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.Search);
    }


    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        loadDefaultStyles();
    }

    /// <summary>
    /// RichText im Editor
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string RichText { get => richEditControl1.RtfText; set => richEditControl1.RtfText = value; }

    /// <summary>
    /// Zugriff auf das eigentliche RichEditControl (nur zur Laufzeit)
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public RichEditControl RichEditControl => richEditControl1;

    /// <summary>
    /// Zugriff auf die BarAndDockingController (nur zur Laufzeit)
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public BarAndDockingController BarController => barController;

    /// <summary>
    /// Zugriff auf den BarManager (nur zur Laufzeit)
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public BarManager BarManager => barManager;


    /// <summary>
    /// Setzt den Modus des Editors
    /// </summary>
    /// <param name="mode">Modus</param>
    public void SetMode(eTextMode mode)
    {
        if (mode == eTextMode.Read && standaloneBarDockControl1.Visible == false)
            return;

        if (mode == eTextMode.Write && standaloneBarDockControl1.Visible)
            return;

        standaloneBarDockControl1.Visible = mode == eTextMode.Write;
        richEditControl1.ReadOnly = mode == eTextMode.Read;
        richEditControl1.Options.Behavior.ShowPopupMenu = mode == eTextMode.Write ? DocumentCapability.Enabled : DocumentCapability.Disabled;
    }

    private void richEditControl1_EmptyDocumentCreated(object sender, EventArgs e)
    {
        loadDefaultStyles();
    }

    private void richEditControl1_DocumentLoaded(object sender, EventArgs e)
    {
        loadDefaultStyles();
    }

    private void loadDefaultStyles()
    {
        richEditControl1.Document.Unit = DocumentUnit.Millimeter;

        if (richEditControl1.Document.ParagraphStyles["Normal"] == null)
        {
            var style = richEditControl1.Document.ParagraphStyles.CreateNew();
            style.Name = "Normal";
            //style.LineSpacingType = ParagraphLineSpacing.Sesquialteral;
            richEditControl1.Document.ParagraphStyles.Add(style);
        }

        if (richEditControl1.Document.ParagraphStyles["Heading 1"] == null)
        {
            var style = richEditControl1.Document.ParagraphStyles.CreateNew();
            style.Name = "Heading 1";
            style.FontSize = 18;
            style.ForeColor = Color.FromArgb(255, 11, 48, 65);
            style.LineSpacing = 0;
            style.LineSpacingMultiplier = 0;
            style.LineSpacingType = ParagraphLineSpacing.Single;
            style.SpacingAfter = 0.7055556f;
            style.SpacingBefore = 7.055555f;
            style.OutlineLevel = 1;
            style.NextStyle = richEditControl1.Document.ParagraphStyles["Normal"];
            richEditControl1.Document.ParagraphStyles.Add(style);
        }

        if (richEditControl1.Document.ParagraphStyles["Heading 2"] == null)
        {
            var style = richEditControl1.Document.ParagraphStyles.CreateNew();
            style.Name = "Heading 2";
            style.FontSize = 16;
            style.ForeColor = Color.FromArgb(255, 16, 72, 97);
            style.LineSpacing = 0;
            style.LineSpacingMultiplier = 0;
            style.LineSpacingType = ParagraphLineSpacing.Single;
            style.SpacingAfter = 0;
            style.SpacingBefore = 0.7055556f;
            style.OutlineLevel = 2;
            style.NextStyle = richEditControl1.Document.ParagraphStyles["Normal"];
            richEditControl1.Document.ParagraphStyles.Add(style);
        }

        if (richEditControl1.Document.ParagraphStyles["Heading 3"] == null)
        {
            var style = richEditControl1.Document.ParagraphStyles.CreateNew();
            style.Name = "Heading 3";
            style.FontSize = 14;
            style.ForeColor = Color.FromArgb(255, 16, 72, 97);
            style.LineSpacing = 0;
            style.LineSpacingMultiplier = 0;
            style.LineSpacingType = ParagraphLineSpacing.Single;
            style.SpacingAfter = 0;
            style.SpacingBefore = 0.7055556f;
            style.OutlineLevel = 3;
            style.NextStyle = richEditControl1.Document.ParagraphStyles["Normal"];
            richEditControl1.Document.ParagraphStyles.Add(style);
        }

        if (richEditControl1.Document.ParagraphStyles["Heading 4"] == null)
        {
            var style = richEditControl1.Document.ParagraphStyles.CreateNew();
            style.Name = "Heading 4";
            style.FontSize = 12;
            style.ForeColor = Color.FromArgb(255, 16, 72, 97);
            style.SpacingAfter = 0;
            style.SpacingBefore = 0.7055556f;
            style.OutlineLevel = 4;
            style.NextStyle = richEditControl1.Document.ParagraphStyles["Normal"];
            richEditControl1.Document.ParagraphStyles.Add(style);
        }

        if (richEditControl1.Document.ParagraphStyles["Heading 5"] == null)
        {
            var style = richEditControl1.Document.ParagraphStyles.CreateNew();
            style.Name = "Heading 5";
            style.ForeColor = Color.FromArgb(255, 16, 72, 97);
            style.SpacingAfter = 0;
            style.SpacingBefore = 0.7055556f;
            style.OutlineLevel = 5;
            style.NextStyle = richEditControl1.Document.ParagraphStyles["Normal"];
            richEditControl1.Document.ParagraphStyles.Add(style);
        }

        if (richEditControl1.Document.ParagraphStyles["Heading 6"] == null)
        {
            var style = richEditControl1.Document.ParagraphStyles.CreateNew();
            style.Name = "Heading 6";
            style.Italic = true;
            style.ForeColor = Color.FromArgb(255, 11, 48, 65);
            style.SpacingAfter = 0;
            style.SpacingBefore = 0.7055556f;
            style.OutlineLevel = 6;
            style.NextStyle = richEditControl1.Document.ParagraphStyles["Normal"];
            richEditControl1.Document.ParagraphStyles.Add(style);
        }

        if (richEditControl1.Document.ParagraphStyles["Title"] == null)
        {
            var style = richEditControl1.Document.ParagraphStyles.CreateNew();
            style.Name = "Title";
            style.FontSize = 36;
            style.ForeColor = Color.FromArgb(255, 14, 40, 65);
            style.LineSpacing = 0.01499306f;
            style.LineSpacingMultiplier = 0.85f;
            style.LineSpacingType = ParagraphLineSpacing.Multiple;
            style.Spacing = -15;
            style.SpacingAfter = 0;
            style.ContextualSpacing = true;
            style.NextStyle = richEditControl1.Document.ParagraphStyles["Normal"];
            richEditControl1.Document.ParagraphStyles.Add(style);
        }

        if (richEditControl1.Document.ParagraphStyles["Subtitle"] == null)
        {
            var style = richEditControl1.Document.ParagraphStyles.CreateNew();
            style.Name = "Subtitle";
            style.FontSize = 14;
            style.ForeColor = Color.FromArgb(255, 21, 96, 130);
            style.LineSpacing = 0;
            style.LineSpacingMultiplier = 0;
            style.LineSpacingType = ParagraphLineSpacing.Single;
            style.SpacingAfter = 4.233333f;
            style.NextStyle = richEditControl1.Document.ParagraphStyles["Normal"];
            richEditControl1.Document.ParagraphStyles.Add(style);
        }

        if (richEditControl1.Document.ParagraphStyles["Quote"] == null)
        {
            var style = richEditControl1.Document.ParagraphStyles.CreateNew();
            style.Name = "Quote";
            style.FontSize = 12;
            style.ForeColor = Color.FromArgb(255, 14, 40, 65);
            style.LeftIndent = 12.7f;
            style.SpacingAfter = 2.116667f;
            style.SpacingBefore = 2.116667f;
            richEditControl1.Document.ParagraphStyles.Add(style);
        }

        if (richEditControl1.Document.ParagraphStyles["Intense Quote"] == null)
        {
            var style = richEditControl1.Document.ParagraphStyles.CreateNew();
            style.Name = "Intense Quote";
            style.FontSize = 16;
            style.ForeColor = Color.FromArgb(255, 14, 40, 65);
            style.Alignment = ParagraphAlignment.Center;
            style.LeftIndent = 12.7f;
            style.LineSpacing = 0;
            style.LineSpacingMultiplier = 0;
            style.LineSpacingType = ParagraphLineSpacing.Single;
            style.Spacing = -6;
            style.SpacingAfter = 4.233333f;
            style.SpacingBefore = 1.763889f;
            richEditControl1.Document.ParagraphStyles.Add(style);
        }

        if (richEditControl1.Document.ParagraphStyles["Code"] == null)
        {
            var style = richEditControl1.Document.ParagraphStyles.CreateNew();
            style.Name = "Code";
            style.FontSize = 9;
            style.FontName = "Lucida Console";
            style.LeftIndent = 7.496528f;
            style.LineSpacing = 0;
            style.LineSpacingMultiplier = 0;
            style.LineSpacingType = ParagraphLineSpacing.Single;
            style.SpacingAfter = 4.233333f;
            style.SpacingBefore = 4.33333f;
            style.ContextualSpacing = true;
            richEditControl1.Document.ParagraphStyles.Add(style);
        }
    }
}

/// <summary>
/// Erweiterungen für Paragraf-Styles
/// </summary>
public static class ParagraphStyleEx
{
    /// <summary>
    /// Style kopieren
    /// </summary>
    /// <param name="source">Quelle</param>
    /// <param name="target">Ziel</param>
    public static void CopyStyles(this ParagraphStyle source, ParagraphStyle target)
    {
        target.Name = source.Name;

        target.FontSize = source.FontSize;
        target.FontName = source.FontName;
        target.Bold = source.Bold;
        target.Underline = source.Underline;
        target.Italic = source.Italic;
        target.Underline = source.Underline;
        target.ForeColor = source.ForeColor;

        target.Alignment = source.Alignment;
        target.LeftIndent = source.LeftIndent;
        target.RightIndent = source.RightIndent;
        target.FirstLineIndent = source.FirstLineIndent;

        target.LineSpacing = source.LineSpacing;
        target.LineSpacingMultiplier = source.LineSpacingMultiplier;
        target.LineSpacingType = (source.LineSpacingType == ParagraphLineSpacing.Exactly ? ParagraphLineSpacing.Single : source.LineSpacingType);

        target.Spacing = source.Spacing;
        target.SpacingAfter = source.SpacingAfter;
        target.SpacingBefore = source.SpacingBefore;
        target.ContextualSpacing = source.ContextualSpacing;

        target.OutlineLevel = source.OutlineLevel;

        (target as ParagraphPropertiesBase).BackColor = (source as ParagraphPropertiesBase).BackColor;
    }
}