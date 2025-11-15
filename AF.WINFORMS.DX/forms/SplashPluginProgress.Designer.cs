namespace AF.WINFORMS.DX
{
    partial class SplashPluginProgress
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
            this.tablePanel1 = new DevExpress.Utils.Layout.TablePanel();
            this.progressPanel1 = new DevExpress.XtraWaitForm.ProgressPanel();
            this.lblMessage = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.tablePanel1)).BeginInit();
            this.tablePanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tablePanel1
            // 
            this.tablePanel1.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn[] {
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 50F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 150F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 50F)});
            this.tablePanel1.Controls.Add(this.progressPanel1);
            this.tablePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tablePanel1.Location = new System.Drawing.Point(10, 10);
            this.tablePanel1.Name = "tablePanel1";
            this.tablePanel1.Rows.AddRange(new DevExpress.Utils.Layout.TablePanelRow[] {
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 50F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 150F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 50F)});
            this.tablePanel1.Size = new System.Drawing.Size(310, 277);
            this.tablePanel1.TabIndex = 0;
            // 
            // progressPanel1
            // 
            this.progressPanel1.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.progressPanel1.Appearance.Options.UseBackColor = true;
            this.progressPanel1.AutoHeight = true;
            this.progressPanel1.AutoWidth = true;
            this.tablePanel1.SetColumn(this.progressPanel1, 1);
            this.progressPanel1.ContentAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.progressPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressPanel1.Location = new System.Drawing.Point(83, 67);
            this.progressPanel1.Name = "progressPanel1";
            this.progressPanel1.RingAnimationDiameter = 80;
            this.tablePanel1.SetRow(this.progressPanel1, 1);
            this.progressPanel1.ShowCaption = false;
            this.progressPanel1.ShowDescription = false;
            this.progressPanel1.Size = new System.Drawing.Size(144, 144);
            this.progressPanel1.TabIndex = 0;
            this.progressPanel1.WaitAnimationType = DevExpress.Utils.Animation.WaitingAnimatorType.Ring;
            // 
            // lblMessage
            // 
            this.lblMessage.AllowHtmlString = true;
            this.lblMessage.Appearance.Options.UseTextOptions = true;
            this.lblMessage.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblMessage.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblMessage.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblMessage.Location = new System.Drawing.Point(10, 287);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Padding = new System.Windows.Forms.Padding(10);
            this.lblMessage.Size = new System.Drawing.Size(310, 33);
            this.lblMessage.TabIndex = 1;
            this.lblMessage.Text = "Wird geladen...";
            // 
            // SplashPluginProgress
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.tablePanel1);
            this.Controls.Add(this.lblMessage);
            this.Name = "SplashPluginProgress";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(330, 330);
            ((System.ComponentModel.ISupportInitialize)(this.tablePanel1)).EndInit();
            this.tablePanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.Utils.Layout.TablePanel tablePanel1;
        private DevExpress.XtraWaitForm.ProgressPanel progressPanel1;
        private DevExpress.XtraEditors.LabelControl lblMessage;
    }
}
