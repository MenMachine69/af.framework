using System.Diagnostics;

namespace AF.CORE;

/// <summary>
/// AF bietet Standarddienste und Anwendungsinformationen.
/// </summary>
public static class AFCore
{
    private static AFApp? _app;
    private static bool? _designMode;
    private static string _dxVersion = string.Empty;
    private static readonly Dictionary<string, IPlaceholder> placeholder = [];

    #region exception handling

    /// <summary>
    /// Handler, der die Fehlerbehandlung durchführt.
    /// 
    /// Der Handler ist eine Observable, die die Fehlermeldung an die registrierten Beobachter weitergibt.
    /// Eine beliebige Anzahl von Beobachtern kann sich beim Handler registrieren.
    /// </summary>
    public static ExceptionHandler ExceptionHandler { get; set; } = new();


    /// <summary>
    /// Ereignis, das bei unbehandelten Thread-Ausnahmen ausgelöst wird.
    /// 
    /// Dieses Ereignis sollte gebunden werden, wenn die Anwendung gestartet wird.
    /// <code>
    /// AppDomain.CurrentDomain.UnhandledException += GlobalException.Handle_UnhandledException;
    /// Application.ThreadException += GlobalException.Handle_ThreadException;
    /// Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
    /// </code>
    /// </summary>
    /// <param name="sender">Absender/Quelle des Ereignisses</param>
    /// <param name="e">Parameter für das Ereignis</param>
    public static void OnThreadException(object sender, ThreadExceptionEventArgs e)
    {
        _handleException(e.Exception);
    }

