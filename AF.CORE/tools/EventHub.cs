namespace AF.CORE;

/// <summary>
/// EventHub, wo Ereignisse für jedes Objekt abonniert werden können.
/// 
/// Der Abonnent teilt dem Hub mit, dass er an Nachrichten (Änderungsbenachrichtigungen) für beliebige Objekte eines 
/// Typs interessiert ist (Aufruf von subscribe) und erhält vom Hub ein Token für das Abonnement, mit dem er 
/// mit dem er das Abonnement auch wieder kündigen kann (unsubscribe). 
/// 
/// Tritt eine Änderungsmeldung für ein Objekt des abonnierten Typs auf (hinzugefügt, geändert oder gelöscht), wird die 
/// Methode des Abonnenten aufgerufen, die bei der Subskription angegeben wurde. Dieser Methode wird das betreffende Objekt übergeben und 
/// eine Nachricht, die angibt, um welche Art von Änderung es sich handelt.
/// 
/// Eine Anwendung kann Nachrichten auch selbst über die Methode Deliver veröffentlichen.
/// 
/// Es sollte nur einen EventHub pro Anwendung geben - standardmäßig wird dieser über AF.EventHub zur Verfügung gestellt 
/// ohne weitere Aktion zur Verfügung gestellt. 
/// </summary>
public sealed class EventHub
{
    private readonly object _locker = new();
    private readonly List<IEventSubscription> _subscriptions = [];

    private class messageSubscription : IEventSubscription
    {
        private readonly Action<object, eHubEventType, int> _deliverMessage;
        private Func<object, bool>? _messageFilter;
        private readonly WeakReference _receiver;

        /// <summary>
        /// Initialisiert eine neue Instanz der Klasse <see cref="messageSubscription"/>.
        /// </summary>
        /// <param name="token">Der Token, der das Abonnement darstellt.</param>
        /// <param name="objectType">Typ der Objekte, deren Ereignisse abonniert werden (z.B. Typ des IModel)</param>
        /// <param name="receiver">Das Objekt, das die Nachrichten empfangen wird.</param>
        /// <param name="deliverMessage">Eine Methode, die aufgerufen wird, um die Nachricht an den Empfänger zuzustellen.</param>
        /// <param name="messageFilter">Eine Methode, die als Filter für die Nachrichten verwendet wird. Die messageFilter-Methode wird dem Objekt vor der Übermittlung der Nachricht übergeben und muss true zurückgeben, wenn die Nachricht verteilt werden soll.</param>
        public messageSubscription(EventToken token, Type objectType, object receiver,
            Action<object, eHubEventType, int> deliverMessage, Func<object, bool>? messageFilter)
        {
            Token = token;
            EventObjectType = objectType;
            _receiver = new(receiver);
            _deliverMessage = deliverMessage;
            _messageFilter = messageFilter;
        }

        /// <summary>
        /// Ruft das Nachrichten-Token des Abonnements ab.
        /// </summary>
        /// <returns>Das Nachrichten-Token des Abonnements.</returns>
        public EventToken Token { get; }

        /// <summary>
        /// Typ der Objekte deren Ereignisse abonniert sind.
        /// </summary>
        public Type EventObjectType { get; init; }

        /// <summary>
        /// Ermittelt, ob das Abonnement eine Nachricht an das angegebene Objekt liefern kann.
        /// </summary>
        /// <param name="model">Das Objekt, das auf Zustellbarkeit geprüft werden soll.</param>
        /// <returns>True, wenn das Abonnement eine Nachricht an das angegebene Objekt zustellen kann, sonst false.</returns>
        public bool CanDeliver(object model)
        {
            // Wenn der Empfänger nicht mehr am Leben ist, Abmeldung und Rückgabe von false
            if (!_receiver.IsAlive)
            {
                AFCore.App.EventHub.Unsubscribe(Token);
                return false;
            }

            // Wenn das Modell nicht der richtige Typ ist, wird false zurückgegeben.
            if (model.GetType() != EventObjectType)
                return false;

            // Wenn der Nachrichtenfilter null ist oder nicht mehr existiert oder das Modell nicht vom richtigen Typ ist, wird true zurückgegeben.
            if (_messageFilter == null || _messageFilter.Target == null )
                return true;

            // Andernfalls rufen Sie den Nachrichtenfilter auf und geben das Ergebnis zurück
            return ((Func<object, bool>)_messageFilter.Target).Invoke(model);
        }

        /// <summary>
        /// Liefert eine Nachricht an das abonnierte Objekt.
        /// </summary>
        /// <param name="model">Das Objekt, das als Nachricht zugestellt werden soll.</param>
        /// <param name="msgType">Der Typ der zuzustellenden Nachricht.</param>
        public void Deliver(object model, eHubEventType msgType)
        {
            Deliver(model, msgType, 0);
        }

