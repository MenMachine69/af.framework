using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.Utils.Svg;

namespace AF.MVC;


/// <summary>
/// Manager, der die Anzeige beliebig vieler IViewPage erlaubt.
/// Neben den IViewPages kann der Manager auch ein IViewSidebar und einen Browser anzeigen.
/// </summary>
[ToolboxItem(true)]
[SupportedOSPlatform("windows")]
public partial class AFViewManager : AFUserControl, IViewManager
{
    /// <summary>
    /// Constructor
    /// </summary>
    public AFViewManager()
    {
        InitializeComponent();

        if (UI.DesignMode) return;

        splitBrowser.PanelVisibility = SplitPanelVisibility.Panel2; // Browser standardmässig unterdrücken
    }

    /// <summary>
    /// aktuell aktive/fokussierte IViewPage
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IViewPage? ActivePage => DocumentManager.View.ActiveDocument?.Control as IViewPage;

    /// <summary>
    /// Liste der Lesezeichen
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public BindingList<ModelBookmark> Bookmarks { get; set; } = [];

    /// <summary>
    /// access to the model browser
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFModelBrowser Browser => (AFModelBrowser)splitBrowser.Panel1.Controls[0];

    /// <summary>
    /// DocumentManager, der die Anzeige der IViewPages in separate Tabs übernimmt.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DocumentManager DocumentManager => pageContainer1.DocumentManager;

    /// <summary>
    /// Liste der Lesezeichen
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public BindingList<ModelBookmark> History { get; set; } = [];

    /// <summary>
    /// Sidebar, die im Manager angezeigt werden kann.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFSidebarControl? Sidebar { get; set; }

    /// <summary>
    /// Bookmark hinzufügen/aktualisieren
    /// </summary>
    /// <param name="page">Page deren Model als Bookmark abgelegt werden soll</param>
    public void AddToBookmarks(IViewPage page)
    {
        if (page.ModelLink is not ModelLink link) return;

        if (link.ModelType.GetController() is not IControllerUI controller)
            throw new Exception(@$"No controller found for model {link.ModelType.Name}.");

        if (!controller.AllowBookmarks) return;

        addBookmark(link, Bookmarks);
    }

    /// <summary>
    /// History/Verlauf hinzufügen/aktualisieren
    /// </summary>
    /// <param name="link">Link der im Verlauf abgelegt werden soll</param>
    public void AddToHistory(ModelLink link)
    {
        if (link.ModelType.GetController() is not IControllerUI controller)
            throw new Exception(@$"No controller found for model {link.ModelType.Name}.");

        if (!controller.AllowHistory) return;

        addBookmark(link, History);
    }

    /// <summary>
    /// Den Browser für Models anzeigen.
    /// </summary>
    public void ShowModelBrowser()
    {
        splitBrowser.PanelVisibility = SplitPanelVisibility.Both;

        if (splitBrowser.Panel1.Controls.Count < 1)
            splitBrowser.Panel1.Controls.Add(new AFModelBrowser() { Dock = DockStyle.Fill });
    }

    /// <summary>
    /// Den Browser für Models verbergen.
    /// </summary>
    public void HideModelBrowser()
    {
        splitBrowser.PanelVisibility = SplitPanelVisibility.Panel2;
    }

    /// <summary>
    /// Sichtbarkeit des Browsers umschalten
    /// </summary>
    public void ToggleModelBrowser()
    {
        if (splitBrowser.PanelVisibility == SplitPanelVisibility.Both)
            HideModelBrowser();
        else
            ShowModelBrowser();
    }

    private void addBookmark(ModelLink link, BindingList<ModelBookmark> liste)
    {
        var bookmark = link.GetBookmark();

        if (bookmark == null) return;

        var existingBookmar = liste.FirstOrDefault(b => b.Id.Equals(bookmark.Id));

        if (existingBookmar == null)
        {
            liste.Add(bookmark);
            return;
        }

        existingBookmar.Caption = bookmark.Caption;
        existingBookmar.Created = DateTime.Now;
    }

    private BaseDocument getDocument(IViewPage page)
    {
        BaseDocument? doc = DocumentManager.View.Documents?.FirstOrDefault(doc => doc.Control == page) ??
                            DocumentManager.View.FloatDocuments?.FirstOrDefault(doc => doc.Control == page);

        if (doc == null)
            throw new ArgumentException(@"Unknown Page. The Page is currently not available.", nameof(page));

        return doc;
    }

