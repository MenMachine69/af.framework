namespace AF.DATA;

/// <summary>
/// Verknüpfung zwischen zwei Elementen
/// </summary>
public interface IJoin
{
    /// <summary>
    /// ID des Elements, von dem die Verknüpfung ausgeht
    /// </summary>
    public Guid ElementSource { get; }

    /// <summary>
    /// ID des Elements, zu dem die Verknüpfung führt
    /// </summary>
    public Guid ElementTarget { get; }

    /// <summary>
    /// Beschriftung der Verbindung
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// Überprüft, ob ein identisches Join bereits in der übergebenen Liste existiert.
    /// 
    /// Kann verwendet werden um zu verhindern, dass ein identisches Join erneut hinzugefügt wird.
    /// 
    /// Kann überschrieben werden, um eine eigene Prüfung auf Eindeutigkeit zu implementieren (wenn z.B. 
    /// mehrere Joins pro Element verwendet werden sollen).
    /// </summary>
    /// <param name="joins">vorhandene Joins</param>
    /// <returns>true, wenn ein identisches Join existiert, sonst false.</returns>
    public bool Exist(IEnumerable<IJoin> joins);
}