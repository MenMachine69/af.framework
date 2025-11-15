using AF.MVC;
using DevExpress.Utils.Svg;
using DevExpress.XtraBars;

namespace AF.WINFORMS.DX;

/// <summary>
/// Basis-Klasse der Details für einen AFEditorDetailContainer
/// </summary>
public class AFEditorDetail : AFUserControl
{
    /// <summary>
    /// Container, der das Element anzeigt
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFEditorDetailContainer? ElementContainer { get; set; }

    /// <summary>
    /// Wird aufgerufen bevor das Control angezeigt wird (Owner ist noch nicht gesetzt!)
    /// </summary>
    public virtual void BeforeShow() { }

    /// <summary>
    /// Wird aufgerufen nachdem das Control angezeigt wird (Owner ist gesetzt!)
    /// </summary>
    public virtual void AfterShow() { }

    /// <summary>
    /// Wird vom Editor aufgerufen, der das Detail anzeigt.
    ///
    /// Übergeben wird das aktuelle Model des Editors. Das Detail kann damit die angezeigten
    /// Informationen, wenn notwendig aktualisieren.
    /// </summary>
    public virtual void Update(IModel model) { }

    /// <summary>
    /// ID des Elements
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Guid ElementID { get; set; } = Guid.Empty;

    /// <summary>
    /// Controller, der das Element zur Verfügung stellt.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IControllerUI? Controller { get; set; }

    /// <summary>
    /// Standardgröße des Details.
    ///
    /// Horizontal: Breite
    /// Vertikal: Höhe
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int DefaultEditorSize { get; set; } = 60;

    /// <summary>
    /// Standardgröße des Details.
    ///
    /// Horizontal: Breite
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int DefaultEditorWidth { get; set; } = 60;

    /// <summary>
    /// Standardgröße des Details.
    ///
    /// Vertical: Höhe
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int DefaultEditorHeight { get; set; } = 60;


    /// <summary>
    /// Methode, die ausgelöst wird, wenn ein Button geklickt wurde.
    /// </summary>
    /// <param name="sender">Absender</param>
    /// <param name="e">Parameter</param>
    public virtual void BtnClick(object sender, ItemClickEventArgs e)
    {
        if (e.Item.Name == "pshEdit")
            OnEdit();
    }

    /// <summary>
    /// Editieren des Elements...
    /// </summary>
    public virtual void OnEdit() { }

    /// <summary>
    /// Der Toolbar einen Text hinzufügen
    /// </summary>
    /// <param name="tbar">Toolbar</param>
    /// <param name="name">Name des Elements</param>
    /// <param name="caption">Text</param>
    /// <returns>das hinzugefügte Element</returns>
    public BarStaticItem AddText(Bar tbar, string name, string caption)
    {
        var lbl = tbar.AddLabel(name, caption: caption);
        return lbl;
    }

    /// <summary>
    /// Einen Button hinzufügen (z.B. Edit)
    /// </summary>
    /// <param name="tbar">Toolbar</param>
    /// <param name="name">Name des Buttons</param>
    /// <param name="img">Symbol</param>
    /// <param name="caption">Text</param>
    /// <param name="description">Beschreibung</param>
    /// <param name="group">Gruppe beginnen</param>
    /// <param name="right">rechts anordnen</param>
    /// <returns>hinzugefügter Button</returns>
    public BarButtonItem AddButton(Bar tbar, string name, SvgImage img, string caption, string description, bool group = false, bool? right = null)
    {
        var btn = tbar.AddButton(name, img: img, begingroup: group, rightalign: right);
        btn.ItemClick += BtnClick;
        btn.SuperTip = UI.GetSuperTip(caption, description);
        return btn;
    }

    /// <summary>
    /// Ein Menü hinzufügen (z.B. More)
    /// </summary>
    /// <param name="tbar">Toolbar</param>
    /// <param name="name">Name des Menüs</param>
    /// <param name="img">Symbol</param>
    /// <param name="caption">Text</param>
    /// <param name="description">Beschreibung</param>
    /// <param name="group">Gruppe beginnen</param>
    /// <param name="right">rechts anordnen</param>
    /// <returns>hinzugefügter Button</returns>
    public BarSubItem AddMenu(Bar tbar, string name, SvgImage img, string caption, string description, bool group = false, bool? right = null)
    {
        var btn = tbar.AddMenu(name, img: img, begingroup: group, rightalign: right);
        btn.ItemClick += BtnClick;
        btn.SuperTip = UI.GetSuperTip(caption, description);
        return btn;
    }

}
