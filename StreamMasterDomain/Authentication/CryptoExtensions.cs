using System.Security.Cryptography;
using System.Text;

namespace StreamMasterDomain.Authentication
{
    public static class CryptoExtension
    {
        private const int HMACSize = 32;

        public static int? DecodeValue(this string valueKey, string serverKey, int keySize = 128)
        {
            string? decodedValue = DecodeValues(valueKey, serverKey, keySize);
            return int.TryParse(decodedValue, out int result) ? result : null;
        }

        public static int? DecodeValue128(this string valueKey, string serverKey)
        {
            return DecodeValue(valueKey, serverKey, 128);
        }

        public static (int?, int?) DecodeValues128(this string valueKey, string serverKey)
        {
            return DecodeTwoValues(valueKey, serverKey, 128);
        }

        public static (int?, int?) DecodeValues192(this string valueKey, string serverKey)
        {
            return DecodeTwoValues(valueKey, serverKey, 192);
        }

        public static (int?, int?) DecodeValues256(this string valueKey, string serverKey)
        {
            return DecodeTwoValues(valueKey, serverKey, 256);
        }
        public static string EncodeValue128(this int value1, string serverKey)
        {
            return EncodeValue(value1, serverKey, 128);
        }

        public static string EncodeValue192(this int value1, string serverKey)
        {
            return EncodeValue(value1, serverKey, 192);
        }

        public static string EncodeValue256(this int value1, string serverKey)
        {
            return EncodeValue(value1, serverKey, 256);
        }
        public static string EncodeValues128(this int value1, int value2, string serverKey)
        {
            return EncodeValues(value1, value2, serverKey, 128);
        }

        public static string EncodeValues192(this int value1, int value2, string serverKey)
        {
            return EncodeValues(value1, value2, serverKey, 192 );
        }

        public static string EncodeValues256(this int value1, int value2, string serverKey)
        {
            return EncodeValues(value1, value2, serverKey, 256);
        }

        private static byte[] CalculateHMAC(byte[] data, byte[] key)
        {
            using var hmac = new HMACSHA256(key);
            return hmac.ComputeHash(data);
        }

        // Private helper methods
        private static bool ConstantTimeComparison(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;

            int result = 0;
            for (int i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }

            return result == 0;
        }

        private static (int?, int?) DecodeTwoValues(this string valueKey, string serverKey, int keySize = 128)
        {
            string? decodedValue = DecodeValues(valueKey, serverKey, keySize);
            if (!string.IsNullOrEmpty(decodedValue))
            {
                string[] parts = decodedValue.ToString().Split('|');
                if (parts.Length == 2 && int.TryParse(parts[0], out int value1) && int.TryParse(parts[1], out int value2))
                    return (value1, value2);
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
                    .PadRight(valueKey.Length + (4 - valueKey.Length % 4) % 4, '=');
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
            using var aes = Aes.Create();
            aes.Key = serverKey;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.IV = iv;

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(encryptedValue, 0, encryptedValue.Length);
                cs.FlushFinalBlock();
            }
            byte[] decryptedBytes = ms.ToArray();
            return Encoding.UTF8.GetString(decryptedBytes);
        }
        public static string EncodeValue(this int value1, string serverKey, int keySize = 128)
        {
            byte[] serverKeyBytes = GenerateKey(serverKey, keySize);
            string valueString =value1.ToString();
            byte[] valueBytes = Encoding.UTF8.GetBytes(valueString);
            byte[] encryptedBytes = EncryptValue(valueBytes, serverKeyBytes);
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

        public static string EncodeValues(this int value1, int value2 , string serverKey, int keySize = 128)
        {
            byte[] serverKeyBytes = GenerateKey(serverKey, keySize);
            string valueString = $"{value1}|{value2}";
            byte[] valueBytes = Encoding.UTF8.GetBytes(valueString);
            byte[] encryptedBytes = EncryptValue(valueBytes, serverKeyBytes);
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

        private static byte[] EncryptValue(byte[] valueBytes, byte[] serverKey)
        {
            using var aes = Aes.Create();
            aes.Key = serverKey;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Generate a random IV
            aes.GenerateIV();
            byte[] iv = aes.IV;

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
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
            using var sha = SHA256.Create();
            var keyBytes = Encoding.UTF8.GetBytes(serverKey);
            var hash = sha.ComputeHash(keyBytes);

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
}
