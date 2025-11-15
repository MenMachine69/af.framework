using System.ComponentModel.Design;
using AF.MVC;
using DevExpress.Utils;
using DevExpress.Utils.Svg;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Informationen zu einer Datenbindung
/// </summary>
public sealed class BoundInfo
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="property"></param>
    /// <param name="control"></param>
    public BoundInfo(PropertyInfo property, Control control)
    {
        Property = property;
        Control = control;
    }

    /// <summary>
    /// Gebundene Eigenschaft
    /// </summary>
    public PropertyInfo Property { get; set; }

    /// <summary>
    /// Control, an das die Eigenschaft gebunden ist.
    /// </summary>
    public Control Control { get; set; }
}

/// <summary>
/// Eine Datenquelle automatisch an die Controls mit gleichem Namen binden
/// </summary>
[ToolboxItem(true)]
[SupportedOSPlatform("windows")]
public class AFBindingConnector : BindingSource, IBindingConnector
{
    private Type? _currentType;
    private AFErrorProvider? _errorProvider;
    private readonly List<AFBindingConnector> _childConnectors = [];
    private IBindable? _currentDatasource;

    private WeakEvent<EventHandler<BindingEventArgs>>? _objectStateChanged;

    /// <summary>
    /// Verknüpften Connector hinzufügen.
    /// 
    /// Beim Validieren und/oder binden werden auch alle verknüpften Connectoren berücksichtigt.
    /// </summary>
    /// <param name="connector"></param>
    public void RegisterChildConnector(AFBindingConnector connector)
    {
        if (_childConnectors.Contains(connector))
            return;

        _childConnectors.Add(connector);

        connector.ParentConnector = this;
    }

    /// <summary>
    /// Verknüpften Connector entfernen
    /// </summary>
    /// <param name="connector"></param>
    public void UnRegisterChildConnector(AFBindingConnector connector)
    {
        if (!_childConnectors.Contains(connector)) return;

        connector.ParentConnector = null;

        _childConnectors.Remove(connector);
    }

