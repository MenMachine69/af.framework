using System.Data;
using System.Data.Common;

namespace AF.DATA;

/// <summary>
/// Schnittstelle für eine Datenbankverbindung.
/// </summary>
public interface IConnection : IDisposable
{
    /// <summary>
    /// Ereignisse über AF.MessageHub unterdrücken
    /// </summary>
    bool Silent { get; set; }

    /// <summary>
    /// Liefert das DBConnection-Objekt der aktuellen Verbindung.
    ///
    /// <see cref="System.Data.Common.DbConnection"/>
    /// </summary>
    DbConnection? CurrentConnection { get; }

    /// <summary>
    /// wählt das Schema einer Tabelle/Ansicht aus der Datenbank als Datentabelle mit Informationen über jede Spalte in der Tabelle/Ansicht aus
    /// </summary>
    /// <param name="tableviewName">Name der Tabelle/des Views in der Datenbank</param>
    /// <returns>Schema als Datentabelle oder null, wenn kein Schema vorhanden ist</returns>
    DataTable? GetScheme(string tableviewName);

    /// <summary>
    /// Liest ein einzelnes Objekt aus einem Lesegerät
    /// </summary>
    /// <typeparam name="T">Typ des Objekts (Tabellen- oder Ansichtstyp)</typeparam>
    /// <param name="reader">Lesegerät, von dem das Objekt gelesen werden soll</param>
    /// <param name="dict">Eigenschaften, die gelesen werden sollen</param>
    /// <param name="tdesc">Typbeschreibung des Models (optional, Standard = null), Zuweisung verbessert die Performance!</param>
    /// <returns>das gelesene Objekt oder NULL</returns>
    T? ReadFromReader<T>(IDataReader reader, PropertyDescription[] dict, TypeDescription? tdesc = null) where T : IDataObject, new();

    /// <summary>
    /// Liest ein einzelnes Objekt aus einem Lesegerät
    /// </summary>
    /// <typeparam name="T">Typ des Objekts (Tabellen- oder Ansichtstyp)</typeparam>
    /// <param name="reader">Lesegerät, von dem das Objekt gelesen werden soll</param>
    /// <param name="fields">Name der zu lesenden Felder</param>
    /// <returns>das gelesene Objekt oder NULL</returns>
    T? ReadFromReader<T>(IDataReader reader, string[] fields) where T : IDataObject, new();

    /// <summary>
    /// read a single row/object from a reader.
    ///
    /// This method can be used for non-generic access.
    /// </summary>
    /// <param name="modelType">type of object</param>
    /// <param name="reader">reader</param>
    /// <param name="dict">dictionary with all properties to read</param>
    /// <param name="tdesc">TypeDescription des Models (optional, default = null), Zuweisung verbessert die Performance!</param>
    /// <returns>row object</returns>
    IDataObject? ReadFromReader(Type modelType, IDataReader reader, PropertyDescription?[] dict, TypeDescription? tdesc = null);

    /// <summary>
    /// Prüft, ob ein Wert eindeutig ist
    /// </summary>
    /// <typeparam name="T">Typ, der die zu prüfende Eigenschaft enthält</typeparam>
    /// <param name="data">zu prüfendes Objekt</param>
    /// <param name="field">Eigenschaft/Feldname, die geprüft werden soll</param>
    /// <param name="value">zu prüfender Wert</param>
    /// <returns>true = eindeutig, sonst false</returns>
    bool IsUnique<T>(T data, string field, object value) where T : ITable;

    /// <summary>
    /// Erzeugt eine neue Transaktion...
    /// </summary>
    void BeginTransaction();

    /// <summary>
    /// Änderungen für eine bestehende Transaktion festschreiben
    /// </summary>
    void CommitTransaction();

    /// <summary>
    /// Rollback von Änderungen für eine bestehende Transaktion
    /// </summary>
    void RollbackTransaction();

    /// <summary>
    /// Prüft, ob Tabelle oder Ansicht existiert und ob die Struktur gültig ist
    /// </summary>
    /// <typeparam name="T">Typ</typeparam>
    void Check<T>() where T : IDataObject;

    /// <summary>
    /// Prüft, ob Tabelle oder Ansicht existiert und ob die Struktur gültig ist
    /// </summary>
    /// <param name="type">Typ</param>
    void Check(Type type);

    /// <summary>
    /// Prüft, ob Tabelle oder Ansicht existiert und ob die Struktur gültig ist
    /// </summary>
    /// <typeparam name="T">Typ</typeparam>
    /// <param name="force">Erzwinge vollständige Prüfung</param>
    void Check<T>(bool force) where T : IDataObject;

