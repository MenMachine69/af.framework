namespace AF.DATA;

/// <summary>
/// Schema, dass eine Datenbank beschreibt.
/// </summary>
public sealed class DatabaseScheme
{
    /// <summary>
    /// Liste der Tabellen/Views in der Datenbank
    /// </summary>
    public BindingList<DatabaseSchemeTable> Tables { get; set; } = [];

    /// <summary>
    /// Liste der Tabellen/Views in der Datenbank
    /// </summary>
    public BindingList<DatabaseSchemeJoin> Joins { get; set; } = [];

    /// <summary>
    /// Liste der Variablen die im Desigenr definiert sind.
    /// 
    /// Diese Liste ist Query spezifisch!
    /// </summary>
    public BindingList<Variable> Variablen { get; set; } = [];
}

/// <summary>
/// Beschreibung einer Tabelle/eines Views in der Datenbank
/// </summary>
public sealed class DatabaseSchemeTable : BaseBuffered
{
    private string? _alias;
    private string _tableScheme = string.Empty;
    private BindingList<DatabaseSchemeField> _fields;
    private string _tableName = string.Empty;
    private string _tableDescription = string.Empty;
    private WeakEvent<EventHandler<EventArgs>>? _needRecreate;


    /// <summary>
    /// Ereignis, das ausgelöst wird, wenn sich Eigenschaften der Tabelle/des Views oder eines der Felder geändert 
    /// haben und der SQL-Code neu generiert werden muss.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public event EventHandler<EventArgs> OnNeedRecreate
    {
        add
        {
            _needRecreate ??= new();
            _needRecreate.Add(value);
        }
        remove => _needRecreate?.Remove(value);
    }


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">Name der Tabelle/des Views</param>
    /// <param name="isview">ist View (optional, default = false)</param>
    /// <param name="description">Beschreibung (optional, default = "")</param>
    /// <param name="scheme">Schema/Namespace der Tabelle in der Datenbank</param>
    public DatabaseSchemeTable(string name, bool isview = false, string description = "", string scheme = "") : this()
    {
        TABLE_NAME = name;
        TABLE_DESCRIPTION = description;
        TABLE_SCHEME = scheme;
        IsView = isview;
       
    }

    private void beforeAddField(object? sender, AddingNewEventArgs e)
    {
        if (e.NewObject is not DatabaseSchemeField field) return;

        field.Table = this;
    }

    /// <summary>
    /// eindeutige ID der Tabelle
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name der Tabelle/des Views
    /// </summary>
    [AFBinding(ReadOnly = true)]
    [AFContext("Tabelle/View", Description = "Name der Tabelle/des Views in der Datenbank.")]
    [AFProperty(Caption = "Tabelle/View", PropertyIndex = 0, LineBreak = true)]
    public string TABLE_NAME
    {
        get => _tableName;
        set => Set(ref _tableName, value);
    }

    /// <summary>
    /// Name der Tabelle/des Views
    /// </summary>
    [AFBinding(ReadOnly = true)]
    [AFContext("Schema/Namespace", Description = "Schema/Namespace in der sich die Tabelle in der Datenbank befindet.")]
    public string TABLE_SCHEME
    {
        get => _tableScheme;
        set => Set(ref _tableScheme, value);
    }

    /// <summary>
    /// Name der Tabelle/des Views
    /// </summary>
    [AFBinding]
    [AFContext("Alias", Description = "Alias Name des Tabelle/des Views in der Abfrage.")]
    [AFProperty(Caption = "Aliasname", PropertyIndex = 1, GroupCaption = "Anpassungen", GroupDescription = "Anpassungen für die Tabelle/den View in der Abfrage.")]
    public string TABLE_ALIAS { get => _alias ?? TABLE_NAME; set => Set(ref _alias, value); }

