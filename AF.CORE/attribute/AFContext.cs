using System.Resources;

namespace AF.CORE;

/// <summary>
/// Attribut zur Beschreibung eines Typs oder einer Eigenschaft
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public sealed class AFContext : Attribute
{
    private string _nameSingular = "";
    private string _namePlural = "";
    private string _description = "";
    private string _talkingName = "";
    private string _hint = "";
    private string? _resname;
    private string? _aliasname = "";
    private readonly ResourceManager? _resourceManager;

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="nameSingular">Name des Typs (Singular)</param>
    /// <param name="namePlural">Name des Typs (Plural)</param>
    /// <param name="description">Beschreibung des Typs (kann z.B. in Tooltips verwendet werden).</param>
    /// <param name="hint">Tipp für diesen Typ (kann z.B. in Tooltips verwendet werden).</param>
    /// <param name="aliasname">Aliasname der Eigenschaft</param>
    public AFContext(string nameSingular, string? namePlural = null, string? description = null, string? hint = null, string? aliasname = null)
    {
        NameSingular = nameSingular;
        NamePlural = namePlural ?? nameSingular;
        Description = description ?? "";
        Hint = hint ?? "";
        AliasName = aliasname;
    }
    
    /// <summary>
    /// Name der zu verwendenden Ressource
    /// </summary>
    public string? RessourceName => _resname;

    /// <summary>
    /// Konstruktor für mehrsprachige Anwendungen mit automatischer Benennung der Ressourcenstrings
    ///
    /// Benennung:
    /// Sprechend: propName_TALKING
    /// Singular: propName_SINGULAR
    /// Plural: propName_PLURAL
    /// Beschreibung: propName_DESC
    /// </summary>
    /// <param name="resourceType">Typ der Ressource, die die Strings enthält (z.B. typeof(MyApp.Resources)).</param>
    /// <param name="resname">Name der Ressource (propName) der anstelle des eigentlichen Namens verwendet werden soll.</param>
    /// <param name="aliasname">Aliasname der Eigenschaft</param>
    public AFContext(Type resourceType, string? resname = null, string? aliasname = null)
    {
        _resourceManager = new ResourceManager(resourceType);
        _resname = resname;
        AliasName = aliasname;
    }
    
    /// <summary>
    /// Zugriff auf den aktuellen RessourceManager für die Strings (falls zugewiesen, sonst null)
    /// </summary>
    public ResourceManager? ResourceManager => _resourceManager;

    /// <summary>
    /// Display-Name.
    /// 
    /// Alternativer Anzeigename für die Eigenschaft, der z. B. in Beschriftungen, Etiketten, Titeln in Tooltips usw. verwendet werden kann.
    /// </summary>
    public string TalkingName
    {
        get
        {
            if (_talkingName.IsEmpty())
                return NameSingular;

            if (_resourceManager == null)
                return _talkingName;

            var name = _resourceManager.GetString(_talkingName);
            return !string.IsNullOrWhiteSpace(name) ? name : _talkingName;
        }
        set => _talkingName = value;
    }

    /// <summary>
    /// Name des Typs (Singular).
    /// 
    /// Dieser Name wird immer dann verwendet, wenn der Name des Typs benötigt wird. Z.B. als 
    /// Kurzbeschreibung des Typs (für Beschriftungen, Labels, Titel in Tooltips, etc.)
    /// </summary>
    public string NameSingular
    {
        get
        {
            if (_resourceManager == null)
                return _nameSingular;

            var name = _resourceManager?.GetString(_nameSingular);
            return name ?? _nameSingular;
        }
        set => _nameSingular = value;
    }

    /// <summary>
    /// Name des Typs (Plural).
    /// 
    /// Dieser Name wird immer dann verwendet, wenn der Name des Typs benötigt wird. Z.B. als 
    /// kurze Beschreibung des Typs (für Beschriftungen, Labels, Titel in Tooltips, etc.)
    /// </summary>
    public string NamePlural
    {
        get
        {
            if (_resourceManager == null)
                return _namePlural;

            var name = _resourceManager.GetString(_namePlural);
            return !string.IsNullOrWhiteSpace(name) ? name : _namePlural;
        }
        set => _namePlural = value;
    }

    /// <summary>
    /// Beschreibung, die immer dann angezeigt wird, wenn der Typ ausgewählt werden kann (z. B. zur Auswahl einer einzelnen Ansicht in einem Master). 
    /// oder wenn zusätzliche Informationen über den Typ benötigt werden.
    /// </summary>
    public string Description
    {
        get
        {
            if (_resourceManager == null)
                return _description;

            var name = _resourceManager.GetString(_description);
            return !string.IsNullOrWhiteSpace(name) ? name : _description;
        }
        set => _description = value;
    }

    /// <summary>
    /// Tipps, die immer dann angezeigt werden, wenn der Typ ausgewählt werden kann (z. B. zur Auswahl einer einzelnen Ansicht in einer Masteransicht).
    /// </summary>
    public string Hint
    {
        get
        {
            if (_resourceManager == null)
                return _hint;

            var name = _resourceManager.GetString(_hint);
            return !string.IsNullOrWhiteSpace(name) ? "" : _hint;
        }
        set => _hint = value;
    }

    /// <summary>
    /// Alias-Name der Eigenschaft (z.B. zur Verwendung als Platzhalter in Dokumenten).
    /// </summary>
    public string? AliasName
    {
        get => _aliasname;
        set => _aliasname = value;
    }
}
