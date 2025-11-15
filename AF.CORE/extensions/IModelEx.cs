using System.Reflection;
using System.Text.Json;

namespace AF.CORE;

/// <summary>
/// Erweiterungsmethoden für IModel
/// </summary>
public static class IModelEx
{
    /// <summary>
    /// Ein IModel in einen Json-String umwandeln.
    /// </summary>
    /// <param name="model">model</param>
    /// <returns>der Json-String</returns>
    public static string ToJson<T>(this T model) where T : class, IModel, new()
    {
        var options = new JsonSerializerOptions
        {
            IgnoreReadOnlyProperties = true,
            WriteIndented = true,
            IncludeFields = false
        };

        return JsonSerializer.Serialize(model, options);
    }

    /// <summary>
    /// Ein IModel aus einem JsonString befüllen.
    /// </summary>
    /// <param name="model">zu befüllendes Model</param>
    /// <param name="jsonString">String mit Json des Objekts</param>
    /// <param name="ignoreSystemFields">Systemfelder (Key, TimeStamps etc.) ignorieren/nicht übernehmen (optional, default = false)</param>
    public static void FromJson<T>(this T model, string jsonString, bool ignoreSystemFields = false) where T : class, IModel, new()
    {
        var options = new JsonSerializerOptions
        {
            IgnoreReadOnlyProperties = true,
            WriteIndented = true,
            IncludeFields = false,
            UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Skip,
        };

        T? modelLoaded = JsonSerializer.Deserialize<T>(jsonString, options);

        if (modelLoaded == null)
            return;

        TypeDescription tdesc = model.GetType().GetTypeDescription();

        foreach (var field in tdesc.Fields.Values.Where(f => ((PropertyInfo)f).CanWrite && ((PropertyInfo)f).CanRead))
        {
            if (field.Field?.SystemFieldFlag != eSystemFieldFlag.None && ignoreSystemFields)
                continue;

            tdesc.Accessor[model, field.Name] = tdesc.Accessor[modelLoaded, field.Name];
        }
    }

    /// <summary>
    /// Liefert das ModelInfo des Models.
    /// </summary>
    /// <typeparam name="T">Typ des Models</typeparam>
    /// <param name="model">das Model</param>
    /// <returns>ModelInfo zum Model</returns>
    public static ModelInfo GetModelInfo<T>(this T model) where T : class, IModel, new()
    {
        ModelInfo ifo = new() { Id = model.PrimaryKey, ModelType = typeof(T), Caption = model.ToString() ?? "", Description = "" };

        var tdesc = typeof(T).GetTypeDescription();

        if (tdesc.ModelInfoFiels.Count < 1) return ifo;

        var sb = StringBuilderPool.GetStringBuilder();

        foreach (var field in tdesc.ModelInfoFiels)
        {
            object value = tdesc.Accessor[model, field.Key];
            ifo.Data.Add(field.Key, value);
            sb.AppendLine($"{field.Key} : {value}");
        }

        ifo.Description = sb.ToString();
        StringBuilderPool.ReturnStringBuilder(sb);

        try
        {
            typeof(T).GetController().CustomizeModelInfo(ifo);
        }
        catch
        {
            // wenn es keinen Controller gibt, wird die ModelInfo eben nicht angepasst.
        }

        return ifo;
    }

    /// <summary>
    /// Liefert den Inhalt des Models (Felder/Eigenschaften) als Dictionary.
    /// 
    /// Key des Dictionarys ist der Name des Feldes/der Eigenschaft, ergänzt um
    /// ein Präfix und ein Suffix (Standard ist #). Value ist ein Tuple aus Wert und
    /// Format-String für den Wert (darf null oder leer sein).
    /// 
    /// Das Dictionary enthält auch die Werte untergeordneter IModel-Objekte (=Eigenschaft ist ein IModel)
    /// bis zur angegebenen Tiefe. Der Name wird dann aus dem Namen der Eigenschaft, die das IModel enthält
    /// und dem Namen der Eigenschaft im untergeordneten IModel zusammengesetzt (Bsp: FIR_ANSCHRIFT.ANS_STRASSE).
    /// Alternativ wird statt des zusammengesetzten Namens der Aliasname der Eigenschaft im untergeordneten Model
    /// verwendet, wenn ein solcher definiert ist (siehe AFContext) und das Dictionary noch keinen Eintrag mit
    /// diesem Namen enthält.
    /// </summary>
    /// <param name="model">Model</param>
    /// <param name="ignoreSystemAndGuid">Alle Felder mit SystemFieldFlag und alle GUID-Felder ignorieren</param>
    /// <param name="depth">Tiefe der Ermittlung</param>
    /// <param name="praefix">Präfix</param>
    /// <param name="suffix">Suffix</param>
    /// <returns>Array der Werte</returns>
    public static SortedDictionary<string, DatasourceField> AsDictionary(this IModel model, bool ignoreSystemAndGuid = false, int depth = 3, string praefix = "#", string suffix = "#")
    {
        return model.GetType().GetTypeDescription().GetAsDictionary(model, ignoreSystemAndGuid, 1, 2, praefix, suffix);
    }
}