    /// <summary>
    /// Eine Page aktivieren/fokussieren.
    /// Es wird davon ausgegangen, dass die Seite auch existiert.
    /// Ist das NICHT der Fall, wird eine Exception ausgelöst.
    /// Es kann daher ratsam sein, die Existenz der Seite vorher
    /// via HasPage(page) zu überprüfen.
    /// </summary>
    /// <param name="page">page</param>
    public void ActivatePage(IViewPage page)
    {
        BaseDocument doc = getDocument(page);

        if (doc.IsFloating)
            doc.Form.Activate();
        else
            DocumentManager.View.ActivateDocument(doc.Control);
    }

    /// <summary>
    /// Check if the page can be closed.
    /// </summary>
    /// <param name="page">page</param>
    /// <returns>true if page can be closed, otherwise false</returns>
    /// <exception cref="ArgumentException">exception if the page can not be found</exception>
    public bool CanClosePage(IViewPage page)
    {
        var doc = getDocument(page);

        return ((IViewPage)doc.Control).CanClose;
    }

    /// <summary>
    /// Prüft, ob für das Model mit der angegebenen Beschreibung eine Seite geöffnet werden kann.
    /// Gibt diese Methode FALSE zurück, ist in der Regel schon eine entsprechende Seite geöffnet.
    /// </summary>
    /// <param name="link">Beschreibung des Models</param>
    public bool CanOpenPage(ModelLink link)
    {
        if (link.ModelType.GetController() is not IControllerUI controller)
            throw new Exception(@$"No controller found for model {link.ModelType.Name}.");

        if (controller.AllowMultiplePages) return true;

        return GetPage(link.ModelID) == null;
    }


    /// <summary>
    /// Close the specific page.
    /// </summary>
    /// <param name="page">page to close</param>
    /// <returns>true if closed, otherwise false</returns>
    /// <exception cref="ArgumentException">exception if the page can not be found</exception>
    public bool ClosePage(IViewPage page)
    {
        var doc = getDocument(page);

        if (!((IViewPage)doc.Control).Close()) return false;

        DocumentManager.View.Controller.Close(doc);
        return true;
    }

    /// <summary>
    /// Eine Page suchen, in der gerade ein Model angezeigt wird, dass dem angegebenen Link entspricht.
    /// </summary>
    /// <param name="link">Beschreibung des Models</param>
    /// <returns>Erste Page, dessen Model der Beschreibung entspricht, wenn keine Page existiert NULL</returns>
    public IViewPage? GetPage(ModelLink link)
    {
        return GetPage(link.ModelID);
    }


    /// <summary>
    /// Eine Page suchen, in der gerade ein Model angezeigt wird, deren ID deren angegebenen ID entspricht.
    /// </summary>
    /// <param name="modelID">ID des Models</param>
    /// <returns>Erste Page, dessen ModelID der angegebenen ID entspricht, wenn keine Page existiert NULL</returns>
    public IViewPage? GetPage(Guid modelID)
    {
        BaseDocument? doc =
            DocumentManager.View.Documents?.FirstOrDefault(doc =>
                doc.Control is IViewPage page && page.ModelID.Equals(modelID)) ??
            DocumentManager.View.FloatDocuments?.FirstOrDefault(doc =>
                doc.Control is IViewPage page && page.ModelID.Equals(modelID));

        return (IViewPage?)doc?.Control;
    }

    /// <summary>
    /// Aktualisiert die Caption einer Seite
    /// </summary>
    /// <param name="page">ID Seite</param>
    /// <param name="link">Link, der die Seite beschreibt</param>
    public void UpdatePage(IViewPage page, ModelLink link)
    {
        BaseDocument? doc =
            DocumentManager.View.Documents?.FirstOrDefault(doc =>
                doc.Control == page) ??
            DocumentManager.View.FloatDocuments?.FirstOrDefault(doc =>
                doc.Control == page);

        if (doc != null) doc.Caption = link.ModelCaption;

        AddToHistory(link);
    }

    /// <summary>
    /// Prüft, ob die angegebene Seite/Page tatsächlich vorhanden ist.
    /// </summary>
    /// <param name="page">zu prüfende Seite/Page</param>
    public bool HasPage(IViewPage page)
    {
        return DocumentManager.View.Documents.FindFirst(d => d.Control == (Control)page) != null;
    }


