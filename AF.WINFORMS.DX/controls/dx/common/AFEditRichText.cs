using DevExpress.Portable.Input;
using DevExpress.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Mouse;
using DevExpress.XtraRichEdit.Utils;

namespace AF.WINFORMS.DX;

/// <inheritdoc/>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
[DefaultBindingProperty("RtfText")]
public class AFEditRichText : RichEditControl
{
    private DocumentRange? sourceSelectedRange;
    private WeakEvent<EventHandler<EventArgs>>? formatCopyFinished;

    /// <summary>
    /// Ereignisdefinition.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public event EventHandler<EventArgs> FormatCopyFinished
    {
        add
        {
            formatCopyFinished ??= new();
            formatCopyFinished.Add(value);
        }
        remove => formatCopyFinished?.Remove(value);
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public AFEditRichText()
    {
        MouseUp += (_, _) =>
        {
            if (FormatCalculatorEnabled && sourceSelectedRange != null)
            {
                applyFormatToSelectedText();
                FormatCalculatorEnabled = false;
                sourceSelectedRange = null;

                formatCopyFinished?.Raise(this, EventArgs.Empty);
            }
        };
    }

    private void saveSelectedRange()
    {
        DocumentRange? savedRange = null;

        if (Document.Selection.Length < 1)
        {
            savedRange = Document.Selection;
            Document.Selection = Document.CreateRange(Document.Selection.Start, Document.Selection.Length + 1);
        }

        DocumentRange selection = Document.Selection;
        SubDocument subDocument = selection.BeginUpdateDocument();
        sourceSelectedRange = subDocument.CreateRange(selection.Start, Document.Selection.Length);
        selection.EndUpdateDocument(subDocument);

        if (savedRange != null)
            Document.Selection = savedRange;
    }

    /// <summary>
    /// Format kopieren auslösen oder abbrechen.
    /// </summary>
    /// <param name="on">true = auslösen, false = abbrechen</param>
    public void ToggleFormatCopy(bool on)
    {
        if (on)
        {
            saveSelectedRange();
            FormatCalculatorEnabled = true;
        }
        else
        {
            FormatCalculatorEnabled = false;
            sourceSelectedRange = null;
            formatCopyFinished?.Raise(this, EventArgs.Empty);
        }
    }

    private void applyFormatToSelectedText()
    {
        if (sourceSelectedRange == null) return;

        DocumentRange targetSelectedRange = Document.Selection;

        BeginUpdate();
        SubDocument targetSubDocument = targetSelectedRange.BeginUpdateDocument();
        SubDocument subDocument = sourceSelectedRange.BeginUpdateDocument();

        CharacterProperties targetCharactersProperties = targetSubDocument.BeginUpdateCharacters(targetSelectedRange);
        CharacterProperties sourceCharactersProperties = subDocument.BeginUpdateCharacters(sourceSelectedRange);
        targetCharactersProperties.AllCaps = sourceCharactersProperties.AllCaps;
        targetCharactersProperties.BackColor = sourceCharactersProperties.BackColor;
        targetCharactersProperties.ForeColor = sourceCharactersProperties.ForeColor;
        targetCharactersProperties.Bold = sourceCharactersProperties.Bold;
        targetCharactersProperties.FontName = sourceCharactersProperties.FontName;
        targetCharactersProperties.FontSize = sourceCharactersProperties.FontSize;
        targetCharactersProperties.HighlightColor = sourceCharactersProperties.HighlightColor;
        targetCharactersProperties.Italic = sourceCharactersProperties.Italic;
        targetCharactersProperties.SmallCaps = sourceCharactersProperties.SmallCaps;
        targetCharactersProperties.Spacing = sourceCharactersProperties.Spacing;
        targetCharactersProperties.Strikeout = sourceCharactersProperties.Strikeout;
        targetCharactersProperties.Subscript = sourceCharactersProperties.Subscript;
        targetCharactersProperties.Superscript = sourceCharactersProperties.Superscript;
        targetCharactersProperties.Underline = sourceCharactersProperties.Underline;
        targetCharactersProperties.UnderlineColor = sourceCharactersProperties.UnderlineColor;
        //targetCharactersProperties.Assign(sourceCharactersProperties);
        subDocument.EndUpdateCharacters(sourceCharactersProperties);
        targetSubDocument.EndUpdateCharacters(targetCharactersProperties);

        //DevExpress.XtraRichEdit.API.Native.ParagraphProperties targetParagraphProperties = targetSubDocument.BeginUpdateParagraphs(targetSelectedRange);
        //DevExpress.XtraRichEdit.API.Native.ParagraphProperties sourceParagraphProperties = subDocument.BeginUpdateParagraphs(sourceSelectedRange);
        //targetParagraphProperties.Assign(sourceParagraphProperties);
        //subDocument.EndUpdateParagraphs(sourceParagraphProperties);
        //targetSubDocument.EndUpdateParagraphs(targetParagraphProperties);

        sourceSelectedRange.EndUpdateDocument(subDocument);
        targetSelectedRange.EndUpdateDocument(targetSubDocument);
        EndUpdate();
    }

    /// <inheritdoc />
    protected override InnerRichEditControl CreateInnerControl() { return new AFInnerRichEditControl(this); }

    /// <summary>
    /// Format kopieren läuft...
    /// </summary>
    [DefaultValue(false)]
    public bool FormatCalculatorEnabled { get; set; }
}

/// <summary>
/// Wird von AFEditRichText benötigt. NICHT DIREKT VERWENDEN!
/// </summary>
[ToolboxItem(false)]
public class AFInnerRichEditControl : InnerRichEditControl
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="owner"></param>
    public AFInnerRichEditControl(IInnerRichEditControlOwner owner) : base(owner) { }

    /// <inheritdoc />
    protected override MouseCursorCalculator CreateMouseCursorCalculator() { return new AFMouseCursorCalculator(ActiveView); }
}

/// <summary>
/// Wird von AFEditRichText benötigt. NICHT DIREKT VERWENDEN!
/// </summary>
[ToolboxItem(false)]
public class AFMouseCursorCalculator : MouseCursorCalculator
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="view"></param>
    public AFMouseCursorCalculator(RichEditView view) : base(view) { }

    /// <inheritdoc />
    public override IPortableCursor Calculate(RichEditHitTestResultCore hitTestResult, Point physicalPoint)
    {
        if ((View.Control as AFEditRichText)!.FormatCalculatorEnabled)
            return RichEditCursors.Hand;

        return base.Calculate(hitTestResult, physicalPoint);
    }
}