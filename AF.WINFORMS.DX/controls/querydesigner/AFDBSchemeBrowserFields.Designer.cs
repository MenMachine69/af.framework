namespace AF.WINFORMS.DX
{
    partial class AFDBSchemeBrowserFields
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
            this.mleInfo = new AF.WINFORMS.DX.AFEditMultiline();
            this.cR3RoundRectPainter1 = new AF.WINFORMS.DX.AFRoundRectPainter();
            this.gridDatasourceBrowserFields = new AF.WINFORMS.DX.AFGridControl();
            this.viewDatasourceBrowserFields = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.extender = new AF.WINFORMS.DX.AFGridExtender();
            this.separatorControl2 = new DevExpress.XtraEditors.SeparatorControl();
            ((System.ComponentModel.ISupportInitialize)(this.mleInfo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDatasourceBrowserFields)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewDatasourceBrowserFields)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl2)).BeginInit();
            this.SuspendLayout();
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
            this.cR3RoundRectPainter1.AppearanceDefault = appearanceObject1;
            this.cR3RoundRectPainter1.AppearanceDisabled = appearanceObject2;
            this.cR3RoundRectPainter1.AppearanceHover = appearanceObject3;
            this.cR3RoundRectPainter1.AppearanceSelected = appearanceObject4;
            this.cR3RoundRectPainter1.AppearanceSelectedHover = appearanceObject5;
            // 
            // gridDatasourceBrowserFields
            // 
            this.gridDatasourceBrowserFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridDatasourceBrowserFields.Location = new System.Drawing.Point(0, 0);
            this.gridDatasourceBrowserFields.MainView = this.viewDatasourceBrowserFields;
            this.gridDatasourceBrowserFields.Name = "gridDatasourceBrowserFields";
            this.gridDatasourceBrowserFields.Size = new System.Drawing.Size(329, 560);
            this.gridDatasourceBrowserFields.TabIndex = 11;
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
            // extender
            // 
            this.extender.AddCustomColumnsMenu = false;
            this.extender.AddDefaultMenu = false;
            this.extender.ContainerControl = this;
            this.extender.Grid = this.gridDatasourceBrowserFields;
            // 
            // separatorControl2
            // 
            this.separatorControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.separatorControl2.Location = new System.Drawing.Point(0, 560);
            this.separatorControl2.Name = "separatorControl2";
            this.separatorControl2.Padding = new System.Windows.Forms.Padding(0);
            this.separatorControl2.Size = new System.Drawing.Size(329, 1);
            this.separatorControl2.TabIndex = 13;
            // 
            // AFDBSchemeBrowserFields
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridDatasourceBrowserFields);
            this.Controls.Add(this.separatorControl2);
            this.Controls.Add(this.mleInfo);
            this.DoubleBuffered = true;
            this.Name = "AFDBSchemeBrowserFields";
            this.Size = new System.Drawing.Size(329, 625);
            ((System.ComponentModel.ISupportInitialize)(this.mleInfo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDatasourceBrowserFields)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewDatasourceBrowserFields)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private AFEditMultiline mleInfo;
        private AFRoundRectPainter cR3RoundRectPainter1;
        private AFGridControl gridDatasourceBrowserFields;
        private DevExpress.XtraGrid.Views.Grid.GridView viewDatasourceBrowserFields;
        private AFGridExtender extender;
        private DevExpress.XtraEditors.SeparatorControl separatorControl2;
    }
}
