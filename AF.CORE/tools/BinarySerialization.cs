using System.Collections.Concurrent;
using System.Drawing;
using System.Reflection;

namespace  AF.CORE;

/// <summary>
/// Attribut, dass eine Eigenschaft markiert, die bei der binären Serialisierung NICHT berücksichtigt werden soll.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class BinaryIgnoreAttribute : Attribute { }

/// <summary>
/// binäre Serialisierung von Objekten
/// </summary>
public static class BinarySerialization
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertyCache = new();

    #region Write
    /// <summary>
    /// Objekt in Datei speichern
    /// </summary>
    /// <param name="obj">Objekt</param>
    /// <param name="file">Dateiname</param>
    public static void SaveObjectToFile(object obj, FileInfo file)
    {
        File.WriteAllBytes(file.FullName, SaveObjectToBytes(obj));
    }

    /// <summary>
    /// Objekt in ByteArray speichern
    /// </summary>
    /// <param name="obj">Objekt</param>
    /// <returns>ByteArray</returns>
    public static byte[] SaveObjectToBytes(object obj)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);
        writeObject(bw, obj);
        bw.Flush();
        return ms.ToArray();
    }

    private static void writeObject(BinaryWriter bw, object? obj)
    {
        if (obj == null)
        {
            bw.Write((byte)DataType.Null);
            return;
        }

        Type t = obj.GetType();
        bw.Write((byte)DataType.Object);
        bw.Write(t.FullName ?? string.Empty);

        var props = getSerializableProperties(t);
        bw.Write(props.Length);

        foreach (var prop in props)
        {
            bw.Write(prop.Name);
            object? val;
            try { val = prop.GetValue(obj); } catch { val = null; }
            writeValue(bw, val, prop.PropertyType);
        }
    }

    private static void writeValue(BinaryWriter bw, object? val, Type type)
    {
        if (val == null)
        {
            bw.Write((byte)DataType.Null);
            return;
        }

        // byte[]
        if (type == typeof(byte[]))
        {
            var bytes = (byte[])val;
            bw.Write((byte)DataType.ByteArray);
            bw.Write(bytes.Length);
            bw.Write(bytes);
            return;
        }

        // DateOnly
        if (type == typeof(DateOnly))
        {
            bw.Write((byte)DataType.DateOnly);
            bw.Write(((DateOnly)val).ToDateTime(TimeOnly.MinValue).ToBinary());
            return;
        }

        // TimeOnly
        if (type == typeof(TimeOnly))
        {
            bw.Write((byte)DataType.TimeOnly);
            bw.Write(((TimeOnly)val).Ticks);
            return;
        }

        // einfache Typen
        if (isSimpleType(type))
        {
            bw.Write((byte)DataType.Primitive);
            bw.Write(type.FullName ?? string.Empty);
            writePrimitive(bw, val, type);
            return;
        }

        // Bild
        if (val is Image img)
        {
            bw.Write((byte)DataType.Image);
            using var ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            bw.Write((int)ms.Length);
            bw.Write(ms.GetBuffer(), 0, (int)ms.Length);
            return;
        }

        // Liste
        if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string))
        {
            bw.Write((byte)DataType.List);
            var list = ((IEnumerable)val).Cast<object>().ToList();
            bw.Write(list.Count);

            Type elementType = type.IsGenericType ? type.GetGenericArguments()[0] : typeof(object);
            foreach (var item in list)
                writeValue(bw, item, elementType);
            return;
        }

        // komplexes Objekt
        writeObject(bw, val);
    }

    private static void writePrimitive(BinaryWriter bw, object val, Type type)
    {
        switch (Type.GetTypeCode(type))
        {
            case TypeCode.Boolean: bw.Write((bool)val); break;
            case TypeCode.Byte: bw.Write((byte)val); break;
            case TypeCode.Char: bw.Write((char)val); break;
            case TypeCode.DateTime: bw.Write(((DateTime)val).ToBinary()); break;
            case TypeCode.Decimal: bw.Write(decimal.ToDouble((decimal)val)); break;
            case TypeCode.Double: bw.Write((double)val); break;
            case TypeCode.Int16: bw.Write((short)val); break;
            case TypeCode.Int32: bw.Write((int)val); break;
            case TypeCode.Int64: bw.Write((long)val); break;
            case TypeCode.Single: bw.Write((float)val); break;
            case TypeCode.String: bw.Write((string?)val ?? string.Empty); break;
            case TypeCode.UInt16: bw.Write((ushort)val); break;
            case TypeCode.UInt32: bw.Write((uint)val); break;
            case TypeCode.UInt64: bw.Write((ulong)val); break;
            default:
                if (type.IsEnum)
                    bw.Write(Convert.ToInt32(val));
                else if (type == typeof(Guid))
                    bw.Write(((Guid)val).ToByteArray());
                else
                    throw new NotSupportedException($"Typ {type} wird nicht unterstützt.");
                break;
        }
    }
    #endregion

    #region Read
    /// <summary>
    /// Objekt aus Datei lesen
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>
    /// <param name="file">Dateiname</param>
    /// <returns>gelesenes Objekt</returns>
    public static T ReadObjectFromFile<T>(FileInfo file) where T : new()
        => ReadObjectFromBytes<T>(File.ReadAllBytes(file.FullName));

    /// <summary>
    /// Objekt aus ByteArray lesen
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>
    /// <param name="data">ByteArray, dass das Objekt enthält</param>
    /// <returns>gelesenes Objekt</returns>
    public static T ReadObjectFromBytes<T>(byte[] data) where T : new()
    {
        using var ms = new MemoryStream(data, false);
        using var br = new BinaryReader(ms);
        return (T)readValue(br, typeof(T))!;
    }

    private static object? readValue(BinaryReader br, Type expectedType)
    {
        DataType typeCode = (DataType)br.ReadByte();

        switch (typeCode)
        {
            case DataType.Null:
                return null;

            case DataType.Primitive:
                string typeName = br.ReadString();
                Type t = Type.GetType(typeName) ?? expectedType;
                return readPrimitive(br, t);

            case DataType.ByteArray:
                int len = br.ReadInt32();
                return br.ReadBytes(len);

            case DataType.Image:
                int imgLen = br.ReadInt32();
                byte[] data = br.ReadBytes(imgLen);
                using (var ms = new MemoryStream(data))
                    return Image.FromStream(ms);

            case DataType.List:
                int count = br.ReadInt32();
                Type elementType = expectedType.IsGenericType ? expectedType.GetGenericArguments()[0] : typeof(object);
                var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType))!;
                for (int i = 0; i < count; i++)
                    list.Add(readValue(br, elementType));
                return list;

            case DataType.Object:
                string objTypeName = br.ReadString();
                Type objType = Type.GetType(objTypeName) ?? expectedType;
                int propCount = br.ReadInt32();
                object instance = Activator.CreateInstance(objType)!;

                var props = getSerializableProperties(objType).ToDictionary(p => p.Name, p => p);
                for (int i = 0; i < propCount; i++)
                {
                    string propName = br.ReadString();
                    if (props.TryGetValue(propName, out var prop))
                    {
                        object? val = readValue(br, prop.PropertyType);
                        prop.SetValue(instance, val);
                    }
                    else
                    {
                        skipValue(br);
                    }
                }
                return instance;

            case DataType.DateOnly:
                long binDate = br.ReadInt64();
                return DateOnly.FromDateTime(DateTime.FromBinary(binDate));

            case DataType.TimeOnly:
                long ticks = br.ReadInt64();
                return new TimeOnly(ticks);

            default:
                throw new InvalidDataException($"Unbekannter Datentyp {typeCode}");
        }
    }

    private static void skipValue(BinaryReader br)
    {
        DataType code = (DataType)br.ReadByte();
        switch (code)
        {
            case DataType.Null: return;
            case DataType.Primitive:
                _ = br.ReadString(); _ = br.ReadString(); return;
            case DataType.ByteArray:
                int len = br.ReadInt32(); br.BaseStream.Seek(len, SeekOrigin.Current); return;
            case DataType.Image:
                int imgLen = br.ReadInt32(); br.BaseStream.Seek(imgLen, SeekOrigin.Current); return;
            case DataType.List:
                int count = br.ReadInt32(); for (int i = 0; i < count; i++) skipValue(br); return;
            case DataType.Object:
                _ = br.ReadString(); int propCount = br.ReadInt32();
                for (int i = 0; i < propCount; i++) { _ = br.ReadString(); skipValue(br); }
                return;
            case DataType.DateOnly:
                br.ReadInt64(); return;
            case DataType.TimeOnly:
                br.ReadInt64(); return;
            default:
                throw new InvalidDataException("Ungültiger Datentyp beim Überspringen.");
        }
    }

    private static object readPrimitive(BinaryReader br, Type type)
    {
        switch (Type.GetTypeCode(type))
        {
            case TypeCode.Boolean: return br.ReadBoolean();
            case TypeCode.Byte: return br.ReadByte();
            case TypeCode.Char: return br.ReadChar();
            case TypeCode.DateTime: return DateTime.FromBinary(br.ReadInt64());
            case TypeCode.Decimal: return (decimal)br.ReadDouble();
            case TypeCode.Double: return br.ReadDouble();
            case TypeCode.Int16: return br.ReadInt16();
            case TypeCode.Int32: return br.ReadInt32();
            case TypeCode.Int64: return br.ReadInt64();
            case TypeCode.Single: return br.ReadSingle();
            case TypeCode.String: return br.ReadString();
            case TypeCode.UInt16: return br.ReadUInt16();
            case TypeCode.UInt32: return br.ReadUInt32();
            case TypeCode.UInt64: return br.ReadUInt64();
            default:
                if (type.IsEnum) return Enum.ToObject(type, br.ReadInt32());
                if (type == typeof(Guid)) return new Guid(br.ReadBytes(16));
                throw new NotSupportedException($"Typ {type} wird nicht unterstützt.");
        }
    }
#endregion

    /// <summary>
    /// Typen-Cache zurücksetzen
    /// </summary>
    public static void ResetTypeCache() { PropertyCache.Clear(); }    

    private static PropertyInfo[] getSerializableProperties(Type t)
    {
        return PropertyCache.GetOrAdd(t, type =>
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.GetCustomAttribute<BinaryIgnoreAttribute>() == null)
                .ToArray());
    }

    private static bool isSimpleType(Type t)
    {
        return t.IsPrimitive || t.IsEnum || t == typeof(string)
            || t == typeof(decimal) || t == typeof(DateTime)
            || t == typeof(Guid);
    }

    private enum DataType : byte
    {
        Null = 0,
        Primitive = 1,
        Image = 2,
        List = 3,
        Object = 4,
        ByteArray = 5,
        DateOnly = 6,
        TimeOnly = 7
    }
}