    /// <summary>
    /// Beschreibung der Tabelle/des Views
    /// </summary>
    [AFBinding]
    [AFContext(typeof(CoreStrings))]
    public string TABLE_DESCRIPTION
    {
        get => _tableDescription;
        set => Set(ref _tableDescription, value);
    }

    /// <summary>
    /// Liste der Felder in der Tabelle/ dem View
    /// </summary>
    public BindingList<DatabaseSchemeField> Fields
    {
        get => _fields;
        set
        {
            _fields.AddingNew -= beforeAddField;
            _fields = value;
            _fields.AddingNew += beforeAddField;
        }
    }

    /// <summary>
    /// Gibt an, ob es sich um einen View handelt
    /// </summary>
    [AFBinding]
    [AFContext(typeof(CoreStrings))]
    public bool IsView { get; set; }

    /// <summary>
    /// Typ des Eintrags (View oder Table)
    /// </summary>
    [AFBinding]
    [AFContext(typeof(CoreStrings))]
    public string SchemeType => IsView ? CoreStrings.LBL_VIEW : CoreStrings.LBL_TABLE;

    /// <summary>
    /// Konstruktor
    /// </summary>
    public DatabaseSchemeTable()
    {
        _fields = [];
        _fields.AddingNew += beforeAddField;

        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Löst das PropertyChanged Ereignis aus.
    /// </summary>
    /// <param name="property">Name des Proertys/der Eigenschaft die geändert wurde</param>
    public override void RaisePropertyChangedEvent(string property)
    {
        base.RaisePropertyChangedEvent(property);

        _needRecreate?.Raise(this, EventArgs.Empty);
    }

    

    /// <summary>
    /// Konstruktor (Copy)
    /// </summary>
    /// <param name="from"></param>
    public DatabaseSchemeTable(DatabaseSchemeTable from) : this()
    {
        TABLE_NAME = from.TABLE_NAME;
        TABLE_ALIAS = from.TABLE_ALIAS;
        TABLE_SCHEME = from.TABLE_SCHEME;
        TABLE_DESCRIPTION = from.TABLE_DESCRIPTION;
        IsView = from.IsView;
        Id = from.Id;

        foreach (var field in from.Fields)
        {
            var newfield = new DatabaseSchemeField(field);
            newfield.Table = this;

            Fields.Add(newfield);
        }
    }
}



/// <summary>
/// Beschreibung eines Feldes in einer Tabelle/einem View
/// </summary>
[AFContext(typeof(CoreStrings))]
[AFOptions(ControllerType = typeof(DatabaseScheme))]
public sealed class DatabaseSchemeField : ModelBase
{
    private string? _fieldAlias;
    private string _fieldExpression = string.Empty;
    private string _fieldDescription = string.Empty;
    private string _fieldWhereCondition = string.Empty;
    private bool _fieldGroupBy;
    private bool _isCalculated;
    private string _fieldName = string.Empty;
    private string _fieldPostReadExpression = "WERT";

    /// <summary>
    /// Tabelle, zu der das Feld gehört
    /// </summary>
    public DatabaseSchemeTable? Table { get; set; }
    
    /// <summary>
    /// eindeutige ID des Feldes
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name des Feldes
    /// </summary>
    [AFBinding(ReadOnly = true)]
    [AFContext("Feldname", Description = "Name des Feldes in der Datenbanktabelle.")]
    [AFProperty(Caption = "Feldname", PropertyIndex = 0, LineBreak = true)]
    public string FIELD_NAME { get => (FIELD_CALCULATED && FIELD_ALIAS.IsNotEmpty() ? FIELD_ALIAS : _fieldName) ; set => Set(ref _fieldName, value); }

    /// <summary>
    /// Aliasname des Feldes
    /// </summary>
    [AFBinding(ReadOnly = true)]
    [AFContext("Feldtyp", Description = "Typ des Feldes in der Datenbank.")]
    [AFProperty(Caption = "Feldtyp", PropertyIndex = 1, GroupCaption = "Feldeigenschaften", GroupDescription = "Eigenschaften des Feldes in der Datenbank.")]
    public string FIELD_TYPE { get; set; } = string.Empty;

