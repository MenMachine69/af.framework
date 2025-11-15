namespace AF.WINFORMS.DX
{
    partial class SplashPluginCreateAdmin
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tablePanel1 = new DevExpress.Utils.Layout.TablePanel();
            this.slePasswordRepeat = new DevExpress.XtraEditors.ButtonEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.pshCancel = new DevExpress.XtraEditors.SimpleButton();
            this.pshCreate = new DevExpress.XtraEditors.SimpleButton();
            this.sleLoginName = new DevExpress.XtraEditors.TextEdit();
            this.slePassword = new DevExpress.XtraEditors.ButtonEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.flyoutManager1 = new AF.WINFORMS.DX.AFFlyoutManager(this.components);
            this.baseEditExtender1 = new AF.WINFORMS.DX.AFBaseEditExtender(this.components);
            this.errprovider = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.tablePanel1)).BeginInit();
            this.tablePanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.slePasswordRepeat.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sleLoginName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.slePassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.baseEditExtender1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errprovider)).BeginInit();
            this.SuspendLayout();
            // 
            // tablePanel1
            // 
            this.tablePanel1.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn[] {
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 120F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 100F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 100F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 100F)});
            this.tablePanel1.Controls.Add(this.slePasswordRepeat);
            this.tablePanel1.Controls.Add(this.labelControl4);
            this.tablePanel1.Controls.Add(this.pshCancel);
            this.tablePanel1.Controls.Add(this.pshCreate);
            this.tablePanel1.Controls.Add(this.sleLoginName);
            this.tablePanel1.Controls.Add(this.slePassword);
            this.tablePanel1.Controls.Add(this.labelControl3);
            this.tablePanel1.Controls.Add(this.labelControl2);
            this.tablePanel1.Controls.Add(this.labelControl1);
            this.tablePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tablePanel1.Location = new System.Drawing.Point(10, 30);
            this.tablePanel1.Name = "tablePanel1";
            this.tablePanel1.Rows.AddRange(new DevExpress.Utils.Layout.TablePanelRow[] {
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 56F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 32F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 32F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 32F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 100F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 32F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 16F)});
            this.tablePanel1.Size = new System.Drawing.Size(341, 312);
            this.tablePanel1.TabIndex = 0;
            // 
            // slePasswordRepeat
            // 
            this.tablePanel1.SetColumn(this.slePasswordRepeat, 1);
            this.tablePanel1.SetColumnSpan(this.slePasswordRepeat, 3);
            this.slePasswordRepeat.Location = new System.Drawing.Point(91, 126);
            this.slePasswordRepeat.Name = "slePasswordRepeat";
            this.slePasswordRepeat.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.slePasswordRepeat.Properties.UseSystemPasswordChar = true;
            this.tablePanel1.SetRow(this.slePasswordRepeat, 3);
            this.slePasswordRepeat.Size = new System.Drawing.Size(247, 20);
            this.slePasswordRepeat.TabIndex = 8;
            // 
            // labelControl4
            // 
            this.tablePanel1.SetColumn(this.labelControl4, 0);
            this.labelControl4.Location = new System.Drawing.Point(13, 129);
            this.labelControl4.Margin = new System.Windows.Forms.Padding(13, 3, 8, 3);
            this.labelControl4.Name = "labelControl4";
            this.tablePanel1.SetRow(this.labelControl4, 3);
            this.labelControl4.Size = new System.Drawing.Size(66, 13);
            this.labelControl4.TabIndex = 7;
            this.labelControl4.Text = "Wiederholung";
            // 
            // pshCancel
            // 
            this.tablePanel1.SetColumn(this.pshCancel, 3);
            this.pshCancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pshCancel.Location = new System.Drawing.Point(244, 267);
            this.pshCancel.Name = "pshCancel";
            this.tablePanel1.SetRow(this.pshCancel, 5);
            this.pshCancel.Size = new System.Drawing.Size(94, 26);
            this.pshCancel.TabIndex = 6;
            this.pshCancel.Text = "ABBRECHEN";
            // 
            // pshCreate
            // 
            this.tablePanel1.SetColumn(this.pshCreate, 2);
            this.pshCreate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pshCreate.Location = new System.Drawing.Point(144, 267);
            this.pshCreate.Name = "pshCreate";
            this.tablePanel1.SetRow(this.pshCreate, 5);
            this.pshCreate.Size = new System.Drawing.Size(94, 26);
            this.pshCreate.TabIndex = 5;
            this.pshCreate.Text = "ERSTELLEN";
            // 
            // sleLoginName
            // 
            this.tablePanel1.SetColumn(this.sleLoginName, 1);
            this.tablePanel1.SetColumnSpan(this.sleLoginName, 3);
            this.sleLoginName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sleLoginName.Location = new System.Drawing.Point(91, 59);
            this.sleLoginName.Name = "sleLoginName";
            this.tablePanel1.SetRow(this.sleLoginName, 1);
            this.sleLoginName.Size = new System.Drawing.Size(247, 20);
            this.sleLoginName.TabIndex = 4;
            // 
            // slePassword
            // 
            this.tablePanel1.SetColumn(this.slePassword, 1);
            this.tablePanel1.SetColumnSpan(this.slePassword, 3);
            this.slePassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.slePassword.Location = new System.Drawing.Point(91, 91);
            this.slePassword.Name = "slePassword";
            this.slePassword.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.slePassword.Properties.UseSystemPasswordChar = true;
            this.tablePanel1.SetRow(this.slePassword, 2);
            this.slePassword.Size = new System.Drawing.Size(247, 20);
            this.slePassword.TabIndex = 3;
            // 
            // labelControl3
            // 
            this.tablePanel1.SetColumn(this.labelControl3, 0);
            this.labelControl3.Location = new System.Drawing.Point(13, 94);
            this.labelControl3.Margin = new System.Windows.Forms.Padding(13, 3, 8, 3);
            this.labelControl3.Name = "labelControl3";
            this.tablePanel1.SetRow(this.labelControl3, 2);
            this.labelControl3.Size = new System.Drawing.Size(46, 13);
            this.labelControl3.TabIndex = 2;
            this.labelControl3.Text = "Kennwort";
            // 
            // labelControl2
            // 
            this.tablePanel1.SetColumn(this.labelControl2, 0);
            this.labelControl2.Location = new System.Drawing.Point(13, 62);
            this.labelControl2.Margin = new System.Windows.Forms.Padding(13, 3, 8, 3);
            this.labelControl2.Name = "labelControl2";
            this.tablePanel1.SetRow(this.labelControl2, 1);
            this.labelControl2.Size = new System.Drawing.Size(67, 13);
            this.labelControl2.TabIndex = 1;
            this.labelControl2.Text = "Anmeldename";
            // 
            // labelControl1
            // 
            this.labelControl1.AllowHtmlString = true;
            this.labelControl1.Appearance.Options.UseTextOptions = true;
            this.labelControl1.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.tablePanel1.SetColumn(this.labelControl1, 0);
            this.tablePanel1.SetColumnSpan(this.labelControl1, 4);
            this.labelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelControl1.Location = new System.Drawing.Point(3, 3);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.tablePanel1.SetRow(this.labelControl1, 0);
            this.labelControl1.Size = new System.Drawing.Size(335, 50);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Geben Sie hier die Zugangsdaten für den <b>Administrator</b> der Anwendung ein. M" +
    "it diesen Daten können Sie sich dann bei der Anwendung anmelden.";
            // 
            // flyoutManager1
            // 
            this.flyoutManager1.ContainerControl = this;
            // 
            // errprovider
            // 
            this.errprovider.ContainerControl = this;
            // 
            // SplashPluginCreateAdmin
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.tablePanel1);
            this.Name = "SplashPluginCreateAdmin";
            this.Padding = new System.Windows.Forms.Padding(10, 30, 10, 10);
            this.Size = new System.Drawing.Size(361, 352);
            ((System.ComponentModel.ISupportInitialize)(this.tablePanel1)).EndInit();
            this.tablePanel1.ResumeLayout(false);
            this.tablePanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.slePasswordRepeat.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sleLoginName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.slePassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.baseEditExtender1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errprovider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.Utils.Layout.TablePanel tablePanel1;
        private DevExpress.XtraEditors.SimpleButton pshCancel;
        private DevExpress.XtraEditors.SimpleButton pshCreate;
        private DevExpress.XtraEditors.TextEdit sleLoginName;
        private DevExpress.XtraEditors.ButtonEdit slePassword;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private AFFlyoutManager flyoutManager1;
        private AFBaseEditExtender baseEditExtender1;
        private DevExpress.XtraEditors.ButtonEdit slePasswordRepeat;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider errprovider;
    }
}
