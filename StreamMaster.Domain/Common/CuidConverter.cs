using System.Security.Cryptography;
using System.Text;

namespace StreamMaster.Domain.Common;

public static class IdConverter
{
    public static string ConvertStringToId(this string str)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(str);
        byte[] hashBytes = MD5.HashData(inputBytes);
        StringBuilder sb = new();

        for (int i = 0; i < hashBytes.Length; i++)
        {
            sb.Append(hashBytes[i].ToString("x2"));
        }

        return sb.ToString();
    }

    public static string GenerateRandomString()
    {
        return Guid.NewGuid().ToString().Replace("-", "");
    }

    public static string GetID()
    {
        return GenerateRandomString().ConvertStringToId();
    }
}