    /// <summary>
    /// Prüft, ob Tabelle oder Ansicht existiert und ob die Struktur gültig ist
    /// </summary>
    /// <param name="type">Typ</param>
    /// <param name="force">Erzwinge vollständige Prüfung</param>
    void Check(Type type, bool force);

    /// <summary>
    /// Lädt den Wert eines einzelnen Feldes aus einer Tabelle
    /// </summary>
    /// <typeparam name="T">Typ des Feldes</typeparam>
    /// <param name="tdesc">Tabellen-/Ansichtsbeschreibung</param>
    /// <param name="id">ID des Datensatzes/Objekts, aus dem der Wert geladen werden soll</param>
    /// <param name="fieldname">Name des Feldes/Spalte</param>
    /// <returns>Wert dieses Feldes</returns>
    T? LoadValue<T>(Type tdesc, Guid id, string fieldname);


    /// <summary>
    /// Lädt den Wert eines einzelnen Feldes aus einer Tabelle
    /// </summary>
    /// <typeparam name="T">Typ des Feldes</typeparam>
    /// <typeparam name="TModel">Typ des Models, von dem der Value gelesen werden soll</typeparam>
    /// <param name="id">ID des Datensatzes/Objekts, aus dem der Wert geladen werden soll</param>
    /// <param name="fieldname">Name des Feldes/Spalte</param>
    /// <returns>Wert dieses Feldes</returns>
    T? LoadValue<T, TModel>(Guid id, string fieldname) where TModel : IDataObject;

    /// <summary>
    /// Mehrere Objekte aus Tabelle oder Ansicht auswählen
    /// </summary>
    /// <typeparam name="T">Typ der Objekte</typeparam>
    /// <param name="options">Optionen zum Lesen</param>
    /// <param name="where">WHERE-Klausel zum Lesen. ? als Platzhalter für einen Parameter/Argument verwenden.
    /// Die Klausel kann nur die WHERE-Klausel der Abfrage oder eine komplette SQL-Abfrage enthalten
    /// (einschließlich SELECT und mehr)</param>
    /// <param name="args">Argumente/Parameter für die WHERE-Klausel</param>
    /// <returns>BindingList mit gelesenen Objekten oder eine leere BindingList</returns>
    BindingList<T> Select<T>(ReadOptions options, string where, params object[]? args) where T : class, IDataObject, new();


    /// <summary>
    /// Mehrere Objekte aus Tabelle oder Ansicht auswählen und als IModelLinks zurückgeben.
    /// </summary>
    /// <typeparam name="T">Typ der Objekte</typeparam>
    /// <param name="options">Optionen zum Lesen</param>
    /// <param name="where">WHERE-Klausel zum Lesen. ? als Platzhalter für einen Parameter/Argument verwenden.
    /// Die Klausel kann nur die WHERE-Klausel der Abfrage oder eine komplette SQL-Abfrage enthalten
    /// (einschließlich SELECT und mehr)</param>
    /// <param name="args">Argumente/Parameter für die WHERE-Klausel</param>
    /// <returns>BindingList mit gelesenen Objekten oder eine leere BindingList</returns>
    BindingList<ModelInfo> SelectInfos<T>(ReadOptions options, string where, params object[]? args) where T : class, IDataObject, new();

    /// <summary>
    /// Mehrere Objekte aus Tabelle oder Ansicht auswählen
    /// </summary>
    /// <typeparam name="T">Typ der Objekte</typeparam>
    /// <param name="options">Optionen zum Lesen</param>
    /// <returns>BindingList mit gelesenen Objekten oder eine leere BindingList</returns>
    BindingList<T> Select<T>(ReadOptions options) where T : class, IDataObject, new();

    /// <summary>
    /// Mehrere Objekte aus Tabelle oder Ansicht auswählen und als IModelLinks zurückgeben.
    /// </summary>
    /// <typeparam name="T">Typ der Objekte</typeparam>
    /// <param name="options">Optionen zum Lesen</param>
    /// <returns>BindingList mit gelesenen Objekten oder eine leere BindingList</returns>
    BindingList<ModelInfo> SelectInfos<T>(ReadOptions options) where T : class, IDataObject, new();

