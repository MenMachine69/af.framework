namespace AF.CORE;

/// <summary>
/// Informationen über einen geänderten Wert.
/// 
/// Wird zum Beispiel verwendet, um Änderungen in einer Datenbank zu protokollieren.
/// </summary>
/// <param name="Name">Name des Wertes</param>
/// <param name="OldValue">Alter Wert</param>
/// <param name="NewValue">Neuer Wert</param>
public record ChangedValueInfo(string Name, object? OldValue, object? NewValue)
{

}