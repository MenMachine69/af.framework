using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace AF.CORE;

/// <summary>
/// Basisklasse einer Variablen
/// </summary>
[JsonDerivedType(typeof(VariableBool))]
[JsonDerivedType(typeof(VariableDateTime))]
[JsonDerivedType(typeof(VariableDecimal))]
[JsonDerivedType(typeof(VariableFormula))]
[JsonDerivedType(typeof(VariableString))]
[JsonDerivedType(typeof(VariableMemo))]
[JsonDerivedType(typeof(VariableRichText))]
[JsonDerivedType(typeof(VariableModel))]
[JsonDerivedType(typeof(VariableScript))]
[JsonDerivedType(typeof(VariableList))]
[JsonDerivedType(typeof(VariableGuid))]
[JsonDerivedType(typeof(VariableInt))]
[JsonDerivedType(typeof(VariableQuery))]
[JsonDerivedType(typeof(VariableTimeSpan))]
[JsonDerivedType(typeof(VariableMonth))]
[JsonDerivedType(typeof(VariableYear))]
[JsonDerivedType(typeof(VariableSection))]
[JsonDerivedType(typeof(VariableCustomBase))]
[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
[Serializable]
public class VariableBase : DataObjectBase
{
    /// <summary>
    /// Variable, zu der die Daten gehören
    /// </summary>
    [JsonIgnore]
    [XmlIgnore]
    public IVariable? Parent { get; set; } = null;

    /// <summary>
    /// prüft, ob der aktuelle Wert für die Variable gültig ist
    /// </summary>
    /// <param name="errors">Liste, die die auftretenden Fehler aufnimmt</param>
    /// <param name="name">Name der Variablen (für Fehlermeldungen etc.)</param>
    /// <param name="value">aktueller/zu prüfender Wert</param>
    /// <returns>true wenn gültig, sonst false</returns>
    public virtual bool ValidateValue(ValidationErrorCollection errors, string name, object? value)
    {
        
        return true;
    }
}

/// <summary>
/// Basistyp der Custom-Variablen.
/// </summary>
[Serializable]
public class VariableCustomBase : VariableBase
{
    /// <summary>
    /// Serialisieren
    /// </summary>
    /// <returns></returns>
    public virtual byte[] Serialize()
    {
        return this.ToJsonBytes();
    }

    /// <summary>
    /// Deserialisieren
    /// </summary>
    /// <param name="data">Daten</param>
    /// <returns>deserialisiertes Objekt</returns>
    public static object? Deserialze(byte[]? data)
    {
        return null;
    }
}