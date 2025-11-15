namespace AF.CORE;

/// <summary>
/// Variable, die einen ja/nein Wert darstellt (ein/aus)
/// </summary>
[Serializable]
public class VariableBool : VariableBase
{
    private string _displayStringOn = CoreStrings.YES;
    private string _displayStringOff = CoreStrings.NO;
    private string _outputStringOn = CoreStrings.YES;
    private string _outputStringOff = CoreStrings.NO;
    private bool _default = false;

    /// <summary>
    /// Zeichenkette, die angezeigt wird, wenn der Wert an ist.
    /// </summary>
    [AFBinding]
    [AFContext("Anzeige 'ein'", Description = "Text, der angezeigt wird, wenn der Wert 'ein' ist.")]
    [AFRules(MinLength = 1)]
    public string DisplayStringOn { get => _displayStringOn; set => Set(ref _displayStringOn, value); }

    /// <summary>
    /// String, der angezeigt wird, wenn der Wert aus ist
    /// </summary>
    [AFBinding]
    [AFContext("Anzeige 'aus'", Description = "Text, der angezeigt wird, wenn der Wert 'aus' ist.")]
    [AFRules(MinLength = 1)]
    public string DisplayStringOff { get => _displayStringOff; set => Set(ref _displayStringOff, value); }

    /// <summary>
    /// Zeichenkette zur Ausgabe (CurrentAsString oder DefaultAsString), wenn der Wert auf
    /// </summary>
    [AFBinding]
    [AFContext("Ausgabetext 'ein'", Description = "Text, der ausgegeben wird, wenn der Wert 'ein' ist.")]
    [AFRules(MinLength = 1)]
    public string OutputStringOn { get => _outputStringOn; set => Set(ref _outputStringOn, value); }

    /// <summary>
    /// Zeichenkette zur Ausgabe (CurrentAsString oder DefaultAsString), wenn der Wert aus ist
    /// </summary>
    [AFBinding]
    [AFContext("Ausgabetext 'aus'", Description = "Text, der ausgegeben wird, wenn der Wert 'aus' ist.")]
    [AFRules(MinLength = 1)]
    public string OutputStringOff { get => _outputStringOff; set => Set(ref _outputStringOff, value); }

    /// <summary>
    /// Standard/Vorgabewert
    /// </summary>
    [AFBinding]
    [AFContext("Vorgabe", Description = "Vorgabewert für die Variable.")]
    public bool Default { get => _default; set => Set(ref _default, value); }
}