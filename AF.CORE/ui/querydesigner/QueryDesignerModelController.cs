using System.Text;

namespace AF.DATA;

/// <summary>
/// Controller für 'QueryDesignerModel'
/// </summary>
public class QueryDesignerModelController : Controller<QueryDesignerModel>
{
    #region Singleton
    private static QueryDesignerModelController? instance;

    /// <summary>
    /// Zugriff auf die Instanz des Controllers
    /// </summary>
    public static QueryDesignerModelController Instance => instance ??= new();

    /// <summary>
    /// Constructor
    /// </summary>
    public QueryDesignerModelController() { }
    #endregion

    /// <summary>
    /// Generiert den Quelltext aus einem Schema
    /// </summary>
    /// <param name="tables">Tabellen/Views</param>
    /// <param name="canvasJoins">Joins</param>
    /// <param name="errors">Liste, die Fehlermeldungen aufnimmt</param>
    /// <returns>Ergebnis der Ausführung. CommandResult.ResultObject enthält den Quelltext (string).</returns>
    public CommandResult GenerateCode(DatabaseSchemeTable[] tables, BindingList<IJoin> canvasJoins, ValidationErrorCollection errors)
    {
        StringBuilder sbFields = StringBuilderPool.GetStringBuilder();
        StringBuilder sbJoins = StringBuilderPool.GetStringBuilder();
        StringBuilder sbWhere = StringBuilderPool.GetStringBuilder();

        if (tables.Length < 1)
            return CommandResult.Warning("Kein Quelltext zu generieren.");

        // Starttabelle ermitteln
        var starttables = tables.Where(t => canvasJoins.All(j => j.ElementTarget != t.Id)).ToArray();

        if (!starttables.Any())
        {
            errors.Add("tables", "Ausgangstabelle/Ausgangsview nicht vorhanden. Alle Tabellen/Views sind Ziel eines Verweises. Es muss eine Tabelle/einen View geben, auf den NICHT verwiesen wird.");
            return CommandResult.Error("Fehler beim Generieren.");
        }

        if (starttables.Length > 1)
        {
            errors.Add("tables", "Es existieren meherer Tabellen/Views, auf die es keinen Verweis gibt. Es darf nur eine Tabelle/ein View existieren, auf den nicht verwiesen wird.");
            return CommandResult.Error("Fehler beim Generieren.");
        }

        generateCode(starttables[0], null, tables, canvasJoins, errors, sbFields, sbJoins, sbWhere);

        if (errors.Count > 0)
            return CommandResult.Error("Fehler beim Generieren.");

        var result = CommandResult.Success("Code generiert.");
        result.ResultObject = "SELECT \r\n" + sbFields + "\r\n" + sbJoins + (sbWhere.Length > 0 ? "\r\nWHERE\r\n\t" + sbWhere : "");

        StringBuilderPool.ReturnStringBuilder(sbFields);
        StringBuilderPool.ReturnStringBuilder(sbJoins);
        StringBuilderPool.ReturnStringBuilder(sbWhere);

        return result;
    }

    /// <summary>
    /// Code für eine Tabelle/einen View generieren.
    /// </summary>
    /// <param name="table"></param>
    /// <param name="currentJoin"></param>
    /// <param name="tables"></param>
    /// <param name="canvasJoins"></param>
    /// <param name="errors"></param>
    /// <param name="sbFields"></param>
    /// <param name="sbJoins"></param>
    /// <param name="sbWhere"></param>
    private void generateCode(DatabaseSchemeTable table, DatabaseSchemeJoin? currentJoin, DatabaseSchemeTable[] tables, BindingList<IJoin> canvasJoins, ValidationErrorCollection errors, StringBuilder sbFields, StringBuilder sbJoins, StringBuilder sbWhere)
    {
        foreach (var field in table.Fields)
        {
            string fullName = (table.TABLE_ALIAS.IsNotEmpty() ? table.TABLE_ALIAS + "." : "") + field.FIELD_NAME;

            if (field.IsSelected)
            {
                if (sbFields.Length > 1)
                    sbFields.Append(", \r\n");

                if (field.FIELD_EXPRESSION.IsNotEmpty())
                {
                    sbFields.AppendEach("\t" + field.FIELD_EXPRESSION.Replace("#FELD#", fullName), " AS ");
                    sbFields.Append(field.FIELD_ALIAS.IsNotEmpty() ? field.FIELD_ALIAS : field.FIELD_NAME);
                }
                else
                {
                    sbFields.AppendEach("\t", fullName);

                    if (field.FIELD_ALIAS.IsNotEmpty())
                        sbFields.AppendEach(" AS ", field.FIELD_ALIAS);
                }
            }

            if (field.FIELD_WHERE.IsEmpty())
                continue;

            if (sbWhere.Length > 1)
                sbWhere.Append(" AND \r\n");

            sbWhere.Append(field.FIELD_WHERE.Replace("#FELD#", fullName));
        }

        if (sbJoins.Length < 1)
            sbJoins.AppendEach("FROM\r\n\t", table.TABLE_SCHEME.IsEmpty() ? table.TABLE_NAME : $"{table.TABLE_SCHEME}.{table.TABLE_NAME}", table.TABLE_ALIAS.IsEmpty() ? "" : " " + table.TABLE_ALIAS); 

        foreach (var join in canvasJoins.Where(j => j.ElementSource == table.Id).OfType<DatabaseSchemeJoin>())
        {
            var tableTarget = tables.FirstOrDefault(t => t.Id == join.ElementTarget);
            var fieldSource = table.Fields.FirstOrDefault(f => f.Id == join.FromField);
            var fieldTarget = tableTarget?.Fields.FirstOrDefault(f => f.Id == join.ToField);

            if (fieldSource == null)
            {
                errors.Add("Ausgangsfeld", $"Ausgangsfeld des Joins von Tabelle {table.TABLE_NAME} konnte nicht ermittelt werden.");
                return;
            }

            if (tableTarget == null)
            {
                errors.Add("Zieltabelle", $"Ziel des Joins von Tabelle {table.TABLE_NAME} Feld {(fieldSource.FIELD_NAME)} konnte nicht ermittelt werden.");
                return;
            }

            if (fieldTarget == null)
            {
                errors.Add("Zielfeld", $"Ziel des Joins von Tabelle {table.TABLE_NAME} Feld {fieldSource.FIELD_NAME} zu Tabelle {tableTarget.TABLE_NAME} konnte nicht ermittelt werden.");
                return;
            }

            sbJoins.AppendEach(
                "\r\n\t",
                join.JoinString,
                tableTarget.TABLE_SCHEME.IsEmpty() ? tableTarget.TABLE_NAME : $"{tableTarget.TABLE_SCHEME}.{tableTarget.TABLE_NAME}",
                (tableTarget.TABLE_ALIAS.IsNotEmpty() ? " " + tableTarget.TABLE_ALIAS : ""),
                " ON ",
                (table.TABLE_ALIAS.IsNotEmpty() ? table.TABLE_ALIAS : table.TABLE_NAME), ".", fieldSource.FIELD_NAME,
                " = ",
                (tableTarget.TABLE_ALIAS.IsNotEmpty() ? tableTarget.TABLE_ALIAS : tableTarget.TABLE_NAME), ".", fieldTarget.FIELD_NAME);

            generateCode(tableTarget, join, tables, canvasJoins, errors, sbFields, sbJoins, sbWhere);
        }
    }
}