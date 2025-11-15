using DevExpress.Utils;

namespace AF.WINFORMS.DX;

/// <summary>
/// BeakForm: Form in Form einer Sprechblase
/// </summary>
[ToolboxItem(false)]
[SupportedOSPlatform("windows")]
public class FormBeak : FlyoutPanel
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (UI.DesignMode) return;

        OptionsBeakPanel.CloseOnOuterClick = true;
        OptionsBeakPanel.BeakLocation = BeakPanelBeakLocation.Top;

    }

    /// <summary>
    /// Anzeigen
    /// </summary>
    /// <param name="owner">Übergeordnetes Fenster</param>
    /// <param name="content">Inhalt</param>
    /// <param name="parent">Control an dem die Sprechblase ausgerichtet wird</param>
    /// <param name="alignment">Anordnung der Sprechblase zum übergeordneten Element</param>
    public void Show(Form owner, Control content, Control parent, BeakPanelBeakLocation alignment = BeakPanelBeakLocation.Top)
    {
        Size = new Size(content.Width , content.Height);
        Controls.Clear();
        content.Dock = DockStyle.Fill;
        Controls.Add(content);
        ParentForm = owner;
        OwnerControl = parent;
        ShowBeakForm();
    }
}