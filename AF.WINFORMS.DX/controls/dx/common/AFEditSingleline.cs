using DevExpress.Data.Mask;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Mask;

namespace AF.WINFORMS.DX;

/// <inheritdoc/>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
public class AFEditSingleline : TextEdit
{
    /// <summary>
    /// Maskierung der Eingabe setzen.
    /// </summary>
    /// <param name="maskType"></param>
    /// <param name="maskDetail"></param>
    public void SetMask(eSinglelineEditMask maskType, string? maskDetail = null)
    {


        switch (maskType)
        {
            case eSinglelineEditMask.Currency:
                var settingscurr = Properties.MaskSettings.Configure<MaskSettings.Numeric>();
                settingscurr.MaskExpression = "c2";
                settingscurr.AutoHideDecimalSeparator = true;
                settingscurr.HideInsignificantZeros = true;
                settingscurr.ValueAfterDelete = NumericMaskManager.ValueAfterDelete.Null;
                settingscurr.ValueType = typeof(decimal);
                break;
            case eSinglelineEditMask.Decimal:
                var settingsdec = Properties.MaskSettings.Configure<MaskSettings.Numeric>();
                settingsdec.MaskExpression = "f2";
                settingsdec.AutoHideDecimalSeparator = true;
                settingsdec.HideInsignificantZeros = true;
                settingsdec.ValueAfterDelete = NumericMaskManager.ValueAfterDelete.Null;
                settingsdec.ValueType = typeof(decimal);
                break;
            case eSinglelineEditMask.Percent:
                var settingsperc = Properties.MaskSettings.Configure<MaskSettings.Numeric>();
                settingsperc.MaskExpression = "p2";
                settingsperc.AutoHideDecimalSeparator = true;
                settingsperc.HideInsignificantZeros = true;
                settingsperc.ValueAfterDelete = NumericMaskManager.ValueAfterDelete.Null;
                settingsperc.ValueType = typeof(decimal);
                break;
            case eSinglelineEditMask.Integer:
                var settingsint = Properties.MaskSettings.Configure<MaskSettings.Numeric>();
                settingsint.MaskExpression = "N0";
                settingsint.AutoHideDecimalSeparator = true;
                settingsint.HideInsignificantZeros = true;
                settingsint.ValueAfterDelete = NumericMaskManager.ValueAfterDelete.Null;
                settingsint.ValueType = typeof(int);
                break;
            case eSinglelineEditMask.Date:
                var settingsdate = Properties.MaskSettings.Configure<MaskSettings.DateTime>();
                settingsdate.MaskExpression = "d";
                settingsdate.SpinWithCarry = true;
                settingsdate.UseAdvancingCaret = true;
                break;
            case eSinglelineEditMask.Time:
                var settingstime = Properties.MaskSettings.Configure<MaskSettings.DateTime>();
                settingstime.MaskExpression = "t";
                settingstime.SpinWithCarry = true;
                settingstime.UseAdvancingCaret = true;
                break;
            case eSinglelineEditMask.DateTime:
                var settingsdatetime = Properties.MaskSettings.Configure<MaskSettings.DateTime>();
                settingsdatetime.MaskExpression = "g";
                settingsdatetime.SpinWithCarry = true;
                settingsdatetime.UseAdvancingCaret = true;
                break;
            case eSinglelineEditMask.IPv4:
                var settingsipv4 = Properties.MaskSettings.Configure<MaskSettings.RegExp>();
                settingsipv4.MaskExpression =
                    "(([01]?[0-9]?[0-9])|(2[0-4][0-9])|(25[0-5]))\\.(([01]?[0-9]?[0-9])|(2[0-4][0-9])|(25[0-5]))\\.(([01]?[0-9]?[0-9])|(2[0-4][0-9])|(25[0-5]))\\.(([01]?[0-9]?[0-9])|(2[0-4][0-9])|(25[0-5]))";
                settingsipv4.Placeholder = '_';
                settingsipv4.IsAutoComplete = true;
                settingsipv4.ShowPlaceholders = true;
                break;
            case eSinglelineEditMask.EMail:
                var settingsemail = Properties.MaskSettings.Configure<MaskSettings.RegExp>();
                settingsemail.MaskExpression =
                    "\\p{Ll}{1,}\\p{P}{0,}\\p{Ll}{0,}@\\p{Ll}{1,}\\p{P}{0,}\\p{Ll}{0,}.\\p{Ll}{1,}";
                settingsemail.Placeholder = '_';
                settingsemail.IsAutoComplete = true;
                settingsemail.ShowPlaceholders = true;
                break;
            case eSinglelineEditMask.Guid:
                var settingsguid = Properties.MaskSettings.Configure<MaskSettings.RegExp>();
                settingsguid.MaskExpression =
                    "[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}";
                settingsguid.Placeholder = '_';
                settingsguid.IsAutoComplete = true;
                settingsguid.ShowPlaceholders = true;
                break;

        }
    }


}

/// <summary>
/// Standard-Masken für einzeilige Eingabe setzen.
/// </summary>
public enum eSinglelineEditMask
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    Guid,
    Currency,
    Date,
    Time,
    DateTime,
    Decimal,
    Integer,
    Percent,
    Phone,
    EMail,
    IPv4
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

}