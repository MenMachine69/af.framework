namespace AF.WINFORMS.DX;

/// <summary>
/// Hilfsfunktionen für Variablen-UI
/// </summary>
public static class VariableUIHelper
{
    /// <summary>
    /// Liefert das passende Control für die Eingabe der Variablen
    /// </summary>
    /// <param name="var">Variable</param>
    /// <returns>passendes Control</returns>
    public static Control? GetControl(VariableBase var)
    {
        if (var is VariableBool)
            return new DevExpress.XtraEditors.ToggleSwitch();
        if (var is VariableString)
            return new DevExpress.XtraEditors.TextEdit();
        if (var is VariableDecimal)
            return new DevExpress.XtraEditors.SpinEdit();
        if (var is VariableInt)
            return new DevExpress.XtraEditors.SpinEdit();
        if (var is VariableDateTime)
            return new DevExpress.XtraEditors.DateEdit();
        if (var is VariableList)
            return new DevExpress.XtraEditors.ComboBoxEdit();
        if (var is VariableScript)
            return null;
        if (var is VariableQuery)
            return new DevExpress.XtraEditors.SearchLookUpEdit();

        return null;
    }
}

