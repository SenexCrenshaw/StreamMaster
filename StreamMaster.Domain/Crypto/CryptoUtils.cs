namespace StreamMaster.Domain.Crypto
{
    /// <summary>
    /// Utility class for encoding and decoding stream group data with AES encryption.
    /// </summary>
    public static class CryptoUtils
    {
        /// <summary>
        /// Encodes three integer values using group and server keys.
        /// </summary>
        /// <param name="streamGroupId">The stream group ID.</param>
        /// <param name="streamGroupProfileId">The stream group profile ID.</param>
        /// <param name="smChannelId">The SM channel ID.</param>
        /// <param name="serverKey">The server key for encryption.</param>
        /// <param name="groupKey">The group key for encryption.</param>
        /// <param name="keySize">The size of the encryption key in bits. Default is 128 bits.</param>
        /// <returns>The encrypted string.</returns>
        public static string EncodeThreeValues(int streamGroupId, int streamGroupProfileId, int smChannelId, string serverKey, string groupKey, int keySize = AesEncryption.DEFAULT_KEY_SIZE)
        {
            string plainText = $"{streamGroupId},{streamGroupProfileId},{smChannelId}";
            string groupEncryptedText = AesEncryption.Encrypt(plainText, groupKey, keySize);
            string combinedText = $"{streamGroupId},{groupEncryptedText}";
            string finalEncryptedText = AesEncryption.Encrypt(combinedText, serverKey, keySize);
            return finalEncryptedText;
        }

        /// <summary>
        /// Encodes two integer values and one string value using group and server keys.
        /// </summary>
        /// <param name="streamGroupId">The stream group ID.</param>
        /// <param name="streamGroupProfileId">The stream group profile ID.</param>
        /// <param name="smStreamId">The SM stream ID.</param>
        /// <param name="serverKey">The server key for encryption.</param>
        /// <param name="groupKey">The group key for encryption.</param>
        /// <param name="keySize">The size of the encryption key in bits. Default is 128 bits.</param>
        /// <returns>The encrypted string.</returns>
        public static string EncodeThreeValues(int streamGroupId, int streamGroupProfileId, string smStreamId, string serverKey, string groupKey, int keySize = AesEncryption.DEFAULT_KEY_SIZE)
        {
            string plainText = $"{streamGroupId},{streamGroupProfileId},{smStreamId}";
            string groupEncryptedText = AesEncryption.Encrypt(plainText, groupKey, keySize);
            string combinedText = $"{streamGroupId},{groupEncryptedText}";
            string finalEncryptedText = AesEncryption.Encrypt(combinedText, serverKey, keySize);
            return finalEncryptedText;
        }

        /// <summary>
        /// Encodes two integer values using group and server keys.
        /// </summary>
        /// <param name="streamGroupId">The stream group ID.</param>
        /// <param name="streamGroupProfileId">The stream group profile ID.</param>
        /// <param name="serverKey">The server key for encryption.</param>
        /// <param name="groupKey">The group key for encryption.</param>
        /// <param name="keySize">The size of the encryption key in bits. Default is 128 bits.</param>
        /// <returns>The encrypted string.</returns>
        public static string EncodeTwoValues(int streamGroupId, int streamGroupProfileId, string serverKey, string groupKey, int keySize = AesEncryption.DEFAULT_KEY_SIZE)
        {
            string plainText = $"{streamGroupId},{streamGroupProfileId}";
            string groupEncryptedText = AesEncryption.Encrypt(plainText, groupKey, keySize);
            string combinedText = $"{streamGroupId},{groupEncryptedText}";
            string finalEncryptedText = AesEncryption.Encrypt(combinedText, serverKey, keySize);
            return finalEncryptedText;
        }

        /// <summary>
        /// Encodes a single integer value using the server key.
        /// </summary>
        /// <param name="streamGroupId">The stream group ID.</param>
        /// <param name="serverKey">The server key for encryption.</param>
        /// <param name="keySize">The size of the encryption key in bits. Default is 128 bits.</param>
        /// <returns>The encrypted string.</returns>
        public static string EncodeValue(int streamGroupId, string serverKey, int keySize = AesEncryption.DEFAULT_KEY_SIZE)
        {
            string plainText = $"{streamGroupId}";
            string encryptedText = AesEncryption.Encrypt(plainText, serverKey, keySize);
            return encryptedText;
        }

        /// <summary>
        /// Decodes the stream group ID and encrypted values string from the provided encrypted string.
        /// </summary>
        /// <param name="encryptedString">The encrypted string to decode.</param>
        /// <param name="serverKey">The server key for decryption.</param>
        /// <returns>A tuple containing the stream group ID and the values encrypted string, or (null, null) if decoding fails.</returns>
        public static (int? streamGroupId, string? valuesEncryptedString) DecodeStreamGroupId(string encryptedString, string serverKey)
        {
            try
            {
                string decryptedText = AesEncryption.Decrypt(encryptedString, serverKey);
                string[] values = decryptedText.Split(',');
                if (values.Length > 0)
                {
                    int streamGroupId = int.Parse(values[0]);
                    string valuesEncryptedString = decryptedText[(values[0].Length + 1)..];
                    return (streamGroupId, valuesEncryptedString);
                }
            }
            catch
            {
                // Handle exception appropriately
            }
            return (null, null);
        }
    }
}
