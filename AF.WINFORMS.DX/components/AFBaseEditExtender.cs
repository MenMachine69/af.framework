using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit;

namespace AF.WINFORMS.DX;

/// <summary>
/// Erweitert alle von XtraEditors.BaseEdit abgeleiteten Steuerelemente um die Eigenschaften
/// AutoFlat = Rahmen um nicht aktive Steuerelemente ausblenden
/// AutoRightAlign = Text des nicht aktiven Steuerelements rechtsbündig anzeigen
/// AutoBold = Text des nicht-aktiven Steuerelements fett darstellen
/// UseDefaultStyle = der Stil-Controller des Steuerelements entspricht UI.DefaultStyleController
/// </summary>
[ProvideProperty("AutoFlat", typeof(BaseEdit))]
[ProvideProperty("AutoRightAlign", typeof(BaseEdit))]
[ProvideProperty("AutoBold", typeof(BaseEdit))]
[ProvideProperty("UseDefaultStyle", typeof(BaseEdit))]
[ProvideProperty("ConnectedLabel", typeof(BaseEdit))]
[ProvideProperty("AutoFlat", typeof(RichEditControl))]
[ProvideProperty("ConnectedLabel", typeof(RichEditControl))]
[DesignerCategory("Code")]
public class AFBaseEditExtender : Component, IExtenderProvider, ISupportInitialize
{
    private readonly Dictionary<object, AutoFlatSettings> _settings;
    private readonly Dictionary<Control, bool> _defstyle;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFBaseEditExtender()
    {
        _settings = new();
        _defstyle = new();
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="parent">Container</param>
    public AFBaseEditExtender(IContainer parent) : this()
    {
        parent.Add(this);
    }

    /// <summary>
    /// Prüft, ob das übergebene Control/Objekt erweitert werden kann
    /// </summary>
    /// <param name="extendee"></param>
    /// <returns></returns>
    public bool CanExtend(object extendee)
    {
        return extendee is BaseEdit || extendee is RichEditControl;
    }

    private void _ensureExists(object key)
    {
        if (!_settings.ContainsKey(key))
            _settings[key] = new();
    }

    void ISupportInitialize.BeginInit() { }

    void ISupportInitialize.EndInit()
    {
        if (AFCore.DesignMode) return;

        if (_defstyle.Count > 0)
        {
            foreach (var ctrl in _defstyle)
            {
                if (ctrl.Key is BaseEdit edit && ctrl.Value)
                    edit.StyleController = UI.DefaultStyleController;
            }
        }

        _defstyle.Clear();

        foreach (var ctrl in _settings)
        {
            ((Control)ctrl.Key).Enter += _Enter;
            ((Control)ctrl.Key).Leave += _Leave;

            if (ctrl.Value.AutoFlat)
                _assignStyle((Control)ctrl.Key, true);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="extendee"></param>
    /// <param name="value"></param>
    public void SetUseDefaultStyle(Control extendee, bool value)
    {
        _defstyle[extendee] = value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="extendee"></param>
    /// <returns></returns>
    [DisplayName("UseDefaultStyle")]
    [DefaultValue(false)]
    [Category("UI Settings")]
    [ExtenderProvidedProperty()]
    public bool GetUseDefaultStyle(Control extendee)
    {
        _defstyle.TryAdd(extendee, false);

        return _defstyle[extendee];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="extendee"></param>
    /// <param name="value"></param>
    public void SetAutoFlat(Control extendee, bool value)
    {
        _ensureExists(extendee);
        _settings[extendee].AutoFlat = value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="extendee"></param>
    /// <returns></returns>
    [DisplayName("AutoFlat")]
    [DefaultValue(false)]
    [Category("UI Settings")]
    [ExtenderProvidedProperty()]
    public bool GetAutoFlat(Control extendee)
    {
        _ensureExists(extendee);

        return _settings[extendee].AutoFlat;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="extendee"></param>
    /// <param name="value"></param>
    public void SetAutoRightAlign(Control extendee, bool value)
    {
        _ensureExists(extendee);
        _settings[extendee].AutoRightAlign = value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="extendee"></param>
    /// <returns></returns>
    [DisplayName("AutoRightAlign")]
    [DefaultValue(false)]
    [Category("UI Settings")]
    [ExtenderProvidedProperty()]
    public bool GetAutoRightAlign(Control extendee)
    {
        _ensureExists(extendee);

        return _settings[extendee].AutoRightAlign;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="extendee"></param>
    /// <param name="value"></param>
    public void SetAutoBold(Control extendee, bool value)
    {
        _ensureExists(extendee);
        _settings[extendee].AutoBold = value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="extendee"></param>
    /// <returns></returns>
    [DisplayName("AutoBold")]
    [DefaultValue(false)]
    [Category("UI Settings")]
    [ExtenderProvidedProperty()]
    public bool GetAutoBold(Control extendee)
    {
        _ensureExists(extendee);

        return _settings[extendee].AutoBold;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="extendee"></param>
    /// <param name="value"></param>
    public void SetConnectedLabel(Control extendee, LabelControl value)
    {
        _ensureExists(extendee);
        _settings[extendee].ConnectedLabel = value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="extendee"></param>
    /// <returns></returns>
    [DisplayName("ConnectedLabel")]
    [DefaultValue(null)]
    [Category("UI Settings")]
    [ExtenderProvidedProperty()]
    public LabelControl? GetConnectedLabel(Control extendee)
    {
        _ensureExists(extendee);

        return _settings[extendee].ConnectedLabel;
    }


    private void _Leave(object? sender, EventArgs e)
    {
        if (sender is Control control)
            _assignStyle(control, true);
    }

    private void _Enter(object? sender, EventArgs e)
    {
        if (sender is Control control)
            _assignStyle(control, false);


    }

    /// <summary>
    /// FlatStyle für das Control ein-/ausschalten
    /// </summary>
    /// <param name="ctrl"></param>
    /// <param name="setFlat"></param>
    private void _assignStyle(Control ctrl, bool setFlat)
    {
        bool autoFlat = false;
        bool rightAlign = false;
        bool bold = false;
        LabelControl? label = null;

        if (_settings.ContainsKey(ctrl))
        {
            rightAlign = _settings[ctrl].AutoRightAlign;
            bold = _settings[ctrl].AutoBold;
            label = _settings[ctrl].ConnectedLabel;
            autoFlat = _settings[ctrl].AutoFlat;
        }

        if (setFlat)
        {
            if (label != null)
            {
                label.Appearance.FontStyleDelta = FontStyle.Regular;
                label.Appearance.Options.UseFont = true;
            }

            if (!autoFlat) return;

            if (ctrl is BaseEdit edit)
            {
                edit.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                edit.Margin = new Padding(4);
                edit.Properties.Appearance.TextOptions.HAlignment = rightAlign
                    ? DevExpress.Utils.HorzAlignment.Far
                    : DevExpress.Utils.HorzAlignment.Default;
                edit.Properties.Appearance.FontStyleDelta =
                    bold ? FontStyle.Bold : FontStyle.Regular;
            }
            else if (ctrl is RichEditControl control)
            {
                control.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                control.Margin = new Padding(4);
            }
        }
        else
        {
            if (label != null)
            {
                label.Appearance.FontStyleDelta = FontStyle.Bold;
                label.Appearance.Options.UseFont = true;
            }

            if (!autoFlat) return;

            if (ctrl is BaseEdit edit)
            {
                edit.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
                edit.Margin = new Padding(3);
                edit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
                edit.Properties.Appearance.FontStyleDelta = FontStyle.Regular;
            }
            else if (ctrl is RichEditControl control)
            {
                control.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
                control.Margin = new Padding(3);
            }
        }
    }
}