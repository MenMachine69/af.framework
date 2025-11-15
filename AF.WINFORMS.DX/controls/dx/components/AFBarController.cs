using DevExpress.Utils;
using DevExpress.XtraBars;

namespace AF.WINFORMS.DX;

/// <summary>
/// Controller für Toolbars und Docking
/// <seealso cref="DevExpress.XtraBars.BarAndDockingController"/>
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Components")]
public class AFBarController : BarAndDockingController
{
    private bool autoBackColorInBars = false;

    /// <summary>
    /// Hintergrundfarbe der Bars automatisch an Fenster anpassen statt Skin-Farbe zu verwenden (SystemColors.Control)
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AutoBackColorInBars
    {
        get => autoBackColorInBars;
        set
        {
            autoBackColorInBars = value;
            if (autoBackColorInBars)
                updateBarBackColors();
        }
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public AFBarController() : base()
    {
        UI.StyleChanged += styleChanged;
    }

    private void styleChanged(object? sender, EventArgs e)
    {
        updateBarBackColors();
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        UI.StyleChanged -= styleChanged;

        base.Dispose(disposing);


    }

    private void updateBarBackColors()
    {
        if (!autoBackColorInBars) return;

        AppearancesBar.Bar.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Control);
        AppearancesBar.Dock.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Control);
        AppearancesBar.BarAppearance.Disabled.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Control);
    }
}