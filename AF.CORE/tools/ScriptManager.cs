#if (NET8_0_OR_GREATER)
using System.Runtime.Loader;
#endif

using System.Reflection;
using Flee.PublicTypes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AF.CORE;

/// <summary>
/// Einfache Scripting-Engine.
/// 
/// Alle kompilierten Scripts werden, wenn die Kompilierung via CompileScript ausgeführt wird in einem
/// Cache abgelegt. Dazu muss der Name des Scripts EINDEUTIG sein. Beim nächsten CompileScript wird das 
/// Script dann aus dem Cache verwendet und NICHT mehr neu kompiliert.
/// 
/// Soll der Script immer neu kompiliert und NICHT im Cache abgelegt werden, muss er via ExecuteScript ausgeführt werden.
/// </summary>
[Localizable(false)]
public class ScriptManager : ScriptManagerBase, IScriptService
{
    private static ScriptManager? _instance;

    /// <summary>
    /// Zugriff auf den ScriptManger (Singleton)
    /// </summary>
    public static ScriptManager Instance => _instance ??= new();

    private ScriptManager()
    {

    }

    /// <inheritdoc />
    public virtual BindingList<IScriptSnippet> GetSnippets()
    {
        return [];
    }

    /// <inheritdoc />
    public AFCommand? CmdGoto { get; set; }

    /// <inheritdoc />
    public AFCommand? CmdAdd { get; set; }
}

/// <summary>
/// Basisklasse der IScriptingService Klassen
/// </summary>
public abstract class ScriptManagerBase
{
    private readonly Dictionary<Guid, AFScriptBase> _scriptcache = new();
    private readonly ExpressionContext _evaluator = new();

    /// <summary>
    /// Konstruktor
    /// </summary>
    public ScriptManagerBase()
    {
        _evaluator.Imports.AddType(typeof(Math));
        _evaluator.Imports.AddType(typeof(ScriptFunctions));
        _evaluator.ParserOptions.DecimalSeparator = '.';
        _evaluator.ParserOptions.RequireDigitsBeforeDecimalPoint = true;
        _evaluator.ParserOptions.FunctionArgumentSeparator = ',';
        _evaluator.ParserOptions.RecreateParser();
    }
    
    #region Flee-Expressions
    /// <summary>
    /// Registriert eine oder mehrere KLassen, deren Methoden anschließend im Expression-Evaluator zur Verfügung stehen.
    /// </summary>
    /// <param name="types">Liste der zu registrierenden Klassen/Typen</param>
    public void RegisterFunctions(Type[] types)
    {
        foreach (var type in types)
            _evaluator.Imports.AddType(type);

        _evaluator.ParserOptions.RecreateParser();
    }

    /// <summary>
    /// Registriert eine Variable die im Evaluator verwendet werden kann.
    /// </summary>
    /// <param name="name">Name der Variablen</param>
    /// <param name="value">Wert der Variablen</param>
    public void SetVariable(string name, object value)
    {
        if (_evaluator.Variables.ContainsKey(name))
            _evaluator.Variables[name] = value;
        else
            _evaluator.Variables.Add(name, value);
    }

    /// <summary>
    /// Löscht eine im Evaluator registrierte Variable. 
    /// </summary>
    /// <param name="name">Name der Variablen</param>
    public void RemoveVariable(string name)
    {
        if (_evaluator.Variables.ContainsKey(name))
            _evaluator.Variables.Remove(name);
    }

    /// <summary>
    /// Löscht alle im Evaluator registrierten Variablen. 
    /// </summary>
    public void ClearVariables()
    {
        _evaluator.Variables.Clear();
    }

    /// <summary>
    /// Einen Ausdruck interpretieren und ausführen (Evaluator).
    /// 
    /// Kann mit einer vorkomplierten Expression arbeiten, um mehrfaches kompilieren zu vermeiden.
    /// </summary>
    /// <param name="expression">zu interpretierender Ausdruck</param>
    /// <param name="preCompiled">vorkompilierte Expression (siehe CompileExpression)</param>
    /// <returns>Wert oder NULL</returns>
    public object EvaluateExpression(string expression, IDynamicExpression? preCompiled = null)
    {
        if (preCompiled != null)
            return preCompiled.Evaluate();

