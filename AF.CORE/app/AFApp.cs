using System.Reflection;
using System.Runtime.InteropServices;

namespace AF.CORE;

/// <summary>
/// Basisklasse für alle Anwendungsobjekte. Diese Klasse kann nicht direkt verwendet werden.
/// </summary>
public abstract class AFApp : IMessageService
{
    private readonly IPersistance? persistance;
    private readonly AssemblyMonitor? assemblyMonitor;
    private StorageService? data;

    /// <summary>
    /// Versteckter Konstruktor
    /// 
    /// Erzeugt ein Anwendungsobjekt und registriert es bei AF. Auf das Anwendungsobjekt kann dann jederzeit über AF.App zugegriffen werden.    
    /// </summary>
    /// <param name="setup">Konfiguration der App.</param>
    protected AFApp(AppSetup setup)
    {
        if (setup.AppAssembly == null)
            throw new ArgumentNullException(nameof(setup.AppAssembly));

        DebugMode = DebugMode;

        var attribtitle = setup.AppAssembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), true).FirstOrDefault();

        if (attribtitle is AssemblyTitleAttribute atTitle)
            ApplicationName = atTitle.Title;

        var attribguid = setup.AppAssembly.GetCustomAttributes(typeof(GuidAttribute), true).FirstOrDefault();

        if (attribguid is GuidAttribute atGuid)
            ApplicationIdentifier = new Guid(atGuid.Value);

        PathApplication = new FileInfo(setup.AppAssembly.Location).DirectoryName ?? throw new NullReferenceException(CoreStrings.ERR_APPPATHNOTAVAILABLE);

        Version = VersionInformation.ReadFromAssembly(setup.AppAssembly);

        AFCore.App = this;

        // Assembly-Überwachung aktivieren
        // nur verfügbar, wenn DebugMode wahr ist
        if (setup.MonitorUsedAssemblies && setup.DebugMode)
        {
            assemblyMonitor = new();
            assemblyMonitor.Start();
        }

        if (setup.MonitorDatabases && setup.DebugMode) 
        {
            DatabaseMonitor = new();
        }

        // Assembly aus dem Programmverzeichnis oder dem Unterverzeichnis Assemblys/Assemblies laden, wenn vorhanden
        // 
        // Dient der Auflsöung von Assembly-Namen wenn die *.exe.config nicht vorhanden ist. 
        AppDomain.CurrentDomain.AssemblyResolve += loadAssembly;

        // DX Cleaner ausführen um überflüssige Dateien DevExpress-Dateien zu löschen
        DXCleaner cleaner = new(new DirectoryInfo(PathApplication));

        FileInfo[] toremove = cleaner.Analyze().GetAwaiter().GetResult();

        if (toremove.Length > 0)
            cleaner.Clear(toremove).GetAwaiter().GetResult();

        if (setup.Persistance != null)
        {
            persistance = setup.Persistance;
            persistance.ApplicationID = ApplicationIdentifier;
        }
    }

    #region exception handling

    /// <summary>
    /// Handler, der die Fehlerbehandlung durchführt
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
        ExceptionHandler.HandleException(ex);
    }

    #endregion

    /// <summary>
    /// Zugriff auf das Objekt, das für die Speicherung von Daten (Einstellungen usw.) verwendet wird
    /// </summary>
    public virtual IPersistance? Persistance => persistance;

    /// <summary>
    /// Zugriff auf den ViewManager der App (wenn vorhanden)
    /// </summary>
    public virtual IViewManager? ViewManager => null;

    /// <summary>
    /// Gibt an, ob der Computer, auf dem die Anwendung läuft gerade gesperrt ist
    /// </summary>
    public bool IsLocked { get; set; }

    /// <summary>
    /// Gibt an, ob die Anwendung im Debug-Modus ausgeführt wird. Dieser Debug-Modus muss nicht
    /// muss nicht unbedingt ein echter Debug-Modus sein (beim Start aus Visual Studio), sondern kann auch über
    /// Parameter z.B. beim Start initiiert werden.
    /// </summary>
    public bool DebugMode { get; }

    /// <summary>
    /// Used Assemblies
    /// 
    /// Only available if Assemblies are mononitored
    /// </summary>
    public Dictionary<string, Assembly>? UsedAssemblies => assemblyMonitor != null ? assemblyMonitor.UsedAssemblies : null;
    
    /// <summary>
    /// Version der Anwendung. Wird normalerweise aus dem 
    /// ausgeführten Programm (Assembly-Eigenschaften).
    /// </summary>
    public VersionInformation Version { get; }

    /// <summary>
    /// Argumente, die dem Programm beim Start über die Kommandozeile übergeben wurden.    /// </summary>
    public Dictionary<string, string> Parameters { get; set; } = new();

    /// <summary>
    /// Assemblies (Dlls) suchen und laden (falls vorhanden).
    ///
    /// Dieser AssemblyResolver sucht in den folgenden Verzeichnissen nach einer benötigten Baugruppe:
    ///
    /// Anwendungsverzeichnis
    /// Anwendungsverzeichnis\Assemblys
    /// Anwendungsverzeichnis\Assemblies
    /// Anwendungsverzeichnis\fb_embedded
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    /// <returns>geladene Assembly oder null</returns>
    internal Assembly? loadAssembly(object? sender, ResolveEventArgs args)
    {
        string filename = args.Name.Split([','], StringSplitOptions.RemoveEmptyEntries)[0].Trim() + @".dll";

        FileInfo fi = new(Path.Combine(PathApplication, filename));

        if (fi.Exists == false)
            fi = new(Path.Combine(PathApplication, @"Assemblys", filename));

        if (fi.Exists == false)
            fi = new(Path.Combine(PathApplication, @"Assemblies", filename));

        if (fi.Exists == false)
            fi = new(Path.Combine(PathApplication, @"fb_embedded", filename));

        return fi.Exists ? Assembly.LoadFrom(fi.FullName) : null;
    }

    /// <summary>
    /// Eindeutige GUID der Anwendung (aus der im Konstruktor angegebenen Assembly)    /// </summary>
    public Guid ApplicationIdentifier { get; }

    /// <summary>
    /// Der Name der Anwendung (aus der im Konstruktor angegebenen Assembly)
    /// </summary>
    public string ApplicationName { get; } = "";

    /// <summary>
    /// Verzeichnis für temporäre Dateien, die von der Anwendung verwendet werden.
    ///
    /// Der Name setzt sich aus dem Standard-TempPath des Systems und dem ApplicationIdentifier zusammen.    
    /// </summary>
    public string PathTemp => Path.Combine(Path.GetTempPath(), ApplicationIdentifier.ToString());

    /// <summary>
    /// Anwendungsverzeichnis (Verzeichnis, in dem sich die Startdatei befindet)
    /// </summary>
    public string PathApplication { get; }

    /// <summary>
    /// Das Verzeichnis, das als gemeinsames Repository für anwendungsspezifische Daten dient, die von allen Benutzern verwendet werden.    
    /// Setzt sich zusammen aus dem Standardverzeichnis und dem ApplicationName.
    /// Beispiel: C:\Users\Public\MeineApp\
    /// <seealso cref="Environment.SpecialFolder.CommonApplicationData"/>
    /// </summary>
    public string PathAllUserAppData => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), ApplicationName);


    /// <summary>
    /// Das Verzeichnis, das als gemeinsames Repository für anwendungsspezifische Daten für den aktuellen Roaming-Benutzer dient.
    /// Ein Roaming-Benutzer arbeitet auf mehr als einem Computer in einem Netz.
    /// Das Profil eines Roaming-Benutzers wird auf einem Server im Netz aufbewahrt und auf ein System geladen, wenn sich der Benutzer anmeldet (Roaming).    
    /// Setzt sich zusammen aus dem Standardverzeichnis und dem ApplicationName.
    /// Beispiel: C:\Users\MuellerH\AppData\Roaming\MeineApp\
    /// <seealso cref="Environment.SpecialFolder.ApplicationData"/>
    /// </summary>
    public string PathUserRoamingAppData => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);

    /// <summary>
    /// Das Verzeichnis, das als gemeinsames Repository für anwendungsspezifische Daten dient, die vom aktuellen Benutzer (ohne Roaming) verwendet werden.
    /// Setzt sich zusammen aus dem Standardverzeichnis und dem ApplicationName.
    /// Beispiel: C:\Users\MuellerH\AppData\Local\MeineApp\
    /// <seealso cref="Environment.SpecialFolder.LocalApplicationData"/>
    /// </summary>
    public string PathUserLocalAppData => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ApplicationName);

    /// <summary>
    /// Das Verzeichnis, das als gemeinsames Repository für Dokumente des aktuellen Benutzers dient (Roaming).    
    /// Setzt sich zusammen aus dem Standardverzeichnis und dem ApplicationName.
    /// Beispiel: C:\Users\MuellerH\Documents\MeineApp\
    /// <seealso cref="Environment.SpecialFolder.MyDocuments"/>
    /// </summary>
    public string PathUserDocuments => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), ApplicationName);

    /// <summary>
    /// Das Verzeichnis, das als gemeinsames Repository für Dokumente des aktuellen Benutzers dient (Roaming).    
    /// Setzt sich zusammen aus dem Standardverzeichnis und dem ApplicationName.
    /// Beispiel: C:\Users\MuellerH\Pictures\MeineApp\
    /// <seealso cref="Environment.SpecialFolder.MyPictures"/>
    /// </summary>
    public string PathUserImages => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), ApplicationName);


    #region MessageService
    /// <inheritdoc />
    public IMessageAnswerStorage AnswerStorage { get; set; } = new MessageAnswerStorage();

    /// <inheritdoc />
    public virtual void ShowMessage(MessageArguments args)
    {
        throw new NotImplementedException(CoreStrings.ERROR_MUSTBEOVERWRITEN);
    }

    /// <inheritdoc />
    public virtual void HandleResult(CommandResult result)
    {
        throw new NotImplementedException(CoreStrings.ERROR_MUSTBEOVERWRITEN);
    }

    /// <inheritdoc />
    public void ShowMessageError(string message, int timeout = 5)
    {
        ShowMessage(new MessageArguments(message)
        {
            TimeOut = timeout,
            Type = eNotificationType.Error
        });
    }

    /// <inheritdoc />
    public virtual void ShowMessageInfo(string message, int timeout = 5)
    {
        ShowMessage(new MessageArguments(message)
        {
            TimeOut = timeout,
            Type = eNotificationType.Information
        });
    }

    /// <inheritdoc />
    public virtual void ShowMessageWarning(string message, int timeout = 5)
    {
        ShowMessage(new MessageArguments(message)
        {
            TimeOut = timeout,
            Type = eNotificationType.Warning
        });
    }

    /// <inheritdoc />
    public virtual eMessageBoxResult ShowInfoOk(string message, string moreinfo)
    {
        return ShowInfoOk(message, moreinfo, 0);
    }

    /// <inheritdoc />
    public virtual eMessageBoxResult ShowInfoOk(string message, string moreinfo, int messageid)
    {
        return ShowMsgBox(new MessageBoxArguments
        {
            Caption = CoreStrings.INFORMATION.ToUpper(),
            Message = message,
            MoreInfo = moreinfo,
            MessageId = messageid,
            Buttons = eMessageBoxButton.OK,
            Icon = eMessageBoxIcon.Information,
            DefaultButton = eMessageBoxDefaultButton.Button1
        });
    }

    /// <inheritdoc />
    public virtual eMessageBoxResult ShowInfoOk(string message)
    {
        return ShowInfoOk(message, string.Empty, 0);
    }

    /// <inheritdoc />
    public virtual eMessageBoxResult ShowInfoOk(string message, int messageid)
    {
        return ShowInfoOk(message, string.Empty, messageid);
    }

    /// <inheritdoc />
    public virtual eMessageBoxResult ShowErrorOk(string message, string moreinfo)
    {
        return ShowErrorOk(message, moreinfo, 0);
    }

    /// <inheritdoc />
    public virtual eMessageBoxResult ShowErrorOk(string message, string moreinfo, int messageid)
    {
        return ShowMsgBox(new MessageBoxArguments
        {
            Caption = CoreStrings.ERROR.ToUpper(),
            Message = message,
            MoreInfo = moreinfo,
            MessageId = messageid,
            Buttons = eMessageBoxButton.OK,
            Icon = eMessageBoxIcon.Error,
            DefaultButton = eMessageBoxDefaultButton.Button1
        });
    }

    /// <inheritdoc />
    public virtual eMessageBoxResult ShowErrorOk(string message)
    {
        return ShowErrorOk(message, string.Empty, 0);
    }

    /// <inheritdoc />
    public virtual eMessageBoxResult ShowErrorOk(string message, int messageid)
    {
        return ShowErrorOk(message, string.Empty, messageid);
    }

    /// <inheritdoc />
    public virtual eMessageBoxResult ShowErrorYesNo(string message)
    {
        return ShowErrorYesNo(message, string.Empty, 0);
    }

    /// <inheritdoc />
    public virtual eMessageBoxResult ShowErrorYesNo(string message, int messageid)
    {
        return ShowErrorYesNo(message, string.Empty, messageid);
    }

    /// <inheritdoc />
    public virtual eMessageBoxResult ShowErrorYesNo(string message, string moreinfo)
    {
        return ShowErrorYesNo(message, moreinfo, 0);
    }

    /// <inheritdoc />
    public virtual eMessageBoxResult ShowErrorYesNo(string message, string moreinfo, int messageid)
    {
        return ShowMsgBox(new MessageBoxArguments
        {
            Caption = CoreStrings.ERROR.ToUpper(),
            Message = message,
            MoreInfo = moreinfo,
            MessageId = messageid,
            Buttons = eMessageBoxButton.YesNo,
            Icon = eMessageBoxIcon.Error,
            DefaultButton = eMessageBoxDefaultButton.Button1
        });
    }

    /// <inheritdoc />
    public virtual eMessageBoxResult ShowQuestionYesNo(string message)
    {
        return ShowQuestionYesNo(message, string.Empty, 0);
    }

    /// <inheritdoc />
    public virtual eMessageBoxResult ShowQuestionYesNo(string message, string moreinfo)
    {
        return ShowQuestionYesNo(message, moreinfo, 0);
    }

    /// <inheritdoc />
    public virtual eMessageBoxResult ShowQuestionYesNo(string message, int messageid)
    {
        return ShowQuestionYesNo(message, string.Empty, messageid);
    }

    /// <inheritdoc />
    public virtual eMessageBoxResult ShowQuestionYesNo(string message, string moreinfo, int messageid)
    {
        return ShowMsgBox(new MessageBoxArguments
        {
            Caption = CoreStrings.QUESTION.ToUpper(),
            Message = message,
            MoreInfo = moreinfo,
            MessageId = messageid,
            Buttons = eMessageBoxButton.YesNo,
            Icon = eMessageBoxIcon.Question,
            DefaultButton = eMessageBoxDefaultButton.Button1
        });
    }

    /// <inheritdoc />
    public virtual eMessageBoxResult ShowWarningOkCancel(string message)
    {
        return ShowWarningOkCancel(message, string.Empty, 0);
    }

    /// <inheritdoc />
    public virtual eMessageBoxResult ShowWarningOkCancel(string message, string moreinfo)
    {
        return ShowWarningOkCancel(message, moreinfo, 0);
    }

    /// <inheritdoc />
    public virtual eMessageBoxResult ShowWarningOkCancel(string message, int messageid)
    {
        return ShowWarningOkCancel(message, string.Empty, messageid);
    }

    /// <inheritdoc />
    public virtual eMessageBoxResult ShowWarningOkCancel(string message, string moreinfo, int messageid)
    {
        return ShowMsgBox(new MessageBoxArguments
        {
            Caption = CoreStrings.WARNING.ToUpper(),
            Message = message,
            MoreInfo = moreinfo,
            MessageId = messageid,
            Buttons = eMessageBoxButton.OKCancel,
            Icon = eMessageBoxIcon.Warning,
            DefaultButton = eMessageBoxDefaultButton.Button1
        });
    }

    /// <inheritdoc />
    public virtual eMessageBoxResult ShowMsgBox(MessageBoxArguments args)
    {
        return ShowMsgBox(null, args);
    }

    /// <inheritdoc />
    public virtual eMessageBoxResult ShowMsgBox(object? owner, MessageBoxArguments args)
    {
        throw new NotImplementedException(CoreStrings.ERROR_MUSTBEOVERWRITEN);
    }

    /// <inheritdoc />
    public virtual eMessageBoxResult ShowMsgBox(string message, string caption, eMessageBoxButton buttons, eMessageBoxIcon icon,
        eMessageBoxDefaultButton defaultButton)
    {
        return ShowMsgBox(new MessageBoxArguments
        {
            Caption = caption,
            Message = message,
            Buttons = buttons,
            Icon = icon,
            DefaultButton = defaultButton
        });
    }

    /// <inheritdoc />
    public virtual eMessageBoxResult ShowMsgBox(string message, string caption, eMessageBoxButton buttons, eMessageBoxIcon icon)
    {
        return ShowMsgBox(new MessageBoxArguments
        {
            Caption = caption,
            Message = message,
            Buttons = buttons,
            Icon = icon
        });
    }

    /// <inheritdoc />
    public virtual eMessageBoxResult ShowMsgBox(string message, string moreinfo, string caption, eMessageBoxButton buttons,
        eMessageBoxIcon icon)
    {
        return ShowMsgBox(new MessageBoxArguments
        {
            Caption = caption,
            MoreInfo = moreinfo,
            Message = message,
            Buttons = buttons,
            Icon = icon
        });
    }
    #endregion

    #region Security

    /// <summary>
    /// Sicherheitsanbieter (Funktionen für Anmeldung, Benutzer- und Rechteverwaltung)
    ///
    /// Das Security-Provider-Objekt muss vor der ersten Verwendung dieser Eigenschaft zugewiesen werden.
    /// </summary>
    /// <exception cref="NullReferenceException">Error if no security provider is assigned to the App object</exception>
    public virtual ISecurityService? SecurityService { get; set; }

    #endregion

    #region Scripting

    /// <summary>
    /// Anbieter von Scripting-Diensten
    ///
    /// Ausführen von Scripten in C#
    /// </summary>
    public virtual IScriptService? ScriptingService { get; set; }

    #endregion

    #region Querying

    /// <summary>
    /// Abfrage des Dienstanbieters
    ///
    /// Ausführen von Datenbankabfragen
    /// </summary>
    public virtual IQueryService? QueryService { get; set; }

    #endregion


    /// <summary>
    /// Datenbankzugriff (ORM)
    ///
    /// Der StorageService muss vor dem ersten Zugriff zugewiesen werden. 
    /// 
    /// Ist das Datenbank-Monitoring eingeschaltet, wird es automatisch für jede datenbank im Storage verwendet.
    /// </summary>
    /// <exception cref="NullReferenceException">Error if no AFStorage is assigned to the App object</exception>
    public virtual StorageService? StorageService { 
        get => data; 
        set
        {
            data = value;
            
            if (data == null) return;

            if (DatabaseMonitor == null) return;

            data.Databases.Values.ForEach(db => db.TraceAfterExecute = DatabaseMonitor.AfterExecute);
        } 
    }

    /// <summary>
    /// Datenbankzugriff (ORM).
    /// 
    /// Nur aus Kompatibilitätsgründen vorhanden stattdessen Storage verwenden!
    /// </summary>
    [Obsolete("StorageService statt Data verwenden!")]
    public virtual StorageService? Data 
    { get => StorageService; set => StorageService = value;}

    /// <summary>
    /// EventHub, wo Ereignisse für jedes Objekt abonniert werden können (neu erstellt, geändert, gelöscht, benutzerdefiniert).
    ///
    /// Der EventHub ermöglicht das Abonnieren von Ereignissen bei der Änderung von Datenobjekten usw.
    /// </summary>
    public EventHub EventHub { get; set; } = new();


    /// <summary>
    /// Sprache für UI der Oberfläche setzen
    /// </summary>
    /// <param name="language"></param>
    public void SetLanguage(string language)
    {
        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(language);
        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(language);
    }

    /// <summary>
    /// Monitor für Datenbankaktivitäten.
    /// 
    /// Der Monitor wird für jede Datenbank des Storage an TraceAfterExecute gebunden und 
    /// gibt Auskunft über die ausgeführten Abfragen in den Datenbanken.
    /// </summary>
    public DatabaseMonitor? DatabaseMonitor { get; private set; }

    #region Tools
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
        return TypeEx.GetTypeDescriptions(tdesc => tdesc.Table?.TableName == tableOrViewName || tdesc.View?.ViewName == tableOrViewName).FirstOrDefault();

    }
    #endregion
}