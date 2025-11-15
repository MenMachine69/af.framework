#if FRAMEWORK
using System;
#endif

namespace AF.CORE;

/// <summary>
/// Ein Array einzelner Bit's, die gesetzt und gelesen werden können.
/// </summary>
public class AFBitArray
{
    private readonly byte[] _data;
    private readonly int _length;

    /// <summary>
    /// Initialisiert eine neue Instanz der Klasse <see cref="AFBitArray"/> mit den angegebenen Daten.
    /// </summary>
    /// <param name="data">Die Daten, mit denen das <see cref="AFBitArray"/> initialisiert wird.</param>
    public AFBitArray(byte[] data)
    {
        _data = data;
        _length = data.Length * 8;
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der Klasse <see cref="AFBitArray"/> mit den angegebenen Daten.
    /// </summary>
    /// <param name="length">Anzahl der Bytes (wenn nicht durch 8 teilbar, wird aufgerundet)</param>
    public AFBitArray(int length)
    {
        if (length % 8 != 0) 
            throw new ArgumentException(CoreStrings.ERR_MUSTMULTIPLEOFEIGHT, nameof(length));

        _data = new byte[length/8];

        // init with false/not set for each bit...
        for (int i = 0; i <= MaxIndex; i++)
            SetBit(i, false);

        _length = length;
    }

    /// <summary>
    /// Setzt alle Bits auf den Wert
    /// </summary>
    /// <param name="value">zu setzender Wert</param>
    public void SetAll(bool value)
    {
        for (int i = 0; i <= MaxIndex; i++)
            SetBit(i, value);
    }

    /// <summary>
    /// Setzt den Wert des Bits am angegebenen Index.
    /// </summary>
    /// <param name="index">Der nullbasierte Index des zu setzenden Bits.</param>
    /// <param name="value">Der Wert, auf den das Bit gesetzt werden soll. Wenn <c>true</c>, wird das Bit auf 1 gesetzt. Wenn <c>false</c>, wird das Bit auf 0 gesetzt.</param>
    /// <exception cref="IndexOutOfRangeException">Wird ausgelöst, wenn der Index kleiner als <see cref="MinIndex"/> oder größer als <see cref="MaxIndex"/> ist.</exception>
    public void SetBit(int index, bool value)
    {
        if (index < MinIndex || index > MaxIndex) throw new IndexOutOfRangeException();

        int byteIndex = index / 8;
        int bitIndex = index % 8;
        if (value)
            _data[byteIndex] |= (byte)(1 << bitIndex);
        else
            _data[byteIndex] &= (byte)~(1 << bitIndex);
    }

    /// <summary>
    /// Ruft den Wert des Bits am angegebenen Index ab.
    /// </summary>
    /// <param name="index">Der Null-basierte Index des abzurufenden Bits.</param>
    /// <returns>Der Wert des Bits am angegebenen Index. Wenn das Bit 1 ist, wird <c>true</c> zurückgegeben. Wenn das Bit 0 ist, wird <c>false</c> zurückgegeben.</returns>
    /// <exception cref="IndexOutOfRangeException">Wird ausgelöst, wenn der Index kleiner als <see cref="MinIndex"/> oder größer als <see cref="MaxIndex"/> ist.</exception>        
    public bool GetBit(int index)
    {

        if (index < MinIndex || index > MaxIndex) throw new IndexOutOfRangeException();

        int byteIndex = index / 8;
        int bitIndex = index % 8;
        return (_data[byteIndex] & (1 << bitIndex)) != 0;
    }

    /// <summary>
    /// Ermittelt den minimalen gültigen Index für das <see cref="AFBitArray"/>.
    /// </summary>
    public int MinIndex => 0;

    /// <summary>
    /// Ermittelt den maximal gültigen Index für das <see cref="AFBitArray"/>.
    /// </summary>
    public int MaxIndex => _length - 1;

    /// <summary>
    /// Ruft eine Nur-Lese-Kopie der Daten für das <see cref="AFBitArray"/> ab.
    /// </summary>
    public byte[] Data => (byte[])_data.Clone();

    /// <summary>
    /// Kombiniert die Daten des BitArrays mit den daten eines zweiten Array's via OR.
    /// 
    /// Das übergebene Array muss die gleiche Länge haben (Storage.Length = data.Lenght).
    /// </summary>
    /// <param name="data">zu kombinierede Daten</param>
    /// <exception cref="ArgumentException">Ausnahme die ausgelöst wird, wenn das übergeben Array 
    /// nicht die gleich Länge wie das aktuelle Array hat.</exception>
    public void CombineWith(byte[] data)
    {
        if (_data.Length != data.Length)
            throw new ArgumentException(CoreStrings.ERR_WRONGARRAYLENGTH);

        for (int i = 0; i < _data.Length; i++) _data[i] = (byte)(_data[i] | data[i]);
    }
}