        /// <summary>
        /// Liefert eine Nachricht an das abonnierte Objekt.
        /// </summary>
        /// <param name="model">Das Objekt, das als Nachricht zugestellt werden soll.</param>
        /// <param name="msgType">Der Typ der zuzustellenden Nachricht</param>
        /// <param name="messageCode">Ein Code, der die zu übermittelnde Nachricht darstellt.</param>
        public void Deliver(object model, eHubEventType msgType, int messageCode)
        {
            // If the model is not the correct type, return
            if (model.GetType() != EventObjectType)
                return;

            // If the receiver is no longer alive, unsubscribe and return
            if (!_receiver.IsAlive)
            {
                AFCore.App.EventHub.Unsubscribe(Token);
                return;
            }

            // If the model is the correct type, invoke the deliver message method
            if (model.GetType() == EventObjectType)
                _deliverMessage.Invoke(model, msgType, messageCode);
        }

        /// <summary>
        /// Löscht das Nachrichtenabonnement.
        /// </summary>
        public void Clear()
        {
            // Set the message filter to null
            _messageFilter = null;
        }
    }

    private interface IEventSubscription
    {
        /// <summary>
        /// Ruft das Nachrichten-Token des Abonnements ab.
        /// </summary>
        /// <returns>Das Nachrichten-Token des Abonnements.</returns>
        EventToken Token { get; }

        /// <summary>
        /// Ermittelt, ob das Abonnement eine Nachricht an das angegebene Objekt liefern kann.
        /// </summary>
        /// <param name="model">Das Objekt, das auf Zustellbarkeit geprüft werden soll.</param>
        /// <returns>True, wenn das Abonnement eine Nachricht an das angegebene Objekt zustellen kann, sonst false.</returns>
        bool CanDeliver(object model);

        /// <summary>
        /// Liefert eine Nachricht an das angegebene Objekt mit dem angegebenen Nachrichtentyp.
        /// </summary>
        /// <param name="model">Das Objekt, an das die Nachricht zugestellt werden soll.</param>
        /// <param name="msgType">Der Typ der zu übermittelnden Nachricht.</param>
        void Deliver(object model, eHubEventType msgType);

        /// <summary>
        /// Löscht das Abonnement.
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// Abonnieren von Ereignissen.
    /// 
    /// Hier kann eine Methode als Filter für die Nachrichten übergeben werden. 
    /// </summary>
    /// <param name="objectType">Typ der Objekte</param>
    /// <param name="receiver">Das Objekt, das die Nachrichten empfangen soll.</param>
    /// <param name="deliverMessage">Eine Methode, die aufgerufen wird, um die Nachricht an den Empfänger zu übermitteln.</param>
    /// <param name="messageFilter">Eine Methode, die als Filter für die Nachrichten verwendet werden soll. 
    /// Die messageFilter-Methode wird für das Objekt aufgerufen, bevor die Nachricht übertragen wird 
    /// und muss true zurückgeben, wenn die Nachricht verteilt werden soll.</param>
    /// <returns>Ein Token, das das Abonnement beschreibt.</returns>
    public EventToken Subscribe(Type objectType, object receiver, Action<object, eHubEventType, int> deliverMessage,
        Func<object, bool> messageFilter)
    {
        EventToken token = new(this, objectType);
        
        lock (_locker)
        {
            _subscriptions.Add(new messageSubscription(token, objectType, receiver, deliverMessage, messageFilter));
        }
        
        return token;
    }


    /// <summary>
    /// Abonnieren von Ereignissen.
    /// 
    /// Hier kann eine Methode übergeben werden, die als Filter für die Nachrichten dient. Diese Methode wird an das Objekt vor den 
    /// Nachrichten übergeben und die Methode muss true zurückgeben, wenn die Nachricht verteilt werden soll.
    /// </summary>
    /// <param name="objectType">Typ der Objekte</param>
    /// <param name="deliverMessage">Methode, an die das Ereignis zugestellt werden soll</param>
    /// <param name="receiver">Empfänger der Nachricht</param>.
    /// <returns>Token, der das Abonnement beschreibt</returns>
    public EventToken Subscribe(Type objectType, object receiver, Action<object, eHubEventType, int> deliverMessage)
    {
        EventToken token = new(this, objectType);
        
        lock (_locker)
        {
            _subscriptions.Add(new messageSubscription(token, objectType, receiver, deliverMessage, null));
        }

        return token;
    }

    /// <summary>
    /// Abbestellen eines Abos
    /// </summary>
    /// <param name="token">Token des zu kündigenden Abonnements</param>
    public void Unsubscribe(EventToken token)
    {
        lock (_locker)
        {
            var currentlySubscribed = (from sub in _subscriptions
                where ReferenceEquals(sub.Token, token)
                select sub).ToList();

            currentlySubscribed.ForEach(sub => _subscriptions.Remove(sub));
            currentlySubscribed.ForEach(sub => sub.Clear());
        }
    }

    /// <summary>
    /// Zustellung einer Nachricht an die Abonnenten.
    /// 
    /// Mit dieser Methode können Sie die Nachrichten auch selbst an die Abonnenten verteilen.
    /// </summary>
    /// <param name="model">Das Objekt, das neu erstellt, geändert oder gelöscht wurde</param>.
    /// <param name="msgType">die Art des Ereignisses</param>
    public void Deliver(object model, eHubEventType msgType)
    {
        List<IEventSubscription> subscriptions;
        lock (_locker)
        {
            subscriptions = (from sub in _subscriptions
                where sub.CanDeliver(model)
                select sub).ToList();
        }

        subscriptions.ForEach(sub => sub.Deliver(model, msgType));

    }
}