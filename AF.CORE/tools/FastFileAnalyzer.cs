using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace AF.CORE;

/// <summary>
/// Informationen über Dateien und Verzeichnisse mit hoher Geschwindigkeit auslesen
/// </summary>
public class FastFileAnyalzer
{
    private string _pattern = "";
    private string _patternRegex = "";

    /// <summary>
    /// Muster für die zu lesenden Dateien (z.B. pd??.* oder file*.txt etc.)
    /// 
    /// Die Syntax des Musters entspricht der Syntax in der Windows-Konsole (z.B. dir...)
    /// 
    /// Standard ist *.* (alle Dateien)
    /// </summary>
    [Localizable(false)]
    public string Pattern
    {
        get => _pattern;
        set
        {
            _pattern = value.ToLowerInvariant();
            _patternRegex = value.ToLowerInvariant().Replace(".",  "[.]").Replace(" ", "[ ]").Replace("*", ".*")
                .Replace("?", ".");
        }
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public FastFileAnyalzer()
    {
        Pattern = "*.*";
    }

    /// <summary>
    /// Delegat, der ausgeführt wird, wenn ein Verzeichnis gelesen wurde. 
    /// Informationen über dieses Verzeichnis werden an das Delgate übergeben.
    /// </summary>
    public Action<FastFileDirectoryInfo>? OnDirectoryFound { get; set; }

    /// <summary>
    /// Delegat, der ausgeführt wird, wenn eine Datei gelesen wurde. 
    /// Informationen über diese Datei werden an das Delgate übergeben.
    /// </summary>
    public Action<FastFileFileInfo>? OnFileFound { get; set; }

    /// <summary>
    /// Beginnt mit dem Lesen der Dateien und Verzeichnisse
    /// </summary>
    /// <param name="directory">Directory to be searched</param>
    /// <param name="subdirectorys">Unterverzeichnisse einbeziehen</param>
    /// <returns>Informationen über die im Verzeichnis identifizierten Dateien und Unterverzeichnisse</returns>
    [Localizable(false)]
    public FastFileDirectoryInfo Scan(string directory, bool subdirectorys)
    {

        // Initialize a variable to store the total size of all files
        FastFileDirectoryInfo folderInfo = new(directory);

        // Use the FindFirstFileEx and FindNextFile functions to enumerate the files and folders in the MFT
        IntPtr handle = FindFirstFileEx(Path.Combine(directory, "*.*"), FINDEX_INFO_LEVELS.FindExInfoStandard,
            out WIN32_FIND_DATA data, FINDEX_SEARCH_OPS.FindExSearchNameMatch, IntPtr.Zero,
            FIND_FIRST_EX_FLAGS.FindExLargeFetch);

        if (handle == IntPtr.Zero) return folderInfo;

        do
        {

            // Marshal the WIN32_FIND_DATA structure from unmanaged memory to a managed object
            //data = (WIN32_FIND_DATA)Marshal.PtrToStructure(handle, typeof(WIN32_FIND_DATA));

            // Add the size of each file to the total size
            if ((data.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) == 0)
            {
                //Console.WriteLine(data.cFileName);
                unchecked
                {
                    ulong size = (data.nFileSizeLow + ((ulong)data.nFileSizeHigh << 32));

                    folderInfo.FolderSize += size;
                    folderInfo.TotalSize += size;
                    folderInfo.FileCount += 1;
                    folderInfo.TotalFileCount += 1;

                    if (OnFileFound == null) continue;

                    if (_pattern != "" &&
                        _pattern != "*.*" &&
                        !Regex.IsMatch(data.cFileName.ToLowerInvariant(), "^" + _patternRegex + "$")) continue;

                    FastFileFileInfo fileinfo = new (directory, data.cFileName, size);
                    OnFileFound(fileinfo);
                }
            }
            // Recursively search each subfolder
            else if ((data.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) != 0 && !data.cFileName.Equals(".", StringComparison.Ordinal) &&
                     !data.cFileName.Equals("..", StringComparison.Ordinal) && !data.cFileName.Equals("", StringComparison.Ordinal))
            {
                if (!subdirectorys) continue;

                var finfo = Scan(Path.Combine(directory, data.cFileName), subdirectorys);

                folderInfo.TotalSize += finfo.TotalSize;
                folderInfo.TotalFileCount += finfo.TotalFileCount;
            }
        } while (FindNextFile(handle, ref data));

        FindClose(handle);
        OnDirectoryFound?.Invoke(folderInfo);

        return folderInfo;
    }

    #region WinAPI

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct WIN32_FIND_DATA
    {
        public uint dwFileAttributes;
        public FILETIME ftCreationTime;
        public FILETIME ftLastAccessTime;
        public FILETIME ftLastWriteTime;
        public uint nFileSizeHigh;
        public uint nFileSizeLow;
        public uint dwReserved0;
        public uint dwReserved1;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string cFileName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
        public string cAlternateFileName;

        public uint dwFileType;
        public uint dwCreatorType;
        public uint wFinderFlags;
    }

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr FindFirstFileEx(string lpFileName, FINDEX_INFO_LEVELS fInfoLevelId,
        out WIN32_FIND_DATA lpFindFileData, FINDEX_SEARCH_OPS fSearchOp, IntPtr lpSearchFilter,
        FIND_FIRST_EX_FLAGS dwAdditionalFlags);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool FindNextFile(IntPtr hFindFile, ref WIN32_FIND_DATA lpFindFileData);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool FindClose(IntPtr hFindFile);

    // Declare the necessary constants and structures for the FindFirstFileEx and FindNextFile functions
    private const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
    private const uint FIND_FIRST_EX_LARGE_FETCH = 2;
    private IntPtr INVALID_HANDLE_VALUE = new(-1);

    private enum FINDEX_INFO_LEVELS
    {
        FindExInfoStandard,
        FindExInfoBasic,
        FindExInfoMaxInfoLevel
    }

    private enum FINDEX_SEARCH_OPS
    {
        FindExSearchNameMatch,
        FindExSearchLimitToDirectories,
        FindExSearchLimitToDevices
    }

    private enum FIND_FIRST_EX_FLAGS
    {
        FindExCaseSensitive = 1,
        FindExLargeFetch = 2
    }

    #endregion
}

/// <summary>
/// Informationen über ein Verzeichnis
/// </summary>
public struct FastFileDirectoryInfo
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="foldername">Directory name</param>
    public FastFileDirectoryInfo(string foldername)
    {
        FolderName = foldername;
        FolderSize = 0;
        TotalSize = 0;
        FileCount = 0;
        TotalFileCount = 0;
    }

