namespace AF.CORE;

/// <summary>
/// Speicher für Objekte, die verzögert geladen werden sollen.
///
/// Sorgt bei Verwendung dafür, dass nicht mehrfach versucht wird, den Wert zu laden.
/// </summary>
/// <typeparam name="T"></typeparam>
public class LazyNullable<T> where T : class
{
    private T? current = null;
    private bool loaded = false;
    private readonly object locker = new();

    /// <summary>
    /// Delegate, der das Laden übernimmt.
    /// </summary>
    public Func<T?>? Loader { get; set; }

    /// <summary>
    /// Liefert das aktuelle Objekt, das bei Bedarf geladen wird.
    /// </summary>
    /// <returns></returns>
    public T? Get()
    {
        if (loaded) return current;

        lock (locker)
        {
            current = Loader?.Invoke();
            loaded = true;
        }

        return current;
    }

    /// <summary>
    /// Zurücksetzen auf nicht geladen.
    /// Der nächste Aufruf von Get lädt das Objekt neu.
    /// </summary>
    public void Reset()
    {
        lock (locker)
        {
            loaded = false;
            current = null;
        }
    }

    /// <summary>
    /// Gibt an, ob der Wert bereits geladen wurde.
    /// </summary>
    public bool IsLoaded => loaded;

    /// <summary>
    /// Zugriff auf den Wert (anstatt via Get)
    /// </summary>
    public T? Value => Get();
}

