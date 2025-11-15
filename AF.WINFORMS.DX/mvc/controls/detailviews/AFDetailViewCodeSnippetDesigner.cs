namespace AF.MVC;

/// <summary>
/// DetailView, dass einen CodeSnippetDesigner anzeigt
/// </summary>
[ToolboxItem(false)]
[SupportedOSPlatform("windows")]
[DesignerCategory("Code")]
public class AFDetailViewCodeSnippetDesigner : AFDetailView, IViewDetail
{
    private readonly AFScriptDesigner designer = null!;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFDetailViewCodeSnippetDesigner(IViewPage page) : base(page)
    {
        if (UI.DesignMode) return;

        AFSkinnedPanel panel = new() { Dock = DockStyle.Fill };

        designer = new(snippetMode: true) { Dock = DockStyle.Fill };
        panel.Controls.Add(designer);

        Controls.Add(panel);
    }

    /// <summary>
    /// Zugriff auf den Designer
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFScriptDesigner Designer => designer;

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
    /// IScriptSnippet, der aktuell im Designer bearbeitet wird.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IScriptSnippet? Script
    {
        get => designer.Snippet;
        set
        {
            designer.Snippet = value;



            // TODO: Assemblies aus dem Script extrahieren und registrieren...
        }
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