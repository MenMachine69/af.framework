using DevExpress.Utils;
using DevExpress.Utils.Layout;

namespace AF.MVC;

/// <summary>
/// Fortschrittsanzeige für eine AFPage-Seite (Progress-Overlay)
/// </summary>
[ToolboxItem(false)]
public sealed class AFPageProgress : AFUserControl, IViewPageProgress
{
    private readonly AFLabelCaption lblCaption;
    private readonly AFLabel lblDescription;
    private readonly IViewPage parentPage;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFPageProgress(IViewPage page)
    {
        parentPage = page;
        ((Control)page).Resize += pageResize;

        Size = new(500, 150);
        Appearance.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Highlight);
        Appearance.Options.UseBackColor = true;

        AFTablePanel table = new() { Dock = DockStyle.Fill, UseSkinIndents = true };
        table.BeginLayout();

        lblCaption = table.Add<AFLabelCaption>(2, 2, colspan: 2);
        lblCaption.Text = "ÜBERSCHRIFT";
        lblCaption.Appearance.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Highlight);
        lblCaption.Appearance.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.HighlightText);
        lblCaption.Appearance.Options.UseBackColor = true;
        lblCaption.Appearance.Options.UseForeColor = true;


        lblDescription = table.Add<AFLabel>(3, 2, colspan: 2);
        lblDescription.Text = "Beschreibung";
        lblDescription.Appearance.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Highlight);
        lblDescription.Appearance.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.HighlightText);
        lblDescription.Appearance.Options.UseBackColor = true;
        lblDescription.Appearance.Options.UseForeColor = true;
        lblDescription.Appearance.TextOptions.VAlignment = VertAlignment.Top;
        lblDescription.Margin = new(0, 6, 0, 0);

        var pgbProgress1 = table.Add<AFProgressBar>(4, 2);
        pgbProgress1.Width = 200;

        var lblProgress1 = table.Add<AFLabel>(4, 3);
        lblProgress1.Text = "0 von 0";
        lblProgress1.Appearance.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Highlight);
        lblProgress1.Appearance.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.HighlightText);
        lblProgress1.Appearance.Options.UseBackColor = true;
        lblProgress1.Appearance.Options.UseForeColor = true;
        lblProgress1.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        
        var pshCancel1 = table.Add<AFButton>(5, 3);
        pshCancel1.Text = "ABBRECHEN";

        //table.Add<AFLabel>(1, 5);

        table.SetColumn(1, TablePanelEntityStyle.Relative, 0.5f);
        table.SetColumn(4, TablePanelEntityStyle.Relative, 0.5f);
        table.SetRow(1, TablePanelEntityStyle.Absolute, 10.0f);
        table.SetRow(3, TablePanelEntityStyle.Relative, 1.0f);
        table.SetRow(6, TablePanelEntityStyle.Absolute, 10.0f);
        table.EndLayout();

        Controls.Add(table);
    }

    /// <inheritdoc />
    protected override void OnHandleDestroyed(EventArgs e)
    {
        ((Control)parentPage).Resize -= pageResize;

        base.OnHandleDestroyed(e);
    }

    private void pageResize(object? sender, EventArgs e)
    {
        Size = new(((Control)parentPage).Width, Height);
        Location = new Point(0, (((Control)parentPage).Height - Height) / 2);
    }

    /// <inheritdoc />
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden )]
    public string Caption { get => lblCaption.Text; set => lblCaption.Text = value; }

    /// <inheritdoc />
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Description { get => lblDescription.Text; set => lblDescription.Text = value; }

    /// <inheritdoc />
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int Total { get; set; }

    /// <inheritdoc />
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int Current { get; set; }

    /// <inheritdoc />
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Canceled { get; set; }
}