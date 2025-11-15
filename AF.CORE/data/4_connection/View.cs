using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace AF.DATA;

/// <summary>
/// Abstrakte Basisklasse für alle Ansichten in einer Datenbank.
///
/// Diese Klasse sollte für eine datenbankspezifische Basisklasse verwendet werden, die die Ansichten
/// in einer bestimmten Datenbank darstellen.
/// </summary>
public abstract class View : Base, IView
{
    #region IModel
    /// <summary>
    /// Primärschlüssel des Objekts.
    /// </summary>
    public virtual Guid PrimaryKey { get; set; } = Guid.Empty;

    /// <summary>
    /// eine Beschreibung des Modells
    ///
    /// die Verwendung von html-Tags ist erlaubt
    /// </summary>
    [JsonIgnore]
    public string? ModelDescription => ToString();

    /// <inheritdoc />
    [JsonIgnore]
    public virtual ModelLink ModelLink => new(PrimaryKey, ToString()!, GetType()) { Model = this };

    /// <inheritdoc />
    [JsonIgnore]
    public virtual ModelInfo ModelInfo => new() { Id = PrimaryKey, Caption = ToString()!, ModelType = GetType() };
    #endregion

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
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return PrimaryKey.Equals(Guid.Empty) ? Guid.NewGuid().GetHashCode() : PrimaryKey.GetHashCode();
    }

    /// <summary>
    /// Typ der übergeordneten Tabelle
    /// </summary>
    public Type MasterType => GetType().GetTypeDescription().View!.MasterType;

    /// <summary>
    /// Gibt ein ModelInfo für ein Objekt zurück, zu dem eine Beziehung anhand der ID besteht und
    /// verwendet einen Puffer, um ein erneutes Laden der ModelInfo bei mehrmaligem Zugriff zu vermeiden 
    /// </summary>
    /// <typeparam name="T">Objekttyp</typeparam>
    /// <param name="primaryKey">ID des Objekts</param>
    /// <param name="buffer">Puffer für das geladene Objekt</param>
    /// <returns>das gefundene Objekt oder NULL</returns>
    public virtual ModelInfo? ReadRelatedInfo<T>(Guid primaryKey, ref ModelInfo? buffer) where T : class, IDataObject, new()
    {
        if (primaryKey.IsEmpty())
            return null;

        if (buffer != null && buffer.Link.ModelID.Equals(primaryKey))
            return buffer;

        buffer = ReadRelatedInfo<T>(primaryKey);

        return buffer;
    }

    /// <summary>
    /// Gibt ein ModelInfo für ein Objekt zurück, zu dem eine Beziehung anhand der ID besteht.
    /// </summary>
    /// <typeparam name="T">Objekttyp</typeparam>
    /// <param name="primaryKey">ID des Objekts</param>
    /// <returns>das gefundene Objekt oder NULL</returns>
    public virtual ModelInfo? ReadRelatedInfo<T>(Guid primaryKey) where T : class, IDataObject, new()
    {
        if (typeof(T).HasInterface(typeof(IModel)) && typeof(T).GetControllerOrNull() != null)
            return typeof(T).GetController().GetModelInfo(primaryKey);

        if (Database == null) throw new ArgumentException(string.Format(CoreStrings.ERR_NODBRELATED, typeof(T).FullName));

        using var conn = Database.GetConnection();
        return conn.LoadInfo<T>(primaryKey);
    }


    #region IDataObject
    private readonly HashSet<string> _lateLoadedFields = [];

    /// <summary>
    /// DateTime angelegt
    /// </summary>
    public virtual DateTime CreateDateTime { get; set; } = DateTime.MinValue;

    /// <summary>
    /// DateTime letzte Änderung
    /// </summary>
    public virtual DateTime UpdateDateTime { get; set; } = DateTime.MinValue;

    /// <summary>
    /// Archiviert
    /// </summary>
    public virtual bool IsArchived { get; set; } = false;


    /// <summary>
    /// Datenbank, aus der das Objekt geladen bzw. in der das Objekt gespeichert wurde.
    /// </summary>
    [XmlIgnore, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [JsonIgnore]
    public IDatabase? Database { get; set; }

    /// <summary>
    /// Methode, die ausgelöst wird, nachdem ein Modell aus der Datenbank geladen wurde
    /// </summary>
    public virtual void AfterLoad() { }

    /// <summary>
    /// Methode, die ausgelöst wird, bevor ein Modell in der Datenbank gespeichert wird
    /// </summary>
    public virtual void BeforeSave() { }


    #region support delayed fields
    /// <summary>
    /// Ermittelt einen Wert, der mit dem Attribut Delayed bereitgestellt wird.
    /// </summary>
    /// <typeparam name="T">Typ des Wertes</typeparam>
    /// <param name="name">Name der Eigenschaft, die den Wert speichert</param>
    /// <param name="value">Variable, in der der Wert gespeichert wird</param>
    /// <returns>der gelesene Wert</returns>
    public T? GetDelayed<T>(ref T? value, [CallerMemberName] string name = "")
    {
        return getDelayed(name, ref value, default);
    }

    /// <summary>
    /// Ermittelt einen Wert, der mit dem Attribut Delayed bereitgestellt wird.
    /// </summary>
    /// <typeparam name="T">Typ des Wertes</typeparam>
    /// <param name="name">Name der Eigenschaft, die den Wert speichert</param>
    /// <param name="value">Variable, in der der Wert gespeichert wird</param>
    /// <param name="nullvalue">Wert, der verwendet werden soll, wenn der gelesene Wert null ist (Standardwert)</param>
    /// <returns>der gelesene Wert</returns>
    public T GetDelayed<T>(ref T? value, T nullvalue, [CallerMemberName] string name = "")
    {
        T? ret = getDelayed(name, ref value, nullvalue);

        return (ret == null ? nullvalue : ret);
    }

    /// <summary>
    /// Prüft, ob die verzögerte Eigenschaft mit dem angegebenen Namen aus der Datenbank geladen ist
    /// </summary>
    /// <param name="fieldName">Name der Eigenschaft/Feld</param>
    /// <returns>true wenn geladen</returns>
    public bool IsDelayedLoaded(string fieldName)
    {
        return _lateLoadedFields.Contains(fieldName);
    }

    /// <summary>
    /// Ermittelt einen Wert, der mit dem Attribut Delayed bereitgestellt wird.
    /// </summary>
    /// <typeparam name="T">Typ des Wertes</typeparam>
    /// <param name="name">Name der Eigenschaft, die den Wert speichert</param>
    /// <param name="value">Variable, in der der Wert gespeichert wird</param>
    /// <param name="nullvalue">Wert, der verwendet werden soll, wenn der gelesene Wert null ist (Standardwert)</param>
    /// <returns>der gelesene Wert</returns>
    private T? getDelayed<T>(string name, ref T? value, T? nullvalue)
    {
        // prüfen ob der Wert schon geladen wurde...
        if (_lateLoadedFields.Contains(name))
            return value;

        if (Database == null)
            throw new(CoreStrings.ERROR_MISSINGDATABASE);

        var fld = GetType().GetTypeDescription().Fields.Values.FirstOrDefault(f => f.Name == name);

        if (fld == null || fld.Field == null)
            throw new(string.Format(CoreStrings.ERR_NOFIELD, name));

        if (fld.Field.Delayed && !PrimaryKey.IsEmpty())
        {
            using var conn = Database.GetConnection();
            value = conn.LoadValue<T>(GetType(), PrimaryKey, name);
        }

        if ((value == null || value is DBNull) && nullvalue != null)
            value = nullvalue;

        _lateLoadedFields.Add(name);

        return value;
    }

    /// <summary>
    /// Legt den Wert eines Feldes fest, das das Attribut "verzögert" hat.
    /// </summary>
    /// <typeparam name="T">Typ des Wertes</typeparam>
    /// <param name="name">Name der Eigenschaft/des Feldes in der Klasse</param>
    /// <param name="storein">Variable, in der der Wert gespeichert werden soll</param>
    /// <param name="value">der zu speichernde Wert</param>
    public void SetDelayed<T>(ref T storein, T value, [CallerMemberName] string name = "")
    {
        Set(ref storein, value);

        _lateLoadedFields.Add(name);
    }

    /// <summary>
    /// Setzt den Status der Felder, die mit dem Attribut Delayed gekennzeichnet sind, auf NICHT GELADEN.
    /// </summary>
    public void ResetDelayed()
    {
        _lateLoadedFields.Clear();
    }

    #endregion

    /// <summary>
    /// Gibt ein Objekt zurück, zu dem eine Beziehung anhand der ID besteht.
    /// </summary>
    /// <typeparam name="T">Objekttyp</typeparam>
    /// <param name="primaryKey">ID des Objekts</param>
    /// <returns>das gefundene Objekt oder NULL</returns>
    public virtual T? ReadRelated<T>(Guid primaryKey) where T : class, IDataObject, new()
    {
        if (typeof(T).HasInterface(typeof(IModel)) && typeof(T).GetControllerOrNull() != null)
            return (T?)typeof(T).GetController().InvokeGeneric($"{nameof(IController.ReadSingle)}", [typeof(T)], primaryKey);

        if (Database == null) throw new ArgumentException(string.Format(CoreStrings.ERR_NODBRELATED, typeof(T).FullName));

        using var conn = Database.GetConnection();
        return conn.Load<T>(primaryKey);
    }

    /// <summary>
    /// Gibt eine Liste von untergeordneten Objekten zurück, die eine Beziehung haben, die durch den Primärschlüssel des übergeordneten Objekts
    /// in einer Zeile mit dem angegebenen Namen 
    /// </summary>
    /// <typeparam name="T">Objekttyp</typeparam>
    /// <param name="primaryKey">ID des Objekts</param>
    /// <param name="childFieldName">Name der Spalte in der untergeordneten Tabelle, die die ID der übergeordneten Tabelle enthält</param>
    /// <param name="includeArchived">Archivierte Childs einbeziehen (default false, optionaler Parameter)</param>
    /// <returns>eine Liste mit Childs oder eine leere Liste</returns>
    public virtual BindingList<T> ReadChilds<T>(Guid primaryKey, string childFieldName, bool includeArchived = false) where T : class, IDataObject, new()
    {
        if (typeof(T).HasInterface(typeof(IModel)) && typeof(T).GetControllerOrNull() != null)
        {
            return (BindingList<T>)typeof(T).GetController().InvokeGeneric(@"Read", [typeof(T)], new ReadOptions { IgnoreArchived = !includeArchived },
                $@"{childFieldName} = ?", primaryKey)!;
        }

        if (Database == null) throw new ArgumentException(string.Format(CoreStrings.ERR_NODBCHILDS, typeof(T).FullName));

        using var conn = Database.GetConnection();
        return conn.Select<T>(new ReadOptions { IgnoreArchived = !includeArchived },
            $@"{childFieldName} = ?", primaryKey);
    }

    /// <summary>
    /// Gibt ein Objekt zurück, zu dem eine Beziehung anhand der ID besteht und
    /// verwendet einen Puffer, um ein erneutes Laden des Objekts bei mehrmaligem Zugriff zu vermeiden 
    /// </summary>
    /// <typeparam name="T">Objekttyp</typeparam>
    /// <param name="primaryKey">ID des Objekts</param>
    /// <param name="buffer">Puffer für das geladene Objekt</param>
    /// <returns>das gefundene Objekt oder NULL</returns>
    public virtual T? ReadRelated<T>(Guid primaryKey, ref T? buffer) where T : class, IDataObject, new()
    {
        if (primaryKey.IsEmpty())
            return default(T);

        if (buffer != null && buffer.PrimaryKey.Equals(primaryKey))
            return buffer;


        buffer = ReadRelated<T>(primaryKey);

        return buffer;
    }
    #endregion

    #region IDatasource
    /// <summary>
    /// Informationen zu den verfügbaren Feldern in der Entität ermitteln
    /// </summary>
    /// <returns>Liste der verfügbaren Felder</returns>
    public virtual BindingList<DatasourceField> GetFieldInformations()
    {
        return GetType().GetTypeDescription().GetFieldInformations();
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
        // kann gleicher View, ein geerbter View oder ein Objekt der übergeordneten Tabelle sein.
        if (this is TModel || typeof(TModel) == MasterType)
            CopyFrom(parent, true);
        else
            throw new ArgumentException($"Der Typ {typeof(TModel).FullName} muss entweder gleich {GetType().FullName} oder {MasterType.FullName} sein, oder von {GetType().FullName} erben.");
    }

    /// <inheritdoc />
    public virtual void LoadFrom(DataRow data)
    {
        CopyFrom(data, true);
    }
    #endregion

    /// <summary>
    /// Kopiert die Daten des 'table'-Objekts in das aktuelle Objekt.
    /// </summary>
    /// <param name="table">Quelle</param>
    /// <param name="keyFields">Schlüsselfelder kopieren (spielt hier keine Rolle, aber in abgeleiteten Klassen)</param>
    public void CopyFromTable(Table table, bool keyFields)
    {
        if (table == null)
            throw new(CoreStrings.ERR_COPY_MISSINGSOURCE);

        TypeDescription desc = GetType().GetTypeDescription();
        TypeDescription descTable = table.GetType().GetTypeDescription();

        foreach (PropertyDescription field in desc.Properties.Values)
        {
            var propInfo = (PropertyInfo)field;

            if (DoNotCopyProperties.Contains(propInfo.Name)) continue;

            if (!propInfo.CanRead || !propInfo.CanWrite || propInfo.SetMethod == null) continue;

            if (field.Field == null || field.Field.SystemFieldFlag == eSystemFieldFlag.None || keyFields)
                if (descTable.Properties.ContainsKey(propInfo.Name))
                    desc.Accessor[this, propInfo.Name] = descTable.Accessor[table, propInfo.Name];
        }
    }
}