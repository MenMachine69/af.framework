using System.Data.SqlTypes;
using System.Reflection;
using FastMember;

namespace AF.CORE;

/// <summary>
/// Beschreibung eines Typs/einer Klasse mit vereinfachtem Zugriff auf die Eigenschaften. 
/// und zusätzlichen Informationen über sie (Attribute des Typs und Eigenschaften)
/// 
/// Auf diese Beschreibung kann über typeof(Example).GetTypeDescription() zugegriffen werden.
/// 
/// Voraussetzung: Der Typ muss die Schnittstelle IBindable implementieren!
/// 
/// Wenn der Typ eine statische Methode AfterRegisterTypeDescription enthält, wird diese automatisch aufgerufen und die 
/// und die TypeDescription wird an diese Methode übergeben. Auf diese Weise kann die TypeDescription 
/// z.B. weitere Informationen zu der TypeDescription hinzugefügt werden.
/// </summary>
public sealed class TypeDescription : IMenuEntry
{
    private readonly Dictionary<string, object> _extensions = [];
    private IController? _controller;
    private bool _controllerSearched;
    private bool _customGridSetupSearched;
    private MethodInfo? _customGridSetupMethod;

    private Dictionary<string, PropertyDescription>? _fields;
    private Dictionary<string, PropertyDescription>? _gridcolumns;
    private Dictionary<string, PropertyDescription>? _bindable;
    private readonly Dictionary<string, PropertyDescription> _modelInfoFields = [];
    private readonly PropertyDescription? _modelCaptionField;
    private readonly PropertyDescription? _modelKeyField;



    /// <summary>
    /// Konstrukteur
    /// 
    /// Dieser Konstruktor ist nur intern. Die TypeDescription wird durch typeof(Model).GetTypeDescription() erzeugt.
    /// </summary>
    /// <param name="type">beschriebener Typ</param>
    internal TypeDescription(Type type)
    {
        Type = type;

        Context = type.GetCustomAttribute<AFContext>(false);
        Table = type.GetCustomAttribute<AFTable>(false);
        SiblingTable = type.GetCustomAttribute<AFSiblingTable>(false);
        SiblingView = type.GetCustomAttribute<AFSiblingView>(false);
        View = type.GetCustomAttribute<AFView>(false);
        Options = type.GetCustomAttribute<AFOptions>(false);

        Accessor = TypeAccessor.Create(type);

        Context ??= new AFContext(Type.Name, Type.Name);

        if (Context.ResourceManager != null)
        {
            Context.NameSingular = Context.NameSingular.IsEmpty() ? (Context.RessourceName ?? Type.Name) + @"_SINGULAR" :Context.NameSingular;
            Context.NamePlural = Context.NamePlural.IsEmpty() ? (Context.RessourceName ?? Type.Name) + @"_PLURAL" : Context.NamePlural;
            Context.Description = Context.Description.IsEmpty() ? (Context.RessourceName ?? Type.Name) + @"_DESC" : Context.Description;
        }

        foreach (PropertyInfo property in type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.GetProperty |
                                                             BindingFlags.SetProperty | BindingFlags.Public |
                                                             BindingFlags.Instance))
        {
            var prop = PropertyDescription.Create(property, Table, View, SiblingTable, SiblingView);

            if (prop.Field?.ModelInfoData ?? false)
                _modelInfoFields.Add(property.Name, prop);

            if (prop.Field?.ModelInfoCaption ?? false)
                _modelCaptionField = prop;

            if (prop.Field?.SystemFieldFlag == eSystemFieldFlag.PrimaryKey)
                _modelKeyField =  prop;

            Properties.Add(property.Name, prop);
        }

        if (Options != null && Options.ControllerType == null)
            throw new ArgumentException($"Bei Verwendung des Attributs AFOptions muss der ControllerType angegeben werden ({type.FullName}).");

        // Überprüfung der Attribute
        if ((Table != null && View != null) || (SiblingTable != null && View != null))
            throw new ArgumentException(string.Format(CoreStrings.ERR_TYPECANNOTBETABLEANDVIEW, type.FullName));

        if (Table != null && SiblingTable != null)
            throw new ArgumentException(string.Format(CoreStrings.ERR_TYPECANNOTBETABLEANDVIEW, type.FullName));

        if (View != null && SiblingView != null)
            throw new ArgumentException(string.Format(CoreStrings.ERR_TYPECANNOTBETABLEANDVIEW, type.FullName));

