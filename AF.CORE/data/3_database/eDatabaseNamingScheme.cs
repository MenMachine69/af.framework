
namespace AF.DATA;

/// <summary>
/// Schema für die Benennung von Tabellen, Ansichten und Feldern.
/// 
/// Dieses Schema kann z.B. verwendet werden, um Tabellen und Felder zu erstellen, wenn 
/// die Datenbank spezielle Namenskonventionen hat (wie PostgeSQL - alle Kleinbuchstaben). 
/// </summary>
public enum eDatabaseNamingScheme
{
    /// <summary>
    /// alle Tabellen-, View- und Feldnamen haben die gleichen Namen wie die Eigenschaften im Modell
    /// </summary>
    original,
    /// <summary>
    /// alle Tabellen-, View- und Feldnamen werden kleingeschrieben
    /// </summary>
    lowercase,
    /// <summary>
    /// alle Tabellen-, View- und Feldnamen werden in Großbuchstaben geschrieben
    /// </summary>
    uppercase
}