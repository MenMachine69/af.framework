using DevExpress.LookAndFeel;
using DevExpress.Utils;

namespace AF.WINFORMS.DX;

/// <summary>
/// Fenster zur Anzeige standardisierter Meldungen.
/// Dieses Fenster ersetzt die MessageBox des Systems.
/// </summary>
[SupportedOSPlatform("windows")]
public partial class _formMessageBox : DevExpress.XtraEditors.XtraForm
{
    private readonly string _MoreMessage = "";
    private bool _ShowMore;
    private readonly Control? _plugin;
    private readonly Color _msgtypeColor = Color.Empty;

    /// <summary>
    /// Constructor
    /// </summary>
    public _formMessageBox()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="sMessage">Die Nachricht, 1. Zeile=Titel</param>
    /// <param name="sMoreMessage">weitere Informationen</param>
    /// <param name="sCaption">Fenstertitel</param>
    /// <param name="iButtons">anzuzeigende Buttons</param>
    /// <param name="iIcon">anzuzeigendes Icon</param>
    /// <param name="iDefault">vorgegebene Antwort</param>
    /// <param name="bAllowDisable">'Diese Nachricht nicht mehr anzeigen' ist sichtbar</param>
    /// <param name="plugin">Plugin, dass im Dialog angezeigt werden soll</param>
    public _formMessageBox(Control? plugin, string sMessage, string sMoreMessage, string sCaption,
        eMessageBoxButton iButtons, eMessageBoxIcon iIcon, eMessageBoxDefaultButton iDefault, bool bAllowDisable)
    {
        InitializeComponent();

        _plugin = plugin;
        KeyPreview = true;

        TopLevel = true;
        TopMost = true;
        //SaveFormState = false;

        if (!bAllowDisable)
        {
            crTablePanel1.Rows[1].Visible = false;
            chkDoNotShowAgain.Hide();

            //panelButtons.Size = new Size(panelButtons.Width, panelButtons.Height - 26);
        }

        _MoreMessage = sMoreMessage;
        pshShowMore.ImageOptions.AllowGlyphSkinning = DefaultBoolean.True;
        pshShowMore.ImageOptions.SvgImage = Glyphs.GetImage(Symbol.ArrowCircleDown);

        if (_MoreMessage == string.Empty) pshShowMore.Hide();

        base.Text = sCaption;
        // Nachricht splitten. Die erste Zeile ist der Titel.
        string[] aMessageLines = sMessage.Split([Environment.NewLine], StringSplitOptions.None);

        lblMessageTitel.Text = aMessageLines[0];

        MessageText = "";
        for (int i = 1; i < aMessageLines.Length; ++i) MessageText += aMessageLines[i] + Environment.NewLine;

        // die passenden Buttons anzeigen und den Default-Button wählen
        switch (iButtons)
        {
            case eMessageBoxButton.OK:
                psh1.Hide();
                psh2.Hide();
                psh3.Text = WinFormsStrings.BTN_OK;
                psh3.DialogResult = DialogResult.OK;
                psh3.Appearance.BackColor = DXSkinColors.FillColors.Success;
                psh3.Appearance.Options.UseBackColor = true;
                AcceptButton = psh3;
                break;
            case eMessageBoxButton.OKCancel:
                psh1.Hide();
                psh2.Text = WinFormsStrings.BTN_OK;
                psh2.DialogResult = DialogResult.OK;
                psh2.Appearance.BackColor = DXSkinColors.FillColors.Success;
                psh2.Appearance.Options.UseBackColor = true;
                psh3.Text = WinFormsStrings.BTN_CANCEL;
                psh3.DialogResult = DialogResult.Cancel;
                psh3.Appearance.BackColor = DXSkinColors.FillColors.Danger;
                psh3.Appearance.Options.UseBackColor = true;
                AcceptButton = iDefault switch
                {
                    eMessageBoxDefaultButton.Button1 => psh2,
                    eMessageBoxDefaultButton.Button2 => psh3,
                    _ => psh2
                };

                break;
            case eMessageBoxButton.YesNo:
                psh1.Hide();
                psh2.Text = WinFormsStrings.BTN_YES;
                psh2.DialogResult = DialogResult.Yes;
                psh2.Appearance.BackColor = DXSkinColors.FillColors.Success;
                psh2.Appearance.Options.UseBackColor = true;
                psh3.Text = WinFormsStrings.BTN_NO;
                psh3.DialogResult = DialogResult.No;
                psh3.Appearance.BackColor = DXSkinColors.FillColors.Danger;
                psh3.Appearance.Options.UseBackColor = true;
                AcceptButton = iDefault switch
                {
                    eMessageBoxDefaultButton.Button1 => psh2,
                    eMessageBoxDefaultButton.Button2 => psh3,
                    _ => psh2
                };

                break;
            case eMessageBoxButton.RetryCancel:
                psh1.Hide();
                psh2.Text = WinFormsStrings.BTN_RETRY;
                psh2.DialogResult = DialogResult.Retry;
                psh2.Appearance.BackColor = DXSkinColors.FillColors.Question;
                psh2.Appearance.Options.UseBackColor = true;
                psh3.Text = WinFormsStrings.BTN_CANCEL;
                psh3.DialogResult = DialogResult.Cancel;
                psh3.DialogResult = DialogResult.No;
                psh3.Appearance.BackColor = DXSkinColors.FillColors.Danger;
                AcceptButton = iDefault switch
                {
                    eMessageBoxDefaultButton.Button1 => psh2,
                    eMessageBoxDefaultButton.Button2 => psh3,
                    _ => psh2
                };

                break;
            case eMessageBoxButton.YesNoCancel:
                psh1.Text = WinFormsStrings.BTN_YES;
                psh1.DialogResult = DialogResult.Yes;
                psh1.Appearance.BackColor = DXSkinColors.FillColors.Success;
                psh1.Appearance.Options.UseBackColor = true;
                psh2.Text = WinFormsStrings.BTN_NO;
                psh2.DialogResult = DialogResult.No;
                psh2.Appearance.BackColor = DXSkinColors.FillColors.Danger;
                psh2.Appearance.Options.UseBackColor = true;
                psh3.Text = WinFormsStrings.BTN_CANCEL;
                psh3.DialogResult = DialogResult.Cancel;
                psh3.Appearance.BackColor = DXSkinColors.FillColors.Warning;
                psh3.Appearance.Options.UseBackColor = true;
                AcceptButton = iDefault switch
                {
                    eMessageBoxDefaultButton.Button1 => psh1,
                    eMessageBoxDefaultButton.Button2 => psh2,
                    eMessageBoxDefaultButton.Button3 => psh3,
                    _ => psh1
                };

                break;
            case eMessageBoxButton.AbortRetryIgnore:
                psh1.Text = WinFormsStrings.BTN_CANCEL;
                psh1.DialogResult = DialogResult.Cancel;
                psh1.Appearance.BackColor = DXSkinColors.FillColors.Danger;
                psh1.Appearance.Options.UseBackColor = true;
                psh2.Text = WinFormsStrings.BTN_RETRY;
                psh2.DialogResult = DialogResult.Retry;
                psh2.Appearance.BackColor = DXSkinColors.FillColors.Question;
                psh2.Appearance.Options.UseBackColor = true;
                psh3.Text = WinFormsStrings.BTN_IGNORE;
                psh3.DialogResult = DialogResult.Ignore;
                psh3.Appearance.BackColor = DXSkinColors.FillColors.Warning;
                psh3.Appearance.Options.UseBackColor = true;
                AcceptButton = iDefault switch
                {
                    eMessageBoxDefaultButton.Button1 => psh1,
                    eMessageBoxDefaultButton.Button2 => psh2,
                    eMessageBoxDefaultButton.Button3 => psh3,
                    _ => psh1
                };

                break;
            default:
                throw new Exception(@"Unknown specification for MessageBoxButtons.");
        }

        picIcon.ImageOptions.SvgImageSize = new Size(32, 32);

        // das passende Icon anzeigen
        picIcon.ImageOptions.SvgImage = iIcon switch
        {
            eMessageBoxIcon.Asterisk => Glyphs.GetImage(Symbol.Info),
            eMessageBoxIcon.Error => Glyphs.GetImage(Symbol.ErrorCircle),
            eMessageBoxIcon.Exclamation => Glyphs.GetImage(Symbol.Warning),
            eMessageBoxIcon.Question => Glyphs.GetImage(Symbol.QuestionCircle),
            _ => Glyphs.GetImage(Symbol.Info)
        };

        _msgtypeColor = iIcon switch
        {
            eMessageBoxIcon.Asterisk => UI.GetSkinPaletteColor(@"Green"),
            eMessageBoxIcon.Error => UI.GetSkinPaletteColor(@"Red"),
            eMessageBoxIcon.Exclamation => UI.GetSkinPaletteColor(@"Yellow"),
            _ => UI.GetSkinPaletteColor(@"Blue")
        };

        if (_plugin == null) return;

        _plugin.Dock = DockStyle.Top;
        panelPlugin.Size = new Size(panelPlugin.Width, _plugin.Height + UI.GetScaled(6));
        panelPlugin.Controls.Add(_plugin);
        panelPlugin.Visible = true;

    }

