namespace AF.DATA;

/// <summary>
/// Attribut, das eine Klasse aus Einstellungseigenschaften beschreibt.
/// 
/// Einstellungseigenschaften werden in der Regel in Dialogen zur Steuerung einer Funktion/eines Commands angezeigt.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class AFOptions : Attribute
{
    /// <summary>
    /// Typ, der den Controller für die Klasse liefert.
    /// </summary>
    public Type? ControllerType { get; set; }

}