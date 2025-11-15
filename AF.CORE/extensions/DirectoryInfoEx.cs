using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace AF.CORE;

/// <summary>
/// Erweiterungen der DirectoryInfo-Klasse
/// </summary>
public static class DirectoryInfoEx
{
    /// <summary>
    /// Gibt das 'kleine' Symbol zurück, das einer Datei zugeordnet ist.
    /// Erweitert die Klasse DirectoryInfo
    /// </summary>
    /// <param name="dir">das DirectoryInfo-Objekt</param>
    /// <returns>das Symbol oder null</returns>
    public static Icon GetSmallIcon(this DirectoryInfo dir)
    {
        return _geticon.GetSmallIcon(dir.FullName);
    }

    /// <summary>
    /// Gibt das 'große' Symbol zurück, das einer Datei zugeordnet ist.
    /// Erweitert die Klasse DirectoryInfo
    /// </summary>
    /// <param name="dir">das DirectoryInfo-Objekt</param>
    /// <returns>das Symbol oder null</returns>
    public static Icon GetLargeIcon(this DirectoryInfo dir)
    {
        return _geticon.GetLargeIcon(dir.FullName);
    }

    /// <summary>
    /// Gibt das 'kleine' Symbol zurück, das mit einer Datei im Zustand 'offen' verbunden ist.
    /// Erweitert die Klasse DirectoryInfo
    /// </summary>
    /// <param name="dir">das DirectoryInfo-Objekt</param>
    /// <returns>das Symbol oder null</returns>
    public static Icon GetLargeOpenIcon(this DirectoryInfo dir)
    {
        return _geticon.GetLargeOpenIcon(dir.FullName);
    }

    /// <summary>
    /// Gibt das "große" Symbol zurück, das einer Datei im Zustand "offen" zugeordnet ist.
    /// Erweitert die Klasse DirectoryInfo
    /// </summary>
    /// <param name="dir">das DirectoryInfo-Objekt</param>
    /// <returns>das Symbol oder null</returns>
    public static Icon GetSmallOpenIcon(this DirectoryInfo dir)
    {
        return _geticon.GetSmallOpenIcon(dir.FullName);

    }

    /// <summary>
    /// Prüft, ob ein Pfad existiert und erstellt ihn ggf.
    /// </summary>
    /// <param name="dir">DirectoryInfo des zu prüfenden/erstellenden Pfades</param>
    /// <param name="allowcreate">true, kann erstellt werden, wenn er nicht existiert</param>
    /// <returns>true wenn der Pfad existiert oder erstellt werden konnte</returns>
    public static bool Check(this DirectoryInfo dir, bool allowcreate)
    {
        bool ret = dir.Exists;

        if (ret || !allowcreate) return ret;

        dir.Create();
        ret = true;

        return ret;
    }

    /// <summary>
    /// Ermittelt die Seriennummer eines Laufwerks als Hex-String
    /// </summary>
    /// <param name="dir">DirectoryInfo, deren VolumeSerial ermittelt werden soll</param>
    /// <returns>Seriennummer als Hexstring (0x wenn ein Fehler aufgetreten ist)</returns>
    public static string VolumeSerial(this DirectoryInfo dir)
    {
        uint serNum = 0;
        uint maxCompLen = 0;
        string ret;
        if (dir.FullName.Substring(1, 1) == @":")
        {
            StringBuilder VolLabel = new(256); // Label
            uint VolFlags = 0;
            StringBuilder FSName = new(256); // File System Name
            string drive = dir.FullName.Left(1) + @":\\";
            Win32Invokes.GetVolumeInformation(drive, VolLabel, (uint)VolLabel.Capacity, ref serNum, ref maxCompLen,
                ref VolFlags, FSName, (uint)FSName.Capacity);
            ret = Convert.ToString(serNum, 16);
        }
        else
            throw new ArgumentException(string.Format(CoreStrings.ERR_NOREALDRIVE, dir.FullName));

        return ret;
    }
}

internal static class _geticon
{
    /// <summary>
    /// Liefert das einer Datei zugeordnete 'kleine' Icon
    /// </summary>
    /// <param name="path">vollst. Name der Datei oder des Verzeichnisses</param>
    /// <returns>das Icon oder null</returns>
    public static Icon GetSmallIcon(string path)
    {
        Win32Structs.SHFILEINFO info = new();
        Win32Invokes.SHGetFileInfo(path, 0, ref info, (uint)Marshal.SizeOf(info),
            (uint)(Win32Enums.SHGFI.SHGFI_ICON | Win32Enums.SHGFI.SHGFI_TYPENAME | Win32Enums.SHGFI.SHGFI_SMALLICON));

        return Icon.FromHandle(info.hIcon);
    }

    /// <summary>
    /// Liefert das einer Datei zugeordnete 'große' Icon
    /// </summary>
    /// <param name="path">vollst. Name der Datei oder des Verzeichnisses</param>
    /// <returns>das Icon oder null</returns>
    public static Icon GetLargeIcon(string path)
    {
        Win32Structs.SHFILEINFO info = new();
        Win32Invokes.SHGetFileInfo(path, 0, ref info, (uint)Marshal.SizeOf(info),
            (uint)(Win32Enums.SHGFI.SHGFI_ICON | Win32Enums.SHGFI.SHGFI_TYPENAME));

        return Icon.FromHandle(info.hIcon);
    }

    /// <summary>
    /// Liefert das einer Datei zugeordnete 'große' Icon
    /// </summary>
    /// <param name="path">vollst. Name der Datei oder des Verzeichnisses</param>
    /// <returns>das Icon oder null</returns>
    public static Icon GetLargeIconFromExtension(string path)
    {
        path = path[path.LastIndexOf('.')..];

        Win32Structs.SHFILEINFO info = new();
        Win32Invokes.SHGetFileInfo(path, 0, ref info, (uint)Marshal.SizeOf(info),
            (uint)(Win32Enums.SHGFI.SHGFI_ICON | Win32Enums.SHGFI.SHGFI_TYPENAME));

        return Icon.FromHandle(info.hIcon);
    }

    /// <summary>
    /// Liefert das einer Datei zugeordnete 'kleine' Icon für den Zustand 'geöffnet'
    /// </summary>
    /// <param name="path">vollst. Name der Datei oder des Verzeichnisses</param>
    /// <returns>das Icon oder null</returns>
    public static Icon GetLargeOpenIcon(string path)
    {
        Win32Structs.SHFILEINFO info = new();
        Win32Invokes.SHGetFileInfo(path, 0, ref info, (uint)Marshal.SizeOf(info),
            (uint)(Win32Enums.SHGFI.SHGFI_ICON | Win32Enums.SHGFI.SHGFI_TYPENAME | Win32Enums.SHGFI.SHGFI_OPENICON));

        return Icon.FromHandle(info.hIcon);
    }

    /// <summary>
    /// Liefert das einer Datei zugeordnete 'große' Icon für den Zustand 'geöffnet'
    /// </summary>
    /// <param name="path">vollst. Name der Datei oder des Verzeichnisses</param>
    /// <returns>das Icon oder null</returns>
    public static Icon GetSmallOpenIcon(string path)
    {
        Win32Structs.SHFILEINFO info = new();
        Win32Invokes.SHGetFileInfo(path, 0, ref info, (uint)Marshal.SizeOf(info),
            (uint)(Win32Enums.SHGFI.SHGFI_ICON | Win32Enums.SHGFI.SHGFI_TYPENAME | Win32Enums.SHGFI.SHGFI_SMALLICON |
                   Win32Enums.SHGFI.SHGFI_OPENICON));

        return Icon.FromHandle(info.hIcon);
    }
}
