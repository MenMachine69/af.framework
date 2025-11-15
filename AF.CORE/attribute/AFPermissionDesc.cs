using System.Resources;

/// <summary>
/// Attribut, das zur Beschreibung einer Erlaubnis verwendet wird. 
/// 
/// Diese Beschreibung wird für Aufzählungen verwendet, die Berechtigungen enthalten.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class AFPermissionDesc : DescriptionAttribute
{
    private string? _name, _section;
    private readonly ResourceManager? _resourceManager;

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="desc">Beschreibung der Erlaubnis</param>
    /// <param name="section">Berechtigungsabschnitt/-gruppe</param>
    /// <param name="name">Erlaubnisname</param>
    public AFPermissionDesc(string section, string name, string desc) : base(desc)
    { _name = name; _section = section; }

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="desc">Beschreibung der Erlaubnis</param>
    /// <param name="section">Berechtigungsabschnitt/-gruppe</param>
    /// <param name="name">Erlaubnisname</param>
    /// <param name="resourceType">Typ der Ressource (resx in der der Wert definiert ist)</param>
    public AFPermissionDesc(string section, string name, string desc, Type resourceType) : base(desc)
    {
        _name = name; _section = section;
        _resourceManager = new ResourceManager(resourceType);
    }

    /// <summary>
    /// Berechtigungsgruppe
    /// </summary>
    public virtual string Section
    {
        get
        {
            if (_section == null)
                return "";

            if (_resourceManager == null)
                return _section;

            string? secname = _resourceManager.GetString(_section);
            return string.IsNullOrWhiteSpace(secname) ? "" : secname;
        }
        set => _section = value;
    }

    /// <summary>
    /// Berechtigung Name
    /// </summary>
    public virtual string Name
    {
        get
        {
            if (_name == null)
                return "";

            if (_resourceManager == null)
                return _name;

            string? pname = _resourceManager.GetString(_name);
            return string.IsNullOrWhiteSpace(pname) ? "" : pname;
        }
        set => _name = value;
    }

    /// <summary>
    /// Beschreibung der Berechtigung
    /// </summary>
    public override string Description
    {
        get
        {
            if (_resourceManager == null)
                return base.Description;

            string? description = _resourceManager.GetString(base.Description);
            return string.IsNullOrWhiteSpace(description) ? "" : description;
        }
    }
}