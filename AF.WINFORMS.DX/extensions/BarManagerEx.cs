using DevExpress.Utils;
using DevExpress.Utils.Svg;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Repository;

namespace AF.WINFORMS.DX;

/// <summary>
/// Erweiterungen für XtraBars.BarManager
/// </summary>
[SupportedOSPlatform("windows")]
public static class BarManagerEx
{
    /// <summary>
    /// Per BoundCommandsToMenu hinzugefügte EventHandler entfernen
    /// </summary>
    /// <param name="manager">BarManager</param>
    /// <param name="menu">Menu</param>
    public static void ClearCommandsInMenu(this BarManager manager, BarSubItem menu)
    {
        clear(manager, menu);
    }

    /// <summary>
    /// Einem Menu eines BarManagers Optionen zuordnen
    /// </summary>
    /// <param name="manager">BarManager</param>
    /// <param name="menu">Menu</param>
    /// <param name="entrys">Einträge</param>
    /// <param name="onclick">Ereignis-Handler für die Bearbeitung des OnClick-Ereignisses</param>
    /// <param name="showimages">Bereich für Bilder im Menü anzeigen (default ist false)</param>
    /// <param name="imgsize">Standardgröße Bilder (default = 24)</param>
    /// <param name="colormode">Modus für Colorization der Symbole (Default = Default)</param>
    public static void BoundCommandsToMenu(this BarManager manager, BarLinksHolder menu, IMenuEntry[] entrys,
        ItemClickEventHandler onclick, bool showimages = false, int imgsize = 24, SvgImageColorizationMode colormode = SvgImageColorizationMode.Default)
    {
        BoundCommandsToMenu(manager, menu, entrys, onclick, true, imgsize, showimages, colormode);
    }

    /// <summary>
    /// Einem Menu eines BarManagers Optionen zuordnen
    /// </summary>
    /// <param name="manager">BarManager</param>
    /// <param name="menu">Menu</param>
    /// <param name="entrys">Einträge</param>
    /// <param name="onclick">Ereignis-Handler für die Bearbeitung des OnClick-Ereignisses</param>
    /// <param name="imgsize">Größe der Bilder</param>
    /// <param name="toupper">Caption in Grossbuchstaben</param>
    /// <param name="showimages">Bereich für Bilder im Menü anzeigen (default ist false)</param>
    /// <param name="colormode">Modus für Colorization der Symbole (Default = Default)</param>
    public static void BoundCommandsToMenu(this BarManager manager, BarLinksHolder menu, IMenuEntry[] entrys,
        ItemClickEventHandler? onclick, bool toupper, int imgsize, bool showimages = false, SvgImageColorizationMode colormode = SvgImageColorizationMode.Default)
    {
        clear(manager, menu);

        List<BarSubItem> submenus = [];
        int itemcnt = 0;
        bool beginGroup = false;

        foreach (IMenuEntry entry in entrys)
        {
            ++itemcnt;

            if (entry.Caption == @"-")
            {
                beginGroup = true;
                continue;
            }

            if (entry.BeginGroup)
                beginGroup = true;

            string caption = (toupper ? entry.Caption.Replace(@"ß", @"SS").ToUpper() : entry.Caption);

            string[] cmdtree = caption.Split(['\\'], StringSplitOptions.RemoveEmptyEntries);

            BarButtonItem item = new(manager, caption);

            if (entry.Description.IsNotEmpty())
                item.SuperTip = UI.GetSuperTip(caption, entry.Description!, entry.Hint);

            item.Name = entry.Name;
            item.Tag = new Tuple<object?, ItemClickEventHandler?>(entry.Tag, onclick);
            item.ItemClick += clickInvoke;

            if (entry.HotKey != eKeys.None)
                item.ItemShortcut = new BarShortcut((Keys)(int)entry.HotKey);

            if ((manager.Images != null && entry.ImageIndex > -1) || entry.Image != null)
            {
                item.PaintStyle = BarItemPaintStyle.CaptionGlyph;
                item.ImageOptions.SvgImageColorizationMode = colormode;
                item.ImageOptions.SvgImageSize = new(imgsize, imgsize);
                
                if (entry.ImageIndex > -1)
                    item.ImageIndex = entry.ImageIndex;
                else
                {
                    if (entry.Image is Image image)
                        item.ImageOptions.Image = image;
                    else if (entry.Image is SvgImage svgimage)
                        item.ImageOptions.SvgImage = svgimage;
                }
            }
            else
            {
                item.ImageOptions.Image = null;
                item.PaintStyle = BarItemPaintStyle.Caption;
            }

            if (cmdtree.Length <= 1)
            {
                BarItemLink lnk = menu.AddItem(item, new() { BeginGroup = beginGroup, Item = item});
                lnk.BeginGroup = beginGroup;
            }
            else
            {
                BarLinksHolder parent = menu;

                // mit Submenu...
                for (int i = 0; i < (cmdtree.Length - 1); i++)
                {
                    string subname = @"msub" +
                                     cmdtree[i].ToUpper().RemoveAllExcept(@"ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_ÄÖÜ");
                    BarSubItem? submen = null;

                    foreach (BarSubItem sublnk in submenus)
                    {
                        if (sublnk.Name == subname)
                        {
                            parent = sublnk;
                            submen = sublnk;
                            break;
                        }
                    }

                    if (submen == null)
                    {
                        submen = new BarSubItem(manager, cmdtree[i])
                        {
                            Id = itemcnt,
                            Name = subname,
                            DrawMenuSideStrip = (showimages ? DefaultBoolean.True : DefaultBoolean.False),
                        };
                        parent.AddItem(submen);
                        submenus.Add(submen);
                        parent = submen;
                    }
                }

                item.Caption = cmdtree[^1];
                BarItemLink lnk = parent.AddItem(item, new() { BeginGroup = beginGroup, Item = item });
                lnk.BeginGroup = beginGroup;
            }

            beginGroup = false;
        }

        if (menu is BarSubItem itm)
        {
            itm.DrawMenuSideStrip = (showimages ? DefaultBoolean.True : DefaultBoolean.False);

            if (menu.ItemLinks.Count < 1)
                itm.AllowDrawArrow = DefaultBoolean.False;
            else
                itm.AllowDrawArrow = DefaultBoolean.True;
        }
    }

