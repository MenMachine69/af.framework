using System.Data;

namespace AF.DATA;

/// <summary>
/// Erweiterungsmethoden für NpgSql* Klassen
/// </summary>
public static class NpgSqlEx
{
    /// <summary>
    /// Verbindung zu einer PostgreSQL-Datenbank herstellen
    /// </summary>
    /// <param name="database">Datenbankdefinition</param>
    /// <returns>FbConnection für die Datenbank</returns>
    public static NpgsqlConnection Connect(IDatabaseConnection database)
    {
        NpgsqlConnectionStringBuilder builder = new(database.ConnectString);
        if (database.Credentials != null && database.Credentials.Username.IsNotEmpty())
        {
            builder.Username = database.Credentials.Username;
            builder.Password = database.Credentials.Password;
        }

        NpgsqlConnection conn = new(builder.ConnectionString);

        return conn;
    }

    /// <summary>
    /// Liefert das Schema der Datenbank
    /// </summary>
    /// <returns>Schema der Datenbank</returns>
    public static DatabaseScheme GetScheme(this NpgsqlConnection conn)
    {
        DatabaseScheme schema = new();

        lock (schema.Tables)
        {
            conn.Open();
            DataTable schemaTables = conn.GetSchema("Tables");
            loadTables(conn, schema, schemaTables, false);
            schemaTables = conn.GetSchema("Views");
            loadTables(conn, schema, schemaTables, true);
        }

        return schema;
    }

    private static void loadTables(NpgsqlConnection conn, DatabaseScheme schema, DataTable schemaTables, bool view)
    {
        foreach (DataRow row in schemaTables.Rows)
        {
            TypeDescription? tdesc = TypeDescription.GetTypeDescriptionByTableName(row["TABLE_NAME"]!.ToString()!);

            DatabaseSchemeTable table = new(row["TABLE_NAME"]!.ToString()!, isview: view, scheme: row["TABLE_SCHEMA"]?.ToString() ?? "", description: tdesc?.Description ?? "");
            schema.Tables.Add(table);

            DataTable columns = conn.GetSchema("Columns", [null, null, table.TABLE_NAME, null]);
            columns.DefaultView.Sort = "column_name ASC";
            
            foreach (DataRow column in columns.DefaultView.ToTable().Rows)
            {
                string name = column["column_name"]?.ToString() ?? throw new Exception("Tabelle enthält keine Spalte 'column_name'");

                var fdesc = tdesc?.Properties.FirstOrDefault(p => p.Key.ToUpper() == name.ToUpper()).Value;

                table.Fields.Add(new()
                {
                    FIELD_NAME = column["column_name"]?.ToString() ?? "<unknown>",
                    FIELD_LENGTH = column["data_type"].ToString() == "text"
                        ? fdesc?.Field?.MaxLength ?? (column["character_maximum_length"] is DBNull ? 0 : Convert.ToInt32(column["character_maximum_length"]))
                        : (column["character_maximum_length"] is DBNull ? 0 : Convert.ToInt32(column["character_maximum_length"])),
                    FIELD_TYPE = column["data_type"]?.ToString() ?? "<unknown>",
                    FIELD_SYSTEMTYPE = PostgreTranslator.Instance.GetColumnType(column["data_type"] as string ?? ""),
                    FIELD_DESCRIPTION = fdesc?.Context?.Description ?? ""
                });
            }
        }
    }
}
