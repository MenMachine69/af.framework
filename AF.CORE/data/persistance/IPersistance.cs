namespace AF.DATA;

/// <summary>
/// Persistieren von Informationen
/// </summary>
public interface IPersistance
{
    /// <summary>
    /// ID der Anwendung. Diese ID wird zum Speichern von Werten verwendet, 
    /// wenn in Set keine Modul-ID angegeben ist. 
    /// </summary>
    Guid ApplicationID { get; set; }

    /// <summary>
    /// Prüfen, ob Persistenz vorhanden ist (Datenbank ist zugewiesen).
    /// </summary>
    bool IsAvailable { get; }
    
    /// <summary>
    /// ID des Benutzers. Diese ID wird zum Speichern von Werten verwendet, 
    /// wenn in Set keine Benutzer-ID angegeben ist. 
    /// </summary>
    Guid CurrentUserID { get; set; }

    /// <summary>
    /// Werte löschen.
    /// </summary>
    /// <param name="userid">ID des Benutzers, falls null alle Benutzer (NULL = CurrentUserID verwenden)</param>
    /// <param name="key">Name/ID des Wertes, wenn null alle Werte</param>
    /// <param name="modulid">ID des Moduls, falls null alle Module (NULL = ApplicationID verwenden)</param>
    /// <param name="extName">Erweiterter Name des Wertes, zusätzlich zur GUID. Durch den erweiterten Namen können mehrere Werte mit identische Guid gespeichert werden)</param>
    /// <param name="name">Wert mit dem angegebenen Namen löschen - nur in Kombination mit einer UserID zulässig!</param>
    void Delete(Guid key, Guid? modulid = null, Guid? userid = null, string? extName = null, string? name = null);

    /// <summary>
    /// Alle gepufferten Werte löschen.
    /// 
    /// Mit dieser Methode kann erzwungen werden, dass alle Werte 
    /// in der aktuellen Sitzung erneut aus der datenbank gelesen werden.
    /// </summary>
    void ClearBuffer();

    /// <summary>
    /// Alle Werte eines Benutzers löschen.
    /// 
    /// Es werden alle Werte des Benutzers gelöscht. 
    /// Das kann genutzt werden, wenn ein Benutzer gelöscht/deaktiviert wird 
    /// oder die Einstellungen des Benutzers KOMPLETT zurückgesetzt werden sollen.
    /// </summary>
    /// <param name="userid">ID des Benutzers</param>
    void DeleteUser(Guid userid);

    /// <summary>
    /// Alle Werte eines Moduls/der Anwendung löschen.
    /// 
    /// Es werden alle, das Modul/die Anwendung betreffenden Werte gelöscht.
    /// Das kann genutzt werden, wenn ein Modul komplett entfällt oder 
    /// sichergestellt werden soll, dass immer die Standardwerte des Moduls/der 
    /// Anwendung verwendet werden (kompletter Reset aller Einstellungen).
    /// </summary>
    /// <param name="modulid">ID des Moduls/der Anwendung</param>
    void DeleteModul(Guid modulid);

    /// <summary>
    /// Alle Werte eines bestimmten Schlüssels löschen.
    /// 
    /// Es werden die Werte ALLER Benutzer gelöscht. Diese Methode 
    /// kann verwendet werden, wenn ein zu persistierender Wert komplett entfällt 
    /// oder sichergestellt werden soll, dass immer die Standardwerte verwendet
    /// werden sollen (z.B. bei Änderungen des Aufbaus des zu persistierenden Wertes).
    /// </summary>
    /// <param name="key">ID des zu persistierenden Wertes</param>
    void DeleteKey(Guid key);

