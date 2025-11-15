namespace AF.DATA;

/// <summary>
/// Persistieren von Informationen in einer Datenbank
/// </summary>
public class Persistance : IPersistance
{
    private IDatabase? _database;
    private readonly Dictionary<Guid, byte[]> _buffer = new();

    /// <summary>
    /// ID der Anwendung. Diese ID wird zum Speichern von Werten verwendet, 
    /// wenn in Set/Get keine Modul-ID angegeben ist. 
    /// </summary>
    public Guid ApplicationID { get; set; } = Guid.Empty;

    /// <summary>
    /// ID des Benutzers. Diese ID wird zum Speichern von Werten verwendet, 
    /// wenn in Set/Get keine Benutzer-ID angegeben ist. 
    /// </summary>
    public Guid CurrentUserID { get; set; } = Guid.Empty;

    /// <summary>
    /// Prüfen, ob Präsenz vorhanden ist (Datenbank ist zugewiesen).
    /// </summary>
    public bool IsAvailable => _database != null;

    /// <summary>
    /// Datenbank für die Tabelle, in der die Werte gespeichert werden sollen.
    /// </summary>
    public IDatabase? Database
    {
        get => _database;
        set
        {
            _database = value;

            if (value != null)
            {
                using IConnection connection = _database!.GetAdminConnection();
                connection.Check<PersistantModel>();
            }
            else
                throw new ArgumentNullException(nameof(Database), CoreStrings.ERR_NULLNOTALLOWED);
        }
    }


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
    public byte[]? Get(Guid key, Guid? modulid = null, Guid? userid = null, string? name = null, string? extName = null)
    {
        name ??= "";
        modulid ??= ApplicationID;
        userid ??= CurrentUserID;

        if (_database == null)
            throw new Exception(@"Datenbank für Persistenz nicht gesetzt.");

        checkRequirements((Guid)modulid);
        
        // versuchen, den Wert aus dem Puffer zu lesen...
        // nur bei Werten die zum aktuellen Benutzer gehören und für die aktuelle Anwendung sind
        if (userid != Guid.Empty && modulid.Equals(ApplicationID) && string.IsNullOrEmpty(name) && _buffer.TryGetValue(key, out var value))
            return value;

        PersistantModel? model;

        using (var conn = _database.GetConnection())
        {
            if (!string.IsNullOrEmpty(name)) //Einstellungen mit bestimmtem Namen laden
            {
                if (string.IsNullOrEmpty(extName))
                {
                    model = conn.SelectSingle<PersistantModel>(
                        $@"{nameof(PersistantModel.SYSPERSIST_MODUL)} = ? and " +
                        $@"{nameof(PersistantModel.SYSPERSIST_USER)} = ? and " +
                        $@"{nameof(PersistantModel.SYSPERSIST_KEY)} = ? and " +
                        $@"{nameof(PersistantModel.SYSPERSIST_NAME)} = ?", modulid, userid, key, name);
                }
                else
                {
                    model = conn.SelectSingle<PersistantModel>(
                        $@"{nameof(PersistantModel.SYSPERSIST_MODUL)} = ? and " +
                        $@"{nameof(PersistantModel.SYSPERSIST_USER)} = ? and " +
                        $@"{nameof(PersistantModel.SYSPERSIST_KEY)} = ? and " +
                        $@"{nameof(PersistantModel.SYSPERSIST_KEYNAME)} = ? and " +
                        $@"{nameof(PersistantModel.SYSPERSIST_NAME)} = ?", modulid, userid, key, extName!, name);
                }
            }
            else // Standardeinstellungen laden (kein Name)
            {
                // Versuch 1: Standardeinstellungen des Benutzers
                if (string.IsNullOrEmpty(extName))
                {
                    model = conn.SelectSingle<PersistantModel>(
                        $@"{nameof(PersistantModel.SYSPERSIST_MODUL)} = ? and " +
                        $@"{nameof(PersistantModel.SYSPERSIST_USER)} = ? and " +
                        $@"{nameof(PersistantModel.SYSPERSIST_KEY)} = ? and " +
                        $@"{nameof(PersistantModel.SYSPERSIST_NAME)} = ?", modulid, userid, key, "");
                }
                else
                {
                    model = conn.SelectSingle<PersistantModel>(
                        $@"{nameof(PersistantModel.SYSPERSIST_MODUL)} = ? and " +
                        $@"{nameof(PersistantModel.SYSPERSIST_USER)} = ? and " +
                        $@"{nameof(PersistantModel.SYSPERSIST_KEY)} = ? and " +
                        $@"{nameof(PersistantModel.SYSPERSIST_KEYNAME)} = ? and " +
                        $@"{nameof(PersistantModel.SYSPERSIST_NAME)} = ?", modulid, userid, key, extName!, "");
                }

                // Versuch 2: Standardeinstellungen für ALLE Benutzer (SYSPERSIST_USER = Guid.Empty)
                if (model == null)
                {
                    if (string.IsNullOrEmpty(extName))
                    {
                        model = conn.SelectSingle<PersistantModel>(
                            $@"{nameof(PersistantModel.SYSPERSIST_MODUL)} = ? and " +
                            $@"{nameof(PersistantModel.SYSPERSIST_USER)} = ? and " +
                            $@"{nameof(PersistantModel.SYSPERSIST_KEY)} = ? and " +
                            $@"{nameof(PersistantModel.SYSPERSIST_NAME)} = ?", modulid, Guid.Empty, key, "");
                    }
                    else
                    {
                        model = conn.SelectSingle<PersistantModel>(
                            $@"{nameof(PersistantModel.SYSPERSIST_MODUL)} = ? and " +
                            $@"{nameof(PersistantModel.SYSPERSIST_USER)} = ? and " +
                            $@"{nameof(PersistantModel.SYSPERSIST_KEY)} = ? and " +
                            $@"{nameof(PersistantModel.SYSPERSIST_KEYNAME)} = ? and " +
                            $@"{nameof(PersistantModel.SYSPERSIST_NAME)} = ?", modulid, Guid.Empty, key, extName!, "");
                    }
                }
            }
        }

        if (model != null)
        {
            // Wert ggf. puffern...(nur Werte ohne KEYNAME!)
            if (userid != Guid.Empty && modulid.Equals(ApplicationID) && model.SYSPERSIST_BUFFER && extName == null)
                _buffer.Add(key, model.SYSPERSIST_DATA);

            return model.SYSPERSIST_DATA;
        }
        
        return null;
    }

