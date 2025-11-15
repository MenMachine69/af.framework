using System.Collections;
using NpgsqlTypes;
using System.Net;
using System.Net.NetworkInformation;
using System.Data.Common;

namespace AF.DATA;

/// <summary>
/// Übersetzer für PostgreSQL-Datenbank
/// </summary>
[Localizable(false)]
public class PostgreTranslator : Translator, ITranslator
{
    private readonly Dictionary<eCommandString, string> _commands = new();
    private static PostgreTranslator? instance;
    private static Dictionary<string, Type> typeTranslation = new() { 
        { "boolean", typeof(bool) },
        { "smallint", typeof(short) },
        { "integer", typeof(int) },
        { "bigint", typeof(long) },
        { "real", typeof(float) },
        { "double precision", typeof(double) },
        { "numeric", typeof(decimal) },
        { "money", typeof(decimal) },
        { "text", typeof(string) },
        { "character varying", typeof(string) },
        { "character", typeof(char) },
        { "citext", typeof(string) },
        { "json", typeof(string) },
        { "jsonb", typeof(string) },
        { "xml", typeof(string) },
        { "uuid", typeof(Guid) },
        { "bytea", typeof(byte[]) },
        { "timestamp with time zone", typeof(DateTime) },
        { "timestamp without time zone", typeof(DateTime ) },
        { "time without time zone", typeof(TimeOnly) },
        { "time with time zone", typeof(TimeOnly) },
        { "date", typeof(DateOnly) },
        { "interval", typeof(TimeSpan) },
        { "cdir", typeof(ValueTuple<IPAddress, int>) },
        { "inet", typeof(IPAddress) },
        { "macaddr", typeof(PhysicalAddress) },
        { "tsquery", typeof(NpgsqlTsQuery) },
        { "tsvector", typeof(NpgsqlTsVector) },
        { "bit", typeof(bool) },
        { "bit varying", typeof(BitArray) },
        { "point", typeof(NpgsqlPoint) },
        { "lseg", typeof(NpgsqlLSeg) },
        { "path", typeof(NpgsqlPath) },
        { "polygon", typeof(NpgsqlPolygon) },
        { "line", typeof(NpgsqlLine) },
        { "circle", typeof(NpgsqlCircle) },
        { "box", typeof(NpgsqlBox) },
        { "hstore", typeof(IDictionary<string, string>) },
        { "oid", typeof(uint) },
        { "xid", typeof(uint) },
        { "cid", typeof(uint) },
        { "oidvector", typeof(uint[]) },
    };
    

    /// <summary>
    /// Zugriff auf ds SIngleton des Translators für datbankunabhängige Dienste
    /// </summary>
    public static PostgreTranslator Instance => instance ??= new();

    /// <summary>
    /// privater Konstrukltor für Singleton Konstruktor
    /// </summary>
    private PostgreTranslator() { Database = "<none>"; }

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="db">Datenbank, die diesen Übersetzer verwendet</param>
    public PostgreTranslator(PostgreDatabase db)
    {
        Database = db.Configuration.DatabaseName;
        UseVarchar = ((PostgreConfiguration)db.Configuration).UseVarchar;
    }

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="dbname">Name der Datenbank</param>
    /// <param name="usevarchar">VARCHAR für Strings verwenden (true = default)</param>
    public PostgreTranslator(string dbname, bool usevarchar = true)
    {
        Database = dbname;
        UseVarchar = usevarchar;
    }

    /// <summary>
    /// Datenbank, die diesen Übersetzer verwendet
    /// </summary>
    internal string Database { get; init; }

    /// <summary>
    /// VARCHAR für Strings verwenden (true = default)
    /// </summary>
    internal bool UseVarchar { get; init; }

