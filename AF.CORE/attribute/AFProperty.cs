using System.Resources;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace AF.CORE;

/// <summary>
/// Attribut, das eine Eigenschaft als 'Property' zur Darstellung in einem IPropertyDialog markiert
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class AFProperty: Attribute, IVariable
{
    private string _caption = "";
    private string _description = "";
    private string _groupCaption = "";
    private string _groupDescription = "";
    private readonly ResourceManager? _resourceManager;

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFProperty() { }

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFProperty(Type resourceType) { _resourceManager = new ResourceManager(resourceType); }

    /// <summary>
    /// Überschrift/Label
    /// </summary>
    public string Caption
    {
        get
        {
            if (_resourceManager == null)
                return _caption;

            string? caption = _resourceManager.GetString(_caption);
            return string.IsNullOrWhiteSpace(caption) ? _caption : caption;
        }
        set => _caption = value;
    }

    /// <summary>
    /// Beschreibung
    /// </summary>
    public string Description
    {
        get
        {
            if (_resourceManager == null)
                return _description;

            string? description = _resourceManager.GetString(_description);
            return string.IsNullOrWhiteSpace(description) ? _description : description;
        }
        set => _description = value;
    }

    /// <summary>
    /// Überschrift/Label der Gruppe
    /// </summary>
    public string GroupCaption
    {
        get
        {
            if (_resourceManager == null)
                return _groupCaption;

            string? caption = _resourceManager.GetString(_groupCaption);
            return string.IsNullOrWhiteSpace(caption) ? _groupCaption : caption;
        }
        set => _groupCaption = value;
    }

    /// <summary>
    /// Beschreibung der Gruppe
    /// </summary>
    public string GroupDescription
    {
        get
        {
            if (_resourceManager == null)
                return _groupDescription;

            string? description = _resourceManager.GetString(_groupDescription);
            return string.IsNullOrWhiteSpace(description) ? _groupDescription : description;
        }
        set => _groupDescription = value;
    }
    
    /// <summary>
    /// Legt fest, ob diese Eigenschaft den Standardeditor für den Wert verwendet (false) oder ob ein benutzerdefinierter Editor verwendet werden soll (false).
    /// Wenn dieser Wert true ist, müssen Sie die Aktion GetCustomEditor und GetCustomEditorType im Controller für diesen Typ behandeln. 
    /// Das Vorhandensein eines Controllers für diesen Typ ist in diesem Fall zwingend erforderlich. Wenn kein Controller vorhanden ist, hat diese Option 
    /// keine Bedeutung
    /// </summary>
    public bool UseCustomEditor { get; set; }

    /// <summary>
    /// Standardposition der Eigenschaft (Reihenfolge im Dialog)
    /// </summary>
    public int PropertyIndex { get; set; } = -1;

    /// <summary>
    /// Spalte, in der der Wert dargestellt wird.
    /// </summary>
    public int Column { get; set; } = 1;

    /// <summary>
    /// Daretsellung auf zwei Zeilen (mit Zeilenumbruch)
    /// </summary>
    public bool LineBreak { get; set; } = false;

    /// <summary>
    /// Darstellung über zwei Spalten (wenn zweispaltiges Layout verwendet wird).
    /// </summary>
    public bool TwoColumn { get; set; }

    /// <summary>
    /// Darstellung als Seite in einem Tab-Control.
    /// </summary>
    public bool InTab { get; set; }

    /// <summary>
    /// PropertyDescription, zu der diese Beschreibung gehört
    /// </summary>
    public PropertyDescription? PropertyDescription { get; set; }

    /// <summary>
    /// Name des Feldes, dass die anzuzeigenden Daten enthält. 
    /// Der Feldname kann alternativ zu ColumnProperty verwendet werden.
    /// Er wird nur verwendet, wenn ColumnProperty NULL ist!
    /// </summary>
    public string? PropertyFieldname { get; set; }

    /// <summary>
    /// Typ der anzuzeigenden Daten.
    /// Kann alternativ zu ColumnProperty verwendet werden.
    /// Wird nur beachtet, wenn ColumnProperty NULL ist!
    /// </summary>
    public Type? ColumnType { get; set; }

    /// <summary>
    /// Feldname der Spalte
    /// </summary>
    public string FieldName =>
        this.PropertyDescription != null
            ? this.PropertyDescription.Name
            : this.PropertyFieldname ?? this.Caption;


    /// <inheritdoc />
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [XmlIgnore]
    [JsonIgnore]
    public string VAR_NAME { get; set; } = string.Empty;

    /// <inheritdoc />
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [XmlIgnore]
    [JsonIgnore]
    public string VAR_CAPTION { get => Caption; set => Caption = value; }

    /// <inheritdoc />
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [XmlIgnore]
    [JsonIgnore]
    public string VAR_DESCRIPTION { get => Description; set => Description = value; }

    /// <inheritdoc />
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [XmlIgnore]
    [JsonIgnore]
    public int VAR_PRIORITY { get => PropertyIndex; set => PropertyIndex = value; }

    /// <summary>
    /// Spezifische Daten der Eigenschaft (abhängig vom Typ) binär serialisiert.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [XmlIgnore]
    [JsonIgnore]
    public VariableStorageObject VAR_STORAGE { get; set; } = new(); // Wird für AFProperty ignoriert!

    /// <inheritdoc />
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [XmlIgnore]
    [JsonIgnore]
    public string VAR_SECCAPTION { get => GroupCaption; set => GroupCaption = value; }

    /// <inheritdoc />
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [XmlIgnore]
    [JsonIgnore]
    public string VAR_SECDESCRIPTION { get => GroupDescription; set => GroupDescription = value; }

    /// <inheritdoc />
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [XmlIgnore]
    [JsonIgnore]
    public int VAR_TYP { get; set; } = (int)eVariableType.Bool; // Wird für AFProperty ignoriert!

    /// <inheritdoc />
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [XmlIgnore]
    [JsonIgnore]
    public VariableBase VAR_VARIABLE { get; set; } = new(); // Wird für AFProperty ignoriert!

    /// <inheritdoc />
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [XmlIgnore]
    [JsonIgnore]
    public bool VAR_READONLY { get; set; } = false;

    /// <inheritdoc />
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [XmlIgnore]
    [JsonIgnore]
    public bool VAR_TWOCOLUMN { get => TwoColumn; set => TwoColumn = value; }

    /// <inheritdoc />
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [XmlIgnore]
    [JsonIgnore]
    public int VAR_COLUMN { get => Column; set => Column = value; }

    /// <inheritdoc />
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [XmlIgnore]
    [JsonIgnore]
    public bool VAR_TABBED { get => InTab; set => InTab = value; }

    /// <inheritdoc />
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [XmlIgnore]
    [JsonIgnore]
    public bool VAR_MULTIPLE { get; set; } = false;

    /// <inheritdoc />
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [XmlIgnore]
    [JsonIgnore]
    public object? VAR_CONTROL { get; set; } = null;

    /// <inheritdoc />
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [XmlIgnore]
    [JsonIgnore]
    public bool VAR_NOEDITOR => false;
}