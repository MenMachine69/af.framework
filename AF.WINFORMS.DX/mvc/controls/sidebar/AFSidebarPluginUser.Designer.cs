namespace AF.MVC
{
    partial class AFSidebarPluginUser
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
            tableLayout = new DevExpress.Utils.Layout.TablePanel();
            pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
            ((System.ComponentModel.ISupportInitialize)tableLayout).BeginInit();
            tableLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureEdit1.Properties).BeginInit();
            SuspendLayout();
            // 
            // tableLayout
            // 
            tableLayout.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn[] { new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 5F), new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 100F) });
            tableLayout.Controls.Add(pictureEdit1);
            tableLayout.Dock = DockStyle.Fill;
            tableLayout.Location = new Point(0, 0);
            tableLayout.Name = "tableLayout";
            tableLayout.Rows.AddRange(new DevExpress.Utils.Layout.TablePanelRow[] { new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 26F), new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 26F) });
            tableLayout.Size = new Size(343, 93);
            tableLayout.TabIndex = 0;
            // 
            // pictureEdit1
            // 
            tableLayout.SetColumn(pictureEdit1, 0);
            pictureEdit1.Location = new Point(0, 8);
            pictureEdit1.Margin = new Padding(0, 8, 8, 8);
            pictureEdit1.Name = "pictureEdit1";
            pictureEdit1.Properties.Appearance.BackColor = Color.Transparent;
            pictureEdit1.Properties.Appearance.Options.UseBackColor = true;
            pictureEdit1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            pictureEdit1.Properties.OptionsMask.Margin = new Padding(0, 3, 6, 3);
            pictureEdit1.Properties.OptionsMask.MaskType = DevExpress.XtraEditors.Controls.PictureEditMaskType.Circle;
            pictureEdit1.Properties.ReadOnly = true;
            pictureEdit1.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            pictureEdit1.Properties.ShowMenu = false;
            pictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
            pictureEdit1.Properties.SvgImageColorizationMode = DevExpress.Utils.SvgImageColorizationMode.None;
            pictureEdit1.Properties.ZoomingOperationMode = DevExpress.XtraEditors.Repository.ZoomingOperationMode.ControlMouseWheel;
            tableLayout.SetRow(pictureEdit1, 0);
            tableLayout.SetRowSpan(pictureEdit1, 2);
            pictureEdit1.Size = new Size(77, 77);
            pictureEdit1.TabIndex = 0;
            // 
            // SidebarPluginUser
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayout);
            Name = "AFSidebarPluginUser";
            Size = new Size(343, 93);
            ((System.ComponentModel.ISupportInitialize)tableLayout).EndInit();
            tableLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureEdit1.Properties).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.Utils.Layout.TablePanel tableLayout;
        private DevExpress.XtraEditors.PictureEdit pictureEdit1;
    }
}