    /// <summary>
    /// Mehrere Objekte aus Tabelle oder Ansicht auswählen
    /// </summary>
    /// <typeparam name="T">Typ der Objekte</typeparam>
    /// <param name="where">WHERE-Klausel zu lesen. Verwenden Sie ? als Platzhalter für einen Parameter/Argument.
    /// Die Klausel kann nur die WHERE-Klausel der Abfrage oder eine komplette SQL-Abfrage enthalten
    /// (einschließlich SELECT und mehr)</param>
    /// <param name="args">Argumente/Parameter für die WHERE-Klausel</param>
    /// <returns>BindingList mit gelesenen Objekten oder eine leere BindingList</returns>
    BindingList<T> Select<T>(string where, params object[]? args) where T : class, IDataObject, new();

    /// <summary>
    /// Mehrere Objekte aus Tabelle oder Ansicht auswählen und als IModelLinks zurückgeben.
    /// </summary>
    /// <typeparam name="T">Typ der Objekte</typeparam>
    /// <param name="where">WHERE-Klausel zu lesen. Verwenden Sie ? als Platzhalter für einen Parameter/Argument.
    /// Die Klausel kann nur die WHERE-Klausel der Abfrage oder eine komplette SQL-Abfrage enthalten
    /// (einschließlich SELECT und mehr)</param>
    /// <param name="args">Argumente/Parameter für die WHERE-Klausel</param>
    /// <returns>BindingList mit gelesenen Objekten oder eine leere BindingList</returns>
    BindingList<ModelInfo> SelectInfos<T>(string where, params object[]? args) where T : class, IDataObject, new();

    /// <summary>
    /// Alle Objekte aus Tabelle oder Ansicht auswählen
    /// </summary>
    /// <typeparam name="T">Typ der Objekte</typeparam>
    /// <returns>BindingList mit allen gelesenen Objekten oder eine leere BindingList, wenn Tabelle oder View leer ist</returns>
    BindingList<T> Select<T>() where T : class, IDataObject, new();

    /// <summary>
    /// Alle Objekte aus Tabelle oder Ansicht auswählen
    /// </summary>
    /// <typeparam name="T">Typ der Objekte</typeparam>
    /// <returns>BindingList mit allen gelesenen Objekten oder eine leere BindingList, wenn Tabelle oder View leer ist</returns>
    BindingList<ModelInfo> SelectInfos<T>() where T : class, IDataObject, new();

    /// <summary>
    /// Wählt alle Objekte aus der Tabelle oder Ansicht aus und gibt einen Vorwärtsleser für die ausgewählten Objekte zurück.
    /// Dieser Vorwärtsleser erlaubt es, Objekt für Objekt zu lesen, einschließlich Stornierung.
    /// </summary>
    /// <typeparam name="T">Typ der Objekte</typeparam>
    /// <returns>Leser für alle Objekte in einer Tabelle oder Ansicht</returns>
    IReader<T> SelectReader<T>() where T : class, IDataObject, new();

    /// <summary>
    /// Wählt alle Zeilen in einer Datentabelle aus
    /// </summary>
    /// <typeparam name="T">Tabellentyp</typeparam>
    /// <returns>Datentabelle</returns>
    DataTable SelectDataTable<T>() where T : class, IDataObject, new();

    /// <summary>
    /// Wählt alle Zeilen in einer Datentabelle aus
    /// </summary>
    /// <typeparam name="T">Tabellentyp</typeparam>
    /// <param name="query">Abfrage</param>
    /// <param name="args">Argumente</param>
    /// <returns>datierbar</returns>
    DataTable SelectDataTable<T>(string query, params object[] args) where T : class, IDataObject, new();

    /// <summary>
    /// Wählt alle Zeilen in einer Datentabelle aus
    /// </summary>
    /// <typeparam name="T">Tabelle/persistenter Typ</typeparam>
    /// <param name="options">Leseroptionen</param>
    /// <param name="query">Abfrage</param>
    /// <param name="args">Argumente</param>
    /// <returns>datierbar</returns>
    DataTable SelectDataTable<T>(ReadOptions options, string query, params object[]? args) where T : class, IDataObject, new();
    
    /// <summary>
    /// wählt Daten in einer Datentabelle aus
    /// </summary>
    /// <param name="query">Abfrage zur Ausführung</param>
    /// <param name="args">Argumente</param>
    /// <returns>datatable</returns>
    DataTable SelectDataTable(string query, params object[] args);

