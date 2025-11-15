using DevExpress.XtraEditors;
using AF.MVC;

namespace AF.WINFORMS.DX;

/// <summary>
/// Combobox to display objects in a grid for selection
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[DesignerCategory("Code")]
public class AFComboboxLookup : SearchLookUpEdit
{
    /// <summary>
    /// Setup the Combobox for the given Type.
    /// 
    /// Uses the UI Controlls
    /// </summary>
    /// <param name="type"></param>
    protected void Setup(Type type)
    {
        AFGridSetup? setup = null;

        var controller = type.GetController();

        if (controller is IControllerUI uiController)
            setup = uiController.GetGridSetup(eGridStyle.ComboboxEntrys);

        setup ??= type.GetTypeDescription().GetGridSetup(eGridStyle.ComboboxEntrys);

        if (setup == null)
            throw new NullReferenceException($@"There is no GridSetup available for type {type.FullName}.");


        // setup.AllowEdit = false;

        // raise event to allow customize setup
        CustomizeSetup?.Invoke(setup, EventArgs.Empty);
               

        this.Setup(setup);
    }

    /// <summary>
    /// event raised at setup (GridSetup)
    /// 
    /// Can be used to modify the setup 
    /// because sender is the AFGridSetup object.
    /// </summary>
    public event EventHandler<EventArgs>? CustomizeSetup;
}

