namespace AF.DATA;

/// <summary>
/// Abstrakter Standard-Logger für Datenbankänderungen (ILogger)
///
/// Schreibt die Änderungen in die im Konstruktor übergebene Datenbank.
/// Die verwendeten Tabellen werden ind erDefinition der konkreten Klasse angegeben.
/// 
/// Damit die Änderungen geschrieben werden können, muss der Aktionsdelegate via 'Output'
/// gesetzt werden, dieser Delegate wird bei jeder Änderung ausgeführt.
/// </summary>
public abstract class BaseLogger<TEntry, TEntryField> : ILogger 
    where TEntry : class, ILoggerEntry, new() 
    where TEntryField : class, ILoggerEntryField, new()
{
    private readonly List<ChangeInformation> _cache = [];
    private readonly IDatabase _dbase;
    private bool _inTransaction;
    private bool _suspended;
    private Queue<ChangeInformation> _stack = new();

    /// <summary>
    /// Standard-Logger, der Änderungen in einer Datenbank speichert
    /// </summary>
    /// <param name="db">Datenbank</param>
    protected BaseLogger(IDatabase db)
    {
        _dbase = db;

        // Controller verbinden
        LogEntryController.Instance.SetDatabase(db);
        LogEntryFieldController.Instance.SetDatabase(db);

        Output = (ifo) =>
        {
            // push changes to stack...
            lock (_stack)
            {
                _stack.Enqueue(ifo);
            }
        };

        // Hintergrundprozess für Datenbankänderungen starten
        Thread backgroundProzess = new (() => doWork(ref _stack, ref _suspended))
            { IsBackground = true };

        backgroundProzess.Start();
    }

    /// <summary>
    /// eine Transaktion beginnen
    /// 
    /// Innerhalb einer Transaktion werden alle Änderungen gepuffert, bis CommitTransaction ausgeführt wird.
    /// </summary>
    public void BeginTransaction()
    {
        _cache.Clear();
        _inTransaction = true;
    }

    /// <summary>
    /// Änderungen innerhalb einer Transaktion festschreiben
    /// 
    /// Für jede gepufferte Änderung wird die Ausgabe ausgeführt.
    /// </summary>
    public void CommitTransaction()
    {
        if (_cache.Count > 0)
            _cache.ForEach(e => Output?.Invoke(e));


        _cache.Clear();
        _inTransaction = false;
    }

    /// <summary>
    /// Eine Änderung protokollieren
    /// </summary>
    /// <param name="information">Informationen über die Änderung</param>
    public void Log(ChangeInformation information)
    {
        if (_inTransaction)
            _cache.Add(information);
        else
            Output?.Invoke(information);
    }

    /// <summary>
    /// Alle Änderungen rückgängig machen
    /// 
    /// Keine Änderung innerhalb einer Transaktion wird an Output übergeben.
    /// </summary>
    public void RollbackTransaction()
    {
        _cache.Clear();
        _inTransaction = false;

    }

    /// <summary>
    /// Aktion: ausgeben.
    /// 
    /// Dieser Delegat wird bei jeder Änderung der zu protokollierenden Daten in der Datenbank ausgeführt.
    /// </summary>
    public Action<ChangeInformation>? Output { get; set; }

    /// <summary>
    /// Keine Änderungen schreiben, wenn Benutzer leer ist
    /// </summary>
    public bool IgnoreEmptyUser { get; set; } = false;

    /// <summary>
    /// Keine Änderungen schreiben, wenn Benutzer die angegebene ID hat (z.B. Systembenutzer)
    /// </summary>
    public Guid IgnoreUser { get; set; } = Guid.Empty;

    /// <summary>
    /// Keine Änderungen schreiben, wenn Benutzer leer ist
    /// </summary>
    public int WriteIntervall { get; set; } = 1000;

    /// <summary>
    /// Unterbindet das Schreibend er Log-Einträge.
    /// 
    /// Kann verwendet werden, wenn aus Performance-Gründen das sofortige Schreiben 
    /// der Log-Einträge für einen definierten Zeitraum unterbrochen werden soll.
    /// 
    /// Muss mit Resume beendet werden, damit angefallene Log-Einträge geschrieben werden.
    /// </summary>
    public void Suspend()
    {
        _suspended = true;
    }

    /// <summary>
    /// Setzt das Schreiben der Log-Einträge fort, wenn es vorher mit Suspend unterbrochen wurde.
    /// </summary>
    public void Resume()
    {
        _suspended = false;
    }

    /// <summary>
    /// Erwingt das Schreiben der noch vorhandenen Log-Einträge (z.B. beim Beenden der Anwendung).
    /// </summary>
    public void Flush()
    {
        _suspended = true;

        if (_cache.Count > 0)
            _cache.ForEach(e => Output?.Invoke(e));

        lock (_stack)
        {
            ReadOptions options = new();

            while (_stack.Count > 0)
            {
                // write all changes
                var element = _stack.Dequeue();
                TEntry entry = new()
                {
                    ModelId = element.RecordID,
                    TimeStamp = DateTime.Now,
                    UserId = AFCore.App.SecurityService?.CurrentUser?.UserId ?? Guid.Empty,
                    Operation = element.Operation
                };

                if (entry.UserId.Equals(Guid.Empty) && IgnoreEmptyUser) // ignore empty user
                    continue;

                if (!IgnoreUser.Equals(Guid.Empty) && entry.UserId.Equals(IgnoreUser)) // ignore specific user
                    continue;

                // using for instead of foreach for max speed...
                for (var pos = 0; pos < element.Fields.Count; pos++)
                {
                    var fieldinfo = element.Fields[pos];
                    TEntryField finfo = new()
                    {
                        PropertyName = fieldinfo.Field,
                        NewValue = fieldinfo.NewValue?.ToString() ?? "",
                        OldValue = fieldinfo.OldValue?.ToString() ?? ""
                    };

                    if (finfo.OldValue.Equals(finfo.NewValue, StringComparison.Ordinal)) // nothing changed
                        continue;

                    entry.Changes.Add(finfo);
                }

                if (entry.Operation == eLoggerOperation.Update && entry.Changes.Count < 1) continue; // nothing changed

                using (var conn = _dbase.GetConnection())
                {
                    conn.Silent = true;
                    conn.Save(options, entry, forcecreate: true);

                    foreach (var change in entry.Changes)
                    {
                        change.OperationId = entry.PrimaryKey;
                        conn.Save(options, change, forcecreate: true);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Hintergrundprozess der Datenbankänderungen in die Log-Datenbank schreibt
    /// </summary>
    /// <param name="changeList"></param>
    /// <param name="suspended"></param>
    private void doWork(ref Queue<ChangeInformation> changeList, ref bool suspended)
    {
        while (true)
        {
            lock (changeList)
            {
                ReadOptions options = new();

                while (changeList.Count > 0 && !suspended)
                {
                    // write all changes
                    var element = changeList.Dequeue();
                    TEntry entry = new()
                    {
                        ModelId = element.RecordID,
                        TimeStamp = DateTime.Now,
                        UserId = AFCore.App.SecurityService?.CurrentUser?.UserId ?? Guid.Empty,
                        Operation = element.Operation
                    };

                    if (entry.UserId.Equals(Guid.Empty) && IgnoreEmptyUser) // ignore empty user
                        continue;

                    if (!IgnoreUser.Equals(Guid.Empty) && entry.UserId.Equals(IgnoreUser)) // ignore specific user
                        continue;

                    // using for instead of foreach for max speed...
                    for (var pos = 0; pos < element.Fields.Count; pos++)
                    {
                        var fieldinfo = element.Fields[pos];
                        TEntryField finfo = new()
                        {
                            PropertyName = fieldinfo.Field,
                            NewValue = fieldinfo.NewValue?.ToString() ?? "",
                            OldValue = fieldinfo.OldValue?.ToString() ?? ""
                        };

                        if (finfo.OldValue.Equals(finfo.NewValue, StringComparison.Ordinal)) // nothing changed
                            continue;

                        entry.Changes.Add(finfo);
                    }

                    if (entry.Operation == eLoggerOperation.Update && entry.Changes.Count < 1) continue; // nothing changed

                    using (var conn = _dbase.GetConnection())
                    {
                        conn.Silent = true;
                        conn.Save(options, entry, forcecreate:true);

                        foreach (var change in entry.Changes)
                        {
                            change.OperationId = entry.PrimaryKey;
                            conn.Save(options, change, forcecreate: true);
                        }
                    }
                }
            }

            // Wait until next write check...
            Thread.Sleep(WriteIntervall);
        }

        // ReSharper disable once FunctionNeverReturns
    }
}