    /// <summary>
    /// max. Länge des Feldes in der Datenbank
    /// </summary>
    [AFBinding(ReadOnly = true)]
    [AFContext("Feldlänge", Description = "Maximale Länge des Feldes in der Datenbank.")]
    [AFProperty(Caption = "Feldlänge", PropertyIndex = 2)]
    public int FIELD_LENGTH { get; set; }

    /// <summary>
    /// System-Typ des Feldes in der Datenbank
    /// </summary>
    [AFBinding(ReadOnly = true)]
    [AFContext("Systemtyp", Description = "Typ Werte in der Ausgabe.")]
    [AFProperty(Caption = "Ausgabetyp", PropertyIndex = 3)]
    public Type FIELD_SYSTEMTYPE { get; set; } = typeof(Nullable);

    
    /// <summary>
    /// Aliasname des Feldes
    /// </summary>
    [AFBinding]
    [AFContext("Alias", Description = "Alias Name des Feldes in der Abfrage.")]
    [AFProperty(Caption = "Aliasname", PropertyIndex = 4, GroupCaption = "Anpassungen", GroupDescription = "Anpassungen für das Feld in der Ausgabe.")]
    public string FIELD_ALIAS { get => _fieldAlias ?? FIELD_NAME; set => Set(ref _fieldAlias, value); }

    /// <summary>
    /// Gibt an, ob nach dem Feld gruppiert werden soll.
    /// </summary>
    [AFBinding]
    [AFContext("Gruppieren", Description = "Gibt an, ob nach dem Feld gruppiert werden soll.")]
    [AFProperty(Caption = "Gruppieren", PropertyIndex = 5)]
    public bool FIELD_GROUPBY { get => _fieldGroupBy; set => Set(ref _fieldGroupBy, value); }

    /// <summary>
    /// WHERE Bedingung für das Feld.
    /// </summary>
    [AFBinding]
    [AFContext("Bedingung", Description = "Bedingung für das Feld.")]
    [AFProperty(Caption = "Bedingung (WHERE)", PropertyIndex = 6, LineBreak = true)]
    public string FIELD_WHERE { get => _fieldWhereCondition; set => Set(ref _fieldWhereCondition, value); }


    /// <summary>
    /// Gibt an, ob es sich um ein berechnetes Feld handelt.
    ///
    /// Berechnete Felder stammen NICHT aus der Datenbank. Sie wurden zum Beispiel im QueryDesigner hinzugefügt.
    /// </summary>
    [AFBinding]
    [AFContext("Berechnet", Description = "Gibt an, ob es sich um ein berechnetes Feld handelt.")]
    public bool FIELD_CALCULATED { get => _isCalculated; set => Set(ref _isCalculated, value); }


    /// <summary>
    /// Formel des berechneten Feldes
    /// </summary>
    [AFBinding]
    [AFContext("Formel/Ausdruck", Description = "Formel/Ausdruck zur berechnung des Felds.")]
    [AFProperty(Caption = "Formel/Ausdruck", PropertyIndex = 9, LineBreak = true)]
    public string FIELD_EXPRESSION { get => _fieldExpression; set => Set(ref _fieldExpression, value); }

    /// <summary>
    /// Beschreibung des Feldes
    /// </summary>
    [AFBinding]
    [AFContext(typeof(CoreStrings))]
    public string FIELD_DESCRIPTION { get => _fieldDescription; set => Set(ref _fieldDescription, value); }

