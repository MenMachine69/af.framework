using System.Text.Json.Serialization;

namespace AF.MVC;

/// <summary>
/// Schnittstelle eines Managers, der die Ansichten innerhalb einer Anwendung anzeigen kann.
/// 
/// Nach der Erstellung kann dieser Manager die Benutzeroberfläche für ein bestimmtes Modell anzeigen.
/// </summary>
public interface IViewManager
{
    /// <summary>
    /// Lesezeichen, die ein Benutzer zur Anzeige in einer Lesezeichenliste festlegen kann, um 
    /// leichteren Zugriff auf verschiedene Modelle.
    /// </summary>
    BindingList<ModelBookmark> Bookmarks { get; }

    /// <summary>
    /// Ein Verlauf der Ansichten, die ein Benutzer kürzlich geöffnet hat.
    /// </summary>
    BindingList<ModelBookmark> History { get; }

    /// <summary>
    /// Überprüft, ob eine Ansicht für ein Modell (beschrieben durch den angegebenen Link.) 
    /// den angegebenen Link mit dem View Manager geöffnet werden kann.
    /// </summary>
    /// <param name="link">Link, der das Modell beschreibt</param>
    /// <returns>true wenn es geöffnet werden kann, sonst false</returns>
    bool CanOpenPage(ModelLink link);
    
    /// <summary>
    /// Überprüft, ob eine Ansicht geschlossen werden kann. 
    /// </summary>
    /// <param name="view">zu schließende Ansicht</param>
    /// <returns>true, wenn sie geschlossen werden kann, sonst false</returns>
    bool CanClosePage(IViewPage view);

    /// <summary>
    /// Schließen einer Ansicht
    /// </summary>
    /// <param name="view">zu schließende Ansicht</param>
    /// <returns>true, wenn geschlossen, sonst false</returns>
    bool ClosePage(IViewPage view);

    /// <summary>
    /// Prüft, ob die angegebene Seite/Page tatsächlich vorhanden ist.
    /// </summary>
    /// <param name="page">zu prüfende Seite/Page</param>
    bool HasPage(IViewPage page);

    /// <summary>
    /// Eine Page suchen, in der gerade ein Model angezeigt wird, dass dem angegebenen Link entspricht.
    /// </summary>
    /// <param name="link">Beschreibung des Models</param>
    /// <returns>Erste Page, dessen Model der Beschreibung entspricht, wenn keine Page existiert NULL</returns>
    IViewPage? GetPage(ModelLink link);
    
    /// <summary>
    /// Eine Page suchen, in der gerade ein Model angezeigt wird, deren ID deren angegebenen ID entspricht.
    /// </summary>
    /// <param name="modelID">ID des Models</param>
    /// <returns>Erste Page, dessen ModelID der angegebenen ID entspricht, wenn keine Page existiert NULL</returns>
    IViewPage? GetPage(Guid modelID);

    /// <summary>
    /// einen View aktivieren (wenn dieser bereits geöffnet ist)
    /// </summary>
    /// <param name="view">Seite, die in den Vordergrund geholt werden soll</param>
    void ActivatePage(IViewPage view);

    /// <summary>
    /// Öffnen einer Ansicht/Seite für ein Model, bei dem ModelTyp und die ID angegeben sind.
    ///
    /// Falls der Controller nicht mehrere Seiten zulässt, wird eine bestehende
    /// Seite für das Modell fokussiert werden. Andernfalls wird eine zusätzliche Seite erstellt.
    /// 
    /// Achtung: Es wird immer das 'Hauptmodel' geöffnet (der vom Controller gesteuerte Typ). 
    /// Wird also z.B. der Typ eines Views und die ID übergeben, wird in der Regel immer das Objekt geöffnet, 
    /// dass die Tabelle beschreibt.
    /// 
    /// Beispiel: 
    /// Show(typeof(vwUserAuswahl), new Guid("{B504DD36-FB3F-4A8C-8503-7E27D93FF246}"));
    /// 
    /// öffnet die Seite mit dem User-Model, nicht! eine Seite mit dem vwUserAuswahl-Model!
    /// </summary>
    /// <param name="modeltype">Typ des Models</param>
    /// <param name="modelid">ID des Models</param>
    /// <param name="targetPage">zu verwendende Page (Default = null = neue Page erzeugen) </param>
    IViewPage OpenPage(Type modeltype, Guid modelid, IViewPage? targetPage = null);

    /// <summary>
    /// Öffnen einer Ansicht/Seite für ein Model.
    ///
    /// Falls der Controller nicht mehrere Seiten zulässt, wird eine bestehende
    /// Seite für das Modell fokussiert werden. Andernfalls wird eine zusätzliche Seite erstellt.
    /// 
    /// Achtung: Es wird immer das 'Hauptmodel' geöffnet (der vom Controller gesteuerte Typ). 
    /// Wird also z.B. der Typ eines Views und die ID übergeben, wird in der Regel immer das Objekt geöffnet, 
    /// dass die Tabelle beschreibt.
    /// 
    /// Beispiel: 
    /// Show(typeof(vwUserAuswahl), new Guid("{B504DD36-FB3F-4A8C-8503-7E27D93FF246}"));
    /// 
    /// öffnet die Seite mit dem User-Model, nicht! eine Seite mit dem vwUserAuswahl-Model!
    /// </summary>
    /// <param name="model">anzuzeigendes Model</param>
    /// <param name="targetPage">zu verwendende Page (Default = null = neue Page erzeugen) </param>
    IViewPage OpenPage(IModel model, IViewPage? targetPage = null);

