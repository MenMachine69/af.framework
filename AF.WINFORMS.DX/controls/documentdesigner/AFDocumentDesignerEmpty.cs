namespace AF.WINFORMS.DX;

/// <summary>
/// Designer/Editor für unbekannte Dokumente
/// </summary>
public class AFDocumentDesignerEmpty : AFUserControl
{

    /// <summary>
    /// Constructor
    /// </summary>
    public AFDocumentDesignerEmpty()
    {
        if (UI.DesignMode) return;

        AFLabelBoldText lbl = new AFLabelBoldText() { Text = "Vorlage vom Typ <unbekannt> kann nicht bearbeitet werden. Wählen Sie ggf. einen anderen Documenttyp.", Dock = DockStyle.Fill };
        lbl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
        lbl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        lbl.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
        lbl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;

        Controls.Add(lbl);
    }
}