        if (SiblingTable != null)
        {
            if (SiblingTable.TableId <= 0)
                throw new ArgumentException(string.Format(CoreStrings.ERR_MISSINGTABLEID, type.FullName));

            if (SiblingTable.SiblingType == null || SiblingTable.SiblingType == typeof(Nullable))
                throw new ArgumentException(string.Format(CoreStrings.ERR_MISSINGSIBLINGTYPE, type.FullName));

            var siblingtype = SiblingTable.SiblingType.GetTypeDescription();
            
            if (siblingtype == null)
                throw new ArgumentException(string.Format(CoreStrings.ERR_UNKNOWNSIBLINGTYPE, type.FullName));

            if (siblingtype.Table == null)
                throw new ArgumentException(string.Format(CoreStrings.ERR_WRONGSIBLINGTYPE, type.FullName));

            Table = new(siblingtype.Table)
            {
                TableId = SiblingTable.TableId
            };
        }

        if (Table != null && SiblingTable == null)
        {
            if (Table.TableId <= 0)
                throw new ArgumentException(string.Format(CoreStrings.ERR_MISSINGTABLEID, type.FullName));

            if (Table.Version <= 0)
            {
                throw new ArgumentException(string.Format(
                    CoreStrings.ERR_TABLEIDTOSMALL, type.FullName));
            }

            TypeEx.checkTable(type, Table);
        }

        if (SiblingView != null)
        {
            if (SiblingView.ViewId <= 0)
                throw new ArgumentException(string.Format(CoreStrings.ERR_MISSINGVIEWID, type.FullName));

            if (SiblingView.SiblingType == null || SiblingView.SiblingType == typeof(Nullable))
                throw new ArgumentException(string.Format(CoreStrings.ERR_MISSINGSIBLINGTYPE, type.FullName));

            var siblingtype = SiblingView.SiblingType.GetTypeDescription();

            if (siblingtype == null)
                throw new ArgumentException(string.Format(CoreStrings.ERR_UNKNOWNSIBLINGTYPE, type.FullName));

            if (siblingtype.View == null)
                throw new ArgumentException(string.Format(CoreStrings.ERR_WRONGSIBLINGTYPE, type.FullName));

            View = new(siblingtype.View)
            {
                ViewId = SiblingView.ViewId,
                MasterType = SiblingView.MasterType
            };
        }

        if (View != null && SiblingView == null)
        {
            if (View.ViewId <= 0)
                throw new ArgumentException(string.Format(CoreStrings.ERR_MISSINGVIEWID, type.FullName));

            
            if (View.Version <= 0)
            {
                throw new ArgumentException(string.Format(
                    CoreStrings.ERR_VIEWIDTOSMALL, type.FullName));
            }

            TypeEx.checkView(type, View);
        }

        // Felder üperprüfen...
        TypeEx.checkFields(this);


        Context ??= new AFContext(type.Name, type.Name);

        // AfterRegisterTypeDescription aufrufen, wenn verfügbar
        MethodInfo? ifo = type.GetMethod(@"AfterRegisterTypeDescription",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.IgnoreCase);

