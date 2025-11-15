using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace AF.WINFORMS.DX;

/// <inheritdoc/>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
[DesignerCategory("Code")]
public class AFEditComboPopup : PopupContainerEdit
{
    private readonly PopupContainerControl popupContainer = null!;
    private AFEditComboPopupPopup? popupControl;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFEditComboPopup()
    {
        if (UI.DesignMode) return;

        Properties.TextEditStyle = TextEditStyles.DisableTextEditor;

        popupContainer = new();
        Properties.PopupControl = popupContainer;
        Properties.NullValuePrompt = "";
        Properties.NullValuePromptShowForEmptyValue = true;
        Properties.AllowNullInput = DefaultBoolean.True;
    }


    /// <summary>
    /// Constructor
    /// </summary>
    public void Setup(AFEditComboPopupPopup popup)
    {
        if (UI.DesignMode) return;

        popupControl = popup;
        popupControl.Dock = DockStyle.Fill;
        popupControl.ComboPopup = this;

        Size size = new(popupControl.Width, popupControl.Height);
        
        popupContainer.Size = new(popupControl.Width, popupControl.Height);
        popupContainer.Controls.Add(popupControl);
    }

    /// <summary>
    /// Zugriff auf das aktuelle Popup-Control
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFEditComboPopupPopup? CurrentPopupControl => popupControl;


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

    /// <summary>
    /// Bestimmt, ob die Breite des Popups automatisch an die 
    /// Breite des Controls angepasst werden soll.
    /// </summary>
    [Browsable(true)]
    [DefaultValue(false)]
    public bool AutoPopupWidth { get; set; }

}