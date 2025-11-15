using System.Data;

namespace AF.DATA;

/// <summary>
/// Erweiterungsmethoden für Fb* Klassen
/// </summary>
public static class FbEx
{
    /// <summary>
    /// Verbindung zu einer Firebird-Datenbank herstellen
    /// </summary>
    /// <param name="database">Datenbankdefinition</param>
    /// <returns>FbConnection für die Datenbank</returns>
    public static FbConnection Connect(IDatabaseConnection database)
    {
        FbConnectionStringBuilder builder = new(database.ConnectString);
        if (database.Credentials != null && database.Credentials.Username.IsNotEmpty())
        {
            builder.UserID = database.Credentials.Username;
            builder.Password = database.Credentials.Password;
        }

        FbConnection conn = new(builder.ConnectionString);

        return conn;
    }

    /// <summary>
    /// Liefert das Schema der Datenbank
    /// </summary>
    /// <returns>Schema der Datenbank</returns>
    public static DatabaseScheme GetScheme(this FbConnection conn)
    {
        DatabaseScheme schema = new();

        lock (schema.Tables)
        {
            conn.Open();
            DataTable schemaTables = conn.GetSchema("Tables", [ null, null, null, "TABLE" ]);
            loadTables(conn, schema, schemaTables, false);
            schemaTables = conn.GetSchema("Views");
            loadTables(conn, schema, schemaTables, true);
        }

        return schema;
    }

    private static void loadTables(FbConnection conn, DatabaseScheme schema, DataTable schemaTables, bool view)
    {
        foreach (DataRow row in schemaTables.Rows)
        {
            TypeDescription? tdesc = TypeDescription.GetTypeDescriptionByTableName(row["TABLE_NAME"]!.ToString()!);

            DatabaseSchemeTable table = new(row["TABLE_NAME"]!.ToString()!, isview: view, scheme: row["TABLE_SCHEMA"]?.ToString() ?? "", description: tdesc?.Description ?? "");
            // DatabaseSchemeTable table = new(view ? row["VIEW_NAME"].ToString() : row["TABLE_NAME"].ToString(), isview: view);
            schema.Tables.Add(table);

            DataTable columns = conn.GetSchema("Columns", [ null, null, table.TABLE_NAME, null ]); 
            columns.DefaultView.Sort = "COLUMN_NAME ASC";

            foreach (DataRow column in columns.DefaultView.ToTable().Rows)
            {
                table.Fields.Add(new()
                {
                    FIELD_NAME = column["COLUMN_NAME"].ToString()!,
                    FIELD_LENGTH = column["COLUMN_SIZE"] is DBNull ? 0 : Convert.ToInt32(column["COLUMN_SIZE"]),
                    FIELD_TYPE = column["COLUMN_DATA_TYPE"].ToString()!
                });
            }
        }
    }
}
