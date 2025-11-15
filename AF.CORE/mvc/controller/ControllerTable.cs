namespace AF.MVC;

/// <summary>
/// BaseController für alle ITable-Modelle
/// </summary>
/// <typeparam name="TModel">Typ des Tabellenmodells (Klasse, die die ITable-Schnittstelle implementiert)</typeparam>
/// <typeparam name="TModelSmallView"></typeparam>
/// <typeparam name="TModelLargeView"></typeparam>
public abstract class ControllerTable<TModel, TModelLargeView, TModelSmallView> : Controller<TModel>, IControllerTable<TModel, TModelLargeView, TModelSmallView>
    where TModel : class, ITable, new()
    where TModelLargeView : class, IDataObject, new()
    where TModelSmallView : class, IDataObject, new()
{
    /// <summary>
    /// Typ des Models (Table)
    /// </summary>
    public new Type ModelType => typeof(TModel);

    /// <summary>
    /// Typ des 'Large' Models (meist View)
    /// </summary>
    public new Type ModelLargeType => typeof(TModelLargeView);

    /// <summary>
    /// Typ des 'Small'Models (meist View)
    /// </summary>
    public new Type ModelSmallType => typeof(TModelSmallView);
        
    /// <summary>
    /// Liste aller vom Controller verwalteten Objekte lesen
    /// </summary>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    public new BindingList<TModel> ReadList()
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
    public new BindingList<TModel> ReadList(string query, params object[] args)
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
    public new BindingList<TModel> ReadList(ReadOptions? options = null, string? query = null, params object[] args)
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
    public new TModel? ReadSingle(string query, params object[] args)
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
    public new TModel? ReadSingle(ReadOptions? options = null, string? query = null, params object[] args)
    {
        using var conn = GetConnection();
        return conn.SelectSingle<TModel>(options ?? new(), query ?? "", args);
    }

    /// <summary>
    /// Liste aller ausführlichen Objekte (meist aus View) lesen, die der Controller verwaltet.
    /// </summary>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt vorhanden sind)</returns>
    public new BindingList<TModelLargeView> ReadLargeList()
    {
        using var conn = GetConnection();
        return conn.Select<TModelLargeView>();
    }

    /// <summary>
    /// Liste der ausführlichen Objekte (meist aus View) lesen, die der Controller verwaltet.
    /// </summary>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    public new BindingList<TModelLargeView> ReadLargeList(string query, params object[] args)
    {
        using var conn = GetConnection();
        return conn.Select<TModelLargeView>(query, args);
    }

    /// <summary>
    /// Liste der ausführlichen Objekte (meist aus View) lesen, die der Controller verwaltet.
    /// </summary>
    /// <param name="options">Optionen</param>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    public new BindingList<TModelLargeView> ReadLargeList(ReadOptions? options = null, string? query = null, params object[] args)
    {
        using var conn = GetConnection();
        return conn.Select<TModelLargeView>(options ?? new(), query ?? "", args);
    }

    /// <summary>
    /// Einzelnes ausführliches Objekt (meist aus View), vom Controller verwaltetes Objekt lesen (erstes Objekt, dass den Bedingungen entspricht)
    /// </summary>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    public new TModelLargeView? ReadLargeSingle(string query, params object[] args)
    {
        using var conn = GetConnection();
        return conn.SelectSingle<TModelLargeView>(query, args);
    }

    /// <summary>
    /// Einzelnes ausführliches Objekt (meist aus View), vom Controller verwaltetes Objekt lesen (erstes Objekt, dass den Bedingungen entspricht)
    /// </summary>
    /// <param name="options">Optionen</param>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    public new TModelLargeView? ReadLargeSingle(ReadOptions? options = null, string? query = null, params object[] args)
    {
        using var conn = GetConnection();
        return conn.SelectSingle<TModelLargeView>(options ?? new(), query ?? "", args);
    }

    /// <summary>
    /// Einzelnes ausführliches Objekt (meist aus View), vom Controller verwaltetes Objekt lesen (anhand des PrimaryKey des Objekts)
    /// </summary>
    /// <param name="guid">PrimaryKey des Objekts</param>
    /// <returns>das geladene Objekt oder NULL</returns>
    public new TModelLargeView? LoadLarge(Guid guid)
    {
        using var conn = GetConnection();
        return conn.Load<TModelLargeView>(guid);
    }

    /// <summary>
    /// Liste aller kleinen Objekte (meist aus View) lesen, die der Controller verwaltet.
    /// </summary>
    /// <returns>Liste der Objekte (leere Liste, wenn keine Objekte vorhanden sind)</returns>
    public new BindingList<TModelSmallView> ReadSmallList()
    {
        using var conn = GetConnection();
        return conn.Select<TModelSmallView>();
    }

    /// <summary>
    /// Liste der kleinen Objekte (meist aus View) lesen, die der Controller verwaltet.
    /// </summary>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    public new BindingList<TModelSmallView> ReadSmallList(string query, params object[] args)
    {
        using var conn = GetConnection();
        return conn.Select<TModelSmallView>(query, args);
    }

    /// <summary>
    /// Liste der kleinen Objekte (meist aus View) lesen, die der Controller verwaltet.
    /// </summary>
    /// <param name="options">Optionen</param>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    public new BindingList<TModelSmallView> ReadSmallList(ReadOptions? options = null, string? query = null, params object[] args)
    {
        using var conn = GetConnection();
        return conn.Select<TModelSmallView>(options ?? new(), query ?? "", args);
    }

    /// <summary>
    /// Einzelnes kleines Objekt (meist aus View), vom Controller verwaltetes Objekt lesen (erstes Objekt, dass den Bedingungen entspricht)
    /// </summary>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    public new TModelSmallView? ReadSmallSingle(string query, params object[] args)
    {
        using var conn = GetConnection();
        return conn.SelectSingle<TModelSmallView>(query, args);
    }

    /// <summary>
    /// Einzelnes kleines Objekt (meist aus View), vom Controller verwaltetes Objekt lesen (erstes Objekt, dass den Bedingungen entspricht)
    /// </summary>
    /// <param name="options">Optionen</param>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    public new TModelSmallView? ReadSmallSingle(ReadOptions? options = null, string? query = null, params object[] args)
    {
        using var conn = GetConnection();
        return conn.SelectSingle<TModelSmallView>(options ?? new(), query ?? "", args);
    }

    /// <summary>
    /// Einzelnes kleines Objekt (meist aus View), vom Controller verwaltetes Objekt lesen (anhand des PrimaryKey des Objekts)
    /// </summary>
    /// <param name="guid">PrimaryKey des Objekts</param>
    /// <returns>das geladene Objekt oder NULL</returns>
    public new TModelSmallView? LoadSmall(Guid guid)
    {
        using var conn = GetConnection();
        return conn.Load<TModelSmallView>(guid);
    }


    /// <summary>
    /// Steuert, wie die Details angezeigt werden.
    /// 
    /// Standard ist ePageDetailMode.NoDetails. Wenn die Seite Detailansichten haben soll, 
    /// muss diese Eigenschaft überschrieben werden.
    /// </summary>
    public override ePageDetailMode DetailViewMode => ePageDetailMode.Default;

    /// <inheritdoc />
    public override AFGridSetup GetGridSetup(eGridStyle style, Type? mastertype = null, Type? detailtype = null, string[]? fields = null, eGridMode gridtype = eGridMode.GridView, string? stylename = null)
    {
        if (detailtype != null && mastertype != null)
            return base.GetGridSetup(style, mastertype, detailtype, fields, gridtype, stylename: stylename);

        Type modeltype = typeof(TModelLargeView);

        if (style is eGridStyle.Browser or
            eGridStyle.ComboboxEntrys or
            eGridStyle.Small or
            eGridStyle.SearchHits or
            eGridStyle.Treeview)

            modeltype = typeof(TModelSmallView);
        
        return base.GetGridSetup(style, mastertype, modeltype, fields, gridtype, stylename: stylename);
    }

    /// <inheritdoc />
    public override Type GetGridModelType(eGridStyle style, Type? mastertype = null)
    {
        if (style is eGridStyle.ComboboxEntrys or
            eGridStyle.Browser or eGridStyle.Small or
            eGridStyle.Treeview or eGridStyle.SearchHits)
            return typeof(TModelSmallView);

        return typeof(TModelLargeView);
    }

    /// <inheritdoc />
    public override Type GetSelectionModelType() { return typeof(TModelSmallView); }

    /// <inheritdoc />
    public override IDataObject? ReadSelectionModel(Guid id, string? filter = null, params object[]? args)
    {
        filter ??= string.Empty;
        args ??= [];

        using var conn = GetConnection();
        
        if (filter.IsEmpty())
            return conn.Load<TModelSmallView>(id);

        filter = $"{typeof(TModelSmallView).GetTypeDescription().FieldKey!.Name} = ?  and ({filter})";
        object[] newargs = [id];
        ArrayEx.Merge(newargs, args);
        
        return conn.SelectSingle<TModelSmallView>(filter, newargs);
    }

    /// <summary>
    /// Liste der auswählbaren Models (z.B. via Combobox)
    /// </summary>
    /// <param name="archived">archivierte anzeigen </param>
    /// <param name="filter">Filterbedingungen (Query)</param>
    /// <param name="args">Parameter für Query</param>
    /// <returns>Liste der Models für die Auswahl</returns>
    public override IBindingList ReadSelectionModels(bool archived = false, string? filter = null, params object[]? args)
    {
        return ReadAny<TModelSmallView>(archived: archived, options: null, query: filter, args: args);
    }


    /// <summary>
    /// Liste der im Browser darstellbaren Models
    /// </summary>
    /// <param name="archived"></param>
    /// <param name="filter"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public override IBindingList ReadBrowserModels(bool archived, string? filter = null, params object[]? args)
    {
        return ReadAny<TModelSmallView>(archived: archived, options: null, query: filter, args: args);
    }

    /// <summary>
    /// Im Browser darstellbares Model mit der gegebenen ID laden.
    /// </summary>
    /// <param name="primaryKey">ID des Models</param>
    /// <returns></returns>
    public override IModel? ReadBrowserModel(Guid primaryKey)
    {
        return ReadSingle<TModelSmallView>(primaryKey);
    }



    #region Standard-Commands (i.d.R. MasterPopup)
    /// <summary>
    /// ID eines Models in die Zwischenablage kopieren
    /// </summary>
    /// <param name="data">Parameter</param>
    /// <returns>Resultat</returns>
    [AFCommand("RÜCKGÄNGIG MACHEN", Long = "Macht alle aktuellen Änderungen am Datensatz rückgängig.", CommandContext = eCommandContext.MasterPopup, CommandType = eCommand.Other)]
    public CommandResult CmdUndoChanges(CommandArgs data)
    {
        if (data.Page?.ViewEditor?.Model is not BaseBuffered model) return CommandResult.Warning("Keine Änderungen verfügbar, die rückgängig gemnacht werden können.");

        model.RollBackChanges();

        return CommandResult.Success("Änderungen rückgängig gemacht.");
    }

    /// <summary>
    /// Model neu laden.
    /// </summary>
    /// <param name="data">Parameter</param>
    /// <returns>Resultat</returns>
    [AFCommand("NEU LADEN", Long = "Den Datensatz neu laden.", CommandContext = eCommandContext.MasterPopup, CommandType = eCommand.Other)]
    public CommandResult CmdReloadModel(CommandArgs data)
    {
        if (data.Page?.ViewEditor?.Model is not IDataObject model) return CommandResult.Warning("Das Objekt kann nicht neu geladen werden. Es stellt keinen Datensatz in einer Datenbank dar.");

        var conn = model.Database?.GetConnection();

        if (conn == null) return CommandResult.Error("Keine Datenbankverbindung verfügbar.");

        conn.InvokeGeneric(nameof(IConnection.ReLoad), [model.GetType()], model);

        data.Page?.ViewEditor?.RefreshDatasource();

        return CommandResult.Success("Daten wurden neu geladen.");
    }

    /// <summary>
    /// Lesezeichen setzen
    /// </summary>
    /// <param name="data">Parameter</param>
    /// <returns>Resultat</returns>
    [AFCommand("LESEZEICHEN SETZEN", Long = "Setzt ein Lesezeichen für den Datensatz, dass dann über die Lesezeichenliste jederzeit augerufen werden kann.", CommandContext = eCommandContext.MasterPopup, CommandType = eCommand.Other, NeedAdminRights = false)]
    public CommandResult CmdSetBookmark(CommandArgs data)
    {
        if (data.Model == null) return CommandResult.None;

        if (data.Model.GetType().GetController().AllowBookmarks == false)
            return CommandResult.Error("Lesezeichen sind für diesen Datentyp nicht erlaubt.");

        AFCore.App.ViewManager?.Bookmarks.Add(new ModelBookmark(data.Model.ModelLink.ModelType, data.Model.ModelLink.ModelID, data.Model.ModelLink.ModelCaption) { TypeName = data.Model.ModelLink.ModelType.GetTypeDescription().Name });

        return CommandResult.Success("Lesezeichen wurde gesetzt.");
    }

    /// <summary>
    /// ID eines Models in die Zwischenablage kopieren
    /// </summary>
    /// <param name="data">Parameter</param>
    /// <returns>Resultat</returns>
    [AFCommand("ID KOPIEREN", Long = "Kopiert die ID (GUID) des aktuellen Datensatzes in die Zwischenablage.", CommandContext = eCommandContext.MasterPopup, CommandType = eCommand.Other, NeedAdminRights = true)]
    public CommandResult CmdTableCopyID(CommandArgs data)
    {
        return CommandResult.Error("Nicht implementiert.");
    }

    /// <summary>
    /// Log für das Model anzeigen
    /// </summary>
    /// <param name="data">Parameter</param>
    /// <returns>Resultat</returns>
    [AFCommand("LOG ANZEIGEN", Long = "Zeigt das Log der Änderungen des aktuellen Datensatzes an (soweit verfügbar).", CommandContext = eCommandContext.MasterPopup, CommandType = eCommand.Other, NeedAdminRights = true, BeginGroup = true)]
    public CommandResult CmdTableShowLog(CommandArgs data)
    {
        return CommandResult.Error("Nicht implementiert.");
    }

    /// <summary>
    /// Inspektor für das Model anzeigen
    /// </summary>
    /// <param name="data">Parameter</param>
    /// <returns>Resultat</returns>
    [AFCommand("INSPEKTOR ANZEIGEN", Long = "Zeigt einen Inspektor für die Daten des aktuellen Datensatzes an.", CommandContext = eCommandContext.MasterPopup, CommandType = eCommand.Other, NeedAdminRights = true)]
    public CommandResult CmdTableInspektor(CommandArgs data)
    {
        if (data.Model == null) return CommandResult.Info("Es ist kein Objekt verfügbar, dass im Inspektor angezeigt werden kann.");

        using (var inspektor = MvcContext.GetInspektor()!)
        {
            inspektor.CurrentModel = data.Model;
            inspektor.ShowInspektor(data.ParentControl);
        }

        return CommandResult.None;
    }

    /// <summary>
    /// Model exportieren
    /// </summary>
    /// <param name="data">Parameter</param>
    /// <returns>Resultat</returns>
    [AFCommand("EXPORTIEREN...", Long = "Exportiert die Daten des aktuellen Datensatzes, so dass diese anschließend in einen anderen (z.B. neuen Datensatz) importiert werden können.", CommandContext = eCommandContext.MasterPopup, CommandType = eCommand.Other, NeedAdminRights = true, BeginGroup = true)]
    public CommandResult CmdTableExport(CommandArgs data)
    {
        return CommandResult.Error("Nicht implementiert.");
    }

    /// <summary>
    /// Daten eines Models importieren
    /// </summary>
    /// <param name="data">Parameter</param>
    /// <returns>Resultat</returns>
    [AFCommand("IMPORTIEREN...", Long = "Importiert die Daten eines exportierten Datensatzes in den aktuellen Datensatz.", CommandContext = eCommandContext.MasterPopup, CommandType = eCommand.Other, NeedAdminRights = true)]
    public CommandResult CmdTableImport(CommandArgs data)
    {
        return CommandResult.Error("Nicht implementiert.");
    }


    /// <summary>
    /// Speichern eine TModel
    /// </summary>
    /// <param name="data">Parameter</param>
    /// <returns>Resultat</returns>
    public virtual CommandResult CmdSave(CommandArgs data)
    {
        if (data.Model is not TModel model) return CommandResult.Error($"Es wurde kein {typeof(TModel).Name}-Objekt übergeben.");

        ValidationErrorCollection errors = [];

        if (data is { CommandContext: eCommandContext.MasterContext, CommandSource: IEditor editor})
        {
            if (data.Page is not null)
            {
                if (!data.Page.IsValid(errors)) return CommandResult.Error("Bitte überprüfen Sie die Eingaben.");
            }
            else
            {
                if (!editor.IsValid(errors)) return CommandResult.Error("Bitte überprüfen Sie die Eingaben.");
            }
        }
        else
        {
            if (!model.IsValid(errors)) return CommandResult.Error("Bitte überprüfen Sie die Eingaben.");
        }


        return base.Save(model);
    }

    /// <summary>
    /// Ein TModel löschen
    /// </summary>
    /// <param name="data">Parameter</param>
    /// <returns>Resultat</returns>
    public virtual CommandResult CmdDelete(CommandArgs data)
    {
        if (data.Model is not TModel model) return CommandResult.Error($"Es wurde kein {typeof(TModel).Name}-Objekt übergeben.");

        if (!model.CanDelete(out var reasonStore)) return CommandResult.Error($"{typeof(TModel).Name} kann nicht gelöscht werden. {reasonStore}");

        if (data is { CommandContext: eCommandContext.MasterContext, CommandSource: IEditor })
        {
            if (AFCore.App.ShowQuestionYesNo($"{typeof(TModel).Name.ToUpper()} LÖSCHEN\r\nMöchten Sie <b>{model}</b> jetzt wirklich löschen?") == eMessageBoxResult.No)
                return CommandResult.Warning("Das Löschen wurde abgebrochen.");
        }

        return base.Delete(model);
    }

    #endregion
}
