using System.Drawing;
using DevExpress.Utils.Svg;
using System.IO.Compression;
using System.Numerics;
using System.Text.Json;

namespace AF.DATA;

/// <summary>
/// Basisklasse für Translator-Klassen, die die 
/// gemeinsamen Eigenschaften und Methoden für alle diese Klassen kapseln.
/// </summary>
public abstract class Translator
{
    private readonly Dictionary<eCommandString, string> _commands = new();

    /// <summary>
    /// Constructor
    /// </summary>
    protected Translator()
    {
        _commands.Add(eCommandString.EventAfterDelete, @"AFTER DELETE");
        _commands.Add(eCommandString.EventAfterInsert, @"AFTER INSERT");
        _commands.Add(eCommandString.EventAfterUpdate, @"AFTER UPDATE");
        _commands.Add(eCommandString.EventBeforeDelete, @"BEFORE DELETE");
        _commands.Add(eCommandString.EventBeforeInsert, @"BEFORE INSERT");
        _commands.Add(eCommandString.EventBeforeUpdate, @"BEFORE UPDATE");
        _commands.Add(eCommandString.CreateView, @"AFEATE VIEW #NAME# (#FIELDS#) AS #QUERY#");
        _commands.Add(eCommandString.DropIndex, @"DROP INDEX #NAME#");
        _commands.Add(eCommandString.DropProcedure, @"DROP PROCEDURE #NAME#");
        _commands.Add(eCommandString.DropTable, @"DROP TABLE #NAME#");
        _commands.Add(eCommandString.DropTrigger, @"DROP TRIGGER #NAME#");
        _commands.Add(eCommandString.DropView, @"DROP VIEW #NAME#");
        _commands.Add(eCommandString.EnableTrigger, @"ALTER TRIGGER #NAME# active");
        _commands.Add(eCommandString.DisableTrigger, @"ALTER TRIGGER #NAME# inactive");
        _commands.Add(eCommandString.CreateField, @"ALTER TABLE #TABLENAME# ADD #NAME# #FIELDOPTIONS#");
        _commands.Add(eCommandString.GetSchema, @"SELECT * FROM #NAME#");
        _commands.Add(eCommandString.LoadValue, @"SELECT #FIELDNAME# FROM #TABLENAME# WHERE #FIELDNAMEKEY# = ?");
        _commands.Add(eCommandString.SelectCount, @"SELECT COUNT(#FIELDNAME#) FROM #TABLENAME#");
        _commands.Add(eCommandString.SelectSum, @"SELECT SUM(#FIELDNAME#) FROM #TABLENAME#");
        _commands.Add(eCommandString.Select, @"SELECT #FIELDNAMES# FROM #TABLENAME#");
        _commands.Add(eCommandString.Load, @"SELECT #FIELDNAMES# FROM #TABLENAME# WHERE #FIELDNAMEKEY# = ?");
        _commands.Add(eCommandString.Delete, @"DELETE FROM #TABLENAME# WHERE #FIELDNAMEKEY# = ?");
        _commands.Add(eCommandString.Update, @"UPDATE #TABLENAME# set #PAIRS# WHERE #FIELDNAMEKEY# = @v0");
        _commands.Add(eCommandString.Insert, @"INSERT INTO #TABLENAME# (#FIELDS#) VALUES (#VALUES#)");
        _commands.Add(eCommandString.ExecProcedure, @"EXECUTE PROCEDURE #PROCEDURE#");
        _commands.Add(eCommandString.Exist, @"SELECT #FIELDNAMEKEY# FROM #TABLENAME# WHERE #FIELDNAMEKEY# = ?");
        _commands.Add(eCommandString.DeleteQuery, @"DELETE FROM #TABLENAME#");



        PlaceHolders = new()
        {
            { @"#TODAY#", DateTime.Now.Date.ToShortDateString() },
            { @"#MONTH#", DateTime.Now.Month.ToString() },
            { @"#YEAR#", DateTime.Now.Year.ToString() },
            { @"#DAY#", DateTime.Now.Day.ToString() },
            { @"#YESTERDAY#", DateTime.Now.Date.Subtract(new TimeSpan(1, 0, 0, 0)).ToShortDateString() },
            { @"#PASTMONTH#", (DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1).ToString() },
            { @"#PASTYEAR#", (DateTime.Now.Year - 1).ToString() },
            { @"#PASTDAY#", DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0)).Day.ToString() },
            { @"#TOMORROW#", DateTime.Now.Date.AddDays(1).ToShortDateString() },
            { @"#FOLLOWMONTH#", (DateTime.Now.Month == 12 ? 1 : DateTime.Now.Month + 1).ToString() },
            { @"#FOLLOWYEAR#", (DateTime.Now.Year + 1).ToString() },
            { @"#FOLLOWDAY#", DateTime.Now.AddDays(1).Day.ToString() },
            { @"#HOUR#", DateTime.Now.Hour.ToString() },
            { @"#MINUTE#", DateTime.Now.Minute.ToString() },
            { @"#EMPTYGUID#", @"'00000000-0000-0000-0000-000000000000'" }
        };
    }

    /// <summary>
    /// Fügt einen Platzhalter in der Liste der universellen Platzhalter hinzu oder ersetzt ihn
    /// </summary>
    /// <param name="name">Name des Platzhalters - muss mit dem Zeichen # beginnen und enden</param>
    /// <param name="value">Wert, der den Platzhalter ersetzt</param>
    public void AddOrReplacePlaceHolder(string name, string value)
    {
        if (!(name.StartsWith(@"#") && name.EndsWith(@"#")))
            throw new ArgumentException(CoreStrings.ERR_TRANS_WRONGNAME);
        
        PlaceHolders[name] = value;
    }

    /// <summary>
    /// Gibt eine global gültige Zeichenkette für den spezifischen Befehl zurück.  
    /// Wirft eine Ausnahme, wenn für diesen Befehl keine Zeichenkette verfügbar ist.
    /// </summary>
    /// <param name="command">Benötigte Befehlszeichenfolge</param>
    /// <returns>Befehlszeichenfolge</returns>
    public virtual string GetCommandString(eCommandString command)
    {
        if (_commands.TryGetValue(command, out var commandString))
            return commandString;

        throw new ArgumentException(string.Format(CoreStrings.DBTRANS_NOCOMMANDDEFINED, command.ToString()),
            nameof(command));
    }

    /// <summary>
    /// Eine Liste (Wörterbuch von Name und Wert) von universellen Platzhaltern
    /// </summary>
    public Dictionary<string, string> PlaceHolders { get; }

    /// <summary>
    /// Benutzerdefinierte Funktionen zur Verwendung in Querys
    /// </summary>
    public List<StringParserSnippet> CustomFunctions { get; } = [];

