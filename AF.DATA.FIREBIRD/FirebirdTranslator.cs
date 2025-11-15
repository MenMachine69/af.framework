namespace AF.DATA;

/// <summary>
/// Translator for Firebird database
/// </summary>
[Localizable(false)]
public class FirebirdTranslator : Translator, ITranslator
{
    private readonly Dictionary<eCommandString, string> _commands = new();

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="db">Datenbank, die diesen Übersetzer verwendet</param>
    public FirebirdTranslator(FirebirdDatabase db)
    {
        Database = db.Configuration.DatabaseName;
    }

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="dbname">Name der Datenbank</param>
    public FirebirdTranslator(string dbname)
    {
        Database = dbname;
    }

    /// <summary>
    /// Datenbank, die diesen Übersetzer verwendet
    /// </summary>
    internal string Database { get; init; }

    private void _initCommands()
    {
        _commands.Add(eCommandString.CreateIndex, "CREATE #UNIQUE# #DESC# INDEX #NAME# ON #TABLENAME#(#FIELDS#);");
        _commands.Add(eCommandString.CreateProcedure, "CREATE PROCEDURE #NAME# AS #CODE#");
        _commands.Add(eCommandString.CreateTable,
            "CREATE TABLE #TABLENAME# (#FIELDNAMEKEY# CHAR(16) CHARACTER SET OCTETS NOT NULL, #FIELDNAMECREATED# TIMESTAMP, #FIELDNAMECHANGED# TIMESTAMP);");
        _commands.Add(eCommandString.CreateKeyField, "ALTER TABLE #TABLENAME# ADD PRIMARY KEY(#FIELDNAMEKEY#)");
        _commands.Add(eCommandString.CreateTrigger, "CREATE TRIGGER #NAME# FOR #TABLENAME# ACTIVE #EVENT# AS #CODE#");
        _commands.Add(eCommandString.ExistProcedure,
            "SELECT Count(RDB$PROCEDURE_NAME) FROM RDB$PROCEDURES WHERE Upper(RDB$PROCEDURE_NAME) = '#NAME#'");
        _commands.Add(eCommandString.ExistTable,
            "SELECT Count(RDB$RELATION_NAME) FROM RDB$RELATIONS WHERE RDB$RELATION_NAME = '#NAME#'");
        _commands.Add(eCommandString.ExistTrigger,
            "SELECT Count(RDB$TRIGGER_NAME) FROM RDB$TRIGGERS WHERE Upper(RDB$TRIGGER_NAME) = '#NAME#'");
        _commands.Add(eCommandString.ExistView,
            "SELECT Count(RDB$VIEW_NAME) FROM RDB$VIEW_RELATIONS WHERE Upper(RDB$VIEW_NAME) = '#NAME#'");
        _commands.Add(eCommandString.ExistIndex,
            "SELECT Count(RDB$INDEX_NAME) FROM RDB$INDICES WHERE RDB$INDEX_NAME = '#NAME#'");
        _commands.Add(eCommandString.BeforeAlterSchema,
            "DELETE FROM MON$ATTACHMENTS WHERE MON$ATTACHMENT_ID <> CURRENT_CONNECTION");
        _commands.Add(eCommandString.AfterAlterSchema, "");
        _commands.Add(eCommandString.TriggerBeforeInsert,
            "CREATE TRIGGER #TABLENAME#_BI FOR #TABLENAME# " +
            "ACTIVE BEFORE INSERT " +
            "POSITION 0 " +
            "AS " +
            "BEGIN " +
            "  NEW.#FIELDNAMECREATED# = CURRENT_TIMESTAMP; " +
            "  NEW.#FIELDNAMECHANGED# = CURRENT_TIMESTAMP; " +
            "END ");
        _commands.Add(eCommandString.TriggerBeforeUpdate,
            "CREATE TRIGGER #TABLENAME#_BU FOR #TABLENAME# " +
            "ACTIVE BEFORE UPDATE " +
            "POSITION 0 " +
            "AS " +
            "BEGIN " +
            "  NEW.#FIELDNAMECHANGED# = CURRENT_TIMESTAMP; " +
            "END ");
        _commands.Add(eCommandString.GetServerTime, "SELECT CURRENT_TIMESTAMP FROM RDB$DATABASE");
        _commands.Add(eCommandString.FieldDefBinary, "BLOB SEGMENT SIZE #BLOCKSIZE#");
        _commands.Add(eCommandString.FieldDefBool, "BOOLEAN");
        _commands.Add(eCommandString.FieldDefByte, "NUMERIC(3, 0)");
        _commands.Add(eCommandString.FieldDefDateTime, "TIMESTAMP");
        _commands.Add(eCommandString.FieldDefDate, "DATE");
        _commands.Add(eCommandString.FieldDefTime, "TIME");
        _commands.Add(eCommandString.FieldDefDecimal, "DECIMAL(18, 4)");
        _commands.Add(eCommandString.FieldDefDouble, "DOUBLE PRECISION");
        _commands.Add(eCommandString.FieldDefFloat, "FLOAT");
        _commands.Add(eCommandString.FieldDefGuid, "CHAR(16) CHARACTER SET OCTETS");
        _commands.Add(eCommandString.FieldDefImage, "BLOB SEGMENT SIZE #BLOCKSIZE#");
        _commands.Add(eCommandString.FieldDefInt, "INTEGER");
        _commands.Add(eCommandString.FieldDefInt16, "SMALLINT");
        _commands.Add(eCommandString.FieldDefInt32, "INTEGER");
        _commands.Add(eCommandString.FieldDefInt64, "BIGINT");
        _commands.Add(eCommandString.FieldDefColor, "INTEGER");
        _commands.Add(eCommandString.FieldDefLong, "BIGINT");
        _commands.Add(eCommandString.FieldDefObject, "BLOB SEGMENT SIZE #BLOCKSIZE#");
        _commands.Add(eCommandString.FieldDefShort, "SMALLINT");
        _commands.Add(eCommandString.FieldDefType, "VARCHAR(250) CHARACTER SET #CHARSET# COLLATE #COLLATION#");
        _commands.Add(eCommandString.FieldDefString, "VARCHAR(#SIZE#) CHARACTER SET #CHARSET# COLLATE #COLLATION#");
        _commands.Add(eCommandString.FieldDefMemo, "BLOB SUB_TYPE 1 SEGMENT SIZE #BLOCKSIZE# CHARACTER SET #CHARSET#");
        _commands.Add(eCommandString.AlterFieldLength,
            "ALTER TABLE #TABLENAME# ALTER COLUMN #FIELDNAME# TYPE VARCHAR(#SIZE#)");
        _commands.Add(eCommandString.DropField, "ALTER TABLE #TABLENAME# DROP #FIELDNAME#");
        _commands.Add(eCommandString.SelectTop, "SELECT FIRST #COUNT# #FIELDNAMES# FROM #TABLENAME#");
        _commands.Add(eCommandString.ExistConstraint,
            "SELECT Count(RDB$CONSTRAINT_NAME) FROM RDB$RELATION_CONSTRAINTS WHERE RDB$CONSTRAINT_NAME = '#NAME#'");
        _commands.Add(eCommandString.DropConstraint, "ALTER TABLE #TABLENAME# DROP CONSTRAINT #NAME#");
        _commands.Add(eCommandString.CreateConstraint,
            "ALTER TABLE #TABLENAME# ADD CONSTRAINT #NAME# FOREIGN KEY(#FIELDNAME#) REFERENCES #TARGETTABLE#(#TARGETFIELD#) #CONSTRAINT#");
        _commands.Add(eCommandString.GrantRightsToUser,
            "");
        _commands.Add(eCommandString.SetComment, "COMMENT ON COLUMN #TABLENAME#.#NAME# IS '#COMMENT#'");
        _commands.Add(eCommandString.GetComment,
            "SELECT f.RDB$DESCRIPTION AS FieldComment " +
            "FROM RDB$RELATION_FIELDS rf " +
            "JOIN RDB$FIELDS f ON rf.RDB$FIELD_SOURCE = f.RDB$FIELD_NAME " +
            "WHERE rf.RDB$RELATION_NAME = '#TABLENAME#' AND rf.RDB$FIELD_NAME = '#NAME#'");
    }

