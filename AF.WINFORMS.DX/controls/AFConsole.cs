namespace AF.WINFORMS.DX;

/// <summary>
/// Command, dass in AFConsole ausgeführt werden kann.
/// </summary>
public struct AFConsoleCommand
{
    /// <summary>
    /// Name des Commands (muss eindeutig sein)
    /// </summary>
    public string CommandName { get; set; }
    /// <summary>
    /// auszuführende Methode
    /// </summary>
    public Action<Dictionary<string, string>> CommandAction { get; set; }

    /// <summary>
    /// Kurzbeschreibung des Commands
    /// </summary>
    public string CommandDescription { get; set; }

    /// <summary>
    /// Langbeschreibung des Commands
    /// </summary>
    public string CommandLongDescription { get; set; }

    /// <summary>
    /// Befehl wird im UI-Thread ausgeführt (kein separater Task)
    /// </summary>
    public bool ExecuteInUI { get; set; }

}

/// <summary>
/// Textbox, die sich wie eine Konsole verhält.
/// 
/// Der Benutzer kann an einem Prompt Befehle mit Parametern 
/// eingeben, die dann ausgeführt werden. Die Ausgabe der Befehle erfolgt in der Konsole.
/// </summary>
public class AFConsole : AFEditMultiline
{
    private bool _locked;
    private readonly Dictionary<string, AFConsoleCommand> _commands = [];
    
    /// <summary>
    /// Ein neues Command registrieren
    /// </summary>
    /// <param name="command">zu registrierendes Command</param>
    public void RegisterCommand(AFConsoleCommand command)
    {
        _commands.Add(command.CommandName.ToLower(), command);
    }

    /// <summary>
    /// Standardcommands registrieren
    /// </summary>
    public void RegisterDefaultCommands()
    {
        RegisterCommand(new AFConsoleCommand { CommandName = "help", CommandAction = showHelp, CommandDescription = "Befehlsliste anzeigen", CommandLongDescription = "Zeigt eine Liste aller verfügbaren Befehle an. Sie können help -c befehlsname eingeben um mehr Informationen zu einem bestimmten Befehl zu erhalten.", ExecuteInUI = true });
        RegisterCommand(new AFConsoleCommand { CommandName = "cls", CommandAction = clearConsole, CommandDescription = "Ausgabe löschen", CommandLongDescription = "Löscht die Textausgabe der Konsole komplett.", ExecuteInUI = true });
    }

    private void clearConsole(Dictionary<string, string> obj)
    {
        ClearConsole();
    }

    private void showHelp(Dictionary<string, string> args)
    {
        if (args.Count < 1)
        {
            foreach (var item in _commands)
                WriteLine($"{item.Key} - {item.Value.CommandDescription}");

            WriteLine("Geben Sie help -b befehlsname ein um mehr Informationen zu einem Befehl zu erhalten.");
        }
        else
        {
            if (args.ContainsKey("b") == false)
            {
                WriteLine("Fehler: Parameter 'b' fehlt.");
                return;
            }

            string name = args["b"].ToLower();

            if (string.IsNullOrWhiteSpace(name))
            {
                WriteLine("Fehler: Name des Befehls nicht angegeben (help -b befehlsname).");
                return;
            }

            if (_commands.ContainsKey(name) == false)
            {
                WriteLine($"Befehl '{name}' nicht gefunden.");
                return;
            }

            WriteLine(_commands[name].CommandLongDescription);
        }
    }

    /// <summary>
    /// Console sperren
    /// </summary>
    public void Lock()
    {
        _locked = true;
    }

    /// <summary>
    /// Console entsperren
    /// </summary>
    public void UnLock()
    {
        _locked = false;

        Text += "\r\n" + Prompt;
        SelectionStart = Text.Length;
        SelectionLength = 0;
        ScrollToCaret();
    }

    /// <summary>
    /// Prompt, der am Anfang der Eingabezeile angezeigt werden soll.
    /// </summary>
    private string Prompt { get; set; } = "> ";

    /// <summary>
    /// Einleitung, die als erste Zeile in einer leeren Konsole angezeigt wird.
    /// </summary>
    private string Intro { get; set; } = "AF Konsole - help = Liste der verfügbaren Befehle anzeigen.";

    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (UI.DesignMode) return;