        return _evaluator.CompileDynamic(expression).Evaluate();
    }

    /// <summary>
    /// Einen Ausdruck interpretieren und die compilierte Expression zurückgeben, um diese dann mit EvaluateExpression verwenden zu können.
    /// </summary>
    /// <param name="expression">zu interpretierender Ausdruck</param>
    /// <returns>Wert oder NULL</returns>
    public IDynamicExpression CompileExpression(string expression)
    {
        return _evaluator.CompileDynamic(expression);
    }

    /// <summary>
    /// Zugriff auf den Flee-Evaluator
    /// </summary>
    public ExpressionContext Evaluator => _evaluator;
    #endregion
    
    /// <summary>
    /// Cache leeren.
    /// 
    /// Kann auch genutzt werden, um beim Start des Programms den Compiler zu initialisieren.
    /// </summary>
    public void ClearCache() { _scriptcache.Clear(); }

    /// <summary>
    /// Script compilieren
    /// </summary>
    /// <param name="source">Quelltext</param>
    /// <param name="id">ID des Script, wenn != Guid.Empty, wird der interne ScriptCache verwendet</param>
    /// <param name="script">Ergebnis des Kompilierens - ein Objekt der Klasse im Script</param>
    /// <param name="result">Ergebnis des Kompilierens (Fehlermeldungen etc.)</param>
    /// <param name="options">weitere Optionen</param>
    /// <param name="domain">
    /// AppDomain, in die der Script geladen werden soll.
    /// 
    /// Wird keine Domain angegeben, wird der Script im aktuellen AppDomain geladen. 
    /// Nur OHNE die Angabe einer Domain werden Scripte gecacht!
    /// </param>
    /// <returns>true, wenn erfolgreich, sonst false</returns>
    public bool TryToCompile<T>(string source, Guid id, out T? script, ScriptCompileResult result, ScriptCompileOptions options, AppDomain? domain) where T : AFScriptBase
    {
        script = null;

        if (domain == null && id.IsEmpty() == false && _scriptcache.ContainsKey(id) && options.ReCompile == false)
        {
            script = _scriptcache[id] as T;
            return true;
        }

        var sbSource = StringBuilderPool.GetStringBuilder(content: source);

        // Platzhalter ersetzen
        foreach (var pair in AFCore.Placeholder)
        {
            if (source.Contains(pair.Value.Name))
                sbSource.Replace(pair.Value.Name, pair.Value.GetValue());
        }

        source = sbSource.ToString();
        StringBuilderPool.ReturnStringBuilder(sbSource);

        List<string> assemblys =
        [
            typeof(object).GetTypeInfo().Assembly.Location,
            typeof(Console).GetTypeInfo().Assembly.Location
        ];

        var path = Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location);

        if (!string.IsNullOrEmpty(path))
            assemblys.Add(Path.Combine(path, "System.Runtime.dll"));

        if (options.Assemblies.Count > 0)
            assemblys.AddRange(options.Assemblies);

        if (source.Contains("using System.Windows.Forms;"))
        {
            assemblys.Add("System.Windows.Forms.dll");
            assemblys.Add("System.Drawing.dll");
        }

        // weitere Assemblies einbinden
        // diese stehen im Script in Zeilen, die mit //@ beginnen (//@ <name der dll>)
        var assemblies = source.GetAssembliesFromCode();

        foreach (var assemblyfile in assemblies)
        {
            if (assemblys.FindIndex(a => a.ToLowerInvariant().Trim().EndsWith(assemblyfile.ToLowerInvariant())) < 0)
                assemblys.Add(assemblyfile);
        }

        var defaultLocation = new FileInfo(typeof(object).Assembly.Location).DirectoryName ?? "";
        var execLocation = new FileInfo(Assembly.GetEntryAssembly()?.Location ?? "").DirectoryName ?? "";
        var ass1Location = Path.Combine(execLocation, "Assemblies");
        var ass2Location = Path.Combine(execLocation, "Assemblys");

