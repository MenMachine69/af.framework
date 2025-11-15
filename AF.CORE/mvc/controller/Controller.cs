using System.Drawing;

namespace AF.MVC;

internal static class controllerConstants
{
    internal static readonly AFHtmlTemplate defaultSearchHitHtmlTemplate = new()
    {
        CssTemplate = ".container { margin-bottom: 5px; margin-left: 6px; margin-right: 6px; width: 320px; } " +
                      ".item_container { display: flex; flex-direction: row; border: 1px solid @Window; padding: 5px; width: 320px; height: 40px; background-color: @Window; color: @WindowText; border-radius: 5px; box-shadow: 0px 2px 4px rgba(0, 0, 0, 0.1); } " +
                      ".item { display: flex; flex-direction: row; justify-content: flex-start; text-overflow: ellipsis; width: 100%; } " +
                      ".symbol { padding-right: 5px; } " +
                      ".button { align-content: center; text-align: center; color: @WindowText; border-color: @WindowText; background-color: @Window; border-radius: 3px; border-width: 1px; border-style: solid; padding: 4px 8px; margin: 8px 4px; text-align: center; user-select: none; } " +
                      ".button:hover { background-color: @HighlightAlternate/0.5; color: @HighlightText } " +
                      ".button:active { background-color: @HighlightAlternate/0.5; color: @HighlightText } " +
                      ":root:focus .item_container { background-color: @HighlightAlternate; color: @HighlightText; border: 1px solid @HighlightAlternate; } " +
                      ":root:hover .item_container { border: 1px solid @HighlightAlternate; }",
        HtmlTemplate = "<div class=\"container\"> " +
                       "   <div class=\"item_container\"> " +
                       "      <div class=\"item\" id=\"item\"><img class=\"symbol\" src=\"#SYMBOL#\" width=\"30\" height = \"30\"> ${Caption} </div>" +
                       "      <div class=\"button \" id=\"pshopen\">öffnen</div>" +
                       "   </div>" +
                       "</div>"
    };

    internal static readonly AFHtmlTemplate defaultSearchMoreHtmlTemplate = new()
    {
        CssTemplate = ".container { margin-bottom: 5px; margin-left: 6px; margin-right: 6px; width: 320px; } " +
                      ".button { width: 308px; height: auto; color: @ControlText; border: 1px solid @ControlText; background-color: @Control; border-radius: 5px; box-shadow: 0px 2px 4px rgba(0, 0, 0, 0.1); padding: 8px 12px; text-align: center; user-select: none; } " +
                      ".button:hover { background-color: @Green; color: @White } " +
                      ".button:active { background-color: @Green; color: @White }",
        HtmlTemplate = "<div class=\"container\"> " +
                       "   <div class=\"button \" id=\"psshowall\">Alle Treffer anzeigen...</div>" +
                       "</div>"
    };
 }

/// <summary>
/// Basis-Controller für IModels, die keine ITable sind.
/// 
/// Für alle Table-Modelle (=Modelle, die in einer Datenbank gespeichert sind) verwenden Sie 
/// ControllerTable als Basis für Ihre konkreten Controller.
/// </summary>
/// <typeparam name="TModel">Typ von IModel</typeparam>
public abstract class Controller<TModel> : IController<TModel> where TModel : class, IDataObject, new()
{
    private bool _commandsReaded;
    private readonly Dictionary<string, AFCommand> _commands = new();
    private TModel? _singleton;

    /// <summary>
    /// Controller
    /// </summary>
    protected Controller()
    {
        if (TypeEx.ExistController(this.GetType()))
            throw new Exception($"Es existiert bereits ein Controller vom Typ {this.GetType().FullName}.");
    }

    #region Beschreibung 
    /// <summary>
    /// Bild (SVG, Bitmap usw.), das den Typ in einer Benutzeroberfläche darstellt.
    /// </summary>
    public virtual object? TypeImage => null;

    /// <summary>
    /// Name des Typs, der in der Benutzeroberfläche angezeigt werden soll (Singular)
    /// </summary>
    public virtual string TypeName => ControlledType.GetTypeDescription().Context?.NameSingular??ControlledType.Name;

    /// <summary>
    /// Name des Typs, der in der Benutzeroberfläche angezeigt werden soll (Plural)
    /// </summary>
    public virtual string TypeNamePlural => ControlledType.GetTypeDescription().Context?.NamePlural??ControlledType.Name;

    /// <summary>
    /// von diesem Controller gesteuerter Typ
    /// </summary>
    public virtual Type ControlledType => typeof(TModel);

    /// <summary>
    /// Typ des in einer Suchmaschine zu verwendenden Models.
    ///
    /// Kann vom Type des Models abweichen (z.B. ein IView-Model sein).
    /// </summary>
    public virtual Type GetSearchType => ControlledType;

    #endregion

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
    public virtual Type GetGridModelType(eGridStyle style, Type? mastertype = null) {  return typeof(TModel); }

    /// <summary>
    /// Steuert, wier die Details angezeigt werden.
    /// 
    /// Standard ist ePageDetailMode.NoDetails. Wenn die Seite Detailansichten haben soll, 
    /// muss diese Eigenschaft überschrieben werden.
    /// </summary>
    public virtual ePageDetailMode DetailViewMode => ePageDetailMode.NoDetails;

    /// <summary>
    /// Liefert den ModelLink für das Model mit der angegebenen ID.
    /// </summary>
    /// <param name="modelid">ID/PK des Models</param>
    /// <returns>ModelLink des Models oder NULL, wenn kein Model mit der ID existiert</returns>
    public ModelLink? GetModelLink(Guid modelid) { return ReadSingle<TModel>(modelid)?.ModelLink; }

    /// <summary>
    /// Liefert die ModelInfo für das Model mit der angegebenen ID.
    /// </summary>
    /// <param name="modelid">ID/PK des Models</param>
    /// <returns>das ModelInfo-Objekt des Models oder NULL wenn kein Model mit der ID existiert</returns>
    public ModelInfo? GetModelInfo(Guid modelid)
    {
        ModelInfo? ret = null;

        using var conn = GetConnection();
        ret = conn.LoadInfo<TModel>(modelid);

        if (ret != null)
            CustomizeModelInfo(ret);

        return ret;
    }
      
    /// <summary>
    /// Gibt das Standardmodell für den Controller zurück. 
    /// Dieses Modell ist ein Singleton von TModel, das mit der Methode Create erstellt wurde.
    ///
    /// Diese Methode kann verwendet werden, wenn das Modell nur einmal zur Laufzeit existieren soll.
    /// </summary>
    /// <returns>das Standardmodell oder null</returns>
    public virtual TModel GetSingleton() { return _singleton ??= Create(); }

    /// <summary>
    /// liefert ein ModelLink-Objekt für das angegebene IModel
    /// </summary>
    /// <param name="model">model</param>
    /// <returns>die ModelLink</returns>
    public ModelLink GetModelLink(IDataObject model) { return model.ModelLink; }

    /// <summary>
    /// Erstellt eine Datenbankverbindung.
    ///
    /// MUSS IN KLASSEN ÜBERSCHRIEBEN WERDEN!
    /// </summary>
    /// <returns>die Verbindung</returns>
    /// <exception cref="NotImplementedException">Diese Methode muss im konkreten Controller überschrieben werden!</exception>
    public virtual IConnection GetConnection()
    {
        throw new NotImplementedException("Diese Methode muss im konkreten Controller überschrieben werden!");
    }

    #region AFUD

    #region AFEATE
    /// <summary>
    /// Ein neues Model anlegen.
    ///
    /// Optional kann ein Model übergeben werden, von dem die Daten kopiert/übernommen werden sollen
    /// (Ausnahme: alle KeyFields (PK, Created etc.) werden NICHT kopiert).
    /// </summary>
    /// <param name="fromModel">Model, von dem kopiert werden soll</param>
    /// <returns>das neue Model</returns>
    public virtual TModel Create(TModel? fromModel = null)
    {
        TModel ret = new();

        if (fromModel != null)
            ret.CopyFrom(fromModel, false);

        ret.CreateDateTime = DateTime.Now;
        ret.UpdateDateTime = DateTime.Now;

        return ret;
    }
    #endregion

