namespace AF.MVC
{
    partial class AFFilterArchived
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
            this.chkSYS_ARCHIVED = new DevExpress.XtraEditors.CheckEdit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSYS_ARCHIVED.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // chkSYS_ARCHIVED
            // 
            this.chkSYS_ARCHIVED.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkSYS_ARCHIVED.Location = new System.Drawing.Point(0, 0);
            this.chkSYS_ARCHIVED.Name = "chkSYS_ARCHIVED";
            this.chkSYS_ARCHIVED.Properties.Caption = "archivierte anzeigen";
            this.chkSYS_ARCHIVED.Size = new System.Drawing.Size(181, 30);
            this.chkSYS_ARCHIVED.TabIndex = 0;
            // 
            // AFFilterArchived
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkSYS_ARCHIVED);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(0, 30);
            this.Name = "AFFilterArchived";
            this.Size = new System.Drawing.Size(181, 30);
            ((System.ComponentModel.ISupportInitialize)(this.chkSYS_ARCHIVED.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.CheckEdit chkSYS_ARCHIVED;
    }
}
