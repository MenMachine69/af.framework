namespace AF.WINFORMS.DX
{
    partial class AFDBSchemeTreeBrowser
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
            this.cmbClass = new AF.WINFORMS.DX.AFComboboxLookup();
            this.crComboboxLookup1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.cR3RoundRectPainter1 = new AF.WINFORMS.DX.AFRoundRectPainter();
            this.separatorControl1 = new DevExpress.XtraEditors.SeparatorControl();
            this.treeClassStructure = new AF.WINFORMS.DX.AFTreeGrid();
            this.mleInfo = new AF.WINFORMS.DX.AFEditMultiline();
            ((System.ComponentModel.ISupportInitialize)(this.cmbClass.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.crComboboxLookup1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeClassStructure)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mleInfo.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbClass
            // 
            this.cmbClass.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmbClass.Location = new System.Drawing.Point(0, 0);
            this.cmbClass.Name = "cmbClass";
            this.cmbClass.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbClass.Properties.PopupView = this.crComboboxLookup1View;
            this.cmbClass.Size = new System.Drawing.Size(357, 20);
            this.cmbClass.TabIndex = 0;
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
            this.cR3RoundRectPainter1.AppearanceDefault = appearanceObject6;
            this.cR3RoundRectPainter1.AppearanceDisabled = appearanceObject7;
            this.cR3RoundRectPainter1.AppearanceHover = appearanceObject8;
            this.cR3RoundRectPainter1.AppearanceSelected = appearanceObject9;
            this.cR3RoundRectPainter1.AppearanceSelectedHover = appearanceObject10;
            // 
            // separatorControl1
            // 
            this.separatorControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.separatorControl1.Location = new System.Drawing.Point(0, 454);
            this.separatorControl1.Name = "separatorControl1";
            this.separatorControl1.Padding = new System.Windows.Forms.Padding(0);
            this.separatorControl1.Size = new System.Drawing.Size(357, 1);
            this.separatorControl1.TabIndex = 3;
            // 
            // treeClassStructure
            // 
            this.treeClassStructure.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.treeClassStructure.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeClassStructure.Location = new System.Drawing.Point(0, 20);
            this.treeClassStructure.Name = "treeClassStructure";
            this.treeClassStructure.Size = new System.Drawing.Size(357, 434);
            this.treeClassStructure.TabIndex = 4;
            // 
            // mleInfo
            // 
            this.mleInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.mleInfo.Location = new System.Drawing.Point(0, 455);
            this.mleInfo.Name = "mleInfo";
            this.mleInfo.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.mleInfo.Properties.ReadOnly = true;
            this.mleInfo.Properties.UseReadOnlyAppearance = false;
            this.mleInfo.Size = new System.Drawing.Size(357, 64);
            this.mleInfo.TabIndex = 6;
            // 
            // AFDBSchemeTreeBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeClassStructure);
            this.Controls.Add(this.separatorControl1);
            this.Controls.Add(this.cmbClass);
            this.Controls.Add(this.mleInfo);
            this.DoubleBuffered = true;
            this.Name = "AFDBSchemeTreeBrowser";
            this.Size = new System.Drawing.Size(357, 519);
            ((System.ComponentModel.ISupportInitialize)(this.cmbClass.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.crComboboxLookup1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeClassStructure)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mleInfo.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AFComboboxLookup cmbClass;
        private DevExpress.XtraGrid.Views.Grid.GridView crComboboxLookup1View;
        private AFRoundRectPainter cR3RoundRectPainter1;
        private DevExpress.XtraEditors.SeparatorControl separatorControl1;
        private AFTreeGrid treeClassStructure;
        private AFEditMultiline mleInfo;
    }
}