    /// <summary>
    /// Liest einen persistenten Wert mit der angegebenen SYS_ID.
    /// </summary>
    /// <param name="id">SYS_ID des Wertes</param>
    /// <returns>der Wert oder null, wenn nichts vorhanden ist</returns>
    /// <exception cref="NullReferenceException">Database is null or no ApplicationKey assigned</exception>
    public byte[]? GetByID(Guid id)
    {
        if (_database == null)
            throw new Exception(@"Datenbank für Persistenz nicht gesetzt.");

        using var conn = _database.GetConnection();
        return conn.Load<PersistantModel>(id)?.SYSPERSIST_DATA;
    }

    /// <summary>
    /// Wert mit der angegebenen ID löschen
    /// </summary>
    /// <param name="id">ID des Wertes</param>
    public void DeleteById(Guid id)
    {
        if (id == Guid.Empty)
            return;

        if (_database == null)
            throw new Exception(@"Datenbank für Persistenz nicht gesetzt.");

        using var conn = _database.GetConnection();
        conn.Delete<PersistantModel>(id);
    }


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
    public void Set(Guid key, byte[] value, Guid? modulid = null, Guid? userid = null, string? name = null, bool? useBuffer = false, string? extName = null)
    {
        name ??= "";
        modulid ??= ApplicationID;
        userid ??= CurrentUserID;

        if (_database == null)
            throw new Exception(@"Datenbank für Persistenz nicht gesetzt.");

        checkRequirements((Guid)modulid);
        
        // den neuen Wert des Benutzers in den Puffer schreiben, wenn bereits ein Wert im Puffer existiert
        // und KEIN Name angegeben wurde
        if (userid != Guid.Empty && modulid.Equals(ApplicationID) && string.IsNullOrEmpty(name) && _buffer.ContainsKey(key))
            _buffer[key] = value;

        PersistantModel? model;


        // vorhandenen Wert laden...
        using (var conn = _database.GetConnection())
        {
            if (extName == null)
            {
                model = conn.SelectSingle<PersistantModel>(
                    $@"{nameof(PersistantModel.SYSPERSIST_MODUL)} = ? and " +
                    $@"{nameof(PersistantModel.SYSPERSIST_USER)} = ? and " +
                    $@"{nameof(PersistantModel.SYSPERSIST_KEY)} = ? and " +
                    $@"{nameof(PersistantModel.SYSPERSIST_NAME)} = ?", modulid, userid, key, name);
            }
            else
            {
                model = conn.SelectSingle<PersistantModel>(
                    $@"{nameof(PersistantModel.SYSPERSIST_MODUL)} = ? and " +
                    $@"{nameof(PersistantModel.SYSPERSIST_USER)} = ? and " +
                    $@"{nameof(PersistantModel.SYSPERSIST_KEY)} = ? and " +
                    $@"{nameof(PersistantModel.SYSPERSIST_KEYNAME)} = ? and " +
                    $@"{nameof(PersistantModel.SYSPERSIST_NAME)} = ?", modulid, userid, key, extName, name);
            }
        }

        model ??= new PersistantModel();

        model.SYSPERSIST_KEY = key;
        model.SYSPERSIST_MODUL = (Guid)modulid;
        model.SYSPERSIST_USER = (Guid)userid;
        model.SYSPERSIST_DATA = value;
        model.SYSPERSIST_BUFFER = (useBuffer ?? false) && extName == null;
        model.SYSPERSIST_NAME = name;
        model.SYSPERSIST_KEYNAME = extName ?? "";

        using (var conn = _database.GetConnection())
            conn.Save(model);

    }

