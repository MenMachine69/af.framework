namespace AF.DATA;

/// <summary>
/// Basisklasse für Views, die Standardfelder enthält:
/// 
/// SYS_ID          Eindeutiger Schlüssel (aus Primärtabelle)
/// SYS_AFEATED     Zeitstempel erstellt (aus Primärtabelle)
/// SYS_CHANGED     Zeitstempel letzte Änderung (aus Primärtabelle)
/// SYS_ARCHIVED    Kennzeichen: wird archiviert (aus Primärtabelle)
///
/// Verwenden Sie diese Klasse für Ansichten, die diese Standardfelder enthalten sollen.
///
/// ACHTUNG! Der Aliasname der primaeren Tabelle/View in der View
/// Definition muss 'pri' sein - denn alle Standardfelder sind definiert als
/// [AFField(SourceField = "pri.fldname"...)!
/// </summary>
public abstract class DefaultView : View
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
    [AFField(SourceField = "pri.SYS_ID", SystemFieldFlag = eSystemFieldFlag.PrimaryKey)]
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
    [AFField(SourceField = "pri.SYS_AFEATED", SystemFieldFlag = eSystemFieldFlag.TimestampCreated)]
    [AFBinding(DisplayFormat = "d", ReadOnly = true)]
    [AFGridColumn(DisplayFormat = "d", AllowEdit = false, InStyles = eGridStyle.Full)]
    public DateTime SYS_AFEATED
    {
        get => _SYS_AFEATED;
        set => Set(ref _SYS_AFEATED, value);
    }

    /// <summary>
    /// DateTime letzte Änderung
    /// </summary>
    [AFContext(typeof(CoreStrings))]
    [AFField(SourceField = "pri.SYS_CHANGED", SystemFieldFlag = eSystemFieldFlag.TimestampChanged)]
    [AFBinding(DisplayFormat = "d", ReadOnly = true)]
    [AFGridColumn(DisplayFormat = "d", AllowEdit = false, InStyles = eGridStyle.Full)]
    public DateTime SYS_CHANGED
    {
        get => _SYS_CHANGED;
        set => Set(ref _SYS_CHANGED, value);
    }

    /// <summary>
    /// Markiert das Objekt als archiviert
    ///
    /// ACHTUNG! AFBindung ist mit readonly definiert. Bearbeiten Sie diesen Wert nicht direkt in Masken.
    /// Statt der direkten Bearbeitung setzen Sie diesen Wert über einen Befehl oder etwas anderes.
    /// </summary>
    [AFContext(typeof(CoreStrings))]
    [AFField(SourceField = "pri.SYS_ARCHIVED", SystemFieldFlag = eSystemFieldFlag.ArchiveFlag)]
    [AFBinding(ReadOnly = true)]
    [AFGridColumn(AllowEdit = false, FixedWidth = true, Width = 80, ColumnIndex = 99)]
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
/// Basisklasse für Views, die Standardfelder enthält und Ordner unterstützt:
///  
/// SYS_ID          eindeutiger Schlüssel (aus Primärtabelle)
/// SYS_AFEATED     Zeitstempel erstellt (aus Primärtabelle)
/// SYS_CHANGED     Zeitstempel letzte Änderung (aus Primärtabelle)
/// SYS_ARCHIVED    Kennzeichen: wird archiviert (aus Primärtabelle)
/// SYS_FOLDER_ID   ID des Ordners (aus Primärtabelle)
/// 
/// Verwenden Sie diese Klasse für Ansichten, die diese Standardfelder enthalten sollen.
///
/// ACHTUNG! Der Aliasname der primären Tabelle/View in der View
/// Definition muss 'pri' sein - denn alle Standardfelder sind definiert als
/// [AFField(SourceField = "pri.fldname"...)!
/// </summary>
public abstract class DefaultViewFolder : DefaultView
{
    private string _SYS_FOLDER = string.Empty;

    /// <summary>
    /// ID des Ordners
    /// </summary>
    [AFContext(typeof(CoreStrings))]
    [AFField(SourceField = "pri.SYS_FOLDER", SystemFieldFlag = eSystemFieldFlag.Folder)]
    [AFBinding]
    [AFGridColumn]
    public string SYS_FOLDER
    {
        get => _SYS_FOLDER;
        set => Set(ref _SYS_FOLDER, value);
    }
}