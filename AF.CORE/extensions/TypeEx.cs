using System.Reflection;
using System.Runtime.CompilerServices;

namespace AF.CORE;

/// <summary>
/// Erweiterungsmethoden for System.Type
/// </summary>
public static class TypeEx
{
    private static readonly Dictionary<Type, TypeDescription> _typedescCache = [];
    private static readonly Dictionary<string, Type?> _typeCache = [];
    private static readonly Dictionary<string, AFPermission> _typePermissions = [];


#if (NET481_OR_GREATER)
    /// <summary>
    /// Gibt an, ob der Typ einer Variablen des angegebenen Typs zugewiesen werden kann.
    /// </summary>
    /// <param name="me"></param>
    /// <param name="assignableTo"></param>
    /// <returns></returns>
    public static bool IsAssignableTo(this Type me, Type assignableTo)
    {
       return assignableTo.GetTypeInfo().IsAssignableFrom(me.GetTypeInfo());
    }
#endif

    /// <summary>
    /// Liefert ein Array der TypeDescriptions die eine bestimmte 
    /// Bedingung erfüllen.
    /// 
    /// Die übergebene Methode bestimmt, die Filterbedingung. 
    /// Liefert die Methode für eine TypeDescription true, ist diese in der Liste enthalten.
    /// </summary>
    /// <param name="predicate">Filterfunktion</param>
    /// <returns>Liste der TypeDescriptions</returns>
    public static IEnumerable<TypeDescription> GetTypeDescriptions(Func<TypeDescription, bool> predicate)
    {
        return _typedescCache.Values.Where(predicate).OrderBy(t => t.Name);
    }

    /// <summary>
    /// AFGridColumn-Definition für die Eigenschaft ermitteln.
    /// 
    /// Das AFGridColumn-Attribut für die Eigenschaft kann zur Erstellung von 
    /// Grid-Ansichten verwendet werden.
    /// </summary>
    /// <param name="type">Typ, der die Eigenschaft enthält</param>
    /// <param name="name">Name der Eigenschaft.</param>
    /// <returns>AFGridColumn oder null</returns>
    public static AFGridColumn? GetGridColumn(this Type type, string name)
    {
        return type.GetTypeDescription().Properties[name].GridColumn;
    }


    /// <summary>
    /// Prüft, ob der angemeldete Benutzer die erforderliche Berechtigung für den Typ hat.
    /// 
    /// Nutzt das AFPermission-Attribut des Typs.
    /// 
    /// Mit Hilfe des AFPermission-Attributes können Berechtigungen auf sehr niedriger, 
    /// nicht funktionaler Ebene vergeben und geprüft werden.
    /// </summary>
    /// <param name="type">Typ der geprüft werden soll</param>
    /// <returns>true wenn der Benutzer berechtigt ist</returns>
    public static bool HasRight(this Type type)
    {
        if (AFCore.App.SecurityService == null) return true; // kein SecurityService -> alle drüfen alles

        if (AFCore.App.SecurityService.CurrentUser == null) return false; // kein Benutzer angemeldet -> niemand darf etwas

        return HasRight(type, AFCore.App.SecurityService.CurrentUser);
    }

    /// <summary>
    /// Prüft, ob der Benutzer die erforderliche Berechtigung für den Typ hat.
    /// 
    /// Nutzt das AFPermission-Attribut des Typs.
    /// 
    /// Mit Hilfe des AFPermission-Attributes können Berechtigungen auf sehr niedriger, 
    /// nicht funktionaler Ebene vergeben und geprüft werden.
    /// </summary>
    /// <param name="type">Typ der geprüft werden soll</param>
    /// <param name="user">Benutzer, der die Berechtigung benötigt.</param>
    /// <returns>true wenn der Benutzer berechtigt ist</returns>
    public static bool HasRight(this Type type, IUser user)
    {
        if (type.FullName == null) throw new ArgumentNullException(nameof(type));

        AFPermission? rights = _typePermissions.TryGetValue(type.FullName, out var permissions) 
            ? permissions : null;

        if (rights == null)
        {
            rights = type.GetCustomAttribute<AFPermission>();
            
            rights ??= new();

            _typePermissions.Add(type.FullName, rights);
        }

        if (user.IsAdmin) return true;

        if (rights.NeedAdminRights) return false;

        if (rights.NeededRight > -1 && !user.HasRight(rights.NeededRight)) return false;

        return true;
    }

