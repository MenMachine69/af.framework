using AF.MVC;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Frames;
using DevExpress.Utils.Svg;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using Microsoft.Win32;
using System.Diagnostics;
using System.Drawing.Text;

namespace AF.WINFORMS.DX;

/// <summary>
/// UI der Anwendung
/// </summary>
[SupportedOSPlatform("windows")]
public static class UI 
{
    private static bool? _designMode;
    private static Font? _consoleFont;
    private static float? _dpi;
    private static readonly Dictionary<string, UIStyle> _styles = [];
    private static WeakEvent<EventHandler<EventArgs>>? _styleChanged;
    private static AppearanceObject _defaultAppearance = new()
    {
        ForeColor = UI.TranslateSystemToSkinColor(SystemColors.ControlText),
        TextOptions = { WordWrap = WordWrap.Wrap, HAlignment = HorzAlignment.Near, VAlignment = VertAlignment.Top },
        Options = { UseTextOptions = true, UseForeColor = true },
    };

    /// <summary>
    /// Standard-Appearance-Objekt:
    /// WordRap, HAlignment.Near, VAlignment.Top, ForeColor: ControlText
    /// </summary>
    public static AppearanceObject DefaultAppearance => _defaultAppearance;
    
    /// <summary>
    /// Standardcontroller für docking und toolbars/menus.
    /// </summary>
    public static DefaultBarAndDockingController? BarController { get; internal set; }

    /// <summary>
    /// current Splash-Screen
    /// </summary>
    public static FormSplash? Splash { get; set; }

    /// <summary>
    /// Hauptfenster der Anwendung.
    /// </summary>
    public static IShell Shell => ((AFWinFormsDXApp)AFCore.App).Shell!;

    /// <summary>
    /// Zugriff auf die Shell der MVC Anwendung
    /// </summary>
    public static IShellMVC MVCShell => ((AFWinFormsMVCApp)AFCore.App).Shell;

    /// <summary>
    /// Zugriff auf den ViewManager der MVC Anwendung
    /// </summary>
    public static AFViewManager ViewManager => MVCShell.ViewManager;

    /// <summary>
    /// Die Anwendung (App)
    /// </summary>
    public static AFWinFormsDXApp App => (AFWinFormsDXApp)AFCore.App;

    /// <summary>
    /// Gibt an, ob die Anwendung im Design-Modus läuft.
    /// Der Wert ist true, wenn im VS Designer ein Fenster oder UserControl entworfen wird.
    /// 
    /// Der Wert sollte genutzt werden um in UserControls und Fenstern Code nur dann auszuführen, wenn das Control/Fenster 
    /// nicht im Designer angezeigt wird.
    /// </summary>
    public static bool DesignMode => _designMode ??= (Process.GetCurrentProcess().ProcessName == "devenv" ||
                                                      Process.GetCurrentProcess().ProcessName == "DesignToolsServer");

    /// <summary>
    /// Panels (AFTablePanel etc.) automatisch skalieren in HighDPI Umgebungen.
    /// </summary>
    public static bool AutoScalePanels { get; set; } = true;

    /// <summary>
    /// Einen UI-Stil für die spätere Verwendung registrieren.
    /// 
    /// UI-Stile kännen z.B. zur Formatierung von Label-Controls verwendet werden.
    /// </summary>
    /// <param name="name">Name unter dem der Stil registriert wird</param>
    /// <param name="style">Stil-Definition</param>
    public static void RegisterStyle(string name, UIStyle style)
    {
        _styles.AddOrReplace(name, style);
    }

    /// <summary>
    /// Liefert einen registrierten Stil anhand seines Namens.
    /// </summary>
    /// <param name="name">Name des Stils</param>
    /// <returns>Stil-Definition oder NULL</returns>
    public static UIStyle? GetStyle(string name)
    {
        return _styles.TryGetValue(name, out var style) ? style : null;
    }

    /// <summary>
    /// Skin-Modus explizit setzen (Light/Dark oder wie System/Windows)
    /// </summary>
    /// <param name="mode">Modus</param>
    public static void SetSkinMode(eSkinMode mode)
    {
        ((AFWinFormsDXApp)AFCore.App).SetSkinMode(mode);
    }

