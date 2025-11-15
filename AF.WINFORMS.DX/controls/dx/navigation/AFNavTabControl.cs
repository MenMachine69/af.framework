using DevExpress.Utils;
using DevExpress.Utils.Extensions;
using DevExpress.XtraTab;
using DevExpress.XtraTab.ViewInfo;

namespace AF.WINFORMS.DX;

/// <inheritdoc/>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Navigation")]
public class AFNavTabControl : XtraTabControl
{
    private bool _paintBackground;

    /// <inheritdoc />
    public AFNavTabControl()
    {
        if (UI.DesignMode) return;

        Transition.AllowTransition = DefaultBoolean.False;
    }

    /// <summary>
    /// Custom draw Background using the BackgroundAppearances
    /// </summary>
    [Category("Custom background")]
    [Browsable(true)]
    [DefaultValue(false)]
    public bool CustomPaintBackground
    {
        get => _paintBackground;
        set
        {
            _paintBackground = value;

            if (value)
                BackgroundAppearance ??= new();
            else
                BackgroundAppearance = null;

            Invalidate();
        }
    }

    /// <summary>
    /// Appearance for custom drawing background
    /// </summary>
    [DefaultValue(null)]
    [Category("Custom background")]
    public AFBackgroundAppearance? BackgroundAppearance { get; set; }


    /// <inheritdoc />
    protected override void OnTabPageAdded(XtraTabPage page)
    {
        base.OnTabPageAdded(page);

        if (UI.DesignMode) return;

        page.PaintEx += customPaintBackground;
    }

    /// <inheritdoc />
    protected override void OnHandleDestroyed(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (UI.DesignMode) return;

        foreach (XtraTabPage page in TabPages)
            page.PaintEx -= customPaintBackground;
    }

    private void customPaintBackground(object sender, XtraPaintEventArgs e)
    {
        if (!CustomPaintBackground) return;

        if (BackgroundAppearance == null) return;

        e.Cache.Clear(UI.TranslateSystemToSkinColor(SystemColors.Control));
        var rect = e.ClipRectangle.WithDeflate(new Padding(0, 0, 1, 1));

        BackgroundAppearance.Draw(e.Cache, rect, Margin, Padding);

    }
}

/// <inheritdoc/>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Navigation")]
public class AFNavTabControlDynamic : AFNavTabControl
{
    private readonly Dictionary<XtraTabPage, Tuple<Type, bool>> _dynamicPages = [];

    /// <summary>
    /// Control handle created...
    ///
    /// Load first page.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (UI.DesignMode) return;

        loadPage(SelectedTabPage);
    }

    /// <summary>
    /// Registriert eine dynamisch anzuzeigende Seite.
    /// </summary>
    /// <param name="page">Page, für die die Definition gilt</param>
    /// <param name="contentControlType">Typ des anzuzeigenden Controls in der Page</param>
    /// <param name="clearPage">Page leeren, wenn andere Page ausgewählt wird</param>
    public void RegisterContent(XtraTabPage page, Type contentControlType, bool clearPage)
    {
        _dynamicPages.Add(page, new Tuple<Type, bool>(contentControlType, clearPage));
    }

    /// <summary>
    /// <see cref="XtraTabControl.OnSelectedPageChanging(object, ViewInfoTabPageChangingEventArgs)"/>
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected override void OnSelectedPageChanging(object sender, ViewInfoTabPageChangingEventArgs e)
    {
        base.OnSelectedPageChanging(sender, e);

        // ggf. alte Seite bereinigen
        if (e.PrevPage != null && ((XtraTabPage)e.PrevPage).Controls.Count > 0)
        {
            XtraTabPage page = (XtraTabPage)e.PrevPage;

            if (_dynamicPages.ContainsKey(page) && _dynamicPages[page].Item2) // prüfen ob die Seite dynamisch ist...
            {
                Control ctrl = page.Controls[0];

                DynamicPageRemoving?.Invoke(this, new AFTabControlEventArgs { Page = page, Control = ctrl });
                    
                page.Controls.Remove(ctrl);
                ctrl.Dispose();
            }
        }

        if (e.Page != null && ((XtraTabPage)e.Page).Controls.Count < 1)
        {
            // TAB ausgewählt, dass noch keine Controls enthält...
            loadPage((XtraTabPage)e.Page);

        }
    }

    /// <summary>
    /// Handler for Page events
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void DynamicPageEventHandler(object sender, AFTabControlEventArgs args);
        
    /// <summary>
    /// Page-Control will be removed
    /// Use this event to save something from the Page-Control that will be removed after this event.
    /// </summary>
    public event DynamicPageEventHandler? DynamicPageRemoving;

    /// <summary>
    /// new Page-Control created
    /// Use this event to init something in the new created Page-Control before this Control will be displayed.
    /// </summary>
    public event DynamicPageEventHandler? DynamicPageCreated;

    private void loadPage(XtraTabPage page)
    {
        if (!_dynamicPages.ContainsKey(page)) return; // prüfen ob die Seite dynamisch ist...

        page.SuspendLayout();
        page.SuspendDrawing();

        Control ctrl = ControlsEx.CreateInstance(_dynamicPages[page].Item1)!;

        ctrl.Dock = DockStyle.Fill;
        page.Controls.Add(ctrl);

        DynamicPageCreated?.Invoke(this, new AFTabControlEventArgs { Page = page, Control = ctrl });

        page.ResumeDrawing();
        page.ResumeLayout(true);

    }
}

/// <summary>
/// Page event arguments
/// </summary>
public class AFTabControlEventArgs : EventArgs
{
    /// <summary>
    /// Page that raises the event
    /// </summary>
    public XtraTabPage? Page { get; set; }
    /// <summary>
    /// Control thats displayed on the page or should be displayed
    /// </summary>
    public Control? Control { get; set; }
}