    /// <summary>
    /// Wählt mehrere Objekte aus einer Tabelle oder Ansicht aus und gibt einen Vorwärtsleser für die ausgewählten Objekte zurück.
    /// Dieser Vorwärtsleser erlaubt es, Objekt für Objekt zu lesen, einschließlich Stornierung.
    /// </summary>
    /// <typeparam name="T">Typ der Objekte</typeparam>
    /// <param name="where">WHERE Klausel zum Lesen. ? als Platzhalter für einen Parameter/Argument verwenden.
    /// Die Klausel kann nur die WHERE-Klausel der Abfrage oder eine komplette SQL-Abfrage enthalten
    /// (einschließlich SELECT und mehr)</param>
    /// <param name="args">Argumente/Parameter für die WHERE-Klausel</param>
    /// <returns>Reader für mehrere Objekte in einer Tabelle oder Ansicht</returns>
    IReader<T> SelectReader<T>(string where, params object[]? args) where T : class, IDataObject, new();

    /// <summary>
    /// Wählt mehrere Objekte aus einer Tabelle oder Ansicht aus und gibt einen Vorwärtsleser für die ausgewählten Objekte zurück.
    /// Dieser Vorwärtsleser erlaubt es, Objekt für Objekt zu lesen, einschließlich Stornierung.
    /// </summary>
    /// <typeparam name="T">Typ der Objekte</typeparam>
    /// <param name="options">Optionen für das Lesen</param>
    /// <param name="where">WHERE-Klausel zum Lesen. ? als Platzhalter für einen Parameter/Argument verwenden.
    /// Die Klausel kann nur die WHERE-Klausel der Abfrage oder eine komplette SQL-Abfrage enthalten
    /// (einschließlich SELECT und mehr)</param>
    /// <param name="args">Argumente/Parameter für die WHERE-Klausel</param>
    /// <returns>Reader für mehrere Objekte in einer Tabelle oder Ansicht</returns>
    IReader<T> SelectReader<T>(ReadOptions options, string where, params object[]? args) where T : class, IDataObject, new();

    /// <summary>
    /// Wählt ein einzelnes/das erste Objekt aus einer Tabelle oder Ansicht aus 
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>
    /// <param name="where">WHERE-Klausel zu lesen. ? als Platzhalter für einen Parameter/Argument verwenden.
    /// Die Klausel kann nur die WHERE-Klausel der Abfrage oder eine komplette SQL-Abfrage enthalten
    /// (einschließlich SELECT und mehr)</param>
    /// <param name="args">Argumente/Parameter für die WHERE-Klausel</param>
    /// <returns>das gelesene Objekt oder NULL, wenn nichts gefunden wurde</returns>
    T? SelectSingle<T>(string where, params object[]? args) where T : class, IDataObject, new();

    /// <summary>
    /// Wählt ein einzelnes/das erste Objekt aus einer Tabelle oder Ansicht aus und gibt den IModelLink des Objekts zurück.
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>
    /// <param name="where">WHERE-Klausel zu lesen. ? als Platzhalter für einen Parameter/Argument verwenden.
    /// Die Klausel kann nur die WHERE-Klausel der Abfrage oder eine komplette SQL-Abfrage enthalten
    /// (einschließlich SELECT und mehr)</param>
    /// <param name="args">Argumente/Parameter für die WHERE-Klausel</param>
    /// <returns>das gelesene Objekt oder NULL, wenn nichts gefunden wurde</returns>
    ModelInfo? SelectSingleInfo<T>(string where, params object[]? args) where T : class, IDataObject, new();

    /// <summary>
    /// Wählt ein einzelnes/das erste Objekt aus einer Tabelle oder Ansicht aus 
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>
    /// <param name="options">Optionen zum Lesen</param>
    /// <param name="where">WHERE-Klausel zum Lesen. ? als Platzhalter für einen Parameter/Argument verwenden.
    /// Die Klausel kann nur die WHERE-Klausel der Abfrage oder eine komplette SQL-Abfrage enthalten
    /// (einschließlich SELECT und mehr)</param>
    /// <param name="args">Argumente/Parameter für die WHERE-Klausel</param>
    /// <returns>das gelesene Objekt oder NULL, wenn nichts gefunden wurde</returns>
    T? SelectSingle<T>(ReadOptions options, string where, params object[]? args) where T : class, IDataObject, new();

    /// <summary>
    /// Wählt ein einzelnes/das erste Objekt aus einer Tabelle oder Ansicht aus und gibt den IModelLink des Objekts zurück.
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>
    /// <param name="options">Optionen zum Lesen</param>
    /// <param name="where">WHERE-Klausel zum Lesen. ? als Platzhalter für einen Parameter/Argument verwenden.
    /// Die Klausel kann nur die WHERE-Klausel der Abfrage oder eine komplette SQL-Abfrage enthalten
    /// (einschließlich SELECT und mehr)</param>
    /// <param name="args">Argumente/Parameter für die WHERE-Klausel</param>
    /// <returns>das gelesene Objekt oder NULL, wenn nichts gefunden wurde</returns>
    ModelInfo? SelectSingleInfo<T>(ReadOptions options, string where, params object[]? args) where T : class, IDataObject, new();