    #region UPDATE/SAVE

    /// <summary>
    /// Speichert ein Model.
    /// 
    /// Standardmässig wird das Model, wenn es ITable implementiert in der Datenbank gespeichert.
    /// 
    /// Diese Methode muss überschrieben werden, wenn das NICHT das Standardverhalten sein soll oder das Model ITable nicht implementiert.
    /// </summary>
    /// <param name="model">zu speicherndes Model</param>
    /// <param name="connection">Datenbankverbindung (optional, wenn NULL wir neue Verbindung erzeugt)</param>
    /// <param name="fields">optional: nur den Inhalt dieser Felder speichern. Wird ignoriert, wenn der PK des Models Guid.Empty ist.</param>
    /// <param name="silent">optional: true, wenn alle Datenbankereignisse (Workflow etc.) unterdrückt werden sollen</param>
    /// <returns>true, wenn erfolgreich, sonst false</returns>
    public virtual CommandResult Save<TRecord>(TRecord model, IConnection? connection = null, string[]? fields = null, bool silent = false) where TRecord : class, TModel, ITable
    {
        bool success;

        if (connection == null)
        {
            using var localconn = GetConnection();

            localconn.Silent = silent;
            success = model.PrimaryKey.Equals(Guid.Empty) ? localconn.Save(model) : localconn.Save(model, fields);
        }
        else
        {
            connection.Silent = silent;
            success = model.PrimaryKey.Equals(Guid.Empty) ? connection.Save(model) : connection.Save(model, fields);
        }

        if (success)
            model.CommitChanges();

        return CommandResult.Success("Die Änderungen wurden gespeichert.");

    }

    /// <summary>
    /// Speichert eine Liste von Models.
    /// </summary>
    /// <param name="models">zu speichernde Models</param>
    /// <param name="connection">Datenbankverbindung (optional, wenn NULL wir neue Verbindung erzeugt)</param>
    /// <param name="fields">optional: nur den Inhalt dieser Felder speichern. Wird ignoriert, wenn der PK des Models Guid.Empty ist.</param>
    /// <param name="silent">optional: true, wenn alle Datenbankereignisse (Workflow etc.) unterdrückt werden sollen</param>
    /// <returns>true, wenn erfolgreich, sonst false</returns>
    public virtual CommandResult Save<TRecord>(IEnumerable<TRecord> models, IConnection? connection = null, string[]? fields = null, bool silent = false) where TRecord : class, TModel, ITable
    {
        CommandResult? result = null;

        foreach (var model in models)
        {
            result = Save(model, connection, fields, silent);

            if (result.Result == eNotificationType.Error) break;
        }

        return (result?.Result == eNotificationType.Error ? CommandResult.Error("Die Änderungen konnten NICHT gespeichert werden.") : CommandResult.Success("Die Änderungen wurden gespeichert."));
    }
    #endregion

    #region DELETE

    /// <summary>
    /// Ein einzelnes Model anhand seiner ID löschen.
    ///
    /// Standardmässig wird das Model, wenn es ITable implementiert aus der Datenbank gelöscht.
    ///
    /// Diese Methode muss überschrieben werden, wenn das NICHT das Standardverhalten sein soll oder das Model ITable nicht implementiert. 
    /// </summary>
    /// <param name="modelID">ID/PK des löschenden Models</param>
    /// <param name="connection">optional: Datenbankverbindung (wenn NULL wird neue Verbindung erzeugt)</param>
    /// <param name="silent">optional: true, wenn alle Datenbankereignisse (Workflow etc.) unterdrückt werden sollen</param>
    /// <returns>true, wenn das Model gelöscht wurde</returns>
    public virtual CommandResult Delete<TRecord>(Guid modelID, IConnection? connection = null, bool silent = false) where TRecord : class, TModel, ITable, new()
    {
        bool success;

        if (modelID.Equals(Guid.Empty)) return CommandResult.Error("Neue Objekte müssen nicht gelöscht werden.");

        if (connection == null)
        {
            using var localconn = GetConnection();
            localconn.Silent = silent;

            success = localconn.Delete<TRecord>(modelID);
        }
        else
        {
            connection.Silent = silent;

            success = connection.Delete<TRecord>(modelID);
        }

        return (success ? CommandResult.Success("Das Objekt wurde gelöscht.") : CommandResult.Error("Objekt konnte nicht gelöscht werden."));

    }

    /// <summary>
    /// Ein einzelnes Model löschen.
    ///
    /// Standardmässig wird das Model, wenn es ITable implementiert aus der Datenbank gelöscht.
    ///
    /// Diese Methode muss überschrieben werden, wenn das NICHT das Standardverhalten sein soll oder das Model ITable nicht implementiert. 
    /// </summary>
    /// <param name="model">zu löschendes Model</param>
    /// <param name="connection">optional: Datenbankverbindung (wenn NULL wird neue Verbindung erzeugt)</param>
    /// <param name="silent">optional: true, wenn alle Datenbankereignisse (Workflow etc.) unterdrückt werden sollen</param>
    /// <returns>true, wenn das Model gelöscht wurde</returns>
    public virtual CommandResult Delete<TRecord>(TRecord model, IConnection? connection = null, bool silent = false) where TRecord : class, TModel, ITable
    {
        bool success;

        if (model.PrimaryKey.Equals(Guid.Empty)) return CommandResult.Error("Neue Objekte müssen nicht gelöscht werden.");
                                                  
        if (connection == null)
        {
            using var localconn = GetConnection();

            localconn.Silent = silent;

            success = localconn.Delete(model);
        }
        else
        {
            connection.Silent = silent;

            success = connection.Delete(model);
        }

        return (success ? CommandResult.Success("Das Objekt wurde gelöscht.") : CommandResult.Error("Objekt konnte nicht gelöscht werden."));
    }

    /// <summary>
    /// Eine Liste von Models löschen.
    /// </summary>
    /// <param name="models">zu speichernde Models</param>
    /// <param name="connection">optional: Datenbankverbindung (wenn NULL wird neue Verbindung erzeugt)</param>
    /// <param name="silent">optional: true, wenn alle Datenbankereignisse (Workflow etc.) unterdrückt werden sollen</param>
    /// <returns>true, wenn erfolgreich, sonst false</returns>
    public virtual CommandResult Delete<TRecord>(IEnumerable<TRecord> models, IConnection? connection = null, bool silent = false) where TRecord : class, TModel, ITable
    {
        CommandResult? result = null;

        foreach (var model in models)
        {
            result = Delete(model, connection);

            if (result.Result == eNotificationType.Error) break;
        }

        return (result?.Result == eNotificationType.Error ? CommandResult.Error("Die Objekts konnten nicht gelöscht werden.") : CommandResult.Success("Die Objekte wurde gelöscht."));

    }
    #endregion

    #region READ
    /// <summary>
    /// Einen einzelnen Wert des Models lesen.
    ///
    /// <example>
    /// <code>
    /// var firmenname = controller.ReadValue&gt;string&lt;(firma.SYS_ID, nameof(Firma.FIR_NAME));
    /// </code>
    /// </example>
    /// </summary>
    /// <typeparam name="TValue">Typ des zu lesenden Wertes</typeparam>
    /// <param name="primaryKey">ID der Models</param>
    /// <param name="field">Name der Eigenschaft, deren Wert gelesen werden soll</param>
    /// <returns>der gelesene Wert oder NULL</returns>
    public virtual TValue? ReadValue<TValue>(Guid primaryKey, string field)
    {
        if (!typeof(TModel).IsAssignableTo(typeof(ITable))) return default;

        using var connection = GetConnection();

        return (TValue?)connection.InvokeGeneric(nameof(IConnection.LoadValue), [typeof(TModel)], primaryKey, field);

    }

