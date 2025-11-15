using System.Runtime.Versioning;
using Microsoft.Win32;

namespace AF.CORE;

/// <summary>
/// Werkzeuge für die Systemregistrierung
/// </summary>
[Localizable(false)]
[SupportedOSPlatform("windows")]
public static class RegistryTools
{
    /// <summary>
    /// Aktuelle Anwendung als AutoRun registrieren
    /// </summary>
    /// <param name="name">Name, unter dem die Anwendung registriert werden soll</param>.
    /// <param name="application">Anwendung, die registriert werden soll (Beispiel: new FileInfo(Assembly.GetExecutingAssembly().Location)</param>
    /// <param name="ex">Fehler, der bei der Eingabe aufgetreten ist, falls vorhanden</param>.
    /// <returns>true wenn erfolgreich, sonst fals (Grund siehe ex)</returns>
    public static bool RegisterAutoRun(string name, FileInfo application, out Exception? ex)
    {
        const string urikey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        string autoRunLocation = application.FullName;
        ex = null;

        bool ret = true;

        try
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(urikey, RegistryKeyPermissionCheck.ReadWriteSubTree);
            if (key == null)
            {
                // create new if not existing
                using RegistryKey? newkey = Registry.CurrentUser.CreateSubKey(urikey);
                newkey?.SetValue(name, autoRunLocation);
            }
            else
                key.SetValue(name, autoRunLocation);
        }
        catch (Exception err)
        {
            ex = err;
            ret = false;
        }

        return ret;
    }

    /// <summary>
    /// AutoRun-Eintrag entfernen
    /// </summary>
    /// <param name="name">Name, unter dem die Anwendung eingegeben wird</param>.
    /// <param name="ex">Fehler, der bei der Eingabe aufgetreten sein kann</param>.
    /// <returns>true wenn erfolgreich, sonst wenn (Grund siehe ex)</returns>
    public static bool UnRegisterAutoRun(string name, out Exception? ex)
    {
        const string urikey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        ex = null;

        bool ret = true;

        try
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(urikey, RegistryKeyPermissionCheck.ReadWriteSubTree);
            if (key != null && key.GetValue(name) != null)
                key.DeleteValue(name);
        }
        catch (Exception err)
        {
            ex = err;
            ret = false;
        }

        return ret;
    }


    /// <summary>
    /// Registrieren Sie eine Anwendung als Protokollhandler (zum Beispiel myapp://somedate).
    ///
    /// Der Aufruf eines solchen Links öffnet die Anwendung und übergibt den vollständigen Link als Parameter.
    /// Auf diese Weise kann der Benutzer in allen anderen Anwendungen einen Link auf die Anwendung erstellen
    /// und an allen anderen Stellen (wie z.B. Webseiten) einen Link erstellen, der bei Klick auf die registrierte
    /// Anwendung öffnet.
    /// </summary>
    /// <param name="name">Name des Protokolls (wie http oder https - aber natürlich nicht das)</param>
    /// <param name="execute">Auszuführende Anwendung</param>
    /// <param name="ex">Fehler, der bei der Eingabe aufgetreten sein kann</param>.
    /// <returns>true wenn erfolgreich, sonst wenn (Grund siehe ex)</returns>
    public static bool RegisterProtocolHandler(string name, FileInfo execute, out Exception? ex) 
    {
        string uriKey = @"SOFTWARE\Classes\"+ name;
        string starterLocation = execute.FullName;
        bool ret = true;

        ex = null;

        try
        {
            using RegistryKey? key =
            Registry.CurrentUser.OpenSubKey(uriKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
        if (key == null)
        {
            // neu anlegen, wenn nicht vorhanden...
            using (RegistryKey? newKey = Registry.CurrentUser.CreateSubKey(uriKey))
            {
                newKey?.SetValue("", $"URL:{name} protocol");
                newKey?.SetValue("URL Protocol", "");

                using (var defaultIcon = newKey?.CreateSubKey("DefaultIcon"))
                    defaultIcon?.SetValue("", starterLocation + ",1");

                using (var commandKey = newKey?.CreateSubKey(@"shell\open\command"))
                    commandKey?.SetValue("", $"\"{starterLocation}\" \"%1\"");
            }
        }
        else
        {
            // wenn vorhanden aktualisieren...
            key.SetValue("", $"URL:{name} protocol");
            key.SetValue("URL Protocol", "");

            using (var defaultIcon = key.OpenSubKey("DefaultIcon", RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                if (defaultIcon != null)
                    defaultIcon.SetValue("", starterLocation + ",1");
                else
                {
                    using (var newdefaultIcon = key.CreateSubKey(@"DefaultIcon"))
                        newdefaultIcon?.SetValue("", starterLocation + @",1");
                }
            }

            using var commandKey = key.OpenSubKey(@"shell\open\command", RegistryKeyPermissionCheck.ReadWriteSubTree);
            if (commandKey != null)
                commandKey.SetValue("", $"\"{starterLocation}\" \"%1\"");
            else
            {
                using var newcommandKey = key.CreateSubKey(@"shell\open\command");
                newcommandKey?.SetValue("", $"\"{starterLocation}\" \"%1\"");
            }
        }
        }
        catch (Exception err)
        {
            ex = err;
            ret = false;
        }

        return ret;
    }
}

