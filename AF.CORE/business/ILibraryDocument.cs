namespace AF.CORE;

/// <summary>
/// Interface für Dokumente aus einer Dokumentenbibliothek
/// </summary>
public interface ILibraryDocument
{
    /// <summary>
    /// Typ des Dokuments
    /// </summary>
    eLibraryDocumentType DocumentType { get; }

    /// <summary>
    /// Daten des Dokuments
    /// </summary>
    byte[] DocumentData { get; }
}

/// <summary>
/// Interface für eine Klasse die in AF.BUSINESS als Controller für LibraryDocument verwendet wird.
/// </summary>
public interface ILibraryDocumentController
{

}

/// <summary>
/// Zentrale Erweiterungsmethoden für ILibraryDocument und ILibraryDocumentController, 
/// die dann im konkreten Implementierungen immer automatisch zur verfügung stehen.
/// </summary>
public static class ILibraryDocumentEx
{

}

/// <summary>
/// Art eines LibraryDokuments
/// </summary>
public enum eLibraryDocumentType
{
    /// <summary>
    /// keinen Typfilter benutzen....
    /// </summary>
    [Description("<unbekannt>")]
    None = 0,
    /// <summary>
    /// PDF-Datei
    /// </summary>
    [Description("PDF-Datei")]
    PDF = 1,
    /// <summary>
    /// Word-Datei
    /// </summary>
    [Description("Word-Datei")]
    Word = 2,
    /// <summary>
    /// Excel-Datei
    /// </summary>
    [Description("Excel-Datei")]
    Excel = 3,
    /// <summary>
    /// Andere Dokumente
    /// </summary>
    [Description("JPEG Bild")]
    Jpeg = 4,
    /// <summary>
    /// Andere Dokumente
    /// </summary>
    [Description("PNG Bild")]
    Png = 5,
    /// <summary>
    /// ZIP-Datei
    /// </summary>
    [Description("ZIP-Datei")]
    Zip = 6,
    /// <summary>
    /// Text-Datei
    /// </summary>
    [Description("Text-Datei")]
    Text = 7,
    /// <summary>
    /// Text-Datei
    /// </summary>
    [Description("XML-Datei")]
    XML = 8,
    /// <summary>
    /// Json-Datei
    /// </summary>
    [Description("Json-Datei")]
    Json = 9,
    /// <summary>
    /// CSS-Datei
    /// </summary>
    [Description("CSS-Datei")]
    CSS = 10,
    /// <summary>
    /// HTML-Datei
    /// </summary>
    [Description("HTML-Datei")]
    HTML = 11,
    /// <summary>
    /// SQL-Datei
    /// </summary>
    [Description("SQL-Datei")]
    SQL = 12,
    /// <summary>
    /// C#-Datei
    /// </summary>
    [Description("C#-Datei")]
    CSharp = 13,
    /// <summary>
    /// Andere Dokumente
    /// </summary>
    [Description("Andere")]
    Other = 14,
}


