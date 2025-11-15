namespace AF.CORE;

/// <summary>
/// Interface für Code-Snippets im Script-Designer.
///
/// In der Regel sind Snippets in einer Datenbank der Anwendung persistiert.
/// </summary>
public interface IScriptSnippet
{
    /// <summary>
    /// Name des Snippets
    /// </summary>
    string Name { get; }

    /// <summary>
    /// ID des Snippets
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Quelltext des Snippets
    /// </summary>
    string Code { get; set; }

    /// <summary>
    /// Beschreibung des Snippets
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gibt an, ob es sich um einen internen,
    /// nicht bearbeitbaren Snippet handelt.
    /// </summary>
    bool System { get; }
}

/// <inheritdoc />
public class DefaultCodeSnippet : IScriptSnippet
{
    /// <inheritdoc />
    public string Name { get; set; } = "";

    /// <inheritdoc />
    public Guid Id => new Guid("{3556DF72-0076-4100-A186-025E0F786152}");

    /// <inheritdoc />
    public string Code { get; set; } = "";

    /// <inheritdoc />
    public string Description { get; set; } = "";

    /// <inheritdoc />
    public bool System { get; set; } = false;
}