using System.Runtime.InteropServices;

namespace AF.CORE;

/// <summary>
/// Klasse zur Anzeige eines Menüsystems in der Konsole.
/// </summary>
public static class ConsoleMenu
{
    /// <summary>
    /// Hauptmenü
    /// </summary>
    public static Menu MainMenu { get; set; } = new();

    /// <summary>
    /// zeige das Menü
    /// </summary>
    public static void Start(ConsoleColor backcolor = ConsoleColor.Black, ConsoleColor forecolor = ConsoleColor.Gray)
    {
        Console.BackgroundColor = backcolor;
        Console.ForegroundColor = forecolor;

        showMenu(MainMenu);
    }

    static void showMenu(Menu menu)
    {
        BeforeOpenMenu?.Invoke(menu, EventArgs.Empty);

        while (true)
        {
            string caption = menu.Caption;

            if (CustomCaption != null)
                caption = CustomCaption.Invoke(menu);

            ShowCaption(caption);

            foreach (var item in menu.Items)
                Console.WriteLine(@"  " + item.Key.ToString().PadRight(6, ' ') + item.Caption);

            Console.WriteLine();

            Console.WriteLine(menu != MainMenu ? CoreStrings.CONMENU_RETURNTOPREVIOUS : CoreStrings.CONMENU_CLOSEAPP);

            Console.WriteLine();

            var key = Console.ReadKey().KeyChar;

            if (key == 'x')
                return;
#if NET481_OR_GREATER
            foreach (var item in menu.Items)
#else
            foreach (var item in CollectionsMarshal.AsSpan(menu.Items))
#endif
            {
                if (item.Key != key) continue;
                
                if (item.Action != null)
                    item.Action.Invoke();
                else if (item.SubMenu != null)
                    showMenu(item.SubMenu);
            }
        }
    }

    /// <summary>
    /// Ereignis, das ausgelöst wird, bevor ein Menü geöffnet wird, 
    /// Absender ist das zu öffnende Menü
    /// </summary>
    public static event EventHandler? BeforeOpenMenu;


    /// <summary>
    /// Methode, die die Überschrift eines Menüs zurückgibt.
    /// </summary>
    public static Func<Menu, string>? CustomCaption { get; set; }

    /// <summary>
    /// zurück zum Menü
    /// </summary>
    public static void ReturnToMenu()
    {
        Console.WriteLine();
        Console.WriteLine(CoreStrings.CONMENU_RETURNTOMENU);
        Console.ReadKey();
    }

    /// <summary>
    /// Kopfzeile anzeigen
    /// </summary>
    /// <param name="caption">Überschrift</param>
    public static void ShowCaption(string caption)
    {
        Console.Clear();
        Console.WriteLine(@"-".PadRight(caption.Length + 4, '-'));
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($@"  {caption}");
        Console.ForegroundColor = color;
        Console.WriteLine(@"-".PadRight(caption.Length + 4, '-'));
        Console.WriteLine();
    }
}

/// <summary>
/// Menü für Konsolenanwendungen
/// </summary>    
public class Menu
{
    /// <summary>
    /// Überschrift
    /// </summary>
    public string Caption { get; set; } = @"Menu";

    /// <summary>
    /// Menüeinträge
    /// </summary>
    public List<MenuItem> Items { get; } = [];

}

/// <summary>
/// Eintrag innerhalb des Menüs
/// </summary>
public class MenuItem
{
    /// <summary>
    /// Text anzeigen
    /// </summary>
    public string Caption { get; set; } = @"caption";

    /// <summary>
    /// Schlüssel/Taste zur Ausführung dieses Eintrags
    /// </summary>
    public char Key { get; set; } = '0';

    /// <summary>
    /// Untermenü
    /// </summary>
    public Menu? SubMenu { get; set; }

    /// <summary>
    /// Aktion, die ausgeführt wird, wenn die Taste gedrückt wird
    /// </summary>
    public Action? Action { get; set; }
}

