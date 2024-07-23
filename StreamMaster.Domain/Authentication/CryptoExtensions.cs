//using System.Security.Cryptography;
//using System.Text;

//namespace StreamMaster.Domain.Authentication;
//public static class CryptoExtension2
//{
//    public const int HMACSize = 32;
//    private const string DefaultIV = "STREAMMASTER1234";

//    public static string EncryptGroupKey(string groupKey, string serverKey, int keySize = 128)
//    {
//        byte[] serverKeyBytes = GenerateKey(serverKey, keySize);
//        byte[] groupKeyBytes = Encoding.UTF8.GetBytes(groupKey);
//        byte[] encryptedGroupKeyBytes = EncryptValue(groupKeyBytes, serverKeyBytes);
//        return ConvertToUrlSafeBase64String(encryptedGroupKeyBytes, serverKeyBytes);
//    }

//    public static string DecryptGroupKey(string encryptedGroupKey, string serverKey, int keySize = 128)
//    {
//        byte[] serverKeyBytes = GenerateKey(serverKey, keySize);
//        string base64String = encryptedGroupKey
//            .Replace('-', '+')
//            .Replace('_', '/')
//            .PadRight(encryptedGroupKey.Length + ((4 - (encryptedGroupKey.Length % 4)) % 4), '=');
//        byte[] encryptedGroupKeyBytes = Convert.FromBase64String(base64String);
//        byte[] hmacBytes = new byte[HMACSize];
//        byte[] encryptedBytes = new byte[encryptedGroupKeyBytes.Length - HMACSize];
//        Buffer.BlockCopy(encryptedGroupKeyBytes, 0, hmacBytes, 0, HMACSize);
//        Buffer.BlockCopy(encryptedGroupKeyBytes, HMACSize, encryptedBytes, 0, encryptedBytes.Length);

//        bool isVerified = VerifyHMAC(encryptedBytes, hmacBytes, serverKeyBytes);
//        if (!isVerified)
//        {
//            throw new CryptographicException("HMAC verification failed.");
//        }

//        byte[] iv = new byte[16];
//        Buffer.BlockCopy(encryptedBytes, 0, iv, 0, iv.Length);
//        byte[] encryptedData = new byte[encryptedBytes.Length - iv.Length];
//        Buffer.BlockCopy(encryptedBytes, iv.Length, encryptedData, 0, encryptedData.Length);

//        return DecryptValues(encryptedData, serverKeyBytes, iv);
//    }

//    public static string EncodeValuesWithGroupKey(this int streamGroupId, int streamGroupProfileId, string serverKey, string encryptedGroupKey, byte[]? iv = null, int keySize = 128)
//    {
//        string groupKey = DecryptGroupKey(encryptedGroupKey, serverKey, keySize);
//        byte[] groupKeyBytes = GenerateKey(groupKey, keySize);
//        string value = $"{streamGroupId}|{streamGroupProfileId}";
//        byte[] valueBytes = Encoding.UTF8.GetBytes(value);
//        byte[] encryptedBytes = EncryptValue(valueBytes, groupKeyBytes, iv);
//        return ConvertToUrlSafeBase64String(encryptedBytes, groupKeyBytes);
//    }

//    public static string EncodeValuesWithGroupKey(this int streamGroupId, string streamGroupProfileId, string smChannelId, string serverKey, string encryptedGroupKey, byte[]? iv = null, int keySize = 128)
//    {
//        string groupKey = DecryptGroupKey(encryptedGroupKey, serverKey, keySize);
//        byte[] groupKeyBytes = GenerateKey(groupKey, keySize);
//        string value = $"{streamGroupId}|{streamGroupProfileId}|{smChannelId}";
//        byte[] valueBytes = Encoding.UTF8.GetBytes(value);
//        byte[] encryptedBytes = EncryptValue(valueBytes, groupKeyBytes, iv);
//        return ConvertToUrlSafeBase64String(encryptedBytes, groupKeyBytes);
//    }

//    public static (int?, int?) DecodeRemainingValuesWithGroupKey(this string valueKey, string serverKey, string encryptedGroupKey, int keySize = 128)
//    {
//        string groupKey = DecryptGroupKey(encryptedGroupKey, serverKey, keySize);
//        byte[] groupKeyBytes = GenerateKey(groupKey, keySize);
//        string? decodedValue = valueKey.DecodeValues(groupKeyBytes, keySize);
//        string[] parts = decodedValue?.Split('|') ?? Array.Empty<string>();
//        return parts.Length >= 3 &&
//            int.TryParse(parts[1], out int value1) &&
//            int.TryParse(parts[2], out int value2)
//            ? ((int?, int?))(value1, value2)
//            : (null, null);
//    }

//    public static string EncodeValue(this int value, string serverKey, byte[]? iv = null, int keySize = 128)
//    {
//        byte[] keyBytes = GenerateKey(serverKey, keySize);
//        string stringValue = value.ToString();
//        byte[] valueBytes = Encoding.UTF8.GetBytes(stringValue);
//        byte[] encryptedBytes = EncryptValue(valueBytes, keyBytes, iv);
//        return ConvertToUrlSafeBase64String(encryptedBytes, keyBytes);
//    }

