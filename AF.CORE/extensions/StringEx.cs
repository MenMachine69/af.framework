using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace AF.CORE;

/// <summary>
/// Erweiterungsmethoden für System.String
/// </summary>
[Localizable(false)]
public static class StringEx
{
    /// <summary>
    /// Mehrzeiligen Text in einzeilig umwandeln und doppelte Leerzeichen vermeiden.
    /// </summary>
    public static string ToSingleLine(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var sb = StringBuilderPool.GetStringBuilder(input.Length);
        bool lastWasWhitespace = false;

        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == '\r' || input[i] == '\n' || char.IsWhiteSpace(input[i]))
            {
                if (lastWasWhitespace) continue;

                sb.Append(' ');
                lastWasWhitespace = true;
            }
            else
            {
                sb.Append(input[i]);
                lastWasWhitespace = false;
            }
        }

        var ret = sb.ToString();

        StringBuilderPool.ReturnStringBuilder(sb);

        return ret;
    }

    /// <summary>
    /// Aufteilen eines strings in ein Array von strings.
    ///
    /// Berücksichtigt auch strings in Anführungszeichen,
    /// die nicht aufgeteilt werden. (CSV-Dateien)
    /// </summary>
    /// <param name="source">aufzuteilende Quelle</param>
    /// <param name="separator">Trennzeichen, Standard = ','</param>
    /// <returns>aufgeteilte strings als array</returns>
    public static string[] SplitCsv(this string source, char separator = ',')
    {
        StringBuilder sb = StringBuilderPool.GetStringBuilder();
        bool insideQuota = false;
        List<string> tokens = [];

        for (var pos = 0; pos < source.Length; pos++)
        {
            if (source[pos] == separator && !insideQuota)
            {
                tokens.Add(sb.ToString());
                sb.Clear();
                continue;
            }

            if (source[pos] == '"')
                insideQuota = !insideQuota;

            sb.Append(source[pos]);
        }

        tokens.Add(sb.ToString());
        StringBuilderPool.ReturnStringBuilder(sb);

        return tokens.ToArray();
    }

    /// <summary>
    /// Prüfen der IBAN auf Gültigkeit.
    /// </summary>
    /// <param name="iban">zu prüfende IBAN</param>
    /// <returns>true, wenn gültig, sonst false</returns>
    public static bool CheckIban(this string iban)
    {
        string ibanCleared = iban.ToUpper().Replace(" ", "").Replace("-", "");
        string ibanSwapped = ibanCleared.Substring(4) + ibanCleared.Substring(0, 4);
        string sum = ibanSwapped.Aggregate("", (current, c) => current + (char.IsLetter(c) ? (c - 55).ToString() : c.ToString()));

        var d = decimal.Parse(sum);
        return ((d % 97) == 1);
    }

    /// <summary>
    /// Prüfen, ob der Wert des Strings zwischen den zwei Werten liegt oder einem der beiden Werte entspricht.
    /// Der Vergleich basiert auf der alphabetischen Sortierung der Werte, wobei Groß- und Kleinschreibung keine Rolle spielt.
    /// 
    /// <code>
    /// string plzVon = "01067";
    /// string plzBis = "01072";
    /// 
    /// "01069".Between(plzVon) -> true;
    /// "01067".Between(plzVon) -> true;
    /// "01072".Between(plzVon) -> true;
    /// "01066".Between(plzVon) -> false;
    /// "01073".Between(plzVon) -> false;
    /// </code>
    /// </summary>
    /// <param name="check">zu prüfender Wert</param>
    /// <param name="upperBorder">Untergrenze</param>
    /// <param name="lowerBorder">Obergrenze</param>
    /// <returns>true wenn innerhalb der Grenzen oder Grenze selbst, sonst false</returns>
    public static bool Between(this string check, string upperBorder, string lowerBorder)
    {
        return string.Compare(check, lowerBorder, StringComparison.OrdinalIgnoreCase) >= 0 && string.Compare(check, upperBorder, StringComparison.OrdinalIgnoreCase) <= 0;
    }

    /// <summary>
    /// Extract characters at the odd position of a string
    /// <example>
    /// string test = "This is a test";
    /// test.ExtractOdd() /// returns "Ti sats"
    /// </example>
    /// </summary>
    /// <param name="source">Source</param>
    /// <returns>characters at the even positions</returns>
    public static string ExtractEven(this string source)
    {
        string ret = "";
        for (int i = 0; i < source.Length; i += 2)
        {
            if (i <= (source.Length - 1))
                ret += source.Substring(i, 1);
        }

        return ret;
    }

    /// <summary>
    /// Die Zeile extrahieren, die das Zeichen an Position characterPosition enthält.
    /// </summary>
    /// <param name="input">String/Zeichenkette, aus der extrahiert wird</param>
    /// <param name="characterPosition">Position des Zeichens, dessen Zeile extrahiert werden soll.</param>
    /// <param name="startpos">Startposition der Zeile in der Zeichenkette</param>
    /// <returns>extrahierte Zeile</returns>
    /// <exception cref="ArgumentOutOfRangeException">Fehler, wenn sich characterPosition außerhalb der Zeicehnkette befindet</exception>
    public static string ExtractLine(this string input, int characterPosition, out int startpos)
    {
        // Validate character position
        if (characterPosition < 0 || characterPosition >= input.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(characterPosition), @"Invalid character position");
        }

        // Find the start index of the line
        int startIndex = input.LastIndexOfAny(['\n', '\r'], characterPosition) + 1;

        startpos = startIndex;

        // Find the end index of the line
        int endIndex = input.IndexOfAny(['\n', '\r'], characterPosition);

        // If the character position is at the end of the string, set endIndex to the end of the string
        if (endIndex == -1) endIndex = input.Length;

        // Extract the line
        int lineLength = endIndex - startIndex;
        string extractedLine = input.Substring(startIndex, lineLength);

        return extractedLine;
    }

    
    /// <summary>
    /// PLatzhalter (#NAME:format#) aus Text extrahieren.
    /// </summary>
    /// <param name="input">Text</param>
    /// <returns>Liste der extrahierten Platzhalter</returns>
    public static List<Tuple<string, string?>> ExtractPlaceholders(this string input)
    {
        var results = new List<Tuple<string, string?>>();
        var span = input.AsSpan();

        var i = 0;
        while (i < span.Length)
        {
            if (span[i] != '#')
            {
                i++;
                continue;
            }

            var start = i + 1;
            var end = span.Slice(start).IndexOf('#');
            if (end == -1) break; // kein schließendes #

            var inside = span.Slice(start, end);

            var colonIndex = inside.IndexOf(':');
            if (colonIndex >= 0)
            {
                var name = inside.Slice(0, colonIndex).ToString();
                var value = inside.Slice(colonIndex + 1).ToString();
                results.Add(new(name, value));
            }
            else
                results.Add(new(inside.ToString(), null));

            i = start + end + 1; // bis zum gefundenen '#' springen
        }

        return results;
    }

    /// <summary>
    /// Prüft, ob der aktuelle Text leer ist, keinen Text enthält oder 
    /// nur Leerzeichen enthält
    /// </summary>
    /// <param name="source">aktueller Text</param>
    /// <returns>True Text ist leer</returns>
    public static bool IsEmpty(this string? source)
    {
        return string.IsNullOrWhiteSpace(source);
    }

    /// <summary>
    /// Erzeugt SHA256-Hash der Zeichenkette
    /// </summary>
    /// <param name="source">String</param>.
    /// <returns>Hash der Zeichenkette</returns>
    public static string GetSHA256Hash(this string source)
    {
        using var hasher = SHA256.Create();
        return BitConverter.ToString(hasher.ComputeHash(Encoding.UTF8.GetBytes(source)));
    }

    /// <summary>
    /// MD5-Hash der Zeichenkette erstellen
    /// </summary>
    /// <param name="source">String</param>.
    /// <returns>Hash der Zeichenkette</returns>
    public static string GetMD5Hash(this string source)
    {
        using var hasher = MD5.Create();
        return BitConverter.ToString(hasher.ComputeHash(Encoding.UTF8.GetBytes(source))).Replace(@"-", "");
    }

    /// <summary>
    /// Prüft, ob der aktuelle Text leer ist, keinen Text enthält oder 
    /// nur Leerzeichen enthält
    /// </summary>
    /// <param name="source">aktueller Text</param>
    /// <returns>true Text ist NICHT leer</returns>
    public static bool IsNotEmpty(this string? source)
    {
        return !(string.IsNullOrWhiteSpace(source));
    }

    /// <summary>
    /// Gibt einen formatierten Text zurück und ersetzt string.Format(text, val1, val2)
    /// <example>
    /// "Der Wert beträgt derzeit {0} EUR".DisplayWith(100.55);
    /// </example>
    /// </summary>
    /// <param name="source">Text</param>.
    /// <param name="args">Argumente, die im Text enthalten sein sollen</param>.
    /// <returns>erweiterter Text</returns>
    public static string DisplayWith(this string source, params object[] args)
    {
        return string.Format(source, args);
    }

    /// <summary>
    /// Returns the number of characters from the left.
    /// If the string is shorter, the entire string is returned - no 'out of index' error is generated.
    /// </summary>
    /// <param name="source">String</param>.
    /// <param name="characters">Number of characters</param>.
    /// <returns>Characters from the left side of the string</returns>.
    public static string Left(this string source, int characters)
    {
        return source.Length > characters ? source[..characters] : source;
    }

    /// <summary>
    /// Gibt die Anzahl der Zeichen von rechts zurück.
    /// Wenn die Zeichenkette kürzer ist, wird die gesamte Zeichenkette zurückgegeben - es wird kein 'out of index'-Fehler erzeugt.
    /// </summary>
    /// <param name="source">String</param>.
    /// <param name="characters">Anzahl der Zeichen</param>.
    /// <returns>Zeichen von der rechten Seite der Zeichenkette</returns>
    public static string Right(this string source, int characters)
    {
        return source.Length > characters ? source.Substring(source.Length - characters, characters) : source;
    }

    /// <summary>
    /// Prüft, ob die Zeichenkette unzulässige Zeichen enthält.
    /// </summary>
    /// <param name="source">Zu prüfender Text</param>
    /// <param name="notallowed">nicht erlaubte Zeichen</param>
    /// <returns>true, wenn der Text unzulässige Zeichen enthält</returns>
    public static bool ContainsNotAllowedChars(this string source, string notallowed)
    {
        return source.Select(c => notallowed.Contains(c) ? c : char.MinValue)
            .Where(c => c != char.MinValue).ToArray()
            .Length > 0;
    }

    /// <summary>
    /// prüft, ob eine Zeichenkette eine andere enthält, ignoriert aber die Groß-/Kleinschreibung
    /// </summary>
    /// <param name="source">Quellzeichenfolge</param>
    /// <param name="search">Zeichenfolge für die Suche in der Quelle</param>
    /// <returns>true wenn gefunden, sonst false</returns>
    public static bool ContainsIgnoreCase(this string source, string search)
    {
        return source.Contains(search, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// findet die erste Position einer Zeichenkette, ignoriert aber die Groß-/Kleinschreibung
    /// <see cref="string.IndexOf(string)"/>
    /// </summary>
    /// <param name="source">Quellzeichenkette</param>
    /// <param name="search">Zeichenfolge, die in der Quelle gesucht werden soll</param>
    /// <returns>Position</returns>
    public static int IndexOfIgnoreCase(this string source, string search)
    {
        return source.IndexOf(search, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// findet die erste Position einer Zeichenkette, die an der angegebenen Position steht, ignoriert aber die Groß-/Kleinschreibung
    /// <see cref="string.IndexOf(string)"/>
    /// </summary>
    /// <param name="source">Quellzeichenkette</param>
    /// <param name="search">Zeichenfolge für die Suche in der Quelle</param>
    /// <param name="startpos">Startposition</param>
    /// <returns>Position</returns>
    public static int IndexOfIgnoreCase(this string source, string search, int startpos)
    {
        return source.IndexOf(search, startpos, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Entfernt alle Zeichen, die nicht in den erlaubten Zeichen enthalten sind.
    /// </summary>
    /// <param name="source">Zeichenfolge</param>
    /// <param name="chars">erlaubte Zeichen</param>.
    /// <returns>String mit nur den erlaubten Zeichen</returns>
    public static string RemoveAllExcept(this string source, string chars)
    {
        return new string((source.Select(c => chars.Contains(c) ? c : char.MinValue))
            .ToArray()).Replace(new string([char.MinValue]), "");
    }

    /// <summary>
    /// Prüft, ob die Zeichenkette nur erlaubte Zeichen enthält.
    /// </summary>
    /// <param name="source">Zu prüfender Text</param>
    /// <param name="allowed">erlaubte Zeichen</param>
    /// <returns>true, wenn der Text nur erlaubte Zeichen enthält</returns>
    public static bool ContainsOnlyAllowedChars(this string source, string allowed)
    {
        return source.Select(c => allowed.Contains(c) ? char.MinValue : c)
            .Where(c => c != char.MinValue).ToArray().Length <= 0;
    }

    /// <summary>
    /// Zählt, wie oft eine Zeichenkette in einer anderen Zeichenkette vorkommt.
    /// Erweitert die Klasse String
    /// <example>
    /// string test="Dies ist ein Test";
    /// int blank=test.Count(" ");
    /// </example>
    ///
    /// Kann mit RegEx-Patterns als Suchbegriff verwendet werden.
    /// /// <example>
    /// string test="Select AFUpper(TEST), AFlower(TEST2), AFSubstring(TEST3,1,3)...";
    /// int blank=test.Count("AF.*\("); // counts all occurences of strings starting with AF and ending with ( - result: 3
    /// </example>
    /// </summary>
    /// <param name="source">Zu suchende Zeichenkette</param>.
    /// <param name="search">Zu durchsuchende Zeichenkette</param>.
    /// <returns>Anzahl der Vorkommen der Suche in der Quelle</returns>
    public static int Count(this string source, string search)
    {
        return new Regex(Regex.Escape(search)).Matches(source).Count;
    }

    /// <summary>
    /// Zählt, wie oft eine Zeichenkette in einer anderen Zeichenkette vorkommt.
    /// Ignoriert die Groß- und Kleinschreibung der Zeichenketten.
    /// Erweitert die Klasse String
    /// <example>
    /// string test="This is a test";
    /// int blank=test.Count(" ");
    /// </example>
    ///
    /// Kann mit RegEx-Patterns als Suchbegriff verwendet werden.
    /// /// <example>
    /// string test="Select AFUpper(TEST), crlower(TEST2), crSubstring(TEST3,1,3)...";
    /// int blank=test.Count("AF.*\("); // counts all occurences of strings starting with AF and ending with ( - result: 3
    /// </example>
    /// </summary>
    /// <param name="source">Zu suchende Zeichenkette</param>.
    /// <param name="search">Zu durchsuchende Zeichenkette</param>.
    /// <returns>Anzahl der Vorkommen der Suche in der Quelle</returns>
    public static int CountIgnoreCase(this string source, string search)
    {
        return new Regex(Regex.Escape(search), RegexOptions.IgnoreCase).Matches(source).Count;
    }


    /// <summary>
    /// Gibt eine Teilzeichenkette zurück
    /// </summary>
    /// <param name="source">Zeichenkette, deren Teilzeichenfolge ermittelt werden soll</param>.
    /// <param name="startpos">Position des ersten Zeichens</param>.
    /// <param name="endpos">Position des letzten Zeichens</param>.
    /// <returns>Substring</returns>
    public static string SubString(this string source, int startpos, int endpos)
    {
        return source.Substring(startpos, endpos - startpos + 1);
    }

    /// <summary>
    /// Überprüft, ob ein String ausschließlich Ziffern enthält.
    /// </summary>
    /// <param name="source">zu prüfender String</param>
    /// <returns>true wenn nur Ziffern enthalten sind, sonst false</returns>
    public static bool IsAllDigits(this string source)
    {
        const string chars = "0123456789";

        for (var pos = 0; pos < source.Length; ++pos)
            if (!chars.Contains(source[pos])) return false;

        return true;
    }


    /// <summary>
    /// Checks whether the first character is a digit.
    /// </summary>
    /// <param name="source">the text to be checked</param>
    /// <returns>true if the first character is a digit, otherwise false (spaces are ignored)</returns>
    public static bool IsDigit(this string source)
    {
        return ("0123456789".Contains(source.Trim().Left(1)));
    }

    /// <summary>
    /// Gibt alle Ziffern am Anfang einer Zeichenkette zurück.
    /// Ignoriert alle Leerzeichen am Anfang oder innerhalb der Ziffern.
    /// Kann z.B. verwendet werden, um eine Postleitzahl unbekannter Länge aus einer Zeichenkette zu extrahieren, wenn diese mit der Postleitzahl beginnt.
    /// <example>
    /// string test=" 013 09 Dresden";
    /// string plz=test.GetDigits(); // return "01309"
    /// </example>
    /// </summary>
    /// <param name="source">der zu prüfende Text</param>.
    /// <returns>Alle Ziffern, mit denen eine Zeichenkette beginnt (Leerzeichen werden ignoriert)</returns>
    public static string GetDigits(this string source)
    {
        string ret = "";

        for (int i = 0; i < source.Length; i++)
        {
            if (source[i] == ' ')
                continue;

            if (char.IsDigit(source[i]))
                ret += source[i];
            else
                break;
        }

        return ret;
    }

    /// <summary>
    /// Ersetze alle Ziffern durch einen Platzhalter
    /// </summary>
    /// <param name="source">der Text, in dem die Ziffern ersetzt werden sollen</param>.
    /// <param name="pattern">Zeichen zum Ersetzen der Ziffern in der Zeichenkette</param>.
    /// <returns>Text, der den Platzhalter anstelle der Ziffern enthält</returns>
    public static string ReplaceDigits(this string source, char pattern)
    {
        StringBuilder sb = new(source);

        foreach (char chr in "0123456789")
            sb.Replace(chr, pattern);

        return sb.ToString();
    }


    /// <summary>
    /// Vergleich mit einem vereinfachten Muster
    /// 
    /// ? = genau ein beliebiges Zeichen 
    /// % = kein oder eine beliebige Anzahl von Zeichen (beliebige Zeichenkette)
    /// * = kein oder eine beliebige Anzahl von Zeichen, aber kein Leerzeichen (=ein Wort)
    /// # = genau eine der Ziffern 0-9
    /// 
    /// eine Guid : ????????-????-????-????-????????????
    /// eine E-Mail Addresse : %?@?%.?%
    /// ein Datum : ##.##.####
    /// </summary>
    /// <param name="source">Zeichenfolge für die Übereinstimmung</param>
    /// <param name="simplepattern"></param>
    /// <returns>true, wenn die Quelle mit dem Muster übereinstimmt</returns>
    public static bool Like(this string source, string simplepattern)
    {
        return simplepattern.GetRegexFromPattern().IsMatch(source);
    }

    /// <summary>
    /// Wandelt ein einfaches Muster in einen RegEx-Ausdruck um, der dann für Vergleiche usw. verwendet werden kann.
    /// 
    /// Das vereinfachte Muster kann die folgenden Zeichen enthalten:
    /// 
    /// ? = genau ein Zeichen 
    /// % = kein oder eine beliebige Anzahl von Zeichen (beliebige Zeichenfolge)
    /// * = kein oder eine beliebige Anzahl von Zeichen, aber kein Leerzeichen (=ein Wort)
    /// # = genau eine der Ziffern 0-9
    /// 
    /// eine Guid : ????????-????-????-????-????????????
    /// eine E-Mail Addresse : %?@?%.?%
    /// ein Datum : ##.##.####
    /// </summary>
    /// <param name="pattern">Vereinfachtes Muster für die Suche</param>.
    /// <returns>RegEx für das vereinfachte Muster</returns>
    public static Regex GetRegexFromPattern(this string pattern)
    {
        pattern = "^" + pattern;
        return new Regex(pattern
                .Replace("\\", "\\\\")
                .Replace(".", "\\.")
                .Replace("{", "\\{")
                .Replace("}", "\\}")
                .Replace("[", "\\[")
                .Replace("]", "\\]")
                .Replace("+", "\\+")
                .Replace("$", "\\$")
                .Replace(" ", "\\s")
                .Replace("#", "[0-9]")
                .Replace("?", ".")
                .Replace("*", "\\w*")
                .Replace("%", ".*")
            , RegexOptions.IgnoreCase);
    }

    /// <summary>
    /// Split a string into an array of strings.
    /// 
    /// Strings inside quotas are not split (for example: "this is a test").
    /// </summary>
    /// <param name="commandline">source string</param>
    /// <param name="command">ignore the first string (for example: the command name)</param>
    /// <param name="quotas">characters used as quota (default = null = ' and ") </param>
    /// <returns></returns>
    public static string[] SplitCommandLine(this string commandline, out string command, char[]? quotas)
    {
        command = "";
        quotas ??= ['"', '\''];
        List<string> ret = [];
        bool inquota = false;
        bool isfirst = true;
        StringBuilder current = new();

        for (int i = 0; i < commandline.Length; i++)
        {
            if (quotas.Contains(commandline[i]))
            {
                inquota = !inquota;
                continue;
            }

            if (commandline[i] == ' ' && !inquota)
            {
                if (current.Length > 0)
                {
                    if (isfirst)
                        command = current.ToString();
                    else
                        ret.Add(current.ToString());

                    current.Clear();
                    isfirst = false;
                }

                continue;
            }

            current.Append(commandline[i]);
        }

        if (current.Length > 0)
        {
            if (isfirst)
                command = current.ToString();
            else
                ret.Add(current.ToString());
        }

        return ret.ToArray();
    }

    /// <summary>
    /// Erstellen Sie den phonetischen Code einer Zeichenkette.
    /// 
    /// Zur phonetischen Kodierung wird die sogenannte Kölner Phonetik verwendet.
    /// </summary>
    /// <param name="input">Eingabe-String</param>.
    /// <returns>Phonetischer Code (Kölner Phonetik)</returns>
    public static string PhoneticEncode(this string input)
    {
        string[] words = input.Split([' ', '\r', '\n', '-', '/', ',', ';', '&'],
            StringSplitOptions.RemoveEmptyEntries);
        string ret = "";

        foreach (string word in words)
        {
            char[] valueChars = word.ToUpperInvariant().ToCharArray();

            // create an array for all the characters without specialities
            char[] value0Chars = ['A', 'E', 'I', 'J', 'O', 'U', 'Y', 'Ä', 'Ö', 'Ü'];
            char[] value1Chars = ['B'];
            char[] value3Chars = ['F', 'V', 'W'];
            char[] value4Chars = ['G', 'K', 'Q'];
            char[] value5Chars = ['L'];
            char[] value6Chars = ['M', 'N'];
            char[] value7Chars = ['R'];
            char[] value8Chars = ['S', 'Z', 'ß'];

            StringBuilder cpCode = StringBuilderPool.GetStringBuilder();

            for (int i = 0; i < valueChars.Length; i++)
            {
                char previousChar = i > 0 ? valueChars[i - 1] : ' ';
                char currentChar = valueChars[i];
                char nextChar = i < valueChars.Length - 1 ? valueChars[i + 1] : ' ';

                bool isFirstChar = (i == 0 || !char.IsLetter(previousChar));

                if (!char.IsLetter(currentChar))
                {
                    if (char.IsWhiteSpace(currentChar))
                        cpCode.Append(' ');

                    continue;
                }

                if (value0Chars.Contains(currentChar))
                {
                    cpCode.Append('0');
                    continue;
                }

                if (value1Chars.Contains(currentChar))
                {
                    cpCode.Append('1');
                    continue;
                }

                if (value3Chars.Contains(currentChar))
                {
                    cpCode.Append('3');
                    continue;
                }

                if (value4Chars.Contains((currentChar)))
                {
                    cpCode.Append('4');
                    continue;
                }

                if (value5Chars.Contains(currentChar))
                {
                    cpCode.Append('5');
                    continue;
                }

                if (value6Chars.Contains(currentChar))
                {
                    cpCode.Append('6');
                    continue;
                }

                if (value7Chars.Contains(currentChar))
                {
                    cpCode.Append('7');
                    continue;
                }

                if (value8Chars.Contains(currentChar))
                {
                    cpCode.Append('8');
                    continue;
                }

                switch (currentChar)
                {
                    case 'C' when isFirstChar:
                        {
                            cpCode.Append(new[] { 'A', 'H', 'K', 'L', 'O', 'Q', 'R', 'U', 'X' }.Contains(nextChar)
                                ? '4'
                                : '8');
                            break;
                        }
                    case 'C' when new[] { 'S', 'Z', 'ß' }.Contains(previousChar):
                        cpCode.Append('8');
                        break;
                    case 'C' when new[] { 'A', 'H', 'K', 'O', 'Q', 'U', 'X' }.Contains(nextChar):
                        cpCode.Append('4');
                        break;
                    case 'C':
                        cpCode.Append('8');
                        break;
                    case 'D':
                    case 'T':
                        {
                            cpCode.Append(new[] { 'C', 'S', 'Z', 'ß' }.Contains(nextChar) ? '8' : '2');
                            break;
                        }
                    case 'P' when nextChar.Equals('H'):
                        cpCode.Append('3');
                        break;
                    case 'P':
                        cpCode.Append('1');
                        break;
                    case 'X' when new[] { 'C', 'K', 'Q' }.Contains(previousChar):
                        cpCode.Append('8');
                        break;
                    case 'X':
                        cpCode.Append('4');
                        cpCode.Append('8');
                        break;
                }
            }

            // cleanup the code (remove double characters and remove 0 values)
            StringBuilder cleanedCpCode = StringBuilderPool.GetStringBuilder(cpCode.Length);

            for (int i = 0; i < cpCode.Length; i++)
            {
                char lastAddedChar = cleanedCpCode.Length > 0 ? cleanedCpCode[^1] : ' ';

                if (lastAddedChar == cpCode[i]) continue;

                if (cpCode[i] != '0' || (cpCode[i] == '0' && lastAddedChar == ' '))
                    cleanedCpCode.Append(cpCode[i]);
            }

            ret += valueChars[0] + cleanedCpCode.ToString() + "|";

            StringBuilderPool.ReturnStringBuilder(cpCode);
            StringBuilderPool.ReturnStringBuilder(cleanedCpCode);
        }

        return ret;
    }

    /// <summary>
    /// Wandelt einen String in eine byte[]
    /// </summary>
    /// <param name="str">string</param>
    /// <returns>byte[] aus dem String</returns>
    public static byte[] ToByteArray(this string str)
    {
        return Encoding.UTF8.GetBytes(str);
    }

    /// <summary>
    /// Wandelt ein byte[] in einen String
    /// </summary>
    /// <param name="byteArray">Byte-Array</param>
    /// <returns>string aus dem Array</returns>
    public static string FromByteArray(this byte[] byteArray)
    {
        return Encoding.UTF8.GetString(byteArray);
    }

    /// <summary>
    /// Liefert die Assemblies, auf die im Code verwiesen wird.
    /// </summary>
    /// <returns></returns>
    public static List<string> GetAssembliesFromCode(this string sourceCode)
    {
        if (!sourceCode.Contains(("//@:"))) return [];

        var assemblies = new List<string>();
        var startIndex = 0;
        var isAssemblyLine = false;
        
        var sb = StringBuilderPool.GetStringBuilder(content: sourceCode);
        foreach (KeyValuePair<string, IPlaceholder> keyValuePair in AFCore.Placeholder)
            if (sb.Contains(keyValuePair.Key))
                sb = sb.Replace(keyValuePair.Key, keyValuePair.Value.GetValue());
        sourceCode = sb.ToString();
        StringBuilderPool.ReturnStringBuilder(sb);

        for (var i = 0; i < sourceCode.Length; i++)
        {
            // Leerzeichen am Anfang ignorieren
            if (i == startIndex && char.IsWhiteSpace(sourceCode[i]))
            {
                startIndex++;
                continue;
            }

            // Überprüfe, ob eine Zeile mit //@: beginnt
            if (i == startIndex && sourceCode.Length >= i + 4 &&
                sourceCode[i] == '/' && sourceCode[i + 1] == '/' &&
                sourceCode[i + 2] == '@' && sourceCode[i + 3] == ':')
            {
                isAssemblyLine = true;
            }

            if (sourceCode[i] != '\n' && sourceCode[i] != '\r') continue;

            if (isAssemblyLine)
            {
                assemblies.Add(sourceCode.Substring(startIndex + 4, i - (startIndex + 4)));
                isAssemblyLine = false;
            }

            startIndex = i + 1; // Setze den Startindex auf die nächste Zeile
        }

        // Letzte Zeile prüfen, da sie möglicherweise nicht mit einem Zeilenumbruch endet
        if (isAssemblyLine)
        {
            assemblies.Add(sourceCode[startIndex..]);
        }

        return assemblies;
    }

}

