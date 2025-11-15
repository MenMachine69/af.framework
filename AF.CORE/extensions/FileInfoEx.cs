using System.Drawing;
using System.IO.Compression;
using System.Security.Cryptography;

namespace AF.CORE;

/// <summary>
/// Erweiterungsmethoden für die Klasse FileInfo
/// </summary>
public static class FileInfoEx
{
    private static readonly double divider_KB = Math.Pow(1024, 1);
    private static readonly double divider_MB = Math.Pow(1024, 2);
    private static readonly double divider_GB = Math.Pow(1024, 3);
    private static readonly double divider_TB = Math.Pow(1024, 4);

    /// <summary>
    /// Konvertiert eine Zahl, die eine Größe in Bytes darstellt, in die größtmögliche passende Einheit.
    /// </summary>
    /// <param name="size">Dateigröße</param>
    /// <returns>Formatierte Zeichenkette, die den Wert mit Einheit angibt</returns>
    public static string FormatedSize(this long size)
    {
        if (size >= divider_TB)
            return $"{size / divider_TB:N1} TB";
        if (size >= divider_GB)
            return $"{size / divider_GB:N0} GB";
        if (size >= divider_MB)
            return $"{size / divider_MB:N0} MB";
        if (size >= divider_KB)
            return $"{size / divider_KB:N0} KB";
        return $"{size:N0} B";
    }

    /// <summary>
    /// Größe der Datei, in die größtmögliche passende Einheit konvertieren.
    /// </summary>
    /// <param name="file">Datei</param>
    /// <returns>Formatierte Zeichenkette, die den Wert mit Einheit angibt</returns>
    public static string FormatedSize(this FileInfo file)
    {
        return FormatedSize(file.Length);
    }

    /// <summary>
    /// Liefert das einer Datei zugeordnete 'kleine' Icon
    /// Erweitert die Klasse FileInfo
    /// </summary>
    /// <param name="file">das FileInfo-Objekt</param>
    /// <returns>das Icon oder null</returns>
    public static Icon GetSmallIcon(this FileInfo file)
    {
        return _geticon.GetSmallIcon(file.FullName);
    }

    /// <summary>
    /// Liefert das einer Datei zugeordnete 'große' Icon
    /// Erweitert die Klasse FileInfo
    /// </summary>
    /// <param name="file">das FileInfo-Objekt</param>
    /// <returns>das Icon oder null</returns>
    public static Icon GetLargeIcon(this FileInfo file)
    {
        return _geticon.GetLargeIcon(file.FullName);
    }

    /// <summary>
    /// Liefert das einer Datei zugeordnete 'große' Icon
    /// Erweitert die Klasse FileInfo
    /// </summary>
    /// <param name="file">das FileInfo-Objekt</param>
    /// <returns>das Icon oder null</returns>
    public static Icon GetLargeIconFromExtension(this FileInfo file)
    {
        return _geticon.GetLargeIconFromExtension(file.FullName);
    }


    /// <summary>
    /// Liefert das einer Datei zugeordnete 'kleine' Icon für den Zustand 'geöffnet'
    /// Erweitert die Klasse FileInfo
    /// </summary>
    /// <param name="file">das FileInfo-Objekt</param>
    /// <returns>das Icon oder null</returns>
    public static Icon GetLargeOpenIcon(this FileInfo file)
    {
        return _geticon.GetLargeOpenIcon(file.FullName);
    }

    /// <summary>
    /// Liefert das einer Datei zugeordnete 'große' Icon für den Zustand 'geöffnet'
    /// Erweitert die Klasse FileInfo
    /// </summary>
    /// <param name="file">das FileInfo-Objekt</param>
    /// <returns>das Icon oder null</returns>
    public static Icon GetSmallOpenIcon(this FileInfo file)
    {
        return _geticon.GetSmallOpenIcon(file.FullName);

    }

    /// <summary>
    /// Berechnet einen MD5-Hash des Inhaltes der Datei
    /// 
    /// Dieser Hash kann für Vergleiche von Dateien verwendet werden
    /// </summary>
    /// <param name="file">Datei</param>
    /// <returns>Hash der Datei</returns>
    public static string GetMD5Hash(this FileInfo file)
    {
        string ret = "";

        if (file.Exists)
        {
            using MD5 md5 = MD5.Create();
            using FileStream stream = file.OpenRead();
            byte[] retVal = md5.ComputeHash(stream);
#if NET48_OR_GREATER
            ret = BitConverter.ToString(retVal).Replace("-", ""); // hex string
#else
            ret = Convert.ToHexString(retVal).Replace("-", ""); // hex string
#endif
        }

        return ret;
    }


    /// <summary>
    /// Liest ein Bild aus einer Datei, ohne die Datei selbst zu sperren
    /// </summary>
    /// <param name="file">Datei, die das Bild enthält</param>
    /// <returns>Bild oder null</returns>
    public static Image LoadImage(this FileInfo file)
    {
        if (file.Exists == false)
            throw new ArgumentException(string.Format(CoreStrings.ERR_FILE_NOTFOUND, file.FullName));

        using FileStream fstream = new (file.FullName, FileMode.Open, FileAccess.Read);
        var img = Image.FromStream(fstream);

        return img;
    }

    /// <summary>
    /// Datei asynchron löschen
    /// </summary>
    /// <param name="file">zu löschende Datei</param>
    /// <returns>Aufgabe, auf die Sie warten können</returns>
    public static Task DeleteAsync(this FileInfo file)
    {
        if (file.Exists == false)
            throw new ArgumentException(string.Format(CoreStrings.ERR_FILE_NOTFOUND, file));

        return Task.Run(file.Delete);
    }

