namespace BuildClientAPI.TS;
public static class StringExtensions
{
    public static string ToCamelCase(this string str)
    {
        return !string.IsNullOrEmpty(str) && str.Length > 1 ? char.ToLowerInvariant(str[0]) + str[1..] : str;
    }
}