        for (var ipos = 0; ipos < assemblys.Count; ++ipos)
        {
            if (File.Exists(assemblys[ipos]))
                continue;

            if (assemblys[ipos].Contains('\\'))
                continue;

            var targetfile = Path.Combine(defaultLocation, assemblys[ipos]);

            if (File.Exists(targetfile))
            {
                assemblys[ipos] = targetfile;
                continue;
            }

            targetfile = Path.Combine(execLocation, assemblys[ipos]);

            if (File.Exists(targetfile))
            {
                assemblys[ipos] = targetfile;
                continue;
            }

            targetfile = Path.Combine(ass1Location, assemblys[ipos]);

            if (File.Exists(targetfile))
            {
                assemblys[ipos] = targetfile;
                continue;
            }

            assemblys[ipos] = Path.Combine(ass2Location, assemblys[ipos]);
        }


        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var assemblyName = Path.GetRandomFileName();
        var references = assemblys.Select(r => MetadataReference.CreateFromFile(r)).ToArray();

        var compilation = CSharpCompilation.Create(
            assemblyName,
            syntaxTrees: [syntaxTree],
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        Assembly? assembly;

        using (var ms = new MemoryStream())
        {
            var emitResult = compilation.Emit(ms);

            if (!emitResult.Success)
            {
                IEnumerable<Diagnostic> failures = emitResult.Diagnostics.Where(diagnostic =>
                    diagnostic.IsWarningAsError ||
                    diagnostic.Severity == DiagnosticSeverity.Error);

                result.Stack.Clear();

                foreach (var diagnostic in failures)
                    result.Stack.Add(new StackEntry { Type = (diagnostic.IsWarningAsError ? CoreStrings.WARNING : CoreStrings.ERROR), Message = diagnostic.GetMessage() });

                return false;
            }

            ms.Seek(0, SeekOrigin.Begin);

            #if NET481_OR_GREATER
            if (domain == null)
                assembly = Assembly.Load(ms.ToArray());
            else
            {
                var assemblyLoader = (IAssemblyLoader)domain.CreateInstanceFromAndUnwrap(typeof(AssemblyLoader).Assembly.Location, typeof(AssemblyLoader).FullName!);
                assembly = assemblyLoader.Load(ms.ToArray());
            }
            #else


                if (domain == null)
                    assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
                else
                {
                    var loader = domain.CreateInstanceFromAndUnwrap(typeof(AssemblyLoader).Assembly.Location,
                        typeof(AssemblyLoader).FullName!);
 
                    if (loader is IAssemblyLoader assemblyLoader)
                        assembly = assemblyLoader.Load(ms.ToArray());
                    else
                        throw new InvalidOperationException($"AssemlyLoader {typeof(AssemblyLoader).FullName} not available");
                }
            #endif
        }

        if (assembly.CreateInstance(options.NameSpace + "." + options.ClassName, false, BindingFlags.CreateInstance, null, null, System.Globalization.CultureInfo.CurrentCulture, null) is T t)
            script = t;

        if (script == null)
            return false;
        else
        {
            if (domain != null || id.IsEmpty()) return true;

            _scriptcache[id] = script;

            return true;
        }

    }

    /// <summary>
    /// Führt einen im Cache abgelegten Script aus und gibt das Resultat zurück.
    /// </summary>
    /// <param name="id">ID des Scripts</param>
    /// <param name="method">aufzurufende Methode (Standard: Execute)</param>
    /// <param name="parameter">Parameter zur Übergabe an die Methode</param>
    /// <returns>Ergebnis der Ausführung</returns>
    public object? Execute(Guid id, string method = "Execute", params object[] parameter)
    {
        object? result;

        if (_scriptcache.ContainsKey(id))
            result = _scriptcache[id].GetType().GetMethod(method)?.Invoke(_scriptcache[id], parameter);
        else
            throw new NullReferenceException(string.Format(CoreStrings.ERR_WRONGSAFIPTID, id));

        return result;
    }

    private Type? scriptSelectControlType;


