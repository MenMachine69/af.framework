using DevExpress.Utils;

namespace AF.WINFORMS.DX;

/// <summary>
/// Label um Text im defauklt Style anzuzeigen (UIStyle.STANDARD)
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
public class AFLabelStandard : AFLabel
{
    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        Style = UIStyle.STANDARD;

        base.OnHandleCreated(e);
    }
}

/// <summary>
/// Label um Text im kleinen Style anzuzeigen (UIStyle.SMALL)
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
public class AFLabelSmall : AFLabel
{
    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        Style = UIStyle.SMALL;

        base.OnHandleCreated(e);
    }
}