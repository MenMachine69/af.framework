using System.Xml.Serialization;

namespace AF.CORE;

/// <summary>
/// Basisklasse der serialisierbaren Einstellungsklassen SettingsApplication und SettingsUser
/// 
/// Bietet grundlegende Funktionalitäten für die Verwaltung von Einstellungen.
/// 
/// Einstellungen können im json oder XML Format gespeichert werden. Das verwendete Format kann von der 
/// Anwendung festgelegt werden, die Voreinstellung ist das json-Format.
/// 
/// Die Einstellungen werden in einer Datei mit dem Namen {Environment.Application.Name}_setting.xml 
/// oder {Environment.Application.Name}_setting.json (je nach gewähltem Format). Die aktuelle Einstellungsdatei 
/// kann per Backup gesichert und per Restore wiederhergestellt werden.
/// 
/// Beim Laden über Load kann ein Pfad übergeben werden, in dem die Einstellungsdatei vorzugsweise gesucht werden soll. 
/// (z.B. im zentralen Anwendungsverzeichnis in einem Netzwerk (Verzeichnis, in dem sich die EXE-Datei befindet)). 
/// Existiert die Einstellungsdatei dort nicht oder wird ein leerer Pfad übergeben, wird die Einstellungsdatei aus 
/// wird der Standardpfad (siehe Pfad) verwendet. Auf diese Weise können z.B.
/// Programmeinstellungen für alle Benutzer zentral von einem Administrator im Netzwerk gespeichert werden und können 
/// durch einen Administrator.
/// 
/// Nach dem Laden wird in der abgeleiteten Klasse automatisch eine Methode AfterLoad aufgerufen. Verwenden Sie diese Methode, um
/// z.B. Standardwerte für bestimmte Einstellungen zu setzen.
/// 
/// Beim Speichern der Einstellungen über Settings.Save(settings) wird vor dem Speichern die Methode BeforeSave aufgerufen.
/// Dort können Sie ggf. weitere Einstellungen festlegen, die gespeichert werden sollen.
/// 
/// <example>
/// <code>
/// [Serializable]
/// public class ApplicationSettings : Settings
/// {
///      private string _mailsender;
///      private string _mailserver;
///      
///      public ApplicationSettings()
///      {
///          Name = "MeineApp";
///          Path = MeineApp.Instance.Name;
///          
///          // setzen der Standardwerte
///          MailServer = "MeinServer";
///          MailSender = "info@ich.de";
///      }
/// 
///      public string MailSender
///      { 
///           get { return _mailsender; }
///           set { _mailsender = value; }
///      }
///      
///      public string MailServer
///      { 
///           get { return _mailserver; }
///           set { _mailserver = value; }
///      }
///      
///     /// <summary>
///     /// Nach dem laden der Einstellungen
///     /// </summary>
///     internal void AfterLoad()
///     {
///         if (MailServer.IsNullOrEMpty())
///             MailServer = "localhost";
///     }
/// }
/// </code>
/// </example>
/// </summary>
[Localizable(false)]
[Serializable]
public abstract class Settings : Base
{
    [NonSerialized] private SettingsFormat _Format = SettingsFormat.XML;
    private string _name = string.Empty;
    private string _path = string.Empty;


    /// <summary>
    /// Constructor
    /// </summary>
    protected Settings() { }

    /// <summary>
    /// Format, in dem die Einstellungen gespeichert werden sollen
    /// </summary>
    [XmlIgnore]
    public SettingsFormat Format
    {
        get => _Format;
        set => Set(ref _Format, value);
    }

    /// <summary>
    /// Name, unter dem die Einstellungen gespeichert werden (z.B. Prototype.Instance.Name)
    /// </summary>
    [XmlIgnore]
    public string Name
    {
        get => _name;
        set => SetNotNullable(nameof(Name), ref _name, value);
    }

    /// <summary>
    /// Verzeichnis, in dem Einstellungen gespeichert werden (z.B. Prototype.Instance.Environment.PathAppData oder Prototype.Instance.Environment.PathUserAppData)
    /// </summary>
    [XmlIgnore]
    public string Path
    {
        get => _path;
        set => SetNotNullable(nameof(Path), ref _path, value);
    }


