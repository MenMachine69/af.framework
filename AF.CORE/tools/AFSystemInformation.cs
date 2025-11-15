using System.Diagnostics;
using System.Management;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace AF.CORE;

/// <summary>
/// Statische Klasse für den Zugriff auf Systeminformationen
/// </summary>
public static class AFSystemInformation
{
    private static MachineInformation? _Machine;
    private static UserInformation? _User;

    /// <summary>
    /// Informationen zum PC, auf dem die Anwendung läuft
    /// </summary>
    public static MachineInformation Machine
    {
        get
        {
            if (_Machine == null) _Machine = new();
            return _Machine;
        }
    }

    /// <summary>
    /// Informationen zum angemeldeten Benutzer auf dem PC.
    /// </summary>
    public static UserInformation User
    {
        get
        {
            if (_User == null) _User = new();
            return _User;
        }
    }

    /// <summary>
    /// Liste der Assemblies
    /// </summary>
    public static List<AssemblyInformation> Assemblies
    {
        get
        {
            List<AssemblyInformation> assemblies = [];

            try
            {
                var source = AppDomain.CurrentDomain.GetAssemblies();

                foreach (Assembly assemblie in source.Where(a => a.IsDynamic == false).OrderBy(a => a.GetName().FullName.ToUpper()))
                    assemblies.Add(new() { Name = assemblie.GetName().FullName, IsDynamic = assemblie.IsDynamic, FullPath = assemblie.Location });
            }
            catch
            {
                // nix unternehmen...
            }

            return assemblies;
        }
    }


    /// <summary>
    /// Informationen zum Computer, auf dem die Anwendung gerade ausgeführt wird.
    /// </summary>
    public sealed class MachineInformation
    {
        private readonly string _cpuid = string.Empty;
        private readonly string _macid = string.Empty;
        private readonly string _mbid = @"unknownmboard";
        private readonly string _systemserial;
        private readonly SysInfoCPU _sysinfocpu;
        private long _totalMemory;

        /// <summary>
        /// Constructor
        /// </summary>
        public MachineInformation()
        {
            _systemserial = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.System)).VolumeSerial();

            using ManagementClass mcCPU = new("Win32_Processor");
            using var mbCPU = mcCPU.GetInstances();
            foreach (var mo in mbCPU)
            {
                if (mo == null) continue;

#if NET48_OR_GREATER 
                _cpuid = mo.Properties["ProcessorId"].Value.ToString();
#else
                _cpuid = mo.Properties["ProcessorId"].Value.ToString()!;
#endif
                break;
            }

            using ManagementClass mcBoard = new("Win32_BaseBoard");
            using var mbBoard = mcBoard.GetInstances();
            foreach (var mo in mbBoard)
            {
                if (mo == null) continue;

#if NET48_OR_GREATER
                _mbid = mo.Properties["SerialNumber"].Value.ToString();
#else
                _mbid = mo.Properties["SerialNumber"].Value.ToString()!;
#endif

                break;
            }