    /// <summary>
    /// Öffnen einer Ansicht/Seite für ein Model dessen ModelLink gegeben ist.
    /// Falls der Controller nicht mehrere Seiten zulässt, wird eine bestehende
    /// Seite für das Modell fokussiert werden. Andernfalls wird eine zusätzliche Seite erstellt.
    /// Achtung: Es wird immer das 'Hauptmodel' geöffnet (der vom Controller gesteuerte Typ).
    /// Wird also z.B. der Typ eines Views und die ID übergeben, wird in der Regel immer das Objekt geöffnet,
    /// dass die Tabelle beschreibt.
    /// Beispiel:
    /// Show(typeof(vwUserAuswahl), new Guid("{B504DD36-FB3F-4A8C-8503-7E27D93FF246}"));
    /// öffnet die Seite mit dem User-Model, nicht! eine Seite mit dem vwUserAuswahl-Model!
    /// </summary>
    /// <param name="link">Beschreibung des anzuzeigenden Models</param>
    /// <param name="targetPage">zu verwendende Page (Default = null = neue Page erzeugen) </param>
    public IViewPage OpenPage(ModelLink link, IViewPage? targetPage = null)
    {
        return link.Model != null
            ? OpenPage(link.Model, targetPage)
            : OpenPage(link.ModelType, link.ModelID, targetPage);
    }

    /// <summary>
    /// Öffnen einer Ansicht/Seite für ein Model.
    /// Falls der Controller nicht mehrere Seiten zulässt, wird eine bestehende
    /// Seite für das Modell fokussiert werden. Andernfalls wird eine zusätzliche Seite erstellt.
    /// Achtung: Es wird immer das 'Hauptmodel' geöffnet (der vom Controller gesteuerte Typ).
    /// Wird also z.B. der Typ eines Views und die ID übergeben, wird in der Regel immer das Objekt geöffnet,
    /// dass die Tabelle beschreibt.
    /// Beispiel:
    /// Show(typeof(vwUserAuswahl), new Guid("{B504DD36-FB3F-4A8C-8503-7E27D93FF246}"));
    /// öffnet die Seite mit dem User-Model, nicht! eine Seite mit dem vwUserAuswahl-Model!
    /// </summary>
    /// <param name="model">anzuzeigendes Model</param>
    /// <param name="targetPage">zu verwendende Page (Default = null = neue Page erzeugen) </param>
    public IViewPage OpenPage(IModel model, IViewPage? targetPage = null)
    {
        CloseSidebar();
        Application.DoEvents();

        if (model.GetType().GetController() is not IControllerUI controller)
            throw new Exception(@$"No controller found for model {model.GetType().Name}.");

        IViewPage? page;

        if (!controller.AllowMultiplePages)
        {
            page = GetPage(model.PrimaryKey);
            if (page != null)
            {
                ActivatePage(page);
                return page;
            }
        }

        UI.ShowWait("Bitte warten", "Die Seite wird geöffnet...");

        try
        {
            // Prüfen, ob das der HauptmodelTyp des Controllers ist
            // und ggf. das HauptModel laden.
            if (controller.ControlledType != model.GetType())
            {
                var id = model.PrimaryKey;
                var mainmodel = controller.InvokeGeneric("ReadSingle", [controller.ControlledType], id)
                                ?? throw new Exception($@"No model found for model {controller.ControlledType.Name} and id {id}.");

                model = (IModel)mainmodel;
            }

            BaseDocument? newpage = null;

            if (targetPage == null)
            {
                page = AFPage.Create(controller).LoadModel(model);
                newpage = DocumentManager.View.AddDocument((Control)page, model.ModelLink.ModelCaption);
            }
            else
            {
                page = targetPage.LoadModel(model);
                if (!HasPage(page))
                    newpage = DocumentManager.View.AddDocument((Control)page, model.ModelLink.ModelCaption);
            }

            if (newpage != null)
            {
                var img = controller.TypeImage;
                if (img is SvgImage svg)
                {
                    newpage.ImageOptions.SvgImage = svg;
                    newpage.ImageOptions.SvgImageSize = new(16, 16);
                    newpage.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.None;
                }
            }

            ActivatePage(page);

            AddToHistory(model.ModelLink);
        }
        finally { UI.HideWait();}

        return page;
    }

