namespace AF.WINFORMS.DX;

/// <summary>
/// Overlay that displays a message and optionaly a progress bar and a abort option (button).
/// </summary>
[ToolboxItem(false)]
[SupportedOSPlatform("windows")]
public partial class OverlayProgressDisplay : OverlayControlDXBase
{
    private bool _isCancel;

    /// <summary>
    /// Constructor
    /// </summary>
    public OverlayProgressDisplay()
    {
        InitializeComponent();

        if (!UI.DesignMode) return;

        Appearance.BackColor = Color.FromArgb(255, DevExpress.LookAndFeel.DXSkinColors.IconColors.Blue);

        lblCaption.Appearance.BackColor = Color.FromArgb(255, DevExpress.LookAndFeel.DXSkinColors.IconColors.Blue);
        lblInformation.Appearance.BackColor =
            Color.FromArgb(255, DevExpress.LookAndFeel.DXSkinColors.IconColors.Blue);
        lblProgress.Appearance.BackColor = Color.FromArgb(255, DevExpress.LookAndFeel.DXSkinColors.IconColors.Blue);
        lblCaption.Appearance.ForeColor = Color.FromArgb(255, DevExpress.LookAndFeel.DXSkinColors.IconColors.White);
        lblInformation.Appearance.ForeColor =
            Color.FromArgb(255, DevExpress.LookAndFeel.DXSkinColors.IconColors.White);
        lblProgress.Appearance.ForeColor =
            Color.FromArgb(255, DevExpress.LookAndFeel.DXSkinColors.IconColors.White);
        lblCaption.Appearance.Options.UseBackColor = true;
        lblCaption.Appearance.Options.UseForeColor = true;
        lblInformation.Appearance.Options.UseBackColor = true;
        lblInformation.Appearance.Options.UseForeColor = true;
        lblProgress.Appearance.Options.UseBackColor = true;
        lblProgress.Appearance.Options.UseForeColor = true;

        progress.Dock = DockStyle.Fill;
        progress.Properties.AllowAnimationOnValueChanged = DevExpress.Utils.DefaultBoolean.False;
        progressInfinite.Dock = DockStyle.Fill;
        progressInfinite.Visible = false;

        pshCancel.Text = @"ABBRECHEN";

        pshCancel.Click += (_, _) =>
        {
            IsCancel = true;
        };
    }

    /// <summary>
    /// Hide the abort option (button).
    /// </summary>
    public void HideCancel()
    {
        if (!pshCancel.Visible) return;

        pshCancel.Visible = false;
        Size = new Size(Width, Height - Convert.ToInt32(tableLayoutPanel1.RowStyles[4].Height));
        tableLayoutPanel1.RowStyles[4].Height = 0.0f;
    }

    /// <summary>
    /// Hide the progress bar.
    /// </summary>
    public void HideProgress()
    {
        if (!lblProgress.Visible) return;

        lblProgress.Visible = false;
        progress.Visible = false;
        progressInfinite.Visible = false;
        panel1.Visible = false;
        Size = new Size(Width,
            Height - Convert.ToInt32(tableLayoutPanel1.RowStyles[3].Height) -
            Convert.ToInt32(tableLayoutPanel1.RowStyles[2].Height));
        tableLayoutPanel1.RowStyles[2].Height = 0.0f;
        tableLayoutPanel1.RowStyles[3].Height = 0.0f;
    }

    /// <summary>
    /// Show progress bar as infinite moving bar
    /// </summary>
    public void ShowInfiniteProgress()
    {
        progress.Visible = false;
        progressInfinite.Visible = true;
    }

    /// <summary>
    /// Set current progress
    /// </summary>
    public void SetProgress(int current, int max)
    {
        if (!progress.Visible) return;

        if (progress.Properties.Maximum != max)
        {
            progress.Properties.Maximum = max;
            progress.Properties.PercentView = true;
            progress.Properties.Minimum = 0;
        }

        progress.Position = current;
        lblProgress.Text = $@"<b>{current}</b> von <b>{max}</b>";
        Application.DoEvents();
    }

    /// <summary>
    /// Disable the abort option (button).
    /// </summary>
    public void DisableCancel()
    {
        pshCancel.Enabled = false;
    }

    /// <summary>
    /// Enable the abort option (button).
    /// </summary>
    public void EnableCancel()
    {
        if (IsCancel == false)
            pshCancel.Enabled = true;
    }

    /// <summary>
    /// Caption
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Caption
    {
        get => lblCaption.Text;
        set => lblCaption.Text = value;
    }

    /// <summary>
    /// Description
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Description
    {
        get => lblInformation.Text;
        set => lblInformation.Text = value;
    }

    /// <summary>
    /// Current Progress (e.g. 'x of y')
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Progress
    {
        get => lblProgress.Text;
        set => lblProgress.Text = value;
    }

    /// <summary>
    /// Indicates that the user has triggered the abort option (button).
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsCancel
    {
        get => _isCancel;
        set
        {
            _isCancel = value;
            pshCancel.Enabled = !_isCancel;
            Application.DoEvents();
        }
    }
}

