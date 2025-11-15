using DevExpress.Utils;
using DevExpress.Utils.Layout;
using DevExpress.Utils.Svg;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Editor für Bilder mit einer Toolbar und rudimentären Bearbeitungsfunktionen.
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[DefaultBindingProperty("Image")]
[ToolboxTabName("AF Common")]
[DesignerCategory("Code")]
public class AFEditPicture : AFUserControl
{
    private readonly AFPictureBox pictureBox = null!;
    private readonly AFBarManager manager = null!;
    private readonly AFBarController barController = null!;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFEditPicture()
    {
        if (UI.DesignMode) return;

        barController = new();
        barController.AutoBackColorInBars = true;

        manager = new();
        manager.Form = this;
        manager.Controller = barController;

        manager.BeginInit();

        pictureBox = new() { Dock = DockStyle.Fill, BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder, AllowSelectArea = true };
        pictureBox.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
        pictureBox.PopupMenuShowing += (_, a) => { a.Cancel = true; };

        AFTablePanel table = new AFTablePanel() { Dock = DockStyle.Fill, AutoSize = true, UseSkinIndents = false };
        Controls.Add(table);

        table.BeginLayout();
        var tbar = table.AddBar(manager, 1, 1);
        addButton(tbar, "btnTake", UI.GetImage(Symbol.Camera), "Bild von Kamera", "Ein Bild mit Hilfe der WebCam des PCs aufnehmen.");
        addButton(tbar, "btnRotateLeft", UI.GetImage(Symbol.RotateLeft), "Nach links drehen", "Das Bild nach links drehen.", true);
        addButton(tbar, "btnRotateRight", UI.GetImage(Symbol.RotateRight), "Nach rechts drehen", "Das Bild nach rechts drehen");
        addButton(tbar, "btnCut", UI.GetImage(Symbol.ResizeImage), "Bild zuschneiden", "Das Bild zuschneiden.");
        addButton(tbar, "btnImport", UI.GetImage(Symbol.OpenFolder), "Bild aus Datei importieren", "Das Bild aus einer Datei importieren.", true);
        addButton(tbar, "btnExport", UI.GetImage(Symbol.Save), "Bild in Datei exportieren", "Das aktuelle Bild in eine Datei exportieren.");
        addButton(tbar, "btnClear", UI.GetImage(Symbol.Delete), "Bild löschen", "Das aktuelle Bild löschen.");
        addButton(tbar, "btnInfo", UI.GetImage(Symbol.Info), "Bildinformationen", "Informationen zum aktuellen Bild anzeigen (Abmessungen etc.).");


        table.Add(pictureBox, 2, 1);

        table.SetColumn(1, TablePanelEntityStyle.Relative, 1.0f);
        table.SetRow(1, TablePanelEntityStyle.Absolute, ScaleUtils.ScaleValue(32));
        table.SetRow(2, TablePanelEntityStyle.Relative, 1.0f);

        table.EndLayout();
        manager.EndInit();
    }

    /// <summary>
    /// Zugriff auf das Bild.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
    public Image Image { get => pictureBox.Image; set => pictureBox.Image = value; }

    private void addButton(Bar tbar, string name, SvgImage img, string caption, string description, bool group = false)
    {
        var btn = tbar.AddButton(name, img: img, begingroup: group);
        btn.ItemClick += btnclick;
        btn.SuperTip = UI.GetSuperTip(caption, description);
    }

    private void btnclick(object sender, ItemClickEventArgs e)
    {
        switch (e.Item.Name)
        {
            case "btnTake":
                takePicture();
                break;
            case "btnRotateLeft":
                rotateLeft();
                break;
            case "btnRotateRight":
                rotateRight();
                break;
            case "btnCut":
                zuschnitt();
                break;
            case "btnImport":
                import();
                break;
            case "btnExport":
                export();
                break;
            case "btnClear":
                clear();
                break;
            case "btnInfo":
                showInfo();
                break;
        }
    }

