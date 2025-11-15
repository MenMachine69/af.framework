namespace AF.DATA;

/// <summary>
/// Informationen zu einer Änderung in einer Datenbank-Operation
/// </summary>
[AFTable(TableName = "TBL_LOGFIELD", TableId = (int)eAFTable.LogField, Version = 1)]
[AFContext(typeof(CoreStrings))]
public class LogEntryField : LogBaseTable, ILoggerEntryField
{
    private Guid _logOperationId = Guid.Empty;
    private string _logOldValue = "";
    private string _logNewValue = "";
    private string _logPropertyName = "";
    
    /// <summary>
    /// ID der Operation (LogEntry)
    /// </summary>
    [AFField(ConstraintType = typeof(LogEntry), ConstraintDelete = eConstraintOperation.Cascade)]
    public Guid LOG_OPERATIONID
    {
        get => _logOperationId;
        set => Set(ref _logOperationId, value);
    }

    /// <summary>
    /// alter Wert
    /// </summary>
    [AFField(MaxLength = 100)]
    [AFContext(typeof(CoreStrings))]
    [AFGridColumn]
    public string LOG_OLDVALUE
    {
        get => _logOldValue;
        set => Set(ref _logOldValue, value.Length >  100 ? value.Left(100) : value);
    }

    /// <summary>
    /// neuer Wert
    /// </summary>
    [AFField(MaxLength = 100)]
    [AFContext(typeof(CoreStrings))]
    [AFGridColumn]
    public string LOG_NEWVALUE
    {
        get => _logNewValue;
        set => Set(ref _logNewValue, value.Length > 100 ? value.Left(100) : value);
    }

    /// <summary>
    /// Name des geänderten Feldes
    /// </summary>
    [AFField(MaxLength = 50)]
    [AFContext(typeof(CoreStrings))]
    [AFGridColumn]
    public string LOG_PROPERTYNAME
    {
        get => _logPropertyName;
        set => Set(ref _logPropertyName, value.Length > 50 ? value.Left(50) : value);
    }

    /// <summary>
    /// ID der Operation (LogEntry)
    /// </summary>
    public Guid OperationId { get => LOG_OPERATIONID; set => LOG_OPERATIONID = value; }

    /// <summary>
    /// alter Wert
    /// </summary>
    public string OldValue { get => LOG_OLDVALUE; set => LOG_OLDVALUE = value; }

    /// <summary>
    /// neuer Wert
    /// </summary>
    public string NewValue { get => LOG_NEWVALUE; set => LOG_NEWVALUE = value; }

    /// <summary>
    /// Name des geänderten Feldes
    /// </summary>
    public string PropertyName { get => LOG_PROPERTYNAME; set => LOG_PROPERTYNAME = value; }
}