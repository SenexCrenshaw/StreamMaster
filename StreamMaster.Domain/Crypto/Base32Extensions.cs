using System.Text;

namespace StreamMaster.Domain.Crypto
{
    internal static class Base32Extensions
    {
        /// <summary>
        /// Converts the specified URL-safe Base32 string back to the original byte array.
        /// </summary>
        /// <param name="base32String">The URL-safe Base32 string to convert.</param>
        /// <returns>A byte array representing the original data.</returns>
        internal static byte[] ConvertFromBase32String(this string base32String)
        {
            return Base32Decode(base32String);
        }

        /// <summary>
        /// Converts the specified byte array to a URL-safe Base32 string and appends an HMAC for integrity.
        /// </summary>
        /// <param name="data">The data to convert.</param>
        /// <param name="key">The key to use for HMAC calculation.</param>
        /// <returns>A URL-safe Base32 string representing the data with appended HMAC.</returns>
        public static string ConvertToBase32String(this byte[] data, byte[] key)
        {
            byte[] hmacBytes = CryptoHelpers.CalculateHMAC(data, key);
            byte[] combinedData = new byte[data.Length + AesEncryption.HMACSize];
            Buffer.BlockCopy(hmacBytes, 0, combinedData, 0, AesEncryption.HMACSize);
            Buffer.BlockCopy(data, 0, combinedData, AesEncryption.HMACSize, data.Length);

            return Base32Encode(combinedData);
        }

        // Base32 Encoding and Decoding
        public static string Base32Encode(this byte[] data)
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
                            currentByte = currentByte << 8 | data[i++];
                            bitsRemaining += 8;
                        }
                        else
                        {
                            int padBits = 5 - bitsRemaining;
                            currentByte <<= padBits;
                            bitsRemaining = 5;
                        }
                    }

                    int index = currentByte >> bitsRemaining - 5 & 31;
                    bitsRemaining -= 5;
                    result.Append(Base32Chars[index]);
                }
            }

            return result.ToString();
        }

        public static byte[] Base32Decode(this string base32String)
        {
            const string Base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            int resultLength = base32String.Length * 5 / 8;
            byte[] result = new byte[resultLength];

            int buffer = 0;
            int bitsRemaining = 0;
            int index = 0;

            foreach (char c in base32String)
            {
                int value = Base32Chars.IndexOf(c);
                if (value < 0)
                {
                    throw new ArgumentException("Invalid Base32 character");
                }

                buffer = buffer << 5 | value;
                bitsRemaining += 5;

                if (bitsRemaining >= 8)
                {
                    result[index++] = (byte)(buffer >> bitsRemaining - 8);
                    bitsRemaining -= 8;
                }
            }

            return result;
        }
    }
}
