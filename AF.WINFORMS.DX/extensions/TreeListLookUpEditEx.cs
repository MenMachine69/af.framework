using AF.MVC;
using DevExpress.Utils;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors.Controls;

namespace AF.WINFORMS.DX;

/// <summary>
/// Extension methods for TreeListLookUpEdit
/// </summary>
[SupportedOSPlatform("windows")]
public static class TreeListLookUpEditEx
{
    /// <summary>
    /// Setup grid an control
    /// </summary>
    /// <param name="searchLookUpEdit">control to which the setup sould be supplied</param>
    /// <param name="setup">setup/settings</param>
    public static void Setup(this DevExpress.XtraEditors.TreeListLookUpEdit searchLookUpEdit, AFGridSetup setup)
    {
        bool clickset = false;

        if (setup.Symbol != null)
        {
            if (setup.Symbol is SvgImage svg)
                searchLookUpEdit.AddButton(svg, name: @"pshSymbol", showleft: true, enabled: false);
            else if (setup.Symbol is Image img)
                searchLookUpEdit.AddButton(img, name: @"pshSymbol", showleft: true, enabled: false);
        }

        if (setup.CmdEdit != null)
        {
            var button = searchLookUpEdit.AddButton(Symbol.Edit, name: @"pshEdit", caption: WinFormsStrings.LBL_EDIT);
            button.Tag = new Tuple<AFCommand?, DevExpress.XtraEditors.TreeListLookUpEdit>(setup.CmdEdit, searchLookUpEdit);
            button.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.Full;
            button.ImageOptions.SvgImageSize = new(12, 12);
            button.ImageOptions.AllowGlyphSkinning = DefaultBoolean.True;
            button.ToolTip = "BEARBEITEN";


            searchLookUpEdit.ButtonClick += invokeCommand;
            clickset = true;
        }

        if (setup.CmdAdd != null)
        {
            var button = searchLookUpEdit.AddButton(Symbol.Add, name: @"pshAdd", caption: WinFormsStrings.LBL_NEW);
            button.Tag = new Tuple<AFCommand?, DevExpress.XtraEditors.TreeListLookUpEdit>(setup.CmdAdd, searchLookUpEdit);
            button.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.Full;
            button.ImageOptions.SvgImageSize = new(12, 12);
            button.ImageOptions.AllowGlyphSkinning = DefaultBoolean.True;
            button.ToolTip = "HINZUFÜGEN/NEU";

            if (!clickset)
            {
                searchLookUpEdit.ButtonClick += invokeCommand;
                clickset = true;
            }
        }

        if (setup.CmdShowDetail != null)
        {
            var button = searchLookUpEdit.AddButton(Symbol.Search, name: @"pshDetail", caption: WinFormsStrings.LBL_SHOWDETAILS);
            button.Tag = new Tuple<AFCommand?, DevExpress.XtraEditors.TreeListLookUpEdit>(setup.CmdShowDetail, searchLookUpEdit);
            button.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.Full;
            button.ImageOptions.SvgImageSize = new(12, 12);
            button.ImageOptions.AllowGlyphSkinning = DefaultBoolean.True;
            button.ToolTip = "DETAILS ANZEIGEN";

            if (!clickset)
            {
                searchLookUpEdit.ButtonClick += invokeCommand;
                clickset = true;
            }
        }

        if (setup.CmdDelete != null)
        {
            var button = searchLookUpEdit.AddButton(Symbol.Delete, name: @"pshDelete", caption: WinFormsStrings.LBL_DELETE);
            button.Tag = new Tuple<AFCommand?, DevExpress.XtraEditors.TreeListLookUpEdit>(setup.CmdDelete, searchLookUpEdit);
            button.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.Full;
            button.ImageOptions.SvgImageSize = new(12, 12);
            button.ImageOptions.AllowGlyphSkinning = DefaultBoolean.True;
            button.ToolTip = "LÖSCHEN";

            if (!clickset)
            {
                searchLookUpEdit.ButtonClick += invokeCommand;
                clickset = true;
            }
        }

        if (setup.CmdGoto != null)
        {
            var button = searchLookUpEdit.AddButton(Symbol.GoTo, name: @"pshGoto", caption: WinFormsStrings.LBL_GOTO);
            button.Tag = new Tuple<AFCommand?, DevExpress.XtraEditors.TreeListLookUpEdit>(setup.CmdGoto, searchLookUpEdit);
            button.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.Full;
            button.ImageOptions.AllowGlyphSkinning = DefaultBoolean.True;
            button.ToolTip = "GEHE ZU";

            if (!clickset)
                searchLookUpEdit.ButtonClick += invokeCommand;
        }

        setup.CmdShowDetail = null;
        setup.CmdDelete = null;
        setup.CmdEdit = null;
        setup.CmdGoto = null;
        setup.CmdAdd = null;

        searchLookUpEdit.Properties.TreeList.Setup(setup);
    }

    /// <summary>
    /// Returns the selected object 
    /// </summary>
    /// <param name="searchLookUpEdit">get object from this lookup edit</param>.
    /// <returns>selected object or null</returns>
    public static object? GetSelectedObject(this DevExpress.XtraEditors.TreeListLookUpEdit searchLookUpEdit)
    {
        if (searchLookUpEdit.EditValue != null)
            return searchLookUpEdit.Properties.GetRowByKeyValue(searchLookUpEdit.EditValue);

        return null;
    }

    private static void invokeCommand(object sender, ButtonPressedEventArgs e)
    {
        if (e.Button.Tag is not Tuple<AFCommand?, DevExpress.XtraEditors.TreeListLookUpEdit> cmd) return;

        var result = cmd.Item1?.Execute(new CommandArgs
        {
            Page = cmd.Item2.GetParentControl<IViewPage>(),
            CommandSource = cmd.Item2,
            CommandContext = eCommandContext.GridContext,
            Model = cmd.Item2.GetSelectedObject() as IModel
        });

        if (result != null)
            cmd.Item2.FindForm()?.HandleResult(result);
    }
}