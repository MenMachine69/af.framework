using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace AF.WINFORMS.DX;

/// <summary>
/// Dashboard zur Anzeige von KPI-Werten
/// </summary>
[DesignerCategory("Code")]
public sealed class AFDashboardKPI : AFUserControl
{
    private AFDashboardKPIModel? currentmodel;
    private WeakEvent<EventHandler<EventArgs>>? customize;
    private readonly AFTablePanel table = null!;
    private readonly AFUserControl scroller = null!;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFDashboardKPI()
    {
        if (UI.DesignMode) return;

        table = new() { Dock = DockStyle.Fill, UseSkinIndents = false, Margin = new(0), Padding = new(0) };
        Controls.Add(table);
        table.BringToFront();

        table.BeginLayout();

        scroller = table.Add<AFUserControl>(1,2, rowspan: 2).Dock(DockStyle.Fill);
        scroller.AutoScroll = false;
        scroller.VerticalScroll.Visible = false;
        scroller.HorizontalScroll.Visible = false;
        scroller.AutoScrollPosition = new(0, 0);
        scroller.SizeChanged += (_, _) =>
        {
            scrollpos = scroller.Width;
            scroller.AutoScrollPosition = new(0, 0);
        };

        var btn = table.Add<AFButton>(1, 3);
        btn.PaintStyle = PaintStyles.Light;
        btn.ImageOptions.Location = ImageLocation.MiddleCenter;
        btn.ImageOptions.SvgImage = UI.GetImage(Symbol.Wrench);
        btn.ImageOptions.SvgImageSize = new(12, 12);
        btn.Margin = new(0);
        btn.AllowFocus = false;
        btn.ShowFocusRectangle = DefaultBoolean.False;
        btn.SuperTip = UI.GetSuperTip("ANPASSEN", "Anpassen des KPI Displays (Auswahl der Werte und Reihenfolge)");
        btn.Click += (_, _) => { customize?.Raise(this, EventArgs.Empty); };

        var btnScrollLeft = table.Add<AFButton>(1, 1, rowspan: 2);
        btnScrollLeft.PaintStyle = PaintStyles.Light;
        btnScrollLeft.ImageOptions.Location = ImageLocation.MiddleCenter;
        btnScrollLeft.ImageOptions.SvgImage = UI.GetImage(Symbol.ArrowLeft);
        btnScrollLeft.ImageOptions.SvgImageSize = new(12, 12);
        btnScrollLeft.Margin = new(0);
        btnScrollLeft.AllowFocus = false;
        btnScrollLeft.ShowFocusRectangle = DefaultBoolean.False;
        btnScrollLeft.SuperTip = UI.GetSuperTip("ANPASSEN", "Anpassen des KPI Displays (Auswahl der Werte und Reihenfolge)");
        btnScrollLeft.Click += (_, _) => { scroll(false); };

        var btnScrollRight = table.Add<AFButton>(2, 3);
        btnScrollRight.PaintStyle = PaintStyles.Light;
        btnScrollRight.ImageOptions.Location = ImageLocation.MiddleCenter;
        btnScrollRight.ImageOptions.SvgImage = UI.GetImage(Symbol.ArrowRight);
        btnScrollRight.ImageOptions.SvgImageSize = new(12, 12);
        btnScrollRight.Margin = new(0);
        btnScrollRight.AllowFocus = false;
        btnScrollRight.ShowFocusRectangle = DefaultBoolean.False;
        btnScrollRight.SuperTip = UI.GetSuperTip("ANPASSEN", "Anpassen des KPI Displays (Auswahl der Werte und Reihenfolge)");
        btnScrollRight.Click += (_, _) => { scroll(true); };

        table.SetRow(1, DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 16f);
        table.SetRow(2, DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 1f);
        table.SetColumn(1, DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 16f);
        table.SetColumn(2, DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 1f);
        table.SetColumn(3, DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 16f);
        
        table.EndLayout();

    }

    private int scrollpos = 0;

    private void scroll(bool right)
    {
        if (right)
        {
            if (scrollpos + 90 < scroller.HorizontalScroll.Maximum)
            {
                scrollpos += 90;
                scroller.HorizontalScroll.Value = scrollpos;
            }
            else
            {
                scrollpos = scroller.HorizontalScroll.Maximum;
                scroller.AutoScrollPosition = new(scroller.HorizontalScroll.Maximum, 0);
            }
        }
        else
        {
            if (scrollpos - 90 > scroller.Width)
            {
                scrollpos -= 90;
                scroller.HorizontalScroll.Value = scrollpos;
            }
            else
            {
                scrollpos = scroller.Width;
                scroller.AutoScrollPosition = new(0, 0);
            }
        }
    }

    /// <summary>
    /// Ereignis, das ausgelöst wird, wenn die Schaltfläche zum Anpassen der
    /// Werte geklickt wird. 
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public event EventHandler<EventArgs> OnCustomize
    {
        add
        {
            customize ??= new();
            customize.Add(value);
        }
        remove => customize?.Remove(value);
    }

    /// <summary>
    /// Model mit Werten laden...
    /// </summary>
    /// <param name="model"></param>
    public void LoadModel(AFDashboardKPIModel? model)
    {
        currentmodel = model;
        showModel(currentmodel);
    }

    private void showModel(AFDashboardKPIModel? model)
    {
        if (model == null) return;

        //SuspendLayout();
        //this.SuspendDrawing();

        //if (Handle != IntPtr.Zero && Controls.Count > 0)
        //    Controls.Clear(true);

        foreach (var element in model.Elements)
        {
            AFDashboardKPISmall kpi = new() { 
                Text = element.Value, 
                Caption = element.Caption, 
                HighlightColor = element.Indicator, 
                Size = new(90, 40), 
                Padding = new(3),
                Margin = new(0),
                Dock = DockStyle.Left,
                AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None
            };
            
            if (element.Description.IsNotEmpty())
                kpi.SuperTip = UI.GetSuperTip(element.Title, element.Description);

            scroller.Controls.Add(kpi);
            kpi.BringToFront();
        }

        scroller.HorizontalScroll.Maximum = scroller.Controls.Count * 90;
        scrollpos = scroller.Width;



        //this.ResumeDrawing();
        //ResumeLayout();

    }

}

