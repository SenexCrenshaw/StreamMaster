using System.Security.Cryptography;

namespace StreamMaster.Domain.Crypto
{
    public static class KeyGenerator
    {
        /// <summary>
        /// Generates a cryptographically secure random key of the specified length.
        /// </summary>
        /// <param name="keySize">The size of the key in bits. Default is 128 bits.</param>
        /// <returns>A Base64-encoded string representing the generated key.</returns>
        public static string GenerateKey(int keySize = AesEncryption.DEFAULT_KEY_SIZE)
        {
            int keyBytesSize = keySize / 8;
            byte[] keyBytes = new byte[keyBytesSize];

            RandomNumberGenerator.Fill(keyBytes);

            return Convert.ToBase64String(keyBytes);
        }
    }
}
