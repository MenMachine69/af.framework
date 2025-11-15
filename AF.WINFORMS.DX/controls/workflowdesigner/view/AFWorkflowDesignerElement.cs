using System.Drawing.Drawing2D;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.DPI;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Svg;
using DevExpress.Utils.Text;

namespace AF.WINFORMS.DX;

/// <summary>
/// Element zur Darstellung eines Filters im WorkflowDesigner
/// </summary>
[ToolboxItem(false)]
public class AFWorkflowDesignerElement : AFDesignerCanvasElement
{
    private WorkflowDesignerModelElement _element;
    private Image? symbol;
    private Rectangle? inBound;
    private Rectangle? outBoundLeft;
    private Rectangle? outBoundRight;
    private Rectangle? outBoundMiddle;
    private bool isDragging;
    private Point mouseOffset;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="element">Model des Elements</param>
    public AFWorkflowDesignerElement(WorkflowDesignerModelElement element)
    {
        _element = element;

        Padding = new(6, 30, 6, 6);
        CustomPaintBackground = true;
        BackgroundAppearance = new()
        {
            AutoColors = true,
            BorderWidth = 1,
            CornerRadius = 8
        };

        SetStyle(ControlStyles.UserMouse, true);

        AllowResize = false;

        DragEnter += (_, e) =>
        {
            if (e.Data != null && e.Data.GetDataPresent(typeof(AFWorkflowDesignerAction)) || 
               (e.Data != null && e.Data.GetDataPresent(typeof(AFWorkflowDesignerFilter))))
                    e.Effect = DragDropEffects.Move;
        };

        DragDrop += (_, e) =>
        {

            if (e.Data != null && e.Data.GetData(typeof(AFWorkflowDesignerAction)) is AFWorkflowDesignerElement action && action != this)
                designerCanvas.FinishDragDrop(this);

            if (e.Data != null && e.Data.GetData(typeof(AFWorkflowDesignerFilter)) is AFWorkflowDesignerElement filter && filter != this)
                designerCanvas.FinishDragDrop(this);

        };
    }

    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        AllowDrop = true;

    }

    /// <inheritdoc />
    protected override void OnHandleDestroyed(EventArgs e)
    {
        base.OnHandleDestroyed(e);

        symbol?.Dispose();
    }


    /// <inheritdoc />
    protected override void WndProc(ref Message m)
    {
        const int WM_LBUTTONDOWN = 0x0201;
        const int WM_MOUSEMOVE = 0x0200;
        const int WM_LBUTTONUP = 0x0202;

        switch (m.Msg)
        {
            case WM_LBUTTONDOWN:
                Activate();
                Point mousePos = PointToClient(Cursor.Position);
                // int centerX = Width / 2;

                Capture = true;

                if (!BeginDrag(mousePos))
                {
                    isDragging = true;
                    mouseOffset = mousePos;
                }
                else
                {
                    isDragging = false;
                    Capture = false;
                }
                break;
            case WM_MOUSEMOVE:
                if (isDragging && ClientRectangle.Contains(PointToClient(Cursor.Position)))
                {
                    Point parentPos = Parent!.PointToClient(Cursor.Position);
                    Left = parentPos.X - mouseOffset.X;
                    Top = parentPos.Y - mouseOffset.Y;
                }
                else
                {
                    isDragging = false;
                    Capture = false;

                    Cursor = isWithinDragZone(PointToClient(Cursor.Position)) ? Cursors.Hand : Cursors.Default;
                }
                break;
            case WM_LBUTTONUP:
                stopDrag();
                isDragging = false;
                Capture = false;
                break;
        }
        base.WndProc(ref m);
    }

    //private bool isWithinDropZone(Point location)
    //{
    //    if (inBound?.Contains(location) ?? false)
    //        return true;

    //    return false;
    //}

    private bool isWithinDragZone(Point location)
    {
        if (outBoundLeft?.Contains(location) ?? false) // Starte OutLeft drag...
            return true;

        if (outBoundRight?.Contains(location) ?? false) // Starte OutRight drag...
            return true;

        if (outBoundMiddle?.Contains(location) ?? false) // Starte OutMiddle drag...
            return true;

        return false;
    }

    private AFWorkflowDesignerCanvas designerCanvas => (AFWorkflowDesignerCanvas)Canvas!;

    private void stopDrag()
    {
        designerCanvas.CancelDragDrop();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool BeginDrag(Point pos)
    {
        if (outBoundLeft?.Contains(pos) ?? false) // Starte OutLeft drag...
        {
            designerCanvas.StartDragDrop(this, eWorkflowDragOutboundOrigin.Left);
            return true;
        }
        
        if (outBoundRight?.Contains(pos) ?? false) // Starte OutRight drag...
        {
            designerCanvas.StartDragDrop(this, eWorkflowDragOutboundOrigin.Right);
            return true;
        }
        
        if (outBoundMiddle?.Contains(pos) ?? false) // Starte OutMiddle drag...
        {
            designerCanvas.StartDragDrop(this, eWorkflowDragOutboundOrigin.Middle);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Zeichnen der Oberfläche
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPaint(PaintEventArgs e)
    {

        base.OnPaint(e);
        // Alle Linien zeichnen
       

        //Color c = CommonSkins.GetSkin(UserLookAndFeel.Default).TranslateColor(SystemColors.ControlText);

        bool isAction = GetType() == typeof(AFWorkflowDesignerAction);
        int imgsize = 24;
        Color rectsymbol = isAction ? Color.LimeGreen : Color.Orange;

        symbol ??= new SvgBitmap(isAction ? UI.GetImage(Symbol.FastForward) : UI.GetImage(Symbol.Filter)).Render(new Size(imgsize, imgsize), new SvgPalette());

        using (GraphicsCache cache = new(e.Graphics))
        {
            cache.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            cache.SmoothingMode = SmoothingMode.HighQuality;
            cache.InterpolationMode = InterpolationMode.HighQualityBilinear;
            cache.CompositingQuality = CompositingQuality.HighQuality;

            int pntSize = ScaleHelper.DefaultDpiScale.ScaleHorizontal(5);

            using (SolidBrush brush = new(rectsymbol))
            {
                cache.DrawLeftRoundedRect(new Rectangle(3, 3, Height - 8, Height - 7), rectsymbol, null);

                inBound = new Rectangle((Width - pntSize) / 2, 0, pntSize, pntSize);

                cache.FillRectangle(brush, (Rectangle)inBound);

                if (isAction)
                {
                    outBoundMiddle = new Rectangle((Width - pntSize) / 2, Height - pntSize, pntSize, pntSize);
                    cache.FillRectangle(brush, (Rectangle)outBoundMiddle);
                }
                else
                {
                    int mid = (Height + (Width - Height)) / 2;
                    int x = (Width - Height) / 4;

                    outBoundLeft = new Rectangle(Height + x, Height - pntSize, pntSize, pntSize);
                    cache.FillRectangle(brush, (Rectangle)outBoundLeft);
                    outBoundRight = new Rectangle(mid + x, Height - pntSize, pntSize, pntSize);
                    cache.FillRectangle(brush, (Rectangle)outBoundRight);

                }
            }

            cache.DrawImageUnscaled(symbol, new Point((Height - imgsize) / 2, (Height - imgsize) / 2));

            Color fore = CommonSkins.GetSkin(UserLookAndFeel.Default).TranslateColor(SystemColors.GrayText);

            Rectangle rect = new Rectangle(Height + 5, 1, Width - Height - Padding.Right - 5, Padding.Top);

            StringPainter.Default.DrawString(
                cache,
                new AppearanceObject()
                {
                    ForeColor = fore,
                    TextOptions = { WordWrap = WordWrap.Wrap, HAlignment = HorzAlignment.Near, VAlignment = VertAlignment.Center },
                    FontStyleDelta = FontStyle.Bold,
                    Options = { UseTextOptions = true, UseForeColor = true, UseFont = true },
                },
                $@"{_element.NAME}", rect);

            fore = CommonSkins.GetSkin(UserLookAndFeel.Default).TranslateColor(SystemColors.ControlText);
            rect = new Rectangle(Height + 5, Padding.Top, Width - Height - Padding.Right - 5, Height - Padding.Vertical);
            
            StringPainter.Default.DrawString(
                cache, 
                new AppearanceObject()
                {
                    ForeColor = fore,
                    TextOptions = { WordWrap = WordWrap.Wrap, HAlignment = HorzAlignment.Near, VAlignment = VertAlignment.Top }, 
                    Options = { UseTextOptions = true, UseForeColor = true },
                }, 
                $@"{_element.DESCRIPTION}", rect);

        }
    }


    /// <inheritdoc />
    public override void Activate()
    {
        base.Activate();

        if (BackgroundAppearance == null) return;

        BackgroundAppearance.Dimmed = true;
        BackgroundAppearance.HighlightAutoColors = true;

        Refresh();

        if (Canvas != null) Canvas.ActiveElement = this;

        if (Canvas is AFWorkflowDesignerCanvas qcanvas)
            qcanvas.WorkflowDesigner?.ElementSelected(this);
    }

    /// <inheritdoc />
    public override void Deactivate()
    {
        base.Deactivate();

        if (BackgroundAppearance == null) return;

        BackgroundAppearance.Dimmed = false;
        BackgroundAppearance.HighlightAutoColors = false;

        Refresh();
    }
}