        ClearConsole();
        UnLock();
    }


    /// <summary>
    /// Inhalt der Konsole löschen, Intro und Prompt anzeigen
    /// </summary>
    public void ClearConsole()
    {
        Text = Intro;
    }

    


    /// <summary>
    /// Eine Zeile schreiben
    /// </summary>
    /// <param name="line">auszugebender Text</param>
    public void WriteLine(string line)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(() =>
            {
                writeLine(line);
            }));
        }
        else
            writeLine(line);
    }

    private void writeLine(string line)
    {
        Win32Invokes.LockWindowUpdate(Handle);

        if (Text.Length > 10000)
            Text = Text.Right(8000) +"\r\n" + line;
        else
            Text += "\r\n" + line;

        SelectionStart = Text.Length;
        SelectionLength = 0;
        ScrollToCaret();
        Win32Invokes.LockWindowUpdate(IntPtr.Zero);
    }

    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (UI.DesignMode) { base.OnKeyDown(e); return; }

        if (_locked)
        {
            e.SuppressKeyPress = true;
            base.OnKeyDown(e);
            return;
        }

        int linepos = GetLineFromCharIndex(SelectionStart);
        int columnpos = SelectionStart - GetFirstCharIndexOfCurrentLine() + 1;
        
        if (SelectionStart < Text.Length || SelectionLength > 0)
        {
            if (linepos != (Lines.Length - 1))
            {
                // wir stehen nicht am Ende des Textes, also keine Eingaben akzeptieren ausser einiger Tasten
                if (!(e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right))
                    e.SuppressKeyPress = true;
            }
        }

        if (e.SuppressKeyPress == false)
        {
            if (columnpos <= Prompt.Length)
            {
                // wir stehen NICHT hinter dem Cursor, also keine Eingaben akzeptieren ausser einiger Tasten
                if (!(e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.End))
                    e.SuppressKeyPress = true;
            }
            else
            {
                // Befehl ausführen
                if (e.KeyCode == Keys.Return) 
                {
                    e.Handled = true;

                    if (SelectionStart == Text.Length && SelectionLength == 0)
                    {
                        string line = Lines.Last().Substring(Prompt.Length);
                        // _lastline = line;
                        _parseLine(line);
                    }

                    e.SuppressKeyPress = true;
                }

                // löschen des Zeichens vor dem Cursor verhindern, wenn das der Prompt ist
                if (e.KeyCode == Keys.Back)
                {
                    if (!(linepos == (Lines.Length - 1) && columnpos > (Prompt.Length + 1)))
                        e.SuppressKeyPress = true;
                }
            }
        }

        base.OnKeyDown(e);
    }

    private void _parseLine(string line)
    {
        var parameters = line.SplitCommandLine(out string cmd, null);
        Dictionary<string, string> commandargs;

        try
        {
            commandargs = parameters.Length < 1 ? [] : CommandLineParser.Parse(parameters);
        }
        catch (Exception ex)
        {
            WriteLine($"syntax error ({ex.Message})");
            UnLock();
            return;
        }

        if (_commands.ContainsKey(cmd.ToLower()) == false)
        {
            WriteLine($"Command '{cmd}' not found.");
            UnLock();
            return;
        }
      
        execCommand(_commands[cmd.ToLower()], commandargs);
    }

    private void execCommand(AFConsoleCommand command, Dictionary<string, string> commandargs)
    {
        Lock();

        if (command.ExecuteInUI)
        {
            command.CommandAction.Invoke(commandargs);
            UnLock();
            return;
        }

        Task task = new(() =>
        {
            try
            {
                command.CommandAction.Invoke(commandargs);
            }
            catch (Exception ex)
            {
                WriteLine("FEHLER BEI DER AUSFÜHRUNG!");
                WriteLine(ex.Message);
                WriteLine("Stacktrace:");
                WriteLine(ex.StackTrace ?? "");
                
                if (ex.InnerException != null)
                {
                    WriteLine("InnerException:");
                    WriteLine(ex.InnerException.Message);
                    WriteLine("Stacktrace:");
                    WriteLine(ex.InnerException.StackTrace ?? "");
                }
            }

            BeginInvoke(UnLock);
        });

        task.Start();
    }
}

