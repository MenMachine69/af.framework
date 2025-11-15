using DevExpress.Utils;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <inheritdoc/>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
public class AFEditDate : DateEdit
{
    /// <inheritdoc />
    public AFEditDate()
    {
        Properties.NullDate = DateTime.MinValue;
        Properties.NullDateCalendarValue = DateTime.MinValue;
    }
}

/// <summary>
/// Eingabe für 'nur Datum'
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
[DefaultBindingProperty("DateOnly")]
public class AFEditDateOnly : AFEditDate
{
    /// <inheritdoc />
    public AFEditDateOnly()
    {
        
    }
}