    /// <summary>
    /// Der aktuell von der Anwendung verwendete Skin.
    /// </summary>
    /// <returns>Name des Skins</returns>
    public static string GetSkinName()
    {
         return ((AFWinFormsDXApp)AFCore.App).GetSkinName();
    }

    /// <summary>
    /// Einen Splash-Screen (Startfenster) für die Anwendung anzeigen.
    /// </summary>
    /// <param name="application">Anwendung, für die der Splash angezeigt wird</param>.
    /// <param name="backround">Hintergrundbild des Splashs. Das Bild bestimmt auch die Größe des Splash-Screens (Empfehlung: 600 * 400)</param>
    public static void ShowSplash(AFWinFormsDXApp application, Bitmap backround)
    {
        Splash = new FormSplash(backround);
        Splash.TopMost = true;
        Splash.ShowInTaskbar = true;
        Splash.Show();
        Splash.ChangeTopMost(true);
        Splash.ShowVersion(application.Version.ToString());
    }

    /// <summary>
    /// Splash-Screen aktualisieren
    /// </summary>
    /// <param name="captionMessage">Überschrift (z.B. Bitte warten...)</param>
    /// <param name="progressStep">Beschreibung des aktuellen Start-Schritts (z.B. Datenbank wird geprüft.)</param>
    public static void UpdateSplash(string captionMessage, string progressStep)
    {
        Splash?.ShowMessage(captionMessage);
        Splash?.ShowProgress(progressStep);
    }

    /// <summary>
    /// Den aktuell geöffneten Splash-Screen schließen
    /// </summary>
    public static void CloseSplash()
    {
        if (Splash == null) return;

        Splash.Close();
        Splash = null;
    }

    /// <summary>
    /// Auswahl eines Mandanten im SplashScreen.
    /// </summary>
    /// <param name="files">Liste der Dateien mit den Mandanten-Konfigurationen</param>
    /// <returns>Ausgewählte Datei mit der Mandantenkonfiguration oder NULL</returns>
    public static FileInfo? SelectMandant(FileInfo[] files)
    {
        return files.Length == 0 ? null : Splash?.SelectMandant(files);
    }

    /// <summary>
    /// Aktueller Skin-Modus der Anwendung (Light, Dark oder System/Auto)
    /// </summary>
    /// <returns>Modus</returns>
    public static eSkinMode GetSkinMode()
    {
        return AFCore.App is AFWinFormsDXApp ? (AFCore.App as AFWinFormsDXApp)!.GetSkinMode() : eSkinMode.System;
    }

    /// <summary>
    /// Zwischen Light und Dark Skin umschalten.
    /// </summary>
    public static void ToggleSkinMode()
    {
        switch (GetSkinMode())
        {
            case eSkinMode.Dark:
                SetSkinMode(eSkinMode.Light);
                break;
            case eSkinMode.Light:
                SetSkinMode(eSkinMode.Dark);
                break;
            case eSkinMode.System:
                SetSkinMode(IsDarkSkin ? eSkinMode.Light : eSkinMode.Dark);
                break;
            default:
                SetSkinMode(eSkinMode.System);
                break;
        }
    }

    /// <summary>
    /// StyleController, der für alle Controls verwendet wird, für die UseDefaultStyle = true gesetzt ist (über AFBaseEditExtender).
    /// </summary>
    public static IStyleController DefaultStyleController { get; set; } = new StyleController();

    /// <summary>
    /// Zeigt an, ob es sich bei dem aktuellen Skin um einen 'DarkSkin' (inverse Darstellung) handelt.
    /// </summary>
    public static bool IsDarkSkin => FrameHelper.IsDarkSkin(UserLookAndFeel.Default);

    /// <summary>
    /// Symbol (SVG) aus der Symbolbibliothek.
    /// </summary>
    /// <param name="imgName">Name des Symbols</param>
    /// <returns>das Symbol (SVG) aus der Symbolbibliothek</returns>
    public static SvgImage GetImage(Symbol imgName) => Glyphs.GetImage(imgName);


