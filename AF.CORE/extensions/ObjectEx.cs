using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace AF.CORE;


/// <summary>
/// Erweiterungsmethoden für die Klasse System.Object
/// </summary>
public static class ObjectEx
{
    private static readonly ConcurrentDictionary<Type, Func<object, string, string>?> toStringWithFormatCache = new();

    /// <summary>
    /// Objekt in String umwandeln.
    ///
    /// Erweiterte Form von ToString, die immer eine Maske unterstützt,
    /// diese aber ignoriert, wenn es keine Möglichkeit gibt.
    /// </summary>
    /// <param name="obj">Objekt</param>
    /// <param name="format">Maske/Format (default = null)</param>
    /// <returns>String-Repräsentation des Objekts</returns>
    public static string AsString(this object obj, string? format = null)
    {   
        if (string.IsNullOrEmpty(format)) return obj.ToString() ?? string.Empty;

        if (obj is string strval) return strval;

        Type type = obj.GetType();

        // Versuche, Delegate aus dem Cache zu holen oder neu zu erstellen
        var toStringFunc = toStringWithFormatCache.GetOrAdd(type, t =>
        {
            // Methode suchen: public string ToString(string)
            var method = t.GetMethod("ToString", [typeof(string)]);

            if (method == null || method.ReturnType != typeof(string))
                return null;

            // Delegate: (object o, string format) => ((T)o).ToString(format)
            var objParam = Expression.Parameter(typeof(object), "obj");
            var formatParam = Expression.Parameter(typeof(string), "format");

            var castObj = Expression.Convert(objParam, t);
            var call = Expression.Call(castObj, method, formatParam);
            var lambda = Expression.Lambda<Func<object, string, string>>(call, objParam, formatParam);
            return lambda.Compile();
        });

        if (toStringFunc == null) return obj.ToString() ?? string.Empty;
        
        return toStringFunc(obj, format!);
    }

    /// <summary>
    /// Erzeugt eine Instanz des übergebenen Typs und prüft dabei, dass der Typ T entspricht.
    ///
    /// Anders als bei Activator.CreateInstance ist die Rückgabe nie null und entspricht immer dem Typen.
    /// </summary>
    /// <param name="type">Typ/Klasse</param>
    /// <typeparam name="T">Typ der Rückgabe</typeparam>
    /// <returns>Erzeugte Instanz.</returns>
    /// <exception cref="ArgumentException">Ausnahme, wenn type nicht T entspricht.</exception>
    public static T CreateInstance<T>(Type type) 
    {
        if (Activator.CreateInstance(type) is not T ret)
            throw new ArgumentException($"{type.FullName} is not of type {typeof(T).FullName}.");

        return ret;
    }

    /// <summary>
    /// Versucht ein Objekt in den angegebenen Objekttyp umzuwandeln. 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="targettype"></param>
    /// <returns></returns>
    public static object? TranslateTo(this object source, Type targettype)
    {
        if (targettype.IsInstanceOfType(source)) // passt direkt...
            return source;

        if (targettype == typeof(bool))
        {
            if (source is string strsrc && strsrc.Length > 0)
            {
                if ("JjYyTt1".Contains(strsrc[0])) return true;

                return false;
            }

            if (source.GetType().IsNumericType())
            {
                try
                {
                    int val = Convert.ToInt32(source);

                    if (val > 0) return true;
                }
                catch { return false; }

                return false;

            }

            // nicht wandelbar ist immer false
            return false;
        }

        if (targettype == typeof(int))
        {
            if (source is string strsrc && strsrc.Length > 0)
            {
                if (int.TryParse(strsrc, out var val))
                    return val;

                return Convert.ToInt32(0);
            }

            if (source.GetType().IsNumericType())
            {
                try
                {
                    return Convert.ToInt32(source);
                }
                catch { return Convert.ToInt32(0); }
            }

            return Convert.ToInt32(0);
        }

        if (targettype == typeof(short))
        {
            if (source is string strsrc && strsrc.Length > 0)
            {
                if (short.TryParse(strsrc, out var val))
                    return val;

                return Convert.ToInt16(0);
            }

            if (source.GetType().IsNumericType())
            {
                try
                {
                    return Convert.ToInt16(source);
                }
                catch { return Convert.ToInt16(0); }
            }

            return Convert.ToInt16(0);
        }

        if (targettype == typeof(long))
        {
            if (source is string strsrc && strsrc.Length > 0)
            {
                if (long.TryParse(strsrc, out var val))
                    return val;

                return Convert.ToInt64(0);
            }

            if (source.GetType().IsNumericType())
            {
                try
                {
                    return Convert.ToInt64(source);
                }
                catch { return Convert.ToInt64(0); }
            }

            return Convert.ToInt64(0);
        }

