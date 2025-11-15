using DevExpress.Utils;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <inheritdoc/>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
public class AFEditTime : TimeEdit
{
    /// <summary>
    /// Eingabe ohne Sekunden
    /// </summary>
    /// <returns>this</returns>
    public AFEditTime AsShortTime()
    {
        Properties.DisplayFormat.FormatString = "t";
        Properties.EditFormat.FormatString = "t";
        Properties.EditMask = "t";

        return this;
    }

    /// <summary>
    /// Eingabe mit Sekunden
    /// </summary>
    /// <returns>this</returns>
    public AFEditTime AsLongTime()
    {
        Properties.DisplayFormat.FormatString = "T";
        Properties.EditFormat.FormatString = "T";
        Properties.EditMask = "T";

        return this;
    }
}