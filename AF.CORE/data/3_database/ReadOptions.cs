namespace AF.DATA;

/// <summary>
/// Optionen zum Lesen und Schreiben von Daten
/// </summary>
public sealed class ReadOptions
{
    /// <summary>
    /// archivierte Daten ignorieren (wenn möglich)
    /// </summary>
    public bool IgnoreArchived { get; set; }
    
    /// <summary>
    /// Daten ordnen nach (SQL: ORDER BY) 
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    /// Daten gruppieren nach (SQL: GROUP BY)
    /// </summary>
    public string? GroupOn { get; set; }

    /// <summary>
    /// Zu lesende Felder (leer = alle).
    /// 
    /// Wenn diese Option leer ist, werden alle Felder aus der Quelle gelesen.
    /// 
    /// Die Option kann verwendet werden, um die zu lesenden Felder auf diejenigen zu beschränken, die
    /// die in einem bestimmten Kontext benötigt werden, was einen erheblichen Einfluss 
    /// auf die Ladezeit haben kann.
    /// </summary>
    public string[] Fields { get; set; } = [];

    /// <summary>
    /// Maximale Anzahl von zu lesenden Datenobjekten
    /// </summary>
    public int MaximumRecordCount { get; set; }

    /// <summary>
    /// Bestellmodus (aufsteigend oder absteigend)
    /// </summary>
    public eOrderMode OrderMode { get; set; }

    /// <summary>
    /// Als verzögert markierte Felder sofort lesen (true)
    /// </summary>
    public bool LoadDelayed { get; set; }

    /// <summary>
    /// Eine Filterfunktion, die für jedes geladene Datenobjekt aufgerufen wird.
    /// 
    /// Wenn diese Funktion für das angegebene Datenobjekt den Wert "false" zurückgibt, /// wird dieses Datenobjekt nicht zum Ergebnis hinzugefügt. 
    /// wird es nicht zum Ergebnis hinzugefügt.
    ///
    /// Diese Filterfunktion wird nur mit der Select-Methode in IConnection verwendet.
    /// </summary>
    public Func<object, bool>? Filter { get; set; }

    /// <summary>
    /// ID der zu gepufferten Abfrage (falls vorhanden)
    ///
    /// NULL, wenn keine gepufferte Abfrage verwendet werden soll
    /// </summary>
    public Guid? BufferedQueryId {get; set; }

    /// <summary>
    /// puffert die Abfrage 
    /// </summary>
    public bool UseQueryBuffer { get; set; }

    /// <summary>
    /// Überspringen der Abfrageübersetzung (AFFunctions etc.)
    /// 
    /// Verwenden Sie dies nur, wenn Sie sicher sind, dass keine Übersetzung erforderlich ist!
    /// </summary>
    public bool SkipTranslator { get; set; }

    /// <summary>
    /// Wenn Daten zur Anzeige in einem Raster verwendet werden, kann hier ein Standardlayout für dieses Raster definiert werden, 
    /// um nur Felder zu lesen, die in diesem Gitter verwendet werden sollen.
    /// 
    /// Dies wird nur innerhalb eines Controllers verwendet. Wenn Sie die Daten nicht über den 
    /// Controller gelesen werden, wird dieser Wert ignoriert.
    /// </summary>
    public eGridStyle GridStyle { get; set; }

    /// <summary>
    /// Typ des Masters, wenn alle einem Master zugeordneten Daten gelesen werden sollen (MasterID muss die ID/PK des Masters enthalten)
    /// </summary>
    public Type? MasterType { get; set; }


    /// <summary>
    /// ID des Masters, für den die Daten gelesen werden sollen (= dem Master zugeordnete Daten)
    /// </summary>
    public Guid? MasterID { get; set; }

    /// <summary>
    /// ID des Datensatzes, der IMMER im Ergebnis enthalten sein muss (beim Lesen von Listen)
    /// </summary>
    public Guid? AllwaysInclude { get; set; }
}