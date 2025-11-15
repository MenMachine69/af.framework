namespace AF.MVC;

/// <summary>
/// Allgemeines Controller Interface
/// </summary>
public interface IController
{
    /// <summary>
    /// Bild (SVG, Bitmap usw.), das den Typ in einer Benutzeroberfläche darstellt.
    /// </summary>
    object? TypeImage { get; }

    /// <summary>
    /// Name des Typs, der in der Benutzeroberfläche angezeigt werden soll (Singular)
    /// </summary>
    string TypeName { get; }

    /// <summary>
    /// Name des Typs, der in der Benutzeroberfläche angezeigt werden soll (Plural)
    /// </summary>
    string TypeNamePlural { get; }

    /// <summary>
    /// von diesem Controller gesteuerter Typ
    /// </summary>
    Type ControlledType { get; }

    /// <summary>
    /// Steuert, wie die Details angezeigt werden.
    /// 
    /// Standard ist ePageDetailMode.NoDetails. Wenn die Seite Detailansichten haben soll, 
    /// muss diese Eigenschaft überschrieben werden.
    /// </summary>
    ePageDetailMode DetailViewMode { get; }

    /// <summary>
    /// Typ des in einer Suchmaschine zu verwendenden Models.
    ///
    /// Kann vom Type des Models abweichen (z.B. ein IView-Model sein).
    /// </summary>
    Type GetSearchType { get; }

    /// <summary>
    /// Der für einen bestimmten Anwendungsfall zu verwendende Modellklassentyp.
    /// 
    /// Z.B.:
    /// ModelType ist User
    /// GetModelType(eGridStyle.ComboboxEntrys) - typeof(vwUserSelect)
    /// 
    /// Überschreiben Sie diese Methode, um eine andere Modellklasse als ControlledType zu verwenden.
    /// </summary>
    /// <param name="style">Grid-Stil</param>
    /// <param name="mastertype">Typ des Masters, für den das Grid angezeigt werden soll</param>
    /// <returns>der Modelltyp, der für den jeweiligen Zweck verwendet werden soll</returns>
    Type GetGridModelType(eGridStyle style, Type? mastertype = null);

    /// <summary>
    /// Liefert den ModelLink für das Model mit der angegebenen ID.
    /// </summary>
    /// <param name="modelid">ID/PK des Models</param>
    /// <returns>ModelLink des Models oder NULL, wenn kein Model mit der ID existiert</returns>
    public ModelLink? GetModelLink(Guid modelid);

    /// <summary>
    /// Liefert die ModelInfo für das Model mit der angegebenen ID.
    /// </summary>
    /// <param name="modelid">ID/PK des Models</param>
    /// <returns>ModelInfo des Models oder NULL, wenn kein Model mit der ID existiert</returns>
    public ModelInfo? GetModelInfo(Guid modelid);

    /// <summary>
    /// Erzeugt ein IModelInfo-Objekt für den verwalteten Typen.
    /// 
    /// IModelInfo werden z.B. zur Darstellung in Trefferlisten bei der Suche verwendet.
    /// </summary>
    /// <param name="modelInfo">anzupassende ModelInfo</param>
    /// <returns>ModelInfo für das Model</returns>
    public void CustomizeModelInfo(ModelInfo modelInfo);
    
    /// <summary>
    /// Erstellt eine Datenbankverbindung. 
    ///
    /// MUSS IN KLASSEN ÜBERSCHRIEBEN WERDEN!
    /// </summary>
    /// <returns>die Verbindung</returns>
    /// <exception cref="NotImplementedException">Diese Methode muss im konkreten Controller überschrieben werden!</exception>
    IConnection GetConnection();

    /// <summary>
    /// Holt einen bestimmten Befehl
    /// </summary>
    /// <param name="type">Befehlstyp</param>
    /// <param name="context">Kontext, in dem der Befehl verwendet werden kann</param>
    /// <param name="checkRights">Rechte prüfen, falls der Benutzer keine hat, null zurückgeben</param>
    /// <returns>der Befehl oder null</returns>
    AFCommand? GetCommand(eCommand type, eCommandContext context, bool checkRights);