    /// <summary>
    /// Liest Daten aus einer beliebigen View der Datenbank.
    /// 
    /// Kann vor allem genutzt werden, um auf View-Daten zuzugreifen
    /// </summary>
    /// <typeparam name="TData">Typ des Views</typeparam>
    /// <param name="options">Optionen</param>
    /// <param name="query">Filterbedingung/Query</param>
    /// <param name="args">Argumente für die Filterbedingung</param>
    /// <returns>Liste der gelesenen Daten (kann leer sein)</returns>
    public virtual BindingList<TData> ReadAny<TData>(ReadOptions? options = null, string? query = null, params object[]? args) where TData : class, IDataObject, new()
    {
        return ReadAny<TData>(archived: false, options: options, query: query, args: args);
    }

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
    public virtual BindingList<TData> ReadAny<TData>(bool archived = false, ReadOptions? options = null, string? query = null, params object[]? args) where TData : class, IDataObject, new()
    {
        var fieldArchived = typeof(TData).GetTypeDescription().FieldArchived;

        if (fieldArchived != null && archived == false)
        {
            if (query.IsEmpty())
            {
                query = $"{fieldArchived.Name} = ?";
                args = [archived];
            }
            else 
            {
                args ??= [];
                query = $"({query}) and {fieldArchived.Name} = ?";
                object[] newargs = new object[args.Length + 1];
                Array.Copy(args, newargs, args.Length);
                newargs[args.Length] = archived;

                args = newargs;
            }
        }

        using var connection = GetConnection();

        if (options == null && query == null)
            return connection.Select<TData>();

        if (query == null && options != null)
            return connection.Select<TData>(options);

        if (query != null && options == null)
            return connection.Select<TData>(query, args);

        if (options != null && query != null)
            return connection.Select<TData>(options, query, args);

        return [];
    }

    /// <summary>
    /// Liest das erste Model, auf das die Bedingungen passen.
    ///
    /// Standardmässig wird das Model, wenn es ITable implementiert aus der Datenbank gelesen.
    ///
    /// Diese Methode muss überschrieben werden, wenn das NICHT das Standardverhalten sein soll oder das Model ITable nicht implementiert.
    /// </summary>
    /// <param name="query">Abfrage/Bedingungen</param>
    /// <param name="args">Argumente für die Abfrage</param>
    /// <returns>das Model oder NULL, wenn kein Model den Bedingungen entspricht</returns>
    public virtual TModel? ReadFirst(string query, params object[]? args)
    {
        return readFirst<TModel>(query, args);
    }

    private TData? readFirst<TData>(string query, params object[]? args) where TData : class, IDataObject, new()
    {
        if (typeof(TData).IsAssignableTo(typeof(ITable)))
        {
            using var connection = GetConnection();

            if (args != null && args.Length > 0)
                return connection.SelectSingle<TData>(query, args);

            return connection.SelectSingle<TData>(query);
        }

        return null;
    }
        
    /// <summary>
    /// List der Models lesen.
    /// 
    /// Standardmässig wird das Model, wenn es ITable implementiert aus der Datenbank gelesen.
    ///
    /// Diese Methode muss überschrieben werden, wenn das NICHT das Standardverhalten sein soll oder das Model ITable nicht implementiert.
    /// </summary>
    /// <param name="options">Optionen für das Lesen</param>
    /// <param name="query">Abfrage/Bedingungen</param>
    /// <param name="args">Parameter für die Abfrage</param>
    /// <returns>Liste der gelesenen Objekte</returns>
    public virtual BindingList<TModel> Read(ReadOptions? options = null, string? query = null, params object[]? args)
    {
        return read<TModel>(options, query, (args == null || args.Length < 1) ? null : args);
    }

    private BindingList<TData> read<TData>(ReadOptions? options = null, string? query = null, params object[]? args) where TData : class, IDataObject, new()
    {
        if (args != null && args.Length < 1)
            args = null;
        
        using var connection = GetConnection();

        if (options != null && query != null && args != null)
            return connection.Select<TData>(options, query, args);

        if (options == null && query != null && args != null)
            return connection.Select<TData>(query, args);

        if (options == null && query != null && args == null)
            return connection.Select<TData>(query);

        if (options != null && query == null && args == null)
            return connection.Select<TData>(options);

        if (options == null && query == null && args == null)
            return connection.Select<TData>();

        return [];
    }


    /// <summary>
    /// Liste der im Browser darstellbaren Models
    /// </summary>
    /// <param name="archived"></param>
    /// <param name="filter"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public virtual IBindingList ReadBrowserModels(bool archived, string? filter = null, params object[]? args)
    {
        return Read<TModel>(eGridStyle.Browser, options: null, query: filter, args: args);
    }

    /// <summary>
    /// Im Browser darstellbares Model mit der gegebenen ID laden.
    /// </summary>
    /// <param name="primaryKey">ID des Models</param>
    /// <returns></returns>
    public virtual IModel? ReadBrowserModel(Guid primaryKey)
    {
        return ReadSingle<TModel>(primaryKey);
    }

    /// <summary>
    /// Typ des Models, dass zur Auswahl verwendet werden soll.
    /// 
    /// Das kann statt des TModel auch ein TViewModel sein, muss dann aber in nachfolgenden Controllern überschrieben werden.
    /// </summary>
    /// <returns></returns>
    public virtual Type GetSelectionModelType() { return typeof(TModel); }

    /// <summary>
    /// Gibt an, ob Filterbedingungen für die Auswahl verwendet werden können.
    /// 
    /// In der Regel ist der Wert true, muss aber für Models, die eigene Controls zur Auswahl anbieten ggf. auf false gesetzt werden.
    /// </summary>
    public virtual bool SupportSelectionFilter => true;

    /// <summary>
    /// Suche aus der SearchEngine ausführen.
    /// </summary>
    /// <param name="query">Abfrage</param>
    /// <param name="para">Parameter</param>
    /// <param name="all">alle Treffer ausgeben</param>
    /// <returns>Liste der gefundenen Einträge</returns>
    public virtual IBindingList Search(bool all, string query, object[] para)
    {
        return new BindingList<IModel>();
    }

    /// <summary>
    /// Liste der als Detail darstellbaren Models vom Typ TDetail.
    /// 
    /// Ist für das Feld, das die ID des Masters enthält ein ConstraintType definiert (dringend empfohlen!), 
    /// liefert die Methode eine Liste der Models, die in diesem Feld die ID des Masters enthält.
    /// </summary>
    /// <typeparam name="TDetails">Typ der zu lesenden Details</typeparam>
    /// <param name="master">Typ des Masters</param>
    /// <param name="masterid">DI des Masters</param>
    /// <param name="filter">Filterbedingung (Query)</param>
    /// <param name="args">Parameter für die Filterbedingungen</param>
    /// <returns>Liste der Models</returns>
    public virtual IBindingList ReadDetailModels<TDetails>(Type master, Guid masterid, string? filter = null, object[]? args = null) where TDetails : class, IDataObject, new()
    {
        var fieldMasterID = ControlledType.GetTypeDescription().Fields.Values.Where(f => f.Field != null && f.Field.ConstraintType != null && f.Field.ConstraintType == master).ToArray();

        if (fieldMasterID.Any() == false || masterid.Equals(Guid.Empty))
            return new BindingList<TDetails>();

        args ??= [];

        var sbFilter = StringBuilderPool.GetStringBuilder();
        List<object> newargslist = [];

        newargslist.AddRange(args);

        foreach (var field in fieldMasterID)
        {
            if (sbFilter.Length > 0)
                sbFilter.Append(" or ");

            sbFilter.Append($"{field.Name} = ?");
            newargslist.Add(masterid);
        }

        filter = filter.IsEmpty() ? $"({sbFilter})" : $"({filter}) and ({sbFilter})";
        StringBuilderPool.ReturnStringBuilder(sbFilter);
       
        return Read<TDetails>(eGridStyle.Full, options: null, query: filter, args: newargslist.ToArray());
    }

