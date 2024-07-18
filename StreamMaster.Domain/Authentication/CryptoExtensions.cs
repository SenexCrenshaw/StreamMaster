using System.Security.Cryptography;
using System.Text;

namespace StreamMaster.Domain.Authentication;

public static class CryptoExtension
{
    private const int HMACSize = 32;

    public static (int?, string?) DecodeTwoValuesAsString(this string valueKey, string serverKey, int keySize = 128)
    {
        string? decodedValue = valueKey.DecodeValues(serverKey, keySize);
        if (!string.IsNullOrEmpty(decodedValue))
        {
            string[] parts = decodedValue.ToString().Split('|');
            if (parts.Length == 2 && int.TryParse(parts[0], out int value1))
            {
                return (value1, parts[1]);
            }
        }

        return (null, null);
    }

    public static (int?, string?) DecodeTwoValuesAsString128(this string valueKey, string serverKey)
    {
        return valueKey.DecodeTwoValuesAsString(serverKey, 128);
    }

    public static (int?, string?) DecodeTwoValuesAsString192(this string valueKey, string serverKey)
    {
        return valueKey.DecodeTwoValuesAsString(serverKey, 192);
    }

    public static (int?, string?) DecodeTwoValuesAsString256(this string valueKey, string serverKey)
    {
        return valueKey.DecodeTwoValuesAsString(serverKey, 256);
    }

    public static int? DecodeValue(this string valueKey, string serverKey, int keySize = 128)
    {
        string? decodedValue = valueKey.DecodeValues(serverKey, keySize);
        return int.TryParse(decodedValue, out int result) ? result : null;
    }

    public static int? DecodeValue128(this string valueKey, string serverKey)
    {
        return valueKey.DecodeValue(serverKey, 128);
    }

    public static string? DecodeValueAsString(this string valueKey, string serverKey, int keySize = 128)
    {
        string? decodedValue = valueKey.DecodeValues(serverKey, keySize);
        return decodedValue;
    }

    public static string? DecodeValueAsString128(this string valueKey, string serverKey)
    {
        return valueKey.DecodeValueAsString(serverKey, 128);
    }

    public static string? DecodeValueAsString192(this string valueKey, string serverKey)
    {
        return valueKey.DecodeValueAsString(serverKey, 192);
    }

    public static string? DecodeValueAsString256(this string valueKey, string serverKey)
    {
        return valueKey.DecodeValueAsString(serverKey, 256);
    }

    public static (int?, int?) DecodeValues128(this string valueKey, string serverKey)
    {
        return valueKey.DecodeTwoValues(serverKey, 128);
    }

    public static (int?, int?) DecodeValues192(this string valueKey, string serverKey)
    {
        return valueKey.DecodeTwoValues(serverKey, 192);
    }

    public static (int?, int?) DecodeValues256(this string valueKey, string serverKey)
    {
        return valueKey.DecodeTwoValues(serverKey, 256);
    }

