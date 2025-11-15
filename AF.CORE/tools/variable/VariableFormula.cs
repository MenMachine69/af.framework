namespace AF.CORE;

/// <summary>
/// eine Variable, die aus einer Formel besteht, um aus anderen Variablen einen Wert zu berechnen
/// </summary>
[Serializable]
public class VariableFormula : VariableBase
{
    /// <summary>
    /// Formel zur Berechnung des Wertes (z.B. 'WertNetto * 0.17' wobei WertNetto der Name einer Variablen vom Typ VariableDecimal ist)
    /// </summary>
    [AFBinding]
    [AFContext("Formel", Description = "Formel zur Berechnung des Wertes (z.B. 'WertNetto * 0.17' wobei WertNetto der Name einer Variablen vom Typ VariableDecimal ist) ")]
    public string Formel { get; set; } = string.Empty;

    /// <summary>
    /// Wert, der verwendet wird, wenn die Formel keinen Wert liefert/liefern kann. Der Wert wird in den entsprechenden Rückgabwert umgewandelt.
    /// </summary>
    [AFBinding]
    [AFContext("Nullwert", Description = "Wert, der verwendet wird, wenn die Formel keinen Wert liefert/liefern kann. Der Wert wird in den entsprechenden Rückgabwert umgewandelt.")]
    public string NullValue { get; set; } = string.Empty;

    /// <summary>
    /// Datentyp, den die Auswertung der Formel liefern muss.
    /// </summary>
    [AFBinding]
    [AFContext("Rückgabetyp", Description = "Datentyp, den die Auswertung der Formel liefert/liefern muss.")]
    public eVariableFormulaType ReturnType { get; set; } = eVariableFormulaType.String;
}