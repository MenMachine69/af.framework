using System.Reflection;

namespace AF.CORE;

/// <summary>
/// Einrichtungskonfiguration für die App.
/// </summary>
public class AppSetup
{
    /// <summary>
    /// Assembly der Anwendung. Normalerweise ist dies Assembly.GetEntryAssembly().
    /// Aus dieser Assembly werden die Anwendungs-ID, der Anwendungsname und die Versionsinformationen ausgelesen.    
    /// </summary>
    public Assembly AppAssembly { get; set; } = Assembly.GetEntryAssembly()!;


    /// <summary>
    /// Gibt an, ob die Anwendung im Debug-Modus ausgeführt wird. Dieser Debug-Modus muss nicht
    /// unbedingt ein echter Debug-Modus sein (beim Start aus Visual Studio), sondern kann auch über
    /// Parameter z.B. beim Start initiiert werden.
    /// </summary>
    public bool DebugMode { get; set; }

    /// <summary>
    /// Objekt, das als Modul für die Persistenz der Daten (Einstellungen usw.) verwendet werden soll.
    /// </summary>
    public IPersistance? Persistance { get; set; }  

    /// <summary>
    /// Verwendete Assemblies überwachen.
    /// 
    /// Wird diese Funktion eingeschaltet, überwacht das Programm automatisch die verwendeten/geladenen 
    /// Assemblies. Dies können dann als Protokoll in eine Datei geschrieben oder zur Bereinigung des 
    /// Build-Verzeichnisses verwenendet werden.
    /// 
    /// Ist nur verfügbar, wenn DebugMode = true ist.
    /// </summary>
    public bool MonitorUsedAssemblies {get; set;}

    /// <summary>
    /// Datenbanken überwachen.
    /// 
    /// Ist nur verfügbar, wenn DebugMode = true ist.
    /// </summary>
    public bool MonitorDatabases {get; set;}
}