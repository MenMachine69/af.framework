namespace AF.DATA;

/// <summary>
/// Attribut, das eine Tabelle in der Datenbank beschreibt
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class AFTable : Attribute
{
    /// <summary>
    /// Constructor
    /// </summary>
    public AFTable() { }

    /// <summary>
    /// Constructor.
    ///
    /// Erzeugt eine Kopie des übergebenen Attributes.
    /// </summary>
    /// <param name="from">Kopie von</param>
    public AFTable(AFTable from)
    {
        TableName = from.TableName;
        Version = from.Version;
        TableId = from.TableId;
        UseCache = from.UseCache;
        Browsable = from.Browsable;
        BrowseNeedAdminRights = from.BrowseNeedAdminRights;
        BrowseNeedRights = from.BrowseNeedRights;
        LogChanges = from.LogChanges;
        AllowSelect = from.AllowSelect;
    }
    
    /// <summary>
    /// Name der Tabelle in der Datenbank.
    /// Wird kein Name angegeben, entspricht der Name der Tabelle dem Namen des Typs mit vorangestelltem TBL_
    /// 
    /// Beispiel: Typ: Firma
    /// Tabellenname: TBL_FIRMA
    /// </summary>
    public string TableName { get; set; } = "";
    
    /// <summary>
    /// Datenbankversion, mit der die Tabelle das letzte Mal angepasst wurde/werden soll.
    /// Wird die Version erhöht, wird die Struktur der Tabelle bei der Datenbankprüfung überprüft.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Interne ID der Tabelle
    /// IDs von 0 - 100 sind für AF3-interne Dinge reserviert. Für eigene Tabellen nur IDs &gt; 100 verwenden!
    /// </summary>
    public int TableId { get; set; }

    /// <summary>
    /// Cache für die Daten verwenden (Daten werden, wenn möglich, zwischengespeichert und aus dem Cache statt 
    /// aus der Datenbank gelesen).
    /// </summary>
    public bool UseCache { get; set; }

    /// <summary>
    /// Änderungen protokollieren. Die Protokollierung muss für jedes einzelne AFFeld eingeschaltet werden und 
    /// ein Handler muss der Datenbank zugewiesen werden. 
    /// </summary>
    public bool LogChanges { get; set; }

    /// <summary>
    /// Gibt an, ob die Tabelle durchsuchbar ist oder nicht (kann im ModelBrowser usw. ausgewählt werden).
    /// </summary>
    public bool Browsable { get; set; } = false;

    /// <summary>
    /// Gibt an, für das 'browsen' (z.B. im ModelBrowser) Admin-Rechte erforderlich sind.
    /// </summary>
    public bool BrowseNeedAdminRights { get; set; } = false;

    /// <summary>
    /// Berechtigung, die für das 'browsen' (z.B. im ModelBrowser) erforderlich ist.
    /// </summary>
    public int BrowseNeedRights { get; set; } = -1;

    /// <summary>
    /// Auswahl erlauben. Ist der Wert auf true gesetzt, kann z.B. in Formularen/Fragebögen eine Auswahl von Werten zur Verfügung gestellt werden.
    /// Die eigentliche Auswahl wird vom UI-Controller zur Verfügung gestellt. In der Regel handelt es sich im eine Combobox die im DropDown die 
    /// Daten zur Verfügung stellt. Als Daten werden die gleichen Daten angeboten, die auch via AFMModelCombobox zur Verfügung gestellt werden. Dies 
    /// können statt der Daten der Tabelle auch die daten eines Views sein.
    /// </summary>
    public bool AllowSelect { get; set; } = false;
}