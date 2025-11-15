namespace AF.WINFORMS.DX;

/// <summary>
/// Popupfenster der AppBar
/// </summary>
public class AppBarPopup : FormBase
{
    private System.Windows.Forms.Timer? _closeTimer;

    /// <summary>
    /// AppBar, zu der das Popup gehört...
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public FormAppBar? AppBar { get; set; }

    /// <summary>
    /// Breite an Bildschirm anpassen
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AutoWidth { get; set; } = true;

    /// <summary>
    /// Ausrichtung zum übergeordneten Label
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public eAlignmentHorizontal Alignment { get; set; } = eAlignmentHorizontal.Far;



    /// <summary>
    /// Wird nach dem Anzeigen ausgeführt
    /// </summary>
    public virtual void AfterShow() {  }

    /// <summary>
    /// Wird vor dem Anzeigen ausgeführt
    /// </summary>
    public virtual void BeforeShow() { }

    /// <summary>
    /// Wird nach dem Erzeugen ausgeführt
    /// </summary>
    public virtual void AfterCreate() {  }

    /// <summary>
    /// 
    /// </summary>
    public new void Hide()
    {
        _closeTimer?.Stop();

        base.Hide();
    }
    
    /// <summary>
    /// Popup anzeigen
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="menuentry">Menüeintrag zu dem das Popup gehört</param>
    public void ShowPopup(FormAppBar owner, AFLabel menuentry)
    {
        AppBar = owner;

        _closeTimer ??= new() { Interval = 5000 };

        _closeTimer.Tick += (_, _) =>
        {
            _closeTimer?.Stop();

            if (ClientRectangle.Contains(PointToClient(Cursor.Position)))
                _closeTimer?.Start();
            else
                AppBar?.HidePopup();
        };

        _closeTimer?.Start();

        BeforeShow();

        TopMost = true;
        StartPosition = FormStartPosition.Manual;
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;

        if (AutoWidth)
        {
            Location = new(owner.Left, owner.Bottom);
            Size = new(owner.Width, Height);
        }
        else
            Location = new(Alignment == eAlignmentHorizontal.Far ?  PointToScreen(menuentry.Location).X + menuentry.Width - Width + 10 : owner.Left + menuentry.Left , owner.Bottom);

        Show(owner);

        AfterShow();
    }
}