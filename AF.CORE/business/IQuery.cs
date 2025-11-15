using System.Data;

namespace AF.CORE;

/// <summary>
/// Schnittstelle für eine Abfrage, deren Ergebnis eine DataTable ist
/// </summary>
public interface IQuery : IDatasource
{
    /// <summary>
    /// Schema im Designer verwenden (WYSIWYG).
    ///
    /// Ist der Wert FALSE, wird direkt der Quellcode bearbeitet.
    /// </summary>
    bool UseScheme { get; set; }

    /// <summary>
    /// SQL Quelltext der Abfrage.
    /// </summary>
    string Query { get; set; }

    /// <summary>
    /// Serialisiertes Schema das im Designer angezeigt wird.
    /// </summary>
    QueryScheme? QuerySchemeModel { get; set; }

    /// <summary>
    /// Im Query verwendete Variablen.
    /// </summary>
    BindingList<Variable> Variablen { get; set; }

    /// <summary>
    /// Datentabelle laden
    /// </summary>
    /// <param name="variableValues">Liste der Werte für die verwendeten Variablen</param>
    /// <returns>Datentabelle</returns>
    DataTable GetData(IList<VariableUserValue> variableValues); 

    /// <summary>
    /// Name des Querys
    /// </summary>
    string QueryName { get; }

    /// <summary>
    /// Beschreibung des Querys
    /// </summary>
    string QueryDescription { get; }

    /// <summary>
    /// Name des Query-Typs
    /// </summary>
    string QueryTypeName { get; }

}

/// <summary>
/// Einfache Implementierung von IQuery
/// </summary>
[Serializable]
public class DefaultQuery : IQuery
{
    /// <inheritdoc />
    public bool UseScheme { get; set; } = true;

    /// <inheritdoc />
    public string Query { get; set; } = string.Empty;

    /// <inheritdoc />
    public QueryScheme? QuerySchemeModel { get; set; } = new();

    /// <inheritdoc />
    public BindingList<Variable> Variablen { get; set; } = [];


    /// <inheritdoc />
    public DataTable GetData(IList<VariableUserValue> variableValues)
    {
        return new();
    }

    /// <inheritdoc />
    public BindingList<DatasourceField> GetFieldInformations()
    {
        return [];
    }

    /// <inheritdoc />
    public virtual void LoadFrom<TModel>(TModel parent) where TModel : IModel { }

    /// <inheritdoc />
    public virtual void LoadFrom(DataRow data) { }


    /// <inheritdoc />
    public virtual SortedDictionary<string, DatasourceField> AsDictionary(bool ignoreGuid = false, string praefix = "#", string suffix = "#")
    {
        SortedDictionary<string, DatasourceField> ret = [];

        var fields = GetFieldInformations();

        foreach (var field in fields)
        {
            if (field.FieldType == typeof(Guid) && ignoreGuid) continue;
            
            ret.Add($"{praefix}{field.FieldName}{suffix}", field);
        }

        return ret;
    }

    /// <summary>
    /// Name des Querys
    /// </summary>
    public virtual string QueryName => "";

    /// <summary>
    /// Beschreibung des Querys
    /// </summary>
    public virtual string QueryDescription => "";

    /// <summary>
    /// Name des Query-Typs
    /// </summary>
    public virtual string QueryTypeName => "";
}