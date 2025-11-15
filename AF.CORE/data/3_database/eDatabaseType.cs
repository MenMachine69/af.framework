
namespace AF.DATA;

/// <summary>
/// Typ der Datenbank
/// </summary>
public enum eDatabaseType
{
    /// <summary>
    /// Microsoft SQL database
    /// </summary>
    [AFDescription("DBTYPE_MSSQL", typeof(CoreStrings))]
    MsSql = 0,
    /// <summary>
    /// Microsoft Azure SQL database
    /// </summary>
    [AFDescription("DBTYPE_AZURESQL", typeof(CoreStrings))]
    AzureSql = 1,
    /// <summary>
    /// PostgreSQL database
    /// </summary>
    [AFDescription("DBTYPE_POSTGRESQL", typeof(CoreStrings))]
    PostgreSql = 2,
    /// <summary>
    /// Firebird SQL database (server)
    /// </summary>
    [AFDescription("DBTYPE_FIREBIRDSQL", typeof(CoreStrings))]
    FirebirdSql = 3,
    /// <summary>
    /// Firebird SQL database (embedded)
    /// </summary>
    [AFDescription("DBTYPE_FIREBIRDEMB", typeof(CoreStrings))]
    FirebirdEmbeddedSql = 4,
    /// <summary>
    /// Firebird SQL database (embedded)
    /// </summary>
    [AFDescription("DBTYPE_ODBC", typeof(CoreStrings))]
    Odbc = 5
}