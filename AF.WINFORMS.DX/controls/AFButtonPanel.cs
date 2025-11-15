using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Panel mit zwei Buttons
/// </summary>
[ToolboxItem(true)]
[SupportedOSPlatform("windows")]
public partial class AFButtonPanel : AFUserControl
{
    /// <summary>
    /// Constructor
    /// </summary>
    public AFButtonPanel()
    {
        InitializeComponent();

        if (UI.DesignMode) return;

        simpleButton3.Appearance.BackColor = DXSkinColors.FillColors.Primary ;
        simpleButton3.Appearance.Options.UseBackColor = true;
        simpleButton2.Appearance.BackColor = DXSkinColors.FillColors.Danger;
        simpleButton2.Appearance.Options.UseBackColor = true;
        simpleButton1.Appearance.BackColor = DXSkinColors.FillColors.Success;
        simpleButton1.Appearance.Options.UseBackColor = true;
        simpleButton1.Click += simpleButton1_Click;
        simpleButton2.Click += simpleButton2_Click;
        simpleButton3.Click += simpleButton3_Click;
    }

    /// <summary>
    /// Aktion, die ausgeführt wird, wenn die 
    /// linke Schaltfläche angeklickt wird
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Action? OnButtonOkClick { get; set; }

    /// <summary>
    /// Aktion, die ausgeführt wird, wenn die 
    /// rechte Schaltfläche angeklickt wird
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Action? OnButtonCancelClick { get; set; }

    /// <summary>
    /// Aktion, die ausgeführt wird, wenn die 
    /// linke Schaltfläche angeklickt wird
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Action? OnButtonMoreClick { get; set; }

    private void simpleButton3_Click(object? sender, EventArgs e)
    {
        OnButtonMoreClick?.Invoke();
    }

    private void simpleButton1_Click(object? sender, EventArgs e)
    {
        OnButtonOkClick?.Invoke();
    }

    private void simpleButton2_Click(object? sender, EventArgs e)
    {
        OnButtonCancelClick?.Invoke();
    }

    /// <summary>
    /// Text linke Schaltfläche
    /// </summary>
    [Browsable(true)]
    [DefaultValue("Mehr Informationen")]
    public string CaptionMore { get => ButtonMore.Text; set => ButtonMore.Text = value; }

    /// <summary>
    /// Text erste rechte Schaltfläche
    /// </summary>
    [Browsable(true)]
    [DefaultValue("Ok")]
    public string CaptionOk { get => ButtonOk.Text; set => ButtonOk.Text = value; }

    /// <summary>
    /// Text zweite rechte Schaltfläche
    /// </summary>
    [Browsable(true)]
    [DefaultValue("Abbrechen")]
    public string CaptionCancel { get => ButtonCancel.Text; set => ButtonCancel.Text = value; }


    /// <summary>
    /// linke Schaltfläche anzeigen
    /// </summary>
    [Browsable(true)]
    [DefaultValue(false)]
    public bool ShowMore { get => ButtonMore.Visible; set => ButtonMore.Visible = value; }

    /// <summary>
    /// erste rechte Schaltfläche anzeigen
    /// </summary>
    [Browsable(true)]
    [DefaultValue(true)]
    public bool ShowOk { get => ButtonOk.Visible; set => ButtonOk.Visible = value; }

    /// <summary>
    /// zweite rechte Schaltfläche anzeigen
    /// </summary>
    [Browsable(true)]
    [DefaultValue(true)]
    public bool ShowCancel { get => ButtonCancel.Visible; set => ButtonCancel.Visible = value; }

    /// <summary>
    /// linke Schaltfläche
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SimpleButton ButtonMore => simpleButton3;

    /// <summary>
    /// erste rechte Schaltfläche
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SimpleButton ButtonOk => simpleButton1;

    /// <summary>
    /// zweite rechte Schaltfläche
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SimpleButton ButtonCancel => simpleButton2;
}
