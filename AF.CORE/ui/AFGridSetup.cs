using DevExpress.Utils.Svg;

namespace AF.CORE;

/// <summary>
/// Konfiguration für ein Grid.
///
/// Dieses Setup unterstützt GridViews, BandedGridViews und AdvBandedGridViews.
/// </summary>
public sealed class AFGridSetup
{
    /// <summary>
    /// Constructor
    /// </summary>
    public AFGridSetup() { }


    /// <summary>
    /// Eindeutige ID der Einstellungen zur Verwendung mit Persistenz
    /// </summary>
    public Guid GridIdentifier { get; set; } = Guid.Empty;

    /// <summary>
    /// Beschreibung der Daten im Grid
    /// </summary>
    public string GridDescription { get; set; } = "";

    /// <summary>
    /// Anzeige als Tree statt Grid
    /// </summary>
    public bool BrowseAsTree { get; set; }


    /// <summary>
    /// Bands des Grid
    /// </summary>
    public List<GridBandSettings> Bands { get; set; } = [];

    /// <summary>
    /// Spalte des Grid
    /// 
    /// Diese Spalten werden für alle Grid-Typen ohne Bands verwendet
    /// </summary>
    public List<AFGridColumn> Columns { get; set; } = [];

    /// <summary>
    /// Spalten/Gruppen in einem KanbanView
    /// </summary>
    public List<AFKanbanGroup> KanbanGroups { get; set; } = [];

    /// <summary>
    /// Optionen für den KanbanView
    /// </summary>
    public AFKanbanOptions KanbanOptions { get; } = new();

    /// <summary>
    /// Erstellt ein neues Band und fügt es zu den Bands hinzu. 
    /// </summary>
    /// <param name="name"></param>
    /// <returns>Einstellungen</returns>
    public GridBandSettings AddBand(string name)
    {
        if (Bands.FirstOrDefault(band => band.Name == name) != null)
            throw new Exception(string.Format(CoreStrings.ERR_GRID_BANDALLWASEXIST, name));

        GridBandSettings bandSettings = new(name, this);
        Bands.Add(bandSettings);

        return bandSettings;
    }
    /// <summary>
    /// Create a new Band and adds it to the Bands 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="caption"></param>
    /// <param name="columns"></param>
    /// <returns>Einstellungen des Bandes</returns>
    public GridBandSettings AddBand(string name, string caption, params string[] columns)
    {
        GridBandSettings bandSettings = AddBand(name);
        bandSettings.Caption = caption;
        foreach (string fieldname in columns)
        {
            var column = Columns.First(x => x.FieldName == fieldname);
            bandSettings.Columns.Add(column);
        }

        return bandSettings;
    }

    /// <summary>
    /// Symbol zur Anzeige in Comboboxen als Symbol für die
    /// Modelle/Objekte, die dort ausgewählt werden können
    ///
    /// Sollte ein SVG oder ein Image Objekt sein.
    /// </summary>
    public object? Symbol { get; set; }

    /// <summary>
    /// Standard-Gridstil
    /// </summary>
    public eGridMode DefaultGridStyle { get; set; } = eGridMode.GridView;

    /// <summary>
    /// verfügbare/unterstützte Gridstile
    /// </summary>
    public eGridMode SupportedGridStyle { get; set; } =
        eGridMode.GridView | eGridMode.BandedGridView | eGridMode.AdvBandedGridView;

    /// <summary>
    /// Einträge im Raster können bearbeitet werden (Standard: false)
    /// </summary>
    public bool AllowEdit { get; set; }

    /// <summary>
    /// Neue Einträge können erstellt werden (Standard: false)
    /// </summary>
    public bool AllowAddNew { get; set; }

    /// <summary>
    /// mehrere Einträge können ausgewählt werden (Standard: false)
    /// </summary>
    public bool AllowMultiSelect { get; set; }


    /// <summary>
    /// Selektionsmodus für Zeilen im Grid
    /// </summary>
    public eSelectionMode SelectionMode { get; set; } = eSelectionMode.Single;

    /// <summary>
    /// Kontrollkästchen für Zeilenauswahl verwenden
    /// </summary>
    public bool UseCheckBoxSelection { get; set; }

    /// <summary>
    /// Feld, das in der Vorschau angezeigt wird (leer = keine Vorschau)
    /// </summary>
    public string PreviewField { get; set; } = string.Empty;

    /// <summary>
    /// Feldname für die Sortierung des Eintrags
    /// </summary>
    public string SortOn { get; set; } = string.Empty;

    /// <summary>
    /// Sortiermodus für die Einträge
    /// </summary>
    public eOrderMode SortOrder { get; set; } = eOrderMode.Ascending;

    /// <summary>
    /// Feldnamen für die Gruppierung der Einträge
    /// </summary>
    public string[] GroupBy { get; set; } = [];

    /// <summary>
    /// Methode, die für die Gestaltung einer Zelle verwendet werden soll.
    ///
    /// Erwartet wird eine Methode, die als ersten Parameter Informationen zur Zelle (i.d.R. EventArgs)  und als zweiten Parameter die Datenzeile erhält. 
    /// </summary>
    public Action<object, object>? GridCellStyler { get; set; }

    /// <summary>
    /// Methode, die für die Gestaltung einer Zeile verwendet werden soll.
    ///
    /// Erwartet wird eine Methode, die als ersten Parameter Informationen zur Zeile (i.d.R. EventArgs)  und als zweiten Parameter die Datenzeile erhält. 
    /// </summary>
    public Action<object, object, object>? GridRowStyler { get; set; }

