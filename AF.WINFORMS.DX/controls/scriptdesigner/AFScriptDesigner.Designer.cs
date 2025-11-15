namespace AF.WINFORMS.DX
{
    partial class AFScriptDesigner
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
            this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager(this.components);
            this.crBarManager1 = new AF.WINFORMS.DX.AFBarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.menMainCheckCode = new DevExpress.XtraBars.BarButtonItem();
            this.standaloneBarDockControl1 = new DevExpress.XtraBars.StandaloneBarDockControl();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.dockClasses = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.crClassBrowser1 = new AF.WINFORMS.DX.AFClassBrowser();
            this.dockORM = new DevExpress.XtraBars.Docking.DockPanel();
            this.controlContainer3 = new DevExpress.XtraBars.Docking.ControlContainer();
            this.ormBrowser = new AF.WINFORMS.DX.AFORMBrowser();
            this.dockDatabase = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel4_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.crdbSchemeBrowser1 = new AF.WINFORMS.DX.AFDBSchemeBrowser();
            this.panelContainer2 = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockVariablen = new DevExpress.XtraBars.Docking.DockPanel();
            this.controlContainer1 = new DevExpress.XtraBars.Docking.ControlContainer();
            this.dockSnippets = new DevExpress.XtraBars.Docking.DockPanel();
            this.controlContainer2 = new DevExpress.XtraBars.Docking.ControlContainer();
            this.gridSnippets = new AF.WINFORMS.DX.AFGridControl();
            this.viewSnippets = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.lblDropSnippet = new AF.WINFORMS.DX.AFLabelGrayText();
            this.dockPlaceholder = new DevExpress.XtraBars.Docking.DockPanel();
            this.controlContainer4 = new DevExpress.XtraBars.Docking.ControlContainer();
            this.gridPlaceholder = new AF.WINFORMS.DX.AFGridControl();
            this.viewPlaceholder = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.panelContainer1 = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockErrors = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel2_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.gridErrors = new AF.WINFORMS.DX.AFGridControl();
            this.viewErrors = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.dockOutput = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel3_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.mleOutput = new AF.WINFORMS.DX.AFEditMultiline();
            this.crDockManager1 = new AF.WINFORMS.DX.AFDockManager();
            this.panelMain = new AF.WINFORMS.DX.AFPanel();
            this.extenderPlaceholder = new AF.WINFORMS.DX.AFGridExtender();
            this.extenderSnippet = new AF.WINFORMS.DX.AFGridExtender();
            this.panelContainer4 = new DevExpress.XtraBars.Docking.DockPanel();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.crBarManager1)).BeginInit();
            this.dockClasses.SuspendLayout();
            this.dockPanel1_Container.SuspendLayout();
            this.dockORM.SuspendLayout();
            this.controlContainer3.SuspendLayout();
            this.dockDatabase.SuspendLayout();
            this.dockPanel4_Container.SuspendLayout();
            this.panelContainer2.SuspendLayout();
            this.dockVariablen.SuspendLayout();
            this.dockSnippets.SuspendLayout();
            this.controlContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridSnippets)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewSnippets)).BeginInit();
            this.dockPlaceholder.SuspendLayout();
            this.controlContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridPlaceholder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewPlaceholder)).BeginInit();
            this.panelContainer1.SuspendLayout();
            this.dockErrors.SuspendLayout();
            this.dockPanel2_Container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridErrors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewErrors)).BeginInit();
            this.dockOutput.SuspendLayout();
            this.dockPanel3_Container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mleOutput.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.crDockManager1)).BeginInit();
            this.panelMain.SuspendLayout();
            this.panelContainer4.SuspendLayout();
            this.SuspendLayout();
            // 
            // dockManager1
            // 
            this.dockManager1.AllowGlyphSkinning = true;
            this.dockManager1.Form = this;
            this.dockManager1.MenuManager = this.crBarManager1;
            this.dockManager1.RootPanels.AddRange(new DevExpress.XtraBars.Docking.DockPanel[] {
            this.panelContainer4,
            this.panelContainer2,
            this.panelContainer1});
            this.dockManager1.Style = DevExpress.XtraBars.Docking2010.Views.DockingViewStyle.Light;
            this.dockManager1.TopZIndexControls.AddRange(new string[] {
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
            // crBarManager1
            // 
            this.crBarManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.crBarManager1.DockControls.Add(this.barDockControlTop);
            this.crBarManager1.DockControls.Add(this.barDockControlBottom);
            this.crBarManager1.DockControls.Add(this.barDockControlLeft);
            this.crBarManager1.DockControls.Add(this.barDockControlRight);
            this.crBarManager1.DockControls.Add(this.standaloneBarDockControl1);
            this.crBarManager1.DockManager = this.dockManager1;
            this.crBarManager1.Form = this;
            this.crBarManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.menMainCheckCode});
            this.crBarManager1.MaxItemId = 2;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Standalone;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.menMainCheckCode)});
            this.bar1.OptionsBar.AllowQuickCustomization = false;
            this.bar1.OptionsBar.DisableClose = true;
            this.bar1.OptionsBar.DisableCustomization = true;
            this.bar1.OptionsBar.DrawBorder = false;
            this.bar1.OptionsBar.DrawDragBorder = false;
            this.bar1.OptionsBar.UseWholeRow = true;
            this.bar1.StandaloneBarDockControl = this.standaloneBarDockControl1;
            this.bar1.Text = "Tools";
            // 
            // menMainCheckCode
            // 
            this.menMainCheckCode.Caption = "Script prüfen";
            this.menMainCheckCode.Id = 0;
            this.menMainCheckCode.Name = "menMainCheckCode";
            // 
            // standaloneBarDockControl1
            // 
            this.standaloneBarDockControl1.AutoSize = true;
            this.standaloneBarDockControl1.CausesValidation = false;
            this.standaloneBarDockControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.standaloneBarDockControl1.Location = new System.Drawing.Point(0, 0);
            this.standaloneBarDockControl1.Manager = this.crBarManager1;
            this.standaloneBarDockControl1.Name = "standaloneBarDockControl1";
            this.standaloneBarDockControl1.Size = new System.Drawing.Size(629, 20);
            this.standaloneBarDockControl1.Text = "standaloneBarDockControl1";
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.crBarManager1;
            this.barDockControlTop.Size = new System.Drawing.Size(1172, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 695);
            this.barDockControlBottom.Manager = this.crBarManager1;
            this.barDockControlBottom.Size = new System.Drawing.Size(1172, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
            this.barDockControlLeft.Manager = this.crBarManager1;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 695);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1172, 0);
            this.barDockControlRight.Manager = this.crBarManager1;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 695);
            // 
            // dockClasses
            // 
            this.dockClasses.Controls.Add(this.dockPanel1_Container);
            this.dockClasses.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
            this.dockClasses.ID = new System.Guid("07444381-e690-4ca1-b73d-5f402a674829");
            this.dockClasses.Location = new System.Drawing.Point(0, 23);
            this.dockClasses.Name = "dockClasses";
            this.dockClasses.Options.AllowDockAsTabbedDocument = false;
            this.dockClasses.Options.AllowFloating = false;
            this.dockClasses.Options.FloatOnDblClick = false;
            this.dockClasses.Options.ShowCloseButton = false;
            this.dockClasses.OriginalSize = new System.Drawing.Size(268, 200);
            this.dockClasses.Size = new System.Drawing.Size(267, 646);
            this.dockClasses.Text = "Klassenbrowser";
            // 
            // dockPanel1_Container
            // 
            this.dockPanel1_Container.Controls.Add(this.crClassBrowser1);
            this.dockPanel1_Container.Location = new System.Drawing.Point(0, 0);
            this.dockPanel1_Container.Name = "dockPanel1_Container";
            this.dockPanel1_Container.Size = new System.Drawing.Size(267, 646);
            this.dockPanel1_Container.TabIndex = 0;
            // 
            // crClassBrowser1
            // 
            this.crClassBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crClassBrowser1.Location = new System.Drawing.Point(0, 0);
            this.crClassBrowser1.Name = "crClassBrowser1";
            this.crClassBrowser1.Size = new System.Drawing.Size(267, 646);
            this.crClassBrowser1.TabIndex = 0;
            // 
            // dockORM
            // 
            this.dockORM.Controls.Add(this.controlContainer3);
            this.dockORM.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
            this.dockORM.ID = new System.Guid("c76d9cfb-cd8c-4e10-8709-bb2fdd6389ed");
            this.dockORM.Location = new System.Drawing.Point(0, 23);
            this.dockORM.Name = "dockORM";
            this.dockORM.Options.ShowCloseButton = false;
            this.dockORM.OriginalSize = new System.Drawing.Size(274, 391);
            this.dockORM.Size = new System.Drawing.Size(267, 646);
            this.dockORM.Text = "ORM Browser";
            // 
            // controlContainer3
            // 
            this.controlContainer3.Controls.Add(this.ormBrowser);
            this.controlContainer3.Location = new System.Drawing.Point(0, 0);
            this.controlContainer3.Name = "controlContainer3";
            this.controlContainer3.Size = new System.Drawing.Size(267, 646);
            this.controlContainer3.TabIndex = 0;
            // 
            // ormBrowser
            // 
            this.ormBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ormBrowser.Location = new System.Drawing.Point(0, 0);
            this.ormBrowser.Name = "ormBrowser";
            this.ormBrowser.Size = new System.Drawing.Size(267, 646);
            this.ormBrowser.TabIndex = 0;
            // 
            // dockDatabase
            // 
            this.dockDatabase.Controls.Add(this.dockPanel4_Container);
            this.dockDatabase.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
            this.dockDatabase.FloatVertical = true;
            this.dockDatabase.ID = new System.Guid("8198a52e-097e-490f-97ad-29e193e2f8ca");
            this.dockDatabase.Location = new System.Drawing.Point(0, 23);
            this.dockDatabase.Name = "dockDatabase";
            this.dockDatabase.Options.AllowDockAsTabbedDocument = false;
            this.dockDatabase.Options.AllowFloating = false;
            this.dockDatabase.Options.FloatOnDblClick = false;
            this.dockDatabase.Options.ShowCloseButton = false;
            this.dockDatabase.OriginalSize = new System.Drawing.Size(275, 441);
            this.dockDatabase.Size = new System.Drawing.Size(267, 646);
            this.dockDatabase.Text = "Datenbank";
            // 
            // dockPanel4_Container
            // 
            this.dockPanel4_Container.Controls.Add(this.crdbSchemeBrowser1);
            this.dockPanel4_Container.Location = new System.Drawing.Point(0, 0);
            this.dockPanel4_Container.Name = "dockPanel4_Container";
            this.dockPanel4_Container.Size = new System.Drawing.Size(267, 646);
            this.dockPanel4_Container.TabIndex = 0;
            // 
            // crdbSchemeBrowser1
            // 
            this.crdbSchemeBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crdbSchemeBrowser1.Location = new System.Drawing.Point(0, 0);
            this.crdbSchemeBrowser1.Name = "crdbSchemeBrowser1";
            this.crdbSchemeBrowser1.Size = new System.Drawing.Size(267, 646);
            this.crdbSchemeBrowser1.TabIndex = 0;
            // 
            // panelContainer2
            // 
            this.panelContainer2.ActiveChild = this.dockVariablen;
            this.panelContainer2.Controls.Add(this.dockVariablen);
            this.panelContainer2.Controls.Add(this.dockSnippets);
            this.panelContainer2.Controls.Add(this.dockPlaceholder);
            this.panelContainer2.Dock = DevExpress.XtraBars.Docking.DockingStyle.Right;
            this.panelContainer2.ID = new System.Guid("8ce2b381-9a63-4de3-9676-1a7b1c5465a0");
            this.panelContainer2.Location = new System.Drawing.Point(897, 0);
            this.panelContainer2.Name = "panelContainer2";
            this.panelContainer2.OriginalSize = new System.Drawing.Size(275, 200);
            this.panelContainer2.SavedDock = DevExpress.XtraBars.Docking.DockingStyle.Right;
            this.panelContainer2.SavedIndex = 1;
            this.panelContainer2.SavedSizeFactor = 0.73094D;
            this.panelContainer2.Size = new System.Drawing.Size(275, 695);
            this.panelContainer2.Tabbed = true;
            this.panelContainer2.Text = "panelContainer2";
            // 
            // dockVariablen
            // 
            this.dockVariablen.Controls.Add(this.controlContainer1);
            this.dockVariablen.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
            this.dockVariablen.ID = new System.Guid("4adc6f33-ed9e-433a-be50-7623c3db8922");
            this.dockVariablen.Location = new System.Drawing.Point(1, 23);
            this.dockVariablen.Name = "dockVariablen";
            this.dockVariablen.Options.AllowDockAsTabbedDocument = false;
            this.dockVariablen.Options.AllowFloating = false;
            this.dockVariablen.Options.FloatOnDblClick = false;
            this.dockVariablen.Options.ShowCloseButton = false;
            this.dockVariablen.OriginalSize = new System.Drawing.Size(274, 204);
            this.dockVariablen.Size = new System.Drawing.Size(274, 646);
            this.dockVariablen.Text = "Variablen";
            // 
            // controlContainer1
            // 
            this.controlContainer1.Location = new System.Drawing.Point(0, 0);
            this.controlContainer1.Name = "controlContainer1";
            this.controlContainer1.Size = new System.Drawing.Size(274, 646);
            this.controlContainer1.TabIndex = 0;
            // 
            // dockSnippets
            // 
            this.dockSnippets.Controls.Add(this.controlContainer2);
            this.dockSnippets.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
            this.dockSnippets.ID = new System.Guid("8fb8fa3f-bc41-489b-b823-7e9b3bd26d9c");
            this.dockSnippets.Location = new System.Drawing.Point(1, 23);
            this.dockSnippets.Name = "dockSnippets";
            this.dockSnippets.Options.AllowDockAsTabbedDocument = false;
            this.dockSnippets.Options.AllowFloating = false;
            this.dockSnippets.Options.FloatOnDblClick = false;
            this.dockSnippets.Options.ShowCloseButton = false;
            this.dockSnippets.OriginalSize = new System.Drawing.Size(274, 204);
            this.dockSnippets.Size = new System.Drawing.Size(274, 646);
            this.dockSnippets.Text = "Codeschnipsel";
            // 
            // controlContainer2
            // 
            this.controlContainer2.Controls.Add(this.gridSnippets);
            this.controlContainer2.Controls.Add(this.lblDropSnippet);
            this.controlContainer2.Location = new System.Drawing.Point(0, 0);
            this.controlContainer2.Name = "controlContainer2";
            this.controlContainer2.Size = new System.Drawing.Size(274, 646);
            this.controlContainer2.TabIndex = 0;
            // 
            // gridSnippets
            // 
            this.gridSnippets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridSnippets.Location = new System.Drawing.Point(0, 23);
            this.gridSnippets.MainView = this.viewSnippets;
            this.gridSnippets.MenuManager = this.crBarManager1;
            this.gridSnippets.Name = "gridSnippets";
            this.gridSnippets.Size = new System.Drawing.Size(274, 623);
            this.gridSnippets.TabIndex = 20;
            this.gridSnippets.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewSnippets});
            // 
            // viewSnippets
            // 
            this.viewSnippets.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.viewSnippets.GridControl = this.gridSnippets;
            this.viewSnippets.Name = "viewSnippets";
            this.viewSnippets.OptionsView.ShowGroupPanel = false;
            this.viewSnippets.OptionsView.ShowIndicator = false;
            // 
            // lblDropSnippet
            // 
            this.lblDropSnippet.AllowDrop = true;
            this.lblDropSnippet.Appearance.Options.UseTextOptions = true;
            this.lblDropSnippet.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblDropSnippet.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblDropSnippet.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDropSnippet.Location = new System.Drawing.Point(0, 0);
            this.lblDropSnippet.Name = "lblDropSnippet";
            this.lblDropSnippet.Padding = new System.Windows.Forms.Padding(5);
            this.lblDropSnippet.Size = new System.Drawing.Size(274, 23);
            this.lblDropSnippet.Style = "Gray";
            this.lblDropSnippet.TabIndex = 21;
            this.lblDropSnippet.Text = "+ neues Snippet (DragDrop Code)";
            // 
            // dockPlaceholder
            // 
            this.dockPlaceholder.Controls.Add(this.controlContainer4);
            this.dockPlaceholder.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
            this.dockPlaceholder.ID = new System.Guid("f931e1ba-a07c-4c8d-9623-95102be3d681");
            this.dockPlaceholder.Location = new System.Drawing.Point(1, 23);
            this.dockPlaceholder.Name = "dockPlaceholder";
            this.dockPlaceholder.Options.ShowCloseButton = false;
            this.dockPlaceholder.OriginalSize = new System.Drawing.Size(274, 204);
            this.dockPlaceholder.Size = new System.Drawing.Size(274, 646);
            this.dockPlaceholder.Text = "Platzhalter";
            // 
            // controlContainer4
            // 
            this.controlContainer4.Controls.Add(this.gridPlaceholder);
            this.controlContainer4.Location = new System.Drawing.Point(0, 0);
            this.controlContainer4.Name = "controlContainer4";
            this.controlContainer4.Size = new System.Drawing.Size(274, 646);
            this.controlContainer4.TabIndex = 0;
            // 
            // gridPlaceholder
            // 
            this.gridPlaceholder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPlaceholder.Location = new System.Drawing.Point(0, 0);
            this.gridPlaceholder.MainView = this.viewPlaceholder;
            this.gridPlaceholder.MenuManager = this.crBarManager1;
            this.gridPlaceholder.Name = "gridPlaceholder";
            this.gridPlaceholder.Size = new System.Drawing.Size(274, 646);
            this.gridPlaceholder.TabIndex = 19;
            this.gridPlaceholder.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewPlaceholder});
            // 
            // viewPlaceholder
            // 
            this.viewPlaceholder.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.viewPlaceholder.GridControl = this.gridPlaceholder;
            this.viewPlaceholder.Name = "viewPlaceholder";
            this.viewPlaceholder.OptionsView.ShowGroupPanel = false;
            this.viewPlaceholder.OptionsView.ShowIndicator = false;
            // 
            // panelContainer1
            // 
            this.panelContainer1.ActiveChild = this.dockErrors;
            this.panelContainer1.Controls.Add(this.dockOutput);
            this.panelContainer1.Controls.Add(this.dockErrors);
            this.panelContainer1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Bottom;
            this.panelContainer1.FloatVertical = true;
            this.panelContainer1.ID = new System.Guid("e80a42b5-dfbf-45b1-82a5-48cc16065b30");
            this.panelContainer1.Location = new System.Drawing.Point(268, 495);
            this.panelContainer1.Name = "panelContainer1";
            this.panelContainer1.OriginalSize = new System.Drawing.Size(200, 200);
            this.panelContainer1.Size = new System.Drawing.Size(629, 200);
            this.panelContainer1.Tabbed = true;
            this.panelContainer1.Text = "panelContainer1";
            // 
            // dockErrors
            // 
            this.dockErrors.Controls.Add(this.dockPanel2_Container);
            this.dockErrors.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
            this.dockErrors.ID = new System.Guid("f88c05e7-6499-4de4-9ce6-d5f04b5fc167");
            this.dockErrors.Location = new System.Drawing.Point(0, 24);
            this.dockErrors.Name = "dockErrors";
            this.dockErrors.Options.AllowDockAsTabbedDocument = false;
            this.dockErrors.Options.AllowFloating = false;
            this.dockErrors.Options.FloatOnDblClick = false;
            this.dockErrors.Options.ShowCloseButton = false;
            this.dockErrors.OriginalSize = new System.Drawing.Size(904, 150);
            this.dockErrors.Size = new System.Drawing.Size(629, 150);
            this.dockErrors.Text = "Fehler";
            // 
            // dockPanel2_Container
            // 
            this.dockPanel2_Container.Controls.Add(this.gridErrors);
            this.dockPanel2_Container.Location = new System.Drawing.Point(0, 0);
            this.dockPanel2_Container.Name = "dockPanel2_Container";
            this.dockPanel2_Container.Size = new System.Drawing.Size(629, 150);
            this.dockPanel2_Container.TabIndex = 0;
            // 
            // gridErrors
            // 
            this.gridErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridErrors.Location = new System.Drawing.Point(0, 0);
            this.gridErrors.MainView = this.viewErrors;
            this.gridErrors.MenuManager = this.crBarManager1;
            this.gridErrors.Name = "gridErrors";
            this.gridErrors.Size = new System.Drawing.Size(629, 150);
            this.gridErrors.TabIndex = 21;
            this.gridErrors.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewErrors});
            // 
            // viewErrors
            // 
            this.viewErrors.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.viewErrors.GridControl = this.gridErrors;
            this.viewErrors.Name = "viewErrors";
            this.viewErrors.OptionsView.ShowGroupPanel = false;
            this.viewErrors.OptionsView.ShowIndicator = false;
            // 
            // dockOutput
            // 
            this.dockOutput.Controls.Add(this.dockPanel3_Container);
            this.dockOutput.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
            this.dockOutput.FloatVertical = true;
            this.dockOutput.ID = new System.Guid("457a1510-6e67-43ac-afbe-b7cfc31387de");
            this.dockOutput.Location = new System.Drawing.Point(0, 24);
            this.dockOutput.Name = "dockOutput";
            this.dockOutput.Options.AllowDockAsTabbedDocument = false;
            this.dockOutput.Options.AllowFloating = false;
            this.dockOutput.Options.FloatOnDblClick = false;
            this.dockOutput.Options.ShowCloseButton = false;
            this.dockOutput.OriginalSize = new System.Drawing.Size(904, 150);
            this.dockOutput.Size = new System.Drawing.Size(629, 150);
            this.dockOutput.Text = "Ausgabe";
            // 
            // dockPanel3_Container
            // 
            this.dockPanel3_Container.Controls.Add(this.mleOutput);
            this.dockPanel3_Container.Location = new System.Drawing.Point(0, 0);
            this.dockPanel3_Container.Name = "dockPanel3_Container";
            this.dockPanel3_Container.Size = new System.Drawing.Size(629, 150);
            this.dockPanel3_Container.TabIndex = 0;
            // 
            // mleOutput
            // 
            this.mleOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mleOutput.Location = new System.Drawing.Point(0, 0);
            this.mleOutput.MenuManager = this.crBarManager1;
            this.mleOutput.Name = "mleOutput";
            this.mleOutput.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.mleOutput.Properties.ReadOnly = true;
            this.mleOutput.Properties.UseReadOnlyAppearance = false;
            this.mleOutput.Size = new System.Drawing.Size(629, 150);
            this.mleOutput.TabIndex = 19;
            // 
            // crDockManager1
            // 
            this.crDockManager1.Form = this;
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
            // panelMain
            // 
            this.panelMain.Controls.Add(this.standaloneBarDockControl1);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(268, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(629, 495);
            this.panelMain.TabIndex = 11;
            // 
            // panelContainer4
            // 
            this.panelContainer4.ActiveChild = this.dockORM;
            this.panelContainer4.Controls.Add(this.dockORM);
            this.panelContainer4.Controls.Add(this.dockClasses);
            this.panelContainer4.Controls.Add(this.dockDatabase);
            this.panelContainer4.Dock = DevExpress.XtraBars.Docking.DockingStyle.Left;
            this.panelContainer4.ID = new System.Guid("7568bdb2-f346-4bc6-b6bc-2aca10ddba22");
            this.panelContainer4.Location = new System.Drawing.Point(0, 0);
            this.panelContainer4.Name = "panelContainer4";
            this.panelContainer4.OriginalSize = new System.Drawing.Size(268, 200);
            this.panelContainer4.SavedDock = DevExpress.XtraBars.Docking.DockingStyle.Left;
            this.panelContainer4.SavedIndex = 0;
            this.panelContainer4.Size = new System.Drawing.Size(268, 695);
            this.panelContainer4.Tabbed = true;
            this.panelContainer4.Text = "panelContainer4";
            // 
            // AFScriptDesigner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelContainer1);
            this.Controls.Add(this.panelContainer2);
            this.Controls.Add(this.panelContainer4);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.DoubleBuffered = true;
            this.Name = "AFScriptDesigner";
            this.Size = new System.Drawing.Size(1172, 695);
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.crBarManager1)).EndInit();
            this.dockClasses.ResumeLayout(false);
            this.dockPanel1_Container.ResumeLayout(false);
            this.dockORM.ResumeLayout(false);
            this.controlContainer3.ResumeLayout(false);
            this.dockDatabase.ResumeLayout(false);
            this.dockPanel4_Container.ResumeLayout(false);
            this.panelContainer2.ResumeLayout(false);
            this.dockVariablen.ResumeLayout(false);
            this.dockSnippets.ResumeLayout(false);
            this.controlContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridSnippets)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewSnippets)).EndInit();
            this.dockPlaceholder.ResumeLayout(false);
            this.controlContainer4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridPlaceholder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewPlaceholder)).EndInit();
            this.panelContainer1.ResumeLayout(false);
            this.dockErrors.ResumeLayout(false);
            this.dockPanel2_Container.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridErrors)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewErrors)).EndInit();
            this.dockOutput.ResumeLayout(false);
            this.dockPanel3_Container.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mleOutput.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.crDockManager1)).EndInit();
            this.panelMain.ResumeLayout(false);
            this.panelMain.PerformLayout();
            this.panelContainer4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Docking.DockManager dockManager1;
        private DevExpress.XtraBars.Docking.DockPanel dockDatabase;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel4_Container;
        private DevExpress.XtraBars.Docking.DockPanel panelContainer1;
        private DevExpress.XtraBars.Docking.DockPanel dockOutput;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel3_Container;
        private DevExpress.XtraBars.Docking.DockPanel dockErrors;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel2_Container;
        private DevExpress.XtraBars.Docking.DockPanel dockClasses;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;
        private AFDockManager crDockManager1;
        private AFPanel panelMain;
        private DevExpress.XtraBars.StandaloneBarDockControl standaloneBarDockControl1;
        private AFBarManager crBarManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem menMainCheckCode;
        private DevExpress.XtraBars.Docking.DockPanel panelContainer2;
        private DevExpress.XtraBars.Docking.DockPanel dockVariablen;
        private DevExpress.XtraBars.Docking.ControlContainer controlContainer1;
        private AFClassBrowser crClassBrowser1;
        private DevExpress.XtraBars.Docking.DockPanel dockSnippets;
        private DevExpress.XtraBars.Docking.ControlContainer controlContainer2;
        private AFDBSchemeBrowser crdbSchemeBrowser1;
        private DevExpress.XtraBars.Docking.DockPanel dockORM;
        private DevExpress.XtraBars.Docking.ControlContainer controlContainer3;
        private AFORMBrowser ormBrowser;
        private AFEditMultiline mleOutput;
        private DevExpress.XtraBars.Docking.DockPanel dockPlaceholder;
        private DevExpress.XtraBars.Docking.ControlContainer controlContainer4;
        private AFGridControl gridPlaceholder;
        private DevExpress.XtraGrid.Views.Grid.GridView viewPlaceholder;
        private AFGridExtender extenderPlaceholder;
        private AFGridControl gridSnippets;
        private DevExpress.XtraGrid.Views.Grid.GridView viewSnippets;
        private AFLabelGrayText lblDropSnippet;
        private AFGridExtender extenderSnippet;
        private AFGridControl gridErrors;
        private DevExpress.XtraGrid.Views.Grid.GridView viewErrors;
        private DevExpress.XtraBars.Docking.DockPanel panelContainer4;
    }
}
