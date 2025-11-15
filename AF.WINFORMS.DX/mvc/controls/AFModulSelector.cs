using DevExpress.Utils;
using DevExpress.Utils.Svg;
using DevExpress.XtraBars.Navigation;

namespace AF.MVC;

/// <summary>
/// Auswahl von Modulen und direkte Commands in einer Seitenleiste (AccordionControl) im Stile eines Hamburger Menus.
/// </summary>
[ToolboxItem(true)]
[SupportedOSPlatform("windows")]
public class AFModulSelector : AccordionControl
{
    private bool _init;

    private void init()
    {
        ViewType = AccordionControlViewType.HamburgerMenu;
        ExpandElementMode = ExpandElementMode.Multiple;
        OptionsFooter.ActiveGroupDisplayMode = ActiveGroupDisplayMode.GroupHeaderAndContent;
        OptionsHamburgerMenu.DisplayMode = AccordionControlDisplayMode.Overlay;
        OptionsMinimizing.PopupFormAutoHeightMode = AccordionPopupFormAutoHeightMode.FitContent;
        RightToLeft = RightToLeft.No;
        ScrollBarMode = ScrollBarMode.Fluent;
        ShowFilterControl = ShowFilterControl.Always;
        _init = true;
    }

    /// <summary>
    /// Gruppe hinzufügen
    /// </summary>
    /// <param name="caption">Beschriftung</param>
    /// <param name="symbol">Symbol/Bild</param>
    /// <param name="allowColorSkin">AutoColor füpr Symbol aktivieren (default: true)</param>
    /// <param name="neededRight">Erforderliche Berechtigung, damit die Gruppe angezeigt wird (default: -1 = keine). 
    /// Achtung! ist der Wert > -1 und AFCore.SecurityService NICHT gesetzt, wird keine Gruppe erzeugt!</param>
    /// <param name="tooltip">Tooltip für das Element</param>
    /// <returns>die erstellte Gruppe oder NULL (wenn z.B. Berechtigung nicht vorhanden ist)</returns>
    public AccordionControlElement? AddGroup(string caption, SvgImage? symbol = null, bool allowColorSkin = true, int neededRight = -1, SuperToolTip? tooltip = null)
    {
        if (neededRight > 0 && !(AFCore.SecurityService?.HasRight(neededRight) ?? false)) return null;

        if (_init) init();
        
        var group = new AccordionControlElement();

        group.Expanded = true;
        group.Name = @"group_"+Elements.Count;
        group.SuperTip = tooltip;
        group.Text = caption;
        group.Style = ElementStyle.Group;

        if (symbol != null)
        {
            group.ImageOptions.SvgImage = symbol;
            group.ImageOptions.SvgImageSize = ImageOptions.SvgImageSize;
            group.ImageOptions.SvgImageColorizationMode = allowColorSkin ? SvgImageColorizationMode.Full : SvgImageColorizationMode.None;
        }

        Elements.Add(group);

        return group;
    }

    /// <summary>
    /// Modul hinzufügen 
    /// </summary>
    /// <typeparam name="TModel">Typ des Modells, das für das Modul verwendet wird; dieses Modell wird verwendet, um den Controller für das Modul zu erhalten</typeparam>
    /// <param name="group">Gruppe, in der das Modul platziert wird (verwenden Sie AddGroup, um eine Gruppe zu erstellen) oder null, wenn das Modul in der obersten Ebene wählbar ist</param>
    /// <param name="caption">Beschriftung</param>
    /// <param name="symbol">Symbol/Bild</param>
    /// <param name="allowColorSkin">AutoColor füpr Symbol aktivieren (default: true)</param>
    /// <param name="neededRight">Erforderliche Berechtigung, damit das Modul angezeigt wird (default: -1 = keine). 
    /// Achtung! ist der Wert > -1 und AFCore.SecurityService NICHT gesetzt, wird kein Modul erzeugt!</param>
    /// <param name="tooltip">Tooltip für das Element</param>
    public void AddModule<TModel>(AccordionControlElement? group, string caption, SvgImage? symbol = null, bool allowColorSkin = true, int neededRight = -1, SuperToolTip? tooltip = null) where TModel : class, IModel
    {
        addElement(group, null, null, caption, symbol, allowColorSkin, neededRight, tooltip, typeof(TModel), true);
    }