    /// <summary>
    /// Directory name
    /// </summary>
    public string FolderName { get; init; }

    /// <summary>
    /// Größe der im Ordner enthaltenen Dateien (in Bytes)
    /// </summary>
    public ulong FolderSize { get; set; }


    /// <summary>
    /// Größe der im Ordner und allen Unterordnern enthaltenen Dateien (in Bytes)
    /// </summary>
    public ulong TotalSize { get; set; }

    /// <summary>
    /// Anzahl der im Ordner enthaltenen Dateien
    /// </summary>
    public ulong FileCount { get; set; }

    /// <summary>
    /// Anzahl der im Ordner und allen Unterordnern enthaltenen Dateien
    /// </summary>
    public ulong TotalFileCount { get; set; }
}

/// <summary>
/// Informationen zu einer Datei
/// </summary>
public struct FastFileFileInfo
{
    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="foldername">Verzeichnisname</param>
    /// <param name="fileName">>Dateiname</param>
    /// <param name="fileSize">Dateigröße (in Bytes)</param>
    public FastFileFileInfo(string foldername, string fileName, ulong fileSize)
    {
        FileName = fileName;
        FileSize = fileSize;
        FolderName = foldername;
    }

    /// <summary>
    /// Name des Ordners, der die Datei enthält
    /// </summary>
    public string FolderName { get; init; }

    /// <summary>
    /// Dateiname
    /// </summary>
    public string FileName { get; init; }

    /// <summary>
    /// Größe der Datei (in Bytes)
    /// </summary>
    public ulong FileSize { get; init; }
}