        if (targettype == typeof(decimal))
        {
            if (source is string strsrc && strsrc.Length > 0)
            {
                if (decimal.TryParse(strsrc, out var val))
                    return val;

                return Convert.ToDecimal(0);
            }

            if (source.GetType().IsNumericType())
            {
                try
                {
                    return Convert.ToDecimal(source);
                }
                catch { return Convert.ToDecimal(0); }
            }

            return Convert.ToDecimal(0);
        }

        if (targettype == typeof(double))
        {
            if (source is string strsrc && strsrc.Length > 0)
            {
                if (double.TryParse(strsrc, out var val))
                    return val;

                return Convert.ToDouble(0);
            }

            if (source.GetType().IsNumericType())
            {
                try
                {
                    return Convert.ToDouble(source);
                }
                catch { return Convert.ToDouble(0); }
            }

            return Convert.ToDouble(0);
        }

        if (targettype == typeof(float))
        {
            if (source is string strsrc && strsrc.Length > 0)
            {
                if (float.TryParse(strsrc, out var val))
                    return val;

                return Convert.ToSingle(0);
            }

            if (source.GetType().IsNumericType())
            {
                try
                {
                    return Convert.ToSingle(source);
                }
                catch { return Convert.ToSingle(0); }
            }

            return Convert.ToSingle(0);
        }

        if (targettype == typeof(byte))
        {
            if (source is string strsrc && strsrc.Length > 0)
            {
                if (byte.TryParse(strsrc, out var val))
                    return val;

                return Convert.ToByte(0);
            }

            if (source.GetType().IsNumericType())
            {
                try
                {
                    return Convert.ToByte(source);
                }
                catch { return Convert.ToByte(0); }
            }

            return Convert.ToByte(0);
        }

        if (targettype == typeof(DateOnly))
        {
            if (source is string strsrc && strsrc.Length > 0)
            {
                if (DateOnly.TryParse(strsrc, out var val))
                    return val;

                return DateOnly.MinValue;
            }

            if (source is DateTime dt) return new DateOnly(dt.Year, dt.Month, dt.Day);

            return DateOnly.MinValue;
        }

        if (targettype == typeof(TimeOnly))
        {
            if (source is string strsrc && strsrc.Length > 0)
            {
                if (TimeOnly.TryParse(strsrc, out var val))
                    return val;

                return TimeOnly.MinValue;
            }

            if (source is DateTime dt) return new TimeOnly(dt.Hour, dt.Minute, dt.Second);

            return TimeOnly.MinValue;
        }

        if (targettype == typeof(DateTime))
        {
            if (source is string strsrc && strsrc.Length > 0)
            {
                if (DateTime.TryParse(strsrc, out var val))
                    return val;

                return DateTime.MinValue;
            }

            if (source is DateOnly dt) return new DateTime(dt.Year, dt.Month, dt.Day);

            return DateTime.MinValue;
        }

        if (targettype == typeof(Guid))
        {
            if (source is string strsrc && strsrc.Length > 0)
                if (Guid.TryParse(strsrc, out var val)) return val;

            return Guid.Empty;
        }

        if (targettype == typeof(string))
            return source.ToString();

        return null;
    }

    /// <summary>
    /// Ermittelt den Maximalwert von zwei Werten
    /// </summary>
    /// <typeparam name="T">Typ der Werte</typeparam>
    /// <param name="first">erster Wert</param>
    /// <param name="second">zweiter Wert</param>
    /// <returns>der größere der beiden Werte</returns>
    public static T Max<T>(T first, T second) 
    {
        if (Comparer<T>.Default.Compare(first, second) > 0)
            return first;
        return second;
    }

    /// <summary>
    /// Ermittelt den Mindestwert von zwei Werten
    /// </summary>
    /// <typeparam name="T">Typ der Werte</typeparam>
    /// <param name="first">erster Wert</param>
    /// <param name="second">zweiter Wert</param>
    /// <returns>der kleinere der beiden Werte</returns>
    public static T Min<T>(T first, T second) 
    {
        if (Comparer<T>.Default.Compare(first, second) > 0)
            return second;
        return first;
    }

    /// <summary>
    /// Ermittelt, ob auf ein Objekt über eine bestimmte Get-Schnittstelle zugegriffen werden kann.
    /// </summary>
    /// <param name="obj">das Objekt</param>.
    /// <param name="getter">Name der Eigenschaft</param>.
    /// <returns>true wenn das Objekt eine entsprechende Schnittstelle hat, sonst false</returns>
    public static bool HasGet(this object obj, string getter)
    {
        return GetGetter(obj, getter) != null;
    }

