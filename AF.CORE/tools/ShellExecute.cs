using System.Diagnostics;
using System.Text;

namespace AF.CORE;

/// <summary>
/// Klasse zum Ausführen von Shell-Befehlen oder Batchdateien.
/// 
/// Die AUsgabe der Befehle/Batchdateien wird im Ereignis 
/// OutputReceived zurückgegeben, dass abonniert werden kann.
/// </summary>
public sealed class ShellExecute : IDisposable
{
    /// <summary>
    /// Fehklerausgabe ebenfalls zum Ereignis umleiten
    /// </summary>
    public bool RedirectErrorOutput { get; set; } = true;

    /// <summary>
    /// Codepage für die Ausgabe
    /// </summary>
    public int CodePage { get; set; } = 850;

    /// <summary>
    /// Shell-Befehl oder Batch-Datei ausführen
    /// </summary>
    /// <param name="command">auszuführender Befehl oder Name der vollst. der Bachdatei</param>
    /// <param name="arguments">an den Befehl/die Batchdatei zu übergebende Parameter</param>
    /// <param name="startuppath">Startverzeichnis (i.d.R. das aktuelle oder das Verezcihnis der Batchdatei)</param>
    /// <param name="cancellationToken">Token, um den Prozess abbrechen zu können</param>
    public Task ExecuteAsync(string command, string? arguments = "", string? startuppath = null, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<object>();

        ProcessStartInfo ifo = new(command)
        {
            StandardOutputEncoding = Encoding.GetEncoding(CodePage),
            StandardErrorEncoding = Encoding.GetEncoding(CodePage),
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true
        };

        if (arguments != null)
            ifo.Arguments = arguments;

        if (command.EndsWith(".bat", StringComparison.InvariantCultureIgnoreCase) || 
            command.EndsWith(".cmd", StringComparison.InvariantCultureIgnoreCase))
        {
            FileInfo fi = new(command);

            if (fi.Exists == false)
                throw new FileNotFoundException($"The file {command} could not be found.");

            startuppath ??= fi.Directory?.FullName;
        }
        else
        {
            ifo.FileName = "cmd.exe";
            ifo.Arguments = $"/c {command} {arguments}";
        }

        if (startuppath != null)
            ifo.WorkingDirectory = startuppath;

        if (RedirectErrorOutput)
            ifo.RedirectStandardError = true;


        var process = new Process
        {
            StartInfo = ifo,
            EnableRaisingEvents = true
        };

        process.OutputDataReceived += (_, e) => OutputReceived?.Invoke(e.Data);
        
        if (RedirectErrorOutput)
            process.ErrorDataReceived += (_, e) => OutputReceived?.Invoke(e.Data);
        
        process.Exited += (sender, e) => { tcs.SetResult(new object()); ProcessExecuted?.Invoke(sender, e); };

        cancellationToken.Register(() =>
        {
            process.Kill();
            tcs.SetCanceled();
        });

        process.Start();
        process.BeginOutputReadLine();

        if (RedirectErrorOutput)
            process.BeginErrorReadLine();

        return tcs.Task;
    }

    /// <summary>
    /// Shell-Befehl oder Batch-Datei ausführen
    /// </summary>
    /// <param name="command">auszuführender Befehl oder Name der vollst. der Bachdatei</param>
    /// <param name="arguments">an den Befehl/die Batchdatei zu übergebende Parameter</param>
    /// <param name="startuppath">Startverzeichnis (i.d.R. das aktuelle oder das Verezcihnis der Batchdatei)</param>
    /// <param name="waitfor">max. Zeit, die auf die Ausführung gewartet werden soll (in ms), wenn NULL bis zum Ende warten</param>
    public void Execute(string command, string? arguments = null, string? startuppath = null, int? waitfor = null)
    {
        ProcessStartInfo ifo = new(command)
        {
            StandardOutputEncoding = Encoding.GetEncoding(CodePage),
            StandardErrorEncoding = Encoding.GetEncoding(CodePage),
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true
        };

        if (arguments != null)
            ifo.Arguments = arguments;

        if (command.EndsWith(".bat", StringComparison.InvariantCultureIgnoreCase) || 
            command.EndsWith(".cmd", StringComparison.InvariantCultureIgnoreCase))
        {
            FileInfo fi = new(command);

            if (fi.Exists == false)
                throw new FileNotFoundException($"The file {command} could not be found.");

            startuppath ??= fi.Directory?.FullName;
        }
        else
        {
            ifo.FileName = "cmd.exe";
            ifo.Arguments = $"/c {command} {arguments}";
        }

        if (startuppath != null)
            ifo.WorkingDirectory = startuppath;
        if (RedirectErrorOutput)
            ifo.RedirectStandardError = true;

        Process process = new() { StartInfo = ifo };
        process.OutputDataReceived += (_, e) => OutputReceived?.Invoke(e.Data);
    
        if (RedirectErrorOutput)
            process.ErrorDataReceived += (_, e) => OutputReceived?.Invoke(e.Data);

        process.Start();
        process.EnableRaisingEvents = true;
        process.BeginOutputReadLine();
    
        if (RedirectErrorOutput)
            process.BeginErrorReadLine();

        process.Exited += (sender, _) =>
        {
            ProcessExecuted?.Invoke(sender, null);
        };

        if (waitfor != null)
            process.WaitForExit((int)waitfor);
        else
            process.WaitForExit();
    }

    /// <summary>
    /// Ereignis, das bei Ausgabe von Standard- oder Fehlerausgabe ausgelöst wird.
    /// 
    /// Als Parameter wird die Ausgabe/der Text übergeben.
    /// </summary>
    public event Action<string?>? OutputReceived;

    /// <summary>
    /// Ereignis, das bei nach Beendigung des Prozesses ausgelöst wird.
    /// 
    /// Als Parameter wird die Ausgabe/der Text übergeben.
    /// </summary>
    public event EventHandler<EventArgs?>? ProcessExecuted;

    private bool disposed;

    private void Dispose(bool disposing)
    {
        if (disposed) return;

        if (disposing)
        {

        }

        OutputReceived = null;

        disposed = true;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Finalizer
    /// </summary>
    ~ShellExecute() // the finalizer
    {
        Dispose(false);
    }
}
