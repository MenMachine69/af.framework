namespace AF.CORE;

/// <summary>
/// Schnittstelle für ein ausführbares Skript
/// </summary>
public interface IScript
{
    /// <summary>
    /// Script Quellcode
    /// </summary>
    string SourceCode { get; set; }

    /// <summary>
    /// Im Script verwendete Variablen.
    /// </summary>
    BindingList<Variable> Variablen { get; set; }
}

/// <summary>
/// Einfache Implementierung von IScript
/// </summary>
[Serializable]
public class DefaultScript : IScript
{
    /// <inheritdoc />
    public string SourceCode { get; set; } = string.Empty;

    /// <inheritdoc />
    public BindingList<Variable> Variablen { get; set; } = [];
}

/// <summary>
/// Interface für eine Klasse die in AF.BUSINESS als Controller für Skript verwendet wird.
/// </summary>
public interface IScriptController
{

}

/// <summary>
/// Script-Typen
/// </summary>
public enum eScriptType
{
    /// <summary>
    /// Script wird für Workflow-Filter verwendet
    /// </summary>
    [AFDescription("Workflow-Filter")]
    WorkflowFilter = 0,
    /// <summary>
    /// Script wird als Workflow-Aktion verwendet
    /// </summary>
    [AFDescription("Workflow-Aktion")]
    WorkflowAktion = 1,
    /// <summary>
    /// Script wird zur Berechnung eines Variablenwertes verwendet
    /// </summary>
    [AFDescription("Berechnete Variable")]
    Variable = 2,
    /// <summary>
    /// Script wird für den Listenimport verwendet
    /// </summary>
    [AFDescription("Import von Listen")]
    ListenImport = 3,
    /// <summary>
    /// Script wird als Command im MasterView verwendet (Kontextmenü der Masteransicht)
    /// </summary>
    [AFDescription("Menüpunkt Master (Kontextmenü)")]
    MasterCommand = 4,
    /// <summary>
    /// Script wird als Command im DetailView verwendet (Kontextmenü der Detailansicht)
    /// </summary>
    [AFDescription("Menüpunkt Detail (Kontextmenü)")]
    DetailCommand = 5,
    /// <summary>
    /// Script wird als DetailView-Plugin verwendet (eigene Detailansicht)
    /// </summary>
    [AFDescription("Detailansicht")]
    DetailView = 6,
    /// <summary>
    /// Element das im Dashboard angezeigt werden kann
    /// </summary>
    [AFDescription("Dasboard-Element")]
    DashboardElement = 7,
    /// <summary>
    /// Element wird als Datenquelle verwendet (DataTable)
    /// </summary>
    [AFDescription("Datenquelle (DataTable)")]
    DataSource = 8,
    /// <summary>
    /// Element das als 'Job' regelmässig ausgeführt werden kann
    /// </summary>
    [AFDescription("Job (geplante Aufgabe)")]
    Job = 9,
    /// <summary>
    /// Undefiniert/Alle
    /// </summary>
    [AFDescription("Sonstiges")]
    Undefined = 10,
    /// <summary>
    /// Kommando für die Konsole
    /// </summary>
    [AFDescription("Befehl für Kommandozeile")]
    ConsoleCommand = 11,
    /// <summary>
    /// Selektionskriterium für Selektionen und Kampagnen
    /// </summary>
    [AFDescription("Selektionskriterium")]
    SelectionCriteria = 12,
    /// <summary>
    /// Datenmanipulation
    /// Script, der auf ein Model bestimmten Typs angewendet werden kann.
    /// </summary>
    [AFDescription("Bearbeitung Model")]
    ObjektManipulation = 13,
    /// <summary>
    /// Script zur Auswertung eines Fragebogens
    /// </summary>
    [AFDescription("Auswertung Fragebogen")]
    FragebogenAuswertung = 14,
    /// <summary>
    /// Script der Daten für einen Textbaustein liefert
    /// </summary>
    [AFDescription("Berechneter Textbaustein")]
    Textbaustein = 15,
    /// <summary>
    /// Filter für DataTable.
    /// Erhält ein DataTable und kann dieses manipulieren (Zeilen/Spalten hinzufügen, löschen etc.)
    /// Rückgabe ist immer auch ein DataTable.
    /// </summary>
    [AFDescription("Filter für Datenquelle")]
    DatenFilter = 16,
    /// <summary>
    /// Script der eine Liste von DataRow-Objekten entgegennimmt, um für diese Liste eine Verarbeitung vorzunehmen.
    /// </summary>
    [AFDescription("Bearbeitung Datentabelle")]
    DatenBearbeitung = 17,
    /// <summary>
    /// Element wird als Datenquelle verwendet (Objektdatenquelle)
    /// </summary>
    [AFDescription("Datenquelle (Objekt)")]
    DataSourceObject = 18
}


