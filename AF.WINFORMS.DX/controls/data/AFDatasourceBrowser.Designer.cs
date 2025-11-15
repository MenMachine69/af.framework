namespace AF.WINFORMS.DX
{
    partial class AFDatasourceBrowser
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
            this.mleInfo = new AF.WINFORMS.DX.AFLabel();
            this.separatorControl2 = new DevExpress.XtraEditors.SeparatorControl();
            this.gridDatasourceBrowser = new AF.WINFORMS.DX.AFGridControl();
            this.viewDatasourceBrowser = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.splitterControl1 = new DevExpress.XtraEditors.SplitterControl();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDatasourceBrowser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewDatasourceBrowser)).BeginInit();
            this.SuspendLayout();
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
            // gridDatasourceBrowser
            // 
            this.gridDatasourceBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridDatasourceBrowser.Location = new System.Drawing.Point(0, 0);
            this.gridDatasourceBrowser.MainView = this.viewDatasourceBrowser;
            this.gridDatasourceBrowser.Name = "gridDatasourceBrowser";
            this.gridDatasourceBrowser.Size = new System.Drawing.Size(417, 408);
            this.gridDatasourceBrowser.TabIndex = 14;
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
            // splitterControl1
            // 
            this.splitterControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitterControl1.Location = new System.Drawing.Point(0, 408);
            this.splitterControl1.Name = "splitterControl1";
            this.splitterControl1.ShowSplitGlyph = DevExpress.Utils.DefaultBoolean.True;
            this.splitterControl1.Size = new System.Drawing.Size(417, 10);
            this.splitterControl1.TabIndex = 15;
            this.splitterControl1.TabStop = false;
            // 
            // AFDatasourceBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridDatasourceBrowser);
            this.Controls.Add(this.splitterControl1);
            this.Controls.Add(this.separatorControl2);
            this.Controls.Add(this.mleInfo);
            this.DoubleBuffered = true;
            this.Name = "AFDatasourceBrowser";
            this.Size = new System.Drawing.Size(417, 499);
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDatasourceBrowser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewDatasourceBrowser)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private AFLabel mleInfo;
        private DevExpress.XtraEditors.SeparatorControl separatorControl2;
        private AFGridControl gridDatasourceBrowser;
        private DevExpress.XtraGrid.Views.Grid.GridView viewDatasourceBrowser;
        private DevExpress.XtraEditors.SplitterControl splitterControl1;
    }
}
