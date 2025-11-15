namespace AF.MVC;

/// <summary>
/// DetailView, dass einen LibraryDocumentDesigner anzeigt (Dokumentvorschau)
/// </summary>
[ToolboxItem(false)]
[SupportedOSPlatform("windows")]
[DesignerCategory("Code")]
public class AFDetailViewLibraryDocumentDesigner : AFDetailView, IViewDetail
{
    private readonly AFFilePreview designer = null!;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFDetailViewLibraryDocumentDesigner(IViewPage page) : base(page)
    {
        if (UI.DesignMode) return;

        AFSkinnedPanel panel = new() { Dock = DockStyle.Fill };

        designer = new() { Dock = DockStyle.Fill };
        panel.Controls.Add(designer);

        Controls.Add(panel);

        designer.SetEditor(eLibraryDocumentType.None);
    }

    /// <summary>
    /// Zugriff auf den Designer
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFFilePreview Designer => designer;

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
    /// Script, der aktuell im Designer bearbeitet wird.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ILibraryDocument? Template
    {
        get => designer.LibraryDocument;
        set => designer.LibraryDocument = value;
    }

    ///// <summary>
    ///// Validiert das DetailView und liefert true, wenn valide (Standard).
    ///// Wenn nicht valide, enthält errors die aufgetretenen Fehler.
    /////
    ///// Hier werden die Daten des Designers in den Query übertragen.
    ///// </summary>
    ///// <param name="errors">Liste, die die Fehler aufnimmt.</param>
    ///// <returns>true, wenn valide - sonst false</returns>
    //public override bool IsValid(ValidationErrorCollection errors)
    //{
    //    return base.IsValid(errors) && designer.IsValid(errors);
    //}

    //public void SetEditor(eDocumentType doctype)
    //{
    //   designer.SetEditor(doctype);
    //}
}