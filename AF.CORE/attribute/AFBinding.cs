namespace AF.CORE;

/// <summary>
/// Attribut, das eine Eigenschaft als bereit für die automatische Datenbindung kennzeichnet, 
/// z.B. in AFBindingConnector.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class AFBinding : Attribute
{
    /// <summary>
    /// Anzeige- und Bearbeitungsformat, wenn der Editor und der Wert es unterstützen (kann z. B. für numerische und Datum/Zeit-Eigenschaften verwendet werden).
    /// </summary>
    public string DisplayFormat { get; set; } = string.Empty;

    /// <summary>
    /// Maximale Länge von Zeichenketten, die an ein- oder mehrzeilige Bearbeitungen gebunden sind.
    /// </summary>
    public int MaxLength { get; set; }

    /// <summary>
    /// Setzt das Steuerelement, an das die Eigenschaft gebunden ist, auf schreibgeschützt, wenn true.
    /// </summary>
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Eigenschaft/Feld, dass einen ModelLink zum angezeigten Wert liefert, um z.B. AFLabelModelLink zu bedienen. 
    /// </summary>
    public string Link { get; set; } = string.Empty;
}