    private void checkRequirements(Guid modulid)
    {
        if (_database == null)
            throw new NullReferenceException(@"No database assigned to Persistance.");

        if (ApplicationID == Guid.Empty)
            throw new NullReferenceException(@"No ApplicationKey/ID assigned to Persistance.");

        if (modulid == Guid.Empty)
            throw new ArgumentNullException(nameof(modulid), @"Module ID can't be empty.");
    }
    
    /// <summary>
    /// Liste aller vorhandenen 
    /// </summary>
    /// <param name="key">ID des Wertes</param>
    /// <param name="modulid">ID des Moduls/der Anwendung (NULL = Alle Module)</param>
    /// <param name="userid">ID des Benutzers (NULL = Alle Benutzer)</param>
    /// <returns></returns>
    public BindingList<PersistantModel> GetAll(Guid key, Guid? modulid, Guid? userid)
    {
        if (_database == null)
            throw new NullReferenceException(@"Datenbank für Persistenz nicht gesetzt.");
        
        object[] parameter;

        using var conn = _database.GetConnection();
        
        if (userid is not null && modulid is not null)
        {
            return conn.Select<PersistantModel>(
                new QueryBuilder<PersistantModel>(_database).Select()
                    .Where(nameof(PersistantModel.SYSPERSIST_KEY), key)
                    .And(nameof(PersistantModel.SYSPERSIST_MODUL), modulid)
                    .And(nameof(PersistantModel.SYSPERSIST_USER), userid)
                    .Parse(out parameter), parameter);
        }
        
        if (userid is not null)
        {
            return conn.Select<PersistantModel>(
                new QueryBuilder<PersistantModel>(_database).Select()
                    .Where(nameof(PersistantModel.SYSPERSIST_KEY), key)
                    .And(nameof(PersistantModel.SYSPERSIST_USER), userid)
                    .Parse(out parameter), parameter);
        }
        
        if (modulid is not null)
        {
            return conn.Select<PersistantModel>(
                new QueryBuilder<PersistantModel>(_database).Select()
                    .Where(nameof(PersistantModel.SYSPERSIST_KEY), key)
                    .And(nameof(PersistantModel.SYSPERSIST_MODUL), modulid)
                    .Parse(out parameter), parameter);
        }
        
        return conn.Select<PersistantModel>(
            new QueryBuilder<PersistantModel>(_database).Select()
                .Where(nameof(PersistantModel.SYSPERSIST_KEY), key)
                .Parse(out parameter), parameter);
    }

