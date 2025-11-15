#if FRAMEWORK
using System.Drawing;
using System.Windows.Forms;
#endif

namespace AF.WINFORMS.DX
{
    partial class OverlayProgressDisplay
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblCaption = new DevExpress.XtraEditors.LabelControl();
            this.lblInformation = new DevExpress.XtraEditors.LabelControl();
            this.lblProgress = new DevExpress.XtraEditors.LabelControl();
            this.pshCancel = new DevExpress.XtraEditors.SimpleButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.progressInfinite = new DevExpress.XtraEditors.MarqueeProgressBarControl();
            this.progress = new DevExpress.XtraEditors.ProgressBarControl();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.progressInfinite.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.progress.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 400F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.lblCaption, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblInformation, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblProgress, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.pshCancel, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(15, 15);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1084, 185);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lblCaption
            // 
            this.lblCaption.Appearance.FontSizeDelta = 2;
            this.lblCaption.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.lblCaption.Appearance.Options.UseFont = true;
            this.lblCaption.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblCaption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCaption.Location = new System.Drawing.Point(345, 3);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(394, 34);
            this.lblCaption.TabIndex = 0;
            this.lblCaption.Text = "<Titel>";
            // 
            // lblInformation
            // 
            this.lblInformation.AllowHtmlString = true;
            this.lblInformation.Appearance.Options.UseTextOptions = true;
            this.lblInformation.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblInformation.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblInformation.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblInformation.Location = new System.Drawing.Point(345, 43);
            this.lblInformation.Name = "lblInformation";
            this.lblInformation.Size = new System.Drawing.Size(394, 53);
            this.lblInformation.TabIndex = 1;
            this.lblInformation.Text = "<Beschreibung>";
            // 
            // lblProgress
            // 
            this.lblProgress.AllowHtmlString = true;
            this.lblProgress.Appearance.FontSizeDelta = -1;
            this.lblProgress.Appearance.Options.UseFont = true;
            this.lblProgress.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblProgress.Location = new System.Drawing.Point(345, 132);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(394, 20);
            this.lblProgress.TabIndex = 2;
            this.lblProgress.Text = "<b>0</b> von <b>0</b>";
            // 
            // pshCancel
            // 
            this.pshCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.pshCancel.Location = new System.Drawing.Point(639, 158);
            this.pshCancel.Name = "pshCancel";
            this.pshCancel.Size = new System.Drawing.Size(100, 24);
            this.pshCancel.TabIndex = 3;
            this.pshCancel.Text = "&Abbrechen";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.progressInfinite);
            this.panel1.Controls.Add(this.progress);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(342, 99);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(400, 30);
            this.panel1.TabIndex = 5;
            // 
            // progressInfinite
            // 
            this.progressInfinite.EditValue = 0;
            this.progressInfinite.Location = new System.Drawing.Point(149, 9);
            this.progressInfinite.Name = "progressInfinite";
            this.progressInfinite.Size = new System.Drawing.Size(100, 18);
            this.progressInfinite.TabIndex = 5;
            // 
            // progress
            // 
            this.progress.Location = new System.Drawing.Point(3, 3);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(111, 24);
            this.progress.TabIndex = 4;
            // 
            // OverlayProgressDisplay
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "OverlayProgressDisplay";
            this.Padding = new System.Windows.Forms.Padding(15);
            this.Size = new System.Drawing.Size(1114, 215);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.progressInfinite.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.progress.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private DevExpress.XtraEditors.LabelControl lblCaption;
        private DevExpress.XtraEditors.LabelControl lblInformation;
        private DevExpress.XtraEditors.LabelControl lblProgress;
        private DevExpress.XtraEditors.SimpleButton pshCancel;
        private Panel panel1;
        private DevExpress.XtraEditors.MarqueeProgressBarControl progressInfinite;
        private DevExpress.XtraEditors.ProgressBarControl progress;
    }
}