    /// <summary>
    /// Click an den Ersteller weiterleiten
    /// </summary>
    /// <param name="sender">TAG des Items</param>
    /// <param name="e">Parameter des Clicks</param>
    private static void clickInvoke(object sender, ItemClickEventArgs e)
    {
        if (e.Item.Tag is Tuple<object, ItemClickEventHandler> tuple)
            tuple.Item2.Invoke(tuple.Item1, e);
    }

    static void clear(BarManager manager, BarLinksHolder menu)
    {
        while (menu.ItemLinks.Count > 0)
        {
            BarItemLink lnk = menu.ItemLinks[0];

            if (lnk.Item != null)
            {
                lnk.Item.ItemClick -= clickInvoke;
                lnk.Item.Tag = null;
            }

            if (manager.Items.Contains(lnk.Item))
                manager.Items.Remove(lnk.Item);

            menu.ItemLinks.Remove(lnk);
            lnk.Dispose();
        }

        menu.ItemLinks.Clear();
    }

    /// <summary>
    /// Standardeinstellungen für einen Menu-Eintrag setzen.
    ///
    /// Das umfasst u.a. die ImageOptions für SVG-Symbole u.ä.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="style"></param>
    public static void PrepareMenuButton(this BarItem item, BarItemPaintStyle style)
    {
        item.PaintStyle = style;
        item.AllowHtmlText = DefaultBoolean.True;
        item.ImageOptions.AllowGlyphSkinning = DefaultBoolean.True;
        item.ImageOptions.SvgImageSize = new(16, 16);
    }

