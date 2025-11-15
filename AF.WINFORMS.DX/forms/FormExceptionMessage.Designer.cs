namespace AF.WINFORMS.DX
{
    partial class FormExceptionMessage
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
            this.styleController1 = new DevExpress.XtraEditors.StyleController(this.components);
            this.panelPlugin = new System.Windows.Forms.Panel();
            this.lblMessageText = new AF.WINFORMS.DX.AFLabel();
            this.picIcon = new AF.WINFORMS.DX.AFLabel();
            this.lblMessageTitel = new AF.WINFORMS.DX.AFLabel();
            this.crTablePanel1 = new AF.WINFORMS.DX.AFTablePanel();
            this.psh3 = new AF.WINFORMS.DX.AFButton();
            this.psh2 = new AF.WINFORMS.DX.AFButton();
            this.psh1 = new AF.WINFORMS.DX.AFButton();
            this.pshShowMore = new AF.WINFORMS.DX.AFButton();
            ((System.ComponentModel.ISupportInitialize)(this.styleController1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.crTablePanel1)).BeginInit();
            this.crTablePanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelPlugin
            // 
            this.panelPlugin.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelPlugin.Location = new System.Drawing.Point(80, 126);
            this.panelPlugin.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.panelPlugin.Name = "panelPlugin";
            this.panelPlugin.Size = new System.Drawing.Size(453, 10);
            this.panelPlugin.TabIndex = 11;
            // 
            // lblMessageText
            // 
            this.lblMessageText.AllowHtmlString = true;
            this.lblMessageText.Appearance.Options.UseTextOptions = true;
            this.lblMessageText.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblMessageText.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblMessageText.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblMessageText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMessageText.Location = new System.Drawing.Point(80, 51);
            this.lblMessageText.Margin = new System.Windows.Forms.Padding(2);
            this.lblMessageText.Name = "lblMessageText";
            this.lblMessageText.Padding = new System.Windows.Forms.Padding(10, 3, 13, 7);
            this.lblMessageText.Size = new System.Drawing.Size(453, 75);
            this.lblMessageText.StyleController = this.styleController1;
            this.lblMessageText.TabIndex = 10;
            this.lblMessageText.Text = "lblMessageText";
            this.lblMessageText.UseMnemonic = false;
            // 
            // picIcon
            // 
            this.picIcon.Appearance.Options.UseTextOptions = true;
            this.picIcon.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.picIcon.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.picIcon.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.picIcon.Dock = System.Windows.Forms.DockStyle.Left;
            this.picIcon.ImageOptions.Alignment = System.Drawing.ContentAlignment.TopCenter;
            this.picIcon.Location = new System.Drawing.Point(20, 20);
            this.picIcon.Margin = new System.Windows.Forms.Padding(2);
            this.picIcon.Name = "picIcon";
            this.picIcon.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.picIcon.Size = new System.Drawing.Size(60, 116);
            this.picIcon.TabIndex = 8;
            this.picIcon.UseMnemonic = false;
            // 
            // lblMessageTitel
            // 
            this.lblMessageTitel.AllowHtmlString = true;
            this.lblMessageTitel.Appearance.FontSizeDelta = 1;
            this.lblMessageTitel.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.lblMessageTitel.Appearance.Options.UseFont = true;
            this.lblMessageTitel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblMessageTitel.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblMessageTitel.Location = new System.Drawing.Point(80, 20);
            this.lblMessageTitel.Margin = new System.Windows.Forms.Padding(2);
            this.lblMessageTitel.Name = "lblMessageTitel";
            this.lblMessageTitel.Padding = new System.Windows.Forms.Padding(10, 7, 13, 10);
            this.lblMessageTitel.Size = new System.Drawing.Size(453, 31);
            this.lblMessageTitel.StyleController = this.styleController1;
            this.lblMessageTitel.TabIndex = 9;
            this.lblMessageTitel.Text = "lblMessageTitel";
            this.lblMessageTitel.UseMnemonic = false;
            // 
            // crTablePanel1
            // 
            this.crTablePanel1.AutoSize = true;
            this.crTablePanel1.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn[] {
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 5F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 100F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 50F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 50F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 50F)});
            this.crTablePanel1.Controls.Add(this.psh3);
            this.crTablePanel1.Controls.Add(this.psh2);
            this.crTablePanel1.Controls.Add(this.psh1);
            this.crTablePanel1.Controls.Add(this.pshShowMore);
            this.crTablePanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.crTablePanel1.Location = new System.Drawing.Point(20, 136);
            this.crTablePanel1.Name = "crTablePanel1";
            this.crTablePanel1.Rows.AddRange(new DevExpress.Utils.Layout.TablePanelRow[] {
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 100F)});
            this.crTablePanel1.Size = new System.Drawing.Size(513, 32);
            this.crTablePanel1.TabIndex = 6;
            // 
            // psh3
            // 
            this.psh3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.psh3.Appearance.FontSizeDelta = 1;
            this.psh3.Appearance.Options.UseFont = true;
            this.psh3.AutoSize = true;
            this.crTablePanel1.SetColumn(this.psh3, 4);
            this.psh3.ImageOptions.SvgImageSize = new System.Drawing.Size(16, 16);
            this.psh3.Location = new System.Drawing.Point(501, 6);
            this.psh3.Margin = new System.Windows.Forms.Padding(2);
            this.psh3.Name = "psh3";
            this.psh3.Padding = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.crTablePanel1.SetRow(this.psh3, 0);
            this.psh3.Size = new System.Drawing.Size(10, 20);
            this.psh3.StyleController = this.styleController1;
            this.psh3.TabIndex = 1;
            // 
            // psh2
            // 
            this.psh2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.psh2.Appearance.FontSizeDelta = 1;
            this.psh2.Appearance.Options.UseFont = true;
            this.psh2.AutoSize = true;
            this.crTablePanel1.SetColumn(this.psh2, 3);
            this.psh2.ImageOptions.SvgImageSize = new System.Drawing.Size(16, 16);
            this.psh2.Location = new System.Drawing.Point(487, 6);
            this.psh2.Margin = new System.Windows.Forms.Padding(2);
            this.psh2.Name = "psh2";
            this.psh2.Padding = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.crTablePanel1.SetRow(this.psh2, 0);
            this.psh2.Size = new System.Drawing.Size(10, 20);
            this.psh2.StyleController = this.styleController1;
            this.psh2.TabIndex = 2;
            // 
            // psh1
            // 
            this.psh1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.psh1.Appearance.FontSizeDelta = 1;
            this.psh1.Appearance.Options.UseFont = true;
            this.psh1.AutoSize = true;
            this.crTablePanel1.SetColumn(this.psh1, 2);
            this.psh1.ImageOptions.SvgImageSize = new System.Drawing.Size(16, 16);
            this.psh1.Location = new System.Drawing.Point(473, 6);
            this.psh1.Margin = new System.Windows.Forms.Padding(2);
            this.psh1.Name = "psh1";
            this.psh1.Padding = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.crTablePanel1.SetRow(this.psh1, 0);
            this.psh1.Size = new System.Drawing.Size(10, 20);
            this.psh1.StyleController = this.styleController1;
            this.psh1.TabIndex = 3;
            // 
            // pshShowMore
            // 
            this.pshShowMore.Appearance.FontSizeDelta = 1;
            this.pshShowMore.Appearance.Options.UseFont = true;
            this.pshShowMore.AutoSize = true;
            this.crTablePanel1.SetColumn(this.pshShowMore, 0);
            this.pshShowMore.ImageOptions.SvgImageSize = new System.Drawing.Size(16, 16);
            this.pshShowMore.Location = new System.Drawing.Point(2, 2);
            this.pshShowMore.Margin = new System.Windows.Forms.Padding(2);
            this.pshShowMore.Name = "pshShowMore";
            this.pshShowMore.Padding = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.crTablePanel1.SetRow(this.pshShowMore, 0);
            this.pshShowMore.Size = new System.Drawing.Size(133, 28);
            this.pshShowMore.StyleController = this.styleController1;
            this.pshShowMore.TabIndex = 4;
            this.pshShowMore.Text = "Mehr informationen";
            // 
            // FormExceptionMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 183);
            this.ControlBox = false;
            this.Controls.Add(this.lblMessageText);
            this.Controls.Add(this.panelPlugin);
            this.Controls.Add(this.lblMessageTitel);
            this.Controls.Add(this.picIcon);
            this.Controls.Add(this.crTablePanel1);
            this.FormBorderEffect = DevExpress.XtraEditors.FormBorderEffect.Shadow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.IconOptions.ShowIcon = false;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormExceptionMessage";
            this.Padding = new System.Windows.Forms.Padding(20, 20, 15, 15);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = " ";
            ((System.ComponentModel.ISupportInitialize)(this.styleController1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.crTablePanel1)).EndInit();
            this.crTablePanel1.ResumeLayout(false);
            this.crTablePanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private AFButton pshShowMore;
        private AFButton psh1;
        private AFButton psh2;
        private AFButton psh3;
        private AFLabel picIcon;
        private AFLabel lblMessageTitel;
        private AFLabel lblMessageText;
        private System.Windows.Forms.Panel panelPlugin;
        private DevExpress.XtraEditors.StyleController styleController1;
        private AFTablePanel crTablePanel1;
    }
}
