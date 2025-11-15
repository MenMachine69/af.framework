namespace AF.DATA;

/// <summary>
/// Modell für allgemeine Informationen über die Datenbank
/// </summary>
[AFTable(TableName = "SYS_INFO", TableId = (int)eAFTable.SysInfo, Version = 1)]
[AFContext(typeof(CoreStrings))]
public class SystemDatabaseInformation : DefaultTable
{
    private int _SYSINFO_DBVERSION;
    private int _SYSINFO_IDENTIFIER;
    private bool _SYSINFO_MAINTENANCE;
    private string _SYSINFO_TABLENAME = "";

    /// <summary>
    /// Datenbank-/Tabellenversion
    /// </summary>
    [AFField()]
    [AFContext(typeof(CoreStrings))]
    public int SYSINFO_DBVERSION
    {
        get => _SYSINFO_DBVERSION;
        set => Set(ref _SYSINFO_DBVERSION, value);
    }

    /// <summary>
    /// eindeutige ID der Tabelle/Ansicht
    /// </summary>
    [AFField(Unique = true, Indexed = true)]
    [AFContext(typeof(CoreStrings))]
    public int SYSINFO_IDENTIFIER
    {
        get => _SYSINFO_IDENTIFIER;
        set => Set(ref _SYSINFO_IDENTIFIER, value);
    }

    /// <summary>
    /// Informationen über den Wartungszustand der Datenbank
    /// </summary>
    [AFField()]
    [AFContext(typeof(CoreStrings))]
    public bool SYSINFO_MAINTENANCE
    {
        get => _SYSINFO_MAINTENANCE;
        set => Set(ref _SYSINFO_MAINTENANCE, value);
    }

    /// <summary>
    /// Tabellenname - wenn dieses Feld leer ist, enthält das Objekt Informationen über die Datenbank selbst. 
    /// </summary>
    [AFField(MaxLength = 200)]
    [AFContext(typeof(CoreStrings))]
    public string SYSINFO_TABLENAME
    {
        get => _SYSINFO_TABLENAME;
        set => Set(ref _SYSINFO_TABLENAME, value);
    }
}