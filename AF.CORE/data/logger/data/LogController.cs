namespace AF.DATA;

/// <summary>
/// Basis-Controller aller Models in der Log-Datenbank
/// </summary>
/// <typeparam name="TModel"></typeparam>
/// <typeparam name="TModelSmallView"></typeparam>
/// <typeparam name="TModelLargeView"></typeparam>
public abstract class LogBaseController<TModel, TModelLargeView, TModelSmallView> : ControllerTable<TModel, TModelLargeView, TModelSmallView>
    where TModel : class, ITable, new()
    where TModelLargeView : class, IDataObject, new()
    where TModelSmallView : class, IDataObject, new()
{
    private IDatabase? _database;
    
    /// <summary>
    /// die zu verwendende Datenbank setzen
    /// </summary>
    /// <param name="db">Datenbank</param>
    /// <param name="feedback">Meldungen zum Status der Überprüfung der Datenbank anzeigen</param>
    public void SetDatabase(IDatabase db, Action<string>? feedback = null)
    {
        _database = db;
        _database.Check([typeof(LogBaseTable)], [typeof(LogBaseView)], feedback, false);
    }

    /// <summary>
    /// Create/Open a database connection
    /// </summary>
    /// <returns>the database connection</returns>
    public override IConnection GetConnection()
    {
        return _database?.GetConnection() ?? throw new Exception(@"Call SetDatabase before using this controller.");
    }
}

/// <summary>
/// Controller für LogEntry-Datenbanken
/// </summary>
public class LogEntryController : LogBaseController<LogEntry, LogEntry, LogEntry>
{
    private static LogEntryController? instance;

    /// <summary>
    /// Zugriff auf die Instanz des Controllers
    /// </summary>
    public static LogEntryController Instance => instance ??= new();


}

/// <summary>
/// Controller für LogEntryField-Datenbanken
/// </summary>
public class LogEntryFieldController : LogBaseController<LogEntryField, LogEntryField, LogEntryField>
{
    private static LogEntryFieldController? instance;

    /// <summary>
    /// Zugriff auf die Instanz des Controllers
    /// </summary>
    public static LogEntryFieldController Instance => instance ??= new();
}