    /// <summary>
    /// Liste der als Detail darstellbaren Models.
    /// 
    /// Ist für das Feld, das die ID des Masters enthält ein ConstraintType definiert (dringend empfohlen!), 
    /// liefert die Methode eine Liste der Models, die in diesem Feld die ID des Masters enthält.
    /// </summary>
    /// <param name="master">Typ des Masters</param>
    /// <param name="masterid">DI des Masters</param>
    /// <param name="filter">Filterbedingung (Query)</param>
    /// <param name="args">Parameter für die Filterbedingungen</param>
    /// <returns>Liste der Models</returns>
    public IBindingList ReadDetailModels(Type master, Guid masterid, string? filter = null, params object[]? args) 
    {
        return ReadDetailModels<TModel>(master, masterid, filter, args);
    }

    /// <summary>
    /// Ein einzelnes, als Detail darstellbares Model laden, dessen ID bekannt ist.
    /// </summary>
    /// <param name="master">Typ des aktuellen Masters</param>
    /// <param name="detailid">ID des zu ladenden Details</param>
    /// <param name="filter">Filterbedingung (WHERE)</param>
    /// <param name="args">Argumente für den Filter (Parameter)</param>
    /// <returns>Das Detail-Model oder NULL</returns>
    public virtual IModel? ReadDetailModel<TDetail>(Type master, Guid detailid, string? filter = null, params object[]? args) where TDetail : class, IDataObject, new()
    {
        return ReadSingle<TDetail>(detailid);
    }

    /// <summary>
    /// Ein einzelnes, als Detail darstellbares Model laden, dessen ID bekannt ist.
    /// </summary>
    /// <param name="master">Typ des aktuellen Masters</param>
    /// <param name="detailid">ID des zu ladenden Details</param>
    /// <param name="filter">Filterbedingung (WHERE)</param>
    /// <param name="args">Argumente für den Filter (Parameter)</param>
    /// <returns>Das Detail-Model oder NULL</returns>
    public virtual IModel? ReadDetailModel(Type master, Guid detailid, string? filter = null, params object[]? args)
    {
        return ReadDetailModel<TModel>(master, detailid, filter, args);
    }

    /// <summary>
    /// Details eines Masters laden und TRUE zurückgeben, wenn die Details geladen wurden.
    ///
    /// Gibt diese Methode FALSE zurück, werden die Detail-Models anschließend via ReadDetailModels geladen.
    /// Diese Methode wird in der Regel verwendet, wenn der DetailView Details darstellt, die sich DIREKT auf
    /// den Master beziehen (z.B. QueryDesigner für Datenquelle, WorkFlowDesigner für einen Workflow etc.). 
    /// </summary>
    /// <param name="master">Master, auf den sich die Details beziehen sollen</param>
    /// <param name="detailview">DetailView, der die Daten aufnimmt.</param>
    /// <param name="typeDetails">Typ der anzuzeigenden Details</param>
    /// <returns>true, wenn Details geladen wurden, sonst false (Standard = false)</returns>
    public virtual bool LoadDetails(IModel? master, IViewDetail detailview, TypeDescription typeDetails)
    {
        return false;
    }

    /// <summary>
    /// Liest ein einzelnes Objekt von TModel mit der angegebenen ID.
    /// </summary>
    /// <param name="primaryKey">ID des zu lesenden Modells</param>
    /// <returns>das Modell oder null</returns>
    public TModel? Load(Guid primaryKey)
    {
        using var conn = GetConnection();
        return conn.Load<TModel>(primaryKey);
    }

    /// <summary>
    /// Liste der auswählbaren Models (z.B. via Combobox)
    /// </summary>
    /// <param name="archived">archivierte anzeigen </param>
    /// <param name="filter">Filterbedingungen (Query)</param>
    /// <param name="args">Parameter für Query</param>
    /// <returns>Liste der Models für die Auswahl</returns>
    public virtual IBindingList ReadSelectionModels(bool archived = false, string? filter = null, params object[]? args)
    {
        var fieldArchived = ControlledType.GetTypeDescription().FieldArchived;

        if (fieldArchived == null)
            return Read<TModel>(eGridStyle.ComboboxEntrys, options: null, query: filter, args: args);

        args ??= [];
        object[] newargs;

        if (filter.IsEmpty() && archived == false)
        {
            filter = $"{fieldArchived.Name} = ?";
            newargs = [archived];
            return Read<TModel>(eGridStyle.ComboboxEntrys, options: null, query: filter, args: newargs);
        }
        else if (filter.IsNotEmpty() && archived == false)
        {
            filter = $"({filter}) and {fieldArchived.Name} = ?";
            newargs = new object[args.Length + 1];
            Array.Copy(args, newargs, args.Length);
            newargs[args.Length] = archived;
            return Read<TModel>(eGridStyle.ComboboxEntrys, options: null, query: filter, args: newargs);
        }

        return Read<TModel>(eGridStyle.ComboboxEntrys, options: null, query: filter, args: args);
    }

    /// <summary>
    /// Aktuelles ausgewähltes Model (z.B. in Combobox) lesen
    /// </summary>
    /// <param name="id">ID des Models</param>
    /// <param name="filter">Filterbedingung, dem das Model entsprecchen muss (null = kein Filter)</param>
    /// <param name="args">Parameter für die Filterbedingungen</param>
    /// <returns>einzelnes Model mit der angegeben ID oder NULL wenn kein Model mit der ID existiert 
    /// oder dieses nicht dem Filter entspricht.</returns>
    public virtual IDataObject? ReadSelectionModel(Guid id, string? filter = null, params object[]? args)
    {
        filter ??= string.Empty;
        args ??= [];

        using var conn = GetConnection();

        if (filter.IsEmpty())
            return conn.Load<TModel>(id);

        filter = $"{typeof(TModel).GetTypeDescription().FieldKey!.Name} = ?  and ({filter})";
        object[] newargs = [id];
        ArrayEx.Merge(newargs, args);

        return conn.SelectSingle<TModel>(filter, newargs);
    }

    #region Icontroller Read
    /// <summary>
    /// Typ des Models (Table)
    /// </summary>
    public Type ModelType => typeof(TModel);

    /// <summary>
    /// Typ des 'Large' Models (meist View)
    /// </summary>
    public Type ModelLargeType => typeof(TModel);

    /// <summary>
    /// Typ des 'Small'Models (meist View)
    /// </summary>
    public Type ModelSmallType => typeof(TModel);

    /// <summary>
    /// Liste aller vom Controller verwalteten Objekte lesen
    /// </summary>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    public BindingList<TModel> ReadList()
    {
        using var conn = GetConnection();
        return conn.Select<TModel>();
    }

    /// <summary>
    /// Liste der vom Controller verwalteten Objekte lesen
    /// </summary>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    public BindingList<TModel> ReadList(string query, params object[] args)
    {
        using var conn = GetConnection();
        return conn.Select<TModel>(query, args);
    }

    /// <summary>
    /// Liste der vom Controller verwalteten Objekte lesen
    /// </summary>
    /// <param name="options">Optionen</param>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    public BindingList<TModel> ReadList(ReadOptions? options = null, string? query = null, params object[] args)
    {
        using var conn = GetConnection();
        return conn.Select<TModel>(options ?? new(), query ?? "", args);
    }

    /// <summary>
    /// Einzelnes, vom Controller verwaltetes Objekt lesen (erstes Objekt, dass den Bedingungen entspricht)
    /// </summary>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    public TModel? ReadSingle(string query, params object[] args)
    {
        using var conn = GetConnection();
        return conn.SelectSingle<TModel>(query, args);
    }

    /// <summary>
    /// Einzelnes, vom Controller verwaltetes Objekt lesen (erstes Objekt, dass den Bedingungen entspricht)
    /// </summary>
    /// <param name="options">Optionen</param>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    public TModel? ReadSingle(ReadOptions? options = null, string? query = null, params object[] args)
    {
        using var conn = GetConnection();
        return conn.SelectSingle<TModel>(options ?? new(), query ?? "", args);
    }

    /// <summary>
    /// Liste aller ausführlichen Objekte (meist aus View) lesen, die der Controller verwaltet.
    /// </summary>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt vorhanden sind)</returns>
    public BindingList<TModel> ReadLargeList()
    {
        using var conn = GetConnection();
        return conn.Select<TModel>();
    }