    /// <summary>
    /// Liefert ein Command anhand des Namens.
    /// 
    /// <example>
    /// <code>
    /// var cmd = BenutzerController.GetCommand("CmdSave");
    /// cmd?.Execute(...):
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="name">Name des Commands (=Name der Methode)</param>
    /// <returns>das Command oder NULL</returns>
    AFCommand? GetCommand(string name);

    /// <summary>
    /// Liefert ein Command anhand des Typs.
    /// <example>
    /// <code>
    /// var cmd = BenutzerController.GetCommand(eCommand.Save);
    /// cmd?.Execute(...):
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="commandType">Typ des Commands</param>
    /// <returns>das Command oder NULL</returns>
    AFCommand? GetCommand(eCommand commandType);

    /// <summary>
    /// Liefert ein Array mit allen Commands des Controllers.
    /// </summary>
    /// <returns>Array der Commands (kann leer sein!)</returns>
    AFCommand[] GetCommands();

    /// <summary>
    /// Liefert ein Array mit allen Commands des Controllers für einen bestimmten Kontext (Verwendungszweck)
    /// </summary>
    /// <param name="context">Kontext, dessen Commands benötigt werden</param>
    /// <param name="viewContext">Context für den die Commands benötigt werden</param>
    /// <returns>Array der Commands (kann leer sein!)</returns>
    AFCommand[] GetCommands(eCommandContext context, string viewContext);

    /// <summary>
    /// Liefert ein Array mit allen Commands des Controllers für einen bestimmten Kontext (Verwendungszweck)
    /// </summary>
    /// <param name="context">Kontext, dessen Commands benötigt werden</param>
    /// <param name="viewContext">Context für den die Commands benötigt werden</param>
    /// <param name="model">Datenobjekt, für die die Commands benötigt werden</param>
    /// <returns>Array der Commands (kann leer sein!)</returns>
    AFCommand[] GetCommands(eCommandContext context, string viewContext, IModel? model);

    /// <summary>
    /// Bookmarks erlauben
    /// </summary>
    public bool AllowBookmarks { get; set; }

    /// <summary>
    /// Darstellung im Verlauf erlauben
    /// </summary>
    public bool AllowHistory { get; set; }

    /// <summary>
    /// Suche via SearchEngine erlauben
    /// </summary>
    public bool AllowSearch { get; set; }

    /// <summary>
    /// Gibt eine Liste von Typ-Beschreibungen zurück, die untergeordnete Modelle dieses Controllers sind.
    /// </summary>
    /// <returns>Liste der Childs oder null, wenn kein Child vorhanden</returns>
    TypeDescription[]? GetChildTypes();

    /// <summary>
    /// Liefert den Child-/Detail-Typ für den angegebenen MasterType.
    /// </summary>
    /// <param name="mastertype">Typ des Masters</param>
    /// <returns>Child-/Detailtyp oder NULL</returns>
    Type? GetChildType(Type mastertype);

    /// <summary>
    /// Gibt den Typ der primären Child-/Detail-Daten zurück.
    /// </summary>
    /// <returns>Typ des primären Detail-Typen</returns>
    Type? GetPrimaryChildType();

    /// <summary>
    /// Prüft, ob ein Model ein Child-Model des Masters ist.
    ///
    /// Wird z.B. verwendet, um zu prüfen, ob ein Model im DetailView zu einem bestimmten Master gehört.
    /// </summary>
    /// <param name="child"></param>
    /// <param name="masterID"></param>
    /// <returns>true, wenn es ein Child-Objekt ist, sonst false</returns>
    bool IsChildOf(IModel child, Guid masterID);

