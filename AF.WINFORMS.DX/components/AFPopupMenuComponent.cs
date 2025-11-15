using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;

namespace AF.WINFORMS.DX;

/// <summary>
/// Komponente zur einfachen Erzeugung und Anzeige von Popup-Menus basierend auf DXPopup
/// </summary>
[ToolboxItem(true)]
[SupportedOSPlatform("windows")]
public class AFPopupMenuComponent : AFVisualComponentBase
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="container"></param>
    public AFPopupMenuComponent(IContainer container)
    {
        container.Add(this);

        if (UI.DesignMode) return;
        
        itemClick += _itemClick;
        dxitemClick += _dxitemClick;
    }

    void _dxitemClick(object? sender, EventArgs e)
    {
        if (sender != null && sender is DXMenuItem)
            PopupMenuItemClick?.Invoke((string)((DXMenuItem)sender).Tag, EventArgs.Empty);
    }

    void _itemClick(object? sender, ItemClickEventArgs e)
    {
        PopupMenuItemClick?.Invoke(e.Item.Name, EventArgs.Empty);
    }

    /// <summary>
    /// Ereignis, das beim Klicken eines Menüpunktes ausgelöst wird
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal event ItemClickEventHandler? itemClick;

    /// <summary>
    /// Ereignis, das beim Klicken eines Menüpunktes ausgelöst wird
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal event EventHandler? dxitemClick;

    /// <summary>
    /// Ereignis, das beim Klicken eines Menüpunktes ausgelöst wird
    /// </summary>
    [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Category("UI")]
    public event EventHandler? PopupMenuItemClick;

    /// <summary>
    /// Bilder die im menü angezeigt werden sollen
    /// </summary>
    [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Category("UI")]
    public SvgImageCollection Images { get; set; } = new();

    /// <summary>
    /// Menüs, die angezeigt werden können (via ShowPopup)
    /// </summary>
    [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Category("UI")]
    public List<PopupMenuDefinition> Menus { get; set; } = [];

    /// <summary>
    /// Ein PoupMenu anzeigen
    /// </summary>
    /// <param name="name">Name des Menus</param>
    /// <param name="owner">Owner in dem das Menü angezeigt wird</param>
    /// <param name="location">Position des Menüs</param>
    /// <param name="manager"></param>
    public void ShowPopup(string name, BarManager manager, Control owner, Point location)
    {
        PopupMenuDefinition? def = Menus.FirstOrDefault(m => m.Name == name);

        if (def == null)
            throw new ArgumentException($@"No menu with such name ({name}).");

        if (def.Entrys.Count < 1)
            throw new Exception($@"Menu does not contain entries ({name}).");

        PopupMenu menu = new(Container)
        {
            Manager = manager
        };

        manager.BoundCommandsToMenu(menu, def.Entrys.OfType<IMenuEntry>().ToArray(), itemClick!, imgsize: Images.ImageSize.Width);

        menu.ShowPopup(location);
    }

    /// <summary>
    /// Ein PoupMenu anzeigen
    /// </summary>
    /// <param name="name">Name des Menus</param>
    /// <param name="owner">Owner in dem das Menü angezeigt wird</param>
    /// <param name="location">Position des Menüs</param>
    /// <param name="viewType">Anzeigetyp des Menüs (optional)</param>
    public void ShowPopup(string name, Control owner, Point location, MenuViewType viewType = MenuViewType.Menu)
    {
        PopupMenuDefinition? def = Menus.FirstOrDefault(m => m.Name == name);

        if (def == null)
            throw new ArgumentException($@"No menu with such name ({name}).");

        if (def.Entrys.Count < 1)
            throw new Exception($@"Menu does not contain entrys ({name}).");

        DXPopupMenu menu = new()
        {
            MenuViewType = viewType
        };

        Dictionary<string, DXSubMenuItem> subs = new();

        foreach (AFPopupMenuEntry entry in def.Entrys)
        {
            if (entry.Caption.Contains('\\'))
            {
                string subname = entry.Caption[..entry.Caption.LastIndexOf('\\')];

                if (!subs.ContainsKey(subname))
                    _createSubs(subname, ref menu, ref subs);

                DXMenuItem item = new(entry.Caption[(entry.Caption.LastIndexOf('\\') + 1)..], dxitemClick);
                _setupItem(item, entry);

                subs[subname].Items.Add(item);
            }
            else
            {
                DXMenuItem item = new(entry.Caption, dxitemClick);

                _setupItem(item, entry);

                menu.Items.Add(item);
            }
        }

        menu.ShowPopup(owner, location);
    }

    void _setupItem(DXMenuItem item, AFPopupMenuEntry entry)
    {
        item.Tag = entry.Name;

        if (entry.Caption.IsNotEmpty() && entry.Description.IsNotEmpty())
            item.SuperTip = UI.GetSuperTip(entry.Caption, entry.Description ?? "", entry.Hint);

        if (entry.ImageIndex >= 0 && Images.Count >= (entry.ImageIndex + 1))
        {
            item.ImageOptions.SvgImage = Images[entry.ImageIndex];
            item.ImageOptions.SvgImageColorizationMode = Images.ImageColorizationMode;
            item.ImageOptions.SvgImageSize = new Size(Images.ImageSize.Width, Images.ImageSize.Height);
        }
    }

    void _createSubs(string subname, ref DXPopupMenu menu, ref Dictionary<string, DXSubMenuItem> subs)
    {
        string[] subnames = subname.Split(['\\'], StringSplitOptions.RemoveEmptyEntries);
        string currsub = "";
        DXSubMenuItem? parentsub = null;

        foreach (string sub in subnames)
        {
            if (currsub.Contains('\\'))
                parentsub = subs[currsub];

            currsub += (currsub.Length > 0 ? '\\' : "") + sub;

            if (subs.ContainsKey(currsub) == false)
            {
                DXSubMenuItem submenu = new(sub);

                subs.Add(currsub, submenu);

                if (parentsub == null)
                    menu.Items.Add(submenu);
                else
                    parentsub.Items.Add(submenu);
            }
        }
    }
}

/// <summary>
/// Ein Popup Menu
/// </summary>
[Serializable]
public class PopupMenuDefinition
{
    /// <summary>
    /// Name des Menüs
    /// 
    /// Der Name muss eindeutig sein um die Menüs vorneinander unterscheiden zu können
    /// </summary>
    [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Category("UI")]
    public string Name { get; set; } = "";


    /// <summary>
    /// Liste der Einträge im Menü
    /// </summary>
    [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Category("UI")]
    public List<AFPopupMenuEntry> Entrys { get; set; } = [];
}

