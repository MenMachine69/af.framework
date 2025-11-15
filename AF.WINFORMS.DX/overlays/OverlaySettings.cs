namespace AF.WINFORMS.DX;

/// <summary>
/// Einstellungen für ein Overlay im ViewManager
/// </summary>
[ToolboxItem(false)]
[SupportedOSPlatform("windows")]
public partial class OverlaySettings : OverlayControlDXBase
{
    private Control? _currentPage;
    private DevExpress.XtraBars.Navigation.AccordionControlElement? _currentItem;

    /// <summary>
    /// Constructor
    /// </summary>
    public OverlaySettings()
    {
        InitializeComponent();

        if (UI.DesignMode) return;

        pshSaveSettings.ImageOptions.SvgImage = UI.GetImage(Symbol.Save);
        grpSettings.Elements.Clear();
    }

    /// <summary>
    /// 
    /// </summary>
    public ISettingsPage? CurrentPage => (_currentPage != null ? (ISettingsPage)_currentPage : null);

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="pages">list of available pages</param>
    public OverlaySettings(IEnumerable<Tuple<string, Type>> pages)
    {
        InitializeComponent();

        if (UI.DesignMode) return;

        pshSaveSettings.ImageOptions.SvgImage = UI.GetImage(Symbol.Save);
        grpSettings.Elements.Clear();

        foreach (var page in pages)
        {
            var item = new DevExpress.XtraBars.Navigation.AccordionControlElement
            {
                Text = page.Item1,
                Style = DevExpress.XtraBars.Navigation.ElementStyle.Item,
                Name = @"page" + grpSettings.Elements.Count + 1,
                Tag = page.Item2
            };
            item.Click += _itemClick;

            item.Appearance.Normal.FontSizeDelta = 1;
            item.Appearance.Normal.FontStyleDelta = FontStyle.Bold;
            item.Appearance.Normal.Options.UseFont = true;
            item.Appearance.Pressed.FontSizeDelta = 1;
            item.Appearance.Pressed.FontStyleDelta = FontStyle.Bold;
            item.Appearance.Pressed.Options.UseFont = true;
            item.Appearance.Hovered.FontSizeDelta = 1;
            item.Appearance.Hovered.FontStyleDelta = FontStyle.Bold;
            item.Appearance.Hovered.Options.UseFont = true;

            grpSettings.Elements.Add(item);
        }

        _itemClick(grpSettings.Elements[0], EventArgs.Empty);
    }

    private void menSettingsClose_Click(object sender, EventArgs e)
    {
        OverlayManager?.CloseOverlay();
    }

    private void menSettingsImport_Click(object sender, EventArgs e)
    {

    }

    private void menSettingsExport_Click(object sender, EventArgs e)
    {

    }

    private void _itemClick(object? sender, EventArgs e)
    {
        if (sender == null)
            return;

        DevExpress.XtraBars.Navigation.AccordionControlElement item =
            (DevExpress.XtraBars.Navigation.AccordionControlElement)sender;

        if (item.Tag is not Type type)
            throw new ArgumentException($"{item.Name} has no Type information in Tag property.");

        if (_currentPage != null)
        {
            if (_currentPage.GetType().Equals(type))
                return;

            if (CurrentPage?.CanClose() == false)
                return;

            tableContent.Controls.Remove(_currentPage);

            _currentPage = null;
        }

        _currentPage = ControlsEx.CreateInstance(type);

        _currentPage.Dock = DockStyle.Fill;
        _currentPage.Margin = new Padding(10);
        tableContent.BeginInit();
        tableContent.Controls.Add(_currentPage);
        tableContent.SetColumn(_currentPage, 0);
        tableContent.SetRow(_currentPage, 0);
        tableContent.EndInit();
        tableContent.PerformLayout();

        accordionControl1.SelectedElement = item;
        _currentItem = item;

        CurrentPage?.LoadSettings();
    }

    private void accordionControl1_SelectedElementChanged(object sender,
        DevExpress.XtraBars.Navigation.SelectedElementChangedEventArgs e)
    {
        if (e.Element == menSettingsExport || e.Element == menSettingsImport)
            accordionControl1.SelectedElement = _currentItem;
    }
}

/// <summary>
/// Interface, dass eine Seite/ein Control zum bearbeiten von EInstellunegn implementieren muss
/// </summary>
public interface ISettingsPage
{
    /// <summary>
    /// Einstellungen laden
    /// </summary>
    void LoadSettings();

    /// <summary>
    /// Einstellungen speichern
    /// </summary>
    /// <returns>true, wenn erfolgreich</returns>
    bool SaveSettings();

    /// <summary>
    /// Prüfen, ob die Seite geshlossen werden kann
    /// </summary>
    /// <returns>true wenn geschlossen werden kann, sonst false</returns>
    bool CanClose();
}
