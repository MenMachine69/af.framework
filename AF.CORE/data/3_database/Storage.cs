using System.Collections.ObjectModel;

namespace AF.DATA;


/// <summary>
/// Eine Klasse, die den Datenbankzugriff in einer Anwendung ermöglicht.
/// Der Zugriff auf eine beliebige Anzahl von Datenbanken kann in dieser Klasse verwaltet werden.
/// Jede Anwendung sollte nur ein Objekt dieser Klasse haben.
/// Normalerweise wird der Zugriff in AF3 gespeichert.
/// <example>
/// <code>
/// AF.Storage = new Storage(configPrimaryDB, new[] { configSecondaryDB, configThirdDB });
/// </code>
/// </example>
/// </summary>
public class StorageService
{
    private readonly Dictionary<string, IDatabase> _innerdatabases = new();
    private readonly IDatabase _primaryDB;

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="primarydb">Konfiguration der primären Datenbank</param>
    /// <param name="additionaldatabases">zusätzliche Datenbanken</param>
    public StorageService(IConfiguration primarydb, IEnumerable<IConfiguration>? additionaldatabases = null)
    {
        _primaryDB = primarydb.Create();

        _innerdatabases.Add(primarydb.DatabaseName, _primaryDB);

        additionaldatabases?.ForEach(db => _innerdatabases.Add(db.DatabaseName, db.Create()));

        Databases = new ReadOnlyDictionary<string, IDatabase>(_innerdatabases);
    }

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="primarydb">primäre Datenbank</param>
    /// <param name="additionaldatabases">zusätzliche Datenbanken</param>
    public StorageService(IDatabase primarydb, IEnumerable<IDatabase>? additionaldatabases = null)
    {
        _primaryDB = primarydb;

        _innerdatabases.Add(@"primary", _primaryDB);

        additionaldatabases?.ForEach(db => _innerdatabases.Add(db.DatabaseName , db));

        Databases = new ReadOnlyDictionary<string, IDatabase>(_innerdatabases);
    }

    /// <summary>
    /// Erstellt eine Verbindung zur primären Datenbank
    /// </summary>
    /// <returns>Datenbankverbindung</returns>
    public IConnection GetConnection()
    {
        return _primaryDB.GetConnection();
    }

    /// <summary>
    /// Erstellt eine Verbindung Datenbank, in der die Objekte des Typs TData gespeichert sind.
    /// </summary>
    /// <returns>Datenbankverbindung</returns>
    public IConnection GetConnection<TData>() where TData : class, IDataObject
    {
        IDatabase? database = null;

        foreach (var type in _primaryDB.Configuration.BaseTableTypes)
        {
            if (!typeof(TData).IsSubclassOf(type)) continue;

            database = _primaryDB;
            break;
        }

        if (database == null)
        {
            foreach (var type in _primaryDB.Configuration.BaseViewTypes)
            {
                if (!typeof(TData).IsSubclassOf(type)) continue;

                database = _primaryDB;
                break;
            }
        }

        if (database == null)
        {
            foreach (var innerdb in _innerdatabases.Values)
            {
                foreach (var type in innerdb.Configuration.BaseTableTypes)
                {
                    if (!typeof(TData).IsSubclassOf(type)) continue;

                    database = innerdb;
                    break;
                }

                if (database == null)
                {
                    foreach (var type in innerdb.Configuration.BaseViewTypes)
                    {
                        if (!typeof(TData).IsSubclassOf(type)) continue;

                        database = innerdb;
                        break;
                    }
                }

                if (database != null)
                    break;
            }
        }

        if (database!= null) 
            return database.GetConnection();

        throw new NotImplementedException(string.Format(CoreStrings.ERR_TYPEMISSINGDB, typeof(TData)));
    }
        

    /// <summary>
    /// Zugriff auf die Datenbanken über deren Namen.
    /// 
    /// Bsp.: MyApp.Storage["Blob"].GetConnection()
    /// </summary>
    /// <param name="dbname">Name der Datenbank</param>
    /// <returns>Datenbank</returns>
    public IDatabase this[string dbname]
    {
        get => _innerdatabases[dbname];
        set => _innerdatabases[dbname] = value;
    }

    /// <summary>
    /// Alle verfügbaren Datenbanken
    /// </summary>
    public ReadOnlyDictionary<string, IDatabase> Databases { get; }

    /// <summary>
    /// Prüfen der Struktur aller Datenbanken (Tabellen, Views usw.)
    /// </summary>
    /// <param name="force">vollständige Überprüfung erzwingen. 
    /// Normalerweise werden nur Tabellen/Views geprüft, deren Version abweicht.</param>
    /// <param name="feedback">Delegate für Feedback während der Prüfung (Informationen zum Status der Überprüfung etc-)</param>
    /// <param name="postcheck">Action dir nach der Prüfung durchgeführt werden soll</param>
    /// <returns>true, wenn alle Überprüfungen erfolgreich waren</returns>
    public bool Check(bool force, Action<string>? feedback = null, Func<bool>? postcheck = null)
    {
        bool ret = true;

        foreach (var db in _innerdatabases.Values)
        {
            ret = db.Check(db.Configuration.BaseTableTypes, db.Configuration.BaseViewTypes, feedback, force);
            if (!ret) break;
        }

        if (ret && postcheck != null)
            ret = postcheck.Invoke();

        return ret;
    }
}


/// <summary>
/// Nur aus Kompatibilitätsgründen vorhanden. Stattdessen StorageService benutzen!
/// </summary>
[Obsolete("StorageService verwenden!")]
public class Storage : StorageService
{
    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="primarydb">Konfiguration der primären Datenbank</param>
    /// <param name="additionaldatabases">zusätzliche Datenbanken</param>
    public Storage(IConfiguration primarydb, IEnumerable<IConfiguration>? additionaldatabases) : base(primarydb, additionaldatabases)   
    { }
}
