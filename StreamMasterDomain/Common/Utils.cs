using StreamMasterDomain.Filtering;

using System.Text;
using System.Text.Json;

namespace StreamMasterDomain.Common;

public static class Utils
{
    private static readonly Random _random = new();

    public static List<DataTableFilterMetaData> GetFiltersFromJSON(string? jsonFiltersString)
    {
        if (!string.IsNullOrEmpty(jsonFiltersString))
        {
            List<DataTableFilterMetaData>? filters = JsonSerializer.Deserialize<List<DataTableFilterMetaData>>(jsonFiltersString);
            return filters ?? new();
        }
        return new();
    }

    public static T? DeepCopy<T>(this T value)
    {
        string jsonString = JsonSerializer.Serialize(value);

        return JsonSerializer.Deserialize<T>(jsonString);
    }

    public static (string fullName, string name) GetRandomFileName(this string directoryName, string extentsion)
    {
        if (!Directory.Exists(directoryName))
        {
            return ("", "");
        }

        DirectoryInfo dir = new(directoryName);
        return dir.GetRandomFileName(extentsion);
    }

    public static (string fullName, string name) GetRandomFileName(this DirectoryInfo directoryInfo, string extentsion)
    {
        while (true)
        {
            string name = $"{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}{extentsion}";
            string fullName = Path.Combine(directoryInfo.FullName, name);

            if (!File.Exists(fullName))
            {
                return (fullName, name);
            }
        }
    }

    public static string RandomString(int size = 12, bool lowerCase = false)
    {
        StringBuilder builder = new(size);

        // Unicode/ASCII Letters are divided into two blocks (Letters 65–90 /
        // 97–122): The first group containing the uppercase letters and the
        // second group containing the lowercase.

        // char is a single Unicode character
        char offset = lowerCase ? 'a' : 'A';
        const int lettersOffset = 26; // A...Z or a..z: length = 26

        for (int i = 0; i < size; i++)
        {
            char @char = (char)_random.Next(offset, offset + lettersOffset);
            _ = builder.Append(@char);
        }

        return lowerCase ? builder.ToString().ToLower() : builder.ToString();
    }
}