    /// <summary>
    /// Lädt ein einzelnes Objekt aus einer Tabelle oder Ansicht auf der Basis seines PrimaryKey
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>
    /// <param name="guid">PrimaryKey des Objekts</param>
    /// <returns>das gelesene Objekt oder NULL, wenn nichts gefunden wurde</returns>
    T? Load<T>(Guid guid) where T : class, IDataObject, new();

    /// <summary>
    /// Lädt ein einzelnes Objekt aus einer Tabelle oder Ansicht auf der Basis seines PrimaryKey
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>
    /// <param name="guid">PrimaryKey des Objekts</param>
    /// <param name="into">Einlesen in dieses Objekt, statt neues zu erzeugen</param>
    /// <param name="ignorecache">Objekt nicht aus Cache lesen, sondern immer aus DB</param>
    /// <returns>das gelesene Objekt oder NULL, wenn nichts gefunden wurde</returns>
    T? Load<T>(Guid guid, T? into, bool ignorecache) where T : class, IDataObject, new();

    /// <summary>
    /// Lädt ein einzelnes Objekt aus einer Tabelle oder Ansicht auf der Basis seines PrimaryKey und gibt dem IModelLink des Objekts zurück.
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>
    /// <param name="guid">PrimaryKey des Objekts</param>
    /// <param name="ignorecache">Objekt nicht aus Cache lesen, sondern immer aus DB</param>
    /// <returns>das gelesene Objekt oder NULL, wenn nichts gefunden wurde</returns>
    ModelInfo? LoadInfo<T>(Guid guid, bool ignorecache = false) where T : class, IDataObject, new();

    /// <summary>
    /// Model erneut aus der Datenbank lesen.
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>
    /// <param name="model">das erneut aus der Datenbank zu lesende Model</param>
    /// <returns>true, wenn gelesen, sonst false (Model nicht gefunden)</returns>
    bool ReLoad<T>(T model) where T : class, IDataObject, new();

    /// <summary>
    /// Prüfen, ob eine Tabelle ein Objekt mit dem angegebenen PrimaryKey enthält
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>
    /// <param name="guid">PrimaryKey des Objekts</param>
    /// <returns>true, wenn ein Objekt existiert, sonst false</returns>
    bool Exist<T>(Guid guid) where T : ITable;

    /// <summary>
    /// Speichert ein Objekt in der Tabelle in der Datenbank und speichert/aktualisiert nur einige Felder
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>
    /// <param name="record">das zu speichernde Objekt</param>
    /// <param name="fields">zu speichernde/zu aktualisierende Felder</param>
    /// <returns>true bei Erfolg, sonst false</returns>
    bool Save<T>(T record, string[]? fields = null) where T : class, ITable;

    /// <summary>
    /// Speichert ein Objekt in der Tabelle in der Datenbank und speichert/aktualisiert nur einige Felder
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>
    /// <param name="options">Optionen zum Speichern</param>
    /// <param name="record">das zu speichernde Objekt</param>
    /// <param name="forcecreate">Immer einen neuen Datensatz anlegen</param>
    /// <param name="writeallfields">Schreibe alle Felder, standardmäßig werden nur geänderte Felder geschrieben, wenn der Datensatz nicht neu ist (= bei Update).</param>
    /// <returns>true wenn erfolgreich, sonst false</returns>
    bool Save<T>(ReadOptions options, T record, bool forcecreate = false, bool writeallfields = false) where T : class, ITable;

    /// <summary>
    /// Mehrere Objekte in einer Tabelle in der Datenbank speichern
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>
    /// <param name="records">Liste der zu speichernden Objekte</param>
    /// <param name="fields">zu speichernde/zu aktualisierende Felder</param>
    /// <returns>Anzahl der gespeicherten Objekte</returns>
    int Save<T>(IEnumerable<T> records, string[]? fields = null) where T : class, ITable;

    /// <summary>
    /// Mehrere Objekte in einer Tabelle in der Datenbank speichern
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>
    /// <param name="options">Optionen zum Speichern</param>
    /// <param name="records">Liste der zu speichernden Objekte</param>
    /// <returns>Anzahl der gespeicherten Objekte</returns>
    int Save<T>(ReadOptions options, IEnumerable<T> records) where T : class, ITable;

