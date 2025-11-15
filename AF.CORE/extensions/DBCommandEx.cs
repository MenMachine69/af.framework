using System.Data.Common;
using System.Text;

namespace AF.DATA;

/// <summary>
/// Erweiterungen für DBCommand.
/// </summary>
public static class DBCommandEx
{
    /// <summary>
    /// Analysiert eine Abfrage mit Platzhaltern (?) für Parameter
    /// </summary>
    /// <param name="command">Befehl</param>
    /// <param name="query">Abfragezeichenfolge</param>
    /// <param name="args">Parameter für Abfrage</param>
    /// <returns>der geänderte Befehl</returns>
    public static DbCommand ParseParameters(this DbCommand command, string query, params object[]? args)
    {
        int cnt = 0;

        if (args == null || args.Length <= 0) return command;

        foreach (object o in args[0] is IEnumerable<object> ? (IEnumerable<object>)args[0] : args)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = @"p" + cnt;
            parameter.Value = o;
            ++cnt;
        }

#if NET48_OR_GREATER
        if (!query.Contains("?")) return command;
#else
        if (!query.Contains('?')) return command;
#endif

        cnt = 0;

        StringBuilder replacer = StringBuilderPool.GetStringBuilder();

        foreach (char c in query)
        {
            if (c == '?')
            {
                replacer.Append(@"@p");
                replacer.Append(cnt);
                ++cnt;
            }
            else
                replacer.Append(c);
        }

        command.CommandText = replacer.ToString();
        StringBuilderPool.ReturnStringBuilder(replacer);

        return command;
    }

}
