namespace AF.DATA;

/// <summary>
/// Basisklasse für Tabellen, die Standardfelder enthält:
/// 
/// SYS_ID          Eindeutiger Schlüssel
/// SYS_AFEATED     Zeitstempel erstellt
/// SYS_CHANGED     Zeitstempel letzte Änderung
/// SYS_ARCHIVED    Kennzeichen: wird archiviert
///
/// Verwenden Sie diese Klasse für Tabellen, die diese Standardfelder enthalten sollen.
/// </summary>
public abstract class DefaultTable : Table
{
    private Guid _SYS_ID = Guid.Empty;
    private DateTime _SYS_AFEATED = DateTime.MinValue;
    private DateTime _SYS_CHANGED = DateTime.MinValue;
    private bool _SYS_ARCHIVED;

    /// <summary>
    /// Primärschlüssel des Objekts.
    /// </summary>
    public override Guid PrimaryKey 
    { 
        get => SYS_ID;
        set => SYS_ID = value;
    }


    /// <summary>
    /// DateTime angelegt
    /// </summary>
    public override DateTime CreateDateTime
    {
        get => SYS_AFEATED;
        set => SYS_AFEATED = value;
    }

    /// <summary>
    /// DateTime letzte Änderung
    /// </summary>
    public override DateTime UpdateDateTime
    {
        get => SYS_CHANGED;
        set => SYS_CHANGED = value;
    }

    /// <summary>
    /// Archiviert
    /// </summary>
    public override bool IsArchived
    {
        get => SYS_ARCHIVED;
        set => SYS_ARCHIVED = value;
    }

    /// <summary>
    /// Primärschlüssel des Objekts.
    /// </summary>
    [AFContext(typeof(CoreStrings))]
    [AFField(SystemFieldFlag = eSystemFieldFlag.PrimaryKey)]
    [AFBinding(ReadOnly = true)]
    [AFGridColumn(Visible = false)]
    public Guid SYS_ID
    {
        get => _SYS_ID;
        set => Set(ref _SYS_ID, value);
    }

    /// <summary>
    /// DateTime angelegt
    /// </summary>
    [AFContext(typeof(CoreStrings))]
    [AFField(SystemFieldFlag = eSystemFieldFlag.TimestampCreated)]
    [AFBinding(DisplayFormat = "d", ReadOnly = true)]
    [AFGridColumn(DisplayFormat = "d", AllowEdit = false, InStyles = eGridStyle.Full, Visible = false)]
    public DateTime SYS_AFEATED
    {
        get => _SYS_AFEATED;
        set => Set(ref _SYS_AFEATED, value);
    }

    /// <summary>
    /// DateTime letzte Änderung
    /// </summary>
    [AFContext(typeof(CoreStrings))]
    [AFField(SystemFieldFlag = eSystemFieldFlag.TimestampChanged)]
    [AFBinding(DisplayFormat = "d", ReadOnly = true)]
    [AFGridColumn(DisplayFormat = "d", AllowEdit = false, InStyles = eGridStyle.Full, Visible = false)]
    public DateTime SYS_CHANGED
    {
        get => _SYS_CHANGED;
        set => Set(ref _SYS_CHANGED, value);
    }

    /// <summary>
    /// Markiert das Objekt als archiviert
    ///
    /// ACHTUNG! AFBindung ist mit readonly definiert. Bearbeiten Sie diesen Wert nicht direkt in Masken.
    /// Anstelle der direkten Bearbeitung setzen Sie diesen Wert über einen Befehl oder etwas anderes.
    /// </summary>
    [AFContext(typeof(CoreStrings))]
    [AFField(SystemFieldFlag = eSystemFieldFlag.ArchiveFlag)]
    [AFBinding]
    [AFGridColumn(FixedWidth = true, Width = 80, ColumnIndex = 99)]
    public bool SYS_ARCHIVED
    {
        get => _SYS_ARCHIVED;
        set => Set(ref _SYS_ARCHIVED, value);
    }

    /// <summary>
    /// Gibt an, ob der Datensatz neu ist.
    /// </summary>
    [AFContext(typeof(CoreStrings))]
    [AFBinding]
    public bool IsNew => SYS_ID.IsEmpty();
}

/// <summary>
/// Basisklasse für Tabellen, die Standardfelder enthält und Ordner unterstützt:
/// 
/// SYS_ID          Eindeutiger Schlüssel
/// SYS_AFEATED     Zeitstempel erstellt
/// SYS_CHANGED     Zeitstempel letzte Änderung
/// SYS_ARCHIVED    Kennzeichen: wird archiviert
/// SYS_FOLDER      Name des Ordners
/// 
/// Verwenden Sie diese Klasse für Tabellen, die diese Standardfelder enthalten sollen.
/// </summary>
public abstract class DefaultTableFolder : DefaultTable
{
    private string _SYS_FOLDER = String.Empty;

    /// <summary>
    /// ID des Ordners
    /// </summary>
    [AFContext(typeof(CoreStrings))]
    [AFField(MaxLength = 100, SystemFieldFlag = eSystemFieldFlag.Folder)]
    [AFBinding]
    [AFGridColumn(Visible = false)]
    public string SYS_FOLDER
    {
        get => _SYS_FOLDER;
        set => Set(ref _SYS_FOLDER, value);
    }
}