    /// <summary>
    /// Methode, die zum Zeichnen einer Zelle verwendet werden soll.
    ///
    /// Erwartet wird eine Methode, die als ersten Parameter Informationen zur Zelle (i.d.R. EventArgs) und als zweiten Parameter die Datenzeile erhält. 
    /// </summary>
    public Action<object, object>? CellPainter { get; set; }

    /// <summary>
    /// Methode, die zum anpassen des anzuzeigenden Textes.
    ///
    /// Erwartet wird eine Methode, die als ersten Parameter Informationen zur Zelle (i.d.R. CustomColumnDisplayTextEventArgs) und als zweiten Parameter die Datenzeile erhält. 
    /// </summary>
    public Action<object, object>? DisplayTextStyler { get; set; }

    /// <summary>
    /// Action, die statt CmdGoto ausgeführt werden soll. 
    /// 
    /// Der Action wird das View übergeben.
    /// 
    /// Kann nicht zusammen mit CmdGoto verwendet werden. Ist beides zugewiesen, wird immer CmdGoto bevorzugt.
    /// </summary>
    public Action<object>? OnGotoAction { get; set; }

    /// <summary>
    /// Action, die statt CmdDelete ausgeführt werden soll. 
    /// 
    /// Der Action wird das View übergeben.
    /// 
    /// Kann nicht zusammen mit CmdDelete verwendet werden. Ist beides zugewiesen, wird immer CmdDelete bevorzugt.
    /// </summary>
    public Action<object>? OnDeleteAction { get; set; }

    /// <summary>
    /// Action, die statt CmdEdit ausgeführt werden soll. 
    /// 
    /// Der Action wird das View übergeben.
    /// 
    /// Kann nicht zusammen mit CmdEdit verwendet werden. Ist beides zugewiesen, wird immer CmdEdit bevorzugt.
    /// </summary>
    public Action<object>? OnEditAction { get; set; }

    /// <summary>
    /// Action, die statt CmdAdd ausgeführt werden soll. 
    /// 
    /// Der Action wird das View übergeben.
    /// 
    /// Kann nicht zusammen mit CmdAdd verwendet werden. Ist beides zugewiesen, wird immer CmdAdd bevorzugt.
    /// </summary>
    public Action<object>? OnAddAction { get; set; }

    /// <summary>
    /// Action, die statt CmdShowDetail ausgeführt werden soll. 
    /// 
    /// Der Action wird das View übergeben.
    /// 
    /// Kann nicht zusammen mit CmdShowDetail verwendet werden. Ist beides zugewiesen, wird immer CmdShowDetail bevorzugt.
    /// </summary>
    public Action<object>? OnShowDetailAction { get; set; }


    /// <summary>
    /// Befehl zum Springen zum Element (Element als Master anzeigen)
    /// </summary>
    public AFCommand? CmdGoto { get; set; }

    /// <summary>
    /// Befehl zum Bearbeiten des Elements
    /// </summary>
    public AFCommand? CmdEdit { get; set; }

    /// <summary>
    /// Befehl zum Löschen des Elements
    /// </summary>
    public AFCommand? CmdDelete { get; set; }

    /// <summary>
    /// Befehl zum Anzeigen von Details für das Element
    /// </summary>
    public AFCommand? CmdShowDetail { get; set; }

    /// <summary>
    /// Befehl zum Hinzufügen eines neuen Elements
    /// </summary>
    public AFCommand? CmdAdd { get; set; }

    /// <summary>
    /// Standardlayout (serialisiert)
    ///
    /// Wird verwendet, um das Layout des Grids zurückzusetzen.
    /// </summary>
    public byte[]? DefaultLayout { get; set; }

    /// <summary>
    /// HTML-Template für TileView-Darstellung mit HTML-Format
    /// </summary>
    public string DefaultHtmlTemplate { get; set; } = "";

    /// <summary>
    /// CSS-Template für TileView-Darstellung mit HTML-Format
    /// </summary>
    public string DefaultCssTemplate { get; set; } = "";

    /// <summary>
    /// Filter, der standardmässig gesetzt werden soll.
    /// </summary>
    public string? DefaultFilter { get; set; } = null;

    /// <summary>
    /// Typ der Models, die im Grid dargestellt werden.
    /// </summary>
    public Type? ModelType { get; set; } = null;

    /// <summary>
    /// Spaltenauswahl und Reihenfolge anpassen
    /// </summary>
    /// <param name="columnnames">Namen der Spalten, die das Grid enthalten soll. Spalten, deren Name hier nicht enthalten sind werden entfernt.
    /// Die Reihenfolge der Spalten bestimmt die Spalten im Grid.</param>
    public void SetColumnsByName(string[] columnnames)
    {
        foreach (AFGridColumn column in Columns)
        {
            if (!columnnames.Contains(column.FieldName))
                column.Visible = false;
            else
                column.ColumnIndex = columnnames.IndexOfString(column.FieldName);
        }
    }

    /// <summary>
    /// Symbole für die Verwendung in HTML-Templates (TilView, ExplorerView usw.)
    /// </summary>
    public Tuple<string, SvgImage>[]? HtmlImages { get; set; }

    /// <summary>
    /// Automatische Anpassung der Spaltenbreite an die Breite des Grids (alle Spalten sichtbar)
    /// </summary>
    public bool ColumnAutoWidth { get; set; } = true;
}