    private void rotateLeft()
    {
        pictureBox.Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
        pictureBox.Refresh();
    }

    private void rotateRight()
    {
        pictureBox.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
        pictureBox.Refresh();
    }

    private void zuschnitt()
    {
        pictureBox.SwitchToSelection();
        pictureBox.Refresh();
    }

    private void import()
    {
        using XtraOpenFileDialog dlg = new();

        dlg.Filter = "JPEG-Dateien|*.jpg|PNG-Dateien|*.png|GIF-Dateien|*.gif|BITMAP-Dateien|*.bmp|Alle Dateien|*.*";
        dlg.FilterIndex = 0;
        dlg.AddExtension = true;
        if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
        {
            try
            {
                IntPtr? bmp = (ImageEx.FromFile(dlg.FileName) as Bitmap)?.GetHbitmap();
                if (bmp is { } ptr)
                {
                    pictureBox.Image = Image.FromHbitmap(ptr);
                    Win32Invokes.DeleteObject(ptr);
                }
            }
            catch (Exception ex)
            {
                pictureBox.Image = null;
                MsgBox.ShowErrorOk("BILD IMPORTIEREN\r\nBeim Importieren der Bilddatei trat ein Fehler auf.", ex.Message);
            }
        }
    }

    private void export()
    {
        using XtraSaveFileDialog dlg = new();
        
        dlg.Filter = "JPEG-Dateien|*.jpg|PNG-Dateien|*.png|GIF-Dateien|*.gif|BITMAP-Dateien|*.bmp|Alle Dateien|*.*";
        dlg.FilterIndex = 0;
        dlg.AddExtension = true;
        if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
        {
            try
            {
                if (dlg.FileName.ToUpper().Trim().EndsWith("PNG"))
                    pictureBox.Image.SaveAsPng(dlg.FileName);
                else if (dlg.FileName.ToUpper().Trim().EndsWith("JPG"))
                    pictureBox.Image.SaveAsJpeg(dlg.FileName);
                else if (dlg.FileName.ToUpper().Trim().EndsWith("JPEG"))
                    pictureBox.Image.SaveAsJpeg(dlg.FileName);
                else if (dlg.FileName.ToUpper().Trim().EndsWith("BMP"))
                    pictureBox.Image.SaveAsBmp(dlg.FileName);
                else if (dlg.FileName.ToUpper().Trim().EndsWith("GIF"))
                    pictureBox.Image.SaveAsGif(dlg.FileName);
                else
                    throw new("Das Format der Datei konnte nicht ermittelt werden (unbekannte Dateiendung - zulässig sind PNG, JPG, JPEG, BMP und GIF)");
            }
            catch (Exception ex)
            {
                MsgBox.ShowErrorOk("BILD EXPORTIEREN\r\nBeim Exportieren der Bilddatei trat ein Fehler auf.", ex.Message);
            }
        }
    }

    private void clear()
    {
        if (MsgBox.ShowQuestionYesNo("BILD LÖSCHEN\r\nMöchten Sie das Bild löschen?") == eMessageBoxResult.Yes)
            pictureBox.Image = null;
    }

    private void showInfo()
    {
        MsgBox.ShowInfoOk("INFORMATIONEN ZUM BILD\r\nDas momentan ausgewählte Bild hat folgende Abmessungen:\r\n\r\n     Breite: {0} px\r\n     Höhe : {1} px\r\n\r\nDie Auflösung beträgt {2} dpi.".DisplayWith(pictureBox.Image.Width, pictureBox.Image.Height, pictureBox.Image.HorizontalResolution));
    }


    private void takePicture()
    {
        using var dlg = new FormImageFromCamera();

        if (dlg.ShowDialog(FindForm()) == DialogResult.OK && dlg.Picture != null)
        {
            pictureBox.Image = dlg.Picture;
        }
    }

    /// <inheritdoc />
    protected override void OnHandleDestroyed(EventArgs e)
    {
        base.OnHandleDestroyed(e);

        manager?.Dispose();
        barController?.Dispose();
    }
}

