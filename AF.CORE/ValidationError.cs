namespace AF.CORE;

/// <summary>
/// Informationen über einen Validierungsfehler
/// </summary>
public sealed class ValidationError
{
    /// <summary>
    /// Name der ungültigen Eigenschaft.
    /// </summary>
    public string PropertyName {get; set;} = "";

    /// <summary>
    /// Beschreibung des Fehlers (was ist ungültig).
    /// </summary>
    public List<string> Messages {get; } = [];

    /// <summary>
    /// Steuerelement, das den Wert enthält/der Editor des Wertes ist.
    /// 
    /// Kann verwendet werden, um die Fehlermeldung am Steuerelement anzuzeigen.
    /// </summary>
    public object? Control {get; set; }

    /// <summary>
    /// Zusätzliche Informationen/Daten für die Nachricht
    /// </summary>
    public object? Tag {get; set;}

    /// <summary>
    /// Gibt die Fehlermeldungen als String zurück.
    /// </summary>
    public string Message => string.Join(Environment.NewLine, Messages);
}

/// <summary>
/// Sammlung von Validierungsfehlern
/// </summary>
public class ValidationErrorCollection : ICollection<ValidationError>
{
    private readonly Dictionary<string, ValidationError> entries = [];

    /// <inheritdoc/>
    public int Count => entries.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public void Add(ValidationError item)
    {
        if (entries.TryGetValue(item.PropertyName, out var entry))
        {
            entry.Messages.AddRange(item.Messages);
            return;
        }

        entries.Add(item.PropertyName, item);
    }

    /// <summary>
    /// Einen Fehler hinzufügen
    /// </summary>
    /// <param name="propertyName">Name der ungültigen Eigenschaft</param>
    /// <param name="message">Meldung zur Gültigkeitsprüfung</param>
    /// <param name="control">Steuerelement, das die ungültigen Daten enthält</param>
    /// <param name="tag">Tag für zusätzliche Informationen</param>
    public void Add(string propertyName, string message, object? control = null, object? tag = null)
    {
        ValidationError error = new()
        {
            PropertyName = propertyName, 
            Control = control, 
            Tag = tag
        };
        error.Messages.Add(message);

        Add(error);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        entries.Clear();
    }
    
    /// <inheritdoc/>
    public bool Contains(ValidationError item)
    {
        return entries.ContainsKey(item.PropertyName);

    }

    /// <inheritdoc/>
    public void CopyTo(ValidationError[] array, int arrayIndex)
    {
        ((ICollection<ValidationError>)this).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<ValidationError> GetEnumerator()
    {
        return entries.Values.GetEnumerator();
    }

    /// <inheritdoc/>
    public bool Remove(ValidationError item)
    {
        return entries.Remove(item.PropertyName);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return entries.Values.GetEnumerator();
    }
}