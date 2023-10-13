namespace StreamMasterDomain.Extensions;

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
}
