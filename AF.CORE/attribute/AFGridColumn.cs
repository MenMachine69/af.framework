using System.Resources;

namespace AF.CORE;

/// <summary>
/// Attribut, das eine Eigenschaft als Spalte in GridViews markiert
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class AFGridColumn : Attribute
{
    private string _caption = "";
    private string? _dispformat;
    private readonly ResourceManager? _resourceManager;

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFGridColumn()
    {
    }

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFGridColumn(Type resourceType)
    {
        _resourceManager = new ResourceManager(resourceType);
    }

    /// <summary>
    /// Anzeige- und Bearbeitungsformat, wenn der Editor und der Wert es unterstützen (kann z. B. für numerische und Datum/Zeit-Eigenschaften verwendet werden).
    /// </summary>
    public string? DisplayFormat
    {
        get
        {
            if (_dispformat == null)
                return "";

            if (_resourceManager == null)
                return _dispformat;

            string? dispformat = _resourceManager.GetString(_dispformat);
            return string.IsNullOrWhiteSpace(dispformat) ? _dispformat : dispformat;
        }
        set => _dispformat = value;
    }

    /// <summary>
    /// Breite der Spalte im Raster
    /// </summary>
    public int Width { get; set; } = -1;

    /// <summary>
    /// Feste Breite im Raster - die Breite der Spalte kann nicht geändert werden. Erfordert die Angabe von Width
    /// </summary>
    public bool FixedWidth { get; set; } = false;

    /// <summary>
    /// Spaltenüberschrift
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
    /// Spalteninhalte in FETT anzeigen
    /// </summary>
    public bool Bold { get; set; }

    /// <summary>
    /// Sortierung erlaubt
    /// </summary>
    public bool AllowSort { get; set; } = true;

    /// <summary>
    /// Verschieben der Spalte erlaubt
    /// </summary>
    public bool AllowMove { get; set; } = true;

    /// <summary>
    /// Inhalt der Spalte kann bearbeitet werden
    /// </summary>
    public bool AllowEdit { get; set; } = true;

    /// <summary>
    /// Legt fest, ob diese Spalte den Standardeditor für den Wert verwendet (false) oder ob ein benutzerdefinierter Editor verwendet werden soll (false).
    /// Wenn dieser Wert true ist, müssen Sie die Aktion RequestCustomEditor im Controller für diesen Typ behandeln. 
    /// Das Vorhandensein eines Controllers für diesen Typ ist in diesem Fall zwingend erforderlich. Wenn kein Controller vorhanden ist, hat diese Option 
    /// keine Bedeutung
    /// </summary>
    public bool UseCustomEditor { get; set; }

    /// <summary>
    /// Spalte als Symbol anzeigen
    /// 
    /// Verwendet das interne SymbolDislpay. Der Spaltenwert muss als INT vorhanden sein.
    /// </summary>
    public bool ShowAsSymbol { get; set; }

    /// <summary>
    /// horizontale Ausrichtung des Inhalts in einer Gitterzelle
    /// </summary>
    public eAlignmentHorizontal AlignmentHorizontal { get; set; } = eAlignmentHorizontal.Default;

    /// <summary>
    /// vertikale Ausrichtung des Inhalts in einer Gitterzelle
    /// </summary>
    public eAlignmentVertical AlignmentVertical { get; set; } = eAlignmentVertical.Default;

    /// <summary>
    /// Sichtbarkeit der Spalte
    /// </summary>
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Index des Bildes in der Spaltenüberschrift
    /// </summary>
    public int ImageIndex { get; set; } = -1;

    /// <summary>
    /// Standardposition der Spalte im Grid
    /// </summary>
    public int ColumnIndex { get; set; } = -1;

    /// <summary>
    /// Typ des benutzerdefinierten Editors...
    /// </summary>
    public Type? CustomEditor { get; set; }

    /// <summary>
    /// Standard-Aggregatfunktion für Spalte
    /// </summary>
    public eAggregate Aggregate { get; set; } = eAggregate.None;

    /// <summary>
    /// PropertyDescription, zu der diese Spaltenbeschreibung gehört
    /// </summary>
    public PropertyDescription? ColumnProperty { get; set; }

    /// <summary>
    /// Name des Feldes, dass die anzuzeigenden Daten enthält. 
    /// Der Feldname kann alternativ zu ColumnProperty verwendet werden.
    /// Er wird nur verwendet, wenn ColumnProperty NULL ist!
    /// </summary>
    public string? ColumnFieldname { get; set; }

    /// <summary>
    /// Typ der anzuzeigenden Daten.
    /// Kann alternativ zu ColumnProperty verwendet werden.
    /// Wird nur beachtet, wenn ColumnProperty NULL ist!
    /// </summary>
    public Type? ColumnType { get; set; }

    /// <summary>
    /// Grid Styles, in denen die Spalte unterstützt wird (Standard ist eGridStyle.All).
    ///
    /// Dieses Attribut wird in erster Linie für GridSetup ohne IControllerUI verwendet, aber auch die Standardimplementierung von IControllerUI kennt diese Einstellung.
    /// </summary>
    public eGridStyle InStyles { get; set; } = eGridStyle.All;

    /// <summary>
    /// Spalte in TreeList für AutoFill verwenden.
    ///
    /// Ist nur in TreeList von Bedeutung. Andere Grids ignorieren diese Einstellung.
    /// </summary>
    public bool AutoFill { get; set; }

    /// <summary>
    /// Feldname der Spalte
    /// </summary>
    public string FieldName =>
        this.ColumnProperty != null
            ? this.ColumnProperty.Name
            : this.ColumnFieldname ?? this.Caption;
}