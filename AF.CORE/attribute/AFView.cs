namespace AF.DATA;

/// <summary>
/// Attribut, das einen View in der Datenbank beschreibt.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class AFView : Attribute
{
    /// <summary>
    /// Constructor
    /// </summary>
    public AFView() { }


    /// <summary>
    /// Constructor.
    ///
    /// Erzeugt eine Kopie des übergebenen Attributes.
    /// </summary>
    /// <param name="from">Kopie von</param>
    public AFView(AFView from)
    {
        ViewName = from.ViewName;
        Version = from.Version;
        ViewId = from.ViewId;
        UseCache = from.UseCache;
        Browsable = from.Browsable;
        Query = from.Query;
        MasterType = from.MasterType;
    }

    /// <summary>
    /// Name des Views in der Datenbank.
    /// Wird kein Name angegeben, entspricht der Name der Sicht dem Namen des Typs mit vorangestelltem VW_.
    /// 
    /// Beispiel: Typ: FirmaAktivitaet
    /// Name des Views: VW_FIRMAAKTIVITAET
    /// </summary>
    public string ViewName { get; set; } = "";

    /// <summary>
    /// Abfrage für den View (Verwendung nicht empfohlen, stattdessen die Methode RequestViewQuery verwenden).
    /// </summary>
    public string Query { get; set; } = "";

    /// <summary>
    /// Gibt an, ob der View durchsuchbar ist oder nicht (kann im ModelBrowser usw. ausgewählt werden).
    /// </summary>
    public bool Browsable { get; set; } = false;

    /// <summary>
    /// Datenbankversion, mit der der View das letzte Mal angepasst wurde/werden soll.
    /// Wird die Version erhöht, wird bei der Datenbankprüfung die Struktur des Views überprüft.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Interne ID des Views
    /// 
    /// Die IDs von 0 - 100 sind für AF3-interne Dinge reserviert. Für eigene Tabellen nur IDs &gt; 100 verwenden!
    /// </summary>
    public int ViewId { get; set; }

    /// <summary>
    /// Cache für die Daten verwenden (Daten werden, wenn möglich, zwischengespeichert und aus dem Cache statt aus der Datenbank gelesen).
    /// </summary>
    public bool UseCache { get; set; }


    /// <summary>
    /// Typ des Master/Tabelle für diese Ansicht.
    /// 
    /// Master muss der primäre Typ der primären Tabelle für diese Ansicht sein (select ... from 'master' ...). 
    /// Dieser Typ wird benötigt, um den richtigen IController für diese Ansicht zu erkennen.
    /// </summary>
    public Type MasterType { get; set; } = typeof(Nullable);
}