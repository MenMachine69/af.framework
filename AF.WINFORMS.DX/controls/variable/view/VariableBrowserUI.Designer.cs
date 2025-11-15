namespace AF.WINFORMS.DX
{
    partial class VariableBrowserUI
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        internal System.ComponentModel.IContainer components = null;

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
            DevExpress.Utils.SuperToolTip superToolTip5 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem5 = new DevExpress.Utils.ToolTipTitleItem();
            DevExpress.Utils.ToolTipItem toolTipItem5 = new DevExpress.Utils.ToolTipItem();
            DevExpress.Utils.SuperToolTip superToolTip6 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem6 = new DevExpress.Utils.ToolTipTitleItem();
            DevExpress.Utils.ToolTipItem toolTipItem6 = new DevExpress.Utils.ToolTipItem();
            DevExpress.Utils.SuperToolTip superToolTip7 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem7 = new DevExpress.Utils.ToolTipTitleItem();
            DevExpress.Utils.ToolTipItem toolTipItem7 = new DevExpress.Utils.ToolTipItem();
            DevExpress.Utils.SuperToolTip superToolTip8 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem8 = new DevExpress.Utils.ToolTipTitleItem();
            DevExpress.Utils.ToolTipItem toolTipItem8 = new DevExpress.Utils.ToolTipItem();
            this.crBarManager1 = new AF.WINFORMS.DX.AFBarManager();
            this.tbar = new DevExpress.XtraBars.Bar();
            this.menNew = new DevExpress.XtraBars.BarButtonItem();
            this.menMore = new DevExpress.XtraBars.BarSubItem();
            this.menTemplateInsert = new DevExpress.XtraBars.BarButtonItem();
            this.menTemplateSave = new DevExpress.XtraBars.BarButtonItem();
            this.menPreview = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.mleInfo = new AF.WINFORMS.DX.AFLabel();
            this.separatorControl2 = new DevExpress.XtraEditors.SeparatorControl();
            this.gridVariableBrowser = new AF.WINFORMS.DX.AFGridControl();
            this.viewVariableBrowser = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.splitterControl1 = new DevExpress.XtraEditors.SplitterControl();
            this.menCorrect = new DevExpress.XtraBars.BarButtonItem();
            ((System.ComponentModel.ISupportInitialize)(this.crBarManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridVariableBrowser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewVariableBrowser)).BeginInit();
            this.SuspendLayout();
            // 
            // crBarManager1
            // 
            this.crBarManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.tbar});
            this.crBarManager1.DockControls.Add(this.barDockControlTop);
            this.crBarManager1.DockControls.Add(this.barDockControlBottom);
            this.crBarManager1.DockControls.Add(this.barDockControlLeft);
            this.crBarManager1.DockControls.Add(this.barDockControlRight);
            this.crBarManager1.Form = this;
            this.crBarManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.menNew,
            this.menMore,
            this.menTemplateInsert,
            this.menTemplateSave,
            this.menPreview,
            this.menCorrect});
            this.crBarManager1.MaxItemId = 8;
            // 
            // tbar
            // 
            this.tbar.BarName = "Tools";
            this.tbar.DockCol = 0;
            this.tbar.DockRow = 0;
            this.tbar.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.tbar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.menNew),
            new DevExpress.XtraBars.LinkPersistInfo(this.menMore),
            new DevExpress.XtraBars.LinkPersistInfo(this.menPreview)});
            this.tbar.OptionsBar.AllowQuickCustomization = false;
            this.tbar.OptionsBar.DisableClose = true;
            this.tbar.OptionsBar.DisableCustomization = true;
            this.tbar.OptionsBar.DrawBorder = false;
            this.tbar.OptionsBar.DrawDragBorder = false;
            this.tbar.OptionsBar.UseWholeRow = true;
            this.tbar.Text = "Tools";
            // 
            // menNew
            // 
            this.menNew.Caption = "Neue Variable";
            this.menNew.Id = 0;
            this.menNew.Name = "menNew";
            toolTipTitleItem5.Text = "Neue Variable";
            toolTipItem5.Text = "Eine neue Variable erzeugen.";
            superToolTip5.Items.Add(toolTipTitleItem5);
            superToolTip5.Items.Add(toolTipItem5);
            this.menNew.SuperTip = superToolTip5;
            // 
            // menMore
            // 
            this.menMore.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.menMore.DrawMenuSideStrip = DevExpress.Utils.DefaultBoolean.False;
            this.menMore.Id = 1;
            this.menMore.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.menCorrect),
            new DevExpress.XtraBars.LinkPersistInfo(this.menTemplateInsert, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.menTemplateSave)});
            this.menMore.Name = "menMore";
            // 
            // menTemplateInsert
            // 
            this.menTemplateInsert.Caption = "Aus Vorlage einfügen...";
            this.menTemplateInsert.Id = 2;
            this.menTemplateInsert.Name = "menTemplateInsert";
            toolTipTitleItem6.Text = "Variable aus Vorlage";
            toolTipItem6.Text = "Einfügen einer Variablen, die aus einer vorhandenen Vorlage erzeugt wird.";
            superToolTip6.Items.Add(toolTipTitleItem6);
            superToolTip6.Items.Add(toolTipItem6);
            this.menTemplateInsert.SuperTip = superToolTip6;
            // 
            // menTemplateSave
            // 
            this.menTemplateSave.Caption = "Als Vorlage speichern...";
            this.menTemplateSave.Id = 3;
            this.menTemplateSave.Name = "menTemplateSave";
            toolTipTitleItem7.Text = "Als Vorlage speichern";
            toolTipItem7.Text = "Ausgewählte Variable als neue Vorlage speichern. \r\n\r\nGespeicherte Vorlagen können" +
    " an jeder Stelle zum Anlegen neuer Variablen genutzt werden. ";
            superToolTip7.Items.Add(toolTipTitleItem7);
            superToolTip7.Items.Add(toolTipItem7);
            this.menTemplateSave.SuperTip = superToolTip7;
            // 
            // menPreview
            // 
            this.menPreview.Caption = "Vorschau";
            this.menPreview.Id = 6;
            this.menPreview.Name = "menPreview";
            toolTipTitleItem8.Text = "Vorschau Eingabeformular";
            toolTipItem8.Text = "Eine Vorschau des Eingabeformulars anzeigen, dass der Benutzer zur Eingabe der Va" +
    "riablenwerte angezeigt bekommt.";
            superToolTip8.Items.Add(toolTipTitleItem8);
            superToolTip8.Items.Add(toolTipItem8);
            this.menPreview.SuperTip = superToolTip8;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.crBarManager1;
            this.barDockControlTop.Size = new System.Drawing.Size(417, 22);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 499);
            this.barDockControlBottom.Manager = this.crBarManager1;
            this.barDockControlBottom.Size = new System.Drawing.Size(417, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 22);
            this.barDockControlLeft.Manager = this.crBarManager1;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 477);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(417, 22);
            this.barDockControlRight.Manager = this.crBarManager1;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 477);
            // 
            // mleInfo
            // 
            this.mleInfo.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.mleInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.mleInfo.Location = new System.Drawing.Point(0, 419);
            this.mleInfo.Name = "mleInfo";
            this.mleInfo.Size = new System.Drawing.Size(417, 80);
            this.mleInfo.TabIndex = 11;
            // 
            // separatorControl2
            // 
            this.separatorControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.separatorControl2.Location = new System.Drawing.Point(0, 418);
            this.separatorControl2.Name = "separatorControl2";
            this.separatorControl2.Padding = new System.Windows.Forms.Padding(0);
            this.separatorControl2.Size = new System.Drawing.Size(417, 1);
            this.separatorControl2.TabIndex = 13;
            // 
            // gridVariableBrowser
            // 
            this.gridVariableBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridVariableBrowser.Location = new System.Drawing.Point(0, 22);
            this.gridVariableBrowser.MainView = this.viewVariableBrowser;
            this.gridVariableBrowser.Name = "gridVariableBrowser";
            this.gridVariableBrowser.Size = new System.Drawing.Size(417, 386);
            this.gridVariableBrowser.TabIndex = 14;
            this.gridVariableBrowser.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewVariableBrowser});
            // 
            // viewVariableBrowser
            // 
            this.viewVariableBrowser.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.viewVariableBrowser.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.viewVariableBrowser.GridControl = this.gridVariableBrowser;
            this.viewVariableBrowser.Name = "viewVariableBrowser";
            this.viewVariableBrowser.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.viewVariableBrowser.OptionsView.EnableAppearanceEvenRow = true;
            this.viewVariableBrowser.OptionsView.EnableAppearanceOddRow = true;
            this.viewVariableBrowser.OptionsView.ShowGroupPanel = false;
            this.viewVariableBrowser.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
            this.viewVariableBrowser.OptionsView.ShowIndicator = false;
            // 
            // splitterControl1
            // 
            this.splitterControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitterControl1.Location = new System.Drawing.Point(0, 408);
            this.splitterControl1.Name = "splitterControl1";
            this.splitterControl1.ShowSplitGlyph = DevExpress.Utils.DefaultBoolean.True;
            this.splitterControl1.Size = new System.Drawing.Size(417, 10);
            this.splitterControl1.TabIndex = 19;
            this.splitterControl1.TabStop = false;
            // 
            // menCorrect
            // 
            this.menCorrect.Caption = "Reihenfolge korrigieren...";
            this.menCorrect.Id = 7;
            this.menCorrect.Name = "menCorrect";
            // 
            // VariableBrowserUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridVariableBrowser);
            this.Controls.Add(this.splitterControl1);
            this.Controls.Add(this.separatorControl2);
            this.Controls.Add(this.mleInfo);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.DoubleBuffered = true;
            this.Name = "VariableBrowserUI";
            this.Size = new System.Drawing.Size(417, 499);
            ((System.ComponentModel.ISupportInitialize)(this.crBarManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridVariableBrowser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewVariableBrowser)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private AFBarManager crBarManager1;
        private DevExpress.XtraBars.Bar tbar;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        internal AFLabel mleInfo;
        private DevExpress.XtraEditors.SeparatorControl separatorControl2;
        internal AFGridControl gridVariableBrowser;
        internal DevExpress.XtraGrid.Views.Grid.GridView viewVariableBrowser;
        internal DevExpress.XtraBars.BarButtonItem menNew;
        internal DevExpress.XtraBars.BarSubItem menMore;
        internal DevExpress.XtraBars.BarButtonItem menTemplateInsert;
        internal DevExpress.XtraBars.BarButtonItem menTemplateSave;
        internal DevExpress.XtraBars.BarButtonItem menPreview;
        private DevExpress.XtraEditors.SplitterControl splitterControl1;
        internal DevExpress.XtraBars.BarButtonItem menCorrect;
    }
}
