namespace AF.CORE;

/// <summary>
/// Model für eine Dashboard-Konfiguration
/// </summary>
[Serializable]
public class AFDashboardModel 
{
    /// <summary>
    /// Seiten im Model
    /// </summary>
    public BindingList<AFDashboardPageModel> Pages { get; set; } = [];

    /// <summary>
    /// Spaltenanzahl (default: 20)
    /// </summary>
    public int Rows { get; set; } = 20;

    /// <summary>
    /// Zeilenanzahl (default: 20)
    /// </summary>
    public int Columns { get; set; } = 20;

    /// <summary>
    /// Abstand zwischen den Elementen (default: 10)
    /// </summary>
    public int SpaceBetween { get; set; } = 4;

    /// <summary>
    /// Model zu Byte-Array serialisieren.
    /// </summary>
    /// <param name="model">das zu serialisierende Model</param>
    /// <returns>serialisierte Daten des Models</returns>
    public static byte[] Save(AFDashboardModel model)
    {
        return model.ToJsonBytes();
    }
    
    /// <summary>
    /// Serialisiertes Model laden.
    /// </summary>
    /// <param name="data">Daten des Models</param>
    /// <returns>das deserialisierte Model oder NULL</returns>
    public static AFDashboardModel? Load(byte[] data)
    {
        return Functions.DeserializeJsonBytes<AFDashboardModel>(data);
    }
}

/// <summary>
/// Seite in einem Dashboard
/// </summary>
[Serializable]
public class AFDashboardPageModel 
{
    /// <summary>
    /// Elemente auf der Seite
    /// </summary>
    public BindingList<AFDashboardPageModel> Elements { get; set; } = [];

    /// <summary>
    /// Überschrift der Seite
    /// </summary>
    public string Caption { get; set; } = "";

    /// <summary>
    /// Index des ObjectImage, dass zur Darstellung der Page verwendet werden soll
    /// </summary>
    public int? PageObjectImageIndex { get; set; } = null;

    /// <summary>
    /// Position/Reihenfolge der Seiten
    /// </summary>
    public int PagePosition { get; set; } = 0;

    /// <summary>
    /// Spaltenanzahl (default: 20)
    /// </summary>
    public int Rows { get; set; } = 20;

    /// <summary>
    /// Zeilenanzahl (default: 20)
    /// </summary>
    public int Columns { get; set; } = 20;

    /// <summary>
    /// Abstand zwischen den Elementen (default: 10)
    /// </summary>
    public int SpaceBetween { get; set; } = 10;
}

/// <summary>
/// Element in einem Dashboard
/// </summary>
[Serializable]
public class AFDashboardElementModel 
{
    /// <summary>
    /// ID des Elements (aus Element-Katalog)
    /// </summary>
    public Guid ElemenetId { get; set; } = Guid.Empty;

    /// <summary>
    /// Zeile (linke obere Ecke)
    /// </summary>
    public int Row { get; set; } = 0;

    /// <summary>
    /// Spalte (linke obere Ecke)
    /// </summary>
    public int Column { get; set; } = 0;

    /// <summary>
    /// Höhe des Elements (in Zeilen)
    /// </summary>
    public int Height { get; set; } = 0;

    /// <summary>
    /// Breite des Elements (in Spalten)
    /// </summary>
    public int Width { get; set; } = 0;

    /// <summary>
    /// Überschrift des Elements
    /// </summary>
    public string Caption { get; set; } = "";
}

