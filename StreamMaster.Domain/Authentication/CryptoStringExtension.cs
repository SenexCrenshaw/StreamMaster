//namespace StreamMaster.Domain.Authentication;
//public static class CryptoStringExtension
//{

//    public static string? GetAPIKeyFromPath(this string requestPath, string serverKey, int keySize = 128)
//    {
//        return requestPath.GetIVFromPath(serverKey) == null ? null : serverKey;
//    }

//    public static byte[]? GetIVFromPath(this string requestPath, string serverKey, int keySize = 128)
//    {
//        try
//        {
//            if (!requestPath.StartsWith("/api/videostreams/", StringComparison.InvariantCultureIgnoreCase) &&
//                !requestPath.StartsWith("/api/streamgroups/", StringComparison.InvariantCultureIgnoreCase))
//            {
//                return null;
//            }

//            string crypt = requestPath
//                .Replace("/api/videostreams/stream/", "", StringComparison.InvariantCultureIgnoreCase)
//                .Replace("/api/streamgroups/stream/", "", StringComparison.InvariantCultureIgnoreCase)
//                .Replace("/api/videostreams/", "", StringComparison.InvariantCultureIgnoreCase)
//                .Replace("/api/streamgroups/", "", StringComparison.InvariantCultureIgnoreCase);

//            if (crypt.Contains("/"))
//            {
//                crypt = crypt[..crypt.IndexOf("/")];
//            }

//            byte[] serverKeyBytes = CryptoExtension.GenerateKey(serverKey, keySize);
//            string base64String = crypt
//                .Replace('-', '+')
//                .Replace('_', '/')
//                .PadRight(crypt.Length + ((4 - (crypt.Length % 4)) % 4), '=');
//            byte[] encodedBytes = Convert.FromBase64String(base64String);
//            byte[] hmacBytes = new byte[CryptoExtension.HMACSize];
//            byte[] encryptedBytes = new byte[encodedBytes.Length - CryptoExtension.HMACSize];
//            Buffer.BlockCopy(encodedBytes, 0, hmacBytes, 0, CryptoExtension.HMACSize);
//            Buffer.BlockCopy(encodedBytes, CryptoExtension.HMACSize, encryptedBytes, 0, encryptedBytes.Length);

//            bool isVerified = CryptoExtension.VerifyHMAC(encryptedBytes, hmacBytes, serverKeyBytes);
//            if (!isVerified)
//            {
//                return null;
//            }

//            byte[] iv = new byte[16];



//            Buffer.BlockCopy(encryptedBytes, 0, iv, 0, iv.Length);
//            return iv;
//        }
//        catch
//        {
//            return null;
//        }
//    }

//}
