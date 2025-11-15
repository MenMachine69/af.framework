
namespace AF.WINFORMS.DX
{
    partial class FormSplash
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.crFlyoutManager1 = new AF.WINFORMS.DX.AFFlyoutManager(this.components);
            this.crTablePanel1 = new AF.WINFORMS.DX.AFTablePanel();
            this.panelPlugin = new AF.WINFORMS.DX.AFSkinnedPanel();
            this.lblMessage = new DevExpress.XtraEditors.LabelControl();
            this.lblVersion = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.crTablePanel1)).BeginInit();
            this.crTablePanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelPlugin)).BeginInit();
            this.SuspendLayout();
            // 
            // crFlyoutManager1
            // 
            this.crFlyoutManager1.ContainerControl = this;
            // 
            // crTablePanel1
            // 
            this.crTablePanel1.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn[] {
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 100F)});
            this.crTablePanel1.Controls.Add(this.panelPlugin);
            this.crTablePanel1.Controls.Add(this.lblMessage);
            this.crTablePanel1.Controls.Add(this.lblVersion);
            this.crTablePanel1.CustomPaintBackground = false;
            this.crTablePanel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.crTablePanel1.Location = new System.Drawing.Point(330, 0);
            this.crTablePanel1.Name = "crTablePanel1";
            this.crTablePanel1.Rows.AddRange(new DevExpress.Utils.Layout.TablePanelRow[] {
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 26F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 100F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 26F)});
            this.crTablePanel1.Size = new System.Drawing.Size(330, 400);
            this.crTablePanel1.TabIndex = 1;
            // 
            // panelPlugin
            // 
            this.panelPlugin.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.crTablePanel1.SetColumn(this.panelPlugin, 0);
            this.panelPlugin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPlugin.Location = new System.Drawing.Point(0, 57);
            this.panelPlugin.Margin = new System.Windows.Forms.Padding(0);
            this.panelPlugin.Name = "panelPlugin";
            this.crTablePanel1.SetRow(this.panelPlugin, 1);
            this.panelPlugin.Size = new System.Drawing.Size(330, 310);
            this.panelPlugin.TabIndex = 5;
            // 
            // lblMessage
            // 
            this.lblMessage.AllowHtmlString = true;
            this.lblMessage.Appearance.FontSizeDelta = 2;
            this.lblMessage.Appearance.Options.UseFont = true;
            this.lblMessage.Appearance.Options.UseTextOptions = true;
            this.lblMessage.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblMessage.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.crTablePanel1.SetColumn(this.lblMessage, 0);
            this.lblMessage.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblMessage.Location = new System.Drawing.Point(0, 0);
            this.lblMessage.Margin = new System.Windows.Forms.Padding(0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Padding = new System.Windows.Forms.Padding(10, 30, 10, 10);
            this.crTablePanel1.SetRow(this.lblMessage, 0);
            this.lblMessage.Size = new System.Drawing.Size(330, 57);
            this.lblMessage.TabIndex = 4;
            this.lblMessage.Text = "...";
            // 
            // lblVersion
            // 
            this.lblVersion.AllowHtmlString = true;
            this.lblVersion.Appearance.Options.UseTextOptions = true;
            this.lblVersion.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblVersion.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.crTablePanel1.SetColumn(this.lblVersion, 0);
            this.lblVersion.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblVersion.Location = new System.Drawing.Point(0, 367);
            this.lblVersion.Margin = new System.Windows.Forms.Padding(0);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Padding = new System.Windows.Forms.Padding(10);
            this.crTablePanel1.SetRow(this.lblVersion, 2);
            this.lblVersion.Size = new System.Drawing.Size(330, 33);
            this.lblVersion.TabIndex = 3;
            this.lblVersion.Text = "0.0.0";
            // 
            // FormSplash
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 400);
            this.ControlBox = false;
            this.Controls.Add(this.crTablePanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSplash";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "formSplash";
            ((System.ComponentModel.ISupportInitialize)(this.crTablePanel1)).EndInit();
            this.crTablePanel1.ResumeLayout(false);
            this.crTablePanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelPlugin)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private AFFlyoutManager crFlyoutManager1;
        private AFTablePanel crTablePanel1;
        private AFSkinnedPanel panelPlugin;
        private DevExpress.XtraEditors.LabelControl lblMessage;
        private DevExpress.XtraEditors.LabelControl lblVersion;
    }
}