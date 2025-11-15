using System.Reflection;

namespace AF.CORE;

/// <summary>
/// Erweiterungsmethoden für ENum
/// </summary>
public static class EnumEx
{
    /// <summary>
    /// Gibt die Beschreibung (über das Attribut Description oder AFDescription) des Aufzählungselements zurück.
    /// Wurde in einem Aufzählungselement keine Beschreibung definiert, wird der Name des Elements zurückgegeben.
    /// </summary>
    /// <param name="value">Wert/Element, nach dessen Beschreibung gesucht wird</param>
    /// <returns>Beschreibung/Name des Elements</returns>
    public static string GetEnumDescription(this Enum value)
    {
        string ret = value.ToString();

        FieldInfo? fi = value.GetType().GetField(ret);

        if (fi == null) return ret;

        DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

        if (attributes.Length > 0)
            return attributes[0].Description;

        AFDescription[] crattributes = (AFDescription[])fi.GetCustomAttributes(typeof(AFDescription), false);

        if (crattributes.Length > 0)
            return crattributes[0].Description;

        return ret;
    }
}


