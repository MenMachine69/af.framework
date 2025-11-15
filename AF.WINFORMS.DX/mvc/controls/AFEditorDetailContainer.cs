using AF.MVC;

namespace AF.WINFORMS.DX;

/// <summary>
/// Container für AFEditorDetail-Controls (ScrollPanel)
/// </summary>
[DesignerCategory("Code")]
public class AFEditorDetailContainer : AFScrollablePanel
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="horizontal">Elemente Horizontal anordnen</param>
    public AFEditorDetailContainer(bool horizontal = false)
    {
        Horizontal = horizontal;
    }


    /// <summary>
    /// Elemente/Details horizontal anzeigen (links nach rechts)
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Horizontal { get; init; }

    /// <summary>
    /// Alle AFEditorDetail-Controls entfernen
    /// </summary>
    public void Clear()
    {
        foreach (var ctrl in Controls)
        {
            if (ctrl is not AFEditorDetail detail) continue;

            detail.ElementContainer = null;
        }

        Controls.Clear(true);
    }

    /// <summary>
    /// Fügt dem Container ein neues Detail hinzu.
    /// </summary>
    /// <typeparam name="T">Typ des Details</typeparam>
    /// <param name="second">In die zweite Spalte/Zeile hinzufügen</param>
    /// <returns>das neu erzeugte und hinzugefügte Detail</returns>
    public T Add<T>(bool second = false) where T : AFEditorDetail, new()
    {
        T ret = new() { Dock = Horizontal ? DockStyle.Left : DockStyle.Top };
        ret.ElementContainer = this;

        ret.BeforeShow();

        ret.Size = new(Horizontal ? ret.DefaultEditorSize : ClientRectangle.Width, Horizontal ? ClientRectangle.Height : ret.DefaultEditorSize);

        Controls.Add(ret);
        ret.BringToFront();

        ret.AfterShow();

        return ret;
    }

    /// <summary>
    /// Fügt dem Container ein neues Detail hinzu.
    /// </summary>
    /// <param name="detail">Hinzuzufügendes Detail</param>
    /// <param name="second">In die zweite Spalte/Zeile hinzufügen</param>
    /// <returns>das neu erzeugte und hinzugefügte Detail</returns>
    public AFEditorDetail Add(AFEditorDetail detail, bool second = false)
    {
        detail.Dock = Horizontal ? DockStyle.Left : DockStyle.Top;
        detail.ElementContainer = this;

        detail.BeforeShow();

        detail.Size = new(Horizontal ? detail.DefaultEditorSize : ClientRectangle.Width, Horizontal ? ClientRectangle.Height : detail.DefaultEditorSize);

        Controls.Add(detail);

        detail.BringToFront();

        detail.AfterShow();

        return detail;
    }

    /// <summary>
    /// Wird vom Editor aufgerufen, der die Details anzeigt.
    ///
    /// Übergeben wird das aktuelle Model des Editors. Das Detail kann damit die angezeigten
    /// Informationen, wenn notwendig aktualisieren.
    /// </summary>
    public virtual void Update(IModel model)
    {
        foreach (AFEditorDetail detail in Controls)
            detail.Update(model);
    }


    
}