    /// <summary>
    /// Ruft die Definition für eine Gitteransicht zur Anzeige der Modelle ab (Spalten usw.)
    /// </summary>
    /// <param name="mastertype">Typ des Masters, wenn das Grid Childs dieses Masters anzeigt</param>
    /// <param name="detailtype">Typ der Details, für die das Grid ermittelt werden soll</param>
    /// <param name="style">Stil des Rasters</param>
    /// <param name="gridtype">Typ des Rasters, Standard ist eGridMode.GridView</param>
    /// <param name="stylename">Name des Styles (nur wenn style = Custom)</param>
    /// <param name="fields">Optionale Liste der Felder, die im Grid dargestellt werden sollen</param>
    /// <returns>GridSetup-Objekt</returns>
    AFGridSetup GetGridSetup(eGridStyle style, Type? mastertype = null, Type? detailtype = null, string[]? fields = null, eGridMode gridtype = eGridMode.GridView, string? stylename = null);
    
    /// <summary>
    /// Liest das erste Objekt von TModel, das der Abfrage entspricht
    /// </summary>
    /// <param name="style">Stil/Umfang der Daten, der benötigt wird</param>
    /// <param name="query">Abfrage (where-Klausel), die ausgeführt wird, um die Modelle zu erhalten</param>
    /// <param name="args">Parameter für diese Abfrage</param>
    /// <returns>das Modell oder null</returns>
    TData? ReadFirst<TData>(eGridStyle style, string query, params object[] args) where TData : class, IDataObject, new();

    /// <summary>
    /// Liest das Objekt von TModel mit der angegebenen ID
    /// </summary>
    /// <param name="id">ID des zu lesenden Datensatzes</param>
    /// <returns>das Modell oder null</returns>
    TData? ReadSingle<TData>(Guid id) where TData : class, IDataObject, new();

    /// <summary>
    /// Liest ein einzelnes Objekt von TViewModel mit der angegebenen ID.
    /// 
    /// Das IViewModel muss als 
    /// </summary>
    /// <param name="primaryKey">ID des zu lesenden Modells</param>
    /// <returns>das Modell oder null</returns>
    public IModel? ReadSingle(Guid primaryKey);

    /// <summary>
    /// Ermittelt eine Liste aller Objekte des Typs TModel, die der Abfrage entsprechen
    /// </summary>
    /// <param name="style">Stil/Umfang der Daten, der benötigt wird</param>
    /// <param name="query">Abfrage (where-Klausel), die ausgeführt wird, um die Modelle zu erhalten</param>
    /// <param name="args">Parameter für diese Abfrage</param>
    /// <param name="options">Abfrageoptionen</param>
    /// <returns>eine Liste der gelesenen Modelle</returns>
    BindingList<TData> Read<TData>(eGridStyle style, ReadOptions? options = null, string? query = null, params object[]? args) where TData : class, IDataObject, new();

    /// <summary>
    /// Liest Daten aus einer beliebigen View der Datenbank.
    /// 
    /// Kann vor allem genutzt werden, um auf View-Daten zuzugreifen
    /// </summary>
    /// <typeparam name="TData">Typ des Views</typeparam>
    /// <param name="archived">archivierte anzeigen</param>
    /// <param name="options">Optionen</param>
    /// <param name="query">Filterbedingung/Query</param>
    /// <param name="args">Argumente für die Filterbedingung</param>
    /// <returns>Liste der gelesenen Daten (kann leer sein)</returns>
    BindingList<TData> ReadAny<TData>(bool archived = false, ReadOptions? options = null, string? query = null, params object[]? args) where TData : class, IDataObject, new();
   
    /// <summary>
    /// Liste der im Browser darstellbaren Models
    /// </summary>
    /// <param name="archived"></param>
    /// <param name="filter"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    IBindingList ReadBrowserModels(bool archived, string? filter = null, params object[]? args);

    /// <summary>
    /// Liest ein einzelnes Objekt mit der angegebenen ID.
    /// </summary>
    /// <param name="primaryKey">ID des zu lesenden Modells</param>
    /// <returns>das Modell oder null</returns>
    IModel? ReadBrowserModel(Guid primaryKey);

