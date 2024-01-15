using System.Security.Cryptography;

namespace StreamMaster.Domain.Extensions;
public class UniqueHexGenerator
{
    public static string GenerateUniqueHex(HashSet<string> existingIds)
    {
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        while (true)
        {
            byte[] buffer = new byte[3]; // 3 bytes for 24 bits
            rng.GetBytes(buffer);
            int randomValue = BitConverter.ToInt32([buffer[0], buffer[1], buffer[2], 0], 0);
            string hex = randomValue.ToString("X6");

            if (hex != "000000" && existingIds.Add(hex))
            {
                return hex;
            }
        }
    }
}