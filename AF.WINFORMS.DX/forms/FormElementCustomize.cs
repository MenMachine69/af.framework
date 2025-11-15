using DevExpress.Utils;
using DevExpress.Utils.Behaviors;
using DevExpress.Utils.DragDrop;
using DevExpress.Utils.Layout;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace AF.WINFORMS.DX;

/// <summary>
/// Dialog zur Auswahl von Elementen und deren Reihenfolge
/// </summary>
public class FormElementCustomize : FormBase
{
    private readonly AFGridControl gridSource = null!;
    private readonly AFGridControl gridTarget = null!;
    private readonly AFLabelGrayText description = null!;
    private readonly AFButton pshAll = null!;
    private readonly AFButton pshNone = null!;
    private readonly AFButton pshSelect = null!;
    private readonly AFButton pshRemove = null!;
    private BindingList<CustomizableElement> dataSource = [];
    private BindingList<CustomizableElement> dataTarget = [];
    private readonly BehaviorManager dragdropmanager = new();
    private readonly ToolTipController toolTipController = null!;

    /// <summary>
    /// Constructor
    /// </summary>
    public FormElementCustomize()
    {
        if (UI.DesignMode) return;

        toolTipController = new() { ToolTipType = ToolTipType.SuperTip };
        toolTipController.GetActiveObjectInfo += (_, e) =>
        {
            if (e.SelectedControl is not AFGridControl grid) return;

            if (grid.GetViewAt(e.ControlMousePosition) is not GridView view) return;

            var hitInfo = view.CalcHitInfo(e.ControlMousePosition);

            if (hitInfo.HitTest is not (GridHitTest.Row or GridHitTest.RowCell)) return;

            if (view.GetRow(hitInfo.RowHandle) is not CustomizableElement element) return;

            e.Info = new(hitInfo.HitTest.ToString() + hitInfo.RowHandle.ToString(), element.Description, element.Caption);
        };

        AFButtonPanel buttons = new() { Dock = DockStyle.Bottom };
        Controls.Add(buttons);

        buttons.ButtonOk.Click += (_, _) =>
        {
            DialogResult = DialogResult.OK;
            Close();
        };

        buttons.ButtonCancel.Click += (_, _) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        AFTablePanel table = new() { Dock = DockStyle.Fill, UseSkinIndents = true };
        Controls.Add(table);

        table.BringToFront();
        table.BeginLayout();

        description = table.Add<AFLabelGrayText>(1, 1, colspan: 3);
        description.Padding = new(5);
        description.Text = "<Beschreibung>";
        description.Appearance.TextOptions.WordWrap = WordWrap.Wrap;
        description.Appearance.Options.UseTextOptions = true;
        description.AutoSizeMode = LabelAutoSizeMode.Vertical;

        gridSource = table.Add<AFGridControl>(2, 1, rowspan: 5);
        gridTarget = table.Add<AFGridControl>(2, 3, rowspan: 5);

        AFGridSetup setup = new();
        setup.Columns.Add(new AFGridColumn() { Caption = "Name", ColumnFieldname = "Caption", Bold = true });
        setup.Columns.Add(new AFGridColumn() { Caption = "ID", ColumnFieldname = "ID", Bold = false, Visible = false });

        gridSource.Setup(setup);
        gridSource.ToolTipController = toolTipController;
        gridTarget.Setup(setup);
        gridTarget.ToolTipController = toolTipController;
        

        ((GridView)gridSource.FocusedView).OptionsView.ShowGroupPanel = false;
        ((GridView)gridSource.FocusedView).OptionsView.ShowColumnHeaders = false;

        ((GridView)gridTarget.FocusedView).OptionsView.ShowGroupPanel = false;
        ((GridView)gridTarget.FocusedView).OptionsView.ShowColumnHeaders = false;

        pshAll = table.Add<AFButton>(2, 2);
        pshNone = table.Add<AFButton>(3, 2);
        pshSelect = table.Add<AFButton>(4, 2);
        pshRemove = table.Add<AFButton>(5, 2);

        pshAll.AllowFocus = false;
        pshAll.PaintStyle = PaintStyles.Light;
        pshAll.ShowFocusRectangle = DefaultBoolean.False;
        pshAll.ImageOptions.Location = ImageLocation.MiddleCenter;
        pshAll.ImageOptions.SvgImageSize = new(16, 16);
        pshAll.ImageOptions.SvgImage = UI.GetImage(Symbol.PaddingRight);
        pshAll.Size = new(26, 26);
        pshAll.SuperTip = UI.GetSuperTip("ALLE AUSWÄHLEN", "Fügt alle verfügbaren Elemente zur Auswahl hinzu.");
        pshAll.Click += (_, _) =>
        {
            dataSource.RaiseListChangedEvents = false;
            dataTarget.RaiseListChangedEvents = false;

            dataTarget.AddRange(dataSource.ToArray());
            dataSource.Clear();

            dataSource.RaiseListChangedEvents = true;
            dataTarget.RaiseListChangedEvents = true;

            gridSource.RefreshDataSource();
            gridTarget.RefreshDataSource();
        };

        pshNone.AllowFocus = false;
        pshNone.PaintStyle = PaintStyles.Light;
        pshNone.ShowFocusRectangle = DefaultBoolean.False;
        pshNone.ImageOptions.Location = ImageLocation.MiddleCenter;
        pshNone.ImageOptions.SvgImageSize = new(16, 16);
        pshNone.ImageOptions.SvgImage = UI.GetImage(Symbol.PaddingLeft);
        pshNone.Size = new(26, 26);
        pshNone.SuperTip = UI.GetSuperTip("KEINE AUSWÄHLEN", "Entfernt alle ausgewählten Elemente.");
        pshNone.Click += (_, _) =>
        {
            dataSource.RaiseListChangedEvents = false;
            dataTarget.RaiseListChangedEvents = false;

            dataSource.AddRange(dataTarget.ToArray());
            dataTarget.Clear();

            dataSource.RaiseListChangedEvents = true;
            dataTarget.RaiseListChangedEvents = true;

            gridSource.RefreshDataSource();
            gridTarget.RefreshDataSource();
        };

        pshSelect.AllowFocus = false;
        pshSelect.PaintStyle = PaintStyles.Light;
        pshSelect.ShowFocusRectangle = DefaultBoolean.False;
        pshSelect.ImageOptions.Location = ImageLocation.MiddleCenter;
        pshSelect.ImageOptions.SvgImageSize = new(16, 16);
        pshSelect.ImageOptions.SvgImage = UI.GetImage(Symbol.ArrowRight);
        pshSelect.Size = new(26, 26);
        pshSelect.SuperTip = UI.GetSuperTip("AUSWÄHLEN", "Fügt das in der Liste der verfügbaren Elemente ausgewählte Element zu den ausgewählten Elementen hinzu.", footer: "Tipp: Sie können die Elemente auch via Drag&Drop verschieben.");
        pshSelect.Click += (_, _) =>
        {
            var row = ((GridView)gridSource.FocusedView).GetFocusedRow();
            
            if (row is not CustomizableElement element) return;
            
            dataTarget.Add(element);
            dataSource.Remove(element);
        };

        pshRemove.AllowFocus = false;
        pshRemove.PaintStyle = PaintStyles.Light;
        pshRemove.ShowFocusRectangle = DefaultBoolean.False;
        pshRemove.ImageOptions.Location = ImageLocation.MiddleCenter;
        pshRemove.ImageOptions.SvgImageSize = new(16, 16);
        pshRemove.ImageOptions.SvgImage = UI.GetImage(Symbol.ArrowLeft);
        pshRemove.Size = new(26, 26);
        pshSelect.SuperTip = UI.GetSuperTip("ENTFERNEN", "Entfernt das in der Liste der ausgewählten Elemente ausgewählte Element aus der Liste.", footer: "Tipp: Sie können die Elemente auch via Drag&Drop verschieben.");
        pshRemove.Click += (_, _) =>
        {
            var row = ((GridView)gridTarget.FocusedView).GetFocusedRow();

            if (row is not CustomizableElement element) return;

            dataSource.Add(element);
            dataTarget.Remove(element);
        };

        table.SetColumn(1, TablePanelEntityStyle.Relative, .5f);
        table.SetColumn(3, TablePanelEntityStyle.Relative, .5f);
        table.SetRow(6, TablePanelEntityStyle.Relative, 1.0f);

        table.EndLayout();

        // Attaches the Behavior.
        dragdropmanager.Attach<DragDropBehavior>(gridSource.FocusedView, behavior => {
            behavior.Properties.AllowDrop = true;
            behavior.Properties.InsertIndicatorVisible = true;
            behavior.Properties.PreviewVisible = true;
        });

        dragdropmanager.Attach<DragDropBehavior>(gridTarget.FocusedView, behavior => {
            behavior.Properties.AllowDrop = true;
            behavior.Properties.InsertIndicatorVisible = true;
            behavior.Properties.PreviewVisible = true;
            // behavior.DragDrop += Behavior_DragDropToGrid1;
        });

        Size = new(600, 500);
    }

    /// <summary>
    /// Auswählbare Elemente
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public BindingList<CustomizableElement> Elements { get => dataSource; set { dataSource = value; gridSource.DataSource = dataSource; gridSource.RefreshDataSource(); } }

    /// <summary>
    /// ausgewählte Elemente
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public BindingList<CustomizableElement> SelectedElements { get => dataTarget; set { dataTarget = value; gridTarget.DataSource = dataTarget; gridTarget.RefreshDataSource(); } }

    /// <summary>
    /// Im Dialog anzuzeigende Beschreibung.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Description { get => description.Text; set => description.Text = value; }
}

