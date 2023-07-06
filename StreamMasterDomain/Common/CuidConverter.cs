using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StreamMasterDomain.Common;

public static class CuidConverter
{
    public static string ConvertUrlToCuid(this string url)
    {
        // Create an instance of SHA256 to compute the hash
        using (var sha256 = SHA256.Create())
        {
            // Compute the hash of the URL
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(url));

            // Convert the hash bytes to a hexadecimal string
            string hashString = BitConverter.ToString(hashBytes).Replace("-", string.Empty);

            // Take the first 8 characters as the CUID
            string cuid = hashString.Substring(0, 8);

            return cuid;
        }
    }
}