    private void _initCommands()
    {
        _commands.Add(eCommandString.CreateIndex, "CREATE #UNIQUE# #DESC# INDEX #NAME# ON #TABLENAME#(#FIELDS#);");
        _commands.Add(eCommandString.CreateProcedure, "CREATE PROCEDURE #NAME# AS #CODE#");
        _commands.Add(eCommandString.CreateTable,
            "CREATE TABLE #TABLENAME#  ( " +
            "#FIELDNAMEKEY# UUID NOT NULL, " +
            "#FIELDNAMECREATED# TIMESTAMP WITHOUT TIME ZONE, " +
            "#FIELDNAMECHANGED# TIMESTAMP WITHOUT TIME ZONE, " +
            "PRIMARY KEY(#FIELDNAMEKEY#))");
        _commands.Add(eCommandString.CreateKeyField, string.Empty);
        _commands.Add(eCommandString.CreateTrigger, "CREATE TRIGGER dbo.#NAME# ON dbo.#TABLENAME# FOR #EVENT# AS #CODE#");
        _commands.Add(eCommandString.ExistProcedure,
            "SELECT Count(proname) FROM pg_proc WHERE proname = '#NAME#'");
        _commands.Add(eCommandString.ExistTable,
            "SELECT Count(tablename) FROM pg_tables WHERE tablename = '#NAME#'");
        _commands.Add(eCommandString.ExistTrigger,
            "SELECT Count(tgname) FROM pg_trigger WHERE tgname = '#NAME#'");
        _commands.Add(eCommandString.ExistView,
            "SELECT Count(viewname) FROM pg_views WHERE viewname = '#NAME#'");
        _commands.Add(eCommandString.ExistIndex,
            "SELECT Count(indexname) FROM pg_indexes WHERE indexname = '#NAME#' and tablename = '#TABLENAME#'");
        _commands.Add(eCommandString.BeforeAlterSchema,
            "SELECT pg_terminate_backend(pg_stat_activity.pid) FROM pg_stat_activity WHERE pg_stat_activity.datname = '#DBNAME#' AND pid <> pg_backend_pid();");
        _commands.Add(eCommandString.AfterAlterSchema, "");
        _commands.Add(eCommandString.TriggerBeforeInsert,
            "CREATE OR REPLACE FUNCTION beforeinsert() " +
            "RETURNS TRIGGER AS $$  " +
            "BEGIN  " +
            "NEW.#FIELDNAMECREATED# = now();  " +
            "NEW.#FIELDNAMECHANGED# = now();  " +
            "RETURN NEW;  " +
            "END;  " +
            "$$ language 'plpgsql';  " +
            "CREATE TRIGGER #TABLENAME#_bi BEFORE INSERT  " +
            "ON #TABLENAME# FOR EACH ROW EXECUTE FUNCTION  " +
            "beforeinsert();");
        _commands.Add(eCommandString.TriggerBeforeUpdate,
            "CREATE OR REPLACE FUNCTION beforeupdate()  " +
            "RETURNS TRIGGER AS $$  " +
            "BEGIN  " +
            "NEW.#FIELDNAMECHANGED# = now();  " +
            "RETURN NEW;  " +
            "END;  " +
            "$$ language 'plpgsql';  " +
            "CREATE TRIGGER #TABLENAME#_bu BEFORE UPDATE  " +
            "ON #TABLENAME# FOR EACH ROW EXECUTE FUNCTION  " +
            "beforeupdate();");
        _commands.Add(eCommandString.GetServerTime, "SELECT CURRENT_TIMESTAMP AS ZEIT");
        _commands.Add(eCommandString.FieldDefBinary, "BYTEA");
        _commands.Add(eCommandString.FieldDefBool, "BOOLEAN");
        _commands.Add(eCommandString.FieldDefByte, "SMALLINT");
        _commands.Add(eCommandString.FieldDefDateTime, "TIMESTAMP WITHOUT TIME ZONE");
        _commands.Add(eCommandString.FieldDefDate, "DATE");
        _commands.Add(eCommandString.FieldDefTime, "TIME WITHOUT TIME ZONE");
        _commands.Add(eCommandString.FieldDefDecimal, "DECIMAL(18, 4)");
        _commands.Add(eCommandString.FieldDefDouble, "DOUBLE PRECISION");
        _commands.Add(eCommandString.FieldDefFloat, "FLOAT");
        _commands.Add(eCommandString.FieldDefGuid, "UUID");
        _commands.Add(eCommandString.FieldDefImage, "BYTEA");
        _commands.Add(eCommandString.FieldDefInt, "SMALLINT");
        _commands.Add(eCommandString.FieldDefInt16, "SMALLINT");
        _commands.Add(eCommandString.FieldDefInt32, "INTEGER");
        _commands.Add(eCommandString.FieldDefInt64, "BIGINT");
        _commands.Add(eCommandString.FieldDefLong, "BIGINT");
        _commands.Add(eCommandString.FieldDefObject, "BYTEA");
        _commands.Add(eCommandString.FieldDefShort, "SMALLINT");
        _commands.Add(eCommandString.FieldDefColor, "INTEGER");
        if (UseVarchar)
        {
            _commands.Add(eCommandString.FieldDefType, "VARCHAR(250)");
            _commands.Add(eCommandString.FieldDefString, "VARCHAR(#SIZE#)");
            _commands.Add(eCommandString.AlterFieldLength,
                "ALTER TABLE #TABLENAME# ALTER COLUMN #FIELDNAME# TYPE VARCHAR(#SIZE#)");
        }
        else
        {
            _commands.Add(eCommandString.FieldDefString, "TEXT");
            _commands.Add(eCommandString.FieldDefType, "TEXT");
            _commands.Add(eCommandString.AlterFieldLength, ""); // not needed...
        }
        _commands.Add(eCommandString.FieldDefMemo, "TEXT");
        _commands.Add(eCommandString.DropField, "ALTER TABLE #TABLENAME# DROP #FIELDNAME#");
        _commands.Add(eCommandString.SelectTop, "SELECT #FIELDNAMES# FROM #TABLENAME#"); // same as select because LIMIT will be applied in BaseConnection.
        _commands.Add(eCommandString.ExistConstraint,
            "SELECT Count(con.conname) FROM pg_constraint con INNER JOIN pg_catalog.pg_class rel ON rel.oid = con.conrelid WHERE con.conname = '#NAME#' and rel.relname = '#TABLENAME#'");
        _commands.Add(eCommandString.DropConstraint, "ALTER TABLE #TABLENAME# DROP CONSTRAINT #NAME#");
        _commands.Add(eCommandString.CreateConstraint,
            "ALTER TABLE #TABLENAME# ADD FOREIGN KEY(#FIELDNAME#) REFERENCES #TARGETTABLE#(#TARGETFIELD#) #CONSTRAINT#");
        _commands.Add(eCommandString.GrantRightsToUser,
            "GRANT SELECT, UPDATE, INSERT, DELETE ON #TABLENAME# TO #USERNAME#");
        _commands.Add(eCommandString.SetComment, "COMMENT ON COLUMN #TABLENAME#.#TARGETFIELD# IS '#COMMENT#'");
        _commands.Add(eCommandString.GetComment,
            "SELECT col_description(c.oid, a.attnum) AS FieldComment " +
            "FROM pg_class c " +
            "JOIN pg_namespace n ON n.oid = c.relnamespace " +
            "JOIN pg_attribute a ON a.attrelid = c.oid " +
            "WHERE c.relname = '#TABLENAME#' AND " +
            "      a.attname = '#NAME#'");
    }

