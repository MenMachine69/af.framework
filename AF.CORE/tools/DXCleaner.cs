using System.Diagnostics;

namespace AF.CORE;

/// <summary>
/// Bereinigt die DexExpress-Dateien in einem Verzeichnis (nur letzte Version behalten)
/// </summary>
public class DXCleaner
{
    private readonly DirectoryInfo _dir;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dir">Directory to be cleaned</param>
    public DXCleaner(DirectoryInfo dir)
    {
        if (dir.Exists == false)
            throw new ArgumentException(string.Format(CoreStrings.ERR_PATHNOTEXIST, dir.FullName));

        _dir = dir;
    }

    /// <summary>
    /// Analysiert das Verzeichnis und gibt eine Liste der zu entfernenden Dateien zurück
    /// </summary>
    /// <returns>zu bereinigende Dateien</returns>
    public async Task<FileInfo[]> Analyze()
    {
        Dictionary<string, FileInfo> tempDic = new();

        int major = 0;
        int minor = 0;

        await Task.Run(() =>
        {
            foreach (FileInfo file in _dir.GetFiles(@"DevExpress*.*"))
            {
                FileVersionInfo finfo = FileVersionInfo.GetVersionInfo(file.FullName);

                if (finfo.FileMajorPart > major)
                {
                    major = finfo.FileMajorPart;
                    minor = finfo.FileMinorPart;
                }
                else if (finfo.FileMajorPart == major)
                    minor = finfo.FileMinorPart > minor ? finfo.FileMinorPart : minor;
            }
        });

        string toCheck = $@"v{major}.{minor}.";

        await Task.Run(() =>
        {
            foreach (FileInfo file in _dir.GetFiles(@"DevExpress*.*"))
            {
                if (file.FullName.Contains(toCheck) == false)
                    tempDic.TryAdd(file.FullName, file);
            }
        });

        major = 0;
        minor = 0;

        await Task.Run(() =>
        {
            foreach (FileInfo file in _dir.GetFiles(@"Alternet.*.*"))
            {
                FileVersionInfo finfo = FileVersionInfo.GetVersionInfo(file.FullName);

                if (finfo.FileMajorPart > major)
                {
                    major = finfo.FileMajorPart;
                    minor = finfo.FileMinorPart;
                }
                else if (finfo.FileMajorPart == major)
                    minor = finfo.FileMinorPart > minor ? finfo.FileMinorPart : minor;
            }
        });

        await Task.Run(() =>
        {
            foreach (FileInfo file in _dir.GetFiles(@"Alternet.*.*"))
            {
                FileVersionInfo finfo = FileVersionInfo.GetVersionInfo(file.FullName);

                if (finfo.FileMajorPart < major)
                {
                    tempDic.Add(file.FullName, file);
                    continue;
                }

                if (finfo.FileMajorPart == major && finfo.FileMinorPart < minor)
                    tempDic.Add(file.FullName, file);
            }
        });

        return tempDic.Values.ToArray();
    }

    /// <summary>
    /// Aufräumen ausführen
    /// </summary>
    /// <param name="files">Liste der zu löschenden Dateien</param>
    /// <returns>Protokoll mit Fehlern (Dateien, die nicht gelöscht werden konnten)</returns>
    public async Task<Log> Clear(FileInfo[] files)
    {
        Log ret = new();

        foreach (FileInfo file in files)
        {
            if (file.Exists)
            {
                try
                {
                    await file.DeleteAsync();
                }
                catch (Exception ex)
                {
                    ret.AddError($@"{file.FullName} ({ex.Message})");
                }
            }
        }

        return ret;
    }
}

