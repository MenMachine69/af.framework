namespace AF.DATA;

/// <summary>
/// Schnittstelle für Klassen, die als Tabellen in einer Datenbank abgebildet werden.
///
/// Wenn BaseTableData verwendet wird, ist es nicht notwendig, diese Schnittstelle
/// in eigenen Klassen zu implementieren, da sie bereits in BaseTableData implementiert ist. 
/// </summary>
public interface ITable : IDataObject
{
    /// <summary>
    /// Methode, die ausgelöst wird, bevor ein Modell in der Datenbank gespeichert wird
    /// </summary>
    void BeforeSave();

    /// <summary>
    /// Methode, die aufgerufen wird, BEVOR ein Objekt gelöscht wird.
    ///
    /// Gibt die Methode TRUE zurück, kann das Objekt gelöscht werden.
    /// Liefert die Methode FALSE, darf das Objekt NICHT gelöscht werden. 'reason' enthält in diesem Fall
    /// Hinweise, warum das Löschen nicht zulässig ist.
    /// </summary>
    /// <param name="reason">Begründung, warum das Löschen nicht zulässig ist.</param>
    /// <returns>TRUE, wenn Löschen zulässig, sonst FALSE.</returns>
    bool CanDelete(out string reason);

    /// <summary>
    /// Status des Objekts (aktuelle Werte der Felder) als Dictionary zurückliefern.
    /// 
    /// Es werden nur folgende Feldtypen berücksichtigt:
    /// Numeric (int, decimal etc.)
    /// Guid
    /// DateTime
    /// TimeSpan
    /// TimeOnly
    /// DateOnly
    /// bool
    /// string (MaxLength > -1)
    /// enum
    /// </summary>
    /// <returns></returns>
    Dictionary<string, object?> GetState();
}