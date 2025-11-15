namespace AF.CORE;

/// <summary>
/// Regeln für die Definition von Grenzwerten für eine Eigenschaft.
/// 
/// Die Validierung der Eigenschaft verwendet diese Regel, um 
/// den aktuellen Inhalt der Eigenschaft zu validieren.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class AFRules : Attribute
{
    /// <summary>
    /// Die Mindestlänge einer Zeichenkette (0 bedeutet keine Begrenzung)
    /// </summary>
    public int MinLength { get; set; } = 0;

    /// <summary>
    /// Die maximale Länge einer Zeichenkette (0 bedeutet keine Begrenzung)
    /// </summary>
    public int MaxLength { get; set; } = 0;

    /// <summary>
    /// Der Mindestwert einer numerischen Eigenschaft (0 bedeutet keine Begrenzung)
    /// </summary>
    public double Maximum { get; set; } = 0;

    /// <summary>
    /// Der maximale Wert einer numerischen Eigenschaft (Standardwert 0)
    /// </summary>
    public double Minimum { get; set; } = 0;

    /// <summary>
    /// Der Mindestwert (Standard: DateTime.MaxValue)
    /// </summary>
    public DateTime MaxDateTime { get; set; } = DateTime.MaxValue;

    /// <summary>
    /// Der maximale Wert einer numerischen Eigenschaft (Standard: DateTime.MinValue)
    /// </summary>
    public DateTime MinDateTime { get; set; } = DateTime.MinValue;

    /// <summary>
    /// Die Zeichen, die in einer Zeichenkette erlaubt sind.
    /// </summary>
    public string AllowedChars { get; set; } = string.Empty;

    /// <summary>
    /// Die Zeichen, die in einer Zeichenkette nicht erlaubt sind.
    /// </summary>
    public string NotAllowedChars { get; set; } = string.Empty;

    /// <summary>
    /// Überprüft einen Wert anhand der Regel
    /// </summary>
    /// <param name="val">Wert</param>
    /// <param name="name">Name der zu prüfenden Eigenschaft</param>
    /// <param name="errors">Sammlung von Fehlern</param>
    /// <returns>true wenn gültig, false wenn resultMessages Fehler enthält</returns>
    public bool Validate(object val, string name, ref ValidationErrorCollection errors)
    {
        if (val is string strVal)
        {
            if (NotAllowedChars.IsNotEmpty() && strVal.ContainsNotAllowedChars(NotAllowedChars))
            {
                errors.Add(name,
                    string.Format(CoreStrings.ERR_NOTALLOWEDCHARS, NotAllowedChars));
            }

            if (AllowedChars.IsNotEmpty() && strVal.ContainsOnlyAllowedChars(AllowedChars) == false)
            {
                errors.Add(name,
                    string.Format(CoreStrings.ERR_ALLOWEDCHARS, AllowedChars));
            }


            if (MaxLength > 0 && strVal.Length > MaxLength)
                errors.Add(name, string.Format(CoreStrings.ERR_TOMUCHCHARS, MaxLength));

            if (MinLength > 0 && strVal.Length < MinLength)
                errors.Add(name, string.Format(CoreStrings.ERR_TOFEWCHARS, MinLength));
        }
        else if (val is DateTime dtVal)
        {
            if (MinDateTime > DateTime.MinValue && dtVal.Date < MinDateTime.Date)
                errors.Add(name, string.Format(CoreStrings.ERR_DATECANNOTBELOWER, MinDateTime));

            if (MaxDateTime > DateTime.MinValue && dtVal.Date > MinDateTime.Date)
                errors.Add(name, string.Format(CoreStrings.ERR_DATECANNOTBEHIGHER, MinDateTime));
        }
        else if (val is short sVal)
        {
            short min = Convert.ToInt16(Minimum);
            short max = Convert.ToInt16(Maximum);

            if (min != 0 && sVal < min)
                errors.Add(name, string.Format(CoreStrings.ERR_VALUETOSMALL, min));

            if (max != 0 && sVal > max)
                errors.Add(name, string.Format(CoreStrings.ERR_VALUETOBIG, max));
        }
        else if (val is int iVal)
        {
            int min = Convert.ToInt32(Minimum);
            int max = Convert.ToInt32(Maximum);

            if (min != 0 && iVal < min)
                errors.Add(name, string.Format(CoreStrings.ERR_VALUETOSMALL, min));

            if (max != 0 && iVal > max)
                errors.Add(name, string.Format(CoreStrings.ERR_VALUETOBIG, max));
        }
        else if (val is long lVal)
        {
            long min = Convert.ToInt64(Minimum);
            long max = Convert.ToInt64(Maximum);

            if (min != 0 && lVal < min)
                errors.Add(name, string.Format(CoreStrings.ERR_VALUETOSMALL, min));

            if (max != 0 && lVal > max)
                errors.Add(name, string.Format(CoreStrings.ERR_VALUETOBIG, max));
        }
        else if (val is ushort usVal)
        {
            ushort min = Convert.ToUInt16(Minimum);
            ushort max = Convert.ToUInt16(Maximum);

            if (min != 0 && usVal < min)
                errors.Add(name, string.Format(CoreStrings.ERR_VALUETOSMALL, min));

            if (max != 0 && usVal > max)
                errors.Add(name, string.Format(CoreStrings.ERR_VALUETOBIG, max));
        }
        else if (val is uint uiVal)
        {
            uint min = Convert.ToUInt32(Minimum);
            uint max = Convert.ToUInt32(Maximum);

            if (min != 0 && uiVal < min)
                errors.Add(name, string.Format(CoreStrings.ERR_VALUETOSMALL, min));

            if (max != 0 && uiVal > max)
                errors.Add(name, string.Format(CoreStrings.ERR_VALUETOBIG, max));
        }
        else if (val is ulong ulVal)
        {
            ulong min = Convert.ToUInt64(Minimum);
            ulong max = Convert.ToUInt64(Maximum);

            if (min != 0 && ulVal < min)
                errors.Add(name, string.Format(CoreStrings.ERR_VALUETOSMALL, min));

            if (max != 0 && ulVal > max)
                errors.Add(name, string.Format(CoreStrings.ERR_VALUETOBIG, max));
        }
        else if (val is decimal decVal)
        {
            decimal min = Convert.ToDecimal(Minimum);
            decimal max = Convert.ToDecimal(Maximum);

            if (min != 0 && decVal < min)
                errors.Add(name, string.Format(CoreStrings.ERR_VALUETOSMALL, min));

            if (max != 0 && decVal > max)
                errors.Add(name, string.Format(CoreStrings.ERR_VALUETOBIG, max));
        }
        else if (val is double dblVal)
        {
            double min = Convert.ToDouble(Minimum);
            double max = Convert.ToDouble(Maximum);

            if (min != 0 && dblVal < min)
                errors.Add(name, string.Format(CoreStrings.ERR_VALUETOSMALL, min));

            if (max != 0 && dblVal > max)
                errors.Add(name, string.Format(CoreStrings.ERR_VALUETOBIG, max));
        }
        else if (val is float fVal)
        {
            float min = Convert.ToSingle(Minimum);
            float max = Convert.ToSingle(Maximum);

            if (min != 0 && fVal < min)
                errors.Add(name, string.Format(CoreStrings.ERR_VALUETOSMALL, min));

            if (max != 0 && fVal > max)
                errors.Add(name, string.Format(CoreStrings.ERR_VALUETOBIG, max));
        }
        else if (val is byte bVal)
        {
            byte min = Convert.ToByte(Minimum);
            byte max = Convert.ToByte(Maximum);

            if (min != 0 && bVal < min)
                errors.Add(name, string.Format(CoreStrings.ERR_VALUETOSMALL, min));

            if (max != 0 && bVal > max)
                errors.Add(name, string.Format(CoreStrings.ERR_VALUETOBIG, max));
        }
        else if (val is sbyte sbVal)
        {
            sbyte min = Convert.ToSByte(Minimum);
            sbyte max = Convert.ToSByte(Maximum);

            if (min != 0 && sbVal < min)
                errors.Add(name, string.Format(CoreStrings.ERR_VALUETOSMALL, min));

            if (max != 0 && sbVal > max)
                errors.Add(name, string.Format(CoreStrings.ERR_VALUETOBIG, max));
        }

        return errors.Count < 1;
    }
}
