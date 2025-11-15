using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using FastMember;

namespace AF.CORE;

/// <summary>
/// Basisklasse, die INotifyPropertyChanged implementiert.
/// </summary>
[Serializable]
public abstract class Base : INotifyPropertyChanged, IBindable
{
    [NonSerialized] 
    private bool _changed;

    /// <summary>
    /// Gibt true zurück, wenn mindestens eine Eigenschaft, die das PropertyChanged-Ereignis 
    /// (implementiert über SetPropertyValue) unterstützt, geändert wurde.
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    [Browsable(false)]
    public virtual bool HasChanged
    {
        get => _changed;
        set => _changed = value;
    }

    /// <summary>
    /// Liste der aktuell geänderten Eigenschaften
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    public virtual Dictionary<string, ChangedValueInfo> ChangedProperties => [];


    /// <summary>
    /// Connector, an den das IBindable-Objekt gerade gebunden ist.
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    public IBindingConnector? Connector { get; set; }

    /// <summary>
    /// Auslösen de PropertyChanged Ereignisses, wenn eine Eigenschaft geändert wird.
    /// 
    /// Wenn false, werden die Ereignisse unterdrückt. Das kann immer dann genutzt werden, 
    /// wenn die Änderungen nicht relevant sind.
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    public bool RaiseChangeEvents { get; set; } = true;


    #region public events
    /// <summary>
    /// Event: Eigenschaft/Property hat sich geändert
    /// </summary>
    [field: NonSerialized]
    public event PropertyChangedEventHandler? PropertyChanged;
    #endregion

    
    #region public methods
    /// <summary>
    /// Löst das PropertyChanged Ereignis aus.
    /// </summary>
    /// <param name="property">Name des Proertys/der Eigenschaft die geändert wurde</param>
    public virtual void RaisePropertyChangedEvent(string property)
    {
        if (RaiseChangeEvents)
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }


    /// <summary>
    /// Einen Wert ändern.
    /// 
    /// Diese Methode erlaubt KEINE Nullwerte (Null oder DBNull).
    /// 
    /// Wenn null oder DBNull als Wert übergeben wird, wird eine ArgumentNullException ausgelöst.
    /// </summary>
    /// <typeparam name="T">Type der Eigenschaft</typeparam>.
    /// <param name="name">Name der Eigenschaft</param>.
    /// <param name="field">Instanzvariable, die den Wert hält</param>.
    /// <param name="value">neuer Wert</param>.
    /// <returns>true, wenn der neue Wert gesetzt wurde, sonst false (= neuer Wert entspricht dem bereits vorhandenen)</returns>
    public virtual bool SetNotNullable<T>(string name, ref T field, T value)
    {
        if (value == null || value is DBNull) throw new ArgumentNullException(name);

        return Set(name, ref field, value);
    }

    /// <summary>
    /// Einen Wert ändern.
    /// 
    /// Diese Methode erlaubt KEINE Nullwerte (Null oder DBNull).
    /// 
    /// Wenn null oder DBNull als Wert übergeben wird, wird eine ArgumentNullException ausgelöst.
    /// </summary>
    /// <typeparam name="T">Type der Eigenschaft</typeparam>.
    /// <param name="name">Name der Eigenschaft (nicht angeben! wird automatisch durch den Namen aus dem Aufruf ersetzt!)</param>.
    /// <param name="field">Instanzvariable, die den Wert hält</param>.
    /// <param name="value">neuer Wert</param>.
    /// <returns>true, wenn der neue Wert gesetzt wurde, sonst false (= neuer Wert entspricht dem bereits vorhandenen)</returns>
    public virtual bool SetNotNullable<T>(ref T field, T value, [CallerMemberName] string name = "")
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

