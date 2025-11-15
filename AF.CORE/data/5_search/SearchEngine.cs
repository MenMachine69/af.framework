using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace AF.DATA;

/// <summary>
/// Owner der SearchEngine
/// </summary>
public interface ISearchEngineConsumer
{
    /// <summary>
    /// Wird von der SearchEngine aufgerufen, bevor ein Typ durchsucht wird.
    ///
    /// Wird true zurückgegeben, wird die Suche ausgeführt, bei false abgebrochen.
    /// </summary>
    /// <param name="target">zu durchsuchender Typ</param>
    /// <returns>true, wenn erlaubt, sonst false</returns>
    bool SearchIn(Type target);

    /// <summary>
    /// Suche wurde gestartet
    /// </summary>
    /// <param name="engine">Engine, die die Suche gestartet hat</param>
    void BeginSearch(SearchEngine engine);

    /// <summary>
    /// Suchefortschritt (Suche in Typ wurde gestartet)
    /// </summary>
    /// <param name="engine">Engine, die die Suche gestartet hat</param>
    /// <param name="tdesc">Typ der durchsucht wird</param>
    void SearchProgress(SearchEngine engine, TypeDescription tdesc);

    /// <summary>
    /// Suche wurde beendet
    /// </summary>
    /// <param name="engine">Engine, die die Suche beendet hat</param>
    void EndSearch(SearchEngine engine);

    /// <summary>
    /// Suche soll abgebrochen werden
    /// </summary>
    bool CancelSearch { get; }
}

/// <summary>
/// Engine für die Suche in Datenbanken mit Freitextsuche (wie Internet-Suchmaschinen).
/// </summary>
public sealed class SearchEngine
{
    private readonly List<Type> _searchInTypes;
    private readonly ISearchEngineConsumer _consumer;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="consumer">Element, das die SearchEngine verwendet.</param>
    public SearchEngine(ISearchEngineConsumer consumer) 
    { 
        _searchInTypes = [];
        _consumer = consumer;
    }

    

    /// <summary>
    /// Registrieren einer Tabelle als Suchziel
    /// </summary>
    /// <typeparam name="TTable">Tabellentyp</typeparam>
    public bool RegisterTable<TTable>() where TTable : class, ITable
    {
        if (_searchInTypes.Contains(typeof(TTable))) return true;

        if (!typeof(TTable).GetController().AllowSearch) return false;
            
        _searchInTypes.Add(typeof(TTable));

        return true;
    }

    /// <summary>
    /// Suche auf Treffer beschränken.
    /// </summary>
    public int HitLimit { get; set; } = 15;

    /// <summary>
    /// Auf true setzen, um die aktuelle Suche abzubrechen
    /// </summary>
    public bool CancelSearch { get; set; }

