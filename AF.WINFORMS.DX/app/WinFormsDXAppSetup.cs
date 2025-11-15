using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// setup properties for a WinFormsDX based desktop application
/// </summary>
public class WinFormsDXAppSetup : AppSetup
{
   
    /// <summary>
    /// Name of the skin to use in application. Default is 'Light'.    
    /// </summary>
    public string SkinNameLight { get; set; } = @"The Bezier";

    /// <summary>
    /// Name of the skin to use in application. Default is 'Dark'.    
    /// </summary>
    public string SkinNameDark { get; set; } = @"The Bezier";

    /// <summary>
    /// Name of the skin palette to use in application. Default is 'Default'.
    /// </summary>
    public string SkinPaletteLight { get; set; } = @"VS Light";
    
    /// <summary>
    /// Name of the skin palette to use in application. Default is 'Default'.
    /// </summary>
    public string SkinPaletteDark { get; set; } = @"VS Dark";

    /// <summary>
    /// SkinPatch laden. Im Patch wird u.a. dafür gesorgt, dass Eingabe-Controls den neuen Windows11 Border haben.
    /// </summary>
    [Obsolete("Nicht mehr verwenden, Skinpatch wird IMMER geladen!")]
    public bool LoadSkinPatch { get; set; } = true;

    /// <summary>
    /// Modus für Skalierung
    /// </summary>
    public eDPIScaleMode ScaleMode { get; set; } = eDPIScaleMode.PerMonitor;

    /// <summary>
    /// Schriftart-Modus
    /// </summary>
    public eSystemFont FontMode { get; set; } = eSystemFont.Default;

    /// <summary>
    /// Custom-Schriftart
    /// </summary>
    public string CustomFontName { get; set; } = "";

    /// <summary>
    /// Custom-Schriftgröße
    /// </summary>
    public float CustomFontSize { get; set; } = 8.25f;

    /// <summary>
    /// Anwendung wird mit einer RemoteConnection verwendet.
    /// </summary>
    public bool RemoteConnection { get; set; }
}

/// <summary>
/// Modus füpr Skalierung
/// </summary>
public enum eDPIScaleMode
{
    /// <summary>
    /// PerMonitor (empfohlen)
    /// </summary>
    PerMonitor = 0,
    /// <summary>
    /// Systemweit
    /// </summary>
    System = 1,
    /// <summary>
    /// Beide Varianten einschalten
    /// </summary>
    Both =2,
    /// <summary>
    /// Keine Skalierung verwenden
    /// </summary>
    Off = 3
}

/// <summary>
/// Schriftnutzung...
/// </summary>
public enum eSystemFont
{
    /// <summary>
    /// <see cref="WindowsFormsFontBehavior.Default"/>
    /// </summary>
    Default = 0,
    /// <summary>
    /// <see cref="WindowsFormsFontBehavior.UseTahoma"/>
    /// </summary>
    Tahoma = 1,
    /// <summary>
    /// <see cref="WindowsFormsFontBehavior.UseSegoeUI"/>
    /// </summary>
    SegoeUI = 2,
    /// <summary>
    /// <see cref="WindowsFormsFontBehavior.UseWindowsFont"/>
    /// </summary>
    Windows = 3,
    /// <summary>
    /// <see cref="WindowsFormsFontBehavior.UseControlFont"/>
    /// </summary>
    Control = 4,
    /// <summary>
    /// <see cref="WindowsFormsFontBehavior.ForceTahoma"/>
    /// </summary>
    TahomaForced = 5,
    /// <summary>
    /// <see cref="WindowsFormsFontBehavior.ForceSegoeUI"/>
    /// </summary>
    SegoeUIForced = 6,
    /// <summary>
    /// <see cref="WindowsFormsFontBehavior.ForceWindowsFont"/>
    /// </summary>
    WindowsForced = 7,
    /// <summary>
    /// <see cref="WindowsFormsSettings.DefaultFont"/>
    /// </summary>
    Custom = 8
}