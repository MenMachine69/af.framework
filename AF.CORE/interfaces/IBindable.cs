namespace AF.CORE;

/// <summary>
/// Schnittstelle, die ein Typ implementieren muss, wenn er verfügbar sein soll über 
/// TypeDescription, wenn er verfügbar sein soll. Dies identifiziert den Typ als einen Typ, 
/// der AF3-Datenbindung unterstützt.
/// </summary>
public interface IBindable : IEditableObject
{
    /// <summary>
    /// Kopiert alle Informationen aus der Quelle in das aktuell gebundene Objekt
    /// </summary>
    /// <param name="source">Quelle, aus der kopiert wird</param>
    /// <param name="keyFields">SYS-Felder ebenfalls kopieren (Bsp.: SYS_ID, SYS_CREATED usw.)</param>
    void CopyFrom(IBindable source, bool keyFields);

    /// <summary>
    /// Objekt validieren
    /// </summary>
    /// <param name="errors">Collection, die die Fehlermeldungen aufnimmt, die bei der Validierung entstehen</param>
    /// <returns>true wenn valide, sonst false</returns>
    bool IsValid(ValidationErrorCollection errors);

    /// <summary>
    /// Connector, an den das Objekt gerade gebunden ist.
    /// 
    /// Darüber kann das Objekt den Connector über Statusänderungen etc. informieren.
    /// </summary>
    IBindingConnector? Connector { get; set; }

    /// <summary>
    /// überträgt alle aktuellen Änderungen - HasChanged ist nach dem Aufruf dieser Methode IMMER falsch.
    /// </summary>
    void CommitChanges();

    /// <summary>
    /// Macht alle Änderungen seit dem letzten CommitChanges rückgängig.
    /// </summary>
    void RollBackChanges();

    /// <summary>
    /// Liste der aktuell geänderten Eigenschaften
    /// </summary>
    Dictionary<string, ChangedValueInfo> ChangedProperties { get; }

    /// <summary>
    /// Gibt true zurück, wenn eine Eigenschaft, die das PropertyChanged-Ereignis (implementiert über SetPropertyValue) unterstützt, geändert wurde.
    /// </summary>
    bool HasChanged { get; }

}

/// <summary>
/// Connector, an den ein IBindable gebunden werden kann.
/// 
/// Diese Connectoren können z.B. verwendet werden, um die Daten eines 
/// IBindable-Objects in einer Eingabemaske darzustellen (DataBinding).
/// 
/// Ein an einen Conector gebundenes IBindable-Objekt kennt seinen Connector 
/// und kann diesen über Statusänderungen (z.B. Changed) informieren.
/// </summary>
public interface IBindingConnector
{
    /// <summary>
    /// Methode, die vom gebundenen IBindable-Objekt aufgerufen 
    /// werden kann, um den Connector über Statusänderungen zu informieren. 
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="data">IBindable-Objekt, dass die Nachricht geschickt hat</param>
    void StateChanged(eBindingStateMessage msg, IBindable? data);
}

/// <summary>
/// Nachrichten, mit denen ein IBindable seinen IConnector über 
/// Änderungen informieren kann.
/// </summary>
public enum eBindingStateMessage
{
    /// <summary>
    /// Daten wurden geändert
    /// </summary>
    Changed,
    /// <summary>
    /// Daten wurde 'commited'
    /// </summary>
    Commit,
    /// <summary>
    /// Rollback der Änderungen wurde ausgeführt
    /// </summary>
    Rollback
}

