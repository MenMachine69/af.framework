using System.Drawing.Imaging;
using DevExpress.Utils;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Picturebox, die ein Bild als Kreis darstellt.
/// 
/// Eigenet sich z.B. zur Anzeige von Benutzern.
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
public sealed class AFPictureBoxRounded : AFPictureBox
{
    /// <summary>
    /// Constrcutor
    /// </summary>
    public AFPictureBoxRounded()
    {
        Properties.OptionsMask.MaskType = DevExpress.XtraEditors.Controls.PictureEditMaskType.Circle;
        Properties.ShowMenu = false;
        BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        BackColor = Color.Transparent;
        Properties.NullText = "";
        Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
        TabStop = false;
        Properties.AllowFocused = false;
        Margin = new(5);
    }
}

/// <inheritdoc/>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
public class AFPictureBox : PictureEdit
{
    private Point RectStartPoint;
    private Rectangle SelectedRect;

    /// <summary>
    /// Gibt an, ob die Bereichsauswahl erlaubt ist
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AllowSelectArea { get; set; } = false;

    /// <summary>
    /// Destructor
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (UI.DesignMode) return;

        if (SelectedArea != null)
            SelectedArea.Dispose();

        base.Dispose(disposing);
    }

    /// <summary>
    /// Ereignis, das EIntritt, wenn das Control gezeichnet werden soll
    /// </summary>
    /// <param name="e">Ereignisparameter</param>
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (UI.DesignMode) return;

        // Draw the rectangle...
        if (AllowSelectArea && Image != null && SelectedRect.Width > 0 && SelectedRect.Height > 0)
        {
            using (Brush selectionBrush = new SolidBrush(Color.FromArgb(128, 72, 145, 220)))
                e.Graphics.FillRectangle(selectionBrush, SelectedRect);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        if (UI.DesignMode) return;

        if (AllowSelectArea && e.Button == MouseButtons.Left)
        {
            RectStartPoint = e.Location;
            Invalidate();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);

        if (UI.DesignMode) return;


        if (AllowSelectArea && e.Button == MouseButtons.Left && SelectedRect.Width > 0 && SelectedRect.Height > 0)
        {
            Point topleft = ViewportToImage(new(SelectedRect.X, SelectedRect.Y));
            topleft = new(Math.Max(0, topleft.X), Math.Max(0, topleft.Y));
            Point bottomright = ViewportToImage(new(SelectedRect.Right, SelectedRect.Bottom));
            bottomright = new(Math.Max(0, bottomright.X), Math.Max(0, bottomright.Y));

            Bitmap src = new(Image);

            int width = bottomright.X - topleft.X;
            if (topleft.X + width > src.Width)
                width = src.Width - topleft.X;

            int height = bottomright.Y - topleft.Y;
            if (topleft.Y + height > src.Height)
                height = src.Height - topleft.Y;

            if (width > 0 && height > 0)
                SelectedArea = src.Clone(new(topleft.X, topleft.Y, width, height), PixelFormat.Format32bppArgb);
        }
    }

    /// <summary>
    /// Auswahl als neues Bild setzen...
    /// </summary>
    public void SwitchToSelection()
    {
        if (SelectedArea != null && SelectedArea.Width > 0 && SelectedArea.Height > 0)
            Image = SelectedArea;

        SelectedArea = null;
        SelectedRect = new();

        Invalidate();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (AllowSelectArea)
        {
            if (e.Button != MouseButtons.Left)
                return;
            Point tempEndPoint = e.Location;
            SelectedRect.Location = new(
                Math.Min(RectStartPoint.X, tempEndPoint.X),
                Math.Min(RectStartPoint.Y, tempEndPoint.Y));
            SelectedRect.Size = new(
                Math.Abs(RectStartPoint.X - tempEndPoint.X),
                Math.Abs(RectStartPoint.Y - tempEndPoint.Y));
            Invalidate();
        }
    }
        
    /// <summary>
    /// Ersetzt die Schnittstelle zum ausgewählten Bild, um den Wert null korrekt zu handeln
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Image? SelectedArea { get; private set; }
}