    /// <summary>
    /// Formel die auf den gelesenen Wert angewendet werden soll
    /// </summary>
    [AFBinding]
    [AFContext("Formel/Ausdruck", Description = "Formel/Ausdruck die auf den aus der Datenquelle gelesenen Wert angewendet wird und den Wert für die Ausgabe liefert (Umwandlungen etc.).")]
    [AFProperty(Caption = "Formel/Ausgabe", PropertyIndex = 7, LineBreak = true)]
    public string FIELD_POSTREADEXPRESSION { get => _fieldPostReadExpression; set => Set(ref _fieldPostReadExpression, value); }

    /// <summary>
    /// vollst. Darstellung des Felds inkl. Typ und max. Länge
    /// </summary>
    public string FIELD_FULLNAME => @$"{FIELD_NAME} ({FIELD_TYPE}{(FIELD_LENGTH > 0 ? @":" + FIELD_LENGTH : "")})";
    
    /// <summary>
    /// Gibt an, ob das Feld ausgewählt ist.
    /// </summary>
    [AFBinding]
    [AFContext(typeof(CoreStrings))]
    public bool IsSelected { get; set; }

    /// <summary>
    /// Konstruktor
    /// </summary>
    public DatabaseSchemeField()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Löst das PropertyChanged Ereignis aus.
    /// </summary>
    /// <param name="property">Name des Proertys/der Eigenschaft die geändert wurde</param>
    public override void RaisePropertyChangedEvent(string property)
    {
        base.RaisePropertyChangedEvent(property);

        Table?.RaisePropertyChangedEvent(property);
    }

    /// <summary>
    /// Konstruktor (Copy)
    /// </summary>
    /// <param name="from"></param>
    public DatabaseSchemeField(DatabaseSchemeField from) : this()
    {
        FIELD_NAME = from.FIELD_NAME;
        FIELD_TYPE = from.FIELD_TYPE;
        FIELD_LENGTH = from.FIELD_LENGTH;
        FIELD_ALIAS = from.FIELD_ALIAS;
        FIELD_DESCRIPTION = from.FIELD_DESCRIPTION;
        FIELD_SYSTEMTYPE = from.FIELD_SYSTEMTYPE;
        FIELD_EXPRESSION = from.FIELD_EXPRESSION;
        FIELD_CALCULATED = from.FIELD_CALCULATED;
        FIELD_POSTREADEXPRESSION = from.FIELD_POSTREADEXPRESSION;
        FIELD_WHERE = from.FIELD_WHERE;
        IsSelected = from.IsSelected;
        Id = from.Id;
    }
}


/// <summary>
/// Join zwischen zwei Tabellen/Views
/// </summary>
public sealed class DatabaseSchemeJoin : BaseBuffered, IJoin
{
    private eJoinType _joinType = eJoinType.LeftJoin;
    private DatabaseSchemeTable _fromTable;
    private DatabaseSchemeField _fromField;
    private DatabaseSchemeTable _toTable;
    private DatabaseSchemeField _toField;


    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="fromTable"></param>
    /// <param name="fromField"></param>
    /// <param name="toTable"></param>
    /// <param name="toField"></param>
    public DatabaseSchemeJoin(
        DatabaseSchemeTable fromTable,
        DatabaseSchemeField fromField, 
        DatabaseSchemeTable toTable,
        DatabaseSchemeField toField)
    {
        _fromTable = fromTable;
        _fromField = fromField;
        _toTable = toTable;
        _toField = toField;

        FromField = fromField.Id;
        ToField = toField.Id;
        ElementSource = fromTable.Id;
        ElementTarget = toTable.Id;
    }

    /// <summary>
    /// eindeutige ID des Joins
    /// </summary>
    [AFBinding]
    [AFContext(typeof(CoreStrings))]
    public Guid Id { get; set; }

    /// <summary>
    /// ID der Ausgangs-Tabelle
    /// </summary>
    [AFBinding]
    [AFContext(typeof(CoreStrings))]
    public Guid ElementSource { get; set; }

    /// <summary>
    /// ID der Ziel-Tabelle
    /// </summary>
    [AFBinding]
    [AFContext(typeof(CoreStrings))] 
    public Guid ElementTarget { get; set; }

