namespace AF.WINFORMS.DX;

/// <summary>
/// Presenter, der mehrere Labels/Werte mit Caption darstellt.
/// </summary>
[DesignerCategory("Code")]
public class AFPresenterValues : AFPresenterBase
{
    /// <summary>
    /// Constructor
    /// </summary>
    public AFPresenterValues() : base(false) { }

    /// <summary>
    /// Neues Element/neuen Wert hinzufügen
    /// </summary>
    /// <param name="caption">Überschrift</param>
    /// <param name="value">Wert</param>
    /// <param name="size">Höhe/Breite</param>
    /// <param name="dockstyle">Dock-Richtung</param>
    public AFLabelValueCaption Add(string caption, string value, int size, DockStyle dockstyle)
    {
        var lbl = new AFLabelValueCaption() 
        {
            Dock = dockstyle,
            Text = value,
            Caption = caption,
            DrawBorder = true,
            AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None,
            Appearance = {
                ForeColor = UI.TranslateSystemToSkinColor(SystemColors.Highlight),
                FontSizeDelta = 3,
                Options =
                {
                    UseForeColor = true,
                    UseFont = true,
                }
            },
        };
        lbl.AdjustAlignment(ContentAlignment.BottomRight);
        lbl.Padding = new(5);

        switch (lbl.Dock)
        {
            case DockStyle.Top:
            case DockStyle.Bottom:
                lbl.Size = new(ClientRectangle.Width, size);
                break;
            default:
                lbl.Size = new(size, ClientRectangle.Height);
                break;
        }


        Controls.Add(lbl);
        lbl.BringToFront();

        return lbl;
    }
}

