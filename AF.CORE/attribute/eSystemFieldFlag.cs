namespace AF.DATA;

/// <summary>
/// Flags für Felder zur Kennzeichnung systemrelevanter Felder
/// </summary>
public enum eSystemFieldFlag
{
    /// <summary>
    /// PrimaryKey
    /// </summary>
    PrimaryKey,

    /// <summary>
    /// Timestamp angelegt
    /// </summary>
    TimestampCreated,

    /// <summary>
    /// Timestamp geändert
    /// </summary>
    TimestampChanged,

    /// <summary>
    /// Archiviert
    /// </summary>
    ArchiveFlag,

    /// <summary>
    /// ID des Ordners
    /// </summary>
    Folder,
        
    /// <summary>
    /// Ohne (alle anderen Felder)
    /// </summary>
    None
}