    /// <summary>
    /// Fügt einen Button zu einer Toolbar hinzu.
    /// </summary>
    /// <param name="bar">Toolbar, der der Button hinzugefügt werden soll</param>
    /// <param name="name">Name des Buttons (muss eindeutig sein für alle Bars, die vom BaraManager verwaltet werden)</param>
    /// <param name="img">Symbol für den Button</param>
    /// <param name="caption">Caption für den Button</param>
    /// <param name="begingroup">Gruppe beginnen (Trennstrich)</param>
    /// <param name="rightalign">rechtsbündig darstellen</param>
    /// <returns>der Button</returns>
    public static BarButtonItem AddButton(this Bar bar, string name, SvgImage? img = null, string? caption = null, bool? begingroup = null, bool? rightalign = null)
    {
        BarButtonItem btn = new() { Name = name };

        bar.LinksPersistInfo.Add(begingroup is true ? new LinkPersistInfo(btn, true) : new LinkPersistInfo(btn));
        
        if (img != null)
        {
            btn.PaintStyle = BarItemPaintStyle.Standard;
            btn.ImageOptions.SvgImage = img;
            btn.ImageOptions.SvgImageSize = new(16, 16);
            btn.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.Full;
        }

        if (caption != null)
        {
            if (img != null)
                btn.PaintStyle = BarItemPaintStyle.CaptionGlyph;
            btn.Caption = caption;
        }

        if (rightalign is true)
            btn.Alignment = BarItemLinkAlignment.Right;

        return btn;
    }

    /// <summary>
    /// Fügt einen Button zu einem Menü (BarSubItem) hinzu.
    /// </summary>
    /// <param name="bar">Toolbar, der der Button hinzugefügt werden soll</param>
    /// <param name="name">Name des Buttons (muss eindeutig sein für alle Bars, die vom BaraManager verwaltet werden)</param>
    /// <param name="img">Symbol für den Button</param>
    /// <param name="caption">Caption für den Button</param>
    /// <param name="begingroup">Gruppe beginnen (Trennstrich)</param>
    /// <param name="rightalign">rechtsbündig darstellen</param>
    /// <param name="toMenu">Menü, zu dem hinzugefügt werden soll</param>
    /// <returns>der Button</returns>
    public static BarButtonItem AddButton(this Bar bar, BarSubItem toMenu, string name, SvgImage? img = null, string? caption = null, bool? begingroup = null, bool? rightalign = null)
    {
        BarButtonItem btn = new() { Name = name };
        
        //bar.LinksPersistInfo.Add(begingroup is true ? new LinkPersistInfo(btn, true) : new LinkPersistInfo(btn));

        if (img != null)
        {
            btn.PaintStyle = BarItemPaintStyle.Standard;
            btn.ImageOptions.SvgImage = img;
            btn.ImageOptions.SvgImageSize = new(16, 16);
            btn.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.Full;
        }

        if (caption != null)
        {
            if (img != null)
                btn.PaintStyle = BarItemPaintStyle.CaptionGlyph;
            btn.Caption = caption;
        }

        if (rightalign is true)
            btn.Alignment = BarItemLinkAlignment.Right;


        var lnk = toMenu.AddItem(btn);
        
        if (begingroup is true)
            lnk.BeginGroup = true;

        return btn;
    }

    /// <summary>
    /// Fügt ein Menu-Button zu einer Toolbar hinzu.
    /// </summary>
    /// <param name="bar">Toolbar, der der Button hinzugefügt werden soll</param>
    /// <param name="name">Name des Menu-Buttons (muss eindeutig sein für alle Bars, die vom BarManager verwaltet werden)</param>
    /// <param name="img">Symbol für den Button</param>
    /// <param name="caption">Caption für den Button</param>
    /// <param name="begingroup">Gruppe beginnen (Trennstrich)</param>
    /// <param name="rightalign">rechtsbündig darstellen</param>
    /// <returns>der Menu-Button</returns>
    public static BarSubItem AddMenu(this Bar bar, string name, SvgImage? img = null, string? caption = null, bool? begingroup = null, bool? rightalign = null)
    {
        BarSubItem btn = new() { Name = name };

        bar.LinksPersistInfo.Add(begingroup is true ? new LinkPersistInfo(btn, true) : new LinkPersistInfo(btn));

        if (img != null)
        {
            btn.PaintStyle = BarItemPaintStyle.CaptionGlyph;
            btn.ImageOptions.SvgImage = img;
            btn.ImageOptions.SvgImageSize = new(16, 16);
            btn.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.Full;
        }

        if (caption != null)
            btn.Caption = caption;

        if (rightalign is true)
            btn.Alignment = BarItemLinkAlignment.Right;

        return btn;
    }