    /// <summary>
    /// Registriert den Typ des Controls über das ein
    /// Script ausgewählt werden kann.
    ///
    /// Das Control/der Typ muss die Schnittstelle IScriptSelectControl implementieren.
    /// </summary>
    /// <param name="controlType"></param>
    public void RegisterScriptSelectControlType(Type controlType)
    {
        if (!controlType.HasInterface(typeof(IScriptSelectControl)))
            throw new ArgumentException($"Der Typ {controlType.FullName} muss das Interface IScriptSelectControl implementieren.", nameof(controlType));

        scriptSelectControlType = controlType;
    }

    /// <summary>
    /// Liefert den vorher via RegisterScriptSelectControlType registrierten Typ des Controls für die Script-Auswahl.
    /// </summary>
    /// <exception cref="NullReferenceException">Wenn kein Typ registriert wurde (via RegisterScriptSelectControlType)</exception>
    public Type GetScriptSelectControlType()
    {
        if (scriptSelectControlType == null) throw new NullReferenceException("Es wurde kein ControlType zur AUswahl der Scripte registriert (via RegisterScriptSelectControlType).");

        return scriptSelectControlType;
    }
}

/// <summary>
/// Optionen für das compilieren
/// </summary>
[Localizable(false)]
public class ScriptCompileOptions
{
    /// <summary>
    /// Code neu übersetzen, den Cache nicht verwenden/überschreiben
    /// </summary>
    public bool ReCompile { get; set; }

    /// <summary>
    /// Liste der einzubindenden Assemblies
    /// Angabe als Name der DLL (*.dll) ohne Pfad, wenn sich diese im Programmverzeichnis 
    /// oder im Unterverzeichnis 'Assemblies' befindet. Sonst Angabe mit Pfad.
    /// 
    /// Alternativ: Angabe im Quelltext //@ dateiname.dll. 
    /// </summary>
    public HashSet<string> Assemblies { get; set; } = [];

    /// <summary>
    /// Name der Klasse im Script, von der eine Objektinstanz erzeugt werden soll (Standard ist 'Script')
    /// </summary>
    public string ClassName { get; set; } = "Script";

    /// <summary>
    /// Name des Namespace im Script, in der sich die Klasse befindet, von der eine Objektinstanz erzeugt werden soll (Standard ist 'cr3.script')
    /// </summary>
    public string NameSpace { get; set; } = "cr3.script";

    /// <summary>
    /// Eine Standard-Assemblies hinzufügen
    /// </summary>
    public void AddDefaultAssemblies()
    {
        string[] toAdd =
        [
            "System.dll",
                "System.Data.dll",
                "System.Linq.dll",
                "System.Xml.dll",
                "System.Drawing.dll"
        ];

        foreach (var ass in toAdd)
            Assemblies.Add(ass);
    }
}

/// <summary>
/// Ergebnis der Ausführung
/// </summary>
public class ScriptCompileResult
{
    /// <summary>
    /// Liste der Fehler und Warnungen
    /// </summary>
    public List<StackEntry> Stack { get; set; } = [];

    /// <summary>
    /// Gibt an ob Warnungen oder Fehler vorhanden sind
    /// </summary>
    public bool HasErrors => Stack.Count > 0;
}

/// <summary>
/// Eintrag/Zeile im StackTrace
/// </summary>
public class StackEntry
{
    /// <summary>
    /// Zeilennummer
    /// </summary>
    public int LineNumber { get; set; }

    /// <summary>
    /// Spaltennummer
    /// </summary>
    public int ColumnNumber { get; set; }

    /// <summary>
    /// Nachricht/Fehler
    /// </summary>
    public string Message { get; set; } = "";

    /// <summary>
    /// Typ: Fehler oder Warnung
    /// </summary>
    public string Type { get; set; } = "";
}

/// <summary>
/// Interface für den AssemblyLoader
/// </summary>
public interface IAssemblyLoader
{
    /// <summary>
    /// Assembly in die aktuelle AppDomain laden
    /// </summary>
    /// <param name="bytes">Assembly-Code</param>
    /// <returns>die geladene Assembly</returns>
    Assembly Load(byte[] bytes);
}

