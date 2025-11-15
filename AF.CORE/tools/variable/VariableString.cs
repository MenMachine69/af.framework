namespace AF.CORE;

/// <summary>
/// Variable, die einen String-Wert repräsentiert
/// </summary>
[Serializable]
public class VariableString : VariableBase
{
    /// <summary>
    /// maximale Länge
    /// </summary>
    [AFBinding]
    [AFContext("Max. Länge", Description = "Maximal mögliche Anzahl von Zeichen, die eingegeben werden können.")]
    public int MaxLength { get; set; } = 1024;

    /// <summary>
    /// minimale Länge
    /// </summary>
    [AFBinding]
    [AFContext("Min. Länge", Description = "Anzahl von Zeichen die mindestens eingegeben werden müssen (0 = keine Begrenzung).")]
    public int MinLength { get; set; } = 0;

    /// <summary>
    /// erlaubte zeichen
    /// </summary>
    [AFBinding]
    [AFContext("Erlaubte Zeichen", Description = "Zeichen, die bei der Eingabe verwendet werden können. Zeichen, die hier nicht enthalten sind, werden als Fehler zurückgewiesen (leer = keine Beschränkungen).")]
    public string CharsAllowed { get; set; } = string.Empty;

    /// <summary>
    /// nicht erlaubte Zeichen
    /// </summary>
    [AFBinding]
    [AFContext("Nicht erlaubte Zeichen", Description = "Zeichen, die bei der Eingabe nicht verwendet werden dürfen. Sind diese Zeichen enthalten, wird dies als Fehler zurückgewiesen (leer = keine Beschränkungen).")]
    public string CharsNotAllowed { get; set; } = string.Empty;

    /// <inheritdoc />
    public override bool IsValid(ValidationErrorCollection errors)
    {
        var ret = base.IsValid(errors);

        if (CharsAllowed.IsNotEmpty() && CharsNotAllowed.IsNotEmpty())
        {
            ret = false;
            errors.Add(nameof(CharsNotAllowed), "Erlaubte und nicht erlaubte Zeichen können nicht gleichzeitig verwendet werden.");
        }

        if (MinLength > MaxLength && MaxLength > 0)
        {
            ret = false;
            errors.Add(nameof(MinLength), "Die Mindestlänge kann nicht größer als die maximale Länge sein.");
        }

        return ret;
    }

    /// <inheritdoc />
    public override bool ValidateValue(ValidationErrorCollection errors, string name, object? value)
    {
        if (value is not string stringvalue) return false;

        if ((MaxLength > 0 && stringvalue.Length > MaxLength) ||
            (MinLength > 0 && stringvalue.Length < MinLength))
        {
            errors.Add(name, string.Format(CoreStrings.ERR_WRONGSTRINGLENGTH, MinLength, MaxLength));
            return false;
        }

        if (CharsAllowed.IsNotEmpty() &&  !stringvalue.ContainsOnlyAllowedChars(CharsAllowed))
        {
            errors.Add(name, string.Format(CoreStrings.ERR_ALLOWEDCHARS, CharsAllowed));
            return false;
        }

        if (CharsNotAllowed.IsNotEmpty() && stringvalue.ContainsNotAllowedChars(CharsNotAllowed))
        {
            errors.Add(name, string.Format(CoreStrings.ERR_NOTALLOWEDCHARS, CharsNotAllowed));
            return false;
        }

        return true;
    }

    /// <summary>
    /// Standard/Vorgabewert
    /// </summary>
    [AFBinding]
    [AFContext("Vorgabewert", Description = "Vorgegebener Wert.")]
    public string Default { get; set; } = string.Empty;
}