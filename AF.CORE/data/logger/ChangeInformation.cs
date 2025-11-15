namespace AF.DATA;

/// <summary>
/// Informationen über die geänderten Daten
/// </summary>
/// <param name="RecordID">ID des Datensatzes</param>
/// <param name="Operation">Typ der Operation</param>
public record ChangeInformation(eLoggerOperation Operation, Guid RecordID)
{
    /// <summary>
    /// Geänderte Felder
    /// </summary>
    public List<ChangeInformationField> Fields { get; } = [];
}

/// <summary>
/// Information about a single field
/// </summary>
/// <param name="Field">Feld</param>
/// <param name="OldValue">alter Wert</param>
/// <param name="NewValue">neuer Wert</param>
public record ChangeInformationField(string Field, object? OldValue, object? NewValue)
{ 

}