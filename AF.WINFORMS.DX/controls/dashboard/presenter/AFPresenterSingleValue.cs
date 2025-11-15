using DevExpress.Utils;
using DevExpress.Utils.Layout;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Darstellung eines einzelnen Wertes zusammen mit einem erläuternden Text.
/// </summary>
[DesignerCategory("Code")]
public sealed class AFPresenterSingleValue : AFPresenterBase
{
    private readonly AFLabelGrayText lblDescription = null!;
    private readonly AFLabelBoldText lblValue = null!;
    private readonly AFLabelBoldText lblCaption = null!;
    private readonly AFLabel lblColorBar = null!;
    private readonly AFTablePanel table = null!;

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFPresenterSingleValue() : base(true)
    {
        if (UI.DesignMode) return;

        Padding = new(3);

        lblColorBar = new()
        {
            Dock = DockStyle.Left, Size = new(10, ClientRectangle.Height), AutoSizeMode = LabelAutoSizeMode.None,
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
            Padding = new(5, 5, 5, 3),
            Visible = false,
            Appearance = { TextOptions = { HAlignment = HorzAlignment.Near, WordWrap = WordWrap.NoWrap, Trimming = Trimming.EllipsisCharacter }, FontSizeDelta = 2, Options = { UseTextOptions = true, UseFont = true } }
        };
        table.Add(lblCaption, 1, 1);

        lblDescription = new()
        {
            Dock = DockStyle.Top, AllowHtmlString = true, AutoSizeMode = LabelAutoSizeMode.Vertical,
            Padding = new(5, 3, 5, 3), Visible = false,
            Appearance = { TextOptions = { HAlignment = HorzAlignment.Center, WordWrap = WordWrap.Wrap, Trimming = Trimming.EllipsisCharacter }, Options = { UseTextOptions = true } }
        };
        table.Add(lblDescription, 2, 1);

        lblValue = new()
        {
            Dock = DockStyle.Fill, AllowHtmlString = true, AutoSizeMode = LabelAutoSizeMode.None, Padding = new(5),
            Appearance = { TextOptions = { HAlignment = HorzAlignment.Center, VAlignment = VertAlignment.Center }, FontSizeDelta = 3, Options = { UseTextOptions = true, UseFont = true } }
        };
        table.Add(lblValue, 3, 1);

        table.SetColumn(1, TablePanelEntityStyle.Relative, 1.0f);
        table.SetRow(3, TablePanelEntityStyle.Relative, 1.0f);

        table.EndLayout();

        table.Rows[0].Visible = lblCaption.Visible;
        table.Rows[1].Visible = lblDescription.Visible;

        BarColor = Color.Transparent;

        lblCaption.TextChanged += (_, _) =>
        {
            lblCaption.Visible = lblCaption.Text.IsNotEmpty();
            table.Rows[0].Visible = lblCaption.Visible;
        };

        lblDescription.TextChanged += (_, _) =>
        {
            lblDescription.Visible = lblDescription.Text.IsNotEmpty();
            table.Rows[1].Visible = lblDescription.Visible;
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
    public AFPresenterSingleValue(string caption, string description, string value) : this()
    {
        lblDescription.Text = description;
        lblValue.Text = value;

        Caption = caption;
    }

    /// <summary>
    /// Überschrift (nur sichtbar, wenn Text nicht leer ist).
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Caption
    {
        get => lblCaption.Text;
        set
        {
            lblCaption.Text = value;
            lblCaption.Visible = value.IsNotEmpty();
            table.Rows[0].Visible = lblCaption.Visible;
        }
    }

    /// <summary>
    /// Beschreibung
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Description { get => lblDescription.Text; set => lblDescription.Text = value; }

    /// <summary>
    /// Wert
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Value { get => lblValue.Text; set => lblValue.Text = value; }

    /// <summary>
    /// Zugriff auf das Label, dass den Wert anzeigt.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFLabelBoldText LabelValue => lblValue;

    /// <summary>
    /// Zugriff auf das Label, dass die Überschrift anzeigt.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFLabelBoldText LabelCaption => lblCaption;

    /// <summary>
    /// Zugriff auf das Label, dass die Beschreibung anzeigt.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFLabelGrayText LabelDescription => lblDescription;
}