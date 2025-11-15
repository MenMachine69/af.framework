namespace AF.MVC;

/// <summary>
/// Schnittstelle eines einzelnen Abschnitts in einem IModelEditor.
/// 
/// Einzelne Abschnitte können separat modifiziert werden (z.B. Speichern von Modifikationen in Feldern, 
/// die in diesem Abschnitt gebunden sind).
/// </summary>
public interface IEditorSection
{
    /// <summary>
    /// Aktuelles IModel
    /// </summary>
    IModel? Model { get; set; }

    /// <summary>
    /// Validiert das aktuelle IModel und gibt bei Ungültigkeit eine Liste von Fehlern zurück.
    /// </summary>
    /// <param name="errors">Liste der Fehler</param>
    /// <returns>true wenn valide, sonst false</returns>
    bool IsValid(ValidationErrorCollection errors);
}