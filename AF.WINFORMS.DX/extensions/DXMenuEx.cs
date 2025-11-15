using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;

namespace AF.WINFORMS.DX;

/// <summary>
/// Erweiterungen für DevExpress.Utils.Menu.DXMenu
/// </summary>
public static class DXMenuEx
{
    /// <summary>
    /// Ein SubMenu anhand der Caption finden
    /// </summary>
    /// <param name="menu">Menü in dem gesucht wird</param>
    /// <param name="caption">zu suchende Caption</param>
    /// <returns>der gefundenen Menüpunkt/das SubMenü oder NULL</returns>
    public static DXSubMenuItem? FoundSubMenu(this DXMenuItemCollection menu, string caption)
    {
        foreach (DXMenuItem item in menu)
        {
            if (item is not DXSubMenuItem menuItem) continue;

            if (menuItem.Caption == caption)
                return menuItem;
            
            DXSubMenuItem? found = menuItem.Items.FoundSubMenu(caption);
            
            if (found != null)
                return found;
        }

        return null;
    }

    

    /// <summary>
    /// Fügt einen Menüeintrag zu einem DXPopupMenu hinzu
    /// </summary>
    /// <param name="menu">Menü, zu dem der Eintrag hinzugefügt werden soll. Der Eintrag kann in der Caption das Zeichen \ enthalten um Untermenü-Strukturen anzulegen.</param>
    /// <param name="entry">hinzuzufügender Eintrag</param>
    /// <param name="tag">Objekt, dass im Tag des Eintrags abgelegt werden soll</param>
    /// <returns>der hinzugefügte Eintrag</returns>
    public static DXMenuItem? AddMenuEntry(this DXPopupMenu menu, IMenuEntry entry, object? tag = null)
    {
        if (entry.Caption.Contains('\\'))
        {
            string[] submenus = entry.Caption.Split('\\', StringSplitOptions.RemoveEmptyEntries);

            DXSubMenuItem? parent = null;

            int currPos = 0;

            foreach (string submenu in submenus)
            {
                ++currPos;

                DXSubMenuItem? newparent = (parent == null ? menu.Items.FoundSubMenu(submenu) : parent.Items.FoundSubMenu(submenu));

                if (newparent != null)
                {
                    parent = newparent;
                    continue;
                }

                if (currPos == submenus.Length) // last item
                {
                    DXMenuItem item = new(submenu);
                    item.Tag = tag;

                    if (entry.Description != null)
                        item.SuperTip = UI.GetSuperTip(submenu.ToUpper(), entry.Description, footer: entry.Hint);

                    item.BeginGroup = entry.BeginGroup;

                    if (entry.Image != null)
                    {
                        if (entry.Image is SvgImage svg)
                        {
                            item.ImageOptions.SvgImage = svg;
                            item.ImageOptions.SvgImageSize = new Size(16, 16);
                            item.ImageOptions.SvgImageColorizationMode = (entry is AFPopupMenuEntry popupentry ? popupentry.SymbolColorizationMode : SvgImageColorizationMode.Default);
                        }
                        else if (entry.Image is Image img)
                            item.ImageOptions.Image = img;
                    }
                    else if (entry.ImageIndex >= 0)
                    {
                        item.ImageOptions.SvgImage = UI.GetImage((Symbol)entry.ImageIndex);
                        item.ImageOptions.SvgImageSize = new Size(16, 16);
                        item.ImageOptions.SvgImageColorizationMode = (entry is AFPopupMenuEntry popupentry ? popupentry.SymbolColorizationMode : SvgImageColorizationMode.Default);
                    }

                    if (parent != null)
                        parent.Items.Add(item);
                    else
                        menu.Items.Add(item);

                    return item;
                }

                var sub = new DXSubMenuItem(submenu);

                sub.BeginGroup = entry.BeginGroup;

                if (entry.GroupImage != null)
                {
                    if (entry.GroupImage is SvgImage svg)
                    {
                        sub.ImageOptions.SvgImage = svg;
                        sub.ImageOptions.SvgImageSize = new Size(16, 16);
                        sub.ImageOptions.SvgImageColorizationMode = (entry is AFPopupMenuEntry popupentry ? popupentry.SymbolColorizationMode : SvgImageColorizationMode.Default);
                    }
                    else if (entry.GroupImage is Image img)
                        sub.ImageOptions.Image = img;
                }


                if (parent != null)
                    parent.Items.Add(sub);
                else
                    menu.Items.Add(sub);

                parent = sub;
            }
        }
        else
        {
            DXMenuItem item = new(entry.Caption);
            item.Tag = tag;

            if (entry.Description != null)
                item.SuperTip = UI.GetSuperTip(entry.Caption.ToUpper(), entry.Description, footer: entry.Hint);

            item.BeginGroup = entry.BeginGroup;

            if (entry.Image != null)
            {
                if (entry.Image is SvgImage svg)
                {
                    item.ImageOptions.SvgImage = svg;
                    item.ImageOptions.SvgImageSize = new Size(16, 16);
                    item.ImageOptions.SvgImageColorizationMode = (entry is AFPopupMenuEntry popupentry ? popupentry.SymbolColorizationMode : SvgImageColorizationMode.Default);
                }
                else if (entry.Image is Image img)
                    item.ImageOptions.Image = img;
            }
            else if (entry.ImageIndex >= 0)
            {
                item.ImageOptions.SvgImage = UI.GetImage((Symbol)entry.ImageIndex);
                item.ImageOptions.SvgImageSize = new Size(16, 16);
                item.ImageOptions.SvgImageColorizationMode = (entry is AFPopupMenuEntry popupentry ? popupentry.SymbolColorizationMode : SvgImageColorizationMode.Default);
            }

            menu.Items.Add(item);

            return item;
        }

        return null;
    }
}