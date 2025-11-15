using AF.BUSINESS;

namespace AF.MVC;

/// <summary>
/// DetailView, dass einen DocumentDesigner anzeigt
/// </summary>
[ToolboxItem(false)]
[SupportedOSPlatform("windows")]
[DesignerCategory("Code")]
public class AFDetailViewDocumentDesigner : AFDetailView, IViewDetail
{
    private readonly AFDocumentDesigner designer = null!;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFDetailViewDocumentDesigner(IViewPage page) : base(page)
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
    public AFDocumentDesigner Designer => designer;

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
    public IDokumentvorlage? Template
    {
        get => designer.Template;
        set => designer.Template = value;
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

    /// <summary>
    /// Passenden Editor für den Dokumenttyp setzen.
    /// </summary>
    /// <param name="doctype"></param>
    public void SetEditor(eDocumentType doctype)
    {
       designer.SetEditor(doctype);
    }
}