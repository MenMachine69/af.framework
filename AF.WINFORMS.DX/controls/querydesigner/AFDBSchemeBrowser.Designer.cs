namespace AF.WINFORMS.DX
{
    partial class AFDBSchemeBrowser
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
            DevExpress.Utils.AppearanceObject appearanceObject1 = new DevExpress.Utils.AppearanceObject();
            DevExpress.Utils.AppearanceObject appearanceObject2 = new DevExpress.Utils.AppearanceObject();
            DevExpress.Utils.AppearanceObject appearanceObject3 = new DevExpress.Utils.AppearanceObject();
            DevExpress.Utils.AppearanceObject appearanceObject4 = new DevExpress.Utils.AppearanceObject();
            DevExpress.Utils.AppearanceObject appearanceObject5 = new DevExpress.Utils.AppearanceObject();
            this.cmbDatasource = new AF.WINFORMS.DX.AFComboboxLookup();
            this.crComboboxLookup1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.cR3RoundRectPainter1 = new AF.WINFORMS.DX.AFRoundRectPainter();
            this.extender = new AF.WINFORMS.DX.AFGridExtender();
            this.separatorControl1 = new DevExpress.XtraEditors.SeparatorControl();
            this.crBarManager1 = new AF.WINFORMS.DX.AFBarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.menRefresh = new DevExpress.XtraBars.BarButtonItem();
            this.menShowTree = new DevExpress.XtraBars.BarCheckItem();
            this.menShowGrid = new DevExpress.XtraBars.BarCheckItem();
            this.standaloneBarDockControl1 = new DevExpress.XtraBars.StandaloneBarDockControl();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.treeClassStructure = new AF.WINFORMS.DX.AFTreeGrid();
            this.grid = new AF.WINFORMS.DX.AFSplitContainer();
            this.gridDatasourceBrowser = new AF.WINFORMS.DX.AFGridControl();
            this.viewDatasourceBrowser = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridDatasourceBrowserFields = new AF.WINFORMS.DX.AFGridControl();
            this.viewDatasourceBrowserFields = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.mleInfo = new AF.WINFORMS.DX.AFLabel();
            this.crNavSplitter1 = new AF.WINFORMS.DX.AFNavSplitter();
            ((System.ComponentModel.ISupportInitialize)(this.cmbDatasource.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.crComboboxLookup1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.crBarManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeClassStructure)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grid.Panel1)).BeginInit();
            this.grid.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid.Panel2)).BeginInit();
            this.grid.Panel2.SuspendLayout();
            this.grid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridDatasourceBrowser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewDatasourceBrowser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDatasourceBrowserFields)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewDatasourceBrowserFields)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbDatasource
            // 
            this.cmbDatasource.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmbDatasource.Location = new System.Drawing.Point(0, 0);
            this.cmbDatasource.Margin = new System.Windows.Forms.Padding(0);
            this.cmbDatasource.Name = "cmbDatasource";
            this.cmbDatasource.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbDatasource.Properties.PopupView = this.crComboboxLookup1View;
            this.cmbDatasource.Size = new System.Drawing.Size(329, 20);
            this.cmbDatasource.TabIndex = 7;
            // 
            // crComboboxLookup1View
            // 
            this.crComboboxLookup1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.crComboboxLookup1View.Name = "crComboboxLookup1View";
            this.crComboboxLookup1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.crComboboxLookup1View.OptionsView.ShowGroupPanel = false;
            // 
            // cR3RoundRectPainter1
            // 
            this.cR3RoundRectPainter1.AppearanceDefault = appearanceObject1;
            this.cR3RoundRectPainter1.AppearanceDisabled = appearanceObject2;
            this.cR3RoundRectPainter1.AppearanceHover = appearanceObject3;
            this.cR3RoundRectPainter1.AppearanceSelected = appearanceObject4;
            this.cR3RoundRectPainter1.AppearanceSelectedHover = appearanceObject5;
            // 
            // extender
            // 
            this.extender.AddCustomColumnsMenu = false;
            this.extender.AddDefaultMenu = false;
            // 
            // separatorControl1
            // 
            this.separatorControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.separatorControl1.Location = new System.Drawing.Point(0, 528);
            this.separatorControl1.Name = "separatorControl1";
            this.separatorControl1.Padding = new System.Windows.Forms.Padding(0);
            this.separatorControl1.Size = new System.Drawing.Size(329, 1);
            this.separatorControl1.TabIndex = 9;
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
            this.crBarManager1.Form = this;
            this.crBarManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.menShowTree,
            this.menShowGrid,
            this.menRefresh});
            this.crBarManager1.MaxItemId = 3;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Standalone;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.menRefresh),
            new DevExpress.XtraBars.LinkPersistInfo(this.menShowTree),
            new DevExpress.XtraBars.LinkPersistInfo(this.menShowGrid)});
            this.bar1.OptionsBar.AllowQuickCustomization = false;
            this.bar1.OptionsBar.DisableClose = true;
            this.bar1.OptionsBar.DisableCustomization = true;
            this.bar1.OptionsBar.DrawDragBorder = false;
            this.bar1.OptionsBar.UseWholeRow = true;
            this.bar1.StandaloneBarDockControl = this.standaloneBarDockControl1;
            this.bar1.Text = "Tools";
            // 
            // menRefresh
            // 
            this.menRefresh.Id = 2;
            this.menRefresh.ImageOptions.SvgImageColorizationMode = DevExpress.Utils.SvgImageColorizationMode.Full;
            this.menRefresh.ImageOptions.SvgImageSize = new System.Drawing.Size(16, 16);
            this.menRefresh.Name = "menRefresh";
            // 
            // menShowTree
            // 
            this.menShowTree.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.menShowTree.BindableChecked = true;
            this.menShowTree.Checked = true;
            this.menShowTree.CheckStyle = DevExpress.XtraBars.BarCheckStyles.Radio;
            this.menShowTree.Id = 0;
            this.menShowTree.ImageOptions.SvgImageColorizationMode = DevExpress.Utils.SvgImageColorizationMode.Full;
            this.menShowTree.ImageOptions.SvgImageSize = new System.Drawing.Size(16, 16);
            this.menShowTree.Name = "menShowTree";
            // 
            // menShowGrid
            // 
            this.menShowGrid.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.menShowGrid.CheckStyle = DevExpress.XtraBars.BarCheckStyles.Radio;
            this.menShowGrid.Id = 1;
            this.menShowGrid.ImageOptions.SvgImageColorizationMode = DevExpress.Utils.SvgImageColorizationMode.Full;
            this.menShowGrid.ImageOptions.SvgImageSize = new System.Drawing.Size(16, 16);
            this.menShowGrid.Name = "menShowGrid";
            // 
            // standaloneBarDockControl1
            // 
            this.standaloneBarDockControl1.AutoSize = true;
            this.standaloneBarDockControl1.CausesValidation = false;
            this.standaloneBarDockControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.standaloneBarDockControl1.Location = new System.Drawing.Point(0, 20);
            this.standaloneBarDockControl1.Manager = this.crBarManager1;
            this.standaloneBarDockControl1.Name = "standaloneBarDockControl1";
            this.standaloneBarDockControl1.Size = new System.Drawing.Size(329, 20);
            this.standaloneBarDockControl1.Text = "standaloneBarDockControl1";
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.crBarManager1;
            this.barDockControlTop.Size = new System.Drawing.Size(329, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 625);
            this.barDockControlBottom.Manager = this.crBarManager1;
            this.barDockControlBottom.Size = new System.Drawing.Size(329, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
            this.barDockControlLeft.Manager = this.crBarManager1;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 625);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(329, 0);
            this.barDockControlRight.Manager = this.crBarManager1;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 625);
            // 
            // treeClassStructure
            // 
            this.treeClassStructure.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.treeClassStructure.Location = new System.Drawing.Point(145, 120);
            this.treeClassStructure.Name = "treeClassStructure";
            this.treeClassStructure.Size = new System.Drawing.Size(247, 350);
            this.treeClassStructure.TabIndex = 15;
            // 
            // grid
            // 
            this.grid.Horizontal = false;
            this.grid.Location = new System.Drawing.Point(28, 80);
            this.grid.Name = "grid";
            // 
            // grid.Panel1
            // 
            this.grid.Panel1.Controls.Add(this.gridDatasourceBrowser);
            this.grid.Panel1.Text = "Panel1";
            // 
            // grid.Panel2
            // 
            this.grid.Panel2.Controls.Add(this.gridDatasourceBrowserFields);
            this.grid.Panel2.Text = "Panel2";
            this.grid.Size = new System.Drawing.Size(200, 336);
            this.grid.SplitterPosition = 156;
            this.grid.TabIndex = 16;
            this.grid.Visible = false;
            // 
            // gridDatasourceBrowser
            // 
            this.gridDatasourceBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridDatasourceBrowser.Location = new System.Drawing.Point(0, 0);
            this.gridDatasourceBrowser.MainView = this.viewDatasourceBrowser;
            this.gridDatasourceBrowser.Name = "gridDatasourceBrowser";
            this.gridDatasourceBrowser.Size = new System.Drawing.Size(200, 156);
            this.gridDatasourceBrowser.TabIndex = 12;
            this.gridDatasourceBrowser.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewDatasourceBrowser});
            // 
            // viewDatasourceBrowser
            // 
            this.viewDatasourceBrowser.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.viewDatasourceBrowser.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.viewDatasourceBrowser.GridControl = this.gridDatasourceBrowser;
            this.viewDatasourceBrowser.Name = "viewDatasourceBrowser";
            this.viewDatasourceBrowser.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.viewDatasourceBrowser.OptionsView.EnableAppearanceEvenRow = true;
            this.viewDatasourceBrowser.OptionsView.EnableAppearanceOddRow = true;
            this.viewDatasourceBrowser.OptionsView.ShowGroupPanel = false;
            this.viewDatasourceBrowser.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
            this.viewDatasourceBrowser.OptionsView.ShowIndicator = false;
            // 
            // gridDatasourceBrowserFields
            // 
            this.gridDatasourceBrowserFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridDatasourceBrowserFields.Location = new System.Drawing.Point(0, 0);
            this.gridDatasourceBrowserFields.MainView = this.viewDatasourceBrowserFields;
            this.gridDatasourceBrowserFields.Name = "gridDatasourceBrowserFields";
            this.gridDatasourceBrowserFields.Size = new System.Drawing.Size(200, 170);
            this.gridDatasourceBrowserFields.TabIndex = 12;
            this.gridDatasourceBrowserFields.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewDatasourceBrowserFields});
            // 
            // viewDatasourceBrowserFields
            // 
            this.viewDatasourceBrowserFields.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.viewDatasourceBrowserFields.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.viewDatasourceBrowserFields.GridControl = this.gridDatasourceBrowserFields;
            this.viewDatasourceBrowserFields.Name = "viewDatasourceBrowserFields";
            this.viewDatasourceBrowserFields.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.viewDatasourceBrowserFields.OptionsView.EnableAppearanceEvenRow = true;
            this.viewDatasourceBrowserFields.OptionsView.EnableAppearanceOddRow = true;
            this.viewDatasourceBrowserFields.OptionsView.ShowGroupPanel = false;
            this.viewDatasourceBrowserFields.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
            this.viewDatasourceBrowserFields.OptionsView.ShowIndicator = false;
            // 
            // mleInfo
            // 
            this.mleInfo.AllowHtmlString = true;
            this.mleInfo.Appearance.Options.UseTextOptions = true;
            this.mleInfo.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.mleInfo.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.mleInfo.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.mleInfo.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.mleInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.mleInfo.Location = new System.Drawing.Point(0, 539);
            this.mleInfo.Name = "mleInfo";
            this.mleInfo.Padding = new System.Windows.Forms.Padding(6);
            this.mleInfo.Size = new System.Drawing.Size(329, 86);
            this.mleInfo.TabIndex = 22;
            this.mleInfo.Text = "...";
            // 
            // AFNavSplitter1
            // 
            this.crNavSplitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.crNavSplitter1.Location = new System.Drawing.Point(0, 529);
            this.crNavSplitter1.Name = "crNavSplitter1";
            this.crNavSplitter1.Size = new System.Drawing.Size(329, 10);
            this.crNavSplitter1.TabIndex = 23;
            this.crNavSplitter1.TabStop = false;
            // 
            // AFDBSchemeBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeClassStructure);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.separatorControl1);
            this.Controls.Add(this.standaloneBarDockControl1);
            this.Controls.Add(this.cmbDatasource);
            this.Controls.Add(this.crNavSplitter1);
            this.Controls.Add(this.mleInfo);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.DoubleBuffered = true;
            this.Name = "AFDBSchemeBrowser";
            this.Size = new System.Drawing.Size(329, 625);
            ((System.ComponentModel.ISupportInitialize)(this.cmbDatasource.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.crComboboxLookup1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.crBarManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeClassStructure)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grid.Panel1)).EndInit();
            this.grid.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid.Panel2)).EndInit();
            this.grid.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.grid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridDatasourceBrowser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewDatasourceBrowser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDatasourceBrowserFields)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewDatasourceBrowserFields)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private AFComboboxLookup cmbDatasource;
        private DevExpress.XtraGrid.Views.Grid.GridView crComboboxLookup1View;
        private AFRoundRectPainter cR3RoundRectPainter1;
        private AFGridExtender extender;
        private DevExpress.XtraEditors.SeparatorControl separatorControl1;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private AFBarManager crBarManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarCheckItem menShowTree;
        private DevExpress.XtraBars.BarCheckItem menShowGrid;
        private DevExpress.XtraBars.BarButtonItem menRefresh;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private AFSplitContainer grid;
        private AFTreeGrid treeClassStructure;
        private AFGridControl gridDatasourceBrowser;
        private DevExpress.XtraGrid.Views.Grid.GridView viewDatasourceBrowser;
        private AFGridControl gridDatasourceBrowserFields;
        private DevExpress.XtraGrid.Views.Grid.GridView viewDatasourceBrowserFields;
        private DevExpress.XtraBars.StandaloneBarDockControl standaloneBarDockControl1;
        private AFNavSplitter crNavSplitter1;
        private AFLabel mleInfo;
    }
}
