using DevExpress.Utils;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <inheritdoc/>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
[DefaultBindingProperty("SelectedValue")]
public class AFEditCombo : ComboBoxEdit, ICustomBindingSupport
{
    /// <summary>
    /// Property name of Items which is used as Value
    /// </summary>
    [Browsable(true)]
    [DefaultValue(null)] 
    public string? ValueMember { get; set; }

    /// <summary>
    /// Selected Value (instead of SelectedItem)
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public object? SelectedValue
    {
        get
        {
            if (SelectedItem != null && ValueMember != null && ValueMember.IsNotEmpty())
                return SelectedItem.InvokeGet(ValueMember);

            return SelectedItem;
        }
        set
        {
            object? item = null;

            if (Properties.Items != null && ValueMember != null && ValueMember.IsNotEmpty())
            {
                foreach (var i in Properties.Items.OfType<object>())
                {
                    var val = i.InvokeGet(ValueMember);

                    if (val != null && val.Equals(value))
                    {
                        item = i;
                        break;
                    }
                }


            }

            if (item != null)
                SelectedItem = item;
            else
                SelectedItem = value;
        }
    }

    /// <inheritdoc />
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual string? CustomBindingProperty { get; set; }

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
        base.OnHandleCreated(e);

        if (UI.DesignMode) return;

        QueryPopUp += (_, _) =>
        {
            if (AutoPopupWidth)
                Properties.PopupFormMinSize = new(Width, Properties.PopupFormMinSize.Height);
        };
    }
}