    /// <summary>
    /// Liste der als Detail darstellbaren Models für den angegebenen Master.
    /// </summary>
    /// <param name="master">Master Typ, für den die Details benötigt werden</param>
    /// <param name="masterid">PrimaryKey des Masters, für den die Details benötigt werden</param>
    /// <param name="filter">Filterbedingungen (Query)</param>
    /// <param name="args">Parameter für die Filterbedingungen (QueryParameter)</param>
    /// <returns>Liste der passenden Detail-Models</returns>
    IBindingList ReadDetailModels(Type master, Guid masterid, string? filter = null, params object[]? args);

    /// <summary>
    /// Liste der als Detail darstellbaren Models vom Typ TDetail. 
    /// </summary>
    /// <typeparam name="TDetail">Typ der zu lesenden Details</typeparam>
    /// <param name="master">Master Typ, für den die Details benötigt werden</param>
    /// <param name="masterid">PrimaryKey des Masters, für den die Details benötigt werden</param>
    /// <param name="filter">Filterbedingungen (Query)</param>
    /// <param name="args">Parameter für die Filterbedingungen (QueryParameter)</param>
    /// <returns>Liste der passenden Detail-Models</returns>
    IBindingList ReadDetailModels<TDetail>(Type master, Guid masterid, string? filter = null, object[]? args = null) where TDetail : class, IDataObject, new();

    /// <summary>
    /// Ein einzelnes, als Detail darstellbares Model vom Typ TDetail laden, dessen ID bekannt ist.
    /// </summary>
    /// <typeparam name="TDetail">Typ der zu lesenden Details</typeparam>
    /// <param name="master">Typ des aktuellen Masters</param>
    /// <param name="detailid">ID des zu ladenden Details</param>
    /// <param name="filter">Filterbedingung (WHERE)</param>
    /// <param name="args">Argumente für den Filter (Parameter)</param>
    /// <returns>Das Detail-Model oder NULL</returns>
    IModel? ReadDetailModel<TDetail>(Type master, Guid detailid, string? filter = null, params object[]? args) where TDetail : class, IDataObject, new();

    /// <summary>
    /// Ein einzelnes, als Detail darstellbares Model laden, dessen ID bekannt ist.
    /// </summary>
    /// <param name="master">Typ des aktuellen Masters</param>
    /// <param name="detailid">ID des zu ladenden Details</param>
    /// <param name="filter">Filterbedingung (WHERE)</param>
    /// <param name="args">Argumente für den Filter (Parameter)</param>
    /// <returns>Das Detail-Model oder NULL</returns>
    IModel? ReadDetailModel(Type master, Guid detailid, string? filter = null, params object[]? args);

    /// <summary>
    /// Liste der auswählbaren Models (z.B. via Combobox)
    /// </summary>
    /// <param name="archived">archivierte Daten berücksichtigen</param>
    /// <param name="filter">Filterbedingung, dem die Models entsprechen müssen (null = kein Filter)</param>
    /// <param name="args">Parameter für die Filterbedingungen</param>
    /// <returns>Liste der Models oder leeer Liste</returns>
    IBindingList ReadSelectionModels(bool archived, string? filter = null, params object[]? args);

    /// <summary>
    /// Aktuelles ausgewähltes Model (z.B. in Combobox) lesen
    /// </summary>
    /// <param name="id">ID des Models</param>
    /// <param name="filter">Filterbedingung, dem das Model entsprecchen muss (null = kein Filter)</param>
    /// <param name="args">Parameter für die Filterbedingungen</param>
    /// <returns>einzelnes Model mit der angegeben ID oder NULL wenn kein Model mit der ID existiert 
    /// oder dieses nicht dem Filter entspricht.</returns>
    IDataObject? ReadSelectionModel(Guid id, string? filter = null, params object[]? args);

