using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Layout;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace AF.MVC;

/// <summary>
/// Dialog zur Anzeige eines Models as Property-Dialog (i.d.R. als Seitenleiste in einer AFPage).
/// </summary>
[ToolboxItem(false)]
public sealed class AFOptionsDialog : AFUserControl, IUIElement, IDialogContainer
{
    private readonly AFTablePanel table;
    private readonly AFLabel? lblCpt;
    private readonly AFBindingConnector connector;
    private readonly AFErrorProvider errorProvider;
    private readonly Container container;
    private readonly IViewPage? page;
    private readonly AFButtonPanel buttons;
    private readonly int widthFolded;
    private readonly int widthUnfolded;

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        container.Dispose();
        base.Dispose(disposing);
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="model">Model, dass die zu bearbeitenden Eigenschaften enthält</param>
    /// <param name="caption">Überschrift des Dialogs</param>
    /// <param name="foldable">Dialog ist Zusammenfaltbar</param>
    /// <param name="folded">Zusammengefaltet anzeigen</param>
    /// <param name="parentpage">Page, in der der Dialog angezeigt wird.</param>
    /// <param name="width">Standardbreite des Dialogs</param>
    /// <param name="collapsedWidth">Breite des Dialogs, wenn zugeklappt</param>
    public AFOptionsDialog(IModel model, string caption = "EINSTELLUNGEN", bool foldable = false, bool folded = false, IViewPage? parentpage = null, int width = 300, int collapsedWidth = 30 )
    {
        if (model.GetType().GetTypeDescription().Options?.ControllerType?.GetController() is not IControllerUI controller)
            throw new Exception($"Es wurde kein Controller für das Model ({model.GetType().FullName}) gefunden, der eine Anzeige in einem Einstellungsdialog erlaubt. Prüfen Sie AFOptions und ControllerType.");

        widthFolded = ScaleUtils.ScaleValue(collapsedWidth);
        widthUnfolded = ScaleUtils.ScaleValue(width);

        buttons = new() { Dock = DockStyle.Bottom };
        Controls.Add(buttons);

        container = new();
        errorProvider = new(container) { ContainerControl = this };
        connector = new(container);
        page = parentpage;

        Dock = DockStyle.Fill;
        Padding = new(1, 0, 0, 0);
        Margin = new(0);

        AFTablePanel tableHeader = new() { Dock = DockStyle.Top, UseSkinIndents = false, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink };
        tableHeader.BeginLayout();

        if (foldable)
        {
            lblCpt = tableHeader.Add<AFLabel>(1, 1);
            lblCpt.Padding = new(8, 5, 8, 5);
            lblCpt.Text = caption;

            var pshFold = tableHeader.Add<AFButton>(1, 2);
            pshFold.Padding = new(5);
            pshFold.Text = "";
            pshFold.ImageOptions.SvgImage = UI.GetImage(Symbol.Wrench);
            pshFold.ImageOptions.SvgImageSize = new(16, 16);
            pshFold.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.Full;
            pshFold.ImageLocation = ImageLocation.MiddleCenter;
            pshFold.PaintStyle = PaintStyles.Light;
            pshFold.ShowFocusRectangle = DefaultBoolean.False;
            pshFold.TabStop = false;
            pshFold.AllowFocus = false;
            pshFold.AutoSize = true;
            pshFold.Padding = new(3);
            pshFold.Dock = DockStyle.Top;

            pshFold.Click += (_, _) =>
            {
                if (lblCpt.Visible)
                    page?.FoldSettingsDialog();
                else
                    page?.UnfoldSettingsDialog();
            };

            buttons.ButtonOk.Visible = false;
            buttons.ButtonCancel.BackColor = DXSkinColors.FillColors.Success;
            buttons.ButtonCancel.Text = "ANWENDEN";
            buttons.ButtonCancel.Click += (_, _) =>
            {
                execute();
            };
        }
        else
        {
            var lblHeader = tableHeader.Add<AFLabelCaption>(1, 1);
            lblHeader.Padding = new(8, 9, 8, 9);
            lblHeader.Margin = new(7);
            lblHeader.Text = caption;

            buttons.ButtonOk.Text = "AUSFÜHREN";
            buttons.ButtonOk.Click += (_, _) =>
            {
                execute();
            };

            buttons.ButtonCancel.Text = "ABBRECHEN";
            buttons.ButtonCancel.Click += (_, _) =>
            {
                cancel();
            };

        }

        //tableHeader.SetRow(1, DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, lblCpt.Size.Height);
        tableHeader.SetColumn(1, TablePanelEntityStyle.Relative, 1.0f);

        tableHeader.EndLayout();

        Controls.Add(tableHeader);


        table = new() { Dock = DockStyle.Fill, UseSkinIndents = true };
        table.FromModel(model.GetType(), controller);

        Controls.Add(table);
        table.BringToFront();

        connector.ErrorProvider = errorProvider;
        connector.StartContainerControl = table;
        connector.DataSource = model;

        if (folded)
            Collapse();

        Size = new(folded ? widthFolded : widthUnfolded, 500);
    }

    private void execute()
    {
        ValidationErrorCollection errors = [];

        if (Validate() && connector.Validate(errorProvider, errors))
        {
            CommandArgs args = new()
            {
                CommandContext = eCommandContext.Other,
                Page = page,
                Model = connector.DataSource as IModel,
                ParentControl = this,
                Dialog = this
            };
            var result = CommandExecute?.Invoke(args);
            
            if (result != null)
                page?.HandleResult(result);
        }
    }

    private void cancel()
    {
        CommandArgs args = new()
        {
            CommandContext = eCommandContext.Other,
            Page = page,
            Model = connector.DataSource as IModel,
            ParentControl = this,
            Dialog = this
        };
        var result = CommandCancel?.Invoke(args);

        if (result != null)
            page?.HandleResult(result);
    }

    /// <summary>
    /// in geöffnete/aufgefaltete Darstellung wechseln
    /// </summary>
    public int Expand()
    {
        table.Enabled = true;
        table.Visible = true;
        lblCpt!.Visible = true;
        buttons.Enabled = true;
        buttons.Visible = true;

        return widthUnfolded;
    }


    /// <summary>
    /// in geschlossene/zusammengefaltete Darstellung wechseln
    /// </summary>
    public int Collapse()
    {
        table.Enabled = false;
        table.Visible = false;
        lblCpt!.Visible = false;
        buttons.Enabled = false;
        buttons.Visible = false;

        return widthFolded;
    }

    /// <summary>
    /// Command, das ausgeführt wird, wenn der Dialog bestätigt wird.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Func<CommandArgs, CommandResult>? CommandExecute { get; set; }

    /// <summary>
    /// Command, das ausgeführt wird, wenn der Dialog abgebrochen wird.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Func<CommandArgs, CommandResult>? CommandCancel { get; set; }

    /// <summary>
    /// Zugriff auf das Model.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IModel? Model => connector.DataSource as IModel;

}