    /// <summary>
    /// Liste der ausführlichen Objekte (meist aus View) lesen, die der Controller verwaltet.
    /// </summary>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    public BindingList<TModel> ReadLargeList(string query, params object[] args)
    {
        using var conn = GetConnection();
        return conn.Select<TModel>(query, args);
    }

    /// <summary>
    /// Liste der ausführlichen Objekte (meist aus View) lesen, die der Controller verwaltet.
    /// </summary>
    /// <param name="options">Optionen</param>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    public BindingList<TModel> ReadLargeList(ReadOptions? options = null, string? query = null, params object[] args)
    {
        using var conn = GetConnection();
        return conn.Select<TModel>(options ?? new(), query ?? "", args);
    }

    /// <summary>
    /// Einzelnes ausführliches Objekt (meist aus View), vom Controller verwaltetes Objekt lesen (erstes Objekt, dass den Bedingungen entspricht)
    /// </summary>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    public TModel? ReadLargeSingle(string query, params object[] args)
    {
        using var conn = GetConnection();
        return conn.SelectSingle<TModel>(query, args);
    }

    /// <summary>
    /// Einzelnes ausführliches Objekt (meist aus View), vom Controller verwaltetes Objekt lesen (erstes Objekt, dass den Bedingungen entspricht)
    /// </summary>
    /// <param name="options">Optionen</param>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    public TModel? ReadLargeSingle(ReadOptions? options = null, string? query = null, params object[] args)
    {
        using var conn = GetConnection();
        return conn.SelectSingle<TModel>(options ?? new(), query ?? "", args);
    }

    /// <summary>
    /// Einzelnes ausführliches Objekt (meist aus View), vom Controller verwaltetes Objekt lesen (anhand des PrimaryKey des Objekts)
    /// </summary>
    /// <param name="guid">PrimaryKey des Objekts</param>
    /// <returns>das geladene Objekt oder NULL</returns>
    public TModel? LoadLarge(Guid guid)
    {
        using var conn = GetConnection();
        return conn.Load<TModel>(guid);
    }

    /// <summary>
    /// Liste aller kleinen Objekte (meist aus View) lesen, die der Controller verwaltet.
    /// </summary>
    /// <returns>Liste der Objekte (leere Liste, wenn keine Objekte vorhanden sind)</returns>
    public BindingList<TModel> ReadSmallList()
    {
        using var conn = GetConnection();
        return conn.Select<TModel>();
    }

    /// <summary>
    /// Liste der kleinen Objekte (meist aus View) lesen, die der Controller verwaltet.
    /// </summary>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    public BindingList<TModel> ReadSmallList(string query, params object[] args)
    {
        using var conn = GetConnection();
        return conn.Select<TModel>(query, args);
    }

    /// <summary>
    /// Liste der kleinen Objekte (meist aus View) lesen, die der Controller verwaltet.
    /// </summary>
    /// <param name="options">Optionen</param>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    public BindingList<TModel> ReadSmallList(ReadOptions? options = null, string? query = null, params object[] args)
    {
        using var conn = GetConnection();
        return conn.Select<TModel>(options ?? new(), query ?? "", args);
    }

    /// <summary>
    /// Einzelnes kleines Objekt (meist aus View), vom Controller verwaltetes Objekt lesen (erstes Objekt, dass den Bedingungen entspricht)
    /// </summary>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    public TModel? ReadSmallSingle(string query, params object[] args)
    {
        using var conn = GetConnection();
        return conn.SelectSingle<TModel>(query, args);
    }

    /// <summary>
    /// Einzelnes kleines Objekt (meist aus View), vom Controller verwaltetes Objekt lesen (erstes Objekt, dass den Bedingungen entspricht)
    /// </summary>
    /// <param name="options">Optionen</param>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    public TModel? ReadSmallSingle(ReadOptions? options = null, string? query = null, params object[] args)
    {
        using var conn = GetConnection();
        return conn.SelectSingle<TModel>(options ?? new(), query ?? "", args);
    }

    /// <summary>
    /// Einzelnes kleines Objekt (meist aus View), vom Controller verwaltetes Objekt lesen (anhand des PrimaryKey des Objekts)
    /// </summary>
    /// <param name="guid">PrimaryKey des Objekts</param>
    /// <returns>das geladene Objekt oder NULL</returns>
    public TModel? LoadSmall(Guid guid)
    {
        using var conn = GetConnection();
        return conn.Load<TModel>(guid);
    }


    /// <summary>
    /// Liste beliebiger Objekte über die Datenbankverbindung des Controllers lesen, die der Controller verwaltet.
    /// 
    /// ACHTUNG! Diese Methode sollte nur in Ausnahmefällen (z.B. für zusätzliche Views neben 'Small' und 'Large' verwendet werden. 
    /// Ansonsten ist der Controller des betreffenden Objekt-Typs zu bevorzugen!
    /// </summary>
    /// <returns>Liste der Objekte (leere Liste, wenn keine Objekte vorhanden sind)</returns>
    public BindingList<T> ReadAnyList<T>() where T : class, IDataObject, new()
    {
        using var conn = GetConnection();
        return conn.Select<T>();
    }

    /// <summary>
    /// Liste beliebiger Objekte über die Datenbankverbindung des Controllers lesen, die der Controller verwaltet.
    /// 
    /// ACHTUNG! Diese Methode sollte nur in Ausnahmefällen (z.B. für zusätzliche Views neben 'Small' und 'Large' verwendet werden. 
    /// Ansonsten ist der Controller des betreffenden Objekt-Typs zu bevorzugen!
    /// </summary>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    public BindingList<T> ReadAnyList<T>(string query, params object[] args) where T : class, IDataObject, new()
    {
        using var conn = GetConnection();
        return conn.Select<T>(query, args);
    }

    /// <summary>
    /// Liste beliebiger Objekte über die Datenbankverbindung des Controllers lesen, die der Controller verwaltet.
    /// 
    /// ACHTUNG! Diese Methode sollte nur in Ausnahmefällen (z.B. für zusätzliche Views neben 'Small' und 'Large' verwendet werden. 
    /// Ansonsten ist der Controller des betreffenden Objekt-Typs zu bevorzugen!
    /// </summary>
    /// <param name="options">Optionen</param>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    public BindingList<T> ReadAnyList<T>(ReadOptions? options = null, string? query = null, params object[] args) where T : class, IDataObject, new()
    {
        using var conn = GetConnection();
        return conn.Select<T>(options ?? new(), query ?? "", args);
    }

    /// <summary>
    /// Einzelnes beliebiges Objekt über die Datenbankverbindung des Controllers lesen (erstes Objekt, dass den Bedingungen entspricht)
    /// 
    /// ACHTUNG! Diese Methode sollte nur in Ausnahmefällen (z.B. für zusätzliche Views neben 'Small' und 'Large' verwendet werden. 
    /// Ansonsten ist der Controller des betreffenden Objekt-Typs zu bevorzugen!
    /// </summary>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    public T? ReadAnySingle<T>(string query, params object[] args) where T : class, IDataObject, new()
    {
        using var conn = GetConnection();
        return conn.SelectSingle<T>(query, args);
    }

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
    public T? ReadAnySingle<T>(ReadOptions? options = null, string? query = "", params object[] args) where T : class, IDataObject, new()
    {
        using var conn = GetConnection();
        return conn.SelectSingle<T>(options ?? new(), query ?? "", args);
    }

    /// <summary>
    /// Beliebiges Objekt über die Datenbankverbindung des Controllers lesen (anhand des PrimaryKey des Objekts)
    /// 
    /// ACHTUNG! Diese Methode sollte nur in Ausnahmefällen (z.B. für zusätzliche Views neben 'Small' und 'Large' verwendet werden. 
    /// Ansonsten ist der Controller des betreffenden Objekt-Typs zu bevorzugen!
    /// </summary>
    /// <param name="guid">PrimaryKey des Objekts</param>
    /// <returns>das geladene Objekt oder NULL</returns>
    public T? LoadAny<T>(Guid guid) where T : class, IDataObject, new()
    {
        using var conn = GetConnection();
        return conn.Load<T>(guid);
    }

