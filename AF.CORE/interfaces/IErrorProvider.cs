namespace AF.CORE;

/// <summary>
/// Schnittstelle für Fehleranbieter zur Verwendung bei der Validierung.
/// </summary>
public interface IErrorProvider
{
    /// <summary>
    /// Anzeige von Fehlern aus einer ValidationErrorCollection.
    /// </summary>
    /// <param name="errors">Sammlung von Fehlern</param>
    public void FromCollection(ValidationErrorCollection errors);


    /// <summary>
    /// Setzt einen Fehler für eine Eigenschaft
    /// </summary>
    /// <param name="propertyName">Eigenschaftsname</param>
    /// <param name="errorMessage">Fehlermeldung</param>
    public void SetError(string propertyName, string errorMessage);

    /// <summary>
    /// Setzt eine Warnung für eine Eigenschaft
    /// </summary>
    /// <param name="propertyName">Eigenschaftsname</param>
    /// <param name="errorMessage">Warnmeldung</param>
    public void SetWarning(string propertyName, string errorMessage);

    /// <summary>
    /// Alle Einträge löschen
    /// </summary>
    public void Clear();

    /// <summary>
    /// Gibt True zurück, wenn Fehler vorhanden sind.
    /// </summary>
    public bool HasErrors { get; }

    /// <summary>
    /// Gibt True zurück, wenn Warnungen vorhanden sind.
    /// </summary>
    public bool HasWarnings { get; }
}
