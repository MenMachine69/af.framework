namespace AF.CORE;

/// <summary>
/// Einstellungen für ein einzelnes Band im Raster
/// </summary>
public sealed class GridBandSettings
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">Name</param>
    /// <param name="settings">Einstellkungen</param>
    internal GridBandSettings(string name, AFGridSetup settings)
    {
        Name = name;
        Settings = settings;
    }

    /// <summary>
    /// Einstellungen
    /// </summary>
    public AFGridSetup Settings { get; private set; }

    /// <summary>
    /// Eindeutiger Name des Bands
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Überschrift/Beschriftung des Bands
    /// </summary>
    public string Caption { get; set; } = "";

    /// <summary>
    /// Band kann verschoben werden (Standard: true)
    /// </summary>
    public bool AllowMove { get; set; } = true;

    /// <summary>
    /// Band kann versteckt werden
    /// </summary>
    public bool AllowHide { get; set; }

    /// <summary>
    /// Band kann in der Größe verändert werden (Standard: true)
    /// </summary>
    public bool AllowSize { get; set; } = true;

    /// <summary>
    /// Standard-Größe des Bandes
    /// 
    /// Wenn AllowSize falsch ist, ist dies die 
    /// feste Breite des Bandes.
    /// </summary>
    public int Size { get; set; } = 100;

    /// <summary>
    /// Unterbänder des aktuellen Bandes
    /// 
    /// Bänder werden in einigen Ansichten wie GridView nicht verwendet. Bei dieser Art von Ansichten werden die Bänder 
    /// ignoriert und die Spalten aller Bänder werden im Grid angezeigt.
    /// </summary>
    public List<GridBandSettings> Bands { get; } = [];

    /// <summary>
    /// Spalten des aktuellen Bandes
    /// </summary>
    public List<AFGridColumn> Columns { get; } = [];

    /// <summary>
    /// liefert eine Spalte nach ihrem Namen (wenn sie im Band oder in einem Unterband enthalten ist)
    /// </summary>
    /// <param name="name">Spaltenname</param>
    /// <returns>Einstellungen für die Spalte oder null</returns>
    internal AFGridColumn? getColumnByName(string name)
    {
        AFGridColumn? ret = null;

        foreach (AFGridColumn column in Columns)
        {
            if (column.ColumnProperty == null || column.ColumnProperty.Name != name) continue;

            ret = column;
            break;
        }

        if (ret != null) return ret;


        foreach (GridBandSettings band in Bands)
        {
            ret = band.getColumnByName(name);
            if (ret == null) break;
        }

        return ret;
    }


    /// <summary>
    /// Create a new Band and adds it to the Bands 
    /// </summary>
    /// <param name="name"></param>
    /// <returns>Einstellungen des Bandes</returns>
    public GridBandSettings AddBand(string name)
    {
        if (Settings.Bands.FirstOrDefault(band => band.Name == name) != null)
            throw new Exception(string.Format(CoreStrings.ERR_GRID_BANDALLWASEXIST, name));

        GridBandSettings bandSettings = new(name, Settings);
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
            var column = Settings.Columns.First(x => x.FieldName == fieldname);
            bandSettings.Columns.Add(column);
        }

        return bandSettings;
    }
}