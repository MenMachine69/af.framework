using DevExpress.Utils;
using DevExpress.XtraEditors;
using Timer = System.Windows.Forms.Timer;

namespace AF.WINFORMS.DX;

/// <summary>
/// Displaying FlyOuts (message or sidebar) in a Form or Control
/// </summary>
public class AFFlyoutManager : AFVisualComponentBase
{
    private FlyoutPanel? _sidebar;
    private FlyoutPanelControl? _sidebarPanel;

    private FlyoutPanel? _msg;
    private FlyoutPanelControl? _msgPanel;

    private LabelControl? _msgLabel;
    private LabelControl? _msgCloseBtn;
    private Timer? _msgTimer;


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="container">IContainer, der die Komponente aufnimmt</param>
    /// <exception cref="ArgumentNullException">wenn kein IContainer übergeben wurde</exception>
    public AFFlyoutManager(IContainer? container)
    {
        if (container == null)
            throw new ArgumentNullException(nameof(container));

        container.Add(this);
    }

    /// <summary>
    /// ContainerControl for the Flyouts
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ContainerControl? Host { get => ContainerControl; set => ContainerControl = value; }

    /// <summary>
    /// Vertical offset for the message flyout
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int VerticalMessageOffset { get; set; }

    /// <summary>
    /// default message timeout in seconds
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int DefaultMessageTimeout { get; set; } = 5;

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (_msgTimer != null)
        {
            _msgTimer.Enabled = false;
            _msgTimer.Tick -= _flyouttimer_Tick;
            _msgTimer.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Flyout als Sidebar anzeigen.
    /// </summary>
    /// <param name="sidebarControl">anzuzeigende Sidebar (Control)</param>
    public void ShowSidebar(Control sidebarControl)
    {
        if (_sidebar == null)
            _initflyout();

        if (_sidebar != null && _sidebarPanel != null)
        {
            _sidebar.OwnerControl = Host;
            _sidebar.Size = new Size(sidebarControl.Width + _sidebarPanel.Padding.Horizontal, _sidebar.Height);
            sidebarControl.Dock = DockStyle.Fill;
            _sidebarPanel.Controls.Add(sidebarControl);
            _sidebar.MinimumSize = new(_sidebar.Width, 20);

            _sidebar.ShowPopup(true);
        }
    }

    /// <summary>
    /// aktuelle Sidebar schließen.
    /// </summary>
    public void CloseSidebar()
    {
        if (_sidebar != null && _sidebar.IsPopupOpen)
            _sidebar.HidePopup();
    }

    /// <summary>
    /// Status der Sidebar (Sichtbarkeit)
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsSidebarVisible => _sidebar != null && _sidebar.IsPopupOpen;

    /// <summary>
    /// Eine Nachricht anzeigen
    /// </summary>
    /// <param name="args">Nachricht</param>
    public void ShowMessage(MessageArguments args)
    {
        ShowMessage(args.Message, args.Type, args.TimeOut);
    }

    /// <summary>
    /// Eine Nachricht anzeigen
    /// </summary>
    /// <param name="msg">Nachricht</param>
    /// <param name="type">Typ der Nachricht</param>
    /// <param name="seconds">Dauer der Anzeige in Sekunden</param>
    public void ShowMessage(string msg, eNotificationType type, int seconds)
    {
        if (_msgPanel != null && _msgPanel.InvokeRequired)
        {
            _msgPanel.Invoke(() => ShowMessage(msg, type, seconds));
            return;
        }

        if (seconds <= 0)
            seconds = DefaultMessageTimeout;

        if (_msg == null)
            _initflyout();

        if (_msgTimer != null && _msgTimer.Enabled) // currently a flyoutmessage is visible, first hide this...
            _flyoutclose_Click(null, EventArgs.Empty);

        Color backColor = Color.FromArgb(255, DevExpress.LookAndFeel.DXSkinColors.IconColors.Green);
        Color foreColor = Color.FromArgb(255, DevExpress.LookAndFeel.DXSkinColors.IconColors.White);


        if (type == eNotificationType.Warning)
        {
            backColor = Color.FromArgb(255, DevExpress.LookAndFeel.DXSkinColors.IconColors.Yellow);
            foreColor = Color.FromArgb(255, DevExpress.LookAndFeel.DXSkinColors.IconColors.Black);
        }
        else if (type == eNotificationType.Error)
        {
            backColor = Color.FromArgb(255, DevExpress.LookAndFeel.DXSkinColors.IconColors.Red);
            foreColor = Color.FromArgb(255, DevExpress.LookAndFeel.DXSkinColors.IconColors.White);
        }

        if (_msgPanel != null)
        {
            _msgPanel.BackColor = backColor;
            _msgPanel.ForeColor = foreColor;
        }

        if (_msgLabel != null)
        {
            _msgLabel.BackColor = backColor;
            _msgLabel.ForeColor = foreColor;
            _msgLabel.Text = msg;
        }

        if (_msgCloseBtn != null)
        {
            _msgCloseBtn.BackColor = backColor;
            _msgCloseBtn.ForeColor = foreColor;
        }

        if (_msg != null)
        {
            // _msgPanel!.Size = new(_msgPanel.Width, UI.GetScaled(Host, 45));

            _msg.OwnerControl = Host;
            _msg.Options.AnimationType = DevExpress.Utils.Win.PopupToolWindowAnimation.Slide;
            _msg.ShowPopup();
        }

        if (_msgTimer == null)
        {
            _msgTimer = new()
            {
                Interval = seconds * 1000
            };
            _msgTimer.Tick += _flyouttimer_Tick;
        }

        _msgTimer.Start();
    }

    void _flyoutclose_Click(object? sender, EventArgs e)
    {
        if (_msgTimer != null)
            _msgTimer.Enabled = false;

        _msg?.HidePopup();
    }

    private void _initflyout()
    {
        _sidebar = new FlyoutPanel();
        _sidebarPanel = new FlyoutPanelControl();
        _msg = new FlyoutPanel();
        _msgPanel = new FlyoutPanelControl();
        _msgLabel = new LabelControl();
        _msgCloseBtn = new LabelControl();

        // 
        // _msg
        // 
        _msg.Controls.Add(_msgPanel);
        _msg.Margin = new Padding(0);
        _msg.Name = "_msg";
        _msg.Size = new Size(634, ScaleUtils.ScaleValue(45));
        _msg.TabIndex = 0;
        _msg.Hidden += _msg_Hidden;

        // 
        // _msgPanel
        // 
        _msgPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        _msgPanel.Controls.Add(_msgCloseBtn);
        _msgPanel.Controls.Add(_msgLabel);
        _msgPanel.Dock = DockStyle.Fill;
        _msgPanel.FlyoutPanel = _msg;
        _msgPanel.Location = new Point(0, 0);
        _msgPanel.Margin = new Padding(0);
        _msgPanel.Name = "_msgPanel";
        _msgPanel.Size = new Size(634, ScaleUtils.ScaleValue(45));
        _msgPanel.TabIndex = 0;

        // 
        // _sidebar
        // 
        _sidebar.Margin = new Padding(0);
        _sidebar.Name = "_sidebar";
        _sidebar.Size = new Size(200, 45);
        _sidebar.Options.AnchorType = DevExpress.Utils.Win.PopupToolWindowAnchor.Right;
        _sidebar.Options.AnimationType = DevExpress.Utils.Win.PopupToolWindowAnimation.Slide;
        _sidebar.Options.CloseOnHidingOwner = true;
        _sidebar.Options.CloseOnOuterClick = true;
        _sidebar.AutoSize = false;
        _sidebar.TabIndex = 0;
        _sidebar.Hidden += _sidebar_Hidden;
        _sidebar.Controls.Add(_sidebarPanel);

        // 
        // _sidebarPanel
        // 
        _sidebarPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        _sidebarPanel.Dock = DockStyle.Fill;
        _sidebarPanel.FlyoutPanel = _sidebar;
        _sidebarPanel.Location = new Point(0, 0);
        _sidebarPanel.Margin = new Padding(0);
        _sidebarPanel.Name = "_sidebarPanel";
        _sidebarPanel.Size = new Size(200, 45);
        _sidebarPanel.TabIndex = 0;

        // 
        // _msgLabel
        // 
        _msgLabel.Appearance.FontSizeDelta = 1;
        _msgLabel.AutoSizeMode = LabelAutoSizeMode.None;
        _msgLabel.AllowHtmlString = true;
        _msgLabel.Dock = DockStyle.Fill;
        _msgLabel.Name = "_msgLabel";
        _msgLabel.Padding = new Padding(15,5,5,5);
        _msgLabel.Size = new Size(604, 45);
        _msgLabel.TabIndex = 0;
        _msgLabel.Text = @"...";

        // 
        // _msgCloseBtn
        // 
        _msgCloseBtn.AutoSizeMode = LabelAutoSizeMode.None;
        _msgCloseBtn.Dock = DockStyle.Right;
        _msgCloseBtn.Name = "_msgCloseBtn";
        _msgCloseBtn.Padding = new Padding(6);
        _msgCloseBtn.Size = new Size(45, 45);
        _msgCloseBtn.TabIndex = 0;
        _msgCloseBtn.Text = "";
        _msgCloseBtn.AllowGlyphSkinning = DefaultBoolean.True;
        _msgCloseBtn.ImageOptions.SvgImageSize = new Size(16, 16);
        _msgCloseBtn.ImageOptions.SvgImage = UI.GetImage(Symbol.DismissCircle);
        _msgCloseBtn.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.Full;
        _msgCloseBtn.Click += _flyoutclose_Click;
    }

    void _msg_Hidden(object sender, FlyoutPanelEventArgs e)
    {
        if (_msg != null)
            _msg.OwnerControl = Host;
    }

    void _sidebar_Hidden(object sender, FlyoutPanelEventArgs e)
    {
        if (_sidebar != null)
        {
            _sidebar.OwnerControl = Host;

            if (_sidebarPanel != null && _sidebarPanel.Controls.Count > 0)
                _sidebarPanel.Controls.Clear();
        }
    }

    private void _flyouttimer_Tick(object? sender, EventArgs e)
    {
        if (_msgTimer != null)
            _msgTimer.Enabled = false;

        _msg?.HidePopup();
    }

}

