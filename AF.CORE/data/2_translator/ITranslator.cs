namespace AF.DATA;

/// <summary>
/// Übersetzer für einen Datenbankdialekt
/// </summary>
public interface ITranslator
{
    /// <summary>
    /// Wert aus Datenbank in Zieltyp konvertieren
    /// </summary>
    /// <param name="value">Wert</param>
    /// <param name="targettype">in Typ konvertieren</param>
    /// <returns>umgewandelter Wert</returns>
    object? FromDatabase(object? value, Type targettype);

    /// <summary>
    /// Wert in Datenbanktyp konvertieren
    /// </summary>
    /// <param name="value">zu konvertierender Wert</param>
    /// <returns></returns>
    object ToDatabase(object value);

    /// <summary>
    /// Wert in Datenbanktyp konvertieren
    /// </summary>
    /// <param name="value">zu konvertierender Wert</param>
    /// <param name="valuetype">Tariftyp</param>
    /// <returns>umgewandelter Wert</returns>
    object ToDatabase(object value, Type valuetype);

    /// <summary>
    /// Konvertiert Wert in Datenbanktyp mit optionaler Komprimierung, wenn Wert ein Bytearray oder ein serialisierbares Objekt ist
    /// </summary>
    /// <param name="value">zu konvertierender Wert</param>
    /// <param name="valuetype">Typ des Wertes</param>
    /// <param name="compress">Komprimierung der Daten (ZIP)</param>
    /// <returns>konvertierter Wert</returns>
    object ToDatabase(object value, Type valuetype, bool compress);

    /// <summary>
    /// liefert Befehlszeichenfolge für einen bestimmten Befehl/Element
    /// </summary>
    /// <param name="command">Befehl/Element</param>
    /// <returns>Code für den Befehl/das Element</returns>
    string GetCommandString(eCommandString command);

    /// <summary>
    /// übersetzt eine Sql-Abfrage für die Datenbank
    /// </summary>
    /// <param name="query">abfrage</param>
    /// <returns>übersetzte Abfrage</returns>
    string TranslateQuery(ref string query);

    /// <summary>
    /// liefert SQL-Code für ein bestimmtes Trigger-Ereignis
    /// </summary>
    /// <param name="code">Trigger</param>
    /// <returns>Code für diesen Trigger</returns>
    string GetTriggerEvent(eTriggerEvent code);

    /// <summary>
    /// Liste von Funktionen, die in Abfragen verwendet werden können und automatisch für den Datenbanktyp übersetzt werden (im Quellcode)
    /// </summary>
    List<StringParserSnippet> CustomFunctions { get; }

#if (NET481_OR_GREATER)
    /// <summary>
    /// Parameter für Query aktualisieren.
    /// </summary>
    /// <param name="parameter">Parameter</param>
    /// <param name="propertyType">Datentyp des Wertes</param>
    void UpdateParameter<TParameter>(TParameter parameter, Type propertyType) where TParameter : DbParameter, new();
#endif
}