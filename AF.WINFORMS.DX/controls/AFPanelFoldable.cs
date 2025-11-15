using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Svg;

namespace AF.WINFORMS.DX;

/// <summary>
/// Panel, dass aus einem Titel und einem zusammenklappbaren Bereich besteht.
/// </summary>
public class AFPanelFoldable : AFUserControl
{
    // private AFHeaderButton? header;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFPanelFoldable()
    {
        
    }

    /// <summary>
    /// Anordnung des Titels
    /// </summary>
    [Browsable(true)]
    [DefaultValue(DockStyle.Top)]
    public DockStyle TitleArrangement { get; set; } = DockStyle.Top;

    /// <summary>
    /// Control, dass im zusammenklappbaren Bereich angezeigt werden soll
    /// </summary>
    [Browsable(true)]
    [DefaultValue(null)]
    public Control? ContentControl { get; set; }
}

/// <summary>
/// Control, dass aus einem Label und einem Button besteht
/// </summary>
public class AFHeaderButton : AFUserControl
{
    private AFLabel? label;
    private AFButton? button;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFHeaderButton()
    {

    }



    /// <summary>
    /// Aktuelles Layout anlegen
    /// </summary>
    public void UpdateLayout()
    {
        SuspendLayout();

        if (Controls.Count > 0)
            Controls.Clear();

        label ??= new() { Text = Caption, Dock = DockStyle.Fill, AllowHtmlString = true };
        button ??= new() { PaintStyle = PaintStyles.Light, Dock = DockStyle.Right, ImageOptions = { SvgImage  = ButtonImage ?? UI.GetImage(Symbol.Settings), SvgImageSize  = new(16,16), Location = ImageLocation.MiddleCenter }};

        Controls.Add(button);
        
        if (ShowLabel)
            Controls.Add(label);
        
        ResumeLayout();

        Show();

        adjustSize();
    }

    private void adjustSize()
    {
        int minHeight = UI.GetScaled(20);

        if (Dock is DockStyle.Top or DockStyle.Bottom)
            Size = new Size(Width, Math.Max(button?.Height ?? minHeight, label?.Height ?? minHeight) + Padding.Vertical);
        else if (Dock is DockStyle.Left or DockStyle.Right)
            Size = new Size(Math.Max(button?.Width ?? minHeight, label?.Width ?? minHeight) + Padding.Vertical, Height);
    }


    /// <summary>
    /// Label anzeigen
    /// </summary>
    [Browsable(true)]
    [DefaultValue(true)]
    public bool ShowLabel { get; set; } = true;

    /// <summary>
    /// Anzuzeigender Titel
    /// </summary>
    [Browsable(true)]
    [DefaultValue("")]
    public string Caption { get; set; } = "";

    /// <summary>
    /// Symbol, dass auf dem Button angezeigt werden soll (leer/null = Standardsymbol anzeigen)
    /// </summary>
    [Browsable(true)]
    [DefaultValue(null)]
    public SvgImage? ButtonImage { get; set; }

    /// <summary>
    /// Ausrichtung des Buttons
    /// </summary>
    [Browsable(true)]
    [DefaultValue(DockStyle.Right)]
    public DockStyle ButtonAlignment { get; set; } = DockStyle.Right;
}

