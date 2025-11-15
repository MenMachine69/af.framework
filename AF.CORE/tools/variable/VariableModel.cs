using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace AF.CORE;

/// <summary>
/// Variable, die ein Model darstellt
/// </summary>
[Serializable]
public class VariableModel : VariableBase
{
    /// <summary>
    /// Typ des Models
    /// </summary>
    [JsonIgnore]
    [XmlIgnore]
    [AFBinding]
    [AFContext("Datentyp", Description = "Typ der auswählbaren Daten.")]
    public Type ModelType { get; set; } = typeof(Nullable);

    /// <summary>
    /// Filterbedingungen für die Auswahl
    /// </summary>
    [AFBinding]
    [AFContext("Filterbedingung", Description = "Bedingung für die Auswahl der Datensätze.")]
    public string Filter { get; set; } = string.Empty;

    /// <summary>
    /// Typ des Models als string (für Serialisierung notwendig!)
    /// </summary>
    public string ModelTypeText
    {
        get => ModelType.FullName ?? "";
        set
        {
            if (string.IsNullOrEmpty(value)) 
                ModelType = typeof(Nullable);
            else
            {
                var type = TypeEx.FindType(value);
                if (type != null && type.HasInterface(typeof(IDataObject)))
                    ModelType = type;
                else
                    throw new NotSupportedException($"Typ {value} wird nicht als ModelType in VariableModel unterstützt.");
            }
        }
    }

}