    /// <summary>
    /// Einstellungen speichern
    /// </summary>
    public void Save()
    {
        CancelEventArgs args = new();
        Saving?.Invoke(this, args);

        if (args.Cancel)
            return;
        
        DirectoryInfo dir = new(Path);
        
        if (!dir.Check(true)) return;

        if (Format == SettingsFormat.XML)
            this.ToXmlFile(new FileInfo(System.IO.Path.Combine(dir.FullName, Name + "_settings.xml")));
        else
            this.ToJsonFile(new FileInfo(System.IO.Path.Combine(dir.FullName, Name + "_settings.json")));
        
        Saved?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Erstellt eine Sicherungskopie der Einstellungsdatei, falls sie existiert.
    /// </summary>
    public void Backup()
    {
        CancelEventArgs args = new();
        Backuping?.Invoke(this, args);

        if (args.Cancel)
            return;

        DirectoryInfo dir = new(Path);

        string file = System.IO.Path.Combine(dir.FullName, Name + (Format == SettingsFormat.XML ? "_settings.xml" : "_settings.json"));

        if (!File.Exists(file)) return;

        if (File.Exists(file + ".backup"))
            File.Delete(file + ".backup");

        File.Copy(file, file + ".backup");
        Backuped?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Stellt eine Einstellungsdatei aus einer Sicherungskopie wieder her, falls sie existiert.
    /// </summary>
    public void Restore()
    {
        CancelEventArgs args = new();
        Restoring?.Invoke(this, args);

        if (args.Cancel)
            return;

        DirectoryInfo dir = new(Path);

        string file = System.IO.Path.Combine(dir.FullName, Name + (Format == SettingsFormat.XML ? "_settings.xml" : "_settings.json"));

        if (!File.Exists(file + ".backup")) return;

        if (File.Exists(file))
            File.Delete(file);

        File.Copy(file + ".backup", file);
        Restored?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Einstellungen aus Datei laden
    /// </summary>
    /// <param name="settings">Prototyp der zu ladenden Einstellungen</param>
    /// <returns>geladene Einstellungen oder Null</returns>
    public static T Load<T>(T settings) where T : Settings, new()
    {
        T? ret = default;
        
        SettingsFormat format = settings.Format;
        string path = settings.Path;
        
        DirectoryInfo dir = new(settings.Path);

        if (dir.Check(true))
        {
            string file = System.IO.Path.Combine(dir.FullName, settings.Name + (settings.Format == SettingsFormat.XML ? @"_settings.xml" : @"_settings.json"));

            int cnt = 1;

            while (cnt <= 2)
            {
                try
                {
                    FileInfo fi = new(file);

                    if (fi.Exists)
                    {
                        ret = settings.Format == SettingsFormat.XML ? Functions.DeserializeXmlFile<T>(fi) : Functions.DeserializeJsonFile<T>(fi);

                        // Create a backup when everything has worked...
                        ret?.Backup();
                    }
                }
                catch 
                {
                    // try a restore from the backup...
                    ++cnt;
                    if (File.Exists(file + ".backup"))
                    {
                        if (File.Exists(file))
                            File.Delete(file);

                        File.Copy(file + ".backup", file);
                        continue;
                    }
                }

                break;
            }
        }

        ret ??= settings;

        ret.Format = format;
        ret.Path = path;
        ret.Loaded?.Invoke(ret, EventArgs.Empty);
        ret.AfterLoad();

        return ret;
    }

    /// <summary>
    /// Methode, die automatisch nach dem Laden der Einstellungen aufgerufen wird.
    /// 
    /// Diese Methode muss/ kann in der abgeleiteten Klasse überschrieben werden.
    /// </summary>
    public virtual void AfterLoad() { }

    /// <summary>
    /// Methode, die automatisch aufgerufen wird, bevor die Einstellungen gespeichert werden.
    /// 
    /// Diese Methode muss/ kann in der abgeleiteten Klasse überschrieben werden.
    /// </summary>
    public virtual void BeforeSave() { }


    #region Events

    /// <summary>
    /// Einstellungen wurden gespeichert
    /// </summary>
    public event EventHandler? Saved;

    /// <summary>
    /// Einstellungen wurden geladen
    /// </summary>
    public event EventHandler? Loaded;

    /// <summary>
    /// Die Einstellungen wurden wiederhergestellt.
    /// </summary>
    public event EventHandler? Restored;

    /// <summary>
    /// Einstellungen wurden gesichert
    /// </summary>
    public event EventHandler? Backuped;

    /// <summary>
    /// Einstellungen werden gespeichert
    /// 
    /// Dieses Ereignis kann durch Übergabe von Cancel=true in den Ereignisargumenten abgebrochen werden.
    /// </summary>
    public event CancelEventHandler? Saving;

    /// <summary>
    /// Einstellungen werden wiederhergestellt
    /// 
    /// Dieses Ereignis kann durch Übergabe von Cancel=true in den Ereignisargumenten abgebrochen werden.
    /// </summary>
    public event CancelEventHandler? Restoring;

    /// <summary>
    /// Einstellungen werden gespeichert
    /// 
    /// Dieses Ereignis kann durch Übergabe von Cancel=true in den Ereignisargumenten abgebrochen werden.
    /// </summary>
    public event CancelEventHandler? Backuping;

    #endregion
}

/// <summary>
/// Format, in dem die Einstellungen gespeichert werden sollen
/// </summary>
public enum SettingsFormat
{
    /// <summary>
    /// Json
    /// </summary>
    Json,

    /// <summary>
    /// XML
    /// </summary>
    XML
}


