using DevExpress.Utils.VisualEffects;
using DevExpress.XtraEditors;
using System.ComponentModel.Design;

namespace AF.WINFORMS.DX;

/// <summary>
/// das aktive Control automatisch durch einen andersfarbigen Rahmen hervorheben
/// Es reicht aus, die Komponente auf ein Form oder UserControl zu ziehen um diese Funktion zu unterstützen.
/// </summary>
[ToolboxItem(true)]
[SupportedOSPlatform("windows")]
[DesignerCategory("Code")]
public class AFControlHighlighter : AdornerUIManager
{
    /// <summary>
    /// Control 
    /// </summary>
    [Browsable(false)]
    [DefaultValue(null)]
    public ContainerControl? ContainerControl { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override ISite? Site
    {
        get => base.Site;
        set
        {
            base.Site = value;

            IDesignerHost? host = (IDesignerHost?)value?.GetService(typeof(IDesignerHost));

            if (host == null) return;

            IComponent componentHost = host.RootComponent;

            if (componentHost is ContainerControl control)
                ContainerControl = control;
        }
    }

    /// <summary>
    /// Rahmenfarbe für Editoren wenn focused.
    /// </summary>
    [Browsable(true), Description("Rahmenfarbe für Editoren wenn focused.")]
    [DefaultValue(typeof(Color), "#0000FF")]
    public Color HighlightBorderColor { get; set; } = Color.Blue;

    /// <summary>
    /// Rahmenfarbe für Editoren wenn focused und readonly.
    /// </summary>
    [Browsable(true), Description("Rahmenfarbe für Editoren wenn focused und readonly.")]
    [DefaultValue(typeof(Color), "#B22222")]
    public Color ReadonlyBorderColor { get; set; } = Color.Firebrick;

    /// <summary>
    /// Hintergrundfarbe für Editoren wenn focused.
    /// </summary>
    [Browsable(true), Description("Hintergrundfarbe für Editoren wenn focused.")]
    [DefaultValue(typeof(Color), "#EEE8AA")]
    public Color HighlightBackColor { get; set; } = Color.PaleGoldenrod;

    /// <summary>
    /// Vordergrundfarbe für Editoren wenn focused.
    /// </summary>
    [Browsable(true), Description("Vordergrundfarbe für Editoren wenn focused.")]
    [DefaultValue(typeof(Color), "#000000")]
    public Color HighlightForeColor { get; set; } = Color.Black;

    /// <summary>
    /// Hintergrundfarbe für Editoren wenn readonly.
    /// </summary>
    [Browsable(true), Description("Hintergrundfarbe für Editoren wenn readonly.")]
    [DefaultValue(typeof(Color), "#DB7093")]
    public Color HighlightBackColorReadonly { get; set; } = Color.PaleVioletRed;

    /// <summary>
    /// Vordergrundfarbe für Editoren wenn readonly.
    /// </summary>
    [Browsable(true), Description("Vordergrundfarbe für Editoren wenn readonly.")]
    [DefaultValue(typeof(Color), "#000000")]
    public Color HighlightForeColorReadonly { get; set; } = Color.Black;

    /// <summary>
    /// Hintergrundfarbe anpassen, wenn ein Editor den Focus bekommt.
    /// </summary>
    [Browsable(true), Description("Hintergrundfarbe anpassen, wenn ein Editor den Focus bekommt.")]
    [DefaultValue(false)]
    public bool UseBackgroundHighlighting { get; set; } = false;

    /// <summary>
    /// Rahmenfarben an den aktuellen Skin anpassen.
    /// </summary>
    [Browsable(true), Description("Rahmenfarben an den aktuellen Skin anpassen.")]
    [DefaultValue(false)]
    public bool UseAutoBorderColor { get; set; } = false;

    /// <summary>
    /// Rahmen von ReadOnly Controls hervorheben.
    /// </summary>
    [Browsable(true), Description("Rahmen von ReadOnly Controls hervorheben.")]
    [DefaultValue(false)]
    public bool UseReadonlyBorderColor { get; set; } = false;

    /// <summary>
    /// Alle Controls registrieren
    /// </summary>
    public void Register()
    {
        if (ContainerControl == null)
            return;

        foreach (Control ctrl in ContainerControl.Controls)
            _subscribe(ctrl, true);
    }

    /// <summary>
    /// Registrierung aller Controls aufheben
    /// </summary>
    public void UnRegister()
    {
        if (ContainerControl == null)
            return;

        foreach (Control ctrl in ContainerControl.Controls)
            _subscribe(ctrl, false);
    }

    /// <summary>
    /// Methode die nach dem Laden ausgeführt wird.
    /// </summary>
    protected override void OnLoad()
    {
        base.OnLoad();

        _styleChanged(null, EventArgs.Empty);

        DevExpress.LookAndFeel.UserLookAndFeel.Default.StyleChanged += _styleChanged;

        Register();
    }

    private void _styleChanged(object? sender, EventArgs e)
    {
        if (UseAutoBorderColor)
            HighlightBorderColor = UI.TranslateSystemToSkinColor(SystemColors.Highlight);
    }

    /// <summary>
    /// Methode die vor dem Entladen ausgeführt wird.
    /// </summary>
    protected override void OnUnload()
    {
        UnRegister();

        DevExpress.LookAndFeel.UserLookAndFeel.Default.StyleChanged -= _styleChanged;

        base.OnUnload();
    }

    private void _subscribe(Control target, bool subscribe)
    {
        if (target.Controls is { Count: > 0 })
        {
            foreach (Control control in target.Controls)
                _subscribe(control, subscribe);
        }
        else
        {
            if (subscribe)
            {
                if (target is BaseEdit edit)
                {
                    target.Enter += _onEnter;

                    if (UseBackgroundHighlighting)
                    {
                        if (edit.ReadOnly)
                        {
                            edit.Properties.AppearanceFocused.BackColor = HighlightBackColorReadonly;
                            edit.Properties.AppearanceFocused.ForeColor = HighlightForeColorReadonly;
                        }
                        else
                        {
                            edit.Properties.AppearanceFocused.BackColor = HighlightBackColor;
                            edit.Properties.AppearanceFocused.ForeColor = HighlightForeColor;
                        }
                        edit.Properties.AppearanceFocused.Options.UseBackColor = true;
                        edit.Properties.AppearanceFocused.Options.UseForeColor = true;
                    }
                }
            }
            else
            {
                if (target is BaseEdit edit)
                {
                    target.Enter -= _onEnter;
                }
            }
        }
    }

    private void _onEnter(object? sender, EventArgs e)
    {
        Elements.Clear();

        if (sender is BaseEdit edit)
        {
            if (UseBackgroundHighlighting)
            {
                edit.Properties.AppearanceFocused.BackColor = edit.Properties.ReadOnly 
                    ? HighlightBackColorReadonly
                    : HighlightBackColor;
                edit.Properties.AppearanceFocused.ForeColor = edit.Properties.ReadOnly 
                    ? HighlightForeColorReadonly
                    : HighlightForeColor;
            }

            ValidationHint hint = new();

            hint.Appearances.IndeterminateState.BorderColor = edit.Properties.ReadOnly && UseReadonlyBorderColor
                ? ReadonlyBorderColor
                : HighlightBorderColor;
            hint.Appearances.IndeterminateState.Options.UseBackColor = true;
            hint.Appearances.IndeterminateState.Options.UseBorderColor = true;
            hint.Properties.IndeterminateState.ShowBorder = DevExpress.Utils.DefaultBoolean.True;
            hint.Properties.IndeterminateState.ShowHint = DevExpress.Utils.DefaultBoolean.True;
            hint.Properties.IndeterminateState.ShowIcon = DevExpress.Utils.DefaultBoolean.True;
            hint.Properties.State = ValidationHintState.Indeterminate;
            hint.TargetElement = edit;
            Elements.Add(hint);
        }
    }
}