    /// <summary>
    /// Öffnen einer Ansicht/Seite für ein Model, bei dem ModelTyp und die ID angegeben sind.
    /// Falls der Controller nicht mehrere Seiten zulässt, wird eine bestehende
    /// Seite für das Modell fokussiert werden. Andernfalls wird eine zusätzliche Seite erstellt.
    /// Achtung: Es wird immer das 'Hauptmodel' geöffnet (der vom Controller gesteuerte Typ).
    /// Wird also z.B. der Typ eines Views und die ID übergeben, wird in der Regel immer das Objekt geöffnet,
    /// dass die Tabelle beschreibt.
    /// Beispiel:
    /// Show(typeof(vwUserAuswahl), new Guid("{B504DD36-FB3F-4A8C-8503-7E27D93FF246}"));
    /// öffnet die Seite mit dem User-Model, nicht! eine Seite mit dem vwUserAuswahl-Model!
    /// </summary>
    /// <param name="modeltype">Typ des Models</param>
    /// <param name="modelid">ID des Models</param>
    /// <param name="targetPage">zu verwendende Page (Default = null = neue Page erzeugen) </param>
    public IViewPage OpenPage(Type modeltype, Guid modelid, IViewPage? targetPage = null)
    {
        if (modeltype.GetController() is not IControllerUI controller)
            throw new Exception(@$"No controller found for model {modeltype.Name}.");

        if (!controller.AllowMultiplePages)
        {
            var page = GetPage(modelid);
            if (page != null)
            {
                ActivatePage(page);
                return page;
            }
        }

        // .InvokeGeneric(nameof(IController.ReadSingle), [controller.ControlledType], modelid)
        var model = controller.ReadSingle(modelid)
                    ?? throw new Exception($@"No model found for model {controller.ControlledType.Name} and id {modelid}.");

        return OpenPage(model, targetPage);
    }


    /// <summary>
    /// Eine Liste der offenen Pages.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IEnumerable<IViewPage> Pages =>
        DocumentManager.View.Documents.Select(doc => doc.Control)
            .Where(ctrl => ctrl is IViewPage)
            .OfType<IViewPage>()
            .Concat(DocumentManager.View.FloatDocuments.Select(doc => doc.Control)
                .Where(ctrl => ctrl is IViewPage)
                .OfType<IViewPage>());

    /// <summary>
    /// Sidebar anzeigen (als FlyOut)
    /// </summary>
    public void ShowSidebar()
    {
        if (Sidebar == null) return;

        if (crFlyoutManager1.IsSidebarVisible) return;

        crFlyoutManager1.ShowSidebar(Sidebar);
    }

    /// <summary>
    /// Sidebar verbergen (als FlyOut)
    /// </summary>
    public void CloseSidebar()
    {
        if (Sidebar == null) return;

        if (!crFlyoutManager1.IsSidebarVisible) return;

        crFlyoutManager1.CloseSidebar();
    }

    /// <summary>
    /// Sidebar anzeigen (als FlyOut)
    /// </summary>
    public void ToggleSidebar()
    {
        if (Sidebar == null) return;

        if (crFlyoutManager1.IsSidebarVisible)
            crFlyoutManager1.CloseSidebar();
        else
            crFlyoutManager1.ShowSidebar(Sidebar);
    }

    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (UI.DesignMode) return;

        // Bookmarks und History wiederherstellen
        var bookmarks = AFCore.App.Persistance?.Get(SystemConstants.IDBookmarks);
        if (bookmarks != null && bookmarks.Length > 0)
        {
            try
            {
                Bookmarks.AddRange(Functions.DeserializeJsonBytes<BindingList<ModelBookmark>>(bookmarks)?.ToArray() ?? []);
            }
            catch //(Exception ex)
            {
                Bookmarks = [];
            }
        }

        var history = AFCore.App.Persistance?.Get(SystemConstants.IDHistory);
        if (history != null && history.Length > 0)
        {
            try
            {
                History.AddRange(Functions.DeserializeJsonBytes<BindingList<ModelBookmark>>(history)?.ToArray() ?? []);
            }
            catch //(Exception ex)
            {
                History = [];
            }
        }

        DocumentManager.View.DocumentClosing += View_DocumentClosing;
    }
    
    private void View_DocumentClosing(object sender, DocumentCancelEventArgs e)
    {
        if (e.Document?.Control is IViewPage page)
            e.Cancel = page.CancelClose();
    }
}