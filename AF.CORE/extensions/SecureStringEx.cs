using System.Security;

namespace AF.CORE;

/// <summary>
/// Erweiterungsmethoden für SecureString
/// </summary>
public static class SecureStringEx
{
    /// <summary>
    /// SecureString mit dem angegebenen Text befüllen
    /// </summary>
    /// <param name="secstring">SecureString-Objekt</param>
    /// <param name="text">Text, der im String abgelegt werden soll</param>
    public static void FromString(this SecureString secstring, string text)
    {
        secstring.Clear();

        for (int pos = 0; pos < text.Length; ++pos)
            secstring.AppendChar(text[pos]);
    }
}