    /// <summary>
    /// Löschen eines einzelnen Datensatzes
    /// </summary>
    /// <typeparam name="T">Typ der Tabelle</typeparam>
    /// <param name="record">der zu löschende Datensatz</param>
    /// <returns>true, wenn gelöscht, sonst false</returns>
    bool Delete<T>(T record) where T : class, ITable;

    /// <summary>
    /// Löschen eines einzelnen Datensatzes anhand seines PrimaryKey
    /// </summary>
    /// <typeparam name="T">Typ der Datensätze</typeparam>
    /// <param name="id">PrimaryKey des zu löschenden Datensatzes</param>
    /// <returns>true, wenn gelöscht, sonst false</returns>
    bool Delete<T>(Guid id) where T : class, ITable, new();

    /// <summary>
    /// Mehrere Datensätze löschen
    /// </summary>
    /// <typeparam name="T">Typ der Tabelle</typeparam>
    /// <param name="records">Liste der zu löschenden Datensätze</param>
    /// <returns>Anzahl der gelöschten Datensätze</returns>
    int Delete<T>(IEnumerable<T> records) where T : class, ITable;

    /// <summary>
    /// Mehrere Objekte aus der Tabelle löschen
    /// </summary>
    /// <typeparam name="T">Typ der Objekte</typeparam>
    /// <param name="where">WHERE-Klausel zum Löschen. ? als Platzhalter für einen Parameter/Argument verwenden.
    /// Die Klausel kann nur die WHERE-Klausel der Abfrage enthalten</param>
    /// <param name="args">Argumente/Parameter für die WHERE-Klausel</param>
    /// <returns>Anzahl der gelöschten Datensätze</returns>
    int Delete<T>(string where, params object[]? args) where T : ITable;


    /// <summary>
    /// Zählt die Anzahl der Datensätze in einer Tabelle oder Ansicht
    /// </summary>
    /// <typeparam name="T">Typ der Tabelle oder Ansicht</typeparam>
    /// <param name="options">Optionen</param>
    /// <param name="query">Abfrage (where-Klausel)</param>
    /// <param name="args">Argumente für die Abfrage</param>
    /// <returns>Anzahl der Datensätze</returns>
    int Count<T>(ReadOptions options, string query, params object[]? args) where T : IDataObject;

    /// <summary>
    /// Zählt die Anzahl der Datensätze in einer Tabelle oder Ansicht
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query">Abfrage (where-Klausel)</param>
    /// <param name="args">Argumente für die Abfrage</param>
    /// <returns>Anzahl der Datensätze</returns>
    int Count<T>(string query, params object[]? args) where T : IDataObject;

    /// <summary>
    /// Zählt die Anzahl der Datensätze in einer Tabelle oder Ansicht
    /// </summary>
    /// <typeparam name="T">Typ der Tabelle oder Ansicht</typeparam>
    /// <returns>Anzahl der Datensätze in Tabelle oder Ansicht</returns>
    int Count<T>() where T : IDataObject;

    /// <summary>
    /// ruft die Summe eines Feldes ab
    /// </summary>
    /// <typeparam name="T">Tabelle tppe</typeparam>
    /// <param name="options">Optionen für die Abfrage</param>
    /// <param name="field">Name des Feldes</param>
    /// <param name="query">Abfrage (where-Klausel)</param>
    /// <param name="args">Argumente für die Abfrage</param>
    /// <returns>die Summe </returns>
    decimal Sum<T>(ReadOptions options, string field, string query, params object[]? args) where T : IDataObject;

    /// <summary>
    /// ruft die Summe eines Feldes ab
    /// </summary>
    /// <typeparam name="T">Typ der Tabelle oder Ansicht</typeparam>
    /// <param name="field">Name des Feldes</param>
    /// <param name="query">Abfrage (where-Klausel)</param>
    /// <param name="args">Argumente für die Abfrage</param>
    /// <returns>die Summe </returns>
    decimal Sum<T>(string field, string query, params object[]? args) where T : IDataObject;

    /// <summary>
    /// ruft die Summe eines Feldes ab
    /// </summary>
    /// <typeparam name="T">Typ der Tabelle oder Ansicht</typeparam>
    /// <param name="field">Name des Feldes</param>
    /// <returns>die Summe </returns>
    decimal Sum<T>(string field) where T : IDataObject;