    /// <summary>
    /// Details eines Masters laden und TRUE zurückgeben, wenn die Details geladen wurden.
    ///
    /// Gibt diese Methode FALSE zurück, werden die Detail-Models anschließend via ReadDetailModels geladen.
    /// Diese Methode wird in der Regel verwendet, wenn der DetailView Details darstellt, die sich DIREKT auf
    /// den Master beziehen (z.B. QueryDesigner für Datenquelle, WorkFlowDesigner für einen Workflow etc.). 
    /// </summary>
    /// <param name="master">Model, dass den Master repräsentiert</param>
    /// <param name="view">DetailView, das die Daten entgegen nimmt</param>
    /// <param name="typeDetails">Typ der anzuzeigenden Details</param>
    /// <returns>true wenn geladen, sonst false</returns>
    bool LoadDetails(IModel? master, IViewDetail view, TypeDescription typeDetails);

    /// <summary>
    /// Typ der Models, die für die Auswahl verwendet werden. 
    /// 
    /// Kann statt des Typs der Tabelle z.B. auch ein View sein.
    /// </summary>
    /// <returns></returns>
    Type? GetSelectionModelType();

    /// <summary>
    /// Gibt an, ob Filterbedingungen für die Auswahl verwendet werden können.
    /// 
    /// In der Regel ist der Wert true, muss aber für Models, die eigene Controls zur Auswahl anbieten ggf. auf false gesetzt werden.
    /// </summary>
    bool SupportSelectionFilter { get; }

    /// <summary>
    /// Suche aus der SearchEngine ausführen.
    /// </summary>
    /// <param name="query">Abfrage</param>
    /// <param name="para">Parameter</param>
    /// <param name="all">alle Treffer ausgeben</param>
    /// <returns>Liste der gefundnen Einträge</returns>
    IBindingList Search(bool all, string query, object[] para);
}

/// <summary>
/// Schnittstelle für einen Controller.
/// 
/// Unter normalen Umständen ist es nicht notwendig, diese Schnittstelle selbst zu implementieren.
/// Verwenden Sie die Controller - Klasse und erben Sie von ihr, anstatt die Schnittstelle 
/// in Ihrem eigenen Controller zu implementieren. 
/// <code>
/// public class ControllerSomeWhat : Controllerlt;SomeWhat&gt;
/// {
/// ...
/// }
/// </code>
/// </summary>
public interface IController<TModel> : IController where TModel : class, IDataObject, new()
{
    /// <summary>
    /// Gibt das Standardmodell für den Controller zurück. 
    /// Dieses Modell ist ein Singleton von TModel, das mit der Methode Create erstellt wurde.
    ///
    /// Diese Methode kann verwendet werden, wenn das Modell nur einmal zur Laufzeit existieren soll.
    /// </summary>
    /// <returns>das Standardmodell oder null</returns>
    TModel? GetSingleton();

    /// <summary>
    /// Typ des Models (Table)
    /// </summary>
    public Type ModelType { get; }

    /// <summary>
    /// Typ des 'Large' Models (meist View)
    /// </summary>
    public Type ModelLargeType { get; }

    /// <summary>
    /// Typ des 'Small'Models (meist View)
    /// </summary>
    public Type ModelSmallType { get; }


    #region AFUD
    /// <summary>
    /// Erzeugt ein neues Modell. Dies sollte der EINZIGE Weg sein, um neue Modelle vom Typ TModel zu erstellen!
    /// 
    /// Die Methode akzeptiert ein 'Quell'-Modell, aus dem die Daten kopiert werden sollen.
    /// </summary>
    /// <param name="source">Quelle/gegebenes Modell</param>
    /// <returns>Neues Objekt vom Typ TModel. Wenn das Quellmodell angegeben wird, enthält das neue Modell dieselben Daten wie das Quellmodell, mit Ausnahme der Systemfelder wie "key", "created" und "changed".</returns>
    TModel Create(TModel? source = null);

