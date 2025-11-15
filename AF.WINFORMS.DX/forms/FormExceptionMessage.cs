using DevExpress.LookAndFeel;
using DevExpress.Utils;

namespace AF.WINFORMS.DX;

/// <summary>
/// Fenster zur Anzeige standardisierter Meldungen.
/// Dieses Fenster ersetzt die MessageBox des Systems.
/// </summary>
[SupportedOSPlatform("windows")]
public partial class FormExceptionMessage : DevExpress.XtraEditors.XtraForm
{
    private readonly Color _msgtypeColor = Color.Empty;

    /// <summary>
    /// Constructor
    /// </summary>
    public FormExceptionMessage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="exception">Die Nachricht, 1. Zeile=Titel</param>
    public FormExceptionMessage(ExceptionInfo exception)
    {
        InitializeComponent();

        Exception showException = exception.Exception!;

        while (showException.InnerException != null)
            showException = showException.InnerException;

        KeyPreview = true;

        TopLevel = true;
        TopMost = true;

        pshShowMore.ImageOptions.AllowGlyphSkinning = DefaultBoolean.True;
        pshShowMore.ImageOptions.SvgImage = UI.GetImage(Symbol.Search);

        base.Text = "UNBEHANDELTE AUSNAHME";
        // Nachricht splitten. Die erste Zeile ist der Titel.
        string[] aMessageLines = ("UNBEHANDELTE AUSNAHME\r\n"+ showException.Message).Split([Environment.NewLine], StringSplitOptions.None);

        lblMessageTitel.Text = aMessageLines[0];

        MessageText = "";
        for (int i = 1; i < aMessageLines.Length; ++i) MessageText += aMessageLines[i] + Environment.NewLine;

        // die passenden Buttons anzeigen und den Default-Button wählen
        psh1.Hide();
        psh2.Hide();
        psh3.Text = WinFormsStrings.BTN_OK;
        psh3.DialogResult = DialogResult.OK;
        psh3.Appearance.BackColor = DXSkinColors.FillColors.Success;
        psh3.Appearance.Options.UseBackColor = true;
        AcceptButton = psh3;

        pshShowMore.Click += (_, _) =>
        {
            using var dlg = new FormExceptionExplorer(exception);
            Hide();
            dlg.ShowDialog();
            Show();
        };

        picIcon.ImageOptions.SvgImageSize = new Size(32, 32);

        // das passende Icon anzeigen
        picIcon.ImageOptions.SvgImage = Glyphs.GetImage(Symbol.ErrorCircle);

        _msgtypeColor = UI.GetSkinPaletteColor(@"Red");

        _ShowMessageText();
    }

    private void _ShowMessageText()
    {
        // Höhe der Nachricht ermitteln und danach die Höhe des Dialogs berechnen

        var iHeight = Padding.Top;
        iHeight += lblMessageTitel.Height;

        using (Graphics g = Graphics.FromHwnd(Handle))
        {
            int textHeight = g.GetStringHeight(MessageText, lblMessageText.Font, lblMessageText.Size.Width) + UI.GetScaled(10);

            if (textHeight < UI.GetScaled(30))
                textHeight = UI.GetScaled(30);

            iHeight += textHeight;
        }

        iHeight += panelPlugin.Height;
        iHeight += crTablePanel1.Height + 10;
        iHeight += Padding.Bottom;

        Size = new Size(Size.Width, iHeight);

        lblMessageText.Text = MessageText;
    }

    /// <summary>
    /// Meldung anzeigen und auf Antwort warten
    /// </summary>
    /// <param name="owner">Parent-Fenster (optional: Standard ist null)</param>
    /// <returns>Antwort des Benutzers (DialogResult)</returns>
    public new eMessageBoxResult ShowDialog(IWin32Window? owner = null)
    {
        owner ??= FindForm();

        if (owner != null && owner.Handle.Equals(Handle))
            owner = null;

        if (owner != null) return toResult(base.ShowDialog(owner));

        StartPosition = FormStartPosition.CenterScreen;
        return toResult(base.ShowDialog());

    }

    private eMessageBoxResult toResult(DialogResult result)
    {
        return result switch
        {
            DialogResult.OK => eMessageBoxResult.OK,
            DialogResult.Yes => eMessageBoxResult.Yes,
            DialogResult.No => eMessageBoxResult.No,
            DialogResult.Cancel => eMessageBoxResult.Cancel,
            DialogResult.Abort => eMessageBoxResult.Abort,
            DialogResult.Retry => eMessageBoxResult.Retry,
            DialogResult.Ignore => eMessageBoxResult.Ignore,
#if NET481_OR_GREATER
#else
            DialogResult.Continue => eMessageBoxResult.Continue,
            DialogResult.TryAgain => eMessageBoxResult.TryAgain,
#endif
            _ => eMessageBoxResult.None
        };
    }

    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs e)
    {
        // Strg+C kopiert Meldung in die Zwischenablage
        if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control) 
        {
            Clipboard.Clear();
            Clipboard.SetText(lblMessageTitel.Text + Environment.NewLine + lblMessageText.Text);
        }

        base.OnKeyDown(e);
    }

    /// <inheritdoc />
    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        Text = Text.ToUpper();
        _ShowMessageText();
    }

    /// <inheritdoc />
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _ = DialogResult switch
        {
            DialogResult.OK => eMessageBoxResult.OK,
            DialogResult.Yes => eMessageBoxResult.Yes,
            DialogResult.Cancel => eMessageBoxResult.Cancel,
            DialogResult.Abort => eMessageBoxResult.Ignore,
            DialogResult.Retry => eMessageBoxResult.Retry,
            DialogResult.Ignore => eMessageBoxResult.Ignore,
            DialogResult.None => eMessageBoxResult.None,
            DialogResult.No => eMessageBoxResult.No,
#if NET481_OR_GREATER
#else
            DialogResult.Continue => eMessageBoxResult.Continue,
            DialogResult.TryAgain => eMessageBoxResult.TryAgain,
#endif
            _ => throw new Exception($"Unknown DialogResult {DialogResult}.")
        };


        base.OnFormClosing(e);
    }

    /// <summary>
    /// Programmweite eindeutige ID dieser Meldung. 
    /// Wird eine ID angegeben, kann die Antwort persistent gespeichert 
    /// werden, sodass der Benutzer beim nächsten Mal nicht mehr gefragt wird.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int MessageID { get; set; }

    /// <summary>
    /// Text der als Meldung angezeigt werden soll
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string MessageText { get; set; } = "";

    /// <inheritdoc />
    protected override void WndProc(ref Message m)
    {
        var handled = false;

        if (m.Msg == 0x84)
        {
            // ReSharper disable once RedundantCast
            m.Result = (IntPtr)2; // HTCAPTION
            handled = true;
        }

        if (handled)
            return;

        base.WndProc(ref m);
    }

    /// <inheritdoc />
    protected override void OnPaintBackground(PaintEventArgs e)
    {
        base.OnPaintBackground(e);

        using var brush = new SolidBrush(_msgtypeColor);
        e.Graphics.FillRectangle(brush, new Rectangle(0, 0, 10, Height));
    }
}