    /// <summary>
    /// führt eine gespeicherte Prozedur aus, die einen einzelnen Wert zurückgibt
    /// </summary>
    /// <typeparam name="T">erwarteter Rückgabetyp</typeparam>
    /// <param name="procedureName">Name der Prozedur</param>
    /// <param name="args">Parameter</param>
    /// <returns>Wert</returns>
    T? ExecuteProcedureScalar<T>(string procedureName, params object[]? args);

    /// <summary>
    /// führt eine gespeicherte Prozedur aus und gibt die Anzahl der betroffenen Zeilen zurück
    /// </summary>
    /// <param name="procedureName">Name der Prozedur</param>
    /// <param name="args">Argumente für die Ausführung</param>
    /// <returns>Anzahl der betroffenen Zeilen</returns>
    int ExecuteProcedure(string procedureName, params object[]? args);

    /// <summary>
    /// führt einen Sql-Befehl aus, der einen einzelnen Wert zurückgibt
    /// Zeile in der von der Abfrage zurückgegebenen Ergebnismenge.
    /// Zusätzliche Spalten oder Zeilen werden ignoriert.
    /// </summary>
    /// <typeparam name="T">Rückgabetyp</typeparam>
    /// <param name="commandQuery">sql-Abfrage</param>
    /// <param name="args">Argumente für die Abfrage</param>
    /// <returns>der Wert oder null</returns>
    T? ExecuteCommandScalar<T>(string commandQuery, params object[]? args);

    /// <summary>
    /// führt einen sql-Befehl aus und gibt die Anzahl der betroffenen Zeilen zurück
    /// </summary>
    /// <param name="commandQuery">sql-Befehl zur Ausführung</param>
    /// <param name="args">Argumente für die Abfrage</param>
    /// <returns>Anzahl der betroffenen Zeilen</returns>
    int ExecuteCommand(string commandQuery, params object[]? args);


    #region structure

    /// <summary>
    /// prüft, ob eine Tabelle existiert
    /// </summary>
    /// <param name="tableName">Tabellenname</param>
    /// <returns>true Tabelle existiert</returns>
    bool ExistTable(string tableName);

    /// <summary>
    /// prüft, ob eine Prozedur existiert
    /// </summary>
    /// <param name="procedureName">Prozedurname</param>
    /// <returns>true Prozedur existiert</returns>
    bool ExistProcedure(string procedureName);

    /// <summary>
    /// prüft, ob ein Trigger existiert
    /// </summary>
    /// <param name="triggerName">Triggername</param>
    /// <returns>true Trigger existiert</returns>
    bool ExistTrigger(string triggerName);

    /// <summary>
    /// prüft, ob eine Ansicht existiert
    /// </summary>
    /// <param name="viewName">Name der Ansicht</param>
    /// <returns>true Ansicht existiert</returns>
    bool ExistView(string viewName);

    /// <summary>
    /// prüft, ob ein Index existiert
    /// </summary>
    /// <param name="indexName">Indexname</param>
    /// <param name="tablename">Name der Tabelle</param>
    /// <returns>true wenn Index existiert</returns>
    bool ExistIndex(string indexName, string tablename);

    /// <summary>
    /// prüft, ob eine Fremdschlüssel-Beschränkung existiert
    /// </summary>
    /// <param name="constraintName">Indexname</param>
    /// <param name="tablename">Name der Tabelle</param>
    /// <returns>true wenn Index vorhanden</returns>
    bool ExistConstraint(string constraintName, string tablename);

    /// <summary>
    /// Löschen einer Tabelle aus der Datenbank
    /// </summary>
    /// <param name="tableName">Tabellenname</param>
    void DropTable(string tableName);

    /// <summary>
    /// Löschen einer Prozedur aus der Datenbank
    /// </summary>
    /// <param name="procedureName">Prozedurname</param>
    void DropProcedure(string procedureName);

    /// <summary>
    /// Löschen eines Triggers aus der Datenbank
    /// </summary>
    /// <param name="triggerName">Triggername</param>
    void DropTrigger(string triggerName);

    /// <summary>
    /// Löschen einer Ansicht aus der Datenbank 
    /// </summary>
    /// <param name="viewName">Name der Ansicht</param>
    void DropView(string viewName);

    /// <summary>
    /// Löschen eines Index aus der Datenbank
    /// </summary>
    /// <param name="indexName">Indexname</param>
    /// <param name="tableName">Tabellenname</param>
    void DropIndex(string indexName, string tableName);

    /// <summary>
    /// löscht eine Fremdschlüssel-Beschränkung aus der Datenbank
    /// </summary>
    /// <param name="constraintName">Beschränkungsname</param>
    /// <param name="tableName">Tabellenname</param>
    void DropConstraint(string constraintName, string tableName);

