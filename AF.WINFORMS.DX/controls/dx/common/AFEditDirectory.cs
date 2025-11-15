using DevExpress.Utils;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Einzeilige Eingabe zur Auswahl eines Verzeichnisses
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
[DefaultBindingProperty("Directory")]
public class AFEditDirectory : AFEditButtons
{
    private XtraFolderBrowserDialog? _dialog;

    /// <summary>
    /// Cosntructor
    /// </summary>
    public AFEditDirectory() : base()
    {
        if (UI.DesignMode) return;

        Properties.Buttons.Clear();

        this.AddButton(UI.GetImage(Symbol.Folder), tooltip: UI.GetSuperTip("ORDNER AUSWÄHLEN", "Auswahl eines Ordners..."));

        this.ButtonClick += (_, _) =>
        {
            _dialog ??= new();

            _dialog.ShowNewFolderButton = true;
            _dialog.DialogStyle = DevExpress.Utils.CommonDialogs.FolderBrowserDialogStyle.Wide;

            if (_dialog.ShowDialog(FindForm()) == DialogResult.OK)
                Directory = _dialog.SelectedPath;
        };
    }


    /// <summary>
    /// Ausgewählte Datei
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Directory { get => Text; set => Text = value; }
}