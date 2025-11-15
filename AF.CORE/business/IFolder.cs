namespace AF.BUSINESS;

/// <summary>
/// Interface für eine Klasse die in AF.BUSINESS als Folder verwendet wird.
/// </summary>
public interface IFolder
{
    /// <summary>
    /// Ordner, in dem der aktuelle Ordner ablegt ist
    /// </summary>
    public IFolder? ParentFolder { get; set; }

    /// <summary>
    /// ID des Ordners, in dem der aktuelle Ordner ablegt ist
    /// </summary>
    public Guid? ParentFolderID { get; set; }

    /// <summary>
    /// ID des aktuellen Ordners
    /// </summary>
    public Guid FolderID { get; }
}

/// <summary>
/// Interface für eine Klasse die in AF.BUSINESS als Controller für Folder verwendet wird.
/// </summary>
public interface IFolderController
{

}

/// <summary>
/// Zentrale Erweiterungsmethoden für IFolderProfile und IFolderController, 
/// die dann im konkreten Implementierungen immer automatisch zur verfügung stehen.
/// </summary>
public static class IFolderEx
{

}