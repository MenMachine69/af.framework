using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace AF.MVC;

/// <summary>
/// A combobox to select a Model-Type (View or Table).
/// Call 'LoadTypes' to load all registered Model-Types.
/// </summary>
[ToolboxItem(true)]
[SupportedOSPlatform("windows")]
[DefaultBindingProperty("SelectedModelType")]
[DesignerCategory("Code")]
public class AFComboboxModelType : ImageComboBoxEdit
{
    private readonly DevExpress.Utils.SvgImageCollection _images = new() { ImageColorizationMode = DevExpress.Utils.SvgImageColorizationMode.None };

    /// <summary>
    /// Event that is fired before a type will be added to the combobox.
    /// Set Cancel to true to prevent the type from being added.
    /// </summary>
    public event CancelEventHandler? BeforeAddType;

    /// <summary>
    /// Currently selected Model-Typ
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Type? SelectedModelType
    {
        get =>  ((ImageComboBoxItem)SelectedItem)?.Value is TypeDescription tdesc
                ? tdesc.Type 
                : SelectedItem is ImageComboBoxItem { Value: Type typ }
                    ? typ 
                    : typeof(Nullable);
        set
        {
            if (value == null || value == typeof(Nullable))
            {
                SelectedItem = Properties.Items.FirstOrDefault(item => item.Value is Type td && td == typeof(Nullable));
                return;
            }

            var tdesc = value.GetTypeDescription();
            SelectedItem = Properties.Items.FirstOrDefault(item => item.Value is TypeDescription td && td == tdesc);
        }
    }

    /// <summary>
    /// Access the currently selected Model-Type.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TypeDescription? SelectedType => ((ImageComboBoxItem)SelectedItem)?.Value as TypeDescription;

    /// <summary>
    /// Liefert true, wenn mindestens ein ModelTyp auswählbar ist.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool HasModels { get; private set; }

    /// <summary>
    /// Nur Auswahl von Models erlauben, die das Attribut AllowSelect auf true gesetzt haben (AFTable.AllowSelect)
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool OnlyAllowSelect { get; set; }

    /// <summary>
    /// Load all registered Model-Types.
    /// 
    /// Before a type will be added, the event BeforeAddType will be fired.
    /// If the event is canceled, the type will not be added.
    /// </summary>
    /// <param name="onlyBrowsable">load only types marked as </param>
    /// <param name="checkRights">Berechtigungen überprüfen. Wenn true, werden die Models angezeigt, für die der User berechtigt ist (siehe AFTable-Attribut)</param>
    /// <param name="includeViews">Ansicht von Views einbeziehen (Standard: false)</param>
    public void LoadTypes(bool onlyBrowsable, bool checkRights = true, bool includeViews = false)
    {
        List<ListItem> items = [];
        _images.Clear();
        _images.Add(UI.GetImage(Symbol.Database));

        if (!onlyBrowsable)
        {
            items.Add(new ListItem
            {
                Value = typeof(Nullable),
                Caption = "<kein Typ>",
                ImageIndex = _images.Count - 1
            });
        }

        foreach (var modelType in (onlyBrowsable
                     ? TypeEx.GetTypeDescriptions(t => t.IsBrowsable)
                     : TypeEx.GetTypeDescriptions(t => t.IsTable || t.IsView)).OrderBy(t => t.Name))
        {
            if (modelType.IsView && includeViews == false) continue;

            if (OnlyAllowSelect && !(modelType.IsTable && modelType.Table!.AllowSelect)) continue;

            if (checkRights && AFCore.SecurityService?.CurrentUser != null && !AFCore.SecurityService.CurrentUser.IsAdmin)
            {
                if (modelType.Table != null && modelType.Table.BrowseNeedAdminRights)
                    continue;

                if (modelType.Table != null && modelType.Table.BrowseNeedRights >= 0 && !AFCore.SecurityService.CurrentUser.HasRight(modelType.Table.BrowseNeedRights))
                    continue;
            }

            CancelEventArgs args = new();
            BeforeAddType?.Invoke(modelType, args);
            
            if (args.Cancel)
                continue;

            HasModels = true;

            if (modelType.GetController()?.TypeImage is SvgImage img)
                _images.Add(img);
            else
                _images.Add(UI.GetImage(Symbol.Database));


            items.Add(new ListItem
            {
                Value = modelType,
                Caption = (modelType.Context == null ? modelType.Name : modelType.Context.NamePlural),
                ImageIndex = _images.Count - 1
            });

            Properties.SmallImages = _images;
            this.Fill(items);
        }
    }
}