    /// <summary>
    /// Liste der vorhandenen Werte mit einem Namen als Dictionary (SYS_ID des Wertes, Name) ausgeben.
    /// Kann dann z.B. zur Auswahl eines Wertes durch den benutzer verwendet werden.
    /// </summary>
    /// <param name="key">ID des Wertes</param>
    /// <param name="modulid">ID des Moduls/der Anwendung (NULL = ApplicationID verwenden)</param>
    /// <param name="userid">ID des Benutzers (NULL = CurrentUserID verwenden)</param>
    /// <param name="extName">Erweiterter Name des Wertes, zusätzlich zur GUID. Durch den erweiterten Namen können mehrere Werte mit identische Guid gespeichert werden)</param>
    /// <returns>Liste der Einstellungen</returns>
    public Dictionary<Guid, string> GetNamedValues(Guid key, Guid? modulid = null, Guid? userid = null, string? extName = null)
    {
        modulid ??= ApplicationID;
        userid ??= CurrentUserID;

        if (_database == null)
            throw new Exception(@"Datenbank für Persistenz nicht gesetzt.");

        using var conn = _database.GetConnection();

        if (string.IsNullOrEmpty(extName))
        {
            return conn.SelectDictionary<PersistantModel, Guid, string>(
                nameof(PersistantModel.SYS_ID),
                nameof(PersistantModel.SYSPERSIST_NAME),
                $@"{nameof(PersistantModel.SYSPERSIST_NAME)} <>? and " +
                $@"{nameof(PersistantModel.SYSPERSIST_MODUL)} = ? and " +
                $@"{nameof(PersistantModel.SYSPERSIST_USER)} = ? and " +
                $@"{nameof(PersistantModel.SYSPERSIST_KEY)} = ? and " +
                $@"{nameof(PersistantModel.SYSPERSIST_KEYNAME)} = ?", "", modulid, userid, key, "");
        }
        else
        {
            return conn.SelectDictionary<PersistantModel, Guid, string>(
                nameof(PersistantModel.SYS_ID),
                nameof(PersistantModel.SYSPERSIST_NAME),
                $@"{nameof(PersistantModel.SYSPERSIST_NAME)} <>? and " +
                $@"{nameof(PersistantModel.SYSPERSIST_MODUL)} = ? and " +
                $@"{nameof(PersistantModel.SYSPERSIST_USER)} = ? and " +
                $@"{nameof(PersistantModel.SYSPERSIST_KEY)} = ? and " +
                $@"{nameof(PersistantModel.SYSPERSIST_KEYNAME)} = ?", "", modulid, userid, key, extName!);
        }
    }

    /// <summary>
    /// Alle gepufferten Werte löschen.
    /// 
    /// Mit dieser Methode kann erzwungen werden, dass alle Werte 
    /// in der aktuellen Sitzung erneut aus der datenbank gelesen werden.
    /// </summary>
    public void ClearBuffer()
    {
        _buffer.Clear();
    }
    
    /// <summary>
    /// Alle Werte eines Benutzers löschen.
    /// 
    /// Es werden alle Werte des Benutzers gelöscht. 
    /// Das kann genutzt werden, wenn ein Benutzer gelöscht/deaktiviert wird 
    /// oder die Einstellungen des Benutzers KOMPLETT zurückgesetzt werden sollen.
    /// </summary>
    /// <param name="userid">ID des Benutzers</param>
    public void DeleteUser(Guid userid)
    {
        if (_database == null)
            throw new NullReferenceException(@"Datenbank für Persistenz nicht gesetzt.");

        using var conn = _database.GetConnection();
        conn.Delete<PersistantModel>(
            $@"{nameof(PersistantModel.SYSPERSIST_USER)} = ?",
            userid);
    }

    /// <summary>
    /// Alle Werte eines Moduls/der Anwendung löschen.
    /// 
    /// Es werden alle, das Modul/die Anwendung betreffenden Werte gelöscht.
    /// Das kann genutzt werden, wenn ein Modul komplett entfällt oder 
    /// sichergestellt werden soll, dass immer die Standardwerte des Moduls/der 
    /// Anwendung verwendet werden (kompletter Reset aller Einstellungen).
    /// </summary>
    /// <param name="modulid">ID des Moduls/der Anwendung</param>
    public void DeleteModul(Guid modulid)
    {
        if (_database == null)
            throw new NullReferenceException(@"Datenbank für Persistenz nicht gesetzt.");

        using var conn = _database.GetConnection();
        conn.Delete<PersistantModel>(
            $@"{nameof(PersistantModel.SYSPERSIST_MODUL)} = ?",
            modulid);
    }

