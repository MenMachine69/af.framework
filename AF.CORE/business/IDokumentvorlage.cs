namespace AF.BUSINESS;

/// <summary>
/// Interface für eine Klasse die in AF.BUSINESS als Dokumentvorlage verwendet wird.
/// </summary>
public interface IDokumentvorlage
{
    /// <summary>
    /// In der Vorlage verwendete Variablen.
    /// </summary>
    BindingList<Variable> Variablen { get; set; }

    /// <summary>
    /// Typ des Documents
    /// </summary>
    eDocumentType DocumentType { get; set; }

    /// <summary>
    /// Daten des Dokuments (serialisiert als ByteArray).
    /// </summary>
    byte[] DocumentData { get; set; }

    /// <summary>
    /// Beispielobjekt für Designer u.ä.
    /// </summary>
    object? DocumentSample { get; }

    /// <summary>
    /// DocumentData als IDocumentvorlageText.
    /// </summary>
    IDokumentvorlageText TemplateText { get; set; }

    /// <summary>
    /// DocumentData als IDokumentvorlageHtml.
    /// </summary>
    IDokumentvorlageHtml TemplateHtml { get; set; }
}

/// <summary>
/// Interface für eine Klasse die in AF.BUSINESS als Controller für Dokumentvorlage verwendet wird.
/// </summary>
public interface IDokumentvorlageController
{

}

/// <summary>
/// Zentrale Erweiterungsmethoden für IDokumentvorlage und IDokumentvorlageController, 
/// die dann im konkreten Implementierungen immer automatisch zur verfügung stehen.
/// </summary>
public static class IDokumentvorlageEx
{

}

/// <summary>
/// Art eines Dokuments
/// </summary>
public enum eDocumentType
{
    /// <summary>
    /// keinen Typfilter benutzen....
    /// </summary>
    [Description("<unbekannt>")]
    None = 0,
    /// <summary>
    /// ein Report
    /// </summary>
    [Description("Report")]
    Report = 1,
    /// <summary>
    /// Vorlage für ein Email
    /// </summary>
    [Description("Email")]
    Email = 2,
    /// <summary>
    /// Textdokument (Richtext)
    /// </summary>
    [Description("Textdokument (RichText)")]
    RichText = 3,
    /// <summary>
    /// Textdokument, das als Overlay verwendet wird
    /// </summary>
    [Description("Overlay (RichText)")]
    RichTextOverlay = 4,
    /// <summary>
    /// NurText-Dokument (z.B. für Datenexport als CSV)
    /// </summary>
    [Description("Export")]
    TextOnly = 5,
    /// <summary>
    /// HTML-Template für Onlineportale
    /// </summary>
    [Description("HTML Vorlage")]
    HTMLTemplate = 7
}


/// <summary>
/// Inhal einer Dokumentvorlage des Formats 'Text'
/// 
/// Der Inhalt wird serialisiert im Feld DOV_DATA gespeiechert
/// </summary>
public interface IDokumentvorlageText
{
    /// <summary>
    /// Kopfbereich (einmal zu Beginn der Ausgabe)
    /// </summary>
    public string Header { get; set; }
    /// <summary>
    /// Fuübereich (einmal am Ende der Ausgabe)
    /// </summary>
    public string Footer { get; set; }
    /// <summary>
    /// Inhalt/Detail (einmal pro Datensatz in der Quelle)
    /// </summary>
    public string Detail { get; set; }
}

/// <summary>
/// Inhal einer Dokumentvorlage des Formats 'Text'
/// 
/// Der Inhalt wird serialisiert im Feld DOV_DATA gespeiechert
/// </summary>
public interface IDokumentvorlageHtml
{
    /// <summary>
    /// Quelltext für den Hauptbereich
    /// </summary>
    public string Master { get; set; }
    /// <summary>
    /// FQuelltext für das Detail (einmal je Detail)
    /// </summary>
    public string Detail { get; set; }
}