    /// <summary>
    /// Fügt einen Button zu einer Toolbar hinzu.
    /// </summary>
    /// <param name="bar">Toolbar, der der Button hinzugefügt werden soll</param>
    /// <param name="name">Name des Buttons (muss eindeutig sein für alle Bars, die vom BaraManager verwaltet werden)</param>
    /// <param name="img">Symbol für den Button</param>
    /// <param name="caption">Caption für den Button</param>
    /// <param name="rightalign">rechtsbündig darstellen</param>
    /// <returns>der Button</returns>
    public static BarStaticItem AddLabel(this Bar bar, string name, string caption, SvgImage? img = null, bool? rightalign = null)
    {
        BarStaticItem label = new() { Name = name, Caption = caption, Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder };

        bar.LinksPersistInfo.Add(new LinkPersistInfo(label));

        if (img != null)
        {
            label.PaintStyle = BarItemPaintStyle.CaptionGlyph;
            label.ImageOptions.SvgImage = img;
            label.ImageOptions.SvgImageSize = new(16, 16);
            label.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.Full;
        }

        if (rightalign is true)
            label.Alignment = BarItemLinkAlignment.Right;

        return label;
    }

    /// <summary>
    /// Fügt eine Toolbar hinzu, die in einem StandaloneBarDockControl angezeigt wird.
    /// </summary>
    /// <param name="manager">BarManager</param>
    /// <param name="bardock">StandaalonBarDockControl in dem die Toolbar dargestellt werden soll</param>
    /// <returns>hinzugefügte Toolbar</returns>
    public static Bar AddBar(this BarManager manager, StandaloneBarDockControl bardock)
    {
        manager.DockControls.Add(bardock);
        Bar bar = new();
        bar.CanDockStyle = BarCanDockStyle.Standalone;
        bar.StandaloneBarDockControl = bardock;
        bar.OptionsBar.UseWholeRow = true;
        bar.OptionsBar.DisableClose = true;
        bar.OptionsBar.AllowQuickCustomization = false;
        bar.OptionsBar.DrawDragBorder = false;
        bar.OptionsBar.DrawSizeGrip = false;
        bar.OptionsBar.DrawBorder = false;
        bar.OptionsBar.DisableCustomization = true;
        manager.Bars.Add(bar);

        return bar;
    }

    /// <summary>
    /// Fügt einer Toolbar ein RepositoryItemImageComboBox hinzu.
    /// </summary>
    /// <param name="bar">Toolbar</param>
    /// <param name="name">Name des Elemenst</param>
    /// <param name="combo">Rückbare des RepositoryItemImageComboBox Objekts</param>
    /// <returns>BarEditItem das das RepositoryItemImageComboBox Objekt enthält.</returns>
    public static BarEditItem AddImageCombobox(this Bar bar, string name, out RepositoryItemImageComboBox combo)
    {
        BarEditItem item = new(bar.Manager, new RepositoryItemImageComboBox());
        item.Name = name;
        item.Edit.Name = "repoitem" + name;

        bar.LinksPersistInfo.Add(new LinkPersistInfo(item));

        combo = (RepositoryItemImageComboBox)item.Edit;

        return item;
    }

    /// <summary>
    /// anzuzeigendes Symbol setzen
    /// </summary>
    /// <param name="item">ImageOptions</param>
    /// <param name="symbol">Symbol</param>
    public static void SetSymbol(this BarItem item, Symbol symbol)
    {
        item.PaintStyle = BarItemPaintStyle.CaptionGlyph;
        item.ImageOptions.SvgImage = UI.GetImage(symbol);
        item.ImageOptions.SvgImageColorizationMode = DevExpress.Utils.SvgImageColorizationMode.Default;
        item.ImageOptions.SvgImageSize = new(16, 16);
    }
}

