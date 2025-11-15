namespace AF.BUSINESS;

/// <summary>
/// Interface für eine Klasse die in AF.BUSINESS als Connectstring verwendet wird.
/// </summary>
public interface IConnectstring
{
    /// <summary>
    /// ID des Connectstrings
    /// </summary>
    Guid Id { get; }
}

/// <summary>
/// Interface für eine Klasse die in AF.BUSINESS als Controller für Connectstring verwendet wird.
/// </summary>
public interface IConnectstringController
{

}

/// <summary>
/// Zentrale Erweiterungsmethoden für IConnectstring und IConnectstringController, 
/// die dann im konkreten Implementierungen immer automatisch zur verfügung stehen.
/// </summary>
public static class IConnectstringEx
{
    
}