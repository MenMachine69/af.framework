using System.Drawing;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace AF.CORE;

/// <summary>
/// Klasse zur Behandlung von nicht abgefangenen Fehlern einer Anwendung (verwendet von ApplicationBase).
/// 
/// Die Klasse ist von Observable abgeleitet (siehe DesignPattern Observer), sodass man verschiedene Beobachter registrieren kann, an die 
/// die Fehler zum Zweck der Dokumentation etc. weitergegeben werden.
/// </summary>
public class ExceptionHandler : ObservableBase
{
    /// <summary>
    /// Constructor
    /// </summary>
    public ExceptionHandler()
    {

    }

    /// <summary>
    /// Behandelt eine Ausnahme
    /// </summary>
    /// <param name="exception">zu behandelnde Ausnahme</param>
    public virtual void HandleException(Exception exception)
    {
        NotifyObservers(exception);
    }
}

/// <summary>
/// Informationen zu einem aufgetretenen Fehler.
///
/// Serialisierbar, um die Daten auszutauschen.
/// </summary>
[Serializable]
public class ExceptionInfo
{
    /// <summary>
    /// Liste der Systeminformationen
    /// </summary>
    public List<KeyValue> SystemInformations { get; set; } = [];

    /// <summary>
    /// Liste der Screenshots
    /// </summary>
    public List<Bitmap> Screenshots { get; set; } = [];

    /// <summary>
    /// Liste der Screenshots
    /// </summary>
    public List<AFSystemInformation.AssemblyInformation> Assemblies { get; set; } = [];

    ///// <summary>
    ///// Liste der geladenen Assemblies
    ///// </summary>
    //public List<> Assemblies { get; set; } = [];

    /// <summary>
    /// Die eigentliche Ausnahme
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    public Exception? Exception { get; init; } 

    /// <summary>
    /// Constructor
    /// </summary>
    public ExceptionInfo(Exception exception)
    {
        Exception = exception;

        try
        {
            SystemInformations.Add(new("Username", AFSystemInformation.User.Name));
            SystemInformations.Add(new("Domain", AFSystemInformation.User.Domain));
            SystemInformations.Add(new("Computer", AFSystemInformation.Machine.Name));
            SystemInformations.Add(new("Windows-Version", AFSystemInformation.Machine.OSVersion));
            SystemInformations.Add(new("Ist Server", AFSystemInformation.Machine.IsWindowsServer ? "JA" : "nein"));
            SystemInformations.Add(new("Memory", (AFSystemInformation.Machine.TotalMemory / 1024 / 1024) + " MB"));
            SystemInformations.Add(new("CPU", AFSystemInformation.Machine.CPU.Name));
            SystemInformations.Add(new("CPU Cores", AFSystemInformation.Machine.CPU.Cores.ToString()));
            SystemInformations.Add(new("CPU Max Speed", AFSystemInformation.Machine.CPU.MaxSpeed + " MHz"));
            SystemInformations.Add(new("CPU Curr Speed", AFSystemInformation.Machine.CPU.CurrentSpeed+ " MHz"));
            SystemInformations.Add(new("CPU Auslastung", AFSystemInformation.Machine.CPU.CurrentLoad+" %"));

            var assemblies = AFSystemInformation.Assemblies;
            Assemblies.AddRange(assemblies);
        }
        catch
        {
            // nix unternehmen...
        }

        
    }
}
