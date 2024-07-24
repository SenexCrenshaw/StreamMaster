using System.Security.Cryptography;

namespace StreamMaster.Domain.Crypto
{
    public static class AesEncryption
    {
        public const int HMACSize = 16; // 128 bits for shorter HMAC
        public const int DEFAULT_KEY_SIZE = 128;

        /// <summary>
        /// Encrypts the specified plain text using AES and appends an HMAC for integrity.
        /// The result is returned as a URL-safe Base64 string.
        /// </summary>
        /// <param name="plainText">The text to encrypt.</param>
        /// <param name="key">The key for encryption. Must be at least as long as specified by <paramref name="keySize"/>.</param>
        /// <param name="keySize">The size of the encryption key in bits. Default is 128 bits.</param>
        /// <returns>A URL-safe Base64 string containing the encrypted data and HMAC.</returns>
        public static string Encrypt(string plainText, string key, int keySize = DEFAULT_KEY_SIZE)
        {
            byte[] keyBytes = CryptoHelpers.GetKeyBytes(key, keySize);

            using Aes aesAlg = Aes.Create();
            aesAlg.KeySize = keySize;
            aesAlg.Key = keyBytes;
            aesAlg.IV = new byte[16]; // Initialize to zeros for simplicity

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            byte[] encryptedBytes;
            using (MemoryStream msEncrypt = new())
            {
                using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using StreamWriter swEncrypt = new(csEncrypt);
                    swEncrypt.Write(plainText);
                }
                encryptedBytes = msEncrypt.ToArray();
            }

            return encryptedBytes.ConvertToUrlSafeBase64String(keyBytes);
        }

        /// <summary>
        /// Decrypts the specified URL-safe Base64 string using AES and verifies the HMAC for integrity.
        /// </summary>
        /// <param name="cipherText">The URL-safe Base64 string containing the encrypted data and HMAC.</param>
        /// <param name="key">The key for decryption. Must be at least as long as specified by <paramref name="keySize"/>.</param>
        /// <param name="keySize">The size of the decryption key in bits. Default is 128 bits.</param>
        /// <returns>The decrypted plain text.</returns>
        /// <exception cref="CryptographicException">Thrown if HMAC validation fails.</exception>
        public static string Decrypt(string cipherText, string key, int keySize = DEFAULT_KEY_SIZE)
        {
            byte[] keyBytes = CryptoHelpers.GetKeyBytes(key, keySize);
            byte[] cipherBytes = cipherText.ConvertFromUrlSafeBase64String();

            // Ensure cipherBytes has the expected length
            if (cipherBytes.Length <= HMACSize)
            {
                throw new CryptographicException("Invalid ciphertext.");
            }

            // Extract HMAC
            byte[] hmac = new byte[HMACSize];
            Buffer.BlockCopy(cipherBytes, 0, hmac, 0, HMACSize);

            // Extract encrypted data
            byte[] encryptedData = new byte[cipherBytes.Length - HMACSize];
            Buffer.BlockCopy(cipherBytes, HMACSize, encryptedData, 0, encryptedData.Length);

            // Verify HMAC
            byte[] computedHmac = CryptoHelpers.CalculateHMAC(encryptedData, keyBytes);
            if (!CryptoHelpers.CompareBytes(hmac, computedHmac))
            {
                Console.WriteLine("HMAC validation failed.");
                Console.WriteLine("Expected HMAC: " + BitConverter.ToString(hmac));
                Console.WriteLine("Computed HMAC: " + BitConverter.ToString(computedHmac));
                throw new CryptographicException("HMAC validation failed.");
            }

            using Aes aesAlg = Aes.Create();
            aesAlg.KeySize = keySize;
            aesAlg.Key = keyBytes;
            aesAlg.IV = new byte[16]; // Initialize to zeros for simplicity

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msDecrypt = new(encryptedData);
            using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);
            return srDecrypt.ReadToEnd();
        }
    }
}
