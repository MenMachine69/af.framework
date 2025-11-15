using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace AF.CORE;

/// <summary>
/// Universeller Parser, der Funktionsaufrufe und Platzhalter in einer Zeichenkette durch sprach
/// spezifische Funktionsaufrufe ersetzt.
/// 
/// Dies kann zum Beispiel in einem SQL-Query-Parser verwendet werden, der mehrere SQL-Dialekte unterstützt.
/// Da die Ersetzung auch ein längerer Teil des Quellcodes sein kann, kann dies auch für etwas
/// wie 'Makros' in Skripten (einen einfachen Funktionsaufruf oder Platzhalter durch einen längeren Codeschnipsel ersetzen).  
/// </summary>
public sealed class StringFunctionParser
{
    private List<StringParserSnippet> _snippets = [];

    /// <summary>
    /// Verfügbare Snippets zuweisen...
    /// </summary>
    /// <param name="snippets">Dictionary mit verfügbaren Snippets</param>
    public void SetSnippets(List<StringParserSnippet> snippets)
    {
        _snippets = snippets;
    }

    /// <summary>
    /// Weitere Snippets hinzufügen
    /// </summary>
    /// <param name="snippets">Liste mit hinzuzufügenden Snippets</param>
    public void AddSnippets(IEnumerable<StringParserSnippet> snippets)
    {
        _snippets.AddRange(snippets);
    }

    ///// <summary>
    ///// Parse a string and replace all function calls and placeholders against the given snippets
    ///// </summary>
    ///// <param name="query">source string</param>
    ///// <returns>strings with replacments</returns>
    ///// <exception cref="ArgumentException">Exception if parameter count in a function mismatch</exception>
    //public string ParseOrg(string query)
    //{
    //    List<string> parameters = new();

    //    foreach (var func in _snippets)
    //    {
    //        if (func.IsPlaceHolder && query.ContainsIgnoreCase(func.FuncName))
    //        {
    //            query = query.Replace(func.FuncName.Trim(), func.SnippetCode.Trim(), StringComparison.OrdinalIgnoreCase);
    //            continue;
    //        }

    //        while (_extractParameters(func.Pattern.Match(query), out var matchcode, ref parameters))
    //        {
    //            if (parameters.Count != func.ParameterCount)
    //                continue;

    //            if (parameters.Count > 0)
    //            {
    //                string toReplace = func.SnippetCode;
    //                for (int i = 1; i <= parameters.Count; i++)
    //                    toReplace = toReplace.Replace(string.Concat("<p", i, ">"), parameters[i - 1]);
    //                query = query.Replace(string.Concat(func.FuncName, "(", matchcode, ")"), toReplace, StringComparison.OrdinalIgnoreCase);
    //            }
    //            else
    //                query = query.Replace(string.Concat(func.FuncName, "(", matchcode, ")"), func.SnippetCode, StringComparison.OrdinalIgnoreCase);
    //        }
    //    }

    //    return query;
    //}

    /// <summary>
    /// Analysiert eine Zeichenkette und ersetzt alle Funktionsaufrufe und Platzhalter durch die angegebenen Snippets
    /// </summary>
    /// <param name="query">Quellstring</param>
    /// <returns>Strings mit Ersetzungen</returns>
    /// <exception cref="ArgumentException">Ausnahme, wenn die Anzahl der Parameter in einer Funktion nicht übereinstimmt</exception>
    public string Parse(ref string query)
    {
        List<string> parameters = [];

#if NET48_OR_GREATER
        foreach (var func in _snippets)
#else
        foreach (var func in CollectionsMarshal.AsSpan(_snippets))
#endif
        {
            if (func.IsPlaceHolder && query.ContainsIgnoreCase(func.FuncName))
            {
                query = query.Replace(func.FuncName.Trim(), func.SnippetCode.Trim(),
                    StringComparison.OrdinalIgnoreCase);
                continue;
            }

            int idx = query.IndexOf(func.FuncName + @"(", StringComparison.OrdinalIgnoreCase);

            while (idx >= 0)
            {
                parameters.Clear();
                var start = idx;
                int last;
                int ptr = start + func.FuncName.Length;
                int depth = 0;
                string currPH = "";
                while (true)
                {
                    ++ptr;
                    if (query[ptr] == ')' && depth == 0) // ende erreicht
                    {
                        if (currPH.Length > 0)
                            parameters.Add(currPH);
                        last = ptr;
                        break;
                    }

                    if (query[ptr] == ',' && depth == 0)
                    {
                        parameters.Add(currPH);
                        currPH = "";
                        continue;
                    }

                    if (query[ptr] == '(')
                        ++depth;

                    if (query[ptr] == ')' && depth > 0)
                        --depth;

                    currPH = string.Concat(currPH, query[ptr]);
                }

                string replace = query.Substring(start, last - start + 1);

                if (parameters.Count > 0)
                {
                    string toReplace = func.SnippetCode;

                    for (int i = 1; i <= parameters.Count; i++)
                    {
                        toReplace = toReplace.Replace(string.Concat(@"<p", i, @">"), parameters[i - 1],
                            StringComparison.Ordinal);
                    }

                    query = query.Replace(replace, toReplace, StringComparison.Ordinal);
                }
                else
                    query = query.Replace(replace, func.SnippetCode, StringComparison.Ordinal);

                idx = query.IndexOf(func.FuncName + @"(", StringComparison.OrdinalIgnoreCase);
            }


        }

        return query;
    }
}

