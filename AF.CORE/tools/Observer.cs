namespace AF.CORE;

/// <summary>
/// Entwurfsmuster: Observer/Observable
/// 
/// Die Beobachter registrieren sich bei dem Observable(Subject), 
/// um vom Observable(Subjekt) über Ereignisse informiert zu werden
/// 
/// Observable kennt alle Beobachter, die benachrichtigt werden wollen.
/// 
/// Beobachter müssen sich von der Observable abmelden, wenn sie nicht mehr /// benachrichtigt werden wollen oder müssen. 
/// nicht mehr benachrichtigt werden wollen oder müssen.
/// </summary>
public interface IObservable
{
    /// <summary>
    /// Registrieren eines Beobachters
    /// </summary>
    /// <param name="anObserver">der Beobachter</param>
    void Register(IObserver anObserver);

    /// <summary>
    /// Aufhebung der Registrierung eines Beobachters
    /// </summary>
    /// <param name="anObserver">der Beobachter</param>
    void UnRegister(IObserver anObserver);
}

/// <summary>
/// Entwurfsmuster: Observer/Observable
/// </summary>
public interface IObserver
{
    /// <summary>
    /// Benachrichtigung des oberen Servers durch das Subjekt
    /// 
    /// Ein Beobachter muss sich beim Observalble/Subjekt registrieren, um Benachrichtigungen zu erhalten.
    /// und sich abmelden, wenn er sie nicht mehr benötigt (UnRegister).
    /// </summary>
    /// <param name="data">übergebene Daten (optional, kann auch null sein)</param>.
    /// <param name="sender">Absender des Ereignisses (das Subjekt, das die Benachrichtigung sendet)</param>
    void Notify(IObservable sender, object data);
}

/// <summary>
/// DesignPattern: Observer/Observable
/// Konkrete Implementierung der Schnittstelle IOberservable
/// 
/// Sie können Ihre eigenen IObservable-Klassen von dieser Klasse ableiten und müssen sich dann nicht mehr 
/// um die Implementierung von Registern/Unregistern kümmern.
/// </summary>
public abstract class ObservableBase : IObservable
{
    private readonly List<IObserver> _observers = [];
    
    /// <summary>
    /// Registrieren eines Beobachters
    /// </summary>
    /// <param name="anObserver">der Beobachter</param>
    public void Register(IObserver anObserver)
    {
        if (_observers.Contains(anObserver) == false)
            _observers.Add(anObserver);
    }

    /// <summary>
    /// Aufhebung der Registrierung eines Beobachters
    /// </summary>
    /// <param name="anObserver">der Beobachter</param>
    public void UnRegister(IObserver anObserver)
    {
        if (_observers.Contains(anObserver))
            _observers.Remove(anObserver);
    }

    /// <summary>
    /// Die Beobachter benachrichtigen
    /// </summary>
    /// <param name="data">Daten, die an die Beobachter übertragen werden sollen</param>
    public void NotifyObservers(object data)
    {
        foreach (IObserver observer in _observers)
            observer.Notify(this, data);
    }
}