    /// <summary>
    /// ObjektSymbol (SVG) aus der ObjektSymbole-Bibliothek.
    /// </summary>
    /// <param name="imgName">Name des ObjektSymbols</param>
    /// <returns>das Symbol (SVG) aus der ObjektSymbole-Bibliothek</returns>
    public static SvgImage GetObjectImage(ObjectImages imgName) => Glyphs.GetObjectImage(imgName);

    /// <summary>
    /// Schwarzweißes ObjektSymbol (SVG) aus der ObjektSymbole-Bibliothek.
    /// </summary>
    /// <param name="imgName">Name des ObjektSymbols</param>
    /// <returns>das Symbol (SVG) aus der ObjektSymbole-Bibliothek</returns>
    public static SvgImage GetObjectImageSw(ObjectImages imgName) => Glyphs.GetObjectImageSw(imgName);


    /// <summary>
    /// LookAndFeel der Anwendung
    /// <see cref="DevExpress.XtraEditors.WindowsFormsSettings.DefaultLookAndFeel"/>
    /// </summary>
    public static UserLookAndFeel LookAndFeel => WindowsFormsSettings.DefaultLookAndFeel;


    /// <summary>
    /// Verwendung von Badge-Bildern anstelle von Standardbildern für einige visuelle Elemente
    /// </summary>
    public static bool UseBadgeImages { get; set; }

    /// <summary>
    /// Einen SuperTooltip erstellen
    /// </summary>
    /// <param name="caption">Beschriftung</param>
    /// <param name="text">Haupttext</param>
    /// <param name="footer">Fußzeilentext</param>
    /// <param name="mainimage">Bild, das rechts vom Text angezeigt werden soll</param>
    /// <param name="footerimage">Bild, das vor dem Fußzeilentext angezeigt werden soll</param>
    /// <returns>der erstellte SuperTooltip</returns>
    public static SuperToolTip GetSuperTip(string caption, string text, string? footer = null, Image? mainimage = null,
        Image? footerimage = null)
    {
        SuperToolTip ret = new() { AllowHtmlText = DefaultBoolean.True };

        ToolTipTitleItem titleitem = new()
        {
            Text = caption
        };
        ret.Items.Add(titleitem);

        ToolTipItem mainitem = new()
        {
            LeftIndent = 6,
            Text = text
        };

        if (mainimage != null)
        {
            mainitem.Appearance.Options.UseImage = true;
            mainitem.Appearance.Image = mainimage;
            mainitem.Image = mainimage;
        }

        ret.Items.Add(mainitem);

        if (footer.IsEmpty()) return ret;


        ret.Items.Add(new ToolTipSeparatorItem());
        ToolTipTitleItem footeritem = new()
        {
            Text = footer,
            LeftIndent = 6
        };

        if (footerimage != null)
        {
            footeritem.Appearance.Options.UseImage = true;
            footeritem.Appearance.Image = footerimage;
            footeritem.Image = footerimage;
        }

        ret.Items.Add(footeritem);


        return ret;
    }


    /// <summary>
    /// Modale 'Bitte warten' Nachricht für die Anwendung anzeigen.
    /// </summary>
    /// <param name="caption">Überschrift (z.B. Bitte warten)</param>
    /// <param name="message">Nachricht (z.B. Daten werden geladen...)</param>
    public static void ShowWait(string caption, string message)
    {
        //SplashScreenManager.Default.
        SplashScreenManager.ShowForm(null, typeof(AFWaitForm), true, true, false, 0 );
        SplashScreenManager splashScreenManager = SplashScreenManager.Default;
        if (!string.IsNullOrEmpty(caption))
            splashScreenManager.SetWaitFormCaption(caption);
        if (string.IsNullOrEmpty(message))
            return;
        splashScreenManager.SetWaitFormDescription(message);
        //SplashScreenManager.ShowDefaultWaitForm(caption, message);

    }

    /// <summary>
    /// Hide/Close the default 'Please wait' form displayed by ShowWait method.
    /// </summary>
    public static void HideWait()
    {
        try
        {
            SplashScreenManager.CloseForm();
        }
        catch { }
    }