    /// <summary>
    /// Returns a database specific string for the specific command.  
    /// Returns the globally valid string for this command if no specific command is available.
    /// </summary>
    /// <param name="command">needed Command string</param>
    /// <returns>command string</returns>

    public override string GetCommandString(eCommandString command)
    {
        if (_commands.Count == 0)
            _initCommands();

        if (_commands.TryGetValue(command, out var cmd))
        {
            return cmd
                .Replace("#CHARSET#", DBCharSet)
                .Replace("#COLLATION#", DBCollation)
                .Replace("#DBNAME#", Database);
        }

        return base.GetCommandString(command);
    }
    
    /// <summary>
    /// Converts a value into the equivalent database value
    /// </summary>
    /// <param name="value">value to convert</param>
    /// <param name="valueType">Type of the value to be converted, if null is passed, the system attempts to determine the type from the value.</param>
    /// <param name="compress">compress data (ZIP)</param>
    /// <returns>converted value</returns>
    public override object ToDatabase(object? value, Type valueType, bool compress)
    {
        object target = base.ToDatabase(value, valueType, compress);

        if (valueType != typeof(Guid)) return target;

        if (value == null || value.Equals(Guid.Empty))
            target = DBNull.Value;
        else
            target = _translateGuid2((Guid)value).ToByteArray();


        return target;
    }

