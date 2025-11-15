namespace AF.CORE;

/// <summary>
/// Basisklasse für alle Services (Windows Service). Diese Klasse kann nicht direkt verwendet werden.
/// </summary>
public abstract class AFServiceApp : AFApp
{
    /// <summary>
    /// Versteckter Konstruktor
    /// 
    /// Erzeugt ein Anwendungsobjekt und registriert es bei AF. Auf das Anwendungsobjekt kann dann jederzeit über AF.App zugegriffen werden.    
    /// </summary>
    /// <param name="setup">Konfiguration der App.</param>
    protected AFServiceApp(ServiceAppSetup setup)
        : base(setup) { }
}