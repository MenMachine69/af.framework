using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace AF.CORE;

/// <summary>
/// Definition einer Variablen
/// </summary>
[Serializable]
public class Variable : DataObjectBase, IVariable
{
    private string _varName = string.Empty;
    private string _varCaption = string.Empty;
    private string _varDescription = string.Empty;
    private int _varTyp = (int)eVariableType.String;
    private int _varPriority;
    private bool _varReadOnly;
    private bool _varTwoColumn;
    private int _varColumn = 1;
    private bool _varTabbed;
    private string _varSecCaption = string.Empty;
    private string _varSecDescription = string.Empty;

    /// <inheritdoc />
    [AFBinding]
    [AFContext("Name", Description = "Name der Variablen. Dieser Name muss eindeutig sein.")]
    [AFGridColumn(Bold = true, ColumnIndex = 1)]
    public string VAR_NAME
    {
        get => _varName;
        set => Set(ref _varName, value);
    }

    /// <inheritdoc />
    [AFBinding]
    [AFContext("Überschrift/Bezeichnung", Description = "Bezeichnung/Überschrift, die dem Benutzer statt des Namens der Variablen angezeigt werden soll.")]
    public string VAR_CAPTION
    {
        get => _varCaption;
        set => Set(ref _varCaption, value);
    }

    /// <inheritdoc />
    [AFBinding]
    [AFContext("Beschreibung", Description = "Beschreibung der Variablen für den Benutzer.")]
    public string VAR_DESCRIPTION
    {
        get => _varDescription;
        set => Set(ref _varDescription, value);
    }

    /// <inheritdoc />
    [AFBinding]
    [AFContext("Typ", Description = "Typ der Variablen.")]
    public int VAR_TYP
    {
        get => _varTyp;
        set => Set(ref _varTyp, value);
    }

    /// <inheritdoc />
    [AFBinding]
    [AFContext("Reihenfolge", Description = "Reihenfolge der Variablen. Diese Reihenfolge bestimmt die Reihenfolge der Eingaben für die Variablenwerte durch den Benutzer.")]
    [AFGridColumn(ColumnIndex = 2, Width = 30, FixedWidth = true)]
    public int VAR_PRIORITY
    {
        get => _varPriority;
        set => Set(ref _varPriority, value);
    }

    /// <inheritdoc />
    [JsonIgnore]
    [XmlIgnore]
    public VariableBase VAR_VARIABLE { get => this.GetVariable(); set => this.SetVariable(value); }

    /// <inheritdoc />
    [AFBinding]
    [AFContext("Nicht bearbeitbar", Description = "Benutzer kann Wert der Variablen nicht bearbeiten (immer Vorgabewert oder berechnet).")]
    public bool VAR_READONLY
    {
        get => _varReadOnly;
        set => Set(ref _varReadOnly, value);
    }

    /// <summary>
    /// Zweispaltige Anzeige des Wertes in Eingabeformularen.
    /// 
    /// Sobald dieser Wert bei einer Variablen auf true gesetzt wird, wird ein zeipaltiges Forumlar erzeugt.
    /// </summary>
    [AFBinding]
    [AFContext("zweispaltig", Description = "Den Wert in Formularen zweipaltig anzeigen.")]
    public bool VAR_TWOCOLUMN
    {
        get => _varTwoColumn;
        set => Set(ref _varTwoColumn, value);
    }

    /// <summary>
    /// Den Wert in Formularen in dieser Spalte anzeigen (1 oder 2).
    /// 
    /// Sobald dieser Wert bei einer Variablen auf den Wert 2 gesetzt wird, wird ein zeipaltiges Forumlar erzeugt.
    /// </summary>
    [AFBinding]
    [AFContext("Spalte", Description = "Den Wert in Formularen in dieser Spalte anzeigen (1 oder 2).")]
    [AFRules(Maximum = 2, Minimum = 1)]
    public int VAR_COLUMN
    {
        get => _varColumn;
        set => Set(ref _varColumn, value);
    }