        ifo?.Invoke(null, null);
    }

    /// <summary>
    /// Liefert eine Dictionary der vorhandenen Properties, dass auch Daten aufnehmen kann.
    ///
    /// Key ist der Name des Propertys, Value ist ein das DatasourceField, dass die Eigenschaft beschreibt, und auch einen Wert aufnehmen kann.
    /// </summary>
    /// <returns></returns>
    public SortedDictionary<string, DatasourceField> GetAsDictionary(IModel? currentModel, bool ignoreSystemAndGuid, int currentdepth, int maxdepth, string praefix, string suffix)
    {
        SortedDictionary<string, DatasourceField> ret = new();

        loadProperties(string.Empty, ret, this, currentModel, ignoreSystemAndGuid, currentdepth, maxdepth, praefix, suffix);

        return ret;
    }

    /// <summary>
    /// Felder, die mit der Eigenschaft ModelInfoData versehen wurden.
    /// </summary>
    public Dictionary<string, PropertyDescription> ModelInfoFiels => _modelInfoFields;

    /// <summary>
    /// Feld, das mit der Eigenschaft ModelInfoCaption versehen wurden.
    /// </summary>
    public PropertyDescription? ModelInfoCaptionField => _modelCaptionField;

    /// <summary>
    /// Feld, das den PrimaryKey repräsentiert.
    /// </summary>
    public PropertyDescription? ModelKeyField => _modelKeyField;

    private static readonly string[] ignore = 
        [
            $"{nameof(IModel.PrimaryKey)}",
            $"{nameof(IModel.ChangedProperties)}",
            $"{nameof(IModel.Connector)}",
            $"{nameof(IModel.HasChanged)}",
            $"{nameof(IModel.ModelDescription)}",
            $"{nameof(IModel.ModelLink)}",
            $"{nameof(Base.RaiseChangeEvents)}",
            $"{nameof(AF.DATA.Table.CreateDateTime)}",
            $"{nameof(AF.DATA.Table.Database)}",
            $"{nameof(AF.DATA.Table.IsArchived)}",
            $"{nameof(AF.DATA.Table.UpdateDateTime)}",
            $"{nameof(IUser.IsAdmin)}",
            $"{nameof(IUser.IsLocked)}",
            $"{nameof(IUser.UserId)}",
            $"{nameof(IUser.UserLoginName)}",
            $"{nameof(IUser.UserName)}",
            $"{nameof(IUser.UserPassword)}"
        ];

    private static void loadProperties(string basename, SortedDictionary<string, DatasourceField> ret, TypeDescription tdesc, IModel? currentModel, bool ignoreSystemAndGuid, int currentdepth, int maxdepth, string praefix, string suffix)
    {
        foreach (var prop in tdesc.Properties.Values)
        {
            if (ignore.Contains(prop.Name)) continue;

            if (!((PropertyInfo)prop).CanRead) continue;

            if (ignoreSystemAndGuid && ((prop.Field != null && prop.Field?.SystemFieldFlag != eSystemFieldFlag.None) || ((PropertyInfo)prop).PropertyType == typeof(Guid)))
                continue;

            if (((PropertyInfo)prop).PropertyType.IsArray) continue;

            if (((PropertyInfo)prop).PropertyType.HasInterface(typeof(IList))) continue;

            if (((PropertyInfo)prop).PropertyType != typeof(ModelInfo)
                && ((PropertyInfo)prop).PropertyType != typeof(ModelLink)
                && ((PropertyInfo)prop).PropertyType.HasInterface(typeof(IModel)))
            {
                if (currentdepth >= maxdepth) continue;

                if (currentModel != null)
                    loadProperties($"{basename}{(basename.IsNotEmpty() ? "." : "")}{prop.Name}", ret, 
                            ((PropertyInfo)prop).PropertyType.GetTypeDescription(), tdesc.Accessor[currentModel, prop.Name] as IModel, ignoreSystemAndGuid, currentdepth + 1, maxdepth, praefix, suffix);
                else
                    loadProperties($"{basename}{(basename.IsNotEmpty() ? "." : "")}{prop.Name}", ret,
                        ((PropertyInfo)prop).PropertyType.GetTypeDescription(), null, ignoreSystemAndGuid, currentdepth + 1, maxdepth, praefix, suffix);

                continue;
            }

            string fullname = $"{praefix}{basename}{(basename.IsNotEmpty() ? "." : "")}{prop.Name}{suffix}";

            if (prop.Context?.AliasName?.IsNotEmpty() ?? false)
            {
                var aliasname = $"{praefix}{prop.Context!.AliasName!}{suffix}";

                if (!ret.ContainsKey(aliasname))
                    fullname = aliasname;
            }

            DatasourceField field = new DatasourceField()
            {
                FieldName = fullname,
                FieldDisplayName = prop.Name,
                FieldDescription = prop.Context?.Description ?? "",
                FieldType = ((PropertyInfo)prop).PropertyType,
                FieldMask = prop.Binding?.DisplayFormat ?? prop.GridColumn?.DisplayFormat ?? string.Empty,

                EntityName = tdesc.Type.Name,
                EntityDisplayName = tdesc.Name,
                EntityDescription = tdesc.Description ?? "",
                EntityType = tdesc.Type
            };
            
            if (((PropertyInfo)prop).PropertyType.IsEnum)
            {
                if (currentModel != null)
                {
                    var value = tdesc.Accessor[currentModel, prop.Name] as Enum;
                    field.CurrentValue = value?.GetEnumDescription() ?? "";
                }
                ret.Add(fullname, field);
                continue;
            }

            if (((PropertyInfo)prop).PropertyType == typeof(System.Drawing.Image))
            {
                if (currentModel != null)
                    field.CurrentValue = tdesc.Accessor[currentModel, prop.Name];

                ret.Add(fullname, field);
                continue;
            }
            if (((PropertyInfo)prop).PropertyType == typeof(bool))
            {
                if (currentModel != null) 
                    field.CurrentValue = (bool)tdesc.Accessor[currentModel, prop.Name] ? "Ja" : "Nein";
                ret.Add(fullname, field);
                continue;
            }

            if (((PropertyInfo)prop).PropertyType == typeof(DateTime))
            {
                if (currentModel != null)
                    field.CurrentValue = tdesc.Accessor[currentModel, prop.Name];

                field.FieldMask = (field.FieldMask.IsEmpty() ? "g" : field.FieldMask);
                ret.Add(fullname, field);
                continue;
            }

            if (((PropertyInfo)prop).PropertyType == typeof(DateOnly))
            {
                if (currentModel != null)
                    field.CurrentValue = tdesc.Accessor[currentModel, prop.Name];

                field.FieldMask = (field.FieldMask.IsEmpty() ? "d" : field.FieldMask);
                ret.Add(fullname, field);
                continue;
            }

            if (((PropertyInfo)prop).PropertyType == typeof(TimeOnly))
            {
                if (currentModel != null)
                    field.CurrentValue = tdesc.Accessor[currentModel, prop.Name];

                field.FieldMask = (field.FieldMask.IsEmpty() ? "t" : field.FieldMask);
                ret.Add(fullname, field);
                continue;
            }

            if (((PropertyInfo)prop).PropertyType.IsValueType)
            {
                if (currentModel != null)
                    field.CurrentValue = tdesc.Accessor[currentModel, prop.Name];

                ret.Add(fullname, field);
                continue;
            }

            if (((PropertyInfo)prop).PropertyType == typeof(string))
            {
                if (currentModel != null)
                    field.CurrentValue = tdesc.Accessor[currentModel, prop.Name];

                ret.Add(fullname, field);
                continue;
            }
        }
    }

    /// <summary>
    /// String-Darstellung (Name des Typs)
    /// </summary>
    /// <returns>Typ.Name</returns>
    public override string ToString()
    {
        return Type.Name;
    }
    
    /// <summary>
    /// Erweiterungsobjekt zurückgeben
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>
    /// <param name="name">Name des Objekts</param>
    /// <returns>Objekt vom Typ T oder null, wenn nicht vorhanden</returns>
    public T? GetExtension<T>(string name) where T : INullable
    {
        T? ret = default;

        if (_extensions.TryGetValue(name, out var extension))
            return (T)extension;

        return ret;
    }

    /// <summary>
    /// Erweiterungsobjekt hinzufügen
    /// </summary>
    /// <param name="name">Name</param>
    /// <param name="value">Wert/Objekt</param>
    public void SetExtension(string name, object value)
    {
        _extensions.Add(name, value);
    }

    /// <summary>
    /// Operator, der es erlaubt, TypeDescription-Objekte direkt als Type zu verwenden
    /// </summary>
    /// <param name="typeDescription"></param>
    public static explicit operator Type(TypeDescription typeDescription)
    {
        return typeDescription.Type;
    }

    /// <summary>
    /// Typ, der durch diese TypeDescription beschrieben wird
    /// </summary>
    public Type Type { get; private set; }

    /// <summary>
    /// Table-Attribut des Typs, wenn Typ eine Tabelle darstellt
    /// </summary>
    public AFContext? Context { get; private set; }

    /// <summary>
    /// Table-Attribut des Typs, wenn Typ eine Tabelle darstellt
    /// </summary>
    public AFTable? Table { get; private set; }

    /// <summary>
    /// Attribut, das eine Tabelle in der Datenbank beschreibt, die genau wie eine vorhandene aufgebaut ist und von dieser erbt.
    /// </summary>
    public AFSiblingTable? SiblingTable { get; private set; }

    /// <summary>
    /// Attribut, das eine View in der Datenbank beschreibt, die genau wie eine vorhandene aufgebaut ist und von dieser erbt.
    /// </summary>
    public AFSiblingView? SiblingView { get; private set; }

    /// <summary>
    /// AFOptions-Attribut des Typs, wenn der Typ z.B. Einstellungsdialog genutzt wird.
    /// </summary>
    public AFOptions? Options { get; private set; }

    /// <summary>
    /// Table-Attribut des Typs, wenn Typ eine Tabelle darstellt
    /// </summary>
    public AFView? View { get; private set; }

    /// <summary>
    /// Wahr, wenn Typ eine Tabelle ist
    /// </summary>
    public bool IsTable => Table != null;

    /// <summary>
    /// Wahr, wenn Typ eine Ansicht ist
    /// </summary>
    public bool IsView => View != null;

    /// <summary>
    /// Wahr, wenn Typ eine Tabelle oder eine als durchsuchbar markierte Ansicht ist
    /// </summary>
    public bool IsBrowsable => (Table != null && Table.Browsable) || (View != null && View.Browsable);

    /// <summary>
    /// eine Liste aller Felder dieses Typs
    /// </summary>
    public Dictionary<string, PropertyDescription> Properties { get; } = new();

    /// <summary>
    /// alle bindbaren Eigenschaften dieses Typs
    /// </summary>
    public Dictionary<string, PropertyDescription> Bindable =>
        Properties.Where(p => p.Value.Binding != null).ToDictionary(p => p.Key, p => p.Value);

    /// <summary>
    /// Zugriff auf den TypeAccessor des Typs (siehe FastMember)
    /// </summary>
    public TypeAccessor Accessor { get; }

    /// <summary>
    /// Name des Typs.
    /// Wenn MVCContext existiert, der Name im Kontext (Singular), sonst der Name des Typs.
    /// </summary>
    public string Name => Context != null ? Context.NameSingular : Type.Name;

    /// <summary>
    /// a list of all fields (properties with the AFField attribute)
    /// </summary>
    /// <returns>Felder als Dictionary</returns>
    public Dictionary<string, PropertyDescription> Fields =>
        _fields ??= Properties.Where(p => p.Value.Field != null).ToDictionary(p => p.Key, p => p.Value, StringComparer.InvariantCultureIgnoreCase);

    /// <summary>
    /// alle Eigenschaften mit einem AFGridColumn Attribut
    /// </summary>
    public Dictionary<string, PropertyDescription> GridColumns =>
        _gridcolumns ??= Properties.Where(p => p.Value.GridColumn != null).ToDictionary(p => p.Key, p => p.Value, StringComparer.InvariantCultureIgnoreCase);
    
    /// <summary>
    /// alle Eigenschaften mit einem AFBindable Attribut
    /// </summary>
    public Dictionary<string, PropertyDescription> BindableFields =>
        _bindable ??= Properties.Where(p => p.Value.Binding != null).ToDictionary(p => p.Key, p => p.Value, StringComparer.InvariantCultureIgnoreCase);

    /// <summary>
    /// Eigenschaft, die als PrimaryKey.
    /// </summary>
    public PropertyDescription? FieldKey =>
        Properties.Values.FirstOrDefault(p => p.Field?.SystemFieldFlag == eSystemFieldFlag.PrimaryKey);

    /// <summary>
    /// Eigenschaft, die als TimeStamp für den Zeitpunkt der ersten Speicherung verwendet wird.
    /// </summary>
    public PropertyDescription? FieldCreated =>
        Properties.Values.FirstOrDefault(p => p.Field?.SystemFieldFlag == eSystemFieldFlag.TimestampCreated);

    /// <summary>
    /// Eigenschaft, die als TimeStamp für den Zeitpunkt der letzten Speicherung verwendet wird.
    /// </summary>
    public PropertyDescription? FieldChanged =>
        Properties.Values.FirstOrDefault(p => p.Field?.SystemFieldFlag == eSystemFieldFlag.TimestampChanged);

    /// <summary>
    /// Eigenschaft, die als Flag für die Archivierung verwendet wird
    /// </summary>
    public PropertyDescription? FieldArchived =>
        Properties.Values.FirstOrDefault(p => p.Field?.SystemFieldFlag == eSystemFieldFlag.ArchiveFlag);
    
    #region IMenuEntry

    /// <inheritdoc />
    public bool BeginGroup => false;

    /// <inheritdoc />
    public bool Toggle { get; set; } = false;

    /// <inheritdoc />
    public string Caption => Context?.NamePlural ?? Name;

    /// <inheritdoc />
    public string Description => Context?.Description ?? "";

    /// <inheritdoc />
    public string Hint => Context?.Hint ?? "";

    /// <inheritdoc />
    public eKeys HotKey => eKeys.None;

    /// <inheritdoc />
    public object? Image => Type.GetUIController()?.TypeImage;

    /// <inheritdoc />
    public object? GroupImage => null;

    /// <inheritdoc />
    public int ImageIndex => -1;

    /// <inheritdoc />
    public object Tag => this;

    /// <inheritdoc />
    public eCommandContext CommandContext => eCommandContext.Other;

    /// <inheritdoc />
    public eCommand CommandType => eCommand.Other;
    #endregion

    /// <summary>
    /// Ruft die Definition für eine Gitteransicht zur Anzeige der Modelle ab (Spalten usw.)
    ///
    /// Diese Methode verwendet den IController des Typs, falls vorhanden.
    /// Ansonsten wird das Setup aus den Attributen (AFGridColumn) erstellt.
    /// </summary>
    /// <param name="mastertype">Typ des Masters, wenn das Grid Childs dieses Masters anzeigt</param>
    /// <param name="detailtype">Typ der Details, für die das Grid ermittelt werden soll</param>
    /// <param name="style">Stil des Rasters</param>
    /// <param name="gridtype">Typ des Rasters, Standard ist eGridMode.GridView</param>
    /// <param name="fromcontroller">Methode wird vom Controller aufgerufen - kein Rückruf des Controllers, wenn true</param>
    /// <param name="fields">Optionale Liste der Felder, die im Grid dargestellt werden sollen</param>
    /// <returns>GridSetup-Objekt</returns>
    public AFGridSetup GetGridSetup(eGridStyle style, Type? mastertype = null, Type? detailtype = null, string[]? fields = null, eGridMode gridtype = eGridMode.GridView, bool fromcontroller = false)
    {
        if (!fromcontroller && GetController() is { } controller)
            return controller.GetGridSetup(style, mastertype, detailtype, fields, gridtype);

        AFGridSetup? setup = null;

        if (!_customGridSetupSearched)
        {
            // GetCustomGridSetup aufrufen, wenn verfügbar...
            MethodInfo? ifo = Type.GetMethod(@"GetCustomGridSetup",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.IgnoreCase,
                [typeof(Type), typeof(eGridStyle), typeof(eGridMode)]);

            if (ifo != null)
                _customGridSetupMethod = ifo;

            _customGridSetupSearched = true;
        }

        if (_customGridSetupMethod != null)
            setup = (AFGridSetup?)_customGridSetupMethod.Invoke(null, [mastertype!, style, gridtype]);

        if (setup != null)
            return setup;

        setup = new();

        if (fields is { Length: > 0 })
        {
            foreach (var field in fields)
            {
                var coldef = Properties.Values.FirstOrDefault(p => p.GridColumn != null && p.Name == field);
                
                if (coldef == null || coldef.GridColumn == null)
                    continue;

                setup.Columns.Add(coldef.GridColumn);
            }
        }

        if (setup.Columns.Count > 0) return setup;

        foreach (var coldef in Properties.Values.Where(p => p.GridColumn != null).OrderBy(p => p.GridColumn?.ColumnIndex))
        {
            if (!coldef.GridColumn!.InStyles.HasFlag(style))
            {
                if (!coldef.GridColumn!.InStyles.HasFlag(eGridStyle.All))
                    continue;
            }

            if (coldef.GridColumn == null) continue;

            setup.Columns.Add(coldef.GridColumn);
        }

        return setup;
    }

    /// <summary>
    /// Die Spaltendefinition für eine bestimmte Eigenschaft/Property des Typs zurückgeben.
    /// </summary>
    /// <param name="propertyname">Name der Eigenschaft/des Properties</param>
    /// <returns>Spaltendefinition</returns>
    /// <exception cref="NullReferenceException">Fehler, wenn die Spalte nicht existiert</exception>
    public AFGridColumn GetColumn(string propertyname)
    {
        var column = Properties[propertyname].GridColumn ?? throw new NullReferenceException($"Der Typ {this.Type.FullName} stellt keine Spalte mit den Namen {propertyname} zur Verfügung.");

        return column;
    }

    private bool _fromSearch = false;

    /// <summary>
    /// Controller für diesen Typ
    /// </summary>
    public IController? GetController()
    {
        if (_controller == null && !_fromSearch) searchController();

        return _controller;
    }

    /// <summary>
    /// Suche nach einem Controller
    /// </summary>
    private void searchController()
    {
        if (_controller != null || _controllerSearched) return;

        // Suche nach dem UI-bezogenen Controller
        string controllerName = Type.Name + @"ControllerUI";

        Type? controllertype = TypeEx.SearchTypeByName(controllerName);

        // Suche im Basis-Controller ohne UI
        if (controllertype == null)
        {
            controllerName = Type.Name + @"Controller";
            controllertype = TypeEx.SearchTypeByName(controllerName);
        }

        // Views haben meist KEINE eigenen Controller.
        // Sie verwenden dann den Controller ihrer Basistabelle.
        if (controllertype == null && IsView && View != null)
        {
            // Suche nach dem UI-bezogenen Controller
            controllerName = View.MasterType.Name + @"ControllerUI";

            controllertype = TypeEx.SearchTypeByName(controllerName);

            // Suche im Basis-Controller ohne UI
            if (controllertype == null)
            {
                controllerName = View.MasterType.Name + @"Controller";
                controllertype = TypeEx.SearchTypeByName(controllerName);
            }
        }

        _fromSearch = true;

        if (controllertype != null)
        {
            if (controllertype.HasInterface(typeof(IController)))
            {
                PropertyInfo? propInfo = controllertype.GetProperty(@"Instance", BindingFlags.Static | BindingFlags.Public);
                if (propInfo != null)
                    _controller = propInfo.GetValue(null, null) as IController;
                else
                {
                    throw new(string.Format(
                        CoreStrings.ERR_CONTROLLER_NOINSTANCE,
                        controllertype.FullName));
                }
            }
            else
                throw new(string.Format(CoreStrings.ERR_CONTROLLER_MISSINGINTERFACE, controllertype.FullName));
        }
        _fromSearch = false;

        _controllerSearched = true;
    }

    /// <summary>
    /// Liefert die TypeDescription für eine bestimmte Tabelle/einen bestimmten View. 
    /// </summary>
    /// <param name="elementId">ID der Tabelle/des Views</param>
    /// <returns>TypeDescription für den Typen</returns>
    public static TypeDescription? GetTypeDescriptionById(int elementId)
    {
        return TypeEx.GetTypeDescriptionById(elementId);
    }

    /// <summary>
    /// Liefert die TypeDescription für eine bestimmte Tabelle/einen bestimmten View. 
    /// </summary>
    /// <param name="name">Name der Tabelle/des Views</param>
    /// <returns>TypeDescription für den Typen</returns>
    public static TypeDescription? GetTypeDescriptionByTableName(string name)
    {
        return TypeEx.GetTypeDescriptionByTableName(name);
    }

    /// <summary>
    /// Liefert eine Liste der Eigenschaften, die mit dem Attribut AFProperty versehen sind.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<PropertyDescription> GetProperties()
    {
        return Properties.Values.Where(p => p.Property != null).OrderBy(p => p.Property?.PropertyIndex);
    }

    /// <summary>
    /// Informationen zu den verfügbaren Feldern in der Entität ermitteln
    /// </summary>
    /// <returns>Liste der verfügbaren Felder</returns>
    public BindingList<DatasourceField> GetFieldInformations()
    {
        BindingList<DatasourceField> ret = [];

        foreach (var field in Properties.Where(p => ((PropertyInfo)p.Value).CanRead))
        {
            ret.Add(new()
            {
                FieldName = field.Value.Name,
                FieldDisplayName = field.Value.Context?.NameSingular ?? field.Value.Name,
                FieldDescription = field.Value.Context?.Description ?? "",
                FieldDisplayFormat = field.Value.Binding?.DisplayFormat ?? field.Value.GridColumn?.DisplayFormat ?? "",
                FieldType = ((PropertyInfo)field.Value).PropertyType,
                EntityName = this.Name,
                EntityType = this.GetType(),
                EntityDisplayName = this.Context?.NameSingular ?? this.Name,
                EntityDescription = this.Context?.Description ?? ""
            });
        }

        return ret;
    }
}




