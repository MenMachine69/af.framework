using AF.CORE;

namespace AF.MVC
{
    partial class AFSidebarControl
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
            tabs = new WINFORMS.DX.AFNavTabPane();
            panelPlugin = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)tabs).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelPlugin).BeginInit();
            SuspendLayout();
            // 
            // tabs
            // 
            tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            tabs.Location = new System.Drawing.Point(5, 137);
            tabs.Name = "tabs";
            tabs.RegularSize = new System.Drawing.Size(380, 527);
            tabs.SelectedPage = null;
            tabs.Size = new System.Drawing.Size(380, 527);
            tabs.TabIndex = 0;
            tabs.Text = "tabs";
            // 
            // panelPlugin
            // 
            panelPlugin.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            panelPlugin.Dock = System.Windows.Forms.DockStyle.Top;
            panelPlugin.Location = new System.Drawing.Point(5, 2);
            panelPlugin.Name = "panelPlugin";
            panelPlugin.Padding = new System.Windows.Forms.Padding(8);
            panelPlugin.Size = new System.Drawing.Size(380, 135);
            panelPlugin.TabIndex = 2;
            panelPlugin.Visible = false;
            // 
            // SidebarControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(tabs);
            Controls.Add(panelPlugin);
            Name = "AFSidebarControl";
            Padding = new System.Windows.Forms.Padding(5, 2, 5, 5);
            Size = new System.Drawing.Size(390, 669);
            ((System.ComponentModel.ISupportInitialize)tabs).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelPlugin).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private AF.WINFORMS.DX.AFNavTabPane tabs;
        private DevExpress.XtraEditors.PanelControl panelPlugin;
    }
}
