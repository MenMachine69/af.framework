using DevExpress.LookAndFeel;
using DevExpress.XtraWaitForm;

namespace AF.WINFORMS.DX;

/// <summary>
/// AF WaitForm, dass durch gegensätzlichen Skin das WaitForm besser hervorhebt.
/// </summary>
public sealed class AFWaitForm : WaitForm
{
    private readonly ProgressPanel progressPanel1 = null!;
    private UserLookAndFeel? lookAndFeel;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFWaitForm()
    {
        if (UI.DesignMode) return;

        progressPanel1 = new ProgressPanel();
        var tableLayoutPanel1 = new TableLayoutPanel();
        tableLayoutPanel1.SuspendLayout();
        SuspendLayout();
        progressPanel1.Appearance.BackColor = Color.Transparent;
        progressPanel1.Appearance.Options.UseBackColor = true;
        progressPanel1.AppearanceCaption.Font = new Font("Microsoft Sans Serif", 12f);
        progressPanel1.AppearanceCaption.Options.UseFont = true;
        progressPanel1.AppearanceDescription.Font = new Font("Microsoft Sans Serif", 8.25f);
        progressPanel1.AppearanceDescription.Options.UseFont = true;
        progressPanel1.Dock = DockStyle.Fill;
        progressPanel1.ImageHorzOffset = 20;
        progressPanel1.Location = new Point(0, 17);
        progressPanel1.Margin = new Padding(0, 3, 0, 3);
        progressPanel1.Name = "progressPanel1";
        progressPanel1.Size = new Size(246, 39);
        progressPanel1.TabIndex = 0;
        progressPanel1.Text = "progressPanel1";
        tableLayoutPanel1.AutoSize = true;
        tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        tableLayoutPanel1.BackColor = Color.Transparent;
        tableLayoutPanel1.ColumnCount = 1;
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        tableLayoutPanel1.Controls.Add((Control)progressPanel1, 0, 0);
        tableLayoutPanel1.Dock = DockStyle.Fill;
        tableLayoutPanel1.Location = new Point(0, 0);
        tableLayoutPanel1.Name = "tableLayoutPanel1";
        tableLayoutPanel1.Padding = new Padding(0, 14, 0, 14);
        tableLayoutPanel1.RowCount = 1;
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
        tableLayoutPanel1.Size = new Size(246, 73);
        tableLayoutPanel1.TabIndex = 1;
        AutoScaleDimensions = new SizeF(6f, 13f);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        ClientSize = new Size(246, 73);
        Controls.Add((Control)tableLayoutPanel1);
        DoubleBuffered = true;
        Name = nameof(DemoWaitForm);
        StartPosition = FormStartPosition.Manual;
        Text = "Form1";
        tableLayoutPanel1.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();

        progressPanel1.AutoHeight = true;
    }


    /// <inheritdoc />
    protected override UserLookAndFeel TargetLookAndFeel
    {
        get
        {
            if (lookAndFeel == null)
            {
                lookAndFeel = new UserLookAndFeel(this);
                lookAndFeel.UseDefaultLookAndFeel = false;
                if (UI.IsDarkSkin)
                    lookAndFeel.SkinName = "Office 2019 White";
                else
                    lookAndFeel.SkinName = "Office 2019 Black";
            }
            return lookAndFeel;
        }
    }

    /// <summary>
    /// Fortschrittsanzeige...
    /// </summary>
    public ProgressPanel ProgressPanel => progressPanel1;

    /// <inheritdoc />
    public override void SetCaption(string caption)
    {
        base.SetCaption(caption);
        progressPanel1.Caption = caption;
    }

    /// <inheritdoc />
    public override void SetDescription(string description)
    {
        base.SetDescription(description);
        progressPanel1.Description = description;
    }
    
    /// <inheritdoc />
    public override void ProcessCommand(Enum cmd, object arg) => base.ProcessCommand(cmd, arg);
}