/// <summary>
/// Loader für Assemblies, die in einer anderen AppDomain geladen werden.
/// </summary>
public class AssemblyLoader : MarshalByRefObject, IAssemblyLoader
{
    /// <summary>
    /// Lädt die Assembly in die AppDomain.
    /// </summary>
    /// <param name="bytes">Assembly</param>
    /// <returns>die geladene Assembly</returns>
    public Assembly Load(byte[] bytes)
    {
        return AppDomain.CurrentDomain.Load(bytes);
    }

}

/// <summary>
/// Statische Funktionen für Scripte und Expressions (ExpressionEvaluator)
/// </summary>
public static class ScriptFunctions
{
    #region String
    /// <summary>
    /// Länge eines Strings
    /// </summary>
    /// <param name="data">string</param>
    /// <returns>Länge des Strings</returns>
    public static int Len(string data) { return data.Length; }

    /// <summary>
    /// Zeichen von Links
    /// </summary>
    /// <param name="data">String</param>
    /// <param name="count">Anzahl</param>
    /// <returns>Teil-String von links beginnend</returns>
    public static string Left(string data, int count) { return data.Left(count); }

    /// <summary>
    /// Zeichen von Rechts
    /// </summary>
    /// <param name="data">String</param>
    /// <param name="count">Anzahl</param>
    /// <returns>Teil-String von links beginnend</returns>
    public static string Right(string data, int count) { return data.Right(count); }

    /// <summary>
    /// Zeichen ab einer Position
    /// </summary>
    /// <param name="data">String</param>
    /// <param name="count">Anzahl</param>
    /// <param name="start">Startposition</param>
    /// <returns>Teil-String ab einer Position</returns>
    public static string SubString(string data, int start, int count) { return data.Substring(start, count); }

    /// <summary>
    /// Zeichen im String ersetzen
    /// </summary>
    /// <param name="data">String</param>
    /// <param name="replace">ersetzen von</param>
    /// <param name="replacewith">ersetzen durch</param>
    /// <returns>Ergebnis</returns>
    public static string Replace(string data, string replace, string replacewith) { return data.Replace(replace, replacewith); }

    /// <summary>
    /// Zwei Texte zusammenfügen
    /// </summary>
    /// <param name="data">String</param>
    /// <param name="with">String der verbunden wird</param>
    /// <returns>Ergebnis</returns>
    public static string Concat(string data, string with) { return data+with; }

    /// <summary>
    /// String in Grossbuchstaben umwandeln
    /// </summary>
    /// <param name="data">String</param>
    /// <returns>Umgewandelter String</returns>
    public static string Upper(string data) { return data.ToUpper(); }

    /// <summary>
    /// String in Kleinbuchstaben umwandeln
    /// </summary>
    /// <param name="data">String</param>
    /// <returns>Umgewandelter String</returns>
    public static string Lower(string data) { return data.ToLowerInvariant(); }

    /// <summary>
    /// Erste Position eines Textes in einem String
    /// </summary>
    /// <param name="data">String</param>
    /// <param name="seek">zu suchender Text</param>
    /// <returns>Position</returns>
    public static int Pos(string data, string seek) { return data.IndexOf(seek, StringComparison.OrdinalIgnoreCase); }

    /// <summary>
    /// Position eines Textes in einem String nach einer bestimmten Position
    /// </summary>
    /// <param name="data">String</param>
    /// <param name="seek">zu suchender Text</param>
    /// <param name="start">Startposition</param>
    /// <returns>Position</returns>
    public static int PosFrom(string data, string seek, int start) { return data.IndexOf(seek, start, StringComparison.OrdinalIgnoreCase); }

    /// <summary>
    /// Letzte Position eines Textes in einem String
    /// </summary>
    /// <param name="data">String</param>
    /// <param name="seek">zu suchender Text</param>
    /// <returns>Position</returns>
    public static int LastPos(string data, string seek) { return data.LastIndexOf(seek, StringComparison.OrdinalIgnoreCase); }