    public static string EncodeValue(this string value1, string serverKey, int keySize = 128, byte[]? iv = null)
    {
        byte[] serverKeyBytes = GenerateKey(serverKey, keySize);
        byte[] valueBytes = Encoding.UTF8.GetBytes(value1);
        byte[] encryptedBytes = EncryptValue(valueBytes, serverKeyBytes, iv);
        byte[] hmacBytes = CalculateHMAC(encryptedBytes, serverKeyBytes);
        byte[] encodedBytes = new byte[encryptedBytes.Length + HMACSize];
        Buffer.BlockCopy(hmacBytes, 0, encodedBytes, 0, HMACSize);
        Buffer.BlockCopy(encryptedBytes, 0, encodedBytes, HMACSize, encryptedBytes.Length);
        string encodedUrlSafeString = Convert.ToBase64String(encodedBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');

        return encodedUrlSafeString;
    }

    public static string EncodeValue(this int value1, string serverKey, int keySize = 128, byte[]? iv = null)
    {
        byte[] serverKeyBytes = GenerateKey(serverKey, keySize);
        string valueString = value1.ToString();
        byte[] valueBytes = Encoding.UTF8.GetBytes(valueString);
        byte[] encryptedBytes = EncryptValue(valueBytes, serverKeyBytes, iv);
        byte[] hmacBytes = CalculateHMAC(encryptedBytes, serverKeyBytes);
        byte[] encodedBytes = new byte[encryptedBytes.Length + HMACSize];
        Buffer.BlockCopy(hmacBytes, 0, encodedBytes, 0, HMACSize);
        Buffer.BlockCopy(encryptedBytes, 0, encodedBytes, HMACSize, encryptedBytes.Length);
        string encodedUrlSafeString = Convert.ToBase64String(encodedBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');

        return encodedUrlSafeString;
    }

    public static string EncodeValue128(this string value1, string serverKey)
    {
        return value1.EncodeValue(serverKey, 128);
    }

    public static string EncodeValue128(this string value1, string serverKey, byte[]? iv)
    {
        return value1.EncodeValue(serverKey, 128, iv);
    }

    public static string EncodeValue128(this int value1, string serverKey)
    {
        return value1.EncodeValue(serverKey, 128);
    }

    public static string EncodeValue128(this int value1, string serverKey, byte[]? iv)
    {
        return value1.EncodeValue(serverKey, 128, iv);

    }
    public static string EncodeValue192(this string value1, string serverKey)
    {
        return value1.EncodeValue(serverKey, 192);
    }

    public static string EncodeValue192(this int value1, string serverKey)
    {
        return value1.EncodeValue(serverKey, 192);
    }

    public static string EncodeValue256(this string value1, string serverKey)
    {
        return value1.EncodeValue(serverKey, 256);
    }

    public static string EncodeValue256(this int value1, string serverKey)
    {
        return value1.EncodeValue(serverKey, 256);
    }

    public static string EncodeValues(this int value1, int value2, string serverKey, int keySize = 128, byte[]? iv = null)
    {
        byte[] serverKeyBytes = GenerateKey(serverKey, keySize);
        string valueString = $"{value1}|{value2}";
        byte[] valueBytes = Encoding.UTF8.GetBytes(valueString);
        byte[] encryptedBytes = EncryptValue(valueBytes, serverKeyBytes, iv);
        byte[] hmacBytes = CalculateHMAC(encryptedBytes, serverKeyBytes);
        byte[] encodedBytes = new byte[encryptedBytes.Length + HMACSize];
        Buffer.BlockCopy(hmacBytes, 0, encodedBytes, 0, HMACSize);
        Buffer.BlockCopy(encryptedBytes, 0, encodedBytes, HMACSize, encryptedBytes.Length);
        string encodedUrlSafeString = Convert.ToBase64String(encodedBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');

        return encodedUrlSafeString;
    }

    public static string EncodeValues(this int value1, string value2, string serverKey, int keySize = 128, byte[]? iv = null)
    {
        byte[] serverKeyBytes = GenerateKey(serverKey, keySize);
        string valueString = $"{value1}|{value2}";
        byte[] valueBytes = Encoding.UTF8.GetBytes(valueString);
        byte[] encryptedBytes = EncryptValue(valueBytes, serverKeyBytes, iv);
        byte[] hmacBytes = CalculateHMAC(encryptedBytes, serverKeyBytes);
        byte[] encodedBytes = new byte[encryptedBytes.Length + HMACSize];
        Buffer.BlockCopy(hmacBytes, 0, encodedBytes, 0, HMACSize);
        Buffer.BlockCopy(encryptedBytes, 0, encodedBytes, HMACSize, encryptedBytes.Length);
        string encodedUrlSafeString = Convert.ToBase64String(encodedBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');

        return encodedUrlSafeString;
    }

    public static string EncodeValues128(this int value1, string value2, string serverKey, byte[]? iv = null)
    {
        return value1.EncodeValues(value2, serverKey, 128, iv);
    }

    public static string EncodeValues128(this int value1, int value2, string serverKey, byte[]? iv = null)
    {
        return value1.EncodeValues(value2, serverKey, 128, iv);
    }

    public static string EncodeValues192(this int value1, string value2, string serverKey)
    {
        return value1.EncodeValues(value2, serverKey, 192);
    }

    public static string EncodeValues192(this int value1, int value2, string serverKey)
    {
        return value1.EncodeValues(value2, serverKey, 192);
    }

    public static string EncodeValues256(this int value1, string value2, string serverKey)
    {
        return value1.EncodeValues(value2, serverKey, 256);
    }

    public static string EncodeValues256(this int value1, int value2, string serverKey)
    {
        return value1.EncodeValues(value2, serverKey, 256);
    }

    public static string? GetAPIKeyFromPath(this string requestPath, string serverKey, int keySize = 128)
    {
        return requestPath.GetIVFromPath(serverKey) == null ? null : serverKey;
    }



    public static byte[]? GetIVFromPath(this string requestPath, string serverKey, int keySize = 128)
    {
        try
        {
            if (
            !requestPath.StartsWith("/api/videostreams/", StringComparison.InvariantCultureIgnoreCase)
            &&
            !requestPath.StartsWith("/api/streamgroups/", StringComparison.InvariantCultureIgnoreCase)
            )
            {
                return null;
            }

            string crypt = requestPath.Replace("/api/videostreams/stream/", "", StringComparison.InvariantCultureIgnoreCase);
            crypt = crypt.Replace("/api/streamgroups/stream/", "", StringComparison.InvariantCultureIgnoreCase);
            crypt = crypt.Replace("/api/videostreams/", "", StringComparison.InvariantCultureIgnoreCase);
            crypt = crypt.Replace("/api/streamgroups/", "", StringComparison.InvariantCultureIgnoreCase);


            if (crypt.Contains("/"))
            {
                crypt = crypt[..crypt.IndexOf("/")];
            }

            byte[] serverKeyBytes = GenerateKey(serverKey, keySize);
            string base64String = crypt
                .Replace('-', '+')
                .Replace('_', '/')
                .PadRight(crypt.Length + ((4 - (crypt.Length % 4)) % 4), '=');
            byte[] encodedBytes = Convert.FromBase64String(base64String);
            byte[] hmacBytes = new byte[HMACSize];
            byte[] encryptedBytes = new byte[encodedBytes.Length - HMACSize];
            Buffer.BlockCopy(encodedBytes, 0, hmacBytes, 0, HMACSize);
            Buffer.BlockCopy(encodedBytes, HMACSize, encryptedBytes, 0, encryptedBytes.Length);

            bool isVerified = VerifyHMAC(encryptedBytes, hmacBytes, serverKeyBytes);
            if (!isVerified)
            {
                // HMAC verification failed
                return null;
            }

            // Extract the IV from the encrypted value
            byte[] iv = new byte[16];
            Buffer.BlockCopy(encryptedBytes, 0, iv, 0, iv.Length);

            return iv;
        }
        catch
        {
            // Failed to decode properly
        }

        return null;
    }

    private static byte[] CalculateHMAC(byte[] data, byte[] key)
    {
        using HMACSHA256 hmac = new(key);
        return hmac.ComputeHash(data);
    }

    // Private helper methods
    private static bool ConstantTimeComparison(byte[] a, byte[] b)
    {
        if (a.Length != b.Length)
        {
            return false;
        }

        int result = 0;
        for (int i = 0; i < a.Length; i++)
        {
            result |= a[i] ^ b[i];
        }

        return result == 0;
    }

    private static (int?, int?) DecodeTwoValues(this string valueKey, string serverKey, int keySize = 128)
    {
        string? decodedValue = valueKey.DecodeValues(serverKey, keySize);
        if (!string.IsNullOrEmpty(decodedValue))
        {
            string[] parts = decodedValue.ToString().Split('|');
            if (parts.Length == 2 && int.TryParse(parts[0], out int value1) && int.TryParse(parts[1], out int value2))
            {
                return (value1, value2);
            }
        }

        return (null, null);
    }

    private static string? DecodeValues(this string valueKey, string serverKey, int keySize)
    {
        try
        {
            byte[] serverKeyBytes = GenerateKey(serverKey, keySize);
            string base64String = valueKey
                .Replace('-', '+')
                .Replace('_', '/')
                .PadRight(valueKey.Length + ((4 - (valueKey.Length % 4)) % 4), '=');
            byte[] encodedBytes = Convert.FromBase64String(base64String);
            byte[] hmacBytes = new byte[HMACSize];
            byte[] encryptedBytes = new byte[encodedBytes.Length - HMACSize];
            Buffer.BlockCopy(encodedBytes, 0, hmacBytes, 0, HMACSize);
            Buffer.BlockCopy(encodedBytes, HMACSize, encryptedBytes, 0, encryptedBytes.Length);

            bool isVerified = VerifyHMAC(encryptedBytes, hmacBytes, serverKeyBytes);
            if (!isVerified)
            {
                // HMAC verification failed
                return null;
            }

            // Extract the IV from the encrypted value
            byte[] iv = new byte[16];
            Buffer.BlockCopy(encryptedBytes, 0, iv, 0, iv.Length);

            byte[] encryptedData = new byte[encryptedBytes.Length - iv.Length];
            Buffer.BlockCopy(encryptedBytes, iv.Length, encryptedData, 0, encryptedData.Length);

            return DecryptValues(encryptedData, serverKeyBytes, iv);
        }
        catch
        {
            // Failed to decode properly
        }

        return null;
    }

    private static string? DecryptValues(byte[] encryptedValue, byte[] serverKey, byte[] iv)
    {
        using Aes aes = Aes.Create();
        aes.Key = serverKey;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.IV = iv;

        using MemoryStream ms = new();
        using (CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
        {
            cs.Write(encryptedValue, 0, encryptedValue.Length);
            cs.FlushFinalBlock();
        }
        byte[] decryptedBytes = ms.ToArray();
        return Encoding.UTF8.GetString(decryptedBytes);
    }

    private static byte[] EncryptValue(byte[] valueBytes, byte[] serverKey, byte[]? isvKey = null)
    {
        using Aes aes = Aes.Create();
        aes.Key = serverKey;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        if (isvKey is not null)
        {
            aes.IV = isvKey;
        }
        else
        {
            // Generate a random IV
            // aes.GenerateIV();
            aes.IV = Encoding.UTF8.GetBytes("STREAMMASTER1234");
        }

        byte[] iv = aes.IV;

        using MemoryStream ms = new();
        using (CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
        {
            // Write the IV as the first 16 bytes in the encrypted value
            ms.Write(iv, 0, iv.Length);
            cs.Write(valueBytes, 0, valueBytes.Length);
            cs.FlushFinalBlock();
        }
        return ms.ToArray();
    }

    private static byte[] GenerateKey(string serverKey, int keySize)
    {
        using SHA256 sha = SHA256.Create();
        byte[] keyBytes = Encoding.UTF8.GetBytes(serverKey);
        byte[] hash = sha.ComputeHash(keyBytes);

        byte[] sizedKey = new byte[keySize / 8];
        Array.Copy(hash, sizedKey, sizedKey.Length);
        return sizedKey;
    }

    private static bool VerifyHMAC(byte[] data, byte[] hmac, byte[] key)
    {
        byte[] calculatedHmac = CalculateHMAC(data, key);
        return ConstantTimeComparison(hmac, calculatedHmac);
    }
}