            using ManagementClass mcMemory = new("Win32_ComputerSystem");
            using var mbMemory = mcMemory.GetInstances();
            foreach (var mo in mbMemory)
            {
                if (mo == null) continue;
                
                _totalMemory = Convert.ToInt64(mo.Properties["TotalPhysicalMemory"].Value);
                break;
            }


            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var ni in interfaces)
            {
                _macid = ni.GetPhysicalAddress().ToString();
                break;
            }

            _sysinfocpu = new();
        }

        /// <summary>
        /// Der Name des Computers.
        /// </summary>
        public string Name => Environment.MachineName;

        /// <summary>
        /// Verzeichnis Anwendungsdaten für alle Benutzer des Computers (z.B.: C:\ProgramData\).
        /// ACHTUNG! Normale Anwender haben auf dieses Verzeichnis nur Leserechte.
        /// </summary>
        public string PathAppData => Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

        /// <summary>
        /// Verzeichnis Public Documents (z.B.: C:\Users\Public\Documents\).
        /// </summary>
        public string PathPublicDocuments => Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);

        /// <summary>
        /// Verzeichnis Public (z.B.: C:\Users\Public\).
        /// 
        /// Unter Betriebssystem >= Vista wird die Umgebungsvariable 'public' unter 
        /// älteren Betriebssystem die Variable 'allusersprofile' verwendet.
        /// </summary>
        public string PathPublic => Environment.GetEnvironmentVariable(Environment.OSVersion.Version.Major >= 6 ? "public" : "allusersprofile") ?? "C:\\Users\\Public";

        /// <summary>
        /// Verzeichnis 'SYSTEM' (system32 - z.B.: C:\Windows\System32\)
        /// </summary>
        public string PathSystem => Environment.GetFolderPath(Environment.SpecialFolder.System);

        /// <summary>
        /// Verzeichnis 'PROGRAMME' (z.B.: C:\Program Files\)-
        /// </summary>
        public string PathProgramFiles => Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

        /// <summary>
        /// ID des Prozessors des PCs
        /// </summary>
        public string IDCpu => _cpuid;

        /// <summary>
        /// ID des Motherboards des PCs
        /// </summary>
        public string IDMotherboard => _mbid;

        /// <summary>
        /// MAC-Adresse der ersten Netzwerkkarte/der ersten Netzwerkschnittstelle des PCs
        /// </summary>
        public string IDMac => _macid;

        /// <summary>
        /// Volume-ID des Systemlaufwerks
        /// 
        /// Seriennummer des Laufwerkes, auf dem sich das Betriebssystem befindet.
        /// </summary>
        public string IDSystemDrive => _systemserial;
        
        /// <summary>
        /// Arbeitsspeicher (RAM) des PCs
        /// </summary>
        public long TotalMemory => _totalMemory;

        /// <summary>
        /// Betriebssystem-Version
        /// </summary>
        public string OSVersion => Environment.OSVersion.VersionString;

        /// <summary>
        /// Gibt an, ob das System auf einem Server läuft
        /// </summary>
        public bool IsWindowsServer => IsOS(OS_ANYSERVER);


        const int OS_ANYSERVER = 29;

        [DllImport("shlwapi.dll", SetLastError = true, EntryPoint = "#437")]
        private static extern bool IsOS(int os);

        /// <summary>
        /// Informationen zur CPU
        /// </summary>
        public SysInfoCPU CPU => _sysinfocpu;

        /// <summary>
        /// Informationen zu den laufenden Prozessen
        /// </summary>
        public SysInfoProcesses Prozesse => new();
    }

    /// <summary>
    /// Informationen zum Speicher...
    /// </summary>
    public sealed class SysInfoProcesses
    {
        /// <summary>
        /// Liste der laufenden Prozesse
        /// </summary>
        public ProzessInfo[] Prozesse { get; private set; }
        
        /// <summary>
        /// Constructor
        /// </summary>
        public SysInfoProcesses()
        {
            Process[] prozesse = Process.GetProcesses();

            Prozesse = new ProzessInfo[prozesse.Length];

            int pos = 0;

            foreach (Process prozess in prozesse)
            {
                Prozesse[pos] = new()
                {
                    Memory = prozess.VirtualMemorySize64,
                    Name = prozess.ProcessName
                };

                ++pos;
            }
        }
    }

    /// <summary>
    /// Informationen zu einem laufenden Prozess
    /// </summary>
    [Serializable]
    public sealed class ProzessInfo
    {
        /// <summary>
        /// CPU-Auslastung
        /// </summary>
        public float CPULoad
        {
            get
            {
                float ret;

                try
                {
                    using (PerformanceCounter cnt = new("Process", "% Processor Time", Name))
                    {
                        cnt.NextValue();
                        Thread.Sleep(1000);
                        ret = cnt.NextValue();
                    }
                }
                catch
                {
                    ret = -1;
                }

                return ret;
            }
        }

        /// <summary>
        /// Speicherverbrauch
        /// </summary>
        public long Memory { get; set; }

        /// <summary>
        /// Name des Prozesses
        /// </summary>
        public string Name { get; internal set; } = "";
    }

    /// <summary>
    /// Informationen zur CPU
    /// </summary>
    public sealed class SysInfoCPU
    {
        /// <summary>
        /// ID der CPU
        /// </summary>
        public string ID { get; init; } = "";

        /// <summary>
        /// Name der CPU
        /// </summary>
        public string Name { get; init; } = "";

        /// <summary>
        /// max. Geschwindigkeit
        /// </summary>
        public int MaxSpeed { get; init; }   

        /// <summary>
        /// aktuelle Geschwindigkeit
        /// </summary>
        public int CurrentSpeed { get; init; }

        /// <summary>
        /// Anzahl der Kerne
        /// </summary>
        public int Cores { get; init; }

        /// <summary>
        /// Anzahl der logischen Prozessoren
        /// </summary>
        public int Processors { get; init; }

        /// <summary>
        /// Aktuelle Auslastung der CPU in %
        /// </summary>
        public int CurrentLoad { get; init; }

        /// <summary>
        /// Constructor
        /// </summary>
        public SysInfoCPU()
        {
            using ManagementClass mc = new("Win32_Processor");
            using ManagementObjectCollection moc = mc.GetInstances();
            foreach (var mo in moc)
            {
#if NET48_OR_GREATER
                ID = mo.Properties["ProcessorId"].Value.ToString();
                Name = mo.Properties["Name"].Value.ToString();
#else
                ID = mo.Properties["ProcessorId"].Value.ToString()!;
                Name = mo.Properties["Name"].Value.ToString()!;
#endif
                MaxSpeed = mo.Properties["MaxClockSpeed"].Value == null ? 0 : Convert.ToInt32(mo.Properties["MaxClockSpeed"].Value);
                CurrentSpeed = mo.Properties["CurrentClockSpeed"].Value == null ? 0 : Convert.ToInt32(mo.Properties["CurrentClockSpeed"].Value);
                Cores = mo.Properties["NumberOfCores"].Value == null ? 0 : Convert.ToInt32(mo.Properties["NumberOfCores"].Value);
                Processors = mo.Properties["NumberOfLogicalProcessors"].Value == null ? 0 : Convert.ToInt32(mo.Properties["NumberOfLogicalProcessors"].Value);
                CurrentLoad = mo.Properties["LoadPercentage"].Value == null ? 0 : Convert.ToInt32(mo.Properties["LoadPercentage"].Value);
                break;
            }
        }
    }

    /// <summary>
    /// Informationen zum momentan angemeldeten Benutzer, der das Programm ausführt.
    /// </summary>
    public sealed class UserInformation
    {
        private List<string> _Groups = [];
        private string _primaryGroup;
        private bool _isLocalAdmin;


        /// <summary>
        /// Constructor
        /// </summary>
        public UserInformation()
        {
            WindowsIdentity ident = WindowsIdentity.GetCurrent();
#if NET48_OR_GREATER
            IdentityReferenceCollection irc = ident.Groups!.Translate(typeof(NTAccount))!;
#else
            IdentityReferenceCollection irc = ident.Groups!.Translate(typeof(NTAccount));
#endif

            _primaryGroup = irc[0].Value;

            foreach (IdentityReference grp in irc)
                _Groups.Add(grp.Value);

            WindowsPrincipal p = new(ident);
            _isLocalAdmin = p.IsInRole(WindowsBuiltInRole.Administrator);

            ident.Dispose();
        }

        /// <summary>
        /// Der Name des angemeldeten Benutzers
        /// 
        /// Der Wert darf nicht NULL oder leer sein!
        /// </summary>
        public string Name => Environment.UserName;

        /// <summary>
        /// Domäne des Benutzers
        /// 
        /// Domäne in der sich das Benutzerkonto des aktuellen Benutzers befindet.
        /// </summary>
        public string Domain => Environment.UserDomainName;

        /// <summary>
        /// Gibt an, ob der User über lokale Adminrechte verfügt.
        /// </summary>
        public bool IsLocalAdmin => _isLocalAdmin;


        /// <summary>
        /// Gibt an, ob der Benutzer angemeldet ist (false, z.B. bei Diensten)
        /// </summary>
        public bool IsInteractive => Environment.UserInteractive;

        /// <summary>
        /// Anwendungsdaten des Benutzers (Roaming, wird in servergespeicherten Profilen abgelegt)
        /// 
        /// Verwenden Sie diesen Pfad um Daten abzulegen, die Bestandteil des auf dem Server gespeicherten Benutzerprofils
        /// sein sollen
        /// 
        /// Unter Windows 7 z.B.: C:\Users\MuellerH\AppData\Roaming\
        /// </summary>
        public string PathAppData => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Trim();

        /// <summary>
        /// 'Eigene Bilder' des Benutzers (Roaming, wird in servergespeicherten Profilen abgelegt)
        /// </summary>
        public string PathImages => Environment.GetFolderPath(Environment.SpecialFolder.MyPictures).Trim();

        /// <summary>
        /// 'Eigene Dokumente' des Benutzers (Roaming, wird in servergespeicherten Profilen abgelegt)
        /// </summary>
        public string PathDocuments => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Trim();

        /// <summary>
        /// 'Eigene Musik' des Benutzers (Roaming, wird in servergespeicherten Profilen abgelegt)
        /// </summary>
        public string PathMusic => Environment.GetFolderPath(Environment.SpecialFolder.MyMusic).Trim();

        /// <summary>
        /// Anwendungsdaten des Benutzers (Local, wird NICHT in servergespeicherten Profilen abgelegt)
        /// 
        /// Verwenden Sie diesen Pfad um Daten abzulegen, die nicht Bestandteil des auf dem Server gespeicherten Benutzerprofils
        /// sein sollen
        /// 
        /// Unter Windows 7 z.B.: C:\Users\MuellerH\AppData\Local\
        /// </summary>
        public string PathLocalAppData => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Trim();

        /// <summary>
        /// primäre Gruppe des Benutzers (siehe ActiveDirectory -&gt; PrimaryGroup)
        /// </summary>
        public string PrimaryGroup => _primaryGroup;

        /// <summary>
        /// Liste der Gruppen, denen der Benutzer angehört.
        /// 
        /// Diese Liste enthält lokale und ActiveDirectory-Gruppen. 
        /// </summary>
        public List<string> Groups => _Groups;
    }

    /// <summary>
    /// Informationen zu einem Bildschirm
    /// </summary>
    [Serializable]
    public class AssemblyInformation
    {
        /// <summary>
        /// Auflösung Horizontal
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Gibt an, ob die Assembly aus dem GAC geladen wurde
        /// </summary>
        public bool IsGAC { get; set; }

        /// <summary>
        /// Gibt an, ob die Assembly dynamisch zur Laufzeit erzeugt wurde (Scripte etc.)
        /// </summary>
        public bool IsDynamic { get; set; }

        /// <summary>
        /// Vollständiger Pfad der Assembly
        /// </summary>
        public string FullPath { get; set; } = "";
    }

}