/// <summary>
/// Zentrale Erweiterungsmethoden für IScript und IScriptController, 
/// die dann im konkreten Implementierungen immer automatisch zur Verfügung stehen.
/// </summary>
public static class IScriptEx
{

}


/// <summary>
/// Script-Templates
/// 
/// Um ein Template zu erhalten ScriptTemplates.GetTemplate(eScriptType) aufrufen.
/// </summary>
public static class ScriptTemplates
{
    /// <summary>
    /// Zusätzliche Namespaces, die immer mit eingebunden werden sollen (je nach Anwendung)
    /// </summary>
    public static string[] AdditionalDefaulNamespaces { get; set; } = [];

    /// <summary>
    /// Standard-Header der Scripte
    /// </summary>
    public const string ScriptHeader = @"
using System;
using System.Collections.Generic;
using System.Linq;
using AF.CORE;
#NAMESPACES#

namespace AF.SAFIPT;

public class Script
{
     public Script() { }

     public Dictionary<string, object> Variablen { get; set; } = [];

     public AF.CORE.Log? Log { get; set; }  
        ";
    
    /// <summary>
    /// Standard-Footer der Scripte
    /// </summary>
    public const string ScriptFooter = @"
}
";

    /// <summary>
    /// Script für eScriptType.DataSourceObject.
    /// </summary>
    public const string DataSourceObject = @"
/// <summary>
/// Methode die ausgeführt wird. 
///
/// Die Methode erhält ein Ausgangsobjekt (optional) und muss dass Datenquellen-Objekt zurückliefern.
/// </summary>
/// <param name=""source"">Quelle (z.B. ein Model: Firma)</param>
public object? Execute(object? source)
{
    if (source is not Firma firma)
        return null;

    FirmaErweitert result = new();
    result.CopyFrom(firma);

    return result;
}

/// <summary>
/// Datenquelle, die zurückgeliefert wird. 
public class FirmaErweitert : Firma
{
    

}
";

    /// <summary>
    /// Script für eScriptType.DataSource.
    /// </summary>
    public const string DataSource = @"
/// <summary>
/// Methode die ausgeführt wird. 
///
/// Kommandozeilenargumente:
///
/// Die Kommandozeilenargumente werden in ein Dictionary übertragen und an die Methode übergeben (siehe AF.CORE.CommandLineParser).
/// 
/// Bsp.: scriptbefehl -f ""mein langer Dateiname"" -p 0 -c -r -k
/// 
/// Der Name eines Parameters beginnt mit - oder /. Er wird immer in Kleinbuchstaben übersetzt.
/// Der Wert hinter dem Namen des Parameters ist optional. Werte die ein Leerzeichen enthalten oder 
/// mit - oder / beginnen müssen immer in Anführungszeichen gesetzt sein.
/// </summary>
/// <param name=""feedback"">Delegate, der aufgerufen wird, wenn die Methode in der Console eine Ausgabe erzeugen möchte.</param>
/// <param name=""parameter"">Liste der Kommandozeilenargumente, die in der Konsole eingegeben wurden.</param>
public bool Execute(Action<string> feedback, Dictionary<string, string> parameter)
{
}
";

