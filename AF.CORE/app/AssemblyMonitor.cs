using System.Reflection;

namespace AF.CORE;

/// <summary>
/// Überwachung, um zu sehen, welche Assemblies geladen werden, während eine Anwendung läuft.
/// 
/// Das Ergebnis kann zum Optimieren der Ausgabe vor dem Packen und Veröffentlichen 
/// einer Anwendung (Entfernen nicht verwendeter Assembly-Dateien)
/// </summary>
public sealed class AssemblyMonitor : IDisposable
{
    private Timer? _timer;
    private readonly Dictionary<string, Assembly> _monitoredAssemblies = new();

    /// <summary>
    /// Monitoring beenden
    /// </summary>
    public void Dispose()
    {
        _timer?.Dispose();
    }

    /// <summary>
    /// Überwachung starten
    /// </summary>
    public void Start()
    {
        _timer = new(onclick, null, 60000, 60000);
    }

    /// <summary>
    /// Überwachung stoppen
    /// </summary>
    public void Stop()
    {
        _timer?.Dispose();
        _timer = null;
    }

    private void onclick(object? state)
    {
        AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.IsDynamic == false)
            .ForEach(a => {if (_monitoredAssemblies.ContainsKey(a.FullName!) == false) _monitoredAssemblies.Add(a.FullName!, a);});
    }

    /// <summary>
    /// Wörterbuch der verwendeten Assemblys
    /// </summary>
    public Dictionary<string, Assembly> UsedAssemblies => _monitoredAssemblies;
}