    /// <summary>
    /// Eine Datei nicht blockierend kopieren.
    /// </summary>
    /// <param name="file">Quelle</param>
    /// <param name="target">Ziel</param>
    /// <param name="overwrite">Überschreiben, wenn vorhanden</param>
    /// <returns>Aufgabe, auf die Sie warten können</returns>
    public static void Copy(this FileInfo file, FileInfo target, bool overwrite = true)
    {
        if (file.Exists == false)
            throw new ArgumentException(string.Format(CoreStrings.ERR_FILE_NOTFOUND, file));

        if (target.Directory == null)
            throw new ArgumentException($"Empty dirtectory information in file {target.FullName}.");


        if (!target.Directory.Exists)
            target.Directory.Create();

        // Datei lesen mit Shared Access, um Sperren zu vermeiden
        using var sourceStream = new FileStream(
            file.FullName,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite | FileShare.Delete);
        using var destinationStream = new FileStream(
            target.FullName,
            (overwrite ? FileMode.Create : FileMode.CreateNew),
            FileAccess.Write,
            FileShare.None);
        sourceStream.CopyTo(destinationStream);
    }

    /// <summary>
    /// Liefert den Typ der Datei (LibraryDocument) anhand der Endung.
    /// </summary>
    /// <param name="file">Datei</param>
    /// <returns>Typ</returns>
    public static eLibraryDocumentType GetLibraryDocumentType(this FileInfo file)
    {
        string ext = file.Extension.ToUpper();

        if (ext == ".DOCX" || ext == ".DOC")
            return eLibraryDocumentType.Word;

        if (ext == ".XLSX" || ext == ".XLS")
            return eLibraryDocumentType.Excel;

        if (ext == ".PDF")
            return eLibraryDocumentType.PDF;

        if (ext == ".PNG")
            return eLibraryDocumentType.Png;

        if (ext == ".JPEG" || ext == ".JPG")
            return eLibraryDocumentType.Jpeg;

        if (ext == ".XML")
            return eLibraryDocumentType.XML;

        if (ext == ".JSON")
            return eLibraryDocumentType.Json;

        if (ext == ".TEXT" || ext == ".TXT" || ext == ".CSV")
            return eLibraryDocumentType.Text;

        if (ext == ".CSS")
            return eLibraryDocumentType.CSS;

        if (ext == ".HTML" || ext == ".HTM")
            return eLibraryDocumentType.HTML;

        if (ext == ".CS")
            return eLibraryDocumentType.CSharp;

        if (ext == ".SQL")
            return eLibraryDocumentType.SQL;

        if (ext == ".ZIP")
            return eLibraryDocumentType.Zip;

        return eLibraryDocumentType.Other;
    }

    /// <summary>
    /// Liefert eine Liste der Einträge in einer ZIP-Datei
    /// </summary>
    /// <param name="zipFile"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public static List<ZipFileInfo> GetZipEntries(this FileInfo zipFile)
    {
        var entries = new List<ZipFileInfo>();

        if (!zipFile.Exists || zipFile.Extension != ".zip")
            throw new FileNotFoundException("Zip file not found or invalid file type.");

        using var zipToOpen = zipFile.OpenRead();
        using var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read);
        
        foreach (var entry in archive.Entries)
        {
            if (!string.IsNullOrEmpty(entry.Name)) // Skip directories
            {
                entries.Add(new ZipFileInfo
                {
                    Name = entry.Name,
                    FullName = entry.FullName,
                    Length = entry.Length,
                    CompressedLength = entry.CompressedLength
                });
            }
        }

        return entries;
    }

    /// <summary>
    /// Liefert eine Liste der Einträge in einer ZIP-Datei, die als ByteArray vorliegt.
    /// </summary>
    /// <param name="zipData"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public static List<ZipFileInfo> GetZipEntries(this byte[] zipData)
    {
        var entries = new List<ZipFileInfo>();

        if (zipData.Length < 1)
            return [];

        using var zipToOpen = new MemoryStream(zipData);
        using var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read);

        foreach (var entry in archive.Entries)
        {
            if (!string.IsNullOrEmpty(entry.Name)) // Skip directories
            {
                entries.Add(new ZipFileInfo
                {
                    Name = entry.Name,
                    FullName = entry.FullName,
                    Length = entry.Length,
                    CompressedLength = entry.CompressedLength
                });
            }
        }

        return entries;
    }
}

/// <summary>
/// Informationen zu einer ZIP-Datei
/// </summary>
public sealed class ZipFileInfo
{
    /// <summary>
    /// Name der Datei/des Eintrags
    /// </summary>
    public string Name { get; set; } = "";
    /// <summary>
    /// Länge der Datei/des Eintrags
    /// </summary>
    public long Length { get; set; }
    /// <summary>
    /// vollst. Name der Datei/des Eintrags
    /// </summary>
    public string FullName { get; set; } = "";
    /// <summary>
    /// Komprimierte Länge des Eintrags
    /// </summary>
    public long CompressedLength { get; set; }
    /// <summary>
    /// Kompressionsfaktor
    /// </summary>
    public decimal CompressionFactor => Length <= 0L ? 0L : 1.0m - Convert.ToDecimal(CompressedLength) / Convert.ToDecimal(Length);


}


