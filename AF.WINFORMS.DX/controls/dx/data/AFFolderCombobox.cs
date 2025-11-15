using DevExpress.XtraEditors.Controls;

namespace AF.WINFORMS.DX;

/// <summary>
/// Combobox zur Auswahl aus SYS_FOLDER-Feldern einer Tabelle/eines Views 
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[DefaultBindingProperty("Text")]
public sealed class AFFolderCombobox : AFEditCombo
{
    private bool folderLoaded = false;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFFolderCombobox()
    {
        if (UI.DesignMode) return;

        Properties.TextEditStyle = TextEditStyles.Standard;

        QueryPopUp += (_, _) =>
        {
            if (folderLoaded) return;

            if (ModelType == null) return;

            var folderField = ModelType.GetTypeDescription().Fields.FirstOrDefault(f => f.Value?.Field?.SystemFieldFlag == eSystemFieldFlag.Folder).Value;

            folderLoaded = true;

            if (folderField == null) return;

            string folderName = folderField.Name;

            var list = ModelType.GetController().ReadSelectionModels(false);
            
            List<string> folders = [];

            for (int pos = 0; pos < list.Count; pos++)
            {
                if (list[pos] == null) continue;

                string? folder = list[pos]!.GetType().GetTypeDescription().Accessor[list[pos], folderName] as string;

                if (string.IsNullOrEmpty(folder)) continue;

                if (folders.Contains(folder!)) continue;

                folders.Add(folder!);
            }
            
            folders.Sort();

            Properties.Items.AddRange([.. folders]);
        };
    }

    /// <summary>
    /// Typ der Models, die zur Auswahl angezeigt werden sollen.
    ///
    /// Der Typ muss dein Feld mit dem Attribut eSystemFieldFlag.Folder enthalten, in dem die Namen der Ordner abgelegt sind.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Type? ModelType { get; set; } = null;

}