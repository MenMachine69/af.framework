namespace AF.DATA;

/// <summary>
/// Schnittstelle für einen Logger, der Änderungen in Datenbanken protokollieren kann
/// 
/// jeder Logger wird Änderungen in einer bestimmten Verbindung protokollieren
/// </summary>
public interface ILogger
{
    /// <summary>
    /// Starte eine Transaktion (Änderungen werden bis zum CommitTransaction gepuffert)
    /// </summary>
    void BeginTransaction();

    /// <summary>
    /// Änderungen in der aktuellen Transaktion übertragen (Änderungen schreiben)
    /// </summary>
    void CommitTransaction();

    /// <summary>
    /// Rollback von Änderungen in der aktuellen Transaktion (Änderungen verwerfen)
    /// </summary>
    void RollbackTransaction();

    /// <summary>
    /// protokolliert eine Änderung
    /// </summary>
    /// <param name="loginfo">Informationen über die Änderung</param>
    void Log(ChangeInformation loginfo);

    /// <summary>
    /// Unterbindet das Schreibend er Log-Einträge.
    /// 
    /// Kann verwendet werden, wenn aus Performance-Gründen das sofortige Schreiben 
    /// der Log-Einträge für einen definierten Zeitraum unterbrochen werden soll.
    /// 
    /// Muss mit Resume beendet werden, damit angefallene Log-Einträge geschrieben werden.
    /// </summary>
    void Suspend();

    /// <summary>
    /// Setzt das Schreiben der Log-Einträge fort, wenn es vorher mit Suspend unterbrochen wurde.
    /// </summary>
    void Resume();

    /// <summary>
    /// Erwingt das Schreiben der noch vorhandenen Log-Einträge (z.B. beim Beenden der Anwendung).
    /// </summary>
    void Flush();
}

/// <summary>
/// Schnittstelle für Protokolleinträge
/// </summary>
public interface ILoggerEntry : ITable
{
    /// <summary>
    /// Art der Operation
    /// </summary>
    eLoggerOperation Operation { get; set; }

    /// <summary>
    /// Model/Record ID
    /// </summary>
    Guid ModelId { get; set; } 

    /// <summary>
    /// UserID
    /// </summary>
    Guid UserId { get; set; }

    /// <summary>
    /// Zeitstempel des Vorgangs
    /// </summary>
    DateTime TimeStamp { get; set; }

    /// <summary>
    /// Liste der Änderungen
    /// </summary>
    List<ILoggerEntryField> Changes { get; }
}

/// <summary>
/// Informationen über die geänderten Informationen
/// </summary>
public interface ILoggerEntryField : ITable
{
    /// <summary>
    /// id des Vorgangs, dem diese Änderung zugeordnet ist
    /// </summary>
    Guid OperationId { get; set; }

    /// <summary>
    /// alter Wert
    /// </summary>
    string OldValue { get; set; }

    /// <summary>
    /// neuer Wert
    /// </summary>
    string NewValue { get; set; }

    /// <summary>
    /// geänderte Eigenschaft
    /// </summary>
    string PropertyName { get; set; }
}

/// <summary>
/// Art des Log-Vorgangs
/// </summary>
public enum eLoggerOperation
{
    /// <summary>
    /// created
    /// </summary>
    Create,
    /// <summary>
    /// updated/changed
    /// </summary>
    Update,
    /// <summary>
    /// deleted
    /// </summary>
    Delete,
    /// <summary>
    /// any other...
    /// </summary>
    Other
}