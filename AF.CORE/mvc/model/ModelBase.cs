using System.Data;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace AF.MVC;

/// <summary>
/// Basisklasse für Models, die nicht in einer Datenbank gespeichert werden sollen (z.B. Modul-Models)
/// </summary>
[Serializable]
public abstract class ModelBase : BaseBuffered, IModel
{
    private readonly Guid id = Guid.NewGuid();

    /// <inheritdoc />
    [XmlIgnore]
    [JsonIgnore]
    public virtual Guid PrimaryKey => id;

    /// <inheritdoc />
    [XmlIgnore]
    [JsonIgnore]
    public virtual string? ModelDescription => "";

    /// <inheritdoc />
    [XmlIgnore]
    [JsonIgnore]
    public virtual ModelLink ModelLink => new (PrimaryKey, ToString() ?? "", GetType()) { Model = this };

    /// <inheritdoc />
    [XmlIgnore]
    [JsonIgnore]
    public virtual ModelInfo ModelInfo => new () { Id = PrimaryKey, Caption = ToString() ?? "", Description = "", ModelType = GetType() };

    /// <inheritdoc />
    public virtual BindingList<DatasourceField> GetFieldInformations()
    {
        return [];
    }

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

    /// <inheritdoc />
    public virtual void LoadFrom<TModel>(TModel parent) where TModel : IModel
    {
        if (typeof(TModel) == GetType() || GetType().IsAssignableFrom(typeof(TModel)))
            CopyFrom(parent, true);
        else
            throw new ArgumentException($"Der Typ {typeof(TModel).FullName} muss gleich {GetType().FullName} sein oder von {GetType().FullName} erben.");
    }

    /// <inheritdoc />
    public virtual void LoadFrom(DataRow data)
    {
        CopyFrom(data, true);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj == null) return false;

        if (obj is not IModel model) return false;

        if (model.PrimaryKey.Equals(Guid.Empty)) return false;

        if (obj.GetType() != GetType()) return false;

        return model.PrimaryKey.Equals(PrimaryKey);
    }
    
    /// <inheritdoc />
    public override int GetHashCode()
    {
        return PrimaryKey.Equals(Guid.Empty) ? Guid.NewGuid().GetHashCode() : PrimaryKey.GetHashCode();
    }
}

/// <summary>
/// Basisklasse für IDatabaseObject-Implementierung
/// </summary>
[Serializable]
public abstract class DataObjectBase : ModelBase, IDataObject
{
    /// <inheritdoc />
    [JsonIgnore]
    [XmlIgnore]
    public virtual DateTime CreateDateTime { get; set; } = DateTime.Now;

    /// <inheritdoc />
    [JsonIgnore]
    [XmlIgnore]
    public virtual DateTime UpdateDateTime { get; set; } = DateTime.Now;

    /// <inheritdoc />
    [JsonIgnore]
    [XmlIgnore]
    public virtual bool IsArchived { get; set; } = false;

    /// <inheritdoc />
    [JsonIgnore]
    [XmlIgnore]
    public virtual IDatabase? Database { get; set; } = null;

    /// <inheritdoc />
    public virtual void AfterLoad() { }

    /// <inheritdoc />
    public virtual bool IsDelayedLoaded(string fieldName)
    {
        return true;
    }
}