    private void pshShowMore_Click(object sender, EventArgs e)
    {
        pshShowMore.Text = WinFormsStrings.LBL_SHOWMOREINFO;

        if (_ShowMore)
        {
            pshShowMore.ImageOptions.SvgImage = Glyphs.GetImage(Symbol.ArrowCircleDown);
            _ShowMore = false;
        }
        else
        {
            pshShowMore.ImageOptions.SvgImage = Glyphs.GetImage(Symbol.ArrowCircleUp);
            _ShowMore = true;
        }

        _ShowMessageText();
    }

    private void _ShowMessageText()
    {
        // Höhe der Nachricht ermitteln und danach die Höhe des Dialogs berechnen
        int iHeight;

        iHeight = Padding.Top;
        iHeight += lblMessageTitel.Height;

        using (Graphics g = Graphics.FromHwnd(Handle))
        {
            int textHeight = g.GetStringHeight(MessageText, lblMessageText.Font, lblMessageText.Size.Width) + UI.GetScaled(10);

            if (textHeight < UI.GetScaled(30))
                textHeight = UI.GetScaled(30);

            iHeight += textHeight;
        }

        if (_MoreMessage != string.Empty && _ShowMore)
            iHeight += UI.GetScaled(90);

        iHeight += (_plugin != null ? panelPlugin.Height : 0);
        iHeight += crTablePanel1.Height + 10;
        iHeight += Padding.Bottom;

        Size = new Size(Size.Width, iHeight);

        if (_MoreMessage != string.Empty && _ShowMore)
        {
            editMultiline1.Visible = true;
            editMultiline1.Text = _MoreMessage;
        }
        else
            editMultiline1.Visible = false;

        lblMessageText.Text = MessageText;
    }

    /// <summary>
    /// Meldung anzeigen und auf Antwort warten
    /// </summary>
    /// <param name="owner">Parent-Fenster (optional: Standared ist null)</param>
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
        eMessageBoxResult result = DialogResult switch
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

        if (chkDoNotShowAgain.Visible && chkDoNotShowAgain.Checked && MessageID != 0)
            ((AFWinFormsDXApp)AFCore.App).DefaultMsgAnswerStorage.SetResult(MessageID, result);

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

        if (m.Msg == 0xA3) // WM_NCLBUTTONDBLCLK
            return;

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
