using System.Security.Cryptography;
using System.Text;

namespace AF.CORE;

/// <summary>
/// Ver- und Entschlüsselung mit AES256
/// </summary>
public class CryptoAES256
{
    private readonly byte[] _aesKey;
    private readonly byte[] _aesIV;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="secret"></param>
    public CryptoAES256(string secret)
    {
        byte[] passwordBytes = Encoding.UTF8.GetBytes(secret);
#if NET481_OR_GREATER
        _aesKey = SHA256.Create().ComputeHash(passwordBytes);
        _aesIV = MD5.Create().ComputeHash(passwordBytes);
#else
        _aesKey = SHA256.HashData(passwordBytes);
        _aesIV = MD5.HashData(passwordBytes);
#endif
    }

    /// <summary> Verschlüsselung eines byte[] mit AES. </summary>
    /// <param name="clearData"> Zu verschlüsselnde Daten </param>
    /// <returns> die verschlüsselten Daten </returns>
    public byte[] Encrypt(byte[] clearData)
    {
        if (_aesKey.Length < 1) throw new Exception(CoreStrings.ERR_AFYPT_MISSINGKEY);

        using var aes = Aes.Create();
        aes.Key = _aesKey;
        aes.IV = _aesIV;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        return _PerformCryptography(clearData, encryptor);
    }

    /// <summary>
    /// Verschlüsselt die Zeichenkette mit Crypto.Encrypt.
    /// </summary>
    /// <param name="source">zu verschlüsselnder Text</param>.
    /// <returns>verschlüsselte Zeichenkette</returns>
    public string EncryptString(string source)
    {
        return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(source)));
    }

    /// <summary>
    /// Entschlüsselung einer mit AES verschlüsselten Zeichenkette. 
    /// </summary>
    /// <param name="source">verschlüsselte Zeichenkette</param>.
    /// <returns>Entschlüsselter String</returns>
    public string DecryptString(string source)
    {
        return DecryptString(Convert.FromBase64String(source));
    }

    /// <summary> 
    /// Entschlüsselung eines mit AES verschlüsselten byte[]. 
    /// 
    /// ACHTUNG! Vor dem Entschlüsseln sicherstellen, dass CreateKey aufgerufen wurde!
    /// </summary>
    /// <param name="encryptedData"> die verschlüsselten Daten </param>
    /// <returns> die entschlüsselten Daten </returns>
    public byte[] Decrypt(byte[] encryptedData)
    {
        if (_aesKey.Length < 1) throw new Exception(CoreStrings.ERR_AFYPT_MISSINGKEY);

        using (var aes = Aes.Create())
        {
            aes.Key = _aesKey;
            aes.IV = _aesIV;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            return _PerformCryptography(encryptedData, decryptor);
        }
    }

    /// <summary>
    /// Entschlüsselt eine Zeichenkette aus einem Byte[] mit Crypto.Decrypt.
    /// </summary>
    /// <param name="data">verschlüsselte Daten</param>
    /// <returns>Entschlüsselte Daten als String</returns>
    public string DecryptString(byte[] data)
    {
        return Encoding.UTF8.GetString(Decrypt(data));
    }

    /// <summary>
    /// Entschlüsselt ein Byte[] mit Crypto.Decrypt.
    /// </summary>
    /// <param name="data">verschlüsselte Daten</param>
    /// <returns>Entschlüsselte Daten als byte[]</returns>
    public byte[] DecryptArray(byte[] data)
    {
        return Decrypt(data);
    }

    private byte[] _PerformCryptography(byte[] data, ICryptoTransform cryptoTransform)
    {
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write);
        cs.Write(data, 0, data.Length);
        cs.FlushFinalBlock();

        return ms.ToArray();
    }
}

/// <summary>
/// Verschlüsselungsroutinen für die Verschlüsselung mit AES256.
/// </summary>
public static class CryptoAES256Static
{
    private static byte[] _aesKey = [];
    private static byte[] _aesIV = [];

