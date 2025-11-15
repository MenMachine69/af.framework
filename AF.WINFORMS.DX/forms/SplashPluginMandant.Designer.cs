
using DevExpress.Office;

namespace AF.WINFORMS.DX
{
    partial class SplashPluginMandant
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
            this.tablePanel1 = new AF.WINFORMS.DX.AFTablePanel();
            this.lstMandanten = new AF.WINFORMS.DX.AFListbox();
            this.pshCancel = new AF.WINFORMS.DX.AFButton();
            this.pshSelect = new AF.WINFORMS.DX.AFButton();
            this.labelControl1 = new AF.WINFORMS.DX.AFLabel();
            this.flyoutManager1 = new AF.WINFORMS.DX.AFFlyoutManager(this.components);
            this.baseEditExtender1 = new AF.WINFORMS.DX.AFBaseEditExtender(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.tablePanel1)).BeginInit();
            this.tablePanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lstMandanten)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.baseEditExtender1)).BeginInit();
            this.SuspendLayout();
            // 
            // tablePanel1
            // 
            this.tablePanel1.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn[] {
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 100F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 100F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 100F)});
            this.tablePanel1.Controls.Add(this.lstMandanten);
            this.tablePanel1.Controls.Add(this.pshCancel);
            this.tablePanel1.Controls.Add(this.pshSelect);
            this.tablePanel1.Controls.Add(this.labelControl1);
            this.tablePanel1.CustomPaintBackground = false;
            this.tablePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tablePanel1.Location = new System.Drawing.Point(10, 10);
            this.tablePanel1.Name = "tablePanel1";
            this.tablePanel1.Rows.AddRange(new DevExpress.Utils.Layout.TablePanelRow[] {
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 46F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 100F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 32F)});
            this.tablePanel1.Size = new System.Drawing.Size(310, 310);
            this.tablePanel1.TabIndex = 0;
            // 
            // lstMandanten
            // 
            this.lstMandanten.Appearance.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.lstMandanten.Appearance.FontSizeDelta = 2;
            this.lstMandanten.Appearance.Options.UseBackColor = true;
            this.lstMandanten.Appearance.Options.UseFont = true;
            this.lstMandanten.AppearanceHighlight.FontSizeDelta = 2;
            this.lstMandanten.AppearanceHighlight.Options.UseFont = true;
            this.lstMandanten.AppearanceSelected.FontSizeDelta = 2;
            this.lstMandanten.AppearanceSelected.Options.UseFont = true;
            this.lstMandanten.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.tablePanel1.SetColumn(this.lstMandanten, 0);
            this.tablePanel1.SetColumnSpan(this.lstMandanten, 3);
            this.lstMandanten.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstMandanten.Location = new System.Drawing.Point(3, 45);
            this.lstMandanten.Name = "lstMandanten";
            this.tablePanel1.SetRow(this.lstMandanten, 1);
            this.lstMandanten.Size = new System.Drawing.Size(304, 228);
            this.lstMandanten.TabIndex = 7;
            // 
            // pshCancel
            // 
            this.pshCancel.AutoSize = true;
            this.tablePanel1.SetColumn(this.pshCancel, 2);
            this.pshCancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pshCancel.Location = new System.Drawing.Point(221, 279);
            this.pshCancel.Name = "pshCancel";
            this.pshCancel.Padding = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.tablePanel1.SetRow(this.pshCancel, 2);
            this.pshCancel.Size = new System.Drawing.Size(86, 28);
            this.pshCancel.TabIndex = 6;
            this.pshCancel.Text = "ABBRECHEN";
            // 
            // pshSelect
            // 
            this.pshSelect.AutoSize = true;
            this.tablePanel1.SetColumn(this.pshSelect, 1);
            this.pshSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pshSelect.Location = new System.Drawing.Point(139, 279);
            this.pshSelect.Name = "pshSelect";
            this.pshSelect.Padding = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.tablePanel1.SetRow(this.pshSelect, 2);
            this.pshSelect.Size = new System.Drawing.Size(76, 28);
            this.pshSelect.TabIndex = 5;
            this.pshSelect.Text = "AUSWAHL";
            // 
            // labelControl1
            // 
            this.labelControl1.AllowHtmlString = true;
            this.labelControl1.Appearance.Options.UseTextOptions = true;
            this.labelControl1.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.tablePanel1.SetColumn(this.labelControl1, 0);
            this.tablePanel1.SetColumnSpan(this.labelControl1, 3);
            this.labelControl1.CustomPaintBackground = false;
            this.labelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelControl1.Location = new System.Drawing.Point(3, 3);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.tablePanel1.SetRow(this.labelControl1, 0);
            this.labelControl1.Size = new System.Drawing.Size(304, 36);
            this.labelControl1.Style = "Standard";
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Bitte wählen Sie den <b>Mandanten</b> aus, mit dem Sie arbeiten möchten.";
            // 
            // flyoutManager1
            // 
            this.flyoutManager1.ContainerControl = this;
            // 
            // SplashPluginMandant
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.tablePanel1);
            this.Name = "SplashPluginMandant";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(330, 330);
            ((System.ComponentModel.ISupportInitialize)(this.tablePanel1)).EndInit();
            this.tablePanel1.ResumeLayout(false);
            this.tablePanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lstMandanten)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.baseEditExtender1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AFTablePanel tablePanel1;
        private AFButton pshCancel;
        private AFButton pshSelect;
        private AFLabel labelControl1;
        private AFFlyoutManager flyoutManager1;
        private AFBaseEditExtender baseEditExtender1;
        private AFListbox lstMandanten;
    }
}
