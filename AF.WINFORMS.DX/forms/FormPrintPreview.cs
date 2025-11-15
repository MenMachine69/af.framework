using DevExpress.XtraPrinting;

namespace AF.WINFORMS.DX;

/// <summary>
/// Klassicher Druck-Vorschaudialog ohne Export und EMail
/// </summary>
public partial class FormPrintPreview : FormBase
{
    readonly PrintableComponentLink? plink;

    /// <summary>
    /// Constructor
    /// </summary>
    public FormPrintPreview()
    {
        InitializeComponent();

        if (UI.DesignMode) return;

        components ??= new Container();

        StartPosition = FormStartPosition.CenterParent;
        documentViewer1.PrintingSystem = new PrintingSystem(components);
        plink = new((PrintingSystem)documentViewer1.PrintingSystem);

    }

    /// <inheritdoc />
    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);

        if (UI.DesignMode) return;

        UI.HideWait();
    }

    /// <summary>
    /// Das anzuzeigende Dokument zuweisen.
    /// </summary>
    /// <param name="source"></param>
    public void SetDocument(object source)
    {
        plink!.Component = source as IPrintable;
        plink.CreateDocument();
        documentViewer1.DocumentSource = plink;
    }


    /// <summary>
    /// Icon des Forms setzen
    /// </summary>
    /// <param name="icon">Icon</param>
    public void SetSymbol(Icon icon)
    {
        IconOptions.Icon = icon;
    }
}