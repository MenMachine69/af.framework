namespace AF.MVC;

/// <summary>
/// DetailView, dass einen QueryDesigner anzeigt
/// </summary>
[ToolboxItem(false)]
[SupportedOSPlatform("windows")]
[DesignerCategory("Code")]
public class AFDetailViewQueryDesigner : AFDetailView, IViewDetail
{
    private readonly AFQueryDesigner designer = null!;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFDetailViewQueryDesigner(IViewPage page) : base(page)
    {
        if (UI.DesignMode) return;

        AFSkinnedPanel panel = new() { Dock = DockStyle.Fill };

        designer = new() { Dock = DockStyle.Fill };
        panel.Controls.Add(designer);

        Controls.Add(panel);
    }

    /// <summary>
    /// Zugriff auf den Designer
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFQueryDesigner Designer => designer;

    /// <summary>
    /// Models im DetailView.
    ///
    /// Im QueryDesigner nicht verwendet!
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IBindingList? Models { get; set; }


    /// <summary>
    /// ausgewählte Models im DetailView.
    ///
    /// Im QueryDesigner nicht verwendet!
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IEnumerable<IModel>? SelectedModels => null;

    /// <summary>
    /// aktuelles Model im DetailView.
    ///
    /// Im QueryDesigner nicht verwendet!
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IModel? CurrentModel => null;
    
    /// <summary>
    /// Query, die aktuell im Designer bearbeitet wird.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IQuery? Query
    {
        get => designer.Query;
        set => designer.Query = value;
    }

    /// <summary>
    /// Validiert das DetailView und liefert true, wenn valide (Standard).
    /// Wenn nicht valide, enthält errors die aufgetretenen Fehler.
    ///
    /// Hier werden die Daten des Designers in den Query übertragen.
    /// </summary>
    /// <param name="errors">Liste, die die Fehler aufnimmt.</param>
    /// <returns>true, wenn valide - sonst false</returns>
    public override bool IsValid(ValidationErrorCollection errors)
    {
        return base.IsValid(errors) && designer.IsValid(errors);
    }
}