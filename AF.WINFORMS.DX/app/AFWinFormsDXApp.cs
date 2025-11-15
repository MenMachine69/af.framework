using AF.MVC;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit;
using Microsoft.Win32;

namespace AF.WINFORMS.DX;

/// <summary>
/// abstract base class for all WinForms application with DevExpress support based on AF
/// </summary>
[SupportedOSPlatform("windows")]
public abstract class AFWinFormsDXApp : AFApp
{
    private string _skinNameLight;
    private string _skinNameDark;
    private string _skinPaletteLight;
    private string _skinPaletteDark;
    private eSkinMode _skinMode = eSkinMode.System;
    private IShell? _shell;

    /// <summary>
    /// Standardmanager für die Anzeige von Overlays.
    /// 
    /// Die Overlays werden in der Regel im Hauptfenster der Anwendung (Shell) angezeigt
    /// </summary>
    public AFOverlayDisplayManager? DefaultOverlayDisplayManager => Shell?.OverlayDisplayManager;

    /// <summary>
    /// Standardanzeige für FylOuts.
    /// 
    /// Die FylOuts werden in der Regel im Hauptfenster der Anwendung (SHell) angezeigt
    /// </summary>
    public AFFlyoutManager? DefaultFlyoutManager => Shell?.FlyoutManager;
    
    /// <summary>
    /// Constructor
    /// 
    /// Create application object and register it with AF. The application object can then be accessed via AFCore.App at any time.
    /// </summary>
    /// <param name="setup">Application startup configuration.</param>
    protected AFWinFormsDXApp(WinFormsDXAppSetup setup) : base(setup)
    {
        ShiftPressed = (Control.ModifierKeys == Keys.Shift);
        CtrlPressed = (Control.ModifierKeys == Keys.Control);
        AltPressed = (Control.ModifierKeys == Keys.Alt);

        DefaultMsgAnswerStorage = new MessageAnswerStorage();

        _skinNameLight = setup.SkinNameLight;
        _skinNameDark = setup.SkinNameDark;
        _skinPaletteLight = setup.SkinPaletteLight;
        _skinPaletteDark = setup.SkinPaletteDark;
    }

