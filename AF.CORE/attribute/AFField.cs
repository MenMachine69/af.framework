using DevExpress.Utils;

namespace AF.DATA;

/// <summary>
/// Attribut, das ein Feld in der Datenbank beschreibt
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class AFField : Attribute
{
    /// <summary>
    /// Konstruktor mit SourceField-Parameter für einfachere FieldDefinitions in View-Klassen
    /// </summary>
    /// <param name="sourceField">Quellfeld-Definiton (z.B. "pri.USR_NAME")</param>
    /// <param name="parent">Typ der Tabelle, die das Feld enthält</param>
    public AFField(string sourceField, Type parent)
    {
        SourceField = sourceField;
        Parent = parent;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public AFField() { }

    /// <summary>
    /// Typ, auf den sich das Feld bezieht.
    /// 
    /// Dieser Typ muss angegeben werden, wenn Constraints verwendet werden.
    /// </summary>
    public Type? ConstraintType { get; set; }

    /// <summary>
    /// ForeignKey-Beschränkung für Aktualisierungsvorgänge.
    /// Standard ist NoAction.
    /// 
    /// Nur sinnvoll in Verbindung mit "ConstraintType".
    /// </summary>
    public eConstraintOperation ConstraintUpdate { get; set; } = eConstraintOperation.NoAction;

    /// <summary>
    /// ForeignKey-Beschränkung für Löschvorgänge
    /// Standard ist NoAction.
    /// 
    /// Nur sinnvoll in Verbindung mit "ConstraintType".
    /// </summary>
    public eConstraintOperation ConstraintDelete { get; set; } = eConstraintOperation.NoAction;

    /// <summary>
    /// Setzen Sie ein Kennzeichen für dieses Feld, das angibt, ob das Feld ein systemrelevantes Feld ist.
    /// Das Standardkennzeichen ist eSystemFieldFlag.None. Jedes Kennzeichen kann nur für ein Feld gesetzt werden.
    /// </summary>
    public eSystemFieldFlag SystemFieldFlag { get; set; } = eSystemFieldFlag.None;

    /// <summary>
    /// Maximale Länge dieses Feldes in der Datenbank (wenn das Feld eine Zeichenkette ist).
    /// 
    /// Der Standardwert ist 100. Verwenden Sie -1, um ein String-Blob-Feld zu definieren.
    /// 
    /// Handelt es sich bei dem Feld nicht um eine Zeichenkette, wird dieser Wert ignoriert.
    /// </summary>
    public int MaxLength { get; set; } = 100;

    /// <summary>
    /// Größe eines Blocks in Blobfeldern (nur relevant, wenn die Datenbank eine solche Option unterstützt).
    /// Die Standardgröße ist 512. Verwenden Sie eine größere Blockgröße, wenn der größte Teil der Blobdaten größer ist.
    /// </summary>
    public int BlobBlockSize { get; set; } = 512;

    /// <summary>
    /// Automatisch Datenkompression für das Feld verwenden.
    /// Für die Komprimierung wird ZIP verwendet.
    /// 
    /// Kann nur mit Blob-, Bild- und Binärfeldern (ByteArray) verwendet werden
    /// </summary>
    public bool Compress { get; set; }

    /// <summary>
    /// Feld als durchsuchbar kennzeichnen (Spalte wird bei der Suche via SearchEngine berücksichtigt)
    /// </summary>
    public bool Searchable { get; set; }

    /// <summary>
    /// Feld wird nicht in einem RecordState geführt (z.B. für Workflows).
    /// 
    /// Für String mit Länge -1 (unlimited), Image, byte[] und sonstige Objekte ist der Werte IMMER true!
    /// </summary>
    public bool NoState { get; set; }


    /// <summary>
    /// Feld mit SoundEx durchsuchbar machen (Fuzzy-Suche in Strings)
    /// </summary>
    public bool UseSoundExSearch { get; set; }

    /// <summary>
    /// Markiert ein Feld als mit einer Verzögerung geladen (beim ersten Zugriff auf den Inhalt).
    /// 
    /// Verwenden Sie diese Option für Blob-, Bild- oder Binärfelder, die nicht sofort benötigt werden, 
    /// sondern z. B. in einem Viewer mit einem Klick des Benutzers geöffnet/angezeigt werden.
    /// </summary>
    public bool Delayed { get; set; }

    /// <summary>
    /// Gibt an, dass ein Index für das Feld erstellt werden soll. In der Indexdefinition, falls erforderlich. 
    /// kann der Index weiter spezifiziert werden
    /// </summary>
    public bool Indexed { get; set; }

    /// <summary>
    /// Definition des Indexes (leer = nur das aktuelle Feld)
    /// </summary>
    public string? IndexDefinition { get; set; }

    /// <summary>
    /// Markiert ein Feld, das nur eindeutige Werte enthalten kann. 
    /// Jeder Versuch, einen nicht eindeutigen Wert in diesem Feld zu speichern, führt zu einem Fehler.
    /// 
    /// Kann nur in Kombination mit Indexed = true verwendet werden. Wenn kein Index erstellt wird, wird die Option ignoriert!
    /// </summary>
    public bool Unique { get; set; }

    /// <summary>
    /// Name des Feldes in der View-Abfrage (z.B. "pri.USR_NAME"). Immer mit Alias angeben!
    /// </summary>
    public string SourceField { get; set; } = string.Empty;

    /// <summary>
    /// Änderungen protokollieren. Nur relevant, wenn die Änderungsprotokollierung aktiv ist 
    /// und die Protokollierung in der Tabelle eingeschaltet ist.    
    /// </summary>
    public bool LogChanges { get; set; }

    /// <summary>
    /// Kennzeichnet ein Feld, das beim Speichern der Daten IMMER mit gespeichert werden soll. 
    /// Ist standardmässig true, wenn das Feld IList/IBindingList implementiert.
    /// </summary>
    public DefaultBoolean? SaveAllways { get; set; } = DefaultBoolean.Default;

    /// <summary>
    /// Typ/Klasse, die das Feld enthält, auf das hier im View verwiesen wird.
    /// Diese Eigenschaft MUSS bei Feldern in Views gesetzt werden und darf
    /// NICHT für Felder in Tabellen verwendet werden.
    /// </summary>
    public Type? Parent { get; set; }

    /// <summary>
    /// Gibt an, dass das Feld Informationen enthält, die für ModelInfo.Caption benötigt werden.
    /// </summary>
    public bool ModelInfoCaption { get; set; }

    /// <summary>
    /// Gibt an, dass das Feld Informationen enthält, die für ModelInfo.Data benötigt werden.
    /// </summary>
    public bool ModelInfoData { get; set; }

}