    /// <summary>
    /// Einfaches AFCommand hinzufügen, das direkt ausgeführt wird.
    /// </summary>
    /// <param name="command">das auszuführenden Command</param>
    /// <param name="group">Gruppe, in der das Command platziert wird (verwenden Sie AddGroup, um eine Gruppe zu erstellen) oder null, wenn das Command in der obersten Ebene wählbar ist</param>
    /// <param name="caption">Beschriftung</param>
    /// <param name="symbol">Symbol/Bild</param>
    /// <param name="allowColorSkin">AutoColor füpr Symbol aktivieren (default: true)</param>
    /// <param name="neededRight">Erforderliche Berechtigung, damit das Command angezeigt wird (default: -1 = keine). 
    /// Achtung! ist der Wert > -1 und AFCore.SecurityService NICHT gesetzt, wird kein Command erzeugt!</param>
    /// <param name="tooltip">Tooltip für das Element</param>
    /// <param name="controller">cController, der das Command enthält</param>
    public void AddCommand(AccordionControlElement? group, IController controller, AFCommand command, string caption, SvgImage? symbol = null, bool allowColorSkin = true, int neededRight = -1, SuperToolTip? tooltip = null)
    {
        addElement(group, controller, command, caption, symbol, allowColorSkin, neededRight, tooltip, null, true);
    }

    /// <summary>
    /// Trennlinie hinzufügen
    /// </summary>
    public void AddSeparator(AccordionControlElement? group = null)
    {
        if (_init) init();

        if (group != null)
            group.Elements.Add(new AccordionControlSeparator());
        else
            Elements.Add(new AccordionControlSeparator());
    }

    /// <summary>
    /// Aufruf eines Overlays hinzufügen
    /// </summary>
    /// <typeparam name="TOverlay">Typ des Overlays</typeparam>
    /// <param name="group">Gruppe, in der das Overlay platziert wird (verwenden Sie AddGroup, um eine Gruppe zu erstellen) oder null, wenn das Overlay in der obersten Ebene wählbar ist</param>
    /// <param name="caption">Beschriftung</param>
    /// <param name="symbol">Symbol/Bild</param>
    /// <param name="allowColorSkin">AutoColor füpr Symbol aktivieren (default: true)</param>
    /// <param name="neededRight">Erforderliche Berechtigung, damit das Overlay angezeigt wird (default: -1 = keine). 
    /// Achtung! ist der Wert > -1 und AFCore.SecurityService NICHT gesetzt, wird kein Overlay erzeugt!</param>
    /// <param name="tooltip">Tooltip für das Element</param>
    public void AddOverlay<TOverlay>(AccordionControlElement? group, string caption, SvgImage? symbol = null, bool allowColorSkin = true, int neededRight = -1, SuperToolTip? tooltip = null) where TOverlay : class, IOverlayControl, new()
    {
        addElement(group, null, null, caption, symbol, allowColorSkin, neededRight, tooltip, typeof(TOverlay), false);
    }

    private void addElement(AccordionControlElement? group, IController? controller, AFCommand? command, string caption, SvgImage? symbol, bool allowColorSkin, int neededRight, SuperToolTip? tooltip, Type? type, bool isModul)
    {
        if (neededRight > 0 && !(AFCore.SecurityService?.HasRight(neededRight) ?? false)) return;

        if (_init) init();


        var modul = new AccordionControlElement
        {
            Expanded = true,
            SuperTip = tooltip,
            Text = caption,
            Style = ElementStyle.Item,
            Tag = (controller != null && command != null ? new Tuple<IController, AFCommand>(controller, command) : type)
        };

        modul.Click += invokeClick;

        if (group != null)
        {
            modul.Name = group.Name+(isModul ? @"_modul" : @"_overlay")+group.Elements.Count;
            modul.HeaderIndent = 50;
        }
        
        else
            modul.Name = (isModul ? @"_modul" : @"_overlay")+Elements.Count;
        
        if (symbol != null)
        {
            modul.ImageOptions.SvgImage = symbol;
            modul.ImageOptions.SvgImageSize = ImageOptions.SvgImageSize;
            modul.ImageOptions.SvgImageColorizationMode = allowColorSkin ? SvgImageColorizationMode.Full : SvgImageColorizationMode.None;
        }

        if (group != null)
            group.Elements.Add(modul);
        else
            Elements.Add(modul);
    }

    private void invokeClick(object? sender, EventArgs e)
    {
        if (sender is not AccordionControlElement element) return;

        if (element.Tag is Tuple<IController, AFCommand> commandTuple) // is element linked to a command?
        {
            // execute this command directly...
            commandTuple.Item2.Execute(new CommandArgs());
        }
        else
        {
            if (element.Name.Contains(@"_modul"))
            {
                if (element.Tag is not Type elementtype)
                    throw new ArgumentException($"Unknown ModelType for module {element.Name}.");

                ((AFWinFormsMVCApp)AFCore.App).Shell.ViewManager.OpenPage(ObjectEx.CreateInstance<IModel>(elementtype));
            }
            else if (element.Name.Contains(@"_overlay"))
            {
                // TODO: Show overlay in ViewManager...
            }
        }
    }
}
