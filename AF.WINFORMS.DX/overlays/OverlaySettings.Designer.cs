using System.Drawing;

namespace AF.WINFORMS.DX
{
    partial class OverlaySettings
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpSettings = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.accordionControlElement5 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.accordionControlElement1 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.accordionControlElement6 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.accordionControlElement7 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.accordionControl1 = new DevExpress.XtraBars.Navigation.AccordionControl();
            this.grpFunctions = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.accordionControlSeparator2 = new DevExpress.XtraBars.Navigation.AccordionControlSeparator();
            this.menSettingsExport = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.menSettingsImport = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.accordionControlSeparator1 = new DevExpress.XtraBars.Navigation.AccordionControlSeparator();
            this.menSettingsClose = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.tableContent = new DevExpress.Utils.Layout.TablePanel();
            this.pshSaveSettings = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.accordionControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableContent)).BeginInit();
            this.tableContent.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpSettings
            // 
            this.grpSettings.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[] {
            this.accordionControlElement5,
            this.accordionControlElement1,
            this.accordionControlElement6,
            this.accordionControlElement7});
            this.grpSettings.Expanded = true;
            this.grpSettings.Name = "grpSettings";
            this.grpSettings.Text = "EINSTELLUNGEN";
            // 
            // accordionControlElement5
            // 
            this.accordionControlElement5.Appearance.Normal.FontSizeDelta = 1;
            this.accordionControlElement5.Appearance.Normal.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.accordionControlElement5.Appearance.Normal.Options.UseFont = true;
            this.accordionControlElement5.Appearance.Pressed.FontSizeDelta = 1;
            this.accordionControlElement5.Appearance.Pressed.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.accordionControlElement5.Appearance.Pressed.Options.UseFont = true;
            this.accordionControlElement5.Name = "accordionControlElement5";
            this.accordionControlElement5.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.accordionControlElement5.Text = "Element5";
            // 
            // accordionControlElement1
            // 
            this.accordionControlElement1.Name = "accordionControlElement1";
            this.accordionControlElement1.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.accordionControlElement1.Text = "Element1";
            // 
            // accordionControlElement6
            // 
            this.accordionControlElement6.Appearance.Normal.FontSizeDelta = 1;
            this.accordionControlElement6.Appearance.Normal.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.accordionControlElement6.Appearance.Normal.Options.UseFont = true;
            this.accordionControlElement6.Name = "accordionControlElement6";
            this.accordionControlElement6.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.accordionControlElement6.Text = "Element6";
            // 
            // accordionControlElement7
            // 
            this.accordionControlElement7.Appearance.Normal.FontSizeDelta = 1;
            this.accordionControlElement7.Appearance.Normal.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.accordionControlElement7.Appearance.Normal.Options.UseFont = true;
            this.accordionControlElement7.Name = "accordionControlElement7";
            this.accordionControlElement7.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.accordionControlElement7.Text = "Element7";
            // 
            // accordionControl1
            // 
            this.accordionControl1.AllowHtmlText = false;
            this.accordionControl1.AllowItemSelection = true;
            this.accordionControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.accordionControl1.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[] {
            this.grpSettings,
            this.grpFunctions});
            this.accordionControl1.ExpandElementMode = DevExpress.XtraBars.Navigation.ExpandElementMode.Multiple;
            this.accordionControl1.ExpandGroupOnHeaderClick = false;
            this.accordionControl1.Location = new System.Drawing.Point(0, 0);
            this.accordionControl1.Name = "accordionControl1";
            this.accordionControl1.OptionsFooter.ActiveGroupDisplayMode = DevExpress.XtraBars.Navigation.ActiveGroupDisplayMode.GroupHeaderAndContent;
            this.accordionControl1.ResizeMode = DevExpress.XtraBars.Navigation.AccordionControlResizeMode.InnerResizeZone;
            this.accordionControl1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.accordionControl1.ScrollBarMode = DevExpress.XtraBars.Navigation.ScrollBarMode.Hidden;
            this.accordionControl1.ShowGroupExpandButtons = false;
            this.accordionControl1.Size = new System.Drawing.Size(258, 578);
            this.accordionControl1.TabIndex = 1;
            this.accordionControl1.ViewType = DevExpress.XtraBars.Navigation.AccordionControlViewType.HamburgerMenu;
            // 
            // grpFunctions
            // 
            this.grpFunctions.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[] {
            this.accordionControlSeparator2,
            this.menSettingsExport,
            this.menSettingsImport,
            this.accordionControlSeparator1,
            this.menSettingsClose});
            this.grpFunctions.Expanded = true;
            this.grpFunctions.HeaderVisible = false;
            this.grpFunctions.Name = "grpFunctions";
            this.grpFunctions.Text = "Element2";
            // 
            // accordionControlSeparator2
            // 
            this.accordionControlSeparator2.Name = "accordionControlSeparator2";
            // 
            // menSettingsExport
            // 
            this.menSettingsExport.Name = "menSettingsExport";
            this.menSettingsExport.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.menSettingsExport.Text = "Einstellungen exportieren...";
            // 
            // menSettingsImport
            // 
            this.menSettingsImport.Name = "menSettingsImport";
            this.menSettingsImport.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.menSettingsImport.Text = "Einstellungen importieren...";
            // 
            // accordionControlSeparator1
            // 
            this.accordionControlSeparator1.Name = "accordionControlSeparator1";
            // 
            // menSettingsClose
            // 
            this.menSettingsClose.Name = "menSettingsClose";
            this.menSettingsClose.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.menSettingsClose.Text = "Einstellungen schließen";
            this.menSettingsClose.VisibleInFooter = false;
            // 
            // tableContent
            // 
            this.tableContent.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn[] {
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 100F)});
            this.tableContent.Controls.Add(this.pshSaveSettings);
            this.tableContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableContent.Location = new System.Drawing.Point(258, 0);
            this.tableContent.Name = "tableContent";
            this.tableContent.Rows.AddRange(new DevExpress.Utils.Layout.TablePanelRow[] {
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 100F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 30F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 10F)});
            this.tableContent.Size = new System.Drawing.Size(690, 578);
            this.tableContent.TabIndex = 2;
            // 
            // pshSaveSettings
            // 
            this.pshSaveSettings.Appearance.BackColor = DevExpress.LookAndFeel.DXSkinColors.FillColors.Success;
            this.pshSaveSettings.Appearance.Options.UseBackColor = true;
            this.tableContent.SetColumn(this.pshSaveSettings, 0);
            this.pshSaveSettings.Dock = System.Windows.Forms.DockStyle.Right;
            this.pshSaveSettings.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.pshSaveSettings.ImageOptions.ImageToTextIndent = 10;
            this.pshSaveSettings.ImageOptions.SvgImageSize = new System.Drawing.Size(16, 16);
            this.pshSaveSettings.Location = new System.Drawing.Point(556, 541);
            this.pshSaveSettings.Margin = new System.Windows.Forms.Padding(3, 3, 15, 3);
            this.pshSaveSettings.Name = "pshSaveSettings";
            this.pshSaveSettings.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.tableContent.SetRow(this.pshSaveSettings, 1);
            this.pshSaveSettings.Size = new System.Drawing.Size(119, 24);
            this.pshSaveSettings.TabIndex = 0;
            this.pshSaveSettings.Text = "Speichern";
            // 
            // OverlaySettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableContent);
            this.Controls.Add(this.accordionControl1);
            this.Name = "OverlaySettings";
            this.Size = new System.Drawing.Size(948, 578);
            ((System.ComponentModel.ISupportInitialize)(this.accordionControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableContent)).EndInit();
            this.tableContent.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.Navigation.AccordionControlElement grpSettings;
        private DevExpress.XtraBars.Navigation.AccordionControlElement accordionControlElement5;
        private DevExpress.XtraBars.Navigation.AccordionControlElement accordionControlElement6;
        private DevExpress.XtraBars.Navigation.AccordionControlElement accordionControlElement7;
        private DevExpress.XtraBars.Navigation.AccordionControl accordionControl1;
        private DevExpress.XtraBars.Navigation.AccordionControlElement grpFunctions;
        private DevExpress.XtraBars.Navigation.AccordionControlElement menSettingsExport;
        private DevExpress.XtraBars.Navigation.AccordionControlElement menSettingsImport;
        private DevExpress.XtraBars.Navigation.AccordionControlSeparator accordionControlSeparator1;
        private DevExpress.XtraBars.Navigation.AccordionControlElement menSettingsClose;
        private DevExpress.Utils.Layout.TablePanel tableContent;
        private DevExpress.XtraEditors.SimpleButton pshSaveSettings;
        private DevExpress.XtraBars.Navigation.AccordionControlSeparator accordionControlSeparator2;
        private DevExpress.XtraBars.Navigation.AccordionControlElement accordionControlElement1;
    }
}
