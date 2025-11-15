using DevExpress.Utils;
using DevExpress.Utils.Layout;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Darstellung von zwei Werten, die zusammen 100% ergeben als Balken.
/// </summary>
[DesignerCategory("Code")]
public sealed class AFPresenterTwoSides : AFPresenterBase
{
    private readonly AFLabelBoldText lblCaption = null!;
    private readonly AFLabelGrayText lblDescription = null!;
    private readonly AFLabelTwoSides lblSides = null!;
    private readonly AFTablePanel table = null!;

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFPresenterTwoSides(string caption, string description) : base(true)
    {
        if (UI.DesignMode) return;

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
            Dock = DockStyle.Top,
            AllowHtmlString = true,
            AutoSizeMode = LabelAutoSizeMode.Vertical,
            Padding = new(5, 3, 5, 3),
            Appearance = { TextOptions = { HAlignment = HorzAlignment.Near, WordWrap = WordWrap.Wrap, Trimming = Trimming.EllipsisCharacter }, Options = { UseTextOptions = true } }
        };
        table.Add(lblDescription, 2, 1);

        lblSides = new() { Dock = DockStyle.Fill, Padding = new(5, 3, 5, 3), };

        table.Add(lblSides, 3, 1);

        table.SetColumn(1, TablePanelEntityStyle.Relative, 1.0f);
        table.SetRow(3, TablePanelEntityStyle.Relative, 1.0f);

        table.EndLayout();

        table.Rows[0].Visible = lblCaption.Visible;
        table.Rows[1].Visible = lblDescription.Visible;

        lblCaption.TextChanged += (_, _) =>
        {
            lblCaption.Visible = lblCaption.Text.IsNotEmpty();
            table.Rows[0].Visible = lblCaption.Visible;
        };

        lblDescription.TextChanged += (_, _) =>
        {
            lblDescription.Visible = lblDescription.Text.IsNotEmpty();
            table.Rows[0].Visible = lblDescription.Visible;
        };

        Caption = caption;
        Description = description;
    }

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFPresenterTwoSides(string caption, string description, string text, decimal leftvalue, decimal rightvalue) : this(caption, description)
    {
        lblSides.LeftValue = leftvalue;
        lblSides.RightValue = rightvalue;
        lblSides.Text = text;
    }

    /// <summary>
    /// Label, dass die Bar darstellt.
    /// </summary>
    public AFLabelTwoSides Presenter => lblSides;

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
    /// Beschreibung (nur sichtbar, wenn Text nicht leer ist).
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Description
    {
        get => lblCaption.Text;
        set
        {
            lblDescription.Text = value;
            lblDescription.Visible = value.IsNotEmpty();
            table.Rows[1].Visible = lblDescription.Visible;
        }
    }
}