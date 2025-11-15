using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace AF.WINFORMS.DX;

/// <inheritdoc/>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
[DefaultBindingProperty("SelectedValue")]
public class AFEditComboImageList : ImageComboBoxEdit
{
    /// <summary>
    /// Property name of Items which is used as Value
    /// </summary>
    [Browsable(true)]
    [DefaultValue(nameof(EditValue))]
    public string ValueMember { get; set; } = nameof(EditValue);

    /// <summary>
    /// Selected Value (instead of SelectedItem)
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public object? SelectedValue
    {
        get
        {
            if (SelectedItem != null && ValueMember.IsEmpty() == false)
                return ((ImageComboBoxItem)SelectedItem).Value.InvokeGet(ValueMember);

            return SelectedItem;
        }
        set
        {
            if (Properties.Items != null && ValueMember.IsEmpty() == false)
            {
                object? item = null;

                foreach (var itm in Properties.Items.OfType<ImageComboBoxItem>())
                {
                    if (itm != null && itm.Value != null)
                    {
                        var val = itm.Value.InvokeGet(ValueMember);
                        if (val != null && val.Equals(value))
                        {
                            item = itm;
                            break;
                        }
                    }
                }

                if (item != null)
                    SelectedItem = item;
                else
                    SelectedItem = value;

                return;
            }

            SelectedItem = value;
        }
    }

    /// <summary>
    /// Bestimmt, ob die Breite des Popups automatisch an die 
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
    /// Füllt die Listbox mit einer Liste von ListItem-Objekten
    /// 
    /// Der Anzeigename des Items entspricht dabei dem Ergebnis der ToSTring() Methode des Items. 
    /// Der ValueMember (Rückgabewert von SelectedValue) entspricht standardmässig der Value-Eigenschaft des Items und 
    /// muss nach dem zuweisen der Liste NICHT manuell gesetzt werden.
    /// </summary>
    /// <param name="items">Liste der zur Auswahl anzuzeigenden Objekte</param>
    public void Fill(IEnumerable<ListItem> items)
    {
        Properties.Items.Clear();
        ValueMember = nameof(ListItem.Value);
        foreach (ListItem o in items)
            Properties.Items.Add(new ImageComboBoxItem(o, o.ImageIndex));
    }
}