    /// <summary>
    /// Erstellen einer Tabelle
    /// </summary>
    /// <param name="tableName">Tabellenname</param>
    /// <param name="keyField">Primärschlüsselfeldname</param>
    /// <param name="createdField">Name des Feldes, das zum Zeitstempel erstellt wurde</param>
    /// <param name="changedField">Name des Feldes für die Änderung zum Zeitpunkt des Zeitstempels</param>
    void CreateTable(string tableName, string keyField, string createdField, string changedField);

    /// <summary>
    /// Erstellen einer Prozedur
    /// </summary>
    /// <param name="procedureName">Prozedurname</param>
    /// <param name="code">Prozedurencode</param>
    void CreateProcedure(string procedureName, string code);

    /// <summary>
    /// Erstellen eines Triggers
    /// </summary>
    /// <param name="triggerName">Triggername</param>
    /// <param name="tableName">Tabellenname</param>
    /// <param name="eventType">Trigger-Ereignis</param>
    /// <param name="code">Quellcode des Auslösers</param>
    void CreateTrigger(string triggerName, string tableName, eTriggerEvent eventType, string code);

    /// <summary>
    /// Als Ansicht erstellen
    /// </summary>
    /// <param name="viewName">Name der Ansicht</param>
    /// <param name="fields">Ansicht Felder</param>
    /// <param name="query">Ansicht abfrage</param>
    void CreateView(string viewName, string fields, string query);

    /// <summary>
    /// Erstellen eines Index
    /// </summary>
    /// <param name="indexName">Indexname</param>
    /// <param name="tableName">Tabellenname</param>
    /// <param name="expression">Index-Ausdruck</param>
    /// <param name="unique">Index mit eindeutigen Werten erstellen</param>
    /// <param name="descending">Index mit absteigender Reihenfolge erstellen</param>
    public void CreateIndex(string indexName, string tableName, string expression, bool unique, bool descending);

    /// <summary>
    /// Erstellen einer Fremdschlüssel-Beschränkung
    /// </summary>
    /// <param name="constraintName">Name der Einschränkung</param>
    /// <param name="tableName">Tabellenname</param>
    /// <param name="targetTable">Zieltabelle</param>
    /// <param name="targetField">Feldname in der Zieltabelle (meist das Primärschlüsselfeld)</param>
    /// <param name="fieldName"></param>
    /// <param name="constraintDelete"></param>
    /// <param name="constraintUpdate"></param>
    public void CreateForeignKeyConstraint(string constraintName, string tableName, string fieldName,
        string targetTable, string targetField, eConstraintOperation constraintUpdate,
        eConstraintOperation constraintDelete);

    /// <summary>
    /// Erstellt ein Feld in der Datenbank
    /// </summary>
    /// <param name="fieldName">Feldname</param>
    /// <param name="tableName">Tabellenname</param>
    /// <param name="fieldType">Feldtyp</param>
    /// <param name="fieldsize">Feldgröße (nur für String-Felder erforderlich - 0 bedeutet, dass das String-Feld ein Blob-Feld mit unbegrenzter Größe ist)</param>
    public void CreateField(string fieldName, string tableName, Type fieldType, int fieldsize);

    /// <summary>
    /// Selektieren eines Dictionary (ein Feld als Key, ein Feld als Value). 
    /// </summary>
    /// <typeparam name="T">Typ der Objekte</typeparam>
    /// <typeparam name="TKey">Typ des Key-Feldes</typeparam>
    /// <typeparam name="TValue">Typ des Value-Feldes</typeparam>
    /// <param name="nameKeyField">Name des Key-Feldes (z.B. "SYS_ID")</param>
    /// <param name="nameValueField">Name des Value-Feldes (z.B. "NAME")</param>
    /// <param name="where">WHERE-Klausel zu lesen. Verwenden Sie ? als Platzhalter für einen Parameter/Argument.
    /// Die Klausel kann nur die WHERE-Klausel der Abfrage oder eine komplette SQL-Abfrage enthalten
    /// (einschließlich SELECT und mehr)</param>
    /// <param name="args">Argumente/Parameter für die WHERE-Klausel</param>
    /// <returns>BindingList mit gelesenen Objekten oder eine leere BindingList</returns>
    public Dictionary<TKey, TValue> SelectDictionary<T, TKey, TValue>(string nameKeyField, string nameValueField, string where, params object[]? args) 
        where T : class, IDataObject, new() 
        where TKey : notnull 
        where TValue : notnull;
    #endregion
}