    /// <summary>
    /// Farbe um Elemente zu überblenden.
    /// Schwarz für LightSkin, weiß für DarkSkin
    /// </summary>
    public static Color BlendColor => (IsDarkSkin ? Color.White : Color.Black);

    /// <summary>
    /// Farbe aus einer ColorPalette.
    /// </summary>
    /// <param name="colorname">Name der Farbe</param>
    /// <returns>Farbe</returns>
    public static Color GetSkinPaletteColor(string colorname)
    {
        if (colorname == "@Red" || colorname == "Red") return Color.FromArgb(255, 89, 94);

        if (colorname == "@Green" || colorname == "Green") return Color.FromArgb(138, 201, 38);

        if (colorname == "@Yellow" || colorname == "Yellow") return Color.FromArgb(255, 202, 58);

        if (colorname == "@Blue" || colorname == "Blue") return Color.FromArgb(25, 130, 196);

        var commonSkin = CommonSkins.GetSkin(UserLookAndFeel.Default);
        SvgPalette svgPalette = commonSkin.SvgPalettes[Skin.DefaultSkinPaletteName];

        Color ret;

        SvgColor? svgcolor = svgPalette.Colors.FirstOrDefault(col =>
            col.Name.Replace(@" ", "") == colorname.Replace(@" ", ""));

        if (svgcolor != null)
            ret = svgcolor.Value;
        else
        {
            ret = colorname switch
            {
                "KeyPaint" => Color.FromArgb(71, 71, 71),
                "KeyBrush" => Color.FromArgb(255, 255, 255),
                "KeyBrushLight" => Color.FromArgb(150, 150, 150),
                _ => svgPalette.GetColor(colorname)
            };
        }

        return ret;
    }

    /// <summary>
    /// Alle Farben aus einer ColorPalette.
    /// </summary>
    /// <returns>Farbe</returns>
    public static List<Tuple<string, Color>> GetSkinPaletteColors()
    {
        var commonSkin = CommonSkins.GetSkin(UserLookAndFeel.Default);
        SvgPalette svgPalette = commonSkin.SvgPalettes[Skin.DefaultSkinPaletteName];

        List<Tuple<string, Color>> ret = [];

        foreach ( var col in svgPalette.Colors)
            ret.Add(new(col.Name, col.Value));

        return ret;
    }


    /// <summary>
    /// Übersetzt eine Window-Standardfarbe (SystemColors) in die passende Skin-Farbe
    /// </summary>
    /// <param name="color"></param>
    /// <returns>übersetzte Farbe</returns>
    public static Color TranslateSystemToSkinColor(Color color)
    {
        if (color.Equals(SystemColors.Highlight))
            return GetSkinPaletteColor(@"AccentPaint");
        else if (color.Equals(SystemColors.HighlightText))
            return GetSkinPaletteColor(@"AccentBrush");

        var commonSkin = CommonSkins.GetSkin(UserLookAndFeel.Default);
        return commonSkin.GetSystemColor(color);
    }

    /// <summary>
    /// Übersetzt eine Farbe in die passende Skin-Farbe
    /// </summary>
    /// <param name="color"></param>
    /// <returns>übersetzte Farbe</returns>
    public static Color TranslateToSkinColor(Color color)
    {
        if (color.Equals(Color.Red))
            return GetSkinPaletteColor(@"Red");

        if (color.Equals(Color.Green))
            return GetSkinPaletteColor(@"Green");

        if (color.Equals(Color.Blue))
            return GetSkinPaletteColor(@"Blue");

        if (color.Equals(Color.Yellow))
            return GetSkinPaletteColor(@"Yellow");

        if (color.Equals(Color.Gray))
            return GetSkinPaletteColor(@"Gray");

        if (color.Equals(Color.White))
            return GetSkinPaletteColor(@"White");

        if (color.Equals(Color.Black))
            return GetSkinPaletteColor(@"Black");

        if (color.Equals(Color.FromArgb(0, 0, 0)))
            return GetSkinPaletteColor(@"Black");

        if (color.Equals(Color.FromArgb(255, 255, 255)))
            return GetSkinPaletteColor(@"White");

        var commonSkin = CommonSkins.GetSkin(UserLookAndFeel.Default);
        return commonSkin.TranslateColor(color);
    }

    
    /// <summary>
    /// aktuelle DPI-Auflösung des Bildschirms
    /// </summary>
    public static float Dpi
    {
        get
        {
            if (_dpi != null) return (float)_dpi;

            using (Graphics ctrl = Graphics.FromHwnd(IntPtr.Zero))
                _dpi = ctrl.DpiX;

            return (float)_dpi;
        }
    }