    /// <summary>
    /// Erzeugt den für die Verschlüsselung/Entschlüsselung erforderlichen Schlüssel.
    /// Alle auf den Aufruf folgenden Ver- und Entschlüsselungsprozesse 
    /// verwenden den erzeugten Schlüssel.
    /// </summary>
    /// <param name="password">Kennwort</param>
    public static void CreateKey(string password)
    {
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
#if NET481_OR_GREATER
        _aesKey = SHA256.Create().ComputeHash(passwordBytes);
        _aesIV = MD5.Create().ComputeHash(passwordBytes);
#else
        _aesKey = SHA256.HashData(passwordBytes);
        _aesIV = MD5.HashData(passwordBytes);
#endif
    }

    /// <summary> Verschlüsselung eines byte[] mit AES. </summary>
    /// <param name="clearData"> Zu verschlüsselnde Daten </param>
    /// <returns> die verschlüsselten Daten </returns>
    public static byte[] Encrypt(byte[] clearData)
    {
        if (clearData == null) throw new ArgumentNullException(nameof(clearData));

        if (_aesKey.Length < 1) throw new Exception(CoreStrings.ERR_AFYPT_MISSINGKEY);

        using (var aes = Aes.Create())
        {
            aes.Key = _aesKey;
            aes.IV = _aesIV;

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV)) return _PerformCryptography(clearData, encryptor);
        }
    }

    /// <summary>
    /// Verschlüsselt die Zeichenkette mit Crypto.Encrypt.
    /// </summary>
    /// <param name="source">zu verschlüsselnder Text</param>.
    /// <returns>verschlüsselte Zeichenkette</returns>
    public static string EncryptString(this string source)
    {
        return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(source)));
    }

    /// <summary>
    /// Entschlüsselung einer mit AES verschlüsselten Zeichenkette. 
    /// </summary>
    /// <param name="source">verschlüsselte Zeichenkette</param>.
    /// <returns>Entschlüsselter String</returns>
    public static string DecryptString(this string source)
    {
        return Convert.FromBase64String(source).DecryptString();
    }

    /// <summary> 
    /// Entschlüsselung eines mit AES verschlüsselten byte[]. 
    /// 
    /// ACHTUNG! Vor dem Entschlüsseln sicherstellen, dass CreateKey aufgerufen wurde!
    /// </summary>
    /// <param name="encryptedData"> die verschlüsselten Daten </param>
    /// <returns> die entschlüsselten Daten </returns>
    public static byte[] Decrypt(byte[] encryptedData)
    {
        if (encryptedData == null) throw new ArgumentNullException(nameof(encryptedData));

        if (_aesKey.Length < 1) throw new Exception(CoreStrings.ERR_AFYPT_MISSINGKEY);

        using (var aes = Aes.Create())
        {
            aes.Key = _aesKey;
            aes.IV = _aesIV;

            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV)) return _PerformCryptography(encryptedData, decryptor);
        }
    }

    private static byte[] _PerformCryptography(byte[] data, ICryptoTransform cryptoTransform)
    {
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write);
        cs.Write(data, 0, data.Length);
        cs.FlushFinalBlock();

        return ms.ToArray();
    }

    /// <summary>
    /// Verschlüsselt die Zeichenkette mit Crypto.Encrypt.
    /// </summary>
    /// <param name="source">zu verschlüsselnder Text</param>.
    /// <returns>verschlüsselter Text als byte[]</returns>
    public static byte[] Encrypt(this string source)
    {
        return Encrypt(Encoding.UTF8.GetBytes(source));
    }

    /// <summary>
    /// Entschlüsselt eine Zeichenkette aus einem Byte[] mit Crypto.Decrypt.
    /// </summary>
    /// <param name="data">verschlüsselte Daten</param>
    /// <returns>Entschlüsselte Daten als String</returns>
    public static string DecryptString(this byte[] data)
    {
        return Encoding.UTF8.GetString(Decrypt(data));
    }

    /// <summary>
    /// Entschlüsselt ein Byte[] mit Crypto.Decrypt.
    /// </summary>
    /// <param name="data">verschlüsselte Daten</param>
    /// <returns>Entschlüsselte Daten als byte[]</returns>
    public static byte[] DecryptArray(this byte[] data)
    {
        return Decrypt(data);
    }
}


