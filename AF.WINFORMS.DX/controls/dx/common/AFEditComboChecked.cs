using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace AF.WINFORMS.DX;

/// <inheritdoc/>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
public class AFEditComboChecked : CheckedComboBoxEdit
{
    /// <summary>
    /// Bestimmt ob die Breite des Popups automatisch an die 
    /// Breite des Controls angepasst werden soll.
    /// </summary>
    [Browsable(true)]
    [DefaultValue(false)]
    public bool AutoPopupWidth { get; set; }

    /// <summary>
    /// Methode die beim Erzeugen des Controls ausgeführt wird.
    /// </summary>
    /// <param name="e">Parameter</param>
    protected override void OnHandleCreated(EventArgs e)
    {
        QueryPopUp += (_, _) =>
        {
            if (AutoPopupWidth)
                Properties.PopupFormMinSize = new(Width, Properties.PopupFormMinSize.Height);
        };
    }

    /// <summary>
    /// Liste der ausgewählten Einträge.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<CheckedListBoxItem> GetCheckedItems()
    {
        return Properties.Items.Where(i => i.CheckState == CheckState.Checked);
    }

    /// <summary>
    /// Gibt an, ob alle Einträge ausgewählt sind
    /// </summary>
    public bool AllItemsChecked => (Properties.Items.Count == Properties.Items.Count(i => i.CheckState == CheckState.Checked));

    /// <summary>
    /// Gibt an, ob keine Einträge ausgewählt sind
    /// </summary>
    public bool NoItemsChecked => (Properties.Items.Count == Properties.Items.Count(i => i.CheckState == CheckState.Unchecked));

}