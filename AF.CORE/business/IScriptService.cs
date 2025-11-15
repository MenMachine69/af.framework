using Flee.PublicTypes;

namespace AF.CORE;

/// <summary>
/// Scripting-Unterstützung
/// </summary>
public interface IScriptService
{
    /// <summary>
    /// Registriert den Typ des Controls über das ein
    /// Script ausgewählt werden kann.
    ///
    /// Das Control/der Typ muss die Schnittstelle IScriptSelectControl implementieren.
    /// </summary>
    void RegisterScriptSelectControlType(Type controlType);

    /// <summary>
    /// Liefert den vorher via RegisterScriptSelectControlType registrierten Typ des Controls für die Script-Auswahl.
    /// </summary>
    Type GetScriptSelectControlType();

    #region Flee-Expressions

    /// <summary>
    /// Registriert eine oder mehrere KLassen, deren Methoden anschließend im Expression-Evaluator zur Verfügung stehen.
    /// </summary>
    /// <param name="types">Liste der zu registrierenden Klassen/Typen</param>
    void RegisterFunctions(Type[] types);

    /// <summary>
    /// Registriert eine Variable die im Evaluator verwendet werden kann.
    /// </summary>
    /// <param name="name">Name der Variablen</param>
    /// <param name="value">Wert der Variablen</param>
    void SetVariable(string name, object value);

    /// <summary>
    /// Löscht eine im Evaluator registrierte Variable. 
    /// </summary>
    /// <param name="name">Name der Variablen</param>
    void RemoveVariable(string name);

    /// <summary>
    /// Löscht alle im Evaluator registrierten Variablen. 
    /// </summary>
    public void ClearVariables();

    /// <summary>
    /// Einen Ausdruck interpretieren und ausführen (Evaluator).
    /// 
    /// Kann mit einer vorkomplierten Expression arbeiten, um mehrfaches kompilieren zu vermeiden.
    /// </summary>
    /// <param name="expression">zu interpretierender Ausdruck</param>
    /// <param name="preCompiled">vorkompilierte Expression (siehe CompileExpression)</param>
    /// <returns>Wert oder NULL</returns>
    object? EvaluateExpression(string expression, IDynamicExpression? preCompiled = null);

    /// <summary>
    /// Zugriff auf den Flee-Evaluator
    /// </summary>
    ExpressionContext Evaluator { get; } 
    #endregion

    /// <summary>
    /// Cache leeren.
    /// 
    /// Kann auch genutzt werden, um beim Start des Programms den Compiler zu initialisieren.
    /// </summary>
    void ClearCache();

    /// <summary>
    /// Script compilieren
    /// </summary>
    /// <param name="source">Quelltext</param>
    /// <param name="id">ID des Script, wenn != Guid.Empty, wird der interne ScriptCache verwendet</param>
    /// <param name="script">Ergebnis des Kompilierens - ein Objekt der Klasse im Script</param>
    /// <param name="result">Ergebnis des Kompilierens (Fehlermeldungen etc.)</param>
    /// <param name="options">weitere Optionen</param>
    /// <param name="domain">
    /// AppDomain, in die der Script geladen werden soll.
    /// 
    /// Wird keine Domain angegeben, wird der Script im aktuellen AppDomain geladen. 
    /// Nur OHNE die Angabe einer Domain werden Scripte gecacht!
    /// </param>
    /// <returns>true, wenn erfolgreich, sonst false</returns>
    bool TryToCompile<T>(string source, Guid id, out T? script, ScriptCompileResult result, ScriptCompileOptions options, AppDomain? domain) where T : AFScriptBase;

    /// <summary>
    /// Führt einen im Cache abgelegten Script aus und gibt das Resultat zurück.
    /// </summary>
    /// <param name="id">ID des Scripts</param>
    /// <param name="method">aufzurufende Methode (Standard: Execute)</param>
    /// <param name="parameter">Parameter zur Übergabe an die Methode</param>
    /// <returns>Ergebnis der Ausführung</returns>
    object? Execute(Guid id, string method = "Execute", params object[] parameter);

    /// <summary>
    /// Liefert eine Liste der Snippets...
    /// </summary>
    /// <returns></returns>
    BindingList<IScriptSnippet> GetSnippets();

    /// <summary>
    /// GEHE ZU Command
    /// </summary>
    AFCommand? CmdGoto { get; set; }

    /// <summary>
    /// NEU Command
    /// </summary>
    AFCommand? CmdAdd { get; set; }
}

/// <summary>
/// Interface, dass ein Control implementieren muss, damit es für die Script-Auswahl verwendet werden kann.
/// </summary>
public interface IScriptSelectControl
{
    /// <summary>
    /// Filterbedingung für die anzuzeigenden Scripte
    /// </summary>
    eScriptType ScriptTypeFilter { get; set; }
}