//    public static int? DecodeValue(this string encodedValue, string serverKey, int keySize = 128)
//    {
//        byte[] keyBytes = GenerateKey(serverKey, keySize);
//        string? decodedValue = encodedValue.DecodeValues(keyBytes, keySize);
//        return int.TryParse(decodedValue, out int result) ? result : null;
//    }

//    private static byte[] CalculateHMAC(byte[] data, byte[] key)
//    {
//        using HMACSHA256 hmac = new(key);
//        return hmac.ComputeHash(data);
//    }

//    private static bool ConstantTimeComparison(byte[] a, byte[] b)
//    {
//        if (a.Length != b.Length)
//        {
//            return false;
//        }

//        int result = 0;
//        for (int i = 0; i < a.Length; i++)
//        {
//            result |= a[i] ^ b[i];
//        }

//        return result == 0;
//    }

//    private static string? DecodeValues(this string valueKey, byte[] keyBytes, int keySize)
//    {
//        try
//        {
//            string base64String = valueKey
//                .Replace('-', '+')
//                .Replace('_', '/')
//                .PadRight(valueKey.Length + ((4 - (valueKey.Length % 4)) % 4), '=');
//            byte[] encodedBytes = Convert.FromBase64String(base64String);
//            byte[] hmacBytes = new byte[HMACSize];
//            byte[] encryptedBytes = new byte[encodedBytes.Length - HMACSize];
//            Buffer.BlockCopy(encodedBytes, 0, hmacBytes, 0, HMACSize);
//            Buffer.BlockCopy(encodedBytes, HMACSize, encryptedBytes, 0, encryptedBytes.Length);

//            bool isVerified = VerifyHMAC(encryptedBytes, hmacBytes, keyBytes);
//            if (!isVerified)
//            {
//                return null;
//            }

//            byte[] iv = new byte[16];
//            Buffer.BlockCopy(encryptedBytes, 0, iv, 0, iv.Length);

//            byte[] encryptedData = new byte[encryptedBytes.Length - iv.Length];
//            Buffer.BlockCopy(encryptedBytes, iv.Length, encryptedData, 0, encryptedData.Length);

//            return DecryptValues(encryptedData, keyBytes, iv);
//        }
//        catch
//        {
//            return null;
//        }
//    }

//    private static string? DecryptValues(byte[] encryptedValue, byte[] key, byte[] iv)
//    {
//        using Aes aes = Aes.Create();
//        aes.Key = key;
//        aes.Mode = CipherMode.CBC;
//        aes.Padding = PaddingMode.PKCS7;
//        aes.IV = iv;

//        using MemoryStream ms = new();
//        using (CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
//        {
//            cs.Write(encryptedValue, 0, encryptedValue.Length);
//            cs.FlushFinalBlock();
//        }
//        byte[] decryptedBytes = ms.ToArray();
//        return Encoding.UTF8.GetString(decryptedBytes);
//    }

//    private static byte[] EncryptValue(byte[] valueBytes, byte[] key, byte[]? iv = null)
//    {
//        using Aes aes = Aes.Create();
//        aes.Key = key;
//        aes.Mode = CipherMode.CBC;
//        aes.Padding = PaddingMode.PKCS7;
//        aes.IV = iv ?? Encoding.UTF8.GetBytes(DefaultIV);

//        using MemoryStream ms = new();
//        using (CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
//        {
//            ms.Write(aes.IV, 0, aes.IV.Length);
//            cs.Write(valueBytes, 0, valueBytes.Length);
//            cs.FlushFinalBlock();
//        }
//        return ms.ToArray();
//    }
//    public static byte[] GenerateKey(string key, int keySize)
//    {
//        using SHA256 sha = SHA256.Create();
//        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
//        byte[] hash = sha.ComputeHash(keyBytes);

//        byte[] sizedKey = new byte[keySize / 8];
//        Array.Copy(hash, sizedKey, sizedKey.Length);
//        return sizedKey;
//    }


//    public static bool VerifyHMAC(byte[] data, byte[] hmac, byte[] key)
//    {
//        byte[] calculatedHmac = CalculateHMAC(data, key);
//        return ConstantTimeComparison(hmac, calculatedHmac);
//    }

//    private static string ConvertToUrlSafeBase64String(byte[] encryptedBytes, byte[] keyBytes)
//    {
//        byte[] hmacBytes = CalculateHMAC(encryptedBytes, keyBytes);
//        byte[] encodedBytes = new byte[encryptedBytes.Length + HMACSize];
//        Buffer.BlockCopy(hmacBytes, 0, encodedBytes, 0, HMACSize);
//        Buffer.BlockCopy(encryptedBytes, 0, encodedBytes, HMACSize, encryptedBytes.Length);
//        string encodedUrlSafeString = Convert.ToBase64String(encodedBytes)
//            .Replace('+', '-')
//            .Replace('/', '_')
//            .TrimEnd('=');

//        return encodedUrlSafeString;
//    }
//}
