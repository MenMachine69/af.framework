
namespace AF.DATA;

/// <summary>
/// Symbole für Datenbank-Trigger-Ereignisse, die in ITranslator verwendet werden
/// </summary>
public enum eTriggerEvent
{
    /// <summary>
    /// Vor dem Einfügen
    /// </summary>
    BeforeInsert,
    /// <summary>
    /// Vor dem Update
    /// </summary>
    BeforeUpdate,
    /// <summary>
    /// Vor dem Löschen
    /// </summary>
    BeforeDelete,
    /// <summary>
    /// Nach dem Einfügen
    /// </summary>
    AfterInsert,
    /// <summary>
    /// Nach dem Update
    /// </summary>
    AfterUpdate,
    /// <summary>
    /// Nach dem Löschen
    /// </summary>
    AfterDelete
}