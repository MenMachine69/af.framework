using DevExpress.Utils.Extensions;
using DevExpress.Utils.Layout;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Editor für Einstellungen.
/// </summary>
[ToolboxItem(false)]
[SupportedOSPlatform("windows")]
[DesignerCategory("Code")]
public sealed class AFSettingsEditor : AFEditor
{
    private readonly AFBindingConnector connector = null!;
    private readonly AFLabelGrayText description = null!;
    private readonly AFStackPanel stack = null!;
    private readonly AFStackPanel breadcrumb = null!;
    private ISettingsController? _controller;
    private ISettingsElement? currentPage;
    
    /// <summary>
    /// Constructor
    /// </summary>
    public AFSettingsEditor()
    {
        if (UI.DesignMode) return;

        DoubleBuffered = true;

        breadcrumb = new() { Dock = DockStyle.Top, AutoScroll = true, LayoutDirection = StackPanelLayoutDirection.LeftToRight, AutoSizeChilds = true, UseSkinIndents = false, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink };
        Controls.Add(breadcrumb);

        SeparatorControl line = new() { Dock = DockStyle.Top, Size = new(Width, 3), Padding = new(0), Margin = new(0), LineColor = UI.TranslateSystemToSkinColor(SystemColors.WindowText) };
        Controls.Add(line);
        line.BringToFront();

        description = new() { Dock = DockStyle.Top, Padding = new(8), Text = "", AutoSizeMode = LabelAutoSizeMode.Vertical, AllowHtmlString = true };
        Controls.Add(description);
        description.BringToFront();

        stack = new() { Dock = DockStyle.Fill, AutoScroll = true, LayoutDirection = StackPanelLayoutDirection.TopDown, AutoSizeChilds = true };
        Controls.Add(stack);
        stack.BringToFront();

        connector = new(Container) { ContainerControl = this, StartContainerControl = stack };
    }

    /// <summary>
    /// Name der Konfiguration, die vom Controller geladen werden soll
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string ConfigName { get; set; } = "SYSTEM";

    /// <summary>
    /// Aktuell vom Controller geladene Konfiguration
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFSettings? CurrentConfig { get; private set; }

    /// <summary>
    /// Setup für die Anzeige
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ISettingsController? Controller
    {
        get => _controller;
        set 
        {
            _controller = value;

            if (_controller == null) return;

            CurrentConfig = _controller.GetSettings(ConfigName);

            LoadElements(CurrentConfig);

        }
    }

    /// <summary>
    /// Elemente laden.
    /// </summary>
    /// <param name="page">Einstellungselement, das geladen werden soll</param>
    /// <param name="navback">true = Quelle ist eine Navigation zurück via BreadCrumb (default = false)</param>
    public void LoadElements(ISettingsElement page, bool navback = false)
    {
        currentPage = page;

        // Reset der Seite, alles entfernen...
        stack.Controls.Clear(true);

        if (page.Elements.Count < 1)
            return;

        if (!navback) // Breadcrumb ergänzen...
        {
            var label = new AFLabelCaption { Text = page.Caption, AutoSizeMode = LabelAutoSizeMode.Horizontal, Padding = new(5), Dock = DockStyle.Left };
            if (page is not AFSettings)
            {
                label.ImageOptions.SvgImage = UI.GetImage(Symbol.ChevronLeft);
                label.ImageOptions.SvgImageSize = new(20, 20);
                label.ImageAlignToText = ImageAlignToText.LeftCenter;
            }

            label.EnableHighlightHoover();
            label.Tag = page;
            label.Click += breadcrumbLabelClick;

            breadcrumb.Controls.Add(label);
        }

        description.Text = page.Description;
        
        foreach (var element in page.Elements)
        {
            if (element.Elements.Count < 1 && element.ValueName == "" && element.Symbol == null) // eine Zwischenüberschrift.
            {
                AFLabelBoldText labelCaption = new() { AutoSizeMode = LabelAutoSizeMode.Vertical, AllowHtmlString = true, Dock = DockStyle.Top, Text = element.Caption, Padding = new(8, 5, 8, 2) };
                stack.AddControl(labelCaption);
                if (element.Description == "") continue;
                AFLabelGrayText labelDescription = new() { AutoSizeMode = LabelAutoSizeMode.Vertical, AllowHtmlString = true, Dock = DockStyle.Top, Text = element.Description, Padding = new(8,0,8,2) };
                stack.AddControl(labelDescription);
                continue;
            }

            AFSettingsEditorElement editor = new(element, this);
            editor.Dock = DockStyle.Top;
            stack.AddControl(editor);
        }

        stack.AlignChilds();
    }

    private void breadcrumbLabelClick(object? sender, EventArgs e)
    {
        if (sender is not AFLabelCaption label) return;

        if (label.Tag == currentPage) return;

        while (breadcrumb.Controls[^1] != sender)
            breadcrumb.Controls.RemoveAt(breadcrumb.Controls.Count - 1);

        if (label.Tag is not ISettingsElement element) return;
        
        LoadElements(element, true);
    }
}