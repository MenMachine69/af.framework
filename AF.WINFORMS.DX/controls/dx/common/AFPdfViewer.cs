using DevExpress.Utils;
using DevExpress.XtraPdfViewer;

namespace AF.WINFORMS.DX;

/// <inheritdoc/>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
public class AFPdfViewer : PdfViewer
{
    /// <summary>
    /// Dokument aus Byte-Array laden
    /// </summary>
    /// <param name="data"></param>
    public void LoadDocument(byte[] data)
    {
        CloseDocument();

        if (data.Length < 1) return;

        DetachStreamAfterLoadComplete = true;

        using MemoryStream stream = new();
        stream.Write(data, 0, data.Length);
        stream.Flush();
        stream.Seek(0, SeekOrigin.Begin);
        LoadDocument(stream);
    }

    /// <summary>
    /// Dokument in ein ByteArray sichern.
    /// </summary>
    /// <returns></returns>
    public byte[] SaveDocumentAsBytes()
    {
        using MemoryStream stream = new();
        SaveDocument(stream);
        stream.Flush();
        stream.Seek(0, SeekOrigin.Begin);

        return stream.GetBuffer();
    }
}