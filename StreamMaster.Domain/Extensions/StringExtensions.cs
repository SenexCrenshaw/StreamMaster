using StreamMaster.Domain.Configuration;

namespace StreamMaster.Domain.Extensions;

public static class StringExtensions
{
    public static bool EqualsIgnoreCase(this string text, string Compare)
    {
        return text.Equals(Compare, BuildInfo.StringComparison);
    }

    public static bool ContainsIgnoreCase(this string text, string Contains)
    {
        return text.Contains(Contains, BuildInfo.StringComparison);
    }

    public static bool StartsWithIgnoreCase(this string text, string StartsWith)
    {
        return text.StartsWith(StartsWith, BuildInfo.StringComparison);
    }

    public static bool EndsWithIgnoreCase(this string text, string EndsWith)
    {
        return text.EndsWith(EndsWith, BuildInfo.StringComparison);
    }
}