    /// <summary>
    /// Ereignis, das bei unbehandelten Ausnahmen ausgelöst wird.
    /// 
    /// Dieses Ereignis sollte gebunden werden, wenn die Anwendung gestartet wird.
    /// <code>
    /// AppDomain.CurrentDomain.UnhandledException += GlobalException.Handle_UnhandledException;
    /// Application.ThreadException += GlobalException.Handle_ThreadException;
    /// Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
    /// </code>
    /// </summary>
    /// <param name="sender">Absender/Quelle des Ereignisses</param>
    /// <param name="e">Parameter für das Ereignis</param>
    public static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception) _handleException(exception);
    }

    /// <summary>
    /// Verarbeiten der Fehlermeldung, indem Sie sie an die registrierten bebachtere weitergeleitet wird.
    /// </summary>
    /// <param name="ex">Fehlermeldung</param>
    private static void _handleException(Exception ex)
    {
        ExceptionHandler.NotifyObservers(ex);
    }

    #endregion

    /// <summary>
    /// Zugriff auf das App-Objekt.
    /// 
    /// In AF3 wird jede Anwendung durch ein spezifisches 
    /// App-Objekt (abhängig vom Anwendungstyp) repräsentiert.
    /// </summary>
    public static AFApp App
    {
        get => _app ?? throw new NullReferenceException("No application object assigned");
        set => _app = value;
    }

    /// <summary>
    /// Gibt an, ob die Anwendung bereits erstellt ist.
    /// </summary>
    public static bool IsAppCreated => _app != null;

    /// <summary>
    /// Erkennen, ob die Anwendung derzeit in Visual Studio ausgeführt/verwendet wird.
    /// Diese Information wird i.d.R. für Designprozesse (Controls, Forms etc.) genutzt 
    /// um bestimmte Dinge abhängig vom Status auszuführen. Ein typisches Beispiel ist der 
    /// Constructor von Forms und Usercontrols.
    /// </summary>
    public static bool DesignMode => _designMode ??= (Process.GetCurrentProcess().ProcessName == "devenv" ||
                                                      Process.GetCurrentProcess().ProcessName == "DesignToolsServer");

    /// <summary>
    /// Datenbankzugriff auf die von der Anewendung genutzte Datenbank.
    /// 
    /// Verwendet die Anwendung keine Datenbank kann der Wert NULL sein.
    /// 
    /// Dies ist ein ShortCut zu AFCore.App.Data...
    /// </summary>
    public static StorageService? Data => App.StorageService;

    /// <summary>
    /// Security Service (Funktionen für Login, Benutzer- und Rechteverwaltung).
    /// 
    /// Verwendet die Anwendung keine Benutzersteuerung kann der Wert NULL sein.
    /// 
    /// Dies ist ein ShortCut zu AFCore.App.SecurityService... 
    /// </summary>
    public static ISecurityService? SecurityService => App.SecurityService;

    /// <summary>
    /// Scripting Service (ausführen von Scripten).
    ///
    /// Verwendet die Anwendung kein Scripting kann der Wert NULL sein.
    ///
    /// Dies ist ein ShortCut zu AFCore.App.ScriptingService...  
    /// </summary>
    public static IScriptService? ScriptingService => App.ScriptingService;

    /// <summary>
    /// Query Service (Datenbankabfragen ohne ORM)
    ///
    /// Verwendet die Anwendung keinen Query Service kann der Wert NULL sein.
    ///
    /// Dies ist ein ShortCut zu AFCore.App.QueryService...  
    /// </summary>
    public static IQueryService? QueryService => App.QueryService;

    /// <summary>
    /// EventHub, wo Ereignisse für Objekte abonniert werden können (neu erstellt, geändert, gelöscht, benutzerdefiniert).
    /// 
    /// Ein typischer Anwendungsfall sind IModel-Objekte, die bei z.B. Änderungen ein Ereignis auslösen, 
    /// dass andere Komponenten der Anwendung dann verarbeiten können um z.B. Listenansichten zu aktualisieren.
    /// 
    /// Dies ist ein ShortCut zu AFCore.App.EventHub...   
    /// </summary>
    public static EventHub EventHub => App.EventHub;

    /// <summary>
    /// Liefert die Typbeschreibung für eine Tabelle oder einen View anhand des Tabellen- oder ViewNames.
    /// <example>
    /// <code>
    /// var tdes = AF.GetTypeDescription("TBL_USER");
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="tableOrViewName">Name der Tabelle/des Views</param>
    /// <returns>TypeDescription für die Tabelle/den View</returns>
    public static TypeDescription? GetTypeDescription(string tableOrViewName)
    {
        return TypeEx.GetTypeDescriptions(tdesc  => 
        (tdesc.IsTable && tdesc.Table!.TableName.Equals(tableOrViewName, StringComparison.OrdinalIgnoreCase)) || 
        (tdesc.IsView && tdesc.View!.ViewName.Equals(tableOrViewName, StringComparison.OrdinalIgnoreCase))).FirstOrDefault();
    }

    /// <summary>
    /// Liefert den Controller für eine Tabelle oder einen View anhand des Tabellen- oder ViewNames.
    /// <example>
    /// <code>
    /// var tdes = AF.GetController("TBL_USER");
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="tableOrViewName">Name der Tabelle/des Views</param>
    /// <returns>Controller für die Tabelle/den View</returns>
    public static IController? GetController(string tableOrViewName)
    {
        return GetTypeDescription(tableOrViewName)?.GetController() ?? null;
    }

    /// <summary>
    /// Liste der verfügbaren Platzhalter
    /// </summary>
    public static Dictionary<string, IPlaceholder> Placeholder
    {
        get
        {
            if (placeholder.Count < 1)
                LoadDefaultPlaceholders();

            return placeholder;
        }
    }


    /// <summary>
    /// Eigene Platzhalter, die zusätzlich zu den Standard-Platzhaltern
    /// zur Verfügung stehen sollen.
    /// </summary>
    public static void RegisterApplicationPlaceholders(IEnumerable<IPlaceholder> placeholders)
    {
        if (placeholder.Count < 1)
            LoadDefaultPlaceholders();

        foreach (var ph in placeholders)
            placeholder[ph.Name] = ph;
    }

    private static void LoadDefaultPlaceholders()
    {
        placeholder.Add("#HEUTE#", new SystemPlatzhalter() { IsExpression = true, Expression = "Now().ToShortDateString()", Name = "#HEUTE#", Description = "aktuelles Datum (TT.MM.JJJJ)" });
        placeholder.Add("#MONAT#", new SystemPlatzhalter() { IsExpression = true, Expression = "Now().Month", Name = "#MONAT#", Description = "aktueller Monat (Ganzzahl)" });
        placeholder.Add("#JAHR#", new SystemPlatzhalter() { IsExpression = true, Expression = "Now().Year", Name = "#JAHR#", Description = "aktuelles Jahr (Ganzzahl)" });
        placeholder.Add("#TAG#", new SystemPlatzhalter() { IsExpression = true, Expression = "Now().Day", Name = "#TAG#", Description = "aktueller Tag (Ganzzahl)" });
        placeholder.Add("#GESTERN#", new SystemPlatzhalter() { IsExpression = true, Expression = "Now().Date.AddHours(-24).ToShortDateString()", Name = "#GESTERN#", Description = "gestriges Datum (TT.MM.JJJJ)" });
        placeholder.Add("#VORMONAT#", new SystemPlatzhalter() { IsExpression = true, Expression = "If (Now().Month = 1, 12, Now().Month - 1)", Name = "#VORMONAT#", Description = "aktueller Monat - 1 (Ganzzahl)" });
        placeholder.Add("#VORJAHR#", new SystemPlatzhalter() { IsExpression = true, Expression = "Now().Year - 1", Name = "#VORJAHR#", Description = "aktuelles Jahr - 1 (Ganzzahl)" });
        placeholder.Add("#VORTAG#", new SystemPlatzhalter() { IsExpression = true, Expression = "Now().Date.AddHours(-24).Day", Name = "#VORTAG#", Description = "aktueller Tag - 1 (Ganzzahl)" });
        placeholder.Add("#MORGEN#", new SystemPlatzhalter() { IsExpression = true, Expression = "Now().Date.AddHours(24).ToShortDateString()", Name = "#MORGEN#", Description = "morgiges Datum (TT.MM.JJJJ)" });
        placeholder.Add("#FOLGEMONAT#", new SystemPlatzhalter() { IsExpression = true, Expression = "If (Now().Month = 12, 1, Now().Month + 1)", Name = "#FOLGEMONAT#", Description = "aktueller Monat + 1 (Ganzzahl)" });
        placeholder.Add("#FOLGEJAHR#", new SystemPlatzhalter() { IsExpression = true, Expression = "Now().Year + 1", Name = "#FOLGEJAHR#", Description = "aktuelles Jahr des Monats + 1" });
        placeholder.Add("#FOLGETAG#", new SystemPlatzhalter() { IsExpression = true, Expression = "Now().Date.AddHours(24).Day", Name = "#FOLGETAG#", Description = "aktuelle Tag des Monats + 1 (Ganzzahl)" });
        placeholder.Add("#STUNDE#", new SystemPlatzhalter() { IsExpression = true, Expression = "Now().Hour", Name = "#STUNDE#", Description = "aktuelle Stunde (Ganzzahl)" });
        placeholder.Add("#MINUTE#", new SystemPlatzhalter() { IsExpression = true, Expression = "Now().Minute", Name = "#MINUTE#", Description = "aktuelle Minute (Ganzzahl)" });
        placeholder.Add("#EMPTYGUID#", new SystemPlatzhalter() { IsExpression = false, Expression = "'00000000-0000-0000-0000-000000000000'", Name = "#EMPTYGUID#", Description = "Text für eine leere Guid ('00000000-0000-0000-0000-000000000000')" });
        placeholder.Add("#DXVERSION#", new SystemPlatzhalter() { IsExpression = false, Expression = DXVersion, Name = "#DXVERSION#", Description = "Aktuelle in AF verwendete Version der DevExpress-Bibliotheken (zur Referenzierung dieser Bibliotheken in Scripten)." });
    }

    /// <summary>
    /// Aktuelle Version von DevExpress
    /// </summary>
    public static string DXVersion
    {
        get
        {
            if (_dxVersion.IsEmpty())
                loadDXVersion();
            return _dxVersion;
        }
    }

    private static void loadDXVersion()
    {
        FileInfo[] dxFiles = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("DevExpress.*", SearchOption.AllDirectories);
        int dxMajor = 0;
        int dxMinor = 0;
        foreach (FileInfo dxFile in dxFiles)
        {
            if (dxFile.Extension.ToLower() != ".dll")
                continue;
            FileVersionInfo dxVersion = FileVersionInfo.GetVersionInfo(dxFile.FullName);
            if (dxVersion.FileMajorPart > dxMajor)
            {
                dxMajor = dxVersion.FileMajorPart;
                dxMinor = dxVersion.FileMinorPart;
            }
            else if (dxVersion.FileMajorPart == dxMajor)
                dxMinor = Math.Max(dxMinor, dxVersion.FileMinorPart);
        }

        _dxVersion = "v" + dxMajor.ToString().Trim() + "." + dxMinor.ToString().Trim();
    }

    /// <summary>
    /// Zur Anwendung gehörende Assemblies
    /// </summary>
    public static string[] AppAssemblies { get; set; } = ["AF.CORE"];
}


