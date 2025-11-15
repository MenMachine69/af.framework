namespace AF.WINFORMS.DX
{
    partial class AFSettingsNavigation
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
            navigation = new DevExpress.XtraEditors.PanelControl();
            breadcrumb = new DevExpress.XtraEditors.PanelControl();
            content = new DevExpress.XtraEditors.PanelControl();
            flyout = new DevExpress.Utils.FlyoutPanel();
            flyoutPanelControl1 = new DevExpress.Utils.FlyoutPanelControl();
            ((System.ComponentModel.ISupportInitialize)navigation).BeginInit();
            ((System.ComponentModel.ISupportInitialize)breadcrumb).BeginInit();
            ((System.ComponentModel.ISupportInitialize)content).BeginInit();
            ((System.ComponentModel.ISupportInitialize)flyout).BeginInit();
            flyout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)flyoutPanelControl1).BeginInit();
            SuspendLayout();
            // 
            // navigation
            // 
            navigation.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
            navigation.Appearance.Options.UseBackColor = true;
            navigation.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            navigation.Dock = System.Windows.Forms.DockStyle.Left;
            navigation.FireScrollEventOnMouseWheel = true;
            navigation.Location = new System.Drawing.Point(0, 0);
            navigation.Name = "navigation";
            navigation.Size = new System.Drawing.Size(341, 642);
            navigation.TabIndex = 0;
            // 
            // breadcrumb
            // 
            breadcrumb.Appearance.BackColor = System.Drawing.Color.FromArgb(192, 255, 255);
            breadcrumb.Appearance.Options.UseBackColor = true;
            breadcrumb.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            breadcrumb.Dock = System.Windows.Forms.DockStyle.Top;
            breadcrumb.FireScrollEventOnMouseWheel = true;
            breadcrumb.Location = new System.Drawing.Point(341, 0);
            breadcrumb.Name = "breadcrumb";
            breadcrumb.Size = new System.Drawing.Size(750, 50);
            breadcrumb.TabIndex = 1;
            // 
            // content
            // 
            content.Appearance.BackColor = System.Drawing.Color.FromArgb(192, 192, 255);
            content.Appearance.Options.UseBackColor = true;
            content.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            content.FireScrollEventOnMouseWheel = true;
            content.Location = new System.Drawing.Point(544, 139);
            content.Name = "content";
            content.Size = new System.Drawing.Size(711, 424);
            content.TabIndex = 2;
            // 
            // flyout
            // 
            flyout.Controls.Add(flyoutPanelControl1);
            flyout.Location = new System.Drawing.Point(427, 122);
            flyout.Name = "flyout";
            flyout.Options.AnchorType = DevExpress.Utils.Win.PopupToolWindowAnchor.Left;
            flyout.Options.CloseOnOuterClick = true;
            flyout.OwnerControl = this;
            flyout.Size = new System.Drawing.Size(150, 150);
            flyout.TabIndex = 3;
            // 
            // flyoutPanelControl1
            // 
            flyoutPanelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            flyoutPanelControl1.FlyoutPanel = flyout;
            flyoutPanelControl1.Location = new System.Drawing.Point(0, 0);
            flyoutPanelControl1.Name = "flyoutPanelControl1";
            flyoutPanelControl1.Size = new System.Drawing.Size(150, 150);
            flyoutPanelControl1.TabIndex = 0;
            // 
            // AFSettingsNavigation
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(flyout);
            Controls.Add(content);
            Controls.Add(breadcrumb);
            Controls.Add(navigation);
            Name = "AFSettingsNavigation";
            Size = new System.Drawing.Size(1091, 642);
            ((System.ComponentModel.ISupportInitialize)navigation).EndInit();
            ((System.ComponentModel.ISupportInitialize)breadcrumb).EndInit();
            ((System.ComponentModel.ISupportInitialize)content).EndInit();
            ((System.ComponentModel.ISupportInitialize)flyout).EndInit();
            flyout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)flyoutPanelControl1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl navigation;
        private DevExpress.XtraEditors.PanelControl breadcrumb;
        private DevExpress.XtraEditors.PanelControl content;
        private DevExpress.Utils.FlyoutPanel flyout;
        private DevExpress.Utils.FlyoutPanelControl flyoutPanelControl1;
    }
}
