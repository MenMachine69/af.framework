using System.Collections.Concurrent;
using System.Text;

namespace AF.CORE;

/// <summary>
/// Universeller StringBuilderPool.
/// </summary>
public static class StringBuilderPool
{
    private static readonly ConcurrentQueue<StringBuilder> pool = new();

    /// <summary>
    /// Einen StringBuilder aus dem Pool erwerben.
    /// </summary>
    /// <param name="size">Größe des StringBuilders (default = 256)</param>
    /// <param name="content">initialer Inhalt des StringBuilders (default = null)</param>
    /// <returns>ein passender StringBuilder</returns>
    public static StringBuilder GetStringBuilder(int size = 256, string? content = null)
    {
        if (pool.TryDequeue(out var sb))
        {
            sb.Clear();

            if (content != null)
                sb.Append(content);

            ++StatHits;

            return sb;
        }

        return content == null ? new StringBuilder(size) : new StringBuilder(content);
    }

    /// <summary>
    /// Den StringBuilder freigeben
    /// </summary>
    /// <param name="sb">freizugebender StringBuilder</param>
    public static void ReturnStringBuilder(StringBuilder sb)
    {
        if (sb.Capacity <= 1024) // Prevent memory bloat
            pool.Enqueue(sb);
        else
            StatRejected += 1;
    }

    /// <summary>
    /// Statistik zurücksetzen.
    /// </summary>
    public static void ResetStat()
    {
        StatHits = 0;
        StatRejected = 0;
    }

    /// <summary>
    /// Anzahl der aus der Queue entnommenen StringBuilder
    /// </summary>
    public static long StatHits { get; private set; } = 0;

    /// <summary>
    /// Anzahl StringBuilder, die vom Pool aufgrund der Größe abgelehnt wurden.
    /// </summary>
    public static long StatRejected { get; private set; } = 0;

    /// <summary>
    /// Anzahl der StringBuilder in der Queue
    /// </summary>
    public static long StatCurrent => pool.Count;

}