    /// <summary>
    /// ID des Ausgangs-Feldes
    /// </summary>
    [AFBinding]
    [AFContext(typeof(CoreStrings))] 
    public Guid FromField { get; set; }

    /// <summary>
    /// ID des Ziel-Feldes
    /// </summary>
    [AFBinding]
    [AFContext(typeof(CoreStrings))] 
    public Guid ToField { get; set; }

    /// <summary>
    /// Art des Joins
    /// </summary>
    [AFBinding]
    [AFContext(typeof(CoreStrings))]
    [AFProperty(Caption = "Join-Typ", PropertyIndex = 5, GroupCaption = "Eigenschaften", GroupDescription = "Eigenschaften die die Art und Weise des Joins zwischen Quelle und Ziel beschreiben.")]
    public eJoinType JoinType { get => _joinType; set => Set(ref _joinType, value); }

    /// <summary>
    /// Art des Joins
    /// </summary>
    [AFBinding(ReadOnly = true)]
    [AFContext(typeof(CoreStrings))]
    [AFProperty(Caption = "Tabelle/View", PropertyIndex = 1, GroupCaption = "Quelle", GroupDescription = "Tabelle/View von der/dem der Join erfolgt.")]
    public string FromTableText => _fromTable.TABLE_NAME;

    /// <summary>
    /// Art des Joins
    /// </summary>
    [AFBinding(ReadOnly = true)]
    [AFContext(typeof(CoreStrings))]
    [AFProperty(Caption = "Feld", PropertyIndex = 2)]
    public string FromFieldText => _fromField.FIELD_NAME;

    /// <summary>
    /// Art des Joins
    /// </summary>
    [AFBinding(ReadOnly = true)]
    [AFContext(typeof(CoreStrings))]
    [AFProperty(Caption = "Tabelle/View", PropertyIndex = 3, GroupCaption = "Ziel", GroupDescription = "Tabelle/View zu der/dem der Join erfolgt.")]
    public string ToTableText => _toTable.TABLE_NAME;

    /// <summary>
    /// Art des Joins
    /// </summary>
    [AFBinding(ReadOnly = true)]
    [AFContext(typeof(CoreStrings))]
    [AFProperty(Caption = "Feld", PropertyIndex = 4)]
    public string ToFieldText => _toField.FIELD_NAME;

    /// <summary>
    /// Beschreibung der Verknüpfung.
    /// </summary>
    public string Label
    {
        get
        {
            return JoinType switch
            {
                eJoinType.LeftJoin => @"n:1",
                eJoinType.RightJoin => @"1:n",
                eJoinType.FullOuterJoin => @"n:n",
                eJoinType.InnerJoin => @"1:1",
                _ => ""
            };
        }
    }

    /// <summary>
    /// JOIN Typ als SQL String.
    /// </summary>
    public string JoinString
    {
        get
        {
            return JoinType switch
            {
                eJoinType.LeftJoin => @"LEFT JOIN ",
                eJoinType.RightJoin => @"RIGHT JOIN ",
                eJoinType.FullOuterJoin => @"FULL OUTER JOIN ",
                eJoinType.InnerJoin => @"INNER JOIN ",
                _ => ""
            };
        }
    }

    /// <summary>
    /// Prüft, ob bereits ein Join in der Auflistung existiert
    /// </summary>
    /// <param name="joins">Liste der vorhandenen Joins</param>
    /// <returns>true, wenn es bereits ein entsprechendes Join gibt</returns>
    public bool Exist(IEnumerable<IJoin> joins)
    {
        return joins.FirstOrDefault(j =>
            j.ElementSource.Equals(ElementSource) &&
            j.ElementSource.Equals(ElementTarget) &&
            j.ElementSource.Equals(FromField) &&
            j.ElementSource.Equals(ToField)) != null;
    }
}