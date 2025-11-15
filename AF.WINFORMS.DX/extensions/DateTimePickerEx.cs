namespace AF.WINFORMS.DX;

/// <summary>
/// Erweiterungsmethoden für DateTimePicker
/// </summary>
public static class DateTimePickerEx
{
    /// <summary>
    /// Set up data binding to the 'Value' property as the default binding
    /// </summary>
    public static Binding Bound(this DateTimePicker ctrl, PropertyInfo property, object datasource)
    {
        return new Binding(nameof(DateTimePicker.Value), datasource, property.Name, false, DataSourceUpdateMode.OnValidation, null, @"d");
    }
}