    /// <summary>
    /// Öffnen einer Ansicht/Seite für ein Model dessen ModelLink gegeben ist.
    ///
    /// Falls der Controller nicht mehrere Seiten zulässt, wird eine bestehende
    /// Seite für das Modell fokussiert werden. Andernfalls wird eine zusätzliche Seite erstellt.
    /// 
    /// Achtung: Es wird immer das 'Hauptmodel' geöffnet (der vom Controller gesteuerte Typ). 
    /// Wird also z.B. der Typ eines Views und die ID übergeben, wird in der Regel immer das Objekt geöffnet, 
    /// dass die Tabelle beschreibt.
    /// 
    /// Beispiel: 
    /// Show(typeof(vwUserAuswahl), new Guid("{B504DD36-FB3F-4A8C-8503-7E27D93FF246}"));
    /// 
    /// öffnet die Seite mit dem User-Model, nicht! eine Seite mit dem vwUserAuswahl-Model!
    /// </summary>
    /// <param name="link">Beschreibung des anzuzeigenden Models</param>
    /// <param name="targetPage">zu verwendende Page (Default = null = neue Page erzeugen) </param>
    IViewPage OpenPage(ModelLink link, IViewPage? targetPage = null);

    /// <summary>
    /// Sidebar anzeigen
    /// </summary>
    void ShowSidebar();

    /// <summary>
    /// Eine Liste der geöffneten Views
    /// </summary>
    IEnumerable<IViewPage>? Pages { get; }

    /// <summary>
    /// der momentan aktive View
    /// </summary>
    IViewPage? ActivePage { get; }
}

/// <summary>
/// Bookmark für ein IModel
/// </summary>
public class ModelBookmark
{
    /// <summary>
    /// Constructor...
    /// </summary>
    public ModelBookmark()
    {
        ModelType = typeof(Nullable);
        Id = Guid.Empty;
        Caption = "";
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="modeltype">Type des Models</param>
    /// <param name="id">ID des Models</param>
    /// <param name="caption">Überschrift/Titel für die Anzeige des Models</param>
    public ModelBookmark(Type modeltype, Guid id, string caption)
    {
        ModelType = modeltype;
        Id = id;
        Caption = caption;
    }

    /// <summary>
    /// ID des Models
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Typ des Models
    /// </summary>
    [JsonIgnore]
    public Type ModelType { get; set; }

    /// <summary>
    /// Typ des Models als string (für Serialisierung notwendig!)
    /// </summary>
    public string ModelTypeText
    {
        get => ModelType.FullName ?? "";
        set
        {
            if (string.IsNullOrEmpty(value))
                ModelType = typeof(Nullable);
            else
            {
                var type = TypeEx.FindType(value);
                if (type != null && type.HasInterface(typeof(IDataObject)))
                    ModelType = type;
                else
                    throw new NotSupportedException($"Typ {value} wird nicht als ModelType in ModelBookmark unterstützt.");
            }
        }
    }

    /// <summary>
    /// Überschrift/Titel für die Anzeige des Models
    /// </summary>
    public string Caption { get; set; }

    /// <summary>
    /// Name des Typs
    /// </summary>
    public string TypeName { get; set; } = "";

    /// <summary>
    /// Zeitpunkt zu dem der Link erzeugt wurde
    /// </summary>
    public DateTime Created { get; set; } = DateTime.Now;

    /// <summary>
    /// Aktueller Typ der IViewDetails.
    /// </summary>
    [JsonIgnore]
    public Type? CurrentDetailType { get; set; }

    /// <summary>
    /// Typ des Models als string (für Serialisierung notwendig!)
    /// </summary>
    public string CurrentDetailTypeText
    {
        get => CurrentDetailType?.FullName ?? "";
        set
        {
            if (string.IsNullOrEmpty(value))
                CurrentDetailType = null;
            else
            {
                var type = TypeEx.FindType(value);

                CurrentDetailType = type ?? null;
            }
        }
    }


    /// <summary>
    /// Liefert den Bookmark als ModelLink
    /// </summary>
    [JsonIgnore]
    public ModelLink Link => new ModelLink(Id, Caption, ModelType) { CurrentDetailType = CurrentDetailType };

    /// <summary>
    /// Zeitpunkt der letzten Verwendung (für Verlauf/History)
    /// </summary>
    [JsonIgnore]
    public string LastUsedText
    {
        get
        {
            string ret = "";
            if (DateTime.Now.Date > Created.Date)
                ret = "vor {0} Tagen".DisplayWith((DateTime.Now.Date - Created.Date).Days);
            else
            {
                if ((DateTime.Now - Created).Hours > 1)
                    ret = "vor {0} Std.".DisplayWith((DateTime.Now - Created).Hours);
                else
                    ret = "vor {0} Min.".DisplayWith((DateTime.Now - Created).Minutes);
            }

            return ret;
        }
    }
}