    /// <summary>
    /// Ermittelt, ob ein Objekt eine bestimmte Get-Schnittstelle hat und gibt das entsprechende PropertyInfo-Objekt zurück.
    /// </summary>
    /// <param name="obj">das Objekt</param>.
    /// <param name="getter">Name der Eigenschaft</param>.
    /// <returns>PropertyInfo des Getters, wenn das Objekt eine entsprechende Schnittstelle hat, sonst null</returns>
    public static PropertyInfo? GetGetter(this object obj, string getter)
    {
        PropertyInfo? ret = obj.GetType().GetProperty(getter,
            BindingFlags.GetProperty | BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy |
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        return ret == null || !ret.CanRead ? null : ret;
    }

    /// <summary>
    /// Gibt den Typ einer bestimmten Get-Schnittstelle eines Objekts zurück.
    /// </summary>
    /// <param name="obj">das Objekt</param>.
    /// <param name="getter">Name der Eigenschaft</param>.
    /// <returns>Typ der vom Getter gelieferten Daten</returns>
    public static Type? GetTypeOfGet(this object obj, string getter)
    {
        return GetGetter(obj, getter)?.PropertyType;
    }

    /// <summary>
    /// Gibt den Typ einer bestimmten Mengenschnittstelle eines Objekts zurück.
    /// </summary>
    /// <param name="obj">das Objekt</param>.
    /// <param name="setter">Name der Eigenschaft</param>.
    /// <returns>Typ der vom Set erwarteten Daten</returns>
    public static Type? GetTypeOfSet(this object obj, string setter)
    {
        return GetSetter(obj, setter)?.PropertyType;
    }

    /// <summary>
    /// Ermittelt, ob auf ein Objekt über eine bestimmte Set-Schnittstelle zugegriffen werden kann.
    /// </summary>
    /// <param name="obj">das Objekt</param>.
    /// <param name="setter">Name der Eigenschaft</param>.
    /// <returns>true wenn das Objekt eine entsprechende Schnittstelle hat, sonst false</returns>
    public static bool HasSet(this object obj, string setter)
    {
        return GetSetter(obj, setter) != null;
    }

    /// <summary>
    /// Ermittelt, ob ein Objekt eine bestimmte Set-Schnittstelle hat und gibt das entsprechende PropertyInfo-Objekt zurück.
    /// </summary>
    /// <param name="obj">das Objekt</param>.
    /// <param name="setter">Name der Eigenschaft</param>.
    /// <returns>PropertyInfo des Setters, wenn das Objekt eine entsprechende Schnittstelle hat, sonst null</returns>
    public static PropertyInfo? GetSetter(this object obj, string setter)
    {
        PropertyInfo? ret = obj.GetType().GetProperty(setter,
            BindingFlags.SetProperty | BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy |
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        return ret == null || !ret.CanWrite ? null : ret;
    }

    /// <summary>
    /// Ermittelt, ob ein Objekt eine bestimmte Methode hat.
    /// </summary>
    /// <param name="obj">Name des Objekts</param>.
    /// <param name="method">Name der Methode (Groß-/Kleinschreibung beachten!)</param>.
    /// <returns>true wenn das Objekt eine entsprechende Methode hat, sonst false</returns>
    public static bool HasMethod(this object obj, string method)
    {
        return obj.GetType().GetMethod(method,
            BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance |
            BindingFlags.IgnoreCase) == null ^ true;
    }

    /// <summary>
    /// ruft eine Methode in einem Objekt über den Namen der Methode auf.
    /// Wenn die Methode nicht existiert, wird eine Methode namens NoMethod im Objekt gesucht, und wenn sie gefunden wird 
    /// gefunden wird, wird sie ebenfalls aufgerufen.
    /// </summary>
    /// <param name="obj">Objekt, in dem die Methode aufgerufen wird</param>.
    /// <param name="method">Name der aufzurufenden Methode</param>.
    /// <param name="args">Argumente, die mit der Methode übergeben werden</param>.
    /// <returns>Returnt die Methode, sonst null</returns>
    public static object? InvokeMethod(this object obj, string method, params object[] args)
    {
        return obj.GetType().InvokeMember(method,
            BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy |
            BindingFlags.Instance | BindingFlags.IgnoreCase, null, obj, args);
    }

    /// <summary>
    /// Holt den Wert einer Eigenschaft durch Zugriff auf den Get-Code nach Name 
    /// </summary>
    /// <param name="obj">Objekt, in dem die Eigenschaft enthalten ist</param>.
    /// <param name="getter">Name der Eigenschaft</param>.
    /// <returns>Wert der Eigenschaft</returns>
    public static object? InvokeGet(this object obj, string getter)
    {
        return GetGetter(obj, getter)?.GetValue(obj, null);
    }

    /// <summary>
    /// Weist einer Eigenschaft einen Wert zu, indem er auf den Setcode nach Namen zugreift. 
    /// </summary>
    /// <param name="obj">Objekt, das die Eigenschaft enthält</param>.
    /// <param name="setter">Name der Eigenschaft</param>.
    /// <param name="value">Zuweisender Wert</param>.
    public static void InvokeSet(this object obj, string setter, object value)
    {
        GetSetter(obj, setter)?.SetValue(obj, value, null);
    }

    /// <summary>
    /// Serialisiere Objekt als JSON in einen Stream
    /// </summary>
    /// <param name="obj">Objekt, das serialisiert werden soll</param>.
    /// <param name="stream">Stream, in den serialisiert werden soll</param>.
    public static void ToJsonStream(this object obj, Stream stream)
    {
        Functions.SerializeToJsonStream(obj, stream);
    }

    /// <summary>
    /// Serialisiere Objekt als JSON ByteArray
    /// </summary>
    /// <param name="obj">Objekt, das serialisiert werden soll</param>.
    public static byte[] ToJsonBytes(this object obj)
    {
        return Functions.SerializeToJsonBytes(obj);
    }

    /// <summary>
    /// Serialisiere Objekt als JSON String
    /// </summary>
    /// <param name="obj">Objekt, das serialisiert werden soll</param>.
    public static string ToJsonString(this object obj)
    {
        return Functions.SerializeToJsonString(obj);
    }

    /// <summary>
    /// Objekt als JSON in eine Datei serialisieren
    /// </summary>
    /// <param name="obj">Objekt, das serialisiert werden soll</param>.
    /// <param name="file">Datei, in die das Objekt geschrieben werden soll</param>.
    public static void ToJsonFile(this object obj, FileInfo file)
    {
        if (file.Directory == null)
            throw new NullReferenceException(CoreStrings.ERR_FILE_NODIRECTORY);

        if (!file.Directory.Exists)
            file.Directory.Create();

        Functions.SerializeToJsonFile(obj, file);
    }

    


    /// <summary>
    /// Objekt als xml in eine Datei serialisieren
    /// </summary>
    /// <param name="obj">Objekt, das serialisiert werden soll</param>
    /// <param name="file">Datei, in die das Objekt geschrieben werden soll</param>
    public static void ToXmlFile(this object obj, FileInfo file)
    {
        if (file.Directory == null)
            throw new NullReferenceException(CoreStrings.ERR_FILE_NODIRECTORY);

        if (!file.Directory.Exists)
            file.Directory.Create();

        Functions.SerializeToXmlFile(obj, file);
    }

    /// <summary>
    /// Objekt in einen XML-String serialisieren
    /// </summary>
    /// <param name="obj">zu serialisierendes Objekt</param>
    /// <returns>XML String mit den serialsierten Daten</returns>
    public static string ToXmlString(this object obj)
    {
        return Functions.SerializeToXmlString(obj);
    }

    /// <summary>
    /// Objekt in einen XML-Stream serialisieren
    /// </summary>
    /// <param name="obj">zu serialisierendes Objekt</param>
    /// <param name="stream">Stream, in den die Daten geschrieben werden</param>
    /// <returns>XML String mit den serialsierten Daten</returns>
    public static void ToXmlStream(this object obj, Stream stream)
    {
        Functions.SerializeToXmlStream(obj, stream);
    }

    /// <summary>
    /// Objekt in einen XML-Stream serialisieren
    /// </summary>
    /// <param name="obj">zu serialisierendes Objekt</param>
    /// <returns>XML String mit den serialsierten Daten</returns>
    public static byte[] ToXmlBytes(this object obj)
    {
        return Functions.SerializeToXmlBytes(obj);
    }
        
    /// <summary>
    /// Komprimiert ein byte[] via ZIP
    /// </summary>
    /// <param name="array">zu komprimierende Daten</param>.
    /// <returns>Byte-Array mit den komprimierten Daten</returns>.
    public static byte[] Compress(this byte[] array)
    {
        return Functions.Compress(array);
    }

    /// <summary>
    /// dekomprimiert über ZIP komprimierte byte[]
    /// </summary>
    /// <param name="array">komprimierte Daten</param>.
    /// <returns> unkomprimierte Daten</returns>
    public static byte[] Decompress(this byte[] array)
    {
        return Functions.Decompress(array);
    }

    /// <summary>
    /// Aufruf einer generischen Methode
    /// 
    /// conn.InvokeGeneric("Check", new Type[] {typeof(User)}, null)
    /// conn.InvokeGeneric("Prüfen", new Type[] {typeof(User)}, ["Heiko", "Müller"])
    /// </summary>
    /// <param name="obj">Objekt, für das die generische Methode aufgerufen werden soll</param>.
    /// <param name="name">Name der Methode</param>.
    /// <param name="parameters">Parameter, die an die Methode übergeben werden sollen</param>.
    /// <param name="genTypes">Generische Typen</param>
    public static object? InvokeGeneric(this object obj, string name, Type[] genTypes, params object?[] parameters)
    {
        var paraTypes = parameters.AsEnumerable().Select(o => o?.GetType() ?? typeof(Nullable)).ToArray();

        var mi = obj.GetType().FindMethod(name, genTypes, paraTypes);

        if (mi == null)
            mi = obj.GetType().GetExtensionMethod(name, true);

        // letzte Variante nur über den Namen der Methode versuchen...
        if (mi == null) mi = obj.GetType().GetMethods().FirstOrDefault(m => m.Name == name && m.GetParameters().Length == parameters.Length);

        if (mi == null)
            throw new Exception($@"InvokeGeneric: Generic method {name} not found.");

        var mref = mi.MakeGenericMethod(genTypes);

        try
        {
            // Methode aufrufen
            return mref.Invoke(obj, parameters);
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

    /// <summary>
    /// Aufruf einer Methode nach Name
    /// 
    /// conn.Invoke("Check", "Heiko", "Müller")
    /// </summary>
    /// <param name="obj">Objekt, für das die generische Methode aufgerufen werden soll</param>.
    /// <param name="name">Name der Methode</param>.
    /// <param name="parameters">Parameter, die an die Methode übergeben werden</param>.
    public static object? InvokeByName(this object obj, string name, params object[] parameters)
    {
        var paraTypes = parameters.AsEnumerable().Select(o => o.GetType()).ToArray();

        var mi = obj.GetType().GetMethod(name, paraTypes);

        if (mi == null)
            mi = obj.GetType().GetExtensionMethod(name, true);

        if (mi == null)
            throw new Exception($@"Invoke: Method {name} not found.");

        return mi.Invoke(obj, parameters); 

    }


    /// <summary>
    /// Aufruf einer generischen Methode
    /// 
    /// conn.InvokeGeneric("Check", typeof(User))
    /// </summary>
    /// <param name="obj">Objekt, für das die generische Methode aufgerufen werden soll</param>.
    /// <param name="name">Name der Methode</param>.
    /// <param name="genericType"></param>
    public static object? InvokeGeneric(this object obj, string name, Type genericType)
    {
        var mi = obj.GetType().GetMethods()
            .FirstOrDefault(method => method.Name == name && !method.GetParameters().Any());

        if (mi == null)
            throw new Exception($@"InvokeGeneric: Generic method {name} not found.");

        var mref = mi.MakeGenericMethod(genericType);

        return mref.Invoke(obj, null);

    }

    /// <summary>
    /// Konvertiert einen Wert in einen Int64-Wert
    /// 
    /// Wenn der übergebene Wert ein BigInteger ist und dieser den Wert 
    /// von Int64.MaxValue überschreitet, wird eine OverflowException ausgelöst.
    /// </summary>
    /// <param name="obj">Wert, der konvertiert werden soll</param>.
    /// <returns>Int64-Wert</returns>
    public static long ToInt64(this object obj)
    {
        return obj switch
        {
            long l => l,
            BigInteger integer when integer > long.MaxValue => throw new OverflowException(),
            BigInteger integer => (long)(integer % long.MaxValue),
            _ => Convert.ToInt64(obj)
        };
    }
}

/// <summary>
/// Erweiterungsmethoden für Arrays
/// </summary>
public static class ArrayEx
{
    /// <summary>
    /// Zwei Arrays zusammenführen.
    ///
    /// Beim Zusammenführen wird der Inhalt des Arrays 'add' an das Array 'source' angehängt.
    /// </summary>
    /// <param name="source">Array, das erweitert wird</param>
    /// <param name="add">Array, dass an das source Array angehängt wird</param>
    public static void Merge(object[] source, object[] add)
    {
        Array.Resize(ref source, source.Length + add.Length);
        Array.Copy(add, 0, source, source.Length - add.Length, add.Length);
    }
}