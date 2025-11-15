using DevExpress.Utils;
using DevExpress.Utils.Layout;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Darstellung von zwei Werten mit je einem erläuternden Text.
/// </summary>
[DesignerCategory("Code")]
public sealed class AFPresenterTwoValues : AFPresenterBase
{
    private readonly AFLabelGrayText lblDescription1 = null!;
    private readonly AFLabelBoldText lblValue1 = null!;
    private readonly AFLabelGrayText lblDescription2 = null!;
    private readonly AFLabelBoldText lblValue2 = null!;
    private readonly AFLabelBoldText lblCaption = null!;
    private readonly AFLabel lblColorBar = null!;
    private readonly AFTablePanel table = null!;

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFPresenterTwoValues() : base(true)
    {
        if (UI.DesignMode) return;

        Padding = new(3);

        lblColorBar = new()
        {
            Dock = DockStyle.Left,
            Size = new(10, ClientRectangle.Height),
            AutoSizeMode = LabelAutoSizeMode.None,
            Appearance = { BackColor = UI.TranslateToSkinColor(Color.Green), Options = { UseBackColor = true } }
        };
        Controls.Add(lblColorBar);

        table = new AFTablePanel() { Dock = DockStyle.Fill, UseSkinIndents = false };
        Controls.Add(table);
        table.BringToFront();

        table.BeginLayout();

        lblCaption = new()
        {
            Dock = DockStyle.Top,
            AllowHtmlString = true,
            AutoSizeMode = LabelAutoSizeMode.Vertical,
            Padding = new(5),
            Visible = false,
            Appearance = { TextOptions = { HAlignment = HorzAlignment.Near, WordWrap = WordWrap.NoWrap, Trimming = Trimming.EllipsisCharacter }, FontSizeDelta = 2, Options = { UseTextOptions = true, UseFont = true } }
        };
        table.Add(lblCaption, 1, 1);

        lblDescription1 = new()
        {
            Dock = DockStyle.Top,
            AllowHtmlString = true,
            AutoSizeMode = LabelAutoSizeMode.Vertical,
            Padding = new(5, 3, 5, 3),
            Visible = false,
            Appearance = { TextOptions = { HAlignment = HorzAlignment.Center, WordWrap = WordWrap.Wrap, Trimming = Trimming.EllipsisCharacter }, Options = { UseTextOptions = true } }
        };
        table.Add(lblDescription1, 2, 1);

        lblValue1 = new()
        {
            Dock = DockStyle.Fill,
            AllowHtmlString = true,
            AutoSizeMode = LabelAutoSizeMode.None,
            Padding = new(5),
            Appearance = { TextOptions = { HAlignment = HorzAlignment.Center, VAlignment = VertAlignment.Center }, FontSizeDelta = 3, Options = { UseTextOptions = true, UseFont = true } }
        };
        table.Add(lblValue1, 3, 1);

        lblDescription2 = new()
        {
            Dock = DockStyle.Top,
            AllowHtmlString = true,
            AutoSizeMode = LabelAutoSizeMode.Vertical,
            Padding = new(5, 5, 5, 3),
            Visible = false,
            Appearance = { TextOptions = { HAlignment = HorzAlignment.Center, WordWrap = WordWrap.Wrap, Trimming = Trimming.EllipsisCharacter }, Options = { UseTextOptions = true } }
        };
        table.Add(lblDescription2, 4, 1);

        lblValue2 = new()
        {
            Dock = DockStyle.Fill,
            AllowHtmlString = true,
            AutoSizeMode = LabelAutoSizeMode.None,
            Padding = new(5),
            Appearance = { TextOptions = { HAlignment = HorzAlignment.Center, VAlignment = VertAlignment.Center }, FontSizeDelta = 3, Options = { UseTextOptions = true, UseFont = true } }
        };
        table.Add(lblValue2, 5, 1);

        table.SetColumn(1, TablePanelEntityStyle.Relative, 1.0f);
        table.SetRow(3, TablePanelEntityStyle.Relative, .5f);
        table.SetRow(5, TablePanelEntityStyle.Relative, .5f);

        table.EndLayout();

        table.Rows[0].Visible = lblCaption.Visible;
        table.Rows[1].Visible = lblDescription1.Visible;
        table.Rows[3].Visible = lblDescription2.Visible;

        BarColor = Color.Transparent;

        lblCaption.TextChanged += (_, _) =>
        {
            lblCaption.Visible = lblCaption.Text.IsNotEmpty();
            table.Rows[0].Visible = lblCaption.Visible;
        };

        lblDescription1.TextChanged += (_, _) =>
        {
            lblDescription1.Visible = lblDescription1.Text.IsNotEmpty();
            table.Rows[1].Visible = lblDescription1.Visible;
        };

        lblDescription2.TextChanged += (_, _) =>
        {
            lblDescription2.Visible = lblDescription2.Text.IsNotEmpty();
            table.Rows[3].Visible = lblDescription2.Visible;
        };
    }

    /// <summary>
    /// Hintergrundfarbe des Farbbalkens an der linken Seite setzen.
    ///
    /// Wenn Transparent, wird der Balken ausgeblendet.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color BarColor
    {
        get => lblColorBar.Appearance.BackColor;
        set
        {
            lblColorBar.Appearance.BackColor = value;
            lblColorBar.Visible = !lblColorBar.BackColor.Equals(Color.Transparent);
        }
    }

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFPresenterTwoValues(string caption, string description, string value, string description2, string value2) : this()
    {
        Caption = caption;

        lblDescription1.Text = description;
        lblValue1.Text = value;

        lblDescription2.Text = description2;
        lblValue2.Text = value2;
    }

    /// <summary>
    /// Überschrift (nur sichtbar, wenn Text nicht leer ist).
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Caption { get => lblCaption.Text; set => lblCaption.Text = value; }

    /// <summary>
    /// Beschreibung
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Description1 { get => lblDescription1.Text; set => lblDescription1.Text = value; }

    /// <summary>
    /// Wert
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Value1 { get => lblValue1.Text; set => lblValue1.Text = value; }

    /// <summary>
    /// Beschreibung
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Description2 { get => lblDescription2.Text; set => lblDescription2.Text = value; }

    /// <summary>
    /// Wert
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Value2 { get => lblValue2.Text; set => lblValue2.Text = value; }

    /// <summary>
    /// Zugriff auf das Label, dass den Wert anzeigt.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFLabelBoldText LabelValue1 => lblValue1;

    /// <summary>
    /// Zugriff auf das Label, dass die Überschrift anzeigt.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFLabelBoldText LabelCaption => lblCaption;

    /// <summary>
    /// Zugriff auf das Label, dass die Beschreibung anzeigt.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFLabelGrayText LabelDescription1 => lblDescription1;

    /// <summary>
    /// Zugriff auf das Label, dass den Wert anzeigt.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFLabelBoldText LabelValue2 => lblValue2;

    /// <summary>
    /// Zugriff auf das Label, dass die Beschreibung anzeigt.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFLabelGrayText LabelDescription2 => lblDescription2;
}