    /// <summary>
    /// Speichert ein Objekt vom Typ TModel in den Speicher, schreibt aber nur Felder, deren Namen im Array stehen (Array mit Feldnamen)
    /// </summary>
    /// <param name="connection">Datenbankverbindung (optional, wenn NULL wir neue Verbindung erzeugt)</param>
    /// <param name="data">TModel zum Speichern</param>
    /// <param name="fields">Speichere nur die Felder, deren Namen in der Liste enthalten sind</param>
    /// <param name="silent">wenn true: keine Ereignisse über diesen Speichervorgang auslösen</param>
    CommandResult Save<TRecord>(TRecord data, IConnection? connection = null, string[]? fields = null, bool silent = false) where TRecord : class, TModel, ITable;

    /// <summary>
    /// Speichert eine Liste von Objekten des Typs TModel in den Speicher, schreibt aber nur Felder, deren Namen im Array stehen (Array mit Feldnamen)
    /// </summary>
    /// <param name="data">zu speicherndes TModel</param>
    /// <param name="connection">Datenbankverbindung (optional, wenn NULL wird neue Verbindung erzeugt)</param>
    /// <param name="fields">Speichere nur die Felder, deren Namen in der Liste enthalten sind</param>
    /// <param name="silent">wenn true: keine Ereignisse über diesen Speichervorgang auslösen</param>
    CommandResult Save<TRecord>(IEnumerable<TRecord> data, IConnection? connection = null, string[]? fields = null, bool silent = false) where TRecord : class, TModel, ITable;

    /// <summary>
    /// Ein Objekt vom Typ TModel aus dem Speicher löschen
    /// </summary>
    /// <param name="data">zu löschendes Objekt</param>
    /// <param name="connection">optional: Datenbankverbindung (wenn NULL wird neue Verbindung erzeugt)</param>
    /// <param name="silent">optional: true, wenn alle Datenbankereignisse (Workflow etc.) unterdrückt werden sollen</param>
    CommandResult Delete<TRecord>(TRecord data, IConnection? connection = null, bool silent = false) where TRecord : class, TModel, ITable;

    /// <summary>
    /// Löscht alle Objekte des Typs TModel in einer Liste aus dem Speicher.
    /// </summary>
    /// <param name="data">Liste der zu löschenden Objekte</param>
    /// <param name="connection">optional: Datenbankverbindung (wenn NULL wird neue Verbindung erzeugt)</param>
    /// <param name="silent">optional: true, wenn alle Datenbankereignisse (Workflow etc.) unterdrückt werden sollen</param>
    CommandResult Delete<TRecord>(IEnumerable<TRecord> data, IConnection? connection = null, bool silent = false) where TRecord : class, TModel, ITable;

    /// <summary>
    /// Liest das erste Objekt von TModel, das der Abfrage entspricht
    /// </summary>
    /// <param name="query">Abfrage (where-Klausel), die ausgeführt wird, um die Modelle zu erhalten</param>
    /// <param name="args">Parameter für diese Abfrage</param>
    /// <returns>das Modell oder null</returns>
    TModel? ReadFirst(string query, params object[] args);

   
    /// <summary>
    /// Ermittelt eine Liste aller Objekte des Typs TModel, die der Abfrage entsprechen
    /// </summary>
    /// <param name="query">Abfrage (where-Klausel), die ausgeführt wird, um die Modelle zu erhalten</param>
    /// <param name="args">Parameter für diese Abfrage</param>
    /// <param name="options">Abfrageoptionen</param>
    /// <returns>eine Liste der gelesenen Modelle</returns>
    BindingList<TModel> Read(ReadOptions? options = null, string? query = null, params object[]? args);

    /// <summary>
    /// Liest das Objekt von TModel mit der angegebenen ID
    /// </summary>
    /// <param name="id">ID des zu lesenden Datensatzes</param>
    /// <returns>das Modell oder null</returns>
    TModel? Load(Guid id);


    /// <summary>
    /// Liste aller vom Controller verwalteten Objekte lesen
    /// </summary>
    /// <returns>Liste der Objekte (leere Liste, wenn keine Objekte vorhanden sind)</returns>
    BindingList<TModel> ReadList();

