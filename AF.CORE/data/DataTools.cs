using System.Data.Common;
using System.Text;

namespace AF.DATA;

/// <summary>
/// Hilfsklasse für Datenbankoperationen
/// </summary>
public static class DataTools
{
    /// <summary>
    /// Eine SQL-Abfrage mit Parametern parsen, bei denen die Parameter in Form des '?'-Zeichens angegeben sind.
    /// </summary>
    /// <param name="command">Abfrage</param>
    /// <param name="args">Argumente für die Abfrage</param>
    /// <param name="translator">Translator (datenbankspezifisch)</param>
    /// <param name="query">auszuführende Abfrage</param>
    public static void ParseQuery<TParameter, TCommand>(TCommand command, ITranslator? translator, string query, params object[]? args)
        where TParameter : DbParameter, new()
        where TCommand : DbCommand, new()
    {
        ParseQuery<TParameter, TCommand>(command, translator, query, null, args);
    }


    /// <summary>
    /// Eine SQL-Abfrage mit Parametern parsen, bei denen die Parameter in Form des '?'-Zeichens angegeben sind.
    /// </summary>
    /// <param name="command">Abfrage</param>
    /// <param name="args">Argumente für die Abfrage</param>
    /// <param name="translator">Translator (datenbankspezifisch)</param>
    /// <param name="query">auszuführende Abfrage</param>
    /// <param name="variablen">Variablenwerte für die Abfrage</param>
    public static void ParseQuery<TParameter, TCommand>(TCommand command, ITranslator? translator, string query, IList<VariableUserValue>? variablen = null, params object[]? args)
        where TParameter : DbParameter, new()
        where TCommand : DbCommand, new()
    {
        int cnt = 0;

        args ??= [];
        List<string> toReplaceWhereGuidEmpty = [];

        foreach (object o in args.Length > 0 && args[0] is IEnumerable<object> ? (IEnumerable<object>)args[0] : args)
        {
            command.Parameters.Add(new TParameter
                { ParameterName = @"p" + cnt, Value = translator == null ? o : translator.ToDatabase(o) });

            if (o is Guid guid && guid == Guid.Empty)
                toReplaceWhereGuidEmpty.Add(@"p" + cnt); 

            ++cnt;
        }

        if (query.Contains('?'))
        {
            cnt = 0;

            StringBuilder creator = StringBuilderPool.GetStringBuilder(size: query.Length);

            for (int i = 0; i < query.Length; i++)
            {
                if (query[i] == '?')
                {
                    creator.Append(@"@p");
                    creator.Append(cnt);
                    ++cnt;
                }
                else
                    creator.Append(query[i]);
            }

            query = creator.ToString();
            StringBuilderPool.ReturnStringBuilder(creator);
        }

        StringBuilder? replacer = null;
        bool hasReplacements = false;

        if (AFCore.App.QueryService?.Placeholders != null && AFCore.App.QueryService?.Placeholders.Count > 0)
        {
            replacer ??= StringBuilderPool.GetStringBuilder(content: query);

            foreach (var ph in AFCore.App.QueryService!.Placeholders)
            {
                if (!query.Contains(ph.Key)) continue;

                replacer.Replace(ph.Key, ph.Value.GetValue());
                hasReplacements = true;
            }
        }

        if (variablen is { Count: > 0 })
        {
            replacer ??= StringBuilderPool.GetStringBuilder(content: query);
            cnt = command.Parameters.Count;

            foreach (var variable in variablen.Where(v => v.Value != null))
            {
                string varName = "{" + variable.Name + "}";
                string pName = @"@p" + cnt;
                string p = @"p" + cnt;

                if (!query.Contains(varName)) continue;
                
                command.Parameters.Add(new TParameter
                    { ParameterName = pName, Value = translator == null ? variable.Value : translator.ToDatabase(variable.Value!) });

                if (variable.Value is Guid guid && guid == Guid.Empty)
                    toReplaceWhereGuidEmpty.Add(p);

                replacer.Replace(varName, pName);
                hasReplacements = true;

                ++cnt;
            }
        }


        if (toReplaceWhereGuidEmpty.Count > 0)
        {
            replacer ??= StringBuilderPool.GetStringBuilder(content: query);
            hasReplacements = true;

            foreach (var itm in toReplaceWhereGuidEmpty)
            {
                string search = "= @" + itm;
                replacer.Replace(search, "IS NULL");

                search = "<> @" + itm;
                replacer.Replace(search, "IS NOT NULL");

                search = "=@" + itm;
                replacer.Replace(search, "IS NULL");

                search = "<>@" + itm;
                replacer.Replace(search, "IS NOT NULL");
            }
        }

        if (hasReplacements && replacer != null)
        {
            query = replacer!.ToString();
            StringBuilderPool.ReturnStringBuilder(replacer);
        }

        command.CommandText = translator == null ? query : translator.TranslateQuery(ref query);
    }
}

