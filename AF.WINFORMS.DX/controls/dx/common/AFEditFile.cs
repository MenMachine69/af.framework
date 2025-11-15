using DevExpress.Utils;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Einzeilige Eingabe zur Auswahl einer Datei
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
[DefaultBindingProperty("File")]
public class AFEditFile : AFEditButtons
{
    private XtraOpenFileDialog? _dialog;

    /// <summary>
    /// Cosntructor
    /// </summary>
    public AFEditFile() : base()
    {
        if (UI.DesignMode) return;

        Properties.Buttons.Clear();

        this.AddButton(UI.GetImage(Symbol.Document), tooltip: UI.GetSuperTip("DATEI AUSWÄHLEN", "Auswahl einer Datei..."));
        this.ButtonClick += (_, _) =>
        {
            _dialog ??= new() { AddExtension = true, DefaultExt = DefaultExtension, Filter = FileFilter, DefaultViewMode = DevExpress.Dialogs.Core.View.ViewMode.Details, CheckFileExists = true };

            if (_dialog.ShowDialog(FindForm()) == DialogResult.OK)
                File = _dialog.FileName;
        };
    }

    /// <summary>
    /// Ausgewählte Datei
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string File { get => Text; set => Text = value; }

    /// <summary>
    /// Standarderweiterung für die Datei (Dateiendung)
    /// </summary>
    [Browsable(true), DefaultValue("")]
    public string DefaultExtension { get; set; } = "";

    /// <summary>
    /// Filter für die auswählbaren Dateien. Standard: *.*
    /// </summary>
    [Browsable(true), DefaultValue("Alle Dateien|*.*")]
    public string FileFilter { get; set; } = "Alle Dateien|*.*";

}

/// <summary>
/// Einzeilige Eingabe zur Auswahl eines Ordners
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
[DefaultBindingProperty("File")]
public class AFEditFolder : AFEditButtons
{
    private XtraFolderBrowserDialog? _dialog;

    /// <summary>
    /// Cosntructor
    /// </summary>
    public AFEditFolder() : base()
    {
        if (UI.DesignMode) return;

        Properties.Buttons.Clear();

        this.AddButton(UI.GetImage(Symbol.Folder), tooltip: UI.GetSuperTip("ORDNER AUSWÄHLEN", "Auswahl eines Ordners..."));
        this.ButtonClick += (_, _) =>
        {
            _dialog ??= new() { SelectedPath = Folder };

            if (_dialog.ShowDialog(FindForm()) == DialogResult.OK)
                Folder = _dialog.SelectedPath;
        };
    }

    /// <summary>
    /// Ausgewählte Datei
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Folder { get => Text; set => Text = value; }
}