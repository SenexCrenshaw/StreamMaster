using System.Security.Cryptography;
using System.Text;

public static class AesEncryption
{
    private const int HMACSize = 16; // 128 bits for shorter HMAC

    /// <summary>
    /// Encrypts the specified plain text using AES and appends an HMAC for integrity.
    /// The result is returned as a URL-safe Base32 string.
    /// </summary>
    /// <param name="plainText">The text to encrypt.</param>
    /// <param name="key">The key for encryption. Must be at least as long as specified by <paramref name="keySize"/>.</param>
    /// <param name="keySize">The size of the encryption key in bits. Default is 128 bits.</param>
    /// <returns>A URL-safe Base32 string containing the encrypted data and HMAC.</returns>
    public static string Encrypt(string plainText, string key, int keySize = 128)
    {
        byte[] keyBytes = GetKeyBytes(key, keySize);

        using Aes aesAlg = Aes.Create();
        aesAlg.KeySize = keySize;
        aesAlg.Key = keyBytes;
        aesAlg.IV = new byte[16]; // Initialize to zeros for simplicity

        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using MemoryStream msEncrypt = new();
        using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
        {
            using StreamWriter swEncrypt = new(csEncrypt);
            swEncrypt.Write(plainText);
        }
        byte[] encryptedBytes = msEncrypt.ToArray();
        return ConvertToBase32String(encryptedBytes, keyBytes);
    }

    /// <summary>
    /// Decrypts the specified URL-safe Base32 string using AES and verifies the HMAC for integrity.
    /// </summary>
    /// <param name="cipherText">The URL-safe Base32 string containing the encrypted data and HMAC.</param>
    /// <param name="key">The key for decryption. Must be at least as long as specified by <paramref name="keySize"/>.</param>
    /// <param name="keySize">The size of the decryption key in bits. Default is 128 bits.</param>
    /// <returns>The decrypted plain text.</returns>
    /// <exception cref="CryptographicException">Thrown if HMAC validation fails.</exception>
    public static string Decrypt(string cipherText, string key, int keySize = 128)
    {
        byte[] keyBytes = GetKeyBytes(key, keySize);
        byte[] cipherBytes = ConvertFromBase32String(cipherText);

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
        byte[] computedHmac = CalculateHMAC(encryptedData, keyBytes);
        if (!CompareBytes(hmac, computedHmac))
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

    /// <summary>
    /// Derives a byte array key from the specified string key, ensuring the length matches the specified key size.
    /// </summary>
    /// <param name="key">The string key to derive the byte array from.</param>
    /// <param name="keySize">The desired size of the key in bits.</param>
    /// <returns>A byte array representing the derived key.</returns>
    private static byte[] GetKeyBytes(string key, int keySize)
    {
        return Encoding.UTF8.GetBytes(key.PadRight(keySize / 8)[..(keySize / 8)]);
    }

    /// <summary>
    /// Calculates the HMAC for the given data using the specified key.
    /// </summary>
    /// <param name="data">The data to calculate the HMAC for.</param>
    /// <param name="key">The key to use for HMAC calculation.</param>
    /// <returns>A truncated HMAC byte array.</returns>
    private static byte[] CalculateHMAC(byte[] data, byte[] key)
    {
        using HMACSHA256 hmac = new(key);
        byte[] fullHmac = hmac.ComputeHash(data);
        // Truncate the HMAC to the expected size (HMACSize)
        byte[] truncatedHmac = new byte[HMACSize];
        Buffer.BlockCopy(fullHmac, 0, truncatedHmac, 0, HMACSize);
        return truncatedHmac;
    }

    /// <summary>
    /// Converts the specified byte array to a URL-safe Base32 string and appends an HMAC for integrity.
    /// </summary>
    /// <param name="data">The data to convert.</param>
    /// <param name="key">The key to use for HMAC calculation.</param>
    /// <returns>A URL-safe Base32 string representing the data with appended HMAC.</returns>
    private static string ConvertToBase32String(byte[] data, byte[] key)
    {
        byte[] hmacBytes = CalculateHMAC(data, key);
        byte[] combinedData = new byte[data.Length + HMACSize];
        Buffer.BlockCopy(hmacBytes, 0, combinedData, 0, HMACSize);
        Buffer.BlockCopy(data, 0, combinedData, HMACSize, data.Length);

        return Base32Encode(combinedData);
    }

    /// <summary>
    /// Converts the specified URL-safe Base32 string back to the original byte array.
    /// </summary>
    /// <param name="base32String">The URL-safe Base32 string to convert.</param>
    /// <returns>A byte array representing the original data.</returns>
    private static byte[] ConvertFromBase32String(string base32String)
    {
        return Base32Decode(base32String);
    }

    /// <summary>
    /// Compares two byte arrays for equality.
    /// </summary>
    /// <param name="a">The first byte array.</param>
    /// <param name="b">The second byte array.</param>
    /// <returns><c>true</c> if the arrays are equal; otherwise, <c>false</c>.</returns>
    private static bool CompareBytes(byte[] a, byte[] b)
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

    // Base32 Encoding and Decoding
    private static string Base32Encode(byte[] data)
    {
        const string Base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        StringBuilder result = new((data.Length + 7) * 8 / 5);

        for (int i = 0; i < data.Length;)
        {
            int currentByte = data[i++];
            int bitsRemaining = 8;

            while (bitsRemaining > 0 || i < data.Length)
            {
                if (bitsRemaining < 5)
                {
                    if (i < data.Length)
                    {
                        currentByte = (currentByte << 8) | data[i++];
                        bitsRemaining += 8;
                    }
                    else
                    {
                        int padBits = 5 - bitsRemaining;
                        currentByte <<= padBits;
                        bitsRemaining = 5;
                    }
                }

                int index = (currentByte >> (bitsRemaining - 5)) & 31;
                bitsRemaining -= 5;
                result.Append(Base32Chars[index]);
            }
        }

        return result.ToString();
    }

    private static byte[] Base32Decode(string base32String)
    {
        const string Base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        int resultLength = base32String.Length * 5 / 8;
        byte[] result = new byte[resultLength];

        int buffer = 0;
        int bitsRemaining = 0;
        int mask = 31;
        int index = 0;

        foreach (char c in base32String)
        {
            int value = Base32Chars.IndexOf(c);
            if (value < 0)
            {
                throw new ArgumentException("Invalid Base32 character");
            }

            buffer = (buffer << 5) | value;
            bitsRemaining += 5;

            if (bitsRemaining >= 8)
            {
                result[index++] = (byte)(buffer >> (bitsRemaining - 8));
                bitsRemaining -= 8;
            }
        }

        return result;
    }
}
