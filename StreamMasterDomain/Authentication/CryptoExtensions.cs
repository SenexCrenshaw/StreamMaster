using System.Security.Cryptography;
using System.Text;

namespace StreamMasterDomain.Authentication
{
    public static class CryptoExtension
    {
        private const int HMACSize = 32;

        public static int? DecodeValue(this string valueKey, string serverKey, int keySize)
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

                int? decodedValue = DecryptValue(encryptedBytes, serverKeyBytes);
                return decodedValue;
            }
            catch
            {
                // Failed to decode properly
            }

            return null;
        }

        public static int? DecodeValue128(this string valueKey, string serverKey)
        {
            return DecodeValue(valueKey, serverKey, 128);
        }

        public static int? DecodeValue192(this string valueKey, string serverKey)
        {
            return DecodeValue(valueKey, serverKey, 192);
        }

        public static int? DecodeValue256(this string valueKey, string serverKey)
        {
            return DecodeValue(valueKey, serverKey, 256);
        }

        public static string EncodeValue(this int value, string serverKey, int keySize)
        {
            byte[] serverKeyBytes = GenerateKey(serverKey, keySize);
            byte[] valueBytes = Encoding.UTF8.GetBytes(value.ToString());
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

        public static string EncodeValue128(this int value, string serverKey)
        {
            return EncodeValue(value, serverKey, 128);
        }

        public static string EncodeValue192(this int value, string serverKey)
        {
            return EncodeValue(value, serverKey, 192);
        }

        public static string EncodeValue256(this int value, string serverKey)
        {
            return EncodeValue(value, serverKey, 256);
        }

        private static byte[] CalculateHMAC(byte[] data, byte[] key)
        {
            using var hmac = new HMACSHA256(key);
            return hmac.ComputeHash(data);
        }

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

        private static int? DecryptValue(byte[] encryptedValue, byte[] serverKey)
        {
            using var aes = Aes.Create();
            aes.Key = serverKey;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.IV = new byte[16]; // Use a fixed IV of all zeros (not secure)

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(encryptedValue, 0, encryptedValue.Length);
                cs.FlushFinalBlock();
            }
            byte[] decryptedBytes = ms.ToArray();
            string decryptedString = Encoding.UTF8.GetString(decryptedBytes);
            if (int.TryParse(decryptedString, out int result))
                return result;

            return null;
        }

        private static byte[] EncryptValue(byte[] valueBytes, byte[] serverKey)
        {
            using var aes = Aes.Create();
            aes.Key = serverKey;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.IV = new byte[16]; // Use a fixed IV of all zeros (not secure)

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
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