    /// <summary>
    /// Script für eScriptType.ConsoleCommand.
    /// </summary>
    public const string DashboardElement = @"
/// <summary>
/// Methode die ausgeführt wird. 
///
/// Kommandozeilenargumente:
///
/// Die Kommandozeilenargumente werden in ein Dictionary übertragen und an die Methode übergeben (siehe AF.CORE.CommandLineParser).
/// 
/// Bsp.: scriptbefehl -f ""mein langer Dateiname"" -p 0 -c -r -k
/// 
/// Der Name eines Parameters beginnt mit - oder /. Er wird immer in Kleinbuchstaben übersetzt.
/// Der Wert hinter dem Namen des Parameters ist optional. Werte die ein Leerzeichen enthalten oder 
/// mit - oder / beginnen müssen immer in Anführungszeichen gesetzt sein.
/// </summary>
/// <param name=""feedback"">Delegate, der aufgerufen wird, wenn die Methode in der Console eine Ausgabe erzeugen möchte.</param>
/// <param name=""parameter"">Liste der Kommandozeilenargumente, die in der Konsole eingegeben wurden.</param>
public bool Execute(Action<string> feedback, Dictionary<string, string> parameter)
{
}
";

    /// <summary>
    /// Script für eScriptType.ConsoleCommand.
    /// </summary>
    public const string ConsoleCommand = @"
/// <summary>
/// Methode die ausgeführt wird. 
///
/// Kommandozeilenargumente:
///
/// Die Kommandozeilenargumente werden in ein Dictionary übertragen und an die Methode übergeben (siehe AF.CORE.CommandLineParser).
/// 
/// Bsp.: scriptbefehl -f ""mein langer Dateiname"" -p 0 -c -r -k
/// 
/// Der Name eines Parameters beginnt mit - oder /. Er wird immer in Kleinbuchstaben übersetzt.
/// Der Wert hinter dem Namen des Parameters ist optional. Werte die ein Leerzeichen enthalten oder 
/// mit - oder / beginnen müssen immer in Anführungszeichen gesetzt sein.
/// </summary>
/// <param name=""feedback"">Delegate, der aufgerufen wird, wenn die Methode in der Console eine Ausgabe erzeugen möchte.</param>
/// <param name=""parameter"">Liste der Kommandozeilenargumente, die in der Konsole eingegeben wurden.</param>
public bool Execute(Action<string> feedback, Dictionary<string, string> parameter)
{
    feedback?.Invoke(""Beginne mit der Ausführung..."");
    Log?.AddMessage(""Ausführung gestartet."");    

    Log?.OnNotify += (entry, _) =>
    {
        feedback?.Invoke($""{entry.MsgType}: {entry.Message}\r\n{(entry.Description.IsNotEmpty() ? entry.Description + ""\r\n"": "" }"")
    };

    // Bsp.: Eine Datei kopieren
    // Aufruf: filecopy /q ""c:\temp\test.txt"" /t ""c:\temp\test.csv""
    bool hasFailure = false;

    if (parameter.ContainsKey(""q"") == false)
    {
        feedback?.Invoke(""Quelle nicht angegeben (Parameter q)."");
        hasFailure = true;
    }
    if (parameter.ContainsKey(""t"") == false)
    {
        feedback?.Invoke(""Ziel nicht angegeben (Parameter t)."");t
        hasFailure = true;
    }

    if (hasFailure)
    {
        feedback?.Invoke(""Ausführung abgebrochen."");
        return;
    }

    FileInfo fiQuelle = new(parameter[""q""]);
    FileInfo fiZiel = new(parameter[""t""]);

    if (fiQuelle.Exist == false)
    {
        feedback?.Invoke($""Quelle {fiQuelle.FullName} nicht gefunden."");
        hasFailure = true;
    }
    
    if (fiZiel.Exist)
    {
        feedback?.Invoke($""Ziel {fiZiel.FullName} existiert bereits."");
        hasFailure = true;
    }

    if (hasFailure)
    {
        feedback?.Invoke(""Ausführung abgebrochen."");
        return;
    }

    try
    {
        fiQuelle.Copy(fiZiel.FullName);
        feedback?.Invoke(""Datei kopiert."");
    }
    catch (Exception ex)
    {
        feedback?.Invoke($""Fehler beim Kopieren der Datei.\r\n{ex.Message}"");
    }

    feedback?.Invoke(""Ausführung beendet..."");
    Log?.AddMessage(""Ausführung beendet."");    
}
";

    /// <summary>
    /// Script für eScriptType.ConsoleCommand.
    /// </summary>
    public const string DatenBearbeitung = @"
/// <summary>
/// Methode die ausgeführt wird. 
///
/// Kommandozeilenargumente:
///
/// Die Kommandozeilenargumente werden in ein Dictionary übertragen und an die Methode übergeben (siehe AF.CORE.CommandLineParser).
/// 
/// Bsp.: scriptbefehl -f ""mein langer Dateiname"" -p 0 -c -r -k
/// 
/// Der Name eines Parameters beginnt mit - oder /. Er wird immer in Kleinbuchstaben übersetzt.
/// Der Wert hinter dem Namen des Parameters ist optional. Werte die ein Leerzeichen enthalten oder 
/// mit - oder / beginnen müssen immer in Anführungszeichen gesetzt sein.
/// </summary>
/// <param name=""feedback"">Delegate, der aufgerufen wird, wenn die Methode in der Console eine Ausgabe erzeugen möchte.</param>
/// <param name=""parameter"">Liste der Kommandozeilenargumente, die in der Konsole eingegeben wurden.</param>
public bool Execute(Action<string> feedback, Dictionary<string, string> parameter)
{
}
";

    /// <summary>
    /// Script für eScriptType.ConsoleCommand.
    /// </summary>
    public const string DatenFilter = @"
/// <summary>
/// Methode die ausgeführt wird. 
///
/// Kommandozeilenargumente:
///
/// Die Kommandozeilenargumente werden in ein Dictionary übertragen und an die Methode übergeben (siehe AF.CORE.CommandLineParser).
/// 
/// Bsp.: scriptbefehl -f ""mein langer Dateiname"" -p 0 -c -r -k
/// 
/// Der Name eines Parameters beginnt mit - oder /. Er wird immer in Kleinbuchstaben übersetzt.
/// Der Wert hinter dem Namen des Parameters ist optional. Werte die ein Leerzeichen enthalten oder 
/// mit - oder / beginnen müssen immer in Anführungszeichen gesetzt sein.
/// </summary>
/// <param name=""feedback"">Delegate, der aufgerufen wird, wenn die Methode in der Console eine Ausgabe erzeugen möchte.</param>
/// <param name=""parameter"">Liste der Kommandozeilenargumente, die in der Konsole eingegeben wurden.</param>
public bool Execute(Action<string> feedback, Dictionary<string, string> parameter)
{
}
";

    /// <summary>
    /// Script für eScriptType.ConsoleCommand.
    /// </summary>
    public const string Textbaustein = @"
/// <summary>
/// Methode die ausgeführt wird. 
///
/// Kommandozeilenargumente:
///
/// Die Kommandozeilenargumente werden in ein Dictionary übertragen und an die Methode übergeben (siehe AF.CORE.CommandLineParser).
/// 
/// Bsp.: scriptbefehl -f ""mein langer Dateiname"" -p 0 -c -r -k
/// 
/// Der Name eines Parameters beginnt mit - oder /. Er wird immer in Kleinbuchstaben übersetzt.
/// Der Wert hinter dem Namen des Parameters ist optional. Werte die ein Leerzeichen enthalten oder 
/// mit - oder / beginnen müssen immer in Anführungszeichen gesetzt sein.
/// </summary>
/// <param name=""feedback"">Delegate, der aufgerufen wird, wenn die Methode in der Console eine Ausgabe erzeugen möchte.</param>
/// <param name=""parameter"">Liste der Kommandozeilenargumente, die in der Konsole eingegeben wurden.</param>
public bool Execute(Action<string> feedback, Dictionary<string, string> parameter)
{
}
";

    /// <summary>
    /// Script für eScriptType.ConsoleCommand.
    /// </summary>
    public const string FragebogenAuswertung = @"
/// <summary>
/// Methode die ausgeführt wird. 
///
/// Kommandozeilenargumente:
///
/// Die Kommandozeilenargumente werden in ein Dictionary übertragen und an die Methode übergeben (siehe AF.CORE.CommandLineParser).
/// 
/// Bsp.: scriptbefehl -f ""mein langer Dateiname"" -p 0 -c -r -k
/// 
/// Der Name eines Parameters beginnt mit - oder /. Er wird immer in Kleinbuchstaben übersetzt.
/// Der Wert hinter dem Namen des Parameters ist optional. Werte die ein Leerzeichen enthalten oder 
/// mit - oder / beginnen müssen immer in Anführungszeichen gesetzt sein.
/// </summary>
/// <param name=""feedback"">Delegate, der aufgerufen wird, wenn die Methode in der Console eine Ausgabe erzeugen möchte.</param>
/// <param name=""parameter"">Liste der Kommandozeilenargumente, die in der Konsole eingegeben wurden.</param>
public bool Execute(Action<string> feedback, Dictionary<string, string> parameter)
{
}
";

    /// <summary>
    /// Script für eScriptType.ConsoleCommand.
    /// </summary>
    public const string ObjektManipulation = @"
/// <summary>
/// Methode die ausgeführt wird. 
///
/// Kommandozeilenargumente:
///
/// Die Kommandozeilenargumente werden in ein Dictionary übertragen und an die Methode übergeben (siehe AF.CORE.CommandLineParser).
/// 
/// Bsp.: scriptbefehl -f ""mein langer Dateiname"" -p 0 -c -r -k
/// 
/// Der Name eines Parameters beginnt mit - oder /. Er wird immer in Kleinbuchstaben übersetzt.
/// Der Wert hinter dem Namen des Parameters ist optional. Werte die ein Leerzeichen enthalten oder 
/// mit - oder / beginnen müssen immer in Anführungszeichen gesetzt sein.
/// </summary>
/// <param name=""feedback"">Delegate, der aufgerufen wird, wenn die Methode in der Console eine Ausgabe erzeugen möchte.</param>
/// <param name=""parameter"">Liste der Kommandozeilenargumente, die in der Konsole eingegeben wurden.</param>
public bool Execute(Action<string> feedback, Dictionary<string, string> parameter)
{
}
";

    /// <summary>
    /// Script für eScriptType.ConsoleCommand.
    /// </summary>
    public const string SelectionCriteria = @"
/// <summary>
/// Methode die ausgeführt wird. 
///
/// Kommandozeilenargumente:
///
/// Die Kommandozeilenargumente werden in ein Dictionary übertragen und an die Methode übergeben (siehe AF.CORE.CommandLineParser).
/// 
/// Bsp.: scriptbefehl -f ""mein langer Dateiname"" -p 0 -c -r -k
/// 
/// Der Name eines Parameters beginnt mit - oder /. Er wird immer in Kleinbuchstaben übersetzt.
/// Der Wert hinter dem Namen des Parameters ist optional. Werte die ein Leerzeichen enthalten oder 
/// mit - oder / beginnen müssen immer in Anführungszeichen gesetzt sein.
/// </summary>
/// <param name=""feedback"">Delegate, der aufgerufen wird, wenn die Methode in der Console eine Ausgabe erzeugen möchte.</param>
/// <param name=""parameter"">Liste der Kommandozeilenargumente, die in der Konsole eingegeben wurden.</param>
public bool Execute(Action<string> feedback, Dictionary<string, string> parameter)
{
}
";

    /// <summary>
    /// Script für eScriptType.ConsoleCommand.
    /// </summary>
    public const string Job = @"
/// <summary>
/// Methode die ausgeführt wird. 
///
/// Kommandozeilenargumente:
///
/// Die Kommandozeilenargumente werden in ein Dictionary übertragen und an die Methode übergeben (siehe AF.CORE.CommandLineParser).
/// 
/// Bsp.: scriptbefehl -f ""mein langer Dateiname"" -p 0 -c -r -k
/// 
/// Der Name eines Parameters beginnt mit - oder /. Er wird immer in Kleinbuchstaben übersetzt.
/// Der Wert hinter dem Namen des Parameters ist optional. Werte die ein Leerzeichen enthalten oder 
/// mit - oder / beginnen müssen immer in Anführungszeichen gesetzt sein.
/// </summary>
/// <param name=""feedback"">Delegate, der aufgerufen wird, wenn die Methode in der Console eine Ausgabe erzeugen möchte.</param>
/// <param name=""parameter"">Liste der Kommandozeilenargumente, die in der Konsole eingegeben wurden.</param>
public bool Execute(Action<string> feedback, Dictionary<string, string> parameter)
{
}
";

    /// <summary>
    /// Script für eScriptType.ConsoleCommand.
    /// </summary>
    public const string DetailView = @"
/// <summary>
/// Methode die ausgeführt wird. 
///
/// Kommandozeilenargumente:
///
/// Die Kommandozeilenargumente werden in ein Dictionary übertragen und an die Methode übergeben (siehe AF.CORE.CommandLineParser).
/// 
/// Bsp.: scriptbefehl -f ""mein langer Dateiname"" -p 0 -c -r -k
/// 
/// Der Name eines Parameters beginnt mit - oder /. Er wird immer in Kleinbuchstaben übersetzt.
/// Der Wert hinter dem Namen des Parameters ist optional. Werte die ein Leerzeichen enthalten oder 
/// mit - oder / beginnen müssen immer in Anführungszeichen gesetzt sein.
/// </summary>
/// <param name=""feedback"">Delegate, der aufgerufen wird, wenn die Methode in der Console eine Ausgabe erzeugen möchte.</param>
/// <param name=""parameter"">Liste der Kommandozeilenargumente, die in der Konsole eingegeben wurden.</param>
public bool Execute(Action<string> feedback, Dictionary<string, string> parameter)
{
}
";

    /// <summary>
    /// Script für eScriptType.ConsoleCommand.
    /// </summary>
    public const string DetailCommand = @"
/// <summary>
/// Methode die ausgeführt wird. 
///
/// Kommandozeilenargumente:
///
/// Die Kommandozeilenargumente werden in ein Dictionary übertragen und an die Methode übergeben (siehe AF.CORE.CommandLineParser).
/// 
/// Bsp.: scriptbefehl -f ""mein langer Dateiname"" -p 0 -c -r -k
/// 
/// Der Name eines Parameters beginnt mit - oder /. Er wird immer in Kleinbuchstaben übersetzt.
/// Der Wert hinter dem Namen des Parameters ist optional. Werte die ein Leerzeichen enthalten oder 
/// mit - oder / beginnen müssen immer in Anführungszeichen gesetzt sein.
/// </summary>
/// <param name=""feedback"">Delegate, der aufgerufen wird, wenn die Methode in der Console eine Ausgabe erzeugen möchte.</param>
/// <param name=""parameter"">Liste der Kommandozeilenargumente, die in der Konsole eingegeben wurden.</param>
public bool Execute(Action<string> feedback, Dictionary<string, string> parameter)
{
}
";

    /// <summary>
    /// Script für eScriptType.ConsoleCommand.
    /// </summary>
    public const string MasterCommand = @"
/// <summary>
/// Methode die ausgeführt wird. 
///
/// Kommandozeilenargumente:
///
/// Die Kommandozeilenargumente werden in ein Dictionary übertragen und an die Methode übergeben (siehe AF.CORE.CommandLineParser).
/// 
/// Bsp.: scriptbefehl -f ""mein langer Dateiname"" -p 0 -c -r -k
/// 
/// Der Name eines Parameters beginnt mit - oder /. Er wird immer in Kleinbuchstaben übersetzt.
/// Der Wert hinter dem Namen des Parameters ist optional. Werte die ein Leerzeichen enthalten oder 
/// mit - oder / beginnen müssen immer in Anführungszeichen gesetzt sein.
/// </summary>
/// <param name=""feedback"">Delegate, der aufgerufen wird, wenn die Methode in der Console eine Ausgabe erzeugen möchte.</param>
/// <param name=""parameter"">Liste der Kommandozeilenargumente, die in der Konsole eingegeben wurden.</param>
public bool Execute(Action<string> feedback, Dictionary<string, string> parameter)
{
}
";

    /// <summary>
    /// Script für eScriptType.ConsoleCommand.
    /// </summary>
    public const string ListenImport = @"
/// <summary>
/// Methode die ausgeführt wird. 
///
/// Kommandozeilenargumente:
///
/// Die Kommandozeilenargumente werden in ein Dictionary übertragen und an die Methode übergeben (siehe AF.CORE.CommandLineParser).
/// 
/// Bsp.: scriptbefehl -f ""mein langer Dateiname"" -p 0 -c -r -k
/// 
/// Der Name eines Parameters beginnt mit - oder /. Er wird immer in Kleinbuchstaben übersetzt.
/// Der Wert hinter dem Namen des Parameters ist optional. Werte die ein Leerzeichen enthalten oder 
/// mit - oder / beginnen müssen immer in Anführungszeichen gesetzt sein.
/// </summary>
/// <param name=""feedback"">Delegate, der aufgerufen wird, wenn die Methode in der Console eine Ausgabe erzeugen möchte.</param>
/// <param name=""parameter"">Liste der Kommandozeilenargumente, die in der Konsole eingegeben wurden.</param>
public bool Execute(Action<string> feedback, Dictionary<string, string> parameter)
{
}
";
    
    /// <summary>
    /// Script für eScriptType.ConsoleCommand.
    /// </summary>
    public const string ScriptVariable = @"
/// <summary>
/// Methode die ausgeführt wird. 
///
/// Kommandozeilenargumente:
///
/// Die Kommandozeilenargumente werden in ein Dictionary übertragen und an die Methode übergeben (siehe AF.CORE.CommandLineParser).
/// 
/// Bsp.: scriptbefehl -f ""mein langer Dateiname"" -p 0 -c -r -k
/// 
/// Der Name eines Parameters beginnt mit - oder /. Er wird immer in Kleinbuchstaben übersetzt.
/// Der Wert hinter dem Namen des Parameters ist optional. Werte die ein Leerzeichen enthalten oder 
/// mit - oder / beginnen müssen immer in Anführungszeichen gesetzt sein.
/// </summary>
/// <param name=""feedback"">Delegate, der aufgerufen wird, wenn die Methode in der Console eine Ausgabe erzeugen möchte.</param>
/// <param name=""parameter"">Liste der Kommandozeilenargumente, die in der Konsole eingegeben wurden.</param>
public bool Execute(Action<string> feedback, Dictionary<string, string> parameter)
{
}
";

    /// <summary>
    /// Script für eScriptType.ConsoleCommand.
    /// </summary>
    public const string WorkflowAktion = @"
/// <summary>
/// Methode die ausgeführt wird. 
///
/// Kommandozeilenargumente:
///
/// Die Kommandozeilenargumente werden in ein Dictionary übertragen und an die Methode übergeben (siehe AF.CORE.CommandLineParser).
/// 
/// Bsp.: scriptbefehl -f ""mein langer Dateiname"" -p 0 -c -r -k
/// 
/// Der Name eines Parameters beginnt mit - oder /. Er wird immer in Kleinbuchstaben übersetzt.
/// Der Wert hinter dem Namen des Parameters ist optional. Werte die ein Leerzeichen enthalten oder 
/// mit - oder / beginnen müssen immer in Anführungszeichen gesetzt sein.
/// </summary>
/// <param name=""feedback"">Delegate, der aufgerufen wird, wenn die Methode in der Console eine Ausgabe erzeugen möchte.</param>
/// <param name=""parameter"">Liste der Kommandozeilenargumente, die in der Konsole eingegeben wurden.</param>
public bool Execute(Action<string> feedback, Dictionary<string, string> parameter)
{
}
";

    /// <summary>
    /// Script für eScriptType.ConsoleCommand.
    /// </summary>
    public const string WorkflowFilter = @"
/// <summary>
/// Methode die ausgeführt wird. 
///
/// Kommandozeilenargumente:
///
/// Die Kommandozeilenargumente werden in ein Dictionary übertragen und an die Methode übergeben (siehe AF.CORE.CommandLineParser).
/// 
/// Bsp.: scriptbefehl -f ""mein langer Dateiname"" -p 0 -c -r -k
/// 
/// Der Name eines Parameters beginnt mit - oder /. Er wird immer in Kleinbuchstaben übersetzt.
/// Der Wert hinter dem Namen des Parameters ist optional. Werte die ein Leerzeichen enthalten oder 
/// mit - oder / beginnen müssen immer in Anführungszeichen gesetzt sein.
/// </summary>
/// <param name=""feedback"">Delegate, der aufgerufen wird, wenn die Methode in der Console eine Ausgabe erzeugen möchte.</param>
/// <param name=""parameter"">Liste der Kommandozeilenargumente, die in der Konsole eingegeben wurden.</param>
public bool Execute(Action<string> feedback, Dictionary<string, string> parameter)
{
}
";
    
    /// <summary>
    /// Liefert ein passendes Template für einen Script-Typ.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetTemplate(eScriptType type)
    {
        var sb = StringBuilderPool.GetStringBuilder();
        var sbUsings = StringBuilderPool.GetStringBuilder();

        AdditionalDefaulNamespaces.ForEach(n => sb.AppendEach("using ", n, ";\r\n"));

        sb.Append(ScriptHeader);

        switch (type)
        {
            case eScriptType.ConsoleCommand:
                sbUsings.Append("using System.IO;\r\n");
                sb.Append(ConsoleCommand);
                break;
            case eScriptType.DashboardElement:
                sb.Append(DashboardElement);
                break;
            case eScriptType.DataSource:
                sb.Append(DataSource);
                break;
            case eScriptType.DataSourceObject:
                sb.Append(DataSourceObject);
                break;
            case eScriptType.WorkflowFilter:
                sb.Append(WorkflowFilter); 
                break;
            case eScriptType.WorkflowAktion:
                sb.Append(WorkflowAktion); 
                break;
            case eScriptType.Variable:
                sb.Append(ScriptVariable); 
                break;
            case eScriptType.ListenImport:
                sb.Append(ListenImport); 
                break;
            case eScriptType.MasterCommand:
                sb.Append(MasterCommand); 
                break;
            case eScriptType.DetailCommand:
                sb.Append(DetailCommand); 
                break;
            case eScriptType.DetailView:
                sb.Append(DetailView); 
                break;
            case eScriptType.Job:
                sb.Append(Job); 
                break;
            case eScriptType.SelectionCriteria:
                sb.Append(SelectionCriteria); 
                break;
            case eScriptType.ObjektManipulation:
                sb.Append(ObjektManipulation); 
                break;
            case eScriptType.FragebogenAuswertung:
                sb.Append(FragebogenAuswertung);
                break;
            case eScriptType.Textbaustein:
                sb.Append(Textbaustein); 
                break;
            case eScriptType.DatenFilter:
                sb.Append(DatenFilter); 
                break;
            case eScriptType.DatenBearbeitung:
                sb.Append(DatenBearbeitung); 
                break;
            default:
                sb.Append(@"
/// <summary>
/// Methode die ausgeführt wird. 
/// </summary>
public bool Execute()
{

}
");
                break;
        }

        sb.Append(ScriptFooter);

        sb.Replace("#NAMESPACES#", sbUsings.ToString());

        var ret = sb.ToString();
        StringBuilderPool.ReturnStringBuilder(sb);
        StringBuilderPool.ReturnStringBuilder(sbUsings);

        return ret;
    }
}