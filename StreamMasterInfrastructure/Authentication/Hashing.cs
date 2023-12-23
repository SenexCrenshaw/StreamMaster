using System.Security.Cryptography;
using System.Text;

namespace StreamMasterInfrastructure.Authentication;

public static class Hashing
{
    public static string SHA256Hash(this string input)
    {
        using SHA256 hash = SHA256.Create();
        Encoding enc = Encoding.UTF8;
        return GetHash(hash.ComputeHash(enc.GetBytes(input)));
    }

    public static string SHA256Hash(this Stream input)
    {
        using SHA256 hash = SHA256.Create();
        return GetHash(hash.ComputeHash(input));
    }

    private static string GetHash(byte[] bytes)
    {
        StringBuilder stringBuilder = new();

        foreach (byte b in bytes)
        {
            stringBuilder.Append(b.ToString("x2"));
        }

        return stringBuilder.ToString();
    }
}