    /// <summary>
    /// Initialisieren der UI
    /// </summary>
    /// <param name="setup"></param>
    public void Init(WinFormsDXAppSetup setup)
    {
        // Fehlerbehandlung
        ExceptionHandler = new DefaultExceptionHandler();
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        Application.ThreadException += OnThreadException;
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

        _skinNameLight = setup.SkinNameLight;
        _skinNameDark = setup.SkinNameDark;
        _skinPaletteLight = setup.SkinPaletteLight;
        _skinPaletteDark = setup.SkinPaletteDark;

        FileInfo startup = new(Assembly.GetEntryAssembly()!.Location);

        SkinManager.Default.RegisterSkinPatch(Path.Combine(startup.Directory!.FullName, @"AF.skinpatch"));

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        switch (setup.ScaleMode)
        {
            case eDPIScaleMode.PerMonitor:
                WindowsFormsSettings.SetPerMonitorDpiAware();
                WindowsFormsSettings.AllowDpiScale = true;
                WindowsFormsSettings.AllowAutoScale = DefaultBoolean.True;
                break;
            case eDPIScaleMode.System:
                WindowsFormsSettings.SetDPIAware();
                WindowsFormsSettings.AllowDpiScale = true;
                WindowsFormsSettings.AllowAutoScale = DefaultBoolean.True;
                break;
            case eDPIScaleMode.Both:
                WindowsFormsSettings.SetPerMonitorDpiAware();
                WindowsFormsSettings.SetDPIAware();
                WindowsFormsSettings.AllowDpiScale = true;
                WindowsFormsSettings.AllowAutoScale = DefaultBoolean.True;
                break;
            case eDPIScaleMode.Off:
                WindowsFormsSettings.AllowDpiScale = false;
                WindowsFormsSettings.AllowAutoScale = DefaultBoolean.False;
                break;
        }

        switch (setup.FontMode)
        {
            case eSystemFont.Default:
                WindowsFormsSettings.FontBehavior = WindowsFormsFontBehavior.Default;
                break;
            case eSystemFont.Tahoma:
                WindowsFormsSettings.FontBehavior = WindowsFormsFontBehavior.UseTahoma;
                break;
            case eSystemFont.SegoeUI:
                WindowsFormsSettings.FontBehavior = WindowsFormsFontBehavior.UseSegoeUI;
                break;
            case eSystemFont.Windows:
                WindowsFormsSettings.FontBehavior = WindowsFormsFontBehavior.UseWindowsFont;
                break;
            case eSystemFont.Control:
                WindowsFormsSettings.FontBehavior = WindowsFormsFontBehavior.UseControlFont;
                break;
            case eSystemFont.TahomaForced:
                WindowsFormsSettings.FontBehavior = WindowsFormsFontBehavior.ForceTahoma;
                break;
            case eSystemFont.SegoeUIForced:
                WindowsFormsSettings.FontBehavior = WindowsFormsFontBehavior.ForceSegoeUI;
                break;
            case eSystemFont.WindowsForced:
                WindowsFormsSettings.FontBehavior = WindowsFormsFontBehavior.ForceWindowsFont;
                break;
            case eSystemFont.Custom:
                WindowsFormsSettings.FontBehavior = WindowsFormsFontBehavior.Default;
                WindowsFormsSettings.DefaultFont = new(setup.CustomFontName, setup.CustomFontSize);
                break;
        }

        WindowsFormsSettings.EnableFormSkins();
        WindowsFormsSettings.AutoCorrectForeColor = DefaultBoolean.True;
        WindowsFormsSettings.UseAdvancedTextEdit = DefaultBoolean.False;
        WindowsFormsSettings.UseDXDialogs = DefaultBoolean.True;
        WindowsFormsSettings.ScrollUIMode = ScrollUIMode.Fluent;
        WindowsFormsSettings.ShowTouchScrollBarOnMouseMove = true;
        WindowsFormsSettings.FocusRectStyle = DXDashStyle.Default;
        WindowsFormsSettings.FormThickBorder = false;

        WindowsFormsSettings.CustomizationFormSnapMode = SnapMode.OwnerControl;
        WindowsFormsSettings.FilterCriteriaDisplayStyle = FilterCriteriaDisplayStyle.Visual;
        WindowsFormsSettings.AllowPixelScrolling = DefaultBoolean.True;
        WindowsFormsSettings.AllowAutoFilterConditionChange = DefaultBoolean.True;

        WindowsFormsSettings.AnimationMode = AnimationMode.EnableAll;
        //WindowsFormsSettings.AllowHoverAnimation = DefaultBoolean.True;

        WindowsFormsSettings.AutoCorrectForeColor = DefaultBoolean.True;
        WindowsFormsSettings.ColumnFilterPopupMode = ColumnFilterPopupMode.Excel;
        WindowsFormsSettings.ColumnAutoFilterMode = ColumnAutoFilterMode.Default;
        WindowsFormsSettings.CompactUIMode = DefaultBoolean.False;
        WindowsFormsSettings.DefaultAllowHtmlDraw = true;

        WindowsFormsSettings.AllowDefaultSvgImages = DefaultBoolean.True;
        WindowsFormsSettings.SvgImageRenderingMode = SvgImageRenderingMode.Default;
        WindowsFormsSettings.SvgImageRenderingMode = SvgImageRenderingMode.HighQuality;
        WindowsFormsSettings.AllowRoundedWindowCorners = DefaultBoolean.True;

        WindowsFormsSettings.TrackWindowsAccentColor = DefaultBoolean.True;

        if (setup.RemoteConnection)
            WindowsFormsSettings.OptimizeRemoteConnectionPerformance = DefaultBoolean.True;

        //WindowsFormsSettings.LoadApplicationSettings();

        ToolTipController.DefaultController.AllowHtmlText = true;
        ToolTipController.DefaultController.ToolTipType = ToolTipType.SuperTip;
        ToolTipController.DefaultController.ToolTipStyle = ToolTipStyle.Default;

        DevExpress.XtraTab.Registrator.PaintStyleCollection.DefaultPaintStyles.Add(new AFFlatTabRegistrator());
        DevExpress.XtraTab.Registrator.PaintStyleCollection.DefaultPaintStyles.Add(new AFLineTabRegistrator());
        DevExpress.XtraTab.Registrator.PaintStyleCollection.DefaultPaintStyles.Add(new AFRoundedTabRegistrator());

        UI.DefaultStyleController.Appearance.FontSizeDelta = 0;
        UI.DefaultStyleController.Appearance.Options.UseFont = false; // true;
        UI.DefaultStyleController.LookAndFeel.SkinName = _skinNameLight;
        UI.DefaultStyleController.LookAndFeel.UseDefaultLookAndFeel = true; // false;
        UI.DefaultStyleController.AppearanceFocused.BackColor = Color.PaleGoldenrod;
        UI.DefaultStyleController.AppearanceFocused.ForeColor = Color.Black;
        UI.DefaultStyleController.AppearanceFocused.Options.UseBackColor = true;
        UI.DefaultStyleController.AppearanceFocused.Options.UseForeColor = true;
        UI.DefaultStyleController.AppearanceReadOnly.BackColor = Color.PaleVioletRed;
        UI.DefaultStyleController.AppearanceReadOnly.ForeColor = Color.Black;
        UI.DefaultStyleController.AppearanceReadOnly.Options.UseBackColor = true;
        UI.DefaultStyleController.AppearanceReadOnly.Options.UseForeColor = true;

        UI.UseBadgeImages = true;

        typeof(LabelControl).RegisterBindingProperty(nameof(LabelControl.Text));
        typeof(RichEditControl).RegisterBindingProperty(nameof(RichEditControl.RtfText));

        if (UI.IsWindowsDarkTheme())
        {
            UserLookAndFeel.Default.SetSkinStyle(_skinNameDark, _skinPaletteDark);
            UI.AdjustSkinColorsDark(_skinNameDark, _skinPaletteDark);
        }
        else
        {
            UserLookAndFeel.Default.SetSkinStyle(_skinNameLight, _skinPaletteLight);
            UI.AdjustSkinColorsLight(_skinNameLight, _skinPaletteLight);
        }

        // register a few default styles
        UI.RegisterStyle(@"Standard", new()
        {
            FontFace = SystemFonts.DefaultFont.Name,
            FontSize = SystemFonts.DefaultFont.Size,
            BackgroundColor = UI.TranslateSystemToSkinColor(SystemColors.Window),
            ForeColor = UI.TranslateSystemToSkinColor(SystemColors.WindowText),
            IgnoreStyleChanges = true
        });

        UI.RegisterStyle(@"Link", new(UI.GetStyle(@"Standard")!, foreColor: UI.TranslateToSkinColor(Color.Blue)));

        UI.RegisterStyle(@"Gray", new(UI.GetStyle(@"Standard")!, foreColor: UI.TranslateSystemToSkinColor(SystemColors.GrayText)));

        UI.RegisterStyle(@"GraySmall", new(UI.GetStyle(@"Standard")!, fontSize: 8.0f, foreColor: UI.TranslateSystemToSkinColor(SystemColors.GrayText)));

        UI.RegisterStyle(@"Small", new(UI.GetStyle(@"Standard")!, fontSize: 8.0f));

        UI.RegisterStyle(@"Bold", new(UI.GetStyle(@"Standard")!, bold: true));

        UI.RegisterStyle(@"BoldLink", new(UI.GetStyle(@"Standard")!, bold: true, foreColor: UI.TranslateToSkinColor(Color.Blue)));

        UI.RegisterStyle(@"Caption", new(UI.GetStyle(@"Standard")!, fontSize: 12.0f, foreColor: UI.TranslateSystemToSkinColor(SystemColors.WindowText), backgroundColor: UI.GetSkinPaletteColor("Paint Shadow")));

        UI.RegisterStyle(@"CaptionSmall", new(UI.GetStyle(@"Standard")!, fontSize: 10.0f, foreColor: UI.TranslateSystemToSkinColor(SystemColors.WindowText), backgroundColor: UI.GetSkinPaletteColor("Paint Shadow")));

        UI.RegisterStyle(@"CaptionH1", new(UI.GetStyle(@"Standard")!, fontSize: 12.0f, bold: true, foreColor: UI.TranslateSystemToSkinColor(SystemColors.WindowText)));

        UI.RegisterStyle(@"CaptionInverted", new(UI.GetStyle(@"Standard")!, fontSize: 12.0f, bold: false, backgroundColor: UI.TranslateSystemToSkinColor(SystemColors.WindowText), foreColor: UI.TranslateSystemToSkinColor(SystemColors.Window)));

        UI.RegisterStyle(@"CaptionH2", new(UI.GetStyle(@"Standard")!, fontSize: 10.0f, bold: false, foreColor: UI.TranslateSystemToSkinColor(SystemColors.Highlight)));

        UI.RegisterStyle(@"CaptionH3", new(UI.GetStyle(@"Standard")!, bold: true, foreColor: UI.TranslateSystemToSkinColor(SystemColors.Highlight)));

        UserLookAndFeel.Default.StyleChanged += styleChanged;
        SystemEvents.SessionSwitch += stateChanged;

        UI.BarController = new();

        UI.BarController.Controller.AppearancesBar.BarAppearance.Hovered.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Highlight);
        UI.BarController.Controller.AppearancesBar.BarAppearance.Hovered.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.HighlightText);
        UI.BarController.Controller.AppearancesBar.BarAppearance.Hovered.Options.UseForeColor = true;
        UI.BarController.Controller.AppearancesBar.BarAppearance.Hovered.Options.UseBackColor = true;
        UI.BarController.Controller.AppearancesBar.BarAppearance.Pressed.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.HighlightText);
        UI.BarController.Controller.AppearancesBar.BarAppearance.Pressed.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Highlight);
        UI.BarController.Controller.AppearancesBar.BarAppearance.Pressed.Options.UseForeColor = true;
        UI.BarController.Controller.AppearancesBar.BarAppearance.Hovered.Options.UseBackColor = true;
        UI.BarController.Controller.AppearancesBar.SubMenu.AppearanceMenu.Hovered.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.HighlightText);
        UI.BarController.Controller.AppearancesBar.SubMenu.AppearanceMenu.Hovered.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Highlight);
        UI.BarController.Controller.AppearancesBar.SubMenu.AppearanceMenu.Hovered.Options.UseForeColor = true;
        UI.BarController.Controller.AppearancesBar.SubMenu.AppearanceMenu.Hovered.Options.UseBackColor = true;
        UI.BarController.Controller.AppearancesBar.SubMenu.AppearanceMenu.Pressed.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.HighlightText);
        UI.BarController.Controller.AppearancesBar.SubMenu.AppearanceMenu.Pressed.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Highlight);
        UI.BarController.Controller.AppearancesBar.SubMenu.AppearanceMenu.Pressed.Options.UseForeColor = true;
        UI.BarController.Controller.AppearancesBar.SubMenu.AppearanceMenu.Pressed.Options.UseBackColor = true;

        // MVC related staff
        MvcContext.ModelInspektorType = typeof(FormModelInspector);

    }


    private void stateChanged(object sender, SessionSwitchEventArgs e)
    {
        if (e.Reason == SessionSwitchReason.SessionLock)
            IsLocked = true;
        else if (e.Reason == SessionSwitchReason.SessionUnlock)
            IsLocked = false;
    }

    private void styleChanged(object? sender, EventArgs e)
    {
        // apply changes to default styles
        var style = UI.GetStyle(UIStyle.STANDARD)!;
        style.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.WindowText);
        style.BackgroundColor = UI.TranslateSystemToSkinColor(SystemColors.Window);
        style = UI.GetStyle(UIStyle.SMALL)!;
        style.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.WindowText);
        style.BackgroundColor = UI.TranslateSystemToSkinColor(SystemColors.Window);
        style = UI.GetStyle(UIStyle.LINK)!;
        style.ForeColor = UI.TranslateToSkinColor(Color.Blue);
        style.BackgroundColor = UI.TranslateSystemToSkinColor(SystemColors.Window);
        style = UI.GetStyle(UIStyle.GRAY)!;
        style.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.GrayText);
        style.BackgroundColor = UI.TranslateSystemToSkinColor(SystemColors.Window);
        style = UI.GetStyle(UIStyle.GRAYSMALL)!;
        style.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.GrayText);
        style.BackgroundColor = UI.TranslateSystemToSkinColor(SystemColors.Window);
        style = UI.GetStyle(UIStyle.BOLD)!;
        style.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.WindowText);
        style.BackgroundColor = UI.TranslateSystemToSkinColor(SystemColors.Window);
        style = UI.GetStyle(UIStyle.BOLDLINK)!;
        style.ForeColor = UI.TranslateToSkinColor(Color.Blue);
        style.BackgroundColor = UI.TranslateSystemToSkinColor(SystemColors.Window);
        style = UI.GetStyle(UIStyle.CAPTION)!;
        style.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.WindowText);
        style.BackgroundColor = UI.GetSkinPaletteColor("Paint Shadow");
        style = UI.GetStyle(UIStyle.CAPTIONSMALL)!;
        style.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.WindowText);
        style.BackgroundColor = UI.GetSkinPaletteColor("Paint Shadow");
        style = UI.GetStyle(UIStyle.CAPTIONH1)!;
        style.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.WindowText);
        style.BackgroundColor = UI.TranslateSystemToSkinColor(SystemColors.Window);
        style = UI.GetStyle(UIStyle.CAPTIONH2)!;
        style.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.Highlight);
        style.BackgroundColor = UI.TranslateSystemToSkinColor(SystemColors.Window);
        style = UI.GetStyle(UIStyle.CAPTIONH3)!;
        style.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.Highlight);
        style.BackgroundColor = UI.TranslateSystemToSkinColor(SystemColors.Window);
        style = UI.GetStyle(UIStyle.CAPTIONINVERTED)!;
        style.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.Window);
        style.BackgroundColor = UI.TranslateSystemToSkinColor(SystemColors.WindowText);

        if (UI.BarController != null)
        {
            UI.BarController.Controller.AppearancesBar.BarAppearance.Hovered.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.HighlightText);
            UI.BarController.Controller.AppearancesBar.BarAppearance.Hovered.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Highlight);
            UI.BarController.Controller.AppearancesBar.BarAppearance.Pressed.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.HighlightText);
            UI.BarController.Controller.AppearancesBar.BarAppearance.Pressed.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Highlight);
            UI.BarController.Controller.AppearancesBar.SubMenu.AppearanceMenu.Hovered.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.HighlightText);
            UI.BarController.Controller.AppearancesBar.SubMenu.AppearanceMenu.Hovered.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Highlight);
            UI.BarController.Controller.AppearancesBar.SubMenu.AppearanceMenu.Pressed.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.HighlightText);
            UI.BarController.Controller.AppearancesBar.SubMenu.AppearanceMenu.Pressed.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Highlight);
        }

        UI.InvokeStyleChanged();
    }

    /// <summary>
    /// Set skin mode for application
    /// </summary>
    /// <param name="mode">mode to set</param>
    public void SetSkinMode(eSkinMode mode)
    {
        if (mode == _skinMode)
            return;

        if (mode == eSkinMode.Dark || (mode == eSkinMode.System && UI.IsWindowsDarkTheme()))
        {
            UserLookAndFeel.Default.SetSkinStyle(_skinNameDark, _skinPaletteDark);
            UI.AdjustSkinColorsDark(_skinNameDark, _skinPaletteDark);
           
        }
        else
        {
            UserLookAndFeel.Default.SetSkinStyle(_skinNameLight, _skinPaletteLight);
            UI.AdjustSkinColorsLight(_skinNameLight, _skinPaletteLight);
        }



        _skinMode = mode;
    }

    /// <summary>
    /// the current used default skin name
    /// </summary>
    /// <returns></returns>
    public string GetSkinName()
    {
        return (UI.IsDarkSkin ? _skinNameDark : _skinNameLight);
    }

    /// <summary>
    /// Current skin mode of the application
    /// </summary>
    /// <returns>skin mode</returns>
    public eSkinMode GetSkinMode()
    {
        return _skinMode;
    }

    /// <summary>
    /// Aktueller Splashscreen der Anwendung
    /// </summary>
    public FormSplash? Splash { get; set; }

    /// <summary>
    /// einen Splashscreen anzeigen
    /// </summary>
    /// <param name="backround">Hintergrundbild des Splash's. Das Bild bestimmt auch die Größe des Splashscreens</param>
    public void ShowSplash(Bitmap backround)
    {
        Splash = new FormSplash(backround);
        Splash.Show();
        Splash.ShowVersion(AFCore.App.Version.ToString());
    }

    #region MessageService
    /// <inheritdoc />
    public override void ShowMessage(MessageArguments args)
    {
        DefaultFlyoutManager?.ShowMessage(args);
    }
    #endregion
    
    /// <summary>
    /// Standardspeicher für Antworten aus der Anzeige einer MsgBox
    /// </summary>
    public MessageAnswerStorage DefaultMsgAnswerStorage { get; }

    /// <summary>
    /// SHIFT was pressed during starup...
    /// </summary>
    public bool ShiftPressed { get; }

    /// <summary>
    /// CTRL was pressed during starup...
    /// </summary>
    public bool CtrlPressed { get; }
    
    /// <summary>
    /// ALT was pressed during starup...
    /// </summary>
    public bool AltPressed { get; }
    
    /// <summary>
    /// Main window of the application
    /// </summary>
    public virtual IShell? Shell
    {
        get => _shell;
        set => _shell = value;
    }

    /// <summary>
    /// Access the current Shell (application main form) 
    /// </summary>
    /// <returns></returns>
    public IShell? GetShell() { return Shell; }

    /// <summary>
    /// Assign the current Shell (application main form)
    /// </summary>
    /// <param name="shell"></param>
    public void SetShell(IShell shell) { _shell = shell; }


    /// <summary>
    /// application mutex (if application can't be started twice)
    /// </summary>
    public Mutex? Mutex { get; private set; }

    /// <summary>
    /// Mutex für eine single instance Anwendung anlegen
    /// </summary>
    /// <param name="instance">Instanz der Anwendung</param>
    /// <returns>true, wenn erfolgreich, sonst false (Anwendung kläuft bereits)</returns>
    public bool CreateMutex(AFWinFormsDXApp instance)
    {
        Mutex = new Mutex(true, instance.ApplicationIdentifier.ToString(), out var created);

        if (created)
            return true;

        MessageBox.Show(string.Format(WinFormsStrings.CANTSTARTUPTWICE, instance.ApplicationName.ToUpper(), instance.ApplicationName), "ERROR", MessageBoxButtons.OK);

        return false;
    }

    /// <inheritdoc />
    public override void HandleResult(CommandResult result)
    {
        switch (result.Result)
        {
            case eNotificationType.None:
                return;
            case eNotificationType.Error:
            case eNotificationType.SystemError:
                ShowMessageError(result.ResultMessage);
                break;
            case eNotificationType.Warning:
                ShowMessageWarning(result.ResultMessage);
                break;
            case eNotificationType.Information:
            case eNotificationType.Success:
                ShowMessageInfo(result.ResultMessage);
                break;
            default:
                ShowMessageInfo(result.ResultMessage);
                break;
        }
    }


    /// <inheritdoc />
    public override eMessageBoxResult ShowMsgBox(object? owner, MessageBoxArguments args)
    {
        return MsgBox.Show(owner ?? Shell, args);
    }

    /// <inheritdoc />
    public override eMessageBoxResult ShowMsgBox(MessageBoxArguments args)
    {
        return MsgBox.Show(Shell, args);
    }

    /// <summary>
    /// Skaliert einen Wert anhand der DPI des Bildschirms, auf dem die Shell dargestellt wird.
    /// </summary>
    /// <param name="val">zu skalierender Wert</param>
    /// <returns>skalierter Wert</returns>
    public int ScaleDpi(int val)
    {
        if (UI.Shell is not XtraForm form) return val;

        return val * form.DeviceDpi / 96;
    }
}