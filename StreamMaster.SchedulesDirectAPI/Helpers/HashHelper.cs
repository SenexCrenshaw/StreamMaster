using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace StreamMaster.SchedulesDirectAPI;

public static partial class HashHelper
{
    public static string GetSHA1Hash(this string input)
    {
        using (SHA1 sha1 = SHA1.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = sha1.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("x2"));
            }
            string strSHA1 = sb.ToString();
            return strSHA1;
        }
    }

    public static bool TestSha1HexHash(string text)
    {
        // Act
        return MyRegex().IsMatch(text);
    }

    [GeneratedRegex("^[0-9a-fA-F]{40}$")]
    private static partial Regex MyRegex();
}
