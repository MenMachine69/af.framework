namespace AF.WINFORMS.DX
{
    partial class AFDocumentDesignerExport
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
            DevExpress.Utils.SuperToolTip superToolTip1 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem1 = new DevExpress.Utils.ToolTipTitleItem();
            DevExpress.Utils.ToolTipItem toolTipItem1 = new DevExpress.Utils.ToolTipItem();
            this.headerFooterRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.HeaderFooterRibbonPageGroup();
            this.symbolsRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.SymbolsRibbonPageGroup();
            this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager(this.components);
            this.crBarManager1 = new AF.WINFORMS.DX.AFBarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.menPreview = new DevExpress.XtraBars.BarButtonItem();
            this.standaloneBarDockControl1 = new DevExpress.XtraBars.StandaloneBarDockControl();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.panelContainer1 = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockVariablen = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.dockDatasource = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel2_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.mleFooter = new AF.WINFORMS.DX.AFEditMultiline();
            this.mleData = new AF.WINFORMS.DX.AFEditMultiline();
            this.lblHeaderDetail = new AF.WINFORMS.DX.AFLabelBoldText();
            this.lblFooter = new AF.WINFORMS.DX.AFLabelBoldText();
            this.splitterControl1 = new DevExpress.XtraEditors.SplitterControl();
            this.splitterControl2 = new DevExpress.XtraEditors.SplitterControl();
            this.crPanel1 = new AF.WINFORMS.DX.AFPanel();
            this.mleHeader = new AF.WINFORMS.DX.AFEditMultiline();
            this.lblHeaderMaster = new AF.WINFORMS.DX.AFLabelBoldText();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.crBarManager1)).BeginInit();
            this.panelContainer1.SuspendLayout();
            this.dockVariablen.SuspendLayout();
            this.dockDatasource.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mleFooter.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mleData.Properties)).BeginInit();
            this.crPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mleHeader.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // headerFooterRibbonPageGroup1
            // 
            this.headerFooterRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            this.headerFooterRibbonPageGroup1.Name = "headerFooterRibbonPageGroup1";
            this.headerFooterRibbonPageGroup1.Text = "";
            // 
            // symbolsRibbonPageGroup1
            // 
            this.symbolsRibbonPageGroup1.AllowTextClipping = false;
            this.symbolsRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            this.symbolsRibbonPageGroup1.Name = "symbolsRibbonPageGroup1";
            this.symbolsRibbonPageGroup1.Text = "";
            // 
            // dockManager1
            // 
            this.dockManager1.Form = this;
            this.dockManager1.MenuManager = this.crBarManager1;
            this.dockManager1.RootPanels.AddRange(new DevExpress.XtraBars.Docking.DockPanel[] {
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
            this.menPreview});
            this.crBarManager1.MaxItemId = 1;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Standalone;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.menPreview)});
            this.bar1.OptionsBar.AllowQuickCustomization = false;
            this.bar1.OptionsBar.DisableClose = true;
            this.bar1.OptionsBar.DisableCustomization = true;
            this.bar1.OptionsBar.DrawBorder = false;
            this.bar1.OptionsBar.DrawDragBorder = false;
            this.bar1.OptionsBar.UseWholeRow = true;
            this.bar1.StandaloneBarDockControl = this.standaloneBarDockControl1;
            this.bar1.Text = "Tools";
            // 
            // menPreview
            // 
            this.menPreview.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.menPreview.Caption = "preview";
            this.menPreview.Id = 0;
            this.menPreview.ImageOptions.SvgImageSize = new System.Drawing.Size(16, 16);
            this.menPreview.Name = "menPreview";
            toolTipTitleItem1.Text = "VORSCHAU";
            toolTipItem1.Text = "Zeigt eine Vorschau des Dokuments an.";
            superToolTip1.Items.Add(toolTipTitleItem1);
            superToolTip1.Items.Add(toolTipItem1);
            this.menPreview.SuperTip = superToolTip1;
            // 
            // standaloneBarDockControl1
            // 
            this.standaloneBarDockControl1.AutoSize = true;
            this.standaloneBarDockControl1.CausesValidation = false;
            this.standaloneBarDockControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.standaloneBarDockControl1.Location = new System.Drawing.Point(0, 0);
            this.standaloneBarDockControl1.Manager = this.crBarManager1;
            this.standaloneBarDockControl1.Name = "standaloneBarDockControl1";
            this.standaloneBarDockControl1.Size = new System.Drawing.Size(1501, 20);
            this.standaloneBarDockControl1.Text = "standaloneBarDockControl1";
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.crBarManager1;
            this.barDockControlTop.Size = new System.Drawing.Size(1701, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 553);
            this.barDockControlBottom.Manager = this.crBarManager1;
            this.barDockControlBottom.Size = new System.Drawing.Size(1701, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
            this.barDockControlLeft.Manager = this.crBarManager1;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 553);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1701, 0);
            this.barDockControlRight.Manager = this.crBarManager1;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 553);
            // 
            // panelContainer1
            // 
            this.panelContainer1.ActiveChild = this.dockVariablen;
            this.panelContainer1.Controls.Add(this.dockDatasource);
            this.panelContainer1.Controls.Add(this.dockVariablen);
            this.panelContainer1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Left;
            this.panelContainer1.ID = new System.Guid("81190e41-89f7-4fed-913d-d2a5aae94602");
            this.panelContainer1.Location = new System.Drawing.Point(0, 0);
            this.panelContainer1.Name = "panelContainer1";
            this.panelContainer1.OriginalSize = new System.Drawing.Size(200, 200);
            this.panelContainer1.Size = new System.Drawing.Size(200, 553);
            this.panelContainer1.Tabbed = true;
            this.panelContainer1.Text = "panelContainer1";
            // 
            // dockVariablen
            // 
            this.dockVariablen.Controls.Add(this.dockPanel1_Container);
            this.dockVariablen.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
            this.dockVariablen.ID = new System.Guid("af6e72bb-799b-4685-bf8a-5ff81fe85c33");
            this.dockVariablen.Location = new System.Drawing.Point(0, 23);
            this.dockVariablen.Name = "dockVariablen";
            this.dockVariablen.Options.AllowDockAsTabbedDocument = false;
            this.dockVariablen.Options.AllowDockBottom = false;
            this.dockVariablen.Options.AllowDockTop = false;
            this.dockVariablen.Options.AllowFloating = false;
            this.dockVariablen.Options.FloatOnDblClick = false;
            this.dockVariablen.Options.ShowCloseButton = false;
            this.dockVariablen.OriginalSize = new System.Drawing.Size(199, 504);
            this.dockVariablen.Size = new System.Drawing.Size(199, 504);
            this.dockVariablen.Text = "Variablen";
            // 
            // dockPanel1_Container
            // 
            this.dockPanel1_Container.Location = new System.Drawing.Point(0, 0);
            this.dockPanel1_Container.Name = "dockPanel1_Container";
            this.dockPanel1_Container.Size = new System.Drawing.Size(199, 504);
            this.dockPanel1_Container.TabIndex = 0;
            // 
            // dockDatasource
            // 
            this.dockDatasource.Controls.Add(this.dockPanel2_Container);
            this.dockDatasource.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
            this.dockDatasource.ID = new System.Guid("f4923e4c-53c2-4c10-9d10-6c511fea89d5");
            this.dockDatasource.Location = new System.Drawing.Point(0, 23);
            this.dockDatasource.Name = "dockDatasource";
            this.dockDatasource.Options.AllowDockAsTabbedDocument = false;
            this.dockDatasource.Options.AllowDockBottom = false;
            this.dockDatasource.Options.AllowDockTop = false;
            this.dockDatasource.Options.AllowFloating = false;
            this.dockDatasource.Options.FloatOnDblClick = false;
            this.dockDatasource.Options.ShowCloseButton = false;
            this.dockDatasource.OriginalSize = new System.Drawing.Size(199, 504);
            this.dockDatasource.Size = new System.Drawing.Size(199, 504);
            this.dockDatasource.Text = "Datenquelle";
            // 
            // dockPanel2_Container
            // 
            this.dockPanel2_Container.Location = new System.Drawing.Point(0, 0);
            this.dockPanel2_Container.Name = "dockPanel2_Container";
            this.dockPanel2_Container.Size = new System.Drawing.Size(199, 504);
            this.dockPanel2_Container.TabIndex = 0;
            // 
            // mleFooter
            // 
            this.mleFooter.AllowDrop = true;
            this.mleFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.mleFooter.Location = new System.Drawing.Point(200, 457);
            this.mleFooter.Name = "mleFooter";
            this.mleFooter.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.mleFooter.Size = new System.Drawing.Size(1501, 96);
            this.mleFooter.TabIndex = 8;
            // 
            // mleData
            // 
            this.mleData.AllowDrop = true;
            this.mleData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mleData.Location = new System.Drawing.Point(200, 181);
            this.mleData.Name = "mleData";
            this.mleData.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.mleData.Size = new System.Drawing.Size(1501, 245);
            this.mleData.TabIndex = 9;
            // 
            // lblHeaderDetail
            // 
            this.lblHeaderDetail.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblHeaderDetail.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblHeaderDetail.Location = new System.Drawing.Point(200, 160);
            this.lblHeaderDetail.Name = "lblHeaderDetail";
            this.lblHeaderDetail.Padding = new System.Windows.Forms.Padding(4);
            this.lblHeaderDetail.Size = new System.Drawing.Size(1501, 21);
            this.lblHeaderDetail.Style = "Bold";
            this.lblHeaderDetail.TabIndex = 10;
            this.lblHeaderDetail.Text = "Daten";
            // 
            // lblFooter
            // 
            this.lblFooter.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblFooter.Location = new System.Drawing.Point(200, 436);
            this.lblFooter.Name = "lblFooter";
            this.lblFooter.Padding = new System.Windows.Forms.Padding(4);
            this.lblFooter.Size = new System.Drawing.Size(1501, 21);
            this.lblFooter.Style = "Bold";
            this.lblFooter.TabIndex = 11;
            this.lblFooter.Text = "Fuss";
            // 
            // splitterControl1
            // 
            this.splitterControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitterControl1.Location = new System.Drawing.Point(200, 150);
            this.splitterControl1.Name = "splitterControl1";
            this.splitterControl1.ShowSplitGlyph = DevExpress.Utils.DefaultBoolean.True;
            this.splitterControl1.Size = new System.Drawing.Size(1501, 10);
            this.splitterControl1.TabIndex = 13;
            this.splitterControl1.TabStop = false;
            // 
            // splitterControl2
            // 
            this.splitterControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitterControl2.Location = new System.Drawing.Point(200, 426);
            this.splitterControl2.Name = "splitterControl2";
            this.splitterControl2.ShowSplitGlyph = DevExpress.Utils.DefaultBoolean.True;
            this.splitterControl2.Size = new System.Drawing.Size(1501, 10);
            this.splitterControl2.TabIndex = 14;
            this.splitterControl2.TabStop = false;
            // 
            // crPanel1
            // 
            this.crPanel1.Controls.Add(this.mleHeader);
            this.crPanel1.Controls.Add(this.lblHeaderMaster);
            this.crPanel1.Controls.Add(this.standaloneBarDockControl1);
            this.crPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.crPanel1.Location = new System.Drawing.Point(200, 0);
            this.crPanel1.Name = "crPanel1";
            this.crPanel1.Size = new System.Drawing.Size(1501, 150);
            this.crPanel1.TabIndex = 20;
            // 
            // mleHeader
            // 
            this.mleHeader.AllowDrop = true;
            this.mleHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mleHeader.Location = new System.Drawing.Point(0, 41);
            this.mleHeader.Name = "mleHeader";
            this.mleHeader.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.mleHeader.Size = new System.Drawing.Size(1501, 109);
            this.mleHeader.TabIndex = 9;
            // 
            // lblHeaderMaster
            // 
            this.lblHeaderMaster.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblHeaderMaster.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblHeaderMaster.Location = new System.Drawing.Point(0, 20);
            this.lblHeaderMaster.Name = "lblHeaderMaster";
            this.lblHeaderMaster.Padding = new System.Windows.Forms.Padding(4);
            this.lblHeaderMaster.Size = new System.Drawing.Size(1501, 21);
            this.lblHeaderMaster.Style = "Bold";
            this.lblHeaderMaster.TabIndex = 8;
            this.lblHeaderMaster.Text = "Kopf";
            // 
            // AFDocumentDesignerExport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mleData);
            this.Controls.Add(this.lblHeaderDetail);
            this.Controls.Add(this.splitterControl1);
            this.Controls.Add(this.crPanel1);
            this.Controls.Add(this.splitterControl2);
            this.Controls.Add(this.lblFooter);
            this.Controls.Add(this.mleFooter);
            this.Controls.Add(this.panelContainer1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.DoubleBuffered = true;
            this.Name = "AFDocumentDesignerExport";
            this.Size = new System.Drawing.Size(1701, 553);
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.crBarManager1)).EndInit();
            this.panelContainer1.ResumeLayout(false);
            this.dockVariablen.ResumeLayout(false);
            this.dockDatasource.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mleFooter.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mleData.Properties)).EndInit();
            this.crPanel1.ResumeLayout(false);
            this.crPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mleHeader.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraRichEdit.UI.HeaderFooterRibbonPageGroup headerFooterRibbonPageGroup1;
        private DevExpress.XtraRichEdit.UI.SymbolsRibbonPageGroup symbolsRibbonPageGroup1;
        private DevExpress.XtraBars.Docking.DockManager dockManager1;
        private DevExpress.XtraBars.Docking.DockPanel panelContainer1;
        private DevExpress.XtraBars.Docking.DockPanel dockDatasource;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel2_Container;
        private DevExpress.XtraBars.Docking.DockPanel dockVariablen;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;
        private AFEditMultiline mleData;
        private AFLabelBoldText lblFooter;
        private AFLabelBoldText lblHeaderDetail;
        private AFEditMultiline mleFooter;
        private DevExpress.XtraEditors.SplitterControl splitterControl2;
        private DevExpress.XtraEditors.SplitterControl splitterControl1;
        private DevExpress.XtraBars.StandaloneBarDockControl standaloneBarDockControl1;
        private AFBarManager crBarManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem menPreview;
        private AFPanel crPanel1;
        private AFEditMultiline mleHeader;
        private AFLabelBoldText lblHeaderMaster;
    }
}
