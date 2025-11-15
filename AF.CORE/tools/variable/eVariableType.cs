namespace AF.CORE;

/// <summary>
/// verfügbare Variablentypen
/// </summary>
public enum eVariableType
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    [Description("Ja/Nein Werte")]
    Bool,
    [Description("Datum/Zeit")]
    DateTime,
    [Description("Dezimalzahl")]
    Decimal,
    [Description("Ganzzahl")]
    Int,
    [Description("Auswahlliste")]
    List,
    [Description("Model-Auswahl")]
    Model,
    [Description("Datenquelle")]
    Query,
    [Description("Script (berechnet)")]
    Script,
    [Description("einzeiliger Text")]
    String,
    [Description("mehrzeiliger Text")]
    Memo,
    [Description("formatierter Text")]
    RichText,
    [Description("ID/Guid")]
    Guid,
    [Description("Formelausdruck (berechnet)")]
    Formula,
    [Description("Abschnitt (keine Eingabe)")]
    Section,
    [Description("Monat")]
    Month,
    [Description("Jahr")]
    Year

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}

/// <summary>
/// Typ der Rückgabe einer VariableFormula
/// </summary>
public enum eVariableFormulaType
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    [Description("Ja/Nein Werte (Bool)")]
    Bool,
    [Description("Datum/Zeit")]
    DateTime,
    [Description("Dezimalzahl (Dezimal)")]
    Decimal,
    [Description("Ganzzahl (Integer)")]
    Int,
    [Description("Text (String)")]
    String,
    [Description("GUID")]
    Guid
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}