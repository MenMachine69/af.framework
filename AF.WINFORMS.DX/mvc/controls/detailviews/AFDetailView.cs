namespace AF.MVC;

/// <summary>
/// Basisklasse der Controls, die als DetailView's angezeigt werden können.
/// </summary>
[ToolboxItem(false)]
[SupportedOSPlatform("windows")]
[DesignerCategory("Code")]
public class AFDetailView : AFUserControl
{
    private IContainer? components;

    /// <summary>
    /// Constructor (für Designer)
    /// </summary>
    public AFDetailView() : base()
    {

    }

    /// <summary>
    /// Constructor
    /// </summary>
    public AFDetailView(IViewPage page) : base()
    {
        if (UI.DesignMode) return;

        components = new Container();
        Page = page;
    }

    /// <summary>
    /// Zugriff auf den IContainer des Controls (zur Registrierung von Komponenten)
    /// </summary>
    public IContainer? ComponentsContainer => components;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();

        base.Dispose(disposing);
    }


    /// <summary>
    /// Die Seite, die diese DetailView enthält
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IViewPage Page { get; set; } = null!;


    /// <summary>
    /// Plugin zur Anzeige des ausgewählten Details.
    /// 
    /// Wird die Auswahl verändert, wird das Detail informiert.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IViewModelDetail? PluginDetail { get; set; } = null;

    /// <summary>
    /// Validiert das DetailView und liefert true, wenn valide (Standard).
    /// Wenn nicht valide, enthält errors die aufgetretenen Fehler.
    ///
    /// Kann in konkreterem DetailView überschrieben werden um z.B. Details eines
    /// Masters zu validieren und/oder in den Master zu übertragen.
    /// </summary>
    /// <param name="errors">Liste, die die Fehler aufnimmt.</param>
    /// <returns>true, wenn valide - sonst false</returns>
    public virtual bool IsValid(ValidationErrorCollection errors) { return true; }
}