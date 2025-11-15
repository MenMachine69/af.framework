namespace AF.WINFORMS.DX
{
    partial class AFRichEditSimple
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
            this.richEditControl1 = new AFEditRichText();
            this.barManager = new DevExpress.XtraBars.BarManager(this.components);
            this.bar = new DevExpress.XtraRichEdit.UI.FontBar();
            this.pasteItem1 = new DevExpress.XtraRichEdit.UI.PasteItem();
            this.cutItem1 = new DevExpress.XtraRichEdit.UI.CutItem();
            this.copyItem1 = new DevExpress.XtraRichEdit.UI.CopyItem();
            this.undoItem1 = new DevExpress.XtraRichEdit.UI.UndoItem();
            this.redoItem1 = new DevExpress.XtraRichEdit.UI.RedoItem();
            this.changeStyleItem1 = new DevExpress.XtraRichEdit.UI.ChangeStyleItem();
            this.repositoryItemRichEditStyleEdit1 = new DevExpress.XtraRichEdit.Design.RepositoryItemRichEditStyleEdit();
            this.showEditStyleFormItem1 = new DevExpress.XtraRichEdit.UI.ShowEditStyleFormItem();
            this.changeFontNameItem1 = new DevExpress.XtraRichEdit.UI.ChangeFontNameItem();
            this.repositoryItemFontEditRichEdit1 = new DevExpress.XtraRichEdit.UI.RepositoryItemFontEditRichEdit();
            this.changeFontSizeItem1 = new DevExpress.XtraRichEdit.UI.ChangeFontSizeItem();
            this.repositoryItemRichEditFontSizeEdit1 = new DevExpress.XtraRichEdit.Design.RepositoryItemRichEditFontSizeEdit();
            this.toggleFontBoldItem1 = new DevExpress.XtraRichEdit.UI.ToggleFontBoldItem();
            this.toggleFontItalicItem1 = new DevExpress.XtraRichEdit.UI.ToggleFontItalicItem();
            this.toggleFontUnderlineItem1 = new DevExpress.XtraRichEdit.UI.ToggleFontUnderlineItem();
            this.toggleFontStrikeoutItem1 = new DevExpress.XtraRichEdit.UI.ToggleFontStrikeoutItem();
            this.toggleFontSuperscriptItem1 = new DevExpress.XtraRichEdit.UI.ToggleFontSuperscriptItem();
            this.toggleFontSubscriptItem1 = new DevExpress.XtraRichEdit.UI.ToggleFontSubscriptItem();
            this.changeFontColorItem1 = new DevExpress.XtraRichEdit.UI.ChangeFontColorItem();
            this.changeFontHighlightColorItem1 = new DevExpress.XtraRichEdit.UI.ChangeFontHighlightColorItem();
            this.changeParagraphBackColorItem1 = new DevExpress.XtraRichEdit.UI.ChangeParagraphBackColorItem();
            this.clearFormattingItem1 = new DevExpress.XtraRichEdit.UI.ClearFormattingItem();
            this.toggleBulletedListItem1 = new DevExpress.XtraRichEdit.UI.ToggleBulletedListItem();
            this.toggleNumberingListItem1 = new DevExpress.XtraRichEdit.UI.ToggleNumberingListItem();
            this.decreaseIndentItem1 = new DevExpress.XtraRichEdit.UI.DecreaseIndentItem();
            this.increaseIndentItem1 = new DevExpress.XtraRichEdit.UI.IncreaseIndentItem();
            this.toggleParagraphAlignmentLeftItem1 = new DevExpress.XtraRichEdit.UI.ToggleParagraphAlignmentLeftItem();
            this.toggleParagraphAlignmentCenterItem1 = new DevExpress.XtraRichEdit.UI.ToggleParagraphAlignmentCenterItem();
            this.toggleParagraphAlignmentRightItem1 = new DevExpress.XtraRichEdit.UI.ToggleParagraphAlignmentRightItem();
            this.insertPageBreakItem21 = new DevExpress.XtraRichEdit.UI.InsertPageBreakItem2();
            this.insertFloatingPictureItem1 = new DevExpress.XtraRichEdit.UI.InsertFloatingPictureItem();
            this.insertTableItem1 = new DevExpress.XtraRichEdit.UI.InsertTableItem();
            this.insertHyperlinkItem1 = new DevExpress.XtraRichEdit.UI.InsertHyperlinkItem();
            this.insertTextBoxItem1 = new DevExpress.XtraRichEdit.UI.InsertTextBoxItem();
            this.findItem1 = new DevExpress.XtraRichEdit.UI.FindItem();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.standaloneBarDockControl1 = new DevExpress.XtraBars.StandaloneBarDockControl();
            this.barController = new DevExpress.XtraBars.BarAndDockingController(this.components);
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.richEditBarController = new DevExpress.XtraRichEdit.UI.RichEditBarController(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditStyleEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEditRichEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditFontSizeEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.richEditBarController)).BeginInit();
            this.SuspendLayout();
            // 
            // richEditControl1
            // 
            this.richEditControl1.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Simple;
            this.richEditControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.richEditControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richEditControl1.LayoutUnit = DevExpress.XtraRichEdit.DocumentLayoutUnit.Pixel;
            this.richEditControl1.Location = new System.Drawing.Point(0, 24);
            this.richEditControl1.MenuManager = this.barManager;
            this.richEditControl1.Name = "richEditControl1";
            this.richEditControl1.Options.DocumentSaveOptions.CurrentFormat = DevExpress.XtraRichEdit.DocumentFormat.PlainText;
            this.richEditControl1.Options.Export.Html.UseHtml5 = true;
            this.richEditControl1.Options.Printing.PrintPreviewFormKind = DevExpress.XtraRichEdit.PrintPreviewFormKind.Bars;
            this.richEditControl1.Size = new System.Drawing.Size(1540, 679);
            this.richEditControl1.TabIndex = 0;
            this.richEditControl1.Unit = DevExpress.Office.DocumentUnit.Centimeter;
            this.richEditControl1.DocumentLoaded += new System.EventHandler(this.richEditControl1_DocumentLoaded);
            this.richEditControl1.EmptyDocumentCreated += new System.EventHandler(this.richEditControl1_EmptyDocumentCreated);
            // 
            // barManager
            // 
            this.barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar});
            this.barManager.Controller = this.barController;
            this.barManager.DockControls.Add(this.barDockControlTop);
            this.barManager.DockControls.Add(this.barDockControlBottom);
            this.barManager.DockControls.Add(this.barDockControlLeft);
            this.barManager.DockControls.Add(this.barDockControlRight);
            this.barManager.DockControls.Add(this.standaloneBarDockControl1);
            this.barManager.Form = this;
            this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.pasteItem1,
            this.cutItem1,
            this.copyItem1,
            this.changeFontNameItem1,
            this.changeFontSizeItem1,
            this.toggleFontBoldItem1,
            this.toggleFontItalicItem1,
            this.toggleFontUnderlineItem1,
            this.toggleFontStrikeoutItem1,
            this.toggleFontSuperscriptItem1,
            this.toggleFontSubscriptItem1,
            this.changeFontColorItem1,
            this.changeFontHighlightColorItem1,
            this.clearFormattingItem1,
            this.toggleBulletedListItem1,
            this.toggleNumberingListItem1,
            this.decreaseIndentItem1,
            this.increaseIndentItem1,
            this.toggleParagraphAlignmentLeftItem1,
            this.toggleParagraphAlignmentCenterItem1,
            this.toggleParagraphAlignmentRightItem1,
            this.findItem1,
            this.insertPageBreakItem21,
            this.insertTableItem1,
            this.insertFloatingPictureItem1,
            this.insertHyperlinkItem1,
            this.insertTextBoxItem1,
            this.undoItem1,
            this.redoItem1,
            this.changeParagraphBackColorItem1,
            this.changeStyleItem1,
            this.showEditStyleFormItem1,
            this.barButtonItem1});
            this.barManager.MaxItemId = 130;
            this.barManager.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemFontEditRichEdit1,
            this.repositoryItemRichEditFontSizeEdit1,
            this.repositoryItemRichEditStyleEdit1});
            // 
            // bar
            // 
            this.bar.Control = this.richEditControl1;
            this.bar.DockCol = 0;
            this.bar.DockRow = 0;
            this.bar.DockStyle = DevExpress.XtraBars.BarDockStyle.Standalone;
            this.bar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.KeyTip, this.pasteItem1, "", false, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.Standard, "V", ""),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.KeyTip, this.cutItem1, "", false, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.Standard, "X", ""),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.KeyTip, this.copyItem1, "", false, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.Standard, "C", ""),
            new DevExpress.XtraBars.LinkPersistInfo(this.undoItem1, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.redoItem1),
            new DevExpress.XtraBars.LinkPersistInfo(this.changeStyleItem1, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.showEditStyleFormItem1),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.KeyTip, this.changeFontNameItem1, "", false, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.Standard, "FF", ""),
            new DevExpress.XtraBars.LinkPersistInfo(this.changeFontSizeItem1),
            new DevExpress.XtraBars.LinkPersistInfo(this.toggleFontBoldItem1),
            new DevExpress.XtraBars.LinkPersistInfo(this.toggleFontItalicItem1),
            new DevExpress.XtraBars.LinkPersistInfo(this.toggleFontUnderlineItem1),
            new DevExpress.XtraBars.LinkPersistInfo(this.toggleFontStrikeoutItem1),
            new DevExpress.XtraBars.LinkPersistInfo(this.toggleFontSuperscriptItem1),
            new DevExpress.XtraBars.LinkPersistInfo(this.toggleFontSubscriptItem1),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.KeyTip, this.changeFontColorItem1, "", false, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.Standard, "FC", ""),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.KeyTip, this.changeFontHighlightColorItem1, "", false, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.Standard, "I", ""),
            new DevExpress.XtraBars.LinkPersistInfo(this.changeParagraphBackColorItem1),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.KeyTip, this.clearFormattingItem1, "", false, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.Standard, "E", ""),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.KeyTip, this.toggleBulletedListItem1, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.Standard, "U", ""),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.KeyTip, this.toggleNumberingListItem1, "", false, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.Standard, "N", ""),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.KeyTip, this.decreaseIndentItem1, "", false, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.Standard, "AO", ""),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.KeyTip, this.increaseIndentItem1, "", false, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.Standard, "AI", ""),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.KeyTip, this.toggleParagraphAlignmentLeftItem1, "", false, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.Standard, "AL", ""),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.KeyTip, this.toggleParagraphAlignmentCenterItem1, "", false, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.Standard, "AC", ""),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.KeyTip, this.toggleParagraphAlignmentRightItem1, "", false, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.Standard, "AR", ""),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.KeyTip, this.insertPageBreakItem21, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.Standard, "B", ""),
            new DevExpress.XtraBars.LinkPersistInfo(this.insertFloatingPictureItem1),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.KeyTip, this.insertTableItem1, "", false, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.Standard, "T", ""),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.KeyTip, this.insertHyperlinkItem1, "", false, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.Standard, "I", ""),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.KeyTip, this.insertTextBoxItem1, "", false, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.Standard, "X", ""),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.KeyTip, this.findItem1, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.Standard, "FD", "")});
            this.bar.OptionsBar.AllowQuickCustomization = false;
            this.bar.OptionsBar.DisableClose = true;
            this.bar.OptionsBar.DisableCustomization = true;
            this.bar.OptionsBar.DrawDragBorder = false;
            this.bar.OptionsBar.MultiLine = true;
            this.bar.OptionsBar.UseWholeRow = true;
            this.bar.StandaloneBarDockControl = this.standaloneBarDockControl1;
            this.bar.Text = "start";
            // 
            // pasteItem1
            // 
            this.pasteItem1.Id = 19;
            this.pasteItem1.Name = "pasteItem1";
            // 
            // cutItem1
            // 
            this.cutItem1.Id = 18;
            this.cutItem1.Name = "cutItem1";
            // 
            // copyItem1
            // 
            this.copyItem1.Id = 17;
            this.copyItem1.Name = "copyItem1";
            // 
            // undoItem1
            // 
            this.undoItem1.Id = 76;
            this.undoItem1.Name = "undoItem1";
            // 
            // redoItem1
            // 
            this.redoItem1.Id = 77;
            this.redoItem1.Name = "redoItem1";
            // 
            // changeStyleItem1
            // 
            this.changeStyleItem1.Edit = this.repositoryItemRichEditStyleEdit1;
            this.changeStyleItem1.Id = 126;
            this.changeStyleItem1.Name = "changeStyleItem1";
            // 
            // repositoryItemRichEditStyleEdit1
            // 
            this.repositoryItemRichEditStyleEdit1.AutoHeight = false;
            this.repositoryItemRichEditStyleEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemRichEditStyleEdit1.Control = this.richEditControl1;
            this.repositoryItemRichEditStyleEdit1.Name = "repositoryItemRichEditStyleEdit1";
            // 
            // showEditStyleFormItem1
            // 
            this.showEditStyleFormItem1.Id = 127;
            this.showEditStyleFormItem1.Name = "showEditStyleFormItem1";
            // 
            // changeFontNameItem1
            // 
            this.changeFontNameItem1.Edit = this.repositoryItemFontEditRichEdit1;
            this.changeFontNameItem1.Id = 26;
            this.changeFontNameItem1.Name = "changeFontNameItem1";
            // 
            // repositoryItemFontEditRichEdit1
            // 
            this.repositoryItemFontEditRichEdit1.AutoHeight = false;
            this.repositoryItemFontEditRichEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemFontEditRichEdit1.Name = "repositoryItemFontEditRichEdit1";
            // 
            // changeFontSizeItem1
            // 
            this.changeFontSizeItem1.Edit = this.repositoryItemRichEditFontSizeEdit1;
            this.changeFontSizeItem1.Id = 35;
            this.changeFontSizeItem1.Name = "changeFontSizeItem1";
            // 
            // repositoryItemRichEditFontSizeEdit1
            // 
            this.repositoryItemRichEditFontSizeEdit1.AutoHeight = false;
            this.repositoryItemRichEditFontSizeEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemRichEditFontSizeEdit1.Control = this.richEditControl1;
            this.repositoryItemRichEditFontSizeEdit1.Name = "repositoryItemRichEditFontSizeEdit1";
            // 
            // toggleFontBoldItem1
            // 
            this.toggleFontBoldItem1.Id = 38;
            this.toggleFontBoldItem1.Name = "toggleFontBoldItem1";
            // 
            // toggleFontItalicItem1
            // 
            this.toggleFontItalicItem1.Id = 39;
            this.toggleFontItalicItem1.Name = "toggleFontItalicItem1";
            // 
            // toggleFontUnderlineItem1
            // 
            this.toggleFontUnderlineItem1.Id = 40;
            this.toggleFontUnderlineItem1.Name = "toggleFontUnderlineItem1";
            // 
            // toggleFontStrikeoutItem1
            // 
            this.toggleFontStrikeoutItem1.Id = 41;
            this.toggleFontStrikeoutItem1.Name = "toggleFontStrikeoutItem1";
            // 
            // toggleFontSuperscriptItem1
            // 
            this.toggleFontSuperscriptItem1.Id = 42;
            this.toggleFontSuperscriptItem1.Name = "toggleFontSuperscriptItem1";
            // 
            // toggleFontSubscriptItem1
            // 
            this.toggleFontSubscriptItem1.Id = 43;
            this.toggleFontSubscriptItem1.Name = "toggleFontSubscriptItem1";
            // 
            // changeFontColorItem1
            // 
            this.changeFontColorItem1.Id = 44;
            this.changeFontColorItem1.Name = "changeFontColorItem1";
            // 
            // changeFontHighlightColorItem1
            // 
            this.changeFontHighlightColorItem1.Id = 45;
            this.changeFontHighlightColorItem1.Name = "changeFontHighlightColorItem1";
            // 
            // changeParagraphBackColorItem1
            // 
            this.changeParagraphBackColorItem1.Id = 124;
            this.changeParagraphBackColorItem1.Name = "changeParagraphBackColorItem1";
            // 
            // clearFormattingItem1
            // 
            this.clearFormattingItem1.Id = 46;
            this.clearFormattingItem1.Name = "clearFormattingItem1";
            // 
            // toggleBulletedListItem1
            // 
            this.toggleBulletedListItem1.Id = 28;
            this.toggleBulletedListItem1.Name = "toggleBulletedListItem1";
            // 
            // toggleNumberingListItem1
            // 
            this.toggleNumberingListItem1.Id = 29;
            this.toggleNumberingListItem1.Name = "toggleNumberingListItem1";
            // 
            // decreaseIndentItem1
            // 
            this.decreaseIndentItem1.Id = 33;
            this.decreaseIndentItem1.Name = "decreaseIndentItem1";
            // 
            // increaseIndentItem1
            // 
            this.increaseIndentItem1.Id = 34;
            this.increaseIndentItem1.Name = "increaseIndentItem1";
            // 
            // toggleParagraphAlignmentLeftItem1
            // 
            this.toggleParagraphAlignmentLeftItem1.Id = 30;
            this.toggleParagraphAlignmentLeftItem1.Name = "toggleParagraphAlignmentLeftItem1";
            // 
            // toggleParagraphAlignmentCenterItem1
            // 
            this.toggleParagraphAlignmentCenterItem1.Id = 31;
            this.toggleParagraphAlignmentCenterItem1.Name = "toggleParagraphAlignmentCenterItem1";
            // 
            // toggleParagraphAlignmentRightItem1
            // 
            this.toggleParagraphAlignmentRightItem1.Id = 32;
            this.toggleParagraphAlignmentRightItem1.Name = "toggleParagraphAlignmentRightItem1";
            // 
            // insertPageBreakItem21
            // 
            this.insertPageBreakItem21.Id = 65;
            this.insertPageBreakItem21.Name = "insertPageBreakItem21";
            // 
            // insertFloatingPictureItem1
            // 
            this.insertFloatingPictureItem1.Id = 64;
            this.insertFloatingPictureItem1.Name = "insertFloatingPictureItem1";
            // 
            // insertTableItem1
            // 
            this.insertTableItem1.Id = 66;
            this.insertTableItem1.Name = "insertTableItem1";
            // 
            // insertHyperlinkItem1
            // 
            this.insertHyperlinkItem1.Id = 67;
            this.insertHyperlinkItem1.Name = "insertHyperlinkItem1";
            // 
            // insertTextBoxItem1
            // 
            this.insertTextBoxItem1.Id = 68;
            this.insertTextBoxItem1.Name = "insertTextBoxItem1";
            // 
            // findItem1
            // 
            this.findItem1.Id = 24;
            this.findItem1.Name = "findItem1";
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "SAVE STYLES";
            this.barButtonItem1.Id = 129;
            this.barButtonItem1.Name = "barButtonItem1";
            // 
            // standaloneBarDockControl1
            // 
            this.standaloneBarDockControl1.AutoSize = true;
            this.standaloneBarDockControl1.CausesValidation = false;
            this.standaloneBarDockControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.standaloneBarDockControl1.Location = new System.Drawing.Point(0, 0);
            this.standaloneBarDockControl1.Manager = this.barManager;
            this.standaloneBarDockControl1.Name = "standaloneBarDockControl1";
            this.standaloneBarDockControl1.Size = new System.Drawing.Size(1540, 24);
            this.standaloneBarDockControl1.Text = "standaloneBarDockControl1";
            // 
            // barController
            // 
            this.barController.PropertiesBar.DistanceBetweenItems = 1;
            this.barController.PropertiesBar.DrawMenuSideStrip = false;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager;
            this.barDockControlTop.Size = new System.Drawing.Size(1540, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 703);
            this.barDockControlBottom.Manager = this.barManager;
            this.barDockControlBottom.Size = new System.Drawing.Size(1540, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
            this.barDockControlLeft.Manager = this.barManager;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 703);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1540, 0);
            this.barDockControlRight.Manager = this.barManager;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 703);
            // 
            // richEditBarController
            // 
            this.richEditBarController.BarItems.Add(this.pasteItem1);
            this.richEditBarController.BarItems.Add(this.cutItem1);
            this.richEditBarController.BarItems.Add(this.copyItem1);
            this.richEditBarController.BarItems.Add(this.changeFontNameItem1);
            this.richEditBarController.BarItems.Add(this.changeFontSizeItem1);
            this.richEditBarController.BarItems.Add(this.toggleFontBoldItem1);
            this.richEditBarController.BarItems.Add(this.toggleFontItalicItem1);
            this.richEditBarController.BarItems.Add(this.toggleFontUnderlineItem1);
            this.richEditBarController.BarItems.Add(this.toggleFontStrikeoutItem1);
            this.richEditBarController.BarItems.Add(this.toggleFontSuperscriptItem1);
            this.richEditBarController.BarItems.Add(this.toggleFontSubscriptItem1);
            this.richEditBarController.BarItems.Add(this.changeFontColorItem1);
            this.richEditBarController.BarItems.Add(this.changeFontHighlightColorItem1);
            this.richEditBarController.BarItems.Add(this.clearFormattingItem1);
            this.richEditBarController.BarItems.Add(this.toggleBulletedListItem1);
            this.richEditBarController.BarItems.Add(this.toggleNumberingListItem1);
            this.richEditBarController.BarItems.Add(this.decreaseIndentItem1);
            this.richEditBarController.BarItems.Add(this.increaseIndentItem1);
            this.richEditBarController.BarItems.Add(this.toggleParagraphAlignmentLeftItem1);
            this.richEditBarController.BarItems.Add(this.toggleParagraphAlignmentCenterItem1);
            this.richEditBarController.BarItems.Add(this.toggleParagraphAlignmentRightItem1);
            this.richEditBarController.BarItems.Add(this.findItem1);
            this.richEditBarController.BarItems.Add(this.insertPageBreakItem21);
            this.richEditBarController.BarItems.Add(this.insertTableItem1);
            this.richEditBarController.BarItems.Add(this.insertFloatingPictureItem1);
            this.richEditBarController.BarItems.Add(this.insertHyperlinkItem1);
            this.richEditBarController.BarItems.Add(this.insertTextBoxItem1);
            this.richEditBarController.BarItems.Add(this.undoItem1);
            this.richEditBarController.BarItems.Add(this.redoItem1);
            this.richEditBarController.BarItems.Add(this.changeParagraphBackColorItem1);
            this.richEditBarController.BarItems.Add(this.changeStyleItem1);
            this.richEditBarController.BarItems.Add(this.showEditStyleFormItem1);
            this.richEditBarController.Control = this.richEditControl1;
            // 
            // AFRichEditSimple
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.richEditControl1);
            this.Controls.Add(this.standaloneBarDockControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.DoubleBuffered = true;
            this.Name = "AFRichEditSimple";
            this.Size = new System.Drawing.Size(1540, 703);
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditStyleEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEditRichEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichEditFontSizeEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.richEditBarController)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AFEditRichText richEditControl1;
        private DevExpress.XtraBars.BarManager barManager;
        private DevExpress.XtraRichEdit.UI.FontBar bar;
        private DevExpress.XtraRichEdit.UI.PasteItem pasteItem1;
        private DevExpress.XtraRichEdit.UI.CutItem cutItem1;
        private DevExpress.XtraRichEdit.UI.CopyItem copyItem1;
        private DevExpress.XtraRichEdit.UI.ChangeFontNameItem changeFontNameItem1;
        private DevExpress.XtraRichEdit.UI.RepositoryItemFontEditRichEdit repositoryItemFontEditRichEdit1;
        private DevExpress.XtraRichEdit.UI.ChangeFontSizeItem changeFontSizeItem1;
        private DevExpress.XtraRichEdit.Design.RepositoryItemRichEditFontSizeEdit repositoryItemRichEditFontSizeEdit1;
        private DevExpress.XtraRichEdit.UI.ToggleFontBoldItem toggleFontBoldItem1;
        private DevExpress.XtraRichEdit.UI.ToggleFontItalicItem toggleFontItalicItem1;
        private DevExpress.XtraRichEdit.UI.ToggleFontUnderlineItem toggleFontUnderlineItem1;
        private DevExpress.XtraRichEdit.UI.ToggleFontStrikeoutItem toggleFontStrikeoutItem1;
        private DevExpress.XtraRichEdit.UI.ToggleFontSuperscriptItem toggleFontSuperscriptItem1;
        private DevExpress.XtraRichEdit.UI.ToggleFontSubscriptItem toggleFontSubscriptItem1;
        private DevExpress.XtraRichEdit.UI.ChangeFontColorItem changeFontColorItem1;
        private DevExpress.XtraRichEdit.UI.ChangeFontHighlightColorItem changeFontHighlightColorItem1;
        private DevExpress.XtraRichEdit.UI.ClearFormattingItem clearFormattingItem1;
        private DevExpress.XtraRichEdit.UI.ToggleBulletedListItem toggleBulletedListItem1;
        private DevExpress.XtraRichEdit.UI.ToggleNumberingListItem toggleNumberingListItem1;
        private DevExpress.XtraRichEdit.UI.DecreaseIndentItem decreaseIndentItem1;
        private DevExpress.XtraRichEdit.UI.IncreaseIndentItem increaseIndentItem1;
        private DevExpress.XtraRichEdit.UI.ToggleParagraphAlignmentLeftItem toggleParagraphAlignmentLeftItem1;
        private DevExpress.XtraRichEdit.UI.ToggleParagraphAlignmentCenterItem toggleParagraphAlignmentCenterItem1;
        private DevExpress.XtraRichEdit.UI.ToggleParagraphAlignmentRightItem toggleParagraphAlignmentRightItem1;
        private DevExpress.XtraRichEdit.UI.FindItem findItem1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraRichEdit.UI.RichEditBarController richEditBarController;
        private DevExpress.XtraRichEdit.UI.InsertPageBreakItem2 insertPageBreakItem21;
        private DevExpress.XtraRichEdit.UI.InsertFloatingPictureItem insertFloatingPictureItem1;
        private DevExpress.XtraRichEdit.UI.InsertTableItem insertTableItem1;
        private DevExpress.XtraRichEdit.UI.InsertHyperlinkItem insertHyperlinkItem1;
        private DevExpress.XtraRichEdit.UI.InsertTextBoxItem insertTextBoxItem1;
        private DevExpress.XtraBars.StandaloneBarDockControl standaloneBarDockControl1;
        private DevExpress.XtraBars.BarAndDockingController barController;
        private DevExpress.XtraRichEdit.UI.UndoItem undoItem1;
        private DevExpress.XtraRichEdit.UI.RedoItem redoItem1;
        private DevExpress.XtraRichEdit.UI.ChangeParagraphBackColorItem changeParagraphBackColorItem1;
        private DevExpress.XtraRichEdit.UI.ChangeStyleItem changeStyleItem1;
        private DevExpress.XtraRichEdit.Design.RepositoryItemRichEditStyleEdit repositoryItemRichEditStyleEdit1;
        private DevExpress.XtraRichEdit.UI.ShowEditStyleFormItem showEditStyleFormItem1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
    }
}
