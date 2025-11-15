namespace AF.CORE;

/// <summary>
/// Einfacher Parser für die Kommandozeile
/// </summary>
public static class CommandLineParser
{
    /// <summary>
    /// Kommandozeilenargumente in ein Dictionary übertragen.
    /// 
    /// Bsp.: meinprog -f "mein langer Dateiname" -p 0 -c -r -k
    /// 
    /// Der Name eines Parameters beginnt mit - oder /. Er wird immer in Kleinbuchstaben übersetzt.
    /// Der Wert hinter dem Namen des Parameters ist optional. Werte die ein Leerzeichen enthalten oder 
    /// mit - oder / beginnen müssen immer in Anführungszeichen gesetzt sein.
    /// </summary>
    /// <param name="args">Kommandolzeilenparameter als string[]</param>
    /// <returns>Dictionary der Parameter (name, wert) wobei wert optional ist und immer vom Typ string (leerer String, wenn kein wert angegeben ist)</returns>
    public static Dictionary<string, string> Parse(string[] args)
    {
        Dictionary<string, string> ret = new();

        string name = "";
        char[] markers = ['-', '/'];

        if (args.Length <= 0) return ret;


        for (int pos = 0; pos < args.Length; ++pos)
        {
            if (markers.Contains(args[pos][0])) // neuer Parameter
            {
                name = args[pos][1..].ToLowerInvariant();

                if (name.Length > 0)
                {
                    while (@"abcdefghijklmnopqrstuvwxyz".Contains(name[0]) == false)
                        name = name[1..];
                }

                if (name.Length < 1)
                    throw new ArgumentException(string.Format(CoreStrings.ERR_PARSER_WRONGNAME, args[pos]));

                ret.Add(name, "");
            }
            else
            {
                if (name.IsEmpty())
                    throw new ArgumentException(string.Format(CoreStrings.ERR_PARSER_NONAME, args[pos]));

                string wert = args[pos].Trim();

                if (wert.StartsWith(@""""))
                    wert = wert[1..];

                if (wert.EndsWith(@""""))
                    wert = wert[..^1];

                ret[name] = wert;
            }
        }

        return ret;
    }

}
