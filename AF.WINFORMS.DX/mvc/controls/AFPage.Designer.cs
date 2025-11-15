namespace AF.MVC
{
    partial class AFPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AFPage));
            DevExpress.Utils.SuperToolTip superToolTip1 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem1 = new DevExpress.Utils.ToolTipTitleItem();
            DevExpress.Utils.ToolTipItem toolTipItem1 = new DevExpress.Utils.ToolTipItem();
            DevExpress.Utils.SuperToolTip superToolTip2 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem2 = new DevExpress.Utils.ToolTipTitleItem();
            DevExpress.Utils.ToolTipItem toolTipItem2 = new DevExpress.Utils.ToolTipItem();
            this.split = new DevExpress.XtraEditors.SplitContainerControl();
            this.panelMaster = new AF.WINFORMS.DX.AFScrollablePanel();
            this.barDockMaster = new DevExpress.XtraBars.StandaloneBarDockControl();
            this.barManager = new DevExpress.XtraBars.BarManager(this.components);
            this.barMainView = new DevExpress.XtraBars.Bar();
            this.menMasterPopup = new DevExpress.XtraBars.BarSubItem();
            this.spacerLeft = new DevExpress.XtraBars.BarStaticItem();
            this.lblSelectView = new DevExpress.XtraBars.BarStaticItem();
            this.cmbPageSelect = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemPageSelect = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.spacerRight = new DevExpress.XtraBars.BarStaticItem();
            this.menAdd = new DevExpress.XtraBars.BarSubItem();
            this.menAddDefault = new DevExpress.XtraBars.BarButtonItem();
            this.menAddCopy = new DevExpress.XtraBars.BarButtonItem();
            this.menSave = new DevExpress.XtraBars.BarButtonItem();
            this.menDelete = new DevExpress.XtraBars.BarButtonItem();
            this.menMasterContext = new DevExpress.XtraBars.BarSubItem();
            this.barDetailView = new DevExpress.XtraBars.Bar();
            this.menDetailViews = new DevExpress.XtraBars.BarSubItem();
            this.menAddDetail = new DevExpress.XtraBars.BarSubItem();
            this.menAddDetailDefault = new DevExpress.XtraBars.BarButtonItem();
            this.menAddDetailCopy = new DevExpress.XtraBars.BarButtonItem();
            this.menDetailMaxView = new DevExpress.XtraBars.BarButtonItem();
            this.menDetailContext = new DevExpress.XtraBars.BarSubItem();
            this.shortcut1 = new DevExpress.XtraBars.BarButtonItem();
            this.shortcut2 = new DevExpress.XtraBars.BarButtonItem();
            this.shortcut3 = new DevExpress.XtraBars.BarButtonItem();
            this.shortcut4 = new DevExpress.XtraBars.BarButtonItem();
            this.shortcut5 = new DevExpress.XtraBars.BarButtonItem();
            this.setupDetailShortcuts = new DevExpress.XtraBars.BarButtonItem();
            this.barDockDetail = new DevExpress.XtraBars.StandaloneBarDockControl();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.panelDetail = new AF.WINFORMS.DX.AFScrollablePanel();
            this.detailViewPluginContainer = new DevExpress.XtraEditors.SidePanel();
            this.barController = new DevExpress.XtraBars.BarAndDockingController(this.components);
            this.sideDialog = new DevExpress.XtraEditors.SidePanel();
            this.sideSettings = new DevExpress.XtraEditors.SidePanel();
            ((System.ComponentModel.ISupportInitialize)(this.split)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.split.Panel1)).BeginInit();
            this.split.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.split.Panel2)).BeginInit();
            this.split.Panel2.SuspendLayout();
            this.split.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPageSelect)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barController)).BeginInit();
            this.SuspendLayout();
            // 
            // split
            // 
            this.split.Dock = System.Windows.Forms.DockStyle.Fill;
            this.split.Horizontal = false;
            this.split.Location = new System.Drawing.Point(0, 0);
            this.split.Name = "split";
            // 
            // split.Panel1
            // 
            this.split.Panel1.Controls.Add(this.panelMaster);
            this.split.Panel1.Controls.Add(this.barDockMaster);
            this.split.Panel1.Padding = new System.Windows.Forms.Padding(8, 8, 12, 12);
            this.split.Panel1.Text = "Panel1";
            // 
            // split.Panel2
            // 
            this.split.Panel2.Controls.Add(this.panelDetail);
            this.split.Panel2.Controls.Add(this.detailViewPluginContainer);
            this.split.Panel2.Controls.Add(this.barDockDetail);
            this.split.Panel2.Padding = new System.Windows.Forms.Padding(8, 8, 8, 5);
            this.split.Panel2.Text = "Panel2";
            this.split.Size = new System.Drawing.Size(730, 865);
            this.split.SplitterPosition = 370;
            this.split.TabIndex = 5;
            // 
            // panelMaster
            // 
            this.panelMaster.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMaster.Location = new System.Drawing.Point(8, 40);
            this.panelMaster.Name = "panelMaster";
            this.panelMaster.Size = new System.Drawing.Size(710, 318);
            this.panelMaster.TabIndex = 1;
            // 
            // barDockMaster
            // 
            this.barDockMaster.AutoSize = true;
            this.barDockMaster.CausesValidation = false;
            this.barDockMaster.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockMaster.Location = new System.Drawing.Point(8, 8);
            this.barDockMaster.Manager = this.barManager;
            this.barDockMaster.Name = "barDockMaster";
            this.barDockMaster.Size = new System.Drawing.Size(710, 32);
            this.barDockMaster.Text = "standaloneBarDockControl1";
            // 
            // barManager
            // 
            this.barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.barMainView,
            this.barDetailView});
            this.barManager.DockControls.Add(this.barDockControlTop);
            this.barManager.DockControls.Add(this.barDockControlBottom);
            this.barManager.DockControls.Add(this.barDockControlLeft);
            this.barManager.DockControls.Add(this.barDockControlRight);
            this.barManager.DockControls.Add(this.barDockDetail);
            this.barManager.DockControls.Add(this.barDockMaster);
            this.barManager.Form = this;
            this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barButtonItem1,
            this.menDetailViews,
            this.menMasterPopup,
            this.menMasterContext,
            this.menDetailContext,
            this.cmbPageSelect,
            this.spacerLeft,
            this.spacerRight,
            this.menAdd,
            this.menSave,
            this.menDelete,
            this.lblSelectView,
            this.menAddDefault,
            this.menAddCopy,
            this.menDetailMaxView,
            this.menAddDetail,
            this.menAddDetailDefault,
            this.menAddDetailCopy,
            this.shortcut1,
            this.shortcut2,
            this.shortcut3,
            this.shortcut4,
            this.shortcut5,
            this.setupDetailShortcuts});
            this.barManager.MaxItemId = 24;
            this.barManager.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemPageSelect});
            // 
            // barMainView
            // 
            this.barMainView.BarName = "Benutzerdefiniert 2";
            this.barMainView.DockCol = 0;
            this.barMainView.DockRow = 0;
            this.barMainView.DockStyle = DevExpress.XtraBars.BarDockStyle.Standalone;
            this.barMainView.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.menMasterPopup),
            new DevExpress.XtraBars.LinkPersistInfo(this.spacerLeft),
            new DevExpress.XtraBars.LinkPersistInfo(this.lblSelectView),
            new DevExpress.XtraBars.LinkPersistInfo(this.cmbPageSelect),
            new DevExpress.XtraBars.LinkPersistInfo(this.spacerRight),
            new DevExpress.XtraBars.LinkPersistInfo(this.menAdd),
            new DevExpress.XtraBars.LinkPersistInfo(this.menSave),
            new DevExpress.XtraBars.LinkPersistInfo(this.menDelete),
            new DevExpress.XtraBars.LinkPersistInfo(this.menMasterContext)});
            this.barMainView.OptionsBar.AllowQuickCustomization = false;
            this.barMainView.OptionsBar.DisableClose = true;
            this.barMainView.OptionsBar.DisableCustomization = true;
            this.barMainView.OptionsBar.DrawBorder = false;
            this.barMainView.OptionsBar.DrawDragBorder = false;
            this.barMainView.OptionsBar.UseWholeRow = true;
            this.barMainView.StandaloneBarDockControl = this.barDockMaster;
            this.barMainView.Text = "Benutzerdefiniert 2";
            // 
            // menMasterPopup
            // 
            this.menMasterPopup.Caption = "<master popup>";
            this.menMasterPopup.Id = 2;
            this.menMasterPopup.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("menMasterPopup.ImageOptions.SvgImage")));
            this.menMasterPopup.ImageOptions.SvgImageColorizationMode = DevExpress.Utils.SvgImageColorizationMode.None;
            this.menMasterPopup.ImageOptions.SvgImageSize = new System.Drawing.Size(24, 24);
            this.menMasterPopup.ItemAppearance.Disabled.FontSizeDelta = 2;
            this.menMasterPopup.ItemAppearance.Disabled.Options.UseFont = true;
            this.menMasterPopup.ItemAppearance.Hovered.FontSizeDelta = 2;
            this.menMasterPopup.ItemAppearance.Hovered.Options.UseFont = true;
            this.menMasterPopup.ItemAppearance.Normal.FontSizeDelta = 2;
            this.menMasterPopup.ItemAppearance.Normal.Options.UseFont = true;
            this.menMasterPopup.ItemAppearance.Pressed.FontSizeDelta = 2;
            this.menMasterPopup.ItemAppearance.Pressed.Options.UseFont = true;
            this.menMasterPopup.MenuDrawMode = DevExpress.XtraBars.MenuDrawMode.SmallImagesText;
            this.menMasterPopup.Name = "menMasterPopup";
            this.menMasterPopup.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // spacerLeft
            // 
            this.spacerLeft.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.spacerLeft.Caption = "                     ";
            this.spacerLeft.Enabled = false;
            this.spacerLeft.Id = 6;
            this.spacerLeft.Name = "spacerLeft";
            // 
            // lblSelectView
            // 
            this.lblSelectView.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.lblSelectView.Caption = "Ansicht";
            this.lblSelectView.Enabled = false;
            this.lblSelectView.Id = 11;
            this.lblSelectView.Name = "lblSelectView";
            // 
            // cmbPageSelect
            // 
            this.cmbPageSelect.AutoFillWidth = true;
            this.cmbPageSelect.Edit = this.repositoryItemPageSelect;
            this.cmbPageSelect.Id = 5;
            this.cmbPageSelect.ImageOptions.SvgImageSize = new System.Drawing.Size(24, 24);
            this.cmbPageSelect.Name = "cmbPageSelect";
            // 
            // repositoryItemPageSelect
            // 
            this.repositoryItemPageSelect.Appearance.FontSizeDelta = 2;
            this.repositoryItemPageSelect.Appearance.Options.UseFont = true;
            this.repositoryItemPageSelect.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemPageSelect.Name = "repositoryItemPageSelect";
            // 
            // spacerRight
            // 
            this.spacerRight.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.spacerRight.Caption = "                    ";
            this.spacerRight.Enabled = false;
            this.spacerRight.Id = 7;
            this.spacerRight.Name = "spacerRight";
            // 
            // menAdd
            // 
            this.menAdd.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.menAdd.Caption = "Neu";
            this.menAdd.Id = 8;
            this.menAdd.ImageOptions.SvgImageSize = new System.Drawing.Size(16, 16);
            this.menAdd.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.menAddDefault),
            new DevExpress.XtraBars.LinkPersistInfo(this.menAddCopy)});
            this.menAdd.Name = "menAdd";
            this.menAdd.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // menAddDefault
            // 
            this.menAddDefault.Caption = "NEU MIT STANDARDWERTEN";
            this.menAddDefault.Id = 12;
            this.menAddDefault.Name = "menAddDefault";
            // 
            // menAddCopy
            // 
            this.menAddCopy.Caption = "NEU AUS KOPIE";
            this.menAddCopy.Id = 13;
            this.menAddCopy.Name = "menAddCopy";
            // 
            // menSave
            // 
            this.menSave.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.menSave.Caption = "Speichern";
            this.menSave.Id = 9;
            this.menSave.ImageOptions.SvgImageSize = new System.Drawing.Size(16, 16);
            this.menSave.Name = "menSave";
            this.menSave.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // menDelete
            // 
            this.menDelete.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.menDelete.Caption = "Löschen";
            this.menDelete.Id = 10;
            this.menDelete.ImageOptions.SvgImageSize = new System.Drawing.Size(16, 16);
            this.menDelete.Name = "menDelete";
            this.menDelete.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // menMasterContext
            // 
            this.menMasterContext.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.menMasterContext.Id = 3;
            this.menMasterContext.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.menMasterContext.ImageOptions.SvgImageSize = new System.Drawing.Size(16, 16);
            this.menMasterContext.Name = "menMasterContext";
            this.menMasterContext.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // barDetailView
            // 
            this.barDetailView.BarName = "Benutzerdefiniert 3";
            this.barDetailView.DockCol = 0;
            this.barDetailView.DockRow = 0;
            this.barDetailView.DockStyle = DevExpress.XtraBars.BarDockStyle.Standalone;
            this.barDetailView.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.menDetailViews),
            new DevExpress.XtraBars.LinkPersistInfo(this.menAddDetail),
            new DevExpress.XtraBars.LinkPersistInfo(this.menDetailMaxView),
            new DevExpress.XtraBars.LinkPersistInfo(this.menDetailContext),
            new DevExpress.XtraBars.LinkPersistInfo(this.shortcut1),
            new DevExpress.XtraBars.LinkPersistInfo(this.shortcut2),
            new DevExpress.XtraBars.LinkPersistInfo(this.shortcut3),
            new DevExpress.XtraBars.LinkPersistInfo(this.shortcut4),
            new DevExpress.XtraBars.LinkPersistInfo(this.shortcut5),
            new DevExpress.XtraBars.LinkPersistInfo(this.setupDetailShortcuts)});
            this.barDetailView.OptionsBar.AllowQuickCustomization = false;
            this.barDetailView.OptionsBar.DisableClose = true;
            this.barDetailView.OptionsBar.DisableCustomization = true;
            this.barDetailView.OptionsBar.DrawBorder = false;
            this.barDetailView.OptionsBar.DrawDragBorder = false;
            this.barDetailView.OptionsBar.UseWholeRow = true;
            this.barDetailView.StandaloneBarDockControl = this.barDockDetail;
            this.barDetailView.Text = "Benutzerdefiniert 3";
            // 
            // menDetailViews
            // 
            this.menDetailViews.Caption = "<Ansicht auswählen>";
            this.menDetailViews.Id = 1;
            this.menDetailViews.ImageOptions.SvgImageSize = new System.Drawing.Size(24, 24);
            this.menDetailViews.ItemAppearance.Disabled.FontSizeDelta = 2;
            this.menDetailViews.ItemAppearance.Disabled.Options.UseFont = true;
            this.menDetailViews.ItemAppearance.Hovered.FontSizeDelta = 2;
            this.menDetailViews.ItemAppearance.Hovered.Options.UseFont = true;
            this.menDetailViews.ItemAppearance.Normal.FontSizeDelta = 2;
            this.menDetailViews.ItemAppearance.Normal.Options.UseFont = true;
            this.menDetailViews.ItemAppearance.Pressed.FontSizeDelta = 2;
            this.menDetailViews.ItemAppearance.Pressed.Options.UseFont = true;
            this.menDetailViews.MenuDrawMode = DevExpress.XtraBars.MenuDrawMode.SmallImagesText;
            this.menDetailViews.Name = "menDetailViews";
            this.menDetailViews.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // menAddDetail
            // 
            this.menAddDetail.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.menAddDetail.Caption = "Neu";
            this.menAddDetail.Id = 15;
            this.menAddDetail.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.menAddDetailDefault),
            new DevExpress.XtraBars.LinkPersistInfo(this.menAddDetailCopy)});
            this.menAddDetail.Name = "menAddDetail";
            this.menAddDetail.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.menAddDetail.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            // 
            // menAddDetailDefault
            // 
            this.menAddDetailDefault.Caption = "NEU MIT STANDARDWERTEN";
            this.menAddDetailDefault.Id = 16;
            this.menAddDetailDefault.Name = "menAddDetailDefault";
            // 
            // menAddDetailCopy
            // 
            this.menAddDetailCopy.Caption = "NEU AUS KOPIE";
            this.menAddDetailCopy.Id = 17;
            this.menAddDetailCopy.Name = "menAddDetailCopy";
            // 
            // menDetailMaxView
            // 
            this.menDetailMaxView.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.menDetailMaxView.Id = 14;
            this.menDetailMaxView.Name = "menDetailMaxView";
            toolTipTitleItem1.Text = "MAXIMIEREN";
            toolTipItem1.Text = "Die Ansicht des Bereiches maximieren.";
            superToolTip1.Items.Add(toolTipTitleItem1);
            superToolTip1.Items.Add(toolTipItem1);
            this.menDetailMaxView.SuperTip = superToolTip1;
            this.menDetailMaxView.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.maxminDetail);
            // 
            // menDetailContext
            // 
            this.menDetailContext.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.menDetailContext.Id = 4;
            this.menDetailContext.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.menDetailContext.ImageOptions.SvgImageSize = new System.Drawing.Size(16, 16);
            this.menDetailContext.Name = "menDetailContext";
            this.menDetailContext.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // shortcut1
            // 
            this.shortcut1.Caption = "sc1";
            this.shortcut1.Id = 18;
            this.shortcut1.Name = "shortcut1";
            this.shortcut1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            // 
            // shortcut2
            // 
            this.shortcut2.Caption = "sc2";
            this.shortcut2.Id = 19;
            this.shortcut2.Name = "shortcut2";
            this.shortcut2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            // 
            // shortcut3
            // 
            this.shortcut3.Caption = "sc3";
            this.shortcut3.Id = 20;
            this.shortcut3.Name = "shortcut3";
            this.shortcut3.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            // 
            // shortcut4
            // 
            this.shortcut4.Caption = "sc4";
            this.shortcut4.Id = 21;
            this.shortcut4.Name = "shortcut4";
            this.shortcut4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            // 
            // shortcut5
            // 
            this.shortcut5.Caption = "sc5";
            this.shortcut5.Id = 22;
            this.shortcut5.Name = "shortcut5";
            this.shortcut5.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            // 
            // setupDetailShortcuts
            // 
            this.setupDetailShortcuts.Caption = "setupSC";
            this.setupDetailShortcuts.Id = 23;
            this.setupDetailShortcuts.Name = "setupDetailShortcuts";
            toolTipTitleItem2.Text = "Schnellzugriff konfigurieren";
            toolTipItem2.Text = "Legen Sie bis zu 5 Detailansichten fest, auf die Sie direkt zugreifen möchten/kön" +
    "nen.";
            superToolTip2.Items.Add(toolTipTitleItem2);
            superToolTip2.Items.Add(toolTipItem2);
            this.setupDetailShortcuts.SuperTip = superToolTip2;
            // 
            // barDockDetail
            // 
            this.barDockDetail.AutoSize = true;
            this.barDockDetail.CausesValidation = false;
            this.barDockDetail.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockDetail.Location = new System.Drawing.Point(8, 8);
            this.barDockDetail.Manager = this.barManager;
            this.barDockDetail.Name = "barDockDetail";
            this.barDockDetail.Size = new System.Drawing.Size(714, 24);
            this.barDockDetail.Text = "standaloneBarDockControl2";
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager;
            this.barDockControlTop.Size = new System.Drawing.Size(1312, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 865);
            this.barDockControlBottom.Manager = this.barManager;
            this.barDockControlBottom.Size = new System.Drawing.Size(1312, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
            this.barDockControlLeft.Manager = this.barManager;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 865);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1312, 0);
            this.barDockControlRight.Manager = this.barManager;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 865);
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "barButtonItem1";
            this.barButtonItem1.Id = 0;
            this.barButtonItem1.Name = "barButtonItem1";
            // 
            // panelDetail
            // 
            this.panelDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDetail.Location = new System.Drawing.Point(8, 32);
            this.panelDetail.Name = "panelDetail";
            this.panelDetail.Size = new System.Drawing.Size(423, 448);
            this.panelDetail.TabIndex = 10;
            // 
            // detailViewPluginContainer
            // 
            this.detailViewPluginContainer.Dock = System.Windows.Forms.DockStyle.Right;
            this.detailViewPluginContainer.Location = new System.Drawing.Point(431, 32);
            this.detailViewPluginContainer.Name = "detailViewPluginContainer";
            this.detailViewPluginContainer.Size = new System.Drawing.Size(291, 448);
            this.detailViewPluginContainer.TabIndex = 2;
            this.detailViewPluginContainer.Text = "sidePanel1";
            // 
            // barController
            // 
            this.barController.PropertiesBar.AllowLinkLighting = false;
            // 
            // sideDialog
            // 
            this.sideDialog.Dock = System.Windows.Forms.DockStyle.Right;
            this.sideDialog.Location = new System.Drawing.Point(1021, 0);
            this.sideDialog.Name = "sideDialog";
            this.sideDialog.Size = new System.Drawing.Size(291, 865);
            this.sideDialog.TabIndex = 10;
            this.sideDialog.Visible = false;
            // 
            // sideSettings
            // 
            this.sideSettings.Dock = System.Windows.Forms.DockStyle.Right;
            this.sideSettings.Location = new System.Drawing.Point(730, 0);
            this.sideSettings.Name = "sideSettings";
            this.sideSettings.Size = new System.Drawing.Size(291, 865);
            this.sideSettings.TabIndex = 15;
            this.sideSettings.Visible = false;
            // 
            // AFPage
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.split);
            this.Controls.Add(this.sideSettings);
            this.Controls.Add(this.sideDialog);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.DoubleBuffered = true;
            this.Name = "AFPage";
            this.Size = new System.Drawing.Size(1312, 865);
            ((System.ComponentModel.ISupportInitialize)(this.split.Panel1)).EndInit();
            this.split.Panel1.ResumeLayout(false);
            this.split.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.split.Panel2)).EndInit();
            this.split.Panel2.ResumeLayout(false);
            this.split.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.split)).EndInit();
            this.split.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPageSelect)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barController)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraBars.BarManager barManager;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.Bar barMainView;
        private DevExpress.XtraBars.Bar barDetailView;
        private DevExpress.XtraEditors.SplitContainerControl split;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraEditors.SidePanel detailViewPluginContainer;
        private DevExpress.XtraBars.BarAndDockingController barController;
        private DevExpress.XtraBars.BarSubItem menMasterPopup;
        private DevExpress.XtraBars.BarStaticItem spacerLeft;
        private DevExpress.XtraBars.BarEditItem cmbPageSelect;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox repositoryItemPageSelect;
        private DevExpress.XtraBars.BarStaticItem spacerRight;
        private DevExpress.XtraBars.BarSubItem menAdd;
        private DevExpress.XtraBars.BarButtonItem menSave;
        private DevExpress.XtraBars.BarButtonItem menDelete;
        private DevExpress.XtraBars.BarSubItem menMasterContext;
        private DevExpress.XtraBars.BarSubItem menDetailViews;
        private DevExpress.XtraBars.BarSubItem menDetailContext;
        private DevExpress.XtraBars.BarStaticItem lblSelectView;
        private DevExpress.XtraBars.BarButtonItem menAddDefault;
        private DevExpress.XtraBars.BarButtonItem menAddCopy;
        private DevExpress.XtraBars.StandaloneBarDockControl barDockDetail;
        private DevExpress.XtraBars.StandaloneBarDockControl barDockMaster;
        private AFScrollablePanel panelMaster;
        private AFScrollablePanel panelDetail;
        private DevExpress.XtraEditors.SidePanel sideDialog;
        private DevExpress.XtraEditors.SidePanel sideSettings;
        private DevExpress.XtraBars.BarButtonItem menDetailMaxView;
        private DevExpress.XtraBars.BarSubItem menAddDetail;
        private DevExpress.XtraBars.BarButtonItem menAddDetailDefault;
        private DevExpress.XtraBars.BarButtonItem menAddDetailCopy;
        private DevExpress.XtraBars.BarButtonItem shortcut1;
        private DevExpress.XtraBars.BarButtonItem shortcut2;
        private DevExpress.XtraBars.BarButtonItem shortcut3;
        private DevExpress.XtraBars.BarButtonItem shortcut4;
        private DevExpress.XtraBars.BarButtonItem shortcut5;
        private DevExpress.XtraBars.BarButtonItem setupDetailShortcuts;
    }
}
