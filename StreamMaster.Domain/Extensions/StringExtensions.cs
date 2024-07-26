namespace StreamMaster.Domain.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrWhiteSpace(this string text)
    {
        return string.IsNullOrWhiteSpace(text);
    }
    public static bool ContainsIgnoreCase(this string text, string contains)
    {
        return text.IndexOf(contains, StringComparison.InvariantCultureIgnoreCase) > -1;
    }
    //public static string ToSafeFileSystemName(this string input)
    //{
    //    // Combine invalid characters for both filenames and directory names
    //    char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
    //    char[] invalidPathChars = Path.GetInvalidPathChars();

    //    // Combine the two sets of invalid characters
    //    HashSet<char> invalidChars = new(invalidFileNameChars);
    //    invalidChars.UnionWith(invalidPathChars);

    //    // Create a regex pattern for invalid characters
    //    Regex regex = new($"[{Regex.Escape(new string(invalidChars.ToArray()))}]");

    //    // Replace invalid characters with an underscore
    //    string sanitized = regex.Replace(input, "_");

    //    // Optionally, trim leading/trailing spaces
    //    return sanitized.Trim();
    //}
    //public static string ToSafeFileName(this string input)
    //{
    //    // Remove invalid characters
    //    char[] invalidChars = Path.GetInvalidFileNameChars();
    //    Regex regex = new($"[{Regex.Escape(new string(invalidChars))}]");
    //    string sanitized = regex.Replace(input, "_");

    //    // Optionally, you can also trim leading/trailing spaces and other characters
    //    return sanitized.Trim();
    //}

    //public static string ToSafeDirectoryName(this string input)
    //{
    //    // Remove invalid characters
    //    char[] invalidChars = Path.GetInvalidPathChars();
    //    Regex regex = new($"[{Regex.Escape(new string(invalidChars))}]");
    //    string sanitized = regex.Replace(input, "_");

    //    // Optionally, you can also trim leading/trailing spaces and other characters
    //    return sanitized.Trim();
    //}
}