    /// <summary>
    /// Den Wert in Formularen in einem separaten TAB anzeigen (Tabcontrol).
    /// 
    /// Sobald dieser Wert bei einer Variablen auf true gesetzt wird, wird das Control zur Eingabe in einem TAB angezeigt. 
    /// Folgende Variablen, die ebenfalls in Tabs angezeigt werden sollen, erzeugen jeweils einen neuen TAB.
    /// </summary>
    [AFBinding]
    [AFContext("in Tab anzeigen", Description = "Den Wert in Formularen in einem separaten TAB anzeigen (Tabcontrol).")]
    public bool VAR_TABBED
    {
        get => _varTabbed;
        set => Set(ref _varTabbed, value);
    }

    /// <summary>
    /// Überschrift eines Abschnitts. 
    /// 
    /// Wird hier ein Text eingetragen, erscheint im Formular automatisch ein neuer Abschnitt mit dieser Überschrift vor der eigentlichen Eingabe.
    /// </summary>
    [AFBinding]
    [AFContext("Überschrift Abschnitt", Description = "Überschrift eines Abschnitts. Wird hier ein Text eingetragen, erscheint im Formular " +
        "automatisch ein neuer Abschnitt mit dieser Überschrift vor der eigentlichen Eingabe.")]
    public string VAR_SECCAPTION
    {
        get => _varSecCaption;
        set => Set(ref _varSecCaption, value);
    }

    /// <summary>
    /// Beschreibung eines Abschnitts. 
    /// 
    /// Wird hier ein Text eingetragen, erscheint im Formular automatisch ein neuer Abschnitt mit dieser Überschrift vor der eigentlichen Eingabe.
    /// </summary>
    [AFBinding]
    [AFContext("Beschreibung Abschnitt", Description = "Beschreibung eines Abschnitts. Wird hier ein Text eingetragen, erscheint im Formular " +
                                                      "automatisch ein neuer Abschnitt mit diesem Text, der die folgenden Eingaben beschreibt vor der eigentlichen Eingabe.")]
    public string VAR_SECDESCRIPTION
    {
        get => _varSecDescription;
        set => Set(ref _varSecDescription, value);
    }

    /// <inheritdoc />
    public bool VAR_MULTIPLE { get; set; } = false;

    /// <inheritdoc />
    [JsonIgnore]
    [XmlIgnore]
    public virtual object? VAR_CONTROL { get; set; } = null;

    /// <summary>
    /// Storage zur Speicherung der VariablenDetails
    /// </summary>
    public VariableStorageObject VAR_STORAGE { get; set; } = new();

    /// <inheritdoc />
    public bool VAR_NOEDITOR { get; set; } = false;
}

/// <summary>
/// Objekt/Variable, die in einem VariableStorage gespeichert werden kann.
/// </summary>
public class VariableStorageObject
{
    /// <summary>
    /// Typ der Variablen
    /// </summary>
    public int VariableType { get; set; } = (int)eVariableType.Bool;

    /// <summary>
    /// Daten (serialisiert) der Variablen
    /// </summary>
    public byte[] VariableStore { get; set; } = [];

