using System.Drawing.Drawing2D;
using DevExpress.Utils;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Label um Text fett anzuzeigen (UIStyle.BOLD)
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
public class AFLabelBoldText : AFLabel
{
    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        Style = UIStyle.BOLD;

        base.OnHandleCreated(e);
    }
}

/// <summary>
/// Label um Text fett anzuzeigen (UIStyle.BOLDLINK).
/// 
/// Fungiert als Link und hat eine ClickAction, die beim Klick ausgeführt wird.
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
public class AFLabelBoldLink : AFLabel
{
    private Action<AFLabel>? _clickAction;

    /// <summary>
    /// Aktion, die beim Klicken ausgeführt werden soll.
    /// 
    /// Wenn !NULL wird das Label beim Hoover mit dem Pointer 'Hand' dargestellt-
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Action<AFLabel>? ClickAction
    {
        get => _clickAction;
        set
        {
            _clickAction = value;

            if (_clickAction != null)
                Cursor = Cursors.Hand;

            Click += (_, _) => { _clickAction?.Invoke(this); };
        }
    }

    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        Style = UIStyle.BOLDLINK;

        base.OnHandleCreated(e);
    }
}

/// <summary>
/// Label um Text fett anzuzeigen (UIStyle.BOLD)
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
public class AFLabelBoldTextRightAligned : AFLabel
{
    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        Style = UIStyle.BOLD;
        Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        Appearance.TextOptions.WordWrap = WordWrap.NoWrap;
        Appearance.TextOptions.Trimming = Trimming.Character;
        Appearance.Options.UseTextOptions = true;
        AutoEllipsis = true;

        base.OnHandleCreated(e);
    }
}

/// <summary>
/// Label um Text fett und als Link anzuzeigen (UIStyle.BOLD)
/// 
/// Fungiert als Link und hat eine ClickAction, die beim Klick ausgeführt wird.
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
public class AFLabelBoldLinkRightAligned : AFLabel
{
    private Action<AFLabel>? _clickAction;

    /// <summary>
    /// Aktion, die beim Klicken ausgeführt werden soll.
    /// 
    /// Wenn !NULL wird das Label beim Hoover mit dem Pointer 'Hand' dargestellt-
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Action<AFLabel>? ClickAction
    {
        get => _clickAction;
        set
        {
            _clickAction = value;

            if (_clickAction != null)
                Cursor = Cursors.Hand;

            Click += (_, _) => { _clickAction?.Invoke(this); };
        }
    }


    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        Style = UIStyle.BOLDLINK;
        Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        Appearance.TextOptions.WordWrap = WordWrap.NoWrap;
        Appearance.TextOptions.Trimming = Trimming.Character;
        Appearance.Options.UseTextOptions = true;
        AutoEllipsis = true;

        base.OnHandleCreated(e);
    }
}

/// <summary>
/// Label um Text fett anzuzeigen mit Linie unter dem Text
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
public class AFLabelBoldLine : AFLabel
{
    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        Style = UIStyle.BOLD;
        LineOrientation = LabelLineOrientation.Horizontal;
        LineLocation = LineLocation.Bottom;
        LineStyle = DashStyle.Solid;
        LineVisible = true;
        LineColor = UI.TranslateSystemToSkinColor(SystemColors.GrayText);
        Padding = new(0, 0, 0, 3);

        base.OnHandleCreated(e);
    }
}

/// <summary>
/// Label zur Anzeige boolscher Werte
/// </summary>
[DefaultBindingProperty("EditValue")]
public class AFLabelBoldBool : AFLabelBoldText
{
    private string stringYes = "Ja";
    private string stringNo = "Nein";
    private bool editValue;


    /// <summary>
    /// Text, der bei true angezeigt werden soll
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string StringYes { get => stringYes; set => stringYes = value; }

    /// <summary>
    /// Text, der bei false angezeigt werden soll
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string StringNo { get => stringNo; set => stringNo = value; }


    /// <summary>
    /// Text, der bei true angezeigt werden soll setzen
    /// </summary>
    public AFLabelBoldBool SetStringYes(string val)
    {
        stringYes = val;

        return this;
    }

    /// <summary>
    /// Text, der bei true angezeigt werden soll setzen
    /// </summary>
    public AFLabelBoldBool SetStringNo(string val)
    {
        stringNo = val;

        return this;
    }

    /// <summary>
    /// Boolscher Wert
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool EditValue
    {
        get => editValue;
        set
        {
            editValue = value;
            Text = editValue ? stringYes : stringNo;
        }
    }
}