    /// <summary>
    /// Liste der vom Controller verwalteten Objekte lesen
    /// </summary>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    BindingList<TModel> ReadList(string query, params object[] args);

    /// <summary>
    /// Liste der vom Controller verwalteten Objekte lesen
    /// </summary>
    /// <param name="options">Optionen</param>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    BindingList<TModel> ReadList(ReadOptions? options = null, string? query = null, params object[] args);

    /// <summary>
    /// Einzelnes, vom Controller verwaltetes Objekt lesen (erstes Objekt, dass den Bedingungen entspricht)
    /// </summary>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    TModel? ReadSingle(string query, params object[] args);

    /// <summary>
    /// Einzelnes, vom Controller verwaltetes Objekt lesen (erstes Objekt, dass den Bedingungen entspricht)
    /// </summary>
    /// <param name="options">Optionen</param>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    TModel? ReadSingle(ReadOptions? options = null, string? query = null, params object[] args);

    /// <summary>
    /// Liste aller ausführlichen Objekte (meist aus View) lesen, die der Controller verwaltet.
    /// </summary>
    /// <returns>Liste der Objekte (leere Liste, wenn keine Objekte vorhanden sind)</returns>
    BindingList<TModel> ReadLargeList();

    /// <summary>
    /// Liste der ausführlichen Objekte (meist aus View) lesen, die der Controller verwaltet.
    /// </summary>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    BindingList<TModel> ReadLargeList(string query, params object[] args);

    /// <summary>
    /// Liste der ausführlichen Objekte (meist aus View) lesen, die der Controller verwaltet.
    /// </summary>
    /// <param name="options">Optionen</param>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    BindingList<TModel> ReadLargeList(ReadOptions? options = null, string? query = null, params object[] args);

    /// <summary>
    /// Einzelnes ausführliches Objekt (meist aus View), vom Controller verwaltetes Objekt lesen (erstes Objekt, dass den Bedingungen entspricht)
    /// </summary>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    TModel? ReadLargeSingle(string query, params object[] args);

    /// <summary>
    /// Einzelnes ausführliches Objekt (meist aus View), vom Controller verwaltetes Objekt lesen (erstes Objekt, dass den Bedingungen entspricht)
    /// </summary>
    /// <param name="options">Optionen</param>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    TModel? ReadLargeSingle(ReadOptions? options = null, string? query = null, params object[] args);

    /// <summary>
    /// Einzelnes ausführliches Objekt (meist aus View), vom Controller verwaltetes Objekt lesen (anhand des PrimaryKey des Objekts)
    /// </summary>
    /// <param name="guid">PrimaryKey des Objekts</param>
    /// <returns>das geladene Objekt oder NULL</returns>
    TModel? LoadLarge(Guid guid);

    /// <summary>
    /// Liste aller kleinen Objekte (meist aus View) lesen, die der Controller verwaltet.
    /// </summary>
    /// <returns>Liste der Objekte</returns>
    BindingList<TModel> ReadSmallList();

    /// <summary>
    /// Liste der kleinen Objekte (meist aus View) lesen, die der Controller verwaltet.
    /// </summary>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    BindingList<TModel> ReadSmallList(string query, params object[] args);

    /// <summary>
    /// Liste der kleinen Objekte (meist aus View) lesen, die der Controller verwaltet.
    /// </summary>
    /// <param name="options">Optionen</param>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    BindingList<TModel> ReadSmallList(ReadOptions? options = null, string? query = null, params object[] args);

    /// <summary>
    /// Einzelnes kleines Objekt (meist aus View), vom Controller verwaltetes Objekt lesen (erstes Objekt, dass den Bedingungen entspricht)
    /// </summary>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    TModel? ReadSmallSingle(string query, params object[] args);

    /// <summary>
    /// Einzelnes kleines Objekt (meist aus View), vom Controller verwaltetes Objekt lesen (erstes Objekt, dass den Bedingungen entspricht)
    /// </summary>
    /// <param name="options">Optionen</param>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    TModel? ReadSmallSingle(ReadOptions? options = null, string? query = null, params object[] args);

