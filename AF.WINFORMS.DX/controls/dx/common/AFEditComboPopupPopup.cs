namespace AF.WINFORMS.DX;

/// <summary>
/// Popup-Control zur Darstellung in AFEditComboPopup.
/// </summary>
public class AFEditComboPopupPopup : AFUserControl
{
    /// <summary>
    /// Combobox zu der das Popup gehört.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Control? ComboPopup { get; set; }
}