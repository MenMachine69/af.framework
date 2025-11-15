namespace AF.CORE;

/// <summary>
/// Basisklasse für alle Konsolenanwendungen. Diese Klasse kann nicht direkt verwendet werden.
/// 
/// Bietet Unterstützung für das Anzeigen von Meldungen (IMessageService)
/// </summary>
public abstract class AFConsoleApp : AFApp
{
    /// <summary>
    /// Versteckter Konstruktor
    /// 
    /// Erzeugt ein Anwendungsobjekt und registriert es bei AF. Auf das Anwendungsobjekt kann dann jederzeit über AF.App zugegriffen werden.    
    /// </summary>
    /// <param name="setup">Konfiguration der App.</param>
    protected AFConsoleApp(ConsoleAppSetup setup)
        : base(setup)
    {
        

    }

    /// <inheritdoc />
    public override void ShowMessage(MessageArguments args)
    {
        string msgType = args.Type switch
        {
            eNotificationType.Error => CoreStrings.ERROR.ToUpper(),
            eNotificationType.Warning => CoreStrings.WARNING.ToUpper(),
            eNotificationType.Information => CoreStrings.INFORMATION.ToUpper(),
            eNotificationType.None => "",
            _ => CoreStrings.UNKNOWN.ToUpper()
        };

        Console.WriteLine($@"{msgType}: {args.Message}");

        if (args.TimeOut > 0)
        {
            Console.WriteLine(CoreStrings.CONSOLE_PRESS_KEY);
            Thread.Sleep(args.TimeOut * 1000);
        }
    }

    /// <inheritdoc />
    public override void HandleResult(CommandResult result)
    {
        string msgType = result.Result switch
        {
            eNotificationType.Error => CoreStrings.ERROR.ToUpper(),
            eNotificationType.Warning => CoreStrings.WARNING.ToUpper(),
            eNotificationType.Information => CoreStrings.INFORMATION.ToUpper(),
            eNotificationType.None => "",
            _ => CoreStrings.UNKNOWN.ToUpper()
        };

        Console.WriteLine($@"{msgType}: {result.ResultMessage}");
    }


    /// <inheritdoc />
    public override eMessageBoxResult ShowMsgBox(object? owner, MessageBoxArguments args)
    {
        Console.WriteLine(args.Caption);
        Console.WriteLine(args.Message);

        if (args.MoreInfo.IsNotEmpty())
            Console.WriteLine(args.MoreInfo);

        if (args.Buttons == eMessageBoxButton.OK)
        {
            Console.WriteLine(CoreStrings.CONSOLE_PRESS_KEY);
            Console.ReadKey();
            return eMessageBoxResult.OK;
        }
        
        if (args.Buttons == eMessageBoxButton.OKCancel)
        {
            while (true)
            {
                Console.WriteLine(CoreStrings.CONSOLE_OK_CANCEL);
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.O)
                    return eMessageBoxResult.OK;

                if (key.Key == ConsoleKey.C || key.Key == ConsoleKey.A)
                    return eMessageBoxResult.Cancel;
            }

        }

        if (args.Buttons == eMessageBoxButton.RetryCancel)
        {
            while (true)
            {
                Console.WriteLine(CoreStrings.CONSOLE_RETRY_CANCEL);
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.R || key.Key == ConsoleKey.W)
                    return eMessageBoxResult.Retry;

                if (key.Key == ConsoleKey.C || key.Key == ConsoleKey.A)
                    return eMessageBoxResult.Cancel;
            }
        }

        if (args.Buttons == eMessageBoxButton.YesNoCancel)
        {
            while (true)
            {
                Console.WriteLine(CoreStrings.CONSOLE_YES_NO_CANCEL);
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Y || key.Key == ConsoleKey.J)
                    return eMessageBoxResult.Yes;

                if (key.Key == ConsoleKey.N)
                    return eMessageBoxResult.No;

                if (key.Key == ConsoleKey.C || key.Key == ConsoleKey.A)
                    return eMessageBoxResult.Cancel;
            }
        }

        if (args.Buttons == eMessageBoxButton.YesNo)
        {
            while (true)
            {
                Console.WriteLine(CoreStrings.CONSOLE_YES_NO);
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Y || key.Key == ConsoleKey.J)
                    return eMessageBoxResult.Yes;

                if (key.Key == ConsoleKey.N)
                    return eMessageBoxResult.No;
            }
        }

        if (args.Buttons == eMessageBoxButton.AbortRetryIgnore)
        {
            while (true)
            {
                Console.WriteLine(CoreStrings.CONSOLE_ABORT_RETRY_IGNORE);
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.A)
                    return eMessageBoxResult.Abort;

                if (key.Key == ConsoleKey.R || key.Key == ConsoleKey.W)
                    return eMessageBoxResult.Retry;

                if (key.Key == ConsoleKey.I)
                    return eMessageBoxResult.Ignore;
            }
        }

        throw new ArgumentException(CoreStrings.ERROR_UNKNOWNBUTTONS);
    }
}