namespace AF.DATA;

/// <summary>
/// Informationen zu einer Datenbankoperation
/// </summary>
[AFTable(TableName = "TBL_LOG", TableId = (int)eAFTable.Log, Version = 1)]
[AFContext(typeof(CoreStrings))]
public class LogEntry : LogBaseTable, ILoggerEntry
{
    private eLoggerOperation _logOperation = eLoggerOperation.Other;
    private Guid _logModelid = Guid.Empty;
    private Guid _logUserid = Guid.Empty;
    private DateTime _logTimestamp = DateTime.Now;

    /// <summary>
    /// Art der Operation
    /// </summary>
    [AFField]
    [AFContext(typeof(CoreStrings))]
    [AFGridColumn]
    public eLoggerOperation LOG_OPERATION
    {
        get => _logOperation;
        set => Set(ref _logOperation, value);
    }

    /// <summary>
    /// ID des Datensatzes
    /// </summary>
    [AFField]
    public Guid LOG_MODELID
    {
        get => _logModelid;
        set => Set(ref _logModelid, value);
    }

    /// <summary>
    /// ID des Benutzers
    /// </summary>
    [AFField]
    [AFGridColumn]
    public Guid LOG_USERID
    {
        get => _logUserid;
        set => Set(ref _logUserid, value);
    }

    /// <summary>
    /// Zeitpunkt der Operation
    /// </summary>
    [AFField]
    [AFContext(typeof(CoreStrings))]
    [AFGridColumn(DisplayFormat = "f")]
    public DateTime LOG_TIMESTAMP
    {
        get => _logTimestamp;
        set => Set(ref _logTimestamp, value);
    }

    /// <summary>
    /// Art der Operation
    /// </summary>
    public eLoggerOperation Operation { get => LOG_OPERATION; set => LOG_OPERATION = value; }

    /// <summary>
    /// ID des Datensatzes
    /// </summary>
    public Guid ModelId { get => LOG_MODELID; set => LOG_MODELID = value; }

    /// <summary>
    /// ID des Benutzers
    /// </summary>
    public Guid UserId { get => LOG_USERID; set => LOG_USERID = value; }

    /// <summary>
    /// Zeitpunkt der Operation
    /// </summary>
    public DateTime TimeStamp { get => LOG_TIMESTAMP; set => LOG_TIMESTAMP = value; }

    /// <summary>
    /// Liste der geänderten Felder
    /// </summary>
    public List<ILoggerEntryField> Changes { get; } = [];
}