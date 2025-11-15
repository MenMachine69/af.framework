namespace AF.DATA;

/// <summary>
/// Tabelle zur Protokollierung von Änderungen an Modellen
/// </summary>
[AFTable(TableName = "SYS_LOGENTRY", TableId = (int)eAFTable.LogEntry, Version = 1)]
public class LogEntryModel : DefaultTable
{
    private string _LOGENTRY_FIELD = "";
    private Guid _LOGENTRY_USER;
    private Guid _LOGENTRY_MODEL;
    private string _LOGENTRY_OLDVAL = "";
    private string _LOGENTRY_NEWVAL = "";

    /// <summary>
    /// Name des geänderten Feldes im Modell
    /// </summary>
    [AFField(MaxLength = 50)]
    public string LOGENTRY_FIELD
    {
        get => _LOGENTRY_FIELD;
        set => Set(ref _LOGENTRY_FIELD, value.Left(50));
    }

    /// <summary>
    /// alter Wert
    /// </summary>
    [AFField(MaxLength = 100)]
    public string LOGENTRY_OLDVAL
    {
        get => _LOGENTRY_OLDVAL;
        set => Set(ref _LOGENTRY_OLDVAL, value.Left(100));
    }

    /// <summary>
    /// neuer Wert
    /// </summary>
    [AFField(MaxLength = 100)]
    public string LOGENTRY_NEWVAL
    {
        get => _LOGENTRY_NEWVAL;
        set => Set(ref _LOGENTRY_NEWVAL, value.Left(100));
    }

    /// <summary>
    /// ID des Benutzers, der den Datensatz geändert hat
    /// </summary>
    [AFField(Indexed = true)]
    public Guid LOGENTRY_USER
    {
        get => _LOGENTRY_USER;
        set => Set(ref _LOGENTRY_USER, value);
    }

    /// <summary>
    /// ID des geänderten Datensatzes
    /// </summary>
    [AFField(Indexed = true)]
    public Guid LOGENTRY_MODEL
    {
        get => _LOGENTRY_MODEL;
        set => Set(ref _LOGENTRY_MODEL, value);
    }
}