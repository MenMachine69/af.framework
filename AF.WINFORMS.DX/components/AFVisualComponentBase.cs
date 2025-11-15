using System.ComponentModel.Design;

namespace AF.WINFORMS.DX;

/// <summary>
/// Base class for components that require access to the control/window 
/// in which they are contained.
/// </summary>

[DesignerCategory("Code")]
public class AFVisualComponentBase : Component
{
    /// <summary>
    /// Container control in which the component is contained.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ContainerControl? ContainerControl { get; set; }

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
}