    /// <summary>
    /// Liest einen persistenten Wert.
    /// 
    /// Es wird zuerst versucht, einen Wert für den aktuellen Benutzer (UserKey) zu lesen und falls nicht gefunden, wird ein 
    /// Wert ohne userid gesucht werden (z.B. ein Wert für 'alle' Benutzer ohne spezifischen Wert)
    /// </summary>
    /// <param name="key">ID des Wertes</param>
    /// <param name="modulid">ID des Moduls/der Anwendung (NULL = ApplicationID verwenden)</param>
    /// <param name="userid">ID des Benutzers (NULL = CurrentUserID verwenden)</param>
    /// <param name="name">Name der Einstellungen (wenn für den Key mehrere Werte mit unterschiedlichen Namen unterstützt werden)</param>
    /// <param name="extName">Erweiterter Name des Wertes, zusätzlich zur GUID. Durch den erweiterten Namen können mehrere Werte mit identische Guid gespeichert werden)</param>
    /// <returns>der Wert oder null, wenn nichts vorhanden ist</returns>
    /// <exception cref="NullReferenceException">Database is null or no ApplicationKey assigned</exception>
    byte[]? Get(Guid key, Guid? modulid = null, Guid? userid = null, string? name = null, string? extName = null);

    /// <summary>
    /// Liest einen persistenten Wert mit der angegebenen SYS_ID.
    /// </summary>
    /// <param name="id">SYS_ID des Wertes</param>
    /// <returns>der Wert oder null, wenn nichts vorhanden ist</returns>
    /// <exception cref="NullReferenceException">Database is null or no ApplicationKey assigned</exception>
    byte[]? GetByID(Guid id);

    /// <summary>
    /// Wert mit der angegebenen ID löschen
    /// </summary>
    /// <param name="id">ID des Wertes</param>
    void DeleteById(Guid id);

    /// <summary>
    /// Speichert einen Wert für einen durch userid angegebenen Benutzer.
    /// 
    /// Ist userid = Guid.Empty ist, wird ein Wert für 'alle' Benutzer gespeichert (z.B. Standardeinstellungen).
    /// </summary>
    /// <param name="key">ID des Wertes</param>
    /// <param name="modulid">ID des Moduls/der Anwendung (NULL = ApplicationID verwenden)</param>
    /// <param name="userid">ID des Benutzers (NULL = CurrentUserID verwenden)</param>
    /// <param name="name">Name der Einstellungen (wenn für den Key mehrere Werte mit unterschiedlichen Namen unterstützt werden)</param>
    /// <param name="value">zu speichernde Informationen</param>
    /// <param name="useBuffer">Gibt an, ob der Wert im Buffer gehalten werden kann/soll</param>
    /// <param name="extName">Erweiterter Name des Wertes, zusätzlich zur GUID. Durch den erweiterten Namen können mehrere Werte mit identische Guid gespeichert werden)</param>
    void Set(Guid key, byte[] value, Guid? modulid = null, Guid? userid = null , string? name = null, bool? useBuffer = false, string? extName = null);

    /// <summary>
    /// Liste aller vorhandenen Werte für einen bestimmten Schlüssel.
    /// </summary>
    /// <param name="key">ID des Wertes</param>
    /// <param name="modulid">ID des Moduls/der Anwendung (NULL = ApplicationID verwenden)</param>
    /// <param name="userid">ID des Benutzers. Wenn NULL Werte aller Benutzer liefern.</param>
    /// <returns>Liste der Einstellungen</returns>
    BindingList<PersistantModel> GetAll(Guid key, Guid? modulid, Guid? userid);

    /// <summary>
    /// Liste der vorhandenen Werte mit einem Namen als Dictionary (Name, SYS_ID des Wertes) ausgeben.
    /// Kann dann z.B. zur Auswahl eines Wertes durch den benutzer verwendet werden.
    /// </summary>
    /// <param name="key">ID des Wertes</param>
    /// <param name="modulid">ID des Moduls/der Anwendung (NULL = ApplicationID verwenden)</param>
    /// <param name="userid">ID des Benutzers (NULL = CurrentUserID verwenden)</param>
    /// <param name="extName">Erweiterter Name des Wertes, zusätzlich zur GUID. Durch den erweiterten Namen können mehrere Werte mit identische Guid gespeichert werden)</param>
    /// <returns>Liste der Einstellungen</returns>
    Dictionary<Guid, string> GetNamedValues(Guid key, Guid? modulid = null, Guid? userid = null, string? extName = null);

}