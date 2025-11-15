using AF.MVC;
using DevExpress.Utils;
using DevExpress.Utils.Menu;

namespace AF.WINFORMS.DX;

/// <summary>
/// Label um Text als wie einen Link anzuzeigen. Ein Klick auf den Link öffnet ein Kontextmenü. 
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
public sealed class AFLabelModelLink : AFLabelBoldText
{
    private ModelLink? _modelLink;
    private DXPopupMenu? popup;
    private bool hasMenuEntries;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFLabelModelLink()
    {
        if (UI.DesignMode) return;

        Padding = new(0, 2, 0, 2);

        AppearanceHovered.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.Highlight);
        AppearanceHovered.Options.UseForeColor = true;
        AppearanceHovered.Options.UseFont = true;
        AppearanceHovered.FontStyleDelta = FontStyle.Bold | FontStyle.Underline;

        Click += (_, _) =>
        {
            if (ModelLink == null) return;

            var controller = ModelLink.ModelType.GetUIControllerOrNull();
            if (controller == null) return;


            if (popup == null)
            {
                popup = new();

                // ReSharper disable once CoVariantArrayConversion
                IMenuEntry[] menuEntries = controller.GetCommands(eCommandContext.GridButton, string.Empty);
                
                foreach (var entry in menuEntries)
                {
                    popup.AddMenuEntry(entry, tag: entry);
                    hasMenuEntries = true;
                }

                // ReSharper disable once CoVariantArrayConversion
                menuEntries = controller.GetCommands(eCommandContext.LinkContext, string.Empty);

                foreach (var entry in menuEntries)
                {
                    popup.AddMenuEntry(entry, tag: entry);
                    hasMenuEntries = true;
                }

                popup.ItemClick += (_, e) =>
                {
                    if (ModelLink == null || ModelLink.ModelID.Equals(Guid.Empty)) return;

                    if (e.Item.Tag is not AFCommand command) return;

                    var form = FindForm() as ICommandResultDisplay;

                    if (form == null)
                        command.Execute(new CommandArgs() { Model = ModelLink.Model });
                    else
                        form.HandleResult(command.Execute(new CommandArgs() { Model = ModelLink.Model }));
                };
            }

            if (hasMenuEntries)
                popup.ShowPopup(this, new(0, Height + 1));
        };
    }

    /// <summary>
    /// ModelLink des Models, auf das sich die Anzeige bezieht.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ModelLink? ModelLink
    {
        get => _modelLink;
        set
        {
            _modelLink = value;
            Visible = _modelLink != null && !_modelLink.ModelID.Equals(Guid.Empty);
        }
    }
}