    /// <summary>
    /// Sucht einen Typen anhand seines Namens in allen geladenen Assemblys
    /// </summary>
    /// <param name="typeName">vollständiger Name des Typs (inkl. NameSpace)</param>
    /// <returns>Type oder NULL wenn nicht gefunden</returns>
    public static Type? FindType(string typeName)
    {
        Type? t = Type.GetType(typeName);

        if (t != null) return t;

        lock (_typeCache)
        {
            if (_typeCache.TryGetValue(typeName, out t)) return t;

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                t = a.GetType(typeName);

                if (t == null) continue;

                _typeCache[typeName] = t;
                break;
            }
        }

        return t;
    }

    /// <summary>
    /// Eine generische Methode finden, deren Parameter mit den übergebenen übereinstimmt.
    /// </summary>
    /// <param name="type">zu durchsuchender Typ</param>
    /// <param name="name">Name der Methode</param>
    /// <param name="genTypes">Liste der generischen Typen</param>
    /// <param name="parameterTypes">Liste der Parameter-Typen</param>
    /// <returns>gefundene Methode (MethodInfo) oder NULL</returns>
    public static MethodInfo? FindMethod(this Type type, string name, Type[] genTypes, Type?[] parameterTypes)
    {
        return type.GetMethods()
            .Where(m => m.Name == name)
            .Select(m => new {
                Method = m,
                Params = m.GetParameters(),
                Args = m.GetGenericArguments()
            })
            .Where(x => x.Params.Length >= parameterTypes.Length
                    && x.Args.Length == genTypes.Length)
            .Select(x => x.Method)
            .First();
    }

    
    /// <summary>
    /// Sucht einen Typen nach Name und gibt den ersten Typen mit dem gefundenen Namen zurück.
    /// </summary>
    /// <param name="typeName">Name des Typs (ohne Namespace!)</param>
    /// <returns>der gefundene Typ oder NULL</returns>
    public static Type? SearchTypeByName(string typeName)
    {
        lock (_typeCache)
        {
            if (_typeCache.TryGetValue(typeName, out var cachetype))
                return cachetype;
        }


        Assembly? currentAssembly = Assembly.GetEntryAssembly();

        if (currentAssembly == null) return null;

        Type? type = currentAssembly.ExportedTypes.FirstOrDefault(t => t.Name == typeName);

        if (type != null)
            return type;

        var referencedAssemblies = currentAssembly.GetReferencedAssemblies();

        foreach (var assemblyName in referencedAssemblies)
        {
            if (assemblyName.FullName.StartsWith(@"DevExpress"))
                continue;

            if (assemblyName.FullName.StartsWith(@"System."))
                continue;

            if (assemblyName.FullName.StartsWith(@"mscorlib"))
                continue;

            Assembly? assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(x => x.FullName == assemblyName.FullName);

            if (assembly == null)
                Assembly.Load(assemblyName);
        }

        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in loadedAssemblies)
        {
            if (assembly.IsDynamic)
                continue;

#if (NET8_0_OR_GREATER)
            if (assembly.FullName == null)
                continue;
#endif
            if (assembly.FullName.StartsWith(@"DevExpress"))
                continue;

            if (assembly.FullName.StartsWith(@"System."))
                continue;

            if (assembly.FullName.StartsWith(@"mscorlib"))
                continue;

            type = assembly.ExportedTypes.FirstOrDefault(t => t.Name == typeName);

            if (type == null) continue;

            lock (_typeCache)
            {
                _typeCache.Add(typeName, type);
            }

            return type;
        }

        lock (_typeCache)
        {
            _typeCache.Add(typeName, null);
        }


        return null;
    }

    /// <summary>
    /// Prüft, ob der Typ ein bestimmtes Interface implementiert
    /// </summary>
    /// <param name="type"></param>
    /// <param name="interfaceType"></param>
    /// <returns></returns>
    public static bool HasInterface(this Type type, Type interfaceType)
    {
        return type.GetInterfaces().Contains(interfaceType);
    }

    /// <summary>
    /// Liefert die TypeDescription für einen bestimmten Typen. Das funktioniert nur, 
    /// wenn der Type das Interface IBindable implementiert. Ist das NICHT der Fall, 
    /// wird eine ArgumenException ausgelöst.
    /// </summary>
    /// <param name="type">Type</param>
    /// <returns>TypeDescription für den Typen</returns>
    public static TypeDescription GetTypeDescription(this Type type)
    {
        if (_typedescCache.TryGetValue(type, out var description))
            return description;

        if (!type.HasInterface(typeof(IBindable)))
            throw new ArgumentException(string.Format(CoreStrings.ERR_TYPEISNOTBINDABLE, type.FullName));

        _typedescCache.Add(type, new TypeDescription(type));

        return _typedescCache[type];
    }

    /// <summary>
    /// Liefert die TypeDescription für eine bestimmte Tabelle/einen bestimmten View. 
    /// </summary>
    /// <param name="elementId">ID der Tabelle/des Views</param>
    /// <returns>TypeDescription für den Typen</returns>
    public static TypeDescription? GetTypeDescriptionById(int elementId)
    {
        return _typedescCache.Values.FirstOrDefault(td => td.View?.ViewId == elementId || td.Table?.TableId == elementId);
    }

    /// <summary>
    /// Liefert die TypeDescription für eine bestimmte Tabelle/einen bestimmten View. 
    /// </summary>
    /// <param name="name">Name der Tabelle/des Views</param>
    /// <returns>TypeDescription für den Typen</returns>
    public static TypeDescription? GetTypeDescriptionByTableName(string name)
    {
        name = name.ToUpper();

        return _typedescCache.Values.FirstOrDefault(td => td.View?.ViewName.ToUpper() == name || td.Table?.TableName.ToUpper() == name);
    }

    /// <summary>
    /// Liefert die Typbeschreibungen alle Views, die auf der Beschreibung der übergebenen Tabelle beruhen.
    /// </summary>
    /// <param name="tabledesc"></param>
    /// <returns></returns>
    public static TypeDescription[] GetViewTypes(TypeDescription tabledesc)
    {
        if (tabledesc.Table == null)
            return [];

        return _typedescCache.Values.Where(t => t.IsView && t.View?.MasterType == tabledesc.Type).ToArray();
    }


    /// <summary>
    /// Überprüft, ob eine ID (TableId oder ViewId) eindeutig ist.
    /// 
    /// Löst einen Fehler aus, wenn dies nicht der Fall ist.
    /// </summary>
    /// <param name="elementId">zu prüfende Id</param>
    /// <param name="type">Typ, zu dem die ID gehört</param>
    public static void CheckUniqueId(int elementId, Type type)
    {
        if (elementId < 0) return;

        var found = _typedescCache.FirstOrDefault(td => td.Value.View?.ViewId == elementId || td.Value.Table?.TableId == elementId);

        if (found.Key == type) return;

        throw new Exception($"Id {elementId} is not unique. Same Id is used by {found.Value.Type.FullName}.");
    }

    /// <summary>
    /// Alle Typen, die vom angegeben Typ erben ermitteln.
    /// </summary>
    /// <param name="type">Typ von dem geerbt wird</param>
    /// <returns>Liste der Typen, die vom Typ erben</returns>
    public static IEnumerable<Type> GetChildTypesOf(this Type type)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .Where(ass => !ass.IsDynamic)
            .SelectMany(t => t.GetExportedTypes())
            .Where(t => t.IsClass &&
                        !t.IsAbstract &&
                        t.IsSubclassOf(type));
    }


    /// <summary>
    /// Liefert alle Erweiterungs-Methoden eines Typs in den aktuell geladenen Assemblies
    /// </summary>
    /// <returns>Rückgabe von MethodInfo[] mit der Erweiterungs-Methode</returns>

    public static IEnumerable<MethodInfo> GetExtensionMethods(this Type t)
    {
        List<Type> types = [];

        foreach (Assembly item in AppDomain.CurrentDomain.GetAssemblies().Where(a => a.IsDynamic == false)) types.AddRange(item.GetExportedTypes());

        return from type in types
            where type.IsSealed && !type.IsGenericType && !type.IsNested
            from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            where method.IsDefined(typeof(ExtensionAttribute), false)
            where method.GetParameters()[0].ParameterType == t
            select method;
    }

    /// <summary>
    /// Prüft, ob ein Typ numerisch ist
    /// </summary>
    /// <param name="type">zu prüfender Typ</param>
    /// <returns>true, wenn der Typ numerisch ist, sonst false</returns>
    public static bool IsNumericType(this Type type)
    {
        return Type.GetTypeCode(type) switch
        {
            TypeCode.Byte => true,
            TypeCode.SByte => true,
            TypeCode.UInt16 => true,
            TypeCode.UInt32 => true,
            TypeCode.UInt64 => true,
            TypeCode.Int16 => true,
            TypeCode.Int32 => true,
            TypeCode.Int64 => true,
            TypeCode.Decimal => true,
            TypeCode.Double => true,
            TypeCode.Single => true,
            _ => false
        };
    }

    /// <summary>
    /// Sucht nach einer bestimmten Erweiterungs-Methode für den Typ.
    /// </summary>
    /// <param name="methodName">Name der Methode</param>
    /// <param name="t">Type für den die Methode gesucht wird</param>
    /// <param name="seekInParentClasses">Die Methode auch für übergeordnete Type des angegeben Typs suchen</param>
    /// <returns>die gefundene Methode oder null</returns>
    public static MethodInfo? GetExtensionMethod(this Type t, string methodName, bool seekInParentClasses)
    {
        Type seekType = t;

        while (true)
        {
            var mi = seekType.GetExtensionMethods().Where(m => m.Name == methodName).ToArray();

            if (mi.Length > 0) return mi.First();

            if (!seekInParentClasses)
                break;

            if (seekType.BaseType == null)
                break;

            seekType = seekType.BaseType;
        }

        return null;
    }


    /// <summary>
    /// Prüft, ob der Typ dem Wert NULL entspricht (selbst null oder vom Type Nullable oder vom Typ DBNull
    /// </summary>
    /// <param name="t">zu prüfenden Typ</param>
    /// <returns>true wenn NULL</returns>
    public static bool IsEmpty(this Type t)
    {
        return (t == typeof(Nullable) || t == typeof(DBNull));
    }

    /// <summary>
    /// Liefert den zum Typ passenden Controller oder NULL, wenn kein solcher existiert.
    /// </summary>
    /// <returns>der Controller oder null</returns>
    public static IController? GetControllerOrNull(this Type t)
    {
        return t.GetTypeDescription().GetController();
    }

    /// <summary>
    /// Liefert den zum Typ passenden Controller. Existiert kein Controller wird  eine Exception ausgelöst!
    /// </summary>
    /// <returns>der Controller oder null</returns>
    public static IController GetController(this Type t)
    {
        return t.GetTypeDescription().GetController() ?? throw new Exception($"Missing Controller for type {t.FullName}.");
    }

    /// <summary>
    /// Liefert den zum Typ passenden UIController oder NULL, wenn keiner existiert.
    /// </summary>
    /// <returns>der Controller oder null</returns>
    public static IController? GetUIControllerOrNull(this Type t)
    {
        return t.GetTypeDescription().GetController();
    }

    /// <summary>
    /// Liefert den zum Typ passenden UIController. Existiert kein Controller wird  eine Exception ausgelöst!
    /// </summary>
    /// <returns>der Controller oder null</returns>
    public static IControllerUI? GetUIController(this Type t)
    {
        return t.GetTypeDescription().GetController() as IControllerUI ?? throw new Exception($"Missing UIController for type {t.FullName}.");
    }

    /// <summary>
    /// Liefert den Tabellennamen, wenn der Typ mit dem Attribut AFTable versehen ist.
    /// </summary>
    /// <param name="type">Typ/Tabelle</param>
    /// <returns>Name der Tabelle oder NULL</returns>
    public static string? GetTableName(this Type type)
    {
        return type.GetTypeDescription().Table?.TableName;
    }

    /// <summary>
    /// Liefert den View-Namen, wenn der Typ mit dem Attribut AFView versehen ist.
    /// </summary>
    /// <param name="type">Typ/View</param>
    /// <returns>Name des Views oder NULL</returns>
    public static string? GetViewName(this Type type)
    {
        return type.GetTypeDescription().View?.ViewName;
    }

    /// <summary>
    /// Tabellendefinition prüfen
    /// </summary>
    internal static void checkTable(Type type, AFTable table)
    {
        var found = _typedescCache.Values.FirstOrDefault(t =>
            t.Table != null && t.Table.TableId == table.TableId && t.Type != type);

        if (found != null)
        {
            throw new Exception(string.Format(CoreStrings.ERR_SAMETABLEID, type.FullName, table.TableId,
                found.Type.FullName));
        }

        found = _typedescCache.Values.FirstOrDefault(t =>
            t.Table != null && t.Table.TableName.ToUpper().Trim() == table.TableName.ToUpper().Trim() &&
            t.Type != type);

        if (found != null && found.SiblingTable == null)
        {
            throw new Exception(string.Format(CoreStrings.ERR_SAMETABLENAME, type.FullName,
                table.TableName, found.Type.FullName));
        }
    }

    /// <summary>
    /// BindingList (typisiert) für den Typen erstellen
    /// </summary>
    /// <param name="type">Type</param>
    /// <returns>IBindinglist</returns>
    public static IBindingList CreateBindingList(this Type type)
    {
        if (Activator.CreateInstance(typeof(BindingList<>).MakeGenericType(type)) is IBindingList list)
            return list;

        throw new ArgumentException($@"Can't create a BindingList for type {type.FullName}");
    }

    /// <summary>
    /// View prüfen
    /// </summary>
    internal static void checkView(Type type, AFView view)
    {
        var found = _typedescCache.Values.FirstOrDefault(t =>
            t.View != null && t.View.ViewId == view.ViewId && t.Type != type);

        if (found != null)
        {
            throw new Exception(string.Format(CoreStrings.ERR_SAMEVIEWID, type.FullName, view.ViewId,
                found.Type.FullName));
        }

        found = _typedescCache.Values.FirstOrDefault(t =>
            t.View != null && t.View.ViewName.ToUpper().Trim() == view.ViewName.ToUpper().Trim() &&
            t.Type != type);

        if (found != null)
        {
            throw new Exception(string.Format(CoreStrings.ERR_SAMEVIEWNAME, type.FullName,
                view.ViewName, found.Type.FullName));
        }

        if (view.Query.IsEmpty())
        {
            var mi = type.GetMethod(@"RequestViewQuery", BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.IgnoreCase);
            if (mi != null)
            {
                var query = mi.Invoke(null, null) as string;
                if (!query.IsEmpty())
                    view.Query = query!;
            }
        }

        if (view.Query.IsEmpty())
            throw new Exception(string.Format(CoreStrings.ERROR_VIEWDOENSTCONTAINQUERY, type.FullName));

        
    }

    /// <summary>
    /// Felder prüfen
    /// </summary>
    internal static void checkFields(TypeDescription tdesc)
    {
        var fields = tdesc.Properties.Values.Where(p => p.Field != null).ToArray();
        var cnt = fields.Count(f => f.Field!.Parent != null);
        var cntSysField = fields.Count(f => f.Field!.SystemFieldFlag != eSystemFieldFlag.None);

        if (tdesc.IsTable && cnt > 0)
            throw new Exception($@"Table {tdesc.Name} contains fields (AFField) with parent attribute.");
        
        if (tdesc.IsView && cnt < (fields.Length - cntSysField))
            throw new Exception($@"View {tdesc.Name} contains fields (AFField) without parent attribute.");
        
        if (!tdesc.IsView && !tdesc.IsTable && fields.Length > 0)
            throw new Exception($@"Type {tdesc.Name} can not contain fields (AFField).");
    }

    /// <summary>
    /// Prüft, ob ein Controller bereits existiert.
    /// </summary>
    /// <param name="controllerType">Typ des Controllers</param>
    /// <returns>true, wenn der Controller existiert, sonst false</returns>
    public static bool ExistController(Type controllerType)
    {

        return false;
    }

    /// <summary>
    /// Eine Liste der Typen ermitteln, die vom Typ erben (nur public)
    /// </summary>
    /// <param name="basetype">Basistyp</param>
    /// <param name="assemblyNames">Namen der zu berücksichtigenden Assemblies, NULL wenn die Standardassemblies berücksichtigt werden sollen (AF.AppAssemblies), leeres Array wenn ALLE Assemblies berücksichtigt werden sollen.</param>
    /// <returns>Liste der Typen</returns>
    public static List<Type> GetPublicNonStaticDerivedTypes(this Type basetype, string[]? assemblyNames)
    {
        if (assemblyNames != null)
            assemblyNames = AFCore.AppAssemblies;

        IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies();

        if (assemblyNames != null && assemblyNames.Length > 0)
            assemblies = assemblies.Where(a => assemblyNames.Contains(a.GetName().Name));

        return assemblies
            .SelectMany(assembly =>
            {
                try
                {
                    return assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    return ex.Types.Where(t => t != null) as Type[] ?? [];
                }
            })
            .Where(type =>
                type.IsClass &&
                !type.IsAbstract &&
                type.IsPublic &&
                basetype.IsAssignableFrom(type) &&
                type != basetype)
            .ToList();
    }

    /// <summary>
    /// Aufruf einer statischen generischen Methode
    /// 
    /// typeof(Functions).InvokeGeneric("DeserializeJsonBytes", new Type[] {typeof(VariableBool)}, [data])
    /// </summary>
    /// <param name="obj">Objekt, für das die generische Methode aufgerufen werden soll</param>.
    /// <param name="name">Name der Methode</param>.
    /// <param name="parameters">Parameter, die an die Methode übergeben werden sollen</param>.
    /// <param name="genTypes">Generische Typen</param>
    public static object? InvokeGeneric(this Type obj, string name, Type[] genTypes, params object?[] parameters)
    {
        var paraTypes = parameters.AsEnumerable().Select(o => o?.GetType() ?? typeof(Nullable)).ToArray();

        var mi = obj.FindMethod(name, genTypes, paraTypes);

        if (mi == null)
            mi = obj.GetExtensionMethod(name, true);

        // letzte Variante nur über den Namen der Methode versuchen...
        if (mi == null) mi = obj.GetMethods().FirstOrDefault(m => m.Name == name && m.GetParameters().Length == parameters.Length);

        if (mi == null)
            throw new Exception($@"InvokeGeneric: Generic method {name} not found.");

        var mref = mi.MakeGenericMethod(genTypes);

        try
        {
            // Methode aufrufen
            return mref.Invoke(null, parameters);
        }
        catch (TargetParameterCountException ex)
        {
            throw new ArgumentException(
                $"Falsche Anzahl von Parametern für die Methode '{mi.Name}'. " +
                $"Erwartet: {mi.GetParameters().Length}, Erhalten: {parameters?.Length ?? 0}", ex);
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException(
                $"Ungültige Parameter für die Methode '{mi.Name}': {ex.Message}", ex);
        }
        catch (TargetInvocationException ex)
        {
            // Inner Exception weitergeben
            throw ex.InnerException ?? ex;
        }
    }
}


