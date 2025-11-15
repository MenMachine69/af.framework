namespace AF.CORE;

/// <summary>
/// Schnittstelle zur Beschreibung eines Menüeintrags
/// </summary>
public interface IMenuEntry
{
    /// <summary>
    /// beginnt eine Gruppe - zeichnet ein Trennzeichen vor diesem Eintrag
    /// </summary>
    bool BeginGroup { get; }

    /// <summary>
    /// gibt an, ob es ein Element mit zwei Stati ist, der durch CLick umgeschaltet wird.
    /// </summary>
    bool Toggle { get; set; }

    /// <summary>
    /// Beschriftung für den Eintrag - verwenden Sie / innerhalb der Beschriftung, um Untermenüs wie FILE / OPEN zu erstellen
    /// </summary>
    string Caption { get; }

    /// <summary>
    /// eindeutiger Name für diesen Eintrag
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Beschreibung für den Eintrag (wird in Tooltips angezeigt)
    /// </summary>
    string? Description { get; }

    /// <summary>
    /// ein Hinweis für diesen Eintrag (wird in Tooltips angezeigt)
    /// </summary>
    string? Hint { get; }

    /// <summary>
    /// Hotkey, der für diesen Eintrag verwendet werden kann
    /// </summary>
    eKeys HotKey { get; }

    /// <summary>
    /// Symbol, dass vor dem Menüeintrag angezeigt werden soll
    /// </summary>
    object? Image { get; }

    /// <summary>
    /// Symbol, dass vor dem Menüeintrag angezeigt werden soll, wenn es sich um ein SubMenu handelt.
    /// 
    /// Das Symbol muss nur dem ersten Menüeintrag des SubMenus zugewiesen werden (Position).
    /// </summary>
    object? GroupImage { get; }

    /// <summary>
    /// Index des anzuzeigenden Bildes, wenn eine Bilderliste für das Menü verwendet wird
    /// </summary>
    int ImageIndex { get; }

    /// <summary>
    /// Speicherplatz für alle Projekte, die mit diesem Eintrag verknüpft sind
    /// </summary>
    object? Tag { get; }

    /// <summary>
    /// Kontexte, in denen der Eintrag sichtbar ist
    /// </summary>
    public eCommandContext CommandContext { get; }

    /// <summary>
    /// Befehlstyp
    /// </summary>
    public eCommand CommandType { get; }
}