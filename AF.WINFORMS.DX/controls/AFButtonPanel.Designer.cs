namespace AF.WINFORMS.DX
{
    partial class AFButtonPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tablePanel1 = new AF.WINFORMS.DX.AFTablePanel();
            this.simpleButton3 = new AF.WINFORMS.DX.AFButton();
            this.simpleButton2 = new AF.WINFORMS.DX.AFButton();
            this.simpleButton1 = new AF.WINFORMS.DX.AFButton();
            this.separatorControl1 = new DevExpress.XtraEditors.SeparatorControl();
            ((System.ComponentModel.ISupportInitialize)(this.tablePanel1)).BeginInit();
            this.tablePanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // tablePanel1
            // 
            this.tablePanel1.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.tablePanel1.Appearance.Options.UseBackColor = true;
            this.tablePanel1.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn[] {
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 50F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 100F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 50F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 50F)});
            this.tablePanel1.Controls.Add(this.simpleButton3);
            this.tablePanel1.Controls.Add(this.simpleButton2);
            this.tablePanel1.Controls.Add(this.simpleButton1);
            this.tablePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tablePanel1.Location = new System.Drawing.Point(0, 1);
            this.tablePanel1.Name = "tablePanel1";
            this.tablePanel1.Rows.AddRange(new DevExpress.Utils.Layout.TablePanelRow[] {
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 100F)});
            this.tablePanel1.Size = new System.Drawing.Size(477, 51);
            this.tablePanel1.TabIndex = 0;
            this.tablePanel1.UseSkinIndents = true;
            // 
            // simpleButton3
            // 
            this.simpleButton3.AutoSize = true;
            this.tablePanel1.SetColumn(this.simpleButton3, 0);
            this.simpleButton3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.simpleButton3.Location = new System.Drawing.Point(13, 12);
            this.simpleButton3.Name = "simpleButton3";
            this.simpleButton3.Padding = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.tablePanel1.SetRow(this.simpleButton3, 0);
            this.simpleButton3.Size = new System.Drawing.Size(41, 26);
            this.simpleButton3.TabIndex = 2;
            this.simpleButton3.Text = "Mehr";
            this.simpleButton3.Visible = false;
            // 
            // simpleButton2
            // 
            this.simpleButton2.AutoSize = true;
            this.tablePanel1.SetColumn(this.simpleButton2, 3);
            this.simpleButton2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.simpleButton2.Location = new System.Drawing.Point(395, 12);
            this.simpleButton2.Margin = new System.Windows.Forms.Padding(2);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Padding = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.tablePanel1.SetRow(this.simpleButton2, 0);
            this.simpleButton2.Size = new System.Drawing.Size(69, 26);
            this.simpleButton2.TabIndex = 1;
            this.simpleButton2.Text = "Abbrechen";
            // 
            // simpleButton1
            // 
            this.simpleButton1.AutoSize = true;
            this.tablePanel1.SetColumn(this.simpleButton1, 2);
            this.simpleButton1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.simpleButton1.Location = new System.Drawing.Point(327, 12);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Padding = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.tablePanel1.SetRow(this.simpleButton1, 0);
            this.simpleButton1.Size = new System.Drawing.Size(64, 26);
            this.simpleButton1.TabIndex = 0;
            this.simpleButton1.Text = "Speichern";
            // 
            // separatorControl1
            // 
            this.separatorControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.separatorControl1.Location = new System.Drawing.Point(0, 0);
            this.separatorControl1.Margin = new System.Windows.Forms.Padding(0);
            this.separatorControl1.Name = "separatorControl1";
            this.separatorControl1.Padding = new System.Windows.Forms.Padding(0);
            this.separatorControl1.Size = new System.Drawing.Size(477, 1);
            this.separatorControl1.TabIndex = 1;
            // 
            // AFButtonPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tablePanel1);
            this.Controls.Add(this.separatorControl1);
            this.DoubleBuffered = true;
            this.Name = "AFButtonPanel";
            this.Size = new System.Drawing.Size(477, 52);
            ((System.ComponentModel.ISupportInitialize)(this.tablePanel1)).EndInit();
            this.tablePanel1.ResumeLayout(false);
            this.tablePanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AFTablePanel tablePanel1;
        private DevExpress.XtraEditors.SeparatorControl separatorControl1;
        private AFButton simpleButton2;
        private AFButton simpleButton1;
        private AFButton simpleButton3;
    }
}