/// <summary>
/// Beschreibung einer Eigenschaft in einer Klasse.
/// 
/// Kann in PropertyInfo gecastet werden.
/// </summary>
public class PropertyDescription
{
    private readonly AFGridColumn? _gridColumn;
    private AFContext? _context;
    private readonly AFRules? _rules;
    private readonly AFBinding? _binding;

    /// <summary>
    /// die dekorierte PropertyInfo
    /// </summary>
    private readonly PropertyInfo _propertyInfo;

    /// <summary>
    /// den Konstruktor ausblenden
    /// </summary>
    private PropertyDescription(PropertyInfo propertyInfo)
    {
        _propertyInfo = propertyInfo;
    }

    /// <summary>
    /// Objet erzeugen (statisch, um Construktor zu umgehen)
    /// </summary>
    /// <param name="propertyInfo">die PropertyInfo, die durch dieses Objekt dargestellt wird</param>
    /// <param name="table">Tabelle, zu der das Property gehört</param>
    /// <param name="view">View, zu der das Property gehört</param>
    /// <param name="sibling">Geschwistertabelle, zu der das Property gehört</param>
    /// <param name="siblingView">Geschwisterview, zu der das Property gehört</param>
    public static PropertyDescription Create(PropertyInfo propertyInfo, AFTable? table, AFView? view, AFSiblingTable? sibling, AFSiblingView? siblingView)
    {
        PropertyDescription ret = new(propertyInfo)
        {
            Rules = propertyInfo.GetCustomAttribute<AFRules>(true),
            Binding = propertyInfo.GetCustomAttribute<AFBinding>(true),
            Context = propertyInfo.GetCustomAttribute<AFContext>(true),
            Field = table == null && view == null && sibling == null && siblingView == null ? null : propertyInfo.GetCustomAttribute<AFField>(true),
            GridColumn = propertyInfo.GetCustomAttribute<AFGridColumn>(true),
            Property = propertyInfo.GetCustomAttribute<AFProperty>(true),
            KPIElement = propertyInfo.GetCustomAttribute<AFKPIElement>(true)
        };

        if (ret.Field != null && ret.Field.SaveAllways == DevExpress.Utils.DefaultBoolean.Default)
        {
            if (propertyInfo.PropertyType != typeof(byte[]) && (propertyInfo.PropertyType.HasInterface(typeof(IList)) ||
                propertyInfo.PropertyType.HasInterface(typeof(IBindingList))))
                ret.Field.SaveAllways = DevExpress.Utils.DefaultBoolean.True;
            else
                ret.Field.SaveAllways = DevExpress.Utils.DefaultBoolean.False;
        }

        ret.Context ??= new AFContext(propertyInfo.Name, propertyInfo.Name);

        if (ret.Context.ResourceManager != null)
        {
            ret.Context.NameSingular = ret.Context.NameSingular.IsEmpty() ? (ret.Context.RessourceName ?? propertyInfo.Name) + @"_SINGULAR" : ret.Context.NameSingular;
            ret.Context.NamePlural = ret.Context.NamePlural.IsEmpty() ? (ret.Context.RessourceName ?? propertyInfo.Name) + @"_PLURAL" : ret.Context.NamePlural;
            ret.Context.Description = ret.Context.Description.IsEmpty() ? (ret.Context.RessourceName ?? propertyInfo.Name) + @"_DESC" : ret.Context.Description;
        }

        if (ret.GridColumn == null) return ret;

        ret.GridColumn.ColumnProperty = ret;
        
        if (ret.GridColumn.Caption.IsEmpty())
            ret.GridColumn.Caption = ret.Context.TalkingName;

        return ret;
    }