    /// <summary>
    /// Speichert eine Variable 
    /// </summary>
    /// <param name="variable"></param>
    public void Store(VariableBase variable)
    {
        Type vartype = variable.GetType();

        if (vartype == typeof(VariableBool))
        {
            VariableType = (int)eVariableType.Bool;
            VariableStore = ((VariableBool)variable).ToJsonBytes();
            return;
        }

        if (vartype == typeof(VariableDateTime))
        {
            VariableType = (int)eVariableType.DateTime;
            VariableStore = ((VariableDateTime)variable).ToJsonBytes();
            return;
        }

        if (vartype == typeof(VariableDecimal))
        {
            VariableType = (int)eVariableType.Decimal;
            VariableStore = ((VariableDecimal)variable).ToJsonBytes();
            return;
        }

        if (vartype == typeof(VariableInt))
        {
            VariableType = (int)eVariableType.Int;
            VariableStore = ((VariableInt)variable).ToJsonBytes();
            return;
        }

        if (vartype == typeof(VariableList))
        {
            VariableType = (int)eVariableType.List;
            VariableStore = ((VariableList)variable).ToJsonBytes();
            return;
        }

        if (vartype == typeof(VariableModel))
        {
            VariableType = (int)eVariableType.Model;
            VariableStore = ((VariableModel)variable).ToJsonBytes();
            return;
        }

        if (vartype == typeof(VariableQuery))
        {
            VariableType = (int)eVariableType.Query;
            VariableStore = ((VariableQuery)variable).ToJsonBytes();
            return;
        }

        if (vartype == typeof(VariableScript))
        {
            VariableType = (int)eVariableType.Script;
            VariableStore = ((VariableScript)variable).ToJsonBytes();
            return;
        }

        if (vartype == typeof(VariableString))
        {
            VariableType = (int)eVariableType.String;
            VariableStore = ((VariableString)variable).ToJsonBytes();
            return;
        }

        if (vartype == typeof(VariableMemo))
        {
            VariableType = (int)eVariableType.Memo;
            VariableStore = ((VariableMemo)variable).ToJsonBytes();
            return;
        }

        if (vartype == typeof(VariableRichText))
        {
            VariableType = (int)eVariableType.RichText;
            VariableStore = ((VariableRichText)variable).ToJsonBytes();
            return;
        }

        if (vartype == typeof(VariableGuid))
        {
            VariableType = (int)eVariableType.Guid;
            VariableStore = ((VariableGuid)variable).ToJsonBytes();
            return;
        }

        if (vartype == typeof(VariableFormula))
        {
            VariableType = (int)eVariableType.Formula;
            VariableStore = ((VariableFormula)variable).ToJsonBytes();
            return;
        }

        if (vartype == typeof(VariableMonth))
        {
            VariableType = (int)eVariableType.Month;
            VariableStore = ((VariableMonth)variable).ToJsonBytes();
            return;
        }

        if (vartype == typeof(VariableYear))
        {
            VariableType = (int)eVariableType.Year;
            VariableStore = ((VariableYear)variable).ToJsonBytes();
            return;
        }

        if (vartype == typeof(VariableSection))
        {
            VariableType = (int)eVariableType.Section;
            VariableStore = ((VariableSection)variable).ToJsonBytes();
            return;
        }

        if (typeof(VariableBase).GetController() is not VariableBaseController ctrl) return;

        var customtype = ctrl.CustomTypes.Values.FirstOrDefault(v => vartype == v.VariableType);

        if (customtype == null) throw new ArgumentOutOfRangeException($"Unbekannter Variablentyp {vartype.FullName}.");
        
        VariableType = customtype.VariableTypIndex;

        var methode = customtype.VariableType.GetMethod("Serialize");
        
        VariableStore = methode!.Invoke(variable, null) as byte[] ?? [];
    }