    /// <summary>
    /// Position eines Textes in einem String vor einer bestimmten Position
    /// </summary>
    /// <param name="data">String</param>
    /// <param name="seek">zu suchender Text</param>
    /// <param name="start">Startposition</param>
    /// <returns>Position</returns>
    public static int LastPosFrom(string data, string seek, int start) { return data.LastIndexOf(seek, start, StringComparison.OrdinalIgnoreCase); }

    /// <summary>
    /// Prüft ob der String mit einem bestimmten Text beginnt (Groß-/Klein-Schreibweise wird ignoriert)
    /// </summary>
    /// <param name="data">String</param>
    /// <param name="starts">gesuchter Text</param>
    /// <returns>true oder false</returns>
    public static bool StartsWith(string data, string starts) { return data.StartsWith(starts, StringComparison.OrdinalIgnoreCase); }

    /// <summary>
    /// Prüft ob der String mit einem bestimmten Text endet (Groß-/Klein-Schreibweise wird ignoriert)
    /// </summary>
    /// <param name="data">String</param>
    /// <param name="ends">gesuchter Text</param>
    /// <returns>true oder false</returns>
    public static bool EndsWith(string data, string ends) { return data.EndsWith(ends, StringComparison.OrdinalIgnoreCase); }


    /// <summary>
    /// Prüft ob der String NICHT mit einem bestimmten Text beginnt (Groß-/Klein-Schreibweise wird ignoriert)
    /// </summary>
    /// <param name="data">String</param>
    /// <param name="starts">gesuchter Text</param>
    /// <returns>true oder false</returns>
    public static bool StartsNotWith(string data, string starts) { return !data.StartsWith(starts, StringComparison.OrdinalIgnoreCase); }

    /// <summary>
    /// Prüft ob der String NICHT mit einem bestimmten Text endet (Groß-/Klein-Schreibweise wird ignoriert)
    /// </summary>
    /// <param name="data">String</param>
    /// <param name="ends">gesuchter Text</param>
    /// <returns>true oder false</returns>
    public static bool EndsNotWith(string data, string ends) { return !data.EndsWith(ends, StringComparison.OrdinalIgnoreCase); }

    /// <summary>
    /// führende Leerzeichen entfernen
    /// </summary>
    /// <param name="data">String</param>
    /// <returns>berbeiteter String</returns>
    public static string LTrim(string data) { return data.TrimStart(); }

    /// <summary>
    /// Leerzeichen am Ende entfernen
    /// </summary>
    /// <param name="data">String</param>
    /// <returns>berbeiteter String</returns>
    public static string RTrim(string data) { return data.TrimEnd(); }

    /// <summary>
    /// Leerzeichen am Ende und am Anfang entfernen
    /// </summary>
    /// <param name="data">String</param>
    /// <returns>berbeiteter String</returns>
    public static string AllTrim(string data) { return data.Trim(); }
    #endregion

    #region DateTime
    /// <summary>
    /// Aktuelles Datum exkl. Uhrezit (0 Uhr)
    /// </summary>
    /// <returns>Datum (0 Uhr)</returns>
    public static DateTime Today() { return DateTime.Now.Date; }

    /// <summary>
    /// Aktuelles Datum inkl. Uhrzeit
    /// </summary>
    /// <returns>Datum und Zeit</returns>
    public static DateTime Now() { return DateTime.Now; }

    /// <summary>
    /// neues Datum erzeugen
    /// </summary>
    /// <param name="year">Jahr</param>
    /// <param name="month">Monat</param>
    /// <param name="day">Tag</param>
    /// <returns>neues Datum</returns>
    public static DateTime NewDate(int year, int month, int day) { return new DateTime(year, month, day); }

    /// <summary>
    /// neues Datum mit Zeit erzeugen
    /// </summary>
    /// <param name="year">Jahr</param>
    /// <param name="month">Monat</param>
    /// <param name="day">Tag</param>
    /// <param name="hour">Stunde</param>
    /// <param name="minute">Minute</param>
    /// <param name="second">Sekunde</param>
    /// <returns>neues Datum</returns>
    public static DateTime NewDateTime(int year, int month, int day, int hour, int minute, int second) { return new DateTime(year, month, day, hour, minute, second); }

