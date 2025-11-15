using System.Globalization;
using System.Reflection;

namespace AF.CORE;

/// <summary>
/// Versionsinformationen
/// 
/// Diese Klasse enthält Informationen über eine Version und Methoden zum Vergleich von Versionsinformationen.
/// 
/// Die Klasse unterstützt INotifyPropertyChanged für alle öffentlichen Eigenschaften und die Serialisierung der Daten.
/// </summary>
[Serializable]
public class VersionInformation
{
    #region public properties

    /// <summary>
    /// Build der Version (3. Stelle der Versionsnummer)
    /// 
    /// Dieser Wert ist immer >= 0!
    /// </summary>
    public int Build { get; set; }

    /// <summary>
    /// Eindeutige ID dieser Version (GUID)
    /// </summary>
    public Guid Identifier { get; set; }

    /// <summary>
    /// Major der Version (1. Stelle der Versionsnummer)
    /// 
    /// Dieser Wert ist immer >= 0!
    /// </summary>
    public int Major { get; set; }

    /// <summary>
    /// Minor der Version (2. Stelle der Versionsnummer)
    /// 
    /// Dieser Wert ist immer >= 0!
    /// </summary>
    public int Minor { get; set; }
    #endregion

    #region overwritten methods
    /// <summary>
    /// Version als Text (Schema 0.0.0)
    /// </summary>
    /// <returns>Textuelle Darstellung der Version</returns>
    public override string ToString()
    {
        return $@"{Major}.{Minor}.{Build}";
    }
    #endregion

    #region Constructor/Destructor
    /// <summary>
    /// Konstruktor, der die Version aus den drei übergebenen numerischen Werten erstellt. 
    /// </summary>
    /// <param name="major">Major der Version</param>.
    /// <param name="minor">Minor der Version</param>.
    /// <param name="build">Build der Version</param>.
    public VersionInformation(int major, int minor, int build)
    {
        Major = major;
        Minor = minor;
        Build = build;
        Identifier = Guid.Empty;
    }

    /// <summary>
    /// Konstruktor, der die Version aus den Versionsinformationen einer Baugruppe erstellt. 
    /// </summary>
    /// <param name="assembly">Assembly, deren Versionsinformationen gelesen werden sollen.</param>
    public VersionInformation(Assembly assembly)
    {
        ReadFromAssembly(assembly);
    }

    /// <summary>
    /// Konstruktor, der die Version aus der übergebenen Zeichenkette erstellt. 
    /// 
    /// Diese Zeichenkette MUSS wie folgt aufgebaut sein:
    /// MAJOR.MINOR.BUILD
    /// Bsp: 1.0.17
    /// </summary>
    /// <param name="version">Versionsangabe als Text in der Form 0.0.0</param>
    public VersionInformation(string version)
    {
        if (version.IsEmpty())
            throw new ArgumentNullException(nameof(version), CoreStrings.VALUECANNOTBENULL);

        if (version.IsEmpty()) return;

        string[] sub = version.Split(['.'], StringSplitOptions.RemoveEmptyEntries);
        
        if (sub.Length >= 3)
        {
            Major = int.Parse(sub[0].Trim(), CultureInfo.CurrentCulture);
            Minor = int.Parse(sub[1].Trim(), CultureInfo.CurrentCulture);
            Build = int.Parse(sub[2].Trim(), CultureInfo.CurrentCulture);
        }
        Identifier = Guid.Empty;
    }

    /// <summary>
    /// Konstruktor ohne Parameter, der die Version 0.0.0 initialisiert.
    /// </summary>
    public VersionInformation()
    {
        Major = 0;
        Minor = 0;
        Build = 0;
        Identifier = Guid.Empty;
    }
    #endregion

    #region public methods

    /// <summary>
    /// Liest die Versionsinformationen einer Assembly aus. 
    /// </summary>
    /// <param name="assembly">Assembly, deren Versionsinformationen ausgelesen werden sollen</param>.
    /// <returns>VersionInformation Objekt</returns>
    public static VersionInformation ReadFromAssembly(Assembly assembly)
    {
        VersionInformation ret = new();

        Version? ver = assembly.GetName().Version;
        if (ver != null)
        {
            ret.Major = ver.Major;
            ret.Minor = ver.Minor;
            ret.Build = ver.Build;
        }

        ret.Identifier = Guid.Empty;

        return ret;
    }

    /// <summary>
    /// Vergleicht die aktuelle Version mit einer anderen zu übergebenden und gibt zurück:
    /// 
    /// eComparisonResult.Equal, wenn beide identisch sind
    /// eComparisonResult.Less, wenn die übergebene Version kleiner ist als die aktuelle 
    /// eComparisonResult.More, wenn die übergebene Version größer ist als die aktuelle 
    /// 
    /// </summary>
    /// <param name="version">Version zum Vergleich</param>.
    /// <returns>eComparisonResult.Equal, wenn gleich, eComparisonResult.Smaller, wenn die übergebene Version. 
    /// kleiner ist oder eComparisonResult.Bigger, wenn die übergebene Version größer ist</returns>
    public eComparisonResult CompareTo(VersionInformation version)
    {
        if (version == null)
            throw new ArgumentNullException(nameof(version));

        eComparisonResult ret = eComparisonResult.Equal;
        if (Major > version.Major)
            ret = eComparisonResult.Smaller;
        else if (Major == version.Major && Minor > version.Minor)
            ret = eComparisonResult.Smaller;
        else if (Major == version.Major && Minor == version.Minor && Build > version.Build)
            ret = eComparisonResult.Smaller;
        else if (Major < version.Major)
            ret = eComparisonResult.Bigger;
        else if (Major == version.Major && Minor < version.Minor)
            ret = eComparisonResult.Bigger;
        else if (Major == version.Major && Minor == version.Minor && Build < version.Build)
            ret = eComparisonResult.Bigger;
        else if (Major == version.Major && Minor == version.Minor && Build == version.Build)
            ret = eComparisonResult.Equal;

        return ret;
    }
    #endregion
}

/// <summary>
/// Ergebnis eines Vergleichs
/// </summary>
public enum eComparisonResult
{
    /// <summary>
    /// Kleiner/kleiner
    /// </summary>
    Smaller = 0,
    /// <summary>
    /// Gleich
    /// </summary>
    Equal = 1,
    /// <summary>
    /// Höher/Größer
    /// </summary>
    Bigger = 2
}