    /// <summary>
    /// Einzelnes kleines Objekt (meist aus View), vom Controller verwaltetes Objekt lesen (anhand des PrimaryKey des Objekts)
    /// </summary>
    /// <param name="guid">PrimaryKey des Objekts</param>
    /// <returns>das geladene Objekt oder NULL</returns>
    TModel? LoadSmall(Guid guid);

    /// <summary>
    /// Liste aller Objekte über die Datenbankverbindung des Controllers lesen.
    /// 
    /// ACHTUNG! Diese Methode sollte nur in Ausnahmefällen (z.B. für zusätzliche Views neben 'Small' und 'Large' verwendet werden. 
    /// Ansonsten ist der Controller des betreffenden Objekt-Typs zu bevorzugen!
    /// </summary>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    BindingList<T> ReadAnyList<T>() where T : class, IDataObject, new();

    /// <summary>
    /// Liste beliebiger Objekte über die Datenbankverbindung des Controllers lesen.
    /// 
    /// ACHTUNG! Diese Methode sollte nur in Ausnahmefällen (z.B. für zusätzliche Views neben 'Small' und 'Large' verwendet werden. 
    /// Ansonsten ist der Controller des betreffenden Objekt-Typs zu bevorzugen!
    /// </summary>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    BindingList<T> ReadAnyList<T>(string query, params object[] args) where T : class, IDataObject, new();


    /// <summary>
    /// Liste beliebiger Objekte über die Datenbankverbindung des Controllers lesen.
    /// 
    /// ACHTUNG! Diese Methode sollte nur in Ausnahmefällen (z.B. für zusätzliche Views neben 'Small' und 'Large' verwendet werden. 
    /// Ansonsten ist der Controller des betreffenden Objekt-Typs zu bevorzugen!
    /// </summary>
    /// <param name="options">Optionen</param>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    BindingList<T> ReadAnyList<T>(ReadOptions? options = null, string? query = null, params object[] args) where T : class, IDataObject, new();

    /// <summary>
    /// Einzelnes beliebiges Objekt über die Datenbankverbindung des Controllers lesen (erstes Objekt, dass den Bedingungen entspricht)
    /// 
    /// ACHTUNG! Diese Methode sollte nur in Ausnahmefällen (z.B. für zusätzliche Views neben 'Small' und 'Large' verwendet werden. 
    /// Ansonsten ist der Controller des betreffenden Objekt-Typs zu bevorzugen!
    /// </summary>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    T? ReadAnySingle<T>(string query, params object[] args) where T : class, IDataObject, new();

    /// <summary>
    /// Einzelnes beliebiges Objekt über die Datenbankverbindung des Controllers lesen (erstes Objekt, dass den Bedingungen entspricht)
    /// 
    /// ACHTUNG! Diese Methode sollte nur in Ausnahmefällen (z.B. für zusätzliche Views neben 'Small' und 'Large' verwendet werden. 
    /// Ansonsten ist der Controller des betreffenden Objekt-Typs zu bevorzugen!
    /// </summary>
    /// <param name="options">Optionen</param>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    T? ReadAnySingle<T>(ReadOptions? options = null, string? query = null, params object[] args) where T : class, IDataObject, new();


    /// <summary>
    /// Beliebiges Objekt über die Datenbankverbindung des Controllers lesen (anhand des PrimaryKey des Objekts)
    /// 
    /// ACHTUNG! Diese Methode sollte nur in Ausnahmefällen (z.B. für zusätzliche Views neben 'Small' und 'Large' verwendet werden. 
    /// Ansonsten ist der Controller des betreffenden Objekt-Typs zu bevorzugen!
    /// </summary>
    /// <param name="guid">PrimaryKey des Objekts</param>
    /// <returns>das geladene Objekt oder NULL</returns>
    T? LoadAny<T>(Guid guid) where T : class, IDataObject, new();


    #endregion
}
