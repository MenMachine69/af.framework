namespace AF.CORE;

/// <summary>
/// Token zur Beschreibung eines Abonnements
/// </summary>
public class EventToken : IDisposable
{
    private readonly WeakReference _hubRef;


    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="hub">Hub, der das Token erstellt und verwaltet</param>.
    /// <param name="messageType">Typ des Objekts, für das die Nachrichten übertragen werden sollen</param>.
    public EventToken(EventHub hub, Type messageType)
    {
        _hubRef = hub != null ? new(hub) : throw new ArgumentException(nameof(hub));
        MessageType = messageType ?? throw new ArgumentException(nameof(messageType));
    }

    /// <summary>
    /// Typ des Objekts, für das die Nachrichten übertragen werden sollen
    /// </summary>
    public Type MessageType { get; }

    /// <summary>
    /// Token löschen - entfernt auch das Abonnement...
    /// </summary>
    public void Dispose()
    {
        if (!_hubRef.IsAlive) return;

        EventHub? hub = _hubRef.Target as EventHub;

        hub?.Unsubscribe(this);
    }
}