    #endregion
    #endregion
    #endregion

    #region Command-Support

    /// <summary>
    /// Holt einen bestimmten Befehl
    /// </summary>
    /// <param name="type">Befehlstyp</param>
    /// <param name="context">Kontext, in dem der Befehl verwendet werden kann</param>
    /// <param name="checkRights">Rechte prüfen, falls der Benutzer keine hat, null zurückgeben</param>
    /// <returns>der Befehl oder null</returns>
    public virtual AFCommand? GetCommand(eCommand type, eCommandContext context, bool checkRights)
    {
        if (!_commandsReaded) mapControllerCommands();

        if (_commands.Count < 1) return null;

        var command = _commands.Values.FirstOrDefault(c => c.CommandType == type && c.CommandContext.HasFlag(context)) ??
               _commands.Values.FirstOrDefault(c => c.CommandType == type && c.CommandContext.HasFlag(eCommandContext.EveryWhere));

        if (command == null) return null;

        if (checkRights && !command.HasRight()) return null;

        return command;
    }

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
    public virtual AFCommand? GetCommand(string name)
    {
        if (!_commandsReaded) mapControllerCommands();

        _commands.TryGetValue(name, out var command);
     
        if (command != null && command.HasRight())
            return command;

        return null;
    }

    /// <summary>
    /// Ein Command anhand des Namens identifizieren und ausführen.
    ///
    /// Beinhaltet die Berechtigungsprüfung, die bei direkter Ausführung nicht zwingend erfolgt!
    /// </summary>
    /// <param name="name">Name des Commands (der Methode)</param>
    /// <param name="data">Parameter für die Ausführung</param>
    /// <returns>Ergebnis der Ausführung</returns>
    public CommandResult ExecuteCommand(string name, CommandArgs data)
    {
        return GetCommand(name)?.Execute(data) ?? CommandResult.Error($"Befehl {name} steht nicht zur Verfügung (nicht vorhanden oder fehlende Berechtigung).");
    }

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
    public virtual AFCommand? GetCommand(eCommand commandType)
    {
        if (!_commandsReaded) mapControllerCommands();

        return _commands.Values.FirstOrDefault(c => c.CommandType == commandType && c.HasRight());
    }

    /// <summary>
    /// Liefert ein Array mit allen Commands des Controllers.
    /// </summary>
    /// <returns>Array der Commands (kann leer sein!)</returns>
    public AFCommand[] GetCommands()
    {
        if (!_commandsReaded)
            mapControllerCommands();

        return _commands.Count < 1 ? [] : _commands.Values.Where(c => c.HasRight()).OrderBy(c => c.Order).ToArray();
    }

    /// <summary>
    /// Liefert ein Array mit allen Commands des Controllers für einen bestimmten Kontext (Verwendungszweck)
    /// </summary>
    /// <param name="context">Kontext, dessen Commands benötigt werden</param>
    /// <param name="viewContext">Kontext für den die Commands benötigt werden</param>
    /// <returns>Array der Commands (kann leer sein!)</returns>
    public virtual AFCommand[] GetCommands(eCommandContext context, string viewContext = "")
    {
        if (!_commandsReaded)
            mapControllerCommands();

        if (_commands.Count < 1)
            return [];

        if (string.IsNullOrEmpty(viewContext))
            return _commands.Values.Where(c => (c.CommandContext.HasFlag(context) || c.CommandContext.HasFlag(eCommandContext.EveryWhere) && c.HasRight() ))
                .OrderBy(c => c.Order).ToArray();

        return _commands.Values.Where(c => (c.CommandContext.HasFlag(context) || c.CommandContext.HasFlag(eCommandContext.EveryWhere)) && (c.VisibiltyContext == viewContext || string.IsNullOrEmpty(c.VisibiltyContext)) && c.HasRight())
            .OrderBy(c => c.Order).ToArray();

    }

    /// <summary>
    /// Liefert ein Array mit allen Commands des Controllers für einen bestimmten Kontext (Verwendungszweck)
    /// </summary>
    /// <param name="context">Kontext, dessen Commands benötigt werden</param>
    /// <param name="viewContext">Kontext für den die Commands benötigt werden</param>
    /// <param name="model">Datenobjekt, für die die Commands benötigt werden</param>
    /// <returns>Array der Commands (kann leer sein!)</returns>
    public virtual AFCommand[] GetCommands(eCommandContext context, string viewContext = "", IModel? model = null)
    {
        return GetCommands(context, viewContext);
    }


    private void mapControllerCommands()
    {
        // Attribute aus der Controller-Klasse in die TypeDescription abbilden.
        // Methoden als Befehle auslesen
        var methods = GetType().GetMethods();

        foreach (var method in methods)
        {
            object[] commands = method.GetCustomAttributes(typeof(AFCommand), true);

            if (commands.Length == 0) continue;

            if (commands.Length > 1)
                throw new ArgumentException(string.Format(CoreStrings.ERR_CTRL_TOMANYCOMMANDS, method.Name));

            if (method.IsPublic == false || method.IsStatic)
                throw new Exception(string.Format(CoreStrings.ERR_CTRL_WRONGMETHOD, method.Name, GetType().Name));

            if (method.GetParameters().Length < 1 ||
                method.GetParameters()[0].ParameterType == typeof(CommandArgs) == false)
                throw new Exception(string.Format(CoreStrings.ERR_CTRL_WRONGPARAMETER, method.Name, GetType().Name));

            if (method.ReturnParameter == null ||
                method.ReturnParameter.ParameterType == typeof(CommandResult) == false)
                throw new Exception(string.Format(CoreStrings.ERR_CTRL_WRONGRETURN, method.Name, GetType().Name));

            if (_commands.ContainsKey(method.Name))
                throw new Exception(string.Format(CoreStrings.ERR_CTRL_WRONGNAME, method.Name, GetType().Name));

            ((AFCommand)commands[0]).Command =
                (Func<CommandArgs, CommandResult>)Delegate.CreateDelegate(
                    typeof(Func<CommandArgs, CommandResult>), this, method);

            ((AFCommand)commands[0]).SetController(this, method.Name);


            _commands.Add(method.Name, (AFCommand)commands[0]);

        }

        _commandsReaded = true;
    }
    #endregion

    #region UI
    /// <summary>
    /// Abbruch einer via Dialog ausführbaren Aktion
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public CommandResult ProcessCloseDialog(CommandArgs args)
    {
        if (args.Page != null) args.Page.ClosePropertyDialog();

        return CommandResult.None;
    }

    /// <summary>
    /// Liefert den Typ des benötigten CustomEditors für das angegebene UI-Element
    /// </summary>
    /// <param name="property">Eigenschaft/Spalte</param>
    /// <param name="element">Element, dass den Editor benötigt (Grid, Einstellungsdialog etc.)</param>
    /// <returns>Typ des Editors oder NULL</returns>
    public virtual Type? GetCustomEditorType(PropertyDescription property, eUIElement element) { return null; }

    /// <summary>
    /// Liefert den benötigten CustomEditors für das angegebene UI-Element
    /// </summary>
    /// <param name="property">Eigenschaft/Spalte</param>
    /// <param name="element">Element, dass den Editor benötigt (Grid, Einstellungsdialog etc.)</param>
    /// <returns>Typ des Editors oder NULL</returns>
    public virtual object? GetCustomEditor(PropertyDescription property, eUIElement element) { return null; }
    
    /// <summary>
    /// Setup des Elements
    /// </summary>
    /// <param name="element">Element, dass konfiguriert wird</param>
    /// <param name="page">Page in der das Element dargestellt wird</param>
    /// <param name="mastertype">MasterType</param>
    /// <param name="detailtype">Typ der Details</param>
    public virtual void SetupUIElement(IUIElement element, IViewPage page, Type? mastertype = null, Type? detailtype = null) { }

