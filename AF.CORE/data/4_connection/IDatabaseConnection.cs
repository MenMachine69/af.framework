using System.Data;
using System.Data.Common;

namespace AF.DATA;

/// <summary>
/// Beschreibung einer einfachen Datenbankschnittstelle (ohne ORM).
///
/// Diese Schnittstelle bietet die Möglichkeit, sich mit einer Datenbank zu verbinden,
/// Strukturinformationen zu lesen und SQL-Abfragen auszuführen. Diese Abfragen unterstützen
/// auch das Auslesen von Daten in Form einer einfachen Tabelle.
/// </summary>
public interface IDatabaseConnection : IDatasource
{
    /// <summary>
    /// Name der Datenbank.
    /// </summary>
    public string DatabaseName { get; set; }

    /// <summary>
    /// Typ der Datenbank.
    /// </summary>
    public eDatabaseType DatabaseType { get; }

    /// <summary>
    /// Verbindungsstring zur Datenbank.
    /// </summary>
    public string ConnectString { get; init; }

    /// <summary>
    /// Zugangsdaten für die Datenbank (Benutzer, Kennwort).
    /// </summary>
    public Credentials? Credentials { get; set; }

    /// <summary>
    /// Erstellt eine Datenbankverbindung unter Verwendung des ConnectStrings und der
    /// angegebenen Credentials.
    /// </summary>
    /// <returns>die Verbindung</returns>
    public DbConnection Connect();

    /// <summary>
    /// Erstellt aus einer SQL-Abfrage eine DbCommand.
    /// </summary>
    /// <param name="query">auszuführende Abfrage</param>
    /// <param name="parameters">Parameter für die Abfrage</param>
    /// <param name="variablen">Variablenwerte für die Abfrage</param>
    /// <returns>DataTable</returns>
    public DbCommand GetCommand(string query, IList<VariableUserValue>? variablen = null, params object[] parameters);

    /// <summary>
    /// Erstellt aus einer SQL-Abfrage eine DbCommand.
    ///
    /// Beim Erstellen wird der übergebene, datenbankspezifische Übersetzer verwendet.
    /// </summary>
    /// <param name="translator">datenbankspezifischer Translator</param>
    /// <param name="query">auszuführende Abfrage</param>
    /// <param name="parameters">Parameter für die Abfrage</param>
    /// <param name="variablen">Variablenwerte für die Abfrage</param>
    /// <returns>DataTable</returns>
    public DbCommand GetCommand(ITranslator translator, string query, IList<VariableUserValue>? variablen = null, params object[] parameters);

    /// <summary>
    /// Liefert das Schema der Datenbank
    /// </summary>
    /// <returns>Schema der Datenbank</returns>
    public DatabaseScheme GetScheme();

    /// <summary>
    /// Query ausführen und einen Reader zurückgeben, über den die selektierten Daten gelesen werden können.
    /// <seealso cref="System.Data.Common.DbCommand.ExecuteReader()"/>
    /// </summary>
    /// <param name="connection">Datenbankverbindung</param>
    /// <param name="command">Abfrage</param>
    /// <returns>Anzahl der betroffenen Zeilen</returns>
    public DbDataReader ExecuteReader(DbConnection connection, DbCommand command);

    /// <summary>
    /// Query ausführen und ein DataTable zurückgeben, dass die gelesenen Informationen enthält.
    /// </summary>
    /// <param name="connection">Datenbankverbindung</param>
    /// <param name="command">Abfrage</param>
    /// <returns>Anzahl der betroffenen Zeilen</returns>
    public DataTable ExecuteTable(DbConnection connection, DbCommand command);


    /// <summary>
    /// Query ausführen und die Anzahl der betroffenen Zeilen zurückgeben.
    /// <seealso cref="System.Data.Common.DbCommand.ExecuteNonQuery"/>
    /// </summary>
    /// <param name="connection">Datenbankverbindung</param>
    /// <param name="command">Abfrage</param>
    /// <returns>Anzahl der betroffenen Zeilen</returns>
    public int ExecuteCommand(DbConnection connection, DbCommand command);

    /// <summary>
    /// Query ausführen und einen einzelnen Wert zurückgeben.
    /// <seealso cref="System.Data.Common.DbCommand.ExecuteScalar"/>
    /// </summary>
    /// <param name="connection">Datenbankverbindung</param>
    /// <param name="command">Abfrage</param>
    /// <typeparam name="T">Typ des zu ermittelnden Wertes - stimmt dieser nicht mit der Rückgabe aus der Datenbank überein, 
    /// wird eine Exception ausgelöst.</typeparam>
    /// <returns>der ermittelte Wert oder NULL</returns>
    public T? ExecuteCommandScalar<T>(DbConnection connection, DbCommand command);

    /// <summary>
    /// Liefert die Beschreibung einer Spalte.
    /// </summary>
    /// <param name="tablename">Name der Tabelle</param>
    /// <param name="columnname">Name der Spalte</param>
    /// <returns>Beschreibung der Spalte (kann leer sein)</returns>
    public string GetComment(string tablename, string columnname);

}