    /// <summary>
    /// Character set to use (default: UTF8)
    /// </summary>
    public string DBCharSet => "UTF8";

    /// <summary>
    /// Sorting to be used (default: UTF8)
    /// </summary>
    public string DBCollation => "UTF8";

    
    
    /// <summary>
    /// Translates a query with universal placeholders into the format required by Firebird.
    /// </summary>
    /// <param name="query">query to be translated</param>.
    /// <returns> translated query</returns>
    public string TranslateQuery(ref string query)
    {
        if (CustomFunctions.Count < 1)
        {
            CustomFunctions.Add(new("AFMax", 1,"MAX(<p1>)"));
            CustomFunctions.Add(new("AFMin", 1,"MIN(<p1>)"));
            CustomFunctions.Add(new("AFUpper", 1,"UPPER(<p1>)"));
            CustomFunctions.Add(new("AFLower", 1,"LOWER(<p1>)"));
            CustomFunctions.Add(new("AFRTrim", 1,"TRIM(TRAILING FROM <p1>)"));
            CustomFunctions.Add(new("AFLTrim", 1,"TRIM(LEADING FROM <p1>)"));
            CustomFunctions.Add(new("AFAllTrim", 1,"TRIM(BOTH FROM <p1>)"));
            CustomFunctions.Add(new("AFSubstring", 3,"SUBSTRING(<p1> FROM <p2> FOR <p3>)"));
            CustomFunctions.Add(new("AFLeft", 2,"LEFT(<p1>, <p2>)"));
            CustomFunctions.Add(new("AFRight", 2,"RIGHT(<p1>, <p2>)"));
            CustomFunctions.Add(new("AFYear", 1,"EXTRACT(YEAR FROM <p1>)"));
            CustomFunctions.Add(new("AFMonth", 1,"EXTRACT(MONTH FROM <p1>)"));
            CustomFunctions.Add(new("AFDay", 1,"EXTRACT(DAY FROM <p1>)"));
            CustomFunctions.Add(new("AFHour", 1,"EXTRACT(HOUR FROM <p1>)"));
            CustomFunctions.Add(new("AFAddDays", 2,"DATEADD(<p2> DAY TO <p1>)"));
            CustomFunctions.Add(new("AFAddMonths", 2,"DATEADD(<p2> MONTH TO <p1>)"));
            CustomFunctions.Add(new("AFAddYears", 2,"DATEADD(<p2> YEAR TO <p1>)"));
            CustomFunctions.Add(new("AFAbs", 1,"ABS(<p1>)"));
            CustomFunctions.Add(new("AFRound", 2,"ROUND(<p1>, <p2>)"));
            CustomFunctions.Add(new("AFAscii", 1,"ASCII_CHAR(<p1>)"));
            CustomFunctions.Add(new("AFIIf", 3,"IIF(<p1>,<p2>,<p3>)"));
            CustomFunctions.Add(new("AFGuid", 1,"CHAR_TO_UUID(<p1>)"));
            CustomFunctions.Add(new("AFDate", 3,"CAST((TRIM(BOTH FROM CAST(<p1> as VARCHAR(4))) || '/' || TRIM(BOTH FROM CAST(<p2> as VARCHAR(2))) || '/' || TRIM(BOTH FROM CAST(<p3> as VARCHAR(2)))) as TIMESTAMP)"));
            CustomFunctions.Add(new("AFDateTime", 6,"CAST('<p1>/<p2>/<p3> <p4>:<p5>:<p6>' as TIMESTAMP)"));
            CustomFunctions.Add(new("AFToInteger", 1,"CAST(TRIM(<p1>) as INTEGER)"));
            CustomFunctions.Add(new("AFToDecimal", 1,"CAST(TRIM(<p1>) as DOUBLE PRECISION)"));
            CustomFunctions.Add(new("AFToDateTime", 1,"CAST(TRIM(<p1>) as TIMESTAMP)"));
            CustomFunctions.Add(new("AFSoundEx", 1,"AFSOUNDEX(<p1>)"));
            CustomFunctions.Add(new("AFBetween", 3,"<p1> BETWEEN <p2> AND <p3>"));
            CustomFunctions.Add(new("AFNotBetween", 3,"<p1> NOT BETWEEN <p2> AND <p3>"));
            CustomFunctions.Add(new("AFCurrentDate", 0,"CAST('NOW' AS DATE)"));
            CustomFunctions.Add(new("AFConcat", 2,"<p1> || <p2>"));
            CustomFunctions.Add(new("AFConcat", 3,"<p1> || <p2> || <p3>"));
            CustomFunctions.Add(new("AFConcat", 4,"<p1> || <p2> || <p3> || <p4>"));
            CustomFunctions.Add(new("AFConcat", 5,"<p1> || <p2> || <p3> || <p4> || <p5>"));
            CustomFunctions.Add(new("AFTrue", 0,"true"));
            CustomFunctions.Add(new("AFFalse", 0,"false"));
            CustomFunctions.Add(new("AFCoalesce", 2,"COALESCE(<p1>, <p2>)"));
            CustomFunctions.Add(new("AFCoalesce", 3,"COALESCE(<p1>, <p2>, <p3>)"));
            CustomFunctions.Add(new("AFCoalesce", 4,"COALESCE(<p1>, <p2>, <p3>, <p4>)"));
            CustomFunctions.Add(new("AFCoalesce", 5,"COALESCE(<p1>, <p2>, <p3>, <p4>, <p5>)"));
            CustomFunctions.Add(new("AFReplace", 3,"REPLACE(<p1>, <p2>, <p3>)"));
            CustomFunctions.Add(new("AFFirst", 1,"FIRST <p1> "));
            CustomFunctions.Add(new("AFRemoveTime", 1,"CAST(CAST(<p1> AS DATE) as TIMESTAMP)"));

            foreach (var placeholder in PlaceHolders) CustomFunctions.Add(new(placeholder.Key, 0, placeholder.Value));
        }

        return translate(ref query);
    }
   
    private Guid _translateGuid(Guid wrongguid)
    {
        var rfc4122bytes = wrongguid.ToByteArray();
        //if (BitConverter.IsLittleEndian)
        //{
        //    Array.Reverse(rfc4122bytes, 0, 4);
        //    Array.Reverse(rfc4122bytes, 4, 2);
        //    Array.Reverse(rfc4122bytes, 6, 2);
        //}

        return new Guid(rfc4122bytes);
    }

    private Guid _translateGuid2(Guid wrongguid)
    {
        var rfc4122bytes = wrongguid.ToByteArray();
        
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(rfc4122bytes, 0, 4);
            Array.Reverse(rfc4122bytes, 4, 2);
            Array.Reverse(rfc4122bytes, 6, 2);
        }

        return new Guid(rfc4122bytes);
    }
}