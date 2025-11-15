namespace AF.WINFORMS.DX
{
    partial class AFWorkflowDesigner
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
            this.crDockManager1 = new AF.WINFORMS.DX.AFDockManager();
            this.dockDatabase = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.crPanel1 = new AF.WINFORMS.DX.AFPanel();
            ((System.ComponentModel.ISupportInitialize)(this.crDockManager1)).BeginInit();
            this.dockDatabase.SuspendLayout();
            this.dockPanel1_Container.SuspendLayout();
            this.SuspendLayout();
            // 
            // crDockManager1
            // 
            this.crDockManager1.Form = this;
            this.crDockManager1.RootPanels.AddRange(new DevExpress.XtraBars.Docking.DockPanel[] {
            this.dockDatabase});
            this.crDockManager1.Style = DevExpress.XtraBars.Docking2010.Views.DockingViewStyle.Light;
            this.crDockManager1.TopZIndexControls.AddRange(new string[] {
            "DevExpress.XtraBars.BarDockControl",
            "DevExpress.XtraBars.StandaloneBarDockControl",
            "System.Windows.Forms.MenuStrip",
            "System.Windows.Forms.StatusStrip",
            "System.Windows.Forms.StatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonStatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonControl",
            "DevExpress.XtraBars.Navigation.OfficeNavigationBar",
            "DevExpress.XtraBars.Navigation.TileNavPane",
            "DevExpress.XtraBars.TabFormControl",
            "DevExpress.XtraBars.FluentDesignSystem.FluentDesignFormControl",
            "DevExpress.XtraBars.ToolbarForm.ToolbarFormControl"});
            // 
            // dockDatabase
            // 
            this.dockDatabase.Controls.Add(this.dockPanel1_Container);
            this.dockDatabase.Dock = DevExpress.XtraBars.Docking.DockingStyle.Left;
            this.dockDatabase.ID = new System.Guid("9d4a5cb4-df9c-4ed8-9469-14129fb0cf82");
            this.dockDatabase.Location = new System.Drawing.Point(0, 0);
            this.dockDatabase.Name = "dockDatabase";
            this.dockDatabase.OriginalSize = new System.Drawing.Size(309, 200);
            this.dockDatabase.Size = new System.Drawing.Size(309, 655);
            this.dockDatabase.Text = " ";
            // 
            // dockPanel1_Container
            // 
            this.dockPanel1_Container.Controls.Add(this.simpleButton2);
            this.dockPanel1_Container.Controls.Add(this.simpleButton1);
            this.dockPanel1_Container.Location = new System.Drawing.Point(0, 23);
            this.dockPanel1_Container.Name = "dockPanel1_Container";
            this.dockPanel1_Container.Size = new System.Drawing.Size(308, 632);
            this.dockPanel1_Container.TabIndex = 0;
            // 
            // simpleButton2
            // 
            this.simpleButton2.Location = new System.Drawing.Point(43, 80);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Size = new System.Drawing.Size(75, 23);
            this.simpleButton2.TabIndex = 15;
            this.simpleButton2.Text = "simpleButton2";
            this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
            // 
            // simpleButton1
            // 
            this.simpleButton1.Location = new System.Drawing.Point(43, 26);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(75, 23);
            this.simpleButton1.TabIndex = 0;
            this.simpleButton1.Text = "simpleButton1";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // crPanel1
            // 
            this.crPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crPanel1.Location = new System.Drawing.Point(309, 0);
            this.crPanel1.Name = "crPanel1";
            this.crPanel1.Size = new System.Drawing.Size(1099, 655);
            this.crPanel1.TabIndex = 9;
            // 
            // AFWorkflowDesigner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.crPanel1);
            this.Controls.Add(this.dockDatabase);
            this.DoubleBuffered = true;
            this.Name = "AFWorkflowDesigner";
            this.Size = new System.Drawing.Size(1408, 655);
            ((System.ComponentModel.ISupportInitialize)(this.crDockManager1)).EndInit();
            this.dockDatabase.ResumeLayout(false);
            this.dockPanel1_Container.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private AFDockManager crDockManager1;
        private DevExpress.XtraBars.Docking.DockPanel dockDatabase;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;
        private AFPanel crPanel1;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
    }
}
