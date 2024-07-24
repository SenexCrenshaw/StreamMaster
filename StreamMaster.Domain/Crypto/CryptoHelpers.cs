using System.Security.Cryptography;
using System.Text;

namespace StreamMaster.Domain.Crypto
{
    internal static class CryptoHelpers
    {
        /// <summary>
        /// Derives a byte array key from the specified string key, ensuring the length matches the specified key size.
        /// </summary>
        /// <param name="key">The string key to derive the byte array from.</param>
        /// <param name="keySize">The desired size of the key in bits.</param>
        /// <returns>A byte array representing the derived key.</returns>
        internal static byte[] GetKeyBytes(string key, int keySize = AesEncryption.DEFAULT_KEY_SIZE)
        {
            int keyBytesSize = keySize / 8;
            byte[] keyBytes = new byte[keyBytesSize];

            byte[] utf8KeyBytes = Encoding.UTF8.GetBytes(key);
            if (utf8KeyBytes.Length < keyBytesSize)
            {
                // If the key is shorter, pad it with zeroes
                Buffer.BlockCopy(utf8KeyBytes, 0, keyBytes, 0, utf8KeyBytes.Length);
            }
            else
            {
                // If the key is longer or equal, truncate it
                Buffer.BlockCopy(utf8KeyBytes, 0, keyBytes, 0, keyBytesSize);
            }

            return keyBytes;
        }

        /// <summary>
        /// Calculates the HMAC for the given data using the specified key.
        /// </summary>
        /// <param name="data">The data to calculate the HMAC for.</param>
        /// <param name="key">The key to use for HMAC calculation.</param>
        /// <returns>A truncated HMAC byte array.</returns>
        internal static byte[] CalculateHMAC(byte[] data, byte[] key)
        {
            using HMACSHA256 hmac = new(key);
            byte[] fullHmac = hmac.ComputeHash(data);
            // Truncate the HMAC to the expected size (HMACSize)
            byte[] truncatedHmac = new byte[AesEncryption.HMACSize];
            Buffer.BlockCopy(fullHmac, 0, truncatedHmac, 0, AesEncryption.HMACSize);
            return truncatedHmac;
        }

        /// <summary>
        /// Compares two byte arrays for equality.
        /// </summary>
        /// <param name="a">The first byte array.</param>
        /// <param name="b">The second byte array.</param>
        /// <returns><c>true</c> if the arrays are equal; otherwise, <c>false</c>.</returns>
        internal static bool CompareBytes(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
