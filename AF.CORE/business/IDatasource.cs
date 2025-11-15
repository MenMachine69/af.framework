using System.Data;

namespace AF.CORE;

/// <summary>
/// Beschreibt eine Klasse, die als Datenquelle für Dokumente u.ä. genutzt werden kann (datensatzbasierter Zugriff).
/// </summary>
public interface IDatasource
{
    /// <summary>
    /// Informationen zu den verfügbaren Feldern in der Entität ermitteln
    /// </summary>
    /// <returns>Liste der verfügbaren Felder</returns>
    BindingList<DatasourceField> GetFieldInformations();

    /// <summary>
    /// Daten von einem übergeordneten oder vorhandenen Model laden.
    /// </summary>
    /// <typeparam name="TModel">Typ des Models</typeparam>
    /// <param name="parent">übergeordnetes/vorhandenes Model</param>
    void LoadFrom<TModel>(TModel parent) where TModel : IModel;


    /// <summary>
    /// Daten von aus einer DataRow laden (Spaltennamen = Namen der Eigenschaften).
    /// </summary>
    /// <param name="data">übergeordnetes/vorhandenes Model</param>
    void LoadFrom(DataRow data);

    /// <summary>
    /// Liefert den Inhalt der Datenquelle (Felder/Eigenschaften) als Dictionary.
    /// 
    /// Key des Dictionaries ist der Name des Feldes/der Eigenschaft, ergänzt um
    /// ein Präfix und ein Suffix (Standard ist #). Value ist ein Tuple aus Wert und
    /// Format-String für den Wert (darf null oder leer sein).
    /// </summary>
    /// <param name="ignoreGuid">Alle Felder mit SystemFieldFlag und alle GUID-Felder ignorieren</param>
    /// <param name="praefix">Präfix</param>
    /// <param name="suffix">Suffix</param>
    /// <returns>Array der Werte</returns>
    SortedDictionary<string, DatasourceField> AsDictionary(bool ignoreGuid = false, string praefix = "#", string suffix = "#");
}

/// <summary>
/// Beschreibung eines Feldes in einer Datenquelle
/// </summary>
[Serializable]
public sealed class DatasourceField
{
    /// <summary>
    /// Name des Feldes
    /// </summary>
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// Anzeigename des Feldes
    /// </summary>
    public string FieldDisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Beschreibung des Feldes
    /// </summary>
    public string FieldDescription { get; set; } = string.Empty;

    /// <summary>
    /// Typ des Feldes
    /// </summary>
    public Type FieldType { get; set; } = typeof(Nullable);

    /// <summary>
    /// Formatangaben für die Anzeige des Wertes
    /// </summary>
    public string FieldDisplayFormat { get; set; } = string.Empty;

    /// <summary>
    /// Name der Entität, die das Feld enthält.
    ///
    /// Das kann z.B. der Name des Models sein oder der Name eines Views etc.
    /// </summary>
    public string EntityName { get; set; } = string.Empty;

    /// <summary>
    /// Anzeigename der Entität, die das Feld enthält.
    ///
    /// Das kann z.B. der Name des Models sein oder der Name eines Views etc.
    /// </summary>
    public string EntityDisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Beschreibung der Entität, die das Feld enthält.
    ///
    /// Das kann z.B. der Name des Models sein oder der Name eines Views etc.
    /// </summary>
    public string EntityDescription { get; set; } = string.Empty;

    /// <summary>
    /// Maske für die Anzeige des Wertes (ToString(...))
    /// </summary>
    public string FieldMask { get; set; } = string.Empty;

    /// <summary>
    /// Typ der Entität
    /// </summary>
    public Type EntityType { get; set; } = typeof(Nullable);
    
    /// <summary>
    /// aktueller Wert für das Feld
    /// </summary>
    public object? CurrentValue { get; set; }
}

