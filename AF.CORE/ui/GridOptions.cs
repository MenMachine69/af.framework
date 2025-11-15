namespace AF.CORE;

/// <summary>
/// Optionen für eine Grid-Ansicht
/// 
/// dies wird verwendet, um einen Controller nach einem gültigen GridSetup-Objekt zu fragen
/// </summary>
public sealed class GridOptions
{
    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="style">erforderlicher Stil</param>
    /// <param name="master">den Master festlegen, für den die Gridview benötigt wird (z.B. typeof(User), um alle 'Logins' dieses Benutzers anzuzeigen</param>
    /// <param name="fields">eine Liste von Feldern, die als Spalten innerhalb des Gridviews verwendet/angezeigt werden sollen</param>
    public GridOptions(eGridStyle style, Type? master, string[]? fields)
    {
        Fields = fields;
        Style = style;
        MasterType = master;
    }

    /// <summary>
    /// erforderlich style
    /// </summary>
    public eGridStyle Style { get; init; }

    /// <summary>
    /// setzt den Master, für den die Gridview benötigt wird (z.B. typeof(User), um alle 'Logins' dieses Benutzers anzuzeigen
    /// </summary>
    public Type? MasterType { get; init; }

    /// <summary>
    /// eine Liste von Feldern, die als Spalten innerhalb des Gridviews verwendet/angezeigt werden sollen
    /// </summary>
    public string[]? Fields { get; init; }
}