    /// <summary>
    /// Gibt eine datenbankspezifische Zeichenfolge für den spezifischen Befehl zurück.  
    /// Gibt die global gültige Zeichenfolge für diesen Befehl zurück, wenn kein spezifischer Befehl verfügbar ist.
    /// </summary>
    /// <param name="command">Benötigte Befehlszeichenfolge</param>
    /// <returns>Befehlszeichenfolge</returns>
    public override string GetCommandString(eCommandString command)
    {
        if (_commands.Count == 0)
            _initCommands();

        if (_commands.TryGetValue(command, out var cmd))
        {
            if (cmd.Contains("#DBNAME#"))
                return cmd.Replace("#DBNAME#", Database);
            else
                return cmd;
        }

        string ret = base.GetCommandString(command);
        
        if (command == eCommandString.SelectTop)
            ret += " LIMIT #COUNT#";

        return ret;
    }

    /// <summary>
    /// Übersetzt eine Abfrage mit universellen Platzhaltern in das von Firebird benötigte Format.
    /// </summary>
    /// <param name="query">zu übersetzende Abfrage</param>.
    /// <returns> übersetzte Abfrage</returns>
    public string TranslateQuery(ref string query)
    {
        if (CustomFunctions.Count >= 1) return translate(ref query);

        CustomFunctions.Add(new("AFMax", 1, "MAX(CAST(<p1> AS VARCHAR))", "Maximalwert für Spalte/Ausdruck ermitteln. (PostgreSQL: MAX())"));
        CustomFunctions.Add(new("AFMin", 1, "MIN(CAST(<p1> AS VARCHAR))", "Minimalwert für Spalte/Ausdruck ermitteln. (PostgreSQL: MIN())"));
        CustomFunctions.Add(new("AFUpper", 1, "upper(CAST(<p1> AS VARCHAR))", "Text in Grossbuchstaben umwandeln. (PostgreSQL: upper())"));
        CustomFunctions.Add(new("AFLower", 1, "lower(CAST(<p1> AS VARCHAR))", "Text in Kleinbuchstaben umwandeln. (PostgreSQL: lower())"));
        CustomFunctions.Add(new("AFRTrim", 1, "rtrim(CAST(<p1> AS VARCHAR))", "Leerzeichen am Ende entfernen. (PostgreSQL: rtrim())"));
        CustomFunctions.Add(new("AFLTrim", 1, "ltrim(CAST(<p1> AS VARCHAR))", "Leerzeichen am Anfang entfernen. (PostgreSQL: ltrim())"));
        CustomFunctions.Add(new("AFAllTrim", 1, "btrim(CAST(<p1> AS VARCHAR)))", "Leerzeichen am Anfang und am Ende entfernen. (PostgreSQL: btrim())"));
        CustomFunctions.Add(new("AFSubstring", 3, "substr(CAST(<p1> AS VARCHAR), <p2>, <p3>)", "Text ab Position p1 mit Länge p2 ermitteln. (PostgreSQL: substr())"));
        CustomFunctions.Add(new("AFLeft", 2, "left(CAST(<p1> AS VARCHAR), <p2>)", "Zeichen ab Anfang ermitteln. (PostgreSQL: left)"));
        CustomFunctions.Add(new("AFRight", 2, "right(CAST(<p1> AS VARCHAR), <p2>)", "Zeichen vom Ende ermitteln. (PostgreSQL: left)"));
        CustomFunctions.Add(new("AFYear", 1,"date_part('year', <p1>)", "Jahr eines Datums ermitteln. (PostgreSQL: date_part('year'...)"));
        CustomFunctions.Add(new("AFMonth", 1,"date_part('month', <p1>)", "Monat eines Datums ermitteln. (PostgreSQL: date_part('month'...)"));
        CustomFunctions.Add(new("AFDay", 1,"date_part('day', <p1>)", "Tag eines Datums ermitteln. (PostgreSQL: date_part('day'...)"));
        CustomFunctions.Add(new("AFHour", 1,"date_part('hour', <p1>)", "Stunde eines Datums/einer Zeitangabe ermitteln. (PostgreSQL: date_part('hour'...)"));
        CustomFunctions.Add(new("AFAddDays", 2, "<p1> + make_interval(days => <p2>)", "Einem Datum (p1) Anzahl (p2) Tage hinzufügen. (PostgreSQL: <p2> + <p1>)"));
        CustomFunctions.Add(new("AFAddMonths", 2, "<p1> + make_interval(months => <p2>)", "Einem Datum (p1) Anzahl (p2) Monate hinzufügen. (PostgreSQL: <p2> + interval '<p1> month')"));
        CustomFunctions.Add(new("AFAddYears", 2, "<p1> + make_interval(years => <p2>)", "Einem Datum (p1) Anzahl (p2) Jahre hinzufügen. (PostgreSQL: <p2> + interval '<p1> year')"));
        CustomFunctions.Add(new("AFAbs", 1,"abs(<p1>)", "Absoluten Wert ermitteln. (PostgreSQL: abs())"));
        CustomFunctions.Add(new("AFRound", 2,"round(<p1>, <p2>)", "Wert (p1) auf angegebene Anzahl Kommastellen (p2) runden. (PostgreSQL: round())\""));
        CustomFunctions.Add(new("AFAscii", 1,"chr(<p1>)", "ASCII-Wert eines Zeichens ermitteln. (PostgreSQL: chr())"));
        CustomFunctions.Add(new("AFIIf", 3,"IIF(<p1>,<p2>,<p3>)", "Wert abhängig von Bedingung zurückgeben. (Wenn <p1> zutrifft, <p2> sonst <p3> zurückgeben, PostgreSQL: IIF())"));
        CustomFunctions.Add(new("AFGuid", 1,"<p1>", "Eine GUID zurückgeben."));
        CustomFunctions.Add(new("AFDate", 3,"make_date(<p1>, <p2>, <p3>)", "Datum aus Jahr (p1), Monat (p2) und Tag (p3) erzeugen. (PostgreSQL: make_date())"));
        CustomFunctions.Add(new("AFDateTime", 6,"make_timestamp(<p1>, <p2>, <p3>, <p4>, <p5>, <p6>)", "Zeitstempel aus Jahr (p1), Monat (p2), Tag (p3), Stunde (p4), Minute (p5) und Sekunde (p6) erzeugen. (PostgreSQL: make_timestamp())"));
        CustomFunctions.Add(new("AFToInteger", 1,"CAST(btrim(<p1>)) AS INTEGER)", "In einen Integer Wert umwandeln. (PostgreSQL: CAST(... AS INTEGER)"));
        CustomFunctions.Add(new("AFToDecimal", 1,"CAST(btrim(<p1>)) AS DOUBLE PRECISION)", "In einen Deimal Wert umwandeln. (PostgreSQL: CAST(... AS DOUBLE PRECISION)"));
        CustomFunctions.Add(new("AFToDateTime", 1,"CAST(btrim(<p1>) AS TIMESTAMP)", "In einen Timestamp umwandeln. (PostgreSQL: CAST(... AS TIMESTAMP)"));
        CustomFunctions.Add(new("AFSoundEx", 1,"AFSOUNDEX(<p1>)", " In SoundEx-Zeichenkette umwandeln (Kölner Notation)."));
        CustomFunctions.Add(new("AFBetween", 3,"<p1> BETWEEN <p2> AND <p3>", "Prüfen ob ein Wert (p1) zwischen p2 und p3 liegt. (PostgreSQL: ... BETWEEN ... AND ...)"));
        CustomFunctions.Add(new("AFNotBetween", 3,"<p1> NOT BETWEEN <p2> AND <p3>", "Prüfen ob ein Wert (p1) NICHT zwischen p2 und p3 liegt. (PostgreSQL: ... NOT BETWEEN ... AND ...)"));
        CustomFunctions.Add(new("AFCurrentDate", 0,"now()", "Aktuelles Datum als Timestamp (PostgreSQL: now())"));
        CustomFunctions.Add(new("AFConcat", 2, "concat(CAST(<p1> AS VARCHAR), CAST(<p2> AS VARCHAR))", "Zwei Zeichenketten miteinander verknüpfen. (PostgreSQL: concat())"));
        CustomFunctions.Add(new("AFConcat", 3, "concat(CAST(<p1> AS VARCHAR), CAST(<p2> AS VARCHAR), CAST(<p3> AS VARCHAR))", "Drei Zeichenketten miteinander verknüpfen. (PostgreSQL: concat())"));
        CustomFunctions.Add(new("AFConcat", 4, "concat(CAST(<p1> AS VARCHAR), CAST(<p2> AS VARCHAR), CAST(<p3> AS VARCHAR), CAST(<p4> AS VARCHAR))", "Vier Zeichenketten miteinander verknüpfen. (PostgreSQL: concat())"));
        CustomFunctions.Add(new("AFConcat", 5, "concat(CAST(<p1> AS VARCHAR), CAST(<p2> AS VARCHAR), CAST(<p3> AS VARCHAR), CAST(<p4> AS VARCHAR), CAST(<p5> AS VARCHAR))", "Fünf Zeichenketten miteinander verknüpfen. (PostgreSQL: concat())"));
        CustomFunctions.Add(new("AFTrue", 0,"IS TRUE", "True/Wahr zurückgeben/prüfen. (PostgreSQL: .. IS TRUE)"));
        CustomFunctions.Add(new("AFFalse", 0,"IS NOT TRUE", "Nicht True/Unwahr zurückgeben/prüfen. (PostgreSQL: .. IS NOT TRUE)"));
        CustomFunctions.Add(new("AFCoalesce", 2,"COALESCE(<p1>, <p2>)", "Ersten Wert von zweien zurückgeben, der nicht null ist. (PostgreSQL: COALESCE())"));
        CustomFunctions.Add(new("AFCoalesce", 3,"COALESCE(<p1>, <p2>, <p3>)", "Ersten Wert von dreien zurückgeben, der nicht null ist. (PostgreSQL: COALESCE())"));
        CustomFunctions.Add(new("AFCoalesce", 4,"COALESCE(<p1>, <p2>, <p3>, <p4>)", "Ersten Wert von vieren zurückgeben, der nicht null ist. (PostgreSQL: COALESCE())"));
        CustomFunctions.Add(new("AFCoalesce", 5,"COALESCE(<p1>, <p2>, <p3>, <p4>, <p5>)", "Ersten Wert von fünfen zurückgeben, der nicht null ist. (PostgreSQL: COALESCE())"));
        CustomFunctions.Add(new("AFReplace", 3,"replace(<p1>, <p2>, <p3>)", "Text p2 durch Text p3 in Text p1 ersetzen. (PostgreSQL: replace())"));
        CustomFunctions.Add(new("AFFirst", 1,"LIMIT <p1> ", "Anzahl der Datensätze für Ergebnis auf p1 Datensätze begrenzen. (PostgreSQL: LIMIT"));
        CustomFunctions.Add(new("AFRemoveTime", 1,"date_trunc('day', <p1>)", "Zeitangabe aus einem Timestamp entfernen (Zeit 00:00:00 setzen). (PostgreSQL: date_trunc('day', ...))"));

        return translate(ref query);
    }

