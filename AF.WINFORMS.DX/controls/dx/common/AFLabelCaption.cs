using DevExpress.Utils;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Label um Text als Überschrift anzuzeigen (UIStyle.CAPTION)
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
[DesignerCategory("Code")]
public sealed class AFLabelCaption : AFLabel
{
    /// <summary>
    /// Constructor
    /// </summary>
    public AFLabelCaption()
    {
        Padding = new(10, 6, 10, 6);
        AutoSizeMode = LabelAutoSizeMode.Vertical;
        Dock = DockStyle.Top;
    }

    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        Style = UIStyle.CAPTION;

        base.OnHandleCreated(e);
    }
}

/// <summary>
/// Label um Text als Überschrift anzuzeigen (UIStyle.CAPTION)
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
[DesignerCategory("Code")]
public sealed class AFLabelCaptionSmall : AFLabel
{
    /// <summary>
    /// Constructor
    /// </summary>
    public AFLabelCaptionSmall()
    {
        Padding = new(6);
        AutoSizeMode = LabelAutoSizeMode.Vertical;
        Dock = DockStyle.Top;
        AllowHtmlString = true;
    }

    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        Style = UIStyle.CAPTIONSMALL;

        base.OnHandleCreated(e);
    }
}


/// <summary>
/// Label um Text als Überschrift anzuzeigen (UIStyle.CAPTIONH1)
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
[DesignerCategory("Code")]
public class AFLabelCaptionH1 : AFLabel
{
    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        Style = UIStyle.CAPTIONH1;

        base.OnHandleCreated(e);
    }
}

/// <summary>
/// Label um Text als Überschrift zweiter Ordnung anzuzeigen (UIStyle.CAPTIONH2)
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
[DesignerCategory("Code")]
public class AFLabelCaptionH2 : AFLabel
{
    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        Style = UIStyle.CAPTIONH2;

        base.OnHandleCreated(e);
    }
}

/// <summary>
/// Label um Text als Überschrift dritter Ordnung anzuzeigen (UIStyle.CAPTIONH3)
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
[DesignerCategory("Code")]
public class AFLabelCaptionH3 : AFLabel
{
    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        Style = UIStyle.CAPTIONH3;

        base.OnHandleCreated(e);
    }
}

/// <summary>
/// Label um Text als Überschrift anzuzeigen (UIStyle.CAPTIONH1)
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
[DesignerCategory("Code")]
public class AFLabelCaptionInverted : AFLabel
{
    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        Style = UIStyle.CAPTIONINVERTED;

        base.OnHandleCreated(e);
    }
}