    /// <summary>
    /// Alle Werte eines bestimmten Schlüssels löschen.
    /// 
    /// Es werden die Werte ALLER Benutzer gelöscht. Diese Methode 
    /// kann verwendet werden, wenn ein zu persistierender Wert komplett entfällt 
    /// oder sichergestellt werden soll, dass immer die Standardwerte verwendet
    /// werden sollen (z.B. bei Änderungen des Aufbaus des zu persistierenden Wertes).
    /// </summary>
    /// <param name="key">ID des zu persistierenden Wertes</param>
    public void DeleteKey(Guid key)
    {
        if (_database == null)
            throw new NullReferenceException(@"Datenbank für Persistenz nicht gesetzt.");

        using var conn = _database.GetConnection();
        conn.Delete<PersistantModel>(
            $@"{nameof(PersistantModel.SYSPERSIST_KEY)} = ?",
            key);
    }

    /// <summary>
    /// Werte löschen.
    /// </summary>
    /// <param name="userid">ID des Benutzers, falls null alle Benutzer (NULL = CurrentUserID verwenden)</param>
    /// <param name="key">Name/ID des Wertes, wenn null alle Werte</param>
    /// <param name="modulid">ID des Moduls, falls null alle Module (NULL = ApplicationID verwenden)</param>
    /// <param name="extName">Erweiterter Name des Wertes, zusätzlich zur GUID. Durch den erweiterten Namen können mehrere Werte mit identische Guid gespeichert werden)</param>
    /// <param name="name">Wert mit dem angegebenen Namen löschen - nur in Kombination mit einer UserID zulässig!</param>
    public void Delete(Guid key, Guid? modulid = null, Guid? userid = null, string? extName = null, string? name = null)
    {
        if (_database == null)
            throw new NullReferenceException(@"Datenbank für Persistenz nicht gesetzt.");

        name ??= "";
        modulid ??= ApplicationID;
        userid ??= CurrentUserID;

       _buffer.Remove((Guid)key); // gepufferten Wert entfernen

       using var conn = _database.GetConnection();
       if (name == "")
       {
           if (extName != null)
           {
               conn.Delete<PersistantModel>(
                   $@"{nameof(PersistantModel.SYSPERSIST_MODUL)} = ? and " +
                   $@"{nameof(PersistantModel.SYSPERSIST_USER)} = ? and " +
                   $@"{nameof(PersistantModel.SYSPERSIST_KEY)} = ? and " +
                   $@"{nameof(PersistantModel.SYSPERSIST_KEYNAME)} = ? and " +
                   $@"{nameof(PersistantModel.SYSPERSIST_NAME)} = ?",
                   modulid, userid, key, extName, name);
           }
           else
           {
               conn.Delete<PersistantModel>(
                   $@"{nameof(PersistantModel.SYSPERSIST_MODUL)} = ? and " +
                   $@"{nameof(PersistantModel.SYSPERSIST_USER)} = ? and " +
                   $@"{nameof(PersistantModel.SYSPERSIST_KEY)} = ? and " +
                   $@"{nameof(PersistantModel.SYSPERSIST_NAME)} = ?",
                   modulid, userid, key, name);
            }
       }
       else
       {
           if (extName != null)
           {
               conn.Delete<PersistantModel>(
                   $@"{nameof(PersistantModel.SYSPERSIST_MODUL)} = ? and " +
                   $@"{nameof(PersistantModel.SYSPERSIST_USER)} = ? and " +
                   $@"{nameof(PersistantModel.SYSPERSIST_KEY)} = ? and " +
                   $@"{nameof(PersistantModel.SYSPERSIST_KEYNAME)} = ?",
                   modulid, userid, key, extName);
           }
           else
           {
               conn.Delete<PersistantModel>(
                   $@"{nameof(PersistantModel.SYSPERSIST_MODUL)} = ? and " +
                   $@"{nameof(PersistantModel.SYSPERSIST_USER)} = ? and " +
                   $@"{nameof(PersistantModel.SYSPERSIST_KEY)} = ?",
                   modulid, userid, key);
           }
        }
    }
}

