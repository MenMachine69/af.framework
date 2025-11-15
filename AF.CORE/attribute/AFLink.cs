namespace AF.CORE;

/// <summary>
/// Attribut, das eine Eigenschaft mit einer anderen Eigenschaft verbindet und 
/// dessen Attribute übernimmt
/// Übernommen werden:
///    AFBinding
///    AFGridColumn
///    AFRules
///    AFContext
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class AFLink : Attribute
{
    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="linkTo">Typ, der die Eigenschaft enthält, auf die verwiesen wird.</param>
    public AFLink(Type linkTo)
    {
        LinkTo = linkTo;
    }

    /// <summary>
    /// Typ, der die Eigenschaft enthält, auf die verwiesen wird.
    /// </summary>
    public Type LinkTo { get; init; }

    /// <summary>
    /// Name der Eigenschaft, auf die verwiesen wird. Wenn leer, entspricht der Name der Eigenschaft dem 
    /// dem PropertyName der verknüpften Eigenschaft (Eigenschaft, in der das Attribut gesetzt wurde).
    /// </summary>
    public string? PropertyName { get; set; }
}