#if (NET481_OR_GREATER)
    /// <summary>
    /// Parameter für Query aktualisieren.
    /// </summary>
    /// <param name="parameter">Parameter</param>
    /// <param name="propertyType">Datentyp des Wertes</param>
    public virtual void UpdateParameter<TParameter>(TParameter parameter, Type propertyType) where TParameter : DbParameter, new() { }
#endif

    /// <summary>
    /// Trigger-Event in datenbankspezifisches Format übersetzen
    /// </summary>
    /// <param name="code"></param>
    /// <returns>SQL Code</returns>
    public virtual string GetTriggerEvent(eTriggerEvent code)
    {
        string ret = "";

        switch (code)
        {
            case eTriggerEvent.AfterDelete:
                ret = @"AFTER DELETE";
                break;
            case eTriggerEvent.BeforeInsert:
                ret = @"BEFORE INSERT";
                break;
            case eTriggerEvent.BeforeUpdate:
                ret = @"BEFORE UPDATE";
                break;
            case eTriggerEvent.BeforeDelete:
                ret = @"BEFORE DELETE";
                break;
            case eTriggerEvent.AfterInsert:
                ret = @"AFTER INSERT";
                break;
            case eTriggerEvent.AfterUpdate:
                ret = @"AFTER UPDATE";
                break;
        }

        return ret;
    }

    /// <summary>
    /// Übersetzt den Quelltext der Abfrage in das datenbankspezifische Format.
    /// 
    /// Beispiel:
    /// select * from test where AFSubstring(AFSubstring(probe, 3, 4), 5, 5) = 'Test'
    /// wird zu 
    /// select * from test where SUBSTRING(SUBSTRING(probe FROM 3 FOR 4) FROM 5 FOR 5) = 'Test' 
    /// 
    /// </summary>
    /// <param name="query">Abfrage</param>
    /// <returns>übersetzte Abfrage</returns>
    public string translate(ref string query)
    {
        StringFunctionParser parser = new ();
        parser.SetSnippets(CustomFunctions);

        return parser.Parse(ref query);
    }

    /// <summary>
    /// Konvertiert einen Wert in den entsprechenden Datenbankwert
    /// </summary>
    /// <param name="value">zu konvertierender Wert</param>
    /// <param name="valueType">Typ des zu konvertierenden Wertes, bei Übergabe von null wird versucht, den Typ aus dem Wert zu ermitteln.</param>
    /// <param name="compress">Daten komprimieren (ZIP)</param>
    /// <returns>konvertierter Wert</returns>
    public virtual object ToDatabase(object? value, Type valueType, bool compress)
    {
        object target;
       
        if (value is Type)
            valueType = typeof(Type);

        var targetType = valueType;

        try
        {
            if (valueType.IsEnum)
                target = (int)(value ?? 0);
            else if (valueType == typeof(Guid))
            {
                if (value == null || value.Equals(Guid.Empty))
                    target = DBNull.Value;
                else
                {
                    targetType = typeof(Guid);
                    target = (Guid)value;
                }
            }
            else if (valueType == typeof(bool))
                target = value ?? false;
            else if (valueType == typeof(Color))
                target = ((Color?)value)?.ToArgb() ?? Color.Black.ToArgb();
            else if (valueType == typeof(int))
                target = (int)(value ?? 0);
            else if (valueType == typeof(short))
                target = (short)(value ?? 0);
            else if (valueType == typeof(decimal))
                target = (decimal)(value ?? 0);
            else if (valueType == typeof(double))
                target = (double)(value ?? 0);
            else if (valueType == typeof(float))
                target = (float)(value ?? 0);
            else if (valueType == typeof(byte))
                target = (byte)(value ?? 0);
            else if (valueType == typeof(long))
                target = (long)(value ?? 0);
            else if (valueType == typeof(DateTime))
                target = (DateTime)(value ?? DateTime.MinValue);
            else if (valueType == typeof(DateOnly))
                target = (DateOnly)(value ?? DateOnly.MinValue);
            else if (valueType == typeof(TimeOnly))
                target = (TimeOnly)(value ?? TimeOnly.MinValue);
            else if (valueType == typeof(Type))
            {
                targetType = typeof(string);

                if (value == null)
                    target = "";
                else
                {
                    string? name = ((Type)value).FullName;
                    target = name ?? "";
                } 
            }
            else if (valueType == typeof(string))
                target = (string)(value ?? "");
            else if (valueType == typeof(byte[]))
            {
                if (value == null)
                    target = DBNull.Value;
                else
                    target = value as byte[] ?? value.ToJsonBytes();
            }
            else if (valueType == typeof(Image) ||
                     valueType == typeof(Bitmap)) // Image nach byte[]
            {
                targetType = typeof(byte[]);

                if (value == null)
                    target = DBNull.Value;
                else
                {
                    using MemoryStream stream = new();
                    try
                    {
                        if (valueType == typeof(Image))
                            ((Image)value).Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        else
                            ((Bitmap)value).Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                        target = stream.ToArray();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(CoreStrings.ERROR_WHILECONVERTIMAGETOBYTEARRAY, ex);
                    }
                }
            }
            else if (valueType == typeof(SvgImage)) // SvgImage nach byte[]
            {
                targetType = typeof(byte[]);

                if (value == null)
                    target = DBNull.Value;
                else
                {
                    using MemoryStream stream = new();
                    try
                    {
                        if (valueType == typeof(SvgImage))
                            ((SvgImage)value).Save(stream);

                        target = stream.ToArray();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(CoreStrings.ERROR_WHILECONVERTIMAGETOBYTEARRAY, ex);
                    }
                }
            }
            else if (valueType == typeof(ModelLinkCollection))
            {
                targetType = typeof(string);

                target = value == null ? DBNull.Value : ((ModelLinkCollection)value).ToString();
            }
            else if (valueType == typeof(AFBitArray))
            {
                targetType = typeof(byte[]);

                target = value == null ? DBNull.Value : ((AFBitArray)value).Data;
            }
            else if (valueType.IsValueType == false) // Object nach byte[]
            {
                targetType = typeof(byte[]);

                target = value == null ? DBNull.Value : value.ToJsonBytes();
            }
            else
                target = value ?? DBNull.Value;
        }
        catch (Exception ex)
        {
            throw new Exception(CoreStrings.ERROR_WHILETRANSLATINGTODB.DisplayWith(valueType, targetType), ex);
        }

        return target;
    }

    /// <summary>
    /// Konvertiert einen Wert in den entsprechenden Datenbankwert
    /// </summary>
    /// <param name="value">zu konvertierender Wert</param>
    /// <returns>umgewandelter Wert</returns>
    public object ToDatabase(object value)
    {
        return ToDatabase(value, value.GetType(), true);
    }

    /// <summary>
    /// Konvertiert einen Wert in den entsprechenden Datenbankwert
    /// </summary>
    /// <param name="value">zu konvertierender Wert</param>
    /// <param name="valuetype">Typ des zu konvertierenden Wertes, bei Übergabe von null wird versucht, den Typ aus dem Wert zu ermitteln.</param>
    /// <returns>konvertierter Wert</returns>
    public object ToDatabase(object value, Type valuetype)
    {
        return ToDatabase(value, valuetype, true);
    }

    /// <summary>
    /// Konvertiert einen Wert aus dem Datenbankformat in das Zielformat
    /// </summary>
    /// <param name="targettype">Zielformat</param>
    /// <param name="value">zu konvertierender Wert</param>
    /// <returns>konvertierter Wert oder default-Wert des Zielformats</returns>
    public virtual object? FromDatabase(object? value, Type targettype)
    {
        Type valtype = value == null ? typeof(DBNull) : value.GetType();

        try
        {
            if (targettype == typeof(string))
                return valtype == typeof(DBNull) || value == null ? string.Empty : value as string ?? value.ToString();

            if (targettype == typeof(Guid))
            {
                return valtype == typeof(DBNull) || value == null
                    ? Guid.Empty
                    : value is Guid guid ? guid : new Guid((byte[])value);
            }

            if (targettype == typeof(Color))
            {
                return valtype == typeof(DBNull) || value == null
                    ? Color.Empty
                    : Color.FromArgb((int)value);
            }

            if (targettype == typeof(bool))
            {
                if (valtype != typeof(DBNull) && value != null && value is string strval)
                    return @"JjYy1".Contains(strval);

                return valtype != typeof(DBNull) && value != null && (bool)value;
            }

            if (targettype == typeof(int))
                return  valtype == typeof(DBNull) ? 0 : value is int intval ? intval : Convert.ToInt32(value);
            
            if (targettype == typeof(decimal))
            {
                return valtype == typeof(DBNull)
                    ? 0
                    : value is decimal decval ? decval : Convert.ToDecimal(value);
            }
            
            if (targettype == typeof(double))
                return valtype == typeof(DBNull) ? 0 : value is double dblval ? dblval : Convert.ToDouble(value);

            if (targettype == typeof(byte))
                return valtype == typeof(DBNull) ? 0 : value is byte byteval ? byteval : Convert.ToByte(value);

            if (targettype == typeof(short))
                return valtype == typeof(DBNull) ? 0 : value is short shortval ? shortval : Convert.ToInt16(value);

            if (targettype == typeof(float))
                return valtype == typeof(DBNull) ? 0 : value is float floatval ? floatval : Convert.ToSingle(value);
            
            if (targettype == typeof(long))
            {
                return valtype == typeof(DBNull)
                    ? 0
                    : value is long longval
                        ? longval
                        : value is BigInteger bigint
                            ? ObjectEx.ToInt64(bigint)
                            : Convert.ToInt64(value);
            }
            
            if (targettype == typeof(DateTime))
            {
                return valtype == typeof(DBNull)
                    ? DateTime.MinValue
                    : value is DateTime dtval ? dtval : Convert.ToDateTime(value);
            }

            if (targettype == typeof(DateOnly))
            {
                if (valtype == typeof(DBNull)) return DateOnly.MinValue;

                if (value is DateOnly dtval) return dtval;

                if (valtype == typeof(DateTime) && value is DateTime ts) return DateOnly.FromDateTime(ts);

                return DateOnly.FromDateTime(Convert.ToDateTime(value));
            }

            if (targettype == typeof(TimeOnly))
            {
                if (valtype == typeof(DBNull)) return TimeOnly.MinValue;

                if (value is TimeOnly dtval) return dtval;

                if (valtype == typeof(TimeSpan) && value is TimeSpan ts) return TimeOnly.FromTimeSpan(ts);

                return TimeOnly.FromDateTime(Convert.ToDateTime(value));
            }

            if (targettype.IsEnum)
            {
                return valtype == typeof(DBNull) || value == null
                    ? 0
                    : Enum.ToObject(targettype, Convert.ToInt32(value));
            }
            
            if (targettype == typeof(byte[]))
                return valtype == typeof(DBNull) || value == null ? [] : (byte[])value;
           
            
            if (targettype == typeof(Type)) // Types sind als FullName gespeichert
            {
                return valtype == typeof(DBNull) || value == null || StringEx.IsEmpty(((string)value))
                    ? typeof(Nullable)
                    : TypeEx.FindType((string)value);
            }

            if (value is byte[] binval && targettype != typeof(byte[]) &&
                     targettype != typeof(Image) &&
                     targettype != typeof(Bitmap) &&
                     targettype != typeof(SvgImage) &&
                     targettype != typeof(AFBitArray)) // Byte-Arrays, bei denen der Zieltyp KEIN Byte-Array oder IMAGE ist,
                // werden als serialisierte Objekte betrachtet und
                // aus dem Byte-Array deserialisiert.

                return valtype == typeof(DBNull) ? null : Functions.DeserializeJsonBytes(targettype, binval);

            if (targettype == typeof(AFBitArray) && value is byte[] bytes)
                return new AFBitArray(bytes);

            if (targettype == typeof(ModelLinkCollection) && value is string data)
            {
                ModelLinkCollection ret = [];
                ret.FromString(data);
                return ret;
            }
                

            if ((targettype == typeof(Image) || targettype == typeof(Bitmap)) &&
                     value is byte[] bytearr &&
                     valtype !=
                     typeof(DBNull)) // Bilder werden ebenfalls in einem Byte-Array gespeichert und aus diesem deserialisiert
            {
                if (bytearr.Length > 0)
                {
                    using MemoryStream stream = new(bytearr);
                    try
                    {
                        return Image.FromStream(stream);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(CoreStrings.ERROR_WHILECONVERTIMAGEFROMBYTEARRAY, ex);
                    }
                }

                return null;
            }

            if (targettype == typeof(SvgImage) && value is byte[] bytesvg && valtype != typeof(DBNull)) // SvgBilder werden ebenfalls in einem Byte-Array gespeichert und aus diesem deserialisiert
            {
                if (bytesvg.Length > 0)
                {
                    using MemoryStream stream = new(bytesvg);
                    try
                    {
                        return SvgImage.FromStream(stream);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(CoreStrings.ERROR_WHILECONVERTIMAGEFROMBYTEARRAY, ex);
                    }
                }

                return null;
            }
        }
        catch (Exception ex)
        {
            throw new Exception(CoreStrings.ERROR_WHILETRANSLATIONFROMDB, ex);
        }

        return null;
    }

    internal byte[] toByteArray(object data)
    {
        using var to = new MemoryStream();
        using var gZipStream = new GZipStream(to, CompressionMode.Compress);
        var bytes = JsonSerializer.SerializeToUtf8Bytes(data,
            new JsonSerializerOptions
            {
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
        gZipStream.Write(bytes, 0, bytes.Length);
        gZipStream.Flush();
        return to.ToArray();
    }

    /// <summary>
    /// Objekt aus einem ByteArray deserialisieren...
    /// </summary>
    /// <param name="data">Byte-Array mit den Daten</param>
    /// <param name="toType">zu Typ deserialisieren</param>
    /// <returns>das deserialisierte Objekt (oder null)</returns>
    internal object? _fromByteArray(Type toType, byte[] data)
    {
        object? ret;

        using var from = new MemoryStream(data);
        using var to = new MemoryStream();
        using var gZipStream = new GZipStream(from, CompressionMode.Decompress);
        {
            gZipStream.CopyTo(to);
            ret = JsonSerializer.Deserialize(to, toType);
        }

        return ret;

        //return data.InvokeGeneric("Deserialize", new Type[] { toType }, data);
    }
}