    /// <summary>
    /// Bookmarks erlauben
    /// </summary>
    public virtual bool AllowBookmarks { get; set; }

    /// <summary>
    /// Darstellung im Verlauf erlauben
    /// </summary>
    public virtual bool AllowHistory { get; set; }

    /// <summary>
    /// Suche via SearchEngine erlauben
    /// </summary>
    public virtual bool AllowSearch { get; set; }

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
    public virtual AFGridSetup GetGridSetup(eGridStyle style, Type? mastertype = null, Type? detailtype = null, string[]? fields = null, eGridMode gridtype = eGridMode.GridView, string? stylename = null)
    {
        if (detailtype == null)
            detailtype = typeof(TModel);

        var setup = detailtype.GetTypeDescription().GetGridSetup(style, mastertype, detailtype, fields, gridtype, true);

        setup.ModelType = ControlledType;

        if (style == eGridStyle.Browser)
        {
            setup.CmdGoto = GetCommand(eCommand.Goto, (style.HasFlag(eGridStyle.ComboboxEntrys) ? eCommandContext.ComboBox : eCommandContext.GridButton), true);
            return setup;
        }
        
        if (style == eGridStyle.SearchHits)
        {
            setup.CmdGoto = GetCommand(eCommand.Goto, (style.HasFlag(eGridStyle.SearchHits) ? eCommandContext.ComboBox : eCommandContext.GridButton), true);
            return setup;
        }

        setup.CmdGoto = GetCommand(eCommand.Goto, (style.HasFlag(eGridStyle.ComboboxEntrys) ? eCommandContext.ComboBox : eCommandContext.GridButton), true);
        setup.CmdAdd = GetCommand(eCommand.New, (style.HasFlag(eGridStyle.ComboboxEntrys) ? eCommandContext.ComboBox : eCommandContext.GridButton), true);
        setup.CmdDelete = GetCommand(eCommand.Delete, (style.HasFlag(eGridStyle.ComboboxEntrys) ? eCommandContext.ComboBox : eCommandContext.GridButton), true);
        setup.CmdEdit = GetCommand(eCommand.Edit, (style.HasFlag(eGridStyle.ComboboxEntrys) ? eCommandContext.ComboBox : eCommandContext.GridButton), true);
        setup.CmdShowDetail = GetCommand(eCommand.ShowDetails, (style.HasFlag(eGridStyle.ComboboxEntrys) ? eCommandContext.ComboBox : eCommandContext.GridButton), true);
        setup.GridIdentifier = mastertype == null ? Guid.Empty : GetDetailIdentifier(mastertype, detailtype);
        setup.Symbol = TypeImage;

        setup.AllowAddNew = setup.CmdAdd != null;
        // setup.AllowEdit = setup.CmdEdit != null;

        return setup;
    }

    /// <summary>
    /// Liefert eine eindeutige ID für das DetailView eines bestimmten Masters.
    ///
    /// Diese ID wird verwendet, um Einstellungen des DetailViews zu persistieren.
    /// </summary>
    /// <param name="master">Typ des Masters, für den das DetailView angezeigt wird.</param>
    /// <param name="detailtype">Typ der Details</param>
    /// <returns>die GUID/ID des DetailViews</returns>
    public virtual Guid GetDetailIdentifier(Type master, Type? detailtype)
    {
        return Guid.Empty;
    }


    /// <summary>
    /// Gibt eine Liste von Typ-Beschreibungen zurück, die untergeordnete Modelle dieses Controllers sind.
    /// </summary>
    /// <returns>Liste der Childs oder null, wenn kein Child vorhanden</returns>
    public virtual TypeDescription[]? GetChildTypes() { return null; }

    /// <summary>
    /// Liefert den Child-/Detail-Typ für den angegebenen MasterType.
    /// </summary>
    /// <param name="mastertype">Typ des Masters</param>
    /// <returns>Child-/Detailtyp oder NULL</returns>
    public virtual Type? GetChildType(Type mastertype) { return null; }

    /// <summary>
    /// Gibt den Typ der primären Child-/Detail-Daten zurück.
    /// </summary>
    /// <returns>Typ des primären Detail-Typen</returns>
    public virtual Type? GetPrimaryChildType() { return null; }

    /// <summary>
    /// Liefert ein UIElement oder NULL, wenn der Controller das Element nicht zur Verfügung stellt.
    /// </summary>
    /// <param name="type">Typ des benötigten UIElements</param>
    /// <param name="mastertype">Typ des Masters, der gerade aktiv ist</param>
    /// <param name="detailtype">Typ der Details</param>
    /// <param name="page">Page, die das Element benötigt. 
    /// Bei einigen UI-Elementen muss hier das Page-Objekt übergeben werden! (Detail, PluginDetail, FooterDetail, HeaderDetail, Editor)(</param>
    /// <returns>das UI Element oder NULL</returns>
    public virtual IUIElement? GetUIElement(eUIElement type, Type? mastertype = null, Type? detailtype = null, IViewPage ? page = null)
    {


        return null;
    }

    /// <summary>
    /// Liefert den Typen eines UIElement oder NULL, wenn der Controller das Element nicht zur Verfügung stellt.
    /// </summary>
    /// <param name="type">Typ des benötigten UIElements</param>
    /// <param name="mastertype">Typ des Masters, der gerade aktiv ist</param>
    /// <param name="detailtype">Typ der Details</param>
    /// <param name="page">Page, die das Element benötigt. 
    /// Bei einigen UI-Elementen muss hier das Page-Objekt übergeben werden! (Detail, PluginDetail, FooterDetail, HeaderDetail, Editor)(</param>
    /// <returns>Type des UI Element oder NULL</returns>
    public virtual Type? GetUIElementType(eUIElement type, Type? mastertype = null, Type? detailtype = null, IViewPage? page = null)
    {
        return null;
    }

    /// <summary>
    /// Template für die Darstellung eines Suchtreffers in den Suchergebnissen.
    /// </summary>
    public virtual AFHtmlTemplate SearchHitHtmlTemplate => controllerConstants.defaultSearchHitHtmlTemplate;
    
    /// <summary>
    /// Template für die Darstellung der Schaltfläche 'Mehr' in den Suchergebnissen.
    /// </summary>
    public virtual AFHtmlTemplate SearchMoreHtmlTemplate => controllerConstants.defaultSearchMoreHtmlTemplate;
    #endregion

    #region non generic
    /// <summary>
    /// Liest ein einzelnes Objekt von TViewModel mit der angegebenen ID.
    /// 
    /// Das IViewModel muss als 
    /// </summary>
    /// <param name="primaryKey">ID des zu lesenden Modells</param>
    /// <returns>das Modell oder null</returns>
    public TData? ReadSingle<TData>(Guid primaryKey) where TData : class, IDataObject, new()
    {
        using var conn = GetConnection();
        return conn.Load<TData>(primaryKey);
    }

    /// <summary>
    /// Liest ein einzelnes Objekt von TViewModel mit der angegebenen ID.
    /// 
    /// Das IViewModel muss als 
    /// </summary>
    /// <param name="primaryKey">ID des zu lesenden Modells</param>
    /// <returns>das Modell oder null</returns>
    public IModel? ReadSingle(Guid primaryKey)
    {
        using var conn = GetConnection();
        return conn.InvokeGeneric(nameof(IConnection.Load), [typeof(TModel)], primaryKey) as IModel;
    }


    /// <summary>
    /// Liest das erste Objekt von TModel, das der Abfrage entspricht
    /// </summary>
    /// <param name="style">Stil/Umfang der Daten, der benötigt wird</param>
    /// <param name="query">Abfrage (where-Klausel), die ausgeführt wird, um die Modelle zu erhalten</param>
    /// <param name="args">Parameter für diese Abfrage</param>
    /// <returns>das Modell oder null</returns>
    public TData? ReadFirst<TData>(eGridStyle style, string query, params object[] args) where TData : class, IDataObject, new()
    {
        return readFirst<TData>(query, args);
    }

