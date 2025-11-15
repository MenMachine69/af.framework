namespace AF.WINFORMS.DX
{
    partial class AFDBSchemeTableBrowser
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
            DevExpress.Utils.AppearanceObject appearanceObject6 = new DevExpress.Utils.AppearanceObject();
            DevExpress.Utils.AppearanceObject appearanceObject7 = new DevExpress.Utils.AppearanceObject();
            DevExpress.Utils.AppearanceObject appearanceObject8 = new DevExpress.Utils.AppearanceObject();
            DevExpress.Utils.AppearanceObject appearanceObject9 = new DevExpress.Utils.AppearanceObject();
            DevExpress.Utils.AppearanceObject appearanceObject10 = new DevExpress.Utils.AppearanceObject();
            this.cmbDatasource = new AF.WINFORMS.DX.AFComboboxLookup();
            this.crComboboxLookup1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.mleInfo = new AF.WINFORMS.DX.AFEditMultiline();
            this.cR3RoundRectPainter1 = new AF.WINFORMS.DX.AFRoundRectPainter();
            this.gridDatasourceBrowser = new AF.WINFORMS.DX.AFGridControl();
            this.viewDatasourceBrowser = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.extender = new AF.WINFORMS.DX.AFGridExtender();
            this.separatorControl2 = new DevExpress.XtraEditors.SeparatorControl();
            ((System.ComponentModel.ISupportInitialize)(this.cmbDatasource.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.crComboboxLookup1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mleInfo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDatasourceBrowser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewDatasourceBrowser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl2)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbDatasource
            // 
            this.cmbDatasource.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmbDatasource.Location = new System.Drawing.Point(0, 0);
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
            // mleInfo
            // 
            this.mleInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.mleInfo.Location = new System.Drawing.Point(0, 561);
            this.mleInfo.Name = "mleInfo";
            this.mleInfo.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.mleInfo.Properties.ReadOnly = true;
            this.mleInfo.Properties.UseReadOnlyAppearance = false;
            this.mleInfo.Size = new System.Drawing.Size(329, 64);
            this.mleInfo.TabIndex = 10;
            // 
            // cR3RoundRectPainter1
            // 
            this.cR3RoundRectPainter1.AppearanceDefault = appearanceObject6;
            this.cR3RoundRectPainter1.AppearanceDisabled = appearanceObject7;
            this.cR3RoundRectPainter1.AppearanceHover = appearanceObject8;
            this.cR3RoundRectPainter1.AppearanceSelected = appearanceObject9;
            this.cR3RoundRectPainter1.AppearanceSelectedHover = appearanceObject10;
            // 
            // gridDatasourceBrowser
            // 
            this.gridDatasourceBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridDatasourceBrowser.Location = new System.Drawing.Point(0, 20);
            this.gridDatasourceBrowser.MainView = this.viewDatasourceBrowser;
            this.gridDatasourceBrowser.Name = "gridDatasourceBrowser";
            this.gridDatasourceBrowser.Size = new System.Drawing.Size(329, 540);
            this.gridDatasourceBrowser.TabIndex = 11;
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
            // extender
            // 
            this.extender.AddCustomColumnsMenu = false;
            this.extender.AddDefaultMenu = false;
            this.extender.ContainerControl = this;
            this.extender.Grid = this.gridDatasourceBrowser;
            // 
            // separatorControl2
            // 
            this.separatorControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.separatorControl2.Location = new System.Drawing.Point(0, 560);
            this.separatorControl2.Name = "separatorControl2";
            this.separatorControl2.Padding = new System.Windows.Forms.Padding(0);
            this.separatorControl2.Size = new System.Drawing.Size(329, 1);
            this.separatorControl2.TabIndex = 12;
            // 
            // AFDBSchemeBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridDatasourceBrowser);
            this.Controls.Add(this.separatorControl2);
            this.Controls.Add(this.cmbDatasource);
            this.Controls.Add(this.mleInfo);
            this.DoubleBuffered = true;
            this.Name = "AFDBSchemeBrowser";
            this.Size = new System.Drawing.Size(329, 625);
            ((System.ComponentModel.ISupportInitialize)(this.cmbDatasource.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.crComboboxLookup1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mleInfo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDatasourceBrowser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewDatasourceBrowser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private AFComboboxLookup cmbDatasource;
        private DevExpress.XtraGrid.Views.Grid.GridView crComboboxLookup1View;
        private AFEditMultiline mleInfo;
        private AFRoundRectPainter cR3RoundRectPainter1;
        private AFGridControl gridDatasourceBrowser;
        private DevExpress.XtraGrid.Views.Grid.GridView viewDatasourceBrowser;
        private AFGridExtender extender;
        private DevExpress.XtraEditors.SeparatorControl separatorControl2;
    }
}
