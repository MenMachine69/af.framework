using System.Resources;

namespace AF.MVC;

/// <summary>
/// Beschreibung einer ausführbaren Methode eines Controllers
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AFCommand : Attribute, IMenuEntry
{
    private string _long = "", _hint = "", _short = "", _caption = "";
    private string _commandName = "";
    private IController? _controller;
    private readonly ResourceManager? _resourceManager;

    /// <summary>
    /// Ausführen des Befehls...
    ///
    /// Dies sollte die einzige Möglichkeit sein, einen Befehl auszuführen!
    /// </summary>
    /// <param name="parameter">Parameter für den Befehl</param>
    /// <returns>Ergebnis der Befehlsausführung</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public CommandResult Execute(CommandArgs parameter)
    {
        if (Command == null)
        {
            throw new ArgumentNullException(
                string.Format(CoreStrings.ERR_CTRL_COMMANDNOTAVAILABLE, _commandName,
                    _controller != null ? _controller.GetType().FullName : CoreStrings.ERR_UNKNOWNCONTROLLER), nameof(Command));
        }

        // check user rights
        if (AFCore.App.SecurityService == null) return Command(parameter);

        if ((NeedAdminRights && !(AFCore.App.SecurityService.CurrentUser?.IsAdmin ?? false)) ||
            (NeededRight > 0 && !(AFCore.App.SecurityService.HasRight(NeededRight))))
            return CommandResult.Error(CoreStrings.NOTALLOWED);

        return Command(parameter);
    }

    /// <summary>
    /// Überprüft, ob der aktuelle Benutzer die Berechtigung besitzt, das Command auszuführen.
    /// </summary>
    /// <returns></returns>
    public bool HasRight()
    {
        if (AFCore.App.SecurityService == null) return true;

        if (AFCore.App.SecurityService.CurrentUser == null) return false;

        if ((NeedAdminRights && !AFCore.App.SecurityService.CurrentUser.IsAdmin) || 
            (NeededRight > 0 && !AFCore.App.SecurityService.HasRight(NeededRight)))
            return false;

        return true;
    }

    /// <summary>
    /// Überprüft, ob der angegebene Benutzer die Berechtigung besitzt, das Command auszuführen.
    /// </summary>
    /// <returns></returns>
    public bool HasRight(IUser user)
    {
        if (AFCore.App.SecurityService == null) return true;

        

        if ((NeedAdminRights && !user.IsAdmin) ||
            (NeededRight > 0 && !AFCore.App.SecurityService.HasRight(NeededRight)))
            return false;

        return true;
    }

    /// <summary>
    /// Befehlsdelegat zur direkten Ausführung des Befehls.
    /// </summary>
    public Func<CommandArgs, CommandResult>? Command { get; set; }

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="resourceType">Typ der Ressource für sprachspezifische Zeichenfolgen</param>
    public AFCommand(Type resourceType) 
    {
        _resourceManager = new ResourceManager(resourceType);
    }

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="caption">Name/Darstellung (z.B. Menüüberschrift)</param>
    public AFCommand(string caption) 
    {
        _caption = caption;
    }

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="caption">Name/Darstellung (z.B. Menü-Überschrift)</param>
    /// <param name="resourceType">Typ der Ressource für sprachspezifische Zeichenketten</param>
    public AFCommand(string caption, Type resourceType) 
    {
        _caption = caption;
        _resourceManager = new ResourceManager(resourceType);
    }

    /// <summary>
    /// benötigt administrative Rechte
    /// </summary>
    public bool NeedAdminRights { get; set; }

    /// <summary>
    /// Erfordert eine bestimmte Berechtigung
    ///
    /// Beachten Sie, dass Werte kleiner oder gleich 100 für das System reserviert sind!
    /// Verwenden Sie sie nicht in Ihren eigenen Controllern!
    /// </summary>
    public int NeededRight { get; set; } = -1;

    /// <summary>
    /// Start einer Gruppe (z.B. ein Trennlinie in Menüs)
    /// </summary>
    public bool Group { get; set; } = false;

    /// <summary>
    /// Reihenfolge der Befehle im Menü. -1 bedeutet: keine Reihenfolge
    /// </summary>
    public int Order { get; set; } = -1;

    /// <summary>
    /// Kontexte, in denen der Befehl sichtbar ist
    /// </summary>
    public eCommandContext CommandContext { get; set; } = eCommandContext.Nowhere;

    /// <summary>
    /// Befehlstyp
    /// </summary>
    public eCommand CommandType { get; set; } = eCommand.Other;

    /// <summary>
    /// Zusatzinformation zu CommandContext.
    ///
    /// Über den VisiblityContext kann die Sichtbarkeit von Commands in einem Context weiter gegliedert werden.
    /// Beispiel: Master mit mehreren Views (z.B. als TABs), bei dem Commands nur für den bestimmten View angezeigt werden sollen.
    /// </summary>
    public string VisibiltyContext { get; set; } = string.Empty;

    /// <summary>
    /// Kurzbeschreibung des Befehls (Titel in Tooltips etc.)
    /// </summary>
    public virtual string Short
    {
        get
        {
            if (_resourceManager == null)
                return _short;

            string? shortdesc = _resourceManager.GetString(_short);
            return string.IsNullOrWhiteSpace(shortdesc) ? _short : shortdesc;
        }
        set => _short = value;
    }

    /// <summary>
    /// Lange Beschreibung des Befehls (Beschreibung in Tooltips etc.)
    /// </summary>
    public virtual string Long
    {
        get
        {
            if (_resourceManager == null)
                return _long;

            string? longdesc = _resourceManager.GetString(_long);
            return string.IsNullOrWhiteSpace(longdesc) ? _long : longdesc;
        }
        set => _long = value;
    }
    
    /// <inheritdoc />
    public string? Description => Long;

    /// <summary>
    /// Tipp Beschreibung des Befehls (Hinweis in Tooltips, etc.)
    /// </summary>
    public virtual string Hint
    {
        get
        {
            if (_resourceManager == null)
                return _hint;

            string? hintdesc = _resourceManager.GetString(_hint);
            return string.IsNullOrWhiteSpace(hintdesc) ? _hint : hintdesc;
        }
        set => _hint = value;
    }


    /// <summary>
    /// Beschriften Sie den Befehl. Verwenden Sie \, um Unterbefehle zu erstellen (z.B. DATEI \ DATEI ÖFFNEN ...)
    /// </summary>
    public virtual string Caption
    {
        get
        {
            if (_resourceManager == null)
                return _caption;

            string? caption = _resourceManager.GetString(_caption);
            return string.IsNullOrWhiteSpace(caption) ? _caption : caption;
        }
        set => _caption = value;
    }

    
    
    /// <summary>
    /// Weisen Sie den Controller und die Befehlsmethode dem Befehl zu
    /// </summary>
    /// <param name="controller">der Controller</param>
    /// <param name="methodName">Controller-Methode</param>
    internal void SetController(IController controller, string methodName)
    {
        _controller = controller;
        _commandName = methodName;
    }

    #region IMenuEntry


    /// <inheritdoc />
    public eKeys HotKey { get; set; }

    /// <inheritdoc />
    public bool Toggle { get; set; }

    /// <inheritdoc />
    public int FunctionKey { get; set; }
    
    /// <inheritdoc />
    public object? Image { get; set; }

    /// <inheritdoc />
    public object? GroupImage { get; set; }

    /// <inheritdoc />
    public int ImageIndex { get; set; } = -1;

    /// <inheritdoc />
    public object? Tag 
    { 
        get => this;
        set => throw new ArgumentException("Commands kann kein Tag zugewiesen werden.");
    }

    /// <inheritdoc />
    public bool BeginGroup { get => Group; set => Group = value; }

    /// <inheritdoc />
    public string Name => _commandName;
    #endregion
}