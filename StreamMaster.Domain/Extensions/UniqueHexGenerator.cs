using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace StreamMaster.Domain.Extensions;
public class UniqueHexGenerator
{
    public const string ShortIdEmpty = "000000";

    public static string GenerateUniqueHex(ConcurrentDictionary<string, byte> existingIds)
    {
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        while (true)
        {
            byte[] buffer = new byte[3];
            rng.GetBytes(buffer);
            int randomValue = BitConverter.ToInt32([buffer[0], buffer[1], buffer[2], 0], 0);
            string hex = randomValue.ToString("X6");

            if (hex != ShortIdEmpty && existingIds.TryAdd(hex, 0))
            {
                return hex;
            }
        }
    }
}