using DevExpress.Utils;

namespace AF.WINFORMS.DX;

/// <summary>
/// Label um Text im ausgegrauten Style anzuzeigen (UIStyle.GRAY)
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
public class AFLabelGrayText : AFLabel
{
    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        Style = UIStyle.GRAY;

        base.OnHandleCreated(e);
    }
}

/// <summary>
/// Label um Text klein und im ausgegrauten Style anzuzeigen (UIStyle.GRAYSMALL)
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
public class AFLabelGrayTextSmall : AFLabel
{
    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        Style = UIStyle.GRAYSMALL;

        base.OnHandleCreated(e);
    }
}