using DevExpress.Utils;
using Timer = System.Windows.Forms.Timer;

namespace AF.WINFORMS.DX;

/// <summary>
/// Einzeilige Eingabe für Passwörter mit 
/// Anzeigemöglichkeit und Kopierfunktion.
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
[DesignerCategory("Code")]
public class AFEditPassword : AFEditButtons
{
    private Timer? timer = null;

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFEditPassword()
    {
        Properties.UseSystemPasswordChar = true;
        Properties.Buttons.Clear();
        this.AddButton(name: "pshShow", image: Symbol.Search, tooltip: UI.GetSuperTip("ANZEIGEN", "Zeigt das aktuelle Kennwort für 5 Sekunden an."));
        this.AddButton(name: "pshCopy", image: Symbol.Copy, tooltip: UI.GetSuperTip("KOPIEREN", "Kopiert das aktuelle Kennwort in die Zwischenablage."));

        this.ButtonClick += (_, a) =>
        {
            if (a.Button.Tag is string nameShow && nameShow == "pshShow" && timer?.Enabled == false)
            {
                Properties.UseSystemPasswordChar = false;
                timer.Start();
            }
            
            if (a.Button.Tag is string nameCopy && nameCopy == "pshCopy")
            {
                Clipboard.Clear();
                Clipboard.SetText(Text);
                if (FindForm() is ICommandResultDisplay disp)
                    disp.HandleResult(MVC.CommandResult.Success("Kennwort wurde in Zwischenablage übertragen."));
            }
        };
    }

    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (UI.DesignMode) return;

        timer = new() { Interval = 5000, Enabled = false };
        timer.Tick += (_, _) =>
        {
            timer.Stop();
            Properties.UseSystemPasswordChar = true;
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected override void OnHandleDestroyed(EventArgs e)
    {
        base.OnHandleDestroyed(e);

        if (UI.DesignMode) return;

        timer?.Stop();
        timer?.Dispose();
    }
}