    /// <summary>
    /// Name des Propertys
    /// </summary>
    public string Name => _propertyInfo.Name;

    /// <summary>
    /// Operator, der es erlaubt, PropertyDescription-Objekte direkt als PropertyInfo zu verwenden
    /// </summary>
    /// <param name="propertyDescription"></param>
    public static explicit operator PropertyInfo(PropertyDescription propertyDescription)
    {
        return propertyDescription._propertyInfo;
    }

    /// <summary>
    /// Regeln für diese Eigenschaft
    /// </summary>
    public AFRules? Rules
    {
        get
        {
            AFRules? ret = null;

            if (Link != null)
            {
                ret = Link.LinkTo.GetTypeDescription()
                    .Properties[Link.PropertyName == null || string.IsNullOrWhiteSpace(Link.PropertyName)
                        ? Name
                        : Link.PropertyName].Rules;
            }

            ret ??= _rules;

            return ret;
        }
        private init => _rules = value;
    }

    /// <summary>
    /// Binding-Definition
    /// </summary>
    public AFBinding? Binding
    {
        get
        {
            AFBinding? ret = null;

            if (Link != null)
            {
                ret = Link.LinkTo.GetTypeDescription()
                    .Properties[Link.PropertyName == null || string.IsNullOrWhiteSpace(Link.PropertyName)
                        ? Name
                        : Link.PropertyName].Binding;
            }

            ret ??= _binding;

            return ret;
        }
        private init => _binding = value;
    }

