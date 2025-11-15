using DevExpress.Utils;
using DevExpress.Utils.Layout;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Darstellung eines Soll und eines Istwertes mit Abweichung, prozentualer Zielerreichung und automatischer Balkenfarbe.
/// </summary>
[DesignerCategory("Code")]
public sealed class AFPresenterTargetAndActual : AFPresenterBase
{
    private readonly AFLabelGrayText lblDescription = null!;
    private readonly AFLabelBoldText lblTargetValue = null!;
    private readonly AFLabelBoldText lblActualValue = null!;
    private readonly AFLabelBoldText lblDifference = null!;
    private readonly AFLabelBoldText lblCaption = null!;
    private readonly AFLabelDonut lblPercent = null!;
    private readonly AFLabel lblColorBar = null!;
    private readonly AFTablePanel table = null!;

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFPresenterTargetAndActual() : base(true)
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
            Padding = new(5, 5, 5, 3),
            Visible = false,
            Appearance = { TextOptions = { HAlignment = HorzAlignment.Near, WordWrap = WordWrap.NoWrap, Trimming = Trimming.EllipsisCharacter }, FontSizeDelta = 2, Options = { UseTextOptions = true, UseFont = true} }
        };
        table.Add(lblCaption, 1, 1, colspan: 3);

        lblDescription = new()
        {
            Dock = DockStyle.Top,
            AllowHtmlString = true,
            AutoSizeMode = LabelAutoSizeMode.Vertical,
            Padding = new(5, 3, 5, 3),
            Appearance = { TextOptions = { HAlignment = HorzAlignment.Near, WordWrap = WordWrap.Wrap, Trimming = Trimming.EllipsisCharacter }, Options = { UseTextOptions = true } }
        };
        table.Add(lblDescription, 2, 1, colspan: 3);

        lblPercent = new()
        {
            Dock = DockStyle.Fill,
            AllowHtmlString = true,
            AutoSizeMode = LabelAutoSizeMode.None,
            Padding = new(8),
            Appearance = { TextOptions = { HAlignment = HorzAlignment.Center, VAlignment = VertAlignment.Center }, FontSizeDelta = 1, FontStyleDelta = FontStyle.Bold, Options = { UseTextOptions = true, UseFont = true } }
        };
        table.Add(lblPercent, 3, 1, rowspan: 3);

        table.Add<AFLabelGrayText>(3, 2).Text("Soll");
        lblTargetValue = table.Add<AFLabelBoldText>(3, 3).Text("0").AdjustAlignment(ContentAlignment.MiddleRight).IndentRight(8);

        table.Add<AFLabelGrayText>(4, 2).Text("Ist");
        lblActualValue = table.Add<AFLabelBoldText>(4, 3).Text("0").AdjustAlignment(ContentAlignment.MiddleRight).IndentRight(8);

        table.Add<AFLabelGrayText>(5, 2).Text("Diff");
        lblDifference = table.Add<AFLabelBoldText>(5, 3).Text("0").AdjustAlignment(ContentAlignment.MiddleRight).IndentRight(8);

        table.SetColumn(1, TablePanelEntityStyle.Relative, 1.0f);
        table.SetRow(3, TablePanelEntityStyle.Relative, .33f);
        table.SetRow(4, TablePanelEntityStyle.Relative, .33f);
        table.SetRow(5, TablePanelEntityStyle.Relative, .34f);

        table.EndLayout();

        table.Rows[0].Visible = lblCaption.Visible;
        table.Rows[1].Visible = lblDescription.Visible;

        table.Columns[0].Visible = lblColorBar.Visible;

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
            table.Columns[0].Visible = lblColorBar.Visible;
        }
    }

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFPresenterTargetAndActual(string description, decimal target, decimal actual, string mask = "C0") : this()
    {
        lblDescription.Text = description;
        lblTargetValue.Text = target.ToString(mask);
        lblActualValue.Text = actual.ToString(mask);
        lblDifference.Text = (0m - target + actual).ToString(mask);

        var percent = (actual / target);
        if (percent >= 1m)
            lblColorBar.Appearance.BackColor = UI.TranslateToSkinColor(Color.Green);
        else if (percent is < 1m and >= 0.75m)
            lblColorBar.Appearance.BackColor = UI.TranslateToSkinColor(Color.Yellow);
        else
            lblColorBar.Appearance.BackColor = UI.TranslateToSkinColor(Color.Red);


        lblPercent.Color = lblColorBar.Appearance.BackColor;
        lblPercent.Percent = percent;
        lblPercent.Text = (percent).ToString("p0");
    }

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFPresenterTargetAndActual(string caption, string description, decimal target, decimal actual, string mask = "C0") : this()
    {
        lblCaption.Text = caption;
        lblDescription.Text = description;
        lblTargetValue.Text = target.ToString(mask);
        lblActualValue.Text = actual.ToString(mask);
        lblDifference.Text = (0m - target + actual).ToString(mask);

        var percent = (actual / target);
        if (percent >= 1m)
            lblColorBar.Appearance.BackColor = UI.TranslateToSkinColor(Color.Green);
        else if (percent is < 1m and >= 0.75m)
            lblColorBar.Appearance.BackColor = UI.TranslateToSkinColor(Color.Yellow);
        else
            lblColorBar.Appearance.BackColor = UI.TranslateToSkinColor(Color.Red);

        lblPercent.Color = lblColorBar.Appearance.BackColor;
        lblPercent.Percent = percent;
        lblPercent.Text = (percent).ToString("p0");
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
    public string Description { get => lblDescription.Text; set => lblDescription.Text = value; }

    /// <summary>
    /// Zugriff auf das Label, dass den Wert anzeigt.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFLabelBoldText LabelTarget => lblTargetValue;

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

    /// <summary>
    /// Zugriff auf das Label, dass den Wert anzeigt.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFLabelBoldText LabelActual => lblActualValue;

    /// <summary>
    /// Zugriff auf das Label, dass den Wert anzeigt.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFLabelBoldText LabelDifference => lblDifference;

    /// <summary>
    /// Zugriff auf das Label, dass die Beschreibung anzeigt.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFLabelDonut LabelPercent => lblPercent;
}