    /// <summary>
    /// Eine Variable aus den Daten laden.
    /// </summary>
    /// <returns>geladene Variable</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public VariableBase Load()
    {
        switch (VariableType)
        {
            case (int)eVariableType.Bool:
                return Functions.DeserializeJsonBytes<VariableBool>(VariableStore, true) ?? new VariableBool();
            case (int)eVariableType.DateTime:
                return Functions.DeserializeJsonBytes<VariableDateTime>(VariableStore, true) ?? new VariableDateTime();
            case (int)eVariableType.Decimal:
                return Functions.DeserializeJsonBytes<VariableDecimal>(VariableStore, true) ?? new VariableDecimal();
            case (int)eVariableType.Int:
                return Functions.DeserializeJsonBytes<VariableInt>(VariableStore, true) ?? new VariableInt();
            case (int)eVariableType.List:
                return Functions.DeserializeJsonBytes<VariableList>(VariableStore, true) ?? new VariableList();
            case (int)eVariableType.Model:
                return Functions.DeserializeJsonBytes<VariableModel>(VariableStore, true) ?? new VariableModel();
            case (int)eVariableType.Query:
                return Functions.DeserializeJsonBytes<VariableQuery>(VariableStore, true) ?? new VariableQuery();
            case (int)eVariableType.Script:
                return Functions.DeserializeJsonBytes<VariableScript>(VariableStore, true) ?? new VariableScript();
            case (int)eVariableType.String:
                return Functions.DeserializeJsonBytes<VariableString>(VariableStore, true) ?? new VariableString();
            case (int)eVariableType.Memo:
                return Functions.DeserializeJsonBytes<VariableMemo>(VariableStore, true) ?? new VariableMemo();
            case (int)eVariableType.RichText:
                return Functions.DeserializeJsonBytes<VariableRichText>(VariableStore, true) ?? new VariableRichText();
            case (int)eVariableType.Guid:
                return Functions.DeserializeJsonBytes<VariableGuid>(VariableStore, true) ?? new VariableGuid();
            case (int)eVariableType.Formula:
                return Functions.DeserializeJsonBytes<VariableFormula>(VariableStore, true) ?? new VariableFormula();
            case (int)eVariableType.Year:
                return Functions.DeserializeJsonBytes<VariableYear>(VariableStore, true) ?? new VariableYear();
            case (int)eVariableType.Month:
                return Functions.DeserializeJsonBytes<VariableMonth>(VariableStore, true) ?? new VariableMonth();
            case (int)eVariableType.Section:
                return Functions.DeserializeJsonBytes<VariableSection>(VariableStore, true) ?? new VariableSection();
            default:
            {
                if (typeof(VariableBase).GetController() is not VariableBaseController ctrl) throw new Exception("VariableBaseController nicht gefunden.");

                if (!ctrl.CustomTypes.TryGetValue(VariableType, out var customtype)) throw new Exception($"Unbekannter Variablentyp {VariableType}.");


                var methode = customtype.VariableType.GetMethod("Deserialze");

                return (VariableBase)(methode!.Invoke(null, [VariableStore]) ?? Activator.CreateInstance(customtype.VariableType))!;
            }
        }
    }
}

/// <summary>
/// Einzelner Variablenwert nach Bearbeitung durch den Benutzer.
/// 
/// Diese Klasse ist serialisierbar, so, dass die User-Eingaben gespeichert werden können. 
/// </summary>
[Serializable]
public class VariableUserValue
{
    /// <summary>
    /// ID der Variablen
    /// </summary>
    public IVariable? Variable { get; set; }
    /// <summary>
    /// Name der Variablen
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Maske für die Textversion der Variablen
    /// </summary>
    public string? Mask { get; set; }

    /// <summary>
    /// Wert der Variablen
    /// </summary>
    public object? Value { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public VariableUserValue() {  }

    /// <summary>
    /// Liefert Value als String für das Ersetzen in einem Script.
    /// </summary>
    /// <returns></returns>
    public string GetScriptValueAsString()
    {
        string ret;
        
        if (Value == null) return "";

        if (Value is string s)
            ret = s.Trim();
        else if (Value is DateTime time)
            ret = $"new DateTime({time.Year.ToString().Trim()}, {time.Month.ToString().Trim()}, {time.Day.ToString().Trim()})";
        else if (Value is bool value)
            ret = value ? "true" : "false";
        else if (Value is short || Value is int || Value is long || Value is float || Value is decimal)
            ret = Value.ToString()?.Replace(".", "").Replace(",", ".") ?? "";
        else
            ret = Value.ToString()?.Trim() ?? "";

        return ret;
    }
}