using System.Resources;

namespace AF.CORE;

/// <summary>
/// Attribut, das zur Beschreibung von Eigenschaften, anstelle der Standardbeschreibung verwendet wird. 
/// Im Gegensatz zur Standardbeschreibung unterstützt dieses Attribut die Verwendung von Ressourcen
/// für mehrsprachige Anwendungen.
/// 
/// Diese Beschreibung wird verwendet, wenn ein beschreibender Name 
/// und/oder weitere Informationen über die Eigenschaft 
/// erforderlich sind (z. B. Beschriftungen in Rastern oder Tooltips für Editoren).
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class AFDescription : DescriptionAttribute
{
    private readonly ResourceManager? _resourceManager;

    /// <summary>
    /// Beschreibung erzeugen
    /// </summary>
    /// <param name="desc">Beschreibung der Eigenschaft</param>
    public AFDescription(string desc) : base(desc)
    {
    }

    /// <summary>
    /// Beschreibung erzeugen
    /// </summary>
    /// <param name="desc">Name der Zeichenfolge in der Ressource, die die Beschreibung enthält</param>
    /// <param name="resourceType">Typ der Ressource, die die Zeichenfolgen enthält (z. B. typeof(MyApp.Ressources)).</param>
    public AFDescription(string desc, Type resourceType) : base(desc)
    {
        _resourceManager = new ResourceManager(resourceType);
    }

    /// <summary>
    /// Text, der als Beschreibung angezeigt werden soll
    /// </summary>
    public override string Description
    {
        get
        {
            if (_resourceManager == null)
                return base.Description;

            string? description = _resourceManager.GetString(base.Description);

            return string.IsNullOrWhiteSpace(description) ? base.Description : description;
        }
    }
}