    /// <summary>
    /// Suche nach einer Phrase in allen registrierten Tabellen und Ansichten
    /// 
    /// Treffer sind auf das aktuelle HitLimit begrenzt.
    /// </summary>
    /// <param name="searchFor">Query/Bedingungen für die Suche</param>
    /// <param name="inType">nur im angegeben Typen suchen</param>
    /// <param name="all">alle Treffer ausgeben</param>
    /// <returns>true, wenn mehr Treffer als das Limit vorhanden sind</returns>
    public void Search(string searchFor, Type? inType = null, bool all = false)
    {
        if (OnFound == null) throw new NotSupportedException(@"Searching without a OnFound delegate is not supported.");

        var tokens = GetTokens(ref searchFor);

        CancelSearch = false;

        _consumer.BeginSearch(this);

        foreach (var target in _searchInTypes)
        {
            CancelSearch = _consumer.CancelSearch;

            if (CancelSearch)
            {
                _consumer.EndSearch(this);
                return;
            }

            if (inType != null && inType != target) continue; // Typ sagt NEIN

            if (_consumer.SearchIn(target) == false) continue; // Consumer sagt NEIN...

            if (!target.GetController().AllowSearch) continue; // Controller sagte NEIN...

            TypeDescription tdesc = target.GetController().GetSearchType.GetTypeDescription();

            _consumer.SearchProgress(this, tdesc);

            StringBuilder sbQuery = new();
            List<object> paraQuery = [];

            foreach (var token in tokens)
            {
                if (sbQuery.Length > 0)
                    sbQuery.Append(") and (");
                else
                    sbQuery.Append("(");

                bool firstProp = true;

                foreach (var prop in tdesc.Fields.Values.Where(f => f.Field!.Searchable))
                {
                    if (token.FieldName.IsNotEmpty() && !prop.Name.Equals(token.FieldName, StringComparison.OrdinalIgnoreCase)) continue;

                    var proptyp = ((PropertyInfo)prop).PropertyType;

                    if (proptyp == typeof(string) && token.TextValue.StartsWith("*") && prop.Field!.UseSoundExSearch == false)
                        continue;

                    if (proptyp == typeof(decimal) && token.DecimalValue == null) continue;

                    if (proptyp == typeof(double) && token.DecimalValue == null) continue;

                    if (proptyp == typeof(int) && token.IntegerValue == null) continue;

                    if (proptyp == typeof(DateTime) && token.DateTimeValue == null) continue;

                    if (proptyp == typeof(bool) && token.BoolValue == null) continue;

                    if (sbQuery.Length > 0 && !firstProp)
                        sbQuery.Append(" or ");

                    firstProp = false;

                    if (proptyp == typeof(string))
                    {
                        if (token.TextValue.StartsWith("*") && prop.Field!.UseSoundExSearch)
                        {
                            sbQuery.AppendLine($"{prop.Name} = ?");
                            paraQuery.Add(token.TextValue[1..].PhoneticEncode());
                            continue;
                        }

                        if (token.TextValue.Length == prop.Field!.MaxLength)
                        {
                            sbQuery.AppendLine($"Upper({prop.Name}) = ?");
                            paraQuery.Add(token.TextValue.ToUpper());
                        }
                        else
                        {
                            sbQuery.AppendLine($"Upper({prop.Name}) like ?");
                            paraQuery.Add($"%{token.TextValue.ToUpper()}%");
                        }
                    }
                    else if (proptyp == typeof(decimal) && token.DecimalValue != null)
                    {
                        sbQuery.AppendLine($"{prop.Name} {token.Operator ?? "="} ?");
                        paraQuery.Add(token.DecimalValue!);
                    }
                    else if (proptyp == typeof(double) && token.DecimalValue != null)
                    {
                        sbQuery.AppendLine($"{prop.Name} {token.Operator ?? "="} ?");
                        paraQuery.Add(token.DecimalValue!);
                    }
                    else if (proptyp == typeof(int) && token.IntegerValue != null)
                    {
                        sbQuery.AppendLine($"{prop.Name} {token.Operator ?? "="} ?");
                        paraQuery.Add(token.DecimalValue!);
                    }
                    else if (proptyp == typeof(DateTime) && token.DateTimeValue != null)
                    {
                        sbQuery.AppendLine($"{prop.Name} {token.Operator ?? "="} ?");
                        paraQuery.Add(token.DateTimeValue!);
                    }
                    else if (proptyp == typeof(bool) && token.BoolValue != null)
                    {
                        sbQuery.AppendLine($"{prop.Name} {token.Operator ?? "="} ?");
                        paraQuery.Add(token.DateTimeValue!);
                    }
                }

            }

            sbQuery.Append(")");

            SearchHitSection section = new() { TypeDesc = tdesc, Caption = tdesc.Context?.NamePlural ?? tdesc.Name, QueryParameter = paraQuery.ToArray(), Query = sbQuery.ToString() };

            var ergebnis = target.GetController().Search(all, section.Query, section.QueryParameter);
            section.HitCount = ergebnis.Count;

            if (all == false && ergebnis.Count > 15)
            {
                section.HasMore = true;
                section.HitCount = 15;
            }

            OnFound?.Invoke(section, ergebnis);
        }

        _consumer.EndSearch(this);
    }