    /// <summary>
    /// Spaltentyp von PostgreSQL nach Type übersetzen.
    /// 
    /// Liefert typeof(Nullable) für unbekannte Spaltentypen.
    /// </summary>
    /// <param name="columntype"></param>
    /// <returns></returns>
    public Type GetColumnType(string columntype)
    {
        if (!typeTranslation.ContainsKey(columntype.ToLower())) return typeof(Nullable);

        return typeTranslation[columntype.ToLower()];
    }

#if (NET481_OR_GREATER)
    /// <summary>
    /// Parameter für Query aktualisieren.
    /// </summary>
    /// <param name="parameter">Parameter</param>
    /// <param name="propertyType">Datentyp des Wertes</param>
    public override void UpdateParameter<TParameter>(TParameter parameter, Type propertyType)
    {
        if (parameter is not NpgsqlParameter npgsqlparameter) return;

        if (propertyType == typeof(DateOnly))
        {
            npgsqlparameter.DataTypeName = "date";
            DateOnly oldval = (DateOnly)parameter.Value;
            npgsqlparameter.Value = new DateTime(oldval.Year, oldval.Month, oldval.Day);
        }
        else if (propertyType == typeof(TimeOnly))
        {
            npgsqlparameter.DataTypeName = "time without time zone";
            TimeOnly oldval = (TimeOnly)parameter.Value;
            npgsqlparameter.Value = new TimeSpan(oldval.Hour, oldval.Minute, oldval.Second); //  DateTime(2000, 1, 1, oldval.Hour, oldval.Minute, oldval.Second);
        }
    }
#endif
}