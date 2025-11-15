namespace AF.MVC
{
    partial class AFViewManager
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
            this.components = new System.ComponentModel.Container();
            this.crFlyoutManager1 = new AF.WINFORMS.DX.AFFlyoutManager(this.components);
            this.panelSidebarFixed = new DevExpress.XtraEditors.SidePanel();
            this.splitBrowser = new DevExpress.XtraEditors.SplitContainerControl();
            this.pageContainer1 = new AF.MVC.AFPageContainer();
            ((System.ComponentModel.ISupportInitialize)(this.splitBrowser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitBrowser.Panel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitBrowser.Panel2)).BeginInit();
            this.splitBrowser.Panel2.SuspendLayout();
            this.splitBrowser.SuspendLayout();
            this.SuspendLayout();
            // 
            // crFlyoutManager1
            // 
            this.crFlyoutManager1.ContainerControl = this;
            // 
            // panelSidebarFixed
            // 
            this.panelSidebarFixed.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelSidebarFixed.Location = new System.Drawing.Point(818, 0);
            this.panelSidebarFixed.Name = "panelSidebarFixed";
            this.panelSidebarFixed.Size = new System.Drawing.Size(309, 673);
            this.panelSidebarFixed.TabIndex = 0;
            this.panelSidebarFixed.Text = "sidePanel1";
            this.panelSidebarFixed.Visible = false;
            // 
            // splitBrowser
            // 
            this.splitBrowser.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel1;
            this.splitBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitBrowser.Location = new System.Drawing.Point(0, 0);
            this.splitBrowser.Name = "splitBrowser";
            // 
            // splitBrowser.Panel1
            // 
            this.splitBrowser.Panel1.Text = "Panel1";
            // 
            // splitBrowser.Panel2
            // 
            this.splitBrowser.Panel2.Controls.Add(this.pageContainer1);
            this.splitBrowser.Panel2.Text = "Panel2";
            this.splitBrowser.Size = new System.Drawing.Size(818, 673);
            this.splitBrowser.SplitterPosition = 250;
            this.splitBrowser.TabIndex = 1;
            // 
            // pageContainer1
            // 
            this.pageContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pageContainer1.Location = new System.Drawing.Point(0, 0);
            this.pageContainer1.Name = "pageContainer1";
            this.pageContainer1.Size = new System.Drawing.Size(558, 673);
            this.pageContainer1.TabIndex = 0;
            // 
            // AFViewManager
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.splitBrowser);
            this.Controls.Add(this.panelSidebarFixed);
            this.DoubleBuffered = true;
            this.Name = "AFViewManager";
            this.Size = new System.Drawing.Size(1127, 673);
            ((System.ComponentModel.ISupportInitialize)(this.splitBrowser.Panel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitBrowser.Panel2)).EndInit();
            this.splitBrowser.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitBrowser)).EndInit();
            this.splitBrowser.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private WINFORMS.DX.AFFlyoutManager crFlyoutManager1;
        private DevExpress.XtraEditors.SidePanel panelSidebarFixed;
        private DevExpress.XtraEditors.SplitContainerControl splitBrowser;
        private AFPageContainer pageContainer1;
    }
}