    /// <summary>
    /// Tag des Datums
    /// </summary>
    /// <returns>Tag (1- 31)</returns>
    public static int Day(DateTime date) { return date.Day; }

    /// <summary>
    /// Monat des Datums
    /// </summary>
    /// <returns>Monat (1- 12)</returns>
    public static int Month(DateTime date) { return date.Month; }

    /// <summary>
    /// Jahr des Datums
    /// </summary>
    /// <returns>Jahr</returns>
    public static int Year(DateTime date) { return date.Year; }

    /// <summary>
    /// Stunde des Datums
    /// </summary>
    /// <returns>Stunde (0 - 23)</returns>
    public static int Hour(DateTime date) { return date.Hour; }

    /// <summary>
    /// Minute des Datums
    /// </summary>
    /// <returns>Minute (0 - 59)</returns>
    public static int Minute(DateTime date) { return date.Minute; }

    /// <summary>
    /// Sekunde des Datums
    /// </summary>
    /// <returns>Sekunde (0 - 59)</returns>
    public static int Sekunde(DateTime date) { return date.Second; }

    /// <summary>
    /// Kalenderwoche
    /// </summary>
    /// <returns>Kalenderwoche</returns>
    public static int Week(DateTime date) { return System.Globalization.ISOWeek.GetWeekOfYear(date); }


    /// <summary>
    /// Wochentag (1-7)
    /// </summary>
    /// <returns>Wochentag (1-Montag bis 7-Sonntag)</returns>
    public static int Weekday(DateTime date) { return date.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)date.DayOfWeek; }

    /// <summary>
    /// Wochentag (Text: Montrag - Sonntag)
    /// </summary>
    /// <returns>Wochentag (Text: Montrag - Sonntag)</returns>
    public static string WeekdayString(DateTime date) { return date.DayOfWeek.ToString(); }

    /// <summary>
    /// Monate zu einem Datum hinzufügen oder vom Datum abziehen (DateTime)
    /// </summary>
    /// <param name="date">Datum</param>
    /// <param name="months">Anzahl der Monate (+/-)</param>
    /// <returns>neues Datum</returns>
    public static DateTime AddMonths(DateTime date, int months) { return date.AddMonths(months); }

    /// <summary>
    /// Tage zu einem Datum hinzufügen oder vom Datum abziehen (DateTime)
    /// </summary>
    /// <param name="date">Datum</param>
    /// <param name="days">Anzahl der Tage (+/-)</param>
    /// <returns>neues Datum</returns>
    public static DateTime AddDays(DateTime date, int days) { return date.AddDays(days); }

    /// <summary>
    /// Jahre zu einem Datum hinzufügen oder vom Datum abziehen (DateTime)
    /// </summary>
    /// <param name="date">Datum</param>
    /// <param name="years">Anzahl der Jahre (+/-)</param>
    /// <returns>neues Datum</returns>
    public static DateTime AddYears(DateTime date, int years) { return date.AddYears(years); }

    /// <summary>
    /// nur Datum ohne Zeit extrahieren
    /// </summary>
    /// <param name="date">Jahr</param>
    /// <returns>Datum ohne Zeitangabe (00:00:00)</returns>
    public static DateTime DateOnly(DateTime date) { return date.Date; }
    #endregion

    #region Boolean
    /// <summary>
    /// Wandelt einen boolschen Wert in das Gegenteil um
    /// </summary>
    /// <param name="value">Wert</param>
    /// <returns>Gegenteil des Wertes</returns>
    public static bool Not(bool value) { return !value; }

    #endregion

    #region Numerisch
    /// <summary>
    /// Wert finzanztechnisch runden
    /// </summary>
    /// <param name="value">zu rundender Wert</param>
    /// <returns>gerundeter Wert</returns>
    public static decimal RoundFinancial(decimal value) { return Math.Round(Math.Truncate(value * 1000) / 1000, 2, MidpointRounding.AwayFromZero); }

    #endregion

    #region Sonstige




    #endregion

}