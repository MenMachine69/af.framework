namespace AF.DATA;

/// <summary>
/// Schnittstelle für alle DataObjects, die in einer Datenbank gespiegelt werden können.
///
/// Diese Schnittstelle sollte nicht direkt verwendet werden.
/// Für Tabellen sollte ITable und für Views IView verwendet werden.
/// </summary>
public interface IDataObject : IModel
{
    /// <summary>
    /// DateTime created
    /// </summary>
    DateTime CreateDateTime { get; set; }

    /// <summary>
    /// DateTime last changed
    /// </summary>
    DateTime UpdateDateTime { get; set; }

    /// <summary>
    /// Markiert das Objekt als archiviert
    /// </summary>
    bool IsArchived { get; set; }

    /// <summary>
    /// Datenbank, aus der das Objekt geladen bzw. in der das Objekt gespeichert wurde.
    /// </summary>
    IDatabase? Database { get; set; }

    /// <summary>
    /// Methode, die ausgelöst wird, nachdem ein Modell aus der Datenbank geladen wurde
    /// </summary>
    void AfterLoad(); 

    /// <summary>
    /// Prüft, ob die verzögerte Eigenschaft mit dem angegebenen Namen aus der Datenbank geladen ist
    /// </summary>
    /// <param name="fieldName">Name der Eigenschaft/Feld</param>
    /// <returns>true, wenn geladen</returns>
    public bool IsDelayedLoaded(string fieldName);
}
