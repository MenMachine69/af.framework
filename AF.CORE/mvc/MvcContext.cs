namespace AF.MVC;

/// <summary>
/// Statische Eigenschaften und Methoden zur MVC Unterstützung
/// </summary>
public static class MvcContext
{
    /// <summary>
    /// Typ der Klasse, die als MVC Inspektor verwendet werden soll
    /// </summary>
    public static Type? ModelInspektorType { get; set; }

    /// <summary>
    /// Einen neuen Inspektor erzeugen und zurückliefern.
    /// </summary>
    /// <returns>der erzeugte Inspektor</returns>
    /// <exception cref="Exception">Ausnahme, wenn kein ModelInspektorType zugewiesen ist.</exception>
    /// <exception cref="ArgumentException">Ausnahme, wenn der in ModelInspektorType zugewiesene Typ die Schnittstelle IModelInspektor nicht implementiert.</exception>
    public static IModelInspektor? GetInspektor()
    {
        if (ModelInspektorType == null) throw new Exception("Assign a Type that should be used as ModelInspektor to MvcContext.ModelInspektorType before using.");

        if (!ModelInspektorType.HasInterface(typeof(IModelInspektor))) throw new ArgumentException($"Type {ModelInspektorType.FullName} can not be used as a ModelInspektor. It does not implement the IModelInspektor Interface.");

        return Activator.CreateInstance(ModelInspektorType) as IModelInspektor;
    }
}

