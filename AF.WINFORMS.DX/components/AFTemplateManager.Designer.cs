namespace AF.WINFORMS.DX
{
    partial class AFTemplateManager
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
            this.crTablePanel1 = new AF.WINFORMS.DX.AFTablePanel();
            ((System.ComponentModel.ISupportInitialize)(this.crTablePanel1)).BeginInit();
            this.SuspendLayout();
            // 
            // crTablePanel1
            // 
            this.crTablePanel1.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn[] {
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 5F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 55F)});
            this.crTablePanel1.CustomPaintBackground = false;
            this.crTablePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crTablePanel1.Location = new System.Drawing.Point(0, 0);
            this.crTablePanel1.Name = "crTablePanel1";
            this.crTablePanel1.Rows.AddRange(new DevExpress.Utils.Layout.TablePanelRow[] {
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 26F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 26F)});
            this.crTablePanel1.Size = new System.Drawing.Size(930, 566);
            this.crTablePanel1.TabIndex = 0;
            this.crTablePanel1.UseSkinIndents = true;
            // 
            // AFTemplateManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.crTablePanel1);
            this.DoubleBuffered = true;
            this.Name = "AFTemplateManager";
            this.Size = new System.Drawing.Size(930, 566);
            ((System.ComponentModel.ISupportInitialize)(this.crTablePanel1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AFTablePanel crTablePanel1;
    }
}