    /// <summary>
    /// Ermittelt eine Liste aller Objekte des Typs TModel, die der Abfrage entsprechen
    /// </summary>
    /// <param name="style">Stil/Umfang der Daten, der benötigt wird</param>
    /// <param name="query">Abfrage (where-Klausel), die ausgeführt wird, um die Modelle zu erhalten</param>
    /// <param name="args">Parameter für diese Abfrage</param>
    /// <param name="options">Abfrageoptionen</param>
    /// <returns>eine Liste der gelesenen Modelle</returns>
    public BindingList<TData> Read<TData>(eGridStyle style, ReadOptions? options = null, string? query = null, params object[]? args) where TData : class, IDataObject, new()
    {
        return read<TData>(options, query, args);
    }

    /// <summary>
    /// Prüft, ob ein Model ein Child-Model des Masters ist.
    ///
    /// Wird z.B. verwendet, um zu prüfen, ob ein Model im DetailView zu einem bestimmten Master gehört.
    ///
    /// Die Methode muss im konkreten Controller überschrieben werden!
    /// </summary>
    /// <param name="child"></param>
    /// <param name="masterID"></param>
    /// <returns>true, wenn es ein Child-Objekt ist, sonst false</returns>
    public virtual bool IsChildOf(IModel child, Guid masterID) { return false; }

    /// <summary>
    /// Erstellt aus den übergebenen Informationen ein ModelInfo-Objekt.
    /// 
    /// Diese Methode kann in konkreten Controllern überschrieben werden,
    /// um die übergebenen Daten für die Ansicht aufzubereiten.
    /// </summary>
    /// <param name="modelInfo">anzupassende ModelInfo</param>
    /// <returns>ModelInfo für das Model</returns>
    public virtual void CustomizeModelInfo(ModelInfo modelInfo) {  }

    #endregion

    #region KPI Dashboard
    /// <summary>
    /// Standard-Konfiguration des KPI-Dashbaords.
    /// 
    /// Liefert eine Konfiguration mit den ersten maximal 20 Elementen. 
    /// </summary>
    /// <returns></returns>
    public AFDashboardKPIConfig GetDefaultKPIDashboard()
    {
        AFDashboardKPIConfig ret = new();
        int cnt = 0;

        foreach (var prop in typeof(TModel).GetTypeDescription().Properties.Values.Where(p => p.KPIElement != null).OrderBy(p => p.KPIElement!.ID))
        {
            ++cnt;

            if (cnt > 20) break;

            ret.Elements.Add(new AFDashboardKPIConfigElement() { ID = prop.KPIElement!.ID });
        }

        return ret;
    }

    /// <summary>
    /// Liefert eine Liste der für das KPI verfügbaren ELemente.
    /// </summary>
    public BindingList<CustomizableElement> GetAvailableKPIDashboardElements()
    {
        BindingList<CustomizableElement> elements = [];

        foreach (var prop in typeof(TModel).GetTypeDescription().Properties.Values.Where(p => p.KPIElement != null).OrderBy(p => p.KPIElement!.ID))
        {
            elements.Add(new() { Caption = prop.KPIElement!.Caption.IsNotEmpty() 
                ? prop.KPIElement!.Caption!
                : prop.Context?.NameSingular ?? prop.Name,
                Description = prop.Context?.Description ?? "", 
                ID = prop.KPIElement!.ID });
        }

        return elements;
    }

    /// <summary>
    /// Liefert das befüllte Model für das KPI-Dashboard
    /// </summary>
    /// <param name="model">Model, dass die daten enthält</param>
    /// <param name="id">ID des Dashboards</param>
    /// <param name="name">Name des Dashboards</param>
    /// <returns>das befüllte Model</returns>
    public AFDashboardKPIModel GetKPIDashboardModel(object model, Guid id, string name)
    {
        AFDashboardKPIModel kpimodel = new();

        var config = GetKPIDashboardConfig(id, name);
        var tdesc = model.GetType().GetTypeDescription();

        foreach (var element in config.Elements)
        {
            var prop = tdesc.Properties.Values.FirstOrDefault(p => p.KPIElement != null && p.KPIElement.ID == element.ID);

            if (prop == null) continue;

            AFDashboardKPIElementModel kpielement = new()
            {
                Caption = prop.KPIElement!.Caption.IsNotEmpty()
                    ? prop.KPIElement!.Caption!
                    : prop.Context?.NameSingular ?? prop.Name,
                Title = prop.Context?.NameSingular ?? prop.Name,
                Description = prop.Context?.Description ?? string.Empty
            };

            var currvalue = tdesc.Accessor[model, prop.Name];
            kpielement.Value = GetKPIDashboardValue(currvalue, prop);
            kpielement.Indicator = GetKPIDashboardColor(currvalue, prop);

            kpimodel.Elements.Add(kpielement);
        }

        return kpimodel;
    }

    /// <summary>
    /// Liefert die Konfiguration für das KPI-Dashboard des Benutzers.
    /// 
    /// Muss im konkreten Controller überschreiben werden!
    /// </summary>
    /// <param name="id">ID des Dashboards</param>
    /// <param name="name">Name des Dashboards</param>
    /// <returns>Konfiguration für das Dashboard</returns>
    public virtual AFDashboardKPIConfig GetKPIDashboardConfig(Guid id, string name)
    { throw new Exception("Die GetKPIDashboardConfig Methode muss im konkreten Controller des Typs, für das das KPI-Dashboard gilt überschrieben werden."); }

    /// <summary>
    /// Liefert die Farbe zur Hervorhebung eiones eines Wertes in einem KPI-Dashboard.
    /// 
    /// Standardformatierung ist Color.Transparent (keine Farbe).
    /// 
    /// Diese Methode kann überschreiben werden um spezielle Farben - z.B. abhängig vom Wert zu ermöglichen.
    /// </summary>
    /// <param name="value">Wert</param>
    /// <param name="property">Eigenschaft aus der der Wert stammt</param>
    /// <returns>formatierter Text für den Wert</returns>
    public virtual Color GetKPIDashboardColor(object value, PropertyDescription property)
    {
        return Color.Transparent;
    }

    /// <summary>
    /// Liefert den formatierten Text zur Anzeige eines Wertes in einem KPI-Dashboard.
    /// 
    /// Standardformatierung ist fett. DisplayFormat des Propertys wird beachtet (Binding vor GridColumn).
    /// 
    /// Diese Methode kann überschreiben werden um spezielle Formatierungen zu ermöglichen (farbig etc.).
    /// </summary>
    /// <param name="value">Wert</param>
    /// <param name="property">Eigenschaft aus der der Wert stammt</param>
    /// <returns>formatierter Text für den Wert</returns>
    public virtual string GetKPIDashboardValue(object? value, PropertyDescription property)
    {
        if (value == null) return "n.b.";

        string? format = property.Binding?.DisplayFormat;
        string strval = "";

        if (format.IsEmpty()) format = property.GridColumn?.DisplayFormat;

        if (value is DateTime date)
            strval = date.ToString(format.IsNotEmpty() ? format : "d");
        else if (value is DateOnly dateonly)
            strval = dateonly.ToString(format.IsNotEmpty() ? format : "d");
        else if (value is TimeOnly timeonly)
            strval = timeonly.ToString(format.IsNotEmpty() ? format : "t");
        else if (value is bool boolean)
            strval = (boolean ? "JA" : "nein");
        else if (value is int intval)
            strval = intval.ToString(format.IsNotEmpty() ? format : "f0");
        else if (value is short shortval)
            strval = shortval.ToString(format.IsNotEmpty() ? format : "f0");
        else if (value is long longval)
            strval = longval.ToString(format.IsNotEmpty() ? format : "f0");
        else if (value is decimal decval)
            strval = decval.ToString(format.IsNotEmpty() ? format : "f0");
        else if (value is double doubleval)
            strval = doubleval.ToString(format.IsNotEmpty() ? format : "f0");
        else if (value is float floatval)
            strval = floatval.ToString(format.IsNotEmpty() ? format : "f0");
        else
            strval = value?.ToString() ?? "n.b.";

        return $"<b>{strval}</b>";
    }


    #endregion
}