    private List<SearchEngineCriteria> GetTokens(ref string searchFor) 
    {
        // Verwenden Sie reguläre Ausdrücke, um in Anführungszeichen eingeschlossene Wörter und Ausdrücke zu finden
        var matches = Regex.Matches(searchFor, @"[^\s""']+|""([^""]*)""|'([^']*)'");

        // Übereinstimmende Werte extrahieren und Anführungszeichen aus den Phrasen entfernen
        var tokens = matches
            .Cast<Match>()
            .Select(match => match.Value.Trim('"').Trim())
            .ToArray();

        List<SearchEngineCriteria> result = [];
        var currOperator = "";

        var operators = new[]
        {
            @">", @"<", @">=", @"<=", @"<>", @"="
        };

        foreach (var token in tokens)
        {
            if (token.IsEmpty())
                continue;

            if (operators.Contains(token))
            {
                currOperator = token;
                continue;
            }

            string fieldname = "";
            string textvalue = token;
            decimal? decvalue = null;
            int? intvalue = null;
            DateTime? dtvalue = null;
            bool? boolvalue = null;


            if (token.Contains(':'))
            {
                fieldname = token[..token.IndexOf(':')];
                textvalue = token[token.IndexOf(':')..];
            }

            if (@"-0123446789".Contains(textvalue[0])) // möglich numerisch oder datetime
            {
                if (decimal.TryParse(textvalue, NumberStyles.Any, CultureInfo.CurrentCulture, out var valdec))
                    decvalue = valdec;

                if (int.TryParse(textvalue, NumberStyles.Any, CultureInfo.CurrentCulture, out var valint))
                    intvalue = valint;

                if (DateTime.TryParse(textvalue, CultureInfo.CurrentCulture, DateTimeStyles.None, out var valdt))
                    dtvalue = valdt;

            }

            if (@"JAYESTRUE".Contains(textvalue.ToUpper())) // möglich boolescher Wert true
                boolvalue = true;

            if (@"NEINNOFALSE".Contains(textvalue.ToUpper())) // eventuell boolescher Wert false
                boolvalue = false;

            result.Add(new SearchEngineCriteria(
                textvalue, 
                fieldname, 
                currOperator.IsEmpty() ? @"like" : currOperator, 
                decvalue, 
                intvalue, 
                dtvalue, 
                boolvalue));

            currOperator = "";
        }



        return result;
    }

    /// <summary>
    /// Delegate, der nach der Suche nach einem Ziel aufgerufen wird.
    /// </summary>
    public Action<SearchHitSection, IBindingList>? OnFound { get; set; }

    internal record SearchEngineCriteria(
        string TextValue,
        string FieldName,
        string Operator,
        decimal? DecimalValue,
        int? IntegerValue,
        DateTime? DateTimeValue,
        bool? BoolValue
    );

}

/// <summary>
/// Eintrag für die Anzeige der Suchtreffer
/// </summary>
public abstract class SearchHit
{
    /// <summary>
    /// Bezeichnung/Überschrift
    /// </summary>
    public string Caption { get; set; } = string.Empty;
}

/// <summary>
/// Eintrag für einen Abschnitt.
/// </summary>
public sealed class SearchHitSection : SearchHit
{
    /// <summary>
    /// Beschreibung des Typs, dessen Treffer im Abschnitt dargestellt werden.
    /// </summary>
    public TypeDescription TypeDesc { get; set; } = null!;

    /// <summary>
    /// Anzahl der Treffer im Abschnitt
    /// </summary>
    public int HitCount { get; set; }

    /// <summary>
    /// true, wenn mehr Trffer als die angezeigten vorhanden sind
    /// </summary>
    public bool HasMore { get; set; }

    /// <summary>
    /// Query, auf der die angezeigten Treffer beruhen
    /// </summary>
    public string Query { get; init; } = string.Empty;

    /// <summary>
    /// Parameter für die Query, auf der die angezeigten Treffer beruhen
    /// </summary>
    public object[] QueryParameter { get; init; } = [];

    /// <summary>
    /// Gibt an, ob die Suche für alle Datensätze ausgeführt wurde (false = nur die ersten 15)
    /// </summary>
    public bool IsAll { get; init; } = false; 
}

/// <summary>
/// Eintrag für einen Treffer.
/// </summary>
public class SearchHitHit : SearchHit
{
    /// <summary>
    /// Model, auf das sich der Treffer bezieht
    /// </summary>
    public IModel Model { get; set; } = null!;

    /// <summary>
    /// Gruppe/durchsuchter Typ
    /// </summary>
    public string Group { get; set; } = string.Empty;

    /// <summary>
    /// Abschnitt zu dem der Treffer gehört (zur Indetifikation der zu verwendenden Schabloen zur Anzeige)
    /// </summary>
    public string SectionName { get; set; } = string.Empty;

    /// <summary>
    /// Kennzeichnet den Eintrag als 'Alle Anzeigen' Schaltfläche.
    /// </summary>
    public bool ShowAll { get; set; }

    /// <summary>
    /// Section/druchsuchter Typ und Query
    /// </summary>
    public SearchHitSection? Section { get; set; }
}



