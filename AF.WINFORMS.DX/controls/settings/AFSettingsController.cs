namespace AF.WINFORMS.DX;

/// <summary>
/// Basis SettingsController, der abhängig vom ValueType bestimmte Standardeingabe-Controls liefert.
/// </summary>
public abstract class AFSettingsController
{
    /// <summary>
    /// Liefert den passenden Editor für ein Element
    /// </summary>
    /// <param name="element">Element, für das der Editor benötigt wird</param>
    /// <returns>das Control, das als Editor verwendet wird.
    /// NULL, wenn ein Standard-Control (vom ValueType abhängig) verwendet werden soll.</returns>
    public virtual Control? GetEditor(AFSettingsElement element)
    {
        if (element.ValueType == null || element.ValueName == "")
            return null;

        if (element.ValueType == typeof(string))
        {
            var editor = new AFEditSingleline() { Name = element.ValueName };
            editor.Size = new(500, 26);
            return editor;
        }
        else if (element.ValueType == typeof(bool))
            return new AFEditToggle() { Name = element.ValueName, Dock = DockStyle.Right };
        else if (element.ValueType == typeof(DateTime))
            return new AFEditDate() { Name = element.ValueName };
        else if (element.ValueType == typeof(int))
            return new AFEditSpin() { Name = element.ValueName };
        else if (element.ValueType == typeof(decimal))
            return new AFEditCalc() { Name = element.ValueName };
        else if (element.ValueType == typeof(FileInfo))
            return new AFEditFile() { Name = element.ValueName, Size = new(500, 26) };
        else if (element.ValueType == typeof(DirectoryInfo))
            return new AFEditDirectory() { Name = element.ValueName, Size = new(500, 26) };

        return null;
    }
}

