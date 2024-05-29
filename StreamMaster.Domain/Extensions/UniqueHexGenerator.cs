using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace StreamMaster.Domain.Extensions;
public class UniqueHexGenerator
{
    public const string SMChannelIdEmpty = "000000";
    private static string GetSMChannelId()
    {
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        byte[] buffer = new byte[3];
        rng.GetBytes(buffer);
        int randomValue = BitConverter.ToInt32([buffer[0], buffer[1], buffer[2], 0], 0);
        string hex = randomValue.ToString("X6");
        return hex;
    }
    public static string GenerateUniqueHex(HashSet<string> existingIds)
    {
        while (true)
        {
            string hex = GetSMChannelId();
            if (hex != SMChannelIdEmpty && existingIds.Add(hex))
            {
                return hex;
            }
        }
    }

    public static string GenerateUniqueHex(ConcurrentDictionary<string, byte> existingIds)
    {
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        while (true)
        {
            string hex = GetSMChannelId();
            if (hex != SMChannelIdEmpty && existingIds.TryAdd(hex, 0))
            {
                return hex;
            }
        }
    }
}