    /// <summary>
    /// Beschreibung für diese Eigenschaft
    /// </summary>
    public AFContext? Context
    {
        get
        {
            AFContext? ret = null;

            if (Link != null)
            {
                ret = Link.LinkTo.GetTypeDescription()
                    .Properties[Link.PropertyName == null || string.IsNullOrWhiteSpace(Link.PropertyName)
                            ? Name
                            : Link.PropertyName].Context;
            }

            ret ??= _context;

            return ret;
        }
        private set => _context = value;
    }

    /// <summary>
    /// Link zu anderer EIgenschaft um von dort Informationen zu übernehmen
    /// </summary>
    public AFLink? Link { get; init; }

    /// <summary>
    /// Feld
    /// </summary>
    public AFField? Field { get; init; }

    /// <summary>
    /// Property-Dialog Eigenschaft
    /// </summary>
    public AFProperty? Property { get; init; }

    /// <summary>
    /// KPI-Element
    /// </summary>
    public AFKPIElement? KPIElement { get; init; }

    /// <summary>
    /// Grid-Spalten Definition
    /// </summary>
    public AFGridColumn? GridColumn
    {
        get
        {
            AFGridColumn? ret = null;

            if (Link != null)
            {
                ret = Link.LinkTo.GetTypeDescription()
                    .Properties[Link.PropertyName == null || string.IsNullOrWhiteSpace(Link.PropertyName)
                            ? Name
                            : Link.PropertyName].GridColumn;
            }

            ret ??= _gridColumn;

            return ret;
        }
        private init => _gridColumn = value;
    }
}

