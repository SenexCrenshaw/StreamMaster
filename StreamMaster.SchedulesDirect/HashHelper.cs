using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

public static class HashHelper
{
    public static string GetSHA1Hash(this string input)
    {
        using (SHA1 sha1 = SHA1.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = sha1.ComputeHash(bytes);

            StringBuilder sb = new StringBuilder(hash.Length * 2);

            foreach (byte b in hash)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }

    public static bool TestSha1HexHash(string text)
    {
        // Act
        return Regex.IsMatch(text, "^[0-9a-fA-F]{40}$");
    }
}
