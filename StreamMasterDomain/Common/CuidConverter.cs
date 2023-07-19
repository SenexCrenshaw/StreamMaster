using System.Security.Cryptography;
using System.Text;

namespace StreamMasterDomain.Common;

public static class CuidConverter
{
    public static string ConvertUrlToCuid(this string url)
    {
        //// Create an instance of SHA256 to compute the hash
        //using (var sha256 = SHA256.Create())
        //{
        //    // Compute the hash of the URL
        //    byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(url));

        // // Convert the hash bytes to a hexadecimal string string hashString =
        // BitConverter.ToString(hashBytes).Replace("-", string.Empty);

        // // Take the first 8 characters as the CUID string cuid =
        // hashString.Substring(0, 8);

        //    return cuid;
        //}
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(url);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2")); // Use lowercase hexadecimal format
            }

            return sb.ToString();
        }
    }
}
