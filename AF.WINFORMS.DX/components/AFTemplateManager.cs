namespace AF.WINFORMS.DX;

/// <summary>
/// Manager für Templates (Grid-Ansichten, PivotTabellen-Konfigurationen etc.)
/// </summary>
public partial class AFTemplateManager : AFUserControl
{
    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFTemplateManager()
    {
        InitializeComponent();

        if (UI.DesignMode) return;

        // Voraussetzungen prüfen...
        if (AFCore.App.Persistance == null)
            throw new Exception("App has not a persistence storage (AFCore.App.Persistance). Templates can not be used.");

    }

    /// <summary>
    /// Control, dessen Templates verwaltet werden sollen
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ITemplateControl? TemplateControl { get; set; }

    /// <summary>
    /// Templates aller Benutzer anzeigen/verwalten
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AllUserTemplates { get; set; } = false;

    /// <summary>
    /// Anzuzeigende/zu bearbeitende Templates
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public BindingList<PersistantModel> Templates {  get; set; } = [];

    /// <summary>
    /// Templates laden.
    /// </summary>
    public void LoadTemplates()
    {
        if (TemplateControl == null) return;

        // AFCore.App.Persistance
    }

}

/// <summary>
/// Interface, dass ein Steuerelelemnt implementieren muss um Templates zu unterstützen
/// </summary>
public interface ITemplateControl
{
    /// <summary>
    /// Template speichern
    /// </summary>
    /// <param name="template">Model, in dem das Template abgelegt werden muss</param>
    public void SaveTo(PersistantModel template);

    /// <summary>
    /// Template laden
    /// </summary>
    /// <param name="template">Model, aus dem das Template geladen werden soll</param>
    public void LoadFrom(PersistantModel template);

    /// <summary>
    /// ID des Controls, damit die Templates zugeordnet werden können (eindeutige ID des GridViews etc.)
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Guid TemplateID { get; }
}