    /// <summary>
    /// Übergeordneter Connector, an den dieses Connector gebunden wurde (via RegisterChildConnector)
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFBindingConnector? ParentConnector { get; private set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="container"></param>
    public AFBindingConnector(IContainer container)
    {
        container.Add(this);
    }

    /// <summary>
    /// Gibt die von <see cref="System.Windows.Forms.BindingSource" /> verwendeten nicht verwalteten 
    /// Ressourcen und optional die verwalteten Ressourcen frei.</summary>
    /// <param name="disposing">
    /// <see langword="true" />, um sowohl verwaltete als auch nicht verwaltete Ressourcen freizugeben, 
    /// <see langword="false" />, um ausschließlich nicht verwaltete Ressourcen freizugeben.
    /// </param>
    protected override void Dispose(bool disposing)
    {
        if (UI.DesignMode) return;

        ParentConnector?.UnRegisterChildConnector(this);

        base.Dispose(disposing);
    }

    /// <summary>
    /// Setzt die Connector-Eigenschaft des gebundenen Objekts, wenn dieses IBindable implementiert.
    /// </summary>
    /// <param name="e">Argumente</param>
    protected override void OnCurrentChanged(EventArgs e)
    {
        if (UI.DesignMode) return;

        if (_currentDatasource != null) _currentDatasource.Connector = null;

        base.OnCurrentChanged(e);

        if (Current is not IBindable bindable) return;

        bindable.Connector = this;
        _currentDatasource = bindable;
    }

    /// <summary>
    /// Container-Control des Connectors. Ohne Angabe von StartContainerControl werden alle Controls dieses Containers gebunden  
    /// </summary>
    [Browsable(true)]
    [DefaultValue(null)]
    public ContainerControl? ContainerControl { get; set; }

    /// <summary>
    /// Container, dessen Controls gebunden werden soll. Leer = alle Controls des Fensters/UsersControls (ContainerControl)
    /// </summary>
    [Browsable(true)]
    [DefaultValue(null)]
    public Control? StartContainerControl { get; set; }

    /// <summary>
    /// Container, dessen Controls gebunden werden soll. Leer = alle Controls des Fensters/UsersControls (ContainerControl)
    /// </summary>
    [Browsable(true)]
    [DefaultValue(DataSourceUpdateMode.OnValidation)]
    public DataSourceUpdateMode BindingMode { get; set; } = DataSourceUpdateMode.OnValidation;

    /// <summary>
    /// Provider zur Anzeige von Fehlermeldungen.
    /// 
    /// Die Anzeige der Fehlermeldungen kann auch in einem bereits vorhandenen ErroroProvider angezeigt werden. 
    /// Dazu muss Validate mit diesem ErrorProvider aufgerufen werden.  
    /// </summary>
    [Browsable(true)]
    [DefaultValue(null)]
    public AFErrorProvider? ErrorProvider { get => _errorProvider; set => _errorProvider = value; }

    /// <summary>
    /// Container, dessen Controls ignoriert werden sollen und bei der Bindung nicht berücksichtigt werden.
    /// 
    /// Durch die Verwendung dieser Eigenschaft können mehrere BindingConnector-Komponenten genutzt werden.
    /// </summary>
    [Browsable(true)]
    [DefaultValue(null)]
    public Control? IgnoreContainerControl { get; set; }

    /// <summary>
    /// Eine Liste der momentan gebundenen Eigenschaften
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public List<BoundInfo> CurrentBoundProperties { get; } = [];

    /// <summary>
    /// <see cref="System.ComponentModel.ISite" />
    /// </summary>
    /// <returns>
    ///<see cref="System.ComponentModel.ISite" />
    /// </returns>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override ISite? Site
    {
        get => base.Site;
        set
        {
            base.Site = value;

            IDesignerHost? host = value?.GetService(typeof(IDesignerHost)) as IDesignerHost;
            IComponent? componentHost = host?.RootComponent;

            if (componentHost is ContainerControl control)
                ContainerControl = control;
        }
    }

    /// <summary>
    /// Datenquelle, die gebunden wird.
    /// 
    /// Die Datenquelle kann ein einzelnes Objekt oder eine Liste von Objekten sein (siehe BindingSource.DataSource)
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new object? DataSource
    {
        get => base.DataSource;
        set
        {
            if (UI.DesignMode) return;

            if (value != null && value is not IBindingList)
                CreateBindings(value.GetType());

            if (value != null)
                base.DataSource = value;
            else
                base.Clear();
        }
    }


    /// <summary>
    /// Alle Bindings für einen bestimmten Objekttyp erzeugen (Typ der Objekte, die als Datenquelle verwendet werden).
    /// Diese Methode wird auch bei der Zuweisung einer Datenquelle an DataSource aufgerufen und muss daher in der Regel 
    /// nicht manuell aufgerufen werden.
    /// </summary>
    /// <param name="type"></param>
    public void CreateBindings(Type type)
    {
        // prüfen, ob der aktuelle Typ schon dem gebundenen entspricht
        if (_currentType != null && _currentType == type)
            return;

        _currentType = type;

        CurrentBoundProperties.Clear();
        boundCommands.Clear();

        base.DataSource = type;

        Control? container = StartContainerControl ?? ContainerControl;

        if (container != null)
            boundToControls(container.Controls, type);

    }

    /// <summary>
    /// Validieren der gebundenen Daten
    /// </summary>
    /// <param name="errors">collection of errors</param>
    /// <returns>true wenn valide, sonst false</returns>
    public bool Validate(ValidationErrorCollection? errors)
    {
        _errorProvider?.ClearErrors();

        return Validate(_errorProvider, errors);
    }

    /// <summary>
    /// Validieren der gebundenen Daten
    /// </summary>
    /// <param name="errorprovider">Errorprovider, der die Fehler anzeigen soll</param>
    /// <param name="errors">collection of errors</param>
    /// <returns>true wenn valide, sonst false</returns>
    public bool Validate(AFErrorProvider? errorprovider, ValidationErrorCollection? errors)
    {
        object ds = Current!;
        bool ret = true;

        errors ??= [];

        if (ds == null)
            throw new InvalidOperationException(@"Can't found data source to validate.");

        ret = (ret && ((Base)ds).IsValid(errors));

        if (_childConnectors.Count > 0)
        {
            foreach (var childConnector in _childConnectors)
                ret = childConnector.Validate(errorprovider, errors);
        }

        if (ret && ds is IBindable bindable)
            ret = ret && bindable.IsValid(errors);

        if (errors.Count > 0)
            errorprovider?.FromCollection(errors);

        return ret;
    }

    private readonly Dictionary<string, AFCommand> boundCommands = [];


    private void boundToControls(Control.ControlCollection collection, Type type)
    {
        TypeDescription tdesc = type.GetTypeDescription();
        var controller = type.GetControllerOrNull();

        foreach (Control ctrl in collection)
        {
            if (ctrl.Controls.Count > 0 &&
                (IgnoreContainerControl == null || IgnoreContainerControl != ctrl))
                boundToControls(ctrl.Controls, type);

            ctrl.DataBindings.Clear();
            
            if (controller != null && ctrl is BarDockControl bdc)
            {
                foreach (Bar bar in bdc.Manager.Bars)
                {
                    foreach (BarItemLink barItemLink in bar.ItemLinks)
                    {
                        var barItem = barItemLink.Item;
                        if (barItem == null)
                            continue;

                        var command = controller.GetCommand(barItem.Name);

                        if (command != null)
                        {
                            boundCommands[barItem.Name] = command;

                            if (barItem.Caption.IsEmpty())
                                barItem.Caption = command.Caption;
                            if (barItem.Description.IsEmpty())
                                barItem.Description = command.Long;

                            var img = command.Image ?? (command.ImageIndex > -1 ? UI.GetImage((Symbol)command.ImageIndex) : null);

                            if (img != null)
                            {
                                if (img is SvgImage svg)
                                {
                                    barItem.ImageOptions.SvgImage = svg;
                                    barItem.ImageOptions.SvgImageSize = new(16, 16);
                                }
                                else if (img is Image image)
                                {
                                    barItem.ImageOptions.Image = image;
                                }
                            }

                            barItem.ItemClick -= invokeCommand;
                            barItem.ItemClick += invokeCommand;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(ctrl.Name))
                continue;

            if (controller != null && ctrl is SimpleButton btn)
            {
                var command = controller.GetCommand(btn.Name);

                if (command != null)
                {
                    boundCommands[btn.Name] = command;

                    if (btn.Text.IsEmpty())
                        btn.Text = command.Caption;

                    var img = command.Image ?? (command.ImageIndex > -1 ? UI.GetImage((Symbol)command.ImageIndex) : null);

                    if (img != null)
                    {
                        if (img is SvgImage svg)
                        {
                            btn.ImageOptions.SvgImage = svg;
                            btn.ImageOptions.SvgImageSize = new(16, 16);
                            btn.ImageOptions.ImageToTextAlignment = ImageAlignToText.LeftCenter;
                            btn.ImageOptions.ImageToTextIndent = 10;
                        }
                        else if (img is Image image)
                        {
                            btn.ImageOptions.Image = image;
                            btn.ImageOptions.ImageToTextAlignment = ImageAlignToText.LeftCenter;
                            btn.ImageOptions.ImageToTextIndent = 10;
                        }
                    }

                    btn.Click -= invokeCommand;
                    btn.Click += invokeCommand;
                }
            }

            if (controller != null && ctrl is LabelControl lbl)
            {
                var command = controller.GetCommand(lbl.Name);

                if (command != null)
                {
                    boundCommands[lbl.Name] = command;

                    if (lbl.Text.IsEmpty())
                        lbl.Text = command.Caption;

                    var img = command.Image ?? (command.ImageIndex > -1 ? UI.GetImage((Symbol)command.ImageIndex) : null);

                    if (img != null)
                    {
                        if (img is SvgImage svg)
                        {
                            lbl.ImageOptions.SvgImage = svg;
                            lbl.ImageOptions.SvgImageSize = new(16, 16);
                            lbl.ImageAlignToText = ImageAlignToText.LeftCenter;
                            lbl.IndentBetweenImageAndText = 10;
                        }
                        else if (img is Image image)
                        {
                            lbl.ImageOptions.Image = image;
                            lbl.ImageAlignToText = ImageAlignToText.LeftCenter;
                            lbl.IndentBetweenImageAndText = 10;
                        }
                    }

                    lbl.Click -= invokeCommand;
                    lbl.Click += invokeCommand;
                }
            }

            

            PropertyInfo? property = type.GetProperty(ctrl.Name);
            PropertyDescription? pdesc = null;

            if (tdesc != null && tdesc.Properties.TryGetValue(ctrl.Name, out var tdescProperty))
                pdesc = tdescProperty;

            if (property == null || pdesc == null) continue;

            if (pdesc.Binding == null) continue;

            Binding? binding = ctrl.Bound(property, this, BindingMode);

            if (binding == null) continue;

            if (property.PropertyType == typeof(Image))
            {
                binding.Format += (_, e) =>
                {
                    if (e.Value is not Image)
                        e.Value = new Bitmap(1, 1);
                };
            }

            if (pdesc.Binding.Link.IsNotEmpty() && ctrl is AFLabelModelLink link)
                ctrl.DataBindings.Add(nameof(AFLabelModelLink.ModelLink), this, pdesc.Binding.Link, true, DataSourceUpdateMode.OnPropertyChanged, ModelLink.Empty );

            object? nullvalue = null;

            if (property.PropertyType.IsValueType)
                nullvalue = Activator.CreateInstance(property.PropertyType);

            CurrentBoundProperties.Add(new BoundInfo(property, ctrl));
            binding.DataSourceNullValue = nullvalue;
            binding.DataSourceUpdateMode = BindingMode;

            if (pdesc.Context == null) continue;

            if (pdesc.Context.Description.IsNotEmpty() && pdesc.Context.NameSingular.IsNotEmpty())
            {
                if (ctrl.HasSet(@"SuperTip"))
                {
                    ctrl.InvokeSet(@"SuperTip",
                        UI.GetSuperTip(pdesc.Context.NameSingular, pdesc.Context.Description,
                            pdesc.Context.Hint));
                }
                else
                {
                    ToolTipController.DefaultController.SetSuperTip(ctrl,
                        UI.GetSuperTip(pdesc.Context.NameSingular, pdesc.Context.Description,
                            pdesc.Context.Hint));
                }
            }

            int maxLength = 0;

            if (pdesc.Binding != null)
            {
                maxLength = pdesc.Binding.MaxLength;

                if (pdesc.Binding.ReadOnly)
                {
                    if (ctrl is BaseEdit edit)
                        edit.Properties.ReadOnly = pdesc.Binding.ReadOnly;
                    else
                    {
                        if (ctrl.HasSet(@"ReadOnly"))
                            ctrl.InvokeSet(@"ReadOnly", pdesc.Binding.ReadOnly);
                    }
                }

                if (pdesc.Binding.DisplayFormat.IsNotEmpty())
                {
                    binding.FormattingEnabled = true;
                    binding.FormatString = pdesc.Binding.DisplayFormat;

                    if (ctrl is BaseEdit edit)
                    {
                        if (property.PropertyType.IsNumericType())
                        {
                            edit.Properties.EditFormat.FormatType = FormatType.Numeric;
                            edit.Properties.EditFormat.FormatString = pdesc.Binding.DisplayFormat;
                            edit.Properties.DisplayFormat.FormatType = FormatType.Numeric;
                            edit.Properties.DisplayFormat.FormatString = pdesc.Binding.DisplayFormat;
                        }
                        else if (property.PropertyType == typeof(DateTime))
                        {
                            edit.Properties.EditFormat.FormatType = FormatType.DateTime;
                            edit.Properties.EditFormat.FormatString = pdesc.Binding.DisplayFormat;
                            edit.Properties.DisplayFormat.FormatType = FormatType.DateTime;
                            edit.Properties.DisplayFormat.FormatString = pdesc.Binding.DisplayFormat;
                        }

                        if (edit.Properties.HasSet(@"EditMask"))
                            edit.Properties.InvokeSet(@"EditMask", pdesc.Binding.DisplayFormat);
                    }

                    if (ctrl is TextBoxBase txedit)
                        txedit.ReadOnly = pdesc.Binding.ReadOnly;
                }
            }

            if (maxLength <= 0 && pdesc.Field != null && pdesc.Field.MaxLength > 0)
                maxLength = pdesc.Field.MaxLength;

            if (maxLength > 0)
            {
                if (ctrl is BaseEdit edit && edit.Properties.HasSet(@"MaxLength"))
                    edit.Properties.InvokeSet(@"MaxLength", maxLength);
                else
                {
                    if (ctrl.HasSet(@"MaxLength"))
                        ctrl.InvokeSet(@"MaxLength", maxLength);
                }
            }
        }
    }

    private void invokeCommand(object? sender, EventArgs e)
    {
        if (sender == null) return;

        string commandName;
        if (sender is Control ctrl)
            commandName = ctrl.Name;
        else if (e is ItemClickEventArgs itemClickEventArgs)
        {
            commandName = itemClickEventArgs.Item.Name;
            ctrl = itemClickEventArgs.Item.Manager.DockControls[0];
        }
        else
            return;

        if (!boundCommands.TryGetValue(commandName, out var command)) return;

        CommandArgs args = new()
        {
            Editor = ctrl.GetParentControl<IEditor>(),
            Dialog = ctrl.GetParentControl<IDialogContainer>(),
            Page = ctrl.GetParentControl<IViewPage>(),
            Model = DataSource is IList ? Current as IModel : DataSource as IModel
        };

        args.ParentControl = args.Editor?.ParentPage;

        ICommandResultDisplay? handler = ctrl.GetParentControl<ICommandResultDisplay>() ?? UI.Shell as ICommandResultDisplay;

        if (handler != null)
            handler.HandleResult(command.Execute(args));
        else
            command.Execute(args);
    }

    /// <summary>
    /// Methode, die vom gebundenen IBindable-Objekt aufgerufen werden kann, 
    /// um den Connector über Statusänderungen zu informieren. Kann z.B. genutzt werden, 
    /// um in der UI bestimmte Controls zu aktivieren/deaktivieren.
    /// 
    /// Diese Methode ruft das ObjectStateChanged - Ereignis des Connectors auf.
    /// </summary>
    /// <param name="msg">Typ der Benachrichtigung</param>
    /// <param name="data">IBindable-Objekt, dass die Nachricht geschickt hat</param>
    public void StateChanged(eBindingStateMessage msg, IBindable? data)
    {
        _objectStateChanged?.Raise(this, new BindingEventArgs { Connector = this, Data = data, Message = msg });
    }


    /// <summary>
    /// Ereignis, das ausgelöst wird, wenn ein an den Connector gebundenes IBindable-Objekt eine Statusänderung meldet.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public event EventHandler<BindingEventArgs> ObjectStateChanged
    {
        add
        {
            _objectStateChanged ??= new();
            _objectStateChanged.Add(value);
        }
        remove => _objectStateChanged?.Remove(value);
    }
}


/// <summary>
/// Eventargumente für ein Event wie AfterSave etc.
/// </summary>
public sealed class BindingEventArgs : EventArgs
{
    /// <summary>
    /// Daten zur Beschreibung des Events
    /// </summary>
    public eBindingStateMessage Message { get; set; }

    /// <summary>
    /// Datenobjekt, dass die Nachricht schickt
    /// </summary>
    public IBindable? Data { get; set; }

    /// <summary>
    /// Connector, der die Nachricht empfangen hat.
    /// </summary>
    public IBindingConnector? Connector { get; set; }
}