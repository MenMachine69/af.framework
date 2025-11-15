namespace AF.DATA;

/// <summary>
/// Symbole für die in ITranslator verwendeten SQL-Befehle
/// </summary>
public enum eCommandString
{
    /// <summary>
    /// Tabelle löschen
    /// </summary>
    DropTable,
    /// <summary>
    /// Index löschen
    /// </summary>
    DropIndex,
    /// <summary>
    /// View löschen
    /// </summary>
    DropView,
    /// <summary>
    /// Procedure löschen
    /// </summary>
    DropProcedure,
    /// <summary>
    /// Trigger löschen
    /// </summary>
    DropTrigger,
    /// <summary>
    /// Feld löschen
    /// </summary>
    DropField,
    /// <summary>
    /// Tabelle prüfen
    /// </summary>
    ExistTable,
    /// <summary>
    /// Indfex prüfen
    /// </summary>
    ExistIndex,
    /// <summary>
    /// View prüfen
    /// </summary>
    ExistView,
    /// <summary>
    /// Procedure prüfen
    /// </summary>
    ExistProcedure,
    /// <summary>
    /// Trigger prüfen
    /// </summary>
    ExistTrigger,
    /// <summary>
    /// Tabelle erzeugen
    /// </summary>
    CreateTable,
    /// <summary>
    /// Index erzeugen
    /// </summary>
    CreateIndex,
    /// <summary>
    /// View erzeugen
    /// </summary>
    CreateView,
    /// <summary>
    /// Procedure erzeugen
    /// </summary>
    CreateProcedure,
    /// <summary>
    /// Trigger erzeugen
    /// </summary>
    CreateTrigger,
    /// <summary>
    /// Feld erzeugen
    /// </summary>
    CreateField,
    /// <summary>
    /// PrimaryKey Feld erzeugen
    /// </summary>
    CreateKeyField,
    /// <summary>
    /// Trigger: BeforeInsert erzeugen
    /// </summary>
    TriggerBeforeInsert,
    /// <summary>
    /// Trigger: BeforeUpdate erzeugen
    /// </summary>
    TriggerBeforeUpdate,
    /// <summary>
    /// Trigger: BeforeInsert-Function erzeugen
    /// </summary>
    TriggerBeforeInsertFunc,
    /// <summary>
    /// Trigger: BeforeUpdate-Function erzeugen
    /// </summary>
    TriggerBeforeUpdateFunc,
    /// <summary>
    /// Code vor Änderung des Schemas
    /// </summary>
    BeforeAlterSchema,
    /// <summary>
    /// Trigger einschalten
    /// </summary>
    EnableTrigger,
    /// <summary>
    /// Trigger ausschalten
    /// </summary>
    DisableTrigger,
    /// <summary>
    /// Serverzeit ermitteln
    /// </summary>
    GetServerTime,
    /// <summary>
    /// Feld: Type
    /// </summary>
    FieldDefType,
    /// <summary>
    /// Feld: Bool
    /// </summary>
    FieldDefBool,
    /// <summary>
    /// Feld: Image
    /// </summary>
    FieldDefImage,
    /// <summary>
    /// Feld: Guid
    /// </summary>
    FieldDefGuid,
    /// <summary>
    /// Feld: DateTime
    /// </summary>
    FieldDefDateTime,
    /// <summary>
    /// Feld: DateOnly
    /// </summary>
    FieldDefDate,
    /// <summary>
    /// Feld: TimeOnly
    /// </summary>
    FieldDefTime,
    /// <summary>
    /// Feld: Binary/Blob
    /// </summary>
    FieldDefBinary,
    /// <summary>
    /// Feld: Integer
    /// </summary>
    FieldDefInt,
    /// <summary>
    /// Feld: Decimal
    /// </summary>
    FieldDefDecimal,
    /// <summary>
    /// Feld: Double
    /// </summary>
    FieldDefDouble,
    /// <summary>
    /// Feld: Byte
    /// </summary>
    FieldDefByte,
    /// <summary>
    /// Feld: Short
    /// </summary>
    FieldDefShort,
    /// <summary>
    /// Feld: Integer16
    /// </summary>
    FieldDefInt16,
    /// <summary>
    /// Feld: Integer32
    /// </summary>
    FieldDefInt32,
    /// <summary>
    /// Feld: Integer64
    /// </summary>
    FieldDefInt64,
    /// <summary>
    /// Feld: Float
    /// </summary>
    FieldDefFloat,
    /// <summary>
    /// Feld: Long
    /// </summary>
    FieldDefLong,
    /// <summary>
    /// Feld: Object (serialisiert)
    /// </summary>
    FieldDefObject,
    /// <summary>
    /// Feld: String
    /// </summary>
    FieldDefString,
    /// <summary>
    /// Feld: Memo (unbegrenzter String)
    /// </summary>
    FieldDefMemo,
    /// <summary>
    /// Feld: Color
    /// </summary>
    FieldDefColor,
    /// <summary>
    /// Schema der DB lesen
    /// </summary>
    GetSchema,
    /// <summary>
    /// KeyWord für FIRST/TOP bei Select
    /// </summary>
    KeyWordFirst,
    /// <summary>
    /// Ereignis: vor dem Insert
    /// </summary>
    EventBeforeInsert,
    /// <summary>
    /// Ereignis: vor dem Update
    /// </summary>
    EventBeforeUpdate,
    /// <summary>
    /// Ereignis: vor dem Löschen
    /// </summary>
    EventBeforeDelete,
    /// <summary>
    /// Ereignis: nach dem Insert
    /// </summary>
    EventAfterInsert,
    /// <summary>
    /// Ereignis: nach dem Update
    /// </summary>
    EventAfterUpdate,
    /// <summary>
    /// Ereignis: nach dem Löschen
    /// </summary>
    EventAfterDelete,
    /// <summary>
    /// Einzelwert laden
    /// </summary>
    LoadValue,
    /// <summary>
    /// Ereignis: nach dem Anpassen der Feldlänge (String)
    /// </summary>
    AlterFieldLength,
    /// <summary>
    /// 
    /// </summary>
    SelectCount,
    /// <summary>
    /// 
    /// </summary>
    SelectSum,
    /// <summary>
    /// 
    /// </summary>
    Select,
    /// <summary>
    /// 
    /// </summary>
    SelectTop,
    /// <summary>
    /// 
    /// </summary>
    Top,
    /// <summary>
    /// 
    /// </summary>
    Load,
    /// <summary>
    /// 
    /// </summary>
    Delete,
    /// <summary>
    /// 
    /// </summary>
    DeleteQuery,
    /// <summary>
    /// 
    /// </summary>
    Update,
    /// <summary>
    /// 
    /// </summary>
    Insert,
    /// <summary>
    /// 
    /// </summary>
    ExecProcedure,
    /// <summary>
    /// 
    /// </summary>
    Exist,
    /// <summary>
    /// 
    /// </summary>
    AfterAlterSchema,
    /// <summary>
    /// Constraint prüfen
    /// </summary>
    ExistConstraint,
    /// <summary>
    /// Constraint löschen
    /// </summary>
    DropConstraint,
    /// <summary>
    /// Constraint anlegen
    /// </summary>
    CreateConstraint,
    /// <summary>
    /// Einen Benutzer für eine Tabelle/einen View berechtigen
    /// </summary>
    GrantRightsToUser,
    /// <summary>
    /// Kommentar/Beschreibung einer Spalte setzen
    /// </summary>
    SetComment,
    /// <summary>
    /// Kommentar/Beschreibung einer Spalte lesen
    /// </summary>
    GetComment
}