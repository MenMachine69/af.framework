namespace AF.DATA;

/// <summary>
/// Tabelle zur Speicherung von Daten wie Benutzereinstellungen, Konfigurationen etc.
/// </summary>
[AFTable(TableName = "SYS_PERSISTANCE", TableId = (int)eAFTable.Persistance, Version = 3)]
public class PersistantModel : DefaultTable
{
    private Guid _SYSPERSIST_KEY = Guid.Empty;
    private Guid _SYSPERSIST_USER;
    private Guid _SYSPERSIST_MODUL;
    private string _SYSPERSIST_NAME = "";
    private string _SYSPERSIST_DESCRIPTION = "";
    private byte[] _SYSPERSIST_DATA = [];
    private bool _SYSPERSIST_BUFFER;
    private string _SYSPERSIST_KEYNAME = "";

    /// <summary>
    /// Eindeutige ID zur Identifizierung des Wertes
    /// </summary>
    [AFField(Indexed = true)]
    public Guid SYSPERSIST_KEY
    {
        get => _SYSPERSIST_KEY;
        set => Set(ref _SYSPERSIST_KEY, value);
    }

    /// <summary>
    /// Name, der zusammen mit SYSPERSIST_KEY der Identifizierung dient
    /// </summary>
    [AFField(Indexed = true, MaxLength = 20)]
    public string SYSPERSIST_KEYNAME
    {
        get => _SYSPERSIST_KEYNAME;
        set => Set(ref _SYSPERSIST_KEYNAME, value);
    }

    /// <summary>
    /// id des Benutzers, dem der Wert gehört.
    /// 
    /// Guid.Empty, wenn der Wert keinem bestimmten Benutzer zugewiesen ist.
    /// </summary>
    [AFField(Indexed = true)]
    public Guid SYSPERSIST_USER
    {
        get => _SYSPERSIST_USER;
        set => Set(ref _SYSPERSIST_USER, value);
    }

    /// <summary>
    /// Id des Moduls/der App. standardmäßig ist dies die GUID der App.
    /// </summary>
    [AFField(Indexed = true)]
    public Guid SYSPERSIST_MODUL
    {
        get => _SYSPERSIST_MODUL;
        set => Set(ref _SYSPERSIST_MODUL, value);
    }

    /// <summary>
    /// Bezeichnung des Wertes (optional, z.B. gespeicherte Namen eines GridTemplates zur Anzeige/Auswahl des Templates)
    /// </summary>
    [AFField(MaxLength = 150, Indexed = true)]
    public string SYSPERSIST_NAME
    {
        get => _SYSPERSIST_NAME;
        set => Set(ref _SYSPERSIST_NAME, value);
    }

    /// <summary>
    /// Beschreibung des Wertes (optional, z.B. gespeicherte Namen eines GridTemplates zur Anzeige/Auswahl des Templates)
    /// </summary>
    [AFField(MaxLength = -1)]
    public string SYSPERSIST_DESCRIPTION
    {
        get => _SYSPERSIST_DESCRIPTION;
        set => Set(ref _SYSPERSIST_DESCRIPTION, value);
    }

    /// <summary>
    /// Daten/Wert (meist ein serialisiertes Objekt)
    /// </summary>
    [AFField]
    public byte[] SYSPERSIST_DATA
    {
        get => _SYSPERSIST_DATA;
        set => Set(ref _SYSPERSIST_DATA, value);
    }

    /// <summary>
    /// Gibt an, ob der Wert gepuffert werden soll.
    /// 
    /// Gepufferte Werte werden nur einmal pro Session aus der Datenbank 
    /// gelesen und dann in einem internen Puffer verwaltet.
    /// </summary>
    [AFField]
    public bool SYSPERSIST_BUFFER
    {
        get => _SYSPERSIST_BUFFER;
        set => Set(ref _SYSPERSIST_BUFFER, value);
    }

}