/// <summary>
/// Snippet für einen String-Parser
/// </summary>
public struct StringParserSnippet
{

    private Regex? _pattern = null;

    /// <summary>
    /// Constructor
    /// </summary>
    public StringParserSnippet(string funcName, int paramCount, string snippet, string? description = null)
    {
        FuncName = funcName;
        ParameterCount = paramCount;
        SnippetCode = snippet;
        SnippetDescription = description ?? "";
    }

    /// <summary>
    /// Vollst. Signatur
    /// </summary>
    public string FullName
    {
        get
        {
            if (ParameterCount == 0)
                return FuncName + "()";
            if (ParameterCount == 1)
                return FuncName + "(p1)";
            if (ParameterCount == 2)
                return FuncName + "(p1, p2)";
            if (ParameterCount == 3)
                return FuncName + "(p1, p2, p3)";
            if (ParameterCount == 4)
                return FuncName + "(p1, p2, p3, p4)";
            if (ParameterCount == 5)
                return FuncName + "(p1, p2, p3, p4, p5)";
            if (ParameterCount == 6)
                return FuncName + "(p1, p2, p3, p4, p5, p6)";

            return FuncName + "()";
        }
    }

    /// <summary>
    /// Muster für Regex-Abgleich
    /// </summary>
    public Regex Pattern
    {
        get
        {
            if (_pattern != null) return _pattern;

            string pattern =
                string.Format(@"{0}\s*\((?<params>[^()]*(\((?<depth>)[^()]*\)(?<-depth>[^()]*)+)*(?(depth)(?!)))\)",
                    FuncName);
            _pattern = new Regex(pattern, RegexOptions.IgnoreCase);

            return _pattern;
        }
    }

    /// <summary>
    /// Name der Funktion (wie 'AFUpper')
    ///
    /// Dieses Snippet wird verwendet, um alle Funktionsaufrufe wie 'AFUpper(...)' durch den angegebenen Snippet-Code zu ersetzen.
    ///
    /// Verwenden Sie einen FuncName, der mit '#' beginnt, für PlaceHolder-Snippets (wie '#CURRENTDATE#').
    /// </summary>
    public string FuncName { get; }

    /// <summary>
    /// Ist dies ein Platzhalter-Snippet?
    ///
    /// PlaceHolder-Snippets sind keine Funktionen mit Parametern, sondern Platzhalter für einen Snippet-Code.
    /// Der FuncName dieses Platzhalters beginnt immer mit '#'.
    /// </summary>
    public bool IsPlaceHolder => FuncName[0] == '#';

    /// <summary>
    /// Gesamtanzahl der Parameter (Anzahl der pX im Funktionsaufruf)
    /// </summary>
    public int ParameterCount { get; }

    /// <summary>
    /// Code, der den Funktionsaufruf ersetzt.
    /// Verwenden Sie pX als Parameter innerhalb des Funktionsaufrufs.
    /// <example>
    /// DATEFROMPARTS(&lt;p1&gt;, &lt;p2&gt;, &lt;p3&gt;)
    /// </example>
    /// </summary>
    public string SnippetCode { get; }

    /// <summary>
    /// Beschreibung des Snippets
    /// </summary>
    public string SnippetDescription { get; set; } = "";
}

