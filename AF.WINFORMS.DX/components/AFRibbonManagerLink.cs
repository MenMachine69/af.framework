using AF.MVC;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;

namespace AF.WINFORMS.DX;

/// <summary>
/// Connects a RibbonBarManager to a Controller
/// 
/// After that any ItemClick in a bar will be routed 
/// to a controller command with the same name. 
/// There is no more need to implement the Click event for each item.
/// </summary>
[ToolboxItem(true)]
[DesignerCategory("Code")]
public class AFRibbonManagerLink : AFVisualComponentBase
{
    private IController? _controller;
    private WeakEvent<EventHandler<EventArgs>>? _commandExecuted;
    private WeakEvent<EventHandler<CancelEventArgs>>? _requestExecutionData;

    /// <summary>
    /// Assign a bar Manager 
    /// </summary>
    [Browsable(true)]
    [DefaultValue(null)]
    public RibbonBarManager? BarManager { get; set; }


    /// <summary>
    /// Controller to connect to
    /// 
    /// This controller will be used to execute the commands. 
    /// </summary>
    [SupportedOSPlatform("windows")]
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IController? Controller
    {
        get => _controller;
        set
        {
            _controller = value;

            if (BarManager == null) throw new NullReferenceException(@"No BarManager assigned.");

            BarManager.ItemClick += barManagerItemClick;
        }
    }

    /// <summary>
    /// Add "Cmd" to the MenuItem-Name to create the Command-Name in the controller.
    /// 
    /// Example: name of the MenuItem is "Save" then the command name is "CmdSave".
    /// </summary>
    [Browsable(true)]
    [DefaultValue(@"Cmd")]
    public string? PrefixCommandName { get; set; } = @"Cmd";

    /// <summary>
    /// Length of the prefix to remove from the MenuItem-Name to create the Command-Name in the controller.
    /// 
    /// Example: name of the MenuItem is "menSave" and PrefixMenuNameLength is 3 then the command name is "Save".
    /// </summary>
    [Browsable(true)]
    [DefaultValue(0)] 
    public int PrefixMenuNameLength { get; set; } = 0;


    [SupportedOSPlatform("windows")]
    private void barManagerItemClick(object sender, ItemClickEventArgs e)
    {
        if (Controller == null) return;

        string commandName = e.Item.Name;
        
        var command = Controller.GetCommand(commandName);
        
        if (command == null)
        {
            if (PrefixMenuNameLength > 0)
                commandName = commandName.Substring(PrefixMenuNameLength);

            if (PrefixCommandName != null)
                commandName = PrefixCommandName + commandName;
        }

        command = Controller.GetCommand(commandName);

        
        // if no command found return without any action
        if (command == null) return;

        CommandArgs data = new() { CommandSource = e.Item, ParentControl = ContainerControl }; 

        CancelEventArgs args = new();
        _requestExecutionData?.Raise(data, args);
        if (args.Cancel) return;

        var result = command.Execute(data);
        
        if (_commandExecuted != null)
            _commandExecuted.Raise(result, EventArgs.Empty);
        else
        {
            ICommandResultDisplay? commandResultDisplay = ContainerControl as ICommandResultDisplay;

            commandResultDisplay ??= findCommandResultDisplay(ContainerControl?.Parent);

            if (commandResultDisplay != null)
                commandResultDisplay.HandleResult(result);
            else
                AFCore.App.HandleResult(result);
        }
    }

    /// <summary>
    /// Find the first parent control that implements ICommandResultDisplay
    /// </summary>
    /// <param name="ctrl">start searching with</param>
    /// <returns>the commandresultdisplay or null</returns>
    private ICommandResultDisplay? findCommandResultDisplay(Control? ctrl)
    {
        if (ctrl == null) return null;

        if (ctrl is ICommandResultDisplay display)
            return display;

        if (ctrl.Parent != null) return findCommandResultDisplay(ctrl.Parent);
       
        
        if (ctrl.FindForm() is ICommandResultDisplay)
            return ctrl.FindForm() as ICommandResultDisplay;

        return null;

    }


    /// <summary>
    /// Event that is fired after a command is executed
    /// 
    /// If this is null the default handling of the command result is used.
    /// That means the the app tries to find a parent control that implements 
    /// ICommandResultDisplay and calls the DisplayResult method.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public event EventHandler<EventArgs> OnCommandExecuted
    {
        add
        {
            _commandExecuted ??= new();
            _commandExecuted.Add(value);
        }
        remove => _commandExecuted?.Remove(value);
    }

    /// <summary>
    /// Event that is fired when the controller needs execution data
    /// 
    /// Sender is allway a CommandData object, that can be filled with data.
    /// The observer can also cancel the command execution by setting the Cancel 
    /// property of the event args.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public event EventHandler<CancelEventArgs> OnRequestExecutionData
    {
        add
        {
            _requestExecutionData ??= new();
            _requestExecutionData.Add(value);
        }
        remove => _requestExecutionData?.Remove(value);
    }
}