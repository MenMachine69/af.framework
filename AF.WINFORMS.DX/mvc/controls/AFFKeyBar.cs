
using DevExpress.Utils;
using DevExpress.Utils.Layout;
using DevExpress.XtraEditors;

namespace AF.MVC;

/// <summary>
/// Leiste zur Anzeige der F-Tasten Belegung
/// </summary>
[ToolboxItem(false)]
[DesignerCategory("Code")]
public class AFFKeyBar : AFUserControl
{
    private readonly IViewPage page;
    private KeyboardHookListener? _KeyboardHookManager;
    private readonly AFTablePanel table;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFFKeyBar(IViewPage parentpage)
    {
        page = parentpage;

        table = new() { Dock = DockStyle.Fill, UseSkinIndents = true };

        table.BeginLayout();

        var commands = page.CurrentController!.GetCommands();

        for (int i = 1; i <= 12; ++i)
        {
            if (i == 10)
            {
                var btn = getButton(10, null);
                btn.Dock = DockStyle.Fill;
                btn.Text = "SCHLIESSEN";
                btn.Tag = (() => page.HideFKeyBar());

                table.Add(btn, 1, i);

            }
            else
                table.Add(getButton(i, commands.FirstOrDefault(c => c.FunctionKey == i)), 1, i).Dock = DockStyle.Fill;
        }
        
        
        table.SetColumn(1, TablePanelEntityStyle.Relative, 0.0833f);
        table.SetColumn(2, TablePanelEntityStyle.Relative, 0.0833f);
        table.SetColumn(3, TablePanelEntityStyle.Relative, 0.0833f);
        table.SetColumn(4, TablePanelEntityStyle.Relative, 0.0833f);
        table.SetColumn(5, TablePanelEntityStyle.Relative, 0.0833f);
        table.SetColumn(6, TablePanelEntityStyle.Relative, 0.0833f);
        table.SetColumn(7, TablePanelEntityStyle.Relative, 0.0833f);
        table.SetColumn(8, TablePanelEntityStyle.Relative, 0.0833f);
        table.SetColumn(9, TablePanelEntityStyle.Relative, 0.0833f);
        table.SetColumn(10, TablePanelEntityStyle.Relative, 0.0833f);
        table.SetColumn(11, TablePanelEntityStyle.Relative, 0.0833f);
        table.SetColumn(12, TablePanelEntityStyle.Relative, 0.0834f);
        table.SetRow(1, TablePanelEntityStyle.Relative, 1.0f);
        table.EndLayout();

        Controls.Add(table);

    }

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.Control.HandleCreated" /> event.
    /// </summary>
    /// <param name="e">
    /// An <see cref="T:System.EventArgs" /> that contains the event data. 
    /// </param>
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        _KeyboardHookManager = new(new AppHooker())
        {
            Enabled = true
        };
        _KeyboardHookManager.KeyDown += keyboardHookManager_KeyDown;
    }

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.Control.HandleDestroyed" /> event.
    /// </summary>
    /// <param name="e">
    /// An <see cref="T:System.EventArgs" /> that contains the event data. 
    /// </param>
    protected override void OnHandleDestroyed(EventArgs e)
    {
        if (_KeyboardHookManager != null)
            _KeyboardHookManager.KeyDown -= keyboardHookManager_KeyDown;

        base.OnHandleDestroyed(e);
    }

    private void keyboardHookManager_KeyDown(object? sender, KeyEventArgs e)
    {
        AFButton? btn = null;

        if (!Visible) return;
        
        if (e.KeyCode == Keys.F1)
            btn = table.Controls[0] as AFButton;
        else if (e.KeyCode == Keys.F2)
            btn = table.Controls[1] as AFButton;
        else if (e.KeyCode == Keys.F3)
            btn = table.Controls[2] as AFButton;
        else if (e.KeyCode == Keys.F4)
            btn = table.Controls[3] as AFButton;
        else if (e.KeyCode == Keys.F5)
            btn = table.Controls[4] as AFButton;
        else if (e.KeyCode == Keys.F6)
            btn = table.Controls[5] as AFButton;
        else if (e.KeyCode == Keys.F7)
            btn = table.Controls[6] as AFButton;
        else if (e.KeyCode == Keys.F8)
            btn = table.Controls[7] as AFButton;
        else if (e.KeyCode == Keys.F9)
            btn = table.Controls[8] as AFButton;
        else if (e.KeyCode == Keys.F10)
            btn = table.Controls[9] as AFButton;
        else if (e.KeyCode == Keys.F11)
            btn = table.Controls[10] as AFButton;
        else if (e.KeyCode == Keys.F12)
            btn = table.Controls[11] as AFButton;

        if (btn == null || btn.Tag == null) return;
        
        action(btn, EventArgs.Empty);
        e.Handled = true;
    }

    private AFButton getButton(int key, AFCommand? command)
    {
        AFButton btn = new()
        {
            Tag = command,
            Text = command?.Caption.ToUpper().Replace("...", "") ?? "",
            Dock = DockStyle.Fill,
            AllowFocus = false,
            ShowFocusRectangle = DefaultBoolean.False,
            TabStop = false,
            Appearance = { TextOptions = { HAlignment = HorzAlignment.Center, VAlignment = VertAlignment.Center, WordWrap = WordWrap.Wrap }}
        };
        btn.Name = "F" + key;
        btn.CustomDraw += draw;
        btn.Click += action;

        return btn;
    }

    private void action(object? sender, EventArgs e)
    {
        if (sender is not AFButton btn) return;

        if (btn.Tag == null) return;

        if (btn.Tag is AFCommand cmd)
        {
            page.HideFKeyBar();
            page.InvokeCommand(cmd);
        }
        else
            (btn.Tag as Action)?.Invoke();
    }

    private void draw(object? sender, ButtonCustomDrawEventArgs e)
    {
        e.DefaultDraw();

        var format = StringFormat.GenericTypographic;
        format.LineAlignment = StringAlignment.Near;
        format.Alignment = StringAlignment.Near;
        var rect = e.Bounds;
        rect.Inflate(-5, -5);
        e.Cache.DrawString((sender as AFButton)!.Name, SystemFonts.DefaultFont, DXColor.DimGray, rect, format);
    }
}

