namespace AF.MVC;

/// <summary>
/// List der Sections in einem Editor
/// </summary>
public class EditorSectionsCollection : ICollection<IEditorSection>
{
    private readonly List<IEditorSection> sections = [];

    /// <inheritdoc/>
    public int Count => sections.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public void Add(IEditorSection item)
    {
        if (!sections.Contains(item))
            sections.Add(item);
    }
    /// <inheritdoc/>
    public void Clear()
    {
        sections.Clear();
    }
    
    /// <inheritdoc/>
    public bool Contains(IEditorSection item)
    {
        return sections.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(IEditorSection[] array, int arrayIndex)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));

        if (array is not { } sectionArray)
            throw new ArgumentException(string.Format(CoreStrings.ERR_WRONGARRAYTYPE, typeof(IEditorSection).FullName), nameof(array));

        ((ICollection<IEditorSection>)this).CopyTo(sectionArray, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<IEditorSection> GetEnumerator()
    {
        return sections.GetEnumerator();
    }

    /// <inheritdoc/>
    public bool Remove(IEditorSection item)
    {
        return sections.Remove(item);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return sections.GetEnumerator();
    }
}