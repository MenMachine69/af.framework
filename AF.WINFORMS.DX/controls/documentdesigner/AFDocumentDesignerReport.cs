using DevExpress.Utils;
using DevExpress.XtraBars.Docking2010.Views.Tabbed;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;

namespace AF.WINFORMS.DX;

/// <summary>
/// Designer für Dokumentvorlagen vom Typ 'Report'
/// </summary>
public partial class AFDocumentDesignerReport : AFUserControl
{

    private XtraReport emptyReport = new();

    /// <summary>
    /// Constructor
    /// </summary>
    public AFDocumentDesignerReport()
    {
        InitializeComponent();

        if (UI.DesignMode) return;

        reportDesigner1.SetCommandVisibility(ReportCommand.ShowScriptsTab, CommandVisibility.All);
        reportDesigner1.SetCommandVisibility(ReportCommand.AddNewDataSource, CommandVisibility.None);
        reportDesigner1.SetCommandVisibility(ReportCommand.Language, CommandVisibility.None);


        //reportDesigner1.DefaultReportSettings = new DevExpress.XtraReports.UserDesigner.ReportSettings(() => true);

        commandBarItem14.ImageOptions.SvgImage = UI.GetImage(Symbol.ClipboardPaste);
        commandBarItem16.ImageOptions.SvgImage = UI.GetImage(Symbol.Copy);
        commandBarItem15.ImageOptions.SvgImage = UI.GetImage(Symbol.Cut);
        commandBarItem4.ImageOptions.SvgImage = UI.GetImage(Symbol.ArrowUndo);
        commandBarItem5.ImageOptions.SvgImage = UI.GetImage(Symbol.ArrowRedo);
        //increaseIndentItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.FontIncrease);
        //decreaseIndentItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.FontDecrease);
        commandBarItem17.ImageOptions.SvgImage = UI.GetImage(Symbol.TextBold);
        commandBarItem18.ImageOptions.SvgImage = UI.GetImage(Symbol.TextItalic);
        commandBarItem19.ImageOptions.SvgImage = UI.GetImage(Symbol.TextUnderline);
        commandBarItem24.ImageOptions.SvgImage = UI.GetImage(Symbol.TextStrikethroughS);
        //toggleFontSuperscriptItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextSuperscript);
        //toggleFontSubscriptItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextSubscript);

        //clearFormattingItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.ClearFormatting);
        //toggleBulletedListItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextBulletListLtr);
        //toggleNumberingListItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextNumberListLtr);
        //decreaseIndentItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextIndentDecreaseLtr);
        //increaseIndentItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextIndentIncreaseLtr);
        //toggleParagraphAlignmentLeftItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextAlignLeft);
        //toggleParagraphAlignmentCenterItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextAlignCenter);
        //toggleParagraphAlignmentRightItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextAlignRight);

        //insertPageBreakItem21.ImageOptions.SvgImage = UI.GetImage(Symbol.DocumentPageBreak);
        //insertTableItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.Table);
        //insertTextBoxItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.Textbox);
        //insertFloatingPictureItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.Image);
        //insertHyperlinkItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.Link);
        //insertSymbolItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.Pi);

        //setLandscapePageOrientationItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.DocumentLandscape);
        //setPortraitPageOrientationItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.Document);
        //changeSectionPageMarginsItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.DocumentMargins);

        //editPageHeaderItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.DocumentHeader);
        //editPageFooterItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.DocumentFooter);
        //editPageFooterItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.DocumentFooter);

        //setSectionOneColumnItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextColumnOne);
        //setSectionTwoColumnsItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextColumnTwo);
        //setSectionThreeColumnsItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextColumnThree);

        //showEditStyleFormItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextEditStyle);
        //showFontFormItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextFont);
        //toggleMultiLevelListItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextBulletListTree);
        //replaceItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.ArrowSwap);
        //printPreviewItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.DocumentSearch);

        ////changeTableCellsHorizontalTextDirectionItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.TextDire);

        //findItem1.ImageOptions.SvgImage = UI.GetImage(Symbol.Search);

        reportDesigner1.DesignPanelLoaded += (_, _) =>
        {
            (reportDesigner1.XtraTabbedMdiManager.View as TabbedView)!.DocumentGroups[0].Properties.ShowTabHeader = DefaultBoolean.False;
        };

        

        XtraReport report = new();
        report.ScriptLanguage = DevExpress.XtraReports.ScriptLanguage.CSharp;
        report.ReportUnit = ReportUnit.TenthsOfAMillimeter;
        reportDesigner1.OpenReport(emptyReport);

        //reportDesigner1.CreateNewReport();
    }

    /// <summary>
    /// Serialisierte Report-Definition
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public byte[] Report
    {
        get
        {
            using MemoryStream stream = new();
            reportDesigner1.ActiveDesignPanel.Report.SaveLayoutToXml(stream);
            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            return stream.GetBuffer();
        }
        set
        {
            XtraReport report = new();

            try
            {
                using MemoryStream stream = new();
                stream.Write(value, 0, value.Length);
                stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                report.LoadLayoutFromXml(stream);
            }
            catch (Exception ex)
            {
                MsgBox.ShowErrorOk("REPORT LADEN\r\nBeim Laden des Reports trat ein Fehler auf.\r\n" + ex.Message);
            }

            if (DataSource != null)
            {
                List<object> ds =
                [
                    DataSource
                ];
                report.DataSource = ds;
            }


            reportDesigner1.ActiveDesignPanel.OpenReport(report);


            reportDesigner1.ActiveDesignPanel.Report.ScriptLanguage = DevExpress.XtraReports.ScriptLanguage.CSharp;
            reportDesigner1.ActiveDesignPanel.Report.ReportUnit = ReportUnit.TenthsOfAMillimeter;
        }
    }

    /// <summary>
    /// Datenquelle setzen...
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public object? DataSource
    {
        get => reportDesigner1.ActiveDesignPanel.Report.DataSource;
        set => reportDesigner1.ActiveDesignPanel.Report.DataSource = value;
    }
}

