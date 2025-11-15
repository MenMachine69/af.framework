using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace AF.CORE;

/// <summary>
/// Basisklasse zur Erfassung von Änderungen in einem Puffer zur Unterstützung eines RollBack.
/// 
/// Alle bearbeitbaren Models sollten von dieser Klasse abgeleitet werden.
/// </summary>
[Serializable]
public abstract class BaseBuffered : Base
{
    [NonSerialized]
    private Dictionary<string, object?>? _buffer;
    [NonSerialized]
    private bool _rollBack;


    #region IEditableObject
    /// <summary>
    /// <see cref="System.ComponentModel.IEditableObject"/>
    /// </summary>
    public override void EndEdit() { CommitChanges(); }

    /// <summary>
    /// <see cref="System.ComponentModel.IEditableObject"/>
    /// </summary>
    public override void CancelEdit() { RollBackChanges(); }
    #endregion


    #region public methods

    /// <inheritdoc />
    public override bool Set<T>(string name, ref T field, T value)
    {
        return Set(name, ref field, value, RaiseChangeEvents);
    }

    /// <inheritdoc />
    public override bool Set<T>(ref T field, T value, [CallerMemberName] string name = "")
    {
        return Set(name, ref field, value, RaiseChangeEvents);
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
    public override bool SetWithBuffer<T>(ref T field, T value, IDataObject? buffer, [CallerMemberName] string name = "")
    {
        if (typeof(T) == typeof(Guid) && buffer != null && !buffer.PrimaryKey.Equals(value))
            buffer = null;

        return Set(name, ref field, value, RaiseChangeEvents);
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
    public override bool SetWithBuffer<T>(ref T field, T value, IDataObject? buffer, ModelInfo? bufferInfo, [CallerMemberName] string name = "")
    {
        if (typeof(T) == typeof(Guid))
        {
            if (buffer != null && !buffer.PrimaryKey.Equals(value))
                buffer = null;

            if (bufferInfo != null && !bufferInfo.Link.ModelID.Equals(value))
                bufferInfo = null;
        }

        return Set(name, ref field, value, RaiseChangeEvents);
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
    public override bool SetWithBuffer<T>(ref T field, T value, ModelInfo? bufferInfo, [CallerMemberName] string name = "")
    {
        if (typeof(T) == typeof(Guid))
        {
            if (bufferInfo != null && !bufferInfo.Link.ModelID.Equals(value))
                bufferInfo = null;
        }

        return Set(name, ref field, value, RaiseChangeEvents);
    }


    /// <summary>
    /// Ändern eines Wertes, dessen alter Wert nicht gepuffert werden soll.
    /// 
    /// Die Änderung kann nicht mit RollBackChanges rückgängig gemacht werden, 
    /// HasChanged wird durch die Änderung NICHT auf true gesetzt.
    /// </summary>
    /// <typeparam name="T">Type der Eigenschaft</typeparam>.
    /// <param name="name">Name der Eigenschaft (nicht angeben! wird automatisch durch den Namen aus dem Aufruf ersetzt!)</param>.
    /// <param name="field">Instanzvariable, die den Wert hält</param>.
    /// <param name="value">neuer Wert</param>.
    /// <returns>true, wenn der neue Wert gesetzt wurde, sonst false (= neuer Wert entspricht dem bereits vorhandenen)</returns>
    public bool SetUnbuffered<T>(ref T field, T value, [CallerMemberName] string name = "")
    {
        return Set(name, ref field, value, false);
    }

    /// <summary>
    /// Ändern eines Wertes, dessen alter Wert nicht gepuffert werden soll.
    /// 
    /// Die Änderung kann nicht mit RollBackChanges rückgängig gemacht werden, 
    /// HasChanged wird durch die Änderung NICHT auf true gesetzt.
    /// </summary>
    /// <typeparam name="T">Type der Eigenschaft</typeparam>.
    /// <param name="name">Name der Eigenschaft (nicht angeben! wird automatisch durch den Namen aus dem Aufruf ersetzt!)</param>.
    /// <param name="field">Instanzvariable, die den Wert hält</param>.
    /// <param name="value">neuer Wert</param>.
    /// <returns>true, wenn der neue Wert gesetzt wurde, sonst false (= neuer Wert entspricht dem bereits vorhandenen)</returns>
    public bool SetUnbuffered<T>(string name, ref T field, T value)
    {
        return Set(name, ref field, value, false);
    }

    /// <summary>
    /// Einen Wert ändern
    /// </summary>
    /// <typeparam name="T">Type der Eigenschaft</typeparam>.
    /// <param name="name">Name der Eigenschaft (nicht angeben! wird automatisch durch den Namen aus dem Aufruf ersetzt!)</param>.
    /// <param name="current">aktueller Wert</param>.
    /// <param name="value">neuer Wert</param>.
    /// <param name="buffered">Puffer verwenden</param>
    /// <returns>true, wenn der neue Wert gesetzt wurde, sonst false (= neuer Wert entspricht dem bereits vorhandenen)</returns>
    public bool Set<T>(string name, ref T current, T value, bool buffered)
    {
        // das Setzen von NULL für einen String verhindern. Stattdessen wird String.Empty verwendet
        if (typeof(T) == typeof(string) && value == null)
            value = (T)(object)string.Empty;

        if (current == null && value == null)
            return false;

        if (current != null && value != null && current.Equals(value))
            return false;

        if (!_rollBack)
        {
            if (buffered)
            {
                _buffer ??= [];

                if (!_buffer.ContainsKey(name))
                    _buffer.Add(name, current);

                if (!HasChanged)
                {
                    HasChanged = true;
                    Connector?.StateChanged(eBindingStateMessage.Changed, this);
                }
            }
        }

        current = value;
        RaisePropertyChangedEvent(name);

        return true;
    }
    
    /// <summary>
    /// alle aktuellen Änderungen übertragen - HasChanged ist nach dem Aufruf dieser Methode IMMER false.
    /// </summary>
    public override void CommitChanges()
    {
        HasChanged = false;
        if (_buffer != null && _buffer.Count > 0) _buffer.Clear();
        Connector?.StateChanged(eBindingStateMessage.Commit, this);
    }

    /// <summary>
    /// Macht alle Änderungen seit dem letzten CommitChanges rückgängig.
    /// </summary>
    public override void RollBackChanges()
    {
        _rollBack = true;
        if (_buffer != null && _buffer.Count > 0)
        {
            foreach (var pair in _buffer)
                GetType().GetProperty(pair.Key)?.GetSetMethod()?.Invoke(this, [pair.Value]);
            
            _buffer.Clear();
        }

        HasChanged = false;
        _rollBack = false;
        Connector?.StateChanged(eBindingStateMessage.Rollback, this);
    }

    /// <summary>
    /// Liste der aktuell geänderten Eigenschaften
    /// </summary>
    [JsonIgnore]
    [XmlIgnore]
    public override Dictionary<string, ChangedValueInfo> ChangedProperties
    {
        get
        {
            var ret = new Dictionary<string,ChangedValueInfo>();

            if (_buffer == null || _buffer.Count == 0)
                return ret;

            foreach (var var in _buffer) 
                ret.Add(var.Key, new(var.Key, var.Value, Get(var.Key)));

            return ret;
        }
    }

    #endregion
}