    /// <summary>
    /// Skalierung (DPI) setzen, wenn diese sich geändert hat...
    /// </summary>
    /// <param name="newdpi">neuer Wert</param>
    public static void ResetDpiTo(float newdpi)
    {
        _dpi = newdpi;
    }

    /// <summary>
    /// Faktor für die Skalierung von Bildschirmelementen (basierend auf der DPI-Auflösung des Bildschirms)
    /// </summary>
    public static float ScaleFactor => (Dpi * 100.0f / 96.0f) / 100.0f;

    /// <summary>
    /// Skaliert einen Float-Wert anhand der DPI-Einstellungen
    /// </summary>
    /// <param name="value">zu skalierender Wert</param>
    /// <returns>skalierter Wert</returns>
    public static float GetScaled(float value)
    {
        return value * ScaleFactor;
    }

    /// <summary>
    /// Skaliert einen Integer-Wert anhand der DPI-Einstellungen
    /// </summary>
    /// <param name="value">zu skalierender Wert</param>
    /// <returns>skalierter Wert</returns>
    public static int GetScaled(int value)
    {
        return Convert.ToInt32(Math.Round(Convert.ToDouble(value) * ScaleFactor, 0));
    }

    /// <summary>
    /// Schriftart mit fester Zeichenbreite (monospace), die in Situationen verwendet werden kann, 
    /// in denen eine solche Schriftart erforderlich ist (z. B. bei Quellcode-Editoren usw.)
    /// </summary>
    public static Font ConsoleFont
    {
        get
        {
            if (_consoleFont != null) return _consoleFont;

            bool exist = new InstalledFontCollection().Families.Any(fontFamily => fontFamily.Name.ToLower().StartsWith(@"fira code"));

            _consoleFont = exist ? new Font(@"Fira Code Medium", 12.0f) : new Font(@"Consolas", 11.0f);

            return _consoleFont;
        }
    }

    /// <summary>
    /// Prüfen Sie, ob Windows aktuell ein dunkles Farbschema verwendet
    /// </summary>
    /// <returns></returns>
    public static bool IsWindowsDarkTheme()
    {
        using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");

        var registryValueObject = key?.GetValue("AppsUseLightTheme");

        if (registryValueObject == null)
            return false;

        return (int)registryValueObject <= 0;
    }

    /// <summary>
    /// Ereignis, das ausgelöst wird, wenn der Style/Skin geändert wurde
    /// </summary>
    public static event EventHandler<EventArgs> StyleChanged
    {
        add
        {
            _styleChanged ??= new();
            _styleChanged.Add(value);
        }
        remove => _styleChanged?.Remove(value);
    }



    /// <summary>
    /// Den StyleChanged-Event auslösen
    /// </summary>
    public static void InvokeStyleChanged()
    {
        _defaultAppearance.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.ControlText);
        _styleChanged?.Raise(App, EventArgs.Empty);
    }

    /// <summary>
    /// Farben anpassen wenn DarkMode geladen
    /// </summary>
    public static void AdjustSkinColorsDark(string skin, string palette)
    {
        if (palette == "Fireball")
        {
            var commonSkin = CommonSkins.GetSkin(UserLookAndFeel.Default);
            SvgPalette svgPalette = commonSkin.SvgPalettes[Skin.DefaultSkinPaletteName];
            SvgColor? svgcolor = svgPalette.Colors.FirstOrDefault(col =>
                col.Name.Replace(@" ", "") == "PaintHigh");

            if (svgcolor != null)
                svgcolor.Value = Color.FromArgb(25, 36, 54);
        }
    }

    /// <summary>
    /// Farben anpassen, wenn LightMode geladen
    /// </summary>
    public static void AdjustSkinColorsLight(string skin, string palette)
    {
        // nix at the moment
    }
}