        return SetNotNullable(name, ref field, value);
    }

    /// <summary>
    /// Einen Wert ändern
    /// </summary>
    /// <typeparam name="T">Type der Eigenschaft</typeparam>.
    /// <param name="name">Name der Eigenschaft (nicht angeben! wird automatisch durch den Namen aus dem Aufruf ersetzt!)</param>.
    /// <param name="field">Instanzvariable, die den Wert hält</param>.
    /// <param name="value">neuer Wert</param>.
    /// <returns>true, wenn der neue Wert gesetzt wurde, sonst false (= neuer Wert entspricht dem bereits vorhandenen)</returns>
    public virtual bool Set<T>(ref T field, T value, [CallerMemberName] string name = "")
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

        return Set(name, ref field, value);
    }

    /// <summary>
    /// Einen Wert ändern
    /// </summary>
    /// <typeparam name="T">Type der Eigenschaft</typeparam>.
    /// <param name="name">Name der Eigenschaft (nicht angeben! wird automatisch durch den Namen aus dem Aufruf ersetzt!)</param>.
    /// <param name="current">aktueller Wert</param>.
    /// <param name="value">neuer Wert</param>.
    /// <returns>true, wenn der neue Wert gesetzt wurde, sonst false (= neuer Wert entspricht dem bereits vorhandenen)</returns>
    public virtual bool Set<T>(string name, ref T current, T value)
    {
        // das Setzen von NULL für einen String verhindern. Stattdessen wird String.Empty verwendet
        if (typeof(T) == typeof(string) && value == null)
            value = (T)(object)string.Empty;

        if (current == null && value == null)
            return false;

        if (current != null && value != null && current.Equals(value))
            return false;

        current = value;


        if (!RaiseChangeEvents) return true;

        _changed = true;
        RaisePropertyChangedEvent(name);

        return true;
    }

    /// <summary>
    /// Setzen des Wertes einer Eigenschaft (wie Set), die auf ein anderes Table-Objekt verweist und ggf. in einem Puffer vorliegt.
    /// </summary>
    /// <typeparam name="T">Typ der Eigenschaft</typeparam>
    /// <param name="field"></param>
    /// <param name="value">Wert</param>
    /// <param name="buffer">Puffer, der ggf. das Objekt enthält</param>
    /// <param name="name">Name der Variablen (muss i.d.R. nicht angegeben werden weil CallerMemberName).</param>
    /// <returns>true, wenn erfolgreich gesetzt</returns>
    public virtual bool SetWithBuffer<T>(ref T field, T value, IDataObject? buffer, [CallerMemberName] string name = "")
    {
        if (typeof(T) == typeof(Guid) && buffer != null && !buffer.PrimaryKey.Equals(value))
            buffer = null;

        return Set(name, ref field, value);
    }

    /// <summary>
    /// Setzen des Wertes einer Eigenschaft (wie Set), die auf ein anderes Table-Objekt verweist und ggf. in einem Puffer vorliegt.
    /// </summary>
    /// <typeparam name="T">Typ der Eigenschaft</typeparam>
    /// <param name="field"></param>
    /// <param name="value">Wert</param>
    /// <param name="buffer">Puffer, der ggf. das Objekt enthält</param>
    /// <param name="bufferInfo">Puffer, der ggf. ein ModelInfo Objekt enthält</param>
    /// <param name="name">Name der Variablen (muss i.d.R. nicht angegeben werden weil CallerMemberName).</param>
    /// <returns>true, wenn erfolgreich gesetzt</returns>
    public virtual bool SetWithBuffer<T>(ref T field, T value, IDataObject? buffer, ModelInfo? bufferInfo, [CallerMemberName] string name = "")
    {
        if (typeof(T) == typeof(Guid))
        {
            if (buffer != null && !buffer.PrimaryKey.Equals(value))
                buffer = null;

            if (bufferInfo != null && !bufferInfo.Link.ModelID.Equals(value))
                bufferInfo = null;
        }

        return Set(name, ref field, value);
    }

    /// <summary>
    /// Setzen des Wertes einer Eigenschaft (wie Set), die auf ein anderes Table-Objekt verweist und für das ggf. ein ModelInfo in einem Puffer vorliegt.
    /// </summary>
    /// <typeparam name="T">Typ der Eigenschaft</typeparam>
    /// <param name="field"></param>
    /// <param name="value">Wert</param>
    /// <param name="bufferInfo">Puffer, der ggf. ein ModelInfo Objekt enthält</param>
    /// <param name="name">Name der Variablen (muss i.d.R. nicht angegeben werden weil CallerMemberName).</param>
    /// <returns>true, wenn erfolgreich gesetzt</returns>
    public virtual bool SetWithBuffer<T>(ref T field, T value, ModelInfo? bufferInfo, [CallerMemberName] string name = "")
    {
        if (typeof(T) == typeof(Guid))
        {
            if (bufferInfo != null && !bufferInfo.Link.ModelID.Equals(value))
                bufferInfo = null;
        }

        return Set(name, ref field, value);
    }


    /// <summary>
    /// Aktueller Wert eines Feldes anhand des Namens ermitteln.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public object? Get(string name)
    {
        TypeDescription? desc = (GetType().HasInterface(typeof(IBindable)) ? GetType().GetTypeDescription() : null);

        if (desc != null)
            return desc.Accessor[this, name];


        // Validierung über Reflection (Attribut AFRule on Properties)
        TypeAccessor accessor = TypeAccessor.Create(GetType());

        return accessor[this, name];
    }


    /// <summary>
    /// alle aktuellen Änderungen übertragen - HasChanged ist nach dem Aufruf dieser Methode IMMER false.
    /// </summary>
    public virtual void CommitChanges()
    {
        _changed = false;
    }

    /// <summary>
    /// alle aktuellen Änderungen rückgängig machen - HasChanged ist nach dem Aufruf dieser Methode IMMER false.
    /// </summary>
    public virtual void RollBackChanges()
    {
        _changed = false;
    }

   
    /// <summary>
    /// Füge das Ergebnis der Überprüfung des Wertes hinzu.
    /// </summary>
    /// <param name="errors">Liste, in der das Ergebnis gespeichert werden soll</param>.
    /// <param name="name">Name der Eigenschaft</param>.
    /// <param name="msg">Fehlermeldung, die angezeigt werden soll</param>.
    /// <param name="control">Steuerelement, das die Meldung anzeigen soll (optional)</param>
    /// <param name="tag">Steuerelement, in dem die Meldung angezeigt werden soll (optional)</param>
    public void AddValidationError(ref ValidationErrorCollection errors, string name, string msg, object? control = null, object? tag = null)
    {
        ValidationError error = new () { Tag = tag, Control = control, PropertyName = name };
        error.Messages.Add(msg);
        errors.Add(error);
    }

    /// <summary>
    /// Füge das Ergebnis der Überprüfung des Wertes hinzu.
    /// </summary>
    /// <param name="errors">Sammlung, in der das Ergebnis gespeichert werden soll</param>.
    /// <param name="error">Fehler, der der Sammlung hinzugefügt werden soll</param>.
    public void AddValidationError(ref ValidationErrorCollection errors, ValidationError error)
    {
        errors.Add(error);
    }

    /// <summary>
    /// Kopiert die Daten des 'source'-Objekts in das aktuelle Objekt.
    /// </summary>
    /// <param name="source">Quelle</param>
    /// <param name="keyFields">Schlüsselfelder kopieren (spielt hier keine Rolle, aber in abgeleiteten Klassen)</param>
    public virtual void CopyFrom(IBindable source, bool keyFields)
    {
        if (source == null)
            throw new(CoreStrings.ERR_COPY_MISSINGSOURCE);
        
        TypeDescription desc = GetType().GetTypeDescription();

        foreach (PropertyDescription field in desc.Properties.Values)
        {
            var propInfo = (PropertyInfo)field;

            if (DoNotCopyProperties.Contains(propInfo.Name)) continue;

            if (!propInfo.CanRead || !propInfo.CanWrite || propInfo.SetMethod == null) continue;
            
            if (field.Field == null || field.Field.SystemFieldFlag == eSystemFieldFlag.None || keyFields)
                desc.Accessor[this, propInfo.Name] = desc.Accessor[source, propInfo.Name];
        }
    }

    /// <summary>
    /// Eigenschaften, die bei CopyFrom niemals kopiert werden sollen.
    /// </summary>
    protected static string[] DoNotCopyProperties =
    [
        nameof(ITable.Connector),
        nameof(ITable.PrimaryKey),
        nameof(ITable.CreateDateTime),
        nameof(ITable.Database),
        nameof(ITable.IsArchived),
        nameof(ITable.UpdateDateTime),
        "SYS_SHARE",
        "SYS_SYNC",
        "SYS_ABO_ID",
        "SYS_ABO_SYNC"
    ];
    /// <summary>
    /// Kopiert die Daten des 'source'-Objekts in das aktuelle Objekt.
    /// </summary>
    /// <param name="source">Quelle</param>
    /// <param name="keyFields">Schlüsselfelder kopieren (spielt hier keine Rolle, aber in abgeleiteten Klassen)</param>
    public virtual void CopyFrom(DataRow source, bool keyFields)
    {
        if (source == null)
            throw new(CoreStrings.ERR_COPY_MISSINGSOURCE);

        TypeDescription desc = GetType().GetTypeDescription();

        foreach (PropertyDescription field in desc.Properties.Values)
        {
            if (!((PropertyInfo)field).CanWrite) continue;

            if (field.Field != null && field.Field.SystemFieldFlag != eSystemFieldFlag.None && !keyFields) continue;

            var value = source[field.Name]?.TranslateTo(((PropertyInfo)field).PropertyType);

            if (value != null)
                desc.Accessor[this, ((PropertyInfo)field).Name] = value;
        }
    }

    /// <summary>
    /// Überprüft das Objekt anhand der zugehörigen Regeln.
    /// 
    /// Die Validierung wird für Typen unterstützt, die entweder IBindable implementieren 
    /// (und somit eine TypeDescription haben) oder deren Eigenschaften ein 
    /// Rules-Attribut zugewiesen ist.
    /// </summary>
    /// <param name="errors">Liste, die die Fehlermeldungen aufnimmt</param>
    /// <returns>true wenn gültig, false sonst</returns>
    public virtual bool IsValid(ValidationErrorCollection errors)
    {
        TypeDescription? desc = (GetType().HasInterface(typeof(IBindable)) ? GetType().GetTypeDescription() : null);

        if (desc != null)
        {
            // anhand der Regeln in der TypeDescription validieren
            foreach (PropertyDescription prop in desc.Properties.Values.Where(p => ((PropertyInfo)p).CanWrite && ((PropertyInfo)p).CanRead && p.Rules != null))
            {
                object val = desc.Accessor[this, ((PropertyInfo)prop).Name];

                prop.Rules?.Validate(val, prop.Name, ref errors);
            }
        }
        else
        {
            // Validierung über Reflection (Attribut AFRule on Properties)
            TypeAccessor accessor = TypeAccessor.Create(GetType());

            foreach (PropertyInfo property in GetType().GetProperties(BindingFlags.FlattenHierarchy |
                                                                      BindingFlags.GetProperty |
                                                                      BindingFlags.SetProperty | BindingFlags.Public |
                                                                      BindingFlags.Instance))
            {
                AFRules? rule = property.GetCustomAttribute<AFRules>(true);

                if (rule == null) continue;

                object val = accessor[this, property.Name];
                rule.Validate(val, property.Name, ref errors);
            }
        }



        return errors.Count < 1;
    }

    /// <summary>
    /// Erweiterte Validierung des Objekts.
    ///
    /// Kann überschrieben werden, um Validierungen außerhalb der AFRules-Regeln der Eigenschaften durchzuführen.
    /// </summary>
    /// <param name="errors"></param>
    public virtual void CustomValidation(ValidationErrorCollection errors) {  }
    #endregion

    #region IEditableObject
    /// <summary>
    /// <see cref="System.ComponentModel.IEditableObject"/>
    /// </summary>
    public virtual void BeginEdit() { }

    /// <summary>
    /// <see cref="System.ComponentModel.IEditableObject"/>
    /// </summary>
    public virtual void EndEdit() { }

    /// <summary>
    /// <see cref="System.ComponentModel.IEditableObject"/>
    /// </summary>
    public virtual void CancelEdit() { }
    #endregion
}