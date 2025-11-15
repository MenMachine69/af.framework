using AF.MVC;
using DevExpress.Utils;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors.Controls;

namespace AF.WINFORMS.DX;

/// <summary>
/// Extension methods for PopupContainerEdit
/// </summary>
[SupportedOSPlatform("windows")]
public static class PopupContainerEditEx
{
    /// <summary>
    /// Setup control
    /// </summary>
    /// <param name="popupContainerEdit">Control, dass eingerichtet werden soll</param>
    /// <param name="setup">Einstellungen</param>
    public static void Setup(this DevExpress.XtraEditors.PopupContainerEdit popupContainerEdit, AFGridSetup setup)
    {
        bool clickset = false;

        if (setup.Symbol != null)
        {
            if (setup.Symbol is SvgImage svg)
                popupContainerEdit.AddButton(svg, name: @"pshSymbol", showleft: true, enabled: false);
            else if (setup.Symbol is Image img)
                popupContainerEdit.AddButton(img, name: @"pshSymbol", showleft: true, enabled: false);
        }

        if (setup.CmdEdit != null && setup.AllowEdit)
        {
            var button = popupContainerEdit.AddButton(Symbol.Edit, name: @"pshEdit", caption: WinFormsStrings.LBL_EDIT);
            button.Tag = new Tuple<AFCommand?, DevExpress.XtraEditors.PopupContainerEdit>(setup.CmdEdit, popupContainerEdit);
            button.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.Full;
            button.ImageOptions.SvgImageSize = new(12, 12);
            button.ImageOptions.AllowGlyphSkinning = DefaultBoolean.True;
            button.ToolTip = "BEARBEITEN";


            popupContainerEdit.ButtonClick += invokeCommand;
            clickset = true;
        }

        if (setup.CmdAdd != null && setup.AllowAddNew)
        {
            var button = popupContainerEdit.AddButton(Symbol.Add, name: @"pshAdd", caption: WinFormsStrings.LBL_NEW);
            button.Tag = new Tuple<AFCommand?, DevExpress.XtraEditors.PopupContainerEdit>(setup.CmdAdd, popupContainerEdit);
            button.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.Full;
            button.ImageOptions.SvgImageSize = new(12,12);
            button.ImageOptions.AllowGlyphSkinning = DefaultBoolean.True;
            button.ToolTip = "HINZUFÜGEN/NEU";

            if (!clickset)
            {
                popupContainerEdit.ButtonClick += invokeCommand;
                clickset = true;
            }
        }

        if (setup.CmdShowDetail != null)
        {
            var button = popupContainerEdit.AddButton(Symbol.Search, name: @"pshDetail", caption: WinFormsStrings.LBL_SHOWDETAILS);
            button.Tag = new Tuple<AFCommand?, DevExpress.XtraEditors.PopupContainerEdit>(setup.CmdShowDetail, popupContainerEdit);
            button.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.Full;
            button.ImageOptions.SvgImageSize = new(12, 12);
            button.ImageOptions.AllowGlyphSkinning = DefaultBoolean.True;
            button.ToolTip = "DETAILS ANZEIGEN";

            if (!clickset)
            {
                popupContainerEdit.ButtonClick += invokeCommand;
                clickset = true;
            }
        }

        if (setup.CmdDelete != null)
        {
            var button = popupContainerEdit.AddButton(Symbol.Delete, name: @"pshDelete", caption: WinFormsStrings.LBL_DELETE);
            button.Tag = new Tuple<AFCommand?, DevExpress.XtraEditors.PopupContainerEdit>(setup.CmdDelete, popupContainerEdit);
            button.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.Full;
            button.ImageOptions.SvgImageSize = new(12, 12);
            button.ImageOptions.AllowGlyphSkinning = DefaultBoolean.True;
            button.ToolTip = "LÖSCHEN";

            if (!clickset)
            {
                popupContainerEdit.ButtonClick += invokeCommand;
                clickset = true;
            }
        }

        if (setup.CmdGoto != null)
        {
            var button = popupContainerEdit.AddButton(Symbol.GoTo, name: @"pshGoto", caption: WinFormsStrings.LBL_GOTO);
            button.Tag = new Tuple<AFCommand?, DevExpress.XtraEditors.PopupContainerEdit>(setup.CmdGoto, popupContainerEdit);
            button.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.Full;
            button.ImageOptions.AllowGlyphSkinning = DefaultBoolean.True;
            button.ToolTip = "GEHE ZU";

            if (!clickset)
                popupContainerEdit.ButtonClick += invokeCommand;
        }

        setup.CmdShowDetail = null;
        setup.CmdDelete = null;
        setup.CmdEdit = null;
        setup.CmdGoto = null;
        setup.CmdAdd = null;
    }
   

    private static void invokeCommand(object sender, ButtonPressedEventArgs e)
    {
        if (e.Button.Tag is not Tuple<AFCommand?, DevExpress.XtraEditors.PopupContainerEdit> cmd) return;

        var result = cmd.Item1?.Execute(new CommandArgs
        {
            Page = cmd.Item2.GetParentControl<IViewPage>(),
            CommandSource = cmd.Item2,
            CommandContext = eCommandContext.GridContext
        });

        if (result != null)
            cmd.Item2.FindForm()?.HandleResult(result);
    }
}