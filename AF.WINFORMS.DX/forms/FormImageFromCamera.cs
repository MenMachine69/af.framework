using DevExpress.Utils.Layout;
using DevExpress.Utils.Svg;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Camera;

namespace AF.WINFORMS.DX;

/// <summary>
/// Dialog zur Aufzeichnung eines Kamerabildes (WebCam)
/// </summary>
public sealed class FormImageFromCamera : FormBase
{
    private readonly CameraControl cameraControl1 = null!;
    private int currentRotation;
    private readonly AFBarManager manager = null!;
    private readonly AFBarController barController = null!;

    /// <summary>
    /// Constructor
    /// </summary>
    public FormImageFromCamera()
    {
        if (UI.DesignMode) return;

        barController = new();
        barController.AutoBackColorInBars = true;

        manager = new();
        manager.Form = this;
        manager.Controller = barController;
        manager.BeginInit();

        Text = "BILD AUFNEHMEN";
        Size = new(800, 600);
        StartPosition = FormStartPosition.CenterParent;

        cameraControl1 = new() {  Name = nameof(cameraControl1), Dock = DockStyle.Fill, ShowSettingsButton = false, DeviceNotFoundString = "Keine Kamera gefunden." };
        AFButtonPanel buttons = new() { Name = nameof(buttons), Dock = DockStyle.Bottom };

        buttons.ButtonOk.Text = "AUFNAHME";
        buttons.ButtonOk.Click += (_, _) =>
        {
            Picture = cameraControl1.TakeSnapshot();
            DialogResult = DialogResult.OK;
            Close();
        };
        buttons.ButtonCancel.Text = "ABBRECHEN";
        buttons.ButtonCancel.Click += (_, _) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        Controls.Add(buttons);

        AFTablePanel table = new AFTablePanel { Dock = DockStyle.Fill, AutoSize = true, UseSkinIndents = true };
        Controls.Add(table);

        table.BeginLayout();
        table.Add<AFLabelCaption>(1, 1).Text = "Bild von Kamera aufnehmen";
        table.Add<AFLabel>(2, 1).Text = "Wenn Sie an Ihrem PC eine Kamera angeschlossen haben, können Sie hier ein Bild mit dieser Kamera aufnehmen.";
        var tbar = table.AddBar(manager, 3, 1);
        addButton(tbar, "btnSettings", UI.GetImage(Symbol.Settings), "Einstellungen", "Einstellungen für die Kamera anzeigen.");
        addButton(tbar, "btnRotate", UI.GetImage(Symbol.RotateLeft), "Rotation", "Bild in 90° Schritten rotieren/drehen.", true);


        table.Add(cameraControl1, 4, 1);
        
        table.SetColumn(1, TablePanelEntityStyle.Relative, 1.0f);
        table.SetRow(4, TablePanelEntityStyle.Relative, 1.0f);
        table.SetRow(3, TablePanelEntityStyle.Absolute, 32.0f);

        table.EndLayout();

        manager.EndInit();
    }

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
            case "btnSettings":
                settings();
                break;
            case "btnRotate":
                rotate();
                break;
        }
    }

    private void rotate()
    {
        if (currentRotation == 0)
        {
            cameraControl1.RotateAngle = RotateAngle.Rotate90;
            currentRotation = 90;
        }
        else if (currentRotation == 90)
        {
            cameraControl1.RotateAngle = RotateAngle.Rotate180;
            currentRotation = 180;
        }
        else if (currentRotation == 180)
        {
            cameraControl1.RotateAngle = RotateAngle.Rotate270;
            currentRotation = 270;
        }
        else if (currentRotation == 270)
        {
            cameraControl1.RotateAngle = RotateAngle.RotateNone;
            currentRotation = 0;
        }
    }

    private void settings()
    {
        cameraControl1.ShowSettingsForm();
    }

    /// <inheritdoc />
    protected override void OnHandleDestroyed(EventArgs e)
    {
        base.OnHandleDestroyed(e);

        manager?.Dispose();
        barController?.Dispose();
    }

    /// <summary>
    /// Das aufgenommene Bild.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
    public Bitmap? Picture { get; set; }
}