namespace AF.WINFORMS.DX;

/// <summary>
/// Basisklasse der Presenter, die z.B. in Dashboards zur Darstellung von Werten/Informationen verwendet werden können.
/// </summary>
[DesignerCategory("Code")]
public class AFPresenterBase : AFUserControl
{
    private readonly bool lightbackground = false;

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFPresenterBase(bool lightBackground)
    {
        lightbackground = lightBackground;

        if (!lightBackground) return;

        Appearance.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Window);
        Appearance.Options.UseBackColor = true;
    }

    /// <summary>
    /// Aktion/Delegate der zum Refresh der Daten ausgeführt werden soll.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Action<Control>? RefreshContent { get; set; }


    /// <summary>
    /// Aktion/Delegate der zum Setup des Elements ausgeführt werden soll.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Action<Control>? SetupElement { get; set; }
       
    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (UI.DesignMode || !lightbackground) return;
        

        UI.StyleChanged += styleChanged;
    }

    /// <inheritdoc />
    protected override void OnHandleDestroyed(EventArgs e)
    {
        base.OnHandleDestroyed(e);

        if (UI.DesignMode || !lightbackground) return;

        UI.StyleChanged -= styleChanged;
    }

    private void styleChanged(object? sender, EventArgs e)
    {
        Appearance.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Window);
    }
}