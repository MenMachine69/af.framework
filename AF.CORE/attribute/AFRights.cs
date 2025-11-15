namespace AF.DATA;

/// <summary>
/// Markiert eine Enumeration als Berechtigungs-Enumeration (Auflistung, die Right-Element enthält)
/// 
/// Enumerationen mit diesem Attribut werden als Recht in den Business-Layer integriert.
/// </summary>
[AttributeUsage(AttributeTargets.Enum)]
public sealed class AFRights : Attribute {  }